using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        /// <summary>
        /// 是否登入
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 回话信息
        /// </summary>
        public string SessionInfo { get; set; }
    }
}
