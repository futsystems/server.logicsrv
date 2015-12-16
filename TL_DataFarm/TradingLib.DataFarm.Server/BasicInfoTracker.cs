using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 全局单例对象
    /// 用于维护基础数据
    /// </summary>
    public class MDBasicTracker
    {
        static MDBasicTracker defaultinstance;

        static MDBasicTracker()
        {
            defaultinstance = new MDBasicTracker();
        }

        private MDBasicTracker()
        {

        }

        //交易所 品种 合约
        CalendarTracker calendartracker;
        DBExchangeTracker extracker;
        DBMarketTimeTracker mktimetracker;

        DBSecurityTracker setracker;
        DBSymbolTracker symtracker;

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

        
    }
}
