using eNetwork3.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork3
{
    public class eServer
    {
        int port;

        TcpListener listener;

        List<Task> taskList;

        List<TcpClient> clientList;

        public delegate void DataReceivedHandler(TcpClient client, byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void ConnectionHandler(TcpClient client);
        public event ConnectionHandler OnClientConnected, OnClientDisconnected;

        public int DebugLevel { get; set; }

        public bool Connected { get { return listener.Server.Connected; } }

        public int SendBufferSize { get { return listener.Server.SendBufferSize; } set { listener.Server.SendBufferSize = value; } }
        public int ReceiveBufferSize { get { return listener.Server.ReceiveBufferSize; } set { listener.Server.ReceiveBufferSize = value; } }

        public eServer(int port)
        {
            DebugLevel = 0;

            this.port = port;

            listener = new TcpListener(IPAddress.Loopback, port);

            taskList = new List<Task>();

            clientList = new List<TcpClient>();
        }

        public bool Start()
        {
            try
            {
                //Start listen for client
                listener.Start();
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
                clientList.Clear();

                //Stop listen for client
                if (listener.Server.Connected)
                    listener.Stop();

                Logger.Log("Server stopped successfully.", DebugLevel);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to stop the server : " + ex.Message, DebugLevel);

                return false;
            }
        }

        public bool SendTo(byte[] buffer, TcpClient client)
        {
            try
            {
                client.Send(buffer);

                Logger.Debug("Buffer sent successfully.", DebugLevel);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to send buffer : " + ex.Message, DebugLevel);
                return false;
            }
        }

        public bool SendToAll(byte[] buffer)
        {
            foreach (TcpClient client in clientList)
            {
                try
                {
                    client.Send(buffer);
                    Logger.Debug("Buffer sent successfully to this client : " + client.Client.RemoteEndPoint.ToString(), DebugLevel);
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to send buffer to this client : " + client.Client.RemoteEndPoint.ToString() + " More info : " + ex.Message, DebugLevel);
                }
            }
            return true;
        }

        public bool SendToAll(byte[] buffer, TcpClient exceptedClient)
        {
            foreach (TcpClient client in clientList)
            {
                if (client != exceptedClient)
                {
                    try
                    {
                        client.Send(buffer);
                        Logger.Debug("Buffer sent successfully to this client : " + client.Client.RemoteEndPoint.ToString(), DebugLevel);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Failed to send buffer to this client : " + client.Client.RemoteEndPoint.ToString() + " More info : " + ex.Message, DebugLevel);
                    }
                }
            }
            return true;
        }

        public List<TcpClient> GetClientList()
        {
            return clientList;
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
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    clientList.Add(client);

                    if (OnClientConnected != null)
                        OnClientConnected(client);

                    StartHandleClient(client);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to listen for new client : " + ex.Message, DebugLevel);
            }
            finally
            {
                Stop();
            }
        }

        void StartHandleClient(TcpClient client)
        {
            taskList.Add(HandleClientAsync(client));
        }

        async Task HandleClientAsync(TcpClient client)
        {
            byte[] buffer = new byte[ReceiveBufferSize];
            byte[] result;

            try
            {
                while (client.Client.Connected)
                {
                    int size = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);

                    if (size == 0)
                        break;

                    result = new byte[size];

                    Array.Copy(buffer, result, result.Length);

                    if (OnDataReceived != null)
                        OnDataReceived(client, result);
                }
            }
            catch (Exception ex)
            {
                if(ex.HResult != -2146232800)
                    Logger.Error("Failed to handle client : " + client.Client.RemoteEndPoint.ToString() + " More info : " + ex.Message, DebugLevel);
            }
            finally
            {
                clientList.Remove(client);
                if (OnClientDisconnected != null)
                    OnClientDisconnected(client);
            }
        }
    }
}
