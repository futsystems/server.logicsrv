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
        [ArgumentAttribute("MarginPerLotStop", EnumArgumentType.DECIMAL, 800, 800)]
        public ArgumentPair MarginPerLotStop { get; set; }


        /// <summary>
        /// 保证金 起始点
        /// </summary>
        [ArgumentAttribute("MarginPerLotStart", EnumArgumentType.DECIMAL, 2000, 2000)]
        public ArgumentPair MarginPerLotStart { get; set; }






        public SPSpecialIF()
            : base("股指专项")
        {

            _chargetype = EnumFeeChargeType.BYRound;//按交易回合计算费用
            _collecttype = EnumFeeCollectType.CollectInTrading;//在交易过程中直接收取

        }


        public override void InitArgument(Dictionary<string, Argument> accountarg, Dictionary<string, Argument> agentarg)
        {
            base.InitArgument(accountarg, agentarg);
            //LibUtil.Debug("调用服务计划的参数初始化");
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


        /// <summary>
        /// 执行风控规则
        /// 股指当平均每手资金降低到800元时候执行强平
        /// </summary>
        public override void CheckAccount()
        {
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
                //
                if (marginperplot <= MarginPerLotStop.AccountArgument.AsDecimal())
                {
                    TLCtxHelper.Debug("SPSpecialIF 触发强平  account:" + this.Account.ID + " now equity:" + nowequity.ToString() + " totalsize:" + totalsize.ToString() + " marginperlot:" + marginperplot + " stopline:" + MarginPerLotStop.AccountArgument.Value);
                    this.Account.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, "配资服务强平");
                }
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
                Position pos = TLCtxHelper.CmdAccount[o.Account].GetPosition(o.symbol, positoinside);

                decimal nowequity = this.Account.NowEquity;

                int frozensize = this.Account.Orders.Where(od => od.IsEntryPosition &&od.IsPending()).Sum(od=>od.UnsignedSize);

                decimal marginperlot = this.MarginPerLot.AccountArgument.AsDecimal();
                decimal marginperlotstart = this.MarginPerLotStart.AccountArgument.AsDecimal();

                int totalsize = 0;
                LibUtil.Debug("nowequity:" + nowequity.ToString() + " marginperlot:" + marginperlot.ToString() + " marginpperlotstop:" + marginperlotstart.ToString());
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
                if (pos.UnsignedSize + o.UnsignedSize + frozensize> totalsize)
                {
                    int cansize = totalsize - pos.UnsignedSize >= 0 ? (totalsize - pos.UnsignedSize) : 0;
                    LibUtil.Debug("pos size:" + pos.UnsignedSize.ToString() + " ordersize:" + o.UnsignedSize.ToString() +" frozensize:"+frozensize.ToString()+ " totalsize:" + totalsize.ToString());

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
            //LibUtil.Debug("specialif can open size ???????????????");
            decimal marginperlot = this.MarginPerLot.AccountArgument.AsDecimal();
            decimal nowequity = this.Account.NowEquity;
            int totalsize = (int)(nowequity / marginperlot) + 1;
            Position longpos = this.Account.GetPosition(symbol.Symbol, true);
            Position shortpos = this.Account.GetPosition(symbol.Symbol, false);
            int totalposzie = longpos.UnsignedSize + shortpos.UnsignedSize;

            return totalsize - totalposzie;
        }
        #endregion
    }
}
