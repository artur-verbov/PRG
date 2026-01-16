namespace TicketApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cbCircuit;
        private System.Windows.Forms.DateTimePicker dtTime;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.PictureBox pbQr;
        private System.Windows.Forms.ListView lvAvailability;

        private System.Windows.Forms.NumericUpDown nudAdult;
        private System.Windows.Forms.NumericUpDown nudChild;
        private System.Windows.Forms.NumericUpDown nudStudent;
        private System.Windows.Forms.Label lblAvailability;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cbCircuit = new System.Windows.Forms.ComboBox();
            this.dtTime = new System.Windows.Forms.DateTimePicker();
            this.btnBuy = new System.Windows.Forms.Button();
            this.pbQr = new System.Windows.Forms.PictureBox();
            this.lvAvailability = new System.Windows.Forms.ListView();
            this.nudAdult = new System.Windows.Forms.NumericUpDown();
            this.nudChild = new System.Windows.Forms.NumericUpDown();
            this.nudStudent = new System.Windows.Forms.NumericUpDown();
            this.lblAvailability = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.pbQr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudChild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStudent)).BeginInit();
            this.SuspendLayout();
            // 
            // cbCircuit
            // 
            this.cbCircuit.Items.AddRange(new object[] { "Okruh A", "Okruh B", "Okruh C" });
            this.cbCircuit.Location = new System.Drawing.Point(20, 20);
            this.cbCircuit.Size = new System.Drawing.Size(121, 23);
            // 
            // dtTime
            // 
            this.dtTime.CustomFormat = "yyyy-MM-dd HH:00";
            this.dtTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTime.Location = new System.Drawing.Point(20, 60);
            this.dtTime.MaxDate = new System.DateTime(2025, 12, 12, 17, 0, 0, 0);
            this.dtTime.MinDate = new System.DateTime(2025, 12, 12, 10, 0, 0, 0);
            this.dtTime.ShowUpDown = true;
            this.dtTime.Size = new System.Drawing.Size(200, 23);
            // 
            // NumericUpDowny
            // 
            this.nudAdult.Location = new System.Drawing.Point(20, 100);
            this.nudAdult.Minimum = 0;
            this.nudAdult.Maximum = 20;
            this.nudAdult.Value = 1;
            this.nudAdult.Size = new System.Drawing.Size(50, 23);
            Label lblAdult = new Label();
            lblAdult.Text = "Dospělí:";
            lblAdult.Location = new System.Drawing.Point(80, 100);
            lblAdult.AutoSize = true;
            this.Controls.Add(lblAdult);
            this.Controls.Add(this.nudAdult);

            this.nudChild.Location = new System.Drawing.Point(20, 130);
            this.nudChild.Minimum = 0;
            this.nudChild.Maximum = 20;
            this.nudChild.Value = 0;
            this.nudChild.Size = new System.Drawing.Size(50, 23);
            Label lblChild = new Label();
            lblChild.Text = "Děti:";
            lblChild.Location = new System.Drawing.Point(80, 130);
            lblChild.AutoSize = true;
            this.Controls.Add(lblChild);
            this.Controls.Add(this.nudChild);

            this.nudStudent.Location = new System.Drawing.Point(20, 160);
            this.nudStudent.Minimum = 0;
            this.nudStudent.Maximum = 20;
            this.nudStudent.Value = 0;
            this.nudStudent.Size = new System.Drawing.Size(50, 23);
            Label lblStudent = new Label();
            lblStudent.Text = "Studenti:";
            lblStudent.Location = new System.Drawing.Point(80, 160);
            lblStudent.AutoSize = true;
            this.Controls.Add(lblStudent);
            this.Controls.Add(this.nudStudent);
            // 
            // btnBuy
            // 
            this.btnBuy.Location = new System.Drawing.Point(20, 200);
            this.btnBuy.Size = new System.Drawing.Size(75, 23);
            this.btnBuy.Text = "Koupit";
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // pbQr
            // 
            this.pbQr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbQr.Location = new System.Drawing.Point(300, 20);
            this.pbQr.Size = new System.Drawing.Size(200, 200);
            this.pbQr.Click += new System.EventHandler(this.pbQr_Click);
            // 
            // lvAvailability
            // 
            this.lvAvailability.Location = new System.Drawing.Point(20, 250);
            this.lvAvailability.Size = new System.Drawing.Size(840, 300);
            this.lvAvailability.View = System.Windows.Forms.View.Details;
            // 
            // lblAvailability
            // 
            this.lblAvailability.Location = new System.Drawing.Point(20, 560);
            this.lblAvailability.Size = new System.Drawing.Size(200, 23);
            this.lblAvailability.Text = "Volná místa: ?";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.cbCircuit);
            this.Controls.Add(this.dtTime);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.pbQr);
            this.Controls.Add(this.lvAvailability);
            this.Controls.Add(this.lblAvailability);
            this.Text = "Prodej vstupenek";
            this.Load += new System.EventHandler(this.Form1_Load);

            ((System.ComponentModel.ISupportInitialize)(this.pbQr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudChild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStudent)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
