  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    internal class BarStoreStruct
    {
        public BarStoreStruct(Symbol symbol, BarImpl bar)
        {
            this.Symbol = symbol;
            this.Bar = bar;
        }

        public Symbol Symbol { get; set; }

        public BarImpl Bar { get; set; }
    }

    internal class BarUploadStruct
    {
        public BarUploadStruct(string key, IEnumerable<BarImpl> bars)
        {
            this.Key = key;
            this.Bars = bars;
        }

        public string Key { get; set; }

        public IEnumerable<BarImpl> Bars { get; set; }
    }

    internal class BarDeleteStruct
    {

        public BarDeleteStruct(Symbol symbol, BarInterval intervaltype, int interval, int[] ids)
        {
            this.Symbol = symbol;
            this.IntervalType = intervaltype;
            this.Interval = interval;
            this.IDs = ids;
        }

        public Symbol Symbol { get; set; }

        public BarInterval IntervalType { get; set; }

        public int Interval { get; set;}

        public int[] IDs { get; set; }
    }
    public partial class DataServerBase
    {

        /// <summary>
        /// 启动数据储存服务
        /// </summary>
        protected void StartDataStoreService()
        {
            if (_saverunning) return;
            _saverunning = true;
            _datathread = new Thread(ProcessBuffer);
            _datathread.IsBackground = true;
            _datathread.Start();
        }

        /// <summary>
        /// 停止数据储存服务
        /// </summary>
        protected void StopDataStoreService()
        {
            if (!_saverunning) return;
            _saverunning = false;
            
        }

        #region 保存行情与Bar数据
        RingBuffer<Tick> tickbuffer = new RingBuffer<Tick>(10000);
        RingBuffer<BarStoreStruct> barbuffer = new RingBuffer<BarStoreStruct>(10000);
        RingBuffer<BarDeleteStruct> deleteBarBuffer = new RingBuffer<BarDeleteStruct>(1000);
        RingBuffer<BarUploadStruct> uploadBarBuffer = new RingBuffer<BarUploadStruct>(10000);
        RingBuffer<BarStoreStruct> partialBarBuffer = new RingBuffer<BarStoreStruct>(10000);

        RingBuffer<BarStoreStruct> barupdatebuffre = new RingBuffer<BarStoreStruct>(10000);

        bool _saverunning = false;
        Thread _datathread = null;
        //List<Tick> tmpticklist = new List<Tick>();
        //List<Bar> tmpbarlist = new List<Bar>();

        //Dictionary<string, List<Tick>> tmpSymbolTicks = new Dictionary<string, List<Tick>>();
        //Dictionary<string, Dictionary<BarFrequency, List<Bar>>> tmpSymbolBars = new Dictionary<string, Dictionary<BarFrequency, List<Bar>>>();


        int _saveThreshold = 100;//当缓存中未保存Tick大于等于该数时执行写库操作
        int _barSaveThreshold = 10;

        bool _batchSave = true;

        const int SLEEPDEFAULTMS = 100;
        static ManualResetEvent _logwaiting = new ManualResetEvent(false);

        void NewData()
        {
            if ((_datathread != null) && (_datathread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _logwaiting.Set();
            }
        }
        /// <summary>
        /// 数据库插入Bar记录
        /// </summary>
        /// <param name="b"></param>
        void DBInsertBar(BarImpl b)
        {
            try
            {
                MBar.InsertBar(b);
            }
            catch (Exception ex)
            {
                logger.Error("InsertBar error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 数据库更新Bar记录
        /// </summary>
        /// <param name="b"></param>
        void DBUpdateBar(BarImpl b)
        {
            try
            {
                MBar.UpdateBar(b);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateBar error:" + ex.ToString());
            }
        }

        DateTime _lastcommit = DateTime.Now;
        TimeSpan _commitpriod = TimeSpan.FromMinutes(1);
        void ProcessBuffer()
        {
            IHistDataStore store = GetHistDataSotre();
            if (store == null)
            {
                logger.Error("HistDataStore is null");
                return;
            }

            while (_saverunning)
            {
                try
                {

                    //实时插入Tick数据
                    while (tickbuffer.hasItems)
                    {
                        Tick k = tickbuffer.Read();
                            
                    }

                    //实时插入Bar数据
                    while (barbuffer.hasItems && _tickRestored)
                    {
                        BarStoreStruct b = barbuffer.Read();
                        store.UpdateBar(b.Symbol,b.Bar);
                        
                    }
                    while (!barbuffer.hasItems && deleteBarBuffer.hasItems)
                    {
                        BarDeleteStruct b = deleteBarBuffer.Read();
                        store.DeleteBar(b.Symbol, b.IntervalType, b.Interval,b.IDs);
                    }

                    while (!barbuffer.hasItems && uploadBarBuffer.hasItems)
                    {
                        BarUploadStruct b = uploadBarBuffer.Read();
                        store.UploadBar(b.Key, b.Bars);
                    }

                    while (!barbuffer.hasItems && partialBarBuffer.hasItems)
                    {
                        BarStoreStruct b = partialBarBuffer.Read();
                        store.UpdatePartialBar(b.Symbol, b.Bar);
                    }

                    //更新Bar缓存 用于从Tick历史数据加载生成Bar进行数据更新
                    while (barupdatebuffre.hasItems)
                    {
                        BarStoreStruct b = barupdatebuffre.Read();
                        store.UpdateBar(b.Symbol, b.Bar);
                    }

                    //if (_batchSave)
                    //{
                    //    DateTime now = DateTime.Now;
                    //    if (now - _lastcommit > _commitpriod)
                    //    {
                    //        IHistDataStore store = GetHistDataSotre();
                    //        if (store != null)
                    //        {
                    //            logger.Info("Commit:" + now.ToShortTimeString());
                    //            store.Commit();
                    //        }
                    //        _lastcommit = now;
                    //    }
                    //}
                    // clear current flag signal
                    _logwaiting.Reset();

                    // wait for a new signal to continue reading
                    _logwaiting.WaitOne(SLEEPDEFAULTMS);
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
            tickbuffer.Write(k);
            NewData();
        }

        /// <summary>
        /// 保存Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public void UpdateBar2(Symbol symbol,BarImpl bar)
        {
            barbuffer.Write(new BarStoreStruct(symbol, bar));
            NewData();
        }

        public void UploadBars(string key, IEnumerable<BarImpl> bars)
        {
            uploadBarBuffer.Write(new BarUploadStruct(key, bars));
            NewData();
        }

        public void DeleteBar(Symbol symbol, BarInterval intervalType, int interval, int[] ids)
        {
            deleteBarBuffer.Write(new BarDeleteStruct(symbol, intervalType, interval, ids));
            NewData();
        }

        /// <summary>
        /// 更新PartialBar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="partialbar"></param>
        public void UpdatePartialBar(Symbol symbol, BarImpl partialbar)
        {
            partialBarBuffer.Write(new BarStoreStruct(symbol, partialbar));
            NewData();
        }
        /// <summary>
        /// 更新Bar数据
        /// 从历史Tick数据恢复的Bar需要调用UpdateBar接口
        /// </summary>
        /// <param name="bar"></param>
        public void UpdateBar(Symbol symbol, BarImpl bar)
        {
            barupdatebuffre.Write(new BarStoreStruct(symbol, bar));
            NewData();
        }
        #endregion
    }
}
