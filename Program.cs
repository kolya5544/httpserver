using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) {
                var port = int.Parse(args[0]);
                Console.WriteLine("=[ Starting HTTP Webserver at "+args[0]+" port!");

                TcpListener tcp = new TcpListener(IPAddress.Any, port);
                tcp.Start();
                var ListenThread = new Thread(() => {
                    Thread.CurrentThread.IsBackground = true;

                    while (true)
                    {
                        TcpClient client = tcp.AcceptTcpClient();

                        var ns = client.GetStream();
                        ns.ReadTimeout = 2000;
                        ns.WriteTimeout = 2000;
                    }
                });
            }
        }
    }
}
