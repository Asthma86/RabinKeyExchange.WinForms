using System;
using System.Net.Sockets;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RabinKeyExchange.WinForms
{
    public partial class ServerForm : Form
    {
        private TcpListener? listener;
        private Rabin? serverRabin;
        private TcpClient? serverTcp;
        private NetworkStream? serverStream;
        private BigInteger? clientN;

        public ServerForm()
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
            txtLogServer.AppendText($"[{DateTime.Now:HH:mm:ss}] {s}\n");
        }

        public async Task StartServer(int port)
        {
            try
            {
                Log($"Starting server on port {port}...");
                listener = new TcpListener(System.Net.IPAddress.Any, port);
                listener.Start();
                Log($"Server listening on port {port}");

                Log("Starting accept loop...");
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await AcceptLoopAsync();
                    }
                    catch (Exception ex)
                    {
                        Log($"AcceptLoop crashed: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Log($"StartServer failed: {ex.Message}");
            }
        }

        private async Task AcceptLoopAsync()
        {
            try
            {
                Log("Waiting for client...");
                serverTcp = await listener!.AcceptTcpClientAsync();
                serverStream = serverTcp.GetStream();
                Log("Client connected");

                if (serverStream == null || !serverStream.CanWrite)
                {
                    Log("Stream is null or not writable!");
                    return;
                }

                serverRabin = Rabin.GenerateKeys(256);
                Log("Keys generated");

                await NetworkHelpers.SendBigIntegerAsync(serverStream, serverRabin.N);
                Log("Server: sent public N");

                clientN = await NetworkHelpers.ReadBigIntegerAsync(serverStream);
                Log("Server: received client public N");

                _ = HandleServerAsync();
            }
            catch (Exception ex)
            {
                Log("AcceptLoop error: " + ex.Message);
            }
        }

        private async Task HandleServerAsync()
        {
            Log("Server is ready for secure messaging!");

            while (true)
            {
                try
                {
                    var msg = await NetworkHelpers.ReadMessageAsync(serverStream!);
                    if (msg.Type == 0x02)
                    {
                        BigInteger c = Utilities.BytesToBigIntegerUnsigned(msg.Payload);
                        byte[]? plain = Rabin.TryDecryptRabin(serverRabin!, c);
                        if (plain != null)
                        {
                            string text = System.Text.Encoding.UTF8.GetString(plain);
                            Log("Клиент: " + text);
                        }
                        else
                        {
                            Log("Server: decryption failed (invalid padding)");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("Server receive loop ended: " + ex.Message);
                    break;
                }
            }
        }

        private async void btnSendServer_Click(object sender, EventArgs e)
        {
            if (serverStream == null || clientN == null)
            {
                Log("Server not ready (no active session)");
                return;
            }

            string text = txtInputServer.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] padded = Rabin.CreatePaddedPlaintext(plainBytes);
                BigInteger m = Utilities.BytesToBigIntegerUnsigned(padded);
                BigInteger c = BigInteger.ModPow(m, 2, clientN.Value);
                byte[] cBytes = Utilities.BigIntegerToBytesUnsigned(c);

                await NetworkHelpers.SendMessageAsync(serverStream, 0x02, cBytes);
                Log("Сервер: " + text);
                txtInputServer.Clear();
            }
            catch (Exception ex)
            {
                Log("Server send error: " + ex.Message);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                listener?.Stop();
                serverTcp?.Close();
            }
            catch { }
        }
    }
}