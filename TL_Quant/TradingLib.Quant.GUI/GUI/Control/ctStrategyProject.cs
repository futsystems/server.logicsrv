using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;

using System.Threading;
using TradingLib.Common;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;

using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;
using TradingLib.Quant.Engine;

namespace TradingLib.Quant.GUI
{
    /// <summary>
    /// 策略配置核心组件,列出了当前StragetManger中可用的策略服务 背后数据组件为StrategyManager
    /// </summary>
    public partial class ctStrategyProject : UserControl
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if(SendDebugEvent!=null)
                SendDebugEvent(msg);
            //QuantGlobals.GDebug(msg);
        }
        StrategyManager _strategymanager;
        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();
        const string FRIENDLYNAME = "策略配置名";
        const string STRATEGYNAME = "策略类名";
        const string ASSEMBLYNAME = "程序集文件";
        const string LIVEDATA = "实时数据";
        const string HISTDATA = "历史数据";
        const string BROKER = "成交接口";
        const string RUNMODE = "运行模式";

        public ctStrategyProject()
        {
            InitializeComponent();
            this.Load += new EventHandler(ctStrategyProject_Load);
        }

        /// <summary>
        /// 初始化显示表格
        /// </summary>
        void InitGrid()
        {
            gt.Columns.Add(FRIENDLYNAME);
            gt.Columns.Add(STRATEGYNAME);
            gt.Columns.Add(ASSEMBLYNAME);
            gt.Columns.Add(LIVEDATA);
            gt.Columns.Add(HISTDATA);
            gt.Columns.Add(BROKER);
            gt.Columns.Add(RUNMODE);


            datasource.DataSource = gt;
            datasource.Sort = FRIENDLYNAME + " ASC";
            strategyGrid.DataSource = datasource;

            for (int i = 0; i < gt.Columns.Count; i++)
            {
                strategyGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        void ctStrategyProject_Load(object sender, EventArgs e)
        {
            try
            {
                //初始化strategymanager
                _strategymanager = QuantGlobals.Access.GetStrategyManager();//new StrategyManager(SynchronizationContext.Current, QuantGlobals.AppDataDirectory, QuantGlobals.StrategyDirectory, false);
                _strategymanager.AddStrategySetupEvent += new StrategySetupDel(_strategymanager_AddStrategySetupEvent);
                //初始化表格
                InitGrid();

                //初始化菜单
                InitMenu();
                //将加载的策略配置填充到表格
                List<StrategySetup> loaded = _strategymanager.GetLoadedStrategyProject();//获得加载的配置策略
                //MessageBox.Show("加载策略:" + loaded.Count.ToString());
                foreach (StrategySetup ss in loaded)
                {
                    this.GotStrategy(ss);
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
            }
        }

        private int CurrentRow { get { return (strategyGrid.SelectedRows.Count > 0 ? strategyGrid.SelectedRows[0].Index : -1); } }

        private StrategySetup CurrentStrategySetup
        {
            get
            {
                int index = CurrentRow;
                string friendlyname = strategyGrid.Rows[index].Cells[FRIENDLYNAME].Value.ToString();

                //MessageBox.Show("current friendly name:" + friendlyname);
                return _strategymanager.GetStrategySetup(friendlyname);
            }
        }
        private string CurrentStrategyName
        {
            get {
                int index = CurrentRow;
                string friendlyname = strategyGrid.Rows[index].Cells[FRIENDLYNAME].Value.ToString();
                return friendlyname;
            }
        }


        


        void InitMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("编辑策略配置",(Image)Resource.editstrategy, new EventHandler(ShowStrategyProjectProperties));
            menu.Items.Add("删除策略配置",(Image)Resource.delete, new EventHandler(DelStrategySetup));
            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            menu.Items.Add("回测策略", (Image)Resource.simulation, new EventHandler(BackTestStrategy));
            menu.Items.Add("优化策略", (Image)Resource.optimization, new EventHandler(OptStrategy));
            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            menu.Items.Add("运行策略", (Image)Resource.started, new EventHandler(demohandler));
            menu.Items.Add("停止策略", (Image)Resource.stopped, new EventHandler(demohandler));
            strategyGrid.ContextMenuStrip = menu;

        }

        /// <summary>
        /// 优化策略参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OptStrategy(object sender, EventArgs e)
        {

            //1.生成当前策略配置对应的rundata
            SharedSystemRunData _rundata = GenerateRunData(CurrentStrategyName);

            //2.生成tradingmodule的参数
            TradingModuleWrapperArgs args;
            ServiceAppDomainFactory factory = new ServiceManagerAppDomainFactory(QuantGlobals.Access.GetServiceManager().GetServiceSetup(QuantGlobals.PaperBroker), QuantGlobals.UserAppDataPath, QuantGlobals.PluginDirectory);
            //交易模块wrapper参数(将所有回测需要的数据生成args传递)
            args = new TradingModuleWrapperArgs();
            //args.Acccess = QuantGlobals.Access;
            args.BrokerFactoryFactory = factory;//Broker
            args.DataStoreSettings = QuantGlobals.DataStoreSetting;//数据读写插件
            args.SystemClassName = _rundata.InternalSettings.SystemClassName;//策略类名
            args.SystemFilename = _rundata.InternalSettings.StrategyFile;//策略文件

            //3.获得当前策略配置文件
            StrategySetting setting = GetStrategySetting(CurrentStrategyName);

            //4.生成策略优化运行器
            OptimizationRunner runner = new OptimizationRunner(_rundata, args, QuantGlobals.OptimizationSetting, setting.SystemParameters, null);// optimizationFile);
            runner.SendDebugEvent +=new DebugDelegate(debug);

            //5.运行策略优化,得到策略优化结果
            List<OptimizationResult>  result = runner.RunOptimization();
            if (result != null)
            {
                OptimizationReport report = new OptimizationReport();
                report.ShowOptimResult(result);
                report.Show();
            }

        }

        /// <summary>
        /// 回测策略
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackTestStrategy(object sender, EventArgs e)
        {
            //SharedSystemRunData rundata = getRunData(CurrentStrategySetup);

            ctStrategySimulation ct = new ctStrategySimulation(GenerateRunData(CurrentStrategyName));
            ct.SendFinishEvent += new StrategySimulationDel(Sim_SendFinishEvent);
            InsertSimulationControl(ct);


        }
        /// <summary>
        /// 策略回测完毕后可以通过回测时候所探测到的策略参数进行二次加载
        /// 第一次运行策略系统参数是不知道的，在运行过程中会提示我们输入默认参数
        /// 第二次运行的时候系统就知道该策略的默认参数了
        /// </summary>
        /// <param name="control"></param>
        void Sim_SendFinishEvent(ctStrategySimulation control)
        {
            //MessageBox.Show("回测完成。补充策略参数");
            string fname = control.StrategySetupFriendlyName;
            StrategySetting setting = GetStrategySetting(fname);
            foreach (string key in control.Result.Data.StrategyParameters.Keys)
            { 
                if(!this.ContainParameter(setting,key))
                {
                    StrategyParameterInfo si = new StrategyParameterInfo(key, control.Result.Data.StrategyParameters[key]);
                    setting.SystemParameters.Add(si);
                    }
            }
            setting.Save();

        }

        bool ContainParameter(StrategySetting setting, string name)
        {
            foreach (StrategyParameterInfo s in setting.SystemParameters)
            {
                if (s.Name == name)
                    return true;
            }
            return false;
        }



        void InsertSimulationControl(ctStrategySimulation control)
        {
            KryptonPage page = new KryptonPage();
            page.TextTitle = control.StrategySetupFriendlyName;
            page.Text = control.StrategySetupFriendlyName;
            page.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            tabholder.Pages.Add(page);
            tabholder.SelectedPage = page;
            
        }
        /// <summary>
        /// 通过setup来生成策略运行时的运行配置数据
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        SharedSystemRunData GenerateRunData(string currentstname)
        {
            StrategySetting setting = GetStrategySetting(currentstname);//加载策略配置文件

            SharedSystemRunData data = new SharedSystemRunData();

            data.RunSettings = new StrategyRunSettings();
            data.RunSettings.AccountCurrency = CurrencyType.RMB;

            data.RunSettings.CreateTicksFromBars = false;
            data.RunSettings.DataStartDate = setting.DataStartTime;
            data.RunSettings.EndDate = setting.EndTime;
            data.RunSettings.HighBeforeLowDuringSimulation = setting.HighBeforeLowDuringSimulation;
            data.RunSettings.IgnoreSystemWarnings = setting.IgnoreSystemWarnings;
            data.RunSettings.LeadBars = 20;
            data.RunSettings.MaxOpenPositions = 10;
            data.RunSettings.MaxOpenPositionsPerSymbol = 2;

            data.RunSettings.RestrictOpenOrders = setting.RestrictOpenOrders;
            data.RunSettings.RunNumber = 10;
            data.RunSettings.SaveOptimizationResults = true;
            data.RunSettings.StartingCapital = setting.StartCapital;
            
            data.RunSettings.SynchronizeBars = true;
            
            data.RunSettings.TradeStartDate = setting.TradeStartTime;
            data.RunSettings.UseTicksForSimulation = setting.TickSimulation;


            //data.RunSettings.StopLoss = 2000;
            //生成模拟的sybol设置以及以及策略参数设置
            List<SymbolSetup> sl = new List<SymbolSetup>();
            Security security = new SecurityImpl("IF");
            security.Margin = 0.1M;
            security.Multiple = 300;
            security.PriceTick = 0.2M;
            //security.EntryCommission = 100;
            //security.ExitCommission = 100;
            //security.Type = SecurityType.FUT;


            sl.Add(new SymbolSetup(security, BarFrequency.ThreeMin));


            //KeyValuePair<string, double> sp = new KeyValuePair<string, double>("demo", 2000);
            //List<KeyValuePair<string, double>> splist = new List<KeyValuePair<string, double>>();
            //splist.Add(sp);
            //这里需要修正 从watchlist中选择对应的symbolsetup
            data.RunSettings.Symbols = sl;
            //data.RunSettings.SystemParameters = splist;

            //设定一些初始化运行配置
            data.InternalSettings = new InternalSystemRunSettings();
            data.InternalSettings.TradingSystemProjectPath = Path.Combine(new string[] { QuantGlobals.ProjectConfigDirectory, setting.FriendlyName });
            data.InternalSettings.SystemClassName = setting.AssemblyName;
            data.InternalSettings.ProjectDir = data.InternalSettings.TradingSystemProjectPath;
            data.InternalSettings.OutputDir = data.InternalSettings.TradingSystemProjectPath;
            data.InternalSettings.StrategyFile = setting.StrategyFileName;
            data.InternalSettings.StrategySettingFriendlyName = setting.FriendlyName;
            data.InternalSettings.LiveMode = false;
            data.InternalSettings.ShutDownWhenDone = true;

            
            //传递策略参数
            Dictionary<string, double> dic = new Dictionary<string, double>();
            List<KeyValuePair<string, double>> klist = new List<KeyValuePair<string, double>>();
            foreach (StrategyParameterInfo info in setting.SystemParameters)
            {
                if (!dic.Keys.Contains(info.Name))
                {
                    //dic.Add(info.Name, info.Value);
                    klist.Add(new KeyValuePair<string, double>(info.Name, info.Value));
                }
                else
                {
                    fmConfirm.Show("策略参数含有同名参数,请检查配置");
                }
            }
            //将设定的策略参数传递给runsetting,然后策略在策略逻辑里面就可以进行引用了
            data.RunSettings.StrategyParameters = klist;

            //传递主频率信息
            PluginSettings ts = EngineHelper.CreateTimeFrequencySettings(setting.Frequency);
            data.SelectedSystemFrequency = ts;

            return data;

        }

        string _laststrategyname=null;
        /// <summary>
        /// 显示策略配置信息 用于修改相应设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShowStrategyProjectProperties(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_laststrategyname) && _laststrategyname != CurrentStrategyName)
            {
                GetStrategySetting(_laststrategyname).Save();
            }

            _laststrategyname = CurrentStrategyName;
            StrategySetting setting = GetStrategySetting(CurrentStrategyName);
            QuantGlobals.Access.GetPropertiesWindow().ShowStrategyProjectProperties(setting);//显示策略配置文件
        }

        Dictionary<string, StrategySetting> settingmap = new Dictionary<string, StrategySetting>();

        StrategySetting GetStrategySetting(string name)
        {
            if (settingmap.Keys.Contains(name))
            {
                return settingmap[name];
            }
            else
            {
                StrategySetup s  =  _strategymanager.GetStrategySetup(name);
                StrategySetting setting = StrategySetting.Load(s);//加载策略配置文件
                settingmap.Add(name, setting);
                //MessageBox.Show("Param num:" + setting.SystemParameters.Count.ToString());
                return setting;

            }
        }


        Dictionary<string, SharedSystemRunData> rundataMap = new Dictionary<string, SharedSystemRunData>();
        //获得当前security的配置文件
        SharedSystemRunData getRunData(StrategySetup s)
        {
            if (rundataMap.Keys.Contains(s.FriendlyName))
                return rundataMap[s.FriendlyName];
            else
            {
                string filename = Path.Combine(QuantGlobals.AppDataDirectory,"");
                //MessageBox.Show("filename:" + filename.ToString());
                SharedSystemRunData data = null;
                if (File.Exists(filename))
                {

                    data = SharedSystemRunData.ReadRunDataFile(filename);
                }
                else
                {
                    //MessageBox.Show("不存在该文件");
                    if (SharedSystemRunData.WriteDefaultRunDataFile(filename))
                        data = SharedSystemRunData.ReadRunDataFile(filename);
                }
                if (data != null)
                {
                    rundataMap.Add(s.FriendlyName, data);
                }
                //MessageBox.Show(data.RunSettings.AccountCurrency.ToString());
                return data;
            }
        }
        


        /// <summary>
        /// 删除策略菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DelStrategySetup(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("current row:" + CurrentRow.ToString());
                StrategySetup setup = CurrentStrategySetup;
                if (setup != null)
                {
                    fmConfirm fm = new fmConfirm();
                    fm.Message = "确认删除策略配置:" + setup.FriendlyName + "[" + setup.StrategyClassName + "]";
                    if (fm.ShowDialog() == DialogResult.OK)
                    {
                        //MessageBox.Show("remove at here");
                        bool re = _strategymanager.RemoveStrategyProject(setup.FriendlyName);
                        if (re)
                            gt.Rows.RemoveAt(DataTableIndex(setup.FriendlyName));
                        else
                        { 
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        void demohandler(object sender, EventArgs e)
        { 
            
        }
        

        void _strategymanager_AddStrategySetupEvent(StrategySetup setup)
        {
            this.GotStrategy(setup);
        }

        
        void GotStrategy(StrategySetup setup)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new StrategySetupDel(GotStrategy), new object[] { setup });
                }
                catch (Exception ex)
                {
                    debug("刷新界面处错:" + ex.ToString());
                }
            }
            else
            {
                string name = setup.FriendlyName;
                int rid = DataTableIndex(name);
                //MessageBox.Show("rowid:" + rid.ToString());
                if (rid < 0)
                {
                    StrategyInfo si = _strategymanager.GetStrategyInfo(setup.FriendlyName);
                    gt.Rows.Add(new object[] { setup.FriendlyName, si.StrategyClassName, si.FileName,"","","", "sim" });
                    //MessageBox.Show("load straegy:" + setup.FriendlyName);
                    setupmap.Add(name, setup);
                    //setuprowIDMap.Add(name, gt.Rows.Count - 1);
                }
                else
                {
                    gt.Rows[rid][RUNMODE] = "testlive";
                }
            }
            

            
        }

        Dictionary<string, StrategySetup> setupmap = new Dictionary<string, StrategySetup>();

        int DataTableIndex(string friendlyname)
        {

            for (int i = 0; i < gt.Rows.Count; i++)
            {
                if (gt.Rows[i][FRIENDLYNAME].ToString() == friendlyname)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 双击策略列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void strategyGrid_DoubleClick(object sender, EventArgs e)
        {
            this.ShowStrategyProjectProperties(null, null);
        }

        

    }
}
