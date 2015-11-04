using System;
using System.Net;
using System.Net.Sockets;

namespace eNetwork3
{
    public class eClientUDP
    {
        //Client
        Socket clientSocket;
        byte[] buffer;

        //Server
        string ipAddress;
        int port;
        EndPoint serverEndPoint;

        //Events
        public delegate void DataReceivedHandler(byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        //Parameters
        public int DebugLevel { get; set; } = 0;
        public int BufferSize { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public eClientUDP(string ipAddress, int port, int bufferSize = 1024)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            BufferSize = bufferSize;
            serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port) as EndPoint;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            buffer = new byte[BufferSize];
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        public void Connect()
        {
            clientSocket.Connect(serverEndPoint);
            clientSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref serverEndPoint, ReceivedCallback, null);
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            clientSocket.Disconnect(true);
        }

        /// <summary>
        /// Send to the server
        /// </summary>
        /// <param name="buffer"></param>
        public void Send(byte[] buffer)
        {
            clientSocket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, serverEndPoint);
        }

        /// <summary>
        /// Show debug message on console
        /// </summary>
        /// <param name="message"></param>
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
        /// ReceivedCallback
        /// </summary>
        /// <param name="result"></param>
        void ReceivedCallback(IAsyncResult result)
        {
            try
            {
                int bufferSize = clientSocket.EndReceiveFrom(result, ref serverEndPoint);
                byte[] packet = new byte[bufferSize];
                Array.Copy(buffer, packet, packet.Length);
                if (OnDataReceived != null)
                    OnDataReceived.Invoke(packet);
                buffer = new byte[BufferSize];
                clientSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref serverEndPoint, ReceivedCallback, null);
            }
            catch (Exception ex)
            {
                DebugMessage("Error when receiving data : " + ex.GetType(), 2);
            }

        }
    }
}
