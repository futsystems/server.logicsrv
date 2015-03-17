using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public class ClearCentrePass:ClearCentre
    {
        /// <summary>
        /// 主帐户监控所用的清算中心 在恢复日内交易数据时 不用从数据库恢复交易记录
        /// </summary>
        protected override void Restore()
        {
            
        }
    }
}
