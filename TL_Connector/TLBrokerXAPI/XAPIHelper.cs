using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.BrokerXAPI.Interop;

namespace TradingLib.BrokerXAPI
{
    public class XAPIHelper
    {

        /// <summary>
        /// 验证配置文件是否正确 是否可以正常加载对应接口
        /// </summary>
        /// <param name="brokerPath"></param>
        /// <param name="brokerName"></param>
        /// <param name="wrapperPath"></param>
        /// <param name="wrapperName"></param>
        /// <returns></returns>
        public static bool ValidBrokerInterface(string brokerPath,string brokerName,string wrapperPath,string wrapperName)
        {
            bool re1 = TLBrokerProxy.ValidBrokerProxy(brokerPath, brokerName);
            bool re2 = TLBrokerWrapperProxy.ValidWrapperProxy(wrapperPath, wrapperName);
            if (re1 && re2) return true;
            return false;
        }
    }
}
