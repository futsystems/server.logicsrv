using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
namespace TradingLib.Quant.Engine
{
    [Serializable]
    public class TickDataStreamer
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        // Fields
        private IDataStore _dataStore;
        private DateTime _endDate;
        private DateTime _startDate;
        private Dictionary<Security, TickStream> _tickStreams;
        private TickStreamQueue<TickStream> _streamqueue;
        internal static long TICK_BUFFER_SIZE = 10000;
        private long _totalticks;


        // Methods
        public TickDataStreamer(IDataStore dataStore, IEnumerable<Security> symbols, DateTime startDate, DateTime endDate)
        {
            this._dataStore = dataStore;

            this._startDate = startDate;
            this._endDate = endDate;

            this.TotalTicks = 0;
            Dictionary<Security, int> symbolOrder = new Dictionary<Security, int>();
            int num = 0;
            foreach (Security symbol in symbols)
            {
                symbolOrder[symbol] = num;
                num++;
            }
            this._tickStreams = new Dictionary<Security, TickStream>();//security->tickstream映射

            //生成tickStream队列
            this._streamqueue = new TickStreamQueue<TickStream>(delegate(TickStream s1, TickStream s2)
            {
                int idnum = s1.NextTick.Tick.time.CompareTo(s2.NextTick.Tick.time);
                if (idnum == 0)
                {
                    idnum = symbolOrder[s1.Symbol].CompareTo(symbolOrder[s2.Symbol]);
                }
                return idnum;
            });


            int i = 0;
            //遍历所有的合约列表
            foreach (Security symbol2 in symbols)
            {

                //1.生成该合约所对应的stream
                TickStream stream = new TickStream(symbol2, dataStore, startDate, endDate);
                stream.SendDebugEvent += new DebugDelegate(debug);

                if (i == 0)
                    firstSymbolStream = stream;//记录第一个symbol的tickstream用于单合约回测的时候进行快速回放
                i++;

                //2.将stream加入到map映射
                this._tickStreams.Add(symbol2, stream);

                //3.如果stream含有数据 则加入到本地streamqueue
                if (stream.NextTick != null)
                {
                    this._streamqueue.Enqueue(stream);
                }

                //4.获得该stream的tick数量 累计到totalticks
                using (IDataAccessor<Tick> accessor = dataStore.GetTickStorage(symbol2))
                {
                    long count = accessor.GetCount(startDate, endDate);
                    this.TotalTicks += count;
                    continue;
                }
            }
        }
        TickStream firstSymbolStream;

        /// <summary>
        /// 单合约Tick数据快速回放 Consume
        /// 只回放合约列表中的第一个合约 适用于单合约策略回测 套利 组合策略需要用排队回放
        /// </summary>
        public void QucikConsumeTick()
        {
            if (firstSymbolStream != null)
                firstSymbolStream.ConsumeTick();
        }
        /// <summary>
        /// 单合约Tick数据快速回放 NextTick
        /// 只回放合约列表中第一个合约 适用于单合约策略回测 套利 组合策略需要用排队回放
        /// </summary>
        public TickItem QuickNextTick
        {

            get
            {
                if (firstSymbolStream != null)
                    return firstSymbolStream.NextTick;
                return null;
            }
        }
        /// <summary>
        /// 消费Tick数据
        /// 混带多合约数据组合回放模式(多个品种的套利回测等)
        /// </summary>
        public void ConsumeTick()
        {
            if (this._streamqueue.Count != 0)
            {
                TickStream item = this._streamqueue.Dequeue();
                item.ConsumeTick();
                if (item.NextTick != null)
                {
                    this._streamqueue.Enqueue(item);
                }
            }
        }

        // Properties
        /// <summary>
        /// 从队列中得到下一条Tick数据
        /// </summary>
        public TickItem NextTick
        {
            get
            {
                if (this._streamqueue.Count == 0)
                {
                    return null;
                }
                return this._streamqueue.Peek().NextTick;
            }
        }

        /// <summary>
        /// 获得tick数据数量
        /// </summary>
        public long TotalTicks
        {
            get
            {
                return this._totalticks;
            }
            private set
            {
                this._totalticks = value;
            }
        }

        #region 单独的symbol的tickStream
        private class outputtickCheck
        {
            public DateTime loadedtime;
            public outputtickCheck(DateTime load)
            {
                loadedtime = load;
            }
            public bool isgoodtick(Tick k)
            {
                return Util.ToDateTime(k.date, k.time) < loadedtime;

            }
        }

        // Nested Types
        private class TickStream
        {
            public event DebugDelegate SendDebugEvent;
            void debug(string msg)
            {
                if (SendDebugEvent != null)
                    SendDebugEvent(msg);
            }

            // Fields
            private bool _finished;
            private long _bufferstartdate;
            private Queue<Tick> _tickqueue;
            private IDataStore _datastore;
            private long _startdate;
            private long _enddate;

            // Methods
            public TickStream(Security symbol, IDataStore dataStore, DateTime dataStartDate, DateTime endDate)
            {
                this.Symbol = symbol;
                this._datastore = dataStore; ;
                this._startdate = Util.ToTLDateTime(dataStartDate);
                this._enddate = Util.ToTLDateTime(endDate);
                this._finished = false;
                this._bufferstartdate = Util.ToTLDateTime(dataStartDate);
            }

            /// <summary>
            /// 消费Tick数据
            /// </summary>
            public void ConsumeTick()
            {
                if (!this._finished)
                {
                    this._tickqueue.Dequeue();
                    if (this._tickqueue.Count == 0)
                    {
                        this.loadmoretick();
                    }
                }
            }
            /// <summary>
            /// 加载一定数量的Tick数据到缓存
            /// </summary>
            private void loadmoretick()
            {
                //  debug("Load more Ticks...");
                //MessageBox.Show("load more ticks");
                using (IDataAccessor<Tick> accessor = this._datastore.GetTickStorage(this.Symbol))
                {
                    long lastLoadedDate;
                    List<Tick> collection = accessor.Load(this._bufferstartdate, this._enddate, TickDataStreamer.TICK_BUFFER_SIZE, false);
                    //如果加载的数据集长度0 表明数据消费完毕
                    if (collection.Count == 0)
                    {
                        this._finished = true;
                    }
                    else
                    {
                        //标记最后一个Tick的时间 为最后加载时间
                        lastLoadedDate = Util.ToTLDateTime(collection[collection.Count - 1].date, collection[collection.Count - 1].time);
                        //如果最后加载时间==当前集合第一个Tick的时间
                        
                        if (lastLoadedDate == Util.ToTLDateTime(collection[0].date, collection[0].time))
                        {
                            Profiler.Instance.EnterSection("sameday");
                            
                            if (collection.Count == TickDataStreamer.TICK_BUFFER_SIZE)
                            {
                                collection = accessor.Load(lastLoadedDate, lastLoadedDate, -1, false);
                            }
                            this._tickqueue = new Queue<Tick>(collection);
                            //
                            this._bufferstartdate = lastLoadedDate+1;
                            
                            //this._finished = true;
                            //this._tickqueue = null;
                            Profiler.Instance.LeaveSection();
                        }
                        else
                        {
                            //Tick数据检查过滤
                            //outputtickCheck t = new outputtickCheck(lastLoadedDate);
                            //this._tickqueue= new Queue<Tick>(collection.Where<Tick>(new Func<Tick, bool>(t.isgoodtick)));
                            this._tickqueue = new Queue<Tick>(collection);
                            //下次缓存开始时间为当前加载最后时间
                            this._bufferstartdate = lastLoadedDate+1;
                        }
                    }
                }
            }


            // Properties
            /// <summary>
            /// 获得下一个Tick数据
            /// </summary>
            public TickItem NextTick
            {
                get
                {
                    if (!this._finished && (this._tickqueue == null))
                    {
                        this.loadmoretick();
                    }
                    if (this._finished)
                    {
                        return null;
                    }
                    Tick k = this._tickqueue.Peek();
                    k.symbol = this.Symbol.Symbol;
                    TickItem it = new TickItem(this.Symbol, k);
                    return it;
                }
            }
            /// <summary>
            /// 获得对应的Security
            /// </summary>
            public Security Symbol { get; private set; }
        }

        #endregion

        #region 多个symbol的tick 队列 这里涉及到多个tick序列 按照时顺序进行回放的一个算法
        internal class TickStreamQueue<T>
        {
            // Fields
            private int _currentsize;//队列中的数据个数
            private int _x272bc993a9d89cb6;
            private int _arraysize;//可以放入的symbol个数
            private Comparison<T> _compare;
            private T[] _streamarray;

            // Methods
            public TickStreamQueue(Comparison<T> comparer)
            {
                this._compare = comparer;
                this._arraysize = 15;
                this._streamarray = new T[this._arraysize];
            }

            /// <summary>
            /// 出队列
            /// </summary>
            /// <returns></returns>
            public T Dequeue()
            {
                if (this._currentsize == 0)
                {
                    throw new InvalidOperationException();
                }
                T local = this._streamarray[0];//获得第一位的stream
                this._currentsize--;

                this.x87a5305887ea3eed(0, this._streamarray[this._currentsize]);
                this._streamarray[this._currentsize] = default(T);
                this._x272bc993a9d89cb6++;
                return local;
            }
            /// <summary>
            /// 入队列
            /// </summary>
            /// <param name="item"></param>
            public void Enqueue(T item)
            {
                if (this._currentsize == this._arraysize)
                {
                    this.expandArray();
                }
                this._currentsize++;
                this.x227bc783746e35e0(this._currentsize - 1, item);
                this._x272bc993a9d89cb6++;
            }

            public T Peek()
            {
                if (this._currentsize == 0)
                {
                    throw new InvalidOperationException();
                }
                return this._streamarray[0];
            }

            //将某个stream
            private void x227bc783746e35e0(int location, T xccb63ca5f63dc470)
            {
                for (int i = this.getHelfSize(location); (location > 0) && (this._compare(this._streamarray[i], xccb63ca5f63dc470) > 0); i = this.getHelfSize(location))
                {

                    this._streamarray[location] = this._streamarray[i];
                    location = i;
                }
                this._streamarray[location] = xccb63ca5f63dc470;
            }

            private int getHelfSize(int location)
            {
                return ((location - 1) / 2);
            }
            //将某个
            /// <summary>
            /// 出队列后 0为为空,将0为与_streamarray[this._currentsize]传入该函数
            /// i=gentExnpandSize(0) = 1;
            /// 2位与_streamarray[this._currentsize]比较,如果成立则(i++) this._streamarray[0] = this._streamarray[2];
            /// 
            /// 
            /// </summary>
            /// <param name="location"></param>
            /// <param name="xccb63ca5f63dc470"></param>
            private void x87a5305887ea3eed(int location, T xccb63ca5f63dc470)
            {
                for (int i = this.getExpanedSize(location); i < this._currentsize; i = this.getExpanedSize(location))
                {
                    //比较相邻2个stream 如果为真 序号递增
                    if (((i + 1) < this._currentsize) && (this._compare(this._streamarray[i], this._streamarray[i + 1]) > 0))
                    {
                        i++;
                    }
                    this._streamarray[location] = this._streamarray[i];
                    location = i;
                }
                this.x227bc783746e35e0(location, xccb63ca5f63dc470);
            }

            private int getExpanedSize(int location)
            {
                return ((location * 2) + 1);
            }
            /// <summary>
            /// 数据队列扩充,当队列中数目达到我们初始化设定的数目时,我对其进行2倍扩充
            /// </summary>
            private void expandArray()
            {
                this._arraysize = (this._arraysize * 2) + 1;
                T[] destinationArray = new T[this._arraysize];
                Array.Copy(this._streamarray, 0, destinationArray, 0, this._currentsize);
                this._streamarray = destinationArray;
            }

            // Properties
            public int Count
            {
                get
                {
                    return this._currentsize;
                }
            }
        }



        #endregion
    }



    public class TickItem
    {
        // Fields
        private Security _symbol;
        private Tick _tick;

        // Methods
        public TickItem(Security symbol, Tick tick)
        {
            this._symbol = symbol;
            this._tick = tick;
        }

        // Properties
        public Security Symbol
        {
            get
            {
                return this._symbol;
            }
        }

        public Tick Tick
        {
            get
            {
                return this._tick;
            }
        }
    }




}
