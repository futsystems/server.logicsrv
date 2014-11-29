using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    
    /// <summary>
    /// 交易帐户基础信息
    /// </summary>
    public interface IAccountLite
    {
        /// <summary>
        /// 交易帐号
        /// </summary>
        string Account { get; set; }

        /// <summary>
        /// 委托路由类别
        /// </summary>
        QSEnumOrderTransferType OrderRouteType { get; set; }

        /// <summary>
        /// 当前交易状态
        /// </summary>
        bool Execute { get; set; }

        /// <summary>
        /// 日内交易
        /// </summary>
        bool IntraDay { get; set; }

        /// <summary>
        /// 交易帐户类别
        /// </summary>
        QSEnumAccountCategory Category { get; set; }

        /// <summary>
        /// 上期权益
        /// </summary>
        decimal LastEquity { get; set; }

        /// <summary>
        /// 当前权益
        /// </summary>
        decimal NowEquity { get; set; }

        /// <summary>
        /// 平仓利润
        /// </summary>
        decimal RealizedPL { get; set; }

        /// <summary>
        /// 未平仓利润
        /// </summary>
        decimal UnRealizedPL { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        decimal Commission { get; set; }

        /// <summary>
        /// 净利
        /// </summary>
        decimal Profit { get; set; }

        /// <summary>
        /// 入金
        /// </summary>
        decimal CashIn { get; set; }

        /// <summary>
        /// 出金
        /// </summary>
        decimal CashOut { get; set; }

        /// <summary>
        /// 总占用资金 = 个品种占用资金之和
        /// </summary>
        decimal MoneyUsed { get; set; }

        /// <summary>
        /// 帐户标识
        /// </summary>
        string Name { get; set; }


        /// <summary>
        /// 期货公司
        /// </summary>
        string Broker { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        int BankID { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        string BankAC { get; set; }

        /// <summary>
        /// 锁仓权限
        /// </summary>
        bool PosLock { get; set; }

        /// <summary>
        /// 所属管理员的ID
        /// </summary>
        int MGRID{ get; set; }

        /// <summary>
        /// 是否已经删除
        /// </summary>
        bool Deleted { get; set; }


        /// <summary>
        /// 路由组编号
        /// </summary>
        int RG_ID { get; set; }
    }
}
