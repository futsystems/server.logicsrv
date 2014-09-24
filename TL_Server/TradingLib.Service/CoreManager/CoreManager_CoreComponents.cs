using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;



namespace TradingLib.ServiceManager
{
    public partial class CoreManager
    {
        private void InitSettleCentre()
        {
            debug("初始化结算中心");
            _settleCentre = new SettleCentre();
        }

        private void DestorySettleCentre()
        {

        }


        private void InitMsgExchSrv()
        {
            debug("1.初始化MsgExchServer");
            //根据不同的配置生成具体的TLServer
            
            _messageExchagne = new MsgExchServer();

        }


        private void DestoryMsgExchSrv()
        {
            _messageExchagne.Dispose();
            //_messageExchagne = null;
        }




        private void InitTradeFollow()
        {
            debug("1.1初始化交易数据流服务");
            _tradeFollow = new TradeFollow();
            _tradeFollow.Start();

            //_messageExchagne.GotFillEvent += new FillDelegate(_tradeFollow.GotTrade);
        }
        
        //初始化清算中心
        private void InitClearCentre()
        {
            debug("2.初始化ClearCentre");
            _clearCentre = new ClearCentre(config.LoadMode);

            //clearcentre对外发送委托 取消委托的请求回调 _srv.SendOrderNoRiskCheck 内函数被 客户请求调用,我们不能再该函数隐藏掉异常，因此我们需要在清算中心的函数中处理异常
            //_clearCentre.newSendOrderRequest += new OrderDelegate(_messageExchagne.SendOrderNoRiskCheck);
            //_clearCentre.newOrderCancelRequest += new LongDelegate(_messageExchagne.CancelOrder);

            //将order,fill,cancle,tick与清算中心的回调函数绑定，清算中心会根据接收到的信息进行账户交易信息记录与财务计算
            //_srv.gottick/等被多个组件订阅，因此该事件处理函数必须无异常，否则会影响其他后续组件的对该事件的订阅
            _messageExchagne.GotTickEvent += new TickDelegate(_clearCentre.GotTick);
            _messageExchagne.GotOrderEvent += new OrderDelegate(_clearCentre.GotOrder);
            _messageExchagne.GotFillEvent += new FillDelegate(_clearCentre.GotFill);
            _messageExchagne.GotCancelEvent += new LongDelegate(_clearCentre.GotCancel);

            //tradingserver得到成交后发送给clearcentre处理，计算完手续费后再通过tradingserver回报给客户端
            _clearCentre.GotCommissionFill += new FillDelegate(_messageExchagne.newCommissionFill);

            //将清算中心传递给tradingserver
            _messageExchagne.ClearCentre = _clearCentre;

            //将清算中心传递给结算中心
            _settleCentre.ClearCentre = _clearCentre;
        }

        private void DestoryClearCentre()
        {
            //clearcentre对外发送委托 取消委托的请求回调 _srv.SendOrderNoRiskCheck 内函数被 客户请求调用,我们不能再该函数隐藏掉异常，因此我们需要在清算中心的函数中处理异常
            //_clearCentre.newSendOrderRequest -= new OrderDelegate(_messageExchagne.SendOrderNoRiskCheck);
            //_clearCentre.newOrderCancelRequest -= new LongDelegate(_messageExchagne.CancelOrder);

            //将order,fill,cancle,tick与清算中心的回调函数绑定，清算中心会根据接收到的信息进行账户交易信息记录与财务计算
            //_srv.gottick/等被多个组件订阅，因此该事件处理函数必须无异常，否则会影响其他后续组件的对该事件的订阅
            _messageExchagne.GotTickEvent -= new TickDelegate(_clearCentre.GotTick);
            _messageExchagne.GotOrderEvent -= new OrderDelegate(_clearCentre.GotOrder);
            _messageExchagne.GotFillEvent -= new FillDelegate(_clearCentre.GotFill);
            _messageExchagne.GotCancelEvent -= new LongDelegate(_clearCentre.GotCancel);

            //tradingserver得到成交后发送给clearcentre处理，计算完手续费后再通过tradingserver回报给客户端
            _clearCentre.GotCommissionFill -= new FillDelegate(_messageExchagne.newCommissionFill);
            //_messageExchagne.ClearCentre = null;

            _clearCentre.Dispose();

        }


       


        //初始化风控中心
        private void InitRiskCentre()
        {
            debug("3.初始化RiskCentre");
            _riskCentre = new RiskCentre(_clearCentre);

            //2.清算中心激活某个账户 调用风控中心重置该账户规则
            _clearCentre.AccountActiveEvent += (string account) =>
            {
                //风控中心重新加载风控规则 解决账户检查规则触发后,状态没复位,账户激活后规则失效的问题
                _riskCentre.ResetRuleSet(account);
            };

            //3.清算中心添加某个配资账户后 调用风控中心设置该账户风控规则
            //_clearCentre.LoaneeAccountAddedEvent += (string account) =>
            //{
            //    _riskCentre.SetLoaneeRule(account);//设置该配资账户风控规则
            //};

            //4.风控中心记录客户端的登入 登出情况
            //_messageExchagne.SendLoginInfoEvent += new LoginInfoDel(_riskCentre.GotLoginInfo);

            //交易服务行情驱动风控中心
            _messageExchagne.GotTickEvent += new TickDelegate(_riskCentre.GotTick);
            _messageExchagne.GotCancelEvent += new LongDelegate(_riskCentre.GotCancel);
            _messageExchagne.GotErrorOrderEvent += new ErrorOrderDel(_riskCentre.GotErrorOrder);
            
            //风控中心从tradingsrv获得委托编号 提交委托 取消委托的操作
            _riskCentre.AssignOrderIDEvent += new AssignOrderIDDel(_messageExchagne.AssignOrderID);
            _riskCentre.newSendOrderRequest += new OrderDelegate(_messageExchagne.SendOrderInternal);
            _riskCentre.newOrderCancelRequest += new LongDelegate(_messageExchagne.CancelOrder);

            //帐户加载事件 用于清算中心Addapter附加到帐户对象
            //_clearCentre.AccountCachedEvent += new IAccountDel(_riskCentre.CacheAccount);
            _clearCentre.PositionRoundClosedEvent += new PositionRoundClosedDel(_riskCentre.GotPostionRoundClosed);
            //绑定风控中心
            _messageExchagne.RiskCentre = _riskCentre;
        }

        private void DestoryRiskCentre()
        {
            //1.tradingserver委托检查事件绑定到风控中心的委托检查函数 以下2个函数由riskcentre处理异常
            //_messageExchagne.SendOrderRiskCheckEvent -= new RiskCheckOrderDel(_riskCentre.CheckOrder);

            /*
            //2.清算中心激活某个账户 调用风控中心重置该账户规则
            _clearCentre.AccountActiveEvent += (string account) =>
            {
                //风控中心重新加载风控规则 解决账户检查规则触发后,状态没复位,账户激活后规则失效的问题
                _riskCentre.ResetRuleSet(account);
            };

            //3.清算中心添加某个配资账户后 调用风控中心设置该账户风控规则
            _clearCentre.LoaneeAccountAddedEvent -= (string account) =>
            {
                _riskCentre.SetLoaneeRule(account);//设置该配资账户风控规则
            };**/

            //4.风控中心记录客户端的登入 登出情况
            //_messageExchagne.SendLoginInfoEvent -= new LoginInfoDel(_riskCentre.GotLoginInfo);

            //交易服务行情驱动风控中心
            _messageExchagne.GotTickEvent -= new TickDelegate(_riskCentre.GotTick);
            _messageExchagne.GotCancelEvent -= new LongDelegate(_riskCentre.GotCancel);

            //风控中心从tradingsrv获得委托编号 提交委托 取消委托的操作
            _riskCentre.AssignOrderIDEvent -= new AssignOrderIDDel(_messageExchagne.AssignOrderID);
            _riskCentre.newSendOrderRequest -= new OrderDelegate(_messageExchagne.SendOrder);
            _riskCentre.newOrderCancelRequest -= new LongDelegate(_messageExchagne.CancelOrder);

            //_clearCentre.FlatPositionEvent -= new FlatPostionDel(_riskCentre.FlatPosition);
            //_clearCentre.AccountCachedEvent -= new IAccountDel(_riskCentre.CacheAccount);
            //绑定风控中心
            _messageExchagne.RiskCentre = null;
            _riskCentre.Dispose();
        }

        //初始化datafeedrouter
        private void InitDataFeedRouter()
        {
            debug("4.1初始化DataFeedRouter");
            _datafeedRouter = new DataFeedRouter(_clearCentre);

            _messageExchagne.BindDataRouter(_datafeedRouter);

            //绑定清算中心的Tick查询函数,用于清算中心查询合约可开仓数量
            _clearCentre.newSymbolTickRequest += new GetSymbolTickDel(_datafeedRouter.getSymbolTick);
        }
        void DestoryDataFeedRouter()
        {
            _messageExchagne.UnBindDataRouter(_datafeedRouter);

            _clearCentre.newSymbolTickRequest -= new GetSymbolTickDel(_datafeedRouter.getSymbolTick);
            _datafeedRouter.Dispose();
        }



        //初始化fastticksrv管理端
        private void InitFastTickMgr()
        {
            debug("4.2初始化FastTickServer Mgr");
            //_ftmgrclient = new FastTickSrvMgrClient(config.TickMgrSrvAddress, config.TickMgrSrvPort);
            //服务端触发注册市场数据 调用 ftmgrclient.registsymbols
            //_srv.RegisterSymbolEvent += new BasketDel(_ftmgrclient.RegistSymbols);
            //_ftmgrclient.Start();
        }

        //初始化brokerselector
        private void InitBrokerRouter()
        {
            debug("5.初始化BrokerRouter");
            _brokerRouter = new BrokerRouter(_clearCentre);

            _brokerRouter.DataFeedRouter = _datafeedRouter;
            _messageExchagne.BindBrokerRouter(_brokerRouter);
        }

        private void DestoryBrokerRouter()
        {
            _brokerRouter.DataFeedRouter = null;
            _messageExchagne.UnBindBrokerRouter(_brokerRouter);
            _brokerRouter.Dispose();
        }







        /// <summary>
        /// 初始化管理与交易信号转发中心
        /// </summary>
        void InitMgrExchSrv()
        {
            debug("6.初始化MgrExchServer");
            _managerExchange = new MgrExchServer(_messageExchagne, _clearCentre, _riskCentre);

            _managerExchange.SendOrderEvent += new OrderDelegate(_messageExchagne.SendOrderInternal);
            _managerExchange.SendOrderCancelEvent += new LongDelegate(_messageExchagne.CancelOrder);
           
            ////管理组件转发 交易服务器过来的委托 成交 取消 tick
            _messageExchagne.GotOrderEvent += new OrderDelegate(_managerExchange.newOrder);
            _messageExchagne.GotErrorOrderEvent += new ErrorOrderDel(_managerExchange.newErrorOrder);
            //_messageExchagne.GotCancelEvent += new LongDelegate(_managerExchange.newCancel);
            _messageExchagne.GotFillEvent += new FillDelegate(_managerExchange.newTrade);
            _messageExchagne.GotTickEvent += new TickDelegate(_managerExchange.newTick);

            
            ////转发账户登入状态信息
            _messageExchagne.ClientLoginInfoEvent += new ClientLoginInfoDelegate<TrdClientInfo>(_managerExchange.newSessionUpdate);
            //_messageExchagne.SendLoginInfoEvent += new LoginInfoDel(_managerExchange.newSessionUpdate);


            ////帐户变动事件，当帐户设置或者相关属性发生变动时 触发该事件
            _clearCentre.AccountChangedEvent += new AccountSettingChangedDel(_managerExchange.newAccountChanged);
            ////添加帐户
            _clearCentre.AccountAddEvent += new AccountIdDel(_managerExchange.newAccountAdded);

            ////qsmgrSrv.FindLoginID2ClientIDEvent += new LoginID2ClientIDDel(_srv.LoginID2ClientID);
            ////注意:由于_srv.gotxxx先绑定了clearcentre然后绑定了mgrserver,因此当mgr回补数据的时候
            ////会lock _clearcentre.ordbook这个时候 将要进入orderbook的order是无法插入的，需要等待mgr取完所有历史order之后才进入orderbook
            ////这个时候刚好继续进入mgr的回调，这样则数据时没有缺失的 并且也不需要做特别负责的同步与处理
            ////_srv.SendAccountUpdateEvent += new AccountUpdateDel(qsmgrSrv.newAccountUpdate);
        }
        private void DestoryMgrExchSrv()
        {

            //_managerExchange.SendOrderEvent -= new OrderDelegate(_messageExchagne.SendOrderNoRiskCheck);
            //_managerExchange.SendOrderCancelEvent -= new LongDelegate(_messageExchagne.CancelOrder);

            ////管理组件转发 交易服务器过来的委托 成交 取消 tick
            //_messageExchagne.GotOrderEvent -= new OrderDelegate(_managerExchange.newOrder);
            //_messageExchagne.GotCancelEvent -= new LongDelegate(_managerExchange.newCancel);
            //_messageExchagne.GotFillEvent -= new FillDelegate(_managerExchange.newTrade);
            //_messageExchagne.GotTickEvent -= new TickDelegate(_managerExchange.newTick);

            ////转发账户登入状态信息
            //_messageExchagne.SendLoginInfoEvent -= new LoginInfoDel(_managerExchange.newSessionUpdate);

            ////

            ////帐户变动事件，当帐户设置或者相关属性发生变动时 触发该事件
            //_clearCentre.AccountChangedEvent -= new AccountSettingChangedDel(_managerExchange.newAccountChanged);

            //_managerExchange.Dispose();
        }
        void InitWebMsgExchSrv()
        {
            debug("7.初始化WebMsgExchSrv");
            _webmsgExchange = new WebMsgExchServer(_messageExchagne, _clearCentre, _riskCentre);

            _messageExchagne.GotTickEvent += new TickDelegate(_webmsgExchange.NewTick);


            //帐户设置变动向web端推送消息
            _clearCentre.AccountChangedEvent +=new AccountSettingChangedDel(_webmsgExchange.NewAccountSettingUpdate);
            
            //转发账户登入状态信息
            //_messageExchagne.SendLoginInfoEvent += new LoginInfoDel(_webmsgExchange.NewSessionUpdate);

        }
        private void DestoryWebMsgExchSrv()
        {
            _messageExchagne.GotTickEvent -= new TickDelegate(_webmsgExchange.NewTick);


            //帐户设置变动向web端推送消息
            _clearCentre.AccountChangedEvent -= new AccountSettingChangedDel(_webmsgExchange.NewAccountSettingUpdate);

            //转发账户登入状态信息
            //_messageExchagne.SendLoginInfoEvent -= new LoginInfoDel(_webmsgExchange.NewSessionUpdate);

            _webmsgExchange.Dispose();
        }

        private void InitTaskCentre()
        {

            _taskcentre = new TaskCentre();
        }

        private void DestoryTaskCentre()
        {

            _taskcentre.Dispose();
        }


        
    }
}
