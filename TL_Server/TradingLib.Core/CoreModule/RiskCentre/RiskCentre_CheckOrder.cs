﻿using System;
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

        //List<string> _cnLocalExchange = new List<string>();
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
            if (!TLCtxHelper.ModuleSettleCentre.IsNormal)
            {
                errortitle = "SETTLECENTRE_NOT_RESET";//结算中心异常
                needlog = false;
                return false;
            }

            //结算中心处于实时模式 才可以接受委托操作
            if (TLCtxHelper.ModuleSettleCentre.SettleMode != QSEnumSettleMode.LiveMode)
            {
                errortitle = "SETTLECENTRE_NOT_RESET";//结算中心异常
                needlog = false;
                return false;
            }

            //1.3检查结算中心是否处于结算状态 结算状态不接受任何委托
            if (TLCtxHelper.ModuleSettleCentre.IsInSettle)
            {
                errortitle = ConstErrorID.SETTLECENTRE_IN_SETTLE;//结算中心出入结算状态
                needlog = false;
                return false;
            }

            //2 清算中心检查
            //2.1检查清算中心是否出入接受委托状态(正常工作状态下系统会定时开启和关闭清算中心,如果是开发模式则可以通过手工来提前开启)
            if (TLCtxHelper.ModuleClearCentre.Status != QSEnumClearCentreStatus.CCOPEN)
            {
                errortitle = "CLEARCENTRE_CLOSED";//清算中心已关闭
                needlog = false;
                return false;
            }

            //3 合约检查
            //3.1合约是否存在
            if (!account.TrckerOrderSymbol(ref o))
            {
                errortitle = ConstErrorID.SYMBOL_NOT_EXISTED;//合约不存在
                needlog = false;
                return false;
            }

            //3.2合约是否可交易
            if (!o.oSymbol.IsTradeable)//合约不可交易
            {
                errortitle = ConstErrorID.SYMBOL_NOT_TRADEABLE;//合约不可交易
                needlog = false;
                return false;
            }

            int exday = o.oSymbol.SecurityFamily.Exchange.GetExchangeTime().ToTLDate();
            if (o.oSymbol.IsExpired(exday))
            {
                errortitle = ConstErrorID.SYMBOL_EXPIRED;//合约不可交易
                needlog = false;
                return false;
            }

            if (o.oSymbol.SecurityFamily.Currency != account.Currency)
            {
                errortitle = "SYMBOL_NOT_TRADEABLE";//合约不可交易
                needlog = false;
                return false;
            }

            //交易时间检查
            int settleday = 0;
            QSEnumActionCheckResult result = o.oSymbol.SecurityFamily.CheckPlaceOrder(out settleday);
            if (result != QSEnumActionCheckResult.Allowed)
            {
                errortitle = ConstErrorID.SYMBOL_NOT_MARKETTIME;
                needlog = false;
                return false;
            }
            //设定交易日
            o.SettleDay = settleday;

            //特定交易日判定
            if (!o.oSymbol.SecurityFamily.CheckSpecialHoliday())
            {
                errortitle = ConstErrorID.SYMBOL_NOT_MARKETTIME;
                needlog = false;
                return false;
            }

            #region demo

            {
                ////交易所时间
                //DateTime extime = o.oSymbol.SecurityFamily.Exchange.ConvertToExchangeTime(DateTime.Now);
                //int span = o.oSymbol.SecurityFamily.Exchange.CloseTime - extime.ToTLTime();
                //if (span > 0 && span < GlobalConfig.FlatTimeAheadOfMarketClose * 60)
                //{

                //}
            }
            #endregion



            //中金所限制 开仓数量不能超过10手
            if (_cffexLimit)
            {
                Order tmp = o;
                //如果委托为开仓委托 且是股指
                if (o.IsEntryPosition && new string[] { "IF", "IH", "IC" }.Contains(tmp.oSymbol.SecurityFamily.Code))
                {
                    //累加所有该品种的开仓成交数量
                    int entry_size = account.Trades.Where(f => f.oSymbol.SecurityFamily.Code == tmp.oSymbol.SecurityFamily.Code).Where(f => f.IsEntryPosition).Sum(f => f.UnsignedSize);
                    //int pending_size = account.GetPendingEntrySize(
                    if (entry_size + o.UnsignedSize > 10)
                    {
                        errortitle = "SYMBOL_NOT_TRADEABLE";//合约不可交易
                        return false;
                    }
                }
            }

            //开仓标识与锁仓权限检查
            //4.1自动开平标识识别
            bool havelong = account.GetHaveLongPosition(o.Symbol);
            bool haveshort = account.GetHaveShortPosition(o.Symbol);
            //自动判定开平标识 这里不区分平今还是平昨
            if (o.OffsetFlag == QSEnumOffsetFlag.UNKNOWN)
            {
                if (havelong && haveshort)
                {
                    o.OffsetFlag = QSEnumOffsetFlag.CLOSE;//如果同时持有多空两个方向的持仓 则自动判定为平仓
                }
                else if (havelong)//多头
                {
                    if (o.Side)
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
                    if (o.Side)
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
                logger.Info("Order offsetFlag unknown,auto detected to:" + o.OffsetFlag.ToString());
            }


            //4.2检查锁仓方向
            //获得委托持仓操作方向

            //开仓操作
            if (o.IsEntryPosition)
            {
                bool orderside = o.PositionSide;
                //反向待成交开仓委托
                bool othersideentry = account.GetPendingEntrySize(o.Symbol, !orderside) > 0;

                //委托多头开仓操作,同时又空头头寸 或者 委托空头开仓操作，同时又有多头头寸 则表明在持有头寸的时候进行了反向头寸的操作
                if (othersideentry || (orderside && haveshort) || ((!orderside) && havelong))//多头持仓操作
                {
                    //非期货品种无法进行锁仓操作 同时帐户设置是否允许锁仓操作
                    if ((o.oSymbol.SecurityType != SecurityType.FUT) || (!account.GetParamPositionLock()))
                    {
                        errortitle = ConstErrorID.POSITION_LOCK_FORBIDDEN;
                        return false;
                    }
                }

                //卖空检查
                switch (o.oSymbol.SecurityType)
                { 
                    case SecurityType.STK:
                        if (o.Side) break;
                        errortitle = ConstErrorID.ORDER_SHORT_FORBIDDEN;
                        return false;
                    default:
                        break;
                }
            }


            //5.委托数量检查
            //5.1委托总数不为0
            if (o.TotalSize == 0)
            {
                errortitle = ConstErrorID.ORDER_SIZE_ZERO;//委托数量为0
                return false;
            }
            //5.2单次开仓数量小于设定值
            if (o.IsEntryPosition)//开仓
            {
                if (Math.Abs(o.TotalSize) > _orderlimitsize)
                {
                    errortitle = ConstErrorID.ORDER_SIZE_LIMIT;//委托数量超过最大委托手数
                    return false;
                }
            }



            //6.委托价格检查
            //6.1查看数据通道是否有对应的合约价格 行情是否正常

            //6.委托价格检查
            //6.1连续竞价阶段需要检查合约有效性是否有正常的价格 集合竞价阶段 该合约可能还没有行情
            //if (periodContinuous)
            {
                Tick tk = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(o.Exchange,o.Symbol);
                if (tk == null || (!tk.IsValid()))
                {
                    errortitle = "SYMBOL_TICK_ERROR";//市场行情异常
                    return false;
                }

                //6.2检查价格是否在涨跌幅度内
                if (o.isLimit || o.isStop)
                {
                    //decimal targetprice = o.isLimit ? o.LimitPrice : o.StopPrice;
                    //if (targetprice > tk.UpperLimit || targetprice < tk.LowerLimit)
                    //{
                    //    errortitle = "ORDERPRICE_OVERT_LIMIT";//保单价格超过涨跌幅
                    //    return false;
                    //}
                }
            }
            //6.2检查价格是否在涨跌幅度内
            if (o.isLimit || o.isStop)
            {
                decimal targetprice = o.isLimit ? o.LimitPrice : o.StopPrice;
                Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(o.Exchange,o.Symbol);
                //如果行情快照中包含有效的最高价和最低价限制 则判定价格是否在涨跌幅度内
                if (k.UpperLimit.ValidPrice() && k.LowerLimit.ValidPrice())
                {
                    if (targetprice > k.UpperLimit || targetprice < k.LowerLimit)
                    {
                        errortitle = ConstErrorID.ORDER_PRICE_LIMIT;//保单价格超过涨跌幅
                        return false;
                    }
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
                //debug("检查委托 " + o.id.ToString(), QSEnumDebugLevel.INFO);
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
                        logger.Warn("Order rejected by [Execute Check]" + o.GetOrderInfo());
                        return false;
                    }
                }

                //检查合约交易时间段
                if (_marketopencheck)
                {
                    //日内交易检查
                    if ((!inter) && account.IntraDay)//非内部委托并且帐户是日内交易帐户 则系统强迫前5秒 不允许交易
                    {
                        //交易所时间
                        DateTime extime = o.oSymbol.SecurityFamily.Exchange.ConvertToExchangeTime(DateTime.Now);
                        int span = o.oSymbol.SecurityFamily.Exchange.CloseTime - extime.ToTLTime();
                        if (span>0 && span < (GlobalConfig.FlatTimeAheadOfMarketClose*60 + 5))
                        {
                            msg = "系统执行强平,日内帐户禁止交易";
                            logger.Warn("Order reject by [IntraDay Check]" + o.GetOrderInfo());
                            return false;
                        }
                        //如果是强平时间段则不可交易 
                        //if (o.oSymbol.SecurityFamily.cl)
                        //{
                        //    msg = "日内交易帐户，系统正在强平，无法处理委托！";
                        //    logger.Info("Order rejected by [FlatTime Check] not in [intraday] trading time" + o.GetOrderInfo());
                        //    return false;
                        //}
                    }
                }

                

                //3.合约交易权限检查(如果帐户存在特殊服务,可由特殊服务进行合约交易权限检查) 有关特殊服务的主力合约控制与检查放入到对应的特殊服务实现中
                //if (!inter)
                {
                    if (!account.CanTakeSymbol(o.oSymbol, out msg))
                    {
                        logger.Info("Order rejected by[Account CanTakeSymbol Check]" + o.GetOrderInfo());
                        return false;
                    }
                }


                //4 委托开仓 平仓项目检查
                Position pos = account.GetPosition(o.Symbol, o.PositionSide);//当前对应持仓
                bool entryposition = o.IsEntryPosition;
                if (entryposition)//开仓执行资金检查
                {
                    if (!account.CanFundTakeOrder(o, out msg))
                    {
                        logger.Info("Order rejected by[Order Margin Check]" + o.GetOrderInfo());
                        return false;
                    }
                }
                else//平仓执行数量检查 
                {

                    //当前委托数量
                    int osize = o.UnsignedSize;
                    switch (o.oSymbol.SecurityType)
                    {
                       
                        case SecurityType.FUT:
                            #region 期货平仓检查
                            {
                                //如果是上期所的期货品种 需要检查今仓和昨仓
                                if (o.oSymbol.SecurityFamily.Exchange.EXCode.Equals("SHFE"))
                                {
                                    /* 上期所
                                     * 平今 必须用CloseToday
                                     * 平昨 必须用Close/CloseYestoday
                                     * 检查开平标识 如果不是Close则报错
                                     * */
                                    switch (o.OffsetFlag)
                                    {
                                        case QSEnumOffsetFlag.CLOSETODAY:
                                        case QSEnumOffsetFlag.CLOSEYESTERDAY:
                                        case QSEnumOffsetFlag.CLOSE:
                                            break;
                                        default:
                                            msg = "委托字段有误";
                                            return false;
                                    }

                                    int voltd = pos.PositionDetailTodayNew.Sum(p => p.Volume);//今日持仓
                                    int volyd = pos.PositionDetailYdNew.Sum(p => p.Volume);//昨日持仓

                                    switch (o.OffsetFlag)
                                    {
                                        //平今
                                        case QSEnumOffsetFlag.CLOSETODAY:
                                            {
                                                int pendingExitSizeCloseToday = account.GetPendingExitOrders(o.Symbol, o.PositionSide).Where(o1 => o1.OffsetFlag == QSEnumOffsetFlag.CLOSETODAY).Sum(o1 => o1.UnsignedSize);
                                                logger.Info(string.Format("position today:{0}  pendingexist closetoday order size:{1} ordersize:", voltd, pendingExitSizeCloseToday));
                                                if (voltd < pendingExitSizeCloseToday + osize)
                                                {
                                                    logger.Info("Order rejected by[Order FlatSize Check]" + o.GetOrderInfo());
                                                    msg = (voltd == 0 ? "无可平今仓" : "可平今仓数量不足");
                                                    return false;
                                                }
                                            }
                                            break;
                                        //平昨
                                        case QSEnumOffsetFlag.CLOSE:
                                        case QSEnumOffsetFlag.CLOSEYESTERDAY:
                                            {
                                                int pendingExitSizeCloseYestoday = account.GetPendingExitOrders(o.Symbol, o.PositionSide).Where(o1 => o1.OffsetFlag == QSEnumOffsetFlag.CLOSEYESTERDAY || o1.OffsetFlag == QSEnumOffsetFlag.CLOSE).Sum(o1 => o1.UnsignedSize);
                                                logger.Info(string.Format("position yestoday:{0}  pendingexist closeyestoday order size:{1} ordersize:", volyd, pendingExitSizeCloseYestoday));
                                                if (volyd < pendingExitSizeCloseYestoday + osize)
                                                {
                                                    logger.Info("Order rejected by[Order FlatSize Check]" + o.GetOrderInfo());
                                                    msg = (volyd == 0 ? "无可平昨仓" : "可平昨仓数量不足");
                                                    return false;
                                                }
                                                break;
                                            }
                                        default:
                                            msg = "委托字段有误";
                                            return false;

                                    }
                                }
                                else
                                {
                                    //其余交易所 平仓标识均统一成Close
                                    switch (o.OffsetFlag)
                                    {
                                        case QSEnumOffsetFlag.CLOSETODAY:
                                        case QSEnumOffsetFlag.CLOSEYESTERDAY:
                                        case QSEnumOffsetFlag.CLOSE:
                                            o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                                            break;
                                        default:
                                            msg = "委托字段有误";
                                            return false;
                                    }
                                    //当前持仓数量
                                    int pos_size = pos.UnsignedSize;
                                    //获得该帐户 该合约 该持仓方向的待成交平仓委托
                                    int pendingExitSize = account.GetPendingExitSize(o.Symbol, o.PositionSide);
                                    //debug("Order try to exit postion,pos size:" + pos.Size.ToString() + " pending exit size:" + pendingExitSize.ToString() + " osize:" + osize.ToString() + " offsetflag:" + o.OffsetFlag.ToString(), QSEnumDebugLevel.INFO);
                                    if (pos_size < pendingExitSize + osize)
                                    {
                                        //debug("限价委托,未成交数量超过当前持仓", QSEnumDebugLevel.INFO);
                                        logger.Info("Order rejected by[Order FlatSize Check]" + o.GetOrderInfo());
                                        msg = (pos_size == 0 ? commentNoPositionForFlat : commentOverFlatPositionSize);
                                        return false;
                                    }
                                }
                                break;
                            }
                            #endregion
                        
                        case SecurityType.STK:
                            #region 股票平仓检查
                            {
                                switch (o.OffsetFlag)
                                {
                                    case QSEnumOffsetFlag.CLOSETODAY:
                                    case QSEnumOffsetFlag.CLOSEYESTERDAY:
                                    case QSEnumOffsetFlag.CLOSE:
                                        o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                                        break;
                                    default:
                                        msg = "委托字段有误";
                                        return false;
                                }
                                int canFlatSize = 0;
                                int voltd = pos.PositionDetailTodayNew.Sum(p => p.Volume);//今日持仓
                                int volyd = pos.PositionDetailYdNew.Sum(p => p.Volume);//昨日持仓
                                //这里加入系统日内判定，如果允许进行股票T+0交易 则总可平数量包含今仓数量
                                canFlatSize = _enableStkT0 ? volyd + voltd : volyd;
                                int pendingExitSize = account.GetPendingExitSize(o.Symbol, o.PositionSide);
                                if (canFlatSize < pendingExitSize + osize)
                                {
                                    logger.Info("Order rejected by[Order FlatSize Check]" + o.GetOrderInfo());
                                    msg = (canFlatSize == 0 ? commentNoPositionForFlat : commentOverFlatPositionSize);
                                    return false;
                                }
                                break;
                            }
                            #endregion

                        default:
                            break;
                    }
                }


                

                //debug("riskcentre run to here", QSEnumDebugLevel.INFO);
                //5.账号所应用的风控规则检查 内部委托不执行帐户个性的自定义检查
                if (!inter)
                {
                    if (!account.CheckOrder(o, out msg))//如果通过风控检查 则置委托状态为Placed
                    {
                        logger.Info("Order rejected by[Order Rule Check]" + o.GetOrderInfo());
                        
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
                logger.Error(s);
                return false;
            }
        }
        #endregion

    }
}
