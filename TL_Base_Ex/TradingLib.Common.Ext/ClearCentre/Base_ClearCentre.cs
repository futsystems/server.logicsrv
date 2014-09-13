using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Common
{
    /// <summary>
    /// 清算中心，为服务器维护了一批交易账户,以及每个交易账户的实时Order,trades,position的相关信息。
    /// </summary>
    public abstract class ClearCentreBase : BaseSrvObject, IClearCentreBase
    {
        

        #region 获得某个合约的当前价格信息
        public event GetSymbolTickDel newSymbolTickRequest;
        protected Tick getSymbolTick(string symbol)
        {
            if (newSymbolTickRequest != null)
                return newSymbolTickRequest(symbol);
            else
                return null;
        }
        /// <summary>
        /// 获得某个合约的有效价格
        /// 如果返回-1则价格无效
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetAvabilePrice(string symbol)
        {
            Tick k = getSymbolTick(symbol);//获得当前合约的最新数据
            if (k == null) return -1;

            decimal price = somePrice(k);

            //如果价格有效则返回价格 否则返回-1无效价格
            return price > 0 ? price : -1;
        }

        /// <summary>
        /// 从Tick数据采获当前可用的价格
        /// 优先序列 最新价/ ask / bid 如果均不可用则返回价格0
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private decimal somePrice(Tick k)
        {
            if (k.isTrade)
                return k.trade;
            if (k.hasAsk)
                return k.ask;
            if (k.hasBid)
                return k.bid;
            else
                return -1;
        }
        #endregion


        #region 数据结构

        protected AccountTracker acctk = new AccountTracker();

        protected TotalTracker totaltk = new TotalTracker();
        #endregion


        public ClearCentreBase(string name = "ClearCentreBase")
            : base(name)
        {

        }


        #region 【IAccountOperation】修改账户相应设置,查询可开,修改密码,验证交易账户,出入金,激活禁止交易账户等
        
        /// <summary>
        /// 激活某个交易账户 允许其进行交易
        /// </summary>
        /// <param name="id"></param>
        public abstract void ActiveAccount(string id);
        /// <summary>
        /// 禁止某个账户进行交易
        /// </summary>
        /// <param name="id"></param>
        public abstract void InactiveAccount(string id);
        /// <summary>
        /// 修改账户类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ca"></param>
        public abstract void UpdateAccountCategory(string id, QSEnumAccountCategory ca);

        /// <summary>
        /// 更改账户密码
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public abstract void ChangeAccountPass(string acc, string pass);

        /// <summary>
        /// 更新账户购买乘数
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="buymup"></param>
        //public abstract void UpdateAccountBuyMultiplier(string acc, int buymultiplier);

        /// <summary>
        /// 修改账户交易转发类别
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        public abstract void UpdateAccountRouterTransferType(string id, QSEnumOrderTransferType type);

        /// <summary>
        /// 平调某个账户所有持仓
        /// </summary>
        /// <param name="accid"></param>
        //public abstract void FlatPosition(string accid,QSEnumOrderSource source, string comment);


        /// <summary>
        /// 平掉某个仓为
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        //public abstract void  FlatPosition(Position pos, QSEnumOrderSource source, string comment);
        /// <summary>
        /// 交易账户的资金操作
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        public abstract void CashOperation(string acc, decimal amount,string transref, string comment);

        /// <summary>
        /// 复位某个账户的资金到多少数值
        /// </summary>
        /// <param name="account"></param>
        /// <param name="value"></param>
        public abstract void ResetEquity(string account, decimal value);

        /// <summary>
        /// 更新账户日内交易设置
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="intraday"></param>
        public abstract void UpdateAccountIntradyType(string acc, bool intraday);
        
        #endregion

        #region 【IFinanceCaculation】【多品种财务数据计算】

        /// <summary>
        /// 获得某个帐户的某种交易集合
        /// </summary>
        /// <param name="acc"></param>
        protected Trade[] FilterTrades(IAccount acc,SecurityType type)
        { 
            try
            {
                return acctk.GetTradeBook(acc.ID).Where(t => t.SecurityType == type).ToArray();
            }
            catch(Exception ex)
            {
                debug("filtertrades error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Trade[] { };
            }
        }

        /// <summary>
        /// 获得某个帐户的所有证券类委托集合
        /// </summary>
        /// <param name="acc"></param>
        protected Order[] FilterOrder(IAccount acc, SecurityType type)
        {
            try
            {
                return acctk.GetOrderBook(acc.ID).ToArray().Where(o => o.SecurityType == type).ToArray() ;
            }
            catch (Exception ex)
            {
                debug("filterorder error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Order[] { };
            }
        }

        /// <summary>
        /// 过滤某个帐户某个证券类别小的的待成交委托,Opened,PartFilled
        /// 风控计算问题
        /// 当系统接受到委托 但是此时委托状态仍然为placed 但是委托没有到达BrokerRouter
        /// 
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Order[] FilterPendingOrder(IAccount acc, SecurityType type)
        {
            try
            {
                return acctk.GetOrderBook(acc.ID).ToArray().Where(delegate(Order o) { return o.SecurityType == type && OrderTracker.IsPending(o); }).ToArray();
            }
            catch (Exception ex)
            {
                debug("filterpendingorder error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Order[]{};
            } 
        }

        /// <summary>
        /// 获得某个帐户所有商品类持仓
        /// </summary>
        /// <param name="acc"></param>
        protected Position[] FilterPosition(IAccount acc, SecurityType type)
        {
            try
            {
                return acctk.GetPositionBook(acc.ID).ToArray().Where(p => p.oSymbol.SecurityType == type).ToArray();
            }
            catch (Exception ex)
            {
                debug("filterposition error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Position[]{};
            }
            
        }

        /// <summary>
        /// 计算某个帐户的期货保证金占用
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutMargin(IAccount acc)
        {
            try
            {
                return Calc.CalPositionMargin(FilterPosition(acc, SecurityType.FUT));
            }
            catch (Exception ex)
            {
                debug("callfutmargin error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
            }
            return 0;
        }

        /// <summary>
        /// 计算期货冻结保证金
        /// 将PendingOrder计算资金占用
        /// 这里的资金占用将所有委托都算入了资金占用,如果存在方向相反的委托应该如何处理？
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutMarginFrozen(IAccount acc)
        {
            //decimal m = 0;
            try
            {
                //if (acc.ID == "9280014")
                //{
                //    int i = 0;
                //}
                return FilterPendingOrder(acc, SecurityType.FUT).Sum(e => acc.CalOrderFundRequired(e,0));
            }
            catch (Exception ex)
            {
                debug("callfutmarginfronzen error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                return decimal.MaxValue;
            }
        }
        /// <summary>
        /// 计算某个帐户的期货浮动盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutUnRealizedPL(IAccount acc)
        {
            return Calc.CalUnRealizedPL(FilterPosition(acc, SecurityType.FUT));
        }

        /// <summary>
        /// 计算某个帐户的期货盯市盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutSettleUnRealizedPL(IAccount acc)
        {
            return Calc.CalSettleUnRealizedPL(FilterPosition(acc, SecurityType.FUT));
        }
        /// <summary>
        /// 计算某个帐户的期货平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutRealizedPL(IAccount acc)
        {
            return Calc.CalRealizedPL(FilterPosition(acc, SecurityType.FUT));
        }

        /// <summary>
        /// 计算某个帐户的期货交易手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutCommission(IAccount acc)
        { 
            return Calc.CalCommission(FilterTrades(acc,SecurityType.FUT));
        }

        #region opt计算
        /// <summary>
        /// 计算某个帐户期权的持仓成本
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptPositionCost(IAccount acc)
        {
            return Calc.CalPositionCost(FilterPosition(acc, SecurityType.OPT));
        }

        /// <summary>
        /// 计算某个帐户期权的持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptPositionValue(IAccount acc)
        {
            return Calc.CalPositionValue(FilterPosition(acc, SecurityType.OPT));
        }

        /// <summary>
        /// 计算期权结算时的结算市价值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptSettlePositionValue(IAccount acc)
        {
            return Calc.CalSettlePositionValue(FilterPosition(acc, SecurityType.OPT));
        }

        /// <summary>
        /// 计算某个帐户期权平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptRealizedPL(IAccount acc)
        {
            return Calc.CalRealizedPL(FilterPosition(acc, SecurityType.OPT));
        }

        /// <summary>
        /// 计算某个帐户的期权交易手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptCommission(IAccount acc)
        {
            return Calc.CalCommission(FilterTrades(acc, SecurityType.OPT));
        }

        /// <summary>
        /// 计算期权资金冻结
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptMoneyFrozen(IAccount acc)
        {
            try
            {
                return FilterPendingOrder(acc, SecurityType.OPT).Sum(e => acc.CalOrderFundRequired(e,0));
            }
            catch (Exception ex)
            {
                return decimal.MaxValue;
            }
        }
        #endregion

        #region INNOV

        /// <summary>
        /// 计算异化合约持仓成本
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovPositionCost(IAccount acc)
        {
            return Calc.CalPositionCost(FilterPosition(acc, SecurityType.INNOV));
        }

        /// <summary>
        /// 计算异化合约持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovPositionValue(IAccount acc)
        {
            return Calc.CalPositionValue(FilterPosition(acc, SecurityType.INNOV));
        }

        /// <summary>
        /// 计算异化合约的持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovSettlePositionValue(IAccount acc)
        {
            return Calc.CalSettlePositionValue(FilterPosition(acc, SecurityType.INNOV));
        }

        /// <summary>
        /// 计算异化合约收学费统计
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovCommission(IAccount acc)
        {
            return Calc.CalCommission(FilterTrades(acc, SecurityType.INNOV));
        }

        /// <summary>
        /// 计算异化合约的平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovRealizedPL(IAccount acc)
        {
            return Calc.CalRealizedPL(FilterPosition(acc, SecurityType.INNOV));
        }

        /// <summary>
        /// 计算异化合约的保证金
        /// 在正常的期权或者股票交易过程中 资金占用就是实际的购入成本
        /// 但是由于异化合约是底层合约的封装,实际购买的是底层合约，但是和客户结算时是按异化合约的保证金数据进行计算
        /// 所以客户可以用低保证金来享受到底层合约的价格波动
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovMargin(IAccount acc)
        {
            return Calc.CalPositionMargin(FilterPosition(acc, SecurityType.INNOV));
        //        decimal m = 0;
        //        foreach (Position p in FilterPosition(acc, SecurityType.INNOV))
        //        {
        //            decimal margin = 0;
        //            if(p.oSymbol.Margin>0)
        //            {
        //                margin =  p.UnsignedSize * (p.oSymbol.Margin + (p.oSymbol.ExtraMargin>0?p.oSymbol.ExtraMargin:0));//通过固定保证金来计算持仓保证金占用
        //            }
        //            else
        //            {
        //                margin = 0;
        //            }
        //            m += margin;
        //        }
        //        return m;
        }


        /// <summary>
        /// 计算异化合约的保证金占用 比如挂单,防止避免保证金计算穿越
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovMarginFrozen(IAccount acc)
        {
            decimal m = 0;
            try
            {
                return FilterPendingOrder(acc, SecurityType.INNOV).Sum(e => acc.CalOrderFundRequired(e,0));
                //foreach (Order o in FilterPendingOrder(acc, SecurityType.INNOV))
                //{
                //    m += acc.CalOrderFundRequired(o);//调用帐户计算委托占用收费的函数进行计算
                //}
                //return m;
            }
            catch (Exception ex)
            {
                return decimal.MaxValue;
            }
        }
        #endregion


        /// <summary>
        /// 计算某个委托所需要占用的保证金
        /// 下单方向与原持仓位委托方向一致(开仓/增仓)某个委托当前所占用的保证金
        /// 价格在正常报价范围内,则用指定价格计算冻结保证金,如果在非正常范围则按照当前最新价格计算保证金
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public decimal CalOrderFundRequired(Order o, decimal mktMvalue = decimal.MaxValue)
        {
            Symbol symbol = o.oSymbol;
            decimal price = 0;
            price = GetAvabilePrice(symbol.TickSymbol);

            //期权委托资金占用计算
            if (symbol.SecurityType == SecurityType.OPT)
            {
                if (price < 0)
                    return mktMvalue;

                if (Math.Abs(o.price - price) / price > 0.1M)
                    return Calc.CalFundRequired(symbol, price, o.UnsignedSize);
                return Calc.CalFundRequired(symbol, o.stopp, o.UnsignedSize);//o.UnsignedSize * o.stopp * symbol.Margin * symbol.Multiple;
            }

            //期货资金占用计算
            if (symbol.SecurityType == SecurityType.FUT)
            {
                //市价委托用当前的市场价格来计算保证金占用
                if (symbol.Margin <= 1)
                {
                    //debug("Orderid:" + o.id.ToString() + " Margin:" + symbol.Margin.ToString() + " price:" + price.ToString() + " mktvalue:" + mktMvalue.ToString(), QSEnumDebugLevel.INFO);
                    if (price < 0)
                        return mktMvalue;
                    //debug(PROGRAME + ":"+sec.ToString()+" margin:"+sec.Margin.ToString(), QSEnumDebugLevel.DEBUG);
                    if (o.isMarket)
                    {
                        return Calc.CalFundRequired(symbol, price, o.UnsignedSize);
                    }
                    //限价委托用限定价格计算保证金占用
                    if (o.isLimit)
                    {

                        if (Math.Abs(o.price - price) / price > 0.1M)//如果价格偏差在10以外 则以当前的价格来计算保证金 10%以内则以 设定的委托价格来计算保证金
                            return Calc.CalFundRequired(symbol, price, o.UnsignedSize);//o.unsignedSize标识剩余委托数量来求保证金占用size为0的委托 保证金占用为0 这里不是按totalsize来进行的
                        return Calc.CalFundRequired(symbol, o.price, o.UnsignedSize);
                    }
                    //追价委托用追价价格计算保证金占用
                    if (o.isStop)
                    {
                        if (Math.Abs(o.stopp - price) / price > 0.1M)
                            return Calc.CalFundRequired(symbol, price, o.UnsignedSize);
                        return Calc.CalFundRequired(symbol, o.stopp, o.UnsignedSize);//o.UnsignedSize * o.stopp * symbol.Margin * symbol.Multiple;
                    }
                    else
                        //如果便利的委托类型未知 则发挥保证金为最大
                        return decimal.MaxValue;


                }
                else
                    return symbol.Margin * o.UnsignedSize;//固定金额保证金计算 手数×保证金额度 = 总保证金额度
            }

            //异化合约资金占用
            if (symbol.SecurityType == SecurityType.INNOV)
            {
                if (symbol.Margin > 0)
                {
                    return (symbol.Margin + (symbol.ExtraMargin > 0 ? symbol.ExtraMargin : 0)) * o.UnsignedSize;
                }
                else
                {
                    return decimal.MaxValue;
                }
            }
            return decimal.MaxValue;
        }

        #endregion

        #region 【IAccountTradingInfo】从内存获得相关数据

        /// <summary>
        /// 检查某个账户是否有暴露的仓位
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public bool AnyPosition(string accid)
        {
            return acctk.AnyPosition(accid);
        }

        /// <summary>
        /// 获得某个账户的仓位管理器
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public Object getPositionTracker(string AccountID)
        {
            return acctk.GetPositionBook(AccountID);
        }

        /// <summary>
        /// 获得某个账户的Order管理器
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public Object getOrderTracker(string AccountID)
        {

            return acctk.GetOrderBook(AccountID);
        }



        #region 获得交易账号对应的交易信息

        //委托 持仓 成交 取消信息
        /// <summary>
        /// 获得昨日持仓数据
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position[] getPositionHold(string account)
        {
            if (string.IsNullOrEmpty(account))
            {
                return totaltk.PositionHoldTracker.ToArray();
            }
            return acctk.GetPositionHold(account);
        }

        /// <summary>
        /// 获得某个账户的所有持仓
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public Position[] getPositions(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                return totaltk.PositionTracker.ToArray();
            }
            return acctk.GetPositions(accountID);
        }

        /// <summary>
        /// 获得某个交易账户当天所有的委托
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public Order[] getOrders(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                return totaltk.OrderTracker.ToArray();
            }
            return acctk.GetOrders(accountID);
        }
        /// <summary>
        /// 获得某个交易账户的成交数据
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public Trade[] getTrades(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                return totaltk.TradeTracker.ToArray();
            }
            return acctk.GetTrades(accountID);
        }
        /// <summary>
        /// 获得某个交易账户的取消委托数据
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public long[] getCancels(string accountID)
        {

            return acctk.GetCancels(accountID);
        }

        

        #endregion


        /// <summary>
        /// 获得交易帐号上次结算持仓数量
        /// </summary>
        public  int getPositionHoldSize(string account, string symbol)
        {
            foreach (Position pos in totaltk.PositionHoldTracker)
            {
                if (pos.Account == account && pos.Symbol == symbol)
                    return pos.Size;
            }
            return 0;
        }

        /// <summary>
        /// 通过AccountID,symbol返回Position
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position getPosition(string account, string symbol)
        {
            return acctk.GetPosition(account, symbol);
        }


        /// <summary>
        /// 获得某个委托对应的Account symbol下 所持仓位的反向为成交委托,用于CTP发送限价委托时检查仓位情况
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int getUnfilledSizeExceptStop(Order o)
        {
            //多头仓位 查询卖委托 空头仓位 查询买委托 (买 卖方向与仓位方向相反,则为未成交的 平仓委托)
            //OrdBook[o.Account].getUnfilledSizeExceptStop(o.symbol, !this.getPosition(o).isLong);
            return (this.getOrderTracker(o.Account) as OrderTracker).getUnfilledSizeExceptStop(o.symbol, !this.getPosition(o.Account,o.symbol).isLong);

        }

        /// <summary>
        /// 获得某个持仓的未成交平仓委托数量
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int getUnfilledPositionFlatOrderSize(Position pos)
        {
            return (this.getOrderTracker(pos.Account) as OrderTracker).getUnfilledSize(pos.Symbol, !pos.isLong);
        }

        /// <summary>
        /// 获得某个账户的所有待成交合约
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public long[] getPendingOrders(string account)
        {
            return (this.getOrderTracker(account) as OrderTracker).getPending();
        }
        /// <summary>
        /// 返回某个委托对应账户与合约下所有与该委托方向相同其他委托,用于提交委托前取消同向委托
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public long[] getPendingOrders(Order o)
        {
            long[] olist = this.getPendingOrders(o.Account, o.symbol, o.side);//(this.getOrderTracker(o.Account) as OrderTracker).getPending(o.symbol, o.side);
            List<long> nlist = new List<long>();
            foreach (long oid in olist)
            {
                if (oid != o.id)
                    nlist.Add(oid);
            }
            return nlist.ToArray();
        }
        /// <summary>
        /// 返回某个账户 某个合约 某个方向的待成交委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public long[] getPendingOrders(string account, string symbol, bool side)
        {
            return (this.getOrderTracker(account) as OrderTracker).getPending(symbol, side);
        }
        /// <summary>
        /// 获得某个账户 某个合约的所有待成交委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public long[] getPendingOrders(string account, string symbol)
        {
            return (this.getOrderTracker(account) as OrderTracker).getPending(symbol);
        }
        #endregion

        #region 【ITotalAccountInfo】 获得整体的交易信息

        ///// <summary>
        ///// 通过OrderId获得该Order
        ///// </summary>
        ///// <param name="oid"></param>
        ///// <returns></returns>
        public Order SentOrder(long oid)
        {
            return totaltk.SentOrder(oid);
        }

        /// <summary>
        /// 获得总账户的PositionTracker
        /// </summary>
        public Object DefaultPositionTracker { get { return totaltk.PositionTracker; ; } }
        /// <summary>
        /// 获得总账户的OrderTracker
        /// </summary>
        public Object DefaultOrderTracker { get { return totaltk.OrderTracker; ; } }
        /// <summary>
        /// 获得总账户的交易列表
        /// </summary>
        public List<Trade> DefaultTradeList { get { return totaltk.TradeTracker; } }

        /// <summary>
        /// 总帐户上次结算持仓管理器
        /// </summary>
        public Object DefaultPositionHoldTracker { get { return totaltk.PositionHoldTracker; } }

        /// <summary>
        /// 获得当前的持仓数据
        /// </summary>
        public Position[] PositionsHoldNow
        {
            get
            {
                List<Position> plist = new List<Position>();
                foreach (Position p in DefaultPositionTracker as PositionTracker)
                {
                    if (!p.isFlat)
                    {
                        plist.Add(p);
                    }
                }
                return plist.ToArray();

            }
        }
        /// <summary>
        /// 返回上次结算持仓数据
        /// </summary>
        public Position[] PositionHoldLastSettleday
        {
            get
            {
                return totaltk.PositionHoldTracker.ToArray();
            }
        }

        #endregion

        #region 【IGotTradingInfo】昨日持仓 委托 成交 取消 Tick数据处理
        //注为了记录隔夜尺长 分账户与总账户的隔夜持仓要单独放置即要体现在当前持仓总又要体现在隔夜持仓中
        /// <summary>
        /// 清算中心获得持仓数据
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(Position p)
        {
            debug("got postioin:" + p.ToString(), QSEnumDebugLevel.INFO);
            if (!HaveAccount(p.Account)) return;
            Symbol symbol = p.oSymbol;
            if (symbol == null)
            {
                debug("symbol:" + p.Symbol + " not exist in basictracker, dropit",QSEnumDebugLevel.ERROR);
                return;
            }
            debug("account tracker got position", QSEnumDebugLevel.INFO);
            acctk.GotPosition(p);
            totaltk.GotPosition(p);
            onGotPosition(p);
        }

        public virtual void onGotPosition(Position p)
        {

        }
        
        /// <summary>
        /// 清算中心获得委托数据
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            try
            {
                if (!HaveAccount(o.Account)) return;
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + o.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }

                bool neworder = !totaltk.IsTracked(o.id);
                acctk.GotOrder(o);
                totaltk.GotOrder(new OrderImpl(o));

                //o.Filled = OrdBook[o.Account].Filled(o.id);//获得委托成交
                onGotOrder(o, neworder);
            }
            catch (Exception ex)
            {
                debug("处理委托异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        public virtual void onGotOrder(Order o, bool neworder)
        {
        }
        
        /// <summary>
        /// 清算中心获得取消
        /// </summary>
        /// <param name="oid"></param>
        public void GotCancel(long oid)
        {
            try
            {
                string account = SentOrder(oid).Account;
                if (!HaveAccount(account)) return;
                acctk.GotCancel(account, oid);
                totaltk.GotCancel(oid);

                onGotCancel(oid);
            }
            catch (Exception ex)
            {
                debug("处理取消异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        public virtual void onGotCancel(long oid)
        {

        }

        /// <summary>
        /// 清算中心获得成交
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f)
        {
            try
            {
                if (!HaveAccount(f.Account)) return;
                Symbol symbol = f.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + f.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }
                    
                PositionTransaction postrans = null;
                Position pos = acctk.GetPosition(f.Account, f.symbol);
                int beforesize = pos.UnsignedSize;
                decimal highest = pos.Highest;
                decimal lowest = pos.Lowest;

                acctk.GotFill(f);

                int aftersize = acctk.GetPosition(f.Account, f.symbol).UnsignedSize;//查询该成交后数量
                decimal c = -1;

                //debug("got fill beforesize:" + beforesize.ToString() + " aftersize:" + aftersize.ToString(), QSEnumDebugLevel.INFO);
                //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                if (f.Commission < 0)
                {
                    decimal commissionrate = 0;
                    //成交后持仓数量大于成交前数量 开仓或者加仓
                    if (aftersize > beforesize)
                    {
                        commissionrate = symbol.EntryCommission;
                    }
                    //成交后持仓数量小于成交后数量 平仓或者减仓
                    if (aftersize < beforesize)
                    {
                       
                        //如果对应的合约是单边计费的或者有特殊计费方式的合约，则我们单独计算该部分费用,注这里还需要加入一个日内交易的判断,暂时不做(当前交易均为日内)
                        //获得平仓手续费特例
                        if (CommissionHelper.AnyCommissionSetting(SymbolHelper.genSecurityCode(f.symbol), out commissionrate))
                        {
                            //debug("合约:" + SymbolHelper.genSecurityCode(f.symbol) + "日内手续费费差异", QSEnumDebugLevel.MUST);
                        }
                        else//没有特殊费率参数,则为标准的出场费率
                        {
                            commissionrate = symbol.ExitCommission;
                        }
                    }

                    f.Commission = Calc.CalCommission(commissionrate, f);
                }

                //生成持仓操作记录 同时结合beforeszie aftersize 设置fill PositionOperation,需要知道帐户的持仓信息才可以知道是开 加 减 平等信息
                f.PositionOperation = PositionTransaction.GenPositionOperation(beforesize, aftersize);
                postrans = new PositionTransaction(f, symbol, beforesize, aftersize, highest, lowest);

                totaltk.GotFill(f);

                //子类函数的onGotFill用于执行数据记录以及其他相关业务逻辑
                onGotFill(f, postrans);
                
            }
            catch (Exception ex)
            {
                debug("Got Fill error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        public virtual void onGotFill(Trade fill, PositionTransaction postrans)
        {
        }

        //得到新的Tick数据
        public void GotTick(Tick k)
        {
            try
            {
                acctk.GotTick(k);
                totaltk.GotTick(k);
            }
            catch (Exception ex)
            {
                debug("Got Tick error:" + ex.ToString());
            }
        }
        #endregion

        #region 【IClearCentreBase】交易账户 操作
        /// <summary>
        /// 获得Account数组
        /// </summary>
        /// <returns></returns>
        public IAccount[] Accounts
        {
            get { return acctk.Accounts; }//AcctList.Values.ToArray(); }
        }
        /// <summary>
        /// 通过AccountID获得某个账户
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public IAccount this[string accid]
        {
            get
            {
                return acctk[accid];
            }
        }

        /// <summary>
        /// 查找某个userid的交易帐户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public IAccount QryAccount(int uid,QSEnumAccountCategory category)
        {
            return acctk.QryAccount(uid, category);
        }


        /// <summary>
        /// 查询是否有某个ID的账户
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool HaveAccount(string a)
        {
            return acctk.HaveAccount(a);
        }
        /// <summary>
        /// 查询是否有某个ID的账户并返回该账户
        /// </summary>
        /// <param name="a"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public bool HaveAccount(string a, out IAccount acc)
        {
            acc = null;
            return acctk.HaveAccount(a, out acc);
        }

        /// <summary>
        /// 将某个账户缓存到服务器内存，注意检查是否已经存在该账户
        /// 生成该账户所对应的数据对象用于实时储存交易信息与合约信息
        /// </summary>
        /// <param name="a"></param>
        protected void CacheAccount(IAccount a)
        {
            acctk.CacheAccount(a);
            //6.账户反向引用clearcentre用于计算财务信息
            onCacheAccount(a);
            //a.ClearCentre = new ClearCentreAdapterToAccount(a as IAccount,this);//this;// as IAccountClearCentre);
        }

        public virtual void onCacheAccount(IAccount a)
        { 
        
        }
        #endregion



    }
}
