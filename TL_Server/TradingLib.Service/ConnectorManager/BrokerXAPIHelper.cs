using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using TradingLib.BrokerXAPI.Interop;

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
        public static bool ValidBrokerInterface(BrokerInterface itface)
        {
            return XAPIHelper.ValidBrokerInterface(itface.libpath_broker, itface.libname_broker, itface.libpath_wrapper, itface.libname_wrapper);
        }


        public static TLBroker CreateBroker(BrokerInterface itface)
        {
            TLBroker broker = new TLBroker(itface.libpath_broker, itface.libname_broker, itface.libpath_wrapper, itface.libname_wrapper);
            return broker;
        }

        /// <summary>
        /// 由BrokerConfig生成服务端连接信息
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static XServerInfoField GenServerInfo(BrokerConfig cfg)
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
        public static XUserInfoField GenUserInfo(BrokerConfig cfg)
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
