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
    public class SPInterest : ServicePlanBase
    {

        /// <summary>
        /// 单位资金成本 多少元/万元
        /// </summary>
        [ArgumentAttribute("FinRate", "每万元资金成本", EnumArgumentType.DECIMAL, true,10,10)]
        public ArgumentPair FinRate { get; set; }

        /// <summary>
        /// 配资比例
        /// </summary>
        [ArgumentAttribute("FinAmount", "配资额度", EnumArgumentType.DECIMAL, true, 0, 0)]
        public ArgumentPair FinAmount { get; set; }


        /// <summary>
        /// 配资比例
        /// </summary>
        [ArgumentAttribute("FinLever", "配资比例", EnumArgumentType.INT, true, 10, 10)]
        public ArgumentPair FinLever { get; set; }
        

        /// <summary>
        /// 强平比例 亏损到安全本金20%时候 执行强平,1万配10万，亏损到2000时执行强平，强平比例为20%
        /// 此比例 为安全本金的比例
        /// </summary>
        [ArgumentAttribute("StopPect", "强平比例", EnumArgumentType.DECIMAL, true, 0.2, 0.2)]
        public ArgumentPair StopPect { get; set; }



        /// <summary>
        /// 获得当前配资金额对应的安全保证金金额
        /// </summary>
        /// <returns></returns>
        decimal GetSafeMargin()
        {
            return this.FinAmount.AccountArgument.AsDecimal()/ this.FinLever.AccountArgument.AsInt();
        }

        /// <summary>
        /// 获得强平金额线
        /// </summary>
        /// <returns></returns>
        decimal GetStopMargin()
        {
            return this.GetSafeMargin() * this.StopPect.AccountArgument.AsDecimal();
        }


        public SPInterest()
        {
            SPNAME = "固定利息";
            _chargetype = EnumFeeChargeType.BYTime;//按时间收取
            _collecttype = EnumFeeCollectType.CollectAfterSettle;//在系统结算后进行收取当日配资费用
        }


        public override void OnInit()
        {
            //如果配资额度为0 则检查当前交易帐户 如果帐户有自己则按照比例自动配置上配资额度
            if (this.FinAmount.AccountArgument.AsDecimal() == 0)
            {
                decimal nowequity = this.Account.NowEquity;
                decimal stopmargin = this.GetStopMargin();

                
                if (nowequity <stopmargin)
                {
                    Util.Debug(string.Format("帐户:{0} 当前权益:{1} 配资额度为:{2} 强平比例为:{3} 强平权益额度为:{4} 不满足本金要求,冻结交易帐户",this.Account.ID,nowequity,this.FinAmount.AccountArgument.AsDecimal(),this.StopPect.AccountArgument.AsDecimal(),stopmargin));
                    this.Account.InactiveAccount();
                }
            }
        }


        /// <summary>
        /// 结算前执行费用计算 并形成扣费记录
        /// </summary>
        public override void OnSettle()
        {
            decimal finamount = this.FinAmount.AccountArgument.AsDecimal()/10000;
            decimal totalfee =  finamount* this.FinRate.AccountArgument.AsDecimal();
            decimal agentfee = finamount * this.FinRate.AgentArgument.AsDecimal();
            //进行直客收费记录
            string comment = SPNAME + " 额度:" + Util.FormatDecimal(this.FinAmount.AccountArgument.AsDecimal() / 10000) + "收费:" + Util.FormatDecimal(totalfee);

            //计算代理收费记录
            AgentCommissionDel func = (Manager agent, Manager parent) =>
            {
                decimal fee = 0;
                //代理的收费 - 代理的父代理的收费
                decimal diff = FinTracker.ArgumentTracker.GetAgentArgument(agent.mgr_fk, this.ServicePlanFK, this.FinRate.AccountArgument.Name).AsDecimal() - FinTracker.ArgumentTracker.GetAgentArgument(parent.mgr_fk, this.ServicePlanFK, this.FinRate.AccountArgument.Name).AsDecimal();
                fee = diff * finamount;
                return fee;
            };
            FeeCharge(totalfee, agentfee, func, comment);
            Util.Debug(SPNAME + " 帐户:" + this.Account.ID + " 额度:" + Util.FormatDecimal(finamount) + "万元 收费:" + Util.FormatDecimal(totalfee));
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

            decimal stopmargin = GetStopMargin();
            //如果当前资金只有  低于配资金额的2% 时候触发强平信号
            if (nowequity < stopmargin)
            {
                Util.Debug(string.Format("帐户:{0} 当前权益:{1} 配资额度为:{2} 强平比例为:{3} 强平权益额度为:{4} 不满足本金要求,执行强平并冻结交易帐户", this.Account.ID, nowequity, this.FinAmount.AccountArgument.AsDecimal(), this.StopPect.AccountArgument.AsDecimal(), stopmargin));
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
            decimal required = this.Account.CalOrderFundRequired(o,0);
            Util.Debug("SPInterest Fundavabile:" + avabile.ToString() + " Required:" + required + " account avabile fund:" + this.Account.AvabileFunds.ToString());
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

            decimal price = TLCtxHelper.CmdUtil.GetAvabilePrice(symbol);

            decimal fundperlot = Calc.CalFundRequired(symbol, price, 1);

            decimal avabilefund = GetFundAvabile(symbol);

            Util.Debug("QryCanOpenSize Fundavablie:" + avabilefund.ToString() + " Symbol:" + symbol.Symbol + " Price:" + price.ToString() + " Fundperlot:" + fundperlot.ToString());
            return (int)(avabilefund / fundperlot);
        }
        #endregion


        /// <summary>
        /// 自动跟随出入金操作 进行配资额度调整
        /// 出金操作 则配资额度会调整为当前帐户权益对应配资比例的配资金额
        /// 入金操作 如果超过了原来的安全保证金 则提高配资额度，否则就当是补充安全保证金
        /// </summary>
        /// <param name="op"></param>
        public override void OnCashOperation(JsonWrapperCashOperation op)
        {
            if (op.Status != QSEnumCashInOutStatus.CONFIRMED) return;//只针对产生作用的出入金进行调整

            decimal nowequity = this.Account.NowEquity;//当前权益

            decimal finamount = this.FinAmount.AccountArgument.AsDecimal();//配资额度

            decimal originMargin = finamount / this.FinLever.AccountArgument.AsInt();//计算原始保证金

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
