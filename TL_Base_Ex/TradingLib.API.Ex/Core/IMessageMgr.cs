using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;


namespace TradingLib.API
{
    public interface IMessageMgr
    {
        /// <summary>
        /// 将IPacket放到缓存 进行发送
        /// </summary>
        /// <param name="packet"></param>
        void Send(IPacket packet);

        /// <summary>
        /// 获得某个通知列表
        /// </summary>
        /// <param name="predictate"></param>
        /// <returns></returns>
        IEnumerable<ILocation> GetNotifyTargets(Predicate<Manager> predictate);
    }
}
