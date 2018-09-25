using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Instrument
{
   public class iTcpServer
    {
        #region "properties, variables"

        Socket server = null;
        public Socket Server
        {
            get
            {
                return server;
            }
        }

        Socket client = null;
        public Socket Client
        {
            get
            {
                return client;
            }
        }

        IPAddress serverIP;
        public IPAddress ServerIP
        {
            get
            {
                return serverIP;
            }
            set
            {
                serverIP = value;
            }
        }

        int serverPort = -1;

        public int ServerPort
        {
            get
            {
                return serverPort;
            }
            set
            {
                serverPort = value;
            }
        }

        bool isListening = false;
        public bool IsListening
        {
            get
            {
                return isListening;
            }
            set
            {
                isListening = value;
            }        
        }
        byte[] buffer;

        #endregion

        public class ReceiveEventArgs : EventArgs
        {
            string data = null;

            public string Data
            {
                get
                {
                    return data;
                }
            }

            public ReceiveEventArgs(string data)
            {
                this.data = data;
            }
        }

        public delegate void ReceiveHandler(object sender, ReceiveEventArgs receiveEventArgs);

        public event ReceiveHandler OnReceive;

        public iTcpServer(IPAddress IP, int Port)
        {
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ServerIP = IP;
            this.serverPort = Port;
        }
        void AcceptCallback(IAsyncResult ar)
        {
            iTcpServer ts = ar.AsyncState as iTcpServer;
            if (ts.client != null)
            {
                ts.client.Shutdown(SocketShutdown.Both);
                ts.client.Disconnect(false);
                ts.client.Close();
                ts.client = null;
            }
            try
            {
                ts.client = ts.server.EndAccept(ar);
                buffer = new byte[1024];
                client.BeginReceive(ts.buffer,0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
                server.BeginAccept(new AsyncCallback(ReceiveCallback),ts);
            }
            catch (SocketException se)
            {

            }
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            iTcpServer ts = ar.AsyncState as iTcpServer;
            try
            {
                int bytesReceived = ts.client.EndReceive(ar);
                if (bytesReceived > 0)
                {
                    if (OnReceive != null && client != null)
                    {
                        OnReceive(client, new ReceiveEventArgs(Encoding.ASCII.GetString(buffer, 0, bytesReceived)));
                    }
                }
                else
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    client = null;
                }

                ts.client.BeginReceive(ts.buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054)
                {
                    return ;
                }
            }
        }

        public static bool IsSocketConnected(Socket client)
        {
            bool connectState = true;
            bool blockingState = client.Blocking;

            try
            {
                byte[] tmp = new byte[1];
                client.Blocking = false;
                client.Send(tmp, 0, SocketFlags.None);
                connectState = true;
            }
            catch (SocketException e)
            {
                if (e.NativeErrorCode == 10035)
                    connectState = true;
                else
                    connectState = false;
            }
            finally
            {
                client.Blocking = blockingState;               
            }
            return connectState;
        }

        public void StartListen(int backlog)
        {
            IPEndPoint ep = new IPEndPoint(this.ServerIP, this.serverPort);
            server.Bind(ep);
            server.Listen(backlog);
            server.BeginAccept(new AsyncCallback(AcceptCallback), this);
            this.isListening = true;
        }
    }
}
