using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.Common.DataFarm;

using Common.Logging;


namespace TickCoreSrv
{
    class Program
    {
        static void Main(string[] args)
        {
            TickServer ts = new TickServer();

            ts.Start();
            System.Threading.Thread.Sleep(10000);
        }
    }
}
