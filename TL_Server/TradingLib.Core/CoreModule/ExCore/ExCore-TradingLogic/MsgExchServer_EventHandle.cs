using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class MsgExchServer
    {
        #region Client-->TLServer发出请求事件触发的回调函数
        //请求服务端支持的特性列表
        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();

            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.ORDERNOTIFY);
            //f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.EXECUTENOTIFY);

            //客户端主动发起请求的功能
            f.Add(MessageTypes.SENDORDER);//发送委托
            //f.Add(MessageTypes.ORDERCANCELREQUEST);//发送取消委托
            f.Add(MessageTypes.QRYACCOUNTINFO);//查询账户信息
            //f.Add(MessageTypes.QRYCANOPENPOSITION);//查询可开
            f.Add(MessageTypes.REQCHANGEPASS);//请求修改密码


            return f.ToArray();
        }


        //服务端启动时,统一请求列表中的数据,具体客户端需求的数据根据需要发送。
        //注册symbol数据机制,客户端维护本地basket,需要向服务端请求symbol数据时,统一将整个basket的symbol信息向服务端请求。
        //当收到信息后，我们通过反解析得到basket，然后用DataRouter向对应的数据通道请求symbol数据
        void tl_newRegisterSymbols(string client, string mbstring)
        {
            try
            {
                //市场数据时IF1302 CN_XXXX FUT这样的字符串,我们需要将他们反序列化成secuirty获得准确的symbol取字头然后才可以得到正确的security
                logger.Debug("Got Market data request : " + client + " " + mbstring);
                //SymbolBasket b = SymbolBasketImpl.FromString(mbstring);
                //FastTickMgrRegisterSymbol(b);
                //用于tradingServer注册获得数据 用于模拟成交以及 财务计算/客户端是直接连接到tick pub获得数据
                //_datafeedRouter.RegisterSymbols(b);//数据路由向不同的数据接口提交数据注册
            }
            catch (Exception ex)
            {
                logger.Error("客户端注册市场数据出错:" + ex.ToString());
            }
        }

        /// <summary>
        /// loginType 
        /// 0:外部鉴权认证
        /// 1:清算中心验证
        /// 2:游客验证
        /// </summary>
        /// <param name="clientinfo"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void tl_newLoginRequest(TrdClientInfo clientinfo, LoginRequest request, ref LoginResponse response)
        {
            try
            {
                bool login = false;
                logger.Info("Got login request:" + request.LoginID + "|" + request.Passwd + " Type:" + request.LoginType.ToString());
                
                IAccount account=null;
                if (request.LoginType == 0)
                {
                    if (!TLCtxHelper.EventSession.IsAuthEventBinded)
                    {
                        login = false;
                        response.Authorized = false;
                        response.RspInfo.Fill("LOGINTYPE_NOT_SUPPORT");
                    }
                    else
                    {
                        logger.Info("系统通过UCenter鉴权认证");
                        TLCtxHelper.EventSession.FireAuthUserEvent(clientinfo, request, ref response);
                        //AuthUserEvent(clientinfo, request, ref response);
                        //如果底层登入成功 则检查具体的服务信息，如果服务不存在则仍然是登入不成功
                        login = response.Authorized;
                        if (login)
                        {
                            response.LoginID = request.LoginID;

                            logger.Info("logined:" + response.LoginID + " userid:" + response.UserID.ToString());
                            
                            bool servicevalid = true;
                            if (request.ServiceType == 0)
                            {
                                //account = _clearcentre.QryAccount(response.UserID, QSEnumAccountCategory.SIMULATION);
                            }
                            else if (request.ServiceType == 1)
                            {
                                //account = _clearcentre.QryAccount(response.UserID, QSEnumAccountCategory.REAL);
                            }
                            else
                            {
                                response.Authorized = false;
                                response.RspInfo.Fill("SERVICE_NOT_VALID");
                                servicevalid = false;
                            }


                            if (servicevalid)
                            {
                                if (account == null)
                                {
                                    response.Authorized = false;
                                    response.RspInfo.Fill("ACCOUNT_NOT_CREATED");
                                }
                                else
                                {
                                    response.Account = account.ID;
                                    response.AccountType = account.Category;
                                }
                            }
                        }
                    }
                }
                else if (request.LoginType == 1)
                {
                    logger.Info("系统通过清算中心认证,LoginID:" + request.LoginID + " Password:" + request.Passwd);
                    //1.检查帐户是否存在

                    //1.检查帐户是否存在
                    //获得当前登入终端数量
                    int loginnums = tl.ClientsForAccount(request.LoginID).Count();
                    //如果当前登入个数大于等于系统允许的登入数量则拒绝登入
                    logger.Info("account:" + request.LoginID + " current login num:" + loginnums.ToString());

                    if (loginnums >= loginTerminalNum)
                    {
                        response.Authorized = false;
                        response.RspInfo.Fill("TERMINAL_NUM_LIMIT");
                    }
                    else
                    {
                        login = TLCtxHelper.ModuleAccountManager.VaildAccount(request.LoginID, request.Passwd);
                        response.Authorized = login;
                        if (login)
                        {
                            response.LoginID = request.LoginID;
                            response.Account = request.LoginID;
                            account = TLCtxHelper.ModuleAccountManager[request.LoginID];
                            response.AccountType = account.Category;


                        }
                        else
                        {
                            response.RspInfo.Fill("INVALID_LOGIN");
                        }
                    }
                }
                //游客登入
                else if (request.LoginType == 2)
                {
                    //string MAC = request.MAC;
                    //string accid = _clearcentre.ValidAccountViaMAC(MAC);
                    //if (!string.IsNullOrEmpty(accid))
                    //{
                    //    debug("MAC认证通过,MAC:" + MAC + " account:" + account);
                    //    response.Authorized = true;
                    //    response.LoginID = MAC;
                    //    response.Account = accid;
                    //    account = TLCtxHelper.CmdAccount[accid];
                    //    response.AccountType = account.Category;

                    //}
                    //else
                    //{
                    //    response.Authorized = false;
                    //    response.RspInfo.Fill("INVALID_LOGIN");
                    //}
                
                }//未支持登入方式
                else
                {
                    response.Authorized = false;
                    response.RspInfo.Fill("LOGINTYPE_NOT_SUPPORT");
                }

                //若有帐户对象 检查域和管理员对象 进行域过期和管理是否激活进行限制
                if (account != null)
                {
                    if (account.Domain.IsExpired())//域过期
                    {
                        response.Authorized = false;
                        response.RspInfo.Fill("PLATFORM_EXPIRED");
                    }
                    Manager mgr = BasicTracker.ManagerTracker[account.Mgr_fk];
                    if (mgr == null || (!mgr.Active))
                    {
                        response.Authorized = false;
                        response.RspInfo.Fill("PLATFORM_EXPIRED");
                    }
                }
                
                //对外触发登入事件
                if (response.Authorized)
                {
                    TLCtxHelper.EventSession.FireAccountLoginSuccessEvent(response.Account);
                    //if (AccountLoginSuccessEvent != null)
                    //    AccountLoginSuccessEvent(response.Account);
                }
                else
                {
                    TLCtxHelper.EventSession.FireAccountLoginFailedEvent(response.Account);
                    //if (AccountLoginFailedEvent != null)
                    //    AccountLoginFailedEvent(response.Account);
                }
            }
            catch (Exception ex)
            {

                throw (new QSTradingServerValidAccountError(ex));
            }
        }


        #endregion

    }
}
