using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public class MgrClientInfo:ClientInfoBase
    {

        public MgrClientInfo()
            :base()
        {
            this.ManagerID = string.Empty;
            this.mgr_fk = 0;
        }
        public string ManagerID { get; set; }

        /// <summary>
        /// 如果管理端登入成功 则会将对应的Manager绑定到该管理端对象上
        /// </summary>
        public int mgr_fk { get; set; }
    }
}
