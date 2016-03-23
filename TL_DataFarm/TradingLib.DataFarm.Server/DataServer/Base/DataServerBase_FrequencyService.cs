﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{

    public partial class DataServerBase
    {

        FrequencyService freqService;

        protected void StartFrequencyService()
        {
            freqService = new FrequencyService();


        }

        void FrequencyServiceProcessTick(Tick k)
        {
            freqService.ProcessTick(k);
        }
    }
}
