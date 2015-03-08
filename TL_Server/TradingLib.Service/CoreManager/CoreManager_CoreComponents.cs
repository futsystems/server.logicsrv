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

        }

        private void DestoryClearCentre()
        {
            _clearCentre.Dispose();
        }


        //初始化风控中心
        private void InitRiskCentre()
        {
            debug("3.初始化RiskCentre");
            _riskCentre = new RiskCentre();
        }

        private void DestoryRiskCentre()
        {
            _riskCentre.Dispose();
        }

        //初始化datafeedrouter
        private void InitDataFeedRouter()
        {
            debug("4.初始化DataFeedRouter");
            _datafeedRouter = new DataFeedRouter();
            _messageExchagne.BindDataRouter(_datafeedRouter);
        }

        void DestoryDataFeedRouter()
        {
            _messageExchagne.UnBindDataRouter(_datafeedRouter);

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
            _managerExchange = new MgrExchServer();
        }
        private void DestoryMgrExchSrv()
        {
            _managerExchange.Dispose();
        }

        void InitWebMsgExchSrv()
        {
            debug("7.初始化WebMsgExchSrv");
            _webmsgExchange = new WebMsgExchServer();
        }
        private void DestoryWebMsgExchSrv()
        {
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
