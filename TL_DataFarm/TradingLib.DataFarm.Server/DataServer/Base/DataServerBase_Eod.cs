using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public partial class DataServerBase
    {

        EodDataService eodservice = null;
        public void InitEodService()
        {
            logger.Info("[Init EOD Service]");
            eodservice = new EodDataService();
        }
    }
}
