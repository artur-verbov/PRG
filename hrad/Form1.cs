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
        }

        void AddImages()
        {
            int w = 140;
            int h = 120;
            int s = 10;

            int x = pbQr.Right + 20;
            int y = pbQr.Top;

            string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            PictureBox p1 = new PictureBox();
            p1.Image = Image.FromFile(Path.Combine(imgDir, "download.jpg"));
            p1.Size = new Size(w, h);
            p1.Location = new Point(x, y);
            p1.SizeMode = PictureBoxSizeMode.StretchImage;
            p1.Cursor = Cursors.Hand;
            Controls.Add(p1);
            p1.Click += (a, b) => cbCircuit.SelectedItem = "Okruh A";

            Label l1 = new Label();
            l1.Text = "Okruh A";
            l1.AutoSize = true;
            l1.Location = new Point(x + 40, y + h + 5);
            Controls.Add(l1);

            PictureBox p2 = new PictureBox();
            p2.Image = Image.FromFile(Path.Combine(imgDir, "download (1).jpg"));
            p2.Size = new Size(w, h);
            p2.Location = new Point(x + w + s, y);
            p2.SizeMode = PictureBoxSizeMode.StretchImage;
            p2.Cursor = Cursors.Hand;
            Controls.Add(p2);
            p2.Click += (a, b) => cbCircuit.SelectedItem = "Okruh B";

            Label l2 = new Label();
            l2.Text = "Okruh B";
            l2.AutoSize = true;
            l2.Location = new Point(x + w + s + 40, y + h + 5);
            Controls.Add(l2);

            PictureBox p3 = new PictureBox();
            p3.Image = Image.FromFile(Path.Combine(imgDir, "download (2).jpg"));
            p3.Size = new Size(w, h);
            p3.Location = new Point(x + 2 * (w + s), y);
            p3.SizeMode = PictureBoxSizeMode.StretchImage;
            p3.Cursor = Cursors.Hand;
            Controls.Add(p3);
            p3.Click += (a, b) => cbCircuit.SelectedItem = "Okruh C";

            Label l3 = new Label();
            l3.Text = "Okruh C";
            l3.AutoSize = true;
            l3.Location = new Point(x + 2 * (w + s) + 40, y + h + 5);
            Controls.Add(l3);
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (cbCircuit.SelectedIndex < 0)
            {
                MessageBox.Show("Vyber okruh!");
                return;
            }

            int adultCount = (int)nudAdult.Value;
            int childCount = (int)nudChild.Value;
            int studentCount = (int)nudStudent.Value;

            if (adultCount + childCount + studentCount == 0)
            {
                MessageBox.Show("Musíte vybrat alespoň jednu vstupenku!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string okruh = cbCircuit.SelectedItem.ToString();
            string cas = dtTime.Value.ToString("yyyy-MM-dd HH:00");

            for (int i = 0; i < adultCount; i++)
                AddTicket(okruh, cas, "Dospělá", 200);
            for (int i = 0; i < childCount; i++)
                AddTicket(okruh, cas, "Dětská", 100);
            for (int i = 0; i < studentCount; i++)
                AddTicket(okruh, cas, "Studentská", 150);

            string lastType = studentCount > 0 ? "Studentská" :
                              childCount > 0 ? "Dětská" : "Dospělá";
            int lastPrice = lastType == "Dospělá" ? 200 :
                            lastType == "Dětská" ? 100 : 150;

            DrawQr(okruh, cas, lastType, lastPrice);
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
            int maxQrSize = pbSize - 20; // немного отступа
            int cellSize = maxQrSize / modules;
            int qrSize = cellSize * modules;
            int offset = (pbSize - qrSize) / 2;

            Bitmap bmp = new Bitmap(pbSize, pbSize);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                Random rnd = new Random((okruh + cas + typ + cena).GetHashCode());

                void DrawFinder(int startX, int startY)
                {
                    int patternSize = 7;
                    for (int x = 0; x < patternSize; x++)
                        for (int y = 0; y < patternSize; y++)
                        {
                            bool black = (x == 0 || x == patternSize - 1 || y == 0 || y == patternSize - 1 ||
                                          (x >= 2 && x <= 4 && y >= 2 && y <= 4));
                            g.FillRectangle(black ? Brushes.Black : Brushes.White,
                                offset + (startX + x) * cellSize,
                                offset + (startY + y) * cellSize,
                                cellSize, cellSize);
                        }
                }

                DrawFinder(0, 0);
                DrawFinder(modules - 7, 0);
                DrawFinder(0, modules - 7);

                for (int x = 0; x < modules; x++)
                    for (int y = 0; y < modules; y++)
                    {
                        if ((x < 7 && y < 7) || (x >= modules - 7 && y < 7) || (x < 7 && y >= modules - 7))
                            continue;

                        bool black = rnd.Next(2) == 0;
                        g.FillRectangle(black ? Brushes.Black : Brushes.White,
                            offset + x * cellSize,
                            offset + y * cellSize,
                            cellSize, cellSize);
                    }
            }

            pbQr.Image = bmp;
        }

        private void pbQr_Click(object sender, EventArgs e)
        {
            MessageBox.Show("QR kód vstupenky");
        }
    }
}
