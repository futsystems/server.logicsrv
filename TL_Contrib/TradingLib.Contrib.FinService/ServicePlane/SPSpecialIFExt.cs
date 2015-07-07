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
    /// 覆写相关函数 实现对应的逻辑
    /// </summary>
    public class SPSpecialIFExt : ServicePlanBase
    {
        /// <summary>
        /// 单手收费
        /// </summary>
        [ArgumentAttribute("Charge","每手收费",EnumArgumentType.DECIMAL,true, 200, 100)]
        public ArgumentPair Charge { get; set; }

        /// <summary>
        /// 亏损回合收费
        /// </summary>
        //[ArgumentAttribute("LossCharge","亏损收费",EnumArgumentType.DECIMAL,true, 100, 50)]
        //public ArgumentPair LossCharge { get; set; }

        /// <summary>
        /// 每手保证金
        /// </summary>
        [ArgumentAttribute("MarginPerLot", "每手保证金", EnumArgumentType.DECIMAL, true, 5000, 5000)]
        public ArgumentPair MarginPerLot { get; set; }

        /// <summary>
        /// 每手保证金 强平线
        /// </summary>
        [ArgumentAttribute("MarginPerLotStop", "单手强平线", EnumArgumentType.DECIMAL, true, 800, 800)]
        public ArgumentPair MarginPerLotStop { get; set; }


        /// <summary>
        /// 保证金 起始点
        /// </summary>
        [ArgumentAttribute("MarginPerLotStart", "保证金起步值", EnumArgumentType.DECIMAL, true, 2000, 2000)]
        public ArgumentPair MarginPerLotStart { get; set; }






        public SPSpecialIFExt()
        {
            SPNAME = "股指专配(增强版)";
            _chargetype = EnumFeeChargeType.BYRound;//按交易回合计算费用
            _collecttype = EnumFeeCollectType.CollectInTrading;//在交易过程中直接收取

        }

        public override void OnInit()
        {
            codelist.Add("IF");
            codelist.Add("IH");
            codelist.Add("IC");
        }
        public override bool ValidArguments(JsonWrapperArgument[] args, out string error)
        {
            error = "参数异常,请检查";

            Dictionary<string, Argument> argmap = new Dictionary<string, Argument>();
            foreach (var arg in args)
            {
                argmap.Add(arg.ArgName, new Argument(arg.ArgName, arg.ArgValue, (EnumArgumentType)Enum.Parse(typeof(EnumArgumentType), arg.ArgType)));
            }

            Argument wincharge = argmap["Charge"];
            if (wincharge != null)
            {
                decimal winchargevalue = wincharge.AsDecimal();
                if (winchargevalue > 1000)
                {
                    error = "手续费不能超过1000";
                    return false;
                }
                decimal winchargevalue_agent = agentargmap["Charge"].AsDecimal();
                if (winchargevalue < winchargevalue_agent)
                {
                    error = "盈利手续费不能低于代理值:"+Util.FormatDecimal(winchargevalue_agent);
                    return false;
                }
            }
            Argument marginperlot = argmap["MarginPerLot"];
            if (marginperlot != null)
            {
                decimal marginperlotvalue = marginperlot.AsDecimal();
                decimal marginperlotvalue_agent = agentargmap["MarginPerLot"].AsDecimal();
                if (marginperlotvalue < marginperlotvalue_agent)
                {
                    error = "每手保证金不能低于代理值:" + Util.FormatDecimal(marginperlotvalue_agent);
                    return false;
                }
            }

            Argument marginperlotstop = argmap["MarginPerLotStop"];
            if (marginperlotstop != null)
            {
                decimal marginperlotstopvalue = marginperlotstop.AsDecimal();
                decimal marginperlotstopvalue_agent = agentargmap["MarginPerLotStop"].AsDecimal();
                if (marginperlotstopvalue < marginperlotstopvalue_agent)
                { 
                    error = "止损线不能低于代理值:"+Util.FormatDecimal(marginperlotstopvalue_agent);
                    return false;
                }
            }

            Argument marginperlotstart = argmap["MarginPerLotStart"];
            if (marginperlotstart != null)
            {
                decimal marginperlotstartvalue = marginperlotstart.AsDecimal();
                decimal marginperlotstartvalue_agent = agentargmap["MarginPerLotStart"].AsDecimal();
                if (marginperlotstartvalue < marginperlotstartvalue_agent)
                {
                    error = "保证金起步值不能低于代理值:" + Util.FormatDecimal(marginperlotstartvalue_agent);
                    return false;
                }
            }


            return true;
        }
        /// <summary>
        /// 调整手续费
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public override decimal OnAdjustCommission(Trade t, PositionRound pr)
        {
            decimal commission = t.Commission;
            //开仓
            if (t.IsEntryPosition)
            {
                commission = t.UnsignedSize * this.Charge.AccountArgument.AsDecimal() / 2;
            }
            //平仓
            else
            {
                commission = t.UnsignedSize * this.Charge.AccountArgument.AsDecimal() / 2;
            }
            return commission;
        }

        /// <summary>
        /// 响应成交事件 每比成交进行收费计算
        /// </summary>
        /// <param name="t"></param>
        public override void OnTrade(Trade t)
        {
            decimal totalfee = t.UnsignedSize * this.Charge.AccountArgument.AsDecimal()/2;
            decimal agentfee = t.UnsignedSize * this.Charge.AgentArgument.AsDecimal()/2;
           

            //进行直客收费记录
            string comment = SPNAME + " 成交编号:" + t.TradeID;

            //计算代理收费记录
            AgentCommissionDel func = (Manager agent, Manager parent) =>
            {
                decimal fee = 0;
                //代理的收费 - 代理的父代理的收费
                decimal diff = FinTracker.ArgumentTracker.GetAgentArgument(agent.mgr_fk, this.ServicePlanFK, this.Charge.AccountArgument.Name).AsDecimal() - FinTracker.ArgumentTracker.GetAgentArgument(parent.mgr_fk, this.ServicePlanFK, this.Charge.AccountArgument.Name).AsDecimal();
                return t.UnsignedSize * diff / 2;
            };
            FeeCharge(totalfee, agentfee, func, comment);
        }

        /// <summary>
        /// 风控规则
        /// 股指当平均每手资金降低到800元时候执行强平
        /// </summary>
        public override bool  RiskCheck(out string msg)
        {
            msg = string.Empty;
            //当前资金
            decimal nowequity = this.Account.NowEquity;
            int totalsize = 0;
            foreach (Position p in this.Account.Positions)
            {
                totalsize += p.UnsignedSize;
            }
            //有持仓
            if (totalsize > 0)
            {
                //当每手资金小于设定的强平金额时执行强平
                decimal marginperplot = nowequity / totalsize;
                if (marginperplot <= MarginPerLotStop.AccountArgument.AsDecimal())
                {
                    Util.Debug("SPSpecialIFExt 触发强平  account:" + this.Account.ID + " now equity:" + nowequity.ToString() + " totalsize:" + totalsize.ToString() + " marginperlot:" + marginperplot + " stopline:" + MarginPerLotStop.AccountArgument.Value);
                    msg = "大富翁股指专配(增强版)";
                    return false;
                }
            }
            return true;
        }


        #region 交易业务逻辑部分

        bool InWhiteList(string code)
        {
            return codelist.Contains(code);
        }
        List<string> codelist = new List<string>();
        /// <summary>
        /// 检查合约交易权限
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            //LibUtil.Debug("xxxxxxxxxxxxxxxxxxxx检查是否可以交易合约:" + symbol.Symbol);
            msg = string.Empty;
            if (InWhiteList(symbol.SecurityFamily.Code))
            {
                return true;
            }
            else
            {
                msg = "配资服务[" + this.SPNAME + "]只能交易品种:"+string.Join(" ",codelist.ToArray());
                return false;
            }
        }

        /// <summary>
        /// 保证金检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool CanTakeOrder(Order o, out string msg)
        {
            msg = string.Empty;
            if (!InWhiteList(o.oSymbol.SecurityFamily.Code))
            {
                msg = "不允许交易股指意外的品种";
                return false;
            }

            bool isentry = o.IsEntryPosition;
            if (isentry)
            {
                bool positoinside = o.PositionSide;
                //持仓数量
                int poszie = this.Account.GetPositionsHold().Where(pos => InWhiteList(pos.oSymbol.SecurityFamily.Code)).Sum(pos => pos.UnsignedSize);
                
                decimal nowequity = this.Account.NowEquity;

                //待开手数
                int frozensize = this.Account.Orders.Where(tmp => InWhiteList(tmp.oSymbol.SecurityFamily.Code)).Where(od => od.IsEntryPosition && od.IsPending()).Sum(od => od.UnsignedSize);

                decimal marginperlot = this.MarginPerLot.AccountArgument.AsDecimal();
                decimal marginperlotstart = this.MarginPerLotStart.AccountArgument.AsDecimal();

                int totalsize = 0;
                Util.Debug("nowequity:" + nowequity.ToString() + " marginperlot:" + marginperlot.ToString() + " marginpperlotstop:" + marginperlotstart.ToString());

                if (nowequity < marginperlot)
                {
                    if (nowequity >= marginperlotstart)
                        totalsize = 1;
                }
                else
                {
                    totalsize = (int)(nowequity / marginperlot) + 1;
                }

                //如果持仓数量+当前委托数量 超过总数量 则拒绝
                if (poszie + o.UnsignedSize + frozensize > totalsize)
                {
                    int cansize = totalsize - poszie >= 0 ? (totalsize - poszie) : 0;
                    Util.Debug("pos size:" + poszie.ToString() + " ordersize:" + o.UnsignedSize.ToString() + " frozensize:" + frozensize.ToString() + " totalsize:" + totalsize.ToString());

                    msg = "保证金不足";
                    return false;
                }
                return true;
            }
            else//平仓
            {
                return true;
            }
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
            return 0;
        }

        /// <summary>
        /// 计算通过配资服务后某个合约的可开手数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public override int CanOpenSize(Symbol symbol)
        {
            decimal marginperlot = this.MarginPerLot.AccountArgument.AsDecimal();
            decimal nowequity = this.Account.NowEquity;

            int totalsize = (int)(nowequity / marginperlot) + 1;
            //计算股指的所有持仓数量
            int possize = this.Account.GetPositionsHold().Where(pos => InWhiteList(pos.oSymbol.SecurityFamily.Code)).Sum(pos => pos.UnsignedSize);

            int canopen= totalsize - possize;
            return canopen >= 0 ? canopen : 0;//为和会计算出来小于0，应为当开仓时计算出来2手，并开了2手，亏钱后，可开就变成了1手，则此时的可开会变成负数
        }
        #endregion
    }
}
