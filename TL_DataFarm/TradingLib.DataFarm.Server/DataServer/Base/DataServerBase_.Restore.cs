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

    

    public partial class DataServerBase
    {

        RestoreService restoresrv = null;

        /// <summary>
        /// 初始化数据恢复服务
        /// </summary>
        protected void InitRestoreService()
        {
            string path = this.ConfigFile["TickPath"].AsString();
            logger.Info("[Init Restore Service] TickPath:"+path);

            restoresrv = new RestoreService(path);

        }

        /// <summary>
        /// 启动数据恢复服务
        /// </summary>
        protected void StartRestoreService()
        {
            logger.Info("[Start Restore Service]");
            restoresrv.NewHistBarEvent += new Action<FreqNewBarEventArgs>(OnNewHistBarEvent);
            restoresrv.NewHistPartialBarEvent += new Action<Symbol, BarImpl>(restoresrv_NewHistPartialBarEvent);
            restoresrv.RestoreTaskCompleteEvent += new Action<RestoreTask>(restoresrv_RestoreTaskCompleteEvent);
            restoresrv.Start();

        }

        void restoresrv_RestoreTaskCompleteEvent(RestoreTask obj)
        {
            eodservice.On1MinBarRestored(obj);
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
            this.UpdateBar2(obj.Symbol, obj.Bar);
        }


        void RestoreServiceProcessTickSnapshot(Symbol symbol, Tick k)
        {
            restoresrv.OnTickSnapshot(symbol, k);
        }

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

            //遍历所有合约执行合约的数据恢复
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                //if (symbol.Symbol != "CLX6") continue;

                restoreProfile.EnterSection("RestoreBar");
                //1.从数据库加载历史数据 获得数据库最后一条Bar更新时间
                DateTime intradayHistBarEndTime = DateTime.MinValue;
                store.RestoreIntradayBar(symbol, BarInterval.CustomTime, 60, out intradayHistBarEndTime);
                restoresrv.OnIntraday1MinHistBarLoaded(symbol, intradayHistBarEndTime);

                //2.从数据库加载日线数据 获得最后一条日线更新时间
                DateTime eodHistBarEndTime = DateTime.MinValue;
                store.RestoreEodBar(symbol, out eodHistBarEndTime);
                restoresrv.OnEodHistBarLoaded(symbol, eodHistBarEndTime);

                restoreProfile.LeaveSection();
            }

        
        }
    }
}
