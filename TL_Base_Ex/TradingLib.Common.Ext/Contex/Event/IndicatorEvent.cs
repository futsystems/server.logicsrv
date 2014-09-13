using System;
using TradingLib.API;

namespace TradingLib.Common
{
        /// <summary>
        /// 交易过程中触发的事件,用于在系统底层逻辑处理完毕后向扩展模块进行事件传递
        /// </summary>
        public class IndicatorEvent
        {
            /// <summary>
            /// 系统内行情数据
            /// </summary>
            public event TickDelegate GotTickEvent;
            /// <summary>
            /// 系统底层获得一个委托()
            /// </summary>
            public event OrderDelegate GotOrderEvent;
            /// <summary>
            /// 系统底层获得一个委托
            /// </summary>
            public event LongDelegate GotCancelEvent;
            /// <summary>
            /// 系统底层获得一个成交
            /// </summary>
            public event FillDelegate GotFillEvent;
            /// <summary>
            /// 系统底层获得一个持仓变更
            /// </summary>
            //public event PositionDelegate GotPositionNotifyEvent;
            /// <summary>
            /// 交易系统某个交易回合结束
            /// </summary>
            public event IPositionRoundDel GotPositionClosedEvent;

            internal void FireTickEvent(Tick k)
            {
                if (GotTickEvent != null)
                    GotTickEvent(k);
            }

            internal void FireOrderEvent(Order o)
            {
                if (GotOrderEvent != null)
                    GotOrderEvent(o);
            }

            internal void FireCancelEvent(long oid)
            {
                if (GotCancelEvent != null)
                    GotCancelEvent(oid);
            }

            internal void FireFillEvent(Trade f)
            {
                if (GotFillEvent != null)
                    GotFillEvent(f);
            }

            internal void FirePositionRoundClosed(IPositionRound pr)
            {
                if (GotPositionClosedEvent != null)
                    GotPositionClosedEvent(pr);
            }
        }
}
