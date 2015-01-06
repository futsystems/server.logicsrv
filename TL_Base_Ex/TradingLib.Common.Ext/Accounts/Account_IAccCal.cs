using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        /// <summary>
        /// 计算某个委托所要占用的资金
        /// 正常情况下按照合约对应的合约乘数和当前市值和保证金比例去计算对应的资金需求
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual decimal CalOrderFundRequired(Order o, decimal defaultvalue = 0)
        {
            decimal price = TLCtxHelper.Ctx.MessageExchange.GetAvabilePrice(o.Symbol);
            return o.CalFundRequired(price, defaultvalue);
        }

        /// <summary>
        /// 获得某个合约的可交易资金
        /// 合约可用涉及到自身帐户的资金数量和对应服务的所提供的相关扩展
        /// 比如配资服务 客户自身资金 + 配资服务的可用资金 这里可以统一IService接口来对此类资金的调整进行抽象
        /// 这里注意多种服务的资金约束冲突
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual decimal GetFundAvabile(Symbol symbol)
        {
            //常规状态下合约的可用资金即为帐户的可用资金
            if (symbol.SecurityType == SecurityType.FUT)
                return this.AvabileFunds;
            if (symbol.SecurityType == SecurityType.OPT)
                return this.AvabileFunds;
            if (symbol.SecurityType == SecurityType.INNOV)
                return this.AvabileFunds;
            else
                return 0;
        }

    }
}
