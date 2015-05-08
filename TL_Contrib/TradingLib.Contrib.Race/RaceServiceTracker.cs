using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.Race
{
    public class RaceServiceTracker
    {

        Dictionary<string, RaceService> rsmap = new Dictionary<string, RaceService>();

        public RaceServiceTracker()
        {
            foreach (var rs in ORM.MRace.SelectRaceServices())
            {
                if (!rsmap.Keys.Contains(rs.Account))
                    rsmap[rs.Account] = rs;
            }
        }


        /// <summary>
        /// 获得某个交易帐户的比赛服务
        /// 如果帐户不为空或null，如果有比赛服务则返回 没有则创建后返回
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public RaceService this[string account]
        {
            get
            {
                if (string.IsNullOrEmpty(account)) return null;
                if (TLCtxHelper.CmdAccount[account] == null) return null;//帐户不存在

                RaceService target = null;
                if (rsmap.TryGetValue(account, out target))
                {
                    return target;
                }
                else//如果没有则给该帐户创建比赛服务并返回
                {
                    RaceService rs = new RaceService();
                    rs.Account = account;

                    this.UpdateRaceService(rs);
                    return rsmap[account];
                }
            }
        }


        /// <summary>
        /// 获得所有比赛服务
        /// </summary>
        public IEnumerable<RaceService> RaceServices
        {
            get
            {
                return rsmap.Values;
            }
        }


        public void UpdateRaceService(RaceService rs)
        {
            RaceService target = null;
            //已经存在比赛服务
            if (rsmap.TryGetValue(rs.Account, out target))
            {
                target.RaceID = rs.RaceID;
                target.RaceStatus = rs.RaceStatus;
                target.EntryTime = rs.EntryTime;

                ORM.MRace.UpdateRaceService(target);
            }
            else
            {
                target = new RaceService();
                target.Account = rs.Account;
                target.InitRaceService();
                target.EntryTime = 0;
                target.RaceID = "";
                target.RaceStatus = QSEnumAccountRaceStatus.NORACE;

                ORM.MRace.InsertRaceService(target);
                if (!rsmap.Keys.Contains(target.Account))
                    rsmap[target.Account] = target;
            }

        }
    }
}
