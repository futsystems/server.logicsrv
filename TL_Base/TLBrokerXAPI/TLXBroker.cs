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
    /// <summary>
    /// XAPI 适配底层c交易接口的 成交通道基类
    /// </summary>
    public abstract class TLXBroker :TLBrokerBase,IBroker,IDisposable
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

        /// <summary>
        /// 返回所有处于有持仓或挂单状态的合约
        /// </summary>
        public IEnumerable<string> WorkingSymbols
        {
            get
            {
                List<string> symlist = new List<string>();
                foreach (Position pos in Positions.Where(p => !p.isFlat))
                {
                    if (!symlist.Contains(pos.Symbol))
                        symlist.Add(pos.Symbol);
                }
                foreach (Order o in Orders.Where(o => o.IsPending()))
                {
                    if (!symlist.Contains(o.Symbol))
                        symlist.Add(o.Symbol);
                }
                return symlist;
            }
        }

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="libPath">接口c++DLL目录</param>
        /// <param name="brokerLibPath">成交接口c++DLL目录</param>
        /// <param name="filename">c++DLL名称</param>
        public TLXBroker()
        {

        }


        public virtual void Start()
        {
            string msg = string.Empty;
            bool success = this.Start(out msg);
            if (success)
            {
                logger.Info("Start Broker:" + this.Token + " " + (success ? "成功" : "失败") + " msg:" + msg);
            }
            else
            {
                logger.Error("Start Broker:" + this.Token + " " + (success ? "成功" : "失败") + " msg:" + msg);
            }
            
        }
        /// <summary>
        /// 启动接口
        /// 启动接口时,同时进行非托管资源的创建 也就是每次启动时都是新的接口实例
        /// </summary>
        public virtual bool Start(out string msg)
        {
            msg = string.Empty;
            logger.Info(string.Format("Start Broker:{0}", this.Token));
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
                logger.Info(string.Format("#{0} wait connected....",i));
                Util.sleep(500);
            }
            if (!_connected)
            {
                msg = "接口连接服务端失败,请检查配置信息";
                logger.Error(msg);
                ResetResource();
                return false;
            }

            i=0;
            while (!_loginreply && i < _waitnum)
            {
                i++;
                logger.Info(string.Format("#{0} wait logined....", i));
                Util.sleep(500);
            }
            if (!_loginreply)
            {
                msg = "登入回报异常,请检查配置信息";
                logger.Error(msg);
                ResetResource();
                return false;
            }
            if (!_loginsuccess)
            {
                msg = "登入失败,请检查配置信息";
                logger.Error(msg);
                ResetResource();
                return false;
            }

            //启动回报消息通知线程 在另外一个线程中将接口返回的回报进行处理
            _working = true;
            _notifythread = new Thread(ProcessCache);
            _notifythread.IsBackground = true;
            _notifythread.Start();

            msg = string.Format("接口:{0}初始化成功,可以接受交易请求",this.Token);
            logger.Info(msg);
            //恢复该接口日内交易数据
            this.OnInit();
            //对外触发连连接成功事件
            this.NotifyConnected();
            return true;
            //对外触发连接成功事件
        }

        /// <summary>
        /// 记录从接口获得的合约数据
        /// 保证金 手续费等
        /// 本地处理成交时 需要计算成交数据
        /// </summary>
        ConcurrentDictionary<string, XSymbol> symbolmap = new ConcurrentDictionary<string, XSymbol>();


        public virtual void Stop()
        {
            if (!this.IsLive) return;
            _working = false;
            //停止消息发送线程
            Util.WaitThreadStop(_notifythread);
            //重置接口对象
            ResetResource();
            //执行接口释放操作
            this.OnDisposed();
            //对外通知接口断开
            this.NotifyDisconnected();
        }

        /// <summary>
        /// 释放底层接口对象
        /// </summary>
        void ResetResource()
        {
            logger.Info("Release Broker C++ Native Resoure,Reset Status");
            //断开底层接口连接
            _wrapper.Disconnect();
            //先释放_broker _broker日志输出依赖于 _wrapper的日志输出/
            _broker.Dispose();
            //最后释放wrapper
            _wrapper.Dispose();

            _wrapper = null;
            _broker = null;
            _connected = false;
            _loginreply = false;
            _loginsuccess = false;
            logger.Info(string.Format("{0} Resource Released Success", this.GetType().Name));
        }

        /// <summary>
        /// 接口是否处于工作状态
        /// </summary>
        public bool IsLive { get { return _working; } }



        #region 交易接口生成与销毁
        /// <summary>
        /// 初始化交易接口
        /// </summary>
        public virtual void InitBroker()
        {
            //1.初始化非托管接口对象
            logger.Debug("WrapperFileName:" + Path.Combine(new string[] { _cfg.Interface.libpath_wrapper, _cfg.Interface.libname_wrapper }));
            logger.Debug("BrokerFileName:" + Path.Combine(new string[] { _cfg.Interface.libpath_broker, _cfg.Interface.libname_broker }));
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
            //_wrapper.OnSymbolEvent += new CBOnSymbol(_wrapper_OnSymbolEvent);
            //_wrapper.OnAccountInfoEvent += new CBOnAccountInfo(_wrapper_OnAccountInfoEvent);

            //_wrapper.OnQryOrderEvent += new CBOnQryOrder(_wrapper_OnQryOrderEvent);
            //_wrapper.OnQryTradeEvent += new CBOnQryTrade(_wrapper_OnQryTradeEvent);
            //_wrapper.OnQryPositionDetailEvent += new CBOnQryPositionDetail(_wrapper_OnQryPositionDetailEvent);

            _wrapper.OnLogEvent += new CBOnLog(_wrapper_OnLogEvent);
            _wrapper.OnMessageEvent += new CBOnMessage(_wrapper_OnMessageEvent);
            //_wrapper.OnTransferEvent += new CBOnTransfer(_wrapper_OnTransferEvent);
        }

      

        /// <summary>
        /// 执行对象销毁
        /// </summary>
        public virtual void OnDisposed()
        {
            

        }

        /// <summary>
        /// 启动时登入成功后 恢复日内交易数据
        /// </summary>
        public virtual void OnInit()
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
            throw new NotImplementedException();
        }

        public virtual void SendOrder(BinaryOptionOrder o)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 通过成交接口取消委托
        /// </summary>
        /// <param name="oid"></param>
        public virtual void CancelOrder(long oid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 响应市场行情
        /// </summary>
        /// <param name="k"></param>
        public virtual void GotTick(Tick k)
        { 
        
        }

        #endregion

        #region 接口返回数据的处理

        //交易实时数据 异步处理
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

        //查询接口 直接同步调用处理
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
        #endregion






        #region 底层wrapper向接口提交操作
        /// <summary>
        /// 提交委托
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        protected bool WrapperSendOrder(ref XOrderField order)
        {
            return _wrapper.SendOrder(ref order);
        }

        /// <summary>
        /// 提交委托操作
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected bool WrapperSendOrderAction(ref XOrderActionField action)
        {
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
        RingBuffer<XErrorField> _messagecache = new RingBuffer<XErrorField>(buffersize);

        Thread _notifythread = null;
        bool _working = false;

        ManualResetEvent _notifywaiting = new ManualResetEvent(false);

        /// <summary>
        /// 通知处理线程 有新的交易回报
        /// </summary>
        void NewNotify()
        {
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

                    while (_messagecache.hasItems)
                    {
                        XErrorField message = _messagecache.Read();
                        NotifyMessage(message);
                    }
                    // clear current flag signal
                    _notifywaiting.Reset();
                    // wait for a new signal to continue reading
                    _notifywaiting.WaitOne(_sleep);
                }
                catch (Exception ex)
                {
                    logger.Error("process cache error:" + ex.ToString());
                }
            }
        }

        #endregion

        #region Proxy底层回报数据处理
        bool _connected = false;
        void _wrapper_OnConnectedEvent()
        {
            logger.Info("--OnConnectedEvent--");
            _connected = true;
            //请求登入
            _wrapper.Login(ref _usrinfo);
        }

        bool _loginreply = false;
        bool _loginsuccess = false;

        void _wrapper_OnLoginEvent(ref XRspUserLoginField pRspUserLogin)
        {
            logger.Info(string.Format("--OnLoginEvent-- ErrorID:{0} Message:{1}", pRspUserLogin.ErrorID, pRspUserLogin.ErrorMsg));
            _loginreply = true;
            if (pRspUserLogin.ErrorID == 0)
            {
                _loginsuccess = true;
                logger.Info("交易帐户登入成功");
            }
            else
            {
                _loginsuccess = false;
                logger.Info("交易帐户登入失败");
            }
        }

        void _wrapper_OnDisconnectedEvent()
        {
            logger.Info("--OnDisconnectedEvent--");
        }



        void _wrapper_OnRtnOrderEvent(ref XOrderField pOrder)
        {
            logger.Info("--OnRtnOrderEvent--");
            _ordercache.Write(pOrder);
            NewNotify();
        }

        void _wrapper_OnRtnOrderErrorEvent(ref XOrderField pOrder, ref XErrorField pError)
        {
            logger.Info("--OnRtnOrderErrorEvent--");
            _ordererrorcache.Write(new XOrderError(pOrder, pError));
            NewNotify();
        }
        void _wrapper_OnRtnOrderActionErrorEvent(ref XOrderActionField pOrderAction, ref XErrorField pError)
        {
            logger.Info("--OrderActionErrorEvent--");
            _orderactionerrorcache.Write(new XOrderActionError(pOrderAction, pError));
            NewNotify();
        }

        void _wrapper_OnRtnTradeEvent(ref XTradeField pTrade)
        {
            logger.Info("--OnRtnTradeEvent--");
            _tradecache.Write(pTrade);
            NewNotify();
        }

        void _wrapper_OnLogEvent(IntPtr pData, int len)
        {
            byte[] data = new byte[len];
            Marshal.Copy(pData, data, 0, len);
            string message = System.Text.Encoding.UTF8.GetString(data, 0, len);
            logger.Info(message);
        }

        /// <summary>
        /// 接口侧返回消息处理
        /// </summary>
        /// <param name="pMessage"></param>
        /// <param name="islast"></param>
        void _wrapper_OnMessageEvent(ref XErrorField pMessage, bool islast)
        {
            logger.Info("--TLBroker OnMessageEvent--");
            _messagecache.Write(pMessage);
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
