﻿  using System;
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
    internal enum EnumDBOperationType
    {
        /// <summary>
        /// 插入
        /// </summary>
        Insert,
        /// <summary>
        /// 更新
        /// </summary>
        Update,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
    }

    internal class BarDBOperation
    {
        public BarDBOperation(BarImpl bar, EnumDBOperationType type)
        {
            this.Bar = bar;
            this.OperationType = type;
        }

        public BarDBOperation(BarInterval type, int interval, int[] ids, EnumDBOperationType optype)
        {
            this.DataCycle = new BarFrequency(type, interval);
            this.OperationType = optype;
            this.IDs = ids;
        }
        public BarFrequency DataCycle { get; set; }

        public int[] IDs { get; set; }
        public BarImpl Bar { get; set; }

        public EnumDBOperationType OperationType { get; set; }
    }

    
    public partial class DataServerBase
    {

        /// <summary>
        /// 启动数据储存服务
        /// </summary>
        protected void StartDataStoreService()
        {
            if (_syncdb)
            {
                if (_saverunning) return;
                _saverunning = true;
                _datathread = new Thread(ProcessBuffer);
                _datathread.IsBackground = false;
                _datathread.Start();
            }
            else
            {
                logger.Info("Do not Sync DataBase, no need to start store service");
            }
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
        RingBuffer<BarDBOperation> dbopbuffer = new RingBuffer<BarDBOperation>(200000);//数据库操作缓存

        bool _saverunning = false;
        Thread _datathread = null;

        const int SLEEPDEFAULTMS = 10000;
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
        void DBInsertBar(BarImpl b,bool eod)
        {
            try
            {
                if (eod)
                {
                    MBar.InsertEodBar(b);
                }
                else
                {
                    MBar.InsertIntradayBar(b);
                }
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
        void DBUpdateBar(BarImpl b, bool eod)
        {
            try
            {
                if (eod)
                {
                    MBar.UpdateEodBar(b);
                }
                else
                {
                    MBar.UpdateIntradayBar(b);
                }
            }
            catch (Exception ex)
            {
                logger.Error("UpdateBar error:" + ex.ToString());
            }
        }
        /// <summary>
        /// 数据库删除Bar记录
        /// </summary>
        /// <param name="id"></param>
        void DBDeleteBar(int id, bool eod)
        {
            try
            {
                if (eod)
                {
                    MBar.DeleteEodBar(id);
                }
                else
                {
                    MBar.DeleteIntradayBar(id);
                }
            }
            catch (Exception ex)
            {
                logger.Error("DeleteBar error:" + ex.ToString());
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
                    while (dbopbuffer.hasItems)
                    {
                        BarDBOperation op = dbopbuffer.Read();
                        switch (op.OperationType)
                        {
                            case EnumDBOperationType.Insert:
                                {
                                    DBInsertBar(op.Bar,op.Bar.IntervalType == BarInterval.Day);
                                    break;
                                }
                            case EnumDBOperationType.Update:
                                {
                                    DBUpdateBar(op.Bar,op.Bar.IntervalType == BarInterval.Day);
                                    break;
                                }
                            case EnumDBOperationType.Delete:
                                {
                                    foreach(var id in op.IDs)
                                    {
                                        DBDeleteBar(id,op.DataCycle.Type == BarInterval.Day);
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
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
        /// 更新Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public void UpdateBar2(Symbol symbol,BarImpl bar)
        {
            bool isInsert = false;
            BarImpl dest = null;

            GetHistDataSotre().UpdateBar(symbol, bar, out dest, out isInsert);

            //只处理1分钟与日级别Bar数据
            if (_syncdb)
            {
                if ((dest.IntervalType == BarInterval.CustomTime && dest.Interval == 60) || (dest.IntervalType == BarInterval.Day && dest.Interval == 1))
                {
                    if (isInsert)
                    {
                        dbopbuffer.Write(new BarDBOperation(dest, EnumDBOperationType.Insert));
                    }
                    else
                    {
                        dbopbuffer.Write(new BarDBOperation(dest, EnumDBOperationType.Update));
                    }
                }
                NewData();
            }
        }

        /// <summary>
        /// 更新一组Bar数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bars"></param>
        public void UploadBars(string key, IEnumerable<BarImpl> bars)
        {
            foreach (var bar in bars)
            {
                bool isInsert = false;
                BarImpl dest = null;

                GetHistDataSotre().UpdateBar(key, bar, out dest, out isInsert);
                if (_syncdb)
                {
                    if ((dest.IntervalType == BarInterval.CustomTime && dest.Interval == 60) || (dest.IntervalType == BarInterval.Day && dest.Interval == 1))
                    {
                        if (isInsert)
                        {
                            dbopbuffer.Write(new BarDBOperation(dest, EnumDBOperationType.Insert));
                        }
                        else
                        {
                            dbopbuffer.Write(new BarDBOperation(dest, EnumDBOperationType.Update));
                        }
                    }
                    NewData();
                }
            }
        }

        public void DeleteBar(Symbol symbol, BarInterval intervalType, int interval, int[] ids)
        {
            GetHistDataSotre().DeleteBar(symbol, intervalType, interval, ids);
            NewData();
        }

        /// <summary>
        /// 更新PartialBar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="partialbar"></param>
         void UpdateRealPartialBar(Symbol symbol, BarImpl partialbar)
        {
            GetHistDataSotre().UpdateRealPartialBar(symbol, partialbar);
        }

        void UpdateHistPartialBar(Symbol symbol, BarImpl partialbar)
        {
            GetHistDataSotre().UpdateHistPartialBar(symbol, partialbar);
        }

        void UpdateFirstRealBar(Symbol symbol,BarImpl partialbar)
        {
            GetHistDataSotre().UpdateFirstRealBar(symbol,partialbar);
        }
       
        #endregion
    }
}
