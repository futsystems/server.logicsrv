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

namespace TradingLib.DataFarm.Common
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

        bool _saverunning = false;
        Thread _datathread = null;
        //List<Tick> tmpticklist = new List<Tick>();
        //List<Bar> tmpbarlist = new List<Bar>();

        //Dictionary<string, List<Tick>> tmpSymbolTicks = new Dictionary<string, List<Tick>>();
        //Dictionary<string, Dictionary<BarFrequency, List<Bar>>> tmpSymbolBars = new Dictionary<string, Dictionary<BarFrequency, List<Bar>>>();


        int _saveThreshold = 100;//当缓存中未保存Tick大于等于该数时执行写库操作
        int _barSaveThreshold = 10;

        bool _saveall = false;

        const int SLEEPDEFAULTMS = 10;
        static ManualResetEvent _logwaiting = new ManualResetEvent(false);

        void NewData()
        {
            if ((_datathread != null) && (_datathread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _logwaiting.Set();
            }
        }
        DateTime _lastcommit = DateTime.Now;
        TimeSpan _commitpriod = TimeSpan.FromMinutes(1);
        void ProcessBuffer()
        {
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
                    while (barbuffer.hasItems)
                    {
                        BarStoreStruct b = barbuffer.Read();
                        IHistDataStore store = GetHistDataSotre();
                        if (store != null)
                        {
                            store.InsertBar(b.Symbol,b.Bar);
                            //store.Commit();
                        }
                    }

                    DateTime now = DateTime.Now;
                    if (now - _lastcommit > _commitpriod)
                    { 
                        IHistDataStore store = GetHistDataSotre();
                        if (store != null)
                        {
                            logger.Info("Commit:" + now.ToShortTimeString());
                            store.Commit();
                        }
                        _lastcommit = now;
                    }

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
        public void SaveBar(Symbol symbol,BarImpl bar)
        {
            barbuffer.Write(new BarStoreStruct(symbol, bar));
            NewData();
        }

        /// <summary>
        /// 更新Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public void UpdateBar(Bar bar)
        { 
        
        }
        #endregion
    }
}
