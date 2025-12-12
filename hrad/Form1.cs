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

        // TextBox pro počet vstupenek
        TextBox tbTickets;

        public Form1()
        {
            InitializeComponent();

            AddImages();

            dtTime.MinDate = new DateTime(2000, 1, 1);
            dtTime.MaxDate = new DateTime(2100, 12, 31);
            dtTime.Value = DateTime.Now;

            AddTicketInput();

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

            string[] imgFiles = {
                @"H:\Programování 3\hrad\hrad\download.jpg",
                @"H:\Programování 3\hrad\hrad\download (1).jpg",
                @"H:\Programování 3\hrad\hrad\download (2).jpg"
            };
            string[] labels = { "Okruh A", "Okruh B", "Okruh C" };

            for (int i = 0; i < 3; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Image = Image.FromFile(imgFiles[i]);
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.Location = new Point(startX + i * (imgWidth + spacing), startY);
                pic.Size = new Size(imgWidth, imgHeight);
                pic.Cursor = Cursors.Hand;
                Controls.Add(pic);

                Label lbl = new Label();
                lbl.Text = labels[i];
                lbl.AutoSize = true;
                lbl.Location = new Point(startX + i * (imgWidth + spacing) + (imgWidth - lbl.PreferredWidth) / 2, startY + imgHeight + 5);
                Controls.Add(lbl);

                int index = i;
                pic.Click += (s, e) => { cbCircuit.SelectedItem = labels[index]; };
            }
        }

        void AddTicketInput()
        {
            int startX = dtTime.Left;
            int startY = dtTime.Bottom + 10;

            tbTickets = new TextBox() { Location = new Point(startX, startY), Width = 40, Text = "1" };
            Controls.Add(tbTickets);
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
            if (!int.TryParse(tbTickets.Text, out int totalCount) || totalCount <= 0)
            {
                MessageBox.Show("Zadej počet vstupenek.");
                return;
            }

            string circuit = cbCircuit.SelectedItem?.ToString() ?? "";
            string time = dtTime.Value.ToString("yyyy-MM-dd HH:00");

            if (string.IsNullOrEmpty(circuit))
            {
                MessageBox.Show("Vyber okruh.");
                return;
            }

            using var tx = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;

            cmd.CommandText = "SELECT id, capacity, sold FROM Shows WHERE circuit=@c AND showtime=@t LIMIT 1;";
            cmd.Parameters.AddWithValue("@c", circuit);
            cmd.Parameters.AddWithValue("@t", time);

            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read())
            {
                MessageBox.Show("Sezení nenalezeno.");
                tx.Rollback();
                return;
            }

            int showId = rdr.GetInt32(0);
            int capacity = rdr.GetInt32(1);
            int sold = rdr.GetInt32(2);
            rdr.Close();

            int free = capacity - sold;
            if (free < totalCount)
            {
                MessageBox.Show($"Volných míst je pouze {free}. Nelze koupit {totalCount} vstupenek.");
                tx.Rollback();
                return;
            }

            var qrList = new System.Collections.Generic.List<string>();
            for (int i = 0; i < totalCount; i++)
            {
                string qr = Guid.NewGuid().ToString();
                qrList.Add(qr);

                cmd.CommandText = "INSERT INTO Tickets(showid, qr, created_at) VALUES(@sid, @qr, @dt);";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@sid", showId);
                cmd.Parameters.AddWithValue("@qr", qr);
                cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("s"));
                cmd.ExecuteNonQuery();
            }

            cmd.CommandText = "UPDATE Shows SET sold = sold + @cnt WHERE id=@sid2;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@cnt", totalCount);
            cmd.Parameters.AddWithValue("@sid2", showId);
            cmd.ExecuteNonQuery();

            tx.Commit();

            using var qrGen = new QRCodeGenerator();
            using var data = qrGen.CreateQrCode(qrList[0], QRCodeGenerator.ECCLevel.Q);
            using var qrImg = new QRCode(data);
            pbQr.Image = qrImg.GetGraphic(5);

            MessageBox.Show($"Koupeno celkem {totalCount} vstupenek.");
            RefreshAvailability();
        }
    }
}
