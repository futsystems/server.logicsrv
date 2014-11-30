using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 系统内的成交帐户对象
    /// 该对象描述了某个实盘帐户的信息，姓名 所属期货公司
    /// 是系统内的结算实体，该对象需要配置对应的成交通道配置,用于路由系统进行交易
    /// </summary>
    public interface Vendor
    {
        /// <summary>
        /// 数据库编号
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 帐户姓名
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 期货公司
        /// </summary>
        string FutCompany { get; set; }


        /// <summary>
        /// 描述信息
        /// </summary>
        string Description { get; set; }


        /// <summary>
        /// 上次结算权益
        /// </summary>
        decimal LastEquity { get; set; }


        /// <summary>
        /// 成交接口
        /// </summary>
        IBroker Broker { get; }

        /// <summary>
        /// 保证金限制
        /// </summary>
        decimal MarginLimit { get; set; }

        /// <summary>
        /// 域
        /// </summary>
        Domain Domain { get; }

    }
}
