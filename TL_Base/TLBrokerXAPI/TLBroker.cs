using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text;
using TradingLib.BrokerXAPI.Interop;
using System.Runtime.InteropServices;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.BrokerXAPI
{

    public abstract class TLBroker :TLBrokerBase,IBroker,IDisposable
    {
        TLBrokerProxy _broker;
        TLBrokerWrapperProxy _wrapper;

        #region 成交接口交易数据
        /// <summary>
        /// 获得成交接口所有委托
        /// </summary>
        public virtual IEnumerable<Order> Orders { get { return new List<Order>(); } }

        /// <summary>
        /// 获得成交接口所有成交
        /// </summary>
        public virtual IEnumerable<Trade> Trades { get { return new List<Trade>(); } }

        /// <summary>
        /// 获得成交接口所有持仓
        /// </summary>
        public virtual IEnumerable<Position> Positions { get { return new List<Position>(); } }


        #endregion


        private bool _disposed;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                //_broker.Dispose();
                //_wrapper.Dispose();
                if (_working)
                {
                    Stop();
                }
            }

            _disposed = true;
        }


        int _waitnum = 10;


        public virtual void Start()
        {
            string msg = string.Empty;
            bool success = this.Start(out msg);
            Util.Debug("Start Broker:" + this.Token + " " + (success ? "成功" : "失败") + " msg:" + msg,success==false?QSEnumDebugLevel.ERROR:QSEnumDebugLevel.INFO);
        }
        /// <summary>
        /// 启动接口
        /// 启动接口时,同时进行非托管资源的创建 也就是每次启动时都是新的接口实例
        /// </summary>
        public virtual bool Start(out string msg)
        {
            msg = string.Empty;
            debug("Try to start broker:" + this.Token, QSEnumDebugLevel.INFO);
            //初始化参数
            ParseConfigInfo();
            //初始化接口
            InitBroker();
            //建立服务端连接
            _wrapper.Connect(ref _srvinfo);
            int i=0;
            while (!_connected && i < _waitnum)
            {
                i++;
                Util.Debug(string.Format("#{0} wait connected....",i));
                Util.sleep(500);
            }
            if (!_connected)
            {
                msg = "接口连接服务端失败,请检查配置信息";
                debug(msg, QSEnumDebugLevel.ERROR);
                ResetResource();
                return false;
            }

            i=0;
            while (!_loginreply && i < _waitnum)
            {
                i++;
                Util.Debug(string.Format("#{0} wait logined....", i));
                Util.sleep(500);
            }
            if (!_loginreply)
            {
                msg = "登入回报异常,请检查配置信息";
                debug(msg, QSEnumDebugLevel.ERROR);
                ResetResource();
                return false;
            }
            if (!_loginsuccess)
            {
                msg = "登入失败,请检查配置信息";
                debug(msg, QSEnumDebugLevel.WARNING);
                ResetResource();
                return false;
            }
            

            //恢复该接口日内交易数据
            OnResume();

            //启动回报消息通知线程 在另外一个线程中将接口返回的回报进行处理
            _working = true;
            _notifythread = new Thread(ProcessCache);
            _notifythread.IsBackground = true;
            _notifythread.Start();

            msg = "接口:" + this.Token + "登入成功,可以接受交易请求";
            debug(msg, QSEnumDebugLevel.INFO);
            return true;
            //对外触发连接成功事件
        }

        void ResetResource()
        {
            Util.Debug("Release Broker c++ resoure and reset start status",QSEnumDebugLevel.INFO);
            //断开底层接口连接
            _wrapper.Disconnect();
            _wrapper.Dispose();
            _broker.Dispose();

            _wrapper = null;
            _broker = null;
            _connected = false;
            _loginreply = false;
            _loginsuccess = false;
        }
        public virtual void Stop()
        {
            if (!this.IsLive) return;
            _working = false;
            //停止消息发送线程
            Util.WaitThreadStop(_notifythread);

            ResetResource();

            this.DestoryBroker();
        }

        public bool IsLive { get { return _working; } }

        #region 交易接口生成与销毁
        public virtual void InitBroker()
        {
            //1.初始化非托管接口对象
            Util.Debug("WrapperFileName:" + Path.Combine(new string[] { _cfg.Interface.libpath_wrapper, _cfg.Interface.libname_wrapper }));
            Util.Debug("BrokerFileName:" + Path.Combine(new string[] { _cfg.Interface.libpath_broker, _cfg.Interface.libname_broker }));
            _wrapper = new TLBrokerWrapperProxy(_cfg.Interface.libpath_wrapper, _cfg.Interface.libname_wrapper);
            _broker = new TLBrokerProxy(_cfg.Interface.libpath_broker, _cfg.Interface.libname_broker);

            //2.注册接口到wrapper
            _wrapper.Register(_broker);

            //3.绑定回调函数
            _wrapper.OnConnectedEvent += new CBOnConnected(_wrapper_OnConnectedEvent);
            _wrapper.OnDisconnectedEvent += new CBOnDisconnected(_wrapper_OnDisconnectedEvent);
            _wrapper.OnLoginEvent += new CBOnLogin(_wrapper_OnLoginEvent);

            _wrapper.OnRtnOrderEvent += new CBRtnOrder(_wrapper_OnRtnOrderEvent);
            _wrapper.OnRtnOrderErrorEvent += new CBRtnOrderError(_wrapper_OnRtnOrderErrorEvent);
            _wrapper.OnRtnOrderActionErrorEvent += new CBRtnOrderActionError(_wrapper_OnRtnOrderActionErrorEvent);
            _wrapper.OnRtnTradeEvent += new CBRtnTrade(_wrapper_OnRtnTradeEvent);
        }

        

        /// <summary>
        /// 执行对象销毁
        /// </summary>
        public virtual void DestoryBroker()
        {
            

        }
        #endregion

        #region 交易接口操作 下单 撤单 与回报处理 子类覆写

        /// <summary>
        /// 通过成交接口提交委托
        /// </summary>
        /// <param name="o"></param>
        public virtual void SendOrder(Order o)
        {
            
        }

        /// <summary>
        /// 通过成交接口取消委托
        /// </summary>
        /// <param name="oid"></param>
        public virtual void CancelOrder(long oid)
        {
            
            
        }

        /// <summary>
        /// 响应市场行情
        /// </summary>
        /// <param name="k"></param>
        public virtual void GotTick(Tick k)
        { 
        
        }

        /// <summary>
        /// 处理接口返回的委托
        /// </summary>
        /// <param name="order"></param>
        public virtual void ProcessOrder(ref XOrderField order)
        {

            
        }

        /// <summary>
        /// 处理接口返回的成交
        /// </summary>
        /// <param name="trade"></param>
        public virtual void ProcessTrade(ref XTradeField trade)
        {
            
        }

        /// <summary>
        /// 处理接口返回的错误
        /// </summary>
        /// <param name="error"></param>
        public virtual void ProcessOrderError(ref XOrderError error)
        { 
            
        }

        /// <summary>
        /// 处理接口返回的委托操作错误
        /// </summary>
        /// <param name="error"></param>
        public virtual void ProcessOrderActionError(ref XOrderActionError error)
        {
        
        }



        /// <summary>
        /// 启动时登入成功后 恢复日内交易数据
        /// </summary>
        public virtual void OnResume()
        {


        }



        #endregion

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="libPath">接口c++DLL目录</param>
        /// <param name="brokerLibPath">成交接口c++DLL目录</param>
        /// <param name="filename">c++DLL名称</param>
        public TLBroker()
        {

        }


        #region 底层wrapper发送委托或取消委托
        protected bool WrapperSendOrder(ref XOrderField order)
        {
            Util.Debug("~~~~~OrderSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderField)));
            return _wrapper.SendOrder(ref order);
        }

        protected bool WrapperSendOrderAction(ref XOrderActionField action)
        {
            Util.Debug("~~~~~OrderSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderActionField)));
            return _wrapper.SendOrderAction(ref action);
        }
        #endregion

        #region 回报缓存
        const int buffersize=1000;
        int _sleep = 50;
        //缓存
        RingBuffer<XOrderField> _ordercache = new RingBuffer<XOrderField>(buffersize);
        RingBuffer<XTradeField> _tradecache = new RingBuffer<XTradeField>(buffersize);
        RingBuffer<XOrderError> _ordererrorcache = new RingBuffer<XOrderError>(buffersize);
        RingBuffer<XOrderActionError> _orderactionerrorcache = new RingBuffer<XOrderActionError>(buffersize);

        Thread _notifythread = null;
        bool _working = false;

        ManualResetEvent _notifywaiting = new ManualResetEvent(false);

        /// <summary>
        /// 通知处理线程 有新的交易回报
        /// </summary>
        void NewNotify()
        {
            //Util.Debug("notify wiaiting event ....", QSEnumDebugLevel.ERROR);
            if ((_notifythread != null) && (_notifythread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _notifywaiting.Set();
            }
        }

        void ProcessCache()
        {
            while (_working)
            {
                try
                {
                    //发送委托回报
                    while (_ordercache.hasItems)
                    {
                        //Util.Debug("process order in cache....", QSEnumDebugLevel.ERROR);
                        XOrderField order = _ordercache.Read();//获得委托数据
                        ProcessOrder(ref order);
                    }
                    //发送委托错误回报
                    while (!_ordercache.hasItems && _ordererrorcache.hasItems)
                    {
                        XOrderError error = _ordererrorcache.Read();
                        ProcessOrderError(ref error);
                    }
                    //发送委托操作错误回报
                    while (!_ordercache.hasItems && _orderactionerrorcache.hasItems)
                    {
                        XOrderActionError error = _orderactionerrorcache.Read();
                        ProcessOrderActionError(ref error);
                    }
                    //发送成交回报
                    while (!_ordererrorcache.hasItems && !_ordererrorcache.hasItems && _tradecache.hasItems)
                    {
                        XTradeField trade = _tradecache.Read();
                        ProcessTrade(ref trade);

                    }

                    // clear current flag signal
                    _notifywaiting.Reset();
                    // wait for a new signal to continue reading
                    _notifywaiting.WaitOne(_sleep);
                }
                catch (Exception ex)
                {
                    Util.Debug("process cache error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
            Util.Debug("Notify thread stopped...");
        }
        #endregion

        #region Proxy底层事件处理
        bool _connected = false;
        void _wrapper_OnConnectedEvent()
        {
            Util.Debug("-----------TLBroker OnConnectedEvent-----------------------", QSEnumDebugLevel.WARNING);
            _connected = true;
            //请求登入
            _wrapper.Login(ref _usrinfo);
        }

        bool _loginreply = false;
        bool _loginsuccess = false;

        void _wrapper_OnLoginEvent(ref XRspUserLoginField pRspUserLogin)
        {
            Util.Debug("-----------TLBroker OnLoginEvent-----------------------", QSEnumDebugLevel.WARNING);
            _loginreply = true;
            if (pRspUserLogin.ErrorID == 0)
            {
                _loginsuccess = true;
                debug("交易帐户登入成功", QSEnumDebugLevel.INFO);
            }
            else
            {
                _loginsuccess = false;
                debug("交易帐户登入失败", QSEnumDebugLevel.INFO);
            }
        }

        void _wrapper_OnDisconnectedEvent()
        {
            Util.Debug("-----------TLBroker OnDisconnectedEvent-----------------------", QSEnumDebugLevel.WARNING);
        }



        void _wrapper_OnRtnOrderEvent(ref XOrderField pOrder)
        {
            Util.Debug("-----------TLBroker OnRtnOrderEvent-----------------------", QSEnumDebugLevel.WARNING);
            Util.Debug("~~~~~OrderSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderField)));
           
            //Util.Debug(" data:" + pOrder.Date + " exchange:" + pOrder.Exchange + " filledsize:" + pOrder.FilledSize.ToString() + " limitprice:" + pOrder.LimitPrice + " offsetflag:" + pOrder.OffsetFlag.ToString() + " orderid:" + pOrder.ID + " status:" + pOrder.OrderStatus.ToString() + " side:" + pOrder.Side + " stopprice:" + pOrder.StopPrice.ToString() + " symbol:" + pOrder.Symbol + " totalsize:" + pOrder.TotalSize.ToString() + " unfilledsize:" + pOrder.UnfilledSize.ToString() + " statusmsg:" + pOrder.StatusMsg);
            _ordercache.Write(pOrder);
            NewNotify();
        }

        void _wrapper_OnRtnOrderErrorEvent(ref XOrderField pOrder, ref XErrorField pError)
        {
            Util.Debug("-----------TLBroker OnRtnOrderErrorEvent-----------------------", QSEnumDebugLevel.WARNING);
            Util.Debug("~~~~~ErrorSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XErrorField)));
            //Util.Debug("order localid:" + pOrder.LocalID + " errorid:" + pError.ErrorID.ToString() + " errmsg:" + pError.ErrorMsg, QSEnumDebugLevel.MUST);
            //Util.Debug(" data:" + pOrder.Date + " exchange:" + pOrder.Exchange + " filledsize:" + pOrder.FilledSize.ToString() + " limitprice:" + pOrder.LimitPrice + " offsetflag:" + pOrder.OffsetFlag.ToString() + " orderid:" + pOrder.ID + " status:" + pOrder.OrderStatus.ToString() + " side:" + pOrder.Side + " stopprice:" + pOrder.StopPrice.ToString() + " symbol:" + pOrder.Symbol + " totalsize:" + pOrder.TotalSize.ToString() + " unfilledsize:" + pOrder.UnfilledSize.ToString() + " statusmsg:" + pOrder.StatusMsg);
            _ordererrorcache.Write(new XOrderError(pOrder, pError));
        }
        void _wrapper_OnRtnOrderActionErrorEvent(ref XOrderActionField pOrderAction, ref XErrorField pError)
        {
            Util.Debug("-----------TLBroker OrderActionErrorEvent-----------------------", QSEnumDebugLevel.WARNING);
            Util.Debug("~~~~~OrderActionSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderActionField)));
           
            _orderactionerrorcache.Write(new XOrderActionError(pOrderAction, pError));
        }

        void _wrapper_OnRtnTradeEvent(ref XTradeField pTrade)
        {
            Util.Debug("-----------TLBroker OnRtnTradeEvent-----------------------", QSEnumDebugLevel.WARNING);
            Util.Debug("~~~~~TradeSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderActionField)));
           
            //Console.WriteLine("got new trade..???????????????????????");
            //Util.Debug("tradefield commission:" + pTrade.Commission + " date:" + pTrade.Date.ToString() + " exchange:" + pTrade.Exchange + " offsetflag:" + pTrade.OffsetFlag.ToString() + " price:" + pTrade.Price.ToString() + " side:" + pTrade.Side.ToString() + " size:" + pTrade.Size.ToString() + " symbol:" + pTrade.Symbol + " time:" + pTrade.Time + " tradeid:" + pTrade.TradeID +" ordresysid:"+pTrade.OrderSysID +" date:"+pTrade.Date.ToString() +" time:"+pTrade.Time.ToString());
            _tradecache.Write(pTrade);
            NewNotify();
        }
        #endregion



        #region 持仓状态数据与判定
        /// <summary>
        /// 返回所有持仓状态统计数据
        /// </summary>
        public virtual IEnumerable<PositionMetric> PositionMetrics { get { return new List<PositionMetric>(); } }

        /// <summary>
        /// 获得某个合约的持仓状态统计数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual PositionMetric GetPositionMetric(string symbol)
        {
            return null;
        }

        /// <summary>
        /// 返回持仓预计调整量
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual int GetPositionAdjustment(Order o)
        {
            if (o.IsEntryPosition)
                return o.UnsignedSize;
            else
                return o.UnsignedSize * -1;
        }

        #endregion


    }
}
