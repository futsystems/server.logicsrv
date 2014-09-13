//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;


//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 记录交易客户端通信息
//    /// </summary>
//    public class TrdClientInfo : ClientInfoBase
//    {
//        public int UserID { get; set; }//记录交易帐号所对应的UserID用于查询用户信息
//        public string Account { get; set; }//记录客户端登入所对应的交易帐号
//        public string Symbols { get; set; }//记录客户端请求的symbol数据集

//        public TrdClientInfo(TrdClientInfo copythis)
//            : base(copythis)
//        {
//            Account = copythis.Account;
//            UserID = copythis.UserID;
//            Symbols = copythis.Symbols;

//        }
//        public TrdClientInfo(string frontid, string clientid, int sessionid)
//            : base(frontid, clientid, sessionid)
//        {
//            UserID = 0;
//            Account = string.Empty;
//            Symbols = string.Empty;
//        }

//        public TrdClientInfo()
//        {

//        }

//        public override string SubSerialize()
//        {
//            StringBuilder sb = new StringBuilder();
//            char d = ',';
//            sb.Append(Account);
//            sb.Append(d);
//            sb.Append(UserID.ToString());
//            return sb.ToString();
//        }

//        public override void SubDeserialize(string str)
//        {
//            string[] rec = str.Split(',');
//            Account = rec[0];
//            UserID = int.Parse(rec[1]);
//        }



//    }
//}
