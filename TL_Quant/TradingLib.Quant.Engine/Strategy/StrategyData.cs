using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 所有的策略都需要在该环境中进行运行,StrategyCentre包含了所有相关系统层次的数据
    /// 为策略维护了对应的数据
    /// </summary>
    [Serializable]
    public class StrategyData : IStrategyData
    {

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        List<Security> securitylist;//合约列表
        
        Dictionary<Security, IBarData> secBarDataMap;//合约的数据映射


        TradingInfoTracker _infotracker;

       


        private Dictionary<Security, BarConstructionType> _barConstructiontype;
        private SortedList<Security, ChartPaneList> symbolChartPaneListMap;

        private CurrencyType _currencytype;
        public StrategyData()
        {
            //this._x69e390fb5d7385c4 = true;
            //this._x7f1302758ae4de4f = new TimeSpan(0x10, 0, 0);
        }

        public bool UseTicksForSimulation { get; set; }
        /// <summary>
        /// 组合初始参数与策略运行参数生成策略对应的策略运行数据
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="internalSettings"></param>
        public StrategyData(StrategyRunSettings settings, InternalSystemRunSettings internalSettings)
        {
            securitylist = new List<Security>();
            this.UseTicksForSimulation = settings.UseTicksForSimulation;
            this.DataStartDate = settings.DataStartDate;
            this.TradeStartDate = settings.TradeStartDate;
            this.EndDate = settings.EndDate;
            //this.SynchronizeBars = settings.sy
            this.DataConnected = true;
            this.BrokerConnected = true;
            this.InLeadBars = false;

            this.livemode = internalSettings.LiveMode;


            secBarDataMap = new Dictionary<Security, IBarData>();

            this.StrategyStarted = false;
            this._currencytype = CurrencyType.RMB;
            this._strategyhistory = new StrategyHistory(settings.StartingCapital, this);
            this._strategyhistory.SendDebugEvent += new DebugDelegate(debug);
            this.BarFrequency = settings.BarFrequency;

            _barConstructiontype = new Dictionary<Security, BarConstructionType>();

            //遍历配置中的合约信息
            foreach (SymbolSetup setup in settings.Symbols)
            {
                Security symbol = setup.Security;
                _barConstructiontype[symbol] = setup.BarConstruction;
                ChartPaneList list = new ChartPaneList();
                //新建2个ChartPane 一个是price 一个是volume
                ChartPane item = new ChartPane(false, 60);
                ChartPane pane2 = new ChartPane(false, 0x19);
                item.Name = "Price Pane";
                pane2.Name = "Volume Pane";
                list.Add(item);
                list.Add(pane2);
                this.ChartPaneCollections.Add(symbol, list);//自动创建
            }

            //将策略运行参数设置到StrategyData
            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> p in settings.StrategyParameters)
            {
                dict[p.Key] = p.Value;
            }
            _strategyparameters = new StrategyParameters(dict);

            //MessageBox.Show("传递参数:"+strategyParameters.Keys.Count.ToString() + string.Join(",",strategyParameters.Keys.ToArray()));

            _infotracker = new TradingInfoTracker(this);
            _infotracker.SendDebugEvent += new DebugDelegate(debug);

            _strategysupport = new StrategySupport(this);
            _strategysupport.SendDebugEvent += new DebugDelegate(debug);
        }
        
        #region 方法
        public Frequency GetFrequency(Security symbol, FrequencyPlugin plugin)
        {
            return this.FrequencyManager.GetFrequency(symbol, plugin);
        }
        public Frequency GetFrequency(Security symbol,BarFrequency freq)
        {
            return this.GetFrequency(symbol, new TimeFrequency(freq));
        }

        public BarConstructionType GetBarConstruction(Security symbol)
        {
            return this._barConstructiontype[symbol];
        }
        /// <summary>
        /// 获得某个合约的 某个数据项
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="barElement"></param>
        /// <param name="ignoreEmptyBars"></param>
        /// <returns></returns>
        public ISeries GetBarElementSeries(Security symbol, BarDataType barElement, bool ignoreEmptyBars)
        {
            return this.BarElementSeries[symbol, barElement, ignoreEmptyBars];
        }


        public ChartPane GetPricePane(Security symbol)
        {
            return this.symbolChartPaneListMap[symbol]["Price Pane"];
        }
        public ChartPane GetVolumePane(Security symbol)
        {
            return this.symbolChartPaneListMap[symbol]["Volume Pane"];
        }


        public Security GetSymbolByName(string name)
        {
            Security symbol = null;
            foreach (Security symbol2 in this.Symbols)
            {
                if (symbol2.Symbol == name)
                {
                    if (symbol != null)
                    {
                        throw new QSQuantError("Multiple symbols found with requested name: " + name);
                    }
                    symbol = symbol2;
                }
            }
            if (symbol == null)
            {
                throw new QSQuantError("No symbol found with requested name: " + name);
            }
            return symbol;
        }


 

 


 

 


 


        #endregion


        #region 属性

        public int TickDate = 0;
        public int TickTime = 0;


        public StrategyStatistic StrategyStatistic
        {
            get { return _strategyhistory.StrategyStatistics; }
        }
        public StrategyStatistic LongStatistic
        {
            get { return _strategyhistory.LongStatistics; }
        }

        public StrategyStatistic ShortStatistic
        {
            get { return _strategyhistory.ShortStatistics; }
        }
        public double StartingCapital
        {
            get
            {
                return this._strategyhistory.StartingCapital;
            }
        }


        StrategyHistory _strategyhistory;
        public StrategyHistory StratgyHistory { get { return _strategyhistory; } }


        bool ignorewarning=false;
        public bool IgnoreSystemWarnings
        {
            get { return ignorewarning; }
            set { ignorewarning = value; }
        }
        /// <summary>
        /// 交易信息记录
        /// </summary>
        public ITradingInfoTracker TradingInfoTracker { get { return _infotracker; } }


        StrategySupport _strategysupport = null;
        public IStrategySupport StrategySupport { get { return _strategysupport as IStrategySupport; } }

        StrategyParameters _strategyparameters;
        /// <summary>
        /// 系统全局参数
        /// </summary>
        public StrategyParameters StrategyParameters
        {
            get
            {
                if (this._strategyparameters == null)
                {
                    this._strategyparameters = new StrategyParameters();
                }
                return this._strategyparameters;
            }
            set
            {
                this._strategyparameters = value;
            }
        }

        IList<Security> _symbollist;
        /// <summary>
        /// 合约列表
        /// </summary>
        public IList<Security> Symbols
        {
            get
            {
                if (this._symbollist == null)
                {
                    this._symbollist = new List<Security>(this._barConstructiontype.Keys).AsReadOnly();
                }
                return this._symbollist;
            }
        }

        IndicatorCollections _indicators;
        /// <summary>
        /// 指标集合
        /// </summary>
        public IndicatorCollections Indicators
        {
            get
            {
                if (this._indicators == null)
                {
                    this._indicators = new IndicatorCollections(this);
                }
                return this._indicators;
            }
        }




        BarDataSeriesManager _barelementseries;
        public BarDataSeriesManager BarElementSeries
        {
            get
            {
                if (this._barelementseries == null)
                {
                    this._barelementseries = new BarDataSeriesManager(this);
                }
                return this._barelementseries;
            }
        }

        /*
        public IDictionary<Security,BarDataV> SymbolBars
        {
            get
            {
                //return this.Bars;
            }
        }

        public IDictionary<Security, BarDataV> SystemBars
        {
            get
            {
                //return this.Bars;
            }
        }**/


        //IDictionary<Security,BarDataV> _allbars;
        TBars _bars;
        /// <summary>
        /// 返回以向量格式储存的Bar数据 OHLCVI
        /// </summary>
        public ITBars Bars
        {
            get
            {
                if (this._bars == null)
                {
                    this._bars = new TBars(this);
                }
                if (this._bars.Count != this.Symbols.Count)
                {
                    this._bars = new TBars(this);
                    /*
                    this._allbars.Clear();
                    foreach (Security symbol in this.Symbols)
                    {
                        debug("准备获得symbol :" + symbol.ToString() + " 的bar数据集............");
                        this._allbars[symbol] = this.FrequencyManager.GetFrequency(symbol, this.BarFrequency).Bars;
                    }**/
                }
                return this._bars as ITBars;
            }
            private set
            {
                _bars = value as TBars;
             }
        }

        IndicatorManager _indicatormanager;
        public IIndicatorManager IndicatorManager
        {
            get
            {
                if (this._indicatormanager == null)
                {
                    this._indicatormanager = new IndicatorManager(this);
                    this._indicatormanager.SendDebugEvent +=new DebugDelegate(debug);
                }
                return this._indicatormanager as IIndicatorManager;
            }
        }

        /// <summary>
        /// 交易通道接口
        /// </summary>
        [NonSerialized]
        ISimBroker _broker = null;
        
        public ISimBroker Broker
        {
            get
            {
                return  _broker;
            }
            set
            {
                this._broker = value;
            }
        }

        

        FrequencyPlugin _barfreqplugin;
        public FrequencyPlugin BarFrequency
        {
            get
            {
                return this._barfreqplugin;
            }
            set
            {
                if (this.StrategyStarted)
                {
                    throw new QSQuantError("Cannot change system frequency after system has started.");
                }
                this._barfreqplugin = value;
            }
        }

        Dictionary<Security, ChartPaneList> _chartpanecollections;
        public Dictionary<Security, ChartPaneList> ChartPaneCollections
        {
            get
            {
                if (this._chartpanecollections == null)
                {
                    this._chartpanecollections = new Dictionary<Security, ChartPaneList>();
                }
                return this._chartpanecollections;
            }
        }

        [NonSerialized]
        FrequencyManager _freqmanager;
        public FrequencyManager FrequencyManager
        {
            get
            {
                if (this._freqmanager == null)
                {
                    //区分是否是单symbol模式加快单symbol系统的回测效率
                    if(IsOneSymbol)
                        this._freqmanager = new FrequencyManager(this._barConstructiontype, this.SynchronizeBars, this.BarFrequency,this.BarEventEnable,true);
                    else
                        this._freqmanager = new FrequencyManager(this._barConstructiontype, this.SynchronizeBars, this.BarFrequency,this.BarEventEnable);
                    //this._freqmanager.SendDebugEvent +=new DebugDelegate(debug);
                }
                return this._freqmanager;
            }
        }

        bool _syncbars = false;
        public bool SynchronizeBars
        {
            get
            {
                return this._syncbars;
            }
        }
 


        bool _systemstarted = false;
        public bool StrategyStarted
        {
          
            get
            {
                return this._systemstarted;
            }
            
            internal set
            {
                this._systemstarted = value;
            }
        }

        bool _creattickformbars = false;
        public bool CreateTicksFromBars
        {
            get
            {
                return this._creattickformbars;
            }
            set
            {
                if (this.StrategyStarted)
                {
                    throw new QSQuantError("Cannot modify SystemData.CreateTicksFromBars after system has started.");
                }
                this._creattickformbars = value;
            }
        }

        public bool ContainsSymbol(Security symbol)
        {
            return this._barConstructiontype.ContainsKey(symbol);
        }

        bool livemode = false;
        public bool LiveMode
        {
            get
            {
                return this.livemode;
            }
            internal set
            {
                this.livemode = value;
            }
        }

        ChartObjectManager _chartobjmanager;
        public ChartObjectManager ChartObjects
        {
            get
            {
                if (this._chartobjmanager == null)
                {
                    this._chartobjmanager = new ChartObjectManager(this);
                }
                return this._chartobjmanager;
            }
        }
 

 



        /// <summary>
        /// 是否是单symbol运行模式,单symbol模式下可以通过优化加快回测速度
        /// </summary>
        public bool IsOneSymbol { get; set; }

        public bool BarEventEnable = true;
 

        public DateTime DataStartDate { get; set; }
        public DateTime TradeStartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool BrokerConnected { get; set; }
        public bool DataConnected { get; set; }
        public bool InLeadBars {get;set;}

        public DateTime CurrentDate { get; set; }



        public bool ShouldProcessBar(Bar bar,QListBar bars)
        {
            bool flag = true;
            if (bar.EmptyBar)
            {
                flag = false;
                if (this.SynchronizeBars && (bars.Count > 0))
                {
                    flag = true;
                }
            }
            return flag;
        }

 

 


 

        #endregion

        public void DeserializeOwnedData(SerializationReader reader, object context)
        {
            
            this._symbollist = (IList<Security>)reader.ReadObject();
            this._currencytype = (CurrencyType)reader.ReadObject();
            this.UseTicksForSimulation = reader.ReadBoolean();
            this.IsOneSymbol = reader.ReadBoolean();
            //this._xa133caf0a34b6428 = SerializationUtils.Specialized.ReadAccountInfoSymbols(reader);
            //this._x7f1302758ae4de4f = reader.ReadTimeSpan();
            //this._xd5c2fad660ceff28 = SerializationUtils.ReadDict<Symbol, double>(reader);
            this.BarFrequency = (FrequencyPlugin)reader.ReadObject();
            this._syncbars = reader.ReadBoolean();
            //this.AllowDuplicateBars = reader.ReadBoolean();
            this._chartobjmanager = (ChartObjectManager)reader.ReadObject();
            this._chartpanecollections = (Dictionary<Security, ChartPaneList>)reader.ReadObject();
            this._creattickformbars = reader.ReadBoolean();
            this.CurrentDate = reader.ReadDateTime();
            //this.CustomString = reader.ReadString();
            this.DataStartDate = reader.ReadDateTime();
            //this.EnableTradeOnClose = reader.ReadBoolean();
            this.EndDate = reader.ReadDateTime();
            //this.xbe0bad9ae3f45ae3 = reader.ReadBoolean();
            this.InLeadBars = reader.ReadBoolean();
            this.LiveMode = reader.ReadBoolean();
            //this.x9c13656d94fc62d0 = (OutputManager)reader.ReadObject();
            this._barConstructiontype = SerializationUtils.ReadDict<Security, BarConstructionType>(reader);
            //this.xd77aa57056351ecc = (BaseSystemHistory)reader.ReadObject();
            this.StrategyParameters = (StrategyParameters)reader.ReadObject();
            this.TradeStartDate = reader.ReadDateTime();

            
            this._bars = (TBars)reader.ReadObject();
            this._strategyhistory = (StrategyHistory)reader.ReadObject();

            this._infotracker = new TradingInfoTracker(this);
            this._infotracker.DeserializeOwnedData(reader, context);


            //this._infotracker = (TradingInfoTracker)reader.ReadObject();
            

            //Bars = (TBars)reader.ReadObject();


            //foreach (KeyValuePair<Security, Bar> pair in SerializationUtils.ReadDict<Security, Bar>(reader))
            //{
                //this.FrequencyManager.GetFrequency(pair.Key, this.BarFrequency).WriteableBars.Add(pair.Value);
            //}
            /*
            if (this.x434107edc2a577d7 != null)
            {
                this.x434107edc2a577d7.SetSystemData(this);
            }
            this.xd77aa57056351ecc.SetSystemData(this);
            //this.Output.SetSystemData(this);**/
        }


        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            
            writer.WriteObject(this.Symbols);
            writer.WriteObject(this._currencytype);
            writer.WriteObject(this.UseTicksForSimulation);
            writer.WriteObject(this.IsOneSymbol);
            //SerializationUtils.Specialized.WriteAccountInfoSymbols(writer, this._xa133caf0a34b6428);
            //writer.Write(this._x7f1302758ae4de4f);
            //SerializationUtils.WriteDict<Symbol, double>(writer, this._xd5c2fad660ceff28);
            writer.WriteObject(this.BarFrequency);
            writer.Write(this.SynchronizeBars);
            //writer.Write(this.AllowDuplicateBars);
            writer.WriteObject(this.ChartObjects);
            writer.WriteObject(this.ChartPaneCollections);
            writer.Write(this.CreateTicksFromBars);
            writer.Write(this.CurrentDate);
            //writer.Write(this.CustomString);
            writer.Write(this.DataStartDate);
            //writer.Write(this.EnableTradeOnClose);
            writer.Write(this.EndDate);
            //writer.Write(this.IngoreSystemWarring);
            writer.Write(this.InLeadBars);
            writer.Write(this.LiveMode);
            //writer.WriteObject(this.x9c13656d94fc62d0);outputmanager
            SerializationUtils.WriteDict<Security, BarConstructionType>(writer, this._barConstructiontype);
            //writer.WriteObject(this.basehistorsystem);
            writer.WriteObject(this.StrategyParameters);
            writer.Write(this.TradeStartDate);

            //_bars.SerializeOwnedData(writer, context);
            writer.WriteObject(this.Bars);
            writer.WriteObject(this.StratgyHistory);
            this.TradingInfoTracker.SerializeOwnedData(writer,context);

            //writer.WriteObject(this.TradingInfoTracker)
            //writer.WriteObject(this.StratgyHistory);
            //writer.WriteObject(this.Bars);
            //Dictionary<Security, Bar> data = new Dictionary<Security, Bar>();
            //foreach (Security symbol in this.Symbols)
            //{
            //    if (this.Bars[symbol].Count > 0)
            //    {
            //        data[symbol] = this.Bars[symbol].Last;//记录当前的最新价
           //     }
           // }
           // SerializationUtils.WriteDict<Security, Bar>(writer, data);
        }

 

 


        
    }

    [Serializable]
    public class TBars : ITBars
    {
        Dictionary<Security, QListBar> _allbars;
        Dictionary<string, QListBar> _securitymap;
        Dictionary<Security, BarDataV> _rawdata;

        public int Count { get { return _allbars.Count; } }

        public TBars(StrategyData strategydata)
        {
            _allbars = new Dictionary<Security, QListBar>();
            _securitymap = new Dictionary<string, QListBar>();
            _rawdata = new Dictionary<Security,BarDataV>();

            foreach (Security symbol in strategydata.Symbols)
            {
                BarDataV d = strategydata.FrequencyManager.GetFrequency(symbol, strategydata.BarFrequency).WriteableBars;
                this._rawdata[symbol] = d;
                this._allbars[symbol] = new QListBar(d);
                this._securitymap[symbol.Symbol] = this._allbars[symbol];
            }
        }

        public TBars(List<BarDataV> bardatalist)
        {
            _allbars = new Dictionary<Security, QListBar>();
            _securitymap = new Dictionary<string, QListBar>();
            _rawdata = new Dictionary<Security, BarDataV>();

            foreach (BarDataV d in bardatalist)
            {
                this._rawdata[d.Security] = d;
                this._allbars[d.Security] = new QListBar(d);
                this._securitymap[d.Security.Symbol] = this._allbars[d.Security];
            }
        }

        public TBars()
        {
            _allbars = new Dictionary<Security, QListBar>();
            _securitymap = new Dictionary<string, QListBar>();
            _rawdata = new Dictionary<Security, BarDataV>();
        }
        /*
        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            writer.WriteObject((Int32)_rawdata.Count);
            foreach (BarDataV d in _rawdata.Values)
            {
                writer.WriteObject(d);
            }

        }

        void addBarDataV(BarDataV d)
        {
            this._rawdata[d.Security] = d;
            this._allbars[d.Security] = new QListBar(d);
            this._securitymap[d.Security.Symbol] = this._allbars[d.Security];
        }

        public void DeserializeOwnedData(SerializationReader reader, object context)
        {
            Int32 num = reader.ReadInt32();

            for (int i = 1; i <= num; i++)
            { 
                BarDataV data = (BarDataV)reader.ReadObject();
                this.addBarDataV(data);
            }

        }**/

        public IBarData this[Security security]
        {
            get
            {

                if (_allbars.Keys.Contains(security))
                    return _allbars[security];
                return null;
            }
        }

        public IBarData  this[string symbol]
        {
            get
            {
                if (_securitymap.Keys.Contains(symbol))
                    return _securitymap[symbol];
                return null;
            }
        }

    }
    
}
