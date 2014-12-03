using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.ServiceManager
{
    public class BrokerXAPIHelper
    {

        /// <summary>
        /// 验证某个成交接口是否有效
        /// 这里的有效是指程序是否可以正常加载 不保证与服务端之间的功能通讯
        /// </summary>
        /// <param name="itface"></param>
        /// <returns></returns>
        public static bool ValidBrokerInterface(ConnectorInterface itface)
        {
            return XAPIHelper.ValidBrokerInterface(itface.libpath_broker, itface.libname_broker, itface.libpath_wrapper, itface.libname_wrapper);
        }
        
    }
}
