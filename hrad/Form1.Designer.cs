namespace TicketApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox cbCircuit;
        private DateTimePicker dtTime;
        private GroupBox gbType;
        private RadioButton rbChild;
        private RadioButton rbStudent;
        private RadioButton rbAdult;
        private Button btnBuy;
        private PictureBox pbQr;
        private ListView lvAvailability;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cbCircuit = new ComboBox();
            this.dtTime = new DateTimePicker();
            this.gbType = new GroupBox();
            this.rbChild = new RadioButton();
            this.rbStudent = new RadioButton();
            this.rbAdult = new RadioButton();
            this.btnBuy = new Button();
            this.pbQr = new PictureBox();
            this.lvAvailability = new ListView();

            this.SuspendLayout();

            this.cbCircuit.Items.AddRange(new object[] { "Okruh A", "Okruh B", "Okruh C" });
            this.cbCircuit.SelectedIndex = 0;
            this.cbCircuit.Location = new System.Drawing.Point(20, 20);

            this.dtTime.Format = DateTimePickerFormat.Custom;
            this.dtTime.CustomFormat = "yyyy-MM-dd HH:00";
            this.dtTime.ShowUpDown = true;
            this.dtTime.Location = new System.Drawing.Point(20, 60);
            this.dtTime.Value = DateTime.Today.AddHours(10);
            this.dtTime.MinDate = DateTime.Today.AddHours(10);
            this.dtTime.MaxDate = DateTime.Today.AddHours(17);

            this.gbType.Text = "Typ vstupenky";
            this.gbType.Location = new System.Drawing.Point(20, 100);
            this.gbType.Size = new System.Drawing.Size(200, 120);
            this.rbChild.Text = "Dětská"; this.rbChild.Location = new System.Drawing.Point(10, 20);
            this.rbStudent.Text = "Studentská"; this.rbStudent.Location = new System.Drawing.Point(10, 50);
            this.rbAdult.Text = "Dospělá"; this.rbAdult.Location = new System.Drawing.Point(10, 80);
            this.rbAdult.Checked = true;
            this.gbType.Controls.AddRange(new Control[] { rbChild, rbStudent, rbAdult });

            this.btnBuy.Text = "Koupit";
            this.btnBuy.Location = new System.Drawing.Point(20, 230);
            this.btnBuy.Click += new EventHandler(this.btnBuy_Click);


            this.pbQr.Location = new System.Drawing.Point(260, 20);
            this.pbQr.Size = new System.Drawing.Size(200, 200);
            this.pbQr.BorderStyle = BorderStyle.FixedSingle;



            this.lvAvailability.Location = new System.Drawing.Point(20, 280);
            this.lvAvailability.Size = new System.Drawing.Size(840, 260);
            this.lvAvailability.View = View.Details;
            this.lvAvailability.Columns.Add("Okruh", 200);
            this.lvAvailability.Columns.Add("Čas", 200);
            this.lvAvailability.Columns.Add("Volno", 120);

            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.AddRange(new Control[] { cbCircuit, dtTime, gbType, btnBuy, pbQr, lvAvailability });
            this.Text = "Prodej vstupenek";

            this.ResumeLayout(false);
        }
    }
}
