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
    public class SPHunanIF : ServicePlanBase
    {

        /// <summary>
        /// 回合收费
        /// </summary>
        [ArgumentAttribute("RoundCharge", "每个回合收费", EnumArgumentType.DECIMAL, true,150,50)]
        public ArgumentPair RoundCharge { get; set; }

        /// <summary>
        /// 配资比例
        /// </summary>
        [ArgumentAttribute("FinAmount", "配资额度", EnumArgumentType.DECIMAL, false, 0, 0)]
        public ArgumentPair FinAmount { get; set; }


        /// <summary>
        /// 配资比例
        /// </summary>
        [ArgumentAttribute("FinLever", "配资比例", EnumArgumentType.INT,false,25,25)]
        public ArgumentPair FinLever { get; set; }
        

        /// <summary>
        /// 强平比例 配资金额的2%
        /// </summary>
        [ArgumentAttribute("StopPect", "强平比例", EnumArgumentType.DECIMAL, false,0.02,0.02)]
        public ArgumentPair StopPect { get; set; }




        public SPHunanIF()
        {
            SPNAME = "湖南股指专配";
            _chargetype = EnumFeeChargeType.BYRound;//按交易回合计算费用
            _collecttype = EnumFeeCollectType.CollectInTrading;//在交易过程中直接收取

        }

        /// <summary>
        /// 调整手续费
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public override decimal OnAdjustCommission(Trade t, IPositionRound pr)
        {
            decimal commission = t.Commission;
            //平仓操作才计算手续费
            if (!t.IsEntryPosition)//平仓
            {
                if (pr.IsClosed)
                {
                    decimal profit = pr.Profit;//该次交易回合的盈利
                    int size = pr.Size;//该次交易回合的成交手数

                    //注:后期这里需要该进程每个帐号按其代理设定的费用标准进行收费
                    commission = size * this.RoundCharge.AccountArgument.AsDecimal();
                }
                else
                {
                    commission = 0;
                }
            }
            else
            {
                commission = 0;
            }
            return commission;
        }


        public override void OnTrade(Trade t)
        {

        }


        /// <summary>
        /// 响应持仓回合事件
        /// 当一次开仓 平仓结束后触发该调用
        /// </summary>
        /// <param name="round"></param>
        public override void OnRound(IPositionRound round)
        {
            if (!round.oSymbol.SecurityFamily.Code.Equals("IF"))
            {
                return;
            }
            decimal totalfee = 0;
            decimal agentfee = 0;
            
            totalfee = round.Size * this.RoundCharge.AccountArgument.AsDecimal();
            agentfee = round.Size * this.RoundCharge.AgentArgument.AsDecimal();

            //进行直客收费记录
            string comment = SPNAME + " 平仓时间:" + Util.ToTLDateTime(round.ExitTime).ToString();

            //计算代理收费记录
            AgentCommissionDel func = (Manager agent, Manager parent) =>
            {
                decimal fee = 0;
                //代理的收费 - 代理的父代理的收费
                decimal diff = FinTracker.ArgumentTracker.GetAgentArgument(agent.mgr_fk, this.ServicePlanFK, this.RoundCharge.AccountArgument.Name).AsDecimal() - FinTracker.ArgumentTracker.GetAgentArgument(parent.mgr_fk, this.ServicePlanFK, this.RoundCharge.AccountArgument.Name).AsDecimal();
                fee = round.Size * diff;
                return fee;
            };
            FeeCharge(totalfee, agentfee, func, comment);
        }



        /// <summary>
        /// 执行风控规则
        /// 自由资金小于配资金额的2%执行强平
        /// </summary>
        public override void CheckAccount()
        {
            //没有持仓直接返回
            if (!this.Account.GetAnyPosition()) return;
            //检查当前配资额度,如果配资额度<=0表明没有配资额度，不用检查，即便使用帐户也是自由资金
            decimal finamount = this.FinAmount.AccountArgument.AsDecimal();
            if (finamount <= 0) return;

            //当前权益
            decimal nowequity = this.Account.NowEquity;
            //计算当前权益占配资资金的比例
            decimal pect = nowequity / this.FinAmount.AccountArgument.AsDecimal();
            decimal stoppect = this.StopPect.AccountArgument.AsDecimal();
            //Util.Debug("finamount:" + finamount.ToString() + " nowequity:" + nowequity.ToString() + " pect:" + pect.ToString() + " stoppect:" + stoppect.ToString());
                
            //如果当前资金只有  低于配资金额的2% 时候触发强平信号
            if (pect < stoppect)
            {
                //Util.Debug("finamount:" + finamount.ToString() + " nowequity:" + nowequity.ToString() + " pect:" + pect.ToString() + " stoppect:" + stoppect.ToString());
                FireFlatPosition("湖南股指专配");       
            }
            


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
            //LibUtil.Debug("xxxxxxxxxxxxxxxxxxxx检查是否可以交易合约:" + symbol.Symbol);
            msg = string.Empty;
            if (symbol.SecurityFamily.Code.Equals("IF"))
            {
                return true;
            }
            else
            {
                msg = "只允许交易:IF股指期货";
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
            //使用传统的保证金计算方式计算
            msg = string.Empty;

            //如果是平仓委托 则直接返回
            if (!o.IsEntryPosition) return true;

            //获得对应方向的持仓
            Position pos = this.Account.GetPosition(o.symbol, o.PositionSide);

            //获得某个帐户交易某个合约的可用资金
            decimal avabile = GetFundAvabile(o.oSymbol);

            //可用资金大于需求资金则可以接受该委托
            decimal required = this.Account.CalOrderFundRequired(o,0);
            Util.Debug("SPHunanIF Fundavabile:" + avabile.ToString() + " Required:" + required +" account avabile fund:"+this.Account.AvabileFunds.ToString());
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
            return this.Account.AvabileFunds + this.FinAmount.AccountArgument.AsDecimal();
        }

        /// <summary>
        /// 计算通过配资服务后某个合约的可开手数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public override int CanOpenSize(Symbol symbol)
        {
            //decimal price = TLCtxHelper.Ctx.MessageExchange.GetAvabilePrice(symbol.Symbol);

            //decimal fundperlot = Calc.CalFundRequired(symbol, price, 1);

            //decimal avabilefund = GetFundAvabile(symbol);

            //TLCtxHelper.Debug("QryCanOpenSize Fundavablie:" + avabilefund.ToString() + " Symbol:" + symbol.Symbol + " Price:" + price.ToString() + " Fundperlot:" + fundperlot.ToString());
            //return (int)(avabilefund / fundperlot);
            return 0;
        }
        #endregion


        public override void AdjustOmCashOperation(JsonWrapperCashOperation op)
        {
            if (op.Status != QSEnumCashInOutStatus.CONFIRMED) return;//只针对产生作用的出入金进行调整

            decimal nowequity = this.Account.NowEquity;

            decimal originMargin = this.FinAmount.AccountArgument.AsDecimal() / this.FinLever.AccountArgument.AsInt();//计算原始保证金

            decimal finamount = this.FinAmount.AccountArgument.AsDecimal();
            //入金
            if (op.Operation == QSEnumCashOperation.Deposit)
            {
                //如果超过了我们需要的最低保证金额度，则自动给他调整配资额度
                if (nowequity > originMargin)
                {
                    finamount = nowequity * this.FinLever.AccountArgument.AsInt();
                }
                //如果仍然小于最低保证金额度，则不作任何动作，该帐户在追加保证金以维持原有的配资额度

            }
            else//如果是出金，则将配资额度调整到剩余金额的对应的配资本额度
            {
                finamount = nowequity * this.FinLever.AccountArgument.AsInt();
            }

            Util.Debug("充值后资金:" + nowequity.ToString() + " 初始最低保证金:" + originMargin.ToString() + " 调整前保证金:" + this.FinAmount.AccountArgument.AsDecimal() + " 调整后保证金:" + finamount);

            Argument newarg = new Argument()
            {
                Name = this.FinAmount.AccountArgument.Name,
                Type = this.FinAmount.AccountArgument.Type,
                Value = finamount.ToString(),
            };
            //调整配资额度
            FinTracker.ArgumentTracker.UpdateArgumentAccount(this.ServiceID,newarg);

            //更新内存参数
            FinServiceStub stub = FinTracker.FinServiceTracker[this.Account.ID];
            if (stub != null)
                stub.LoadArgument();
        }
    }
}
