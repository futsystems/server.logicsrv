using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.DataFarm.Common;


namespace TradingLib.DataFarm.Common
{

    public partial class DataServer
    {

        void InitDataBaseService()
        {
            _saverunning = true;
            datathread = new Thread(ProcessBuffer);
            datathread.IsBackground = true;
            datathread.Start();

            LoadTick();
        }
        BinaryDataStore database = new BinaryDataStore();


        void LoadTick()
        {
            IDataAccessor<Tick> store = database.GetTickStorage("GCZ5");
            List<Tick> ticklist = store.Load(DateTime.MinValue, DateTime.MaxValue, -1, true);
            foreach (var k in ticklist)
            {
                //logger.Info("Tick:" + k.ToString());
            }
        }
        #region 保存行情与Bar数据
        RingBuffer<Tick> tickbuffer = new RingBuffer<Tick>(10000);

        bool _saverunning = false;
        Thread datathread = null;
        List<Tick> tmplist = new List<Tick>();
        Dictionary<string, List<Tick>> tmpSymbolTicks = new Dictionary<string, List<Tick>>();
        int _saveThreshold = 100;//当缓存中未保存Tick大于等于该数时执行写库操作
        bool _saveall = false;
        void ProcessBuffer()
        {
            while (_saverunning)
            {
                try
                {
                    //当缓存中tick数量大于设定数值时候执行写库操作 避免频繁写库
                    if (tickbuffer.Count >= _saveThreshold || _saveall)
                    {
                        //读取缓存中的tick按合约分组
                        while (tickbuffer.hasItems)
                        {
                            Tick k = tickbuffer.Read();
                            if (!tmpSymbolTicks.Keys.Contains(k.Symbol))
                            {
                                tmpSymbolTicks.Add(k.Symbol, new List<Tick>());
                            }
                            tmpSymbolTicks[k.Symbol].Add(k);
                        }

                        //遍历所有有Tick数据的分组 调用数据库对象执行写入操作
                        foreach (var p in tmpSymbolTicks.Where(pair => pair.Value.Count > 0))
                        {
                            IDataAccessor<Tick> store = database.GetTickStorage(p.Key);
                            if (store != null)
                            {
                                store.Save(p.Value);
                            }
                            p.Value.Clear();
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    logger.Error("save data error:" + e.ToString());
                }
            }
        }


        void SaveTick(Tick k)
        {
            //logger.Info("save tick...");
            tickbuffer.Write(k);
        }
        #endregion


    }
}
