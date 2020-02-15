using System;
using System.IO;
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
                Console.WriteLine("=[ Starting HTTP Webserver at "+args[0]+" port! ]=");

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
                        var sr = new StreamReader(ns);
                        var sw = new StreamWriter(ns);
                        sw.AutoFlush = true;

                        Console.WriteLine("-> New client connected!");

                        bool DataAccepted = false;

                        var UserThread = new Thread(() => {
                            Thread.CurrentThread.IsBackground = true;

                            //RECEIVER HANDLING
                            string Builder = "";
                            while (!DataAccepted && client.Connected)
                            {
                                try
                                {
                                    string answ = sr.ReadLine();
                                    
                                    if (answ.Length < 1)
                                    {
                                        DataAccepted = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Answ - " + answ);
                                        Builder += answ + "\r\n";
                                    }
                                } catch
                                {

                                }
                            }

                            //REQUEST HANDLING
                            string[] request = Builder.Split("\r\n");
                            if (request[0].StartsWith("GET"))
                            {
                                string filename = request[0].Split(' ')[1];

                                //Default 200 response
                                sw.Write("HTTP/1.1 200 OK\r\n");
                                sw.Write("Server: IKTeam\r\n");
                                sw.Write("Content-Type: text/html; charset=utf-8\r\n");
                                sw.Write("Connection: close\r\n");

                                //Payload
                                sw.Write("\r\n");
                                sw.Write("<html><head><h1>Hello! You tried to request - "+filename+"</h1></head></html>\r\n");
                                client.Close();
                            }
                            
                        });
                        UserThread.Start();
                    }
                });
                ListenThread.Start();
                while (true)
                {
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
