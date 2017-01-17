using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.DataFarm.API
{

    public enum EnumConnProtocolType
    { 
        /// <summary>
        /// Json
        /// </summary>
        Json,
        /// <summary>
        /// 第三方XL协议
        /// </summary>
        XL,
        /// <summary>
        /// 内部TL协议
        /// </summary>
        TL,
        /// <summary>
        /// CTP标准协议
        /// </summary>
        CTP,
    }

    public class Command
    {
        public Command(int reqId,string module, string cmd, string parameters)
        {
            this.RequestId = reqId;
            this.ModuleID = module;
            this.CMDStr = cmd;
            this.Parameters = parameters;
        }


        public int RequestId { get; set; }
        public string ModuleID { get; set; }
        public string CMDStr { get; set; }
        public string Parameters { get; set; }
    }

    /// <summary>
    /// Connection接口 用于实现客户端连接的维护与管理
    /// 由ServiceHost负责创建
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Connection所处ServiceHost
        /// </summary>
        IServiceHost ServiceHost { get; set; }

        /// <summary>
        /// 回话编号
        /// </summary>
        string SessionID { get; set; }

        /// <summary>
        /// 登入ID
        /// </summary>
        string LoginID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        string IPAddress { get; set; }

        /// <summary>
        /// 最近的客户端心跳
        /// </summary>
        DateTime LastHeartBeat { get; set; }

        /// <summary>
        /// 向Connection发送消息
        /// </summary>
        /// <param name="packet"></param>
        void Send(IPacket packet);


        void Send(byte[] data);

        /// <summary>
        /// 关闭会话
        /// </summary>
        void Close();


        /// <summary>
        /// 扩展命令信息
        /// 如果IConnection请求的是扩展命令,则在命令解析过程中将命令信息设置到该对象 用于结果返回
        /// </summary>
        Command Command { get; set; }
    }
}
