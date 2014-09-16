using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.Contrib.FinService
{
    /// <summary>
    /// 配资服务
    /// 
    /// </summary>
    public class FinServiceStub
    {
        /// <summary>
        /// 交易帐户ID
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 服务计划外键
        /// </summary>
        public int serviceplan_fk { get; set; }


        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public long ModifiedTime { get; set; }

         
        

    }
}
