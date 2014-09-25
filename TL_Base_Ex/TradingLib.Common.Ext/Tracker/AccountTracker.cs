using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 帐户管理器
    /// 用于维护帐户以及与帐户相关的交易信息
    /// 
    /// </summary>
    public class AccountTracker:BaseSrvObject
    {
        protected ConcurrentDictionary<string, IAccount> AcctList = new ConcurrentDictionary<string, IAccount>();
        //为每个账户映射一个OrderTracker用于跟踪该账户的Order
        protected ConcurrentDictionary<string, OrderTracker> OrdBook = new ConcurrentDictionary<string, OrderTracker>();
        //为每个账户映射一个PositionTracker用户维护该Account的Position
        protected ConcurrentDictionary<string, LSPositionTracker> PosBook = new ConcurrentDictionary<string, LSPositionTracker>();
        //为每个账户映射一个昨日持仓数据
        protected ConcurrentDictionary<string, LSPositionTracker> PosHold = new ConcurrentDictionary<string, LSPositionTracker>();
        //为每个账户映射一个TradeList用于记录实时的成交记录
        protected ConcurrentDictionary<string, ThreadSafeList<Trade>> TradeBook = new ConcurrentDictionary<string, ThreadSafeList<Trade>>();

        /// <summary>
        /// 获得所有帐户对象
        /// </summary>
        public IAccount[] Accounts
        {
            get
            {
                return AcctList.Values.ToArray();
            }
        }


        /// <summary>
        /// 按Account帐号获得帐号对象
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IAccount this[string account]
        {
            get
            {
                if (string.IsNullOrEmpty(account)) return null;
                IAccount ac = null;
                AcctList.TryGetValue(account, out ac);
                return ac;
            }
        }

        /// <summary>
        /// 查询某个userid下的某个类型的交易帐户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public IAccount QryAccount(int uid, QSEnumAccountCategory category)
        {
            return AcctList.Values.FirstOrDefault(t => (t.UserID == uid && t.Category == category));
        }

        /// <summary>
        /// 为accouont生成交易记录内存数据结构
        /// </summary>
        /// <param name="account"></param>
        internal void CacheAccount(IAccount account)
        {
            AccountBase baseacc = account as AccountBase;
            if (baseacc == null)
            {
                return;
            }
            //1.添加到帐户列表
            AcctList.TryAdd(account.ID, account);

            //2.添加账户对应的委托管理器
            OrderTracker ot = new OrderTracker();
            if (!OrdBook.ContainsKey(account.ID))
                OrdBook.TryAdd(account.ID,ot);
            baseacc.Orders = ot;

            //3.添加账户对应的仓位管理器
            LSPositionTracker pt = new LSPositionTracker();
            if (!PosBook.ContainsKey(account.ID))
            {
                pt.DefaultAccount = account.ID;
                PosBook.TryAdd(account.ID, pt);
            }
            baseacc.Positions = pt;

            LSPositionTracker ydpt = new LSPositionTracker();
            if (!PosHold.ContainsKey(account.ID))
            {
                ydpt.DefaultAccount = account.ID;
                PosHold.TryAdd(account.ID, ydpt);
            }
            baseacc.Positions = ydpt;


            //4.添加账户对应的成交管理器
            ThreadSafeList<Trade> tt = new ThreadSafeList<Trade>();
            if (!TradeBook.ContainsKey(account.ID))
                TradeBook.TryAdd(account.ID, tt);
            baseacc.Trades = tt;

        }

        public void Clear()
        {
            OrdBook.Clear();
            PosBook.Clear();
            TradeBook.Clear();
            PosHold.Clear();

        }

        public bool HaveAccount(string account)
        {
            if (AcctList.ContainsKey(account))
                return true;
            else
                return false;
        }

        public bool HaveAccount(string account, out IAccount acc)
        {
            acc = null;
            if (AcctList.TryGetValue(account, out acc))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 返回某个帐户是否有持仓
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool AnyPosition(string account)
        {
            if (!HaveAccount(account)) return false;//如果没有该账户则直接返回
            foreach (Position pos in this.GetPositionBook(account))//遍历该账户的所有仓位 若不是空仓则市价平仓
            {
                if (!pos.isFlat)
                {
                    return true;
                }
            }
            
            return false;
        }

        public bool HaveLongPosition(string account)
        {
            if (!HaveAccount(account)) return false;//如果没有该账户则直接返回
            return this.GetPositionBook(account).HaveLongPosition;
        }

        public bool HaveShortPosition(string account)
        {
            if (!HaveAccount(account)) return false;//如果没有该账户则直接返回
            return this.GetPositionBook(account).HaveShortPosition;
        }


        #region 获得帐户交易信息管理器
        public ThreadSafeList<Trade> GetTradeBook(string account)
        {
            try
            {
                return TradeBook[account];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public OrderTracker GetOrderBook(string account)
        {
            try
            {
                return OrdBook[account];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public LSPositionTracker GetPositionBook(string account)
        {
            try
            {
                return PosBook[account];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public LSPositionTracker GetPositionHoldBook(string account)
        {
            try
            {
                return PosHold[account];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion





        #region 获得帐户交易信息
        /// <summary>
        /// 获得某个用户的所有隔夜持仓
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position[] GetPositionHold(string account)
        {
            List<Position> l = new List<Position>();
            if (!HaveAccount(account)) return l.ToArray();
            return PosHold[account].ToArray();
        }

        /// <summary>
        /// 获得某个用户的所有持仓
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position[] GetPositions(string account)
        {
            List<Position> l = new List<Position>();
            if (!HaveAccount(account)) return l.ToArray();
            return PosBook[account].ToArray();
        }

        /// <summary>
        /// 返回净持仓
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position[] GetNetPositions(string account)
        {
            List<Position> l = new List<Position>();
            if (!HaveAccount(account)) return l.ToArray();
            return PosBook[account].ToNetArray();
        }
        /// <summary>
        /// 获得某个帐户所有委托
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Order[] GetOrders(string account)
        {
            List<Order> l = new List<Order>();
            if (!HaveAccount(account)) return l.ToArray();
            return OrdBook[account].ToArray();
        }

        /// <summary>
        /// 获得某个帐户的所有成交
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Trade[] GetTrades(string account)
        {
            ThreadSafeList<Trade> flist = new ThreadSafeList<Trade>();
            if (!HaveAccount(account)) return flist.ToArray();
            return TradeBook[account].ToArray();
        }

        /// <summary>
        /// 获得某个帐户的所有取消
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public long[] GetCancels(string account)
        {
            List<long> clist = new List<long>();
            if (!HaveAccount(account)) return clist.ToArray();
            foreach (Order o in OrdBook[account])
            {
                if (OrdBook[account].isCanceled(o.id))
                    clist.Add(o.id);
            }
            return clist.ToArray();
        }

        /// <summary>
        /// 获得某个帐户某个合约的持仓
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal Position GetPosition(string account, string symbol,bool side)
        {
            return PosBook[account][symbol, account, side];
        }

        /// <summary>
        /// 如果不分方向 则获得的就是净持仓
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal Position GetPosition(string account, string symbol)
        {
            return PosBook[account][symbol, account];
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void ResetAccount(IAccount account)
        {
            AccountBase wa = (account as AccountBase);
            wa.LastEquity = 0;//将昨日权益归0 
            wa.CashIn = 0;//归零日内出入金记录
            wa.CashOut = 0;
            //昨日权益以及出入金数据从数据库重新加载
            account.Reset();//结算后要对account进行reset 用于将相关标识复位
            //清空交易帐户的当日交易记录
            OrdBook[account.ID].Clear();
            PosBook[account.ID].Clear();
            PosHold[account.ID].Clear();
            TradeBook[account.ID].Clear();

        }
        #endregion




        #region 响应交易对象
        /// <summary>
        /// 从数据库加载昨日持仓数据 然后加载到系统中 此时持仓需要加载到隔夜持仓管理器与当前累计持仓管理器
        /// </summary>
        /// <param name="pos"></param>
        internal void GotPosition(Position pos)
        {
            //将昨持仓填充到对应交易账户的仓位管理器中
            PosBook[pos.Account].GotPosition(pos);
            //将昨日持仓填充到账户对应的昨日持仓管理器中
            PosHold[pos.Account].GotPosition(pos);
        }

        /// <summary>
        /// 获得委托 用于记录帐户记录
        /// </summary>
        /// <param name="order"></param>
        internal void GotOrder(Order order)
        {
            OrdBook[order.Account].GotOrder(order);//分账户记录
        }

        /// <summary>
        /// 获得成交 用户记录成交记录并反映到当前持仓变化
        /// </summary>
        /// <param name="fill"></param>
        internal void GotFill(Trade fill)
        {
            OrdBook[fill.Account].GotFill(fill);
            PosBook[fill.Account].GotFill(fill);
            TradeBook[fill.Account].Add(fill);
        }

        /// <summary>
        /// 获得取消 用于取消委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        internal void GotCancel(string account,long id)
        {
            OrdBook[account].GotCancel(id);//分账户记录
        }

        /// <summary>
        /// 获得实时行情数据,用于更新持仓的价格信息和浮动盈亏
        /// </summary>
        /// <param name="k"></param>
        internal void GotTick(Tick k)
        {
            foreach (LSPositionTracker pt in PosBook.Values)
            {
                pt.GotTick(k);
            }
        }

        #endregion


    }
}
