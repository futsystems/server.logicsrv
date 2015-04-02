using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.MiniService
{
    public class MinServiceTracker
    {
        Dictionary<string, MiniService> miniServiceMap = new Dictionary<string, MiniService>();
        Dictionary<int, MiniServiceSetting> miniSettingMap = new Dictionary<int, MiniServiceSetting>();
        public MinServiceTracker()
        {
            foreach (var ms in ORM.MMiniService.SelectMiniService())
            {
                MiniService service = new MiniService(ms);
                if (service.IsValid)
                {
                    miniServiceMap.Add(ms.Account, service);
                    miniSettingMap.Add(ms.ID, ms);
                }
            }
        }

        public void UpdateMiniService(MiniServiceSetting ms)
        {
            MiniServiceSetting target = null;
            if (miniSettingMap.TryGetValue(ms.ID, out target))
            {
                target.Active = ms.Active;
                ORM.MMiniService.UpdateMiniService(ms);
            }
        }

        /// <summary>
        /// 获得某个交易帐户的迷你交易服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public MiniService this[string account]
        {
            get
            {
                MiniService service;
                if (miniServiceMap.TryGetValue(account, out service))
                {
                    return service;
                }
                return null;
            }
        }
    }
}
