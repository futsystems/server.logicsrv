using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        public void ReqQryHistOrders(string account, int settleday)
        {
            MGRQryOrderRequest request = RequestTemplate<MGRQryOrderRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);

        }

        public void ReqQryHistTrades(string account, int settleday)
        {
            MGRQryTradeRequest request = RequestTemplate<MGRQryTradeRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);
        }

        public void ReqQryHistPosition(string account, int settleday)
        {
            MGRQryPositionRequest request = RequestTemplate<MGRQryPositionRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);
        }

        public void ReqQryHistCashTransaction(string account, int settleday)
        {
            MGRQryCashRequest request = RequestTemplate<MGRQryCashRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);
        }

        public void ReqQryHistSettlement(string account, int settleday)
        {
            MGRQrySettleRequest request = RequestTemplate<MGRQrySettleRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);

        }
      
    }
}
