using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class VendorUtils
    {

        /// <summary>
        /// 获得实盘帐户对象的成交通道标识
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string GetBrokerToken(this Vendor v)
        {
            if (v.Broker == null)
                return string.Empty;
            return v.Broker.Token;
        }

        /// <summary>
        /// 判断某个实盘帐户是否可用
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool AcceptEntryOrder(this Vendor v,Order o,decimal margintouse)
        {
            //这里需要考虑到净持仓,如果可以进行净持仓操作,则需要按规则下到净持仓里面,而不受保证金占用
            decimal marginused = v.CalMargin() + v.CalMarginFrozen();
            if (v.MarginLimit > 1)
            {
                //Util.Debug("Marginused:" + marginused.ToString() + " MarginToUse:" + margintouse.ToString() + " MarginLimit:" + v.MarginLimit.ToString(), QSEnumDebugLevel.WARNING);
                return marginused + margintouse < v.MarginLimit;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断Vendoer的底层通道是否正常可用
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsBrokerAvabile(this Vendor v)
        {
            if (v.Broker != null && v.Broker.IsLive)
            {
                return true;
            }
            return false;
        }

        public static decimal CalMargin(this Vendor v)
        {
            if (v.Broker != null)
                return v.Broker.CalFutMargin();
            return 0;
        }
        public static decimal CalMarginFrozen(this Vendor v)
        {
            if (v.Broker != null)
                return v.Broker.CalFutMarginFrozen();
            return 0;
        }

        public static decimal CalRealizedPL(this Vendor v)
        {
            if (v.Broker != null)
                return v.Broker.CalFutRealizedPL();
            return 0;
        }

        public static decimal CalUnRealizedPL(this Vendor v)
        {
            if (v.Broker != null)
                return v.Broker.CalFutUnRealizedPL();
            return 0;
        }
    }
}
