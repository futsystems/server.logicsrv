using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.MainAcctFinService
{
    public class FinGlobal
    {
        static FinGlobal defaultinstance = null;
        static FinGlobal()
        { 
            defaultinstance = new FinGlobal();
        }

        private FinGlobal()
        { 
            
        }

        public static void Init()
        {
            FinServiceTracker ft = FinGlobal.FinServiceTracker;
            BrokerTransTracker tt = FinGlobal.BrokerTransTracker;
            BrokerAccountInfoTracker at = FinGlobal.BrokerAccountInfoTracker;
        }
        FinServiceTracker _fstracker = null;
        public static FinServiceTracker FinServiceTracker
        {
            get
            {
                if (defaultinstance._fstracker == null)
                    defaultinstance._fstracker = new FinServiceTracker();
                return defaultinstance._fstracker;
            }
        }


        BrokerTransTracker _txntracker = null;
        public static BrokerTransTracker BrokerTransTracker
        {
            get
            {
                if (defaultinstance._txntracker == null)
                    defaultinstance._txntracker = new BrokerTransTracker();
                return defaultinstance._txntracker;
            }
        }

        BrokerAccountInfoTracker _acctinfotracker = null;
        public static BrokerAccountInfoTracker BrokerAccountInfoTracker
        {
            get
            {
                if (defaultinstance._acctinfotracker == null)
                    defaultinstance._acctinfotracker = new BrokerAccountInfoTracker();
                return defaultinstance._acctinfotracker;
            }
        }
    }
}
