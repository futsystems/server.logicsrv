//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    public class NotifyMGRSessionUpdateNotify : NotifyResponsePacket
//    {
//        public NotifyMGRSessionUpdateNotify()
//        {
//            _type = MessageTypes.MGRSESSIONSTATUSUPDATE;
//            this.TradingAccount = string.Empty;
//            IsLogin = false;
//            IPAddress = string.Empty;
//            FrontID = string.Empty;
//            ClientID = string.Empty;
//            HardwarCode = string.Empty;
//            ProductInfo = string.Empty;

//        }

//        /// <summary>
//        /// 交易帐号
//        /// </summary>
//        public string TradingAccount { get; set; }

//        /// <summary>
//        /// 是否登入
//        /// </summary>
//        public bool IsLogin { get; set; }

//        /// <summary>
//        /// IP地址
//        /// </summary>
//        public string IPAddress { get; set; }

//        /// <summary>
//        /// 前置地址
//        /// </summary>
//        public string FrontID { get; set; }

//        /// <summary>
//        /// 客户端ID
//        /// </summary>
//        public string ClientID { get; set; }

//        /// <summary>
//        /// 设备硬件编码
//        /// </summary>
//        public string HardwarCode { get; set; }

//        /// <summary>
//        /// 客户端产品信息
//        /// </summary>
//        public string ProductInfo { get; set; }

//        public override string ContentSerialize()
//        {
//            StringBuilder sb = new StringBuilder();
//            char d = ',';
//            sb.Append(this.TradingAccount);
//            sb.Append(d);
//            sb.Append(this.IsLogin.ToString());
//            sb.Append(d);
//            sb.Append(this.IPAddress);
//            sb.Append(d);
//            sb.Append(this.FrontID);
//            sb.Append(d);
//            sb.Append(this.ClientID);
//            sb.Append(d);
//            sb.Append(this.HardwarCode);
//            sb.Append(d);
//            sb.Append(this.ProductInfo);
//            return sb.ToString();
//        }

//        public override void ContentDeserialize(string contentstr)
//        {
//            string[] rec = contentstr.Split(',');
//            this.TradingAccount = rec[0];
//            this.IsLogin = bool.Parse(rec[1]);
//            this.IPAddress = rec[2];
//            this.FrontID = rec[3];
//            this.ClientID = rec[4];
//            this.HardwarCode = rec[5];
//            this.ProductInfo = rec[6];
//        }


//    }
//}
