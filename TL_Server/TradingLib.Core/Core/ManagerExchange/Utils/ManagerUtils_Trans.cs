using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public static partial class MangerUtils
    {

        /// <summary>
        /// 消息通知模型
        /// 1.针对应答的回报 通过绑定Request或者ISession中的 地址来定位到消息对端，形成一对一的发送
        /// 2.Root端的操作 需要同步发送到对应的代理客户端 即Root端确认出金操作 需要广播到对应的管理端 比如代理或代理的财务人员也出入登入状态
        /// </summary>
        /// <returns></returns>
        //public static ILocation[] GetTargets(this Manager mgr,int basemgr)
        //{ 
            
        //}
    }
}
