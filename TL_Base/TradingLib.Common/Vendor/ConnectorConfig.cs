using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 成交接口配置
    /// 设定外部调用dll所在目录和文件名
    /// </summary>
    public class ConnectorInterface
    {
        /// <summary>
        /// 数据库全局编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 接口类型名
        /// </summary>
        public string type_name { get; set; }

        /// <summary>
        /// 是否是XAPI统一接口
        /// XAPI统一接口是统一将成交接口转换成标准C接口然后通过XAPI访问层统一调用访问
        /// </summary>
        public bool IsXAPI { get; set; }
        /// <summary>
        /// wrapper目录
        /// </summary>
        public string libpath_wrapper { get; set; }

        /// <summary>
        /// wrapper名称
        /// </summary>
        public string libname_wrapper { get; set; }

        /// <summary>
        /// 成交接口目录
        /// </summary>
        public string libpath_broker { get; set; }

        /// <summary>
        /// 成交接口地址
        /// </summary>
        public string libname_broker { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 通道类别
        /// </summary>
        public QSEnumConnectorType Type { get; set; }

        /// <summary>
        /// 域ID
        /// </summary>
        public int Domain_ID { get; set; }

        /// <summary>
        /// 实盘帐户对象ID
        /// </summary>
        public int Vendor_ID { get; set; }
    }

    /// <summary>
    /// 成交接口配置信息 
    /// 设定服务器地址 端口 登入用户名 和密码等相关参数
    /// </summary>
    public class ConnectorConfig
    {
        /// <summary>
        /// 数据库编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string srvinfo_ipaddress { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int srvinfo_port { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string srvinfo_field1 { get; set; }

        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string srvinfo_field2 { get; set; }

        /// <summary>
        /// 扩展字段3
        /// </summary>
        public string srvinfo_field3 { get; set; }

        /// <summary>
        /// 登入名
        /// </summary>
        public string usrinfo_userid { get; set; }

        /// <summary>
        /// 登入密码
        /// </summary>
        public string usrinfo_password { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string usrinfo_field1 { get; set; }

        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string usrinfo_field2 { get; set; }

        /// <summary>
        /// 接口定义编号
        /// </summary>
        public int interface_fk { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 接口
        /// </summary>
        public ConnectorInterface Interface { get; set; }


        /// <summary>
        /// 对应的实盘帐户全局ID
        /// </summary>
        public int vendor_id { get; set; }
        
    }
}
