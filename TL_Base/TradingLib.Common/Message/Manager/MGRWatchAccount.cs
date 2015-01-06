using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    /// <summary>
    /// 设定观察交易帐号
    /// 将一组帐号设定为观察列表,服务端针对这组帐户推送实时数据
    /// </summary>
    public class MGRWatchAccountRequest:RequestPacket
    {
        public List<string> AccountList { get; set; }
        public MGRWatchAccountRequest()
        {
            _type = MessageTypes.MGRWATCHACCOUNTS;
            AccountList = new List<string>();
        }

        public void Add(IEnumerable<string> acclist)
        {
            AccountList.Clear();
            AccountList.AddRange(acclist);
        }

        public override string ContentSerialize()
        {
            return string.Join(",", AccountList.ToArray());
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Add(rec);
        }
    }
}
