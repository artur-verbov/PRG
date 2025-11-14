using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QRCoder;

namespace TicketApp
{
    public partial class Form1 : Form
    {
        SQLiteConnection? conn;

        public Form1()
        {
            InitializeComponent();

            
            dtTime.MinDate = new DateTime(2000, 1, 1);
            dtTime.MaxDate = new DateTime(2100, 12, 31);

            
            dtTime.Value = new DateTime(2025, 10, 3);

            AddImages();
            InitDb();
            RefreshAvailability();
        }

        
        void AddImages()
        {
            int imgWidth = 140;
            int imgHeight = 120;
            int spacing = 10;

            int startX = pbQr.Right + 20;
            int startY = pbQr.Top;

            
            var pic1 = new PictureBox();
            pic1.Image = Image.FromFile(@"H:\Programování 3\hrad\hrad\download.jpg");
            pic1.SizeMode = PictureBoxSizeMode.StretchImage;
            pic1.Location = new Point(startX, startY);
            pic1.Size = new Size(imgWidth, imgHeight);
            pic1.Cursor = Cursors.Hand;
            Controls.Add(pic1);

            var lbl1 = new Label();
            lbl1.Text = "Okruh A";
            lbl1.AutoSize = true;
            lbl1.Location = new Point(startX + (imgWidth - lbl1.PreferredWidth) / 2, startY + imgHeight + 5);
            Controls.Add(lbl1);

            pic1.Click += (s, e) => { cbCircuit.SelectedItem = "Okruh A"; };

            
            var pic2 = new PictureBox();
            pic2.Image = Image.FromFile(@"H:\Programování 3\hrad\hrad\download (1).jpg");
            pic2.SizeMode = PictureBoxSizeMode.StretchImage;
            pic2.Location = new Point(startX + imgWidth + spacing, startY);
            pic2.Size = new Size(imgWidth, imgHeight);
            pic2.Cursor = Cursors.Hand;
            Controls.Add(pic2);

            var lbl2 = new Label();
            lbl2.Text = "Okruh B";
            lbl2.AutoSize = true;
            lbl2.Location = new Point(startX + imgWidth + spacing + (imgWidth - lbl2.PreferredWidth) / 2, startY + imgHeight + 5);
            Controls.Add(lbl2);

            pic2.Click += (s, e) => { cbCircuit.SelectedItem = "Okruh B"; };

            
            var pic3 = new PictureBox();
            pic3.Image = Image.FromFile(@"H:\Programování 3\hrad\hrad\download (2).jpg");
            pic3.SizeMode = PictureBoxSizeMode.StretchImage;
            pic3.Location = new Point(startX + 2 * (imgWidth + spacing), startY);
            pic3.Size = new Size(imgWidth, imgHeight);
            pic3.Cursor = Cursors.Hand;
            Controls.Add(pic3);

            var lbl3 = new Label();
            lbl3.Text = "Okruh C";
            lbl3.AutoSize = true;
            lbl3.Location = new Point(startX + 2 * (imgWidth + spacing) + (imgWidth - lbl3.PreferredWidth) / 2, startY + imgHeight + 5);
            Controls.Add(lbl3);

            pic3.Click += (s, e) => { cbCircuit.SelectedItem = "Okruh C"; };
        }
        

        void InitDb()
        {
            var dbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tickets.db");
            if (!File.Exists(dbFile))
                SQLiteConnection.CreateFile(dbFile);

            conn = new SQLiteConnection($"Data Source={dbFile};Version=3;");
            conn.Open();
        }

        void RefreshAvailability()
        {
            lvAvailability.Items.Clear();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT circuit, showtime, capacity - sold AS free FROM Shows ORDER BY circuit, showtime;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var item = new ListViewItem(new string[]
                {
                    rdr.GetString(0),
                    rdr.GetString(1),
                    rdr.GetInt32(2).ToString()
                });
                lvAvailability.Items.Add(item);
            }
        }

        private void btnBuy_Click(object? sender, EventArgs e)
        {
            var circuit = cbCircuit.SelectedItem.ToString();
            var time = dtTime.Value.ToString("yyyy-MM-dd HH:00");
            var type = rbChild.Checked ? "Dětská" : rbStudent.Checked ? "Studentská" : "Dospělá";

            using var tx = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "SELECT id, capacity, sold FROM Shows WHERE circuit=@c AND showtime=@t LIMIT 1;";
            cmd.Parameters.AddWithValue("@c", circuit);
            cmd.Parameters.AddWithValue("@t", time);

            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read())
            {
                MessageBox.Show("Sezení nenalezeno");
                tx.Rollback();
                return;
            }

            int showId = rdr.GetInt32(0);
            int capacity = rdr.GetInt32(1);
            int sold = rdr.GetInt32(2);
            if (sold >= capacity)
            {
                MessageBox.Show("Na vybraný čas nejsou volná místa");
                tx.Rollback();
                return;
            }
            rdr.Close();

            var ticketGuid = Guid.NewGuid().ToString();
            cmd.CommandText = "INSERT INTO Tickets(showid, type, qr, created_at) VALUES(@sid, @type, @qr, @dt);";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@sid", showId);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@qr", ticketGuid);
            cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("s"));
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE Shows SET sold = sold + 1 WHERE id=@sid2;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@sid2", showId);
            cmd.ExecuteNonQuery();
            tx.Commit();

            using var qrGen = new QRCodeGenerator();
            using var data = qrGen.CreateQrCode(ticketGuid, QRCodeGenerator.ECCLevel.Q);
            using var qr = new QRCode(data);
            pbQr.Image = qr.GetGraphic(5);

            MessageBox.Show($"Vstupenka koupena: {circuit} {time} {type}");
            RefreshAvailability();
        }
    }
}
