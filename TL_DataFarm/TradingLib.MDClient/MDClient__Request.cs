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
        /// 订阅合约实时行情
        /// </summary>
        public void RegisterSymbol(string[] symbols)
        {
            logger.Info(string.Format("Subscribe market data for symbol:{0}", string.Join(",", symbols)));
            RegisterSymbolTickRequest request = RequestTemplate<RegisterSymbolTickRequest>.CliSendRequest(NextRequestID);
            foreach (var symbol in symbols)
            {
                Symbol sym = this.GetSymbol(symbol);
                if (sym == null)
                {
                    logger.Warn(string.Format("Symbol:{0} do not exist", symbol));
                    continue;
                }
                request.SymbolList.Add(symbol);
            }
            histClient.TLSend(request);
        }

        /// <summary>
        /// 注销合约实时行情
        /// </summary>
        /// <param name="symbol"></param>
        public void UnRegisterSymbol(string[] symbols)
        {
            logger.Info(string.Format("Unsubscribe market data for symbol:{0}", string.Join(",",symbols)));
            UnregisterSymbolTickRequest request = RequestTemplate<UnregisterSymbolTickRequest>.CliSendRequest(NextRequestID);
                
            foreach (var symbol in symbols)
            {
                Symbol sym = this.GetSymbol(symbol);
                if (sym == null)
                {
                    logger.Warn(string.Format("Symbol:{0} do not exist", symbol));
                    continue;
                }
                request.SymbolList.Add(symbol);
            }
            histClient.TLSend(request);
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
        public int QryBar(string symbol,int interval,DateTime start,DateTime end,int maxcount=1000,bool fromend = true)
        {
            int reqid = NextRequestID;
            QryBarRequest request = RequestTemplate<QryBarRequest>.CliSendRequest(reqid);
            request.FromEnd = fromend;
            request.Symbol = symbol;
            request.MaxCount = maxcount;
            request.Interval = interval;
            request.Start = start;
            request.End = end;
            request.BarResponseType = EnumBarResponseType.BINARY;

            histClient.TLSend(request);
            return reqid;
        }
    }
}
