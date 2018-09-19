using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 基础信息维护器
    /// 
    /// </summary>
    public class BasicTracker:IDisposable
    {

        static BasicTracker defaultinstance;

        //交易所 品种 合约
        CalendarTracker calendartracker;
        DBExchangeTracker extracker;
        DBMarketTimeTracker mktimetracker;
        SecurityTracker setracker;
        SymbolTracker symtracker;

        //
        DomainTracker domaintracker;
        DBManagerTracker mgrtracker;

        AgentTracker agentTracker;
        UIAccessTracker uiaccesstracker;
        DBContractBankTracker banktracker;

        //
        ConnectorConfigTracker connectorcfgtracker;
        RouterGrouperTracker rgtracker;
        //VendorTracker vendortracker;

        //帐户绑定主帐户
        AccountConnectorTracker accconnectortracker;

        //手续费模板
        CommissionTemplateTracker commissiontracker;
        //保证金模板
        MarginTemplateTracker margintracker;
        //计算策略模板
        ExStrategyTemplateTracker exstrategytracker;
        //配置模板
        ConfigTemplateTracker cfgtemplatetracker;
        
        //交易帐户个人信息维护器
        AccountProfileTracker accprofiletracker;
        
        //汇率数据维护器
        ExchangeRateTracker exchangeratetracker;

        PowerDataTracker powerdatatracker;

        //结算价维护期
        SettlementPriceTracker settlementpricetracker;

        static BasicTracker()
        {
            defaultinstance = new BasicTracker();
        }

        private BasicTracker()
        { 
        
        }

        public void Dispose()
        {
            // Do something here
            //Util.Debug("xxxxxxxxx basictracker disposed.....");
        }

        public static void DisposeInstance()
        {
            if (defaultinstance != null)
            {
                defaultinstance.extracker = null;
                defaultinstance.mktimetracker = null;
                defaultinstance.setracker = null;
                defaultinstance.symtracker = null;
                defaultinstance.rgtracker = null;
                defaultinstance.uiaccesstracker = null;
                //defaultinstance.Dispose();
                //defaultinstance = null;
            }
        }

        public static void Init()
        { 
            
        }
        /// <summary>
        /// 管理员对象管理器
        /// </summary>
        public static DBManagerTracker ManagerTracker
        {
            get
            {
                if (defaultinstance.mgrtracker == null)
                    defaultinstance.mgrtracker = new DBManagerTracker();
                return defaultinstance.mgrtracker;
            }
        }


        /// <summary>
        /// 日历对象维护器
        /// </summary>
        public static CalendarTracker CalendarTracker
        {
            get
            {
                if (defaultinstance.calendartracker == null)
                {
                    defaultinstance.calendartracker = new CalendarTracker();
                }
                return defaultinstance.calendartracker;
            }
        }
        /// <summary>
        /// 交易所对象管理器
        /// </summary>
        public static DBExchangeTracker ExchagneTracker
        {
            get
            {
                if (defaultinstance.extracker == null)
                    defaultinstance.extracker = new DBExchangeTracker();
                return defaultinstance.extracker;
            }
        }

        /// <summary>
        /// 证券品种管理器
        /// </summary>
        public static SecurityTracker SecurityTracker
        {
            get
            {
                if (defaultinstance.setracker == null)
                    defaultinstance.setracker = new SecurityTracker();
                return defaultinstance.setracker;
            }
        }

        /// <summary>
        /// 合约对象管理器
        /// </summary>
        public static SymbolTracker SymbolTracker
        {
            get
            {
                if (defaultinstance.symtracker == null)
                    defaultinstance.symtracker = new SymbolTracker();
                return defaultinstance.symtracker;
            }
        }

        /// <summary>
        /// 交易时间段管理器
        /// </summary>
        public static DBMarketTimeTracker MarketTimeTracker
        {
            get
            {
                if (defaultinstance.mktimetracker == null)
                    defaultinstance.mktimetracker = new DBMarketTimeTracker();
                return defaultinstance.mktimetracker;
            }
        }

        /// <summary>
        /// 签约银行列表
        /// </summary>
        public static DBContractBankTracker ContractBankTracker
        {
            get
            {
                if (defaultinstance.banktracker == null)
                    defaultinstance.banktracker = new DBContractBankTracker();
                return defaultinstance.banktracker;
            }
        }

        /// <summary>
        /// 获得路由组维护器
        /// </summary>
        public static RouterGrouperTracker RouterGroupTracker
        {
            get
            {
                if (defaultinstance.rgtracker == null)
                    defaultinstance.rgtracker = new RouterGrouperTracker();
                return defaultinstance.rgtracker;
            }
        }

        /// <summary>
        /// 获得实盘帐户维护器
        /// </summary>
        //public static VendorTracker VendorTracker
        //{
        //    get
        //    {
        //        if (defaultinstance.vendortracker == null)
        //            defaultinstance.vendortracker = new VendorTracker();
        //        return defaultinstance.vendortracker;
        //    }
        //}

        
        /// <summary>
        /// 域维护器
        /// </summary>
        public static DomainTracker DomainTracker
        {
            get
            {
                if (defaultinstance.domaintracker == null)
                    defaultinstance.domaintracker = new DomainTracker();
                return defaultinstance.domaintracker;
            }
        }

        /// <summary>
        /// 通道参数维护器
        /// </summary>
        public static ConnectorConfigTracker ConnectorConfigTracker
        {
            get
            {
                if (defaultinstance.connectorcfgtracker == null)
                    defaultinstance.connectorcfgtracker = new ConnectorConfigTracker();
                return defaultinstance.connectorcfgtracker;
            }
        }

        /// <summary>
        /// 获得权限维护器
        /// </summary>
        public static UIAccessTracker UIAccessTracker
        {
            get
            {
                if (defaultinstance.uiaccesstracker == null)
                    defaultinstance.uiaccesstracker = new UIAccessTracker();
                return defaultinstance.uiaccesstracker;
            }
        }

        /// <summary>
        /// 手续费模板维护器
        /// </summary>
        public static CommissionTemplateTracker CommissionTemplateTracker
        {
            get
            {
                if (defaultinstance.commissiontracker == null)
                    defaultinstance.commissiontracker = new CommissionTemplateTracker();
                return defaultinstance.commissiontracker;
            }
        }

        /// <summary>
        /// 保证金模板维护器
        /// </summary>
        public static MarginTemplateTracker MarginTemplateTracker
        {
            get
            {
                if (defaultinstance.margintracker == null)
                    defaultinstance.margintracker = new MarginTemplateTracker();
                return defaultinstance.margintracker;
            }
        }

        /// <summary>
        /// 计算策略模板维护器
        /// </summary>
        public static ExStrategyTemplateTracker ExStrategyTemplateTracker
        {
            get
            {
                if (defaultinstance.exstrategytracker == null)
                    defaultinstance.exstrategytracker = new ExStrategyTemplateTracker();
                return defaultinstance.exstrategytracker;
            }
        }

        /// <summary>
        /// 配置模板维护器
        /// </summary>
        public static ConfigTemplateTracker ConfigTemplateTracker
        {
            get
            {
                if (defaultinstance.cfgtemplatetracker == null)
                    defaultinstance.cfgtemplatetracker = new ConfigTemplateTracker();
                return defaultinstance.cfgtemplatetracker;
            }
        }
        /// <summary>
        /// 交易帐户通道绑定维护器
        /// </summary>
        public static AccountConnectorTracker ConnectorMapTracker
        {
            get
            {
                if (defaultinstance.accconnectortracker == null)
                    defaultinstance.accconnectortracker = new AccountConnectorTracker();
                return defaultinstance.accconnectortracker;
            }
        
        }

        /// <summary>
        /// 交易帐户个人信息维护器
        /// </summary>
        public static AccountProfileTracker AccountProfileTracker
        {
            get
            {
                if (defaultinstance.accprofiletracker == null)
                    defaultinstance.accprofiletracker = new AccountProfileTracker();
                return defaultinstance.accprofiletracker;
            }
        }

        /// <summary>
        /// 汇率数据维护器
        /// </summary>
        public static ExchangeRateTracker ExchangeRateTracker
        {
            get
            {
                if (defaultinstance.exchangeratetracker == null)
                    defaultinstance.exchangeratetracker = new ExchangeRateTracker();
                return defaultinstance.exchangeratetracker;
            }
        }


        /// <summary>
        /// 除权数据维护器
        /// </summary>
        public static PowerDataTracker PowerDataTracker
        {
            get
            {
                if (defaultinstance.powerdatatracker == null)
                    defaultinstance.powerdatatracker = new PowerDataTracker();
                return defaultinstance.powerdatatracker;
            }
        }

        /// <summary>
        /// 结算价维护器
        /// </summary>
        public static SettlementPriceTracker SettlementPriceTracker
        {
            get
            {
                if (defaultinstance.settlementpricetracker == null)
                    defaultinstance.settlementpricetracker = new SettlementPriceTracker();
                return defaultinstance.settlementpricetracker;
            }
        }

        /// <summary>
        /// 代理财务账户维护器
        /// </summary>
        public static AgentTracker AgentTracker
        {
            get 
            {
                if (defaultinstance.agentTracker == null)
                    defaultinstance.agentTracker = new AgentTracker();
                return defaultinstance.agentTracker;
            }
        }

        //public static void Release()
        //{
        //    defaultinstance.extracker = null;
        //    defaultinstance.mktimetracker = null;
        //    defaultinstance.setracker = null;
        //    defaultinstance.symtracker = null;
        //}
    }
}
