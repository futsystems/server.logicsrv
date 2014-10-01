using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// chartPrice定义了一组用于chart显示的数据组
    /// </summary>
    [Serializable]
    public class ChartPriceSeries :ChartSeries
    {
        public event TickDelegate NewtTickEvent;
        public event BarDelegate NewBarEvent;
        public event BarQListDelegate NewResetBarListEvent;


        Security security;
        QList<Bar> hisbarlist;//初始的Bar
        QList<Bar> realtimebarlist;//实时生成的bar
        //IntervalData bargenerater;//用于接收Tick累计生成产生Bar数据

        //FrequencyPlugin sourcefreq;
        //BarFrequency frequency;

        public override double[] Data
        {
            get { throw new NotImplementedException(); }
        }
        bool inited = true;//标识是否已经初始化
        IBarData _bardata;
        public ChartPriceSeries(IBarData sourceData)
        {
            security = sourceData.Security;
            //hisbarlist = sourceData;
            //frequency = ;
            _bardata = sourceData as IBarData;
            //_bardata.NewBarEvent += new BarDelegate(_bardata_NewBarEvent);
            //sourcefreq = freqplugin;

            //sourcefreq = freqplugin;
            
            inited= true;


        }

        void _bardata_NewBarEvent(Bar bar)
        {
            SendNewBarEvent(bar);
        }

        /*
        /// <summary>
        /// 初始化Bar生成器
        /// </summary>
        public void InitBarGenerater()
        {
            inited = false;
            try
            {
                realtimebarlist = new QList<Bar>();//
                bargenerater = sourcefreq.CreateFrequencyGenerator();
                bargenerater.NewBar += new SymBarIntervalDelegate(bargenerater_NewBar);

                SendResetBarEvent();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                inited = true;
            }
            
        }
        //当有新的Bar生成时将该Bar加入到realtime数据
        bool bareventavabile = true;
        void bargenerater_NewBar(string symbol, int interval)
        {
            Bar nbar = bargenerater.GetBar(symbol);//得到当前最新的Bar
            realtimebarlist.Add(nbar);//将最新的Bar放入到实时产生的barlist

            SendNewBarEvent(nbar);

        }
        **/
        bool bareventavabile = true;
        /// <summary>
        /// 对外发送新Bar事件
        /// </summary>
        /// <param name="nbar"></param>
        void SendNewBarEvent(Bar nbar)
        {
            if (bareventavabile && NewBarEvent != null)
                NewBarEvent(nbar);//向外推送新的Bar数据
        }
        /// <summary>
        /// 对外触发重置序列事件
        /// </summary>
        void SendResetBarEvent()
        {
            /*
            if (NewResetBarListEvent != null)
                NewResetBarListEvent(this.PriceData);
            **/
        }


        public override ChartSeries Clone()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 回溯n个bar
        /// </summary>
        /// <param name="nBars"></param>
        /// <returns></returns>
        public override double LookBack(int nBars)
        {
            throw new NotImplementedException();
            //return (double)this.PriceData.LookBack(nBars).Volume;
        }
        bool tickflag = false;
        /// <summary>
        /// 当有新的Bar到达时进行的操作
        /// </summary>
        /// <param name="newbar"></param>
        public void NewBar(Bar newbar)
        {
            /*
            hisbarlist.Add(newbar);
            lasttime = Util.ToTLDateTime(Util.ToDateTime(newbar.Bardate, newbar.Bartime));
            if (realtimebarlist == null)
            {
                SendNewBarEvent(newbar);//如果没有实时数据序列,则直接对外发送barevent
            }
            else
            {
                tickflag = false;
                bargenerater.addbar(newbar);//如果有实时序列 则通过bargenerater进行处理
                
            }
            **/
        }


        long lasttime;
        /// <summary>
        /// 当有新的Tick数据达到时触发的操作
        /// </summary>
        /// <param name="k"></param>
        public void NewTick(Tick k)
        {
            lasttime = k.datetime;
            tickflag = true;
            //_bardata.NewTick(k);
            /*
            if (realtimebarlist != null)
            {
                //向bargenerator推送Tick数据,用于实时产生Bar数据
                bargenerater.newTick(k);
            }**/
            
        }
        /// <summary>
        /// 设定Bar生成的方式 ask,bid,trade等
        /// </summary>
        /// <param name="barConstruction"></param>
        /*
        public void SetBarConstruction(BarConstructionType barConstruction)
        {
            this._x0d63afdb95496466 = barConstruction;
            if (this._x516ee253b946ec9e != null)
            {
                this.xb22dcf0384e65973();
            }
        }**/

        // Properties
        /// <summary>
        /// 序列数量
        /// </summary>
        public override int Count
        {
            get
            {
                return this._bardata.Count;
            }
        }
        /*
        /// <summary>
        /// 当前显示的序列频率类型
        /// </summary>
        
        public FrequencyPlugin DisplayFrequency
        {
            get
            {
                return this.sourcefreq;
            }
            set
            {
                if (!value.Equals(this.sourcefreq))
                {
                    this.sourcefreq= value;
                    //if (this.sourcefreq.Equals(this._xaa15459dd749a420))
                    //{
                        this.realtimebarlist = null;
                    //}
                    //else
                    {
                        this.InitBarGenerater();
                    }
                }
            }
        }**/
        /*
        public override bool HasPartialItem
        {
            get
            {
                return this._xeaa4008a82e55001.HasPartialItem;
            }
        }

        public override double PartialItem
        {
            get
            {
                if (!this.HasPartialItem)
                {
                    return double.NaN;
                }
                return (double)this._xeaa4008a82e55001.PartialItem.Volume;
            }
        }**/

        public IBarData PriceData
        {
            get
            {
                return _bardata;
            }
        }
        /*
        public FrequencyPlugin SourceFrequency
        {
            get
            {
                return this._xaa15459dd749a420;
            }
        }**/








    }
}
