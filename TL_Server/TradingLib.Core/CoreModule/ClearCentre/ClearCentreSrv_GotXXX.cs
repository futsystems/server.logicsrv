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
        /// 获得隔夜持仓明细数据
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(IAccount account, PositionDetail p)
        {
            try
            {
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
                logger.Error("Process PositionDetail Setted Error:" + ex.ToString());
            }
        }


        /// <summary>
        /// 响应委托错误
        /// 这里需要判断如果委托已经被记录过则继续响应委托事件 用于更新委托的状态
        /// </summary>
        /// <param name="error"></param>
        public void GotOrderError(IAccount account, Order o, RspInfo e)
        {
            try
            {
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + o.Symbol + " not exist in basictracker, drop errororder");
                    return;
                }
                acctk.GotOrder(o);
            }
            catch (Exception ex)
            {
                logger.Error("Process ErrorOrder Error:" + ex.ToString());
            }
        }


        /// <summary>
        /// 获得委托数据
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(IAccount account, Order o)
        {
            try
            {
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + o.Symbol + " not exist in basictracker, drop order");
                    return;
                }
                acctk.GotOrder(o);
                //整体交易数据维护器和每个帐户交易数据维护器 维护的数据是统一对象，避免内存占用 当有新委托时 才调用整体交易数据维护器维护该委托
                if (!totaltk.IsTracked(o))
                {
                    totaltk.NewOrder(o);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Process Order Error:" + ex.ToString());
            }
        }



        /// <summary>
        /// 清算中心获得取消
        /// </summary>
        /// <param name="oid"></param>
        public void GotCancel(IAccount account,long oid)
        {
            try
            {
                acctk.GotCancel(account.ID, oid);
            }
            catch (Exception ex)
            {
                logger.Error("Process Order Action Error:" + ex.ToString());
            }
        }

        QSEnumPosOperation GetPosOperatin(int aftersize,int beforesize)
        { 
            if (aftersize > beforesize)
            {
                if (beforesize == 0)
                    return QSEnumPosOperation.EntryPosition;
                else
                    return QSEnumPosOperation.AddPosition;
            }
            else
            {
                if (aftersize == 0)
                    return QSEnumPosOperation.ExitPosition;
                else
                    return QSEnumPosOperation.DelPosition;
            }
        }
        /// <summary>
        /// 清算中心获得成交
        /// 增加成交处理标识符accept 如果成交异常无法体现在持仓上则返回false
        /// 1.比如 多次平仓成交 造成的多余平仓成交 则后续操作不进行成交记录以及回报 并且要更新对应委托状态为拒绝
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(IAccount account,Trade f,out bool accept)
        {
            accept = false;
            try
            {
                Symbol symbol = f.oSymbol;
                if (symbol == null)
                {
                    logger.Warn("symbol:" + f.Symbol + " not exist in basictracker, drop trade");
                    return;
                }

                //通过成交的多空以及开平标志 判断是多方操作还是空方操作
                bool positionside = f.PositionSide;
                //获得对应的持仓
                Position pos = account.GetPosition(f.Symbol, positionside);
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
                        this.GotOrder(account,tmp);
                    }
                    return;
                }

                totaltk.NewFill(f);//所有的成交都只有一次回报 都需要进行记录
                pos = account.GetPosition(f.Symbol, positionside);
                int aftersize = pos.UnsignedSize;//查询该成交后数量

                //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                if (f.Commission < 0)
                {
                    //计算标准手续费
                    f.Commission = account.CalCommission(f);
                    f.StampTax = account.CalcStampTax(f);
                    f.TransferFee = account.CalcTransferFee(f);
                }
                f.PositionOperation = GetPosOperatin(aftersize, beforesize);
            }
            catch (Exception ex)
            {
                logger.Error("Got Fill error:" + ex.ToString());
            }
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
