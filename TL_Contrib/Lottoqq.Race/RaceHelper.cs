using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottoqq.Race
{
    public class RaceHelper
    {
        public static string PRStatisticKey(PRStatistic prs)
        {
            return prs.Account + "-" + prs.Type.ToString();
        }
        static RaceHelper defaultinstance;
        RaceServiceTracker _rstracker;

        static RaceHelper()
        {
            defaultinstance = new RaceHelper();
        }

        public RaceHelper()
        { 
        
        }


        public static RaceServiceTracker RaceServiceTracker
        {
            get
            {
                if (defaultinstance._rstracker == null)
                    defaultinstance._rstracker = new RaceServiceTracker();
                return defaultinstance._rstracker;
            }
        }

    }
}
