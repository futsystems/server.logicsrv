using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using Easychart.Finance;


using Easychart.Finance.DataProvider;
using Easychart.Finance.Objects;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using TradingLib.API;
using TradingLib.Quant.Base;
using TradingLib.Quant.Engine;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Chart;
using TradingLib.Quant.ChartObjects;

namespace TradingLib.Quant.GUI
{
    public partial class ChartForm : DockContent, IChartDisplay
    {

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        private ChartFormIndicatorManager chartFormIndicatorManager;//指标管理器
        private ChartFormDataManager chartFormDataManager;//数据管理器 用于向chartcontrol提供数据源
        private ChartPriceSeries chartpriceseries;//显示的价格ChartSeries该图标的主显示
        public ChartPriceSeries PriceSeries { get { return chartpriceseries; } }
        private List<ChartObject> chartobjlist;//chartobject list

        private ObjectManager objectManager;
        private ChartEasyConvert chartObjectConvert;
        //EasyChart-IChartObject的对象映射 
        private Dictionary<BaseObject, IChartObject> easyChartObjectMap = new Dictionary<BaseObject, IChartObject>();
        FML.VOLUME Volume;

        string priceName;//价格数据名
        string volumeName;//成交量数据名
        string documentname;//统一编号
        Security security;//合约
        BarFrequency frequency;//频率


        public ChartForm(SecurityFreq sf)
            :this(sf.Security,sf.Frequency)
        { 
        
        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        public ChartForm(Security sec,BarFrequency freq)
        {
            //初始化组件
            this.InitializeComponent();
            security = sec;
            frequency = freq;

            priceName = "Price Data";
            volumeName = "Volume Data";

            chartobjlist = new List<ChartObject>();//chartobj列表

            

            displayname = "Chart - " + security.Symbol;
            this.Text = displayname;
            documentname = security.Symbol + "-" + freq.ToString();//编号由symbol-freq组成的唯一编号;
            
            //Common.Chart.QSPluginManager.RegAssemblyFromMemory();//注册FML 公式
            QSPluginManager.RegExecutingAssembly();
            
            FormulaBase.ClearCache();

            //初始化ChartControl 绑定菜单和对应的事件
            InitChartControl();

            //初始化indicatorManager
            chartFormIndicatorManager = new ChartFormIndicatorManager(this);
            chartFormIndicatorManager.SendDebugEvent +=new API.DebugDelegate(debug);



            objectManager = new ObjectManager(this.WinChartControl);
            objectManager.AfterCreateStart += new ObjectEventHandler(objectManager_AfterCreateStart);
            objectManager.AfterCreateFinished+=new ObjectEventHandler(objectManager_AfterCreateFinished);
            objectManager.AfterSelect +=new ObjectEventHandler(objectManager_AfterSelect);

            objectManager.SetCanvas(this.WinChartControl);

            chartObjectConvert = new ChartEasyConvert(objectManager);
            //演示数据
            //LoadCSVFile(Environment.CurrentDirectory + "\\MSFT.CSV");
        }

        void objectManager_AfterSelect(object sender, BaseObject Object)
        {
            //throw new NotImplementedException();
        }

        void objectManager_AfterCreateFinished(object sender, BaseObject Object)
        {
            //throw new NotImplementedException();
        }

        void objectManager_AfterCreateStart(object sender, BaseObject Object)
        {
            //throw new NotImplementedException();
        }

        public void demochart()
        { 
            //IBarData bardata = new BarData(security,frequency);
            //this.DisplayPriceAndVolumeChart(bardata);
        }

        #region AddChartSeries  into ChartObject(显示区域)
        public void AddChartSeries(ChartObject chart, ChartSeries series)
        {
            this.AddChartSeries(chart, series, false);
        }
        /// <summary>
        /// 将ChartSereis添加到Chart 对chart显示对象添加formula
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="series"></param>
        /// <param name="pad"></param>
        public void AddChartSeries(ChartObject chart, ChartSeries series, bool pad)
        {

            debug("AddChartSereis 将数据序列加入到对应的chartobj:"+chart.ChartName + " series:"+series.SeriesName);
            //检查是否存在该seriesName的ChartSeries
            if (this.GetChartSeries(series.SeriesName) == null)
            {
                int num2=0;
                int padding = 0;
                if (pad && (series is ChartDataSeries))//检查是否显示 并且series是否是ChartSereis
                {
                    num2 = this.chartpriceseries.Count - series.Count;
                    padding = num2;
                    series = series as ChartSeries;
                }
                FormulaArea fa = (FormulaArea)chart.Chart;//将formula加载到对应的formulaarea
                FML.ChartSeriesFormula fb = new FML.ChartSeriesFormula(series, padding);//直接将series形成ChartSereiesFormula
                fa.AddFormula(fb);
                //fa.InsertFormula(fb);
                //if ((num2>= 0)
                {
                    chart.Pane.AddSeries(series.SeriesName, series);
                    return;
                }
            }
            //提示用户已经存在了该名称的series
            MessageBox.Show("The chart already contains a series with the name " + series.SeriesName);
        }


        /// <summary>
        /// 新增indicator 外部调用新增指标序列
        /// </summary>
        /// <param name="plugin"></param>
        public void AddIndicatorPlugin(IIndicatorPlugin plugin)
        {
            try
            {
                //获得panelist
                fmIndicatorConfig fm = new fmIndicatorConfig(plugin);
                debug("the pane num:"+chartobjlist.Count.ToString());
                foreach (ChartObject obj in this.chartobjlist)
                {
                    fm.PaneList.Add(obj.ChartName.ToString());
                }
                fm.ExistingIndicatorList.AddRange(this.chartFormIndicatorManager.GetIndicatorNames());
                if (fm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.AddIndicator(plugin, fm.Info);
                    this.ShowIndicatorProperties(fm.Info);
                }

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
            }
        }
        /// <summary>
        /// 调用全局属性窗口 查看指标属性
        /// </summary>
        /// <param name="info"></param>
        private void ShowIndicatorProperties(IndicatorInfo info)
        {
            //通过属性窗口显示指标属性
            //x7d323be108bfd078.mainForm.GetPropertiesWindow().ShowIndicatorProperties(info, this.x90854228dee82183.GetAvailableInputs(x8d3f74e5f925679c.SeriesName), new IndicatorInfo.ChangeDelegate(this.x4c5fb1ce90ae77c2), this.xbeddd4cd35c0ce74);
            QuantGlobals.Access.GetPropertiesWindow().ShowIndicatorProperties(info, this.IndicatorManager.GetAvailableInputs(info.SeriesName), new IndicatorInfo.ChangeDelegate(this.IndicatorInfoChangeHandler), "chartform");
        }
        /// <summary>
        /// 指标属性变动后的回调函数
        /// </summary>
        /// <param name="xe0292b9ed559da7d"></param>
        /// <param name="xce8d8c7e3c2c2426"></param>
        private void IndicatorInfoChangeHandler(object xe0292b9ed559da7d, IndicatorInfo.ChangeEventArgs xce8d8c7e3c2c2426)
        { 
            
        }


        public bool AddIndicator(IIndicatorPlugin plugin, IndicatorInfo info)
        {
            return this.AddIndicator(plugin, info, false);
        }
        /// <summary>
        /// 添加技术指标
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="info"></param>
        /// <param name="promptReplace"></param>
        public bool AddIndicator(IIndicatorPlugin plugin, IndicatorInfo info, bool promptReplace)
        {
            //1.通过plugin 获得对应的指标 然后创建对应的ChartIndicatorSeries
            ISeries indicator =  PluginHelper.ConstructIndicator(plugin.GetIndicatorClassName(), info.ConstructorArguments);

            //输入数据检验
            //输入input 存在性检验
            foreach (SeriesInputValue input in info.SeriesInputs)
            {
                if (input.Value is string)
                {
                    string str = (string)input.Value;
                    if (this.chartFormIndicatorManager.GetIndicatorSetup(str) == null)
                    {
                        MessageBox.Show("The indicator you are adding has the input " + input.Name + " do not exist");
                        return false;
                    }
                }
            }
            //依赖检验
            List<IndicatorInfo> inputlist = new List<IndicatorInfo>();
            foreach (string str2 in this.IndicatorManager.GetIndicatorNames())
            {
                if (str2 != info.SeriesName)
                {
                    inputlist.Add(this.IndicatorManager.GetIndicatorSetup(str2).info);
                }
            }
            //依赖关系检查
            IList<string> circularDependency = IndicatorInfo.GetCircularDependency(inputlist);
            if (circularDependency != null)
            {
                string str = "Adding this indicator would create the following circular dependency:";
                foreach (string d in circularDependency)
                {
                    str = str + "\r\n" + d;
                }
                MessageBox.Show(str);
                return false;
            }
            /*
            if (!this.RemoveIndicator(info.SeriesName, false, false))
            {
                return false;
            }**/

            //2.获得对应的ChartObject
            info.Attribute = IndicatorAttribute.GetIndicatorAttribute(indicator);

            ChartObject chartobj = FindChart(info.ChartName);//查找是否存在对应的chartobject 通过ChartName来查找对应的Pane
            debug("查找对应的pane:"+info.ChartName);
            if (chartobj == null)
            {
                chartobj = this.CreateChart(info.ChartName);//如果不存在该pane则新建一个pane
            }
            //检查chartobj中是否已经存在该SeriesName
            if (chartobj.Indicators.ContainsKey(info.SeriesName))
            {
                MessageBox.Show("This chart already contains an indicator with this name.  Select a unique name for this series.");
                return false;
            }

            //3.将ChartIndicator加入到对应的ChartObject
            chartobj.Indicators.Add(info.SeriesName, plugin);//将对应的chartobject中加入sereisname->plugin键值对
            chartobj.IndicatorArguments.Add(info.SeriesName, info.ConstructorArguments);//并将构造函数加入

            //生成对应的ChartIndicatorSeries
            ChartIndicatorSeries series = new ChartIndicatorSeries(indicator, info.LineType, info.SeriesName);
            series.SeriesColor = info.LineColor;
            series.SeriesLineType = info.LineType;
            series.LineSize = info.LineSize;

            //将chartobj series 组合并添加到Chart图表
            this.AddChartSeries(chartobj, series);//加入该ChartSeries/add indicator其实内部通过indicator创建了Indicator对应的chartsereies

            //4.将该指标加入到chartformindicatormanager中同时实现数据绑定
            this.chartFormIndicatorManager.AddSeries(info.CloneInfo(), indicator);


            //重置依赖关系
            
            this.chartFormIndicatorManager.ResetDependencies(info.SeriesName);


            //5.重新bind数据进行数据显示
            this.WinChartControl.NeedRebind();//重新绑定绘图数据

            return true;
        }

        public bool RemoveIndicator(string indicatorName, bool checkDependencies)
        {
            return this.RemoveIndicator(indicatorName, checkDependencies, true);

        }


        /// <summary>
        /// 删除指标
        /// </summary>
        /// <param name="indicatorname"></param>
        /// <param name="checkDependencies"></param>
        /// <param name="resetProperties"></param>
        public bool RemoveIndicator(string indicatorname, bool checkDependencies, bool resetProperties)
        {
            return true;
            /*
             * 
              IList<string> list;
    string str;
    ChartSeries series;
    x97792eafdaa6f99c xeafdaafc;
    if (!checkDependencies)
    {
        goto Label_01F3;
    }
    if (0 == 0)
    {
        goto Label_021D;
    }
    goto Label_006B;
Label_0017:
    this.WinChartControl.NeedRebind();
    if (0 == 0)
    {
        goto Label_003C;
    }
Label_0025:
    x7d323be108bfd078.mainForm.GetPropertiesWindow().ClearIndicatorProperties(this.xbeddd4cd35c0ce74);
    goto Label_0041;
Label_003C:
    if (resetProperties)
    {
        goto Label_0025;
    }
    if (((uint) checkDependencies) <= uint.MaxValue)
    {
        goto Label_0069;
    }
Label_0041:
    if ((((uint) resetProperties) & 0) != 0)
    {
        goto Label_003C;
    }
Label_0069:
    return true;
Label_006B:
    this.RemoveChart(xeafdaafc);
    goto Label_0017;
Label_0093:
    xeafdaafc.Indicators.Remove(series.SeriesName);
    xeafdaafc.IndicatorArguments.Remove(series.SeriesName);
    xeafdaafc.Pane.RemoveSeries(series.SeriesName);
    this.x90854228dee82183.RemoveIndicator(indicatorName);
    if (((xeafdaafc.Indicators.Count != 0) || ((0xff != 0) && (xeafdaafc.ChartName == SharedGlobals.PriceChartName))) || !(xeafdaafc.ChartName != SharedGlobals.VolumeChartName))
    {
        goto Label_0017;
    }
    goto Label_0216;
Label_015F:
    foreach (string str2 in list)
    {
        str = str + str2 + "\r\n";
    }
    str = str + "Are you sure you want to continue?";
    if (xf266856f631ec016.ShowYNConfirmationBox(str) == DialogResult.No)
    {
        return false;
    }
    foreach (string str3 in list)
    {
        if (!this.RemoveIndicator(str3, false, resetProperties))
        {
            return false;
        }
    }
Label_01F3:
    series = this.GetChartSeries(indicatorName);
    if (series != null)
    {
        string chartName = this.xcb4815ab2294a8b2(indicatorName);
        xeafdaafc = this.FindChart(chartName);
        x2a1d4f14e33a9ebc chart = (x2a1d4f14e33a9ebc) xeafdaafc.Chart;
        xf8711d01ab211905 fb = this.xca48a949e506c135(indicatorName);
        if (fb != null)
        {
            bool flag;
            chart.RemoveFormula(fb);
            if (((uint) flag) >= 0)
            {
                if (((uint) flag) >= 0)
                {
                    goto Label_0093;
                }
                goto Label_0216;
            }
        }
        else
        {
            goto Label_0093;
        }
        goto Label_015F;
    }
    return false;
Label_0216:
    if (3 != 0)
    {
        goto Label_006B;
    }
Label_021D:
    list = this.x90854228dee82183.GetDependentIndicators(indicatorName);
    if (list.Count > 0)
    {
        str = "Deleting this indicator (" + indicatorName + ") will cause the following dependent indicator(s) to also be deleted:\r\n";
        goto Label_015F;
    }
    goto Label_01F3;

             * */

        }


        #endregion


        #region ChartObject Section
        /// <summary>
        /// 通过chartName找到对应的chartobject
        /// </summary>
        /// <param name="chartName"></param>
        /// <returns></returns>
        public ChartObject FindChart(string chartName)
        {
            foreach (ChartObject obj in this.chartobjlist)
            {
                //MessageBox.Show("find chart:"+obj.ChartName);
                if (obj.ChartName == chartName)
                {
                    //MessageBox.Show("we got chart:" + chartName);
                    return obj;
                }
            }
            return null;
        }

        public ChartObject CreateChart(string chartName)
        {
            return this.CreateChart(0, -1, chartName);
        }
        public ChartObject CreateChart(int chartY, int size, string chartName)
        {
            return this.CreateChart(size, chartName, false);
        }


        public void RemoveChart(ChartObject chart)
        {
            FormulaArea fa= (FormulaArea)chart.Chart;
            WinChartControl.Chart.Areas.Remove(fa);//从控件中删除Chart
            this.chartobjlist.Remove(chart);//从列表中删除chart
        }

        /// <summary>
        /// 创建一个ChartObject
        /// </summary>
        /// <param name="size"></param>
        /// <param name="chartName"></param>
        /// <param name="abovePrices"></param>
        /// <returns></returns>
        public ChartObject CreateChart(int size, string chartName, bool abovePrices)
        {
            debug("新增加绘图区域 name:"+chartName +" size:"+size +" aboveprice:"+abovePrices.ToString());
            //新建立一个公式区域forumla area
            FormulaArea fa = new FormulaArea(WinChartControl.Chart);
            //chartobject中的Chart为显示控件中的FormulaArea
            

       
            //设定区域大小 区域大小为-1则为系统自己调节
            if (size == -1)
            {/*
                if (abovePrices)//在主图覆盖,则直接
                {
                    for (int i = 0; i < this.WinChartControl.AreaCount; i++)
                    {
                        if (this.WinChartControl.Chart.Areas[i] == this.WinChartControl.Chart.MainArea)
                        {
                            this.WinChartControl.Chart.Areas.Insert(i, fa);//找到mainarea 然后直接插入该Formula
                        }
                    }
                    return chartobj;
                }**/
                fa.HeightPercent = this.WinChartControl.Chart.MainArea.HeightPercent / 3.0;
            }
            else
            {
                fa.HeightPercent = size;
            }
            
            //如果不覆盖在价格显示
            if (!abovePrices)
            {
                //MessageBox.Show("非主图覆盖,新建区域");
                debug("非主图覆盖,新建区域");
                this.WinChartControl.Chart.Areas.Add(fa);
                
            }
            else//如果在price区域上覆盖显示
            {
                for (int i = 0; i < this.WinChartControl.AreaCount; i++)
                {
                    if (this.WinChartControl.Chart.Areas[i] == this.WinChartControl.Chart.MainArea)
                    {
                        this.WinChartControl.Chart.Areas.Insert(i, fa);//找到mainarea 然后直接插入该Formula
                    }
                }
            }
            ChartObject chartobj = new ChartObject(fa, ChartOwner.UserChart, new ChartPane());
            chartobj.ChartName = chartName;
            this.chartobjlist.Add(chartobj);//将该charobject添加到对应的chartobject list
            return chartobj;
        }

        #endregion


        //通过seriesname获得ChartSeries
        public ChartSeries GetChartSeries(string seriesName)
        {
            foreach (ChartObject obj in this.chartobjlist)
            {
                while (obj.Pane.ChartData.ChartSeriesCollection.ContainsKey(seriesName))
                {
                    return obj.Pane.ChartData.ChartSeriesCollection[seriesName];
                }
            }
            return null;
        }


        /// <summary>
        /// 显示BarList
        /// </summary>
        /// <param name="bars"></param>
        public void DisplayPriceAndVolumeChart(IBarData bardata)
        {
            ShowPrice(bardata);
            ShowVolume();
            this.WinChartControl.NeedRebind();
        }

        FML.MAIN MAIN;
        private void ShowPrice(IBarData bardata)
        {
            if (bardata.Count == 0) return;
            //查找是否存在主图区域
            ChartObject mainchart = this.FindChart(QuantGlobals.PriceChartName);
            FormulaArea chart = null;
            if (mainchart == null)
            {
                debug("新建Volume ChartObject");

                mainchart = new ChartObject(this.WinChartControl.Chart.MainArea, ChartOwner.PriceChart, new ChartPane());
                mainchart.ChartName = QuantGlobals.PriceChartName;

               // mainchart = this.CreateChart(QuantGlobals.PriceChartName);
                //mainchart.ChartOwner = ChartOwner.VolumeChart;
                chart = (FormulaArea)mainchart.Chart;

                //ChartObject chartobj = new ChartObject(fa, ChartOwner.UserChart, new ChartPane());
                //chartobj.ChartName = chartName;
                this.chartobjlist.Add(mainchart);//将该charobject添加到对
            }
            else
            {
                chart = (FormulaArea)mainchart.Chart;
                chart.RemoveFormula(MAIN);
            }
            //MessageBox.Show("add volume avabile ");
            //MessageBox.Show("total chartobj:" + chartobjlist.Count.ToString());
            MAIN = new FML.MAIN();
            chart.AddFormula(MAIN);


            //ChartObject mainchart = new ChartObject(this.WinChartControl.Chart.MainArea,ChartOwner.PriceChart,new ChartPane());
            //mainchart.ChartName = QuantGlobals.PriceChartName;
            //chartobjlist.Add(mainchart);//将ChartObject加入到chartobject list

            //MessageBox.Show("total chartobj:" + chartobjlist.Count.ToString());

            //初始化Chartcontrol
            SetChartControl();

            

            //初始化chartpriceseries 合约数据,初始数据,频率参数
            this.chartpriceseries = new ChartPriceSeries(bardata);//初始化ChartPriceSeries
            //将ChartPriceSereis的事件进行绑定 将ChartPriceSeries的事件绑定到IndicatorManager上的事件,用于驱动Indicator进行更新
            //indicator的数据驱动由ChartPriceSeries来进行
            //chartpriceseries.NewBarEvent +=new BarDelegate(chartpriceseries_NewBarEvent);

            //chartpriceseries.NewtTickEvent += new TickDelegate(chartFormDataManager.NewTick);
            //chartpriceseries.NewtTickEvent += new TickDelegate(chartFormDataManager.NewTick);
            //chartpriceseries.GotTickEvent += new TickDelegate(chartFormDataManager.NewTick);
            chartpriceseries.NewtTickEvent += new TickDelegate(chartFormIndicatorManager.NewTick);

            //初始化datamanager
            InitChartFormDataManager();

            //初始化指标管理器数据
            this.chartFormIndicatorManager.ResetBarData(bardata);//

            mainchart.Pane.SetPriceSeries(this.chartpriceseries,QuantGlobals.PriceChartName);

            //根据设置更新Volume区域
            
            
            /**
             if (0x7fffffff != 0)
    {
        do
        {
            this.x6735e4337b8b3f12(this.WinChartControl.Chart.MainArea);
            do
            {
                this.WinChartControl.StockBars = Settings.Default.BarsPerPage;
                TimeFrequency frequency = new TimeFrequency {
                    BarLength = TimeSpan.FromMinutes((double) this.x227fefe64408b240)
                };
                FrequencyPlugin sourceFrequency = frequency;
                this.xc2db61def569c1b3 = new ChartPriceSeries(this.xc1db5dbaf009ebd2, xa90af1bb0ada0f53, sourceFrequency);
                this.xc2db61def569c1b3.NewBarEvent += new Action<BarData>(this.x90854228dee82183.NewBar);
                this.xc2db61def569c1b3.NewTickEvent += new Action<BarData>(this.x90854228dee82183.NewTick);
                this.xc2db61def569c1b3.BarsReset += new Action<RList<BarData>>(this.x90854228dee82183.ResetBarData);
            }
            while (0x7fffffff == 0);
            this.x90854228dee82183.ResetBarData(xa90af1bb0ada0f53);
            this.x9297f141edc97d14();
            x97792eafdaa6f99c xeafdaafc2 = item;
            xeafdaafc2.Pane.SetPriceSeries(this.xc2db61def569c1b3, this.x1a83bdb356e260d8);
        }
        while (0 != 0);
    }

             * **/

        }

        bool showvolume = true;
        
        void ShowVolume()
        {
            if (showvolume)
            {
                ChartObject vchart = this.FindChart(QuantGlobals.VolumeChartName);
                FormulaArea chart=null;
                if (vchart == null)
                {
                    debug("新建Volume ChartObject");
                    vchart = this.CreateChart(QuantGlobals.VolumeChartName);
                    vchart.ChartOwner = ChartOwner.VolumeChart;
                    chart = (FormulaArea)vchart.Chart;

                }
                else
                {
                    chart = (FormulaArea)vchart.Chart;
                    chart.RemoveFormula(this.Volume);
                }
                //MessageBox.Show("add volume avabile ");
                //MessageBox.Show("total chartobj:" + chartobjlist.Count.ToString());
                this.Volume = new FML.VOLUME(this.PriceSeries);
                chart.AddFormula(Volume);
                
            }
        }


        void ApplyFormulaSkin(FormulaSkin skin)
        {
            skin.ShowXAxisInLastArea = false;
            skin.Colors = new Color[] { };
            skin.NameBrush.Color = Color.Aqua;
            skin.Back.BackGround.Color = Color.Aqua;
            skin.CursorPen.Color = Color.Aqua;
            skin.AxisX.MajorTick.TickPen.Color = Color.Aqua;
            skin.AxisX.MinorTick.TickPen.Color = Color.Aqua;
            skin.AxisY.MajorTick.TickPen.Color = Color.Aqua;
            skin.AxisY.MinorTick.TickPen.Color = Color.Aqua;
            skin.AxisX.MajorTick.LinePen.Color = Color.Aqua;
            skin.AxisX.MinorTick.LinePen.Color = Color.Aqua;
            skin.AxisY.MajorTick.LinePen.Color = Color.Aqua;
            skin.AxisY.MinorTick.LinePen.Color = Color.Aqua;

            skin.AxisX.LabelBrush.Color = Color.Aqua;
            skin.AxisY.LabelBrush.Color = Color.Aqua;
            skin.AxisX.Back.BackGround.Color = Color.Aqua;
            skin.AxisY.Back.BackGround.Color = Color.Aqua;
            skin.BarPens = new PenMapper[] { };
            skin.BarBrushes = new BrushMapper[] { };
            skin.Back.FrameColor = Color.Aqua;
            skin.AxisX.Back.FrameColor = Color.Aqua;
            skin.AxisY.Back.FrameColor = Color.Aqua;

            skin.AxisY.MultiplyBack.BackGround.Color = Color.Aqua;

            /*
            switch (chartStyle)
            {
                case ChartStyle.CandleStick:
                    WinChartControl.StockRenderType = StockRenderType.Candle;
                case ChartStyle.UpDownStick:
                case ChartStyle.Line:
                   WinChartControl.StockRenderType = StockRenderType.Line;
            }**/
            
        }
        /// <summary>
        /// 设置ChartControl
        /// </summary>
        void InitChartControl()
        { 
            //对ChartControl进行事件绑定
            
            WinChartControl.ContextMenu = null;
            //WinChartControl.PriceLabelFormat = "Text:xxx {CODE} :{OPEN} 5H:{HIGH} L:{LOW} C:{CLOSE} Chg:{CHANGE} ";//价格样式
            
            //WinChartControl.Skin = "GreenRed";//样式
            WinChartControl.DefaultFormulas = null;//默认公式区域

            //样式应用
            WinChartControl.IntradayInfo = new ExchangeIntraday();
            WinChartControl.MaxPrice = 0.0;
            WinChartControl.NativeContextMenu = false;
            //WinChartControl.ShowStatistic = false;
            //WinChartControl.Margin = new System.Windows.Forms.Padding(0);
            WinChartControl.ZoomPosition = Easychart.Finance.Win.ZoomCenterPosition.Right;
            WinChartControl.ViewChanged += new ViewChangedHandler(WinChartControl_ViewChanged);
            WinChartControl.NativePaint += new NativePaintHandler(WinChartControl_NativePaint);
            WinChartControl.DragEnter += new DragEventHandler(WinChartControl_DragEnter);
            WinChartControl.DragDrop += new DragEventHandler(WinChartControl_DragDrop);
            WinChartControl.AfterApplySkin += new EventHandler(WinChartControl_AfterApplySkin);
            WinChartControl.MouseUp += new MouseEventHandler(WinChartControl_MouseUp);

            WinChartControl.BeforeApplySkin += new Easychart.Finance.Win.ApplySkinHandler(WinChartControl_BeforeApplySkin);
            //WinChartControl.VerticalScroll = 
            //Easychart.Finance.Win.StatisticControl cc = new Easychart.Finance.Win.StatisticControl();
            //cc.BackColor = Color.Red;
            //WinChartControl.StatisticWindow = cc;
            
            
        }
        //在样式应用前根据我们的设定进行调整样式
        void WinChartControl_BeforeApplySkin(object sender, FormulaSkin skin)
        {
            TradingLib.Quant.Chart.ChartSetting setting = new TradingLib.Quant.Chart.ChartSetting();
            skin.Colors = new Color[] { setting.LastPriceColor};//{ Color.Blue, Color.Red, Color.Green, Color.Black, Color.Orange, Color.DarkGray, Color.DarkTurquoise };

            //公式文字标注颜色
            skin.NameBrush.Color = setting.ChartTextColor;

            //绘图区域背景颜色
            skin.Back.BackGround.Color = setting.ChartBackgroundColor;

            //十字光标颜色
            skin.CursorPen.Color = setting.CrossHairColor;

            //坐标尺上的标尺线颜色
            skin.AxisX.MajorTick.TickPen.Color = setting.TickLineColor;
            skin.AxisX.MinorTick.TickPen.Color = setting.TickLineColor;
            skin.AxisY.MajorTick.TickPen.Color = setting.TickLineColor;
            skin.AxisY.MinorTick.TickPen.Color = setting.TickLineColor;

            //背景格子线颜色
            skin.AxisX.MajorTick.LinePen.Color = setting.GridLineColor; 
            skin.AxisX.MinorTick.LinePen.Color = setting.GridLineColor;
            skin.AxisY.MajorTick.LinePen.Color = setting.GridLineColor;
            skin.AxisY.MinorTick.LinePen.Color = setting.GridLineColor;

            skin.AxisX.LabelBrush.Color = setting.LabelColor;
            skin.AxisY.LabelBrush.Color = setting.LabelColor;
            //skin.AxisY.AxisPos = AxisPos.Left;
            //skin.AxisY.ShowAsPercent = true;
            skin.AxisY.Format = "N1";
            
            //坐标面板颜色
            skin.AxisX.Back.BackGround.Color = setting.SolidFrameColor;
            skin.AxisY.Back.BackGround.Color = setting.SolidFrameColor;

            skin.BarPens = new PenMapper[] { new PenMapper(setting.CandleUpBorder), new PenMapper(setting.CandleDownBorder), new PenMapper(setting.CandleDownBorder) };
            skin.BarBrushes = new BrushMapper[] { new BrushMapper(setting.CandleUpColor), new BrushMapper(setting.CandleUpColor), new BrushMapper(setting.CandleDownColor) };
            
            
            //绘图pane的边框颜色
            //skin.Back.FrameColor = setting.ChartBorderColor;
            skin.Back.TopPen = new PenMapper(Color.Red);
            //skin.Back.LeftPen = new PenMapper(Color.Red);
            //skin.Back.BottomPen =  new PenMapper(Color.Red);

            //坐标面板边框 可分为上 下 左 右
            skin.AxisX.Back.TopPen = new PenMapper(Color.Red);
            //skin.AxisX.Back.FrameColor = setting.ChartBorderColor;
            //skin.AxisY.Back.FrameColor = setting.ChartBorderColor;
            skin.AxisY.Back.TopPen = new PenMapper(Color.Red);
            skin.AxisY.Back.LeftPen = new PenMapper(Color.Red);
            //skin.AxisY.Back.RightPen = new PenMapper(Color.Red);


            skin.AxisY.MultiplyBack.BackGround.Color = setting.MultiplierBackgroundColor;
            skin.AxisY.MultiplyBack.FrameColor = setting.MultiplierBackgroundColor;

            switch (setting.ChartStyle)
            {
                case Chart.ChartStyle.CandleStick:
                    WinChartControl.StockRenderType = StockRenderType.Candle;
                    break;
                case Chart.ChartStyle.UpDownStick:
                    WinChartControl.StockRenderType = StockRenderType.OHLCBars;
                    break;
                case Chart.ChartStyle.Line:
                    WinChartControl.StockRenderType = StockRenderType.Line;
                    break;
            }

            skin.ShowXAxisInLastArea = true;
        }
        /// <summary>
        /// 初始化chartcontrol配置
        /// </summary>
        void SetChartControl()
        {
            //WinChartControl.MouseZoomBackColor = Color.Red;

            WinChartControl.ShowCrossCursor = true;
            WinChartControl.ShowCursorLabel = true;
            WinChartControl.StockBars = 100;//chart显示的Bar数
            WinChartControl.ShowVerticalGrid = Easychart.Finance.Win.ShowLineMode.HideAll;//水平
            WinChartControl.ShowHorizontalGrid = Easychart.Finance.Win.ShowLineMode.Show;//垂直

            //WinChartControl.ShowVerticalGrid = Easychart.Finance.Win.ShowLineMode.Show;//水平
            //WinChartControl.ShowHorizontalGrid = Easychart.Finance.Win.ShowLineMode.HideAll;//垂直

            WinChartControl.ShowTopLine = true;
            WinChartControl.StickRenderType = StickRenderType.Column;
            
        }
        /// <summary>
        /// 初始化datamanager
        /// </summary>
        void InitChartFormDataManager()
        {
            this.chartFormDataManager = new ChartFormDataManager(this.chartpriceseries);
            WinChartControl.DataManager = this.chartFormDataManager;
            WinChartControl.EndTime = DateTime.MinValue;
            WinChartControl.Symbol = this._symbol;//绑定合约
            WinChartControl.CurrentDataCycle = Easychart.Finance.DataCycle.Minute;

            
        }

        void WinChartControl_DragDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void WinChartControl_MouseUp(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void WinChartControl_AfterApplySkin(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void WinChartControl_DragEnter(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void WinChartControl_NativePaint(object sender, NativePaintArgs e)
        {
            //throw new NotImplementedException();
        }

        void WinChartControl_ViewChanged(object sender, ViewChangedArgs e)
        {
            //throw new NotImplementedException();
        }

        private string _symbol;
        string _chartname=string.Empty;
        public string ChartName
        {
            get { return _chartname; }
            set { _chartname = value; }
        }

        int _barperpage;
        /// <summary>
        /// bar数
        /// </summary>
        public int BarsPerpage
        {
            get {
                return _barperpage;
                }
        }


        /// <summary>
        /// 获得新的Bar
        /// 
        /// </summary>
        /// <param name="bar"></param>
        public void NewBar(Bar bar)
        {
            //1.更新数据
            /* chartFormDataManager.newtick会调用其内部ChartPriceSeries的newtick.
             * chartPriceSeries->bargenerator来通过一定的方法生成Bar数据
             * chartPriceSeries通过事件绑定通知了indicatorManager来驱动指标计算
             * */
            if (chartFormDataManager != null)
            {
                chartFormDataManager.NewBar(bar);
            }

            //2.更新图表
            RefreshChart();


        
        }

        int BarsPerPage = 200;
        /// <summary>
        /// 计算一个显示参数 
        /// </summary>
        void ChartCalc()
        {
            if (WinChartControl.Chart.DataProvider != null)
            {
                int totalbars = WinChartControl.Chart.GetTotalBars();
                if (IsChartTime())
                {
                    if (FirstBarVisible() || totalbars > BarsPerpage)
                    {
                        //int num2 = ChartControl.Chart.DateToIndex(chartFormDataManager.Bars.Bars.Current.BarStartTime);
                        int num3 = WinChartControl.Chart.DateToIndex(WinChartControl.EndTime);

                        //int diff = num3 - num2;
                    
                    }
                    
                }
            }
        }

        bool FirstBarVisible()
        {
            DateTime maxValue = DateTime.MaxValue;
            if (chartFormDataManager.BarData.Count > 0)
            {
                //maxValue = this.chartFormDataManager.Bars[0].BarStartTime;
            }
            return false;
        }
         bool IsChartTime()
        {
            DateTime minValue = DateTime.MinValue;
            if (this.chartFormDataManager.BarData.Count > 0)
            {
                //minValue = this.chartFormDataManager.BarData.Current.BarStartTime;
                return (this.WinChartControl.EndTime >= minValue);
            }
            return false;
        }


        DateTime lasttime;
        void RefreshChart()
        {
            lasttime = DateTime.Now;
            WinChartControl.NeedRefresh();
            WinChartControl.NeedRebind();
            
        }
        /// <summary>
        /// 获得新的Tick
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        { 
        
        }
       
        int _interval;

        /// <summary>
        /// 周期
        /// </summary>
        public BarInterval DataFrequency
        {
            get
            {
                return BarInterval.CustomTicks;
            }
        }
        /// <summary>
        /// 指标管理器
        /// </summary>
        public ChartFormIndicatorManager IndicatorManager
        {
            get
            {
                return this.chartFormIndicatorManager;
            }
        }


        /// <summary>
        /// 从数据中心加载Bar
        /// </summary>
        /// <returns></returns>
        public QList<Bar> LoadBars()
        {
            //return x7d323be108bfd078.mainForm.GetBarDataStoragePlugin().LoadBars(new SymbolFreq(this.xc1db5dbaf009ebd2, this.x227fefe64408b240), this.WinChartControl.StartTime, this.WinChartControl.EndTime, -1, false);

            return new QList<Bar>();
        }

        /// <summary>
        /// 获得现实的名称
        /// </summary>
        string displayname;
        public string DisplayName
        {
            get
            {
                return this.displayname;
            }
            set
            {
                this.displayname = value;
            }
        }

        #region 功能函数
        private void LookBackChart(int nb)
        {
            if (nb < this.WinChartControl.StockBars)
            {
                nb = this.WinChartControl.StockBars;
            }
            if (nb >= this.PriceSeries.PriceData.Count)
            {
                nb = this.PriceSeries.PriceData.Count - 1;
            }
            int nBars = nb - this.WinChartControl.StockBars;
            if (nBars < 0)
            {
                nBars = 0;
            }
            this.WinChartControl.StartTime = this.PriceSeries.PriceData.LookBack(nb).BarStartTime;
            this.WinChartControl.EndTime = this.PriceSeries.PriceData.LookBack(nBars).BarStartTime;
        }




        #endregion




        #region IChartDisplay

        /// <summary>
        /// 绑定交易模型生成的数据集ChartDataSeries
        /// </summary>
        /// <param name="chartPanes"></param>
        public void SetUserChartCollection(List<ChartPane> chartPanes)
        {
            //this.xef45523e3219bc5e();
            //this.xedad721086606993();
           
            ChartObject chartobj;
            //遍历所有的ChartPane 
            foreach (ChartPane pane in chartPanes)
            {
                //QuantGlobals.GDebug("it is run in Chart form here:" + pane.Name);
                chartobj = this.FindChart(pane.Name);
                if(chartobj == null)//如果没有找到对应的ChartPane则新建一个ChartPane
                {
                    chartobj = this.CreateChart(pane.Size, pane.Name, pane.AbovePrices);
                }
                if (chartobj == null)
                    throw new QSQuantError("无法创建或获得对应的ChartPane");

                //QuantGlobals.GDebug("ChartSeriesCollection :" + pane.ChartData.ChartSeriesCollection.Count.ToString());
                //遍历ChartPane中的所有ChartSeries数据
                foreach (ChartSeries series in pane.ChartData.ChartSeriesCollection.Values)
                {
                    try
                    {
                    QuantGlobals.GDebug("xxxxxxxxxxxxxxxxxxxfind the Chartobj :" + series.SeriesName + " "+   pane.ChartData.LinkedIndicators.Count.ToString() + "  "  +series.Count.ToString());
                    

                        this.AddChartSeries(chartobj, series);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    if (pane.ChartData.LinkedIndicators.Count==0)
                    {
                        debug("pane.ChartData.LinkedIndicators.Count == 0 ");
                        //this.AddChartSeries(chartobj, series);
                    }
                    else
                    {
                        if (pane.ChartData.LinkedIndicators.ContainsKey(series.SeriesName))
                        {
                            //pair = pane.ChartData.LinkedIndicators[series.SeriesName];
                            //series2 = pane.ChartData.ChartSeriesCollection[pair.Key];
                            //Color fillColor = pair.Value;
                            //this.AddLinkedSeries(xeafdaafc, series, series2, fillColor);
                            //this.AddChartSeries


                        }
                        else
                        {
                            this.AddChartSeries(chartobj, series);
                        }
                    }
                    //this.WinChartControl.NeedRefresh();
                    this.WinChartControl.NeedRebind();

                }
                foreach (ChartPointAttributes attributes in pane.ChartData.BackgroundAttributes)
                {
                    ChartPoint leftPoint = new ChartPoint(attributes.StartBar.BarStartTime, 0.0);
                    ChartPoint rightPoint = new ChartPoint(attributes.EndBar.BarStartTime, 0.0);
                    //ChartRectangleBand chartObject = new ChartRectangleBand(leftPoint, rightPoint, attributes.BackgroundColor);
                    //while (this.xe09c38a457ec490c == null)
                    //{
                    //    this.xe09c38a457ec490c = new x7754386d75d9075d(this.xbc02e26d3d5cb9e1);
                    //    break;
                    //}
                    // this.xe09c38a457ec490c.ConstructChartObject(chartObject, xeafdaafc);
                }
                //Dictionary<Color, ColorBar> dictionary = new Dictionary<Color, ColorBar>();

                foreach (ChartPointAttributes cpa in pane.ChartData.ForegroundAttributes)
                {
                    /*
                    ColorBar bar = null;
                    if (!dictionary.ContainsKey(current.ForegroundColor))
                    {
                        bar = new ColorBar(current.ForegroundColor);
                        dictionary.Add(current.ForegroundColor, bar);
                        ((x2a1d4f14e33a9ebc) xeafdaafc.Chart).AddFormula(bar);
                    }
                    else
                    {
                        bar = dictionary[current.ForegroundColor];
                    }
                    bar.AddColorBar(current.StartBar.BarStartTime);
                    **/
                }
                //显示文字
                foreach (ChartPointAttributes testpoint in pane.ChartData.TextAttributes)
                {

                }
            }
        }

        /// <summary>
        /// 将ChartScroll到特定的日期
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="offset"></param>
        public void ScrollChart(DateTime dateTime, int offset)
        {
            //二分法查找日期序列中的日期 然后将图标移置该位置
        }
        /// <summary>
        /// 获得每页显示的Bar数
        /// </summary>
        /// <returns></returns>
        public int GetBarsPerPage()
        {
            return this.BarsPerpage;
        }
        /// <summary>
        /// 设定每页显示多少个Bar
        /// </summary>
        /// <param name="nBars"></param>
        public void SetBarsPerPage(int nBars)
        {
            int num;
            DateTime barStartTime = this.chartFormDataManager.BarStartTime;
            if (this.WinChartControl.EndTime > barStartTime)
            {
                num = this.chartFormDataManager.BarData.Count - 1;
                this.WinChartControl.EndTime = barStartTime;
            }
            else
            {
                num = this.WinChartControl.Chart.DateToIndex(this.WinChartControl.EndTime);
            }
            int i = (num - nBars) + 1;
            if (i < 0)
            {
                i = 0;
            }
            DateTime time2 = this.WinChartControl.Chart.IndexToDate(i);
            bool flag1 = time2 == DateTime.MaxValue;
            this.WinChartControl.StartTime = time2;
        }


        string getTradeLabel(Trade f,DateTime bartime)
        {
            string op = LibUtil.GetEnumDescription(f.PositionOperation);
            string direction = GetTradeDirection(f).ToString();
            return op + "[" + direction + "] @" + f.xprice.ToString("F1");// +" |" + this.WinChartControl.Chart.DateToIndex(bartime);
        }

        SignalChartDirection GetTradeDirection(Trade f)
        { 
            
            //if(f.side && f.PositionOperation == API.QSEnumPosOperation.EntryPosition || f.PositionOperation == API.QSEnumPosOperation.)
            bool lcon1 = f.side && (f.PositionOperation == API.QSEnumPosOperation.EntryPosition || f.PositionOperation == API.QSEnumPosOperation.AddPosition);//买 开仓 加仓 就为多头
            bool lcon2 = (!f.side) && (f.PositionOperation == API.QSEnumPosOperation.ExitPosition || f.PositionOperation == API.QSEnumPosOperation.DelPosition);// 卖 平仓 减仓 就为多
            return (lcon1 || lcon2) ? SignalChartDirection.Long :SignalChartDirection.Short;   
        }

        SignalChartOperation GetTradeOperation(Trade f)
        {
            //建仓
            if (f.PositionOperation == API.QSEnumPosOperation.EntryPosition || f.PositionOperation == API.QSEnumPosOperation.AddPosition)
            {
                return SignalChartOperation.Entry;
            }
            else
                return SignalChartOperation.Exit;
        }

        double GetTradeLocation(Trade trade,DateTime bartime)
        {
            Bar b = PriceSeries.PriceData[bartime];
            if (b == null) return (double)trade.xprice;
            return (double)(trade.side ? b.Low : b.High);

            
        }
        public void AddTrade(Trade trade)
        {
            QuantGlobals.GDebug("加入trade到图表:" + trade.ToString());
            DateTime tradetime = Util.ToDateTime(trade.xdate, trade.xtime);//成交发生的时间
            DateTime bartime = TimeFrequency.RoundTime(tradetime,frequency.TimeSpan);


            ChartSignal signal = new ChartSignal(new ChartPoint(bartime,GetTradeLocation(trade,bartime)),GetTradeDirection(trade),GetTradeOperation(trade),trade.UnsignedSize.ToString() +"*"+trade.xprice.ToString("F1"));

            

            //ChartLabel chartlabel = new ChartLabel(new ChartPoint(bartime,(double)trade.xprice), getTradeLabel(trade,bartime));
            //chartlabel.LabelAlignment = ChartLabelAlignment.RightTop;

            ChartObject chartinfo =  this.FindChart(QuantGlobals.PriceChartName);//获得price pane绘图区域

            //BaseObject key = chartObjectConvert.ConstructChartObject(chartlabel, chartinfo);//利用chartlabel生成Easychart Object
            //this.easyChartObjectMap.Add(key,chartlabel);

            BaseObject skey = chartObjectConvert.ConstructChartObject(signal, chartinfo);
            this.easyChartObjectMap.Add(skey, signal);


        }

        public void AddPositoinRound(PositionRound pr)
        {
            DateTime entry = TimeFrequency.RoundTime(pr.EntryTime, frequency.TimeSpan);
            DateTime exit = TimeFrequency.RoundTime(pr.ExitTime, frequency.TimeSpan);

            ChartLine line = new ChartLine(new ChartPoint(entry, (double)pr.EntryPrice), new ChartPoint(exit, (double)pr.ExitPrice), pr.WL ? Color.DarkOrange : Color.Cyan);
            line.SetDashStyle(System.Drawing.Drawing2D.DashStyle.Dash);

            ChartObject chartinfo = this.FindChart(QuantGlobals.PriceChartName);//获得price pane绘图区域
            BaseObject key = chartObjectConvert.ConstructChartObject(line, chartinfo);

            this.easyChartObjectMap.Add(key, line);

        }
        /// <summary>
        /// 绑定PositionRound数据 用于显示持仓回合数据
        /// </summary>
        /// <param name="prs"></param>
        public void SetPRData(List<PositionRound> prs)
        {
            foreach (PositionRound pr in prs)
            {
                this.AddPositoinRound(pr);
            }
        }
        public void SetTradeData(List<Trade> trades)
        {
            foreach (Trade info in trades)
            {
                this.AddTrade(info);
            }
        }


        public void SetChartObjects(List<IChartObject> chartObjects)
        {
            if (chartObjects != null)
            {
                ChartObject pane = null;
                foreach (IChartObject obj in chartObjects)
                {
                    pane = this.FindChart(obj.GetChartPane());
                    if (pane != null)
                    {
                        BaseObject key = chartObjectConvert.ConstructChartObject(obj, pane);
                        this.easyChartObjectMap.Add(key,obj);
                    }
                    
                }
            }
        }

 

 






 


        #endregion


    }
}
