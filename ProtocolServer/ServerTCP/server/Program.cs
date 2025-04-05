using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpServer
{
    class Program
    {
        static int port = 8005;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, port);
            using Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(ipPoint);
            listenSocket.Listen(10);

            while (true)
            {
                Socket handler = listenSocket.Accept();
                
                // Сохраняем RemoteEndPoint ДО обработки клиента
                string clientEndPoint = handler.RemoteEndPoint?.ToString() ?? "неизвестный клиент";
                Console.WriteLine($"\nПодключен клиент {clientEndPoint}");
                
                try
                {
                    ProcessClient(handler);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка с клиентом {clientEndPoint}: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine($"Клиент {clientEndPoint} отключен");
                }
            }
        }

        static void ProcessClient(Socket handler)
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[256];
                    int bytes = handler.Receive(data);
                    string message = Encoding.Unicode.GetString(data, 0, bytes);
                    
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");

                    if (message.ToLower() == "exit")
                    {
                        handler.Send(Encoding.Unicode.GetBytes("Соединение закрыто"));
                        break;
                    }

                    handler.Send(Encoding.Unicode.GetBytes("Сообщение получено: " + message));
                }
            }
            finally
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}