using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    partial class MainForm
    {


        #region 系统管理类回调
        public void OnMGRConnectorResponse(ConnectorInfo c, bool islast)
        {
            if (routerform != null)
            {
                routerform.GotConnector(c, islast);
            }
        }


        public void OnMGRMarketTimeResponse(MarketTime mt, bool islast)
        {
            if (basicinfotracker != null)
            {
                basicinfotracker.GotMarketTime(mt);
            }
            //当最后一条交易时间回报时,我们进行交易所查询
            if (islast && !_basicinfodone)
            {
                ShowInfo("交易时间查询完毕,查询交易所信息");
                Globals.TLClient.ReqQryExchange();
            }
        
        }
        public void OnMGRExchangeResponse(Exchange ex, bool islast)
        {
            if (basicinfotracker != null)
            {
                basicinfotracker.GotExchange(ex);
            }
            if (islast && !_basicinfodone)
            {
                //绑定交易所列表
                if (securityform != null)
                {
                    securityform.ReBindExchangeCombList();
                }
                if (symbolform != null)
                {
                    symbolform.ReBindExchangeCombList();
                }

                //
                ShowInfo("交易所查询完毕,查询品种信息");
                Globals.TLClient.ReqQrySecurity();

            }

        }


        public void OnMGRSecurityResponse(SecurityFamilyImpl sec, bool islast)
        {
            if (basicinfotracker != null)
            {
                basicinfotracker.GotSecurity(sec);
            }

            if (islast && !_basicinfodone)
            {
                ShowInfo("品种查询完毕,查询合约信息");
                Globals.TLClient.ReqQrySymbol();
            }

           
        }

        public void OnMGRSecurityAddResponse(SecurityFamilyImpl security, bool islast)
        {
            if (basicinfotracker != null)
            {
                basicinfotracker.GotSecurity(security);
            }        
        }


        public void OnMGRSymbolResponse(SymbolImpl sym, bool islast)
        {
            if (basicinfotracker != null)
            {
                basicinfotracker.GotSymbol(sym);
            }
            if (islast)
            {
                ShowInfo("合约查询完毕,查询委托风控规则");
                Globals.TLClient.ReqQryRuleSet();
            }
        }

        public void OnMGRRuleClassResponse(RuleClassItem item, bool islast)
        {
            //debug("111mainform ruleclass handler: is last:" + islast.ToString());
            if (basicinfotracker != null)
            {
                basicinfotracker.GotRuleClass(item);
            }
            //debug("mainform ruleclass handler: is last:" + islast.ToString());
            if (islast)
            {
                ShowInfo("风控规则下载完毕，下载管理员列表");
                tlclient.ReqQryManager();
                //basicinfotracker.OnFinishLoad();//数据加载完毕后调用 用于建立对象绑定并进行界面

            }
        }

        public void OnMGRMangerResponse(Manager manger, bool islast)
        {
            debug("xxxxxxxxxxxxxxxxxxxxx here:"+manger.Name);
            if (basicinfotracker != null)
            {
                basicinfotracker.GotManager(manger);
            }
            //debug("mainform ruleclass handler: is last:" + islast.ToString());
            if (islast)
            {
                ShowInfo("基础信息下载完成,下载帐户信息");
                tlclient.ReqQryAccountList();
                basicinfotracker.OnFinishLoad();//数据加载完毕后调用 用于建立对象绑定并进行界面

            }
        }


        public void OnMGRSymbolAddResponse(SymbolImpl symbol, bool islast)
        {
            if (basicinfotracker != null)
            {
                basicinfotracker.GotSymbol(symbol);
            }

        
        }

        #endregion


        



        #region 行情处理
        /// <summary>
        /// 行情回报处理
        /// </summary>
        /// <param name="k"></param>
        public void OnTick(Tick k)
        {
            //debug("main form got tick:" + k.ToString());
            infotracker.GotTick(k);
            ctAccountMontier1.GotTick(k);
            
        }

        /// <summary>
        /// 获得服务端委托回报
        /// </summary>
        /// <param name="o"></param>
        public void OnOrder(Order o)
        {
            infotracker.GotOrder(o);
            ctAccountMontier1.GotOrder(o);
        }

        /// <summary>
        /// 获得服务端昨日持仓回报
        /// </summary>
        /// <param name="pos"></param>
        public void OnHoldPosition(Position pos)
        {
            infotracker.GotHoldPosition(pos);
        }

        /// <summary>
        /// 获得服务端成交回报
        /// </summary>
        /// <param name="f"></param>
        public void OnTrade(Trade f)
        {
            infotracker.GotFill(f);
            ctAccountMontier1.GotTrade(f);
        }


        /// <summary>
        /// 持仓更新回报
        /// </summary>
        /// <param name="pos"></param>
        public void OnPositionUpdate(Position pos)
        { 
           
        }

        #endregion


        /// <summary>
        /// 收到交易帐户
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountLite(IAccountLite account, bool islast)
        {
            ctAccountMontier1.GotAccount(account);
            if (islast)
            {
                ShowInfo("帐户数据下载完成");
                //填充表格
                string info = "帐户数据填充中";
                while (ctAccountMontier1.AnyAccountInCache)
                {

                    ShowInfo(info += ".");
                    Thread.Sleep(100);
                }
                _basicinfodone = true;
            }
        }

        public void OnAccountInfoLite(IAccountInfoLite account)
        {
            ctAccountMontier1.GotAccountInfoLite(account);
        }

        public void OnMGRResumeResponse(RspMGRResumeAccountResponse response)
        {
            ctAccountMontier1.GotMGRResumeResponse(response);
        }


        public void OnMGRSessionUpdate(NotifyMGRSessionUpdateNotify notify)
        {
            ctAccountMontier1.GotMGRSessionUpdate(notify);
        }

        public void OnAccountInfo(IAccountInfo accountinfo)
        {
            
            ctAccountMontier1.GotAccountInfo(accountinfo);
            
        }

        /// <summary>
        /// 交易帐户变动，用于更新帐户变化
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountChagne(IAccountLite account)
        {
            ctAccountMontier1.GotAccountChanged(account);
        }


        #region 风控类接口实现
        

        /// <summary>
        /// 帐户风控规则项目回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        public void OnMGRRuleItemResponse(RuleItem item, bool islast)
        {
            ctAccountMontier1.GotRuleItem(item, islast);
        }

        public void OnMGRRuleItemUpdate(RuleItem item, bool islast)
        {
            ctAccountMontier1.GotRuleItem(item, islast);
        }

        public void OnMGRRulteItemDelete(RuleItem item, bool islast)
        {

            ctAccountMontier1.GotRuleItemDel(item, islast);
        }

        #endregion


        #region 系统状态与通知类
        public void OnMGRSytstemStatus(SystemStatus status, bool islast)
        {
            if (systemstatusfrom != null)
            {
                systemstatusfrom.GotSystemStatus(status);
            }
        
        }
        #endregion


        #region 查询历史记录
        public void OnMGROrderResponse(Order o, bool islast)
        {
            histqryform.GotHistOrder(o, islast);
        }
        public void OnMGRTradeResponse(Trade f, bool islast)
        { 
            //histqryform
            histqryform.GotHistTrade(f, islast);
        
        }

        public void OnMGRPositionResponse(SettlePosition pos, bool islast)
        {
            histqryform.GotHistPosition(pos, islast);
        }

        public void OnMGRCashTransactionResponse(CashTransaction c, bool islast)
        {
            histqryform.GotHistCashTransaction(c, islast);
        }
        public void OnMGRSettlementResponse(RspMGRQrySettleResponse response)
        {
            histqryform.GotHistSettlement(response);
        }
        #endregion

        string genmessage(RspInfo info)
        {
            if (info.ErrorID == 0)
            {
                return "操作成功!";
            }
            else
            {
                return "操作失败,原因:"+info.ErrorMessage;
            }
        }
        public void PopRspInfo(RspInfo info)
        {
            StatusMessage(genmessage(info));
        }
    }
}
