using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.API
{
    public interface IMessageMgr
    {
        /// <summary>
        /// 将IPacket放到缓存 进行发送
        /// </summary>
        /// <param name="packet"></param>
        void Send(IPacket packet);

    }
}
