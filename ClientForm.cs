using System;
using System.Net.Sockets;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RabinKeyExchange.WinForms
{
    public partial class ClientForm : Form
    {
        private TcpClient? client;
        private NetworkStream? stream;
        private Rabin? clientRabin;
        private BigInteger? serverN;

        public ClientForm()
        {
            InitializeComponent();
        }

        private void Log(string s)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new Action(() => Log(s)));
                }
                catch (ObjectDisposedException) { /* форма закрыта */ }
                return;
            }
            txtLogClient.AppendText($"[{DateTime.Now:HH:mm:ss}] {s}\n");
        }

        public async Task StartClient(string host, int port)
        {
            try
            {
                Log("Connecting to server...");
                client = new TcpClient();
                await client.ConnectAsync(host, port);
                stream = client.GetStream();
                Log("Connected to server");

                await HandleClientAsync();
            }
            catch (Exception ex)
            {
                Log("Connect error: " + ex.Message);
            }
        }

        private async Task HandleClientAsync()
        {
            try
            {
                serverN = await NetworkHelpers.ReadBigIntegerAsync(stream!);
                Log("Client: received server public N");

                clientRabin = Rabin.GenerateKeys(256);
                Log("Client: Rabin keys generated");

                await NetworkHelpers.SendBigIntegerAsync(stream!, clientRabin.N);
                Log("Client: sent public N");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (true)
                        {
                            var msg = await NetworkHelpers.ReadMessageAsync(stream!);
                            if (msg.Type == 0x02)
                            {
                                BigInteger c = Utilities.BytesToBigIntegerUnsigned(msg.Payload);
                                byte[]? plain = Rabin.TryDecryptRabin(clientRabin!, c);
                                if (plain != null)
                                {
                                    string text = System.Text.Encoding.UTF8.GetString(plain);
                                    Log("Сервер: " + text);
                                }
                                else
                                {
                                    Log("Client: decryption failed");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("Receive loop ended: " + ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                Log("HandleClientAsync error: " + ex.Message);
            }
        }

        private async void btnSendClient_Click(object sender, EventArgs e)
        {
            if (stream == null || serverN == null)
            {
                Log("Not ready (no session)");
                return;
            }

            string text = txtInputClient.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] padded = Rabin.CreatePaddedPlaintext(plainBytes);
                BigInteger m = Utilities.BytesToBigIntegerUnsigned(padded);
                BigInteger c = BigInteger.ModPow(m, 2, serverN.Value);
                byte[] cBytes = Utilities.BigIntegerToBytesUnsigned(c);

                await NetworkHelpers.SendMessageAsync(stream, 0x02, cBytes);
                Log("Клиент: " + text);
                txtInputClient.Clear();
            }
            catch (Exception ex)
            {
                Log("Send error: " + ex.Message);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                client?.Close();
            }
            catch { }
        }
    }
}