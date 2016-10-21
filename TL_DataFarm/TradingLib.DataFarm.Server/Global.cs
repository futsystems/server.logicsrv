using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common.DataFarm
{
    public class Global
    {
        static Global defaulatinstance = null;
        static Global()
        {
            defaulatinstance = new Global();
        }

        private Global()
        {

        }

        TickTracker ticktracker = null;

        /// <summary>
        /// 合约快照维护器
        /// </summary>
        public static TickTracker TickTracker
        {
            get
            {
                if (defaulatinstance.ticktracker == null)
                {
                    defaulatinstance.ticktracker = new TickTracker();
                }
                return defaulatinstance.ticktracker;
            }
        }
        TaskService taskservice = null;
        /// <summary>
        /// 全局任务服务
        /// </summary>
        public static TaskService TaskService
        {

            get
            {
                if (defaulatinstance.taskservice == null)
                {
                    defaulatinstance.taskservice = new TaskService();
                }
                return defaulatinstance.taskservice;
            }
        
        }


        TimeZoneHelper timezonehelper = null;

        public static TimeZoneHelper TimeZoneHelper
        {
            get
            {
                if (defaulatinstance.timezonehelper == null)
                {
                    defaulatinstance.timezonehelper = new TimeZoneHelper();
                }
                return defaulatinstance.timezonehelper;
            }
        }


    }
}
