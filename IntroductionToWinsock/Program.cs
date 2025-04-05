using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Display special IP addresses
            Console.WriteLine("IP Loopback = " + IPAddress.Loopback);
            Console.WriteLine("IP None = " + IPAddress.None);
            Console.WriteLine("IP Any = " + IPAddress.Any);
            Console.WriteLine("IP Broadcast = " + IPAddress.Broadcast);

            IPAddress ip = IPAddress.Parse("123.45.67.89");
            Console.WriteLine("Parsed IP = " + ip.ToString());
            string hostName = Dns.GetHostName();
            Console.WriteLine("\nMy computer name = " + hostName);

            Console.WriteLine("\nAll IP addresses for this device:");
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                Console.WriteLine($"Address: {ipAddress.ToString()} (Family: {ipAddress.AddressFamily})");
            }

            Console.WriteLine("\nInformation about unn.ru:");
            IPHostEntry unnHost = Dns.GetHostEntry("unn.ru");
            Console.WriteLine("Host name: " + unnHost.HostName);
            
            foreach (IPAddress ip0 in unnHost.AddressList)
                Console.WriteLine("Address: " + ip0.ToString());

            Console.ReadLine();
        }
    }
}