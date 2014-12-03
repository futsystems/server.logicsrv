using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public static class ISessionUtils
    {
        /// <summary>
        /// 获得Sessoin对应的Manager
        /// 如果是ClientManager则返回null
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Manager GetManager(this ISession session)
        {
            if (session.IsManager() && session is Client2Session)
            {
                return (session as Client2Session).Manager;
            }
            return null;
        }
    }
}
