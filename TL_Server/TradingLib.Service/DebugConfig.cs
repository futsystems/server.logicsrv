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
    /// </summary>
    public class DebugConfig
    {
        public static DebugConfig DevDebugConfig
        {
            get
            {
                DebugConfig d = new DebugConfig(false);
                d.D_TrdMessage = true;
                d.DL_TrdMessage = QSEnumDebugLevel.DEBUG;

                d.D_TrdLogic = true;
                d.DL_TrdLogic = QSEnumDebugLevel.DEBUG;

                d.D_MgrMessage = true;
                d.DL_MgrMessage = QSEnumDebugLevel.DEBUG;

                d.D_MgrLogic = true;
                d.DL_MgrLogic = QSEnumDebugLevel.DEBUG;

                d.D_ClearCentre = true;
                d.DL_ClearCentre = QSEnumDebugLevel.DEBUG;

                d.D_RiskCentre = true;
                d.DL_RiskCentre = QSEnumDebugLevel.DEBUG;

                d.D_DataFeedRouter = true;
                d.DL_DataFeedRouter = QSEnumDebugLevel.DEBUG;

                d.D_BrokerRouter = true;
                d.DL_BrokerRouter = QSEnumDebugLevel.DEBUG;

                d.D_TrdLoger = true;
                d.DL_TrdLoger = QSEnumDebugLevel.DEBUG;
                return d;
            }
        }
       
        public DebugConfig(bool verbose)
        {

            D_TrdMessage = verbose ? false : true;
            DL_TrdMessage = verbose ? QSEnumDebugLevel.ERROR : QSEnumDebugLevel.DEBUG;

            D_TrdLogic = verbose ? true : true;
            DL_TrdLogic = verbose ? QSEnumDebugLevel.INFO : QSEnumDebugLevel.DEBUG;

            D_MgrMessage = verbose ? false : true;
            DL_MgrMessage = verbose ? QSEnumDebugLevel.ERROR : QSEnumDebugLevel.DEBUG;

            D_MgrLogic = verbose ? true : true;
            DL_MgrLogic = verbose ? QSEnumDebugLevel.INFO : QSEnumDebugLevel.DEBUG;

            D_ClearCentre = verbose ? true : true;
            DL_ClearCentre = verbose ? QSEnumDebugLevel.INFO : QSEnumDebugLevel.DEBUG;

            D_RiskCentre = verbose ? true : true;
            DL_RiskCentre = verbose ? QSEnumDebugLevel.INFO : QSEnumDebugLevel.DEBUG;

            D_DataFeedRouter = verbose ? false : true;
            DL_DataFeedRouter = verbose ? QSEnumDebugLevel.ERROR : QSEnumDebugLevel.DEBUG;

            D_BrokerRouter = verbose ? false : true;
            DL_BrokerRouter = verbose ? QSEnumDebugLevel.ERROR : QSEnumDebugLevel.DEBUG;

            D_TrdLoger = verbose ? false : true;
            DL_TrdLoger = verbose ? QSEnumDebugLevel.ERROR : QSEnumDebugLevel.DEBUG;

        }

        public event VoidDelegate ApplyDebugConfigEvent;
        public void ApplyDebugConfig()
        {
            if (ApplyDebugConfigEvent != null)
                ApplyDebugConfigEvent();
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
