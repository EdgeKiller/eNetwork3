using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace eNetwork3
{
    public class eServer
    {
        //Server
        Socket serverSocket;
        byte[] buffer;
        int port;

        //Clients
        public List<Socket> Clients { get { return clients; } }
        List<Socket> clients;

        //Events
        public delegate void DataReceivedHandler(Socket client, byte[] buffer);
        public event DataReceivedHandler OnDataReceived;
        public delegate void ConnectionHandler(Socket client);
        public event ConnectionHandler OnClientConnected, OnClientDisconnected;

        //Parameters
        public int DebugLevel { get; set; } = 0;
        public bool Connected { get { return serverSocket.Connected; } }
        public int BufferSize { get; set; }
        public int Backlog { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port"></param>
        public eServer(int port, int bufferSize = 1024)
        {
            this.port = port;

            BufferSize = bufferSize;
            buffer = new byte[BufferSize];

            clients = new List<Socket>();

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            }
            catch(Exception ex)
            {
                DebugMessage("Failed to bind the server, more info : " + ex.Message, 1);
            }
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            serverSocket.Listen(Backlog);
            Accept();
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            serverSocket.Shutdown(SocketShutdown.Both);
        }

        /// <summary>
        /// Send to specify client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public void SendTo(Socket client, byte[] buffer)
        {
            serverSocket.SendTo(buffer, client.RemoteEndPoint);
        }

        /// <summary>
        /// Send to all clients
        /// </summary>
        /// <param name="buffer"></param>
        public void SendToAll(byte[] buffer)
        {
            foreach (Socket c in clients)
            {
                serverSocket.SendTo(buffer, c.RemoteEndPoint);
            }
        }

        /// <summary>
        /// Send to all except one client
        /// </summary>
        /// <param name="exceptedClient"></param>
        /// <param name="buffer"></param>
        public void SendToAllExcept(Socket exceptedClient, byte[] buffer)
        {
            foreach (Socket c in clients)
            {
                if (c != exceptedClient)
                    serverSocket.SendTo(buffer, c.RemoteEndPoint);
            }
        }

        /// <summary>
        /// Show debug message on console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        void DebugMessage(object message, int level)
        {
            if (DebugLevel >= level)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Debug] " + message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Accept
        /// </summary>
        void Accept()
        {
            serverSocket.BeginAccept(AcceptedCallback, null);
        }

        /// <summary>
        /// AcceptedCallback
        /// </summary>
        /// <param name="result"></param>
        void AcceptedCallback(IAsyncResult result)
        {
            Socket clientSocket = serverSocket.EndAccept(result);
            if (clientSocket != null && clientSocket.Connected)
            {
                DebugMessage("New client connected : " + clientSocket.RemoteEndPoint.ToString(), 1);
                if (OnClientConnected != null)
                    OnClientConnected.Invoke(clientSocket);
                clients.Add(clientSocket);
                buffer = new byte[BufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, clientSocket);
            }
            else
            {
                DebugMessage("Problem with connection of client : " + clientSocket.RemoteEndPoint.ToString(), 2);
            }
            Accept();
        }

        /// <summary>
        /// ReceivedCallback
        /// </summary>
        /// <param name="result"></param>
        void ReceivedCallback(IAsyncResult result)
        {
            Socket clientSocket = result.AsyncState as Socket;
            if (clientSocket != null && clientSocket.Connected)
            {
                try
                {
                    int bufferSize = clientSocket.EndReceive(result);
                    if (bufferSize > 0)
                    {
                        byte[] packet = new byte[bufferSize];
                        Array.Copy(buffer, packet, packet.Length);

                        if (OnDataReceived != null)
                            OnDataReceived.Invoke(clientSocket, packet);

                        buffer = new byte[BufferSize];
                        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, clientSocket);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2147467259 && ex is SocketException)
                    {
                        DebugMessage("Client disconnected : " + clientSocket.RemoteEndPoint.ToString(), 1);
                        if (clients.Contains(clientSocket))
                            clients.Remove(clientSocket);
                    }
                    else
                    {
                        DebugMessage("Error when receiving data from : " + clientSocket.RemoteEndPoint.ToString(), 2);
                    }
                }
            }
            else
            {
                DebugMessage("Client disconnected : " + clientSocket.RemoteEndPoint.ToString(), 1);
                if (OnClientDisconnected != null)
                    OnClientDisconnected.Invoke(clientSocket);
                if (clients.Contains(clientSocket))
                    clients.Remove(clientSocket);
            }
        }
    }
}
