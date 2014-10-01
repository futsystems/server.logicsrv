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


        #region 【帐户检查】
        /// <summary>
        /// 执行账户的 账户规则检查
        /// </summary>
        /// <param name="a"></param>
        public void CheckAccount(IAccount a)
        {
            try
            {
                if (!a.RuleItemLoaded)
                    LoadRuleItem(a);
                string msg = string.Empty;
                if (!a.CheckAccount(out msg) && msg != string.Empty)//账户检查不通过并且有返回消息则我们打印消息
                {
                    debug(msg, QSEnumDebugLevel.INFO);
                }
            }
            catch (Exception ex)
            {
                string s = PROGRAME + ":checkAccount error:" + ex.ToString();
                debug(s, QSEnumDebugLevel.ERROR);
                _othercheklog.GotDebug(s);
            }
        }


        /// <summary>
        /// 检查委托对应的交易帐号是否存在并有效
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool TrckerOrderAccount(Order o,out IAccount account)
        {
            account = _clearcentre[o.Account];
            if (account == null) return false;
            return true;
        }
        #endregion
        
    }
}
