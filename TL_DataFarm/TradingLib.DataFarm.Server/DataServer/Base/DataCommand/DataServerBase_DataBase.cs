using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;
using TradingLib.MarketData;

namespace TradingLib.Common.DataFarm
{
    public partial class DataServerBase
    {
        [DataCommandAttr("QryHistTable", "QryHistTable - Qry HistBar Table Info", "查询历史数据表信息")]
        public void Command_QryTableInfo(IServiceHost host, IConnection conn)
        {
            logger.Info("Conn:{0} QryHistTable Infomation".Put(conn.SessionID));
            //foreach (var t in GetHistDataSotre().HistTableInfo)
            //{
            //    if(t.Name.StartsWith("CFFEX"))
            //    logger.Info(t.ToString());
            //}
            
        }

        [DataCommandAttr("LoadTickFile", "LoadTickFile - Load Tick From Files", "加载Tick数据")]
        public void Command_QryTickFile(IServiceHost host, IConnection conn)
        {
            List<Tick> tmpticklist = new List<Tick>();
            logger.Info("Conn:{0} LoadTick File".Put(conn.SessionID));
            MDSymbol _symbol = new MDSymbol();
            _symbol.Exchange = "NYME";
            _symbol.Symbol = "CLX6";
            _symbol.SecCode = "CL";
            string path = MDTradeTickWriter.GetTickPath(_symbol.Exchange, _symbol.SecCode);
            string fn = MDTradeTickWriter.SafeFilename(path,_symbol.Symbol,20160923);//TikWriter.SafeFilename(path, symbol.Symbol, current.ToTLDate());
            logger.Info("File Path: " + fn);
            //如果该Tick文件存在
            if (File.Exists(fn))
            {
                //实例化一个文件流--->与写入文件相关联  
                using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    //实例化一个StreamWriter-->与fs相关联  
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        while (sw.Peek() > 0)
                        {
                            string str = sw.ReadLine();
                            Tick k = TickImpl.ReadTrade(str);
                            if (k == null) continue;
                            k.Symbol = _symbol.Symbol;
                            DateTime ticktime = k.DateTime();
                            //如果Tick时间在开始与结束之间 则需要回放该Tick数据 需要确保在盘中重启后 在start和end之间的所有数据均加载完毕
                            //if (ticktime >= start && ticktime <= end)
                            {
                                tmpticklist.Add(k);
                            }
                        }
                        sw.Close();
                    }
                    fs.Close();
                }
            }

            logger.Info(string.Format("Read {0} Tick from files", tmpticklist.Count));

            //处理缓存中的Tick数据
            logger.Info("{0} need process {1} Ticks".Put(_symbol.Symbol, tmpticklist.Count));
            Tick tmp = tmpticklist.Last();
            foreach (var k in tmpticklist)
            {
                freqService.RestoreTick(k);
            }
            logger.Info("finished");
        }

    }
}
