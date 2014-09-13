using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Common
{
    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class QBackTestReport :BackTestReportAttribute 
    {
        // Methods
        public QBackTestReport()
        {
            this.SetDefaults();
        }

        protected void SetDefaults()
        {
            base.Author = "钱波";
            base.CompanyName = "FutureClub";
            base.Version = "2013";
        }
    }


}
