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
    public class Tracker
    {
        static Tracker defaultinstance;
        static Tracker()
        {
            defaultinstance = new Tracker();
        }

        private Tracker()
        { 
        
        }


        ServicePlanTrcker _serviceplantracker = null;

        public static ServicePlanTrcker ServicePlaneTracker
        {
            get
            {
                if (defaultinstance._serviceplantracker == null)
                    defaultinstance._serviceplantracker = new ServicePlanTrcker();
                return defaultinstance._serviceplantracker;
            }
        }


    }
}
