using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.Common.DataFarm
{

    public partial class EodDataService
    {





        public void RestoreTickBakcground()
        {
            //在后台工作线程执行恢复操作
            (new Action(this.RestoreTask)).BeginInvoke((re) => { logger.Info("RegisterSymbol Complate"); }, null);
        }

        void RestoreTask()
        {
            logger.Info("Restore Tick Files into Cache");
            //遍历所有合约 查找到当前对应的MarketDay 然后加载对应开盘与收盘时间段的分笔成交数据
            //该操作可以再启动后就放入后台线程执行,应为启动后 Tick数据就再接受会进入tmplist,当恢复Tick完毕后会自行合并到TradeList具体参考TradeCache的相关代码
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                this.RestoreTick(symbol);
            }
        }


        void RestoreTick(Symbol symbol)
        {
            //查找对应品种当前MarketDay 通过开盘 与 收盘时间区间加载分笔成交数据
            MarketDay md = GetCurrentMarketDay(symbol.SecurityFamily);
            //查找对应的tradeMap并恢复Tick数据
            TradeCache cache = null;
            if (tradeMap.TryGetValue(symbol.UniqueKey, out cache))
            {
                List<Tick> tmplist = MDUtil.LoadTick(_tickpath, symbol, md.MarketOpen, md.MarketClose);
                cache.RestoreTrade(tmplist);
            }
        }
    }
}
