using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 交易账户信号
    /// </summary>
    public class SignalWrapperAccount:ISignal
    {

        IAccount _account = null;

        SignalConfig _cfg = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cfg"></param>
        public void Init(SignalConfig cfg)
        {
            _cfg = cfg;
            _account = TLCtxHelper.ModuleAccountManager[cfg.SignalToken];
            if (_account == null)
            {
                throw new ArgumentNullException("Signal Account can not be null");
            }
            _account.GotFillEvent += new FillDelegate(_account_GotFillEvent);
            _account.GotOrderEvent += new OrderDelegate(_account_GotOrderEvent);
            _account.GotPositionCloseDetailEvent += new Action<Trade, PositionCloseDetail>(_account_GotPositionCloseDetailEvent);
            _account.GotPositionDetailEvent += new Action<Trade, PositionDetail>(_account_GotPositionDetailEvent);
        }

        void _account_GotPositionDetailEvent(Trade arg1, PositionDetail arg2)
        {
            PositionEvent e = new PositionEvent();
            e.EventType = QSEnumPositionEventType.EntryPosition;
            e.PositionEntry = arg2;
            if (GotPositionEvent != null)
            {
                GotPositionEvent(this, arg1, e);
            }
        }

        void _account_GotPositionCloseDetailEvent(Trade arg1, PositionCloseDetail arg2)
        {
            PositionEvent e = new PositionEvent();
            e.EventType = QSEnumPositionEventType.ExitPosition;
            e.PositionExit = arg2;
            if (GotPositionEvent != null)
            {
                GotPositionEvent(this, arg1, e);
            }
        }

        void _account_GotOrderEvent(Order order)
        {
            if (GotOrderEvent != null)
                GotOrderEvent(order);
        }

        void _account_GotFillEvent(Trade t)
        {
            if (GotFillEvent != null)
            {
                GotFillEvent(t);
            }
        }

        public int ID { get { return _cfg.ID; } }
        public string Token { get { return _account.ID; } }

        public IEnumerable<Trade> Trades { get { return _account.Trades; } }

        public IEnumerable<Order> Orders { get { return _account.Orders; } }

        public IEnumerable<Position> Positions { get { return _account.Positions; } }

        public IEnumerable<Position> PositionsLong { get { return _account.PositionsLong; } }

        public IEnumerable<Position> PositionsShort { get { return _account.PositionsShort; } }



        /// <summary>
        /// 信号委托事件
        /// </summary>
        public event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 信号成交事件
        /// </summary>
        public event FillDelegate GotFillEvent;

        /// <summary>
        /// 持仓变动事件
        /// </summary>
        public event Action<ISignal, Trade, IPositionEvent> GotPositionEvent;
        
    }
}
