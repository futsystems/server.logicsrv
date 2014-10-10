using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class MGRQryAccountInfoRequest:RequestPacket
    {

        public MGRQryAccountInfoRequest()
        {
            _type = MessageTypes.MGRQRYACCOUNTINFO;
        }

        public string Account { get; set; }

        public override string ContentSerialize()
        {
            return this.Account;
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.Account = contentstr;
        }
    }

    public class RspMGRQryAccountInfoResponse:RspResponsePacket
    {
        public RspMGRQryAccountInfoResponse()
        {
            _type = MessageTypes.MGRACCOUNTINFORESPONSE;
            AccountInfoToSend = new AccountInfo();
        }

        public IAccountInfo AccountInfoToSend {get;set;}


        public override string  ResponseSerialize()
        {
 	         return AccountInfo.Serialize(AccountInfoToSend);
        }

        public override void  ResponseDeserialize(string content)
        {
 	        AccountInfoToSend = AccountInfo.Deserialize(content);
        }
    }

}
