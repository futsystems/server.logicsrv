using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.API
{
    public interface IWebManager:IDebug,IMail,IService
    {
        
        /// <summary>
        /// 添加模拟帐户
        /// </summary>
        event AddAccountDel AddSimAccoountEvent;

        /// <summary>
        /// 报名参加晋级赛
        /// </summary>
        event IAccountSignForPreraceDel SignupPreraceEvent;//报名参加晋级赛

        /// <summary>
        /// 查询晋级赛比赛状态
        /// </summary>
        event GetRaceInfoDel QryRaceInfoEvent;


        /// <summary>
        /// 添加配资账户
        /// </summary>
        event AddAccountDel AddFinAccoountEvent;
        /// <summary>
        /// 添加配资服务
        /// </summary>
        event AddFinServiceDel AddFinServiceEvent;
        /// <summary>
        /// 更新配资服务
        /// </summary>
        event UpdateFinServiceDel UpdateFinServiceEvent;
        /// <summary>
        /// 激活配资服务
        /// </summary>
        event AccountParamDel ActiveFinServiceEvent;
        /// <summary>
        /// 冻结配资服务
        /// </summary>
        event AccountParamDel InActiveFinServiceEvent;
        /// <summary>
        /// 验证出金
        /// </summary>
        event ValidWithdrawDel ValidWithdrawEvent;
        
        /// <summary>
        /// 查询配资服务
        /// </summary>
        event AccountParamDel QryFinServiceEvent;

        /// <summary>
        /// 调整配资服务
        /// </summary>
        event TrimFinServiceDel TrimFinServiceEvent;
    }
}
