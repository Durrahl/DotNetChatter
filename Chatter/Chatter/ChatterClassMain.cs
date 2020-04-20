using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Chatter
{
    class ChatterClassMain
    {
        static void Main(string[] args)
        {
            // Initial Starting Line
            Console.WriteLine("------------------------ Welcome to ChatterBot ------------------------");

            // Setting up class instance
            ChatterClassMain chtr = new ChatterClassMain();

            // Starting main loop
            chtr.EntryLoop();
        }

        void EntryLoop()
        {
            Console.WriteLine("Would you like to host(H) or connect(C) to a chat session?");
            string input = "";
            

            while (true)
            {
                input = Console.ReadLine();
                switch (input)
                {
                    // Host
                    case "H": case "h":
                        StartHost();
                        break;

                    // Connect
                    case "C": case "c":
                        StartClient();
                        break;
                }

                // Wrong or no input
                Console.Clear();
                Console.WriteLine("The inputted data was incorrect or incomplete!");
                Console.WriteLine("Would you like to host(H) or connect(C) to a chat session?");
            }
        }
        
        void StartHost()
        {
            int port = 0;
            while (port == 0)
            {
                Console.Clear();
                Console.WriteLine("What port would you like to open?");
                string tmpPort = Console.ReadLine();
                port = int.Parse(tmpPort);
            }

            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            // Listen for client
            Console.Clear();
            Console.WriteLine("Waiting for connection...");

            TcpClient client = server.AcceptTcpClient();

            // Get Stream
            NetworkStream stream = client.GetStream();

            // Start chat session
            ChatSession(stream, client);
        }

        void StartClient()
        {

            IPAddress ipAddress = null;
            string tmpAddress = null;
            while (ipAddress == null)
            {
                Console.Clear();
                Console.WriteLine("What IP Address would you like to connect to (excluding port)?");
                tmpAddress = Console.ReadLine();
                ipAddress = IPAddress.Parse(tmpAddress);
            }


            int port = 0;
            while (port == 0)
            {
                Console.Clear();
                Console.WriteLine("What port would you like to connect to?");
                string tmpPort = Console.ReadLine();
                port = int.Parse(tmpPort);
            }

            TcpClient client = new TcpClient(tmpAddress, port);
            NetworkStream stream = client.GetStream();
            ChatSession(stream, client);
        }

        void ChatSession(NetworkStream stream, TcpClient client)
        {
            Console.Clear();
            Console.WriteLine("Connected! ");

            Thread listenThread = new Thread(GetMessages);
            listenThread.Start(client);

            while (client.Connected)
            {
                string message = Console.ReadLine();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                stream.Write(buffer);
                
            }
        }

        void GetMessages(Object objClient)
        {
            TcpClient client = objClient as TcpClient;
            NetworkStream stream = client.GetStream();

            while (client.Connected)
            {
                byte[] buffer = new byte[1024];

                
                stream.Read(buffer, 0, buffer.Length);
                
                
                System.Collections.Generic.List<byte> bytes = new System.Collections.Generic.List<byte>();

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != 0)
                    {
                        // Add to list
                        bytes.Add(buffer[i]);
                    }
                    else
                    {
                        // Ensure breakout
                        i = buffer.Length + 2;
                        break;
                    }
                }

                buffer = bytes.ToArray();
                string message = Encoding.ASCII.GetString(buffer);    
                Console.WriteLine("RECIEVED: " + message);

                

                
            }
        }
    }
}
