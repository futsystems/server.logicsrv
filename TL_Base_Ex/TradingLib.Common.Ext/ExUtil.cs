using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class ExUtil
    {
        public static string SectionHeader(string title)
        {
            return (ExComConst.SectionPrefix + " " + title + " ").PadRight(ExComConst.SectionNum, ExComConst.SectionChar) + ExComConst.Line;
        }

        public static string ObjectStr(object obj)
        {
            return obj == null ? "null" : obj.ToString();
        }

        //public static string ClientInfo2Str(IClientInfo info)
        //{
        //    return info.Address.PadRight(40, ' ') + ObjectStr(info.FrontID).PadRight(10, ' ') + info.HeartBeat.ToString() + " " + ObjectStr(info.AccountID) + ExComConst.Line;
        //}
        /// <summary>
        /// 检查帐户是否是实盘帐户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsRealAccount(IAccount account)
        {
            if (account.OrderRouteType == QSEnumOrderTransferType.LIVE)
                return true;
            //实盘路由 或者 配资客户 均作为实盘处理
            switch (account.Category)
            { 
                case QSEnumAccountCategory.REAL:
                    return true;
                default:
                    return false;
            }
        }

        public static string GenAccountPass()
        {
            Random rnd = new Random();
            int intpass = rnd.Next(100000, 999999);
            string pass = intpass.ToString();

            return pass;
            
        }
    }
}
