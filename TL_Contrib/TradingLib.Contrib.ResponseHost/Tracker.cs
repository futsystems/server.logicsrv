using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.ResponseHost
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

        ArgumentTracker _argumenttracker = null;
        ResponseTracker _responsetracker = null;
        ResponseTemplateTracker _responsetemplatetracker = null;

        /// <summary>
        /// 参数维护器
        /// </summary>
        public static ArgumentTracker ArgumentTracker
        {
            get
            {
                if (defaultinstance._argumenttracker == null)
                    defaultinstance._argumenttracker = new ArgumentTracker();
                return defaultinstance._argumenttracker;
            }
        }

        /// <summary>
        /// 策略实例维护器
        /// </summary>
        public static ResponseTracker ResponseTracker
        {
            get
            {
                if (defaultinstance._responsetracker == null)
                    defaultinstance._responsetracker = new ResponseTracker();
                return defaultinstance._responsetracker;
            }
        }

        /// <summary>
        /// 策略模板维护器
        /// </summary>
        public static ResponseTemplateTracker ResponseTemplateTracker
        {
            get
            {
                if (defaultinstance._responsetemplatetracker == null)
                    defaultinstance._responsetemplatetracker = new ResponseTemplateTracker();
                return defaultinstance._responsetemplatetracker;
            }
        }

    }
}
