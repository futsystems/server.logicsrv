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

        /// <summary>
        /// 注册某个合约的行情数据
        /// </summary>
        /// <param name="symbol"></param>
        public void RegisterSymbol(Symbol sym)
        {
            debug("Register Symbol Market Data:" + sym.Symbol, QSEnumDebugLevel.INFO);
            SymbolBasket b = new SymbolBasketImpl(sym);
            _datafeedRouter.RegisterSymbols(b);
            
        }

        /// <summary>
        /// 注册某个品种的所有可交易合约的行情数据
        /// </summary>
        /// <param name="sec"></param>
        //public void RegisterSymbol(SecurityFamily sec)
        //{
        //    debug("Register security's market data:" + sec.Code, QSEnumDebugLevel.INFO);
        //    SymbolBasket b = BasicTracker.SymbolTracker.GetBasketAvabileViaSecurity(sec);
        //    _datafeedRouter.RegisterSymbols(b);
        //}

        //服务端启动时,统一请求列表中的数据,具体客户端需求的数据根据需要发送。
        //注册symbol数据机制,客户端维护本地basket,需要向服务端请求symbol数据时,统一将整个basket的symbol信息向服务端请求。
        //当收到信息后，我们通过反解析得到basket，然后用DataRouter向对应的数据通道请求symbol数据
        void tl_newRegisterSymbols(string client, string mbstring)
        {
            try
            {
                //市场数据时IF1302 CN_XXXX FUT这样的字符串,我们需要将他们反序列化成secuirty获得准确的symbol取字头然后才可以得到正确的security
                debug("Got Market data request : " + client + " " + mbstring, QSEnumDebugLevel.DEBUG);
                //SymbolBasket b = SymbolBasketImpl.FromString(mbstring);
                //FastTickMgrRegisterSymbol(b);
                //用于tradingServer注册获得数据 用于模拟成交以及 财务计算/客户端是直接连接到tick pub获得数据
                //_datafeedRouter.RegisterSymbols(b);//数据路由向不同的数据接口提交数据注册
            }
            catch (Exception ex)
            {
                debug("客户端注册市场数据出错:" + ex.ToString());
                throw (new QSTradingServerRegistSymbolError(ex));
            }
        }



        /// <summary>
        /// 对外提供取消委托操作
        /// 管理端取消委托
        /// </summary>
        /// <param name="val"></param>
        public void CancelOrder(long val)
        {
            try
            {
                debug("Got CancelOrder :" + val, QSEnumDebugLevel.INFO);
                Order o = _clearcentre.SentOrder(val);
                //如果委托处于pending状态
                if (o.IsPending())
                {
                    //如果委托状态表面需要通过broker来取消委托 则通过broker来进行撤单
                    if (o.CanCancel())//opened partfilled
                    {
                        _brokerRouter.CancelOrder(o.id);
                    }
                    else if (o.Status == QSEnumOrderStatus.Submited)//已经通过broker提交 该状态无法立即撤单 需要等待委托状态更新为Opened或者 被定时程序发现是一个错误委托
                    {
                        debug("委托:" + val.ToString() + "处于Submited,等待broker返回", QSEnumDebugLevel.INFO);
                    }
                    else if (o.Status == QSEnumOrderStatus.Placed)//
                    {
                        debug("委托:" + val.ToString() + "处于Placed,等待系统返回", QSEnumDebugLevel.INFO);
                    }
                }
                else
                {
                    debug("委托:" + val.ToString() + "不可撤销", QSEnumDebugLevel.WARNING);
                }
                
            }
            catch (Exception ex)
            {
                debug("取消委托出错:" + ex.ToString());
                throw (new QSTradingServerCancleError(ex));
            }

        }



        void tl_newLoginRequest(TrdClientInfo clientinfo, LoginRequest request, ref LoginResponse response)
        {
            try
            {
                bool login = false;
                debug("Got login request:" + request.LoginID + "|" + request.Passwd +" Type:"+request.LoginType.ToString(), QSEnumDebugLevel.INFO);
                
                IAccount account=null;
                //如果没有用户认证事件的绑定 则调用清算中心的帐号验证进行验证，否则通过认证事件的绑定进行认证
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
                        debug("系统通过UCenter鉴权认证", QSEnumDebugLevel.INFO);
                        AuthUserEvent(clientinfo, request, ref response);
                        //如果底层登入成功 则检查具体的服务信息，如果服务不存在则仍然是登入不成功
                        login = response.Authorized;
                        if (login)
                        {
                            response.LoginID = request.LoginID;

                            debug("logined:" + response.LoginID + " userid:" + response.UserID.ToString(), QSEnumDebugLevel.INFO);
                            
                            bool servicevalid = true;
                            if (request.ServiceType == 0)
                            {
                                account = _clearcentre.QryAccount(response.UserID, QSEnumAccountCategory.SIMULATION);
                            }
                            else if (request.ServiceType == 1)
                            {
                                account = _clearcentre.QryAccount(response.UserID, QSEnumAccountCategory.REAL);
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
                    debug("系统通过清算中心认证,LoginID:" + request.LoginID + " Password:" + request.Passwd, QSEnumDebugLevel.INFO);
                    login = _clearcentre.VaildAccount(request.LoginID, request.Passwd);
                    response.Authorized = login;
                    if (login)
                    {
                        response.LoginID = request.LoginID;
                        response.Account = request.LoginID;
                        account = _clearcentre[request.LoginID];
                        response.AccountType = account.Category;
                    }
                    else
                    {
                        response.RspInfo.Fill("INVALID_LOGIN");
                    }
                }
                //游客登入
                else if (request.LoginType == 2)
                {
                    string MAC = request.MAC;
                    string accid = _clearcentre.ValidAccountViaMAC(MAC);
                    if (!string.IsNullOrEmpty(accid))
                    {
                        debug("MAC认证通过,MAC:" + MAC + " account:" + account);
                        response.Authorized = true;
                        response.LoginID = MAC;
                        response.Account = accid;
                        account = _clearcentre[accid];
                        response.AccountType = account.Category;

                    }
                    else
                    {
                        response.Authorized = false;
                        response.RspInfo.Fill("INVALID_LOGIN");
                    }
                
                }
                else
                {
                    response.Authorized = false;
                    response.RspInfo.Fill("LOGINTYPE_NOT_SUPPORT");
                }
                if (account.Domain.DateExpired > 0 && account.Domain.DateExpired < Util.ToTLDate())//域过期
                {
                    clientinfo.AuthorizedFail();
                    response.Authorized = false;
                    response.RspInfo.Fill("PLATFORM_EXPIRED");
                }
                
                //对外触发登入事件
                if (response.Authorized)
                {
                    if (AccountLoginSuccessEvent != null)
                        AccountLoginSuccessEvent(response.Account);
                }
                else
                {
                    if (AccountLoginFailedEvent != null)
                        AccountLoginFailedEvent(response.Account);
                }
            }
            catch (Exception ex)
            {

                throw (new QSTradingServerValidAccountError(ex));
            }
        }


        /// <summary>
        /// 响应客户端委托操作
        /// </summary>
        /// <param name="request"></param>
        void tl_newOrderActionRequest(OrderActionRequest request)
        {
            OrderAction action = request.OrderAction;
            Order o = null;
            IAccount account = _clearcentre[request.OrderAction.Account];

            //1.通过交易系统分配的全局委托ID进行识别委托
            if (action.OrderID != 0)
            {
                o = _clearcentre.SentOrder(action.OrderID);
            }
            else 
            {
                debug("OrderAction OrderRef:" + action.OrderRef + " Front:" + action.FrontID.ToString() + " Session:" + action.SessionID.ToString() + " OrderSysID:" + action.OrderExchID +" OrderRef:"+action.OrderRef +" Request:"+action.RequestID, QSEnumDebugLevel.INFO);
                foreach (Order tmp in account.Orders)
                {
                    if ((tmp.OrderRef == action.OrderRef && tmp.FrontIDi == action.FrontID && tmp.SessionIDi == action.SessionID) || (tmp.OrderSysID == action.OrderExchID))
                    {
                        o = tmp;
                    }
                }
            }

            if (o != null)
            {
                //撤销委托操作
                if (action.ActionFlag == QSEnumOrderActionFlag.Delete)
                {
                    //如果委托处于pending状态
                    if (o.IsPending())
                    {
                        //如果委托状态表面需要通过broker来取消委托 则通过broker来进行撤单
                        if (o.CanCancel())//opened partfilled
                        {
                            //委托操作回报
                            OrderActionNotify notify = ResponseTemplate<OrderActionNotify>.SrvSendNotifyResponse(action.Account);
                            notify.BindRequest(request);

                            //通过brokerrouter取消委托
                            _brokerRouter.CancelOrder(o.id);
                        }
                        //处于中间状态Placed或Submited由系统单独逻辑进行定时检查 用于清除处于未知状态的委托
                        else if (o.Status == QSEnumOrderStatus.Submited || o.Status == QSEnumOrderStatus.Placed)//已经通过broker提交 该状态无法立即撤单 需要等待委托状态更新为Opened或者 被定时程序发现是一个错误委托
                        {
                            debug(string.Format("委托:{0} 处于:{1},等待broker返回",o.id,o.Status), QSEnumDebugLevel.INFO);
                            ErrorOrderActionNotify notify = ResponseTemplate<ErrorOrderActionNotify>.SrvSendNotifyResponse(action.Account);
                            notify.OrderAction = action;
                            notify.RspInfo.Fill("ORDER_IN_PRESTAGE");
                            CachePacket(notify);
                        }
                        
                    }
                    else
                    {
                        //委托不可撤销
                        ErrorOrderActionNotify notify = ResponseTemplate<ErrorOrderActionNotify>.SrvSendNotifyResponse(action.Account);
                        notify.OrderAction = action;
                        notify.RspInfo.Fill("ORDER_CAN_NOT_BE_DELETE");
                        CachePacket(notify);
                    }
                }
            }
            else//委托操作所指定的委托不存在 委托操作字段错误
            {
                Util.Debug("对应委托没有找到", QSEnumDebugLevel.WARNING);
                ErrorOrderActionNotify notify = ResponseTemplate<ErrorOrderActionNotify>.SrvSendNotifyResponse(action.Account);
                notify.OrderAction = action;
                notify.RspInfo.Fill("ORDERACTION_BAD_FIELD");
                CachePacket(notify);
            }

        }

        #endregion

    }
}
