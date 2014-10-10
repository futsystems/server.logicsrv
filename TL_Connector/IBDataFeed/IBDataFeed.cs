using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

using TradingLib.API;
using Krs.Ats.IBNet;
using Krs.Ats.IBNet.Contracts;


namespace DataFeed.IB
{
    public class IBDataFeed:IDataFeed
    {
        public string Title
        {
            get { return "IB盈透数据通道"; }
        }

        
        public bool IsLive { get { return _connected; } }


        private IBClient client;

        private bool _connected = false;


        string _srvIP = "127.0.0.1";
        int _srvPort= 4001;
        int _clientID = 0;

        IBHelper _ibhelper = new IBHelper();
        public IBDataFeed()
        { 
            _ibhelper.SendDebugEvent +=new DebugDelegate(debug);
            
        }
        public void Start()
        {
          
            if (!_connected)
            {
                client = new IBClient();
                client.ThrowExceptions = true;
                bindEvent();
                debug("Connecting to IB.");
                client.Connect(_srvIP,_srvPort,_clientID);

                //demoRegisterSymbol();
            }
            //Form1 fm = new Form1(this);
            //fm.ShowDialog();

        }

        void bindEvent()
        {
            client.TickPrice += client_TickPrice;
            client.TickSize += client_TickSize;
            client.Error += client_Error;
            //client.NextValidId += client_NextValidId;
            //client.UpdateMarketDepth += client_UpdateMktDepth;
            //client.RealTimeBar += client_RealTimeBar;
            //client.OrderStatus += client_OrderStatus;
            //client.ExecDetails += client_ExecDetails;
            client.OpenOrder += new EventHandler<OpenOrderEventArgs>(client_OpenOrder);       
            
        }

        void client_OpenOrder(object sender, OpenOrderEventArgs e)
        {
            debug(e.OrderId.ToString() + " " + e.OrderState.Status.ToString());
        }

        #region ib回调函数

       

        void client_TickSize(object sender, TickSizeEventArgs e)
        {
            //debug("tickid:" + e.TickerId.ToString() + "ticktype:" + e.TickType.ToString() + "size:" + e.Size.ToString());
            onTickPrice(e.TickerId, e.TickType, e.Size, false);
        }   

        void client_Error(object sender, ErrorEventArgs e)
        {
            debug("Error ID"+e.ErrorCode.ToString()+"Error: " + e.ErrorMsg);
            string errorID = e.ErrorCode.ToString();
            switch(errorID)
            {
                case "2104":
                    onMktDataConnectOK();
                    break;
                default:
                    debug("Error ID"+e.ErrorCode.ToString()+"Error: " + e.ErrorMsg);
                    break;

            }
        }
        //返回市场数据连接成功
        void onMktDataConnectOK()
        {
            debug("市场数据连接成功");
            _connected = true;

            
            //对外市场数据连接成功事件
            if (Connected!=null)
                Connected(this);
        
        }

        void client_TickPrice(object sender, TickPriceEventArgs e)
        {
            //debug("tickid:"+e.TickerId.ToString() + "ticktype" + e.TickType.ToString() +"price:" + e.Price.ToString());
            this.onTickPrice(e.TickerId, e.TickType, e.Price, e.CanAutoExecute);
        }
        void onTickPrice(int reqId,TickType field,decimal price,bool canAutoExecute)
        {
            Tick k = getTickSnapshot(reqId);
            if (k == null) return;
            switch (field)
            { 
                case TickType.AskPrice:
                case TickType.AskSize:
                case TickType.BidPrice:
                case TickType.BidSize:
                    onAskBidChange(ref k, field, price);
                    break;
                case TickType.OpenPrice:
                    k.Open = price;
                    break;
                case TickType.HighPrice:
                    k.High =price;
                    break;
                case TickType.LowPrice:
                    k.Low = price;
                    break;
                case TickType.ClosePrice:
                    break;
                case TickType.LastSize:
                    //k.size = Convert.ToInt32(price);
                    onLastSizeChange(ref k, Convert.ToInt32(price));
                    break;
                case TickType.Volume:
                    onVolumeChange(ref k,Convert.ToInt32(price));
                    break;
                case TickType.LastPrice:
                    k.trade = price;
                    break;
                
            }
        
        }
        void onLastSizeChange(ref Tick k, int size)
        {
            decimal price = k.trade;
            k.size = size;
            Tick nk = copyTick(k);
            nk.trade = price;
            nk.size = size;
            GotTick(nk);
        }
        //IB中当成交的手数由volume相减来确认,否则会造成重复发送成交数据(成交数据是过程数据)
        //IB中ask bid数据是根据当前ask(size) bid(size)变动来更新(ask bid数据是状态数据)
        void onVolumeChange(ref Tick k,int vol)
        {
            
            int size = vol - k.Vol;
            decimal price = k.trade;
            k.Vol = vol;//将volume状态更新到ticksnapshot中
            Tick nk = copyTick(k);
            nk.trade = price;
            nk.size = size;
            GotTick(nk);//将含有成交信息的Tick发送出去
            //debug("new trade:" + nk.ToString());
        }
        //当买 卖报盘发生有新数据进来，我们比较数据是否发生变化然后决定是否发送Tick
        void onAskBidChange(ref Tick k,TickType askbidfield,decimal value)
        {
            switch (askbidfield)
            { 
                case TickType.AskSize:
                    int asksize = Convert.ToInt32(value);
                    if (k.AskSize != asksize)//只有当ask bid状态发生变化时 我们才生成新的tick数据用于通知当前状态
                    {
                        Tick nk = copyTick(k);
                        k.os = asksize;//更新ticksnapshot中的asksize;
                        nk.os = asksize;
                        GotTick(nk);
                        return;
                    }
                    else
                        return;
                case TickType.AskPrice:
                    decimal askprice = value;
                    if (k.ask != askprice)
                    {
                        Tick nk = copyTick(k);
                        k.ask = askprice;
                        nk.ask = askprice;
                        GotTick(nk);
                        return;
                    }
                    else
                        return;
                case TickType.BidSize:
                    int bidsize = Convert.ToInt32(value);
                    if (k.bs != bidsize)
                    {
                        Tick nk = copyTick(k);
                        k.bs = bidsize;
                        nk.bs = bidsize;
                        GotTick(nk);
                        return;
                    }
                    else
                        return;
                case TickType.BidPrice:
                    decimal bidprice = value;
                    if (k.bid != bidprice)
                    {
                        Tick nk = copyTick(k);
                        k.bid = bidprice;
                        nk.bid = bidprice;
                        GotTick(nk);
                        return;
                    }
                    else
                        return;
                

            }
        
        }

        Tick copyTick(Tick k)
        {
            Tick nk = new TickImpl(k.symbol);
            copyOHLC(k, nk);
            copyAskBid(k, nk);
            return nk;
            
        }
        void copyOHLC(Tick k, Tick nk)
        {
            nk.Open = k.Open;
            nk.High = k.High;
            nk.Low = k.Low;
            nk.Vol = k.Vol;
        }
        void copyAskBid(Tick k, Tick nk)
        {
            nk.ask = k.ask;
            nk.os = k.os;
            nk.bid = k.bid;
            nk.bs = k.bs;
        }
        
        #endregion

        void GotTick(Tick k)
        {
            
            if (GotTickEvent == null) return;
            if (!k.isValid) return;
            if (k.hasAsk && k.hasBid)
            {
                timeStampTick(k);//设定Tick时间
                GotTickEvent(k);
                //debug("Tick:" + k.ToString());
            }
            
        }
        void timeStampTick(Tick k)
        {
            k.time = Util.ToTLTime(DateTime.Now);
            k.date = Util.ToTLDate(DateTime.Now);
        }
        public void Stop()
        {

            if (_connected)
            {
                client.Disconnect();
                _connected = false;
                if (Disconnected != null)
                    Disconnected(this);
            }
        
        }
        public void RegisterSymbols(string[] symbols)
        {
            foreach (string s in symbols)
            {
                registerSymbol(s);
            }
        }

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if(SendDebugEvent!=null)
                SendDebugEvent(msg);
        }
        public event IConnecterParamDel Connected;
        public event IConnecterParamDel Disconnected;
        public event TickDelegate GotTickEvent;


        int _reqId = 1;
        //获得一个新的数据请求标识
        int askReqId()
        {
            int reqid = _reqId;
            _reqId++;
            return reqid;
        }
        //用于记录reqId到都应数据请求的映射
        Dictionary<int, MktDataRequest> mktDataRequestMap = new Dictionary<int, MktDataRequest>();
        //用于记录symbol与对应数据订阅reqId的映射
        Dictionary<string, int> symbolReqIdMap = new Dictionary<string, int>();

        void registerSymbol(string symbol)
        {
            debug("请求symbol数据:"+symbol);
            
            try
            {
                if (symbolReqIdMap.ContainsKey(symbol))
                {
                    debug("请求过该symbol数据");
                    if (IsMktDataRequested(symbolReqIdMap[symbol])) return;
                    debug("没有有效请求,重新进行请求");
                    //symbolReqIdMap[symbol] = reqMktData(symbol);
                }
                else
                {
                    debug("未请求过该symbol数据");
                    symbolReqIdMap.Add(symbol, reqMktData(symbol));
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
            }
        }
        int reqMktData(string symbol)
        {
            
            //通过symbol得到对应的Contract数据
            //Contract c = IBHelper.genContract(string symbol);
            //1.得到品种数据,2形成对应的Contract返回
            Contract c = _ibhelper.Symbol2IBContract(symbol);
            //Contract c = new Future("ES", "GLOBEX", "201212");
            if(c==null) 
            {
                debug("没有有效合约数据");
                return -1;
            }
            //Future c= new Future("ES", "GLOBEX", "201212");
            int reqid = askReqId();
            
            MktDataRequest mktReq= new MktDataRequest();
            mktReq.reqId = reqid;
            mktReq.contract = c;
            TickImpl tick = new TickImpl(symbol);
            mktReq.tickSnapshot = tick;

            if (mktDataRequestMap.ContainsKey(reqid))
                mktDataRequestMap[reqid] = mktReq;
            else
                mktDataRequestMap.Add(reqid, mktReq);
            //通过数据通道请求数据
            client.RequestMarketData(reqid,c, null, false, false);
            debug("Request Mkt Data for Symbol: " + symbol + "ReqID:" + reqid.ToString());
            return reqid;

             
        }
        //取消某个市场数据订阅
        void cancelMktDataRequest(int reqId)
        {
            client.CancelMarketData(reqId);
            if (mktDataRequestMap.ContainsKey(reqId))
                mktDataRequestMap.Remove(reqId);
        }

        //查看某个reqId是否有订阅数据
        bool IsMktDataRequested(int reqId)
        {
            return mktDataRequestMap.ContainsKey(reqId);
        }
        //通过某个reqId返回对应的Tick数据
        Tick getTickSnapshot(int reqId)
        {
            return mktDataRequestMap.ContainsKey(reqId) ? mktDataRequestMap[reqId].tickSnapshot : null;
        }


        public void QryAllOpenOrders()
        {
            debug("请求所有委托");
            client.RequestAllOpenOrders();
        }

    }

    //封装了市场数据请求
    class MktDataRequest
    {
        public int reqId; // 请求标识
        public Contract contract;//对应合约
        public Tick tickSnapshot;//对应Tick
    }
}
