using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;

using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace DataClient
{
    public class TLClient_Socket
    {

        protected ILog logger = null;
        Socket server;
        List<IPEndPoint> serverip = new List<IPEndPoint>();
        System.ComponentModel.BackgroundWorker _bw;
        System.ComponentModel.BackgroundWorker _bw2;


        static bool IsValidAddress(string ipaddr)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipaddr);
                return true;
            }
            catch { }
            return false;

        }

        static List<IPEndPoint> GetEndpoints(int port, params string[] servers)
        {
            List<IPEndPoint> ip = new List<IPEndPoint>();
            foreach (string server in servers)
                if (IsValidAddress(server))
                    ip.Add(new IPEndPoint(IPAddress.Parse(server), port));
            return ip;
        }


        #region 构造函数
        public TLClient_Socket(string server, int port, string clientName)
            : this(GetEndpoints(port, new string[] { server }), 0,clientName)
        { 
            
        }
        public TLClient_Socket(string[] servers, int port, string clientName)
            : this(GetEndpoints(port, servers), 0, clientName)
        { 
            
        }
        public TLClient_Socket(List<IPEndPoint> servers,int index,string clientName)
        {
            logger = LogManager.GetLogger(clientName);
            serverip = servers;

        }
        #endregion


        public void TLFound()
        {
            logger.Info("Searching avabile server list");
            foreach (IPEndPoint ep in serverip)
            {
                try
                {
                    logger.Info("Attempting to connect to:" + ep.ToString());
                    // attempt to connect
                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    s.Connect(ep);
                    //string command = "LOGIN kerry 123456\n\r";
                    //Stream socketStream = new NetworkStream(s);
                    //string command = "CMD:ECHO ";
                    //string[] parameters = new string[] { "Kerry", "Jiang", "China", "Shanghai" };
                    //string parameter = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Join(" ", parameters)));
                    
                    ////StreamReader reader = new StreamReader(socketStream, Encoding.Default, true)
                    //StreamWriter writer = new StreamWriter(socketStream, Encoding.Default, 1024 * 8);
                    //writer.WriteLine(command + parameter);
                    //writer.Flush();
                    byte[] nrequest = Message.sendmessage(MessageTypes.BROKERNAMEREQUEST, string.Empty);
                    s.Send(nrequest);

                    //byte[] data = new byte[command.Length];
                    //Array.Copy(command,data,command.Length);
                    //s.Send(Encoding.ASCII.GetBytes(command));
                    // try to get result
                    //byte[] data = new byte[s.ReceiveBufferSize];
                }
                catch (Exception ex)
                { 
                
                }
            }
        }
            
    }
    
}
