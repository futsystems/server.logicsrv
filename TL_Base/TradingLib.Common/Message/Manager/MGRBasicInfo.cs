using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    ///// <summary>
    ///// 查询交易所信息
    ///// </summary>
    //public class MGRQryExchangeRequuest : RequestPacket
    //{
    //    public MGRQryExchangeRequuest()
    //    {
    //        _type = MessageTypes.MGRQRYEXCHANGE;
    //    }

    //    public override string ContentSerialize()
    //    {
    //        return string.Empty;
    //    }

    //    public override void ContentDeserialize(string contentstr)
    //    {
            
    //    }
    //}

    ///// <summary>
    ///// 更新交易所信息
    ///// </summary>
    //public class MGRUpdateExchangeRequest : RequestPacket
    //{
    //    public MGRUpdateExchangeRequest()
    //    {
    //        _type = MessageTypes.MGRUPDATEEXCHANGE;
    //        this.Exchange = null;
    //    }

    //    public ExchangeImpl Exchange { get; set; }

    //    public override string ContentSerialize()
    //    {
    //        if (this.Exchange == null)
    //            return string.Empty;
    //        return TradingLib.Common.ExchangeImpl.Serialize(this.Exchange);
    //    }

    //    public override void ContentDeserialize(string content)
    //    {
    //        if (string.IsNullOrEmpty(content))
    //        {
    //            this.Exchange = null;
    //            return;
    //        }
    //        this.Exchange = TradingLib.Common.ExchangeImpl.Deserialize(content);
    //    }
    //}

    //public class RspMGRUpdateExchangeResponse : RspResponsePacket
    //{

    //    public RspMGRUpdateExchangeResponse()
    //    {
    //        _type = MessageTypes.MGRUPDATEEXCHANGERESPONSE;
    //        this.Exchange = null;
    //    }

    //    public ExchangeImpl Exchange { get; set; }

    //    public override string ResponseSerialize()
    //    {
    //        if (this.Exchange == null)
    //            return string.Empty;
    //        return TradingLib.Common.ExchangeImpl.Serialize(this.Exchange);
    //    }

    //    public override void ResponseDeserialize(string content)
    //    {
    //        if (string.IsNullOrEmpty(content))
    //        {
    //            this.Exchange = null;
    //            return;
    //        }
    //        this.Exchange = TradingLib.Common.ExchangeImpl.Deserialize(content);
    //    }
    //}



    ///// <summary>
    ///// 查询交易所回报
    ///// </summary>
    //public class RspMGRQryExchangeResponse : RspResponsePacket
    //{
    //    public RspMGRQryExchangeResponse()
    //    {
    //        _type = MessageTypes.MGREXCHANGERESPONSE;
    //        Exchange = null;
    //    }

    //    public ExchangeImpl Exchange { get; set; }
    //    public override string ResponseSerialize()
    //    {
    //        if (this.Exchange == null) return string.Empty;
    //        return TradingLib.Common.ExchangeImpl.Serialize(this.Exchange);
    //    }

    //    public override void ResponseDeserialize(string content)
    //    {
    //        if (string.IsNullOrEmpty(content))
    //        {
    //            this.Exchange = null;
    //            return;
    //        }
    //        this.Exchange = TradingLib.Common.ExchangeImpl.Deserialize(content);
    //    }
    //}


    //public class MGRQryMarketTimeRequest : RequestPacket
    //{
    //    public MGRQryMarketTimeRequest()
    //    {
    //        _type = MessageTypes.MGRQRYMARKETTIME;
    //    }
    //    public override string ContentSerialize()
    //    {
    //        return string.Empty;
    //    }

    //    public override void ContentDeserialize(string contentstr)
    //    {

    //    }

    //}

    //public class RspMGRQryMarketTimeResponse : RspResponsePacket
    //{
    //    public RspMGRQryMarketTimeResponse()
    //    {
    //        _type = MessageTypes.MGRMARKETTIMERESPONSE;
    //        this.MarketTime = null;
    //    }

    //    public MarketTimeImpl MarketTime { get; set; }

    //    public override string ResponseSerialize()
    //    {
    //        if (this.MarketTime == null)
    //            return string.Empty;
    //        return TradingLib.Common.MarketTimeImpl.Serialize(this.MarketTime);
    //    }

    //    public override void ResponseDeserialize(string content)
    //    {
    //        if (string.IsNullOrEmpty(content))
    //        {
    //            this.MarketTime = null;
    //            return;
    //        }
    //        this.MarketTime = TradingLib.Common.MarketTimeImpl.Deserialize(content);
    //    }
    //}


    //public class MGRUpdateMarketTimeRequest : RequestPacket
    //{
    //    public MGRUpdateMarketTimeRequest()
    //    {
    //        _type = MessageTypes.MGRUPDATEMARKETTIME;
    //        this.MarketTime = null;
    //    }

    //    public MarketTimeImpl MarketTime { get; set; }

    //    public override string ContentSerialize()
    //    {
    //        if (this.MarketTime == null)
    //            return string.Empty;
    //        return TradingLib.Common.MarketTimeImpl.Serialize(this.MarketTime);
    //    }


    //    public override void ContentDeserialize(string contentstr)
    //    {
    //        if (string.IsNullOrEmpty(contentstr))
    //        {
    //            this.MarketTime = null;
    //            return;
    //        }
    //        this.MarketTime = TradingLib.Common.MarketTimeImpl.Deserialize(contentstr);
    //    }

    //}

    //public class RspMGRUpdateMarketTimeResponse : RspResponsePacket
    //{
    //    public RspMGRUpdateMarketTimeResponse()
    //    {
    //        _type = MessageTypes.MGRUPDATEMARKETTIMERESPONSE;
    //        this.MarketTime = null;
    //    }

    //    public MarketTimeImpl MarketTime { get; set; }

    //    public override string ResponseSerialize()
    //    {
    //        if (this.MarketTime == null)
    //            return string.Empty;
    //        return TradingLib.Common.MarketTimeImpl.Serialize(this.MarketTime);
    //    }


    //    public override void ResponseDeserialize(string contentstr)
    //    {
    //        if (string.IsNullOrEmpty(contentstr))
    //        {
    //            this.MarketTime = null;
    //            return;
    //        }
    //        this.MarketTime = TradingLib.Common.MarketTimeImpl.Deserialize(contentstr);
    //    }
    //}
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
    /// 更新品种信息
    /// </summary>
    public class MGRUpdateSecurityRequest : RequestPacket
    {
        public MGRUpdateSecurityRequest()
        {
            _type = MessageTypes.MGRUPDATESECURITY;
            SecurityFaimly = null;
        }

        public SecurityFamilyImpl SecurityFaimly { get; set; }

        public override string ContentSerialize()
        {
            if (this.SecurityFaimly == null)
                return string.Empty;
            return SecurityFaimly.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            if (string.IsNullOrEmpty(contentstr))
            {
                this.SecurityFaimly = null;
                return;
            }
            this.SecurityFaimly = new SecurityFamilyImpl();
            SecurityFaimly.Deserialize(contentstr);
        }
    }

    /// <summary>
    /// 更新品种回报
    /// </summary>
    public class RspMGRUpdateSecurityResponse : RspResponsePacket
    {
        public RspMGRUpdateSecurityResponse()
        {
            _type = MessageTypes.MGRUPDATESECURITYRESPONSE;
            SecurityFaimly = null;
        }
        public SecurityFamilyImpl SecurityFaimly { get; set; }

        public override string ResponseSerialize()
        {
            if (SecurityFaimly == null)
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

    ///// <summary>
    ///// 请求添加合约
    ///// </summary>
    //public class MGRReqAddSymbolRequest : RequestPacket
    //{
    //    public MGRReqAddSymbolRequest()
    //    {
    //        _type = MessageTypes.MGRADDSYMBOL;
    //        this.Symbol = null;
    //    }
    //    public SymbolImpl Symbol { get; set; }

    //    public override string ContentSerialize()
    //    {
    //        if (this.Symbol == null)
    //            return string.Empty;
    //        return this.Symbol.Serialize();
    //    }

    //    public override void ContentDeserialize(string contentstr)
    //    {
    //        if (string.IsNullOrEmpty(contentstr))
    //        {
    //            this.Symbol = null;
    //            return;
    //        }
    //        this.Symbol = new SymbolImpl();
    //        this.Symbol.Deserialize(contentstr);
    //    }
    //}

    ///// <summary>
    ///// 添加合约回报
    ///// </summary>
    //public class RspMGRReqAddSymbolResponse : RspResponsePacket
    //{
    //    public RspMGRReqAddSymbolResponse()
    //    {
    //        _type = MessageTypes.MGRADDSYMBOLRESPONSE;
    //        this.Symbol = null;
    //    }
    //    public SymbolImpl Symbol { get; set; }
    //    public override string ResponseSerialize()
    //    {
    //        if (this.Symbol == null)
    //            return string.Empty;
    //        return this.Symbol.Serialize();
    //    }

    //    public override void ResponseDeserialize(string content)
    //    {
    //        if (string.IsNullOrEmpty(content))
    //        {
    //            this.Symbol = null;
    //            return;
    //        }
    //        this.Symbol = new SymbolImpl();
    //        this.Symbol.Deserialize(content);
    //    }
    //}

    /// <summary>
    /// 更新合约信息
    /// </summary>
    public class MGRUpdateSymbolRequest : RequestPacket
    {
        public MGRUpdateSymbolRequest()
        {
            _type = MessageTypes.MGRUPDATESYMBOL;
            this.Symbol = null;
        }

        public SymbolImpl Symbol { get; set; }

        public override string ContentSerialize()
        {
            if (this.Symbol == null)
                return string.Empty;
            return this.Symbol.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            if (string.IsNullOrEmpty(contentstr))
            {
                this.Symbol = null;
                return;
            }
            this.Symbol = new SymbolImpl();
            this.Symbol.Deserialize(contentstr);
        }
    }


    public class RspMGRUpdateSymbolResponse : RspResponsePacket
    {
        public RspMGRUpdateSymbolResponse()
        {
            _type = MessageTypes.MGRUPDATESYMBOLRESPONSE;
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

    /// <summary>
    /// 查询行情快照请求
    /// </summary>
    public class MGRQryTickSnapShotRequest : RequestPacket
    {
        public MGRQryTickSnapShotRequest()
        {
            _type = MessageTypes.MGRQRYTICKSNAPSHOT;

            this.Exchange = string.Empty;
            this.Symbol = string.Empty;
        }

        public string Exchange { get; set; }
        public string Symbol { get; set; }

        public override string ContentSerialize()
        {
            return string.Format("{0},{1}", this.Exchange, this.Symbol);
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            if (rec.Length == 2)
            {
                this.Exchange = rec[0];
                this.Symbol = rec[1];
            }
        }

    }

    /// <summary>
    /// 行情快照回报
    /// </summary>
    public class RspMGRQryTickSnapShotResponse : RspResponsePacket
    {
        public RspMGRQryTickSnapShotResponse()
        {
            _type = MessageTypes.MGRQRYTICKSNAPSHOTRESPONSE;
            this.Tick = null;
        }

        public Tick Tick { get; set; }

        public override string ResponseSerialize()
        {
            if (this.Tick == null)
                return string.Empty;
            return TickImpl.Serialize2(this.Tick);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.Tick = null;
                return;
            }
            this.Tick = TickImpl.Deserialize2(content);
        }
    }

}
