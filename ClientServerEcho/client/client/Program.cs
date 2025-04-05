using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpClient
{
    class Program
    {
        static int port = 8005;
        static string address = "127.0.0.1";
        
        static void Main(string[] args)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                Console.WriteLine("Подключение к серверу...");
                socket.Connect(ipPoint);
                
                Console.Write("Введите сообщение: ");
                string message = Console.ReadLine();
                
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);
                
                // Получаем ответ
                StringBuilder builder = new StringBuilder();
                data = new byte[256];
                int bytes = 0;
                do
                {   
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                
                Console.WriteLine("Ответ сервера: " + builder.ToString());

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.Read();
        }
    }
}

