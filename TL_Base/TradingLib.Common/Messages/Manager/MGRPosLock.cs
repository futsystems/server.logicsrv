using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 请求修改帐户锁仓权限
    /// </summary>
    public class MGRReqUpdatePosLockRequest:RequestPacket
    {
        public MGRReqUpdatePosLockRequest()
        {
            _type = MessageTypes.MGRUPDATEPOSLOCK;
            this.TradingAccount = string.Empty;
        }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 是否进行锁仓控制
        /// </summary>
        public bool PosLock { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.PosLock.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.PosLock = bool.Parse(rec[1]);
        }
    }

    /// <summary>
    /// 请求修改锁仓权限回报
    /// </summary>
    public class RspMGRReqUpdatePosLockResponse:RspResponsePacket
    {
        public RspMGRReqUpdatePosLockResponse()
        {
            _type = MessageTypes.MGRUPDATEPOSLOCKRESPONSE;
        }

        public override string ResponseSerialize()
        {
            return base.ResponseSerialize();
        }

        public override void ResponseDeserialize(string content)
        {
            base.ResponseDeserialize(content);
        }
    }
}
