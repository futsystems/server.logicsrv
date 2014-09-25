using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 关于将委托开平数量检查移动到风控中心
    /// 在接受委托时 需要对委托开平进行检查
    /// 开仓不进行平仓数量检查,需要进行资金检查
    /// 平仓要进行可平仓数量检查，不需要进行资金检查
    /// 
    /// 获得某个帐户某个合约的可平仓数量,如果平仓委托数量超过过可平持仓数量 则拒绝
    /// 可平仓数量 = 总持仓数量-未成交平仓数量
    /// 
    /// 同时需要增加 自动判断开平的功能
    /// 如果order.offsetflag设定为unknow
    /// 则需要结合当枪持仓进行自动判断
    /// 如果当前持仓只有多头，下多单则表明开仓，下空单表明平仓
    /// 如果当前持仓只有空头，下空单则表明开仓，下多单表明平仓
    /// 如果当前同时持有多头和空头，无法判断 该委托是要处理多头仓位还是空头仓位
    /// 这里的自动判断逻辑 是假定用户不进行锁仓，之愿意持有一个方向的仓位
    /// 
    /// 同时这里需要暴露设置 用于设定帐户是否可以进行锁仓操作 这里的锁仓操作标识还需要和品种挂钩
    /// 不能锁仓标识 不能进行任何锁仓操作。 可以锁仓并且未制定可以以锁仓的品种，则默认只有期货才可以进行锁仓
    /// 
    /// 逻辑部分 
    /// 1.绑定自动标识 通过自动判定来设定委托的开 平标识
    /// 2.检查锁仓操作的权限，如果不允许锁仓则拒绝
    /// 3.进行开仓资金检查 和 平仓数量检查
    /// </summary>
    public partial class RiskCentre
    {
        #region 【委托检查1】

        /// <summary>
        /// 风控中心委托一段检查
        /// 如果未通过检查则则给出具体的错误报告
        /// 一段检查中有一些委托错误并不需要记录数据
        /// 比如非交易日，结算中心异常，以及清算中心以关闭等
        /// 这些回报不需要进行记录 否则会形成大量无意义的拒绝数据
        /// </summary>
        /// <param name="o"></param>
        /// <param name="acc"></param>
        /// <param name="errortitle"></param>
        /// <param name="inter">是否是内部委托 比如风控系统产生的委托</param>
        /// <returns></returns>
        public bool CheckOrderStep1(ref Order o,IAccount account,out bool needlog, out string errortitle,bool inter=false)
        {
            errortitle = string.Empty;
            needlog = true;
            //1 结算中心检查
            //1.1检查结算中心是否正常状态 如果历史结算状态则需要将结算记录补充完毕后才可以接受新的委托
            if (!TLCtxHelper.Ctx.SettleCentre.IsNormal)
            {
                errortitle = "SETTLECENTRE_NOT_RESET";//结算中心异常
                needlog = false;
                return false;
            }

            //1.2检查当前是否是交易日
            if (!TLCtxHelper.Ctx.SettleCentre.IsTradingday)//非周六0->2:30 周六0:00->2:30有交易(金银夜盘交易)
            {
                errortitle = "NOT_TRADINGDAY";//非交易日
                needlog = false;
                return false;
            }

            //1.3检查结算中心是否处于结算状态 结算状态不接受任何委托
            if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
            {
                errortitle = "SETTLECENTRE_IN_SETTLE";//结算中心出入结算状态
                needlog = false;
                return false;
            }

            //2 清算中心检查
            //2.1检查清算中心是否出入接受委托状态(正常工作状态下系统会定时开启和关闭清算中心,如果是开发模式则可以通过手工来提前开启)
            if (_clearcentre.Status != QSEnumClearCentreStatus.CCOPEN)
            {
                errortitle = "CLEARCENTRE_CLOSED";//清算中心已关闭
                needlog = false;
                return false;
            }

            //3 合约检查
            //3.1合约是否存在
            if (!BasicTracker.SymbolTracker.TrckerOrderSymbol(o))
            {
                errortitle = "SYMBOL_NOT_EXISTED";//合约不存在
                needlog = false;
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

            //4.开仓标识与锁仓权限检查
            //4.1自动开平标识识别
            bool havelong = account.GetHaveLongPosition();
            bool haveshort = account.GetHaveShortPosition();
            //自动判定开平标识
            if (o.OffsetFlag == QSEnumOffsetFlag.UNKNOWN)
            {
                if (havelong && haveshort)
                {
                    //errortitle = "OFFSETFLAG_CAN_NOT_TETECTED";
                    //return false;

                    o.OffsetFlag = QSEnumOffsetFlag.CLOSE;//如果同时持有多空两个方向的持仓 则自动判定为平仓
                }
                else if (havelong)//多头
                {
                    if (o.side)
                    {
                        o.OffsetFlag = QSEnumOffsetFlag.OPEN;
                    }
                    else
                    {
                        o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                    }
                }
                else if (haveshort)//空头
                {
                    if (o.side)
                    {
                        o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                    }
                    else
                    {
                        o.OffsetFlag = QSEnumOffsetFlag.OPEN;
                    }
                }
                else//如果没有任何持仓 则默认为开仓操作
                {
                    o.OffsetFlag = QSEnumOffsetFlag.OPEN;
                }
                debug("Order offsetflag unknown ,detected and set to:" + o.OffsetFlag.ToString());
            }


            //4.2检查锁仓方向
            //获得委托持仓操作方向
            bool orderside = o.PositionSide;

            //反待成交开仓委托
            bool othersideentry = account.GetPendingEntrySize(o.symbol,!orderside) > 0;

            //开仓操作
            if (o.IsEntryPosition)
            {
                //委托多头开仓操作,同时又空头头寸 或者 委托空头开仓操作，同时又有多头头寸 则表明在持有头寸的时候进行了反向头寸的操作
                if (othersideentry || (orderside && haveshort) || ((!orderside) && havelong))//多头持仓操作
                {
                    //非期货品种无法进行锁仓操作 同时帐户设置是否允许锁仓操作
                    if ((o.oSymbol.SecurityType != SecurityType.FUT) || (!account.PosLock))
                    {
                        errortitle = "TWO_SIDE_POSITION_HOLD_FORBIDDEN";
                        debug("SecurityType:" + o.oSymbol.SecurityType.ToString() + " account PosLock:" + account.PosLock.ToString(), QSEnumDebugLevel.INFO);
                        return false;
                    }
                }
            }


            //5.委托数量检查
            //5.1委托总数不为0
            if (o.TotalSize == 0)
            {
                errortitle = "ORDERSIZE_ZERO_ERROR";//委托数量为0
                return false;
            }
            //5.2单次开仓数量小于设定值
            if (o.IsEntryPosition)//开仓
            {
                if (Math.Abs(o.TotalSize) > _orderlimitsize)
                {
                    errortitle = "ORDERSIZE_LIMIT";//委托数量超过最大委托手数
                    return false;
                }
            }

            //6.委托价格检查
            //6.1查看数据通道是否有对应的合约价格
            decimal avabileprice = TLCtxHelper.Ctx.MessageExchange.GetAvabilePrice(o.symbol);
            if (avabileprice <= 0)
            {
                errortitle = "SYMBOL_TICK_ERROR";//市场旱情异常
                return false;
            }

            //6.2检查价格是否在涨跌幅度内
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
                //if (!inter)
                {
                    if (!account.CanTakeSymbol(o.oSymbol, out msg))
                    {
                        debug("Order rejected by[Account CanTakeSymbol Check]" + o.ToString(), QSEnumDebugLevel.INFO);
                        return false;
                    }
                }


                //4 委托开仓 平仓项目检查
                //通过account symbol 以及委托的持仓操作方向查找对应的position
                Position pos = account.GetPosition(o.symbol, o.PositionSide);//当前对应持仓
                //检查该委托是否是开仓委托
                bool entryposition = o.IsEntryPosition;
                debug("Order[" + o.id.ToString() + "]" + " try to " + (o.IsEntryPosition ? "开仓" : "平仓") + " 操作方向:" + (o.PositionSide ? "多头持仓" : "空头"), QSEnumDebugLevel.INFO);
                if (entryposition)//开仓执行资金检查
                {
                    //如果是开仓委托 则直接允许
                    //保证金检查(如果帐户存在特殊的服务,可由特殊的服务进行保证金检查)
                    if (!account.CanFundTakeOrder(o, out msg))
                    {
                        debug("Order rejected by[Order Margin Check]" + o.ToString(), QSEnumDebugLevel.INFO);
                        return false;
                    }
                }
                else//平仓执行数量检查
                {
                    //获得该帐户 该合约 该持仓方向的待成交平仓委托
                    int pendingExitSize =account.GetPendingExitSize(o.symbol, o.PositionSide);
                    //当前持仓数量
                    int pos_size = pos.UnsignedSize;
                    //当前委托数量
                    int osize = o.UnsignedSize;
                    debug("Order try to exit postion,pos size:" + pos.Size.ToString() + " pending exit size:" + pendingExitSize.ToString() + " osize:" + osize.ToString() + " offsetflag:" + o.OffsetFlag.ToString(), QSEnumDebugLevel.INFO);

                    if (pos_size < pendingExitSize+osize)
                    {
                        //debug("限价委托,未成交数量超过当前持仓", QSEnumDebugLevel.INFO);
                        debug("Order rejected by[Order FlatSize Check]", QSEnumDebugLevel.INFO);
                        msg = (pos_size == 0 ? commentNoPositionForFlat : commentOverFlatPositionSize);
                        return false;
                    }
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
