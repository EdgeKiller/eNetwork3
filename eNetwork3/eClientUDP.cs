using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork3
{
    public class eClientUDP
    {
        string hostName;
        int port;

        List<Task> taskList;

        UdpClient client;

        public delegate void DataReceivedHandler(byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void ConnectionHandler();
        public event ConnectionHandler OnConnected, OnDisconnected;

        public int DebugLevel { get; set; }

        public bool Connected { get { return client.Client.Connected; } }

        public int SendBufferSize { get { return client.Client.SendBufferSize; } set { client.Client.SendBufferSize = value; } }
        public int ReceiveBufferSize { get { return client.Client.ReceiveBufferSize; } set { client.Client.ReceiveBufferSize = value; } }

        public eClientUDP(string hostName, int port)
        {
            DebugLevel = 0;

            this.hostName = hostName;
            this.port = port;

            client = new UdpClient();

            taskList = new List<Task>();
        }

        public bool Connect()
        {
            try
            {
                //Connect the TcpClient
                client.Connect(hostName, port);

                //Invoke the event
                if (OnConnected != null)
                    OnConnected.Invoke();

                Logger.Log("Connected successfully to " + hostName + ":" + port, DebugLevel);

                StartHandle();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to connect : " + ex.Message, DebugLevel);

                Disconnect(); //To be sure the TcpClient is disconnected

                return false;
            }
        }

        public bool Disconnect()
        {
            if (Connected)
            {
                try
                {
                    //Disconnect the TcpClient
                    client.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to disconnect : " + ex.Message, DebugLevel);

                    return false;
                }
            }

            //Invoke the event
            if (OnDisconnected != null)
                OnDisconnected.Invoke();

            return true;
        }

        public bool Send(byte[] buffer)
        {
            try
            {
                client.Send(buffer, buffer.Length, hostName, port);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to send buffer : " + ex.Message, DebugLevel);

                return false;
            }
        }

        void StartHandle()
        {
            taskList.Add(HandleAsync());
        }

        async Task HandleAsync()
        {
            try
            {
                while (Connected)
                {
                    UdpReceiveResult result = await client.ReceiveAsync();

                    if (OnDataReceived != null)
                        OnDataReceived.Invoke(result.Buffer);
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2146232800) //Server closed
                    Logger.Error("Failed to receive buffer : " + ex.Message, DebugLevel);
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
