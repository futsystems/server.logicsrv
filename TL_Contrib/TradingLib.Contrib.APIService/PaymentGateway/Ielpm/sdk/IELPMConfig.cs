//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Web;
//using System.Web.Configuration;

//namespace ielpm_merchant_web_demo.com.ielpm.merchant.web.sdk
//{
//    public class IELPMConfig
//    {
//        private static Configuration config = WebConfigurationManager.OpenWebConfiguration("~");


//        private static string payUrl = config.AppSettings.Settings["ielpm.payUrl"].Value;

//        public static string PayUrl
//        {
//            get { return IELPMConfig.payUrl; }
//            set { IELPMConfig.payUrl = value; }
//        }
//        private static string refundUrl = config.AppSettings.Settings["ielpm.refundUrl"].Value;

//        public static string RefundUrl
//        {
//            get { return IELPMConfig.refundUrl; }
//            set { IELPMConfig.refundUrl = value; }
//        }
//        private static string queryUrl = config.AppSettings.Settings["ielpm.queryUrl"].Value;

//        public static string QueryUrl
//        {
//            get { return IELPMConfig.queryUrl; }
//            set { IELPMConfig.queryUrl = value; }
//        }
//        private static string returnUrl = config.AppSettings.Settings["mer.returnUrl"].Value;

//        public static string ReturnUrl
//        {
//            get { return IELPMConfig.returnUrl; }
//            set { IELPMConfig.returnUrl = value; }
//        }
//        private static string notifyUrl = config.AppSettings.Settings["mer.notifyUrl"].Value;

//        public static string NotifyUrl
//        {
//            get { return IELPMConfig.notifyUrl; }
//            set { IELPMConfig.notifyUrl = value; }
//        }
//        private static string merchantNo = config.AppSettings.Settings["ielpm.merchantNo"].Value;

//        public static string MerchantNo
//        {
//            get { return IELPMConfig.merchantNo; }
//            set { IELPMConfig.merchantNo = value; }
//        }
//        private static string version = config.AppSettings.Settings["ielpm.version"].Value;

//        public static string Version
//        {
//            get { return IELPMConfig.version; }
//            set { IELPMConfig.version = value; }
//        }
//        private static string channelNo = config.AppSettings.Settings["ielpm.channelNo"].Value;

//        public static string ChannelNo
//        {
//            get { return IELPMConfig.channelNo; }
//            set { IELPMConfig.channelNo = value; }
//        }

//        private static string signCertPath = config.AppSettings.Settings["ielpm.signCert.path"].Value;

//        public static string SignCertPath
//        {
//            get { return IELPMConfig.signCertPath; }
//            set { IELPMConfig.signCertPath = value; }
//        }

//        private static string signCertPwd = config.AppSettings.Settings["ielpm.signCert.pwd"].Value;

//        public static string SignCertPwd
//        {
//            get { return IELPMConfig.signCertPwd; }
//            set { IELPMConfig.signCertPwd = value; }
//        }

//        private static string signCertType = config.AppSettings.Settings["ielpm.signCert.type"].Value;

//        public static string SignCertType
//        {
//            get { return IELPMConfig.signCertType; }
//            set { IELPMConfig.signCertType = value; }
//        }

//        private static string publicCertPath = config.AppSettings.Settings["ielpm.publicCert.path"].Value;

//        public static string PublicCertPath
//        {
//            get { return IELPMConfig.publicCertPath; }
//            set { IELPMConfig.publicCertPath = value; }
//        }
        

//    }
//}