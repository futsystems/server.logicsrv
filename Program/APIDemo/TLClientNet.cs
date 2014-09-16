using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    public class TLClientNet
    {

        #region Event
        /// <summary>
        /// 日志输出回调
        /// </summary>
        public event DebugDelegate OnDebugEvent;
        /// <summary>
        /// 行情连接建立回调
        /// </summary>
        public event VoidDelegate OnDataConnectEvent;
        /// <summary>
        /// 行情连接断开回调
        /// </summary>
        public event VoidDelegate OnDataDisconnectEvent;
        /// <summary>
        /// 交易连接建立回调
        /// </summary>
        public event VoidDelegate OnConnectEvent;
        /// <summary>
        /// 交易连接断开回调
        /// </summary>
        public event VoidDelegate OnDisconnectEvent;
        /// <summary>
        /// 登入回调
        /// </summary>
        public event LoginResponseDel OnLoginEvent;


        /// <summary>
        /// 行情回调
        /// </summary>
        public event TickDelegate OnTickEvent;
        /// <summary>
        /// 委托回报回调
        /// </summary>
        public event OrderDelegate OnOrderEvent;
        /// <summary>
        /// 昨日持仓回调
        /// </summary>
        public event PositionDelegate OnOldPositionEvent;
        /// <summary>
        /// 持仓更新回调
        /// </summary>
        public event PositionDelegate OnPositionUpdateEvent;
        /// <summary>
        /// 成交回调
        /// </summary>
        public event FillDelegate OnTradeEvent;


        #endregion

        TLClient_MQ connecton = null;


        string[] _servers = new string[] { };
        int _port = 5570;


        string _account = "";
        public TLClientNet(string[] servers, int port, bool verb)
        {
            _servers = servers;
            _port = port;
            _noverb = !verb;

        }
        public void Start()
        {
            debug("TLClientNet Starting......", QSEnumDebugLevel.INFO);
            connecton = new TLClient_MQ(_servers, _port, "demo", _noverb);
            connecton.ProviderType = QSEnumProviderType.Both;
            BindConnectionEvent();

            connecton.Start();


        
        }

        public void Stop()
        {
            debug("TLClientNet Stopping......");
            if (connecton != null && connecton.IsConnected)
            {
                connecton.Stop();
            }
            connecton = null;
            
        }

        void BindConnectionEvent()
        {
            connecton.OnDebugEvent += new DebugDelegate(connecton_OnDebugEvent);
            connecton.OnConnectEvent += new ConnectDel(connecton_OnConnectEvent);
            connecton.OnDisconnectEvent += new DisconnectDel(connecton_OnDisconnectEvent);
            connecton.OnDataPubConnectEvent += new DataPubConnectDel(connecton_OnDataPubConnectEvent);
            connecton.OnDataPubDisconnectEvent += new DataPubDisconnectDel(connecton_OnDataPubDisconnectEvent);
            connecton.OnLoginResponse += new LoginResponseDel(connecton_OnLoginResponse);
            connecton.OnPacketEvent += new IPacketDelegate(connecton_OnPacketEvent);


        }

        int requestid = 0;
        #region 客户端暴露的操作

        void SendPacket(IPacket packet)
        { 
            //权限或者登入状态检查
            if (connecton != null && connecton.IsConnected)
            {
                connecton.TLSend(packet);
                debug("Sendpacket to server, type:" + packet.Type.ToString() + " content:" + packet.ToString(), QSEnumDebugLevel.INFO);
            }
        }
        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="order"></param>
        public void ReqOrderInsert(Order order)
        {

            order.Account = _account;

            OrderInsertRequest request = RequestTemplate<OrderInsertRequest>.CliSendRequest(requestid++);
            request.Order = order;

            SendPacket(request);
        }
        /// <summary>
        /// 提交委托操作
        /// </summary>
        /// <param name="action"></param>
        public void ReqOrderAction(OrderAction action)
        {
            OrderActionRequest requets = RequestTemplate<OrderActionRequest>.CliSendRequest(requestid++);
            action.Account = _account;
            requets.OrderAction = action;

            SendPacket(requets);
        }
        /// <summary>
        /// 直接取消委托
        /// </summary>
        /// <param name="id"></param>
        public void ReqCancelOrder(long id)
        {
            OrderAction action = new OrderActionImpl();
            action.Account = "";
            action.ActionFlag = QSEnumOrderActionFlag.Delete;
            action.OrderID = id;
            ReqOrderAction(action);
        }

        /// <summary>
        /// 请求注销行情数据
        /// </summary>
        public void ReqUnRegisterSymbols()
        {
            UnregisterSymbolsRequest request = RequestTemplate<UnregisterSymbolsRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        /// <summary>
        /// 请求登入
        /// </summary>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public void ReqLogin(string loginid, string pass,int logintype,string mac)
        {
            LoginRequest request = RequestTemplate<LoginRequest>.CliSendRequest(requestid++);
            request.LoginID = loginid;
            request.Passwd = pass;
            request.LoginType = logintype;
            request.MAC = mac;
            //request.

            SendPacket(request);
        }

        /// <summary>
        /// 请求帐户信息
        /// </summary>
        public void ReqQryAccountInfo()
        {
            QryAccountInfoRequest request = RequestTemplate<QryAccountInfoRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            
            SendPacket(request);
        }
        /// <summary>
        /// 请求查询可开手数
        /// </summary>
        public void ReqQryMaxOrderVol(string symbol)
        {
            QryMaxOrderVolRequest request = RequestTemplate<QryMaxOrderVolRequest>.CliSendRequest(requestid++);
            request.Symbol = symbol;
            request.OffsetFlag = QSEnumOffsetFlag.UNKNOWN;
            request.Account = _account;

            SendPacket(request);
        }

        /// <summary>
        /// 查询交易者信息
        /// </summary>
        public void ReqQryInvestor()
        {
            QryInvestorRequest request = RequestTemplate<QryInvestorRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            SendPacket(request);
        }

        /// <summary>
        /// 查询合约信息
        /// </summary>
        public void ReqQrySymbol()
        {
            QrySymbolRequest request = RequestTemplate<QrySymbolRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        /// <summary>
        /// 查询结算信息
        /// </summary>
        public void ReqQrySettleInfo()
        {
            QrySettleInfoRequest request = RequestTemplate<QrySettleInfoRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Tradingday = 0;
            SendPacket(request);
        }

        /// <summary>
        /// 查询结算确认
        /// </summary>
        public void ReqQrySettleInfoConfirm()
        {
            QrySettleInfoConfirmRequest request = RequestTemplate<QrySettleInfoConfirmRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            
            SendPacket(request);
        }

        /// <summary>
        /// 确认结算单
        /// </summary>
        public void ReqConfirmSettlement()
        {
            ConfirmSettlementRequest request = RequestTemplate<ConfirmSettlementRequest>.CliSendRequest(requestid++);
            request.Account = _account;

            SendPacket(request);
        }
        /// <summary>
        /// 查询委托
        /// </summary>
        public void ReqQryOrder(string symbol="",long orderid=0)
        {
            QryOrderRequest request = RequestTemplate<QryOrderRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Symbol = symbol;
            request.OrderID = orderid;
            SendPacket(request);
        }

        /// <summary>
        /// 查询成交
        /// </summary>
        public void ReqQryTrade(string symbol="")
        {
            QryTradeRequest request = RequestTemplate<QryTradeRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Symbol = symbol;

            SendPacket(request);
        }

        /// <summary>
        /// 查询持仓
        /// </summary>
        public void ReqQryPosition(string symbol="")
        {
            QryPositionRequest request = RequestTemplate<QryPositionRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Symbol = symbol;

            SendPacket(request);
        }

        /// <summary>
        /// 查询Bar数据
        /// </summary>
        public void ReqBar()
        {
            QryBarRequest request = RequestTemplate<QryBarRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        /// <summary>
        /// 请求注册行情数据
        /// </summary>
        public void ReqRegisterSymbols(string[] symbols)
        {
            RegisterSymbolsRequest request = RequestTemplate<RegisterSymbolsRequest>.CliSendRequest(requestid++);
            request.SetSymbols(symbols);

            SendPacket(request);
            connecton.Subscribe(symbols);
            
        }

        public void ReqContribRequest(string moduleid, string cmdstr, string args)
        {
            ContribRequest request = RequestTemplate<ContribRequest>.CliSendRequest(requestid++);
            request.ModuleID = moduleid;
            request.CMDStr = cmdstr;
            request.Parameters = args;

            SendPacket(request);
        }

        public void ReqChangePassowrd(string oldpass,string newpass)
        {
            ReqChangePasswordRequest request = RequestTemplate<ReqChangePasswordRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.OldPassword = oldpass;
            request.NewPassword = newpass;

            SendPacket(request);
        }


        public void ReqQrySymbol(string symbol)
        {
            QrySymbolRequest request = RequestTemplate<QrySymbolRequest>.CliSendRequest(requestid++);
            request.Symbol = symbol;

            SendPacket(request);
        }

        public void ReqQryOpenSize(string symbol)
        {
            QryMaxOrderVolRequest request = RequestTemplate<QryMaxOrderVolRequest>.CliSendRequest(requestid++);
            request.Symbol = symbol;
            request.Account = _account;

            SendPacket(request);
        }
        #endregion


        #region 底层连接暴露上来的通道
        void CliOnTickNotify(TickNotify response)
        {
            if (OnTickEvent != null)
                OnTickEvent(response.Tick);
        }

        void CliOnOldPositionNotify(HoldPositionNotify response)
        {
            //if (OnOldPositionEvent != null)
                //OnOldPositionEvent(response.Position);
        }
        void CliOnOrderNotify(OrderNotify response)
        {
            debug("got order notify:" + response.Order.ToString(), QSEnumDebugLevel.INFO);
            debug("##Order:" + response.Order.ToString(), QSEnumDebugLevel.INFO);
            if (OnOrderEvent != null)
                OnOrderEvent(response.Order);
        }

        void CliOnErrorOrderNotify(ErrorOrderNotify response)
        {
            debug(string.Format("got order error:{0} message:{1} order:{2}", response.RspInfo.ErrorID, response.RspInfo.ErrorMessage, OrderImpl.Serialize(response.Order)));
        }

        void CliOnTradeNotify(TradeNotify response)
        {
            debug("got trade notify:" + response.Trade.ToString(), QSEnumDebugLevel.INFO);
            if (OnTradeEvent != null)
                OnTradeEvent(response.Trade);
        }

        void CliOnPositionUpdateNotify(PositionNotify response)
        {
            debug("got postion notify:" + response.Position.ToString(), QSEnumDebugLevel.INFO);
            //if (OnPositionUpdateEvent != null)
            //    OnPositionUpdateEvent(response.Position);
        }

        void CliOnSettleInfoConfirm(RspQrySettleInfoConfirmResponse response)
        {
            debug("got confirmsettleconfirm data:" + response.ConfirmDay.ToString() + " time:" + response.ConfirmTime.ToString(), QSEnumDebugLevel.INFO);

        }

        void CliOnSettleInfo(RspQrySettleInfoResponse response)
        {
            debug("got settleinfo:", QSEnumDebugLevel.INFO);
            string[] rec = response.Content.Split('\n');
            foreach (string s in rec)
            {
                debug(s, QSEnumDebugLevel.INFO);
            }



        }


        void CliOnOrderAction(OrderActionNotify response)
        {
            debug("got order action:" + response.ToString());
        }

        void CliOnErrorOrderActionNotify(ErrorOrderActionNotify response)
        {
            debug(string.Format("got orderaction error:{0} message:{1} orderaction:{2}", response.RspInfo.ErrorID, response.RspInfo.ErrorMessage, OrderActionImpl.Serialize(response.OrderAction)));
        }

        void CliOnChangePass(RspReqChangePasswordResponse response)
        {
            debug("got changepassword response:" + response.RspInfo.ErrorID.ToString() + " " + response.RspInfo.ErrorMessage);
        }
        #region 查询
        void CliOnRspQryAccountInfoResponse(RspQryAccountInfoResponse response)
        {
            debug("------------帐户信息-------------", QSEnumDebugLevel.INFO);
            debug("         Account:" + response.AccInfo.Account, QSEnumDebugLevel.INFO);
            debug("LastEqutiy:" + response.AccInfo.LastEquity.ToString(), QSEnumDebugLevel.INFO);
            debug("NowEquity:" + response.AccInfo.NowEquity.ToString(), QSEnumDebugLevel.INFO);
            debug("RealizedPL:" + response.AccInfo.RealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("UnRealizedPL:" + response.AccInfo.UnRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("Commission:" + response.AccInfo.Commission.ToString(), QSEnumDebugLevel.INFO);
            debug("Profit:" + response.AccInfo.Profit.ToString(), QSEnumDebugLevel.INFO);
            debug("CashIn:" + response.AccInfo.CashIn.ToString(), QSEnumDebugLevel.INFO);
            debug("CashOut:" + response.AccInfo.CashOut.ToString(), QSEnumDebugLevel.INFO);
            debug("MoneyUsed:" + response.AccInfo.MoneyUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("TotalLiquidation:" + response.AccInfo.TotalLiquidation.ToString(), QSEnumDebugLevel.INFO);
            debug("AvabileFunds:" + response.AccInfo.AvabileFunds.ToString(), QSEnumDebugLevel.INFO);
            debug("Category:" + response.AccInfo.Category.ToString(), QSEnumDebugLevel.INFO);
            debug("OrderRouterType:" + response.AccInfo.OrderRouteType.ToString(), QSEnumDebugLevel.INFO);
            debug("Excute:" + response.AccInfo.Execute.ToString(), QSEnumDebugLevel.INFO);
            debug("Intraday:" + response.AccInfo.IntraDay.ToString(), QSEnumDebugLevel.INFO);

            debug("FutMarginUsed:" + response.AccInfo.FutMarginUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("FutMarginFrozen:" + response.AccInfo.FutMarginFrozen.ToString(), QSEnumDebugLevel.INFO);
            debug("FutRealizedPL:" + response.AccInfo.FutRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("FutUnRealizedPL:" + response.AccInfo.FutUnRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("FutCommission:" + response.AccInfo.FutCommission.ToString(), QSEnumDebugLevel.INFO);
            debug("FutCash:" + response.AccInfo.FutCash.ToString(), QSEnumDebugLevel.INFO);
            debug("FutLiquidation:" + response.AccInfo.FutLiquidation.ToString(), QSEnumDebugLevel.INFO);
            debug("FutMoneyUsed:" + response.AccInfo.FutMoneyUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("FutAvabileFunds:" + response.AccInfo.FutAvabileFunds.ToString(), QSEnumDebugLevel.INFO);

            debug("OptPositionCost:" + response.AccInfo.OptPositionCost.ToString(), QSEnumDebugLevel.INFO);
            debug("OptPositionValue:" + response.AccInfo.OptPositionValue.ToString(), QSEnumDebugLevel.INFO);
            debug("OptRealizedPL:" + response.AccInfo.OptRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("OptCommission:" + response.AccInfo.OptCommission.ToString(), QSEnumDebugLevel.INFO);
            debug("OptMoneyFrozen:" + response.AccInfo.OptMoneyFrozen.ToString(), QSEnumDebugLevel.INFO);
            debug("OptCash:" + response.AccInfo.OptCash.ToString(), QSEnumDebugLevel.INFO);
            debug("OptMarketValue:" + response.AccInfo.OptMarketValue.ToString(), QSEnumDebugLevel.INFO);
            debug("OptLiquidation:" + response.AccInfo.OptLiquidation.ToString(), QSEnumDebugLevel.INFO);
            debug("OptMoneyUsed:" + response.AccInfo.OptMoneyUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("OptAvabileFunds:" + response.AccInfo.OptAvabileFunds.ToString(), QSEnumDebugLevel.INFO);

            debug("InnovPositionCost:" + response.AccInfo.InnovPositionCost.ToString(), QSEnumDebugLevel.INFO);
            debug("InnovPositionValue:" + response.AccInfo.InnovPositionValue.ToString(), QSEnumDebugLevel.INFO);
            debug("InnovCommission:" + response.AccInfo.InnovCommission.ToString(), QSEnumDebugLevel.INFO);
            debug("InnovRealizedPL:" + response.AccInfo.InnovRealizedPL.ToString(), QSEnumDebugLevel.INFO);


        }

        void CliOnMaxOrderVol(RspQryMaxOrderVolResponse response)
        {

        }

        /// <summary>
        /// 查询委托回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnRspQryOrderResponse(RspQryOrderResponse response)
        {
            debug("##Order:" + response.OrderToSend.ToString(), QSEnumDebugLevel.INFO);
        }
        /// <summary>
        /// 查询成交回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnRspQryTradeResponse(RspQryTradeResponse response)
        { 
        
        }

        /// <summary>
        /// 查询持仓回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnRspQryPositionResponse(RspQryPositionResponse response)
        { 
        
        }

        void CliOnRspQryInvestorResponse(RspQryInvestorResponse response)
        { 
            
        }
        #endregion



        void connecton_OnPacketEvent(IPacket packet)
        {
            switch (packet.Type)
            { 
                    //Tick数据
                case MessageTypes.TICKNOTIFY:
                    CliOnTickNotify(packet as TickNotify);
                    break;
                    //昨日持仓数据
                case MessageTypes.OLDPOSITIONNOTIFY:
                    CliOnOldPositionNotify(packet as HoldPositionNotify);
                    break;
                    //委托回报
                case MessageTypes.ORDERNOTIFY:
                    CliOnOrderNotify(packet as OrderNotify);
                    break;
                case MessageTypes.ERRORORDERNOTIFY:
                    CliOnErrorOrderNotify(packet as ErrorOrderNotify);
                    break;
                    //成交回报
                case MessageTypes.EXECUTENOTIFY:
                    CliOnTradeNotify(packet as TradeNotify);
                    break;
                    //持仓更新回报
                case MessageTypes.POSITIONUPDATENOTIFY:
                    CliOnPositionUpdateNotify(packet as PositionNotify);
                    break;
                    //委托操作回报
                case MessageTypes.ORDERACTIONNOTIFY:
                    CliOnOrderAction(packet as OrderActionNotify);
                    break;
                  
                case MessageTypes.ERRORORDERACTIONNOTIFY:
                    break;

                case MessageTypes.CHANGEPASSRESPONSE:



                #region 查询
                case MessageTypes.ORDERRESPONSE://查询委托回报
                    CliOnRspQryOrderResponse(packet as RspQryOrderResponse);
                    break;
                case MessageTypes.TRADERESPONSE://查询成交回报
                    CliOnRspQryTradeResponse(packet as RspQryTradeResponse);
                    break;
                case MessageTypes.POSITIONRESPONSE://查询持仓回报
                    CliOnRspQryPositionResponse(packet as RspQryPositionResponse);
                    break;

                case MessageTypes.ACCOUNTINFORESPONSE://帐户信息回报
                    CliOnRspQryAccountInfoResponse(packet as RspQryAccountInfoResponse);
                    break;
                case MessageTypes.INVESTORRESPONSE:
                    CliOnRspQryInvestorResponse(packet as RspQryInvestorResponse);
                    break;

                case MessageTypes.MAXORDERVOLRESPONSE: //最大可开数量回报
                    CliOnMaxOrderVol(packet as RspQryMaxOrderVolResponse);
                    break;
                case MessageTypes.SETTLEINFOCONFIRMRESPONSE://结算确认回报
                    CliOnSettleInfoConfirm(packet as RspQrySettleInfoConfirmResponse);
                    break;
                  
                case MessageTypes.SETTLEINFORESPONSE://结算信息会回报
                    CliOnSettleInfo(packet as RspQrySettleInfoResponse);
                    break;
                #endregion

                default:
                    debug("Packet Handler Not Set, Packet:" + packet.ToString(), QSEnumDebugLevel.ERROR);
                    break;
            }
        }

        int _tradingday = 0;
        void connecton_OnLoginResponse(LoginResponse response)
        {
            debug(" got loginresponse:" + response.ToString(), QSEnumDebugLevel.DEBUG);
            if (response.Authorized)
            {
                _account = response.Account;
                _tradingday = response.Date;
            }
            if (OnLoginEvent != null)
                OnLoginEvent(response);
        }

        void connecton_OnDataPubDisconnectEvent()
        {
            if (OnDataDisconnectEvent != null)
                OnDataDisconnectEvent();
        }

        void connecton_OnDataPubConnectEvent()
        {
            if (OnDataConnectEvent != null)
                OnDataConnectEvent();
        }

        void connecton_OnDisconnectEvent()
        {
            if (OnDisconnectEvent != null)
                OnDisconnectEvent();
        }

        void connecton_OnConnectEvent()
        {
            if (OnConnectEvent != null)
                OnConnectEvent();
        }

        void connecton_OnDebugEvent(string msg)
        {
            if (OnDebugEvent != null)
                OnDebugEvent(msg);
        }

        #endregion







        #region 功能函数

        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.DEBUG;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("[" + level.ToString() + "] " + msg);
        }
        void msgdebug(string msg)
        {
            if (OnDebugEvent != null)
                OnDebugEvent(msg);

        }
        bool _noverb = true;
        /// <summary>
        /// enable/disable extended debugging
        /// </summary>
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        void v(string msg)
        {
            if (_noverb) return;
            msgdebug(msg);
        }


        #endregion

    }
}
