using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.Race
{
    /// <summary>
    /// 比赛维护器
    /// </summary>
    public class RaceTracker
    {

        Dictionary<string, Race> racemap = new Dictionary<string, Race>();

        public RaceTracker()
        {
            //加载所有比赛
            foreach (var r in ORM.MRace.SelectRaces())
            {
                if (!racemap.Keys.Contains(r.RaceID))
                    racemap[r.RaceID] = r;
            }
        }


        /// <summary>
        /// 获得某个比赛
        /// </summary>
        /// <param name="raceid"></param>
        /// <returns></returns>
        public Race this[string raceid]
        {
            get
            {
                if (string.IsNullOrEmpty(raceid)) return null;
                Race target = null;
                if (racemap.TryGetValue(raceid, out target))
                {
                    return target;
                }
                return null;
            }
        }

        /// <summary>
        /// 获得所有比赛
        /// </summary>
        public IEnumerable<Race> Races
        {
            get
            {
                return racemap.Values;
            }
        }

        /// <summary>
        /// 判断是否有某个类型的比赛
        /// </summary>
        /// <param name="racetype"></param>
        /// <returns></returns>
        public bool HaveAnyRace(QSEnumRaceType racetype)
        {
            return racemap.Any(r => r.Value.RaceType == racetype);
        }

        public void UpdateRaceSetting(RaceSetting race)
        {
            Race target = null;
            if (racemap.TryGetValue(race.RaceID, out target))
            {
                target.PromotNum = race.PromotNum;
                target.EliminateNum = race.EliminateNum;
                target.EntryNum = race.EntryNum;

                ORM.MRace.UpdateRace(target);
            }
            else
            {
                target = new Race();
                target.PromotNum = race.PromotNum;
                target.EliminateNum = race.EliminateNum;
                target.EntryNum = race.EntryNum;

                target.RaceID = race.RaceID;
                target.RaceType = race.RaceType;

                target.StartTime = race.StartTime;
                target.BeginSignTime = race.BeginSignTime;
                target.EndSignTime = race.EndSignTime;

                ORM.MRace.InsertRace(target);

                //加入内存
                racemap[target.RaceID] = target;
            }

        }
        /// <summary>
        /// 通过accountracestatus得到对应的比赛
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public Race GetRaceViaRaceStatus(QSEnumAccountRaceStatus status)
        {
            switch (status)
            {
                case QSEnumAccountRaceStatus.ELIMINATE:
                    return null;
                case QSEnumAccountRaceStatus.INSEMIRACE:
                    return this["SEMIRACE"];
                case QSEnumAccountRaceStatus.INREAL1:
                    return this["REAL1"];
                case QSEnumAccountRaceStatus.INREAL2:
                    return this["REAL2"];
                case QSEnumAccountRaceStatus.INREAL3:
                    return this["REAL3"];
                case QSEnumAccountRaceStatus.INREAL4:
                    return this["REAL4"];
                case QSEnumAccountRaceStatus.INREAL5:
                    return this["REAL5"];
                default:
                    return null;
            }
        }


        /// <summary>
        /// 最新的比赛
        /// </summary>
        public Race LatestPrerace
        {
            get
            {
                Race latest = null;
                foreach (Race r in racemap.Values)
                {
                    if (r.RaceType != QSEnumRaceType.PRERACE) continue;
                    if (latest == null || latest.StartTime < r.StartTime)
                        latest = r;

                }
                return latest;
            }

        }
    }
}
