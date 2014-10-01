using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        ConcurrentDictionary<string, IAccountService> servicemap = new ConcurrentDictionary<string, IAccountService>();

        /// <summary>
        /// 帐户加载服务
        /// </summary>
        /// <param name="service"></param>
        public void BindService(IAccountService service,bool force=true)
        {
            string servicename = service.SN;
            if (servicemap.Keys.Contains(servicename))
            {

                if(force)
                    servicemap[servicename] = service;
            }
            else
            {
                servicemap[servicename] = service;
            }
        }

        /// <summary>
        /// 获得某个服务
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool GetService(string sn, out IAccountService service)
        {
            service = null;

            if (servicemap.TryGetValue(sn, out service))
            {
                return true;
            }
            service = null;
            return false;
            
        }


    }
}
