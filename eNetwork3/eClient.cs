using eNetwork3.Utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork3
{
    public class eClient
    {
        string hostName;
        int port;

        List<Task> taskList;

        TcpClient client;

        public delegate void DataReceivedHandler(byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void ConnectionHandler();
        public event ConnectionHandler OnConnected, OnDisconnected;

        public int DebugLevel { get; set; }

        public bool Connected { get { return client.Connected; } }

        public int SendBufferSize { get { return client.SendBufferSize; } set { client.SendBufferSize = value; } }
        public int ReceiveBufferSize { get { return client.ReceiveBufferSize; } set { client.ReceiveBufferSize = value; } }

        public eClient(string hostName, int port)
        {
            DebugLevel = 0;

            this.hostName = hostName;
            this.port = port;

            client = new TcpClient();

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
                client.Send(buffer);

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
            byte[] buffer = new byte[client.ReceiveBufferSize];
            byte[] result;

            try
            {
                while (Connected)
                {

                    int size = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);

                    if (size == 0)
                        break;

                    result = new byte[size];
                    Array.Copy(buffer, result, result.Length);

                    if (OnDataReceived != null)
                        OnDataReceived.Invoke(result);

                }
            }
            catch (Exception ex)
            {
                if(ex.HResult != -2146232800) //Server closed
                    Logger.Error("Failed to receive buffer : " + ex.Message, DebugLevel);
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
