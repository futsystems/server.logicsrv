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

using STSdb4.WaterfallTree;
using STSdb4.General.Collections;
using STSdb4.General.Comparers;

using STSdb4.Data;
using STSdb4.Database;
using STSdb4.General.Extensions;
using STSdb4.Storage;


namespace TradingLib.DataFarm.Common
{

    public partial class DataServer
    {

        STSLocalDB localdb;
        void InitDataBaseService()
        {
            _saverunning = true;
            datathread = new Thread(ProcessBuffer);
            datathread.IsBackground = true;
            datathread.Start();

            LoadTick();
            LoadBar();

            localdb =  new STSLocalDB("test.stsdb4");
            localdb.Init();
            localdb.RegisterSymbolFreq("HGZ5",BarInterval.CustomTime,30);
        }
        BinaryDataStore database = new BinaryDataStore();


        void LoadBar()
        {
            SymbolFreq sf = new SymbolFreq("HGZ5", new BarFrequency(BarInterval.CustomTime, 30));
            IDataAccessor<Bar> store = database.GetBarStorage(sf);

            QList<Bar> list = new QList<Bar>();
            foreach (var bar in store.Load(DateTime.MinValue, DateTime.MaxValue, -1, true))
            {
                logger.Info("Bar:" + bar.ToString());
                list.Add(bar);
            }
            const string FILE_NAME = "test.stsdb4";
            File.Delete(FILE_NAME);
            using (IStorageEngine engine = STSdb.FromFile(FILE_NAME))
            {
                string name = STSDBBase.GetTableName("HGZ5", BarInterval.CustomTime, 30);
                ITable<long, BarImpl> table = engine.OpenXTable<long, BarImpl>(name);

                //table.Descriptor.KeyComparer = new TLLongComparer();
                //table.Descriptor.KeyEqualityComparer = new TLLongEqualityComparer();

                //table.Descriptor.KeyPersist = new TLLongPersist();
                //table.Descriptor.RecordPersist = new TLBarPersist();

                //table.Descriptor.KeyIndexerPersist = new TLLongIndexerPersist();
                //table.Descriptor.RecordIndexerPersist = new TLBarIndexPersist();


                foreach (var b in list.Items)
                {
                    b.Symbol = "xyz";
                    b.TradeCount = 0;
                    b.TradingDay = 1;
                    
                    table[b.BarStartTime.ToTLDateTime()] = b as BarImpl;
                    logger.Info("Bar:" + b.ToString());
                }
                //Bar b = list.LookBack(3);
                //table.InsertOrIgnore(b.BarStartTime,b as BarImpl);

                //b = list.LookBack(50);
                //table.InsertOrIgnore(b.BarStartTime, b as BarImpl);

                //b = list.LookBack(17);
                //table.InsertOrIgnore(b.BarStartTime, b as BarImpl);

                //b = list.LookBack(44);
                //table.InsertOrIgnore(b.BarStartTime, b as BarImpl);

                //logger.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`");
                //foreach (var x in table)
                //{
                //    logger.Info("Bar:" + x.Value.ToString());
                //}
                //logger.Info("TotalCount:" + table.Count());
                //logger.Info("FristRow:" + table.FirstRow.Value.ToString());
                //logger.Info("LastRow:" + table.LastRow.Value.ToString());

                //DateTime start = new DateTime(2015,11,10,8,31,0);
                //DateTime end = new DateTime(2015,11,10,23,1,0);
                //foreach (var x in table.Forward(start,true,end,false))
                //{
                //    logger.Info("Bar:" + x.Value.ToString());
                //}
                engine.Commit();
            }

            STSLocalDB db = new STSLocalDB("xyz.demo.data");
            db.Init();
            DateTime start = new DateTime(2015,11,10,8,31,0);
            DateTime end = new DateTime(2015,11,10,8,40,0);
            logger.Info("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            foreach (var b in db.QryBar("HGZ5", BarInterval.CustomTime, 30, start,end,0,true))
            {
                logger.Info("got bar:" + b.ToString());
            }
            logger.Info("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            //foreach (var bar in store.Load(DateTime.MinValue,DateTime.MaxValue,5,true))
            //{
            //    logger.Info("Bar:" + bar.ToString());
            //    //list.Add(bar);
            //}
            //logger.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            //logger.Info("lookback[0]:" + list.LookBack(0).ToString());
            //logger.Info("lookback[3]:" + list.LookBack(3).ToString());

            //foreach (var b in list.LoadItem(100,false))
            //{
            //    logger.Info("Bar:" + b.ToString());
            //}

            //SortedQueue<int> queue = new SortedQueue<int>(this.PriorityComparer);
            ////如果插入的对象比前面的小，就会排序到前面，如果插入的对象比前面一个大，则会保持原状
            //queue.Enqueue(20);
            //queue.Enqueue(5);
            //queue.Enqueue(13);

            //queue.Enqueue(1);
            //queue.Enqueue(4);
            //foreach (var i in queue.Items)
            //{
            //    logger.Info("Item:" + i.ToString());
            //}

            


        }

        int PriorityComparer(int  item1, int  item2)
        {
            return item1.CompareTo(item2);
        }

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
        RingBuffer<Bar> barbuffer = new RingBuffer<Bar>(10000);

        bool _saverunning = false;
        Thread datathread = null;
        List<Tick> tmpticklist = new List<Tick>();
        List<Bar> tmpbarlist = new List<Bar>();

        Dictionary<string, List<Tick>> tmpSymbolTicks = new Dictionary<string, List<Tick>>();
        Dictionary<string, Dictionary<BarFrequency, List<Bar>>> tmpSymbolBars = new Dictionary<string, Dictionary<BarFrequency, List<Bar>>>();


        int _saveThreshold = 100;//当缓存中未保存Tick大于等于该数时执行写库操作
        int _barSaveThreshold = 10;

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

                    if (barbuffer.Count >= _barSaveThreshold || _saveall)
                    {
                        while (barbuffer.hasItems)
                        {
                            Bar b = barbuffer.Read();
                            if (!tmpSymbolBars.Keys.Contains(b.Symbol))
                            {
                                tmpSymbolBars.Add(b.Symbol,new Dictionary<BarFrequency,List<Bar>>());
                            }
                            BarFrequency bf = b.GetBarFrequency();

                            if(!tmpSymbolBars[b.Symbol].Keys.Contains(bf))
                            {
                                tmpSymbolBars[b.Symbol].Add(bf,new List<Bar>());
                            }
                            tmpSymbolBars[b.Symbol][bf].Add(b);
                        }

                        foreach (var p in tmpSymbolBars.Where(pair => pair.Value.Count > 0))
                        { 
                            foreach(var freqpair in p.Value)
                            {
                                SymbolFreq sf = new SymbolFreq(p.Key,freqpair.Key);
                                IDataAccessor<Bar> store = database.GetBarStorage(sf);
                                if (store != null)
                                {
                                    store.Save(freqpair.Value);
                                }
                                freqpair.Value.Clear();
                            }
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

        /// <summary>
        /// 储存行情
        /// </summary>
        /// <param name="k"></param>
        public void SaveTick(Tick k)
        {
            //logger.Info("save tick...");
            tickbuffer.Write(k);
        }

        /// <summary>
        /// 储存Bar
        /// </summary>
        /// <param name="bar"></param>
        public void SaveBar(Bar bar)
        {
            barbuffer.Write(bar);
        }
        #endregion


    }
}
