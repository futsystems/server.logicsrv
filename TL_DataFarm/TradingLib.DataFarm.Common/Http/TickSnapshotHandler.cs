using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHttp;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.DataFarm.Common
{
   public class TickSnapshotHandler:RequestHandler
    {
        ILog logger = LogManager.GetLogger("CashHandler");


        public TickSnapshotHandler()
        {
            this.Module = "Tick";
        }

        const string ERROR_TPL_ID = "ERROR";

        public override object Process(HttpRequest request)
        {
            try
            {
                string[] path = request.Path.Split('/');
                if (path.Length < 4)
                {
                    return "Argument Error";
                }
                string exchange = path[2];
                string symbol = path[3];

                Tick snap = Global.TickTracker[exchange, symbol];
                if (snap != null)
                {
                    return snap.ToJsonNotify();
                }
                else
                {
                    return "No Tick Data";
                }
            }
            catch (Exception ex)
            {
                logger.Error("Process Request Error:" + ex.ToString());
                return "Server Side Error";
            }
        }
    }


}
