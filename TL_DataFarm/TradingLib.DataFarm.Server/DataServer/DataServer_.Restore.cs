using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// 行情源时间
    /// 用于记录本次启动后
    /// 行情源时间信息
    /// </summary>
    public class DataFeedTime
    {
        public DataFeedTime(QSEnumDataFeedTypes type)
        {
            this.DataFeed = type;
            this.StartTime = DateTime.MinValue;
            this.StartTime = DateTime.MinValue;
        }
        /// <summary>
        /// 行情源类别
        /// </summary>
        public QSEnumDataFeedTypes DataFeed { get; set; }

        /// <summary>
        /// 是否已经覆盖1分钟
        /// 覆盖掉1分钟后 就可以取开始后的第一个一分钟时刻 进行数据恢复
        /// </summary>
        public bool Cover1Minute
        {
            get { return this.CurrentTime.Subtract(this.StartTime).TotalMinutes > 1; }
        }

        /// <summary>
        /// 第一个一分钟Round
        /// </summary>
        public DateTime First1MinRoundEnd
        {
            get
            {
                DateTime next = this.StartTime.AddMinutes(1);
                return new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, 0);
            }
        }
        /// <summary>
        /// 启动后收到的第一个行情源事件
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 当前最新时间
        /// </summary>
        public DateTime CurrentTime { get; set; }

    }


    public partial class DataServer
    {

        RestoreService restoresrv = null;

        /// <summary>
        /// 初始化数据恢复服务
        /// </summary>
        protected void InitRestoreService()
        {
            string path = this.ConfigFile["TickPath"].AsString();
            logger.Info("[Init Restore Service] TickPath:"+path);

            restoresrv = new RestoreService(path,dfTimeMap,eodservice);

        }

        /// <summary>
        /// 启动数据恢复服务
        /// </summary>
        protected void StartRestoreService()
        {
            logger.Info("[Start Restore Service]");
            restoresrv.NewHistBarEvent += new Action<FreqNewBarEventArgs>(OnNewHistBarEvent);
            restoresrv.NewHistPartialBarEvent += new Action<Symbol, BarImpl>(restoresrv_NewHistPartialBarEvent);
            restoresrv.Start();

        }

        void restoresrv_NewHistPartialBarEvent(Symbol arg1, BarImpl arg2)
        {
            UpdateHistPartialBar(arg1, arg2);
        }


        /// <summary>
        /// 回放tick所生成的Bar数据事件
        /// </summary>
        /// <param name="obj"></param>
        void OnNewHistBarEvent(FreqNewBarEventArgs obj)
        {
            obj.Bar.Symbol = obj.Symbol.GetContinuousSymbol();
            this.UpdateBar(obj.Symbol, obj.Bar);
        }


        //void RestoreServiceProcessTickSnapshot(Symbol symbol, Tick k)
        //{
        //    restoresrv.OnTickSnapshot(symbol, k);
        //}

        Profiler restoreProfile = new Profiler();

        /// <summary>
        /// 恢复数据
        /// 恢复数据过程中
        /// </summary>
        protected void LoadData()
        {
            logger.Info("[Load Bar Data]");
            IHistDataStore store = this.GetHistDataSotre();
            if (store == null)
            {
                logger.Warn("HistDataSotre is null, can not restore data");
            }

            pf.EnterSection("DB Load");
            //遍历所有合约执行合约的数据恢复 合理：采用品种 并遍历1-12进行数据恢复 恢复某个品种的1-12个月数据
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                //if (store.IsRestored(symbol.Exchange, symbol.GetBarSymbol())) continue; 
                //if (symbol.Symbol != "CLX6") continue;
                
                restoreProfile.EnterSection("RestoreBar");
                //1.从数据库加载历史数据 获得数据库最后一条Bar更新时间
                DateTime intradayHistBarEndTime = DateTime.MinValue;
                store.RestoreIntradayBar(symbol, out intradayHistBarEndTime);
                restoresrv.OnIntraday1MinHistBarLoaded(symbol, intradayHistBarEndTime);

                //2.从数据库加载日线数据 获得最后一条日线更新时间
                int eodHistBarEndTradingDay = int.MinValue;
                store.RestoreEodBar(symbol, out eodHistBarEndTradingDay);
                restoresrv.OnEodHistBarLoaded(symbol, eodHistBarEndTradingDay);

                restoreProfile.LeaveSection();
            }
            pf.LeaveSection();
            logger.Info(pf.GetStatsString());
        
        }
    }
}
