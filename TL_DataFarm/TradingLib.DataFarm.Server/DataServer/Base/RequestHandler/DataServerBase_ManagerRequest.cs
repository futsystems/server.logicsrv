using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public partial class DataServerBase
    {

        //查询日历列表
        [DataCommandAttr("QryCalendarList", "QryCalendarList -  qry calendar list", "查询日历对象列表")]
        public void CTE_QryCalendarList(IServiceHost host, IConnection conn)
        {
            CalendarItem[] items = MDBasicTracker.CalendarTracker.Calendars.Select(c => new CalendarItem() { Code = c.Code, Name = c.Name }).ToArray();
            int totalnum = items.Length;

            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    conn.SendContribResponse(items[i], i == totalnum - 1);
                }
            }
            else
            {
                conn.SendContribResponse(null);
            }
            
        }

        /// <summary>
        /// 更新合约
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void SrvOnMGRUpdateSymbol(IServiceHost host, IConnection conn, MGRUpdateSymbolRequest request)
        {
            logger.Info(string.Format("Conn:{0} 请求查询合约:{1}", conn.SessionID, request.ToString()));

            SymbolImpl symbol = request.Symbol;
            symbol.Domain_ID = 1;

            //检查品种
            SecurityFamilyImpl sec = MDBasicTracker.SecurityTracker[symbol.security_fk];
            if (sec == null)
            {
                throw new FutsRspError("品种数据异常");
            }
            symbol.SecurityFamily = sec;

            //如果是添加合约 检查合约是否存在
            if (symbol.ID == 0 && MDBasicTracker.SymbolTracker[symbol.Exchange, symbol.Symbol] != null)
            {
                throw new FutsRspError("已经存在合约:" + symbol.Symbol);
            }

            //调用该域更新该合约
            MDBasicTracker.SymbolTracker.UpdateSymbol(symbol);

            RspMGRUpdateSymbolResponse response = ResponseTemplate<RspMGRUpdateSymbolResponse>.SrvSendRspResponse(request);
            SymbolImpl localsymbol = MDBasicTracker.SymbolTracker[symbol.ID];
            response.Symbol = localsymbol;

            conn.SendResponse(response);
        }

        /// <summary>
        /// 更新品种
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void SrvOnMGRUpdateSecurity(IServiceHost host, IConnection conn, MGRUpdateSecurityRequest request)
        {
            logger.Info(string.Format("Conn:{0} 请求更新品种:{1}",conn.SessionID, request.ToString()));

            SecurityFamilyImpl sec = request.SecurityFaimly;
            //如果已经存在该品种则不执行添加操作
            if (sec.ID == 0 && MDBasicTracker.SecurityTracker[sec.Code] != null)
            {
                throw new FutsRspError("已经存在品种:" + sec.Code);
            }
            //通过对应的域更新品种对象
            MDBasicTracker.SecurityTracker.UpdateSecurity(sec);
            int secidupdate = sec.ID;

               
            //需要通过第一次更新获得sec_id来获得对象进行回报 否则在更新其他域的品种对象时id会发生同步变化
            RspMGRUpdateSecurityResponse response = ResponseTemplate<RspMGRUpdateSecurityResponse>.SrvSendRspResponse(request);
            response.SecurityFaimly = MDBasicTracker.SecurityTracker[sec.ID];

            conn.SendResponse(response);

        }

        /// <summary>
        /// 更新交易所
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void SrvOnMGRUpdateExchange(IServiceHost host, IConnection conn, MGRUpdateExchangeRequest request)
        {
            logger.Info(string.Format("Conn:{0} 请求更新交易所信息:{1}", conn.SessionID, request.ToString()));

            if (request.Exchange != null)
            {
                MDBasicTracker.ExchagneTracker.UpdateExchange(request.Exchange);
               
                RspMGRUpdateExchangeResponse response = ResponseTemplate<RspMGRUpdateExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = MDBasicTracker.ExchagneTracker[request.Exchange.ID];
                conn.SendResponse(response);
            }
            
        }

        /// <summary>
        /// 更新交易小节
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRUpdateMarketTime(IServiceHost host, IConnection conn, MGRUpdateMarketTimeRequest request)
        {
            logger.Info(string.Format("Conn:{0} 请求更新交易时间段:{1}", conn.SessionID, request.ToString()));
            if (request.MarketTime != null)
            {
                MDBasicTracker.MarketTimeTracker.UpdateMarketTime(request.MarketTime);
                RspMGRUpdateMarketTimeResponse response = ResponseTemplate<RspMGRUpdateMarketTimeResponse>.SrvSendRspResponse(request);
                response.MarketTime = MDBasicTracker.MarketTimeTracker[request.MarketTime.ID];

                conn.SendResponse(response);
            }
        }



    }
}
