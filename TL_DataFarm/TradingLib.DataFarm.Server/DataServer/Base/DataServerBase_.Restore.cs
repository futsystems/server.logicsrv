using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 恢复数据
        /// </summary>
        protected void RestoreData()
        {
            IHistDataStore store = this.GetHistDataSotre();
            if (store == null)
            {
                logger.Warn("HistDataSotre is null, can not restore data");
            }

            //遍历所有合约执行合约的数据恢复
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                if (symbol.Symbol != "rb1610") continue;
                
                //从数据库加载历史数据 获得数据库最后一条Bar更新时间
                DateTime lastBarTime = DateTime.MinValue;
                store.RestoreBar(symbol, BarInterval.CustomTime, 60, out lastBarTime);

                //从频率发生器获得该合约当前有效Bar时间
                
                logger.Info("Symbol:{0} LastBarTime:{1}".Put(symbol.Symbol, lastBarTime));
            }
        }

        void RestoreSymbol(Symbol symbol)
        { 
            //1.从数据库加载该合约的历史分钟数据 同时获得该合约对应的数据库内的最新的Bar数据时间

            //2.查询frequency中记录的该合约的最早的行情事件 该时间的NextRound就是我们需要恢复到的时间 应为Tick处理后 NextRound的Bar应该就是正确的Bar数据

            //3.从本地文件获得symbol在2个时间间隔内的所有tick数据 并生成Bar数据
        }
        
    }
}
