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
            this.ManagerToSend = null;
        }

        public Manager ManagerToSend { get; set; }

        public override string ResponseSerialize()
        {
            if (this.ManagerToSend == null) return string.Empty;
            return this.ManagerToSend.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.ManagerToSend = null;
                return;
            }
            this.ManagerToSend = new Manager();
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
            this.ManagerToSend = null;
           
        }

        public Manager ManagerToSend { get; set; }

        public override string ContentSerialize()
        {
            if (this.ManagerToSend == null)
                return string.Empty;
            return this.ManagerToSend.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            if (string.IsNullOrEmpty(contentstr))
            {
                this.ManagerToSend = null;
                return;
            }
            this.ManagerToSend = new Manager();
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
            _type = MessageTypes.MGRUPDATEMANAGER;
            this.ManagerToSend = null;
           
        }

        public Manager ManagerToSend { get; set; }

        public override string ContentSerialize()
        {
            if (this.ManagerToSend == null) return string.Empty;
            return this.ManagerToSend.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            if (string.IsNullOrEmpty(contentstr))
            {
                this.ManagerToSend = null;
            }
            this.ManagerToSend = new Manager();
            this.ManagerToSend.Deserialize(contentstr);
        }
    }
}
