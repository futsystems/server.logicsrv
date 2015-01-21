using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {
        #region 【onXXXXX order trade cancel tick】后处理

        /// <summary>
        /// clearcentre得到委托后的数据库记录处理
        /// 系统内的委托不更改基本属性,只进行委托状态修改
        /// </summary>
        /// <param name="o"></param>
        internal override void onGotOrder(Order o, bool neworder)
        {
            try
            {
                if (o != null && o.isValid)
                {
                    if (_status == QSEnumClearCentreStatus.CCOPEN)
                    {
                        if (neworder)
                        {
                            debug("Got Order:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                            LogAcctOrder(o);
                        }
                        else
                        {
                            debug("Update Order:" + o.GetOrderStatus(), QSEnumDebugLevel.INFO);
                            LogAcctOrderUpdate(o);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debug("onGotOrder error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        internal override void onGotCancel(long oid)
        {
            if (_status == QSEnumClearCentreStatus.CCOPEN)
            {
                OrderAction oc = new OrderActionImpl();
                Order o = this.SentOrder(oid);
                if (o != null)
                {
                    oc.Account = o.Account;
                    oc.ActionFlag = QSEnumOrderActionFlag.Delete;
                    oc.OrderID = o.id;
                    debug("Got Cancel:" + oid, QSEnumDebugLevel.INFO);
                    LogAcctOrderAction(oc);
                }
            }
        }


        internal override void onGotFill(Trade f, PositionTransaction postrans)
        {
            try
            {
                PositionRound pr = null;
                //PR数据在从数据库恢复数据的时候任然需要被加载到PositionRoundTracker用于恢复PR数据
                if (postrans != null)
                {
                    pr = prt.GotPositionTransaction(postrans);
                }
                //如果交易中心处于开启状态
                if (_status == QSEnumClearCentreStatus.CCOPEN)
                {
                    //调整手续费 注意 这里手续费已经进行了标准手续费计算
                    f.Commission = AdjuestCommission(f, pr);

                    debug("Got Fill:" + f.GetTradeInfo(), QSEnumDebugLevel.INFO);
                    //记录帐户成交记录
                    LogAcctTrade(f);
                    //当PositionRound关闭后 对外触发PositionRound关闭事件
                    if (pr.IsClosed)
                    {
                        LogAcctPositionRound(pr);
                        //持仓回合才需要获得对应的持仓信息
                        IAccount account = this[f.Account];
                        Position pos = account.GetPosition(f.Symbol, f.PositionSide);
                        //向事件中继触发持仓回合关闭事件
                        TLCtxHelper.EventIndicator.FirePositionRoundClosed(pr, pos);
                    }
                }
            }
            catch (Exception ex)
            {
                debug("onGotFill error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 通过事件中继计算手续费调整
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="positionround"></param>
        /// <returns></returns>
        internal decimal AdjuestCommission(Trade fill, PositionRound positionround)
        {
            return TLCtxHelper.ExContribEvent.AdjustCommission(fill, positionround);
        }

        #endregion
    }
}
