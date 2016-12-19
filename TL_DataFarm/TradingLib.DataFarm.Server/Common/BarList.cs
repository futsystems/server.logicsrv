using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public class BarList
    {
        ILog logger = LogManager.GetLogger("BarList");

        /// <summary>
        /// 按Bar时间排列的Bar列表
        /// </summary>
        SortedList<long, BarImpl> barlist = new SortedList<long, BarImpl>();


        /// <summary>
        /// 日级别以上关联BarList
        /// 在恢复EOD日线数据时 将其余日级别以上周期的BarList数据关联到该日级别数据
        /// 日级别以上的数据统一用EOD数据进行当前周期OHLC数据的更新
        /// </summary>
        Dictionary<string, BarList> eodList = new Dictionary<string, BarList>();


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        public BarList(string key)
        {
            this._key = key;
        }

        bool _iseod = false;

        /// <summary>
        /// 该BarList是否是EODBar数据集
        /// </summary>
        public bool IsEOD { get { return _iseod; } set { _iseod = value; ; } }

        /// <summary>
        /// 返回最后一个Bar时间
        /// </summary>
        public DateTime LastBarTime
        {
            get
            {
                if (barlist.Count == 0) return DateTime.MinValue;
                return barlist.Last().Value.EndTime;
            }
        }

        /// <summary>
        /// 返回最后一个Bar的交易日
        /// </summary>
        public int LastBarTradingDay
        {
            get
            {
                if (barlist.Count == 0) return int.MinValue;
                return barlist.Last().Value.TradingDay;
            }
        }

        string _key = string.Empty;
        /// <summary>
        /// BarList的键
        /// </summary>
        public string Key
        {
            get { return _key; }
        }


        

        object _object = new object();
        /// <summary>
        /// 更新Bar
        /// BarList中不包含对应结束时间的Bar则执行插入,否则更新
        /// </summary>
        /// <param name="bar"></param>
        public void Update(BarImpl bar, out bool isInsert)
        {
            lock (_object)
            {
                long key = bar.GetTimeKey();
                isInsert = !barlist.Keys.Contains(key);
                if (isInsert)
                {
                    barlist[key] = bar;
                }
                else
                {
                    barlist[key].CopyData(bar);
                }
            }
        }

        /// <summary>
        /// 删除一组数据库ID对应的Bar
        /// </summary>
        /// <param name="ids"></param>
        public void Delete(int[] ids)
        {
            lock (_object)
            {
                List<long> keylist = barlist.Where(s => ids.Contains(s.Value.ID)).Select(s => s.Key).ToList();

                foreach (var key in keylist)
                {
                    barlist.Remove(key);
                }
            }
        }

        /// <summary>
        /// 加载一组Bar数据
        /// 启动时从数据库加载Bar数据并恢复到内存BarList中
        /// </summary>
        /// <param name="source"></param>
        public void RestoreBars(IEnumerable<BarImpl> source,bool eodrestore=false)
        {
            lock (_object)
            {
                foreach (var b in source)
                {
                    barlist[b.GetTimeKey()] = b;
                }
                
                //以周线为例 记录恢复数据最后一个Bar的成交量,该成交量为合并到上一个交易日位置所累加的成家量,当前交易日的周线成交量 = 该成交量 + 当前EODBar数据中的成交量(日线成交量)
                if (eodrestore && source.Count() > 0)
                {
                    _totalVolToLastDay = source.Last().Volume;
                }
            }
        }


        /// <summary>
        /// 关联日级别以上数据
        /// </summary>
        /// <param name="barlist"></param>
        public void AppendEODList(BarList barlist)
        {
            eodList[barlist.Key]= barlist;
        }


        /// <summary>
        /// 通过Bar结束时间来获取Bar对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public BarImpl this[long key]
        {
            get
            {
                BarImpl target = null;
                if (barlist.TryGetValue(key, out target))
                {
                    return target;
                }
                return null;
            }
        }



        bool _hasPartial = false;
        public bool HasPartialBar
        {
            get { return _hasPartial; }
        }

        public void ClearPartialBar()
        {
            _hasPartial = false;
        }


        BarImpl _realPartialBar = null;
        /// <summary>
        /// 实时Bar系统生成的PartialBar
        /// </summary>
        public BarImpl RealPartialBar
        {
            get
            {
                if (_hasPartial)
                    return _realPartialBar;
                return null;
            }
            set
            {
                _hasPartial = true;
                _realPartialBar = value;
                if (this.IsEOD)
                {
                    //更新日级别以上数据
                    foreach (var tmp in eodList.Values)
                    {
                        tmp.UpdateEODPartialBar(RealPartialBar);
                    }
                }

            }
        }

        /// <summary>
        /// 周期内上一个交易日成家量数据
        /// </summary>
        int _totalVolToLastDay = 0;

        /// <summary>
        /// 日级别以上数据 在某个交易日内始终为数据集中最后一个
        /// </summary>
        /// <param name="partialBar"></param>
        public void UpdateEODPartialBar(BarImpl partialBar)
        {
            if(partialBar == null) return;
            BarImpl tmp = barlist.Values.LastOrDefault();
            if (tmp != null)
            {
                tmp.High = Math.Max(partialBar.High, tmp.High);
                tmp.Low = Math.Min(partialBar.Low, tmp.Low);
                tmp.Close = partialBar.Close;
                tmp.Volume = _totalVolToLastDay + partialBar.Volume;//当前周期成交量 = 上一个交易日成家量 + 当前交易日成交量
            }
        }


        BarImpl _histPartialBar = null;
        /// <summary>
        /// 启动时加载历史Tick恢复Bar数据完毕后获得的PartialBar
        /// 历史Tick数据恢复只能确保从最近一个1小时Bar开始,数据结尾不能确保完整
        /// 如果在收盘后执行数据恢复则恢复的Bar数据均完整,且没有PartialBar,如果从某个Bar中间时刻开始恢复则恢复完毕后会存在PartialBar
        /// </summary>
        public BarImpl HistPartialBar
        {
            get { return _histPartialBar; }
            set 
            { 
                _histPartialBar = value;
                MergePartialBar();
            }
        }

        BarImpl _firstRealBar = null;
        /// <summary>
        /// 实时Bar系统生成的第一个Bar数据
        /// </summary>
        public BarImpl FirstRealBar
        {
            get { return _firstRealBar; }
            set 
            { 
                _firstRealBar = value;
                MergePartialBar();
            }
        }

        /// <summary>
        /// 合并HistPartialBar与FirstRealBar
        /// 当第一个Bar生成完毕时 也就是第一个RealPartialBar关闭时 此时需要用合并后的数据去更新数据集,注：第一个Bar都没有执行数据集的更新
        /// </summary>
        void MergePartialBar()
        {
            //当FirstRealBar和HistPartialBar都生成完毕 则可执行数据合并
            if (this.FirstRealBar != null && this.HistPartialBar != null)
            {
                //HistPartialBar时间在FirstRealBar之后 则表明FristRealBar对应周期的Bar数据已经在历史Bar恢复中完成 且FirstRealBar之后所有的Bar数据都是完毕的
                if (this.HistPartialBar.GetTimeKey() > this.FirstRealBar.GetTimeKey())
                {
                    //数据完毕不用做任何操作
                    logger.Info(string.Format("BarList[{0}] HistPartialBar'Time > FirstRealBar'Time /Data Complete", this.Key));
                }
                //HistPartialBar时间小于FirstRealBar,表明加载的历史数据无法与实时数据重叠 有数据缺失
                if (this.HistPartialBar.GetTimeKey() < this.FirstRealBar.GetTimeKey())
                {
                    logger.Warn(string.Format("BarList[{0}] HistPartialBar'Time < FirstRealBar'Time /Data Miss", this.Key));
                }

                if (this.HistPartialBar.GetTimeKey() == this.FirstRealBar.GetTimeKey())
                {
                    BarImpl tmp = MergeBar(this.HistPartialBar, this.FirstRealBar);
                    bool insert = false;
                    this.Update(tmp, out insert);
                    logger.Info(string.Format("BarList[{0}] HistPartialBar'Time = FirstRealBar'Time /Data Merge", this.Key));
                }
            }
        }

        /// <summary>
        /// A在时间前段 B在时间后段
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        BarImpl MergeBar(BarImpl a, BarImpl b)
        {
            try
            {
                BarImpl tmp = new BarImpl(a.Symbol, new BarFrequency(a.IntervalType, a.Interval), a.EndTime);
                tmp.Open = a.Open;
                tmp.High = Math.Max(a.High, b.High);
                tmp.Low = Math.Min(a.Low, a.Low);
                tmp.Close = b.Close;
                tmp.OpenInterest = b.OpenInterest;
                tmp.Volume = b.LastTick.Vol - a.FirstTick.Vol;//用tick数据相减 可以获得准确的成交量信息，否则Hist Real相互叠加 无法准确获得成交量数据
                tmp.FirstTick = a.FirstTick;
                tmp.LastTick = b.LastTick;

                return tmp;
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("BarA:{0} BarB:{1} BLasttick:{2} AFirstTick:{3}", a == null ? "Null" : a.ToString(), b == null ? "Null" : b.ToString(), b.LastTick == null ? "Null" : b.LastTick.Vol.ToString(), a.FirstTick == null ? "Null" : a.FirstTick.Vol.ToString()));
                logger.Error("Merge Bar Error:" + ex.ToString());

                return b;
            }
        }

        

        /// <summary>
        /// 获得PartialBar
        /// HistPartailBar---Bar---RealPartialBar
        /// </summary>
        /// <returns></returns>
        BarImpl GetPartialBar()
        { 
            BarImpl realPartial = this.RealPartialBar;
            /*
             *  实时PartialBar存在 
             *  在1分钟周期上的第一个Bar上是不完备的,第一个Bar之后的PartialBar是完备的
             *  在其他日内周期上的PartialBar也有可能不完备 需要与历史Tick回补完毕后的最后一个PartialBar执行合并
             * 
             * */
            if (realPartial != null)//实时PartialBar存在
            {
                if (this.HistPartialBar != null)
                {
                    //实时Partial时间在历史Partial之后 则表明已经越过了恢复时刻的那个周期，直接返回RealPartial
                    if (realPartial.GetTimeKey() > this.HistPartialBar.GetTimeKey()) return realPartial;
                    //实时Partial与历史Partail在一个周期，则历史Partial在周期的前半部分，RealPartial在周期的后半部分 需要进行2个PartialBar的合并
                    if (realPartial.GetTimeKey() == this.HistPartialBar.GetTimeKey())
                    {
                        return MergeBar(this.HistPartialBar, realPartial);
                    }
                    //实时Partial在历史Partial之前 数据处理异常逻辑
                    if (realPartial.GetTimeKey() < this.HistPartialBar.GetTimeKey())
                    {
                        logger.Error("logic error:real partial time < hist partil time");
                        return null;
                    }
                    return null;
                }
                else //HistPartialBar不存在 则返回partial
                {
                    return realPartial;
                }
            }
            else//实时PartialBar不存在 则表明没有实时Tick驱动生成Partial 直接返回历史PartialBar
            {
                return this.HistPartialBar;
            }
        }


        #region 数据查询
        /// <summary>
        /// 通过交易日查询Bar数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxcount"></param>
        /// <param name="havePartial"></param>
        /// <returns></returns>
        public List<BarImpl> QryBar(int start, int end, int startIndex, int maxcount, bool havePartial)
        {
            lock (_object)
            {
                IEnumerable<BarImpl> records = barlist.Values;
                if (havePartial)
                {
                    BarImpl partial = GetPartialBar();
                    if (partial != null)
                    {
                        /*
                         *  当从1分钟数据合并生成3，5，15，30等其他周期的数据时，由于1分钟数据的不完整可能导致合并后的其他周期的最后一个Bar数据不完整
                         *  处理方法
                         *  1.判断完整性 将不完整的Bar剔除
                         *  2.保留该Bar,该Bar的Open数据是正确的，将实时系统生成的PartialBar数据与该Bar执行逻辑合并
                         * 
                         * */
                        //合并PartialBar时需要检查 数据集中最后一个数据与PartialBar的时间 如果一致 则更新数据集中的数据即可
                        if (records.Count() > 0 && partial.GetTimeKey() == records.Last().GetTimeKey())
                        {
                            records.Last().CopyData(partial);
                        }
                        else
                        {
                            //当前recoreds数量为0 或 最后一个Bar数据时间不一致 则直接将PartialBar放到records中
                            records = records.Concat(new BarImpl[] { partial });
                        }
                    }
                }

                if (start != int.MinValue || end != int.MaxValue)
                {
                    //交易日过滤
                    records = records.Where(bar => bar.TradingDay >= start && bar.TradingDay <= end);
                }

                if (maxcount <= 0)
                {
                    records = records.Take(Math.Max(0, records.Count() - startIndex));
                }
                else 
                {
                    records = records.Take(Math.Max(0, records.Count() - startIndex)).Skip(Math.Max(0, (records.Count() - startIndex) - maxcount));//返回序列后段元素
                }

                return records.ToList();
            }



        }
        /// <summary>
        /// 通过BarTime查询Bar数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxcount"></param>
        /// <param name="fromEnd">是否先返回最新的数据</param>
        /// <returns></returns>
        public List<BarImpl> QryBar(DateTime start, DateTime end, int startIndex, int maxcount, bool havePartail)
        {
            lock (_object)
            {
                //整理控制返回Bar数量
                maxcount = Math.Min(maxcount, ConstantData.MAXBARCNT);

                IEnumerable<BarImpl> records = barlist.Values ;
                if (havePartail)
                {
                    BarImpl partial = GetPartialBar();
                    if (partial != null)
                    {
                        /*
                         *  当从1分钟数据合并生成3，5，15，30等其他周期的数据时，由于1分钟数据的不完整可能导致合并后的其他周期的最后一个Bar数据不完整
                         *  处理方法
                         *  1.判断完整性 将不完整的Bar剔除
                         *  2.保留该Bar,该Bar的Open数据是正确的，将实时系统生成的PartialBar数据与该Bar执行逻辑合并
                         * 
                         * */
                        //合并PartialBar时需要检查 数据集中最后一个数据与PartialBar的时间 如果一致 则更新数据集中的数据即可
                        if (records.Count() > 0 && partial.GetTimeKey() == records.Last().GetTimeKey())
                        {
                            records.Last().CopyData(partial);
                        }
                        else
                        {
                            records = records.Concat(new BarImpl[] { partial });
                        }
                    }
                }


                if (start != DateTime.MinValue || end != DateTime.MaxValue)
                {
                    long lstart = long.MinValue;
                    long lend = long.MaxValue;
                    if (start != DateTime.MinValue) lstart = start.ToTLDateTime();
                    if (end != DateTime.MaxValue) lend = end.ToTLDateTime();
                    //执行时间过滤
                    records = records.Where(bar => bar.EndTime >= start && bar.EndTime <= end);//barlist.Where(v => v.Key >= lstart && v.Key <= lend).Select(v => v.Value);
                }

                if (maxcount <= 0)
                {
                    //不限制最大返回数量 
                    records = records.Take(Math.Max(0, records.Count() - startIndex));
                }
                else //设定最大数量 返回数据要求 按时间先后排列
                {
                    //startIndex 首先从数据序列开头截取对应数量的数据
                    //maxcount 然后从数据序列末尾截取最大数量的数据
                    records = records.Take(Math.Max(0, records.Count() - startIndex)).Skip(Math.Max(0, (records.Count() - startIndex) - maxcount));//返回序列后段元素
                }
                return records.ToList();

            }
        }

        #endregion

    }

}
