﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


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
        //public static JsonWrapperAccountBankAC GetBankAC(this IAccount acc)
        //{
        //    JsonWrapperAccountBankAC bkacc = new JsonWrapperAccountBankAC();
        //    bkacc.Name = string.IsNullOrEmpty(acc.Name) ? null : acc.Name;
        //    bkacc.BankAC = acc.BankAC;
        //    bkacc.Branch = "";
        //    ContractBank bk = BasicTracker.ContractBankTracker[acc.BankID];
        //    bkacc.Bank = bk != null ? bk.Name : null;
        //    bkacc.Account = acc.ID;
        //    bkacc.AgentInfo = GetAgentInfo(acc);
        //    return bkacc;
        //}




        /// <summary>
        /// 账户成财务信息
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

            a.StkPositionCost = acc.CalcStkPositionCost();
            a.StkPositionValue = acc.CalcStkPositionMarketValue();
            a.StkCommission = acc.CalcStkCommission();
            a.StkBuyAmount = acc.CalcStkBuyAmount();
            a.StkSellAmount = acc.CalcStkSellAmount();
            a.StkMoneyFronzen = acc.CalcStkMoneyFrozen();
            a.StkRealizedPL = acc.CalcStkRealizedPL();
            a.StkAvabileFunds = acc.AvabileFunds;

            a.Margin = acc.Margin;
            a.MarginFrozen = acc.MarginFrozen;
            a.Credit = acc.Credit;
            a.LastCredit = acc.LastCredit;
            a.CreditCashIn = acc.CreditCashIn;
            a.CreditCashOut = acc.CreditCashOut;
            AccountProfile profile = BasicTracker.AccountProfileTracker[acc.ID];
            a.Name = string.IsNullOrEmpty(profile.Name) ? acc.ID : profile.Name;

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
            info.ForzenMargin = acc.MarginFrozen;
            info.BuyPower = acc.AvabileFunds;
            info.RealizedPL = acc.RealizedPL;
            info.UnRealizedPL = acc.UnRealizedPL;
            info.Commission = acc.Commission;
            info.Profit = acc.Profit;
            info.TotalPositionSize = acc.GetTotalPositionSize();
            info.Credit = acc.Credit;

            info.StkBuyAmount = acc.CalcStkBuyAmount();
            info.StkSellAmount = acc.CalcStkSellAmount();
            info.StkCommission = acc.CalcStkCommission();
            info.StkMoneyFronzen = acc.CalcStkMoneyFrozen();
            info.StkAvabileFunds = acc.AvabileFunds;
            info.StkPositoinValue = acc.CalcStkPositionMarketValue();
            info.StkPositionCost = acc.CalcStkPositionCost();
            info.StkRealizedPL = acc.CalcStkRealizedPL();
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
            info.Currency = acc.Currency;

            AccountProfile profile = BasicTracker.AccountProfileTracker[acc.ID];
            if (profile != null)
            {
                info.Name = profile.Name;
            }

            info.MGRID = acc.Mgr_fk;
            info.Deleted = acc.Deleted;
            info.RG_ID = acc.RG_FK;

            

            //如果将其他模块的数据返回
            info.Commissin_ID = acc.Commission_ID;
            info.Credit = acc.Credit;
            info.Margin_ID = acc.Margin_ID;
            info.ExStrategy_ID = acc.ExStrategy_ID;



            //if(TLCtxHelper.Version.ProductType == QSEnumProductType.VendorMoniter)
            //{
            //    IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(acc.ID);
            //    if(broker != null)
            //    {
            //        int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(acc.ID);
            //        ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(id);
            //        info.ConnectorToken = cfg!= null?(string.Format("{0}-{1}",cfg.Name,cfg.usrinfo_userid)):"";
            //        info.MAcctConnected = broker.IsLive;
            //        Util.Debug(string.Format("Broker:{0} Connected:{1}", broker.Token, broker.IsLive));
            //    }

            //    IAccountCheck rs = acc.AccountChecks.Where(check => check.GetType().FullName.Equals("AccountRuleSet.RSVendorFlat")).FirstOrDefault();
            //    info.MAcctRiskRule = rs != null ? rs.RuleDescription : "未设置";
            
            //}
            if (TLCtxHelper.Version.ProductType == QSEnumProductType.CounterSystem)
            {

                info.IsLogin = acc.IsLogin;
            }

            info.IsWarn = acc.IsWarn;
            return info;
        }
    }
}
