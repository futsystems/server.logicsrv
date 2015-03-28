using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Common
{
    public static class AccountUtils_ObjectWrapper
    {

        public static string GetAgentInfo(this IAccount acc)
        {
            Manager basemgr = BasicTracker.ManagerTracker[acc.Mgr_fk];
            if (basemgr != null)
            {
                if (basemgr.Type == QSEnumManagerType.ROOT)
                {
                    return "主域(" + acc.Mgr_fk + ")-" + basemgr.Name;
                }
                else
                {
                    return "代理域(" + acc.Mgr_fk + ")-" + basemgr.Name;
                }
            }
            return "未设置";
        }
        public static JsonWrapperAccountBankAC GetBankAC(this IAccount acc)
        {
            JsonWrapperAccountBankAC bkacc = new JsonWrapperAccountBankAC();
            bkacc.Name = string.IsNullOrEmpty(acc.Name) ? null : acc.Name;
            bkacc.BankAC = acc.BankAC;
            bkacc.Branch = "";
            ContractBank bk = BasicTracker.ContractBankTracker[acc.BankID];
            bkacc.Bank = bk != null ? bk.Name : null;
            bkacc.Account = acc.ID;
            bkacc.AgentInfo = GetAgentInfo(acc);
            return bkacc;
        }




        /// <summary>
        /// 生成财务信息
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static AccountInfo GenAccountInfo(this IAccount acc)
        {
            AccountInfo a = new AccountInfo();
            a.LastEquity = acc.LastEquity;
            a.NowEquity = acc.NowEquity;
            a.RealizedPL = acc.RealizedPL;
            a.UnRealizedPL = acc.UnRealizedPL;
            a.Commission = acc.Commission;
            a.CashIn = acc.CashIn;
            a.CashOut = acc.CashOut;
            a.Profit = acc.Profit;
            a.MoneyUsed = acc.MoneyUsed;
            a.Category = acc.Category;
            a.OrderRouteType = acc.OrderRouteType;
            a.Execute = acc.Execute;
            a.IntraDay = acc.IntraDay;
            a.Account = acc.ID;

            a.FutMarginUsed = acc.CalFutMargin();
            a.FutMarginFrozen = acc.CalFutMarginFrozen();
            a.FutRealizedPL = acc.CalFutRealizedPL();
            a.FutUnRealizedPL = acc.CalFutUnRealizedPL();
            a.FutCommission = acc.CalFutCommission();

            a.OptPositionCost = acc.CalOptPositionCost();
            a.OptPositionValue = acc.CalOptPositionValue();
            a.OptRealizedPL = acc.CalOptRealizedPL();
            a.OptCommission = acc.CalOptCommission();

            a.FutCash = acc.CalFutCash();
            a.FutLiquidation = acc.CalFutLiquidation();
            a.FutMoneyUsed = acc.CalFutMoneyUsed();


            a.OptCash = acc.CalOptCash();
            a.OptMarketValue = acc.CalOptPositionValue();
            a.OptLiquidation = acc.CalOptLiquidation();
            a.OptMoneyUsed = acc.CalOptMoneyUsed();


            a.TotalLiquidation = acc.TotalLiquidation;
            a.AvabileFunds = acc.AvabileFunds;
            a.FutAvabileFunds = acc.AvabileFunds; ;
            a.OptAvabileFunds = acc.AvabileFunds;

            a.InnovPositionCost = acc.CalInnovPositionCost();
            a.InnovPositionValue = acc.CalInnovPositionValue();
            a.InnovCommission = acc.CalInnovCommission() ;
            a.InnovRealizedPL = acc.CalInnovRealizedPL();
            a.InnovMargin = acc.CalInnovMargin();
            a.InnovCash = acc.CalInnovCash();
            a.InnovMarketValue = acc.CalInnovPositionValue();
            a.InnovLiquidation = acc.CalInnovLiquidation();
            a.InnovMoneyUsed = acc.CalInnovMoneyUsed();
            a.InnovAvabileFunds = acc.AvabileFunds;

            a.Margin = acc.Margin;
            a.MarginFrozen = acc.MarginFrozen;
            a.Credit = acc.Credit;
            
            return a;
        }

        /// <summary>
        /// 生成当前实时财务统计信息
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static AccountInfoLite GenAccountInfoLite(this IAccount acc)
        {
            AccountInfoLite info = new AccountInfoLite();
            info.Account = acc.ID;
            info.NowEquity = acc.NowEquity;
            info.Margin = acc.Margin;
            info.ForzenMargin = acc.MarginFrozen;// acc.ForzenMargin;
            info.BuyPower = acc.AvabileFunds;
            info.RealizedPL = acc.RealizedPL;
            info.UnRealizedPL = acc.UnRealizedPL;
            info.Commission = acc.Commission;
            info.Profit = acc.Profit;
            info.TotalPositionSize = acc.GetTotalPositionSize();
            info.Credit = acc.Credit;
            return info;
        }

        /// <summary>
        /// 生成客户端使用的交易帐户信息
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static AccountLite GenAccountLite(this IAccount acc)
        {
            AccountLite info = new AccountLite();
            info.Account = acc.ID;
            info.CashIn = acc.CashIn;
            info.CashOut = acc.CashOut;
            info.Category = acc.Category;
            info.Commission = acc.Commission;
            info.Execute = acc.Execute;
            info.IntraDay = acc.IntraDay;
            info.LastEquity = acc.LastEquity;
            info.MoneyUsed = acc.MoneyUsed;
            info.NowEquity = acc.NowEquity;
            info.OrderRouteType = acc.OrderRouteType;
            info.Profit = acc.Profit;
            info.RealizedPL = acc.RealizedPL;
            info.UnRealizedPL = acc.UnRealizedPL;
            
            info.Name = acc.Name;
            AccountProfile profile = BasicTracker.AccountProfileTracker[acc.ID];
            if (profile != null)
            {
                info.Name = profile.Name;
            }

            info.Broker = acc.Broker;
            info.BankID = acc.BankID;
            info.BankAC = acc.BankAC;
            //info.PosLock = acc.PosLock;
            info.MGRID = acc.Mgr_fk;
            info.Deleted = acc.Deleted;
            info.RG_ID = acc.RG_FK;

            //IEnumerable<ClientInfoBase> clients = TLCtxHelper.Ctx.MessageExchange.GetNotifyTargets(info.Account);
            //info.IsLogin = clients.Count() > 0;
            //info.IPAddress = info.IsLogin ? clients.FirstOrDefault().IPAddress : "";
            //info.SideMargin = acc.SideMargin;
            //如果将其他模块的数据返回
            info.Commissin_ID = acc.Commission_ID;
            info.Credit = acc.Credit;
            //info.CreditSeparate = acc.CreditSeparate;
            info.Margin_ID = acc.Margin_ID;
            info.ExStrategy_ID = acc.ExStrategy_ID;
            if(TLCtxHelper.Version.ProductType == QSEnumProductType.VendorMoniter)
            {
                IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(acc.ID);
                if(broker != null)
                {
                    int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(acc.ID);
                    ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(id);
                    info.ConnectorToken = cfg!= null?(string.Format("{0}-{1}",cfg.Token,cfg.usrinfo_userid)):"";
                    info.MAcctConnected = broker.IsLive;
                    Util.Debug(string.Format("Broker:{0} Connected:{1}", broker.Token, broker.IsLive));
                }
            }
            //info.ConnectorToken = TLCtxHelper.Version.ProductType== QSEnumProductType.VendorMoniter ?BasicTracke
            return info;
        }

        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Settlement ToSettlement(this IAccount account)
        {
            Settlement settle = new SettlementImpl();
            settle.Account = account.ID;
            settle.CashIn = account.CashIn;
            settle.CashOut = account.CashOut;
            settle.Commission = account.Commission;
            settle.Confirmed = false;
            settle.LastEquity = account.LastEquity;
            settle.RealizedPL = account.RealizedPL;
            settle.UnRealizedPL = account.SettleUnRealizedPL;
            settle.NowEquity = settle.LastEquity + settle.RealizedPL + settle.UnRealizedPL - settle.Commission + settle.CashIn - settle.CashOut;

            //指定交易日期
            settle.SettleDay = Util.ToTLDate();
            settle.SettleTime = Util.ToTLTime();
            return settle;
        }
    }
}
