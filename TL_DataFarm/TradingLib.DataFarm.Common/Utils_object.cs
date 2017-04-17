using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;


namespace TradingLib.DataFarm.Common
{
    public static class Utils_object
    {
        public static string ToJsonNotify(this Tick k)
        {
            XLPacketData pkt = new XLPacketData(XLMessageType.T_RTN_MARKETDATA);
            XLDepthMarketDataField data = new XLDepthMarketDataField();

            data.Date = k.Date;
            data.Time = k.Time;
            data.TradingDay = 0;

            data.SymbolID = k.Symbol;
            data.ExchangeID = k.Exchange;
            data.LastPrice = (double)k.Trade;
            data.PreSettlementPrice = (double)k.PreSettlement;
            data.PreClosePrice = (double)k.PreClose;
            data.PreOpenInterest = k.PreOpenInterest;
            data.OpenPrice = (double)k.Open;
            data.HighestPrice = (double)k.High;
            data.LowestPrice = (double)k.Low;
            data.Volume = k.Vol;
            data.OpenInterest = k.OpenInterest;
            data.ClosePrice = (double)k.Trade;
            data.SettlementPrice = (double)k.Settlement;
            data.UpperLimitPrice = (double)k.UpperLimit;
            data.LowerLimitPrice = (double)k.LowerLimit;
            data.BidPrice1 = (double)k.BidPrice;
            data.BidVolume1 = k.BidSize;
            data.AskPrice1 = (double)k.AskPrice;
            data.AskVolume1 = k.AskSize;
            pkt.AddField(data);

            return XLPacketData.PackJsonNotify(pkt);
        }
    }
}
