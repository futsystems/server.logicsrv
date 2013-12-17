using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        /// <summary>
        /// 查询管理员
        /// </summary>
        public void ReqQryManager()
        {
            this.ReqContribRequest("MgrExchServer", "QryManager",""); 
        }

        #region 基础数据维护

        public void ReqUpdateRecvBank(JsonWrapperReceivableAccount bank)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateReceiveableBank",TradingLib.Mixins.LitJson.JsonMapper.ToJson(bank)); 
        }
        /// <summary>
        /// 请求同步品种
        /// </summary>
        public void ReqSyncSecurity()
        {
            this.ReqContribRequest("MgrExchServer", "SyncSecInfo", "");
        }

        /// <summary>
        /// 请求同步合约数据
        /// </summary>
        public void ReqSyncSymbol()
        {
            this.ReqContribRequest("MgrExchServer", "SyncSymbol", "");
        }


        public void ReqQryExchange()
        {
            debug("请求查询交易所列表", QSEnumDebugLevel.INFO);
            MGRQryExchangeRequuest request = RequestTemplate<MGRQryExchangeRequuest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        public void ReqQryMarketTime()
        {
            debug("请求查询市场时间列表", QSEnumDebugLevel.INFO);
            MGRQryMarketTimeRequest request = RequestTemplate<MGRQryMarketTimeRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }
        public void ReqQrySecurity()
        {
            debug("请求查询品种列表", QSEnumDebugLevel.INFO);
            MGRQrySecurityRequest request = RequestTemplate<MGRQrySecurityRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }
        public void ReqUpdateSecurity(SecurityFamilyImpl sec)
        {
            debug("请求更新品种信息", QSEnumDebugLevel.INFO);
            MGRUpdateSecurityRequest request = RequestTemplate<MGRUpdateSecurityRequest>.CliSendRequest(requestid++);
            request.SecurityFaimly = sec;

            SendPacket(request);
        }

        public void ReqAddSecurity(SecurityFamilyImpl sec)
        {
            debug("请求添加品种信息", QSEnumDebugLevel.INFO);
            MGRReqAddSecurityRequest request = RequestTemplate<MGRReqAddSecurityRequest>.CliSendRequest(requestid++);
            request.SecurityFaimly = sec;

            SendPacket(request);
        }

        public void ReqQrySymbol()
        {
            debug("请求查询合约列表", QSEnumDebugLevel.INFO);
            MGRQrySymbolRequest request = RequestTemplate<MGRQrySymbolRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        public void ReqUpdateSymbol(SymbolImpl sym)
        {
            debug("请求更新合约", QSEnumDebugLevel.INFO);
            MGRUpdateSymbolRequest request = RequestTemplate<MGRUpdateSymbolRequest>.CliSendRequest(requestid++);
            request.Symbol = sym;

            SendPacket(request);
        }

        public void ReqAddSymbol(SymbolImpl sym)
        {
            debug("请求添加合约: expiredate:"+sym.ExpireDate.ToString(), QSEnumDebugLevel.INFO);
            MGRReqAddSymbolRequest request = RequestTemplate<MGRReqAddSymbolRequest>.CliSendRequest(requestid++);
            request.Symbol = sym;

            SendPacket(request);
        }
        #endregion
    }
}
