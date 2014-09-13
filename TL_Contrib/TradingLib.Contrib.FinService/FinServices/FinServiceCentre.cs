using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;

using System.Data;

namespace TradingLib.Contrib
{
    /// <summary>
    /// 配子服务中心,用于生成,修改,加载配子服务
    /// finservices表格记录了对应的配子服务信息,系统初始化时候从该表加载对应的服务
    /// 1.配资代理创建交易帐号,并按照客户要求为该帐号添加对应的配资服务
    /// 2.客户入金以后,对应的配资服务自动生效
    /// 3.客户停止配资服务,则24小时后该服务失效.
    /// </summary>
     [ContribAttr("FinService","配资服务模块", "配资服务用于给系统提供配资服务功能模块,配资服务可以给某个配资帐号绑定配资服务,配资服务是给该帐号提供额外的购买力,同时针对不同的计费方式执行扣费")]
    public class FinServiceCentre : BaseSrvObject, IContrib
    {

        //配资服务的资金放大比例
        decimal FinPower = FinGlobals.FinPower;


        //public event FindAccountDel FindAccountEvent;
        //public event IsTradingDayDel IsTradingDayEvent;
        /// <summary>
        /// 对账户进行实时的账户检查比如权益亏损强平并冻结等
        /// </summary>
        //public event IAccountCheckDel AccountCheckEvent;


        ConnectionPoll<mysqlDBFinService> conn;
        //所有的费率计划都是固定的,价格的微调是通过discount来进行。
        /// <summary>
        /// 利息费率计划
        /// </summary>
        IRatePlan _InterestRatePlan = new InterestRatePlan();
        /// <summary>
        /// 分成费率计划
        /// </summary>
        IRatePlan _BonusRatePlan = new BonusRatePlan();

        IRatePlan _SepcialIF_FJ = new SPECIALIFFJRatePlan();

        //用于记录账户对应的 finservice
        ConcurrentDictionary<string, IFinService> accountfinmap = new ConcurrentDictionary<string, IFinService>();

        public FinServiceCentre():base("FinServiceCentre")
        {
            ConfigFile cfg = ConfigFile.GetConfigFile("finservice.cfg");

            //初始化数据库连接对象
            conn = new ConnectionPoll<mysqlDBFinService>(cfg["DBAddress"].AsString(), cfg["DBUser"].AsString(), cfg["DBPass"].AsString(), cfg["DBName"].AsString(), cfg["DBPort"].AsInt());

            //加载配资服务参数
            FinGlobals.InitFinServiceConfig(cfg["FinPower"].AsInt(),
                                            cfg["FinPowerOverNight"].AsInt(),
                                            cfg["FinFlatRate"].AsDecimal(),
                                            cfg["CutBuyPowerRate"].AsDecimal(),
                                            cfg["IsCutInMarket"].AsBool(),
                                            cfg["IsCutAfterMarket"].AsBool(),
                                            cfg["FinFee5"].AsDecimal(),
                                            cfg["FinFee20"].AsDecimal(),
                                            cfg["FinFee50"].AsDecimal(),
                                            cfg["FinBonusrate"].AsDecimal(),
                                            cfg["FinBonusCommission"].AsDecimal());
            FinGlobals.InitFinServiceFJ(cfg["FJ_FixMarginEbable"].AsBool(),
                                        cfg["FJ_IFCommission"].AsDecimal(),
                                        cfg["FJ_WinFee_Agent"].AsDecimal(),
                                        cfg["FJ_LossFee_Agent"].AsDecimal(),
                                        cfg["FJ_Pledge_Agent"].AsDecimal(),
                                        cfg["FJ_WinFee_Customer"].AsDecimal(),
                                        cfg["FJ_LossFee_Customer"].AsDecimal(),
                                        cfg["FJ_Margin_PerLot"].AsDecimal(),
                                        cfg["FJ_Margin_Flat"].AsDecimal());
            
            //将定时任务注册到任务中心
            //TaskCentre.RegisterTask(new TaskProc("保存PositionRound日志", new TimeSpan(0, 0, 1), Task_StorePositionRound));
            //TaskCentre.RegisterTask(new TaskProc("检查配资账户权益", new TimeSpan(0, 0, 1), Task_CheckAccountLOANEE));
        }

        #region IContrib 接口实现
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {

            //调用风控中心检查配资帐户的accountrule
            //this.AccountCheckEvent += new IAccountCheckDel(TLCtxHelper.Ctx.RiskCentre.CheckAccount);

            //2.将融资中心的 获得账户配资额度的函数绑定到清算中心对应的事件
            TLCtxHelper.ExContribEvent.GetFinAmmountAvabileEvent += new AccountFinAmmountDel(this.GetFinAmmountAvabile);
            TLCtxHelper.ExContribEvent.GetFinAmmountTotalEvent += new AccountFinAmmountDel(this.GetFinAmmountTotal);
            //4.将清算中心的手续费调整操作绑定到配资中心的手续费调整函数
            TLCtxHelper.ExContribEvent.AdjustCommissionEvent += new AdjustCommissionDel(this.AdjestCommission);


            //清算中心生成配资本帐号时 调用配资中心 用于绑定默认配资服务
            TLCtxHelper.EventAccount.AccountAddEvent += new AccountIdDel(this.OnLoaneeAccountCreated);

            //5.清算中心持仓回合关闭 调用配资中心执行相关逻辑
            TLCtxHelper.EventIndicator.GotPositionClosedEvent += new IPositionRoundDel(this.OnPositionRoundClosed);

            //风控中心查询配资服务的事件绑定到 配资中心
            //TLCtxHelper.Ctx.RiskCentre.GetFinServiceDelEvent += (string account) => { return this[account]; };

            //将管理中心的事件绑定到_fincentre组件 管理中心查询某个账户的配资服务
            //查询配资服务
            //TLCtxHelper.Ctx.MessageMgr.GetFinServiceInfoEvent += (string account) =>
            //{
            //    IFinService fs = this[account];
            //    if (fs == null) return null;
            //    return ObjectInfoHelper.GenFinServiceInfo(fs);
            //};

            /*
            //激活配资服务
            TLCtxHelper.Ctx.MessageMgr.ActiveFinServiceEvent += (string account, out string msg) =>
            {
                return this.UpdateFinServiceActive(account, true, out msg);
            };
             
            //冻结配资服务
            TLCtxHelper.Ctx.MessageMgr.InActiveFinServiceEvent += (string account, out string msg) =>
            {
                return this.UpdateFinServiceActive(account, false, out msg);
            };
             
            TLCtxHelper.Ctx.MessageMgr.UpdateFinServiceEvent += (string account, decimal ammount, QSEnumFinServiceType type, decimal discount, int agentcode, out string msg) =>
            {
                IAccount acc = TLCtxHelper.OpAccount[account];
                if (account == null)
                {
                    msg = "帐号不存在";
                    return false;
                }
                IFinService fs = acc.FinService;
                if (fs == null)
                {
                    //固定保证金型的配资服务 特殊添加模式
                    if (type == QSEnumFinServiceType.SPECIAL_IF_FJ)
                    {
                        return this.AddFinService(account, 0, type, 1, acc.AgentCode, out msg);
                    }
                    return this.AddFinService(account, ammount, type, discount, agentcode.ToString(), out msg);
                }

                msg = string.Empty;
                string o1;
                string o2;
                string o3 = string.Empty;
                string o4 = string.Empty;
                bool re1 = this.UpdateFinServiceAmmount(account, ammount, out o1);
                bool re2 = this.UpdateFinServiceType(account, type, out o2);
                bool re3 = true;
                bool re4 = true;
                //管理端可以调整折扣,终端用户无法调整折扣 默认折扣系数为0
                if (discount >= 0.7M && discount <= 1M)
                {
                    re3 = this.UpdateFinServiceDiscount(account, discount, out o3);
                }
                if (agentcode != 0)
                {
                    re4 = this.UpdateFinServiceAgent(account, agentcode, out o4);
                }

                if (re1 && re2 & re3 & re4)
                {
                    return true;
                }
                else
                {
                    msg = o1 + o2 + o3 + o4;
                    return false;
                }
                
            };



            //重置配资服务,从数据库加载对应的配资服务
            this.Reset();
             * * **/
        }
             

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        /// <summary>
        /// 运行
        /// </summary>
        //[CLICommandAttr("startfin","startfin - Start FinService Contrib module")]
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        //[CLICommandAttr("stopfin", "stopfin - Stop FinService Contrib module")]
        public void Stop()
        {

        }

        #endregion

        

        #region 功能函数

        bool IsTradingDay()
        {
            return TLCtxHelper.CmdClearCentre.IsTradeDay;
        }
        /// <summary>
        /// 获得有效配资额度
        /// 获得某个账户的可用金额,账户buypower中计算可用购买的力的时候，需要向融资服务中心 查询该账户的融资额度
        /// 配资额度是动态的一个数值,在设定服务时,满额为5万，当权益小于安全权益的50%时,则额度自动减半
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public decimal GetFinAmmountAvabile(string account)
        {
            if (!accountfinmap.Keys.Contains(account)) return 0;
            IFinService fs = accountfinmap[account];
            
            decimal totalammount = fs.GetAvabileAmmount();//服务额度
            decimal safeequity = totalammount/FinPower;//按1:10计算安全保证金

            if (FinGlobals.IsCutInMarket)
            {
                //debug("账户:" + fs.Account.ID + " 配资额度:" + totalammount.ToString() + " 安全本金:" + safeequity.ToString() + " 当前动态权益:" + fs.Account.NowEquity.ToString(), QSEnumDebugLevel.ERROR);
                if (fs.Account.NowEquity >= safeequity * FinGlobals.CutBuyPowerRate) //如果当前账户额度 大于等于 50%安全保证金，则可用额度为全额
                    return totalammount;
                else
                    return totalammount * FinGlobals.CutBuyPowerRate;
            }
            else
            {
                return totalammount;
            }
        }

        /// <summary>
        /// 获得名义配资额度,每个交易日从数据库加载对应的额度数值,盘中强平是按照该额度对应的安全金额进行的
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public decimal GetFinAmmountTotal(string account)
        {
            if (!accountfinmap.Keys.Contains(account)) return 0;
            IFinService fs = accountfinmap[account];
            return fs.GetAvabileAmmount();
        }

        public IFinService this[string account]
        {
            get
            {
                //检查清算中心是否有该账户 没有该账户返回空
                IAccount acc = FindAccount(account);
                if (acc == null) return null;

                if (!accountfinmap.Keys.Contains(account)) return null;
                return accountfinmap[account];
            }
        }
        /// <summary>
        /// 获得account 所对应的账户
        /// 通过委托对外触发事件
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        IAccount FindAccount(string account)
        {
            return TLCtxHelper.CmdAccount[account];
            /*
            if (FindAccountEvent != null)
                return FindAccountEvent(account);
            return null;**/
        }

        /// <summary>
        /// 根据账户的不同费率计划调整交易手续费
        /// </summary>
        /// <param name="account"></param>
        /// <param name="size"></param>
        /// <param name="commission"></param>
        /// <returns></returns>
        public decimal AdjestCommission(Trade fill,IPositionRound positionround)
        {
            IFinService fs = this[fill.Account];
            if (fs == null) return fill.Commission;//如果该账户没有对应的配资本服务

            return fs.AdjustCommission(fill, positionround);
        }


        /// <summary>
        /// 获得对应的计费规则
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IRatePlan GetRatePlane(QSEnumFinServiceType type)
        {
            IRatePlan rp = null;
            switch (type)
            {
                case QSEnumFinServiceType.INTEREST:
                    rp = _InterestRatePlan;
                    break;
                case QSEnumFinServiceType.BONUS:
                    rp = _BonusRatePlan;
                    break;
                case QSEnumFinServiceType.SPECIAL_IF_FJ:
                    rp = _SepcialIF_FJ;
                    break;
                default:
                    rp = _InterestRatePlan;
                    break;
            }
            return rp;
        }


        /// <summary>
        /// 输出所有配资服务信息
        /// </summary>
        public void DisplayFinServices()
        {
            foreach (IFinService fs in accountfinmap.Values)
            {
                debug(fs.ToString(), QSEnumDebugLevel.MUST);
            }

        }
        #endregion


        #region 修改配资服务相关参数
        /// <summary>
        /// 激活或关闭某个账户的配资服务
        /// 执行该操作的同时 内存数据同步修改
        /// </summary>
        /// <param name="account"></param>
        public bool UpdateFinServiceActive(string account, bool active, out string msg)
        {
            mysqlDBFinService db = conn.mysqlDB;
            try
            {
                //1.检查帐号是否存在
                debug("try to update finservice active:"+account,QSEnumDebugLevel.INFO);
                msg = string.Empty;
                IAccount acc = FindAccount(account);
                if (acc == null)
                {
                    msg = "清算中心无该帐号信息";
                    conn.Return(db);
                    return false;
                }
                //2.检查FinService是否存在
                //获得账户对应的配资服务
                IFinService fs = this[account];
                if (fs == null)
                {
                    msg = "该帐号没有配资服务";
                    conn.Return(db);
                    return false;
                }
                
                //3.按照不同的配资服务类型来检查激活条件
                //福建固定保证金配资意外的服务 需要以购买力来计算可开仓手数,因此需要判断融资比例
                if (fs.RatePlan.Type != QSEnumFinServiceType.SPECIAL_IF_FJ)
                {
                    debug("2try to update finservice active:" + account, QSEnumDebugLevel.INFO);
                    //如果我们需要激活该配资服务,则需要检查账户的当前权益
                    if (active)
                    {

                        if (acc.NowEquity * FinPower < fs.FinAmmount)
                        {
                            debug(PROGRAME + ":账户:" + account + " 权益:" + acc.NowEquity.ToString() + " 无法激活配资额:" + fs.FinAmmount.ToString() + " 的配资服务", QSEnumDebugLevel.ERROR);
                            msg = "超过最高融资额度(账户权益的10倍),无法激活";
                            conn.Return(db);
                            return false;

                        }
                    }
                }
                else//固定保证金型配资服务 至少有一手的资金才可以激活
                {

                    if (acc.NowEquity < FinGlobals.MarginPerLot * 1)
                    {
                        debug(PROGRAME + ":账户:" + account + " 权益:" + acc.NowEquity.ToString() + " 激活固定保证金形式的配资服务", QSEnumDebugLevel.ERROR);
                        msg = "固定保证金型配资服务需要最低启动资金:" + FinGlobals.MarginPerLot.ToString();
                        conn.Return(db);
                        return false;
                    }
                }
                debug("3try to update finservice active:" + account, QSEnumDebugLevel.INFO);

                //数据库操作 标记激活服务状态
                if (db.UpdateFinServiceActive(account, active))
                {
                    //将finservice标记为可用
                    fs.Active = active;
                    conn.Return(db);
                    return true;
                }
                else
                {
                    msg = "数据库更新错误";
                    conn.Return(db);
                    return false;
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":更新账户[" + account + "] Active " + active.ToString() + " 出错 " + ex.ToString(), QSEnumDebugLevel.ERROR);
                msg = "服务更新激活设置异常";
                conn.Return(db);
                return false;
            }

        }

        /// <summary>
        /// 调整配资服务
        /// 1.资金小于权益的20% 强平线 则冻结配资服务
        /// 2.资金小于当前 安全权益50% 则我们按照资金重新设置其配资额度
        /// </summary>
        /// <param name="account"></param>
        public void TrimFinService(string account, bool iswithdraw = true)
        {
            debug(PROGRAME + ":调整账户 " + account + "的配资服务", QSEnumDebugLevel.INFO);
            IAccount acc = FindAccount(account);
            if (acc == null)
            {
                return;
            }
            //获得账户对应的配资服务
            IFinService fs = this[account];
            if (fs == null)
            {
                return;
            }

            //固定保证金类型的配资服务不用修正配资服务额度 他是按照每手需要多少保证金来计算可开数量 不是以标准的保证金计算来计算可开数量的
            if (fs.RatePlan.Type == QSEnumFinServiceType.SPECIAL_IF_FJ)
            {
                return;
            }


            //其余类型的配资服务
            decimal eq = acc.NowEquity;//当前权益

            decimal fa = fs.FinAmmount;//配资额度

            if (eq >= fa / FinPower) return;

            string msg = string.Empty;
            //达到强平线 安全本金的20%这配资服务失效
            if (eq < fa / FinPower * FinGlobals.FinFlatRate)
            {
                debug(PROGRAME + ":账户 " + account + "权益" + eq.ToString() + "小于安全本金20%:" + (fa / 10 * 0.2M).ToString() + "  冻结配资服务", QSEnumDebugLevel.INFO);
                //冻结配资服
                UpdateFinServiceActive(account, false, out msg);
                return;
            }

            //如果不是出金形成的配资调整，比如亏损，则我们设定有50%作为调整界限
            if (!iswithdraw)
            {

                //如果安全本金减半,则配资额度减半 
                if (FinGlobals.IsCutAfterMarket && (eq < fa / FinPower * FinGlobals.CutBuyPowerRate))
                {

                    UpdateFinServiceAmmount(account, eq * FinPower, out msg, true);
                    debug(PROGRAME + ":账户 " + account + "权益" + eq.ToString() + "小于安全本金50%:" + (fa / 10 * 0.5M).ToString() + "  降级配资服务 out msg:" + msg, QSEnumDebugLevel.INFO);
                    return;
                }
            }
            else //手工出金导致权益低于安全本金，按照当前的权益重新设置 配资服务额度
            {
                UpdateFinServiceAmmount(account, eq * 10, out msg, true);
                debug(PROGRAME + ":账户 " + account + "权益" + eq.ToString() + "小于安全本金50%:" + (fa / 10 * 0.5M).ToString() + "  降级配资服务 out msg:" + msg, QSEnumDebugLevel.INFO);
                return;
            }
        }
        /// <summary>
        /// 修改配资服务计费类别
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        public bool UpdateFinServiceType(string account, QSEnumFinServiceType type, out string msg, bool issync = false)
        {

                mysqlDBFinService db = conn.mysqlDB;
                try
                {

                    msg = string.Empty;
                    IAccount acc = FindAccount(account);
                    if (acc == null)
                    {
                        msg = "清算中心无该帐号信息";
                        conn.Return(db);
                        return false;
                    }
                    //获得账户对应的配资服务
                    IFinService fs = this[account];
                    if (fs == null)
                    {
                        msg = "该帐号没有配资服务";
                        conn.Return(db);
                        return false;
                    }

                    //固定保证金类型的配资服务一经生成 不作类型修改
                    if (fs.RatePlan.Type == QSEnumFinServiceType.SPECIAL_IF_FJ)
                    {
                        msg = "固定保证金类型的配资服务不可更改成其他服务类型";
                        conn.Return(db);
                        return false;
                    }


                    if (fs.Active)//激活条件下只可以修改数据库数据
                    {
                        if (db.UpdateFinServiceType(account, type))
                        {
                            conn.Return(db);
                            return true;
                        }
                        else
                        {
                            msg = "数据库更新失败";
                            conn.Return(db);
                            return false;
                        }
                    }
                    else//非激活条件可以修改内存数据
                    {
                        if (db.UpdateFinServiceType(account, type))
                        {

                            IRatePlan rp = GetRatePlane(type);
                            fs.RatePlan = rp;
                            conn.Return(db);
                            return true;
                        }
                        else
                        {
                            msg = "数据库更新失败";
                            conn.Return(db);
                            return false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    debug(PROGRAME + ":修改账户[" + account + "] 配资计费类别为 " + type.ToString() + "出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    msg = "更新配资计费类别异常";
                    conn.Return(db);
                    return false;
                }
            
        }

        /// <summary>
        /// 修改账户配资服务额度
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ammount"></param>
        public bool UpdateFinServiceAmmount(string account, decimal ammount, out string msg, bool issync = false)
        {
                mysqlDBFinService db = conn.mysqlDB;
                try
                {
                    msg = string.Empty;
                    IAccount acc = FindAccount(account);
                    if (acc == null)
                    {
                        msg = "清算中心无该帐号信息";
                        conn.Return(db);
                        return false;
                    }
                    //获得账户对应的配资服务
                    IFinService fs = this[account];
                    if (fs == null)
                    {
                        msg = "该帐号没有配资服务";
                        conn.Return(db);
                        return false;
                    }

                    //固定保证金类型的配资服务一经生成 不作类型修改
                    if (fs.RatePlan.Type == QSEnumFinServiceType.SPECIAL_IF_FJ)
                    {
                        msg = "固定保证金类型的配资服务不可更改其配资额度";
                        conn.Return(db);
                        return false;
                    }

                    //如果配资服务已经激活,则我们需要检查其修改的额度是否超过其权益的10倍，如果没有激活则可以自行修改
                    //当激活的时候进行权益检查
                    if (fs.Active)
                    {
                        //如果融资额度超过账户权益可融资额度，直接返回
                        if (acc.NowEquity * FinPower < ammount)
                        {
                            msg = "超过最高融资额度(账户权益的10倍),无法激活";
                            conn.Return(db);
                            return false;
                        }//判断激活条件
                        else
                        {
                            if (db.UpdateFinServiceAmmount(account, ammount))
                            {
                                conn.Return(db);
                                return true;
                            }
                            else
                            {
                                msg = "数据库更新出错";
                                conn.Return(db);
                                return false;
                            }
                        }
                    }
                    else//非激活状态可以任意修改额度以及费率计划
                    {

                        if (db.UpdateFinServiceAmmount(account, ammount))
                        {
                            fs.FinAmmount = ammount;
                            conn.Return(db);
                            return true;
                        }
                        else
                        {
                            msg = "数据库更新出错";
                            conn.Return(db);
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    debug(PROGRAME + ":更新账户[" + account + "] 配资额度为 " + ammount + " 出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    msg = "更新配资额度异常";
                    conn.Return(db);
                    return false;

                }
            
        }
        /// <summary>
        /// 将代理部分的逻辑 转移到 帐号Account模块中去
        /// </summary>
        /// <param name="account"></param>
        /// <param name="agentcode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateFinServiceAgent(string account, int agentcode, out string msg)
        {
            mysqlDBFinService db = conn.mysqlDB;
            try
            {
                msg = string.Empty;
                if (agentcode == 0)
                {
                    msg = "代理编码必须为4位有效整数";
                    conn.Return(db);
                    return false;
                }
                IAccount acc = FindAccount(account);
                if (acc == null)
                {
                    msg = "清算中心无该帐号信息";
                    conn.Return(db);
                    return false;
                }
                //获得账户对应的配资服务
                IFinService fs = this[account];
                if (fs == null)
                {
                    msg = "该帐号没有配资服务";
                    conn.Return(db);
                    return false;
                }

               

                if (db.UpdateFinServiceAgent(account, agentcode))
                {
                    conn.Return(db);
                    return true;
                }
                else
                {
                    msg = "数据库更新出错";
                    conn.Return(db);
                    return false;
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":更新账户[" + account + "] 代理编码 " + agentcode.ToString() + " 出错 " + ex.ToString(), QSEnumDebugLevel.ERROR);
                msg = "更新配资代理编码异常";
                conn.Return(db);
                return false;
            }
        }
        /// <summary>
        /// 更新账户配资服务费用折扣
        /// </summary>
        /// <param name="account"></param>
        /// <param name="discount"></param>
        public bool UpdateFinServiceDiscount(string account, decimal discount, out string msg, bool issync = false)
        {
            mysqlDBFinService db = conn.mysqlDB;
            try
            {
                msg = string.Empty;
                if (discount < 0.7M || discount > 1M)
                {
                    msg = "折扣参数只能在0.7-1之间";
                    conn.Return(db);
                    return false;
                }
                IAccount acc = FindAccount(account);
                if (acc == null)
                {
                    msg = "清算中心无该帐号信息";
                    conn.Return(db);
                    return false;
                }
                //获得账户对应的配资服务
                IFinService fs = this[account];
                if (fs == null)
                {
                    msg = "该帐号没有配资服务";
                    conn.Return(db);
                    return false;
                }

                //固定保证金类型的配资服务一经生成 不作类型修改
                if (fs.RatePlan.Type == QSEnumFinServiceType.SPECIAL_IF_FJ)
                {
                    msg = "固定保证金类型的配资服务不可更改其配资费用折扣";
                    conn.Return(db);
                    return false;
                }



                if (db.UpdateFinServiceDiscount(account, discount))
                {
                    fs.Discount = discount;
                    conn.Return(db);
                    return true;
                }
                else
                {
                    msg = "数据库更新出错";
                    conn.Return(db);
                    return false;
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":更新账户[" + account + "] 费用折扣 " + discount.ToString() + " 出错 " + ex.ToString(), QSEnumDebugLevel.ERROR);
                msg = "更新配资折扣异常";
                conn.Return(db);
                return false;
            }

        }


        #endregion


        #region 验证出金账户 查询融资服务
        /// <summary>
        /// 验证账户出金
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ammount"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool ValidWithdraw(string account, decimal ammount, out string msg)
        {
            try
            {
                msg = string.Empty;
                IAccount acc = FindAccount(account);
                if (acc == null)
                {
                    msg = "清算中心无该帐号信息";
                    return false;
                }
                //获得账户对应的配资服务
                IFinService fs = this[account];
                if (fs == null)
                {
                    msg = "该帐号没有配资服务";
                    return false;
                }
                //配资服务有效
                if (fs.Active)
                {
                    string start = "16:00";
                    string end = "21:00";

                    decimal leq = fs.FinAmmount / 10;
                    //如果出金额度是安全资金，则任何时候可以出金
                    //当前权益8000，配资额度50000 对应本金为5000 则3000为安全资金
                    if (ammount <= acc.NowEquity - leq)
                    {
                        return true;
                    }
                    else
                    {
                        //msg = "安全本金只有在取消配资服务后才可以出金";
                        //return false;


                        DateTime now = DateTime.Now;
                        //如果服务有效,则必须在下午4点到晚上9点之间才可以出金
                        if (now > DateTime.Parse(start) && now < DateTime.Parse(end))
                        {
                            if (ammount <= acc.NowEquity)
                            {
                                return true;
                            }
                            else
                            {
                                msg = "出金额度超过账户权益:" + acc.NowEquity.ToString();
                                return false;
                            }
                        }
                        else
                        {
                            msg = "安全本金只有在下午16:00至晚上21:00才可出金[收盘]";
                            return false;
                        }
                    }
                }
                else//配资服务无效
                {
                    //配资服务无效,则账户可以出金所有的权益
                    if (ammount <= acc.NowEquity)
                    {
                        return true;
                    }
                    else
                    {
                        msg = "出金额度超过账户权益:" + acc.NowEquity.ToString();
                        return false;
                    }


                }

            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":出金验证出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
                msg = "出金验证异常";
                return false;

            }

        }

        /// <summary>
        /// 查询配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool QryFinService(string account, out string msg)
        {
            try
            {
                msg = string.Empty;
                //获得账户对应的配资服务
                IFinService fs = this[account];
                if (fs == null)
                {
                    return false;
                }

                msg = fs.FinAmmount.ToString() + "," + fs.RatePlan.Type.ToString() + "," + fs.Discount.ToString() + "," + fs.Active.ToString();
                return true;
            }
            catch (Exception ex)
            {
                msg = string.Empty;
                return false;
            }
        }

        #endregion


        #region 配资统计信息 费用 额度 收息额度 分红额度 当日最大保证金占用等运营数据 以及配资风控检查

        public int NumActived
        {
            get
            {
                IEnumerable<IFinService> fslist =
                from fs in accountfinmap.Values
                 where fs.Active
                    select fs;

                return fslist.Count();
                /*
                int i = 0;
                foreach (IFinService fs in accountfinmap.Values)
                {
                    if (fs.Active)
                    {
                        i++;
                    }
                }
                return i;**/
            }
        }
        /// <summary>
        /// 计算当日所有配资费用,当结算后 配资费用会发生变化比如额度进行了调整等
        /// </summary>
        public decimal TotalFee { get {
            decimal _totalfee = 0;
            foreach (IFinService fs in accountfinmap.Values)
            {
                if (fs.Active)
                {
                    _totalfee += fs.GetRate();
                }
            }
            return _totalfee; } }

        /// <summary>
        /// 所有激活的配资额度
        /// </summary>
        public decimal TotalFinAmmount
        {
            get
            {
                decimal ammount = 0;
                foreach (IFinService fs in accountfinmap.Values)
                {
                    if (fs.Active)
                    {
                        ammount += fs.FinAmmount;
                    }
                }
                return ammount;
            }
        }
        /// <summary>
        /// 所有激活的收息配资额度
        /// </summary>
        public decimal TotalInterestAmmount
        { 
            get
            {
                decimal ammount = 0;
                foreach (IFinService fs in accountfinmap.Values)
                {
                    if (fs.Active && fs.RatePlan.Type == QSEnumFinServiceType.INTEREST)
                    {
                        ammount += fs.FinAmmount;
                    }
                }
                return ammount;
            }
        }
        /// <summary>
        /// 所有激活的盈利分红的配资额度
        /// </summary>
        public decimal TotalBonusAmmount
        {
            get
            {
                decimal ammount = 0;
                foreach (IFinService fs in accountfinmap.Values)
                {
                    if (fs.Active && fs.RatePlan.Type == QSEnumFinServiceType.BONUS)
                    {
                        ammount += fs.FinAmmount;
                    }
                }
                return ammount;
            }
        }

        decimal marginused = 0;

        /// <summary>
        /// 最大保证金占用
        /// </summary>
        public decimal MarginUsed {
            get {
                decimal r = 0;
                    foreach (IFinService fs in accountfinmap.Values)
                    {
                        if (fs.Active)
                        {
                            //累加账户的权益
                            r += fs.Account.Margin + fs.Account.ForzenMargin;

                        }
                    }
                    return r;
            }
        }



        /*
        public void Start()
        {
            if (checkgo) return;
            checkgo = true;
            checkthread = new Thread(checkprocess);

            checkthread.IsBackground = true;
            checkthread.Name = "FinService Check Margin,AccountRisk check";
            checkthread.Start();
            ThreadTracker.Register(checkthread);
        }

        public void Stop()
        {
            if (!checkgo) return;
            checkgo = false;
            checkthread.Abort();
            checkthread = null;
        }
        **/

        const int buffersize = 1000;
        RingBuffer<PositionRoundForClear> _prcache = new RingBuffer<PositionRoundForClear>(buffersize);

        int margincheckfreq = 30;
        /// <summary>
        /// 保证金检查周期
        /// </summary>
        public int MarginCheckFreq { get { return margincheckfreq; } set { margincheckfreq = value; } }

        //Thread checkthread = null;
        //bool checkgo = false;

        DateTime margintime = DateTime.Now;
        void collectmargion()
        {
            decimal totalmagin = 0;
            foreach (IFinService fs in accountfinmap.Values)
            {
                if (fs.Active)
                {
                    //累加账户的权益
                    totalmagin += fs.Account.Margin + fs.Account.ForzenMargin;
                }
            }
            marginused = marginused > totalmagin ? marginused : totalmagin;
            margintime = DateTime.Now;
        }

        int _freqLoanee = 1000;
        void Task_CheckAccountLOANEE()
        {
            foreach (IAccount a in TLCtxHelper.CmdAccount.Accounts.Where<IAccount>(x => x.Category == QSEnumAccountCategory.LOANEE))
            {
                try
                {
                    //if (AccountCheckEvent != null)
                    //    AccountCheckEvent(a);
                    TLCtxHelper.CmdRiskCentre.CheckAccount(a);
                }
                catch (Exception ex)
                {
                    debug("配资帐户风控检查出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }
        /// <summary>
        /// 将需要储存的交易回合记录 储存到数据库
        /// </summary>
        void Task_StorePositionRound()
        {
            try
            {
                while (_prcache.hasItems)
                {
                    mysqlDBFinService db = conn.mysqlDB;
                    db.InsertPositionTransaction(_prcache.Read());
                    conn.Return(db);
                }
            }
            catch (Exception ex)
            {
                debug("配资交易回合清算记录写入失败"+ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        
        /*
        void checkprocess()
        {
            try
            {
                while (checkgo)
                {
                    //每隔30秒统计一下保证金占用
                    if (DateTime.Now.Subtract(margintime).TotalSeconds > MarginCheckFreq)
                    {
                        collectmargion();
                    }
                    //检查配资帐号风控规则
                    CheckAccountLOANEE();
                    //储存交易回合记录
                    StorePositionRound();
                    Thread.Sleep(_freqLoanee);//等待500ms再次进行检查
                }

            }
            catch (Exception ex)
            {
                debug("配资中心保证金检查出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
            
        }***/

        #endregion

        #region 重置 结算 加载
        /// <summary>
        /// 重置配资服务,从数据库重新生成对应账户的配资本服务,响应客户在盘中的修改(计费方式,配资额度等)
        /// </summary>
        public void Reset()
        {
            //重置保证金占用检测
            marginused = 0;

            //加载配资服务
            LoadFinServices();
            Notify("加载配资服务", "配资中心加载配资服务");
        }

        /// <summary>
        /// 结算所有融资服务,将费用记录插入数据库,并从账户权益出金,用于扣除当天的费用
       

        //public event ChargeFinFeeDel ChargeFinFeeEvent;// </summary>
        public void SettleFinServices()
        {
            if (!IsTradingDay()) return;
            debug("开始结算配资服务,扣费并调整配资额度", QSEnumDebugLevel.INFO);
            mysqlDBFinService db = conn.mysqlDB;
            foreach (IFinService fs in accountfinmap.Values)
            {
                if (fs.Active)
                {
                    //1.计算当日配资费用 记录到数据库 并触发事件，通知清算中心进行扣费
                    decimal fee = SettleFinService(fs,db);
                    //对外触发费用 清算中心从账户权益中扣除融资费用
                    if (fee > 0)//需要扣费的我们才调用清算中心进行扣费
                    {
                        //内部直接调用 清算中心进行扣费
                        TLCtxHelper.CmdAccountCritical.CashOperation(fs.Account.ID, fee * (-1), "FinFee");
               
                    }

                    //2.调整配资服务
                    TrimFinService(fs.Account.ID, false);
                }

            }
            conn.Return(db);
            //重新加载配资服务
            this.LoadFinServices();
            Notify("结算配资服务,执行扣费","");
        }
        /// <summary>
        /// 结算某个配资服务,形成配资扣费记录并插入数据库
        /// </summary>
        /// <param name="fs"></param>
        decimal SettleFinService(IFinService fs,mysqlDBFinService db)
        {
            try
            {
                //获得当日计费
                decimal rate = fs.GetRate();
                //检测当日是否已经扣费 防止单日重复收费
                if (db.IsFeeCharged(fs.Account.ID, DateTime.Now))
                    return 0;
                //需要扣费的我们才在数据库插入扣费记录
                if(rate>0)
                    db.InsertFinFee(fs.Account.ID, rate);
                return rate;
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":结算 [" + fs.Account.ID + " ] 出错 " + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
            return 0;

        }

        /// <summary>
        /// 为某个账户添加融资服务
        /// </summary>
        /// <param name="account">账户</param>
        /// <param name="ammount">金额</param>
        /// <param name="type">计费类别</param>
        /// <param name="discount">折扣</param>
        public bool AddFinService(string account, decimal ammount, QSEnumFinServiceType type, decimal discount, string agent, out string msg)
        {
            mysqlDBFinService db = conn.mysqlDB;
            try
            {
                msg = string.Empty;
                IAccount acc = FindAccount(account);
                if (acc == null)
                {
                    msg = "清算中心无该帐号信息";
                    conn.Return(db);
                    return false;
                }
                IRatePlan rp = GetRatePlane(type);

                FinService fs = new FinService(acc, ammount, rp, discount, false,agent);
                //数据库插入记录            
                if (!db.HaveAccountFinService(account))
                {
                    debug(PROGRAME + ":插入配资服务:");
                    if (db.InsertFinService(fs.Account.ID, fs.FinAmmount, fs.RatePlan.Type, fs.Discount, agent, fs.Active))
                    {    //将融资混存到内存映射字典
                        accountfinmap[account] = fs;
                        conn.Return(db);
                        return true;
                    }
                    else
                    {
                        msg = "配资服务数据库插入失败";
                        conn.Return(db);
                        return false;
                    }
                }
                else
                {
                    msg = "该账户已经存在配资服务,添加配资服务失败";
                    conn.Return(db);
                    return false;
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":添加配资服务异常" + ex.ToString(), QSEnumDebugLevel.ERROR);
                msg = "添加配资服务异常";
                conn.Return(db);
                return false;
            }

        }


        /// <summary>
        /// 加载配资服务
        /// </summary>
        public void LoadFinServices(string accid = null)
        {
            debug(PROGRAME + ":加载配资服务...", QSEnumDebugLevel.INFO);
            accountfinmap.Clear();
            DataSet ds = null;
            mysqlDBFinService db = conn.mysqlDB;
            if (string.IsNullOrEmpty(accid))
            {
                ds = db.getFinServices();
            }
            else
            {
                ds = db.getFinServices(accid);
            }
            conn.Return(db);

            DataTable dt = ds.Tables["finservices"];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];

                string account = Convert.ToString(dr["account"]);
                IAccount acc = FindAccount(account);
                if (acc == null) continue;

                decimal ammount = Convert.ToDecimal(dr["ammount"]);
                QSEnumFinServiceType type = (QSEnumFinServiceType)Enum.Parse(typeof(QSEnumFinServiceType), Convert.ToString(dr["type"]));
                decimal discount = Convert.ToDecimal(dr["discount"]);
                bool active = Convert.ToBoolean(dr["active"]);
                string agentcode = Convert.ToString(dr["agent"]);
                IFinService fs = new FinService(acc, ammount, GetRatePlane(type), discount, active, agentcode);

                //建立配资服务的双向绑定
                accountfinmap[account] = fs;
                acc.FinService = fs;

                debug(PROGRAME + ":加载配资" + acc.ID + " " + fs.FinAmmount.ToString(), QSEnumDebugLevel.INFO);
            }
        }

        #endregion

        #region 统计部分
         /*
        public IFinStatistic GetFinStatForTotal(IClearCentreBase cc)
        {
            return FinStatistic.GetFinStatForTotal(cc, this);
        }

        public IFinStatistic GetFinStatForSIM(IClearCentreBase cc)
        {
            return FinStatistic.GetFinStatForSIM(cc, this);
        }

        public IFinStatistic GetFinStatForLIVE(IClearCentreBase cc)
        {
            return FinStatistic.GetFinStatForLIVE(cc, this);
        }

        public IFinSummary GetFinSummary(IClearCentreBase cc)
        {
            return FinSummary.GetFinSummary(cc, this);
        }**/
        #endregion


        #region 处理PositionRound关闭事件
        public void OnPositionRoundClosed(IPositionRound pr)
        {
            IFinService fs = this[pr.Account];
            //不存在配资服务 则直接返回
            if (fs == null) return;
            //设定的配资服务不为 SPECIAL_IF_FJ 则直接返回
            if (fs.RatePlan.Type != QSEnumFinServiceType.SPECIAL_IF_FJ) return;
            debug("Account:" + fs.Account.ID + " got positionround information,will insert into business db table", QSEnumDebugLevel.INFO);
            PositionRoundForClear prc = new PositionRoundForClear(fs.Account, pr);
            _prcache.Write(prc);
        }
        #endregion

        #region 处理配资帐户增加事件 用于添加设定的默认配资服务
        public void OnLoaneeAccountCreated(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if(acc == null || acc.Category != QSEnumAccountCategory.LOANEE) return;
            //该帐号一定存在
            if (FinGlobals.EnableFixedMargin)
            {
                string msg;
                if (!AddFinService(account, 0, QSEnumFinServiceType.SPECIAL_IF_FJ, 1, acc.AgentCode, out msg))
                {
                    debug("绑定默认配资服务:SPECIAL_IF_FJ出错 " + msg, QSEnumDebugLevel.ERROR);
                }
            }
        }
        #endregion

    }

}
