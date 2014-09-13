using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public sealed class SingleRunResults
    {
        // Properties
        public BarStatistic FinalStatistic { get; set; }

        public string ResultsFile { get; set; }

        //public List<RiskAssessmentResults> RiskResults { get; set; }
    }

 

}
