namespace RabinKeyExchange.WinForms
{
    partial class ServerForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtLogServer = new System.Windows.Forms.RichTextBox();
            this.txtInputServer = new System.Windows.Forms.TextBox();
            this.btnSendServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
          
            this.txtLogServer.Location = new System.Drawing.Point(12, 12);
            this.txtLogServer.Size = new System.Drawing.Size(380, 350);
            this.txtLogServer.ReadOnly = true;
         
            this.txtInputServer.Location = new System.Drawing.Point(12, 370);
            this.txtInputServer.Name = "txtInputServer";
            
            this.btnSendServer.Location = new System.Drawing.Point(280, 370);
            this.btnSendServer.Text = "Send (Server)";
            this.btnSendServer.Click += new System.EventHandler(this.btnSendServer_Click);
           
            this.ClientSize = new System.Drawing.Size(404, 405);
            this.Controls.Add(this.txtLogServer);
            this.Controls.Add(this.txtInputServer);
            this.Controls.Add(this.btnSendServer);
            this.Name = "ServerForm";
            this.Text = "Rabin Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.RichTextBox txtLogServer;
        private System.Windows.Forms.TextBox txtInputServer;
        private System.Windows.Forms.Button btnSendServer;
    }
}