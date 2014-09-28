using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.ServiceManager
{
    /// <summary>
    /// 组件日志配置
    /// 用于设定系统内组件日志输出标志和日志输出级别
    /// </summary>
    public class DebugConfig
    {
        ConfigDB _cfgdb = null;
        QSEnumDebugLevel GetDebugLevel(string level)
        {
            return (QSEnumDebugLevel)Enum.Parse(typeof(QSEnumDebugLevel), level);
        }

        public DebugConfig()
        {

            _cfgdb = new ConfigDB("DebugConfig");
            if (!_cfgdb.HaveConfig("MsgExchTransDebugEnable"))
            {
                _cfgdb.UpdateConfig("MsgExchTransDebugEnable", QSEnumCfgType.Bool, true, "交易消息传输日志输出标志");
            }
            if (!_cfgdb.HaveConfig("MsgExchTransDebugLevel"))
            {
                _cfgdb.UpdateConfig("MsgExchTransDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.ERROR.ToString(), "交易消息传输日志输出级别");
            }
            D_TrdMessage = _cfgdb["MsgExchTransDebugEnable"].AsBool();
            DL_TrdMessage = GetDebugLevel(_cfgdb["MsgExchTransDebugLevel"].AsString());

            if (!_cfgdb.HaveConfig("MsgExchDebugEnable"))
            {
                _cfgdb.UpdateConfig("MsgExchDebugEnable", QSEnumCfgType.Bool, true, "交易逻辑服务日志输出标志");
            }
            if (!_cfgdb.HaveConfig("MsgExchDebugLevel"))
            {
                _cfgdb.UpdateConfig("MsgExchDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.INFO.ToString(), "交易逻辑服务日志输出级别");
            }
            D_TrdLogic = _cfgdb["MsgExchDebugEnable"].AsBool();
            DL_TrdLogic = GetDebugLevel(_cfgdb["MsgExchDebugLevel"].AsString());


            if (!_cfgdb.HaveConfig("MgrExchTransDebugEnable"))
            {
                _cfgdb.UpdateConfig("MgrExchTransDebugEnable", QSEnumCfgType.Bool, true, "管理消息传输日志输出标志");
            }
            if (!_cfgdb.HaveConfig("MgrExchTransDebugLevel"))
            {
                _cfgdb.UpdateConfig("MgrExchTransDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.ERROR.ToString(), "管理消息传输日志输出级别");
            }
            D_MgrMessage = _cfgdb["MgrExchTransDebugEnable"].AsBool();
            DL_MgrMessage = GetDebugLevel(_cfgdb["MgrExchTransDebugLevel"].AsString());

            if (!_cfgdb.HaveConfig("MsgExchDebugEnable"))
            {
                _cfgdb.UpdateConfig("MsgExchDebugEnable", QSEnumCfgType.Bool, true, "管理逻辑服务日志输出标志");
            }
            if (!_cfgdb.HaveConfig("MsgExchDebugLevel"))
            {
                _cfgdb.UpdateConfig("MsgExchDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.INFO.ToString(), "管理逻辑服务日志输出级别");
            }
            D_MgrLogic = _cfgdb["MsgExchDebugEnable"].AsBool();
            DL_MgrLogic = GetDebugLevel(_cfgdb["MsgExchDebugLevel"].AsString());


            if (!_cfgdb.HaveConfig("ClearCentreDebugEnable"))
            {
                _cfgdb.UpdateConfig("ClearCentreDebugEnable", QSEnumCfgType.Bool, true, "清算中心服务日志输出标志");
            }
            if (!_cfgdb.HaveConfig("ClearCentreDebugLevel"))
            {
                _cfgdb.UpdateConfig("ClearCentreDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.INFO.ToString(), "清算中心服务日志输出级别");
            }
            D_ClearCentre = _cfgdb["ClearCentreDebugEnable"].AsBool();
            DL_ClearCentre = GetDebugLevel(_cfgdb["ClearCentreDebugLevel"].AsString());


            if (!_cfgdb.HaveConfig("RiskCenreDebugEnable"))
            {
                _cfgdb.UpdateConfig("RiskCenreDebugEnable", QSEnumCfgType.Bool, true, "风控中心服务日志输出标志");
            }
            if (!_cfgdb.HaveConfig("RiskCentreDebugLevel"))
            {
                _cfgdb.UpdateConfig("RiskCentreDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.INFO.ToString(), "风控中心服务日志输出级别");
            }
            D_RiskCentre = _cfgdb["RiskCenreDebugEnable"].AsBool();
            DL_RiskCentre = GetDebugLevel(_cfgdb["RiskCentreDebugLevel"].AsString());


            if (!_cfgdb.HaveConfig("DataRouterDebugEnable"))
            {
                _cfgdb.UpdateConfig("DataRouterDebugEnable", QSEnumCfgType.Bool, true, "行情路由服务日志输出标志");
            }
            if (!_cfgdb.HaveConfig("DataRouterDebugLevel"))
            {
                _cfgdb.UpdateConfig("DataRouterDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.INFO.ToString(), "行情路由服务日志输出级别");
            }
            D_DataFeedRouter = _cfgdb["DataRouterDebugEnable"].AsBool();
            DL_DataFeedRouter = GetDebugLevel(_cfgdb["DataRouterDebugLevel"].AsString());

            if (!_cfgdb.HaveConfig("BrokerRouterDebugEnable"))
            {
                _cfgdb.UpdateConfig("BrokerRouterDebugEnable", QSEnumCfgType.Bool, true, "成交路由服务日志输出标志");
            }
            if (!_cfgdb.HaveConfig("BrokerRouterDebugLevel"))
            {
                _cfgdb.UpdateConfig("BrokerRouterDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.INFO.ToString(), "成交路由服务日志输出级别");
            }
            D_BrokerRouter = _cfgdb["BrokerRouterDebugEnable"].AsBool();
            DL_BrokerRouter = GetDebugLevel(_cfgdb["BrokerRouterDebugLevel"].AsString());


            if (!_cfgdb.HaveConfig("InfoLogDebugEnable"))
            {
                _cfgdb.UpdateConfig("InfoLogDebugEnable", QSEnumCfgType.Bool, true, "信息记录服务日志输出标志");
            }
            if (!_cfgdb.HaveConfig("InfoLogDebugLevel"))
            {
                _cfgdb.UpdateConfig("InfoLogDebugLevel", QSEnumCfgType.String, QSEnumDebugLevel.INFO.ToString(), "信息记录服务日志输出级别");
            }
            D_TrdLoger = _cfgdb["InfoLogDebugEnable"].AsBool();
            DL_TrdLoger = GetDebugLevel(_cfgdb["InfoLogDebugLevel"].AsString());

        }

        
        public bool D_TrdMessage {get;set;}
        /// <summary>
        /// 交易信息日志级别
        /// </summary>
        public QSEnumDebugLevel DL_TrdMessage { get;set;}

        public bool D_TrdLogic { get; set; } 
        /// <summary>
        /// 交易业务日志级别
        /// </summary>
        public QSEnumDebugLevel DL_TrdLogic { get; set; }


        public bool D_MgrMessage { get; set; } 
        /// <summary>
        /// 管理消息日志级别
        /// </summary>
        public QSEnumDebugLevel DL_MgrMessage { get; set; }


        public bool D_MgrLogic { get; set; } 
        /// <summary>
        /// 管理业务日志级别
        /// </summary>
        public QSEnumDebugLevel DL_MgrLogic { get; set; }


        public bool D_ClearCentre { get; set; } 
        /// <summary>
        /// 清算中心日志级别
        /// </summary>
        public QSEnumDebugLevel DL_ClearCentre { get; set; }

        public bool D_RiskCentre { get; set; } 
        /// <summary>
        /// 风控中心日志级别
        /// </summary>
        public QSEnumDebugLevel DL_RiskCentre { get; set; }

        public bool D_DataFeedRouter { get; set; } 
        /// <summary>
        /// 数据路由日志级别
        /// </summary>
        public QSEnumDebugLevel DL_DataFeedRouter { get; set; }

        public bool D_BrokerRouter { get; set; } 
        /// <summary>
        /// 交易路由日志级别
        /// </summary>
        public QSEnumDebugLevel DL_BrokerRouter { get; set; }

        public bool D_TrdLoger { get; set; } 
        /// <summary>
        /// 交易记录日志级别
        /// </summary>
        public QSEnumDebugLevel DL_TrdLoger { get; set; } 
    }
}
