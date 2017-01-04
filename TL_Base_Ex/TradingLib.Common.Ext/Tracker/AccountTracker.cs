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
        private ConcurrentDictionary<string, OrderTracker> OrdBook = new ConcurrentDictionary<string, OrderTracker>();
        private ConcurrentDictionary<string, LSPositionTracker> PosBook = new ConcurrentDictionary<string, LSPositionTracker>();
        private ConcurrentDictionary<string, TradeTracker> TradeBook = new ConcurrentDictionary<string, TradeTracker>();

        private ConcurrentDictionary<string, BOOrderTracker> BOOrderBook = new ConcurrentDictionary<string, BOOrderTracker>();

        void NewPositionCloseDetail(Trade close,PositionCloseDetail detail)
        {
            if (NewPositionCloseDetailEvent != null)
                NewPositionCloseDetailEvent(close,detail);
        }
        public event Action<Trade,PositionCloseDetail> NewPositionCloseDetailEvent;

        /// <summary>
        /// 新的持仓明细生成事件
        /// </summary>
        void NewPositionDetail(Trade open, PositionDetail detail)
        {
            if (NewPositionDetailEvent != null)
            {
                NewPositionDetailEvent(open, detail);
            }
        }
        public event Action<Trade, PositionDetail> NewPositionDetailEvent;

        void NewPosition(Position pos)
        {
            if (NewPositionEvent != null)
                NewPositionEvent(pos);
        }
        public event Action<Position> NewPositionEvent;


        /// <summary>
        /// 删除某个账户
        /// </summary>
        /// <param name="account"></param>
        internal void DropAccount(IAccount account)
        { 
            OrderTracker otremove = null;
            OrdBook.TryRemove(account.ID, out otremove);//删除委托维护器
            if (otremove != null)
                otremove.Clear();

            LSPositionTracker ptremove = null;
            PosBook.TryRemove(account.ID, out ptremove);//删除持仓维护器
            if (ptremove != null)
                ptremove.Clear();

            TradeTracker ttremove = null;
            TradeBook.TryRemove(account.ID, out ttremove);//删除成交列器
            if (ttremove != null)
                ttremove.Clear();

            BOOrderTracker bootremove = null;
            BOOrderBook.TryRemove(account.ID, out bootremove);//删除BO委托维护器

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

            //2.添加账户对应的委托管理器
            OrderTracker ot = new OrderTracker();
            if (!OrdBook.ContainsKey(account.ID))
                OrdBook.TryAdd(account.ID,ot);
            baseacc.TKOrder = ot;

            //3.添加账户对应的仓位管理器
            LSPositionTracker pt = new LSPositionTracker(account.ID);
            if (!PosBook.ContainsKey(account.ID))
            {
                PosBook.TryAdd(account.ID, pt);
                //绑定仓位管理器中的相关事件
                pt.NewPositionCloseDetailEvent += new Action<Trade,PositionCloseDetail>(NewPositionCloseDetail);
                pt.NewPositionDetailEvent += new Action<Trade, PositionDetail>(NewPositionDetail);
                pt.NewPositionEvent += new Action<Position>(NewPosition);
            }
            baseacc.TKPosition = pt;

            //4.添加账户对应的成交管理器
            TradeTracker tt = new TradeTracker();
            if (!TradeBook.ContainsKey(account.ID))
                TradeBook.TryAdd(account.ID, tt);
            baseacc.TKTrade = tt;

            BOOrderTracker boot = new BOOrderTracker();
            if (!BOOrderBook.ContainsKey(account.ID))
                BOOrderBook.TryAdd(account.ID, boot);
            baseacc.TKBOOrder = boot;
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            OrdBook.Clear();
            PosBook.Clear();
            TradeBook.Clear();
            BOOrderBook.Clear();
        }


        /// <summary>
        /// 重置某个交易帐户
        /// 清空委托维护器 持仓维护器 成交维护器
        /// </summary>
        public void Reset(IAccount account)
        {
            //清空交易帐户的当日交易记录
            OrdBook[account.ID].Clear();
            PosBook[account.ID].Clear();
            TradeBook[account.ID].Clear();
            BOOrderBook[account.ID].Clear();
        }
        
        /// <summary>
        /// 重新生成某个交易账户的持仓数据
        /// </summary>
        /// <param name="account"></param>
        public void Reload(IAccount account)
        {
            LSPositionTracker tk = PosBook[account.ID];
            PositionDetail[] pdlist = tk.YDPositionDetails.ToArray();

            //重置持仓维护器
            tk.Clear();
            tk.InReCalculate = true;

            foreach (var pd in pdlist)
            {
                tk.GotPosition(pd);
            }
            bool accept = false;
            foreach (var fill in TradeBook[account.ID])
            {
                tk.GotFill(fill, out accept);
            }
            tk.InReCalculate = false;
        }


        #region 响应交易对象
        /// <summary>
        /// 从数据库加载昨日持仓明细数据 
        /// </summary>
        /// <param name="pos"></param>
        internal void GotPosition(PositionDetail pos)
        {
            PosBook[pos.Account].GotPosition(pos);
        }

        /// <summary>
        /// 记录委托
        /// </summary>
        /// <param name="order"></param>
        internal void GotOrder(Order order)
        {
            OrdBook[order.Account].GotOrder(order);
        }


        internal void GotOrder(BinaryOptionOrder order)
        {
            BOOrderBook[order.Account].GotOrder(order);
        }
        /// <summary>
        /// 记录成交
        /// </summary>
        /// <param name="fill"></param>
        internal void GotFill(Trade fill,out bool accept)
        {

            PosBook[fill.Account].GotFill(fill,out accept);
            //如果成交可接受 则用成交数据更新委托与成交列表
            if (accept)
            {
                OrdBook[fill.Account].GotFill(fill);
                TradeBook[fill.Account].GotFill(fill);
            }
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
