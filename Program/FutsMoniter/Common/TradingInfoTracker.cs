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
        /// <summary>
        /// 恢复开始
        /// </summary>
        RESUMEBEGIN,
        /// <summary>
        /// 恢复结束
        /// </summary>
        RESUMEEND,
        /// <summary>
        /// 请求恢复 本地
        /// </summary>
        RESUMEREQUEST,
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
        public void GotHoldPosition(PositionDetail pos)
        {
            if (Account.Account != pos.Account) return;
            
            PositionTracker.GotPosition(pos);
            HoldPositionTracker.GotPosition(pos);
        }

        /// <summary>
        /// 是否处于恢复数据状态
        /// </summary>
        public bool IsInResume
        {
            get
            {
                if (status == QSEnumInfoTrackerStatus.RESUMEEND || status == QSEnumInfoTrackerStatus.UNKNOWN)
                    return false;
                return true;
            }
        }


        /// <summary>
        /// 获得服务端回报 获得开始恢复数据标记
        /// </summary>
        /// <param name="account"></param>
        public void StartResume(IAccountLite account)
        {
            debug("set account:" + account.Account);
            Account = account;
            status = QSEnumInfoTrackerStatus.RESUMEBEGIN;
        }

        /// <summary>
        /// 获得服务端回报 获得结束恢复数据标记
        /// </summary>
        public void EndResume()
        {
            status = QSEnumInfoTrackerStatus.RESUMEEND;
        }

        /// <summary>
        /// 请求恢复某个交易帐户的数据
        /// </summary>
        /// <param name="account"></param>
        public void RequetResume(string account)
        {
            status = QSEnumInfoTrackerStatus.RESUMEREQUEST;
            //请求恢复交易帐户交易记录
            Globals.TLClient.ReqResumeAccount(account);
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
