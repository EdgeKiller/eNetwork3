using System;
using System.Net;
using System.Net.Sockets;

namespace eNetwork3
{
    public class eServerUDP
    {
        //Server
        Socket serverSocket;
        byte[] buffer;
        int port;
        EndPoint endPoint;

        //Events
        public delegate void DataReceivedHandler(EndPoint endPoint, byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        //Parameters
        public int DebugLevel { get; set; } = 0;
        public bool Connected { get { return serverSocket.Connected; } }
        public int BufferSize { get; set; }
        public int Backlog { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port"></param>
        public eServerUDP(int port, int bufferSize = 1024)
        {
            this.port = port;
            BufferSize = bufferSize;
            endPoint = new IPEndPoint(IPAddress.Any, port) as EndPoint;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            buffer = new byte[BufferSize];
            serverSocket.Bind(endPoint);
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            DebugMessage("Server started on port : " + port, 1);
            serverSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endPoint, ReceivedCallback, endPoint);
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
        public void SendTo(EndPoint endPoint, byte[] buffer)
        {
            serverSocket.SendTo(buffer, endPoint);
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
        /// ReceivedCallback
        /// </summary>
        /// <param name="result"></param>
        void ReceivedCallback(IAsyncResult result)
        {
            EndPoint localEndPoint = result.AsyncState as EndPoint;
            try
            {
                int bufferSize = serverSocket.EndReceiveFrom(result, ref localEndPoint);
                byte[] packet = new byte[bufferSize];
                Array.Copy(buffer, packet, packet.Length);

                if (OnDataReceived != null)
                    OnDataReceived.Invoke(localEndPoint, packet);

                buffer = new byte[BufferSize];
                endPoint = new IPEndPoint(IPAddress.Any, port);
                serverSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endPoint, ReceivedCallback, endPoint);
            }
            catch
            {
                DebugMessage("Error when receiving data from : " + localEndPoint.ToString(), 2);
            }
        }
    }
}
