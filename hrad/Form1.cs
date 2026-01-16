using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TicketApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (cbCircuit.Items.Count > 0)
                cbCircuit.SelectedIndex = 0;

            dtTime.Value = dtTime.MinDate;

            lvAvailability.Columns.Clear();
            lvAvailability.Columns.Add("Okruh", 120);
            lvAvailability.Columns.Add("Čas", 160);
            lvAvailability.Columns.Add("Typ", 120);
            lvAvailability.Columns.Add("Cena", 80);

            AddImages();
            UpdateAvailabilityStatus();
        }

        void AddImages()
        {
            int w = 140, h = 120, s = 10;
            int x = pbQr.Right + 20, y = pbQr.Top;
            string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            void AddPic(string file, string okruh, int offsetX)
            {
                PictureBox pb = new PictureBox();
                pb.Image = Image.FromFile(Path.Combine(imgDir, file));
                pb.Size = new Size(w, h);
                pb.Location = new Point(x + offsetX, y);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Cursor = Cursors.Hand;
                pb.Click += (a, b) => cbCircuit.SelectedItem = okruh;
                Controls.Add(pb);

                Label lbl = new Label();
                lbl.Text = okruh;
                lbl.AutoSize = true;
                lbl.Location = new Point(x + offsetX + 40, y + h + 5);
                Controls.Add(lbl);
            }

            AddPic("download.jpg", "Okruh A", 0);
            AddPic("download (1).jpg", "Okruh B", w + s);
            AddPic("download (2).jpg", "Okruh C", 2 * (w + s));
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (cbCircuit.SelectedIndex < 0)
            {
                MessageBox.Show("Vyber okruh!");
                return;
            }

            int adult = (int)nudAdult.Value;
            int child = (int)nudChild.Value;
            int student = (int)nudStudent.Value;

            if (adult + child + student == 0)
            {
                MessageBox.Show("Musíte vybrat alespoň jednu vstupenku!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string okruh = cbCircuit.SelectedItem.ToString();
            string cas = dtTime.Value.ToString("yyyy-MM-dd HH:00");

            for (int i = 0; i < adult; i++)
                AddTicket(okruh, cas, "Dospělá", 200);
            for (int i = 0; i < child; i++)
                AddTicket(okruh, cas, "Dětská", 100);
            for (int i = 0; i < student; i++)
                AddTicket(okruh, cas, "Studentská", 150);

            string lastType = student > 0 ? "Studentská" : child > 0 ? "Dětská" : "Dospělá";
            int lastPrice = lastType == "Dospělá" ? 200 : lastType == "Dětská" ? 100 : 150;

            DrawQr(okruh, cas, lastType, lastPrice);
            UpdateAvailabilityStatus();
        }

        private void AddTicket(string okruh, string cas, string typ, int cena)
        {
            ListViewItem item = new ListViewItem(okruh);
            item.SubItems.Add(cas);
            item.SubItems.Add(typ);
            item.SubItems.Add(cena + " Kč");
            lvAvailability.Items.Add(item);
        }

        private void DrawQr(string okruh, string cas, string typ, int cena)
        {
            int pbSize = pbQr.Width;
            int modules = 21;
            int maxQrSize = pbSize - 20;
            int cellSize = maxQrSize / modules;
            int qrSize = cellSize * modules;
            int offset = (pbSize - qrSize) / 2;

            Bitmap bmp = new Bitmap(pbSize, pbSize);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                Random rnd = new Random((okruh + cas + typ + cena).GetHashCode());

                void DrawFinder(int sx, int sy)
                {
                    int sz = 7;
                    for (int x = 0; x < sz; x++)
                        for (int y = 0; y < sz; y++)
                        {
                            bool black = (x == 0 || x == sz - 1 || y == 0 || y == sz - 1 || (x >= 2 && x <= 4 && y >= 2 && y <= 4));
                            g.FillRectangle(black ? Brushes.Black : Brushes.White,
                                offset + (sx + x) * cellSize,
                                offset + (sy + y) * cellSize,
                                cellSize, cellSize);
                        }
                }

                DrawFinder(0, 0);
                DrawFinder(modules - 7, 0);
                DrawFinder(0, modules - 7);

                for (int x = 0; x < modules; x++)
                    for (int y = 0; y < modules; y++)
                    {
                        if ((x < 7 && y < 7) || (x >= modules - 7 && y < 7) || (x < 7 && y >= modules - 7)) continue;
                        bool black = rnd.Next(2) == 0;
                        g.FillRectangle(black ? Brushes.Black : Brushes.White,
                            offset + x * cellSize, offset + y * cellSize, cellSize, cellSize);
                    }
            }
            pbQr.Image = bmp;
        }

        private void UpdateAvailabilityStatus()
        {
            bool anyFree = lvAvailability.Items.Count > 0;
            lblAvailability.Text = anyFree ? "Volná místa: ano" : "Volná místa: ne";
        }

        private void pbQr_Click(object sender, EventArgs e)
        {
            MessageBox.Show("QR kód vstupenky");
        }
    }
}
