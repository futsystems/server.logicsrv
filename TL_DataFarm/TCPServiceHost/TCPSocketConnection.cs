using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TCPServiceHost
{
    /// <summary>
    /// 封装了一个TCPSocketServiceHost 中的Connection对象
    /// </summary>
    public class TCPSocketConnection:IConnection
    {
        ILog logger = LogManager.GetLogger("conn");

        public string IPAddress 
        { 
            get 
            { 
                return _session.RemoteEndPoint.Address.ToString(); 
            } 
            set 
            { 
                throw new NotSupportedException();
            } 
        }


        public Command Command { get; set; }
        public string SessionID
        {
            get { return _session.SessionID; }
            set { throw new NotSupportedException(); }
        }

        public IServiceHost ServiceHost
        {
            get { return _serviceHost; }
            set { throw new NotSupportedException(); }
        }



        string _loginID = string.Empty;
        public string LoginID
        {
            get { return _loginID; }
            set { _loginID = value; }
        }



        IServiceHost _serviceHost = null;
        TLSessionBase _session = null;
        public TCPSocketConnection(IServiceHost host,TLSessionBase session)
        {
            _session = session;
            _serviceHost = host;
            this.LastHeartBeat = DateTime.Now;
            this.Command = null;
        }


        public DateTime LastHeartBeat { get; set; }
        object _obj = new object();
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="packet"></param>
        public void Send(IPacket packet)
        {

            byte[] data = packet.Data;
            _session.Send(data, 0, data.Length);
            //bool re = _session.TrySend(data, 0, data.Length);
            //logger.Info(string.Format("send data lenght:{0} ret:{1}", data.Length, re));
            
            
        }

        
    }
}
