using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerTCP
{
    class Program
    {
        static int port = 8005;
        static void Main(string[] args)
        {
            String Host = Dns.GetHostName();
            Console.WriteLine("Host: " + Host);
            IPAddress[] IPs = Dns.GetHostAddresses(Host);
            foreach (IPAddress ip1 in IPs)
            {
                Console.WriteLine(ip1);
            }

            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);

                while (true)
                {
                    try
                    {
                        new ClientHandlerThread(listenSocket);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при подключении клиента: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сервера: {ex.Message}");
            }
            finally
            {
                listenSocket?.Close();
                Console.WriteLine("Сервер завершил работу. Нажмите Enter для выхода...");
                Console.ReadLine();
            }
        }
    }
}