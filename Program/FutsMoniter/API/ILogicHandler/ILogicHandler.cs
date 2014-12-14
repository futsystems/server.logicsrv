using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    /// <summary>
    /// 用于植入TLClientNet 响应客户端的回报
    /// </summary>
    public partial interface ILogicHandler
    {


 


        /// <summary>
        /// 响应服务端的查询通道列表回报
        /// </summary>
        /// <param name="response"></param>
        //void OnMGRConnectorResponse(ConnectorInfo c, bool islast);








        #region 系统状态与通知
        /// <summary>
        /// 查询系统状态回报
        /// </summary>
        /// <param name="status"></param>
        /// <param name="islast"></param>
        //void OnMGRSytstemStatus(SystemStatus status, bool islast);
        #endregion


        #region 历史记录查询
        void OnMGROrderResponse(Order o, bool islast);
        void OnMGRTradeResponse(Trade f, bool islast);
        //void OnMGRPositionResponse(PositionDetail pos, bool islast);
        //void OnMGRCashTransactionResponse(CashTransaction c, bool islast);
        void OnMGRSettlementResponse(RspMGRQrySettleResponse response);
        #endregion


        void PopRspInfo(RspInfo info);

    }
}
