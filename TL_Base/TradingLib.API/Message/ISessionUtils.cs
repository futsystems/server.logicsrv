using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public static class ISessionUtils
    {
        /// <summary>
        /// 是否是管理端
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool IsManager(this ISession session)
        {
            return session.SessionType == QSEnumSessionType.MANAGER;
        }


        /// <summary>
        /// 是否是管理端
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool IsClient(this ISession session)
        {
            return session.SessionType == QSEnumSessionType.CLIENT;
        }

    }
}
