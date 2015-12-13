using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.MDClient
{

    public partial class MDClient
    {

        /// <summary>
        /// 查询交易时间段
        /// </summary>
        private void QryMarketTime()
        {
            XQryMarketTimeRequest request = RequestTemplate<XQryMarketTimeRequest>.CliSendRequest(0);
            histClient.TLSend(request);
        }

        /// <summary>
        /// 查询交易所
        /// </summary>
        private void QryExchange()
        { 
            XQryExchangeRequuest request = RequestTemplate<XQryExchangeRequuest>.CliSendRequest(0);
            histClient.TLSend(request);
        }

        /// <summary>
        /// 查询品种数据
        /// </summary>
        private void QrySecurity()
        {
            XQrySecurityRequest request = RequestTemplate<XQrySecurityRequest>.CliSendRequest(0);
            histClient.TLSend(request);
        }

        /// <summary>
        /// 查询合约数据
        /// </summary>
        private void QrySymbol()
        {
            XQrySymbolRequest request = RequestTemplate<XQrySymbolRequest>.CliSendRequest(0);
            histClient.TLSend(request);
        }

        /// <summary>
        /// 请求登入
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pass"></param>
        public void Login(string username, string pass)
        { 
            
        }

        /// <summary>
        /// 注册实时行情
        /// </summary>
        public void Register(string symbol)
        {
            MDRegisterSymbolsRequest request = RequestTemplate<MDRegisterSymbolsRequest>.CliSendRequest(NextRequestID);
        }

        /// <summary>
        /// 底层查询Bar数据接口
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxcount"></param>
        /// <param name="fromend"></param>
        public void QryBar(string symbol,int interval,DateTime start,DateTime end,int maxcount=1000,bool fromend = true)
        {
            QryBarRequest request = RequestTemplate<QryBarRequest>.CliSendRequest(0);
            request.FromEnd = fromend;
            request.Symbol = symbol;
            request.MaxCount = maxcount;
            request.Interval = interval;
            request.Start = start;
            request.End = end;

            histClient.TLSend(request);
        }

        /// <summary>
        /// 返回一定数目的Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="maxcount"></param>
        public void QryBar(string symbol, int interval, int maxcount = 1000)
        {
            QryBar(symbol, interval, DateTime.MinValue, DateTime.MaxValue, maxcount, true);
        }

        /// <summary>
        /// 查询某个时间段的Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void QryBar(string symbol, int interval, DateTime start, DateTime end)
        {
            QryBar(symbol, interval, start, end, 0, true);
        }
    }
}
