using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter;

namespace TradingLib.Common
{
    public enum QSEnumInfoTrackerStatus
    { 
        UNKNOWN,
        RESUMEBEGIN,
        RESUMEEND,
    }

    public class TradingInfoTracker
    {


        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }


        public OrderTracker OrderTracker { get; set; }
        public LSPositionTracker PositionTracker { get; set; }
        public LSPositionTracker HoldPositionTracker { get; set; }
        public ThreadSafeList<Trade> TradeTracker { get; set; }

        public IAccountLite Account { get; set; }
        QSEnumInfoTrackerStatus status = QSEnumInfoTrackerStatus.UNKNOWN;
        public QSEnumInfoTrackerStatus Status { get { return status; } }
        public TradingInfoTracker()
        {
            OrderTracker = new OrderTracker();
            PositionTracker = new LSPositionTracker();
            HoldPositionTracker = new LSPositionTracker();
            TradeTracker = new ThreadSafeList<Trade>();
            Account = new AccountLite();
        }

        /// <summary>
        /// 获得持仓回报
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            if (Account.Account != o.Account) return;
            
            OrderTracker.GotOrder(o);
        }


        /// <summary>
        /// 获得成交回报
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f)
        {
            if (Account.Account != f.Account) return;

            OrderTracker.GotFill(f);
            PositionTracker.GotFill(f);
            TradeTracker.Add(f);

        }

        public void GotTick(Tick k)
        {
            PositionTracker.GotTick(k);
        }

        /// <summary>
        /// 获得隔夜持仓数据
        /// </summary>
        /// <param name="pos"></param>
        public void GotHoldPosition(Position pos)
        {
            if (Account.Account != pos.Account) return;
            
            PositionTracker.GotPosition(pos);
            HoldPositionTracker.GotPosition(pos);
        }


        public void StartResume(IAccountLite account)
        {
            debug("set account:" + account.Account);
            Account = account;
            status = QSEnumInfoTrackerStatus.RESUMEBEGIN;
        }

        public void EndResume()
        {
            status = QSEnumInfoTrackerStatus.RESUMEEND;
        }


        public bool IsReady(string account)
        {
            if (string.IsNullOrEmpty(account)) return false;
            if (account == Account.Account && status == QSEnumInfoTrackerStatus.RESUMEEND) return true;
            return false;
            
        }
        /// <summary>
        /// 清空所有历史数据
        /// </summary>
        public void Clear()
        {
            OrderTracker.Clear();
            HoldPositionTracker.Clear();
            PositionTracker.Clear();
            TradeTracker.Clear();
        }
    }
}
