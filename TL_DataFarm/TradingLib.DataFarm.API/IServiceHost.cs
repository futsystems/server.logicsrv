using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.DataFarm.API
{
    public interface IServiceHost
    {
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        string Name { get;}

        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();


        /// <summary>
        /// 创建Connection对象
        /// DataServer不负责传输层面逻辑，只负责业务层面的逻辑,当DataServer认证通过后需要调用ServiceHost来创建Connection
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        IConnection CreateConnection(string sessionID);


        event Action<IServiceHost,IPacket> RequestEvent;
    }
}
