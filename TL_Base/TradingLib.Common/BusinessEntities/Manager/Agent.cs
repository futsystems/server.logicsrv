using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 代理创建时 会在系统中创建一个对应的财务账户
    /// 该账户用于记录代理的相关权益数据工作逻辑如下
    /// 0.代理分普通代理和自营代理，普通代理没有出入金权限，客户出入金与上级代理/公司进行结算；自营代理有出入金权限，客户出入金与代理进行结算
    /// 1.根据不同层级的手续模板设定，每笔成交产生时放入队列并对该成交手续费进行拆分，计算代理在该笔成交上的手续费成本，手续费收入
    /// 2.普通代理结算时，累加当天手续费提成，并根据分润结算方式计算代理分润。然后统一以入金的方式打入代理账户 并生成结算数据
    /// 3.自营代理结算时，累加其所有客户的盈亏数据，手续费，生成结算数据
    /// </summary>
    public class AgentSetting
    {

        public int ID { get; set; }
        /// <summary>
        /// 代理财务账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 代理类别
        /// </summary>
        public EnumAgentType AgentType { get; set; }

        /// <summary>
        /// 昨日权益
        /// </summary>
        public decimal LastEquity { get; set; }

        /// <summary>
        /// 昨日信用额度
        /// </summary>
        public decimal LastCredit { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 保证金模板
        /// </summary>
        public int Margin_ID { get; set; }

        /// <summary>
        /// 手续费模板
        /// </summary>
        public int Commission_ID { get; set; }

        /// <summary>
        /// 交易参数模板
        /// </summary>
        public int ExStrategy_ID { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreatedTime { get;set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public long Settledtime { get; set; }

        /// <summary>
        /// 强平权益
        /// </summary>
        public decimal FlatEquity { get; set; }

        /// <summary>
        /// 代理账户冻结
        /// </summary>
        public bool Freezed { get; set; }

    }



}
