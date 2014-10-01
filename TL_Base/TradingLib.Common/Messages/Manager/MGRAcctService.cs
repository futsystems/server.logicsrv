using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 查询帐户服务
    /// 查询某个帐户的服务如果服务存在则返回服务信息
    /// 每个服务的信息不同 通过json格式返回
    /// 接收端进行处理
    /// </summary>
    public class MGRQryAcctServiceRequest:RequestPacket
    {
        public MGRQryAcctServiceRequest()
        {

            _type = MessageTypes.MGRQRYACCTSERVICE;
            this.TradingAccount = string.Empty;
            this.ServiceName = string.Empty;
        }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.ServiceName);
            sb.Append(d);
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.ServiceName = rec[1];
        }
    }

    public class RspMGRQryAcctServiceResponse : RspResponsePacket
    {
        public RspMGRQryAcctServiceResponse()
        {
            _type = MessageTypes.MGRQRYACCTSERVICERESPONSE;
            this.TradingAccount = string.Empty;
            this.ServiceName = string.Empty;
            this.JsonRet = string.Empty;
        }

        public string TradingAccount { get; set; }

        public string ServiceName { get; set; }

        public string JsonRet { get; set; }

        public override string ResponseSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = '~';

            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.ServiceName);
            sb.Append(d);
            sb.Append(this.JsonRet);
            sb.Append(d);
            return sb.ToString();
        }

        public override void ResponseDeserialize(string content)
        {
            string[] rec = content.Split('~');
            this.TradingAccount = rec[0];
            this.ServiceName = rec[1];
            this.JsonRet = rec[2];
        }
    }
}
