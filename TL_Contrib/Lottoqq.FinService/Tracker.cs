using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Contrib.FinService
{
    /// <summary>
    /// 单例Tracker
    /// </summary>
    public class FinTracker
    {
        static FinTracker defaultinstance;
        static FinTracker()
        {
            defaultinstance = new FinTracker();
        }

        private FinTracker()
        { 
        
        }


        ServicePlanTracker _serviceplantracker = null;
        ArgumentTracker _argtracker = null;
        FinServiceTracker _finsservicetracker = null;



        public static ServicePlanTracker ServicePlaneTracker
        {
            get
            {
                if (defaultinstance._serviceplantracker == null)
                    defaultinstance._serviceplantracker = new ServicePlanTracker();
                return defaultinstance._serviceplantracker;
            }
        }


        public static ArgumentTracker ArgumentTracker
        {
            get
            {
                if (defaultinstance._argtracker == null)
                    defaultinstance._argtracker = new ArgumentTracker();
                return defaultinstance._argtracker;
            }
        }


        public static FinServiceTracker FinServiceTracker
        {
            get
            {
                if (defaultinstance._finsservicetracker == null)
                    defaultinstance._finsservicetracker = new FinServiceTracker();
                return defaultinstance._finsservicetracker;
            }
        }
    }
}
