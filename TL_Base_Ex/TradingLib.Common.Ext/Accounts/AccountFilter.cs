using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 帐户过滤 用于按照一定的搜索语法匹配出对应的Account
    /// </summary>
    public class AccountFilter:ObjectFilter
    {
        #region [match 匹配逻辑]
        /// <summary>
        /// 返回dictionary是否匹配当前ObjectFilter
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public override bool Match(object obj)
        {
            IAccount account = null;
            if (obj is IAccount)
                account = obj as IAccount;
            else
                return false;
            return InnerMatch(account);
        }

        public override object GetAttr(object obj, string attr)
        {
            IAccount account = obj as IAccount;
            if(account ==null) return null;
            string key = attr.ToUpper();
            switch (key)
            { 
                //交易帐号ID
                case "ACC":
                    return account.ID;
                //交易路由
                case "ROUTE":
                    return account.OrderRouteType.ToString();
                //代理编号
                //case "AGENT":
                //    return account.AgentCode;
                //帐户激活状态
                case "ACTIVE":
                    return account.Execute;
                //日内交易
                case "DAY":
                    return account.IntraDay;
                //占用保证金
                case "MARGIN":
                case "M":
                    return account.MoneyUsed;
                //手续费
                case "COMMISSION":
                case "C":
                    return account.Commission;
                
                //净利润
                case "PROFIT":
                case "P":
                    return account.Profit;

                //绑定的用户
                case "USER":
                    return account.UserID;
                
                //平仓盈亏
                case "REALIZEDPL":
                case "RPL":
                    return account.RealizedPL;
                default :
                    return null;
            }
        }

        
        #endregion
    }

    
}
