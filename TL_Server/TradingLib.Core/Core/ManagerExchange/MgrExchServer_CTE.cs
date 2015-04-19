using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        /// <summary>
        /// 显示某个委托情况
        /// </summary>
        /// <param name="id"></param>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "pmgr", "pmgr - print manger information", "")]
        public string CTE_Manager()
        {

            StringBuilder sb = new StringBuilder();
            foreach (CustInfoEx cst in customerExInfoMap.Values)
            {
                sb.Append(cst.ToString(true) + System.Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
