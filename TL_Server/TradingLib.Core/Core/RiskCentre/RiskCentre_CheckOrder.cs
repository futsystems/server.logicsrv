using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class RiskCentre
    {
        #region 【委托检查1】

        /// <summary>
        /// 风控中心委托一段检查
        /// 如果未通过检查则则给出具体的错误报告
        /// </summary>
        /// <param name="o"></param>
        /// <param name="acc"></param>
        /// <param name="errortitle"></param>
        /// <param name="inter">是否是内部委托 比如风控系统产生的委托</param>
        /// <returns></returns>
        public bool CheckOrderStep1(ref Order o,IAccount acc, out string errortitle,bool inter=false)
        {
            errortitle = string.Empty;
            //1 结算中心检查
            //1.1检查结算中心是否正常状态 如果历史结算状态则需要将结算记录补充完毕后才可以接受新的委托
            if (!TLCtxHelper.Ctx.SettleCentre.IsNormal)
            {
                errortitle = "SETTLECENTRE_NOT_RESET";//结算中心异常
                return false;
            }

            //1.2检查当前是否是交易日
            if (!TLCtxHelper.Ctx.SettleCentre.IsTradingday)//非周六0->2:30 周六0:00->2:30有交易(金银夜盘交易)
            {
                errortitle = "NOT_TRADINGDAY";//非交易日
                return false;
            }

            //1.3检查结算中心是否处于结算状态 结算状态不接受任何委托
            if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
            {
                errortitle = "SETTLECENTRE_IN_SETTLE";//结算中心出入结算状态
                return false;
            }

            //2 清算中心检查
            //2.1检查清算中心是否出入接受委托状态(正常工作状态下系统会定时开启和关闭清算中心,如果是开发模式则可以通过手工来提前开启)
            if (_clearcentre.Status != QSEnumClearCentreStatus.CCOPEN)
            {
                errortitle = "CLEARCENTRE_CLOSED";//清算中心已关闭
                return false;
            }

            //3 合约检查
            //3.1合约是否存在
            if (!BasicTracker.SymbolTracker.TrckerOrderSymbol(o))
            {
                errortitle = "SYMBOL_NOT_EXISTED";//合约不存在
                return false;
            }

            //3.2合约是否可交易
            if (!o.oSymbol.IsTradeable)//合约不可交易
            {
                errortitle = "SYMBOL_NOT_TRADEABLE";//合约不可交易
                return false;
            }

            //3.3检查合约交易时间段
            if (_marketopencheck)
            {
                //合约市场开市时间检查(对不同的市场进行开市时间常规检查)
                if (!o.oSymbol.IsMarketTime)
                {
                    errortitle = "SYMBOL_NOT_MARKETTIME";//非交易时间段
                    return false;
                }
            }

            //4.委托数量检查
            //4.1委托总数不为0
            if (o.TotalSize == 0)
            {
                errortitle = "ORDERSIZE_ZERO_ERROR";//委托数量为0
                return false;
            }
            //4.2单次开仓数量小于设定值
            Position pos = _clearcentre.getPosition(o.Account, o.symbol);
            if (pos.isFlat || (pos.isLong && o.side) || (pos.isShort && !o.side))
            //if ((pos.isLong && o.side) || (pos.isShort && !o.side))//开仓限制手数量
            {
                if (Math.Abs(o.TotalSize) > _orderlimitsize)
                {
                    errortitle = "ORDERSIZE_LIMIT";//委托数量超过最大委托手数
                    return false;
                }
            }

            //5.委托价格检查
            //5.1查看数据通道是否有对应的合约价格
            decimal avabileprice = TLCtxHelper.Ctx.MessageExchange.GetAvabilePrice(o.symbol);
            if (avabileprice <= 0)
            {
                errortitle = "SYMBOL_TICK_ERROR";//市场旱情异常
                return false;
            }

            //5.2检查价格是否在涨跌幅度内
            if (o.isLimit || o.isStop)
            {
                decimal targetprice = o.isLimit ? o.price : o.stopp;
                decimal diff = Math.Abs(targetprice - avabileprice);
                //如果价格超过涨跌幅 则回报操作
                if ((diff / avabileprice) > 0.1M)
                {
                    errortitle = "ORDERPRICE_OVERT_LIMIT";//保单价格超过涨跌幅
                    return false;
                }

            }
            return true;

        }

        #endregion



        #region 【委托检查】
        /// <summary>
        /// 检查某个委托是否符合风控规则
        /// 注:这里需要实现多并发支持(某个账户未必是多并发,但是可能有很多账户同时请求该操作)
        /// 风控检查部分包括了
        /// 常规检查与帐户特定检查
        /// 常规检查:常规交易中的检查,保证金,合约等相关通用规则
        /// 帐户特定检查:帐户设定的特定风控规则进行检查
        /// 
        /// 1.清算中心是否缓存该账号
        /// 2.该交易账户当前是否有交易权限
        /// 3.合约交易时间段
        /// 4.保证金检查常规(不能超过可用资金)
        /// 5.账户本身设定的委托规则检查
        /// 
        /// inter标识为内部委托 比如风控规则触发强平的委托
        /// 强平的委托则不受相关风控规则的限制,强平只要满足基本的交易时间段和合约可交易即可
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckOrderStep2(ref Order o, IAccount account,out string msg, bool inter = false)
        {
            try
            {
                debug("检查委托 " + o.id.ToString(), QSEnumDebugLevel.INFO);
                msg = "";
                //延迟加载规则,这样系统就没有必要加载不没有登入的账户规则
                if (!account.RuleItemLoaded)
                    this.LoadRuleItem(account);

                //1.账号是否被冻结 内部委托不检查帐号是否冻结
                if (!inter)
                {
                    if (!account.Execute)
                    {
                        msg = "账户被冻结";
                        debug("Order rejected by [Execute Check]" + o.ToString(), QSEnumDebugLevel.INFO);
                        return false;
                    }
                }

                //检查合约交易时间段
                if (_marketopencheck)
                {
                    //日内交易检查
                    if ((!inter) && account.IntraDay)//非内部委托并且帐户是日内交易帐户则要检查日内交易时间
                    {
                        //如果是强平时间段则不可交易 
                        if (o.oSymbol.IsFlatTime)
                        {
                            msg = "日内交易帐户，系统正在强平，无法处理委托！";
                            debug("Order rejected by [FlatTime Check] not in [intraday] trading time" + o.ToString(), QSEnumDebugLevel.INFO);
                            return false;
                        }
                    }
                }

                

                //3.合约交易权限检查(如果帐户存在特殊服务,可由特殊服务进行合约交易权限检查) 有关特殊服务的主力合约控制与检查放入到对应的特殊服务实现中
                if (!inter)
                {
                    if (!account.CanTakeSymbol(o.oSymbol, out msg))
                    {
                        debug("Order rejected by[Account CanTakeSymbol Check]" + o.ToString(), QSEnumDebugLevel.INFO);
                        return false;
                    }
                }

                //4.保证金检查(如果帐户存在特殊的服务,可由特殊的服务进行保证金检查)
                if (!account.CanFundTakeOrder(o, out msg))
                {   
                    debug("Order rejected by[Accoun t CanFundTakeOrder Check]" + o.ToString(), QSEnumDebugLevel.INFO);
                    return false;
                }

                //debug("riskcentre run to here", QSEnumDebugLevel.INFO);
                //5.账号所应用的风控规则检查 内部委托不执行帐户个性的自定义检查
                if (!inter)
                {
                    if (!account.CheckOrder(o, out msg))//如果通过风控检查 则置委托状态为Placed
                    {
                        debug("Order rejected by[Order Rule Check]" + o.ToString(), QSEnumDebugLevel.INFO);
                        return false;
                    }
                }

                //如果委托通过了所有风控检查,则置委托为提交状态(通过交易服务的委托检查,将发送到对应的成交接口),
                o.Status = QSEnumOrderStatus.Placed;
                return true;
            }
            catch (Exception ex)
            {
                msg = "风控检查异常";
                string s = PROGRAME + ":委托风控检查异常" + ex.ToString();
                debug(s, QSEnumDebugLevel.ERROR);
                _othercheklog.GotDebug(s);
                return false;
            }
        }
        #endregion

    }
}
