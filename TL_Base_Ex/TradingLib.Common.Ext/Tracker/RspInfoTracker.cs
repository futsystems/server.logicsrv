using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class RspInfoTracker
    {


        XMLRspInfoTracker webapitracker = null;
        XMLRspInfoTracker exRspInfoTracker = null;

        static RspInfoTracker _defaultinstance = null;
        static RspInfoTracker()
        {
            _defaultinstance = new RspInfoTracker();
            
        }

        private RspInfoTracker()
        { 
            
        }


        //public static void Init()
        //{
        //    _defaultinstance.webapitracker = new XMLRspInfoTracker("WEBAPI", "webapierror.xml", "errors");
        //    _defaultinstance.exRspInfoTracker = new XMLRspInfoTracker("EX", "error.xml", "errors");
        //}


        /// <summary>
        /// 获得web同步调用的回报信息维护器
        /// </summary>
        public static XMLRspInfoTracker WebAPIRspInfo
        {
            get
            {
                if (_defaultinstance.webapitracker == null)
                    _defaultinstance.webapitracker = new XMLRspInfoTracker("WEBAPI", "webapierror.xml", "errors");
                return _defaultinstance.webapitracker;
            }
        }

        /// <summary>
        /// 交易RspInfo维护器
        /// </summary>
        public static XMLRspInfoTracker ExRspInfo
        {
            get
            {
                if (_defaultinstance.exRspInfoTracker == null)
                    _defaultinstance.exRspInfoTracker = new XMLRspInfoTracker("EX", "error.xml", "errors");
                return _defaultinstance.exRspInfoTracker;
            }
        }
    }
}
