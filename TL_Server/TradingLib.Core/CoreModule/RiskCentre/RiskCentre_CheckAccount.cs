using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class RiskCentre
    {
        /// <summary>
        /// 执行账户的 账户规则检查
        /// </summary>
        /// <param name="a"></param>
        void CheckAccountRule(IAccount a)
        {
            try
            {
                string msg = string.Empty;
                if (!CheckAccountRule(a.ID,out msg) && !string.IsNullOrEmpty(msg))
                {
                    logger.Warn(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Check Account Rule Error:{0}", ex.ToString()));
            }
        }
        
    }
}
