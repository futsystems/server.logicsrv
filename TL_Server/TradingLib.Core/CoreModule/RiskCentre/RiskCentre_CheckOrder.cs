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
        public bool CheckOrderStep1(ref Order o, ISession session, IAccount account, out bool needlog, out string errortitle, bool inter = false)
        {
            errortitle = string.Empty;
            needlog = true;

            #region 系统组件状态检查
            //结算中心处于实时模式 才可以接受委托操作
            if (TLCtxHelper.ModuleSettleCentre.SettleMode != QSEnumSettleMode.StandbyMode)
            {
                errortitle = "SETTLECENTRE_NOT_RESET";//结算中心异常
                needlog = false;
                return false;
            }

            //2 清算中心检查
            //2.1检查清算中心是否出入接受委托状态(正常工作状态下系统会定时开启和关闭清算中心,如果是开发模式则可以通过手工来提前开启)
            if (!TLCtxHelper.ModuleClearCentre.IsLive)
            {
                errortitle = "CLEARCENTRE_CLOSED";//清算中心已关闭
                needlog = false;
                return false;
            }

            #endregion

            #region 合约检查
            //合约是否存在
            if (!account.TrckerOrderSymbol(ref o))
            {
                errortitle = ConstErrorID.SYMBOL_NOT_EXISTED;//合约不存在
                needlog = false;
                return false;
            }

            //合约是否可交易
            if (!o.oSymbol.IsTradeable)//合约不可交易
            {
                errortitle = ConstErrorID.SYMBOL_NOT_TRADEABLE;//合约不可交易
                needlog = false;
                return false;
            }
            //合约过期
            int exday = o.oSymbol.SecurityFamily.Exchange.GetExchangeTime().ToTLDate();
            if (o.oSymbol.IsExpired(exday))
            {
                errortitle = ConstErrorID.SYMBOL_EXPIRED;//合约不可交易
                needlog = false;
                return false;
            }


            //交易时间段检查 此处设定委托交易日
            int settleday = 0;
            QSEnumActionCheckResult result = o.oSymbol.SecurityFamily.CheckPlaceOrder(out settleday);//判定当前是否处于品种交易时间段内
            if (result != QSEnumActionCheckResult.Allowed)
            {
                errortitle = ConstErrorID.SYMBOL_NOT_MARKETTIME;
                needlog = false;
                return false;
            }
            //设定交易日
            o.SettleDay = settleday;

            //特定交易日判定
            //if (!o.oSymbol.SecurityFamily.CheckSpecialHoliday())
            //{
            //    errortitle = ConstErrorID.SYMBOL_NOT_MARKETTIME;
            //    needlog = false;
            //    return false;
            //}

            //临时禁止品种
            if (blockSecCodeList.Count > 0)
            { 
                var code = o.oSymbol.SecurityFamily.Code;
                if (blockSecCodeList.Any(item => item == code))
                {
                    errortitle = ConstErrorID.SYMBOL_NOT_TRADEABLE;//合约不可交易
                    needlog = false;
                    return false;
                }
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

            #region 锁仓与卖空检查
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

            //开仓操作 检查锁仓与卖空
            if (o.IsEntryPosition)
            {
                bool orderside = o.PositionSide;
                //反向待成交开仓委托
                bool othersideentry = account.GetPendingEntrySize(o.Symbol, !orderside) > 0;

                //委托多头开仓操作,同时又空头头寸 或者 委托空头开仓操作，同时又有多头头寸 则表明在持有头寸的时候进行了反向头寸的操作
                if (othersideentry || (orderside && haveshort) || ((!orderside) && havelong))//多头持仓操作
                {
                    //非期货品种无法进行锁仓操作 同时帐户设置是否允许锁仓操作
                    if ((o.oSymbol.SecurityType != SecurityType.FUT) || (!account.GetParamPositionLock(o.oSymbol.SecurityFamily)))
                    {
                        errortitle = ConstErrorID.POSITION_LOCK_FORBIDDEN;
                        return false;
                    }
                }

                //股票卖空检查
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
            #endregion

            #region 委托数量检查
            //5.1委托总数不为0
            if (o.TotalSize == 0)
            {
                errortitle = ConstErrorID.ORDER_SIZE_ZERO;//委托数量为0
                return false;
            }

            //5.2单次开仓数量小于设定值
            if (o.IsEntryPosition)//开仓
            {
                if (o.oSymbol.SecurityType == SecurityType.FUT)
                {
                    if (Math.Abs(o.TotalSize) > _orderlimitsize)
                    {
                        errortitle = ConstErrorID.ORDER_SIZE_LIMIT;//委托数量超过最大委托手数
                        return false;
                    }
                }

                if (o.oSymbol.SecurityType == SecurityType.STK)
                {
                    if (Math.Abs(o.TotalSize) / 100 < 1 || Math.Abs(o.TotalSize) % 100 !=0)
                    {
                        errortitle = ConstErrorID.STK_ENTRYSIZE_100;
                        return false;
                    }
                }
            }

            #endregion

            #region 报单价格检查
            Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(o.Exchange,o.Symbol);
            if (k == null || (!k.IsValid()))
            {
                errortitle = "SYMBOL_TICK_ERROR";//市场行情异常
                return false;
            }

            
            if (o.isLimit || o.isStop)
            {
                decimal targetprice = o.isLimit ? o.LimitPrice : o.StopPrice;
                //如果行情快照中包含有效的最高价和最低价限制 则判定价格是否在涨跌幅度内
                if (k.UpperLimit.ValidPrice() && k.LowerLimit.ValidPrice())
                {
                    if (targetprice > k.UpperLimit || targetprice < k.LowerLimit)
                    {
                        errortitle = ConstErrorID.ORDER_PRICE_LIMIT;//保单价格超过涨跌幅
                        return false;
                    }
                }

                //涨停 不允许买入
                if (k.UpperLimit.ValidPrice() && k.Trade == k.UpperLimit)
                {
                    if (o.Side)
                    {
                        errortitle = "RISKCENTRE_CHECK_ERROR";
                        return false;
                    }
                }

                //跌停 不允许卖出
                if (k.LowerLimit.ValidPrice() && k.Trade == k.LowerLimit)
                {
                    if (!o.Side)
                    {
                        errortitle = "RISKCENTRE_CHECK_ERROR";
                        return false;
                    }
                }

            }
            #endregion

            #region 日内账户检查
            //日内账户收盘强平前5分钟禁止开仓 需要先判定unkonwn offset 否则无法准确判断EntryPosition
            if (account.IntraDay && o.IsEntryPosition)
            {
                //交易所时间
                DateTime extime = o.oSymbol.SecurityFamily.Exchange.ConvertToExchangeTime(DateTime.Now);
                int span = o.oSymbol.SecurityFamily.Exchange.CloseTime - extime.ToTLTime();
                if (span > 0 && span < (GlobalConfig.FlatTimeAheadOfMarketClose * 60 + 5))
                {
                    errortitle = ConstErrorID.SYMBOL_NOT_MARKETTIME;
                    needlog = false;
                    return false;
                }
            }
            #endregion


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
        public bool CheckOrderStep2(ref Order o, ISession session, IAccount account, out string msg, bool inter = false)
        {
            try
            {
                msg = "";
                //委托开仓 平仓项目检查
                Position pos = account.GetPosition(o.Symbol, o.PositionSide);//当前对应持仓
                if (o.IsEntryPosition)//开仓执行资金检查
                {
                    if (!account.CheckEquityAdequacy(o, out msg))
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
                                bool ismobile = false;
                                if (session != null && session.ProductInfo.StartsWith("M."))
                                {
                                    ismobile = true;
                                }
                                //如果是上期所的期货品种 需要检查今仓和昨仓 手机端直接执行先开先平操作
                                if ( (!ismobile)&& Util.IsCloseOffsetFlagDiff(o.oSymbol.SecurityFamily.Exchange.EXCode))
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

                //内部委托(管理端) 不执行下列检查
                if (!inter)
                {
                    //交易账户被冻结 禁止交易
                    if (!account.Execute)
                    {
                        msg = "账户被冻结";
                        logger.Warn("Order rejected by [Execute Check]" + o.GetOrderInfo());
                        return false;
                    }

                    //投资者账户销户
                    if (account.Deleted)
                    {
                        msg = "交易帐户销户中";
                        logger.Warn("Order rejected by [Deleted Check]" + o.GetOrderInfo());
                        return false;
                    }

                    //代理账户
                    Manager mgr = BasicTracker.ManagerTracker[account.Mgr_fk];
                    while (mgr != null && mgr.Type != QSEnumManagerType.ROOT)
                    {
                        if (mgr.AgentAccount.Freezed)
                        {
                            msg = "会员结算账户冻结";
                            logger.Warn("Order rejected by [AgentFreezed Check]" + o.GetOrderInfo());
                            return false;
                        }
                        mgr = mgr.ParentManager;
                    }

                    //执行账号风控规则检查
                    if(!CheckOrderRule(o,out msg))
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
