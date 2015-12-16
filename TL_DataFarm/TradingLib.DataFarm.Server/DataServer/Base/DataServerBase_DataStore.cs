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
    public partial class DataServerBase
    {

        /// <summary>
        /// 启动数据储存服务
        /// </summary>
        void StartDataStoreService()
        {
            if (_saverunning) return;
            _saverunning = true;
            datathread = new Thread(ProcessBuffer);
            datathread.IsBackground = true;
            datathread.Start();
        }

        /// <summary>
        /// 停止数据储存服务
        /// </summary>
        void StopDataStoreService()
        {
            if (!_saverunning) return;
            _saverunning = false;
            
        }

        #region 保存行情与Bar数据
        RingBuffer<Tick> tickbuffer = new RingBuffer<Tick>(10000);
        RingBuffer<Bar> barbuffer = new RingBuffer<Bar>(10000);

        bool _saverunning = false;
        Thread datathread = null;
        //List<Tick> tmpticklist = new List<Tick>();
        //List<Bar> tmpbarlist = new List<Bar>();

        //Dictionary<string, List<Tick>> tmpSymbolTicks = new Dictionary<string, List<Tick>>();
        //Dictionary<string, Dictionary<BarFrequency, List<Bar>>> tmpSymbolBars = new Dictionary<string, Dictionary<BarFrequency, List<Bar>>>();


        int _saveThreshold = 100;//当缓存中未保存Tick大于等于该数时执行写库操作
        int _barSaveThreshold = 10;

        bool _saveall = false;
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
                        Bar b = barbuffer.Read();
                        IHistDataStore store = GetHistDataSotre();
                        if (store != null)
                        { 
                            BarImpl bar = b as BarImpl;
                            if(bar != null)
                            {
                                store.InsertBar(bar);
                            }
                        }
                    }

                    Thread.Sleep(100);
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
        }

        /// <summary>
        /// 保存Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public void SaveBar(Bar bar)
        {
            barbuffer.Write(bar);
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
