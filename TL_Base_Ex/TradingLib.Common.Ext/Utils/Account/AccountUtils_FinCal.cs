using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 品种大边数据
    /// </summary>
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

        /// <summary>
        /// 占用保证金
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// 持仓数量
        /// </summary>
        public int HoldSize { get; set; }

        /// <summary>
        /// 冻结保证金
        /// </summary>
        public decimal MarginFrozen { get; set; }

        /// <summary>
        /// 待开仓数量
        /// </summary>
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

    /// <summary>
    /// 按单向大边统计出来的保证金数据
    /// 记录某品种 保证金占用，冻结保证金等数据
    /// </summary>
    public class MarginSet
    {

        public MarginSet(bool bigside, SecurityFamily sec, decimal margin, int netfrozensize, decimal frozen)
        {
            this.MarginSide = bigside;
            this.Code = sec.Code;
            this.Margin = margin;
            this.MarginFrozen = frozen;
            this.NetFronzenSize = netfrozensize;
            this.Security = sec;
        }
        
        /// <summary>
        /// 品种
        /// </summary>
        public SecurityFamily Security { get; set; }

        /// <summary>
        /// 键值 品种-方向
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 保证金方向 记录了该保证金是按多方计算 还是空方计算
        /// </summary>
        public bool MarginSide { get; set; }

        /// <summary>
        /// 占用保证金
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// 大边持仓数量
        /// </summary>
        public int  BigHoldSize { get; set; }

        /// <summary>
        /// 小边持仓数量
        /// </summary>
        public int SmallHoldSize { get; set; }
       
        /// <summary>
        /// 冻结保证金
        /// </summary>
        public decimal MarginFrozen { get; set; }

        /// <summary>
        /// 大边待开仓数量
        /// </summary>
        public int BigPendingOpenSize { get; set; }

        /// <summary>
        /// 小边待开仓数量
        /// </summary>
        public int SmallPendingOpenSize { get; set; }

        /// <summary>
        /// 冻结保证金对应手数
        /// 这里不是原始挂单数量 是按单边规则计算出来的净冻结挂单数量
        /// </summary>
        public int NetFronzenSize { get; set; }

        public override string ToString()
        {
            return string.Format("Code:{0} Margin:{1} MarginFrozen:{2}", this.Code, this.Margin, this.MarginFrozen);
        }
    }


    public static class AccountUtils_FinCal
    {

        #region 对象过滤 用于过滤出不同品种类别的委托 成交 持仓等数据


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
        /// EntryOrder开仓委托,用于按照单向大边
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IEnumerable<MarginSet> CalFutMarginSet(this IAccount account,Order entryorder=null)
        {
            Dictionary<string, SecSide> map = new Dictionary<string, SecSide>();

            //持仓分组 按品种和方向
            var gposresult = FilterPositions(account, SecurityType.FUT).GroupBy(p => new SecSide(p));
            foreach (var g in gposresult)
            {
                g.Key.Margin = g.Sum(p => account.CalPositionMargin(p));
                g.Key.HoldSize = g.Sum(p => p.UnsignedSize);
                map.Add(g.Key.Key, g.Key);
            }

            //委托分组 按品种和方向
            IEnumerable<Order> pendingorders = FilterPendingOrders(account, SecurityType.FUT);
            pendingorders = entryorder != null ? pendingorders.Concat(new Order[] { entryorder }) : pendingorders;
            var gorderresult = pendingorders.Where(o => o.IsEntryPosition).GroupBy(o => new SecSide(o));

            foreach (var g in gorderresult)
            {
                //如果持仓分组中存在了对应的分组 则将委托分组数据记录对应的分组
                if (map.Keys.Contains(g.Key.Key))
                {
                    SecSide s = map[g.Key.Key];
                    s.MarginFrozen = g.Sum(o => account.CalOrderMarginFrozen(o));
                    s.PendingOpenSize = g.Sum(o => o.UnsignedSize);
                }
                else
                {
                    g.Key.MarginFrozen = g.Sum(o => account.CalOrderMarginFrozen(o));
                    g.Key.PendingOpenSize = g.Sum(o => o.UnsignedSize);
                    map.Add(g.Key.Key, g.Key);
                }

            }
            //通过以上分组统计整理成 品种 大边的统计数据 每个品种最多包含 多和空2个方向

            var maginlist = map.Values.GroupBy(s => s.SecCode).Select(g =>
            {
                
                //如果只有1边持仓
                if (g.Count() == 1)
                {

                    MarginSet tmp = new MarginSet(g.ElementAt(0).Side ,account.GetSecurity(g.Key), g.ElementAt(0).Margin, g.ElementAt(0).PendingOpenSize,g.ElementAt(0).MarginFrozen);
                    tmp.BigHoldSize = g.ElementAt(0).HoldSize;
                    tmp.BigPendingOpenSize = g.ElementAt(0).PendingOpenSize;
                    tmp.SmallHoldSize = 0;
                    tmp.SmallPendingOpenSize = 0;
                    return tmp;
                }
                //双边持仓
                else
                {
                    int netfrozensize = 0;

                    int maxidx = g.ElementAt(0).Margin > g.ElementAt(1).Margin ? 0 : 1;
                    SecSide big = g.ElementAt(maxidx);//大边
                    SecSide small = g.ElementAt(maxidx == 0 ? 1 : 0);//小边
                    //
                    decimal marginfrozen = 0;
                    //大边挂单大于小边挂单 则必然使用大边挂单作为冻结保证金
                    if (big.PendingOpenSize >= small.PendingOpenSize)
                    {
                        marginfrozen = big.MarginFrozen;
                        netfrozensize = big.PendingOpenSize;
                    }
                    else//大边挂单小于小边挂单 
                    {
                        //
                        int holddiff = big.HoldSize - small.HoldSize;//大边与小边的持仓差，这个差需要从小边挂单中扣除。小边挂单要超过大边持仓才占用保证金
                        //小边挂单数量如果小于持仓仓差，则小边不计算冻结保证金
                        if (small.PendingOpenSize <= holddiff)
                        {
                            netfrozensize = big.PendingOpenSize;
                            marginfrozen = big.MarginFrozen;
                        }
                        else //小边挂单数量大于持仓仓差，则小边需要计算冻结保证金
                        {
                            //小边净挂单量
                            int netsize = small.PendingOpenSize - holddiff;
                            if (netsize <= big.PendingOpenSize)//小边净挂单量小于等于 大边挂单量，则按大边挂单量计算
                            {
                                marginfrozen = big.MarginFrozen;
                                netfrozensize = big.PendingOpenSize;
                            }
                            else
                            {
                                netfrozensize = netsize;
                                marginfrozen = (small.MarginFrozen / small.PendingOpenSize) * netsize;
                            }
                        }

                    }
                    //有多空2边的统计 按大边占用保证金和 规则统计出来的保证金占用来体现当前保证金数据
                    MarginSet tmp = new MarginSet(big.Side ,account.GetSecurity(g.Key), big.Margin, netfrozensize,marginfrozen);
                    tmp.BigHoldSize = big.HoldSize;
                    tmp.BigPendingOpenSize = big.PendingOpenSize;
                    tmp.SmallHoldSize = small.HoldSize;
                    tmp.SmallPendingOpenSize = small.PendingOpenSize;
                    return tmp;
                }
            });
            return maginlist;
        }

        /// <summary>
        /// 计算期货占用保证金
        /// 在计算单个持仓的保证金是调用交易账户的CalPositionMargin方法
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalFutMargin(this IAccount account)
        {
            if (account.GetParamSideMargin())
            {
                return account.CalFutMarginSet().Sum(ms => ms.Margin * account.GetExchangeRate(ms.Security));
            }
            else
            {
                return FilterPositions(account, SecurityType.FUT).Sum(pos => account.CalPositionMargin(pos) * account.GetExchangeRate(pos.oSymbol.SecurityFamily));
            }
        }

        public static decimal CalFutMarginFrozen(this IAccount account)
        {
            if (account.GetParamSideMargin())
            {
                return account.CalFutMarginSet().Sum(ms => ms.MarginFrozen * account.GetExchangeRate(ms.Security));
            }
            else
            {
                return FilterPendingOrders(account, SecurityType.FUT).Where(o => o.IsEntryPosition).Sum(e => account.CalOrderMarginFrozen(e) * account.GetExchangeRate(e.oSymbol.SecurityFamily));
            }
        }



        public static decimal CalFutUnRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcUnRealizedPL() * account.GetExchangeRate(pos.oSymbol.SecurityFamily));
        }

        //public static decimal CalFutSettleUnRealizedPL(this IAccount account)
        //{
        //    return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcSettleUnRealizedPL());
        //}

        public static decimal CalFutRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcRealizedPL() * account.GetExchangeRate(pos.oSymbol.SecurityFamily));
        }

        public static decimal CalFutCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.FUT).Sum(fill => fill.GetCommission() * account.GetExchangeRate(fill.oSymbol.SecurityFamily));
        }

        /// <summary>
        /// 计算期货现金值
        /// 平仓盈亏 + 浮动盈亏 - 交易手续费
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
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
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcPositionCostValue());
        }
        /// <summary>
        /// 期权持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalOptPositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcPositionMarketValue());
        }
        /// <summary>
        /// 期权结算市值
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        //public static decimal CalOptSettlePositionValue(this IAccount account)
        //{
        //    return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcSettlePositionValue());
        //}
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

        #region 股票合约财务计算
        /// <summary>
        /// 股票持仓成本
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalcStkPositionCost(this IAccount account)
        {
            return FilterPositions(account, SecurityType.STK).Sum(pos => pos.CalcPositionCostValue());
        }

        /// <summary>
        /// 股票持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalcStkPositionMarketValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.STK).Sum(pos => pos.CalcPositionMarketValue());
        }

        /// <summary>
        /// 股票手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalcStkCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.STK).Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 股票平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalcStkRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.STK).Sum(pos => pos.CalcRealizedPL());
        }

        /// <summary>
        /// 股票保证金
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalcStkMargin(this IAccount account)
        {
            return FilterPositions(account, SecurityType.STK).Sum(pos => pos.CalcPositionMargin());
        }

        /// <summary>
        /// 股票冻结保证金
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalcStkMarginFrozen(this IAccount account)
        {
            return FilterPendingOrders(account, SecurityType.STK).Sum(e => account.CalOrderFundRequired(e, 0));

        }

        /// <summary>
        /// 股票现金值
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalcStkCash(this IAccount account)
        {
            return account.CalcStkRealizedPL() - account.CalcStkCommission() - account.CalcStkPositionCost();
        }

        /// <summary>
        /// 股票市值
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalcStkLiquidation(this IAccount account)
        {
            return account.CalcStkPositionMarketValue() + account.CalcStkCash();
        }

        /// <summary>
        /// 股票资金占用
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalcStkMoneyUsed(this IAccount account)
        {
            return account.CalcStkMargin() + account.CalcStkMarginFrozen();
        }
        #endregion


        #region 二元期权财务计算


        /// <summary>
        /// 没有退出或拒绝的二元期权 累加获得占用资金
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalcBOMoneyFrozen(this IAccount account)
        {
            return account.BinaryOptionOrders.Where(o=>o.IsOpen()).Sum(o => o.Amount);
        }

        /// <summary>
        /// 正常退出的二元期权 下单金额为已经使用掉的资金
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalcBOMoneyUsed(this IAccount account)
        {
            return account.BinaryOptionOrders.Where(o => o.IsExit()).Sum(o => o.Amount);
        }

        /// <summary>
        /// 正常退出的二元期权 平仓盈亏累加获得该帐户该项下的平仓盈亏
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalcBORealziedPL(this IAccount account)
        {
            return account.BinaryOptionOrders.Where(o => o.IsExit()).Sum(o => o.RealizedPL);
        }


        #endregion


    }
}
