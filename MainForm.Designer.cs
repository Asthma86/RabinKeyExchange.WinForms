namespace RabinKeyExchange.WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStartClient = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            this.btnStartServer.Location = new System.Drawing.Point(50, 30);
            this.btnStartServer.Size = new System.Drawing.Size(120, 30);
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            this.btnStartClient.Location = new System.Drawing.Point(50, 80);
            this.btnStartClient.Size = new System.Drawing.Size(120, 30);
            this.btnStartClient.Text = "Start Client";
            this.btnStartClient.Click += new System.EventHandler(this.btnStartClient_Click);
            // 
            this.ClientSize = new System.Drawing.Size(220, 150);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.btnStartClient);
            this.Text = "Rabin Key Exchange";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStartClient;
    }
}