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
        /// <summary>
        /// 获得交易所结算数据
        /// </summary>
        /// <param name="settle"></param>
        public void GotExchangeSettlement(ExchangeSettlement settle)
        {
            try
            {
                IAccount account = TLCtxHelper.ModuleAccountManager[settle.Account];
                if (account == null) return;
                acctk.GotExchangeSettlement(settle);
            }
            catch (Exception ex)
            {
                logger.Error("处理交易所结算数据异常:" + ex.ToString());
            }
        }

        /// <summary>
        /// 获得隔夜持仓明细数据
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(PositionDetail p)
        {
            try
            {
                IAccount account = TLCtxHelper.ModuleAccountManager[p.Account];
                if (account == null) return;
                Symbol symbol = p.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + p.Symbol + " not exist in basictracker, drop positiondetail");
                    return;
                }
                acctk.GotPosition(p);
            }
            catch (Exception ex)
            {
                logger.Error("处理隔夜持仓明细数据异常:" + ex.ToString());
            }
        }


        /// <summary>
        /// 响应委托错误
        /// 这里需要判断如果委托已经被记录过则继续响应委托事件 用于更新委托的状态
        /// </summary>
        /// <param name="error"></param>
        public void GotOrderError(Order o, RspInfo e)
        {
            try
            {
                IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
                if (account == null) return;
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + o.Symbol + " not exist in basictracker, drop errororder");
                    return;
                }
                bool neworder = !totaltk.IsTracked(o);
                acctk.GotOrder(o);

                if (_status == QSEnumClearCentreStatus.CCOPEN || TLCtxHelper.ModuleSettleCentre.SettleCentreStatus == QSEnumSettleCentreStatus.HISTSETTLE)
                {
                    if (neworder)
                    {
                        logger.Info("Got Order:" + o.GetOrderInfo());
                        TLCtxHelper.ModuleDataRepository.NewOrder(o);
                    }
                    else
                    {
                        logger.Info("Update Order:" + o.GetOrderStatus());
                        TLCtxHelper.ModuleDataRepository.UpdateOrder(o);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("处理委托错误异常:" + ex.ToString());
            }
        }


        /// <summary>
        /// 获得委托数据
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            try
            {
                if (o == null || (!o.isValid)) return;

                //检查交易帐户与合约
                IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
                if (account == null) return;
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + o.Symbol + " not exist in basictracker, drop order");
                    return;
                }

                //帐户维护器获得委托 用于更新帐户委托数据与状态
                acctk.GotOrder(o);
                //通过整体数据维护器查看委托是否是新委托
                bool neworder = !totaltk.IsTracked(o);
                //整体交易数据维护器和每个帐户交易数据维护器 维护的数据是统一对象，避免内存占用 当有新委托时 才调用整体交易数据维护器维护该委托
                if (neworder)
                {
                    totaltk.NewOrder(o);
                }

                if (_status == QSEnumClearCentreStatus.CCOPEN || TLCtxHelper.ModuleSettleCentre.SettleCentreStatus == QSEnumSettleCentreStatus.HISTSETTLE)
                {
                    if (neworder)
                    {
                        logger.Info("Got Order:" + o.GetOrderInfo()); 
                        TLCtxHelper.ModuleDataRepository.NewOrder(o);
                    }
                    else
                    {
                        logger.Info("Update Order:" + o.GetOrderStatus());
                        TLCtxHelper.ModuleDataRepository.UpdateOrder(o);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("处理委托异常:" + ex.ToString());
            }
        }


        /// <summary>
        /// 清算中心记录BO委托
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(BinaryOptionOrder o)
        {
            try
            { 
                //检查交易帐户与合约
                IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
                if (account == null) return;
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + o.BinaryOption.Symbol + " not exist in basictracker, drop order");
                    return;
                }

                //帐户维护器获得委托 用于更新帐户委托数据与状态
                acctk.GotOrder(o);
                //通过整体数据维护器查看委托是否是新委托
                bool neworder = !totaltk.IsTracked(o);

                //整体交易数据维护器和每个帐户交易数据维护器 维护的数据是统一对象，避免内存占用 当有新委托时 才调用整体交易数据维护器维护该委托
                if (neworder)
                {
                    totaltk.NewOrder(o);
                }

                if (_status == QSEnumClearCentreStatus.CCOPEN || TLCtxHelper.ModuleSettleCentre.SettleCentreStatus == QSEnumSettleCentreStatus.HISTSETTLE)
                {
                    if (neworder)
                    {
                        logger.Info("Got Order:" + o.ToString());
                        //TLCtxHelper.ModuleDataRepository.NewOrder(o);
                    }
                    else
                    {
                        logger.Info("Update Order:" + o.ToString());
                        //TLCtxHelper.ModuleDataRepository.UpdateOrder(o);
                    }
                }



            }
            catch (Exception ex)
            {
                logger.Error("处理BO委托异常:" + ex.ToString());
            }
        
        }

        /// <summary>
        /// 清算中心获得取消
        /// </summary>
        /// <param name="oid"></param>
        public void GotCancel(long oid)
        {
            try
            {
                string account = SentOrder(oid).Account;
                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                if (acc == null) return;

                acctk.GotCancel(account, oid);

                if (_status == QSEnumClearCentreStatus.CCOPEN)
                {
                    OrderAction oc = new OrderActionImpl();
                    Order o = this.SentOrder(oid);
                    if (o != null)
                    {
                        oc.Account = o.Account;
                        oc.ActionFlag = QSEnumOrderActionFlag.Delete;
                        oc.OrderID = o.id;
                        logger.Info("Got Cancel:" + oid);
                        TLCtxHelper.ModuleDataRepository.NewOrderAction(oc);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("处理取消异常:" + ex.ToString());
            }
        }

        /// <summary>
        /// 清算中心获得成交
        /// 增加成交处理标识符accept 如果成交异常无法体现在持仓上则返回false
        /// 1.比如 多次平仓成交 造成的多余平仓成交 则后续操作不进行成交记录以及回报 并且要更新对应委托状态为拒绝
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f,out bool accept)
        {
            accept = false;
            try
            {
                if (f == null || (!f.isValid)) return;

                //检查交易帐户和合约对象
                IAccount account = TLCtxHelper.ModuleAccountManager[f.Account];
                if (account == null) return;
                Symbol symbol = f.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + f.Symbol + " not exist in basictracker, drop trade");
                    return;
                }

                //通过成交的多空以及开平标志 判断是多方操作还是空方操作
                bool positionside = f.PositionSide;
                PositionTransaction postrans = null;

                //获得对应的持仓
                Position pos = account.GetPosition(f.Symbol, positionside);//acctk.GetPosition(f.Account, f.symbol, positionside);
                int beforesize = pos.UnsignedSize;

                //累加持仓
                acctk.GotFill(f,out accept);
                //如果帐户维护器无法处理该成交 则直接返回 不用计算成交手续费或记录成交
                if (!accept)
                {
                    Order o = this.SentOrder(f.id);
                    if (o != null)
                    {
                        //如果成交没有接受 则获得对应的委托将委托状态更新未reject;
                        Order tmp = new OrderImpl(o);
                        tmp.Status = QSEnumOrderStatus.Reject;
                        this.GotOrder(tmp);

                    }
                    return;
                }

                totaltk.NewFill(f);//所有的成交都只有一次回报 都需要进行记录
                pos = account.GetPosition(f.Symbol, positionside);//acctk.GetPosition(f.Account, f.symbol, positionside);
                int aftersize = pos.UnsignedSize;//查询该成交后数量

                //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                if (f.Commission < 0)
                {
                    //计算标准手续费
                    f.Commission = account.CalCommission(f);
                    f.StampTax = account.CalcStampTax(f);
                    f.TransferFee = account.CalcTransferFee(f);
                }

                //生成持仓操作记录 同时结合beforeszie aftersize 设置fill PositionOperation,需要知道帐户的持仓信息才可以知道是开 加 减 平等信息
                postrans = new PositionTransaction(f, symbol, beforesize, aftersize, pos.Highest, pos.Lowest);
                f.PositionOperation = postrans.PosOperation;
                
                PositionRound pr = null;
                //PR数据在从数据库恢复数据的时候任然需要被加载到PositionRoundTracker用于恢复PR数据
                if (postrans != null)
                {
                    pr = prt.GotPositionTransaction(postrans);
                }
                //如果交易中心处于开启状态
                if (_status == QSEnumClearCentreStatus.CCOPEN || TLCtxHelper.ModuleSettleCentre.SettleCentreStatus == QSEnumSettleCentreStatus.HISTSETTLE)
                {
                    //调整手续费 注意 这里手续费已经进行了标准手续费计算
                    f.Commission = AdjuestCommission(f, pr);

                    logger.Info("Got Fill:" + f.GetTradeInfo());
                    //记录帐户成交记录
                    TLCtxHelper.ModuleDataRepository.NewTrade(f);
                    //当PositionRound关闭后 对外触发PositionRound关闭事件
                    if (pr.IsClosed)
                    {
                        //向事件中继触发持仓回合关闭事件
                        TLCtxHelper.EventIndicator.FirePositionRoundClosed(pr, pos);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Got Fill error:" + ex.ToString());
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



        /// <summary>
        /// 响应行情数据
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            try
            {
                acctk.GotTick(k);
            }
            catch (Exception ex)
            {
                logger.Error("Got Tick error:" + ex.ToString());
            }
        }

    }
}
