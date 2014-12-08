using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;

namespace TradingLib.Common
{
    public class BasicTracker:IDisposable
    {

        static BasicTracker defaultinstance;
        DBExchangeTracker extracker;
        DBSecurityTracker setracker;
        SymbolTracker symtracker;
        DBMarketTimeTracker mktimetracker;

        DBManagerTracker mgrtracker;

        DBContractBankTracker banktracker;

        RouterGrouperTracker rgtracker;
        VendorTracker vendortracker;

        DomainTracker domaintracker;

        ConnectorConfigTracker connectorcfgtracker;
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
            Util.Debug("xxxxxxxxx basictracker disposed.....");
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
                defaultinstance.Dispose();
                defaultinstance = null;
            }
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
        public static DBSecurityTracker SecurityTracker
        {
            get
            {
                if (defaultinstance.setracker == null)
                    defaultinstance.setracker = new DBSecurityTracker();
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
        public static VendorTracker VendorTracker
        {
            get
            {
                if (defaultinstance.vendortracker == null)
                    defaultinstance.vendortracker = new VendorTracker();
                return defaultinstance.vendortracker;
            }
        }

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
        //public static void Release()
        //{
        //    defaultinstance.extracker = null;
        //    defaultinstance.mktimetracker = null;
        //    defaultinstance.setracker = null;
        //    defaultinstance.symtracker = null;
        //}
    }
}
