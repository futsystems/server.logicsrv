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
            debug("0.初始化结算中心");
            _settleCentre = new SettleCentre();
        }

        private void DestorySettleCentre()
        {

        }


        private void InitMsgExchSrv()
        {
            debug("1.初始化MsgExchServer");
            _messageExchagne = new MsgExchServer();
            _settleCentre.BindExchSrv(_messageExchagne);
        }


        private void DestoryMsgExchSrv()
        {
            _messageExchagne.Dispose();
        }
        
        //初始化清算中心
        private void InitClearCentre()
        {
            debug("2.初始化ClearCentre");
            _clearCentre = new ClearCentre();

            //tradingserver得到成交后发送给clearcentre处理，计算完手续费后再通过tradingserver回报给客户端
            //_clearCentre.GotCommissionFill += new FillDelegate(_messageExchagne.newCommissionFill);

            //将清算中心传递给tradingserver
            _messageExchagne.ClearCentre = _clearCentre;

            //将清算中心传递给结算中心
            _settleCentre.BindClearCentre(_clearCentre);
        }

        private void DestoryClearCentre()
        {
            //tradingserver得到成交后发送给clearcentre处理，计算完手续费后再通过tradingserver回报给客户端
            //_clearCentre.GotCommissionFill -= new FillDelegate(_messageExchagne.newCommissionFill);
            //_messageExchagne.ClearCentre = null;
            _clearCentre.Dispose();
        }


        //初始化风控中心
        private void InitRiskCentre()
        {
            debug("3.初始化RiskCentre");
            _riskCentre = new RiskCentre(_clearCentre);

            //绑定风控中心
            _messageExchagne.RiskCentre = _riskCentre;
            _settleCentre.BindRiskCentre(_riskCentre);

            //2.清算中心激活某个账户 调用风控中心重置该账户规则 解决账户检查规则触发后,状态没复位,账户激活后规则失效的问题
            _clearCentre.AccountActiveEvent += new AccoundIDDel(_riskCentre.ResetRuleSet);

            //交易服务回报风控中心
            _messageExchagne.GotTickEvent += new TickDelegate(_riskCentre.GotTick);
            _messageExchagne.GotOrderEvent +=new OrderDelegate(_riskCentre.GotOrder);
            _messageExchagne.GotOrderErrorEvent += new OrderErrorDelegate(_riskCentre.GotOrderError);
            
            //风控中心从tradingsrv获得委托编号 提交委托 取消委托的操作
            _riskCentre.AssignOrderIDEvent += new AssignOrderIDDel(_messageExchagne.AssignOrderID);
            _riskCentre.newSendOrderRequest += new OrderDelegate(_messageExchagne.SendOrderInternal);
            _riskCentre.newOrderCancelRequest += new LongDelegate(_messageExchagne.CancelOrder);

            //帐户加载事件 用于清算中心Addapter附加到帐户对象
            _clearCentre.PositionRoundClosedEvent += new PositionRoundClosedDel(_riskCentre.GotPostionRoundClosed);
            
        }

        private void DestoryRiskCentre()
        {
            //4.风控中心记录客户端的登入 登出情况
            //_messageExchagne.SendLoginInfoEvent -= new LoginInfoDel(_riskCentre.GotLoginInfo);

            _clearCentre.AccountActiveEvent -= new AccoundIDDel(_riskCentre.ResetRuleSet);

            //交易服务行情驱动风控中心
            _messageExchagne.GotTickEvent -= new TickDelegate(_riskCentre.GotTick);
            _messageExchagne.GotOrderEvent -= new OrderDelegate(_riskCentre.GotOrder);
            _messageExchagne.GotOrderErrorEvent -= new OrderErrorDelegate(_riskCentre.GotOrderError);

            //风控中心从tradingsrv获得委托编号 提交委托 取消委托的操作
            _riskCentre.AssignOrderIDEvent -= new AssignOrderIDDel(_messageExchagne.AssignOrderID);
            _riskCentre.newSendOrderRequest -= new OrderDelegate(_messageExchagne.SendOrder);
            _riskCentre.newOrderCancelRequest -= new LongDelegate(_messageExchagne.CancelOrder);

            _clearCentre.PositionRoundClosedEvent -= new PositionRoundClosedDel(_riskCentre.GotPostionRoundClosed);

            //绑定风控中心
            _messageExchagne.RiskCentre = null;
            _riskCentre.Dispose();
        }

        //初始化datafeedrouter
        private void InitDataFeedRouter()
        {
            debug("4.初始化DataFeedRouter");
            _datafeedRouter = new DataFeedRouter();
            _messageExchagne.BindDataRouter(_datafeedRouter);

            //绑定清算中心的Tick查询函数,用于清算中心查询合约可开仓数量
            _clearCentre.newSymbolTickRequest += new GetSymbolTickDel(_datafeedRouter.GetTickSnapshot);
        }
        void DestoryDataFeedRouter()
        {
            _messageExchagne.UnBindDataRouter(_datafeedRouter);

            _clearCentre.newSymbolTickRequest -= new GetSymbolTickDel(_datafeedRouter.GetTickSnapshot);
            _datafeedRouter.Dispose();
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
            _messageExchagne.GotOrderErrorEvent += new OrderErrorDelegate(_managerExchange.newOrderError);
            //_messageExchagne.GotCancelEvent += new LongDelegate(_managerExchange.newCancel);
            _messageExchagne.GotFillEvent += new FillDelegate(_managerExchange.newTrade);
            _messageExchagne.GotTickEvent += new TickDelegate(_managerExchange.newTick);

            
            ////转发账户登入状态信息
            _messageExchagne.ClientLoginInfoEvent += new ClientLoginInfoDelegate<TrdClientInfo>(_managerExchange.newSessionUpdate);

            ////帐户变动事件，当帐户设置或者相关属性发生变动时 触发该事件
            _clearCentre.AccountChangedEvent += new AccountSettingChangedDel(_managerExchange.newAccountChanged);
            ////添加帐户
            _clearCentre.AccountAddEvent += new AccoundIDDel(_managerExchange.newAccountAdded);
        }
        private void DestoryMgrExchSrv()
        {
            _managerExchange.SendOrderEvent -= new OrderDelegate(_messageExchagne.SendOrderInternal);
            _managerExchange.SendOrderCancelEvent -= new LongDelegate(_messageExchagne.CancelOrder);

            ////管理组件转发 交易服务器过来的委托 成交 取消 tick
            _messageExchagne.GotOrderEvent -= new OrderDelegate(_managerExchange.newOrder);
            _messageExchagne.GotOrderErrorEvent -= new OrderErrorDelegate(_managerExchange.newOrderError);
            //_messageExchagne.GotCancelEvent += new LongDelegate(_managerExchange.newCancel);
            _messageExchagne.GotFillEvent -= new FillDelegate(_managerExchange.newTrade);
            _messageExchagne.GotTickEvent -= new TickDelegate(_managerExchange.newTick);

            ////转发账户登入状态信息
            _messageExchagne.ClientLoginInfoEvent -= new ClientLoginInfoDelegate<TrdClientInfo>(_managerExchange.newSessionUpdate);

            ////帐户变动事件，当帐户设置或者相关属性发生变动时 触发该事件
            _clearCentre.AccountChangedEvent -= new AccountSettingChangedDel(_managerExchange.newAccountChanged);
            ////添加帐户
            _clearCentre.AccountAddEvent -= new AccoundIDDel(_managerExchange.newAccountAdded);
           
            _managerExchange.Dispose();
        }

        void InitWebMsgExchSrv()
        {
            debug("7.初始化WebMsgExchSrv");
            _webmsgExchange = new WebMsgExchServer(_messageExchagne, _clearCentre, _riskCentre);

            //_messageExchagne.GotTickEvent += new TickDelegate(_webmsgExchange.NewTick);
            //帐户设置变动向web端推送消息
            //_clearCentre.AccountChangedEvent +=new AccountSettingChangedDel(_webmsgExchange.NewAccountSettingUpdate);
            
            //转发账户登入状态信息
            //_messageExchagne.SendLoginInfoEvent += new LoginInfoDel(_webmsgExchange.NewSessionUpdate);

        }
        private void DestoryWebMsgExchSrv()
        {
            //_messageExchagne.GotTickEvent -= new TickDelegate(_webmsgExchange.NewTick);


            //帐户设置变动向web端推送消息
            //_clearCentre.AccountChangedEvent -= new AccountSettingChangedDel(_webmsgExchange.NewAccountSettingUpdate);

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
