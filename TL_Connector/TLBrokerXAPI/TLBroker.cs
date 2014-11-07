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
    internal class XOrderError
    {
        public XOrderError(XOrderField order,XErrorField error)
        {
            this.Order = order;
            this.Error = error;
        }
        public XOrderField Order;
        public XErrorField Error;
    }
    public class TLBroker : TLBrokerBase,IBroker,IDisposable
    {
        

        TLBrokerProxy _broker;
        public TLBrokerWrapperProxy _wrapper;

        public IBrokerClearCentre ClearCentre { get; set; }


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



        #region 事件
        /// <summary>
        /// 当数据服务器登入成功后调用
        /// </summary>
        public event IConnecterParamDel Connected;
        /// <summary>
        /// 当数据服务器断开后触发事件
        /// </summary>
        public event IConnecterParamDel Disconnected;

        /// <summary>
        /// 当接口有成交数据时 对外触发
        /// </summary>
        public event FillDelegate GotFillEvent;
        /// <summary>
        /// 当接口有委托更新时 对外触发
        /// </summary>
        public event OrderDelegate GotOrderEvent;

        /// <summary>
        /// cancel acknowledgement, order is canceled
        /// </summary>
        public event LongDelegate GotCancelEvent;

        /// <summary>
        /// ordermessage acknowledgement
        /// </summary>
        public event OrderMessageDel GotOrderMessageEvent;


        /// <summary>
        /// 获得当前Tick的市场快照,模拟成交时需要获得当前市场快照用于进行取价操作
        /// </summary>
        public event GetSymbolTickDel GetSymbolTickEvent;

        #endregion



        #region 接口操作

        int _waitnum = 100;
        public void Start()
        {
            debug("Try to start broker:" + this.BrokerToken, QSEnumDebugLevel.INFO);
            //初始化接口
            InitBroker();
            //建立服务端连接
            _wrapper.Connect(ref _srvinfo);
            debug("it is here now ??????????????????????????????????????????????????????????",QSEnumDebugLevel.INFO);
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
            debug("接口:" + this.BrokerToken + "登入成功,可以接受交易请求", QSEnumDebugLevel.MUST);

            _working = true;
            _notifythread = new Thread(ProcessCache);
            _notifythread.IsBackground = true;
            _notifythread.Start();
            //对外触发连接成功事件
            if (Connected != null)
                Connected(this);

        }
        public void Stop()
        { 
            
        }

        public bool IsLive { get { return _working; } }


        #region 委托索引map用于按不同的方式定位委托
        /// <summary>
        /// 本地系统委托ID与委托的map
        /// </summary>
        ConcurrentDictionary<long, Order> platformid_order_map = new ConcurrentDictionary<long, Order>();
        /// <summary>
        /// 通过本地系统id查找对应的委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order PlatformID2Order(long id)
        {
            Order o = null;
            if (platformid_order_map.TryGetValue(id, out o))
            {
                return o;
            }
            return null;
        }

        ConcurrentDictionary<string, Order> localid_order_map = new ConcurrentDictionary<string, Order>();
        /// <summary>
        /// 通过成交对端localid查找委托
        /// 本端向成交端提交委托时需要按一定的方式储存一个委托本地编号,用于远端定位
        /// 具体来讲就是通过该编号可以按一定方法告知成交对端进行撤单
        /// </summary>
        /// <param name="localid"></param>
        /// <returns></returns>
        Order LocalID2Order(string localid)
        {
            Order o = null;
            if (localid_order_map.TryGetValue(localid, out o))
            {
                return o;
            }
            return null;
        }

        /// <summary>
        /// 交易所编号 委托 map
        /// </summary>
        ConcurrentDictionary<string, Order> exchange_order_map = new ConcurrentDictionary<string, Order>();
        string GetExchKey(Order o)
        {
            return o.Exchange + ":" + o.OrderExchID;
        }
        string GetExchKey(ref XTradeField f)
        {
            return f.Exchange + ":" + f.OrderSysID;
        }

        Order ExchKey2Order(string sysid)
        {
            Order o = null;
            if (exchange_order_map.TryGetValue(sysid, out o))
            {
                return o;
            }
            return null;
        }
        #endregion



        public void SendOrder(Order o)
        {
            debug("TLBrokerXAP[" + this.BrokerToken + "]: " + o.GetOrderInfo(), QSEnumDebugLevel.INFO);

            
            XOrderField order = new XOrderField();

            order.ID = o.id.ToString();
            order.Date = o.date;
            order.Time = o.time;
            order.Symbol = o.symbol;
            order.Exchange = o.Exchange;
            order.Side = o.side;
            order.TotalSize = o.TotalSize;
            order.FilledSize = 0;
            order.UnfilledSize = 0;

            order.LimitPrice = (double)o.price;
            order.StopPrice = 0;

            order.OffsetFlag = o.OffsetFlag;

            o.Broker = this.BrokerToken;
            //通过接口发送委托
            string localid = _wrapper.SendOrder(ref order);
            bool success = !string.IsNullOrEmpty(localid);
            if (success)
            {
                //1.将委托加入到接口委托维护列表
                o.LocalID = localid;
                //将委托复制后加入到接口维护的map中
                Order lo = new OrderImpl(o);
                platformid_order_map.TryAdd(o.id, lo);
                localid_order_map.TryAdd(o.LocalID, lo);

                debug("Send Order Success,LocalID:" + localid, QSEnumDebugLevel.INFO);

            }
            else
            {
                debug("Send Order Fail,will notify to client", QSEnumDebugLevel.WARNING);
                o.Status = QSEnumOrderStatus.Reject;
            }
        }

        public void CancelOrder(long oid)
        { 
            
        }


        public void GotTick(Tick k)
        { 
        
        }

        #endregion

        string _brokerPath = "";
        string _brokerName = "";
        string _wrapperPath = "";
        string _wrapperName = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="libPath">接口c++DLL目录</param>
        /// <param name="brokerLibPath">成交接口c++DLL目录</param>
        /// <param name="filename">c++DLL名称</param>
        public TLBroker(string brokerPath, string brokerName, string wrapperPath="libbroker", string wrapperName = "TLBrokerWrapper.dll")
        {

            Util.Debug("WrapperFileName:" + Path.Combine(new string[] { wrapperPath, wrapperName }));
            Util.Debug("BrokerFileName:" + Path.Combine(new string[] { brokerPath, brokerName }));
            _brokerPath = brokerPath;
            _brokerName = brokerName;
            _wrapperPath = wrapperPath;
            _wrapperName = wrapperName;
        }

        void InitBroker()
        {
            //1.初始化非托管接口对象
            _wrapper = new TLBrokerWrapperProxy(_wrapperPath, _wrapperName);
            _broker = new TLBrokerProxy(_brokerPath, _brokerName);

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

        const int buffersize=1000;

        int _sleep = 50;
        //缓存
        RingBuffer<XOrderField> _ordercache = new RingBuffer<XOrderField>(buffersize);
        RingBuffer<XTradeField> _tradecache = new RingBuffer<XTradeField>(buffersize);
        RingBuffer<XOrderError> _ordererrorcache = new RingBuffer<XOrderError>();

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
                        //1.获得本地委托数据 更新相关状态后对外触发
                        Order o = LocalID2Order(order.LocalID);
                        if (o != null)//本地记录了该委托 更新数量 状态 并对外发送
                        {
                            o.Status = order.OrderStatus;//更新委托状态
                            o.comment = order.StatusMsg;//填充状态信息
                            o.Filled = order.FilledSize;//成交数量
                            o.size = order.UnfilledSize * (o.side ? 1 : -1);//更新当前数量
                            
                            o.OrderExchID = order.OrderSysID;//更新交易所委托编号

                           
                            if (!string.IsNullOrEmpty(o.OrderExchID))//如果orderexchid存在 则加入对应的键值
                            {
                                string exchkey = GetExchKey(o);
                                Util.Debug("order exchange is not emty,try to insert into exch_order_map," + exchkey);
                                //如果不存在该委托则加入该委托
                                if (!exchange_order_map.Keys.Contains(exchkey))
                                {
                                    exchange_order_map.TryAdd(exchkey, o);
                                }
                                
                            }
                            if (GotOrderEvent != null)
                                GotOrderEvent(o);
                        }
                    }
                    //发送委托错误回报
                    while (!_ordercache.hasItems && _ordererrorcache.hasItems)
                    {

                    }
                    //发送成交回报
                    while (!_ordererrorcache.hasItems && !_ordererrorcache.hasItems && _tradecache.hasItems)
                    {
                        XTradeField trade = _tradecache.Read();
                        string exchkey = GetExchKey(ref trade);
                        Order o = ExchKey2Order(exchkey);
                        //
                        if (o != null)
                        {
                            Trade fill = (Trade)(new OrderImpl(o));
                            fill.xsize = trade.Size * (trade.Side ? 1 : -1);
                            fill.xprice = (decimal)trade.Price;

                            fill.xdate = trade.Date;
                            fill.xtime = trade.Time;

                            fill.Broker = this.BrokerToken;
                            fill.OrderExchID = o.OrderExchID;
                            fill.BrokerKey = trade.TradeID;


                            if (GotFillEvent != null)
                                GotFillEvent(fill);
                        }

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

        #region 底层事件处理
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

        

    }
}
