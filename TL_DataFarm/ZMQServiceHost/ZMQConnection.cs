using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;


namespace ZMQServiceHost
{
    public class ZMQConnection:IConnection
    {
        public EnumFrontType FrontType { get { return EnumFrontType.TLSocket; } }
        ZMQServiceHost _host;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="address"></param>
        public ZMQConnection(ZMQServiceHost host, string serssionId)
        {
            _host = host;
            this.ServiceHost = host;
            this.SessionID = serssionId;
            this.LoginID = string.Empty;
            this.IPAddress = string.Empty;
            this.LastHeartBeat = DateTime.Now;
            this.Command = null;

        }
        public Version Version { get; set; }
        public Command Command { get; set; }
        /// <summary>
        /// Connection所处ServiceHost
        /// </summary>
        public IServiceHost ServiceHost { get; set; }

        /// <summary>
        /// 回话编号
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 登入ID
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }


        /// <summary>
        /// 最近客户端心跳
        /// </summary>
        public DateTime LastHeartBeat { get; set; }
        /// <summary>
        /// 向Connection发送消息
        /// </summary>
        /// <param name="packet"></param>
        public void Send(IPacket packet)
        {
            _host.Send(packet);
        }

        public void Send(string json)
        {
            //logger.Warn("Send string NotSupportedException");
        }

        public void Send(byte[] data)
        {

        }
        public void Close()
        { 
            
        }
    }
}
