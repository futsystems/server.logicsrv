using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.FinService
{
    /// <summary>
    /// 收取利息的配资计划
    /// 业务逻辑如下:
    /// 客户出资1000元，按相应的配资比例融出资金10000元 帐户累计可使用资金11000元
    /// 客户入金时自动设定配资额度,亏损到强平线 执行强平,盘中不调整额度，盘后也不调整额度
    /// 覆写相关函数 实现对应的逻辑
    /// </summary>
    public class SPCommonissionFixMargin : ServicePlanBase
    {

        /// <summary>
        /// 手续费加收值
        /// </summary>
        [ArgumentAttribute("CommissionOpenMarkup", "开仓加收值", EnumArgumentType.DECIMAL, true, 0.1, 0.1)]
        public ArgumentPair CommissionOpenMarkup { get; set; }

        [ArgumentAttribute("CommissionCloseMarkup", "平仓加收值", EnumArgumentType.DECIMAL, true, 0.1, 0.1)]
        public ArgumentPair CommissionCloseMarkup { get; set; }

        [ArgumentAttribute("CommissionMarkupPect", "按百分比加收", EnumArgumentType.BOOLEAN, true, true, true)]
        public ArgumentPair CommissionMarginPect { get; set; }

        [ArgumentAttribute("CommissionMarkupAbsolute", "直接收取", EnumArgumentType.BOOLEAN, true, false, false)]
        public ArgumentPair CommissionMarginAbsolute { get; set; }
        /// <summary>
        /// 配资比例
        /// </summary>
        [ArgumentAttribute("FinAmount", "配资额度", EnumArgumentType.DECIMAL, true, 0, 0)]
        public ArgumentPair FinAmount { get; set; }


        /// <summary>
        /// 强平权益 当客户权益低于该值时 执行强平
        /// 此比例 为安全本金的比例
        /// </summary>
        [ArgumentAttribute("StopEquity", "强平权益", EnumArgumentType.DECIMAL, true, 10000,10000)]
        public ArgumentPair StopEquity { get; set; }


        public SPCommonissionFixMargin()
        {
            SPNAME = "手续费加收(固定配资额度)";
            _chargetype = EnumFeeChargeType.BYTrade;//按时间收取
            _collecttype = EnumFeeCollectType.CollectInTrading;//在交易过程中直接收取
        }

        public override void OnInit()
        { 
        
        }


        /// <summary>
        /// 结算前执行费用计算 并形成扣费记录
        /// </summary>
        public override void OnSettle()
        {

        }

        /// <summary>
        /// 计算手续费加收值
        /// </summary>
        /// <param name="ispect"></param>
        /// <param name="markup"></param>
        /// <param name="basecommission"></param>
        /// <returns></returns>
        decimal GetCommission(bool ispect, decimal markup, decimal basecommission,bool absolute)
        {

            if (ispect)
            {
                return absolute ? (basecommission * markup):(basecommission + basecommission * markup);
            }
            else
            {
                return absolute?markup:(basecommission +markup);
            }
        }


        /// <summary>
        /// 调整手续费
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public override decimal OnAdjustCommission(Trade t, PositionRound pr)
        {
            //按标准计算公式得到的手续费
            decimal commission = t.Commission;

            bool ispect = this.CommissionMarginPect.AccountArgument.AsBool();
            bool isabsolute = this.CommissionMarginAbsolute.AccountArgument.AsBool();
            decimal markupvalue = t.IsEntryPosition?this.CommissionOpenMarkup.AccountArgument.AsDecimal():this.CommissionCloseMarkup.AccountArgument.AsDecimal();
            decimal accountcommission = GetCommission(ispect, markupvalue, commission,isabsolute);

            bool agentispect = this.CommissionMarginPect.AgentArgument.AsBool();
            bool agentisabsolute = this.CommissionMarginAbsolute.AgentArgument.AsBool();
            decimal agentmarkupvalue = t.IsEntryPosition ? this.CommissionOpenMarkup.AgentArgument.AsDecimal() : this.CommissionCloseMarkup.AgentArgument.AsDecimal();
            decimal agentcommission = GetCommission(agentispect, agentmarkupvalue, commission, agentisabsolute);
            
            decimal totalfee = accountcommission - commission;
            decimal agentfee = agentcommission - commission;
            //Util.Debug("agent  perct:" + agentispect.ToString() + " absolut:" + agentisabsolute.ToString() + " agentmarkup:" + agentmarkupvalue.ToString() + " agentcommission:" + agentcommission.ToString());
            //Util.Debug("account perct:" + ispect.ToString() + " absolute:" + isabsolute.ToString() + " markup:" + markupvalue.ToString() + " commission:" + accountcommission.ToString());
            //进行收费记录
            string comment = SPNAME + " Seq:" + t.TradeID + " Time:" + t.GetDateTime().ToString();
            //计算代理收费记录
            AgentCommissionDel func = (Manager agent, Manager parent) =>
            {
                bool agentpect = FinTracker.ArgumentTracker.GetAgentArgument(agent.mgr_fk, this.ServicePlanFK, this.CommissionMarginPect.AgentArgument.Name).AsBool();
                decimal  agentmarkup = FinTracker.ArgumentTracker.GetAgentArgument(agent.mgr_fk, this.ServicePlanFK,t.IsEntryPosition?this.CommissionOpenMarkup.AgentArgument.Name:this.CommissionCloseMarkup.AgentArgument.Name).AsDecimal();
                bool agentabsolute = FinTracker.ArgumentTracker.GetAgentArgument(agent.mgr_fk, this.ServicePlanFK, this.CommissionMarginAbsolute.AgentArgument.Name).AsBool();

                bool pagentpect = FinTracker.ArgumentTracker.GetAgentArgument(parent.mgr_fk, this.ServicePlanFK, this.CommissionMarginPect.AgentArgument.Name).AsBool();
                decimal pagentmarkup = FinTracker.ArgumentTracker.GetAgentArgument(parent.mgr_fk, this.ServicePlanFK, t.IsEntryPosition ? this.CommissionOpenMarkup.AgentArgument.Name : this.CommissionCloseMarkup.AgentArgument.Name).AsDecimal();
                bool pagentabsolute = FinTracker.ArgumentTracker.GetAgentArgument(parent.mgr_fk, this.ServicePlanFK, this.CommissionMarginAbsolute.AgentArgument.Name).AsBool();
                return GetCommission(agentpect, agentmarkup, commission, agentabsolute) - GetCommission(pagentpect, pagentmarkup, commission, agentabsolute);
            };
            //执行收费
            FeeCharge(totalfee, agentfee, func, comment);

            return accountcommission;
        }


        /// <summary>
        /// 执行风控规则
        /// 自由资金小于配资金额的2%执行强平
        /// </summary>
        public override bool RiskCheck(out string msg)
        {
            msg = string.Empty;
            //没有持仓直接返回
            if (!this.Account.GetAnyPosition()) return true;

            //当前权益
            decimal nowequity = this.Account.NowEquity;

            decimal stopmargin = this.StopEquity.AccountArgument.AsDecimal();
            //如果当前资金只有  低于配资金额的2% 时候触发强平信号
            if (nowequity <= stopmargin)
            {
                Util.Debug(string.Format("帐户:{0} 当前权益:{1} 配资额度为:{2}  强平权益额度为:{3} 不满足本金要求,执行强平并冻结交易帐户", this.Account.ID, nowequity, this.FinAmount.AccountArgument.AsDecimal(), stopmargin));
                msg = "手续费,自有资金小于最低保证金";
                return false;
            }
            return true;
        }


        #region 交易业务逻辑部分

        /// <summary>
        /// 检查合约交易权限
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            //可以交易所有合约
            msg = string.Empty;
            return true;
        }

        /// <summary>
        /// 保证金检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool CanTakeOrder(Order o, out string msg)
        {
            //使用传统的保证金计算方式计算
            msg = string.Empty;

            //如果是平仓委托 则直接返回
            if (!o.IsEntryPosition) return true;

            //获得某个帐户交易某个合约的可用资金
            decimal avabile = GetFundAvabile(o.oSymbol);

            //可用资金大于需求资金则可以接受该委托
            decimal required = this.Account.CalOrderFundRequired(o, 0);

            Util.Debug("SPCommonissionBonus Fundavabile:" + avabile.ToString() + " Required:" + required + " account avabile fund:" + this.Account.AvabileFunds.ToString());
            if (required > avabile)
            {
                msg = "资金不足";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得帐户某个合约的可用资金
        /// 在进行保证金检查时需要查询某个合约的可用资金
        /// 在业务逻辑覆写时 通过服务对应的结构对外暴露
        /// 然后在account主逻辑中进行调用
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public override decimal GetFundAvabile(Symbol symbol)
        {
            //帐户当前可用资金即为所有额度， 在帐户可用额度计算时 加上了配资扩展的额度 见GetFinAmountAvabile()
            return this.Account.AvabileFunds;
        }

        /// <summary>
        /// 获得配资额度 传递到核心帐户对象 核心帐户可用资金 = 帐户当前权益 + 配资服务所提供的配资金额
        /// </summary>
        /// <returns></returns>
        public override decimal GetFinAmountAvabile()
        {
            return this.FinAmount.AccountArgument.AsDecimal();
        }

        /// <summary>
        /// 计算通过配资服务后某个合约的可开手数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public override int CanOpenSize(Symbol symbol)
        {

            decimal price = TLCtxHelper.CmdUtils.GetAvabilePrice(symbol.Symbol);

            decimal fundperlot = Calc.CalFundRequired(symbol, price, 1);

            decimal avabilefund = GetFundAvabile(symbol);

            Util.Debug("QryCanOpenSize Fundavablie:" + avabilefund.ToString() + " Symbol:" + symbol.Symbol + " Price:" + price.ToString() + " Fundperlot:" + fundperlot.ToString());
            return (int)(avabilefund / fundperlot);
        }
        #endregion
    }
}
