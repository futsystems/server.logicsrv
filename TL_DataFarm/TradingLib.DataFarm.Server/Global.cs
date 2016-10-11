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



    }
}
