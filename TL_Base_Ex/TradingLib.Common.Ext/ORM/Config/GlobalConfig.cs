using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class GlobalConfig
    {
        static GlobalConfig defaultinstance = null;
        ConfigDB config = null;

        static GlobalConfig()
        {
            defaultinstance = new GlobalConfig();
        }

        private GlobalConfig()
        {
            config = new ConfigDB("Global");
            if (!config.HaveConfig("DEVELOP"))
            {
                config.UpdateConfig("DEVELOP", QSEnumCfgType.Bool,true, "是否运行在开发模式");
            }

            if (!config.HaveConfig("DefaultPassword"))
            {
                config.UpdateConfig("DefaultPassword",QSEnumCfgType.String,"123456","默认帐户密码");
            }

            if (!config.HaveConfig("VendorName"))
            {
                config.UpdateConfig("VendorName", QSEnumCfgType.String, "乐透", "系统商标,用于显示软件品牌");
            }

            if (!config.HaveConfig("DealerPrompt"))
            {
                config.UpdateConfig("DealerPrompt", QSEnumCfgType.String, "", "交易员帐户登入提示");
            }

            if (!config.HaveConfig("SimPrompt"))
            {
                config.UpdateConfig("SimPrompt", QSEnumCfgType.String, "", "模拟帐户登入提示");
            }

            if (!config.HaveConfig("RealPrompt"))
            {
                config.UpdateConfig("RealPrompt", QSEnumCfgType.String, "", "实盘帐户登入提示");
            }


            if (!config.HaveConfig("DefaultBroker"))
            {
                config.UpdateConfig("DefaultBroker", QSEnumCfgType.String, "申银万国期货有限公司", "默认期货公司名称");
            }

            if (!config.HaveConfig("DefaultBankID"))
            {
                config.UpdateConfig("DefaultBankID", QSEnumCfgType.String, "1", "默认银行名称");
            }

            if (!config.HaveConfig("DefaultBankAC"))
            {
                config.UpdateConfig("DefaultBankAC", QSEnumCfgType.String, "95993939899002123", "默认银行卡号");
            }

        }

        /// <summary>
        /// 默认期货公司
        /// </summary>
        public static string DefaultBroker
        {
            get
            {
                return defaultinstance.config["DefaultBroker"].AsString();
            }
        }


        /// <summary>
        /// 默认银行
        /// </summary>
        public static string DefaultBankID
        {
            get
            {
                return defaultinstance.config["DefaultBankID"].AsString();
            }
        }

        /// <summary>
        /// 默认银行帐号
        /// </summary>
        public static string DefaultBankAC
        {
            get
            {
                return defaultinstance.config["DefaultBankAC"].AsString();
            }
        }
        /// <summary>
        /// 是否处于开发模式
        /// </summary>
        public static bool IsDevelop
        {
            get
            {
                return defaultinstance.config["DEVELOP"].AsBool();
            }
        }

        /// <summary>
        /// 默认交易帐户密码
        /// 创建交易帐号时，如果没有设定密码则使用默认密码
        /// </summary>
        public static string DefaultPassword
        {
            get
            {
                return defaultinstance.config["DefaultPassword"].AsString();
            }
        }

        /// <summary>
        /// 平台名称
        /// </summary>
        public static string VendorName
        {
            get
            {
                return defaultinstance.config["VendorName"].AsString();
            }
        }

        /// <summary>
        /// 交易员登入提示
        /// </summary>
        public static string DealerPrompt
        {
            get
            {
                return defaultinstance.config["DealerPrompt"].AsString();
            }
        }

        /// <summary>
        /// 模拟交易帐号登入提示
        /// </summary>
        public static string SimPrompt
        {
            get
            {
                return defaultinstance.config["SimPrompt"].AsString();
            }
        }

        /// <summary>
        /// 实盘帐户登入提示
        /// </summary>
        public static string RealPrompt
        {
            get
            {
                return defaultinstance.config["RealPrompt"].AsString();
            }
        }
    }
}
