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
        public override void onGotOrder(Order o, bool neworder)
        {
            try
            {
                if (o != null && o.isValid)
                {
                    //debug("Got Order:" + o.ToString() + " neworder:" + neworder.ToString(), QSEnumDebugLevel.INFO);
                    if (this.Status == QSEnumClearCentreStatus.CCOPEN)
                        debug("Got  Order:" + o.ToString(), QSEnumDebugLevel.INFO);
                    if (_status == QSEnumClearCentreStatus.CCOPEN)
                    {
                        if (neworder)
                        {
                            //如果没有记录记录该委托,则新记录该委托
                            _asynLoger.newOrder(o);//记录委托
                        }
                        else
                        {
                            //如果系统没有记录该位图,则更新该委托
                            _asynLoger.updateOrder(o);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debug("onGotOrder error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        public override void onGotCancel(long oid)
        {
            if (this.Status == QSEnumClearCentreStatus.CCOPEN)
                debug("Got cancel:" + oid, QSEnumDebugLevel.INFO);
            if (_status == QSEnumClearCentreStatus.CCOPEN)
            {
                OrderAction oc = new OrderActionImpl();
                Order o = this.SentOrder(oid);
                if (o != null)
                {
                    oc.Account = o.Account;
                    oc.ActionFlag = QSEnumOrderActionFlag.Delete;
                    oc.OrderID = o.id;
                    _asynLoger.newOrderAction(oc);
                }
                
            }
        }

        
        public override void onGotFill(Trade f, PositionTransaction postrans)
        {
            try
            {
                if(this.Status == QSEnumClearCentreStatus.CCOPEN)
                    debug("Got Fill:" + f.ToString(), QSEnumDebugLevel.INFO);
                IPositionRound pr = null;
                //PR数据在从数据库恢复数据的时候任然需要被加载到PositionRoundTracker用于恢复PR数据
                if (postrans != null)
                {
                    pr =  prt.GotPositionTransaction(postrans);
                    //调整手续费 注意 这里手续费已经进行了标准手续费计算
                    f.Commission = onAdjuestCommission(f,pr);   
                }

                Position pos = getPosition(f.Account, f.symbol, f.PositionSide);
                //如果交易中心处于开启状态 则对外触发包含交易手续费的fill回报 通过tradingserver向管理端与交易客户端发送
                if (_status == QSEnumClearCentreStatus.CCOPEN)
                {
                    if (GotCommissionFill != null)
                        GotCommissionFill(f);
                    //数据库记录成交
                    _asynLoger.newTrade(f);

                    //当PositionRound关闭后 对外触发PositionRound关闭事件
                    //debug("PositionRound:" + pr.ToString(), QSEnumDebugLevel.INFO);
                    if (pr.IsClosed)
                    {
                        _asynLoger.newPositonRound(pr);
                        if (PositionRoundClosedEvent != null)
                        {
                            PositionRoundClosedEvent(pr,pos);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                debug("onGotFill error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// GotFill中有调整手续费的函数,在clearcentresrv中进行覆写
        /// 目的是建立与收费策略相关的计费方式
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public decimal onAdjuestCommission(Trade fill, IPositionRound positionround)
        {
            if (AdjustCommissionEvent != null)
                return AdjustCommissionEvent(fill, positionround);
            else
                return fill.Commission;
        }

        #endregion
    }
}
