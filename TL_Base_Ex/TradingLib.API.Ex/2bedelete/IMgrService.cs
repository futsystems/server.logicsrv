//﻿using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{

//    public interface IMgrService : ITLService, ISTrdRep, ISTrdReq, ISDataRep//TradeReqInterface,TradeRepInterface,DataReqInterface,DataRepInterface
//    {
//        /// <summary>
//        /// 返回所有客户端所订阅的数据
//        /// </summary>
//        //Basket AllClientBasket { get; }
//        event FindAccountDel SendFindAccountEvent;
//        event FindOrderDel SendFindOrderEvent;
//        /// <summary>
//        /// 验证customer
//        /// </summary>
//        event LoginRequestDel newLoginRequest;//登入服务器
//        /// <summary>
//        /// 查询customer数据集
//        /// </summary>
//        event FindCustomerDataSet newCustomerSetRequest;//查找customer对应的数据集
//        /// <summary>
//        /// server接收到登入请求事件(对外调用验证函数)
//        /// </summary>
//        //event LoginRequestDel newLoginRequest;
//        /// <summary>
//        /// 触发某个客户端登入或者注销信息(通知程序某个客户端登入或注销)
//        /// </summary>
//        //event LoginInfoDel SendLoginInfoEvent;
//        /// <summary>
//        /// 接收到客户端功能列表请求
//        /// </summary>
//        event MessageArrayDelegate newFeatureRequest;//请求功能特性列表
//        /// <summary>
//        /// 接收到客户端的其他未定义请求,需要在具体的TradingServer/中定义
//        /// </summary>
//        event UnknownMessageDelegateSession newUnknownRequestSource;//带地址信息处理

//        /// <summary>
//        /// 检查某个clientID是否有权限接收IAccount的交易信息
//        /// </summary>
//        /// <param name="clientId"></param>
//        /// <param name="a"></param>
//        /// <returns></returns>
//        bool ViewAccountRight(string clientId, IAccount a);

//        /// <summary>
//        /// 向客户端发送服务器信息回报
//        /// </summary>
//        /// <param name="h"></param>
//        //void newHealth(IHealthInfo h);


//        /// <summary>
//        /// 向客户端发送某个交易账号的登入或者注销情况
//        /// </summary>
//        /// <param name="accId"></param>
//        /// <param name="online"></param>
//        /// <param name="clientID"></param>
//        void newSessionUpdate(string accId, bool online, string clientID);

//        /// <summary>
//        /// 向管理客户端发送某个帐户设置变化
//        /// </summary>
//        /// <param name="account"></param>
//        void newAccountSettingChanged(IAccount account);
//        /// <summary>
//        /// 向客户端发送交易账户的比赛变更信息
//        /// </summary>
//        /// <param name="ri"></param>
//        //void newRaceInfo(IRaceInfo ri);

//    }
//}