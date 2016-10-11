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
    public class RestoreTask
    {

        public RestoreTask(Symbol symbol)
        {
            this.Symbol = symbol;
            this.Intraday1MinHistBarEnd = DateTime.MinValue;
            this.Intraday1MinRealBarStart = DateTime.MaxValue;
            this.EodHistBarEnd = DateTime.MinValue;
            this.IsRestored = false;
            this.CreatedTime = DateTime.Now;
            this.CanRestored = false;

            this.HaveFirst1MinRealBar = false;
            this.HaveGotTickSnapshot = false;
        }

        /// <summary>
        /// 数据库加载历史Bar数据最新的一个Bar周期
        /// 在该Bar周期之后的所有Tick数据需要被加载
        /// </summary>
        public DateTime Intraday1MinHistBarEnd { get; set; }

        /// <summary>
        /// 实时Bar生成系统生成的第一个Bar不保存,这里的结束时间记录的是第二个Bar的结束时间
        /// </summary>
        public DateTime Intraday1MinRealBarStart { get; set; }


        /// <summary>
        /// 数据库加载日线数据后 最近的一个Bar时间
        /// </summary>
        public DateTime EodHistBarEnd { get; set; }




        public bool HaveFirst1MinRealBar { get; set; }




        BarImpl _first1minbar = null;
        /// <summary>
        /// 实时Bar系统生成的第一个Bar数据
        /// </summary>
        public BarImpl Intraday1MinFirstRealBar 
        {
            get { return _first1minbar; }
            set {
                if (value == null) return;
                _first1minbar = value;
                this.HaveFirst1MinRealBar = true;
            }
        }

        Tick _snapshot = null;
        /// <summary>
        /// 标记该合约当前是否处于交易状态
        /// </summary>
        public Tick TickSnapshot 
        {
            get { return _snapshot; } 
            set{
                if (value == null) return;
                _snapshot = value;
                this.HaveGotTickSnapshot = true;
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
