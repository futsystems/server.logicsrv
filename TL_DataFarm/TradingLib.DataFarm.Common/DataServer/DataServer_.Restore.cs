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

namespace TradingLib.DataFarm.Common
{
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
            restoresrv.NewHistBarEvent += new Action<FreqNewBarEventArgs>(OnNewHistBarEvent);
            restoresrv.NewHistPartialBarEvent += new Action<Symbol, BarImpl>(OnNewHistPartialBarEvent);
        }

        /// <summary>
        /// 启动数据恢复服务
        /// </summary>
        protected void StartRestoreService()
        {
            logger.Info("[Start Restore Service]");
            //加载历史数据
            LoadData();
            //启动后台线程执行数据恢复
            restoresrv.Start();
        }

        /// <summary>
        /// 历史Tick恢复完毕后 如果有PartialBar则记录PartialBar 用于与FirstRealBar进行数据合并
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void OnNewHistPartialBarEvent(Symbol arg1, BarImpl arg2)
        {
            UpdateHistPartialBar(arg1, arg2);
        }


        /// <summary>
        /// 历史Tick恢复过程中 生成Bar 直接更新数据集
        /// </summary>
        /// <param name="obj"></param>
        void OnNewHistBarEvent(FreqNewBarEventArgs obj)
        {
            obj.Bar.Symbol = obj.Symbol.GetContinuousSymbol();
            this.UpdateBar(obj.Symbol, obj.Bar);
        }



        /// <summary>
        /// 恢复数据
        /// 恢复数据过程中
        /// </summary>
        void LoadData()
        {
            logger.Info("Load bar data from cache");
            IHistDataStore store = this.GetHistDataSotre();
            if (store == null)
            {
                logger.Warn("HistDataSotre is null, can not restore data");
            }

            Global.Profile.EnterSection("Bar Restore");
            //遍历所有合约执行合约的数据恢复 合理：采用品种 并遍历1-12进行数据恢复 恢复某个品种的1-12个月数据
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                //if (store.IsRestored(symbol.Exchange, symbol.GetBarSymbol())) continue; 
                //if (symbol.Symbol != "CLX6") continue;

                //1.从数据库加载历史数据 获得数据库最后一条Bar更新时间
                DateTime intradayHistBarEndTime = DateTime.MinValue;
                store.RestoreIntradayBar(symbol, out intradayHistBarEndTime);
                restoresrv.OnIntraday1MinHistBarLoaded(symbol, intradayHistBarEndTime);

                //2.从数据库加载日线数据 获得最后一条日线更新时间
                int eodHistBarEndTradingDay = int.MinValue;
                store.RestoreEodBar(symbol, out eodHistBarEndTradingDay);
                restoresrv.OnEodHistBarLoaded(symbol, eodHistBarEndTradingDay);
            }
            Global.Profile.LeaveSection();
            logger.Info(Global.Profile.GetStatsString());
        
        }
    }
}
