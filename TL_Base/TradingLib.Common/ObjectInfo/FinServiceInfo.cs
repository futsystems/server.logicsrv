//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    public class FinServiceInfo : IFinServiceInfo
//    {
//        public string Account { get; set; }//账户
//        public decimal Ammount { get; set; }//金额
//        public QSEnumFinServiceType ServiceType { get; set; }//类别
//        public decimal Discount { get; set; }//折扣
//        public bool Active { get; set; }//激活
//        public string AgentCode { get; set; }
        
//        /// <summary>
//        /// 将FinServiceInfo生成文本序列化信息
//        /// </summary>
//        /// <param name="info"></param>
//        /// <returns></returns>
//        public static string Serialize(IFinServiceInfo info)
//        {
//            const char d = ',';
//            StringBuilder sb = new StringBuilder();
//            sb.Append(info.Account);
//            sb.Append(d);
//            sb.Append(info.Ammount);
//            sb.Append(d);
//            sb.Append(info.ServiceType.ToString());
//            sb.Append(d);
//            sb.Append(info.Discount);
//            sb.Append(d);
//            sb.Append(info.Active);
//            sb.Append(d);
//            sb.Append(info.AgentCode);
//            return sb.ToString();
//        }
//        /// <summary>
//        /// 从文字消息解析获得FinServiceInfo
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        public static IFinServiceInfo Deserialize(string msg)
//        {
//            string[] r = msg.Split(',');
//            FinServiceInfo fs = new FinServiceInfo();
//            if (r.Length >= 5)
//            {
//                fs.Account = Convert.ToString(r[0]);
//                fs.Ammount = Convert.ToDecimal(r[1]);
//                fs.ServiceType = (QSEnumFinServiceType)Enum.Parse(typeof(QSEnumFinServiceType), r[2]);
//                fs.Discount = Convert.ToDecimal(r[3]);
//                fs.Active = Convert.ToBoolean(r[4]);
//                fs.AgentCode = Convert.ToString(r[5]);
//            }
//            return fs;
//        }
//    }
//}
