using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 开启清算中心
    /// </summary>
    public class MGRReqOpenClearCentreRequest:RequestPacket
    {
        public MGRReqOpenClearCentreRequest()
        {
            _type = MessageTypes.MGROPENCLEARCENTRE;
        }
    }

    /// <summary>
    /// 关闭清算中心
    /// </summary>
    public class MGRReqCloseClearCentreRequest : RequestPacket
    {
        public MGRReqCloseClearCentreRequest()
        {
            _type = MessageTypes.MGRCLOSECLEARCENTRE;
        }
    }



}
