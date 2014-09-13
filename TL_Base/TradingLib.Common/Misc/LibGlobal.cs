using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace TradingLib.Common
{
    public class LibGlobal
    {
        /// <summary>
        /// 全局日志输出前缀
        /// </summary>
        public static string GlobalPrefix = ">>> ";

        #region 配置文件全局设置
        /// <summary>
        /// 程序加载时初始化lib中所用到的地址信息
        /// </summary>
        public static void InitPath()
        {
            string programPath = System.Environment.CurrentDirectory;
            string volume = programPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(':'));
            CONFIGPATH = volume + ":\\QSConfiguration\\Account\\";
            LOGPATH = volume + ":\\QSLOG\\";
            TICKDIR = volume + ":\\TickFiles";

            if (!Directory.Exists(CONFIGPATH))
                Directory.CreateDirectory(CONFIGPATH);

            if (!Directory.Exists(LOGPATH))
                Directory.CreateDirectory(LOGPATH);

        }
        public static string CONFIGPATH = "D:\\QSConfiguration\\Account\\";
        public static string LOGPATH = "D:\\QSLOG\\";
        public static string TICKDIR = "D:\\TickFiles";
        #endregion


        #region 路由全局设置

        public static void InitSimBrokerConfig(bool usebidask,bool filltickbytick)
        {
            SimBrokerUseBikAsk = usebidask;
            FillOrderTickByTick = filltickbytick;
        }

        public static void InitCTPConfig(int offset)
        {
            CTPMarketOrderPriceOffset = offset;
        }


        /// <summary>
        /// CTP接口 用限价单模拟市价单的超价跳数
        /// </summary>
        public static int CTPMarketOrderPriceOffset = 10;

        /// <summary>
        /// 模拟成交用盘口数据成交还是用最新价成交
        /// </summary>
        public static bool SimBrokerUseBikAsk = true;

        /// <summary>
        /// 挂单成交是否使用 最新价成交 用于区分模拟与实盘
        /// </summary>
        public static bool SimLimitReal = false;

        /// <summary>
        /// 模拟成交用Tick数据逐个成交还是用市场切片快速成交
        /// </summary>
        public static bool FillOrderTickByTick = false;

        #endregion







    }
}
