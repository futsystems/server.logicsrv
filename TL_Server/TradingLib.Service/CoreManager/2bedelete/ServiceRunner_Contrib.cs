using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.ServiceManager
{
    public partial class CoreManager
    {
        //初始化比赛维护中心
        private void InitRaceCentre()
        {
            /*
            debug("初始化比赛维护中心");
            List<Type> tlist = ContribHelper.GetContribListViaType<IRaceCentre>();
            if (tlist.Count <= 0)
            {
                debug("没有比赛中心的实现文件,无法正确加载配资模块");
                return;
            }

            _racecentre = (IRaceCentre)Activator.CreateInstance(tlist[0],null);
            _racecentre.SendDebugEvent += new DebugDelegate(debug);


            //当某个账户进入一个某个比赛的时候 调用风控中心修改 该账户的风控规则
            _racecentre.AccountEntryRaceEvent += new AccountEntryRaceDel(_riskCentre.OnAccountEntryRace);//处理
            //当某个账户比赛状态发生变化的时候 通知客户端更新比赛状态
            _racecentre.AccountRaceStatusChanged += new IAccountChangedDel(_srv.newRaceInfo);//处理
            //客户端请求报名的时候 调用比赛中心报名函数
            _srv.SignupPreraceEvent += new IAccountSignForPreraceDel(_racecentre.Signup);//处理
            //tradingserver发送比赛状态的时候需要获取比赛信息 调用比赛中心获取比赛信息
            _srv.GetRaceInfoEvent += new GetRaceInfoDel(_racecentre.GetAccountRaceInfo);//获得账户raceinfo//处理
            //将比赛中心绑定给风控中心,风控中心对不同的比赛阶段有账户检查设置
            //_riskCentre.RaceCentre = _racecentre;

            //比赛组件内部的实时风控需要调用风控中心的checkaccount,风控中心只处理 配资客户 配资客户在账户加载时就已经在清算中心有所区分
            _racecentre.AccountCheckEvent += new IAccountCheckDel(_riskCentre.CheckAccount);
            //比赛中心与基本组件的交叉调用
            //账户raceinfo信息更新 1.将比赛状态转发给交易客户端 2.将比赛状态转发给管理端
            _racecentre.AccountRaceStatusChanged += new IAccountChangedDel(qsmgrSrv.newRaceInfo);
            //管理服务端请求比赛信息 
            qsmgrSrv.GetRaceInfoEvent += new GetRaceInfoDel(_racecentre.GetAccountRaceInfo);
            //管理端请求比赛统计信息
             * **/
            /*
            qsmgrSrv.GetRaceStatisticStrListEvent += () =>
            {

                List<string> l = new List<string>();
                
                foreach (IRaceStatistic  rs in  _racecentre.RaceStatList)
                {
                    l.Add(rs.ToString());
                }
                return l;
            };**/
            //_racecentre.Start();
            //racemode_loaded = true;
        }

        //初始化配资服务中心
        private void InitFinServiceCentre()
        {
            /*
            debug("初始化配资服务中心");

            List<Type> tlist = ContribHelper.GetContribListViaType<IFinServiceCentre>();
            if (tlist.Count <= 0)
            {
                debug("没有配资中心的实现文件,无法正确加载配资模块");
                return;
            }

            _fincentre = (IFinServiceCentre)Activator.CreateInstance(tlist[0], new object[] { _clearCentre, config.DBServer, config.CCDBUser, config.CCDBPass }); ;//= new FinServiceCentre(config.DBServer, config.CCDBUser, config.CCDBPass);
            _fincentre.SendDebugEvent += new DebugDelegate(debug);

            //1.将配资中心的查询账户事件绑定到清算中心对应的函数
            //_fincentre.FindAccountEvent += (string account) => { return _clearCentre[account]; };
            //调用风控中心检查配资帐户的accountrule
            _fincentre.AccountCheckEvent += new IAccountCheckDel(_riskCentre.CheckAccount);
            
            //2.将融资中心的 获得账户配资额度的函数绑定到清算中心对应的事件
            _clearCentre.GetAccountFinAmmountAvabileEvent += new AccountFinAmmountDel(_fincentre.GetFinAmmountAvabile);
            _clearCentre.GetAccountFinAmmountTotalEvent += new AccountFinAmmountDel(_fincentre.GetFinAmmountTotal);
            //4.将清算中心的手续费调整操作绑定到配资中心的手续费调整函数
            _clearCentre.AdjustCommissionEvent += new AdjustCommissionDel(_fincentre.AdjestCommission);
            
            //清算中心生成配资本帐号时 调用配资中心 用于绑定默认配资服务
            _clearCentre.LoaneeAccountAddedEvent += new StringParamDelegate(_fincentre.OnLoaneeAccountCreated);

            //5.清算中心持仓回合关闭 调用配资中心执行相关逻辑
            _clearCentre.PositionRoundClosedEvent += new IPositionRoundDel(_fincentre.OnPositionRoundClosed);

            //风控中心查询配资服务的事件绑定到 配资中心
            _riskCentre.GetFinServiceDelEvent += (string account) => { return _fincentre[account]; };

            //将管理中心的事件绑定到_fincentre组件 管理中心查询某个账户的配资服务
            //查询配资服务
            qsmgrSrv.GetFinServiceInfoEvent += (string account) =>
            {
                IFinService fs = _fincentre[account];
                if (fs == null) return null;
                return ObjectInfoHelper.GenFinServiceInfo(fs);
            };
            //激活配资服务
            qsmgrSrv.ActiveFinServiceEvent += (string account,out string msg)=>
            {
                return _fincentre.UpdateFinServiceActive(account, true, out msg);
            };
            //冻结配资服务
            qsmgrSrv.InActiveFinServiceEvent += (string account, out string msg) =>
            {
                return _fincentre.UpdateFinServiceActive(account, false, out msg);
            };
            qsmgrSrv.UpdateFinServiceEvent += (string account, decimal ammount, QSEnumFinServiceType type, decimal discount, int agentcode, out string msg) =>
            {
                IAccount acc= _clearCentre[account];
                if(account == null)
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
                        return _fincentre.AddFinService(account, 0, type, 1, acc.AgentCode, out msg);
                    }
                    return _fincentre.AddFinService(account, ammount, type, discount,agentcode.ToString(), out msg);
                }

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


            
            //重置配资服务,从数据库加载对应的配资服务
            _fincentre.Reset();
            //_fincentre.Start();
            finmode_loaded = true;
            **/
        }

        //初始化web管理组件
        private void InitWebManager()
        {
            /*
            debug("初始化WebChannel");
            List<Type> tlist = ContribHelper.GetContribListViaType<IWebManager>();
            if (tlist.Count <= 0)
            {
                debug("没有WebManager的实现文件,无法正确加载配资模块");
                return;
            }
            _webmanager = (IWebManager)Activator.CreateInstance(tlist[0], new object[] { config.FunctionAddress, config.FunctionPort, _clearCentre });
            _webmanager.SendDebugEvent += new DebugDelegate(mgrdebug);

            if (_fincentre != null)
            {
                //更新配资服务
                _webmanager.UpdateFinServiceEvent += (string account, decimal ammount, QSEnumFinServiceType type, decimal discount, int agentcode, out string msg) =>
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
                    _webmanager.SignupPreraceEvent += new IAccountSignForPreraceDel(_racecentre.Signup);//报名

                    _webmanager.QryRaceInfoEvent += new GetRaceInfoDel(_racecentre.GetAccountRaceInfo);//获得比赛状态信息
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
            }

            if (_clearCentre != null)
            {
                //添加配资账户
                _webmanager.AddFinAccoountEvent += new AddAccountDel(_clearCentre.AddNewFinAccount);
            }
            _webmanager.Start();
            webmgrmode_loaded = true;
             * **/
        }

        //初始化信息中心
        private void InitInformationSrv()
        {
            /*
            debug("初始化informationSrv");
            List<Type> tlist = ContribHelper.GetContribListViaType<IInformationCentre>();
            if (tlist.Count <= 0)
            {
                debug("没有InformationCenter的实现文件,无法正确加载配资模块");
                return;
            }

            _infosrv = (IInformationCentre)Activator.CreateInstance(tlist[0], new object[] { config.WebDBServer, config.WebDBUser, config.WebDBPass, config.WebDBPort, config.InfoAddress, config.InfoPort });
            //_infosrv = new InformationCentre(config.WebDBServer, config.WebDBUser, config.WebDBPass, config.WebDBPort, config.InfoAddress, config.InfoPort);
            _infosrv.SendDebugEvent += new DebugDelegate(mgrdebug);
            //注意:通过SocketType.DEALER转发消息,这里会产生当积累的Tick数据达到一定数量的时候会导致原有的tick数据处理发生错误
            //DataFeed->DataFeedRouter->GotTick->TradingServer->websocket.notify_tick
            //dealer发送一定数据,无法到达对应目的地的时,会导致zmq崩溃。因此这里需要修改成统一从FastTickPub获得数据
            //_srv.GotTickEvent += new TickDelegate(_infosrv.newTick);
            //_srv.SendLoginInfoEvent += new LoginInfoDel(_infosrv.newSessionUpdate);

            //管理端请求用户信息
            //qsmgrSrv.GetUserProfileEvent += new GetUserProfileDel(_infosrv.GetUserProfile);

            infomode_loaded = true;
            **/
        }
    }
}
