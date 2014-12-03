using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
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
			bool re2 = TLBrokerWrapperProxy.ValidWrapperProxy(wrapperPath, wrapperName);

            bool re1 = TLBrokerProxy.ValidBrokerProxy(brokerPath, brokerName);
            
            if (re1 && re2) return true;
            return false;
        }

        /// <summary>
        /// 由BrokerConfig生成服务端连接信息
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static XServerInfoField GenServerInfo(ConnectorConfig cfg)
        {
            XServerInfoField srvinfo = new XServerInfoField();
            srvinfo.ServerAddress = cfg.srvinfo_ipaddress;
            srvinfo.ServerPort = cfg.srvinfo_port;
            srvinfo.Field1 = cfg.srvinfo_field1;
            srvinfo.Field2 = cfg.srvinfo_field2;
            srvinfo.Field3 = cfg.srvinfo_field3;
            return srvinfo;
        }

        /// <summary>
        /// 由BrokerConfig生成登入信息
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static XUserInfoField GenUserInfo(ConnectorConfig cfg)
        {
            XUserInfoField usrinfo = new XUserInfoField();
            usrinfo.UserID = cfg.usrinfo_userid;
            usrinfo.Password = cfg.usrinfo_password;
            usrinfo.Field1 = cfg.usrinfo_field1;
            usrinfo.Field2 = cfg.usrinfo_field2;
            return usrinfo;
        }
    }
}
