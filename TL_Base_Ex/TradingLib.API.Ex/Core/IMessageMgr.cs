using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.API
{
    public interface IMessageMgr
    {
        //void newRaceInfo(IAccount acc);

        /// <summary>
        /// 查询配资服务
        /// </summary>
        //event GetRaceInfoDel GetRaceInfoEvent;

        /// <summary>
        /// 获得某个账户的配资服务信息
        /// </summary>
        //event GetFinServiceInfoDel GetFinServiceInfoEvent;
        /// <summary>
        /// 激活配资服务
        /// </summary>
        //event AccountParamDel ActiveFinServiceEvent;
        /// <summary>
        /// 冻结配资服务
        /// </summary>
        //event AccountParamDel InActiveFinServiceEvent;
        /// <summary>
        /// 更新配资服务
        /// </summary>
        //event UpdateFinServiceDel UpdateFinServiceEvent;

        /// <summary>
        /// 获得帐户网站注册信息
        /// </summary>
        //event GetUserProfileDel GetUserProfileEvent;

        /// <summary>
        /// 将IPacket放到缓存 进行发送
        /// </summary>
        /// <param name="packet"></param>
        void Send(IPacket packet);

    }
}
