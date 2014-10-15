using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public static partial class MangerUtils
    {
        /// <summary>
        /// 判断Manager是否属于Root主域
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static bool RightRootDomain(this Manager mgr)
        {
            if (mgr.Type == QSEnumManagerType.ROOT || mgr.BaseManager.Type == QSEnumManagerType.ROOT)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 返回某个交易帐户的主域FK
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static int GetBaseMGR(this Manager mgr)
        {
            return mgr.mgr_fk;
        }
        /// <summary>
        /// 是否可以访问某个交易帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool RightAccessAccount(this Manager mgr, IAccount account)
        {
            //如果交易帐户属于该Manager的主MGR则有权限访问该帐户
            if (mgr.GetBaseMGR().Equals(account.Mgr_fk))
            {
                return true;
            }
            if (mgr.RightRootDomain()) return true;
            return false;
        }

    }
}
