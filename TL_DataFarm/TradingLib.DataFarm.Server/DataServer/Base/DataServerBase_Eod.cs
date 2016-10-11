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
            //throw new NotImplementedException(); ?日线数据是否需要每次都去更新下该值，日线只要当天绑定一个PartialBar即可
            this.UpdateRealPartialBar(obj.Symbol, obj.EodPartialBar);
        }

        void eodservice_EodBarClose(EodBarEventArgs obj)
        {
            //throw new NotImplementedException();
            this.UpdateBar2(obj.Symbol, obj.EodPartialBar);
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
