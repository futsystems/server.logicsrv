using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Protocol;

namespace TradingLib.Contrib
{
    
    public partial class HistReportCentre
    {

        /// <summary>
        /// 查询代理客户的统计信息
        /// 按品种的交易手数,手续费，平仓盈亏等信息
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySummaryViaSecCode", "QrySummaryViaSecCode - query agent report", "查询代理发展客户的统计信息", QSEnumArgParseType.Json)]
        public void CTE_AgentReport(ISession session, string reqeust)
        {
            Manager manager = session.GetManager();
            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(reqeust);

            var startDate = int.Parse(data["start_date"].ToString());
            var endDate = int.Parse(data["end_date"].ToString());
            var manager_id = int.Parse(data["manager_id"].ToString());

            IEnumerable<SummaryViaSec> reports = ORM.MReport.GenSummaryViaSecCode(startDate, endDate, manager_id);

            if (reports != null && reports.Count()>=1)
            {
                //debug("report:" + Mixins.Json.JsonMapper.ToJson(report),QSEnumDebugLevel.INFO);
                session.ReplyMgr(reports.ToArray());
            }
            else
            {
                session.OperationError(new FutsRspError("未查到任何统计结果"));
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountSummary", "QryAccountSummary - query agent summary report", "查询某个交易帐户一段时间内的统计", QSEnumArgParseType.Json)]
        public void CTE_AccountReport(ISession session, string reqeust)
        {
            Manager manager = session.GetManager();
            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(reqeust);

            var startDate = int.Parse(data["start_date"].ToString());
            var endDate = int.Parse(data["end_date"].ToString());
            var account = data["account"].ToString();

            if (TLCtxHelper.CmdAccount[account] == null)
            {
                session.OperationError(new FutsRspError("未查到任何统计结果"));
                return;
            }

            SummaryAccount summary = new SummaryAccount();
            summary.Account = account;
            summary.CashIn = ORM.MReport.CashIn(account, startDate, endDate);
            summary.CashOut = ORM.MReport.CashOut(account, startDate, endDate);

            IEnumerable<SummaryAccountItem> reports = ORM.MReport.GenSummaryAccount(startDate, endDate, account);
            if (reports != null && reports.Count() >= 0)
            {
                summary.Items = reports.ToArray();
            }
            else
            {
                summary.Items = null;
            }
            session.ReplyMgr(summary);
        }

        /// <summary>
        /// 提供某个Manager 返回与该管理员相同的 判定谓词
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        Predicate<Manager> GetSameRoot(Manager root)
        {
            Predicate<Manager>  func = (mgr) =>
            {
                //该custinfoex 绑定了管理端
                if (mgr == null) return false;
                if (mgr.ID == root.ID)
                    return true;
                return false;
            };
            return func;
        }

        [TaskAttr("每10秒采集不同分区的统计信息",10,0,"每10秒采集不同分区的统计信息")]
        public void CTE_RealReport()
        { 
            foreach(var d in BasicTracker.DomainTracker.Domains)
            {
                //获得所有分区的超级管理员
                Manager m = d.GetRootManager();
                if(m != null)
                {
                    Util.Debug("send status to manger");
                    TLCtxHelper.Ctx.MessageMgr.Notify(ContribName, "Statistic", UpdateDomainStaticMap(d), GetSameRoot(m));
                }  
            }
        }


        Dictionary<int,DomainStatistic> domainStaticMap = new Dictionary<int,DomainStatistic>();
        public DomainStatistic UpdateDomainStaticMap(Domain d)
        {
            IEnumerable<IAccount> accounts = d.GetAccounts().ToArray();

            if (!domainStaticMap.Keys.Contains(d.ID))
            {
                DomainStatistic ds = new DomainStatistic();
                ds.Domain_ID = d.ID;
                domainStaticMap.Add(d.ID,ds);
            }

            DomainStatistic statistic = domainStaticMap[d.ID];

            statistic.AccNumTotal = accounts.Count();
            statistic.AccNumRegisted = 0;
            statistic.AccNumTraded = accounts.Where(ac => ac.Commission != 0 || ac.Positions.Count() != 0).Count();
            
            
            statistic.Commission = accounts.Sum(ac => ac.Commission);
            statistic.MarginTotal = accounts.Sum(ac => ac.Margin);
            statistic.RealizedPL = accounts.Sum(ac => ac.RealizedPL);
            statistic.UnRealizedPL = accounts.Sum(ac => ac.UnRealizedPL);
            statistic.LongPositionHold = accounts.Sum(ac => ac.PositionsLong.Sum(pos => pos.UnsignedSize));
            statistic.ShortPositionHold = accounts.Sum(ac => ac.PositionsShort.Sum(pos => pos.UnsignedSize));

            statistic.MaxMarginTotal = Math.Max(statistic.MaxMarginTotal, statistic.MarginTotal);
            statistic.MaxLongPositionHold = Math.Max(statistic.MaxLongPositionHold, statistic.LongPositionHold);
            statistic.MaxShortPositionHold = Math.Max(statistic.MaxShortPositionHold, statistic.ShortPositionHold);

            statistic.Equity = accounts.Sum(acc => acc.NowEquity);
            statistic.Credit = accounts.Sum(acc => acc.Credit);
            statistic.CashIn = accounts.Sum(acc => acc.CashIn);
            statistic.CashOut = accounts.Sum(acc => acc.CashOut);

            return statistic;
        }



    }
}
