using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Delta
{
    public class TCPIPHostHelper
    {
        private Socket hostSocket;

        private string preDefinedIP1, preDefinedIP2;

        private string hostIP;
        public string HostIP
        {
            get
            {
                return hostIP;
            }
            set
            {
                hostIP = value;
            }
        }

        private int hostPort;
        public int HostPort
        {
            get
            {
                return hostPort;
            }
            set
            {
                hostPort = value;
            }
        }

        private bool listening;
        public bool Listening
        {
            get
            {
                return listening;
            }
            set
            {
                listening = value;
            }
        }

        private Dictionary<Socket, Thread> stDic;
        public Dictionary<Socket, Thread> StDic
        {
            get
            {
                return stDic;
            }
        }

        Thread acceptThread = null;

        public delegate void DiconnHandler(Socket socket, int cause);
        public event DiconnHandler OnDiconn;

        public delegate void ReceiveHandler(string msg);
        public event ReceiveHandler OnReceive;

        public int Count
        {
            get
            {
                return stDic.Count;
            }
        }

        public TCPIPHostHelper(string ipstr, int Port)
        {
            InitHost(ipstr, Port);
        }

        public void InitHost(string IPstr, int Port)
        {
            if (hostSocket != null)
            {
                hostSocket.Disconnect(false);
                hostSocket.Close();
            }

            hostSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IPstr), Port);

            hostSocket.Bind(iep);
            hostSocket.Listen(10);
            listening = true;

            if (stDic != null && stDic.Count != 0)
            {
                foreach (Socket s in stDic.Keys)
                {
                    s.Disconnect(false);
                    stDic[s].Abort();
                }
            }
            stDic = new Dictionary<Socket, Thread>();
            if (acceptThread != null)
            {
                acceptThread.Abort();
                acceptThread = null;
            }

            acceptThread = new Thread(new ThreadStart(AcceptThreadFunc));
            acceptThread.Start();

            hostIP = IPstr;
            hostPort = Port;           
        }

        public void AcceptThreadFunc()
        {
            while (true)
            {
                Socket cs = hostSocket.Accept();

                Thread t = new Thread(new ParameterizedThreadStart(ReadThreadFunc));
                t.Start(cs);
                stDic.Add(cs, t);
            }
        }

        public void ReadThreadFunc(object param)
        {
            Socket socket = param as Socket;

            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int ret = socket.Receive(buffer);
                    if (ret <= 0)
                    {
                        if (OnDiconn != null)
                        {
                            OnDiconn(socket, 0);
                            stDic.Remove(socket);
                            return;
                        }
                    }
                    else
                    {
                        string msg = Encoding.ASCII.GetString(buffer, 0, ret);
                        OnReceive?.Invoke(msg);

                    }

                }
                catch (Exception ex)
                {

                    string ss = ex.ToString();

                    if (OnDiconn != null)
                    {
                        OnDiconn(socket, 1);
                        stDic.Remove(socket);

                        Thread.Sleep(50);
                    }
                }
            }
        }

        public void SendMsg(Socket socket, string msg)
        {
            if (socket == null)
                return;
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            if (stDic.ContainsKey(socket))
            {
                socket.Send(buffer);
            }

        }

        public void Dispose()
        {
            if (hostSocket != null)
            {
                hostSocket.Close();
                listening = false;
            }
            if (stDic != null && stDic.Count != 0)
            {
                foreach (Socket s in stDic.Keys)
                {
                    s.Disconnect(false);
                    stDic[s].Abort();
                }
            }
            if (acceptThread != null)
            {
                acceptThread.Abort();
                acceptThread = null;
            }
        }
    }

    public class TCPIPClientHost
    {
        private string hostIP = null;
        private Socket clientSocket = null;
        private Thread readThread = null;

        public delegate void DiconnHandler(Socket socket, int cause);
        public event DiconnHandler OnDiconn;

        public delegate void ReceiveHandler(string msg);
        public event ReceiveHandler OnReceive;

        public TCPIPClientHost(string IP, int Port)
        {
            InitClient(IP, Port);
        }

        public void InitClient(string hostIP, int Port)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(IPAddress.Parse(hostIP), Port);

            if (readThread != null)
            {
                readThread.Abort();
                readThread = null;
            }

            readThread = new Thread(new ThreadStart(ReadThreadFunc));
            readThread.Start();
        }

        public void Diconnect()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
        }

        public void ReadThreadFunc()
        {
            byte[] buffer =  new byte[1024];
            while (true)
            {
                int ret = clientSocket.Receive(buffer);
                if (ret <= 0)
                {
                    if (OnDiconn != null)
                    {
                        OnDiconn(clientSocket, 0);
                        return;
                    }
                }
                else
                {
                    string msg = Encoding.ASCII.GetString(buffer, 0, ret);
                    OnReceive?.Invoke(msg);

                }

                Thread.Sleep(50);
            }
        }

        public void SendMsg(Socket socket, string msg)
        {
            if (socket == null)
                return;
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            clientSocket.Send(buffer);          
        }
    }
}
