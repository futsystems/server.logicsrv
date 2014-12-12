﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        /// <summary>
        /// 查询系统状态
        /// </summary>
        public void ReqQrySystemStatus()
        {
            this.ReqContribRequest("MgrExchServer", "QrySystemStatus","");
        }
    }
}
