using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.DataFarm.Common;
using Common.Logging;

namespace DataFrontSrv
{
    class Program
    {
        static void Main(string[] args)
        {
            DataFront df = new DataFront();
            df.Start();
            System.Threading.Thread.Sleep(20000);

        }
    }
}
