using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class HandlerMixins
    {
        #region  通用检查 检查过程中不符合条件会抛出异常
        public static void Valid_ObjectNotNull(object obj)
        {
            if(obj == null)
            {
                throw new FutsRspError("请求访问对象不存在");
            }
        }
        #endregion

    }
}
