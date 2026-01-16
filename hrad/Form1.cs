using System;
using System.Drawing;
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

            // Inicializace ListView
            lvAvailability.Columns.Clear();
            lvAvailability.Columns.Add("Okruh", 120);
            lvAvailability.Columns.Add("Čas", 160);
            lvAvailability.Columns.Add("Typ", 120);
            lvAvailability.Columns.Add("Cena", 80);
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

            // Kontrola, zda byla vybrána alespoň jedna vstupenka
            if (adultCount + childCount + studentCount == 0)
            {
                MessageBox.Show("Musíte vybrat alespoň jednu vstupenku!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string okruh = cbCircuit.SelectedItem.ToString();
            string cas = dtTime.Value.ToString("yyyy-MM-dd HH:00");

            // Přidání vstupenek do ListView
            for (int i = 0; i < adultCount; i++)
                AddTicket(okruh, cas, "Dospělá", 200);
            for (int i = 0; i < childCount; i++)
                AddTicket(okruh, cas, "Dětská", 100);
            for (int i = 0; i < studentCount; i++)
                AddTicket(okruh, cas, "Studentská", 150);

            // Vygenerování QR kódu pro poslední přidanou vstupenku
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

        // QR kód vypadající jako skutečný a vyplňuje celý PictureBox
        private void DrawQr(string okruh, string cas, string typ, int cena)
        {
            int size = pbQr.Width;  // předpokládáme čtvercový PictureBox
            int modules = 21;       // velikost QR matice 21x21
            int cellSize = size / modules;

            int qrSize = cellSize * modules;
            int offset = (size - qrSize) / 2; // centrování

            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                Random rnd = new Random((okruh + cas + typ + cena).GetHashCode());

                // Funkce pro vykreslení finder pattern
                void DrawFinder(int startX, int startY)
                {
                    int patternSize = 7;
                    for (int x = 0; x < patternSize; x++)
                    {
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
                }

                // Tři finder patterns
                DrawFinder(0, 0);                // levý horní
                DrawFinder(modules - 7, 0);      // pravý horní
                DrawFinder(0, modules - 7);      // levý dolní

                // Náhodné černobílé moduly pro zbytek QR
                for (int x = 0; x < modules; x++)
                {
                    for (int y = 0; y < modules; y++)
                    {
                        if ((x < 7 && y < 7) || (x >= modules - 7 && y < 7) || (x < 7 && y >= modules - 7))
                            continue;

                        bool black = rnd.Next(0, 2) == 0;
                        g.FillRectangle(black ? Brushes.Black : Brushes.White,
                            offset + x * cellSize,
                            offset + y * cellSize,
                            cellSize, cellSize);
                    }
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
