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
        //protected ConcurrentDictionary<string, LSPositionTracker> PosHold = new ConcurrentDictionary<string, LSPositionTracker>();
        //为每个账户映射一个TradeList用于记录实时的成交记录
        protected ConcurrentDictionary<string, ThreadSafeList<Trade>> TradeBook = new ConcurrentDictionary<string, ThreadSafeList<Trade>>();

        #region 持仓创建事件和平仓明细事件
        void NewPositionCloseDetail(PositionCloseDetail detail)
        {
            if (NewPositionCloseDetailEvent != null)
                NewPositionCloseDetailEvent(detail);
        }
        public event Action<PositionCloseDetail> NewPositionCloseDetailEvent;


        void NewPosition(Position pos)
        {
            if (NewPositionEvent != null)
                NewPositionEvent(pos);
        }
        public event Action<Position> NewPositionEvent;
        #endregion


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



        internal void DropAccount(IAccount account)
        { 
            IAccount accremove = null;
            AcctList.TryRemove(account.ID, out accremove);//从帐户列表删除

            OrderTracker otremove = null;
            OrdBook.TryRemove(account.ID, out otremove);//删除委托维护其
            if (otremove != null)
                otremove.Clear();

            LSPositionTracker ptremove = null;
            PosBook.TryRemove(account.ID, out ptremove);//删除持仓维护其
            if (ptremove != null)
                ptremove.Clear();

            ThreadSafeList<Trade> ttremove = null;
            TradeBook.TryRemove(account.ID, out ttremove);//删除成交列表
            if (ttremove != null)
                ttremove.Clear();

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
            baseacc.TKOrder = ot;

            //3.添加账户对应的仓位管理器
            LSPositionTracker pt = new LSPositionTracker(account.ID);
            if (!PosBook.ContainsKey(account.ID))
            {
                //pt.DefaultAccount = account.ID;
                PosBook.TryAdd(account.ID, pt);
                pt.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(NewPositionCloseDetail);
                pt.NewPositionEvent += new Action<Position>(NewPosition);
            }
            baseacc.TKPosition = pt;

            //这里单独给出两个一个昨日持仓维护器 用于获得交易帐户的昨日持仓数据，这里可以考虑从Position自带的昨日持仓明细获得昨日持仓汇总信息
            //LSPositionTracker ydpt = new LSPositionTracker();
            //if (!PosHold.ContainsKey(account.ID))
            //{
            //    ydpt.DefaultAccount = account.ID;
            //    PosHold.TryAdd(account.ID, ydpt);
            //}
            //baseacc.TKYdPosition = ydpt;


            //4.添加账户对应的成交管理器
            ThreadSafeList<Trade> tt = new ThreadSafeList<Trade>();
            if (!TradeBook.ContainsKey(account.ID))
                TradeBook.TryAdd(account.ID, tt);
            baseacc.TKTrade = tt;

            
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
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            OrdBook.Clear();
            PosBook.Clear();
            TradeBook.Clear();
            //PosHold.Clear();

        }


        /// <summary>
        /// 重置
        /// </summary>
        public void ResetAccount(IAccount account)
        {
            account.Reset();//结算后要对account进行reset 包括出入金数据 同时将相关标识复位
            //昨日权益以及出入金数据从数据库重新加载

            //清空交易帐户的当日交易记录
            OrdBook[account.ID].Clear();
            PosBook[account.ID].Clear();
            //PosHold[account.ID].Clear();
            TradeBook[account.ID].Clear();

        }


        #region 响应交易对象
        /// <summary>
        /// 从数据库加载昨日持仓明细数据 
        /// </summary>
        /// <param name="pos"></param>
        internal void GotPosition(PositionDetail pos)
        {
            //将昨持仓填充到对应交易账户的仓位管理器中
            PosBook[pos.Account].GotPosition(pos);
        }

        /// <summary>
        /// 记录委托
        /// </summary>
        /// <param name="order"></param>
        internal void GotOrder(Order order)
        {
            OrdBook[order.Account].GotOrder(order);//分账户记录
        }

        /// <summary>
        /// 记录成交
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
