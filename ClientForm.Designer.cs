namespace RabinKeyExchange.WinForms
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLogClient = new System.Windows.Forms.RichTextBox();
            this.txtInputClient = new System.Windows.Forms.TextBox();
            this.btnSendClient = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLogClient
            // 
            this.txtLogClient.Location = new System.Drawing.Point(12, 12);
            this.txtLogClient.Name = "txtLogClient";
            this.txtLogClient.ReadOnly = true;
            this.txtLogClient.Size = new System.Drawing.Size(380, 350);
            this.txtLogClient.TabIndex = 0;
            this.txtLogClient.Text = "";
            // 
            // txtInputClient
            // 
            this.txtInputClient.Location = new System.Drawing.Point(12, 370);
            this.txtInputClient.Name = "txtInputClient";
            this.txtInputClient.Size = new System.Drawing.Size(260, 23);
            this.txtInputClient.TabIndex = 1;
            // 
            // btnSendClient
            // 
            this.btnSendClient.Location = new System.Drawing.Point(280, 370);
            this.btnSendClient.Name = "btnSendClient";
            this.btnSendClient.Size = new System.Drawing.Size(112, 27);
            this.btnSendClient.TabIndex = 2;
            this.btnSendClient.Text = "Send (Client)";
            this.btnSendClient.UseVisualStyleBackColor = true;
            this.btnSendClient.Click += new System.EventHandler(this.btnSendClient_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 405);
            this.Controls.Add(this.btnSendClient);
            this.Controls.Add(this.txtInputClient);
            this.Controls.Add(this.txtLogClient);
            this.Name = "ClientForm";
            this.Text = "Rabin Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtLogClient;
        private System.Windows.Forms.TextBox txtInputClient;
        private System.Windows.Forms.Button btnSendClient;
    }
}