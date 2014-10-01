using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace Lottoqq.Race
{
    /// <summary>
    /// 比赛服务的Tracker 用于管理和维护所有的比赛服务
    /// 
    /// </summary>
    public class RaceServiceTracker
    {

        ConcurrentDictionary<int, RaceService> idxrsmap = new ConcurrentDictionary<int, RaceService>();//数据库全局编号与RaceService的映射
        ConcurrentDictionary<string, RaceService> accountrsmap = new ConcurrentDictionary<string, RaceService>();//帐户与RaceService的映射

        public RaceServiceTracker()
        {
            //从数据库加载秘籍服务
            foreach (RaceService rs in TradingLib.ORM.MRaceService.SelectRaceService())
            {
                try
                {
                    AddRaceService(rs);

                }
                catch (Exception ex)
                {

                }
            }
        }

        public void AddRaceService(RaceService rs)
        {
            if (rs.ID > 0)
            {
                if (rs.Account == null)
                {
                    throw new RaceErrorAccountNull();
                }
                //如果未加载该帐户的秘籍服务 则进行加载并绑定
                if (!HaveRaceService(rs.Account.ID))
                {
                    //TLCtxHelper.Debug("--------------为帐户:" + rs.Account.ID + " 添加比赛服务-------------------");
                    accountrsmap.TryAdd(rs.Account.ID, rs);
                    idxrsmap.TryAdd(rs.ID, rs);

                    //将秘籍服务绑定到帐户
                    //rs.Account.RaceService = rs;
                    rs.Account.BindService(rs);
                }
                else
                {
                    throw new RaceErrorServiceLoaded();
                }
            }
            else
            {
                TradingLib.ORM.MRaceService.InsertRaceService(rs);
                this.AddRaceService(rs);
            }
        }

        /// <summary>
        /// 检查某个帐号是否有比赛服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool HaveRaceService(string account)
        {
            if (accountrsmap.Keys.Contains(account))
                return true;
            return false;
        
        }

        /// <summary>
        /// 返回某个帐号的比赛服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public RaceService this[string account]
        {
            get
            {
                RaceService rs = null;
                if (accountrsmap.TryGetValue(account, out rs))
                {
                    return rs;
                }
                return null;
            }
        }

        public RaceService[] RaceServices
        {
            get
            {
                return accountrsmap.Values.ToArray();
            }
        }

        /// <summary>
        /// 更新比赛状态
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="status"></param>
        public void UpdateRaceStatus(RaceService rs, QSEnumRaceStatus status)
        {
            rs.Status = status;
            TradingLib.ORM.MRaceService.UpdateRaceStatus(rs);
        }

        /// <summary>
        /// 参加比赛重置比赛服务参数
        /// </summary>
        /// <param name="rs"></param>
        public void EntryRace(RaceService rs)
        {
            rs.EntryTime = DateTime.Now;//设定参赛时间为当前

            rs.Status = QSEnumRaceStatus.INRACEE;//设置比赛服务状态

            //重置比赛资金
            rs.LastFutEquity = RaceConstant.FutSimEquity;
            rs.LastOptEquity = RaceConstant.OptSimEquity;
            rs.LastMJEquity = RaceConstant.MJSimEquity;

            TradingLib.ORM.MRaceService.UpdateAll(rs);
        }
    }
}
