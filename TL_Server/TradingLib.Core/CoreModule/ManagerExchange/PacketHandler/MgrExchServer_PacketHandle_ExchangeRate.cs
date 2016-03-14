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
            //if (!manager.BaseManager.IsRoot())//
            //{
            //    throw new FutsRspError("无权查询手续费模板");
            //}

            ExchangeRate[] rates = BasicTracker.ExchangeRateTracker.GetExchangeRates(TLCtxHelper.ModuleSettleCentre.Tradingday).ToArray();
            //for (int i = 0; i < rates.Length; i++)
            //{
            //    session.ReplyMgr(rates[i], i == rates.Length - 1);
            //}
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
            BasicTracker.ExchangeRateTracker.UpdateExchangeRate(rate);

            //通知汇率更新
            session.NotifyMgr("NotifyExchangeRateUpdate", BasicTracker.ExchangeRateTracker[rate.ID]);
        }


    }
}
