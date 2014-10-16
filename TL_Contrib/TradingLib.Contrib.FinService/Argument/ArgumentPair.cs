using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.FinService
{
    /// <summary>
    /// 封装2个argument 用于形成2个值
    /// </summary>
    public class ArgumentPair
    {
        public ArgumentPair(Argument acctarg, Argument agtarg)
        {
            this.AccountArgument = acctarg;
            this.AgentArgument = agtarg;
        }
        /// <summary>
        /// 帐户计算参数
        /// </summary>
        public Argument AccountArgument { get; set; }
        /// <summary>
        /// 代理计算参数
        /// </summary>
        public Argument AgentArgument { get; set; }
    }
}
