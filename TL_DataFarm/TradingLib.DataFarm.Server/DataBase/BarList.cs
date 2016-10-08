using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    public class BarList
    {
        /// <summary>
        /// 合约
        /// </summary>
        //public Symbol Symbol { get; set; }

        /// <summary>
        /// 间隔类别
        /// </summary>
        //public BarInterval IntervalType { get; set; }

        /// <summary>
        /// 间隔
        /// </summary>
        //public int Interval { get; set; }

        /// <summary>
        /// 是否处于工作模式
        /// </summary>
        public bool Working { get; set; }


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

        public BarList(string key)
        {

            //this.Symbol = symbol;
            //this.IntervalType = type;
            //this.Interval = interval;
            //this._key = "{0}-{1}-{2}-{3}".Put(symbol.Exchange, symbol.Symbol, type, interval);
            this._key = key;
            this.Working = false;
        }

        string _key = string.Empty;
        /// <summary>
        /// BarList的键
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// 按Bar时间排列的Bar列表
        /// </summary>
        SortedList<long, BarImpl> barlist = new SortedList<long, BarImpl>();

        object _object = new object();
        /// <summary>
        /// 更新Bar
        /// </summary>
        /// <param name="bar"></param>
        public void Update(BarImpl bar, out bool isInsert)
        {
            lock (_object)
            {
                long key = bar.EndTime.ToTLDateTime();
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

        public BarImpl this[long datetime]
        {
            get
            {
                BarImpl target = null;
                if (barlist.TryGetValue(datetime, out target))
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

        BarImpl _partialBar = null;
        public BarImpl PartialBar
        {
            get
            {
                if (_hasPartial)
                    return _partialBar;
                return null;
            }
            set
            {
                _hasPartial = true;
                _partialBar = value;
            }
        }

        /// <summary>
        /// 添加一组Bar数据
        /// 从数据库加载一组Bar并添加到内存中
        /// </summary>
        /// <param name="source"></param>
        public void RestoreBars(IEnumerable<BarImpl> source)
        {
            lock (_object)
            {
                foreach (var b in source)
                {
                    barlist[b.EndTime.ToTLDateTime()] = b;
                }
            }
        }


        /// <summary>
        /// 从数据集中查询结果
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxcount"></param>
        /// <param name="fromEnd">是否先返回最新的数据</param>
        /// <returns></returns>
        public IEnumerable<BarImpl> QryBar(DateTime start, DateTime end, int startIndex, int maxcount, bool fromEnd, bool havePartail)
        {
            lock (_object)
            {
                IEnumerable<BarImpl> records = null;
                if (start != DateTime.MinValue || end != DateTime.MaxValue)
                {
                    long lstart = long.MinValue;
                    long lend = long.MaxValue;
                    if (start != DateTime.MinValue) lstart = start.ToTLDateTime();
                    if (end != DateTime.MaxValue) lend = end.ToTLDateTime();
                    //执行时间过滤
                    records = barlist.Where(v => v.Key >= lstart && v.Key <= lend).Select(v => v.Value);
                }
                else
                {
                    records = barlist.Select(v => v.Value);//不执行时间检查
                }

                //合并PartialBar
                if (havePartail)
                {
                    BarImpl partial = this.PartialBar;
                    if (partial != null)
                    {
                        records = records.Concat(new BarImpl[] { partial });
                    }
                }

                //截取数据集
                if (maxcount <= 0)
                {
                    records = records.Take(Math.Max(0, records.Count() - startIndex));
                }
                else //设定最大数量 返回数据要求 按时间先后排列
                {
                    //startIndex 首先从数据序列开头截取对应数量的数据
                    //maxcount 然后从数据序列末尾截取最大数量的数据
                    records = records.Take(Math.Max(0, records.Count() - startIndex)).Skip(Math.Max(0, (records.Count() - startIndex) - maxcount));//返回序列后段元素
                }

                //数据翻转
                if (fromEnd)
                {
                    return records.Reverse();
                }
                else
                {
                    return records;
                }

            }
        }
    }

}
