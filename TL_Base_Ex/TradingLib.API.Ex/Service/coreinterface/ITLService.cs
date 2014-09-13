using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    ///// <summary>
    ///// TLServer继承于接口IService,IDebug拥有基本的start stop islive 以及日志输出功能，同时扩展了TLServer所需要的基础功能
    ///// TrdServer,MgrServer接口均继承该接口
    ///// </summary>
    //public interface ITLService : IService, IDebug
    //{
    //    /// <summary>
    //    /// 服务标识
    //    /// </summary>
    //    Providers ProviderName { get; set; }
    //    /// <summary>
    //    /// 总客户端连接数
    //    /// </summary>
    //    int NumClients { get; }
    //    /// <summary>
    //    /// 登入的客户端数
    //    /// </summary>
    //    int NumClientsLoggedIn { get; }
    //    /// <summary>
    //    /// 恢复与客户端的连接信息
    //    /// </summary>
    //    void RestoreSession();
    //    /// <summary>
    //    /// 通过底层传输发送自定义信息,实现客户端与服务端的通讯
    //    /// </summary>
    //    /// <param name="msg"></param>
    //    /// <param name="type"></param>
    //    /// <param name="client"></param>
    //    void TLSend(string msg, MessageTypes type,string address);
    //    /// <summary>
    //    /// 底层开启的消息处理工作线程数
    //    /// 设定线程数时,不一定要在初始化的时候进行设置,只要在调用对应服务Start函数之前进行设置即可生效
    //    /// 底层具体的线程创建是在Start函数内进行创建的
    //    /// </summary>
    //    int NumWorkers { get; set; }
    //    /// <summary>
    //    /// 底层消息是否启用流控,即当某个客户端消息发送频率大于多少时,我们直接屏蔽该客户端的所有消息
    //    /// 用于防治攻击型的消息阻塞正常业务消息
    //    /// </summary>
    //    bool EnableTPTracker { get; set; }


        

    //    /// <summary>
    //    /// 获取某个交易帐号所有连接地址
    //    /// 某个帐号可能有多个客户端连接
    //    /// </summary>
    //    /// <param name="account"></param>
    //    /// <returns></returns>
    //    string[] AddListForAccount(string account);

    //    /// <summary>
    //    /// 获得某个地址所绑定的交易帐号
    //    /// </summary>
    //    /// <param name="address"></param>
    //    /// <returns></returns>
    //    string  AccountForAddress(string address);

    //    /// <summary>
    //    /// 缓存消息,用于缓存到业务层统一的消息队列然后对外发送消息
    //    /// </summary>
    //    event CacheMessageDel SendCacheMessage;

    //    /// <summary>
    //    /// 客户端注册事件
    //    /// </summary>
    //    event ClientParamDel ClientRegistedEvent;

    //    /// <summary>
    //    /// 客户端注销事件
    //    /// </summary>
    //    event ClientParamDel ClientUnRegistedEvent;

    //}

   
}
