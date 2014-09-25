using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 请求修改密码
    /// </summary>
    public class MGRUpdatePassRequest:RequestPacket
    {
        public MGRUpdatePassRequest()
        {
            _type = MessageTypes.MGRUPDATEPASS;
            this.OldPass = string.Empty;
            this.NewPass = string.Empty;
        }

        public string OldPass { get; set; }

        public string NewPass { get; set; }

        public override string ContentSerialize()
        {
            return OldPass + "," + NewPass;
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');

            this.OldPass = rec[0];
            this.NewPass = rec[1];
        }
    }
}
