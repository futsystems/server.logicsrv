using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.Common.DataFarm;

using Common.Logging;


namespace DataCoreSrv
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //DateTime dt = new DateTime(2011, 1, 1, 24, 0, 0);

            DataCore df = new DataCore();

            df.Start();
            System.Threading.Thread.Sleep(10000);
        }
    }
}
