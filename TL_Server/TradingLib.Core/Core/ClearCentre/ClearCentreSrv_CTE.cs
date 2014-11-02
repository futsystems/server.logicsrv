using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

using TradingLib.LitJson;

namespace TradingLib.Core
{
    /// <summary>
    /// 定时任务 被TaskCentre调用的定时执行的操作
    /// </summary>
    public partial class ClearCentre
    {
        #region 【帐户参数修改】

        #region 添加或删除帐户
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "addaccount", "addaccount - 某个用户添加某个类别的交易帐号", "某个用户添加某个类别的交易帐号")]
        [CoreCommandAttr(QSEnumCommandSource.CLI,"addaccount","addaccount - clearcentre add a new account","清算中心增加一个交易帐号")]
        [MethodArgument("", QSEnumMethodArgumentType.Integer, 1, "UCenter用户id")]
        [MethodArgument("", QSEnumMethodArgumentType.String, 2, "指定交易帐户号码,如果采用系统递增的号码则为空,指定的号码不能以9开头")]
        [MethodArgument("", QSEnumMethodArgumentType.String, 3, "指定交易帐户密码,如果采用随机Miami,则设置为空")]
        [MethodArgument("", QSEnumMethodArgumentType.Enum, 4, "交易帐户类别类别 DEALER(交易员) SIMULATION(模拟帐号) REAL(实盘帐号)")]
        public string CTE_AddAccount(int userid,string setaccount,string pass,QSEnumAccountCategory type)
        {
            string npass=pass;
            if (string.IsNullOrEmpty(pass))
            {
                npass = ExUtil.GenAccountPass();
            }
            string account = null;
            bool re = this.AddAccount(out account,userid.ToString(),setaccount,npass, type);
            if (re)
            {
                JsonWriter w = ReplyHelper.NewJWriterSuccess();
                w.WritePropertyName("Account");
                w.Write(account);
                ReplyHelper.EndWriter(w);
                return w.ToString();
            }

            return ReplyHelper.Error_ServerSide;
        }


        [CoreCommandAttr(QSEnumCommandSource.CLI, "addbatchacc", "addbatchacc - clearcentre add batch accounts", "清算中心增加一批交易帐号")]
        public string CTE_AddAccountCli()
        {
            string npass = "123456";
            string account = null;
            bool re = this.AddAccount(out account,"0","", npass,QSEnumAccountCategory.DEALER);
            if (!string.IsNullOrEmpty(account))
            {
                JsonWriter w = ReplyHelper.NewJWriterSuccess();
                w.WritePropertyName("Account");
                w.Write(account);
                w.WritePropertyName("Pass");
                w.Write(npass);
                ReplyHelper.EndWriter(w);
                return w.ToString();
            }

            return ReplyHelper.Error_ServerSide;
        }


        

        #endregion


        #region 修改帐户属性
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "updateacccategory", "updateacccategory - 更新某个交易帐号的帐户类别", "更新某个交易帐号的帐户类别")]
        public void CTE_UpdateAccountCategory(string account,QSEnumAccountCategory category)
        {
            this.UpdateAccountCategory(account, category);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "updateaccroute", "updateaccroute - 更新某个交易帐号的路由类别", "更新某个交易帐号的路由类别")]
        public void CTE_UpdateAccountRoute(string account, QSEnumOrderTransferType type)
        {
            debug("update account:" + account + " execution:" + type.ToString(), QSEnumDebugLevel.INFO);
            this.UpdateAccountRouterTransferType(account, type);
        }

      

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "updateaccexecute", "updateaccexecute - 激活或冻结某个交易帐户", "激活或冻结某个交易帐户")]
        public void CTE_UpdateAccountExecution(string account, bool active)
        {
            debug("update account:" + account + " execution:" + active.ToString(), QSEnumDebugLevel.INFO);
            if (active)
            {
                this.ActiveAccount(account);
            }
            else
            {
                this.InactiveAccount(account);
            }
        }
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "updateaccintraday", "updateaccintraday - 激活或冻结某个交易帐户", "激活或冻结某个交易帐户")]
        public void CTE_UpdateAccountIntraday(string account, bool intraday)
        {
            debug("update account:" + account + " intraday:" + intraday.ToString(), QSEnumDebugLevel.INFO);
            this.UpdateAccountIntradyType(account, intraday);
        }

        #endregion

        #region 出入金操作
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "cashoperation", "cashoperation - 出入金操作", "出入金操作")]
        public void CTE_Cash(string account,decimal amount,string comment)
        {
            this.CashOperation(account, amount,"", comment);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "resetequity", "resetequity - 重置某个交易帐户资金至多少资金", "重置某个交易帐户资金至多少资金")]

        public void CTE_ResetEquity(string account, decimal amaount)
        { 
        
        }


        #endregion

        #endregion

        #region 【web端操作或命令行操作】
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryaccount", "qryaccount - qryaccount", "查找某个交易帐号并返回该交易帐号的相关信息")]
        [MethodArgument("", QSEnumMethodArgumentType.String,1, @"交易帐号 返回信息返回定义
        public string Account { get { return _acc.ID; } }//帐户ID
        public decimal LastEquity { get { return _acc.LastEquity; } }//昨日权益
        public decimal NowEquity { get { return _acc.NowEquity; } }//当前动态权益

        public decimal RealizedPL { get { return _acc.RealizedPL; } }//平仓盈亏
        public decimal UnRealizedPL { get { return _acc.UnRealizedPL; } }//浮动盈亏
        public decimal Commission { get { return _acc.Commission; } }//手续费
        public decimal Profit { get { return _acc.Profit; } }//净利
        public decimal CashIn { get { return _acc.Profit; } }//入金
        public decimal CashOut { get { return _acc.CashOut; } }//出金
        public decimal MoneyUsed { get { return _acc.MoneyUsed; } } //总资金占用
        public decimal TotalLiquidation { get { return _acc.TotalLiquidation; } }//帐户总净值
        public decimal AvabileFunds { get { return _acc.AvabileFunds; } }//帐户总可用资金


        public QSEnumAccountCategory Category { get { return _acc.Category; } }//账户类别
        public QSEnumOrderTransferType OrderRouteType { get {return  _acc.OrderRouteType; } }//路由类别
        public bool Execute { get { return _acc.Execute; } }//冻结
        public bool IntraDay { get { return _acc.IntraDay; } }//日内



        #region 多品种交易 账户财务数据
        public decimal FutMarginUsed { get { return _acc.FutMarginUsed; } }//期货占用保证金
        public decimal FutMarginFrozen { get { return _acc.FutMarginFrozen; } }//期货冻结保证金
        public decimal FutRealizedPL { get { return _acc.FutRealizedPL; } }//期货平仓盈亏
        public decimal FutUnRealizedPL { get { return _acc.FutUnRealizedPL; } }//期货浮动盈亏
        public decimal FutCommission { get { return _acc.FutCommission; } }//期货交易手续费
        public decimal FutCash { get { return _acc.FutCash; } }//期货交易现金
        public decimal FutLiquidation { get { return _acc.FutLiquidation; } }//期货总净值
        public decimal FutMoneyUsed { get { return _acc.FutMoneyUsed; } }//期货资金占用
        public decimal FutAvabileFunds { get { return _acc.FutAvabileFunds; } }


        public decimal OptPositionCost { get { return _acc.OptPositionCost; } }//期权持仓成本
        public decimal OptPositionValue { get { return _acc.OptPositionValue; } }//期权持仓市值
        public decimal OptRealizedPL { get { return _acc.OptRealizedPL; } }//期权平仓盈亏
        public decimal OptCommission { get { return _acc.OptCommission; } }//期权交易手续费
        public decimal OptMoneyFrozen { get { return _acc.OptMoneyFrozen; } }//期权资金冻结
        public decimal OptCash { get { return _acc.OptCash; } }//期权交易现金
        public decimal OptMarketValue { get { return _acc.OptMarketValue; } }//期权总市值
        public decimal OptLiquidation { get { return _acc.OptLiquidation; } }//期权总净值
        public decimal OptMoneyUsed { get { return _acc.OptMoneyUsed; } }//期权资金占用
        public decimal OptAvabileFunds { get { return _acc.OptAvabileFunds; } }

        public decimal InnovPositionCost { get { return _acc.InnovPositionCost; } }//异化合约持仓成本
        public decimal InnovPositionValue { get { return _acc.InnovPositionValue; } }//异化合约持仓市值
        public decimal InnovCommission { get { return _acc.InnovCommission; } }//异化合约手续费
        public decimal InnovRealizedPL { get { return _acc.InnovRealizedPL; } }//异化合约平仓盈亏
        public decimal InnovMargin { get { return _acc.InnovMargin; } }//异化合约保证金
        public decimal InnovMarginFrozen { get { return _acc.InnovMarginFrozen; } }//异化合约冻结


        public decimal InnovCash { get { return _acc.InnovCash; } }//异化合约现金流
        public decimal InnovMarketValue { get { return _acc.InnovMarketValue; } }//异化合约市值
        public decimal InnovLiquidation { get { return _acc.InnovLiquidation; } }//异化合约净值
        public decimal InnovMoneyUsed { get { return _acc.InnovMoneyUsed; } }//异化合约资金占用
        public decimal InnovAvabileFunds { get { return _acc.InnovAvabileFunds; } }//异化合约可用资金
        #endregion")]
        public string QryAccount(string account)
        {
            IAccount acc = this[account];
            if (acc == null)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            else
            {
                JsonWriter w = ReplyHelper.NewJWriterSuccess();
                ReplyHelper.FillJWriter(new JsonWrapperAccount(acc), w);
                ReplyHelper.EndWriter(w);

                return w.ToString();
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "accfut", "accfut - accfut", "查找某个交易帐号期货信息")]
        public string CTE_QryAccountFut(string account)
        {
            IAccount acc = this[account];
            if (acc == null)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            else
            {
                return string.Format("Account:{0} UnrealizedPL:{1} SettleUnrealizedPL:{2}", acc.ID, acc.CalFutRealizedPL(), acc.CalFutSettleUnRealizedPL());
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryaccountlite", "qryaccountlite - qryaccountlite", "查找某个交易帐号返回总计信息")]
        public string QryAccountLite(string account)
        {
            IAccount acc = this[account];
            if (acc == null)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            else
            {
                JsonWriter w = ReplyHelper.NewJWriterSuccess();
                ReplyHelper.FillJWriter(new JsonWrapperAccountLite(acc), w);
                ReplyHelper.EndWriter(w);

                return w.ToString();
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "filteraccount", "filteraccount - filteraccount","按条件过滤交易帐号")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "filteraccount", "filteraccount - filteraccount", "按条件过滤交易帐号")]
        public string filterAccount(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return ReplyHelper.Error_ServerSide;
            }

            //1.生成对象过滤器
            AccountFilter f = ObjectFilter.Create<AccountFilter>(filter);

            //2.通过linq表达式获得我们需要筛选的对象
            IEnumerable<JsonWrapperAccount> list =
            from acc in acctk.Accounts
            where f.Match(acc)
            select new JsonWrapperAccount(acc);

            //2.生成json对象返回
            JsonWriter w = ReplyHelper.NewJWriterSuccess();
            ReplyHelper.FillJWriter(list.ToArray(),w);
            ReplyHelper.EndWriter(w);

            return w.ToString();
        }




        /// <summary>
        /// 开启清算中心
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "opencc", "opencc - Open ClearCentre", "开启交易中心,接收客户端提交的委托")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "opencc", "opencc - Open ClearCentre", "开启交易中心,接受客户端提交的委托")]
        public void EXCH_OpenClearCentre()
        {
            OpenClearCentre();
        }


        [ContribCommandAttr(QSEnumCommandSource.CLI, "closecc", "closecc - Close ClearCentre", "关闭交易中心,拒绝收客户端提交的委托")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "closecc", "closecc - Close ClearCentre", "关闭交易中心,拒绝客户端提交的委托")]
        public void EXCH_CloseClearCentre()
        {
            CloseClearCentre();
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryccstatus", "qryccstatus - Query Status of clearcentre", "查看清算中心状态")]
        public string CTE_QryClearCentre()
        {

            JsonWriter w = ReplyHelper.NewJWriterSuccess();
            ReplyHelper.FillJWriter(new JsonWrapperClearCentreStatus(this), w);
            ReplyHelper.EndWriter(w);

            return w.ToString();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "qryaccount", "qryaccount - 查询帐户信息", "查询帐户信息")]
        public string CLI_QryOrder(string account)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-------------------- Account:" + account + "------------------" + ExComConst.Line);
            IAccount acc = this[account];
            if (acc == null)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            else
            {
                JsonWriter w = ReplyHelper.NewJWriterSuccess();
                ReplyHelper.FillJWriter(new JsonWrapperAccount(acc), w);
                ReplyHelper.EndWriter(w);

                sb.Append(w.ToString() + ExComConst.Line);
                foreach (Order o in acc.Orders)
                {
                    sb.Append(o.ToString() + ExComConst.Line);
                }
            }

            return sb.ToString();
        }

        #endregion



        #region 命令行操作

        /// <summary>
        /// 显示某个委托情况
        /// </summary>
        /// <param name="id"></param>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "porder", "porder - print order information", "")]
        public string CTE_DisplayOrder(long id)
        {
            if (!totaltk.IsTracked(id))
            {
                return "委托不存在";
            }
            //return string.Format("委托:{0} pending:{1} canceled:{2} complete:{3} tracked:{4}", SentOrder(id).ToString(), totaltk.OrderTracker.isPending(id), totaltk.OrderTracker.isCanceled(id), totaltk.OrderTracker.isCompleted(id), totaltk.OrderTracker.isTracked(id));
            return string.Empty;
        }

        /// <summary>
        /// 显示关闭的尺长回合信息
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "prclose", "prclose - list all positionround opened", "")]
        public string CTP_PrintPositionRoundClosed()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("持仓回合(Closed)列表:" + Environment.NewLine);
            foreach (PositionRound p in prt.RoundClosed)
            {
                sb.Append(p.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 显示开启的持仓回合信息
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "propen", "propen - list all positionround opened", "")]
        public string CTE_PrintPositionRoundOpened()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("持仓回合(Opened)列表:" + Environment.NewLine);
            foreach (PositionRound p in prt.RoundOpened)
            {
                sb.Append(p.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "poslast", "poslast - list all postion lastday", "")]
        public string CTE_PrintPositionsHoldLast()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("上次结算持仓:" + Environment.NewLine);
            //foreach (Position pos in this.TotalYdPositions)
            //{
            //    sb.Append(pos.ToString() + Environment.NewLine);
            //}
            return sb.ToString();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "posnow", "posnow - list all postion now", "")]
        public string CTE_PrintPositionsHoldNow()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("当前持仓:" + Environment.NewLine);
            foreach (Position pos in this.TotalPositions)
            {
                sb.Append(pos.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }



        [ContribCommandAttr(QSEnumCommandSource.CLI, "pacclist", "pacclist - list all the account", "")]
        public string CTE_PrintAccountList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (IAccount acc in acctk.Accounts)
            {
                sb.Append(acc.ID.PadRight(10,' ') + acc.UserID.ToString());
            }
            return sb.ToString();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "accpos", "accfin - print out account finance infod", "")]
        public string CTE_PrintAccountInfo(string id)
        {

            IAccount acc = this[id];
            if (acc != null)
            {
                string re = "";
                StringBuilder sb = new StringBuilder();
                foreach (Position pos in acc.Positions)
                {
                    sb.Append(pos.ToString() + Environment.NewLine);
                }

                return re + sb.ToString();
            }
            return "";
        }


        [ContribCommandAttr(QSEnumCommandSource.CLI, "posdetail", "posdetail - print out position details", "")]
        public string CTE_PrintPosDetails(string id)
        {
            IAccount acc = this[id];
            if (acc != null)
            {
                string re = "";
                StringBuilder sb = new StringBuilder();
                foreach (Position pos in acc.Positions)
                {
                    sb.Append("持仓:"+pos.GetPositionKey() +" "+pos.ToString() + Environment.NewLine);
                    //sb.Append("昨日持仓明细" + Environment.NewLine);
                    //foreach (PositionDetail p in pos.PositionDetailYdRef)
                    //{
                    //    sb.Append(p.GetPositionDetailStr() + Environment.NewLine);
                    //}

                    //IEnumerable<PositionCloseDetail> closedetail = null;
                    //IEnumerable<PositionDetail> nowdetails = pos.CalPositionDetail(out closedetail);

                    //sb.Append("当前持仓明细" + Environment.NewLine);
                    //foreach (PositionDetail p in nowdetails)
                    //{
                    //    sb.Append(p.GetPositionDetailStr() + Environment.NewLine);
                    //}

                    //sb.Append("当日平仓明细" + Environment.NewLine);

                    //foreach (PositionCloseDetail p in closedetail)
                    //{
                    //    sb.Append(p.GetPositionCloseStr() + Environment.NewLine);
                    //}

                    //sb.Append("实时生成数据"+Environment.NewLine);
                    sb.Append("---------持仓明细--------------------------------------------------" + Environment.NewLine);
                    foreach (PositionDetail p in pos.PositionDetailTotal)
                    {
                        sb.Append(p.GetPositionDetailStr() + Environment.NewLine);
                        sb.Append(TradingLib.Mixins.LitJson.JsonMapper.ToJson(p) + Environment.NewLine);
                    }
                    sb.Append("Sum Size:" + pos.PositionDetailTotal.Where(p => !p.IsClosed()).Sum(pd => pd.Volume));
                    sb.Append(Environment.NewLine);
                    sb.Append("---------平仓明细--------------------------------------------------" + Environment.NewLine);
                    foreach (PositionCloseDetail p in pos.PositionCloseDetail)
                    {
                        sb.Append(p.GetPositionCloseStr() + Environment.NewLine);
                    }
                    sb.Append("------持仓汇总------------" + Environment.NewLine);
                    sb.Append(TradingLib.Mixins.LitJson.JsonMapper.ToJson(pos.GenPositionEx()) + Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                }
                return sb.ToString();
            }
            return "";
        }


        [ContribCommandAttr(QSEnumCommandSource.CLI, "accfutpos", "accfutpos - print out account fut postions", "")]
        public string CTE_Printfutpostions(string id)
        {

            IAccount acc = this[id];
            if (acc != null)
            {
                string re = "";
                StringBuilder sb = new StringBuilder();
                foreach (Position pos in acc.Positions.Where(p => p.oSymbol.SecurityType == SecurityType.FUT).ToArray())
                {
                    sb.Append(pos.ToString() + Environment.NewLine);
                }

                return re + sb.ToString();
            }
            return "";
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "symbol", "symbol - xxxxx print out symbol information", "")]
        public string CTE_PrintSymnbol(string symbol)
        {
            Symbol sym = BasicTracker.SymbolTracker[symbol];
            if (sym == null)
            {
                return "Symbol:" + symbol + " not exist";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Symbol:".PadRight(10, ' ') + sym.Symbol+ExComConst.Line);
                sb.Append("TickSymbol".PadRight(10, ' ') + sym.TickSymbol + ExComConst.Line);
                sb.Append("Type:".PadRight(10, ' ') + sym.SecurityType.ToString() + ExComConst.Line);
                sb.Append("Currency:".PadRight(10, ' ') + sym.Currency.ToString() + ExComConst.Line);
                sb.Append("Exchange:".PadRight(10, ' ') + sym.Exchange.ToString() + ExComConst.Line);
                sb.Append("Multiple:".PadRight(10, ' ') + sym.Multiple.ToString() + ExComConst.Line);
                sb.Append("Margin:".PadRight(10, ' ') + sym.Margin.ToString() + ExComConst.Line);
                sb.Append("ExtraMargin:".PadRight(10, ' ') + sym.ExtraMargin.ToString() + ExComConst.Line);
                sb.Append("MaintanceMargin:".PadRight(10, ' ') + sym.MaintanceMargin.ToString() + ExComConst.Line);
                sb.Append("EntyC:".PadRight(10, ' ') + sym.EntryCommission.ToString() + ExComConst.Line);
                sb.Append("ExitC:".PadRight(10, ' ') + sym.ExitCommission.ToString() + ExComConst.Line);
                return sb.ToString();
            }
        }
        #endregion
    }
}
