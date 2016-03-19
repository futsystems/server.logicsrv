using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 二元期权合约与报价引擎
    /// 用于定时生成二元期权同时按一定的规则生成对应的报价和参数
    /// </summary>
    public class BinaryOptionQuoteEngine
    {
        ConcurrentDictionary<string, BinaryOption> optionmap = new ConcurrentDictionary<string, BinaryOption>();


        /// <summary>
        /// 返回所有二元期权
        /// </summary>
        public IEnumerable<BinaryOption> BinaryOptions
        {
            get 
            {
                long now  = Util.ToTLDateTime();    
                return optionmap.Values.Where(o=>o.IsExpired(now));
            }
        }

        /// <summary>
        /// 获得某种类别的二元期权
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<BinaryOption> GetBinaryOptions(EnumBinaryOptionType type)
        {
            return BinaryOptions.Where(o => o.OptionType == type);
        }


    }
}
