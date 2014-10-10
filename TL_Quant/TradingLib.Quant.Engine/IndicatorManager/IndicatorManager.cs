using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Drawing;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    /*
     * Manages indicators and user series. 
       Remarks
       This class handles calling the NewBar method for each indicator in the system in the correct order. 
     * Indicators must be registered with this class for this to work. 
     * Indicators that are declared as fields of your symbol script class will automatically be registered. 
     * If you need to register an indicator manually, you can call Register from your Startup method. 
     * 管理了系统内的所有indicator用于统一按照次序对indicator进行数据更新,调用Caculate


     * 
     * */
    [Serializable]
    public class IndicatorManager
    {
        public const string PROGRAME = "IndicatorManager";
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        // Fields
        private List<SeriesFillInfo> seriesfillinfolist = new List<SeriesFillInfo>();//记录2个序列间填充
        private List<long> updateorder = new List<long>();//记录更新顺序
        private List<KeyValuePair<Security, object>> _securityobjMap = new List<KeyValuePair<Security, object>>();//记录security object映射对
        private StrategyData _strategydata;//系统全局数据
        private bool _inited;//是否已经初始化
        private Dictionary<long, SeriesInfo> _seriesInfoMap = new Dictionary<long, SeriesInfo>();//全局id->seriesinfo映射
        private ObjectIDGenerator _idmanager = new ObjectIDGenerator();//id管理

        // Methods
        internal IndicatorManager(StrategyData systemData)
        {
            this._strategydata = systemData;
        }

        /// <summary>
        /// 用某个颜色填充2个序列间
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="series1"></param>
        /// <param name="series2"></param>
        /// <param name="fillColor"></param>
        public void FillIndicatorRegion(Security symbol, ISeries series1, ISeries series2, Color fillColor)
        {
            SeriesInfo xccfbbc = this.registerCall(series1, symbol, null) ?? this.GetSeriesInfo(series1);
            SeriesInfo xccfbbc2 = this.registerCall(series2, symbol, null) ?? this.GetSeriesInfo(series2);
            if ((xccfbbc == null) || (xccfbbc2 == null))
            {
                throw new QSQuantError("Series not registered.");
            }
            SeriesFillInfo item = new SeriesFillInfo
            {
                Data1 = xccfbbc,
                Data2 = xccfbbc2,
                FillColor = fillColor
            };
            this.seriesfillinfolist.Add(item);
        }

        /// <summary>
        /// 获得指标当最新值
        /// </summary>
        /// <returns></returns>
        public Dictionary<Security, Dictionary<string, double>> GetLatestIndicatorValues()
        {
            Dictionary<Security, Dictionary<string, double>> dictionary = new Dictionary<Security, Dictionary<string, double>>();
            foreach (SeriesInfo xccfbbc in this._seriesInfoMap.Values)
            {
                Dictionary<string, double> dictionary2;
                double current;
                //如果没有在图表中显示则 继续下一个指标
                if (!xccfbbc.LastChartSettings.ShowInChart)
                {
                    continue;
                }
                //通过security查找对应的 指标数值映射
                if (!dictionary.TryGetValue(xccfbbc.Symbol, out dictionary2))
                {
                    dictionary2 = new Dictionary<string, double>();
                    dictionary[xccfbbc.Symbol] = dictionary2;
                }
                //如果有数值 则取值,或者至空
                if (xccfbbc.Indicator.Count > 0)
                {
                    current = xccfbbc.Indicator.Last;
                }
                else
                {
                    current = double.NaN;
                }
                dictionary2[xccfbbc.DisplayName] = current;
            }
            return dictionary;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            debug(PROGRAME + ":指标管理器 初始化.....");
            if (this._inited)
            {
                throw new InvalidOperationException("Initialize() method should only be called once.");
            }
            //1.注册security-obj 这里是针对自动注册的指标本身进行注册
            debug(PROGRAME+":#自动注册Security所对应的Object所含有的ISeries Field");
            foreach (KeyValuePair<Security, object> pair in this._securityobjMap)
            {
                this.RegisterISeriesField(pair.Value, pair.Key);
            }
            //2.注册seriesinfo通过注册seriesinfo迭代 将inputs也注册到系统(input有可能也是indicator iseries序列)
            debug(PROGRAME+":#注册SeriesInfo中指标的输入数据序列");
            foreach (SeriesInfo xccfbbc in new List<SeriesInfo>(this._seriesInfoMap.Values))
            {
                this.RegisterSereisInput(xccfbbc);
            }
            //如果series没有默认的频率数据,则将系统默认频率赋予该series
            foreach (SeriesInfo xccfbbc2 in this._seriesInfoMap.Values)
            {
                if (xccfbbc2.Frequency == null)
                {
                    xccfbbc2.Frequency = this._strategydata.BarFrequency;
                }
            }
            //重新计算更新顺序
            debug(PROGRAME +"#更新计算顺序");
            this.CalUpdateOrder();

            //给指标绑定输入序列
            debug(PROGRAME +"#绑定数据输入序列");
            foreach (SeriesInfo xccfbbc3 in this._seriesInfoMap.Values)
            {
                this.SetSeriesInputs(xccfbbc3);
            }
            this._inited = true;
        }

        /// <summary>
        /// 获得最新的Bar数据
        /// </summary>
        /// <param name="args"></param>
        public void NewBar(FrequencyNewBarEventArgs args)
        {
            if (!this._inited)
            {
                throw new InvalidOperationException("Initialize() method should be called before system begins processing bars.");
            }
            //按照顺序更新指标计算
            foreach (long num in this.updateorder)
            {
                SingleBarEventArgs args2;
                SeriesInfo xccfbbc = this._seriesInfoMap[num];
                FreqKey key = new FreqKey(xccfbbc.Frequency, xccfbbc.Symbol);
                if (args.FrequencyEvents.TryGetValue(key, out args2))
                {
                    bool flag = true;
                    if ((xccfbbc.Symbol != null) && args2.Bar.EmptyBar)
                    {
                        flag = false;
                        if (this._strategydata.SynchronizeBars && (xccfbbc.Indicator.Count > 0))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        
                        if (xccfbbc.Indicator is IIndicator)
                        {

                            (xccfbbc.Indicator as IIndicator).Caculate();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当有Tick数据进来是按照顺序更新指标
        /// 注只有需要该symbol数据的Series才进行更新计算(如果某个指标利用了2个不同symbol的相关数据如何更新？)
        /// </summary>
        /// <param name="args"></param>
        public void NewTick (Security symbol,Tick k)
        {
            if (!this._inited)
            {
                throw new InvalidOperationException("Initialize() method should be called before system begins processing bars.");
            }
            //按照顺序更新指标计算
            foreach (long num in this.updateorder)
            {
                SeriesInfo xccfbbc = this._seriesInfoMap[num];
                if (xccfbbc.Symbol != symbol) continue;//只更新该合约对应的指标数据计算
                if (xccfbbc.Indicator is IIndicator)
                {
                    (xccfbbc.Indicator as IIndicator).Caculate();
                }
            }
        }

        /// <summary>
        /// 为某个security注册一个ISeries
        /// </summary>
        /// <param name="series"></param>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        public void Register(ISeries series, Security symbol, string name)
        {
            this.registerCall(series, symbol, name);
        }

        /// <summary>
        /// 将某个对象的Field自动注册到指标管理器
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="symbol"></param>
        public void RegisterMembers(object obj, Security symbol)
        {
            this._securityobjMap.Add(new KeyValuePair<Security, object>(symbol, obj));
            this.RegisterISeriesField(obj, symbol);
        }

        /// <summary>
        /// 设定频率
        /// </summary>
        /// <param name="series"></param>
        /// <param name="frequency"></param>
        public void SetFrequency(ISeries series, Frequency frequency)
        {
            if (series == null)
            {
                throw new ArgumentNullException("series");
            }
            if (frequency == null)
            {
                throw new ArgumentNullException("frequency");
            }
            SeriesInfo xccfbbc = this.registerCall(series, frequency.Symbol, "");
            if (xccfbbc == null)
            {
                throw new InvalidOperationException("The series paramater was not a series that can be registered to the IndicatorManager.");
            }
            xccfbbc.Frequency = frequency.FrequencySettings;
            
            if (series is UserSeries)
            {
                ((UserSeries)series).SetFrequency(frequency);
            }
        }

        /// <summary>
        /// 更新图表
        /// </summary>
        public void UpdateCharts()
        {
            debug("IndicatorManager 更新图表显示 :" + this._seriesInfoMap.Count.ToString());
            
            //MessageBox.Show();
            foreach (SeriesInfo xccfbbc in this._seriesInfoMap.Values)
            {

                //如果series当前设定与最后一次设定不一致，则重置图表 进行重绘
                if (!SeriesChartSettings.Equals(xccfbbc.LastChartSettings, xccfbbc.Indicator.ChartSettings))
                {
                    debug("检查seriesChartsettings");
                    this.ResetSeriesChartPane(xccfbbc);
                    this.DisplaySeries(xccfbbc);
                }

                if (xccfbbc.ChartSeries != null)
                {
                    debug(PROGRAME + ":更新指标数值....");
                    //xccfbbc.ChartSeries.set
                    
                    //xccfbbc.ChartSeries.OldestValueChanged = -1;
                    int num = xccfbbc.Indicator.Count - xccfbbc.SynchronizedBarsInChart;
                    int num2 = 0;
                    //检查需要往回复制多少位数据(oldvalue change)
                    /*
                    if (xccfbbc.Indicator.OldValuesChange)
                    {
                        num2 = (xccfbbc.Indicator.OldestValueChanged - num) + 1;
                    }
                     * */
                    IBarData list = this._strategydata.Bars[xccfbbc.Symbol];
                    

                    if (num2 > 0)
                    {
                        int lookBack = 0;
                        for (int j = 0; j < num2; j++)
                        {
                            int nBars = num + j;
                            if (nBars >= list.Count)
                            {
                                break;
                            }
                            if (!list.LookBack(nBars).EmptyBar)
                            {
                                xccfbbc.ChartSeries.SetValue(lookBack, xccfbbc.Indicator.LookBack(nBars));
                                lookBack++;
                            }
                        }
                    }
                    //   5 14   19
                    // # # # # # * * * * * * * * * * * * * *
                    //同步Bar的指标计算数据
                    for (int i = xccfbbc.SynchronizedBarsInChart; i < xccfbbc.Indicator.Count; i++)
                    {
                        int num7 = (xccfbbc.Indicator.Count - i) - 1;
                        //if (!list.LookBack(num7).EmptyBar)
                        {
                            xccfbbc.ChartSeries.Add(xccfbbc.Indicator.LookBack(num7));
                        }
                        xccfbbc.SynchronizedBarsInChart++;
                    }
                    debug(" xccfbbc.ChartSeries num:" + xccfbbc.ChartSeries.Count.ToString() + "  bar num:" +list.Count.ToString());
                }
            }
            foreach (SeriesFillInfo xbcecfac in this.seriesfillinfolist)
            {
                string displayName = xbcecfac.Data1.DisplayName;
                string key = xbcecfac.Data2.DisplayName;
                Security symbol = xbcecfac.Data1.Symbol;
                if (xbcecfac.Data2.Symbol != symbol)
                {
                    throw new QSQuantError("Series " + displayName + " and " + key + " must apply to the same symbol to have the region between them filled.");
                }
                string chartPaneName = xbcecfac.Data1.Indicator.ChartSettings.ChartPaneName;
                int paneIndex = this._strategydata.ChartPaneCollections[symbol].GetPaneIndex(chartPaneName, false);
                string name = xbcecfac.Data2.Indicator.ChartSettings.ChartPaneName;
                int num9 = this._strategydata.ChartPaneCollections[symbol].GetPaneIndex(name, false);
                if (paneIndex != num9)
                {
                    throw new QSQuantError("Series " + displayName + " and " + key + " must be in the same chart pane to have the region between them filled.");
                }
                this._strategydata.ChartPaneCollections[xbcecfac.Data1.Symbol][paneIndex].ChartData.LinkedIndicators[displayName] = new KeyValuePair<string, Color>(key, xbcecfac.FillColor);
            }
        }
        //重置series对应的pane
        private void ResetSeriesChartPane(SeriesInfo seriesinfo)
        {
            if (seriesinfo.ChartSeries != null)
            {
                Security symbol = seriesinfo.LastChartSettings.Symbol ?? seriesinfo.Symbol;
                int paneIndex = this._strategydata.ChartPaneCollections[symbol].GetPaneIndex(seriesinfo.LastChartSettings.ChartPaneName, false);
                if (paneIndex != -1)
                {
                    string key = seriesinfo.LastChartSettings.DisplayName ?? seriesinfo.Name;
                    //this._strategydata.ChartPaneCollections[symbol])[paneIndex] 
                    ChartPane pane = this._strategydata.ChartPaneCollections[symbol][paneIndex];
                    if (pane.ChartData.ChartSeriesCollection.ContainsKey(key))
                    {
                        pane.RemoveSeries(key);
                        if (((pane.ChartData.ChartSeriesCollection.Count == 0) && (pane.Name != "Price Pane")) && (pane.Name != "Volume Pane"))
                        {
                            this._strategydata.ChartPaneCollections[symbol].RemoveAt(paneIndex);
                        }
                    }
                    seriesinfo.ChartSeries = null;
                    seriesinfo.SynchronizedBarsInChart = 0;
                }
            }
        }

        /// <summary>
        /// 重新计算指标的计算先后关系
        /// </summary>
        private void CalUpdateOrder()
        {
            SeriesDependencyProcesser processer = new SeriesDependencyProcesser(true)
            {
                NameLookup = new DependencyProcessorBase.ObjectNameLookup(this.GetName)
            };
            this.updateorder.Clear();
            List<ISeries> seriesList = new List<ISeries>();
            foreach (SeriesInfo xccfbbc in this._seriesInfoMap.Values)
            {
                seriesList.Add(xccfbbc.Indicator);
            }
            foreach (ISeries series in processer.CalculateUpdateOrder(seriesList))
            {
                bool firstTime = false;
                long id = this._idmanager.GetId(series, out firstTime);
                if (firstTime)
                {
                    throw new QSQuantError("Series returned from CalculateUpdateOrder was not registered.");
                }
                this.updateorder.Add(id);
            }
        }

        /// <summary>
        /// 通过循环迭代 将series的input也同时注册到系统
        /// </summary>
        /// <param name="seriesinfo"></param>
        private void RegisterSereisInput(SeriesInfo seriesinfo)
        {
            debug(PROGRAME + ":注册 " + seriesinfo.DisplayName + "的输入数据序列");
            IIndicator indicator = seriesinfo.Indicator as IIndicator;
            if (indicator != null)
            {
                ISeries[] inputs = indicator.GetInputs();
                for (int i = 0; i < inputs.Length; i++)
                {
                    bool flag;
                    string str = seriesinfo.Name + @"\" + i;
                    if (inputs[i] == null)
                    {
                        throw new QSQuantError(string.Concat(new object[] { "Input ", i, " to indicator ", seriesinfo.Name, " was null.  You probably need to call SetInputs() for this indicator in your Startup() method, or use a constructor overload which specifies the inputs." }));
                    }
                    
                    if (inputs[i] is UserSeries)
                    {
                        throw new QSQuantError(string.Concat(new object[] { "Input ", i, " to indicator ", seriesinfo.Name, " was a user series.  User series cannot be used as inputs to indicators." }));
                    }
                    //尝试注册input 如果input=null/input不是指标/input 均返回null
                    SeriesInfo xccfbbc = this.registerCall(inputs[i], seriesinfo.Symbol, str, out flag);
                    if ((xccfbbc != null) && !flag)
                    {
                        this.RegisterSereisInput(xccfbbc);
                    }
                }
            }
        }

        private void SetSeriesInputs(SeriesInfo seriesinfo)
        {
            debug(PROGRAME +":给指标系统绑定输入数据序列");
            IIndicator indicator = seriesinfo.Indicator as IIndicator;
            if (indicator != null)
            {
                FreqKey freqKey = seriesinfo.FreqKey;//获得指标本身的freqkey
                ISeries[] inputs = indicator.GetInputs();
                bool flag = false;//是否重新setinput(如果输入频率与输出频率数据周期一致,则我们不需要重新绑定数据/如果不一致 则需要重新绑定)
                for (int i = 0; i < inputs.Length; i++)
                {
                    ISeries series = inputs[i];
                    FreqKey key2 = null;
                    SeriesInfo seriesin = this.GetSeriesInfo(series);
                   
                    bool oldValuesChange = false;

                    //如果输入Series是其他Series则获得该输入series对应的freqkey
                    if (seriesin != null)//查看该series是否是系统已经注册的series
                    {
                        debug(PROGRAME + ":Series:" + seriesinfo.DisplayName + " Input:" + seriesin.DisplayName);
                        key2 = seriesin.FreqKey;
                        //oldValuesChange = xccfbbc.Indicator.OldValuesChange;
                    }
                    //如果不是注册的Series则就是原始的Bar数据序列
                    else
                    {
                        //BarElementSeries series2 = series as BarElementSeries;
                        BarDataV2BarElementISeries series2 = series as BarDataV2BarElementISeries;
                        FrequencyBarElementSeries series3 = series as FrequencyBarElementSeries;
                        //动态的判断频率 一个是系统默认频率的 BarElementSeries 一个是带频率信息的DataSeries
                        //MessageBox.Show("it is run here");
                        if (series2 != null)
                        {
                            key2 = new FreqKey(this._strategydata.BarFrequency, series2.Secuirty);
                        }
                        else if (series3 != null)
                        {
                            //MessageBox.Show("mutil frequency");
                            key2 = new FreqKey(series3.Frequency.FrequencySettings, series3.Frequency.Symbol);
                        }
                    }

                    bool flag3 = false;
                    //如果输入数据序列与指标数据序列不一致 则需要频率转换器来实现夸周期的数据调用
                    if (!key2.Equals(freqKey))//如果输入的series的freqkey与series的freqkey不一致
                    {
                        
                        flag3 = true;
                        if (this._strategydata.SynchronizeBars && key2.Settings.Equals(freqKey.Settings))
                        {
                            flag3 = false;
                        }
                    }
                    //输入数据与输出数据频率不一致(存在跨周期调用,则我们需要注册一个频率转换器)
                    if (flag3)
                    {
                        debug(PROGRAME + ":输入与输出频率不一致,设定频率转换器");
                        //输入series的frequency(datav) 输入数据的其他频率 如5分钟 等
                        Frequency destFrequency = this._strategydata.FrequencyManager.GetFrequency(key2.Symbol, key2.Settings);
                        //输出series(本身series的freqkey) 系统主频率
                        Frequency frequency = this._strategydata.FrequencyManager.GetFrequency(freqKey.Symbol, freqKey.Settings);
                        //注册不同频率间的转换 如何从系统默认频率转换到我们需要的 其他频率 在frequency.destfrequency中加入一个5分钟对应的时间，即1分钟产生bar数据时 同步记录当前5分钟Bar所对应的时间
                        this._strategydata.FrequencyManager.RegisterFrequencyConversion(frequency, destFrequency);
                        //如果计算过后的数值 会发生变化 则需要注册一个 反向频率数据转换
                        if (oldValuesChange)
                        {
                            this._strategydata.FrequencyManager.RegisterFrequencyConversion(destFrequency, frequency);
                        }

                        inputs[i] = new FreqConvertor(series, destFrequency, frequency, this._strategydata.FrequencyManager);
                        flag = true;
                    }
                }
                if (flag)
                {
                    indicator.SetInputs(inputs);
                }
            }
        }

        /// <summary>
        /// 将对象注册到管理器
        /// </summary>
        /// <param name="registerobject"></param>
        /// <param name="symbol"></param>
        private void RegisterISeriesField(object registerobject, Security symbol)
        {
            debug(PROGRAME + ":注册Object["+symbol.Symbol+"] 的ISeries Field");
            foreach (FieldInfo info in registerobject.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(ISeries).IsAssignableFrom(info.FieldType))
                {
                    ISeries series = (ISeries)info.GetValue(registerobject);
                    this.Register(series,symbol, info.Name);
                }
            }
        }

        //获得某个register object的string名称
        private string GetName(object registerobject)
        {
            SeriesInfo xccfbbc = this.GetSeriesInfo((ISeries)registerobject);
            if (xccfbbc == null)
            {
                return null;
            }
            if (xccfbbc.Symbol == null)
            {
                return xccfbbc.Name;
            }
            return (xccfbbc.Symbol + ": " + xccfbbc.Name);
        }

        private SeriesInfo registerCall(ISeries series,Security symbol, string seriesName)
        {
            bool flag;
            return this.registerCall(series, symbol, seriesName, out flag);
        }

        //注册Series到系统将Indicator记录到 SeriesInfo 并按一定规则同步数据
        private SeriesInfo registerCall(ISeries series,Security symbol, string seriesName, out bool x73a517330bcd0eb0)
        {
            debug(PROGRAME + ":注册 ISeries[" + seriesName + "] For " + symbol.Symbol);
            bool flag;
            Func<SeriesInfo, bool> predicate = null;
            x73a517330bcd0eb0 = false;
            if (this._inited)
            {
                throw new InvalidOperationException("Cannot register an indicator after system has started running.  Name: " + seriesName);
            }
            if (series == null)
            {
                debug("Series 为null 直接返回");
                return null;
            }
            //输入可以是指标输入/用户输入数据序列等
            if (!(series is IIndicator)  && !(series is UserSeries))
            {
                debug("该ISeries不是指标 是Bar数据 直接返回");
                return null;
            }
            //获得指标信息对应的id 如果是系统没有的series则返回false 则接下来直接生成一个新的seriesinfo 第一次初始化我们不给该seriesinfo绑定indicator 在指标管理器init期间会再次调用该函数 则会得到具体的Indicator 等信息
            long id = this._idmanager.GetId(series, out flag);
            if (!flag)
            {
                debug(PROGRAME + ":生成对应的指标seriesinfo:" +seriesName);
                SeriesInfo xccfbbc = this._seriesInfoMap[id];
                if (xccfbbc.BadName && !string.IsNullOrEmpty(seriesName))
                {
                    xccfbbc.Name = seriesName;
                    xccfbbc.BadName = false;
                }
                x73a517330bcd0eb0 = true;
                return xccfbbc;//直接返回
            }
            /*
            if (series is ISystemAccess)
            {
                (series as ISystemAccess).Initialize(this._strategydata, symbol);
            }
            **/
            //如果系统已经有该Series则我们绑定对应的series / symbol(指标管理init的时候 第二次访问该函数)
            SeriesInfo data = new SeriesInfo {
                Indicator = series,
                Symbol = symbol
            };
            if (!string.IsNullOrEmpty(seriesName))
            {
                data.Name = seriesName;
                this._seriesInfoMap[id] = data;
                return data;
            }
            data.Name = series.GetType().Name;
            int num2 = 1;
            if (predicate == null)
            {
                CheckName c = new CheckName(data);
                predicate = new Func<SeriesInfo, bool>(c.Check);
            }
            while(this._seriesInfoMap.Values.Any<SeriesInfo>(predicate))
            {
                data.Name = series.GetType().Name + num2;
                num2++;
            }
            data.BadName = true;
            this._seriesInfoMap[id] = data;
            return data;
        }

        


        /// <summary>
        /// 通过iseries获得SeriesInfo信息
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        private SeriesInfo GetSeriesInfo(ISeries series)
        {
            bool flag=false;
            long num = this._idmanager.HasId(series, out flag);
            if (flag)
            {
                return null;
            }
            return this._seriesInfoMap[num];
        }

        /// <summary>
        /// 将某个seriesinfo显示到图表
        /// </summary>
        /// <param name="seriesinfo"></param>
        private void DisplaySeries(SeriesInfo seriesinfo)
        {
            debug(PROGRAME + "Display Seriesinfo via chartsetting..");
            if (seriesinfo == null)
            {
                throw new ArgumentNullException("data");
            }
            //如果ChartSeries存在 则表明已经显示到了Chart图表
            if (seriesinfo.ChartSeries != null) //xf0c3ea8f5e810e86
            {
                throw new InvalidOperationException("Series has already been added to charts.");
            }
            //获得指标本省的Chartsetting里面包含了图表显示的 显示设置
            ISeriesChartSettings chartSettings = seriesinfo.Indicator.ChartSettings;
            //如果不是主频率 则不在图表显示
            if (!this._strategydata.BarFrequency.Equals(seriesinfo.Frequency))
            {
                chartSettings.ShowInChart = false;
            }
            //如果不显示则将指标显示设置复制到SeriesInfo作为上次显示设置
            if (!chartSettings.ShowInChart)
            {
                seriesinfo.LastChartSettings = chartSettings.Clone();
                return;//不显示则直接返回
            }
            else
            {
                Security symbol = chartSettings.Symbol;
                if (symbol == null)
                {
                    symbol = seriesinfo.Symbol;
                }
                if (symbol == null)
                {
                    throw new InvalidOperationException("Symbol not specified");
                }
                debug(PROGRAME + ":指标显示的pane:" + chartSettings.ChartPaneName);
                int paneIndex = this._strategydata.ChartPaneCollections[symbol].GetPaneIndex(chartSettings.ChartPaneName, true);
                debug(PROGRAME + ":pane num:" + this._strategydata.ChartPaneCollections[symbol].Count.ToString() +" paneindex:"+paneIndex.ToString());
                //生成图表所对应的ChartDataseries
                ChartDataSeries series = new ChartDataSeries
                {
                    SeriesColor = chartSettings.Color,
                    SeriesName = seriesinfo.DisplayName,
                    LineSize = chartSettings.LineSize,
                    SeriesLineType = chartSettings.LineType
                };

                //将ChartDataSeries加入到对应的Pane
                this._strategydata.ChartPaneCollections[symbol][paneIndex].AddSeries(series.SeriesName, series);
                seriesinfo.ChartSeries = series;//seriesinfo.ChartSeries！=null 表明已经在系统进行了显示
                seriesinfo.LastChartSettings = chartSettings.Clone();
                debug(PROGRAME + ": ChartSeriesCollection num:" + this._strategydata.ChartPaneCollections[symbol][paneIndex].ChartData.ChartSeriesCollection.Count.ToString());
            }
        }

        // Nested Types
        [Serializable]
        private class SeriesFillInfo
        {
            // Fields
            
            private Color x656b37f1e9e76cb7;
           
            private IndicatorManager.SeriesInfo x6e3be5351d93107f;
            
            private IndicatorManager.SeriesInfo xe1a6ecc58a43dc92;

            // Properties
            public IndicatorManager.SeriesInfo Data1
            {
               
                get
                {
                    return this.x6e3be5351d93107f;
                }
               
                set
                {
                    this.x6e3be5351d93107f = value;
                }
            }

            public IndicatorManager.SeriesInfo Data2
            {
                
                get
                {
                    return this.xe1a6ecc58a43dc92;
                }
                
                set
                {
                    this.xe1a6ecc58a43dc92 = value;
                }
            }

            public Color FillColor
            { 
                get
                {
                    return this.x656b37f1e9e76cb7;
                }
               
                set
                {
                    this.x656b37f1e9e76cb7 = value;
                }
            }
        }

        /// <summary>
        /// 某个ISeries所对应的信息用于图表显示
        /// 这里图表显示的数据是储存在ChartDataSeries中 ChartDataSeries作为Indicator的数据中间缓存来给图表进行提供数据
        /// </summary>
        [Serializable]
        private class SeriesInfo
        {
            // Fields
            private FrequencyPlugin _frequency;
            private string _name;
            private ISeries _indicator;
            private int _synchronizedbarsinchart=0;
            private bool _badname;
            private ISeriesChartSettings _lastchartsetting;//图表设置
            private ChartDataSeries _chartseries;//储存指标数据
            private Security _symbol;

            // Properties
            public bool BadName
            {
                get
                {
                    return this._badname;
                }
                set
                {
                    this._badname = value;
                }
            }

            public ChartDataSeries ChartSeries
            {
                get
                {
                    return this._chartseries;
                }
                set
                {
                    this._chartseries = value;
                }
            }

            public string DisplayName
            {
                get
                {
                    //检查indicator是否设置里的DisplayName如果设置了则返回DisplayName 否则就是用系统默认的Name来做显示
                    return (this.Indicator.ChartSettings.DisplayName ?? this.Name);
                }
            }

            public FreqKey FreqKey
            {
                get
                {
                    return new FreqKey(this.Frequency, this.Symbol);
                }
            }

            public FrequencyPlugin Frequency
            {
                get
                {
                    return this._frequency;
                }
              
                set
                {
                    this._frequency = value;
                }
            }

            public ISeries Indicator
            {
              
                get
                {
                    return this._indicator;
                }
                
                set
                {
                    this._indicator = value;
                }
            }

            public ISeriesChartSettings LastChartSettings
            {
                
                get
                {
                    return this._lastchartsetting;
                }
                
                set
                {
                    this._lastchartsetting = value;
                }
            }

            public string Name
            {
                
                get
                {
                    return this._name;
                }
                
                set
                {
                    this._name = value;
                }
            }

            public Security Symbol
            {
               
                get
                {
                    return this._symbol;
                }
                
                set
                {
                    this._symbol = value;
                }
            }

            public int SynchronizedBarsInChart
            {
                
                get
                {
                    return this._synchronizedbarsinchart;
                }
                
                set
                {
                    this._synchronizedbarsinchart = value;
                }
            }
        }

        private sealed class CheckName
        {
        // Fields
            public SeriesInfo data;

            // Methods
            public CheckName(SeriesInfo d)
            {
                data = d;
            }
            public bool Check(SeriesInfo d)
            {
                return d.Name == data.Name;
            }
        }
        //我们在1分钟的主频策略中  计算5分钟的指标,我们计算的指标数值是按1分钟排列的。输入的数据少 是5分钟。

        internal class FreqConvertor : ISeries
        {
            // Fields
            private readonly FrequencyManager freqmanager;
            private readonly Frequency outfrequency;
            private readonly Frequency infrequency;
            private readonly ISeries inseries;

            // Methods 正向转换1分钟 ->5分钟。1分钟上计算输入1分钟的index lookbacks.然后再1分钟destionfrequency通过FreqManager超找对对应的时间然后找到对应的序号
            //输入5分钟 输出1分钟。将5分钟的数据在格式上形成1分钟数据的格式
            //                          5                      5                           1
            public FreqConvertor(ISeries innerSeries, Frequency innerFrequency, Frequency outerFrequency, FrequencyManager manager)
            {
                this.inseries = innerSeries;
                this.infrequency = innerFrequency;
                this.outfrequency = outerFrequency;
                this.freqmanager = manager;
            }
            /// <summary>
            /// 数据访问方法,在1分钟频率数据中注册一个5分钟的目标频率,当1分钟产生bar数据时候在对应的BarStartTime列表中记录下当前目标频率的barStartTime
            /// 在1分钟上以1分钟的方式调用5分钟数据的时候 通过时间映射到对应的5分钟的数值上
            /// </summary>
            /// <param name="nBars"></param>
            /// <returns></returns>
            public double LookBack(int nBars)
            {
                //转换lookbacks
                int num = this.freqmanager.ConvertLookBack(nBars, this.outfrequency, this.infrequency);
                //MessageBox.Show("转换得到的lookback:" + num.ToString());
                if ((num >= 0) && (num < this.inseries.Count))
                {
                    return this.inseries.LookBack(num);
                }
                return double.NaN;
            }

            // Properties
            public ISeriesChartSettings ChartSettings
            {
                get
                {
                    return this.inseries.ChartSettings;
                }
                set
                {
                    this.inseries.ChartSettings = value;
                }
            }

            public int Count
            {
                get
                {
                    return this.outfrequency.Bars.Count;
                }
            }
            /*
            public double Current
            {
                get
                {
                    return this.inseries.Last;
                }
            }
            **/
            public int OldestValueChanged
            {
                get
                {
                    if (this.OldValuesChange)
                    {
                        int num = 0;// this.freqmanager.ConvertLookBack(this.inseries.OldestValueChanged, this.infrequency, this.outfrequency);
                        if ((num >= 0) && (num <= this.Count))
                        {
                            return num;
                        }
                    }
                    return 0;
                }
            }

            public bool OldValuesChange
            {
                get
                {
                    return true;// this.inseries.OldValuesChange;
                }
            }
            // index    [0] [1] [2] [3] [4] count 5
            // lookback [4] [3] [2] [1] [0]
            //4:5-1  -  4
            //3:5-1  -3
            public double this[int index]
            {
                get
                {
                    int id = this.freqmanager.ConvertIndex(index, this.outfrequency, this.infrequency);
                    if (id >= 0)
                        return this.inseries[id];
                    else
                        return double.NaN;

                }
            }

            public double[] Data
            {
                get
                {
                   List<double> outdata = new List<double>();
                   for (int i = 0; i < this.Count; i++)
                   {
                       outdata.Add(this[i]);
                   }
                   return outdata.ToArray();

                }
            }
            public double Last
            {
                get
                {
                    return this.inseries.Last;
                }
            }

            public double First
            {
                get
                {
                    return this.inseries.First;
                }
            }
            
        }


    }

    
}
