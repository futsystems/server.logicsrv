using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib
{
    public class PositionRoundForClear
    {
        IPositionRound _pr;
        IAccount _account;
        public PositionRoundForClear(IAccount acc,IPositionRound pr)
        {
            _pr = pr;
            _account=acc;
        
        }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get { return _pr.Account; } }
        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get { return _pr.Symbol; } }
        /// <summary>
        /// 品种
        /// </summary>
        public string Security { get { return _pr.Security; } }


        /// <summary>
        /// 开仓时间
        /// </summary>
        public DateTime EntryTime { get { return _pr.EntryTime; } }
        /// <summary>
        /// 开仓数量
        /// </summary>
        public int EntrySize { get { return _pr.EntrySize; } }
        /// <summary>
        /// 开仓价格
        /// </summary>
        public decimal EntryPrice { get { return _pr.EntryPrice; } }

        /// <summary>
        /// 平仓时间
        /// </summary>
        public DateTime ExitTime { get { return _pr.ExitTime; } }
        /// <summary>
        /// 平仓数量
        /// </summary>
        public int ExitSize { get { return _pr.ExitSize; } }
        /// <summary>
        /// 平仓价格
        /// </summary>
        public decimal ExitPrice { get { return _pr.ExitPrice; } }

        /// <summary>
        /// 交易手数
        /// </summary>
        public int Size { get { return _pr.Size; } }
        /// <summary>
        /// 每手盈亏点数
        /// </summary>
        public decimal Points { get { return _pr.Points; } }
        /// <summary>
        /// 累计盈亏点数
        /// </summary>
        public decimal TotalPoints { get { return _pr.TotalPoints; } }

        public int Multiple { get { return _pr.Multiple; } }
        /// <summary>
        /// 累计盈利
        /// </summary>
        public decimal Profit { get { return _pr.Profit; } }

        /// <summary>
        /// 盈亏标识
        /// </summary>
        public bool WL { get { return this.Profit > 0; } }
        /// <summary>
        /// 客户收费
        /// </summary>
        public decimal AccountFee { get { return _pr.Commissoin; } }

        /// <summary>
        /// 居间代码
        /// </summary>
        public string AgentCode { get { return _account.AgentCode; } }

        public decimal Commission { get {
            decimal r = (_pr.EntryPrice * this.Size * _pr.Multiple / 10000) * FinGlobals.CommissionRate * 2;
            return r;
        } }


        /// <summary>
        /// 代理服务费
        /// </summary>
        public decimal ServiceFee { get {

                if (this.Profit > 0)
                {
                    return FinGlobals.ServiceFeePerWinAgent * this.Size;
                }
                else
                {
                    return FinGlobals.ServiceFeePerLossAgent * this.Size;
                }
            
            } }

        /// <summary>
        /// 代理所有费用
        /// </summary>
        public decimal AgentFee {
            get
            {
                return this.Commission + this.ServiceFee;
            }
        }
        /// <summary>
        /// 居间当日押金 月底返还
        /// </summary>
        public decimal AgentPledge { get { return FinGlobals.PledgeAgent * this.Size; } }
        /// <summary>
        /// 居间人当日收入 当日结算
        /// </summary>
        public decimal AgentRevenue { get { return this.AccountFee - this.AgentFee - this.AgentPledge; } }
        /// <summary>
        /// 该交易对应的 介绍人标识
        /// </summary>
        public string AgentSubToken { get { return _account.AgentSubToken; } }



    }
}
