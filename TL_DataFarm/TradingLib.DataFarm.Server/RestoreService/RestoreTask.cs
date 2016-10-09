using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// 数据恢复任务
    /// 系统启动过程中用于将合约对应Bar数据恢复到当前最新状态
    /// </summary>
    internal class RestoreTask
    {

        public RestoreTask(Symbol symbol)
        {
            this.Symbol = symbol;
            this.IntradayHistBarEnd = DateTime.MinValue;
            this.IntradayRealBarStart = DateTime.MaxValue;
            this.IsRestored = false;
            this.CreatedTime = DateTime.Now;
            this.CanRestored = false;
            //this.TickSnapshot = null;
        }

        /// <summary>
        /// 数据库加载历史Bar数据最新的一个Bar周期
        /// 在该Bar周期之后的所有Tick数据需要被加载
        /// </summary>
        public DateTime IntradayHistBarEnd { get; set; }

        /// <summary>
        /// 实时Bar生成系统生成的第一个Bar不保存,这里的结束时间记录的是第二个Bar的结束时间
        /// </summary>
        public DateTime IntradayRealBarStart { get; set; }


        Tick _snapshot = null;
        /// <summary>
        /// 标记该合约当前是否处于交易状态
        /// </summary>
        public Tick TickSnapshot 
        {
            get { return _snapshot; } 
            set{
                
                _snapshot = value;
                HaveGotTickSnapshot = true;
            }
        }

        /// <summary>
        /// 是否获得行情快照
        /// </summary>
        public bool HaveGotTickSnapshot { get; set; }


        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 是否可以执行Tick数据恢复
        /// </summary>
        public bool CanRestored { get; set; }
        /// <summary>
        /// 数据恢复标识
        /// </summary>
        public bool IsRestored { get; set; }
    }


}
