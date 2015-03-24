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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySummaryViaSecCode", "QryAgentReport - query agent report", "查询代理发展客户的统计信息", QSEnumArgParseType.Json)]
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
            

            //session.ReplyMgr(reqeust);
        }
    }
}
