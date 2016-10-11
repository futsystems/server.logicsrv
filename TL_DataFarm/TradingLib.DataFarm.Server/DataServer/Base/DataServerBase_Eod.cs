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
            eodservice = new EodDataService(GetHistDataSotre());
            eodservice.EodBarResotred += new Action<Symbol, IEnumerable<BarImpl>>(eodservice_EodBarResotred);
            eodservice.EodBarClose += new Action<EodBarEventArgs>(eodservice_EodBarClose);
            eodservice.EodBarUpdate += new Action<EodBarEventArgs>(eodservice_EodBarUpdate);
        }

        void eodservice_EodBarUpdate(EodBarEventArgs obj)
        {
            //throw new NotImplementedException();
        }

        void eodservice_EodBarClose(EodBarEventArgs obj)
        {
            //throw new NotImplementedException();
        }

        void eodservice_EodBarResotred(Symbol arg1, IEnumerable<BarImpl> arg2)
        {

            foreach (var bar in arg2)
            {
                this.UpdateBar2(arg1, bar);
            }
            
        }
    }
}
