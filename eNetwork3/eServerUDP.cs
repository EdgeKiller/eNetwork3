using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork3
{
    public class eServerUDP
    {
        int port;

        UdpClient listener;

        List<Task> taskList;

        public delegate void DataReceivedHandler(IPEndPoint endPoint, byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public int DebugLevel { get; set; }

        public bool Connected { get { return listener.Client.Connected; } }

        public int SendBufferSize { get { return listener.Client.SendBufferSize; } set { listener.Client.SendBufferSize = value; } }
        public int ReceiveBufferSize { get { return listener.Client.ReceiveBufferSize; } set { listener.Client.ReceiveBufferSize = value; } }

        public eServerUDP(int port)
        {
            DebugLevel = 0;

            this.port = port;

            listener = new UdpClient(new IPEndPoint(IPAddress.Loopback, port));

            taskList = new List<Task>();
        }

        public bool Start()
        {
            try
            {
                //Start listen
                StartListen();

                Logger.Log("Server started on " + IPAddress.Loopback + ":" + port, DebugLevel);

                return true;

            }
            catch (Exception ex)
            {
                Logger.Error("Failed to start the server : " + ex.Message, DebugLevel);

                Stop();

                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                taskList.Clear();

                Logger.Log("Server stopped successfully.", DebugLevel);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to stop the server : " + ex.Message, DebugLevel);

                return false;
            }
        }

        public bool SendTo(byte[] buffer, IPEndPoint endPoint)
        {
            try
            {
                listener.Send(buffer, buffer.Length, endPoint);

                Logger.Debug("Buffer sent successfully.", DebugLevel);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to send buffer : " + ex.Message, DebugLevel);
                return false;
            }
        }

        void StartListen()
        {
            taskList.Add(ListenAsync());
        }

        async Task ListenAsync()
        {
            try
            {
                while (true)
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
                    UdpReceiveResult result = await listener.ReceiveAsync();

                    if (OnDataReceived != null)
                        OnDataReceived(result.RemoteEndPoint, result.Buffer);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to handle receiver. More info : " + ex.Message, DebugLevel);
            }
        }


    }
}
