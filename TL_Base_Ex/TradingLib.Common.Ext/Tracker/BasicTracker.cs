using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;

namespace TradingLib.Common
{
    public class BasicTracker
    {

        static BasicTracker defaultinstance;
        DBExchangeTracker extracker;
        DBSecurityTracker setracker;
        DBSymbolTracker symtracker;
        DBMarketTimeTracker mktimetracker;

        DBManagerTracker mgrtracker;

        DBContractBankTracker banktracker;

        static BasicTracker()
        {
            defaultinstance = new BasicTracker();
        }

        public BasicTracker()
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
        public static DBSymbolTracker SymbolTracker
        {
            get
            {
                if (defaultinstance.symtracker == null)
                    defaultinstance.symtracker = new DBSymbolTracker();
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

        public static void Release()
        {
            defaultinstance.extracker = null;
            defaultinstance.mktimetracker = null;
            defaultinstance.setracker = null;
            defaultinstance.symtracker = null;
        }
    }
}
