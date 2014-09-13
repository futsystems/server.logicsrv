//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;


//namespace TradingLib.MySql
//{
//    /**
//     * milfut比赛帐号前缀
//     * 1.晋级赛758xxxx1
//     * 2.晋级赛768xxxx1
//     * 3.期货交易帐号 858xxxx1
//     * */
//    public static class SqlGlobal
//    {

//        public static string DBAddress = "db_dev.huiky.com";
//        public static string UserName = "dbroot";
//        public static string PassWord = "hky$2014#";
//        public static int Port = 4406;
//        public static string DBName = "hky_system";


//        public static void InitDBSetting(string dbaddress, string username, string passwd, string dbname, int port)
//        {
//            DBAddress = dbaddress;
//            UserName = username;
//            PassWord = passwd;
//            DBName = dbname;
//            Port = port;
//        }
//        public static void InitAccountConfig(int dealerprefix = 170, int dealernum = 3, int loaneeprefix = 168, int loaneenum = 3)
//        {
//            DealerPrefix = dealerprefix;
//            DealerAccountNum = dealernum;

//            LoaneePrefix = loaneeprefix;
//            LoaneeAccountNum = loaneenum;
//        }

//        /// <summary>
//        /// 交易员帐号前缀
//        /// </summary>
//        public static int DealerPrefix = 170;
//        /// <summary>
//        /// 交易员帐号位数
//        /// </summary>
//        public static int DealerAccountNum = 3;
//        /// <summary>
//        /// 配子帐号前缀
//        /// </summary>
//        public static int LoaneePrefix = 168;
//        /// <summary>
//        /// 配资本帐号位数
//        /// </summary>
//        public static int LoaneeAccountNum = 3;

//        /// <summary>
//        /// 季赛比赛帐号前缀
//        /// </summary>
//        public static int SeasonPrefix = 768;

//        public static int SeasonAccountNum = 5;

//        /// <summary>
//        /// 模拟帐号前缀
//        /// </summary>
//        public static int SimPrefix = 688;
//        /// <summary>
//        /// 模拟帐号长度
//        /// </summary>
//        public static int SimAccountNum = 7;

//        ///// <summary>
//        ///// 获得某类帐户的前缀
//        ///// </summary>
//        ///// <param name="category"></param>
//        ///// <returns></returns>
//        //public static int GetAccountPrefix(QSEnumAccountCategory category = QSEnumAccountCategory.DEALER)
//        //{
//        //    switch (category)
//        //    { 
//        //        case QSEnumAccountCategory.DEALER:
//        //            return DealerPrefix;
//        //        case QSEnumAccountCategory.LOANEE:
//        //            return LoaneePrefix;
//        //        case QSEnumAccountCategory.SIMULATION:
//        //            return SimPrefix;
//        //        default :
//        //            return 999;
//        //    }
//        //}

//        ///// <summary>
//        ///// 获得某种帐户类别的乘幂数
//        ///// </summary>
//        ///// <param name="category"></param>
//        ///// <returns></returns>
//        //public static int GetAccountPowerLength(QSEnumAccountCategory category = QSEnumAccountCategory.DEALER)
//        //{
//        //    int prefix = GetAccountPrefix(category);
//        //    int prefixlen = prefix.ToString().Length;
//        //    switch (category)
//        //    { 
//        //        case QSEnumAccountCategory.DEALER:
//        //            return DealerAccountNum - prefixlen;
//        //        case QSEnumAccountCategory.LOANEE:
//        //            return LoaneeAccountNum - prefixlen;
//        //        case QSEnumAccountCategory.SIMULATION:
//        //            return SimAccountNum - prefixlen;
//        //        default:
//        //            return 4;
//        //    }
//        //}
           
//    }
//}
