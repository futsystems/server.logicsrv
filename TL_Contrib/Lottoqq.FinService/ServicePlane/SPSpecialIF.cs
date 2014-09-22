﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.FinService
{
    /// <summary>
    /// 覆写相关函数 实现对应的逻辑
    /// </summary>
    public class SPSpecialIF : ServicePlanBase
    {



        /// <summary>
        /// 盈利回合收费
        /// </summary>
        [ArgumentAttribute("WinCharge", EnumArgumentType.DECIMAL, 200, 100)]
        public ArgumentPair WinCharge { get; set; }

        /// <summary>
        /// 亏损回合收费
        /// </summary>
        [ArgumentAttribute("LossCharge", EnumArgumentType.DECIMAL, 100, 50)]
        public ArgumentPair LossCharge { get; set; }

        /// <summary>
        /// 每手保证金
        /// </summary>
        [ArgumentAttribute("MarginPerLot", EnumArgumentType.DECIMAL, 5000, 5000)]
        public ArgumentPair MarginPerLot { get; set; }

        /// <summary>
        /// 每手保证金 强平线
        /// </summary>
        [ArgumentAttribute("MarginPerLotStop", EnumArgumentType.DECIMAL, 2000, 2000)]
        public ArgumentPair MarginPerLotStop { get; set; }


        public SPSpecialIF()
            : base("股指专项")
        {

            _chargetype = EnumFeeChargeType.BYRound;//按交易回合计算费用
            _collecttype = EnumFeeCollectType.CollectInTrading;//在交易过程中直接收取

        }
        public override void InitArgument(Dictionary<string, Argument> accountarg, Dictionary<string, Argument> agentarg)
        {
            base.InitArgument(accountarg, agentarg);
            LibUtil.Debug("调用服务计划的参数初始化");
            //将参数加载到内存
            //LibUtil.Debug("account args wincharge:" + this.WinCharge.AccountArgument.Value + " losscharge:" + this.LossCharge.AccountArgument.Value + " marginperlot:" + this.MarginPerLot.AccountArgument.Value + " marginperlotstop:" + this.MarginPerLotStop.AccountArgument.Value);

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
                    //如果盈利则按盈利标准扣除手续费
                    if (profit > 0)
                    {
                        commission = size * this.WinCharge.AccountArgument.AsDecimal();
                    }
                    else//如果亏损则按亏损标准扣除手续费
                    {
                        commission = size * this.LossCharge.AccountArgument.AsDecimal();
                    }
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
            //盈利
            if (round.Profit > 0)
            {
                totalfee = round.Size * this.WinCharge.AccountArgument.AsDecimal();
                agentfee = round.Size * this.WinCharge.AgentArgument.AsDecimal();
            }
            //亏损
            else
            {
                totalfee = round.Size * this.LossCharge.AccountArgument.AsDecimal();
                agentfee = round.Size * this.LossCharge.AgentArgument.AsDecimal(); ;
            }
            
            //进行收费记录
            string comment = SPNAME + " 平仓时间:" + Util.ToTLDateTime(round.ExitTime).ToString();
            FeeCharge(totalfee, agentfee,comment);
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
            msg = string.Empty;
            if (symbol.SecurityFamily.Code.Equals("IF"))
            {
                return true;
            }
            else
            {
                msg = "配资服务[" + this.SPNAME + "]只能交易品种:IF";
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
            bool isentry = o.IsEntryPosition;
            if (isentry)
            {
                bool positoinside = o.PositionSide;
                //获得对应的持仓数据
                Position pos = TLCtxHelper.CmdTradingInfo.getPosition(o.Account, o.symbol, positoinside);

                decimal nowequity = this.Account.NowEquity;

                decimal marginperlot = this.MarginPerLot.AccountArgument.AsDecimal();
                decimal marginperlotstop = this.MarginPerLot.AccountArgument.AsDecimal();
                int totalsize = 0;
                if (nowequity < marginperlot)
                {
                    if (marginperlot > marginperlotstop)
                        totalsize = 1;
                }
                else
                {
                    totalsize = (int)(nowequity / marginperlot) + 1;
                }
                //如果持仓数量+当前委托数量 超过总数量 则拒绝
                if (pos.UnsignedSize + o.UnsignedSize > totalsize)
                {
                    msg = "保证金不足,单手保证金:" + marginperlot.ToString() + " 当前最多开:" + (totalsize - pos.UnsignedSize).ToString() + "手";
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
            //LibUtil.Debug("specialif can open size ???????????????");
            decimal marginperlot = this.MarginPerLot.AccountArgument.AsDecimal();
            decimal nowequity = this.Account.NowEquity;
            int totalsize = (int)(nowequity / marginperlot) + 1;
            Position longpos = TLCtxHelper.CmdTradingInfo.getPosition(this.Account.ID, symbol.Symbol, true);
            Position shortpos = TLCtxHelper.CmdTradingInfo.getPosition(this.Account.ID, symbol.Symbol, false);
            int totalposzie = longpos.UnsignedSize + shortpos.UnsignedSize;

            return totalsize - totalposzie;
        }
        #endregion
    }
}
