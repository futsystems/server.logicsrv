using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.FinService
{

    /// <summary>
    /// 收费项目和记录
    /// 记录了某个收费项目
    /// 每日利息收费
    /// 每日盈利分红收费
    /// 每笔成交手续费加成收费
    /// 每成交回合收费
    /// 收费项目需要记录
    /// 向客户的收费总和，代理成本，代理分润，费用计算方式，费用采集方式，帐户，费用计划
    /// </summary>
    public class FeeChargeItem
    {

        public FeeChargeItem()
        {
            this.TotalFee = 0;
            this.AgentFee = 0;
            this.AgetProfit = 0;
            this.ChargeType = EnumFeeChargeType.BYTime;
            this.CollectType = EnumFeeCollectType.CollectAfterSettle;
            this.Account = string.Empty;
            this.serviceplan_fk = 0;
        }
        /// <summary>
        /// 向客户收取的费用总和
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 给代理商的费用
        /// </summary>
        public decimal AgentFee { get; set; }

        /// <summary>
        /// 代理商的利润 
        /// 代理商利润 =  费用总和 - 代理商费用
        /// </summary>
        public decimal AgetProfit { get; set; }

        /// <summary>
        /// 费用计算方式
        /// </summary>
        public EnumFeeChargeType ChargeType { get; set; }

        /// <summary>
        /// 费用采集方式
        /// </summary>
        public EnumFeeCollectType CollectType { get; set; }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 对应的服务外键
        /// 明确该帐户是使用那个服务计划所收取的费用
        /// </summary>
        public int serviceplan_fk { get; set; }


    }
}
