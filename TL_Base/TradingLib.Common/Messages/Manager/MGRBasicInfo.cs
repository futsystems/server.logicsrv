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
    public class MGRQryExchangeRequuest : RequestPacket
    {
        public MGRQryExchangeRequuest()
        {
            _type = MessageTypes.MGRQRYEXCHANGE;
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
    public class RspMGRQryExchangeResponse : RspResponsePacket
    {
        public RspMGRQryExchangeResponse()
        {
            _type = MessageTypes.MGREXCHANGERESPONSE;
            Exchange = new Exchange();
        }

        public Exchange Exchange { get; set; }
        public override string ResponseSerialize()
        {
            return this.Exchange.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.Exchange.Deserialize(content);
        }
    }


    public class MGRQryMarketTimeRequest : RequestPacket
    {
        public MGRQryMarketTimeRequest()
        {
            _type = MessageTypes.MGRQRYMARKETTIME;
        }
        public override string ContentSerialize()
        {
            return string.Empty;
        }

        public override void ContentDeserialize(string contentstr)
        {

        }

    }

    public class RspMGRQryMarketTimeResponse : RspResponsePacket
    {
        public RspMGRQryMarketTimeResponse()
        {
            _type = MessageTypes.MGRMARKETTIMERESPONSE;
            this.MarketTime = new MarketTime();
        }

        public MarketTime MarketTime { get; set; }

        public override string ResponseSerialize()
        {
            return this.MarketTime.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.MarketTime.Deserialize(content);
        }
    }

    /// <summary>
    /// 查询品种
    /// </summary>
    public class MGRQrySecurityRequest : RequestPacket
    {
        public MGRQrySecurityRequest()
        {
            _type = MessageTypes.MGRQRYSECURITY;
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
    public class RspMGRQrySecurityResponse : RspResponsePacket
    {
        public RspMGRQrySecurityResponse()
        {
            _type = MessageTypes.MGRSECURITYRESPONSE;
            SecurityFaimly = new SecurityFamilyImpl();
        }

        public SecurityFamilyImpl SecurityFaimly { get; set; }

        public override string ResponseSerialize()
        {
            return this.SecurityFaimly.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.SecurityFaimly.Deserialize(content);
        }
    }

    /// <summary>
    /// 添加品种请求
    /// </summary>
    public class MGRReqAddSecurityRequest : RequestPacket
    {
        public MGRReqAddSecurityRequest()
        {
            _type = MessageTypes.MGRADDSECURITY;
            SecurityFaimly = new SecurityFamilyImpl();
        }

        public SecurityFamilyImpl SecurityFaimly { get; set; }

        public override string ContentSerialize()
        {
            return this.SecurityFaimly.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            int i = 0;
            this.SecurityFaimly.Deserialize(contentstr);
        }
    
    }

    /// <summary>
    /// 添加品种回报
    /// </summary>
    public class RspMGRReqAddSecurityResponse : RspResponsePacket
    {
        public RspMGRReqAddSecurityResponse()
        {
            _type = MessageTypes.MGRADDSECURITYRESPONSE;
            SecurityFaimly = new SecurityFamilyImpl();
        }
        public SecurityFamilyImpl SecurityFaimly { get; set; }

        public override string ResponseSerialize()
        {
            return this.SecurityFaimly.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.SecurityFaimly.Deserialize(content);
        }
    }
    /// <summary>
    /// 更新品种信息
    /// </summary>
    public class MGRUpdateSecurityRequest : RequestPacket
    {
        public MGRUpdateSecurityRequest()
        {
            _type = MessageTypes.MGRUPDATESECURITY;
            SecurityFaimly = new SecurityFamilyImpl();
        }

        public SecurityFamilyImpl SecurityFaimly { get; set; }

        public override string ContentSerialize()
        {
            return SecurityFaimly.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            SecurityFaimly.Deserialize(contentstr);
        }
    }


    /// <summary>
    /// 查询合约信息
    /// </summary>
    public class MGRQrySymbolRequest : RequestPacket
    {
        public MGRQrySymbolRequest()
        {
            _type = MessageTypes.MGRQRYSYMBOL;
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
    public class RspMGRQrySymbolResponse : RspResponsePacket
    {
        public RspMGRQrySymbolResponse()
        {
            _type = MessageTypes.MGRSYMBOLRESPONSE;
            this.Symbol = new SymbolImpl();
        }

        public SymbolImpl Symbol { get; set; }

        public override string ResponseSerialize()
        {
            return this.Symbol.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.Symbol.Deserialize(content);
        }
    }

    /// <summary>
    /// 请求添加合约
    /// </summary>
    public class MGRReqAddSymbolRequest : RequestPacket
    {
        public MGRReqAddSymbolRequest()
        {
            _type = MessageTypes.MGRADDSYMBOL;
            this.Symbol = new SymbolImpl();
        }
        public SymbolImpl Symbol { get; set; }

        public override string ContentSerialize()
        {
            return this.Symbol.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.Symbol.Deserialize(contentstr);
        }
    }

    /// <summary>
    /// 添加合约回报
    /// </summary>
    public class RspMGRReqAddSymbolResponse : RspResponsePacket
    {
        public RspMGRReqAddSymbolResponse()
        {
            _type = MessageTypes.MGRADDSYMBOLRESPONSE;
            this.Symbol = new SymbolImpl();
        }
        public SymbolImpl Symbol { get; set; }
        public override string ResponseSerialize()
        {
            return this.Symbol.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.Symbol.Deserialize(content);
        }
    }
    /// <summary>
    /// 更新合约信息
    /// </summary>
    public class MGRUpdateSymbolRequest : RequestPacket
    {
        public MGRUpdateSymbolRequest()
        {
            _type = MessageTypes.MGRUPDATESYMBOL;
            this.Symbol = new SymbolImpl();
        }

        public SymbolImpl Symbol { get; set; }

        public override string ContentSerialize()
        {
            return this.Symbol.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.Symbol.Deserialize(contentstr);
        }
    }
}
