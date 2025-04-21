using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientTCP
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private Socket clientSocket;
        private const int port = 8005;
        private const string address = "127.0.0.1";
        private TextBox txtMessage = new TextBox();
        private Button btnSend = new Button();
        private TextBox txtStatus = new TextBox();
        private Button btnConnect = new Button();
        private Button btnDisconnect = new Button();

        public MainForm()
        {
            this.Text = "TCP Клиент";
            this.ClientSize = new System.Drawing.Size(465, 290);

            txtMessage.Location = new System.Drawing.Point(12, 32);
            txtMessage.Size = new System.Drawing.Size(360, 20);
            txtMessage.Enabled = false;

            btnSend.Location = new System.Drawing.Point(378, 30);
            btnSend.Size = new System.Drawing.Size(75, 23);
            btnSend.Text = "Отправить";
            btnSend.Enabled = false;
            btnSend.Click += BtnSend_Click;

            txtStatus.Location = new System.Drawing.Point(12, 71);
            txtStatus.Multiline = true;
            txtStatus.Size = new System.Drawing.Size(441, 178);
            txtStatus.ReadOnly = true;
            txtStatus.ScrollBars = ScrollBars.Vertical;

            btnConnect.Location = new System.Drawing.Point(12, 255);
            btnConnect.Size = new System.Drawing.Size(120, 23);
            btnConnect.Text = "Подключиться";
            btnConnect.Click += BtnConnect_Click;

            btnDisconnect.Location = new System.Drawing.Point(138, 255);
            btnDisconnect.Size = new System.Drawing.Size(120, 23);
            btnDisconnect.Text = "Отключиться";
            btnDisconnect.Enabled = false;
            btnDisconnect.Click += BtnDisconnect_Click;

            this.Controls.Add(txtMessage);
            this.Controls.Add(btnSend);
            this.Controls.Add(txtStatus);
            this.Controls.Add(btnConnect);
            this.Controls.Add(btnDisconnect);

            this.FormClosing += MainForm_FormClosing;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                txtStatus.AppendText($"Подключение к серверу...{Environment.NewLine}");
                clientSocket.Connect(ipPoint);
                
                txtStatus.AppendText($"Подключено. Введите сообщение и нажмите 'Отправить'{Environment.NewLine}");
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                btnSend.Enabled = true;
                txtMessage.Enabled = true;
            }
            catch (Exception ex)
            {
                txtStatus.AppendText($"Ошибка подключения: {ex.Message}{Environment.NewLine}");
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                txtStatus.AppendText($"Нет подключения к серверу{Environment.NewLine}");
                return;
            }

            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Введите сообщение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                clientSocket.Send(data);
                txtStatus.AppendText($"Отправлено: {message}{Environment.NewLine}");

                if (message.ToLower() == "exit")
                {
                    Disconnect();
                    txtStatus.AppendText($"Отключено от сервера (команда exit){Environment.NewLine}");
                    return;
                }

                data = new byte[256];
                int bytes = clientSocket.Receive(data);
                string response = Encoding.Unicode.GetString(data, 0, bytes);
                txtStatus.AppendText($"Ответ сервера: {response}{Environment.NewLine}");

                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                txtStatus.AppendText($"Ошибка: {ex.Message}{Environment.NewLine}");
                Disconnect();
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
            txtStatus.AppendText($"Отключено от сервера{Environment.NewLine}");
        }

        private void Disconnect()
        {
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch { }
            finally
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                btnSend.Enabled = false;
                txtMessage.Enabled = false;
                txtMessage.Clear();
                clientSocket = null;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }
    }
}