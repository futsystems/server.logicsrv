using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
namespace QSSimBroker
{
    public class QSBroker : QSTradingSession,IBroker
    {

        //string[] _servers;
        //int _port;
        public QSBroker(string[] servers, int port):base(servers,port)
        {


        }
        public QSBroker():this(new string[] {"cps.if888.com.cn"},5570)
        { 
            
        }
        public string Title
        {
            get { return "空白Broker模板"; }
        }
        //该Broker支持的交易所
        IExchange[] _exlist = null;
        public IExchange[] ExchangeSupported { get { return _exlist; } }
        //该broker支持的证券类型
        SecurityType[] _sectypelist = null;
        public SecurityType[] SecuritySupported { get { return _sectypelist; } }

        /// <summary>
        /// 当数据服务器登入成功后调用
        /// </summary>
        public event IConnecterParamDel Connected;
        /// <summary>
        /// 当数据服务器断开后触发事件
        /// </summary>
        public event IConnecterParamDel Disconnected;


        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /*
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("[" + level.ToString() + "] " + msg);
        }

        //public event DebugDelegate SendDebugEvent;
        //void msgdebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }**/

        bool _noverb = true;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }
        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }

        public void Show(object o)
        {

        }
        public event GetSymbolTickDel GetSymbolTickEvent;
        Tick GetSymbolTick(string symbol)
        {
            if (GetSymbolTickEvent != null)
                return GetSymbolTickEvent(symbol);
            return null;
        }

        private IBrokerClearCentre _clearCentre;
        public IBrokerClearCentre ClearCentre
        {
            get
            {
                return _clearCentre;
            }
            set
            {
                _clearCentre = value;
            }
        }

        /// <summary>
        /// 产生新的tick用于引擎Fill Order
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            //_kt.addindex(k.symbol);
            //_kt.newTick(k);
            //process();
        }

        /// <summary>
        /// fill acknowledged, order filled.
        /// </summary>
        public event FillDelegate GotFillEvent;
        /// <summary>
        /// order acknowledgement, order placed.
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        /// <summary>
        /// cancel acknowledgement, order is canceled
        /// </summary>
        public event LongDelegate GotCancelEvent;
        public event OrderMessageDel GotOrderMessageEvent;


        #region QSTradingSession回调函数
        /// <summary>
        /// QSTradingSession返回委托
        /// </summary>
        /// <param name="order"></param>
        public override void OnOrder(Order order)
        {
            
        }

        /// <summary>
        /// QStradingSession返回成交
        /// </summary>
        /// <param name="fill"></param>
        public override void OnFill(Trade fill)
        {
            
        }
        /// <summary>
        /// QSTradingSession返回取消
        /// </summary>
        /// <param name="id"></param>
        public override void OnCancel(long id)
        {
            
        }

        bool _started = false;
        public override void OnLogin(bool ok)
        {
            debug(PROGRAME + ":登入状态:" + (ok ? "成功" : "失败"));
            if (ok)
            {
                if (Connected!=null)
                    Connected(this);
                _started = true;
            }
            else
                _started = false;

        }

        public override void OnPosition(Position pos)
        {
            throw new NotImplementedException();
        }

        public override void OnTick(Tick tick)
        {
            throw new NotImplementedException();
        }
        public override void OnConnected()
        {
            debug(Title + ":连接建立...准备请求登入");
            ReqLogin("170010", "123456");
        }

        public override void OnDataPubAvabile()
        {
            
        }

        #endregion


        public new void Start()
        {
            base.Start();
        }

        public new void Stop()
        {

            base.Stop();
        }

        string _acc = null;
        public  new void SendOrder(Order o)
        {
            Order no = new  OrderImpl(o);
            no.Account = _acc;

            base.SendOrder(no);

        }
        public new void CancelOrder(long oid)
        { 
            
        
        }
        public bool IsLive
        {
            get
            {
                return _started;
            }
        }

    }
}
