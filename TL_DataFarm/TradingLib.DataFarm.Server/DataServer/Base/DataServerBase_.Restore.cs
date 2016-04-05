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

                //1.从数据库加载历史数据 获得数据库最后一条Bar更新时间
                DateTime lastBarTime = DateTime.MinValue;
                store.RestoreBar(symbol, BarInterval.CustomTime, 60, out lastBarTime);

                //2.从frequencyService获得该合约第一个Tick时间,通过该事件推算出下2个Bar的截止时间 则为恢复时间
                DateTime firstTickTime = freqService.GetFirstTickTime(symbol);

                //3.加载时间区间内的所有Tick数据重新恢复生成Bar数据
                BackFillSymbol(symbol, lastBarTime, firstTickTime);
            }
        }

        /// <summary>
        /// 将某个合约某个时间段内的Bar数据恢复
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void BackFillSymbol(Symbol symbol,DateTime start,DateTime end)
        {

            //遍历start和end之间所有tickfile进行处理
            long lstart = start.ToTLDateTime();
            long lend = end.ToTLDateTime();

            //获得Tick文件的开始和结束日期
            int tickend = -1;
            int tickstart = -1;
            string path = TikWriter.GetTickPath(symbol);
            if (TikWriter.HaveAnyTickFiles(path, symbol.Symbol))
            {
                tickend = TikWriter.GetEndTickDate(path, symbol.Symbol);
                tickstart = TikWriter.GetStartTickDate(path, symbol.Symbol);
            }


            //如果tickfile 开始时间大于数据库加载Bar对应的最新更新时间 则将开始时间设置为tick文件开始时间
            DateTime current = start;
            if (tickstart > current.ToTLDate())
            {
                current = Util.ToDateTime(tickstart, 0);
            }

            //取tickfile结束时间和end中较小的一个日期为 tick回放结束日期
            int flag = Math.Min(tickend, end.ToTLDate());

            //tick数据缓存
            List<Tick> tmpticklist = new List<Tick>();
            while(current.ToTLDate()<= flag)
            {
                string fn = TikWriter.SafeFilename(path, symbol.Symbol, current.ToTLDate());
                //如果该Tick文件存在
                if (File.Exists(fn))
                {
                     //实例化一个文件流--->与写入文件相关联  
                    using (FileStream fs = new FileStream(fn,FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        //实例化一个StreamWriter-->与fs相关联  
                        using (StreamReader sw = new StreamReader(fs))
                        {
                            while (sw.Peek() > 0)
                            {
                                string str = sw.ReadLine();
                                Tick k = TickImpl.ReadTrade(str);
                                k.Symbol = symbol.Symbol;
                                DateTime ticktime = k.DateTime();
                                //如果Tick时间在开始与结束之间 则需要回放该Tick数据
                                if (ticktime >= start && ticktime <= end)
                                {
                                    tmpticklist.Add(k);
                                }
                            }
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
                current = current.AddDays(1);
            }

            //处理缓存中的Tick数据
            logger.Info("{0} need process {1} Ticks".Put(symbol.Symbol, tmpticklist.Count));
            foreach (var k in tmpticklist)
            {
                freqService.RestoreTick(k);
            }
        }

       
        //string path = GetTickPath(symbol);
        //string fn = TikWriter.SafeFilename(path, k.Symbol, k.Date);

        
    }
}
