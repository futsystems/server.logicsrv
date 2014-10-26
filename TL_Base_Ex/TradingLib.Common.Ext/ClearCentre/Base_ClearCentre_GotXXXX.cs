using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class ClearCentreBase
    {

        #region 【IGotTradingInfo】昨日持仓 委托 成交 取消 Tick数据处理

        internal void GotPosition(PositionDetail p)
        {
            try
            {
                if (!HaveAccount(p.Account)) return;
                Symbol symbol = p.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + p.Symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }

                acctk.GotPosition(p);
                totaltk.GotPosition(p);
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


        //注为了记录隔夜尺长 分账户与总账户的隔夜持仓要单独放置即要体现在当前持仓总又要体现在隔夜持仓中
        /// <summary>
        /// 清算中心获得持仓数据
        /// </summary>
        /// <param name="p"></param>
        internal void GotPosition(Position p)
        {
            try
            {
                if (!HaveAccount(p.Account)) return;
                Symbol symbol = p.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + p.Symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }

                acctk.GotPosition(p);
                totaltk.GotPosition(p);
                onGotPosition(p);
            }
            catch (Exception ex)
            {
                debug("处理隔夜持仓异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        internal virtual void onGotPosition(Position p)
        {

        }

        /// <summary>
        /// 响应委托错误
        /// 这里需要判断如果委托已经被记录过则继续响应委托事件 用于更新委托的状态
        /// </summary>
        /// <param name="error"></param>
        internal void GotErrorOrder(ErrorOrder error)
        {
            try
            {
                if (!HaveAccount(error.Order.Account)) return;
                Symbol symbol = error.Order.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + error.Order.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }
                bool neworder = !totaltk.IsTracked(error.Order.id);
                acctk.GotOrder(error.Order);
                totaltk.GotOrder(new OrderImpl(error.Order));
                onGotOrder(error.Order, neworder);
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
        internal void GotOrder(Order o)
        {
            try
            {
                if (!HaveAccount(o.Account)) return;
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + o.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }

                bool neworder = !totaltk.IsTracked(o.id);
                acctk.GotOrder(o);
                totaltk.GotOrder(new OrderImpl(o));
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
        internal void GotCancel(long oid)
        {
            try
            {
                string account = SentOrder(oid).Account;
                if (!HaveAccount(account)) return;

                acctk.GotCancel(account, oid);
                totaltk.GotCancel(oid);
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
        internal void GotFill(Trade f)
        {
            try
            {
                if (!HaveAccount(f.Account)) return;
                IAccount account = this[f.Account];
                Symbol symbol = f.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + f.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }

                //通过成交的多空以及开平标志 判断是多方操作还是空方操作
                bool positionside = f.PositionSide;

                PositionTransaction postrans = null;

                //获得对应的持仓
                Position pos = account.GetPosition(f.symbol, positionside);//acctk.GetPosition(f.Account, f.symbol, positionside);
                int beforesize = pos.UnsignedSize;
                //decimal avgprice = pos.AvgPrice;
                //累加持仓
                acctk.GotFill(f);
                pos = account.GetPosition(f.symbol, positionside);//acctk.GetPosition(f.Account, f.symbol, positionside);
                int aftersize = pos.UnsignedSize;//查询该成交后数量
                //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                if (f.Commission < 0)
                {
                    decimal commissionrate = 0;
                    //开仓
                    if (f.IsEntryPosition)
                    {
                        commissionrate = symbol.EntryCommission;
                    }
                    //平仓
                    else
                    {
                        //进行特殊手续费判定并设定对应的手续费费率
                        //如果对应的合约是单边计费的或者有特殊计费方式的合约，则我们单独计算该部分费用,注这里还需要加入一个日内交易的判断,暂时不做(当前交易均为日内)
                        //获得平仓手续费特例
                        if (CommissionHelper.AnyCommissionSetting(SymbolHelper.genSecurityCode(f.symbol), out commissionrate))
                        {
                            //debug("合约:" + SymbolHelper.genSecurityCode(f.symbol) + "日内手续费费差异", QSEnumDebugLevel.MUST);
                        }
                        else//没有特殊费率参数,则为标准的出场费率
                        {
                            commissionrate = symbol.ExitCommission;
                        }
                    }
                    //计算标准手续费
                    f.Commission = Calc.CalCommission(commissionrate, f);
                }
                
                

                //生成持仓操作记录 同时结合beforeszie aftersize 设置fill PositionOperation,需要知道帐户的持仓信息才可以知道是开 加 减 平等信息
                postrans = new PositionTransaction(f, symbol, beforesize, aftersize, pos.Highest,pos.Lowest);
                f.PositionOperation = postrans.PosOperation;
                totaltk.GotFill(f);

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
        internal void GotTick(Tick k)
        {
            try
            {
                acctk.GotTick(k);
                totaltk.GotTick(k);
            }
            catch (Exception ex)
            {
                debug("Got Tick error:" + ex.ToString());
            }
        }
        #endregion

    }
}
