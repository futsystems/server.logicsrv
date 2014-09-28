using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 查询管理员列表
    /// </summary>
    public class MGRQryManagerRequest:RequestPacket
    {
        public MGRQryManagerRequest()
        {
            _type = MessageTypes.MGRQRYMANAGER;
        }

        public override string ContentSerialize()
        {
            return base.ContentSerialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            base.ContentDeserialize(contentstr);
        }
    }

    /// <summary>
    /// 查询管理员列表回报
    /// </summary>
    public class RspMGRQryManagerResponse : RspResponsePacket
    {
        public RspMGRQryManagerResponse()
        {
            _type = MessageTypes.MGRMANAGERRESPONSE;
            this.ManagerToSend = new Manager();
        }

        public Manager ManagerToSend { get; set; }

        public override string ResponseSerialize()
        {
            return this.ManagerToSend.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.ManagerToSend.Deserialize(content);
        }
    }

    /// <summary>
    /// 请求添加管理员
    /// </summary>
    public class MGRReqAddManagerRequest : RequestPacket
    {
        public MGRReqAddManagerRequest()
        {
            _type = MessageTypes.MGRADDMANAGER;
            this.ManagerToSend = new Manager();
           
        }

        public Manager ManagerToSend { get; set; }

        public override string ContentSerialize()
        {
            return this.ManagerToSend.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.ManagerToSend.Deserialize(contentstr);
        }
        
    }

    /// <summary>
    /// 请求更新管理员
    /// </summary>
    public class MGRReqUpdateManagerRequest : RequestPacket
    {
        public MGRReqUpdateManagerRequest()
        {
            _type = MessageTypes.MGRADDMANAGER;
            this.ManagerToSend = new Manager();
           
        }

        public Manager ManagerToSend { get; set; }

        public override string ContentSerialize()
        {
            return this.ManagerToSend.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.ManagerToSend.Deserialize(contentstr);
        }
    }
}
