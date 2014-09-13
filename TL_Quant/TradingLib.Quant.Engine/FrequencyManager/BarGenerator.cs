using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.Xml;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
using System.Threading;

namespace TradingLib.Quant.Engine
{
    public class BarGenerator
    {
        // Fields
        private BarConstructionType barconstruction;
        private bool _updated;
        private Bar _barpartialbar;
        private Security security;
        private Bar _currentpartialbar;
        private bool _tickissent;

        public event SingleBarEventArgsDel SendSingleBarEvent;
        public event NewTickEventArgsDel SendNewTickEvent;


        // Methods
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="barConstruction"></param>
        public BarGenerator(Security symbol,int intival, BarConstructionType barConstruction,BarDataV barstore=null)
        {
            this.security = symbol;
            this.Interval = intival;
            this.barconstruction = barConstruction;
            _barstore = barstore;
            this.CloseBar(DateTime.MinValue);
        }

        /// <summary>
        /// 给BarGenerator绑定数据储存结构,那么BarGenerator生成的Bar就会自动存储在数据结构中
        /// </summary>
        /// <param name="datastore"></param>
        public void SetDataStore(BarDataV datastore)
        {
            _barstore = datastore;
            
        }
        BarDataV _barstore = null;

        /// <summary>
        /// 处理bar信息
        /// </summary>
        /// <param name="args"></param>
        public void ProcessBar(SingleBarEventArgs args)
        {
            if (args.Symbol == this.security)//security过滤
            {
                if (this._barpartialbar.EmptyBar)
                {
                    if (!args.Bar.EmptyBar)
                    {
                        this._barpartialbar.Open = args.Bar.Open;
                        this._barpartialbar.High = args.Bar.High;
                        this._barpartialbar.Low = args.Bar.Low;
                        this._barpartialbar.Close = args.Bar.Close;
                        this._barpartialbar.Bid = args.Bar.Bid;
                        this._barpartialbar.Ask = args.Bar.Ask;
                        this._barpartialbar.OpenInterest = args.Bar.OpenInterest;
                        this._barpartialbar.Volume = args.Bar.Volume;
                        this._barpartialbar.EmptyBar = false;
                    }
                }
                else
                {
                    if (args.Bar.High > this._barpartialbar.High)
                    {
                        this._barpartialbar.High = args.Bar.High;
                    }
                    if (args.Bar.Low < this._barpartialbar.Low)
                    {
                        this._barpartialbar.Low = args.Bar.Low;
                    }
                    this._barpartialbar.Volume += args.Bar.Volume;
                    this._barpartialbar.Close = args.Bar.Close;
                    this._barpartialbar.Bid = args.Bar.Bid;
                    this._barpartialbar.Ask = args.Bar.Ask;
                    this._barpartialbar.OpenInterest = args.Bar.OpenInterest;
                }
                this._currentpartialbar = this._barpartialbar.Clone();
            }
        }

        /// <summary>
        /// 处理Tick信息
        /// </summary>
        /// <param name="tick"></param>
        public void ProcessTick(Tick tick)
        {
            //如果Tick数据有效 则更新Bar的相关数据项目
            
            if (tick.isValid)
            {
                this._currentpartialbar.EmptyBar = false;//重要只有标记了费emptyd 表明Bar接收过了数据 才会被发送出去
                this._currentpartialbar.time = tick.time;
                //过滤掉一下2个比较速递提高5万左右
                //if(tick.hasAsk)
                //    this._currentpartialbar.Bid = (double)tick.ask;
                //    this._currentpartialbar.EmptyBar = false;
                //if(tick.hasBid)
                //    this._currentpartialbar.Ask = (double)tick.bid;
                //    
                if(tick.isTrade)
                    this._currentpartialbar.Volume += tick.size;
                    this._currentpartialbar.OpenInterest = tick.OpenInterest;
                
            }
            
            //通过Bar的生成方式 来计算对应的价格 以及价格更新标志
            double naN = double.NaN;
            bool flag = false;
            if (this.barconstruction == BarConstructionType.Trades && tick.isTrade)
            {
                    naN = (double)tick.trade;
                    flag = true;
            }
            else if (this.barconstruction == BarConstructionType.Bid)
            {
                if (tick.hasBid)
                {
                    naN = (double)tick.bid;
                    flag = true;
                }
            }
            else if (this.barconstruction == BarConstructionType.Ask)
            {
                if (tick.hasAsk)
                {
                    naN = (double)tick.ask;
                    flag = true;
                }
            }
            else
            {
                if (this.barconstruction != BarConstructionType.Mid)
                {
                    throw new QSQuantError("Unrecognized bar construction type: " + this.barconstruction);
                }
                if (((tick.hasAsk) || (tick.hasBid)) && ((!double.IsNaN((double)this._currentpartialbar.Bid) && !double.IsNaN((double)this._currentpartialbar.Ask)) && ((this._currentpartialbar.Bid != 0.0) && (this._currentpartialbar.Ask != 0.0))))
                {
                    naN = ((double)(this._currentpartialbar.Ask + this._currentpartialbar.Bid)) / 2.0;
                    flag = true;
                }
            }


            //通过比较更新 HLC数据 这里速度没有太多优化的地方
            if (flag)
            {
                //如果没有更新则 将当前数值赋值到所有数据项
                if (!this._updated)
                {
                    this._currentpartialbar.Open = naN;
                    this._currentpartialbar.High = naN;
                    this._currentpartialbar.Low =naN;
                    this._updated = true;
                }
                else if (naN > this._currentpartialbar.High)
                {
                    //MessageBox.Show("Last high:" + this._currentpartialbar.High.ToString() + " now high:" + naN.ToString());
                    this._currentpartialbar.High = naN;
                }
                else if (naN < this._currentpartialbar.Low)
                {
                    this._currentpartialbar.Low = naN;
                }
                this._currentpartialbar.Close = naN;
                
                //标记Tick数据已经处理 进而发送
                this._tickissent = true;
            }

            //EventHandler<NewTickEventArgs> handler = this.tickEventHandler;
            //if (handler != null)
            if (_barstore==null&&this.SendNewTickEvent !=null )
            {
                NewTickEventArgs e = new NewTickEventArgs
                {
                    PartialBar = this._currentpartialbar,
                    Symbol = this.security,
                    Tick = tick
                };

                SendNewTickEvent(e);
            }
            
        }
        /// <summary>
        /// 对外发送新的Bar数据
        /// </summary>
        /// <param name="barEndTime"></param>
        public void SendNewBar(DateTime barEndTime)
        {
            bool flag = !this._updated && (this._currentpartialbar.Close == 0.0);
            //MessageBox.Show("BarGen-sendbar"+this._currentpartialbar.ToString());
            //注意 我们需要下个tick的时间来证明该Bar已经结束，下个tick检查时间会重新对currentpartialbar赋值，这里需要copy
            //Bar data = this._currentpartialbar;
            SingleBarEventArgs e = new SingleBarEventArgs(this.security,new BarImpl(this._currentpartialbar), barEndTime, this._tickissent);
            
            //发送当前Bar的时候需要关闭当前Bar
            this.CloseBar(barEndTime);//结算掉当前的Bar
            if (flag)//如果当前这个Bar生成后没有更新,则我们需要手工得到其对应OHLC
            {
                bool flag2 = (e.Bar.Bid != 0.0) && !double.IsNaN((double)e.Bar.Bid);
                bool flag3 = (e.Bar.Ask != 0.0) && !double.IsNaN((double)e.Bar.Ask);
                if (flag2 && flag3)
                {
                    e.Bar.Open = e.Bar.High = e.Bar.Low = e.Bar.Close = ((e.Bar.Bid + e.Bar.Ask) / 2.0);
                }
                else if (flag2)
                {
                    e.Bar.Open = e.Bar.High = e.Bar.Low = e.Bar.Close = e.Bar.Bid;
                }
                else if (flag3)
                {
                    e.Bar.Open = e.Bar.High = e.Bar.Low = e.Bar.Close = e.Bar.Ask;
                }
            }
            //Profiler.Instance.EnterSection("bargenSendbar");
            //Bar过滤 开始时间部位minvalue,且部位emptyBar的Bar数据 我们对外发送
            if ((e.Bar.BarStartTime != DateTime.MinValue && !e.Bar.EmptyBar))
            {
                    if (SendSingleBarEvent != null)
                        SendSingleBarEvent(e);
            }
            //Profiler.Instance.LeaveSection();
        }

        int _interval = 0;
        public int Interval { get { return _interval; } set { _interval = value; } }

        long _barID = 0;

        public long CurrentBarID { get { return _barID; } set { _barID = value; } }
        /// <summary>
        /// 设定Bar的起始时间
        /// </summary>
        /// <param name="barStartTime"></param>
        public void SetBarStartTime(DateTime barStartTime)
        {
            this._currentpartialbar.BarStartTime = barStartTime;
            this._barpartialbar.BarStartTime = barStartTime;
        }
        /// <summary>
        /// 结算掉一个Bar,重新生成一个新的Bar数据
        /// </summary>
        /// <param name="nowbarendtime"></param>
        private void CloseBar(DateTime nowbarendtime)
        {
            //Profiler.Instance.EnterSection("CreateBar");
            Bar data = this._currentpartialbar;//new BarImpl(this._currentpartialbar);
            this._tickissent = false;

            if (_barstore == null)
                this._currentpartialbar = new BarImpl(_interval);
            else
            {
                //Profiler.Instance.EnterSection("getBarWrapper");
                _barstore.Add();//新增一条数据条目,然后将该新增条目封装成Bar格式 进行数据更新
                this._currentpartialbar = new BarDataV2PartialBar(_barstore);
                _barstore.PartialBar = this._currentpartialbar;//将新的partialBar传递给BarDataV
                //Profiler.Instance.LeaveSection();
            }
            this._currentpartialbar.BarStartTime = nowbarendtime;
            this._currentpartialbar.EmptyBar = true;//并且该Bar为空
            this._updated = false;//标注CurrentBar没有更新过数据新生成的Bar

            if (data != null)
            {
                //上根k线的价格为 下根K线的的OHLC价格
                this._currentpartialbar.Open = this._currentpartialbar.Close = this._currentpartialbar.High = this._currentpartialbar.Low = data.Close;
                this._currentpartialbar.Bid = data.Bid;
                this._currentpartialbar.Ask = data.Ask;
            }
            //将tick生成的Bar赋值到barpartialbar当中
            this._barpartialbar = this._currentpartialbar.Clone();
           // Profiler.Instance.LeaveSection();
        }

        // Properties
        public BarConstructionType BarConstruction
        {
            get
            {
                return this.barconstruction;
            }
        }

        public Bar BarPartialBar
        {
            get
            {
                return this._barpartialbar;
            }
        }
        /// <summary>
        /// 获得当前Bar的起始时间
        /// </summary>
        public DateTime BarStartTime
        {
            get
            {
                return this._currentpartialbar.BarStartTime;
            }
        }
        /// <summary>
        /// 获得当前更新中的当前Bar
        /// </summary>
        public Bar PartialBar
        {
            get
            {
                return this._currentpartialbar;
            }
        }
    }


}
