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
        /// 这里的计算与单纯计算某个委托需要占用的保证金有所不同，这里需要按照
        /// 保证金计算算法 试算该委托下达后所增加的保证金占用 包含单向大边的处理
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual decimal CalOrderFundRequired(Order o, decimal defaultvalue = 0)
        {
            //需要判断是否启用单向大边
            if (!this.GetParamSideMargin())
            {
                //decimal price = TLCtxHelper.CmdUtils.GetAvabilePrice(o.Symbol);
                return this.CalOrderMarginFrozen(o);
            }
            else
            {
                decimal marginfrozennow = this.CalFutMarginSet().Sum(ms => ms.MarginFrozen);
                //将当前委托纳入待成交委托集，然后按单向大边规则计算冻结保证金
                decimal marginfrozenwill = this.CalFutMarginSet(o).Sum(ms => ms.MarginFrozen);
                return marginfrozenwill - marginfrozennow;//纳入开仓委托的单向大边冻结保证金 - 当前冻结保证金 为该委托所需冻结保证金
            }
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
            else
                return 0;
        }

    }
}
