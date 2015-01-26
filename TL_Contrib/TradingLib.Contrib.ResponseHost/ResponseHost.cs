using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.ResponseHost
{
    [ContribAttr(ResponseServer.ContribName, "策略托管宿主服务", "用于运行服务端策略,执行程序化交易")]
    public partial class ResponseServer : ContribSrvObject, IContrib
    {
        const string ContribName = "ResponseHost";
        public ResponseServer()
            : base(ResponseServer.ContribName)
        { 
            
        }

        System.Timers.Timer _timer = null;

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            debug("ResponseServer loaded....", QSEnumDebugLevel.INFO);

            Tracker.ResponseTemplateTracker.LoadResponseTemplate();
            Tracker.ResponseTracker.LoadResponse();

            TLCtxHelper.EventIndicator.GotTickEvent += new TickDelegate(EventIndicator_GotTickEvent);
            TLCtxHelper.EventIndicator.GotOrderEvent += new OrderDelegate(EventIndicator_GotOrderEvent);
            TLCtxHelper.EventIndicator.GotFillEvent += new FillDelegate(EventIndicator_GotFillEvent);
            TLCtxHelper.EventIndicator.GotCancelEvent += new LongDelegate(EventIndicator_GotCancelEvent);
        }
        bool _started = false;

        void EventIndicator_GotCancelEvent(long val)
        {
            if (!_started) return;
        }

        void EventIndicator_GotFillEvent(Trade fill)
        {
            if (!_started) return;
            ResponseWrapper resp = Tracker.ResponseTracker[fill.Account];
            if (resp != null)
            {
                try
                {
                    resp.Wrapper.OnFill(fill);
                }
                catch (Exception ex)
                {
                    debug("response onfill error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }

        void EventIndicator_GotOrderEvent(Order order)
        {
            if (!_started) return;
            ResponseWrapper resp = Tracker.ResponseTracker[order.Account];
            if (resp != null)
            {
                try
                {
                    resp.Wrapper.OnOrder(order);
                }
                catch (Exception ex)
                {
                    debug("response onorder error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }

        void EventIndicator_GotTickEvent(Tick t)
        {
            if (!_started) return;
            foreach (ResponseWrapper resp in Tracker.ResponseTracker)
            {
                try
                {
                    resp.Wrapper.IsSymbolRegisted(t.Symbol);
                    resp.Wrapper.OnTick(t);
                }
                catch (Exception ex)
                {
                    debug("response ontick error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory() { }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (_started) return;
            _started = true;
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(TimeEvent);
                _timer.Interval = 100;
                _timer.Enabled = true;
                _timer.Start();
            }
        }
        void TimeEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            foreach (ResponseWrapper resp in Tracker.ResponseTracker)
            {
                try
                {
                    resp.Wrapper.OnTimer();
                }
                catch (Exception ex)
                {
                    debug("response ontimer error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }


        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() { }

    }
}
