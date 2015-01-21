using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    internal class SecSide
    {
        string _key = string.Empty;
        public SecSide(Position pos)
        {
            this.SecCode = pos.oSymbol.SecurityFamily.Code;
            this.Side = pos.isLong;
            _key = string.Format("{0}-{1}", this.SecCode, this.Side);
        }

        public SecSide(Order o)
        {
            this.SecCode = o.oSymbol.SecurityFamily.Code;
            this.Side = o.PositionSide;
            _key = string.Format("{0}-{1}", this.SecCode, this.Side);
        }
        /// <summary>
        /// 品种代码
        /// </summary>
        public string SecCode { get; set; }

        /// <summary>
        /// 多空方向
        /// </summary>
        public bool Side { get; set; }

        public decimal Margin { get; set; }

        public int HoldSize { get; set; }

        public decimal MarginFrozen { get; set; }

        public int PendingOpenSize { get; set; }

        /// <summary>
        /// 所有保证金
        /// </summary>
        public decimal TotalMargin { get { return this.Margin + this.MarginFrozen; } }


        public string Key { get { return _key; } }


        public override string ToString()
        {
            return string.Format("Key:{0} Total:{1} Margin:{2} MarginFrozen:{3}", _key, this.TotalMargin, this.Margin, this.MarginFrozen);
        }

        public override bool Equals(object obj)
        {
            if (obj is SecSide)
            {
                SecSide other = obj as SecSide;
                return this.GetHashCode() == other.GetHashCode();
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }
    }


    public class MarginSet
    {

        public MarginSet(string code, decimal margin, decimal frozen)
        {
            this.Code = code;
            this.Margin = margin;
            this.MarginFrozen = frozen;
        }
        /// <summary>
        /// 键值 品种-方向
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 占用保证金
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// 冻结保证金
        /// </summary>
        public decimal MarginFrozen { get; set; }

        public override string ToString()
        {
            return string.Format("Code:{0} Margin:{1} MarginFrozen:{2}", this.Code, this.Margin, this.MarginFrozen);
        }
    }


    public static class AccountUtils_FinCal
    {

        #region 对象过滤 返回对象不toarray避免的内存引用copy所有的计算只需要进行一次foreach循环
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Order> FilterOrders(this IAccount account, SecurityType type)
        {
            return account.Orders.Where(o => o.SecurityType == type);
        }

        public static IEnumerable<Order> FilterPendingOrders(this IAccount account, SecurityType type)
        {
            return account.Orders.Where(o => o.SecurityType == type && o.IsPending());
        }

        public static IEnumerable<Trade> FilterTrades(this IAccount account, SecurityType type)
        {
            return account.Trades.Where(f => f.SecurityType == type);
        }

        public static IEnumerable<Position> FilterPositions(this IAccount account, SecurityType type)
        {
            return account.Positions.Where(p => p.oSymbol.SecurityType == type);
        }
        #endregion

        #region 计算期货财务数据
        /// <summary>
        /// 按照单向大边规则计算品种的保证金数据集
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IEnumerable<MarginSet> CalFutMarginSet(this IAccount account)
        {
            Dictionary<string, SecSide> map = new Dictionary<string, SecSide>();
            var gposresult = FilterPositions(account, SecurityType.FUT).GroupBy(p => new SecSide(p));
            foreach (var g in gposresult)
            {
                g.Key.Margin = g.Sum(p => p.CalcPositionMargin());
                g.Key.HoldSize = g.Sum(p => p.UnsignedSize);
                map.Add(g.Key.Key, g.Key);
            }

            var gorderresult = FilterPendingOrders(account, SecurityType.FUT).Where(o => o.IsEntryPosition).GroupBy(o => new SecSide(o));
            foreach (var g in gorderresult)
            {
                if (map.Keys.Contains(g.Key.Key))
                {
                    SecSide s = map[g.Key.Key];
                    s.MarginFrozen = g.Sum(o => o.CalFundRequired(TLCtxHelper.CmdUtils.GetAvabilePrice(o.Symbol)));
                    s.PendingOpenSize = g.Sum(o => o.UnsignedSize);
                }
                else
                {
                    g.Key.MarginFrozen = g.Sum(o => o.CalFundRequired(TLCtxHelper.CmdUtils.GetAvabilePrice(o.Symbol)));
                    g.Key.PendingOpenSize = g.Sum(o => o.UnsignedSize);
                    map.Add(g.Key.Key, g.Key);
                }
            }

            var maginlist = map.Values.GroupBy(s => s.SecCode).Select(g =>
            {
                //如果只有1边持仓
                if (g.Count() == 1)
                {
                    return new MarginSet(g.Key, g.ElementAt(0).Margin, g.ElementAt(0).MarginFrozen);
                }
                //双边持仓
                else
                {
                    int maxidx = g.ElementAt(0).Margin > g.ElementAt(1).Margin ? 0 : 1;
                    SecSide big = g.ElementAt(maxidx);//大边
                    SecSide small = g.ElementAt(maxidx == 0 ? 1 : 0);//小边
                    //
                    decimal marginfrozen = 0;
                    //大边挂单大于小边挂单 则必然使用大边挂单作为冻结保证金
                    if (big.PendingOpenSize >= small.PendingOpenSize)
                    {
                        marginfrozen = big.MarginFrozen;
                    }
                    else//大边挂单小于小边挂单 
                    {
                        //
                        int holddiff = big.HoldSize - small.HoldSize;//大边与小边的持仓差，这个差需要从小边挂单中扣除。小边挂单要超过大边持仓才占用保证金
                        //小边挂单数量如果小于持仓仓差，则小边不计算冻结保证金
                        if (small.PendingOpenSize <= holddiff)
                            marginfrozen = big.MarginFrozen;
                        else //小边挂单数量大于持仓仓差，则小边需要计算冻结保证金
                        {
                            //小边净挂单量
                            int netsize = small.PendingOpenSize - holddiff;
                            if (netsize <= big.PendingOpenSize)//小边净挂单量小于等于 大边挂单量，则按大边挂单量计算
                                marginfrozen = big.MarginFrozen;
                            else
                                marginfrozen = (small.MarginFrozen / small.PendingOpenSize) * netsize;
                        }

                    }
                    return new MarginSet(g.Key, big.Margin, marginfrozen);
                }
            });
            return maginlist;
        }

        public static decimal CalFutMargin(this IAccount account)
        {
            if (account.SideMargin)
            {
                return account.CalFutMarginSet().Sum(ms => ms.Margin);
            }
            else
            {
                return FilterPositions(account, SecurityType.FUT).Sum(pos => account.CalPositionMargin(pos));
            }
        }

        public static decimal CalFutMarginFrozen(this IAccount account)
        {
            if (account.SideMargin)
            {
                return account.CalFutMarginSet().Sum(ms => ms.MarginFrozen);
            }
            else
            {
                return FilterPendingOrders(account, SecurityType.FUT).Where(o => o.IsEntryPosition).Sum(e => account.CalOrderFundRequired(e, 0));
            }
        }

        public static decimal CalFutUnRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcUnRealizedPL());
        }

        public static decimal CalFutSettleUnRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcSettleUnRealizedPL());
        }

        public static decimal CalFutRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcRealizedPL());
        }

        public static decimal CalFutCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.FUT).Sum(fill => fill.GetCommission());
        }

        public static decimal CalFutCash(this IAccount account)
        {
            return CalFutRealizedPL(account) + CalFutUnRealizedPL(account) - CalFutCommission(account);
        }

        public static decimal CalFutLiquidation(this IAccount account)
        {
            return CalFutCash(account);
        }

        public static decimal CalFutMoneyUsed(this IAccount account)
        {
            return CalFutMargin(account) + CalFutMarginFrozen(account);
        }


        #endregion

        #region 期权计算
        /// <summary>
        /// 期权持仓成本
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalOptPositionCost(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcPositionCost());
        }
        /// <summary>
        /// 期权持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalOptPositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcPositionValue());
        }
        /// <summary>
        /// 期权结算市值
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalOptSettlePositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcSettlePositionValue());
        }
        /// <summary>
        /// 期权平仓盈亏
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalOptRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcRealizedPL());
        }
        /// <summary>
        /// 期权交易手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalOptCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.OPT).Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 期权资金占用
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalOptMoneyFrozen(this IAccount account)
        {
            try
            {
                return FilterPendingOrders(account, SecurityType.OPT).Sum(e => account.CalOrderFundRequired(e, 0));
            }
            catch (Exception ex)
            {
                return decimal.MaxValue;
            }
        }


        public static decimal CalOptCash(this IAccount account)
        {
            return CalOptRealizedPL(account) - CalOptCommission(account) - CalOptPositionCost(account);
        }

        public static decimal CalOptLiquidation(this IAccount account)
        {
            return CalOptPositionValue(account) + CalOptCash(account);
        }

        public static decimal CalOptMoneyUsed(this IAccount account)
        {
            return CalOptMoneyFrozen(account) + CalOptPositionCost(account);
        }
  
        #endregion

        #region 异化合约财务计算
        /// <summary>
        /// 异化合约持仓成本
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovPositionCost(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcPositionCost());
        }

        /// <summary>
        /// 异化合约持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovPositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcPositionValue());
        }

        /// <summary>
        /// 异化合约结算市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovSettlePositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcSettlePositionValue());
        }

        /// <summary>
        /// 异化合约手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.INNOV).Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 异化合约平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcRealizedPL());
        }

        /// <summary>
        /// 异化合约保证金
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovMargin(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcPositionMargin());
        }

        /// <summary>
        /// 异化合约保证金冻结
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovMarginFrozen(this IAccount account)
        {
            return FilterPendingOrders(account, SecurityType.INNOV).Sum(e => account.CalOrderFundRequired(e, 0));

        }

        public static decimal CalInnovCash(this IAccount account)
        {
            return CalInnovRealizedPL(account) - CalInnovCommission(account) - CalInnovPositionCost(account);
        }

        public static decimal CalInnovLiquidation(this IAccount account)
        {
            return CalInnovPositionValue(account) + CalInnovCash(account);
        }

        public static decimal CalInnovMoneyUsed(this IAccount account)
        {
            return CalInnovMargin(account) + CalInnovMarginFrozen(account);
        }
        #endregion

        public static string GetCustName(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.Name))
            {
                return  "帐号[" + account.ID + "]";
            }
            return account.Name;
        }

        public static string GetCustBroker(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.Broker))
            {
                return GlobalConfig.DefaultBroker;
            }
            return account.Broker;
        }
    }
}
