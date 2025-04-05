using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpServer
{
    class Program
    {
        static int port = 8005; // порт для приема входящих запросов
        
        static void Main(string[] args)
        {     
            String Host = Dns.GetHostName();
            Console.WriteLine("Comp name: " + Host);
            IPAddress[] IPs = Dns.GetHostAddresses(Host);
            foreach (IPAddress ip1 in IPs)
                Console.WriteLine(ip1);

            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет сервера
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("\nСервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();  // сокет для связи с клиентом
                    
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    int kol_bytes = 0;
                    byte[] data = new byte[255];
                    
                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        kol_bytes += bytes;
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                    Console.WriteLine($"Получено байт: {kol_bytes}\n");
                    
                    // отправляем ответ клиенту (эхо)
                    string reply = builder.ToString();
                    data = Encoding.Unicode.GetBytes(reply);
                    handler.Send(data);
                    
                    // закрываем соединение
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}