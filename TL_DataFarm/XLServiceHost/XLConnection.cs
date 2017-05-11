using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace XLServiceHost
{
    /// <summary>
    /// 
    /// </summary>
    public class XLConnection:IConnection
    {
        ILog logger = LogManager.GetLogger("conn");
        public EnumFrontType FrontType { get { return EnumFrontType.XLTinny; } }
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
        XLSessionBase _session = null;
        public XLConnection(IServiceHost host, XLSessionBase session)
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
        //public void Send(IPacket packet)
        //{

        //    byte[] data = packet.Data;
        //    _session.Send(data, 0, data.Length);
        //    //bool re = _session.TrySend(data, 0, data.Length);
        //    //logger.Info(string.Format("send data lenght:{0} ret:{1}", data.Length, re));
            
            
        //}
        public void Send(byte[] data)
        {
            if (data == null || data.Length < 1) return;
            _session.Send(data, 0, data.Length);
        }

        public void Send(string json)
        {
            logger.Warn("Send string NotSupportedException");
        }


        /// <summary>
        /// 关闭Socket
        /// </summary>
        public void Close()
        {
            _session.Close();
        }

        
    }
}
