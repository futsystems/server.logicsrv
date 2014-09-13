using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottoqq.Race;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
namespace TradingLib.ORM
{
    public class MRaceSettle : MBase
    {
        /// <summary>
        /// 插入结算记录 使用过程,当数据库操作完成时,我们再更新内存对象数据
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int SettleRaceService(RaceService service)
        {
            if (!service.IsSettled)
            {
                //将比赛服务转换成对应的结算记录
                RaceSettle rs = RaceSettle.RaceService2Settle(service);

                using (DBMySql db = new DBMySql())
                {
                    int row = 0;
                    using (var transaction = db.Connection.BeginTransaction())
                    {

                        string query = "INSERT INTO lottoqq_race_settle (account,settleday,optlastequity,optrealizedpl,optcommission,optnowequity,mjlastequity,mjrealizedpl,mjcommission,mjnowequity,futlastequity,futrealizedpl,futcommission,futnowequity) values (@account,@settleday,@optlastequity,@optrealizedpl,@optcommission,@optnowequity,@mjlastequity,@mjrealizedpl,@mjcommission,@mjnowequity,@futlastequity,@futrealizedpl,@futcommission,@futnowequity)";
                        row = db.Connection.Execute(query, new { account = rs.Account, settleday = rs.SettleDay, optlastequity = rs.OptLastEquity, optrealizedpl = rs.OptRealizedPL, optcommission = rs.OptCommission, optnowequity = rs.OptNowEquity, mjlastequity = rs.MJLastEquity, mjrealizedpl = rs.MJRealizedPL, mjcommission = rs.MJCommission, mjnowequity = rs.MJNowEquity, futlastequity = rs.FutLastEquity, futrealizedpl = rs.FutRealizedPL, futcommission = rs.FutCommission, futnowequity = rs.FutNowEquity });
                        //更新对象的Id为数据库里新增的Id,假如增加之后不需要获得新增的对象，
                        //只需将对象添加到数据库里，可以将下面的一行注释掉。
                        SetIdentity(db.Connection, id => rs.ID = id, "id", "lottoqq_race_settle");

                        query = "UPDATE lottoqq_race SET lastfutequity = @lastfutequity,lastoptequity = @lastoptequity,lastmjequity = @lastmjequity,settleday = @settleday WHERE id = @ID";
                        row += db.Connection.Execute(query, new { lastfutequity = rs.FutNowEquity, lastoptequity = rs.OptNowEquity, lastmjequity = rs.MJNowEquity, settleday = rs.SettleDay, ID = service.ID });

                        transaction.Commit();

                        //更新内存中结算日和昨日权益 结算完毕后，昨日权益需要更新
                        service.SettleDay = rs.SettleDay;
                        service.LastFutEquity = rs.FutNowEquity;
                        service.LastOptEquity = rs.OptNowEquity;
                        service.LastMJEquity = rs.MJNowEquity;
                    }

                    return row;
                }
            }
            return 0;
        }
    }
}
