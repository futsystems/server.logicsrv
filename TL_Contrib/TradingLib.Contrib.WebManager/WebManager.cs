using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;

namespace TradingLib.Contrib
{
    /// <summary>
    /// 建立通过web管理交易系统的入口组件
    /// web系统可以通过与webmanager的监听端口进行通讯,然后进行添加账户,添加配资服务,查询账户等操作
    /// 通过网站来管理配资本客户的出入金,账户查询等
    /// </summary>
    [ContribAttr("WebMgr","Web管理模块","建立与web业务通讯通道,接受web端发送过来的相关管理命令执行添加账户,添加配资服务,报名比赛等等")]
    public class WebManager:BaseSrvObject,IContrib, IWebManager
    {

        WebTaskRepServer _srv;

        public WebManager(string address,int port,IClearCentreSrv c):base("WebManager")
        {

            ConfigFile cfg = ConfigFile.GetConfigFile("webmgr.cfg");
            _srv = new WebTaskRepServer(cfg["WebMgrAddress"].AsString(), cfg["WebMgrPort"].AsInt());
            _srv.SendDebugEvent +=new DebugDelegate(msgdebug);
            _srv.GotWebTaskEvent += new WebTaskDel(handleWebTaskMessage);
        }
        #region IContrib 接口实现
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {

         /*
            //更新配资服务
                this.UpdateFinServiceEvent += (string account, decimal ammount, QSEnumFinServiceType type, decimal discount, int agentcode, out string msg) =>
                {
                    msg = string.Empty;
                    string o1;
                    string o2;
                    string o3 = string.Empty;
                    string o4 = string.Empty;
                    bool re1 = _fincentre.UpdateFinServiceAmmount(account, ammount, out o1);
                    bool re2 = _fincentre.UpdateFinServiceType(account, type, out o2);
                    bool re3 = true;
                    bool re4 = true;
                    //管理端可以调整折扣,终端用户无法调整折扣 默认折扣系数为0
                    if (discount >= 0.7M && discount <= 1M)
                    {
                        re3 = _fincentre.UpdateFinServiceDiscount(account, discount, out o3);
                    }
                    if (agentcode != 0)
                    {
                        re4 = _fincentre.UpdateFinServiceAgent(account, agentcode, out o4);
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

                if (racemode_loaded)
                {
                    this.SignupPreraceEvent += new IAccountSignForPreraceDel(_racecentre.Signup);//报名

                    this.QryRaceInfoEvent += new GetRaceInfoDel(_racecentre.GetAccountRaceInfo);//获得比赛状态信息
                }

                //添加配资服务
                _webmanager.AddFinServiceEvent += (string account, decimal ammount, QSEnumFinServiceType type, string agent, decimal discount, out string msg) =>
                {
                    return _fincentre.AddFinService(account, ammount, type, discount, agent, out msg);
                };

                //激活配资服务
                _webmanager.ActiveFinServiceEvent += (string account, out string msg) =>
                {
                    return _fincentre.UpdateFinServiceActive(account, true, out msg);
                };

                //冻结配资服务
                _webmanager.InActiveFinServiceEvent += (string account, out string msg) =>
                {
                    return _fincentre.UpdateFinServiceActive(account, false, out msg);
                };

                //出金检验
                _webmanager.ValidWithdrawEvent += (string account, decimal ammount, out string msg) =>
                {
                    return _fincentre.ValidWithdraw(account, ammount, out msg);
                };
                //出金后进行 配资服务调整，该冻结 冻结，该降级 降级
                _webmanager.TrimFinServiceEvent += (string account) =>
                {
                    _fincentre.TrimFinService(account);
                };
                //查询配资服务
                _webmanager.QryFinServiceEvent += (string account, out string msg) =>
                {
                    return _fincentre.QryFinService(account, out msg);
                };

                
                //添加配资账户
                this.AddFinAccoountEvent += new AddAccountDel(TLCtxHelper.Ctx.ClearCentre.AddNewFinAccount);
            **/
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        public void Start()
        {
            debug("##########启动 WebManager Server###################", QSEnumDebugLevel.INFO);
            _srv.Start();
        }

        public void Stop()
        {
            _srv.Stop();
        }
        #endregion



        public bool IsLive
        {
            get
            {
                if (_srv != null && _srv.IsLive)
                    return true;
                return false;
            }
        }
        

        public event IAccountSignForPreraceDel SignupPreraceEvent;//报名参加晋级赛
        public event GetRaceInfoDel QryRaceInfoEvent;
        
        public event AddFinServiceDel AddFinServiceEvent;//添加配资服务
        public event AddAccountDel AddSimAccoountEvent;//添加比赛账户
        public event AddAccountDel AddFinAccoountEvent;//添加配资账户

        public event UpdateFinServiceDel UpdateFinServiceEvent;//更新配资服务
        public event AccountParamDel ActiveFinServiceEvent;//激活配资服务
        public event AccountParamDel InActiveFinServiceEvent;//冻结配资服务

        public event ValidWithdrawDel ValidWithdrawEvent;
        public event TrimFinServiceDel TrimFinServiceEvent;
        public event AccountParamDel QryFinServiceEvent;

        const string REP_WRONG = "WRONG|";
        const string REP_OK = "OK|";
        //按照web端与交易服务器的通讯机制解析消息,然后执行相应的处理通过事件对外调用其他模块的相应函数进行操作
        string handleWebTaskMessage(string param)
        {
            string[] p = param.Split('|');
            string command = p[0];
            string[] args = p[1].Split(',');
            debug(PROGRAME + ":收到web前端消息:" + param, QSEnumDebugLevel.INFO);
            //消息类型|消息体
            switch (command)
            {

                #region 晋级赛帐号操作
                //请求模拟比赛账户
                case "RequstTradingAccount":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG + "提供参数错误";
                            string account = null;
                            string user_id = args[0];

                            Random rnd = new Random();
                            int intpass = rnd.Next(100000, 999999);
                            string pass = intpass.ToString();

                            account = TLCtxHelper.Ctx.AddNewAccount(user_id, pass);
                            if (string.IsNullOrEmpty(account)) return REP_WRONG + "交易系统创建帐号失败";
                            debug(PROGRAME + ":web端 账户:" + user_id.ToString() + " 请求建立模拟交易账户,执行该任务");

                            string response = REP_OK + user_id.ToString() + "," + account + "," + pass;
                            return response;
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求注册模拟交易账户失败,请检查:" + ex.ToString());
                            return REP_WRONG + "交易系统创建帐号异常";
                        }
                    }
                //报名晋级赛比赛
                case "SignupQualifier":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG + "提供参数错误";
                            string account = args[0];

                            IAccount acc = TLCtxHelper.Ctx.ClearCentre[account];
                            if(acc==null)
                            {
                                return REP_WRONG + "交易系统无该交易帐号信息";
                            }
                            if (SignupPreraceEvent != null)
                            {
                                string msg="";
                                SignupPreraceEvent(acc, out msg);
                                return REP_OK + msg;
                            }
                            else
                            {
                                debug("报名事件SignupPreraceEvent未绑定操作");
                                return REP_WRONG + "交易处理报名请求异常";
                            }

                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求晋级赛报名异常,请检查:" + ex.ToString());
                            return REP_WRONG + "交易处理报名请求异常";
                        }
                    
                    }
                //查询晋级赛信息 晋级 淘汰差额等信息
                case "QueryQualifierInfo":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG;//
                            string account = args[0];

                            debug(PROGRAME + ":web端 查询账户:" + account + " 晋级赛比赛信息", QSEnumDebugLevel.INFO);

                            IAccount acc = TLCtxHelper.Ctx.ClearCentre[account];
                            if (acc == null)
                            {
                                return REP_WRONG + "不存在该交易账户";
                            }
                            if(QryRaceInfoEvent != null)
                            {
                                //信息字段 当前权益  当日盈利 手续费 
                                IRaceInfo ri = QryRaceInfoEvent(account);

                                return REP_OK + RaceInfo.Serialize(ri);
                            }
                            else
                            {
                                debug("查询晋级赛信息QryRaceInfoEvent未绑定操作");
                                return REP_WRONG + "处理查询晋级赛信息请求异常";
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":web端 查询晋级赛信息出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "查询晋级赛信息异常";
                        }
                    }
                #endregion

                case "RequestSeasonAccount":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG + "提供参数错误";
                            string account = null;
                            string user_id = args[0];

                            Random rnd = new Random();
                            int intpass = rnd.Next(100000, 999999);
                            string pass = intpass.ToString();

                            account = TLCtxHelper.Ctx.ClearCentre.AddSeasonAccount(user_id, pass);
                            if (string.IsNullOrEmpty(account)) return REP_WRONG + "交易系统创建季赛帐号失败";
                            debug(PROGRAME + ":web端 账户:" + user_id.ToString() + " 请求建立季赛交易账户,执行该任务");

                            string response = REP_OK + user_id.ToString() + "," + account + "," + pass;
                            return response;
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求注册季赛模拟交易账户失败,请检查:" + ex.ToString());
                            return REP_WRONG + "交易系统创建季赛帐号异常";
                        }
                    }
                //请求配资账户
                case "RequstFinAccount":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG + "提供参数错误";
                            string account = null;
                            string user_id = args[0];
                            Random rnd = new Random();
                            int intpass = rnd.Next(100000, 999999);
                            string pass = intpass.ToString();
                            //建立配资帐号

                            if (AddFinAccoountEvent != null)
                                account = AddFinAccoountEvent(user_id, pass);
                            if (string.IsNullOrEmpty(account)) return REP_WRONG + "交易系统创建配资帐号失败";
                            debug(PROGRAME + ":web端 账户:" + user_id.ToString() + " 请求建立配资账户,执行该任务", QSEnumDebugLevel.INFO);

                            if (account == null) return REP_WRONG;
                            string response = REP_OK + user_id.ToString() + "," + account + "," + pass;

                            return response;
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求注册模拟交易账户失败,请检查:" + ex.ToString());
                            return REP_WRONG + "交易系统创建配资帐号异常";
                        }
                    }
                //更新配资服务
                case "UpdateFinService":
                    {
                        try
                        {
                            if (args.Length < 4) return REP_WRONG + "提供参数错误";//3个参数 account ammount type
                            string account = args[0];
                            decimal ammount = Convert.ToDecimal(args[1]);
                            QSEnumFinServiceType type = (QSEnumFinServiceType)Enum.Parse(typeof(QSEnumFinServiceType), args[2]);
                            decimal discount = Convert.ToDecimal(args[3]);
                            int agentcode = Convert.ToInt32(args[4]);
                            if (UpdateFinServiceEvent != null)
                            {
                                string o = string.Empty;
                                if (UpdateFinServiceEvent(account, ammount, type, discount, agentcode,out o))
                                {
                                    return REP_OK;
                                }
                                else
                                {
                                    return REP_WRONG + o;
                                }
                            }
                            else
                            {
                                return REP_WRONG + "服务端不支持该操作";
                            }

                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求更新配资参数失败" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "更新配资服务异常";
                        }
                    }

                //激活配资服务
                case "ActiveFinService":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG + "提供参数错误";//3个参数 account ammount type
                            string account = args[0];
                            debug(PROGRAME + ":web端 激活账户:" + account + " 的配资服务", QSEnumDebugLevel.INFO);
                            if (ActiveFinServiceEvent != null)
                            {
                                string o = string.Empty;
                                if (ActiveFinServiceEvent(account, out o))
                                {
                                    return REP_OK;
                                }
                                else
                                {
                                    return REP_WRONG + o;
                                }
                            }
                            else
                            {
                                return REP_WRONG + "服务端不支持该操作";
                            }

                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求激活配资服务失败" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "激活配资服务异常";
                        }
                    }
                //冻结配资服务
                case "InActiveFinService":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];
                            debug(PROGRAME + ":web端 冻结账户:" + account + " 的配资服务", QSEnumDebugLevel.INFO);
                            if (InActiveFinServiceEvent != null)
                            {
                                string o = string.Empty;
                                if (InActiveFinServiceEvent(account, out o))
                                {
                                    return REP_OK;
                                }
                                else
                                {
                                    return REP_WRONG + o;
                                }
                            }
                            else
                            {
                                return REP_WRONG + "服务端不支持该操作";
                            }

                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求冻结配资服务失败" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "冻结配资服务异常";
                        }
                    }

                //添加配资服务
                case "AddFinService":
                    {

                        try
                        {
                            if (args.Length < 3) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];
                            decimal ammount = Convert.ToDecimal(args[1]);
                            QSEnumFinServiceType type = (QSEnumFinServiceType)Enum.Parse(typeof(QSEnumFinServiceType), args[2]);
                            string agent = args[3];
                            decimal discount = Convert.ToDecimal(args[4]);
                            if (AddFinServiceEvent != null)
                            {
                                string o = string.Empty;
                                if (AddFinServiceEvent(account, ammount, type, agent, discount, out o))
                                {
                                    return REP_OK;
                                }
                                else
                                {
                                    return REP_WRONG + o;
                                }
                            }
                            else
                            {
                                return REP_WRONG + "服务端不支持该操作";
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":请求添加配资服务失败:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "添加配资服务异常";
                        }
                    }
                //账户入金
                case "DepositAccount":
                    {
                        try
                        {
                            if (WebMgrUtils.IsSettle2Reset()) return REP_WRONG + "系统结算中,请稍候进行出入金操作";
                            if (args.Length < 3) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];
                            decimal ammount = Convert.ToDecimal(args[1]);
                            string comment = args[2];

                            debug(PROGRAME + ":web端 给账户:" + account + " 入金:" + ammount.ToString() + " 编号:" + comment, QSEnumDebugLevel.INFO);

                            string o = string.Empty;
                            if (TLCtxHelper.Ctx.ClearCentre.CashOperationSafe(account, ammount, comment, out o))
                            {

                                return REP_OK;
                            }
                            else
                            {
                                return REP_WRONG + o;
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":web端 给账户入金操作错误" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "账户入金异常";
                        }
                    }
                //账户出金
                case "WithdrawAccount":
                    {
                        try
                        {
                            if (args.Length < 3) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];
                            decimal ammount = Convert.ToDecimal(args[1]) * -1M;
                            string comment = args[2];

                            debug(PROGRAME + ":web端 给账户:" + account + " 出金:" + ammount.ToString() + " 编号:" + comment, QSEnumDebugLevel.INFO);

                            string o = string.Empty;
                            if (TLCtxHelper.Ctx.ClearCentre.CashOperationSafe(account, ammount, comment, out o))
                            {
                                //出金成功后,我们需要进行调整配资服务
                                if (TrimFinServiceEvent != null)
                                {
                                    TrimFinServiceEvent(account);
                                }
                                return REP_OK;
                            }
                            else
                            {
                                return REP_WRONG + o;
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":web端 给账户出金操作错误" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "账户出金异常";
                        }


                    }
                //账户出金
                case "QueryAccountEquity":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];

                            debug(PROGRAME + ":web端 查询账户:" + account + " 当前权益", QSEnumDebugLevel.INFO);

                            IAccount acc = TLCtxHelper.Ctx.ClearCentre[account];
                            if (acc == null)
                            {
                                return REP_WRONG + "不存在该交易账户";
                            }
                            else
                            {
                                return REP_OK + acc.NowEquity.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":web端 查询账户权益出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "查询账户权益异常";
                        }
                    }

                //账户出金
                case "QueryAccountInfo":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];

                            debug(PROGRAME + ":web端 查询账户:" + account + " 当前信息", QSEnumDebugLevel.INFO);

                            IAccount acc = TLCtxHelper.Ctx.ClearCentre[account];
                            if (acc == null)
                            {
                                return REP_WRONG + "不存在该交易账户";
                            }
                            else
                            {
                                //信息字段 当前权益  当日盈利 手续费 
                                IAccountInfo a = ObjectInfoHelper.GenAccountInfo(acc);
                                return REP_OK + AccountInfo.Serialize(a);
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":web端 查询账户信息出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "查询账户信息异常";
                        }
                    }
                case "QueryFinService":
                    {
                        try
                        {
                            if (args.Length < 1) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];

                            if (QryFinServiceEvent != null)
                            {
                                string o = string.Empty;
                                if (QryFinServiceEvent(account, out o))
                                {
                                    return REP_OK + o;
                                }
                                else
                                {
                                    return REP_WRONG + "不存在配资服务";
                                }
                            }
                            else
                            {
                                return REP_WRONG;
                            }

                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":web端 查询配资服务信息出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "查询配资服务信息异常";
                        }

                    }
                //查询账户规则信息
                case "QueryRuleSet":
                    {
                        try
                        {

                            return REP_OK;
                        }
                        catch (Exception ex)
                        {
                            return REP_WRONG + "查询规则异常";
                        }


                    }
                //出金验证
                case "ValidWithdraw":
                    {
                        /* 1.出金金额小于当前权益扣除当前费用
                         * 2.如果当前配资服务可用,则需要4点以后出金
                         * 3.
                         * 
                         * 
                         * */


                        try
                        {
                            if (args.Length < 2) return REP_WRONG;//3个参数 account ammount type
                            string account = args[0];
                            decimal ammount = Convert.ToDecimal(args[1]);

                            if (ValidWithdrawEvent != null)
                            {
                                string o;
                                if (ValidWithdrawEvent(account, ammount, out o))
                                {
                                    return REP_OK;
                                }
                                else
                                {
                                    return REP_WRONG + o;
                                }
                            }
                            else
                            {
                                return REP_WRONG + "服务端不支持该操作";
                            }


                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":出金检查出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            return REP_WRONG + "出金验证出错";
                        }

                    }
            }
            return REP_WRONG + "交易系统无法解析该命令";
        }
        
    }
}
