using System;
using System.Net;
using System.Net.Sockets;

namespace eNetwork3
{
    public class eClient
    {
        //Client
        Socket clientSocket;
        byte[] buffer;

        //Server
        string ipAddress;
        int port;

        //Events
        public delegate void DataReceivedHandler(byte[] buffer);
        public event DataReceivedHandler OnDataReceived;
        public delegate void ConnectionHandler();
        public event ConnectionHandler OnConnected, OnDisconnected;

        //Parameters
        public int DebugLevel { get; set; } = 0;
        public bool Connected { get { return clientSocket.Connected; } }
        public int BufferSize { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public eClient(string ipAddress, int port, int bufferSize = 1024)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            buffer = new byte[BufferSize];
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        public void Connect()
        {
            clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectCallback, null);
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
        }

        /// <summary>
        /// Send to the server
        /// </summary>
        /// <param name="buffer"></param>
        public void Send(byte[] buffer)
        {
            clientSocket.Send(buffer);
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
        /// ConnectCallback
        /// </summary>
        /// <param name="result"></param>
        void ConnectCallback(IAsyncResult result)
        {
            if (clientSocket.Connected)
            {
                DebugMessage("Connected to the server !", 1);
                if (OnConnected != null)
                    OnConnected.Invoke();
                buffer = new byte[BufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, null);
            }
            else
            {
                DebugMessage("Could not connect.", 1);
            }
        }

        /// <summary>
        /// ReceivedCallback
        /// </summary>
        /// <param name="result"></param>
        void ReceivedCallback(IAsyncResult result)
        {
            if (clientSocket.Connected)
            {
                try
                {
                    int bufferSize = clientSocket.EndReceive(result);
                    byte[] packet = new byte[bufferSize];
                    Array.Copy(buffer, packet, packet.Length);
                    if (OnDataReceived != null)
                        OnDataReceived.Invoke(packet);
                    buffer = new byte[BufferSize];
                    clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, null);
                }
                catch(Exception ex)
                {
                    if (ex.HResult == -2147467259 && ex is SocketException)
                        DebugMessage("Server closed, client disconnected !", 1);
                    else
                        DebugMessage("Error when receiving data : " + ex.GetType(), 2);
                }
            }
            else
                DebugMessage("Client disconnected.", 1);
            
            if (!clientSocket.Connected)
            {
                if (OnDisconnected != null)
                    OnDisconnected.Invoke();
            }


        }
    }
}
