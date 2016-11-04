using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        /// <summary>
        /// 查询汇率信息
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExchangeRates", "QryExchangeRates - qry exchange rate", "查询汇率信息")]
        public void CTE_QryExchangeRates(ISession session)
        {
            Manager manager = session.GetManager();
            ExchangeRate[] rates = manager.Domain.GetExchangeRates(TLCtxHelper.ModuleSettleCentre.Tradingday).ToArray();
            session.ReplyMgr(rates);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExchangeRate", "UpdateExchangeRate - update exchange rate", "更新汇率信息", QSEnumArgParseType.Json)]
        public void CTE_UpdateExchangeRate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (!manager.BaseManager.IsRoot())//
            {
                throw new FutsRspError("无权更新手续费模板");
            }

            ExchangeRate rate = Mixins.Json.JsonMapper.ToObject<ExchangeRate>(json);
            //更新汇率信息
            manager.Domain.UpdateExchangeRate(rate);

            //通知汇率更新
            session.NotifyMgr("NotifyExchangeRateUpdate",manager.Domain.GetExchangeRate(rate.ID));
            session.OperationSuccess("更新汇率成功");
        }



        /// <summary>
        /// 查询汇率信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnQryExchagneRate(MGRQryExchangeRateRequuest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求查询汇率信息:{1}", session.AuthorizedID, request.ToString()));
            IEnumerable<ExchangeRate> ratelist = manager.Domain.GetExchangeRates(TLCtxHelper.ModuleSettleCentre.Tradingday);

            for (int i = 0; i < ratelist.Count(); i++)
            {
                RspMGRQryExchangeRateResponse response = ResponseTemplate<RspMGRQryExchangeRateResponse>.SrvSendRspResponse(request);
                response.ExchangeRate = ratelist.ElementAt(i);

                CacheRspResponse(response, i == ratelist.Count() - 1);
            }
        }

    }
}
