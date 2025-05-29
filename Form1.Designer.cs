namespace lib
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnPilihCSV;
        private System.Windows.Forms.Button btnPilihLogo;
        private System.Windows.Forms.Button btnGeneratePDF;
        private System.Windows.Forms.Label lblCSV;
        private System.Windows.Forms.Label lblLogo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnPilihCSV = new System.Windows.Forms.Button();
            this.btnPilihLogo = new System.Windows.Forms.Button();
            this.btnGeneratePDF = new System.Windows.Forms.Button();
            this.lblCSV = new System.Windows.Forms.Label();
            this.lblLogo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnPilihCSV
            // 
            this.btnPilihCSV.Location = new System.Drawing.Point(30, 30);
            this.btnPilihCSV.Name = "btnPilihCSV";
            this.btnPilihCSV.Size = new System.Drawing.Size(100, 30);
            this.btnPilihCSV.Text = "Pilih CSV";
            this.btnPilihCSV.Click += new System.EventHandler(this.btnPilihCSV_Click);
            // 
            // lblCSV
            // 
            this.lblCSV.Location = new System.Drawing.Point(150, 30);
            this.lblCSV.Size = new System.Drawing.Size(400, 30);
            this.lblCSV.Text = "Silahkan pilih file CSV";
            // 
            // btnPilihLogo
            // 
            this.btnPilihLogo.Location = new System.Drawing.Point(30, 70);
            this.btnPilihLogo.Size = new System.Drawing.Size(100, 30);
            this.btnPilihLogo.Text = "Pilih Logo";
            this.btnPilihLogo.Click += new System.EventHandler(this.btnPilihLogo_Click);
            // 
            // lblLogo
            // 
            this.lblLogo.Location = new System.Drawing.Point(150, 70);
            this.lblLogo.Size = new System.Drawing.Size(400, 30);
            this.lblLogo.Text = "Silahkan pilih file logo";
            // 
            // btnGeneratePDF
            // 
            this.btnGeneratePDF.Location = new System.Drawing.Point(30, 110);
            this.btnGeneratePDF.Size = new System.Drawing.Size(150, 30);
            this.btnGeneratePDF.Text = "Generate PDF";
            this.btnGeneratePDF.Click += new System.EventHandler(this.btnGeneratePDF_Click);

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(600, 180);
            this.Controls.Add(this.btnPilihCSV);
            this.Controls.Add(this.lblCSV);
            this.Controls.Add(this.btnPilihLogo);
            this.Controls.Add(this.lblLogo);
            this.Controls.Add(this.btnGeneratePDF);
            this.Text = "CSV ke PDF Generator";
            this.ResumeLayout(false);
        }
    }
}
