using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.Common
{
    public class StrategySettingsPropertiesWrapper
    {
        StrategySetting _setting;
        public StrategySettingsPropertiesWrapper(StrategySetting setting)
        {
            _setting = setting;
        }

        [Description("策略配置名称,用于区别统一策略的不同运行配置"), Category("1.基本信息"), DisplayName("策略配置名称"),ReadOnly(true)]
        public string StrategySettingName {

            get { return _setting.FriendlyName; }
            set { }
        }
        [Description("该策略的类名称"), Category("1.基本信息"), DisplayName("策略类名"), ReadOnly(true)]
        public string AssemblyName {
            get
            { return _setting.AssemblyName; }
            set { }
        }
        [Description("保存该策略配置设置的文件名称"), Category("1.基本信息"), DisplayName("配置文件名"), ReadOnly(true)]
        public string CfgFileName {
            get { return _setting.StrategyRunDataFile; }
            set { }
        }
        [Description("程序集文件名"), Category("1.基本信息"), DisplayName("策略程序集"), ReadOnly(true)]
        public string StrategyFileName
        {
            get { return _setting.StrategyFileName; }
            set { }
        }

        [Description("加载回测数据的起始时间"), Category("2.模拟设置"), DisplayName("数据开始")]
        public DateTime DataStratDate
        {
            get
            {
                return _setting.DataStartTime;
            }
            set
            {
                _setting.DataStartTime = value;
                QuantGlobals.GDebug("save datastarttime");
                _setting.Save();
            }
        }
        [Description("开始交易的时间"), Category("2.模拟设置"), DisplayName("交易开始")]
        public DateTime TradeStartDate
        {
            get { return _setting.TradeStartTime; }
            set { _setting.TradeStartTime = value;
            _setting.Save();
            }
        }

        [Description("模拟结束时间"), Category("2.模拟设置"), DisplayName("结束时间")]
        public DateTime EndTime
        {
            get { return _setting.EndTime; }
            set { _setting.EndTime = value;
            _setting.Save();
            }
        }
        [Description("回测初始模拟资金"), Category("2.模拟设置"), DisplayName("初始权益")]
        public double StartCapital
        {
            get { return _setting.StartCapital; }
            set { _setting.StartCapital = value;
            _setting.Save();
            }
        }
        [Description("加载回测数据的起始时间"), Category("2.模拟设置"), DisplayName("LeadBars")]
        public int LeadBars
        {
            get { return 10; }
            set { }
        }
        [Description("是否用Tick对策略进行回测"), Category("2.模拟设置"), DisplayName("Tick回测")]
        public bool TickSim
        {
            get { return _setting.TickSimulation; }
            set { _setting.TickSimulation=value;
            _setting.Save();
            }
        }

        

        [Browsable(false)]
        public BarFrequency BarFrequency
        {
            get { return _setting.Frequency; }
            set { _setting.Frequency = value;
            _setting.Save();
            }
        }

       
        [Description("策略基本频率,用于向策略发送对应频率的Bar数据"), Category("3.基础设置"), DisplayName("synchbars")]
        public bool SynchBars
        {
            get { return true;}//_rundata.RunSettings.SynchronizeBars; }
            set { }           
        }
        [Description("忽略策略运行时触发的警告信息"), Category("3.基础设置"), DisplayName("忽略警告")]
        public bool IgnoreSystemWarnings
        {
            get { return _setting.IgnoreSystemWarnings; }
            set { _setting.IgnoreSystemWarnings = value;
            _setting.Save();
            }
        }
        [Description("是否按OHLC先后顺序生成模拟Tick数据"), Category("3.基础设置"), DisplayName("模拟Tick生成")]
        public bool HighBeforeLowDuringSimulation
        {
            get { return _setting.HighBeforeLowDuringSimulation; }
            set { _setting.HighBeforeLowDuringSimulation = value;
            _setting.Save();
            }
        }
        [Description("是否利用Bar数据生成模拟的Tick数据"), Category("3.基础设置"), DisplayName("Bar数据生成Tick")]
        public bool CreateTicksFromBars
        {
            get { return _setting.CreateTicksFromBars; }
            set { _setting.CreateTicksFromBars = value;
            _setting.Save();
                }
        }

        

        [Description("多少个Bar数据后自动平仓"), Category("4.仓位管理"), DisplayName("RestrictOpenOrdres")]
        public bool RestrictOpenOrders
        {
            get { return _setting.RestrictOpenOrders; }
            set { _setting.RestrictOpenOrders= value;
            _setting.Save();
            }
        }

        List<StrategyParameterInfo> _plist = new List<StrategyParameterInfo>();
        [Browsable(false)]
        public List<StrategyParameterInfo> SystemParameters
        {
            get
            {
                return _setting.SystemParameters; ;
                //return this._rundata.RunSettings.SystemParameters;
            }
            set
            {
                _setting.SystemParameters = value;
                //MessageBox.Show("保存参数");
                //this.Settings.SystemParameters = value;
                _setting.Save();
            }
        }
 

 

  
    
    }
}
