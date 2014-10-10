using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class MGRAddAccountRequest:RequestPacket
    {
        public MGRAddAccountRequest()
        {
            _type = MessageTypes.MGRADDACCOUNT;
            this.Category = QSEnumAccountCategory.DEALER;
            this.AccountID = string.Empty;
            this.Password = string.Empty;
            this.UserID = 0;
            this.MgrID = 0;
        }

        /// <summary>
        /// 添加帐户类别
        /// </summary>
        public QSEnumAccountCategory Category { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 绑定的用户UserID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 代理商ID
        /// 用于指明该交易帐户属于哪个Manager域
        /// 每个Manager可以开始多个管理帐户，是以Root或Agent标识的管理来跟踪ID
        /// </summary>
        public int MgrID { get; set; }
        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.Category.ToString());
            sb.Append(d);
            sb.Append(this.AccountID);
            sb.Append(d);
            sb.Append(this.Password);
            sb.Append(d);
            sb.Append(this.UserID.ToString());
            sb.Append(d);
            sb.Append(this.MgrID.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Category = (QSEnumAccountCategory)Enum.Parse(typeof(QSEnumAccountCategory), rec[0]);
            this.AccountID = rec[1];
            this.Password = rec[2];
            this.UserID = int.Parse(rec[3]);
            this.MgrID = int.Parse(rec[4]);
        }
    }
}
