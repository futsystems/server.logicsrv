using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class ClearCentreBase
    {
        public void GotPosition(PositionDetail p)
        {
            try
            {
                IAccount account = TLCtxHelper.ModuleAccountManager[p.Account];
                if (account == null) return;
                Symbol symbol = p.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + p.Symbol + " not exist in basictracker, drop positiondetail", QSEnumDebugLevel.ERROR);
                    return;
                }
                acctk.GotPosition(p);
                onGotPosition(p);
            }
            catch (Exception ex)
            {
                debug("处理隔夜持仓明细数据异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        internal virtual void onGotPosition(PositionDetail p)
        {

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
                    debug("symbol:" + o.Symbol + " not exist in basictracker, drop errororder", QSEnumDebugLevel.ERROR);
                    return;
                }
                bool neworder = !totaltk.IsTracked(o.id);
                acctk.GotOrder(o);
                onGotOrder(o, neworder);
            }
            catch (Exception ex)
            {
                debug("处理委托错误异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        /// <summary>
        /// 清算中心获得委托数据
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            try
            {
                IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
                if (account == null) return;
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + o.Symbol + " not exist in basictracker, drop order", QSEnumDebugLevel.ERROR);
                    return;
                }

                bool neworder = !totaltk.IsTracked(o.id);
                acctk.GotOrder(o);

                //整体交易数据维护器和每个帐户交易数据维护器 维护的数据是统一对象，避免内存占用 当有新委托时 才调用整体交易数据维护器维护该委托
                if (neworder)
                {
                    totaltk.NewOrder(o);
                }
                onGotOrder(o, neworder);

            }
            catch (Exception ex)
            {
                debug("处理委托异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        internal virtual void onGotOrder(Order o, bool neworder)
        {
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
                onGotCancel(oid);
            }
            catch (Exception ex)
            {
                debug("处理取消异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        internal virtual void onGotCancel(long oid)
        {

        }

        /// <summary>
        /// 清算中心获得成交
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f)
        {
            try
            {
                IAccount account = TLCtxHelper.ModuleAccountManager[f.Account];
                if (account == null) return;
                Symbol symbol = f.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + f.Symbol + " not exist in basictracker, drop trade", QSEnumDebugLevel.ERROR);
                    return;
                }

                //通过成交的多空以及开平标志 判断是多方操作还是空方操作
                bool positionside = f.PositionSide;

                PositionTransaction postrans = null;

                //获得对应的持仓
                Position pos = account.GetPosition(f.Symbol, positionside);//acctk.GetPosition(f.Account, f.symbol, positionside);
                int beforesize = pos.UnsignedSize;
                
                //累加持仓
                acctk.GotFill(f);
                totaltk.NewFill(f);//所有的成交都只有一次回报 都需要进行记录
                pos = account.GetPosition(f.Symbol, positionside);//acctk.GetPosition(f.Account, f.symbol, positionside);
                int aftersize = pos.UnsignedSize;//查询该成交后数量

                //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                if (f.Commission < 0)
                {
                    //计算标准手续费
                    f.Commission = account.CalCommission(f);
                }
                
                //生成持仓操作记录 同时结合beforeszie aftersize 设置fill PositionOperation,需要知道帐户的持仓信息才可以知道是开 加 减 平等信息
                postrans = new PositionTransaction(f, symbol, beforesize, aftersize, pos.Highest,pos.Lowest);
                f.PositionOperation = postrans.PosOperation;
                //子类函数的onGotFill用于执行数据记录以及其他相关业务逻辑
                onGotFill(f, postrans);

            }
            catch (Exception ex)
            {
                debug("Got Fill error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        internal virtual void onGotFill(Trade fill, PositionTransaction postrans)
        {
        }

        //得到新的Tick数据
        public void GotTick(Tick k)
        {
            try
            {
                acctk.GotTick(k);
            }
            catch (Exception ex)
            {
                debug("Got Tick error:" + ex.ToString());
            }
        }
    }
}
