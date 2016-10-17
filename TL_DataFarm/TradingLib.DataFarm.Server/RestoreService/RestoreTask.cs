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
            //this.Intraday1MinRealBarStart = DateTime.MaxValue;
            this.EodHistBarEnd = DateTime.MinValue;
            this.IsRestored = false;
            this.CreatedTime = DateTime.Now;
            this.CanRestored = false;

            this.HaveFirst1MinRealBar = false;
            //this.HaveGotTickSnapshot = false;
            this.First1MinRoundtime = DateTime.MinValue;
        }

        /// <summary>
        /// 数据库加载日内数据后 最近的一个Bar结束时间
        /// </summary>
        public DateTime Intraday1MinHistBarEnd { get; set; }

        //TODO 储存Bar逻辑
        /// <summary>
        /// 行情源第一个1分钟Round结束时间
        /// 通过该时刻来作为历史Tick数据加载的结束时间
        /// 无论是开盘还是收盘 主力还是非主力 当时间跨越过1分钟后 则该时刻之前的数据就是完整的,而之后的Bar需要被储存,
        /// 这里原先第一个Bar不储存的方法有漏洞
        /// 如果非主力合约 中间一段时间没有数据 过了1分钟 数据恢复完毕之后 才有Tick数据过来 生成第一个Bar 则该Bar是完整的,可以直接储存
        /// </summary>
        public DateTime First1MinRoundtime { get; set; }


        /// <summary>
        /// 数据库加载日线数据后 最近的一个Bar结束时间
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

        //Tick _snapshot = null;
        ///// <summary>
        ///// 标记该合约当前是否处于交易状态
        ///// </summary>
        //public Tick TickSnapshot 
        //{
        //    get { return _snapshot; } 
        //    set{
        //        if (value == null) return;
        //        _snapshot = value;
        //        this.HaveGotTickSnapshot = true;
        //    }
        //}

        ///// <summary>
        ///// 是否获得行情快照
        ///// </summary>
        //public bool HaveGotTickSnapshot { get; set; }


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
