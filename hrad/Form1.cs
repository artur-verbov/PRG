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
            InitDb();
            RefreshAvailability();
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
            pbQr.Image = qr.GetGraphic(5); // QR

            MessageBox.Show($"Vstupenka koupena: {circuit} {time} {type}");
            RefreshAvailability();
        }
    }
}
