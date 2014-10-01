using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Core
{

    /// <summary>
    /// 清算中心状态Json对象 用于返回清算中心状态查询
    /// </summary>
    public class JsonWrapperClearCentreStatus
    {
        ClearCentre _clearcentre;
        public JsonWrapperClearCentreStatus(ClearCentre c)
        {
            _clearcentre = c;
        }
        public string Status { get { return _clearcentre.Status.ToString(); } }

        public bool IsOpen { get { return _clearcentre.Status == QSEnumClearCentreStatus.CCOPEN; } }
    }
}
