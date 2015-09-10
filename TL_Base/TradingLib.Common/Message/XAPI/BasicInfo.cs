using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 查询交易所信息
    /// </summary>
    public class XQryExchangeRequuest : RequestPacket
    {
        public XQryExchangeRequuest()
        {
            _type = MessageTypes.XQRYEXCHANGE;
        }

        public override string ContentSerialize()
        {
            return string.Empty;
        }

        public override void ContentDeserialize(string contentstr)
        {

        }
    }

    /// <summary>
    /// 查询交易所回报
    /// </summary>
    public class RspXQryExchangeResponse : RspResponsePacket
    {
        public RspXQryExchangeResponse()
        {
            _type = MessageTypes.XEXCHANGERESPNSE;
            Exchange = null;
        }

        public Exchange Exchange { get; set; }
        public override string ResponseSerialize()
        {
            if (this.Exchange == null) return string.Empty;
            return this.Exchange.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.Exchange = null;
                return;
            }
            this.Exchange = new Exchange();
            this.Exchange.Deserialize(content);
        }
    }


    public class XQryMarketTimeRequest : RequestPacket
    {
        public XQryMarketTimeRequest()
        {
            _type = MessageTypes.XQRYMARKETTIME;
        }
        public override string ContentSerialize()
        {
            return string.Empty;
        }

        public override void ContentDeserialize(string contentstr)
        {

        }

    }

    public class RspXQryMarketTimeResponse : RspResponsePacket
    {
        public RspXQryMarketTimeResponse()
        {
            _type = MessageTypes.XMARKETTIMERESPONSE;
            this.MarketTime = null;
        }

        public MarketTime MarketTime { get; set; }

        public override string ResponseSerialize()
        {
            if (this.MarketTime == null)
                return string.Empty;
            return this.MarketTime.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.MarketTime = null;
                return;
            }
            this.MarketTime = new MarketTime();
            this.MarketTime.Deserialize(content);
        }
    }

    /// <summary>
    /// 查询品种
    /// </summary>
    public class XQrySecurityRequest : RequestPacket
    {
        public XQrySecurityRequest()
        {
            _type = MessageTypes.XQRYSECURITY;
        }
        public override string ContentSerialize()
        {
            return string.Empty;
        }

        public override void ContentDeserialize(string contentstr)
        {

        }
    }

    /// <summary>
    /// 查询品种回报
    /// </summary>
    public class RspXQrySecurityResponse : RspResponsePacket
    {
        public RspXQrySecurityResponse()
        {
            _type = MessageTypes.XSECURITYRESPONSE;
            SecurityFaimly = null;
        }

        public SecurityFamilyImpl SecurityFaimly { get; set; }

        public override string ResponseSerialize()
        {
            if (this.SecurityFaimly == null)
                return string.Empty;
            return this.SecurityFaimly.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.SecurityFaimly = null;
                return;
            }
            this.SecurityFaimly = new SecurityFamilyImpl();
            this.SecurityFaimly.Deserialize(content);
        }
    }

    /// <summary>
    /// 查询合约信息
    /// </summary>
    public class XQrySymbolRequest : RequestPacket
    {
        public XQrySymbolRequest()
        {
            _type = MessageTypes.XQRYSYMBOL;
        }
        public override string ContentSerialize()
        {
            return string.Empty;
        }

        public override void ContentDeserialize(string contentstr)
        {

        }

    }

    /// <summary>
    /// 合约信息回报
    /// </summary>
    public class RspXQrySymbolResponse : RspResponsePacket
    {
        public RspXQrySymbolResponse()
        {
            _type = MessageTypes.XSYMBOLRESPONSE;
            this.Symbol = null;
        }

        public SymbolImpl Symbol { get; set; }

        public override string ResponseSerialize()
        {
            if (this.Symbol == null)
                return string.Empty;
            return this.Symbol.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.Symbol = null;
                return;
            }
            this.Symbol = new SymbolImpl();
            this.Symbol.Deserialize(content);
        }
    }

}
