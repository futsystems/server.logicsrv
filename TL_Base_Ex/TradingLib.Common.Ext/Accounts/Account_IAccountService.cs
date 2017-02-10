﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 帐户服务类接口
    /// 交易帐户服务的相关操作 加载 卸载 查找等
    /// </summary>
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
        /// 卸载服务
        /// </summary>
        /// <param name="service"></param>
        public void UnBindService(IAccountService service)
        {
            string servicename = service.SN;
            IAccountService s = null;
            if (servicemap.Keys.Contains(servicename))
            {
                servicemap.TryRemove(servicename, out s);
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