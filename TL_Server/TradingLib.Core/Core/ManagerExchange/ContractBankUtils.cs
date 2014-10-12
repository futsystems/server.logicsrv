using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Core
{
    public static class ContractBankUtils
    {
        /// <summary>
        /// 转换成Json对象
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        public static JsonWrapperBank ToJsonWrapperBank(this ContractBank bank)
        {
            return new JsonWrapperBank 
            {
                ID = bank.ID,
                Name = bank.Name,
            };

        }
    }
}
