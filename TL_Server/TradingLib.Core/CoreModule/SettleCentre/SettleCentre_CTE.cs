using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


/* 进一步完善结算机制
 * 目前结算 1.数据保存 结算持仓明细 当日交易记录 2.执行帐户结算 3.清空日内交易记录
 * 持仓对象的 closedpl是按照持仓开平过程中产生的平仓盈亏的实时累加， 对应通过累加持仓明细的 平仓盈亏也是得到相同的金额
 *           unrealizedpl 是按照最新价格计算的浮动盈亏，结算时需要计算 结算价格为基础的浮动盈亏
 *           按持仓成本-结算价格 来计算盯市浮动盈亏
 *           
 *           累加持仓明细的 盯市浮动盈亏 也应该得到相同的金额，在持仓明细的盯市浮动盈亏的计算过程中，采的是动态结算价格，在交易过程中该价格是持仓对象的最新价格，当持仓结算后，其对应的价格是结算价格。
 *           因此计算出来的浮动盈亏也是一致的。只是采用了2中不同的计算方式
 *           
 *           这里考虑结算过程中从 通常的财务计算过程分离，独立的去计算盯市浮动盈亏
 *           把持仓的成本移动到结算价,同时将移动部分的浮动盈亏结算到交易帐户中，逐日结算制度
 *           
 *           这里需要细化结算过程
 *           1.结算之前需要确认获得了所有结算价格
 *           2.价格推送到系统持仓后，保存持仓明细（持仓明细会按结算价格进行计算当前的盯市浮动盈亏）保存数据
 *           3.然后执行帐户结算，将帐户当日盈亏 手续费 出入金 盯市浮动盈亏 计算 形成结算记录
 *           4.结算单实际上是某个时间节点 所记录的状态数据的在线。
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */
namespace TradingLib.Core
{
    /// <summary>
    /// 关于系统结算
    /// 系统结算时 如果帐户有持仓 则结算当时的权益并不会停止不动
    /// 系统仍然会根据行情的变化而产生数据变化
    /// 同时隔夜持仓也是进行了利润结转 不利于获得持仓的历史成本
    /// 这里进行改进
    /// 1.保存结算持仓时 记录持仓的历史成本 同时记录该持仓的结算价格 用于记录当时的盯市盈亏
    /// 2.系统结算报表记录时,将每个持仓的盯市盈亏 累计后计入浮动盈亏
    /// 3.帐户重置时,需要从数据库加载所有数据而不是将当时的权益进行结转，应为当时的权益可能已经和执行结算时候的权益发生了变化
    /// </summary>
    public partial class SettleCentre
    {

        [ContribCommandAttr(QSEnumCommandSource.CLI, "settlestatus", "settlestatus - settlestatus", "settlestatus")]
        public string CTE_SettleStatus()
        {
            return string.Format("Last Settleday:{0} Current Tradingday:{1} SettleMode:{2}", this.LastSettleday, this.Tradingday,this.SettleMode);
        }
    }
}
