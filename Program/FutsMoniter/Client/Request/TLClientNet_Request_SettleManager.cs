using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        /// <summary>
        /// 删除某个交易日的结算信息
        /// </summary>
        /// <param name="bank"></param>
        public void ReqDeleteSettleInfo(int settleday)
        {
            this.ReqContribRequest("SettleCentre", "DeleteSettleInfo", TradingLib.Mixins.Json.JsonMapper.ToJson(new { settleday = settleday }));
        }

        /// <summary>
        /// 重新对某个交易日进行结算操作
        /// </summary>
        /// <param name="settleday"></param>
        public void ReqReSettle(int settleday)
        {
            this.ReqContribRequest("SettleCentre", "ReSettle", TradingLib.Mixins.Json.JsonMapper.ToJson(new { settleday = settleday }));
        }

        /// <summary>
        /// 重新加载某个日期的交易数据
        /// </summary>
        /// <param name="settleday"></param>
        public void ReqRollBackToDay(int currentday)
        {
            this.ReqContribRequest("SettleCentre", "RollBackToDay", TradingLib.Mixins.Json.JsonMapper.ToJson(new { currentday = currentday }));
        }

        /// <summary>
        /// 查询某个交易日的结算价格信息
        /// </summary>
        /// <param name="currentday"></param>
        public void ReqQrySettlementPrice(int settleday)
        {
            this.ReqContribRequest("SettleCentre", "QrySettlementPrice", TradingLib.Mixins.Json.JsonMapper.ToJson(new { settleday = settleday }));
        }


        /// <summary>
        /// 查询结算状态
        /// </summary>
        public void ReqQrySettleStatus()
        {
            this.ReqContribRequest("SettleCentre", "QrySettleStatus", "");
        }

        /// <summary>
        /// 查询当前所有持仓数据
        /// </summary>
        public void ReqQryPositionHold()
        {
            this.ReqContribRequest("SettleCentre", "QryPositionHold", "");
        }

        /// <summary>
        /// 更新结算价格
        /// </summary>
        /// <param name="price"></param>
        public void ReqUpdateSettlementPrice(SettlementPrice price)
        {
            this.ReqContribRequest("SettleCentre", "UpdateSettlementPrice", TradingLib.Mixins.Json.JsonMapper.ToJson(price));
        }


        /// <summary>
        /// 手工平掉某个持仓
        /// </summary>
        /// <param name="data"></param>
        public void ReqFlatPositionHold(object data)
        {
            this.ReqContribRequest("SettleCentre", "FlatPositionHold", TradingLib.Mixins.Json.JsonMapper.ToJson(data));
        }

    }
}
