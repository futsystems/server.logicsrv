//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using TradingLib.API;
//using TradingLib.Common;

//namespace FutsMoniter
//{
//    partial class MainForm
//    {








//        }

//        #endregion















//        #region 风控类接口实现


//        /// <summary>
//        /// 帐户风控规则项目回报
//        /// </summary>
//        /// <param name="item"></param>
//        /// <param name="islast"></param>
//        public void OnMGRRuleItemResponse(RuleItem item, bool islast)
//        {
//            ctAccountMontier1.GotRuleItem(item, islast);
//        }

//        public void OnMGRRuleItemUpdate(RuleItem item, bool islast)
//        {
//            ctAccountMontier1.GotRuleItem(item, islast);
//        }

//        public void OnMGRRulteItemDelete(RuleItem item, bool islast)
//        {

//            ctAccountMontier1.GotRuleItemDel(item, islast);
//        }

//        #endregion



//        #region 查询历史记录
//        public void OnMGROrderResponse(Order o, bool islast)
//        {
//            histqryform.GotHistOrder(o, islast);
//        }
//        public void OnMGRTradeResponse(Trade f, bool islast)
//        {
//            histqryform.GotHistTrade(f, islast);

//        }

//        public void OnMGRPositionResponse(PositionDetail pos, bool islast)
//        {
//            histqryform.GotHistPosition(pos, islast);
//        }

//        public void OnMGRCashTransactionResponse(CashTransaction c, bool islast)
//        {
//            //histqryform.GotHistCashTransaction(c, islast);
//        }
//        public void OnMGRSettlementResponse(RspMGRQrySettleResponse response)
//        {
//            //Globals.Debug("mainform:" + response.SettlementContent);
//            settlementform.GotSettlement(response);
//        }
//        #endregion




//    }
//}
