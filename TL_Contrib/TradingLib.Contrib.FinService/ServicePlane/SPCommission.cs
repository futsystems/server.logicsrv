using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.FinService
{
    public class SPCommission : ServicePlanBase
    {
        /// <summary>
        /// 手续费加收值
        /// </summary>
        [ArgumentAttribute("CommissionMarkup", "加收值", EnumArgumentType.DECIMAL, true, 0.1, 0.1)]
        public ArgumentPair CommissionMargin { get; set; }

        [ArgumentAttribute("CommissionMarkupPect", "按百分比加收", EnumArgumentType.BOOLEAN, true, true, true)]
        public ArgumentPair CommissionMarginPect { get; set; }


        [ArgumentAttribute("FinAmount", "配资额度", EnumArgumentType.DECIMAL, true,100000, 100000)]
        public ArgumentPair FinAmount { get; set; }


        [ArgumentAttribute("FinLever", "配资比例", EnumArgumentType.DECIMAL, true, 10, 10)]
        public ArgumentPair FinLever { get; set; }

        /// <summary>
        /// 按照配资额度反向计算应缴自有资金额度，当资金亏损到应缴纳自由资金的20%时强平
        /// </summary>
        [ArgumentAttribute("StopPect", "强平比例", EnumArgumentType.DECIMAL, false, 0.2, 0.2)]
        public ArgumentPair StopPect { get; set; }

        /// <summary>
        /// 当自有资金亏损到一半时,动态减少配资额度 减少额度为原配资额度的一半
        /// </summary>
        [ArgumentAttribute("CutAmountInTrading", "盘中动态减额", EnumArgumentType.BOOLEAN, false, true, true)]
        public ArgumentPair CutAmountInTrading { get; set; }

        /// <summary>
        /// 收盘后如果自有资金不足应缴自有资金的50%时，自动降低配资额到1:10比例
        /// </summary>
        [ArgumentAttribute("CutAmountAfterSettle", "盘后动态减额", EnumArgumentType.BOOLEAN, false, true, true)]
        public ArgumentPair CutAmountAfterSettle { get; set; }

        
        public SPCommission()
        {
            SPNAME = "手续费加收";
            _chargetype = EnumFeeChargeType.BYTrade;//按交易回合计算费用
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
            //按标准计算公式得到的手续费
            decimal commission = t.Commission;

            bool ispect = this.CommissionMarginPect.AccountArgument.AsBool();
            decimal markupvalue = this.CommissionMargin.AccountArgument.AsDecimal();
            decimal accountcommission = 0;
            if (ispect)
            {
                accountcommission = commission * (1 + markupvalue);
            }
            else
            {
                accountcommission = commission + markupvalue;
            }

            bool agentispect = this.CommissionMarginPect.AgentArgument.AsBool();
            decimal agentmarkupvalue = this.CommissionMargin.AgentArgument.AsDecimal();
            decimal agentcommission = 0;
            if (agentispect)
            {
                agentcommission = commission * (1 + agentmarkupvalue);
            }
            else
            {
                agentcommission = commission + agentmarkupvalue;
            }


            decimal totalfee = accountcommission - commission;
            decimal agentfee = agentcommission - commission;

            //进行收费记录
            string comment = SPNAME + " Seq:" +t.BrokerKey +" Time:"+t.GetDateTime().ToString();
            FeeCharge(totalfee, agentfee, comment);

            return accountcommission;
        }

        public override bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            msg = string.Empty;
            return true;
        }

        public override bool CanTakeOrder(Order o, out string msg)
        {
            msg = string.Empty;
            return true;
        }
    }
}
