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
        //注为了记录隔夜尺长 分账户与总账户的隔夜持仓要单独放置即要体现在当前持仓总又要体现在隔夜持仓中
        /// <summary>
        /// 清算中心获得持仓数据
        /// </summary>
        /// <param name="p"></param>
        internal void GotPosition(Position p)
        {
            debug("got postioin:" + p.ToString(), QSEnumDebugLevel.INFO);
            if (!HaveAccount(p.Account)) return;
            Symbol symbol = p.oSymbol;
            if (symbol == null)
            {
                debug("symbol:" + p.Symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                return;
            }
            debug("account tracker got position", QSEnumDebugLevel.INFO);
            acctk.GotPosition(p);
            totaltk.GotPosition(p);
            onGotPosition(p);
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
                
                //如果委托错误不是新委托 则处理该委托
                //if (!neworder)
                {
                    acctk.GotOrder(error.Order);
                    totaltk.GotOrder(new OrderImpl(error.Order));
                    onGotOrder(error.Order, neworder);
                }
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

                //o.Filled = OrdBook[o.Account].Filled(o.id);//获得委托成交
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
                Symbol symbol = f.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + f.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }
                bool positionside = f.PositionSide;

                PositionTransaction postrans = null;
                Position pos = acctk.GetPosition(f.Account, f.symbol, positionside);
                int beforesize = pos.UnsignedSize;
                decimal highest = pos.Highest;
                decimal lowest = pos.Lowest;

                acctk.GotFill(f);

                int aftersize = acctk.GetPosition(f.Account, f.symbol, positionside).UnsignedSize;//查询该成交后数量
                decimal c = -1;

                //debug("got fill beforesize:" + beforesize.ToString() + " aftersize:" + aftersize.ToString(), QSEnumDebugLevel.INFO);
                //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                if (f.Commission < 0)
                {
                    decimal commissionrate = 0;
                    //成交后持仓数量大于成交前数量 开仓或者加仓
                    if (aftersize > beforesize)
                    {
                        commissionrate = symbol.EntryCommission;
                    }
                    //成交后持仓数量小于成交后数量 平仓或者减仓
                    if (aftersize < beforesize)
                    {

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

                    f.Commission = Calc.CalCommission(commissionrate, f);
                }

                //生成持仓操作记录 同时结合beforeszie aftersize 设置fill PositionOperation,需要知道帐户的持仓信息才可以知道是开 加 减 平等信息
                f.PositionOperation = PositionTransaction.GenPositionOperation(beforesize, aftersize);
                postrans = new PositionTransaction(f, symbol, beforesize, aftersize, highest, lowest);

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
