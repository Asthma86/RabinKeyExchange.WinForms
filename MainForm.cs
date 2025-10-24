using System;
using System.Windows.Forms;

namespace RabinKeyExchange.WinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void btnStartServer_Click(object sender, EventArgs e)
        {
            int port = 8080;
            var server = new ServerForm();
            server.Show();
            await server.StartServer(port); 
        }

        private async void btnStartClient_Click(object sender, EventArgs e)
        {
            var client = new ClientForm();
            client.Show();
            await client.StartClient("127.0.0.1", 8080);
        }
    }
}