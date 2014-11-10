using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.API
{
    public interface IConnecter:IService
    {
        /// <summary>
        /// 连接标识
        /// </summary>
        string Token { get; }
        /// <summary>
        /// 连接成功事件
        /// </summary>
        event IConnecterParamDel Connected;
        /// <summary>
        /// 连接断开事件
        /// </summary>
        event IConnecterParamDel Disconnected;

        
    }
}
