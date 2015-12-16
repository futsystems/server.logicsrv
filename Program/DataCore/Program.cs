using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.DataFarm.Common;
using Common.Logging;


namespace DataCoreSrv
{
    class Program
    {
        
        static void Main(string[] args)
        {
            DataCore df = new DataCore();

            df.Start();
            System.Threading.Thread.Sleep(10000);
        }
    }
}
