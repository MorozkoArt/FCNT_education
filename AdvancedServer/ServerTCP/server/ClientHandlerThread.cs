using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ServerTCP
{
    class ClientHandlerThread
    {
        private Thread thread;
        private Socket handler;


        public ClientHandlerThread(Socket listenSocket)
        {
            this.handler = listenSocket.Accept();
            thread = new Thread(ClientProcessing);
            thread.Start();
        }

        void ClientProcessing()
        {
            string clientEndPoint = handler.RemoteEndPoint?.ToString() ?? "неизвестный клиент";
            Console.WriteLine($"Подключен клиент: {clientEndPoint}");
            byte[] data = new byte[256];
            try
            {
                while (true)
                {
                    int bytesReceived = handler.Receive(data);
                    if (bytesReceived == 0) break;

                    string message = Encoding.Unicode.GetString(data, 0, bytesReceived);
                    Console.WriteLine($"[{clientEndPoint}] {DateTime.Now:t}: {message}");

                    if (message.ToLower() == "exit"){
                        break;
                    }
                    string response = "Сообщение получено: " + message;
                    handler.Send(Encoding.Unicode.GetBytes(response));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки клиента {clientEndPoint}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Клиент {clientEndPoint} отключен");
                handler?.Shutdown(SocketShutdown.Both);
                handler?.Close();
            }
        }
    }
}