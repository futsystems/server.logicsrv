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
                _broker.Dispose();
                _wrapper.Dispose();
            }

            _disposed = true;
        }







        

        int _waitnum = 100;
        public virtual void Start()
        {
            debug("Try to start broker:" + this.Token, QSEnumDebugLevel.INFO);
            //初始化接口
            InitBroker();
            //建立服务端连接
            _wrapper.Connect(ref _srvinfo);
            int i=0;
            while (!_connected && i < _waitnum)
            {
                Util.sleep(500);
            }
            if (!_connected)
            {
                debug("接口连接服务端失败,请检查配置信息", QSEnumDebugLevel.ERROR);
                return;
            }
            i=0;
            while (!_loginreply && i < _waitnum)
            {
                Util.sleep(500);
            }
            if (!_loginreply)
            {
                debug("登入回报异常,请检查配置信息", QSEnumDebugLevel.ERROR);
                return;
            }
            if (!_loginsuccess)
            {
                debug("登入失败,请检查配置信息", QSEnumDebugLevel.WARNING);
                return;
            }
            debug("接口:" + this.Token + "登入成功,可以接受交易请求", QSEnumDebugLevel.MUST);

            //恢复该接口日内交易数据
            OnResume();

            //启动回报消息通知线程 在另外一个线程中将接口返回的回报进行处理
            _working = true;
            _notifythread = new Thread(ProcessCache);
            _notifythread.IsBackground = true;
            _notifythread.Start();


            //对外触发连接成功事件


        }
        public virtual void Stop()
        { 
            
        }

        public bool IsLive { get { return _working; } }


        #region 交易接口操作 下单 撤单

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


        public virtual void ProcessOrderError(ref XOrderError error)
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
            _wrapper.OnRtnTradeEvent += new CBRtnTrade(_wrapper_OnRtnTradeEvent);
        }



        protected string WrapperSendOrder(ref XOrderField order)
        {
            return _wrapper.SendOrder(ref order);
        }

        protected bool WrapperSendOrderAction(ref XOrderActionField action)
        {
            return _wrapper.SendOrderAction(ref action);
        }







        #region 回报缓存
        const int buffersize=1000;
        int _sleep = 50;
        //缓存
        RingBuffer<XOrderField> _ordercache = new RingBuffer<XOrderField>(buffersize);
        RingBuffer<XTradeField> _tradecache = new RingBuffer<XTradeField>(buffersize);
        RingBuffer<XOrderError> _ordererrorcache = new RingBuffer<XOrderError>(buffersize);

        Thread _notifythread = null;
        bool _working = false;

        ManualResetEvent _notifywaiting = new ManualResetEvent(false);
        void NewNotify()
        {
            Util.Debug("notify wiaiting event ....", QSEnumDebugLevel.ERROR);
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
                        Util.Debug("process order in cache....", QSEnumDebugLevel.ERROR);
                        XOrderField order = _ordercache.Read();//获得委托数据
                        ProcessOrder(ref order);
                    }
                    //发送委托错误回报
                    while (!_ordercache.hasItems && _ordererrorcache.hasItems)
                    {
                        XOrderError error = _ordererrorcache.Read();
                        ProcessOrderError(ref error);
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
            //Util.Debug(" data:" + pOrder.Date + " exchange:" + pOrder.Exchange + " filledsize:" + pOrder.FilledSize.ToString() + " limitprice:" + pOrder.LimitPrice + " offsetflag:" + pOrder.OffsetFlag.ToString() + " orderid:" + pOrder.ID + " status:" + pOrder.OrderStatus.ToString() + " side:" + pOrder.Side + " stopprice:" + pOrder.StopPrice.ToString() + " symbol:" + pOrder.Symbol + " totalsize:" + pOrder.TotalSize.ToString() + " unfilledsize:" + pOrder.UnfilledSize.ToString() + " statusmsg:" + pOrder.StatusMsg);
            _ordercache.Write(pOrder);
            NewNotify();
        }

        void _wrapper_OnRtnOrderErrorEvent(ref XOrderField pOrder, ref XErrorField pError)
        {
            Util.Debug("-----------TLBroker OnRtnOrderErrorEvent-----------------------", QSEnumDebugLevel.WARNING);
            //Util.Debug("order localid:" + pOrder.LocalID + " errorid:" + pError.ErrorID.ToString() + " errmsg:" + pError.ErrorMsg, QSEnumDebugLevel.MUST);
            //Util.Debug(" data:" + pOrder.Date + " exchange:" + pOrder.Exchange + " filledsize:" + pOrder.FilledSize.ToString() + " limitprice:" + pOrder.LimitPrice + " offsetflag:" + pOrder.OffsetFlag.ToString() + " orderid:" + pOrder.ID + " status:" + pOrder.OrderStatus.ToString() + " side:" + pOrder.Side + " stopprice:" + pOrder.StopPrice.ToString() + " symbol:" + pOrder.Symbol + " totalsize:" + pOrder.TotalSize.ToString() + " unfilledsize:" + pOrder.UnfilledSize.ToString() + " statusmsg:" + pOrder.StatusMsg);
            _ordererrorcache.Write(new XOrderError(pOrder, pError));
        }


        void _wrapper_OnRtnTradeEvent(ref XTradeField pTrade)
        {
            Util.Debug("-----------TLBroker OnRtnTradeEvent-----------------------", QSEnumDebugLevel.WARNING);
            //Console.WriteLine("got new trade..???????????????????????");
            //Util.Debug("tradefield commission:" + pTrade.Commission + " date:" + pTrade.Date.ToString() + " exchange:" + pTrade.Exchange + " offsetflag:" + pTrade.OffsetFlag.ToString() + " price:" + pTrade.Price.ToString() + " side:" + pTrade.Side.ToString() + " size:" + pTrade.Size.ToString() + " symbol:" + pTrade.Symbol + " time:" + pTrade.Time + " tradeid:" + pTrade.TradeID +" ordresysid:"+pTrade.OrderSysID +" date:"+pTrade.Date.ToString() +" time:"+pTrade.Time.ToString());
            _tradecache.Write(pTrade);
            NewNotify();
        }
        #endregion




        #region

        /// <summary>
        /// 启动时登入成功后 恢复日内交易数据
        /// </summary>
        public virtual void OnResume()
        {
           

        }
        #endregion


    }
}
