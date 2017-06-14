using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InternalSystemRunSettings
    {
        // Fields
        public Dictionary<SecurityFreq, List<Bar>> AccountInfoSymbols = new Dictionary<SecurityFreq, List<Bar>>();
        //public List<ActionInfo> Actions = new List<ActionInfo>();
        public List<SymbolSetup> AllSymbols;
        //public PortfolioXml ExistingPositions;
        public List<IndicatorInfo> Indicators = new List<IndicatorInfo>();
        public bool LiveMode;
        //public List<string> RiskAssessmentPluginIDs = new List<string>();
        public bool ShutDownWhenDone = true;
        //[NonSerialized]
        //public SynchronizationContext SynchronizationContext;
        //public List<TriggerInfo> Triggers = new List<TriggerInfo>();
        //public bool UseBrokerBuyingPower;

        public string StrategyFile { get; set; }
        // Properties
        public string OutputDir { get; set; }

        public string ProjectDir { get; set; }

        public string SystemClassName { get; set; }

        public string TradingSystemProjectPath { get;  set; }

        public string StrategySettingFriendlyName { get; set; }
    }

 

}