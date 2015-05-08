using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.Race
{
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

        RaceTracker _racetracker = null;

        public static RaceTracker RaceTracker
        {
            get
            {
                if (defaultinstance._racetracker == null)
                    defaultinstance._racetracker = new RaceTracker();
                return defaultinstance._racetracker;
            }
        }

        RaceServiceTracker _raceservicetracker = null;
        public static RaceServiceTracker RaceServiceTracker
        {
            get
            {
                if (defaultinstance._raceservicetracker == null)
                    defaultinstance._raceservicetracker = new RaceServiceTracker();
                return defaultinstance._raceservicetracker;
            }
        }
    
    
    }
}
