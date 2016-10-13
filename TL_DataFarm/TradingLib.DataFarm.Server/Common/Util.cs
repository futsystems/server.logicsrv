using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public class MDUtil
    {
        static ILog logger = LogManager.GetLogger("Util");
        /// <summary>
        /// 从Tick目录加载某个时间段之间的所有Tick数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="symbol"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Tick> LoadTick(string path,Symbol symbol,DateTime startDate, DateTime endDate)
        {
            DateTime current = startDate;
            //tick数据缓存
            List<Tick> tmpticklist = new List<Tick>();
            while (current.ToTLDate() <= endDate.ToTLDate())
            {
                string fn = TikWriter.GetTickFileName(path, symbol.Symbol, current.ToTLDate());
                logger.Info("Tick file:" + fn);
                //如果该Tick文件存在
                if (File.Exists(fn))
                {
                    using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sw = new StreamReader(fs))
                        {
                            while (sw.Peek() > 0)
                            {
                                string str = sw.ReadLine();
                                if (string.IsNullOrEmpty(str))
                                    continue;
                                Tick k = TickImpl.Deserialize2(str);
                                k.Symbol = symbol.Symbol;
                                DateTime ticktime = k.DateTime();
                                if (ticktime >= startDate && ticktime < endDate)
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
            return tmpticklist;
        }
    }
}
