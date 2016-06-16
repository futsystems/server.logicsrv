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
            return TradingLib.Common.Exchange.Serialize(this.Exchange);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.Exchange = null;
                return;
            }
            this.Exchange = TradingLib.Common.Exchange.Deserialize(content);
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
            return TradingLib.Common.MarketTime.Serialize(this.MarketTime);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.MarketTime = null;
                return;
            }
            this.MarketTime = TradingLib.Common.MarketTime.Deserialize(content);
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
            this.Symbol = string.Empty;
        }

        public string Symbol { get; set; }
        public override string ContentSerialize()
        {
            return this.Symbol;
        }

        public override void ContentDeserialize(string contentstr)
        {
            if (string.IsNullOrEmpty(contentstr))
            {
                this.Symbol = string.Empty;
            }
            else
            {
                this.Symbol = contentstr;
            }
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

    /// <summary>
    /// 查询行情快照请求
    /// </summary>
    public class XQryTickSnapShotRequest : RequestPacket
    {
        public XQryTickSnapShotRequest()
        {
            _type = MessageTypes.XQRYTICKSNAPSHOT;
            this.Symbol = string.Empty;
        }

        public string Symbol { get; set; }
        public override string ContentSerialize()
        {
            return this.Symbol;
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.Symbol = contentstr;
        }

    }

    /// <summary>
    /// 行情快照回报
    /// </summary>
    public class RspXQryTickSnapShotResponse : RspResponsePacket
    {
        public RspXQryTickSnapShotResponse()
        {
            _type = MessageTypes.XTICKSNAPSHOTRESPONSE;
            this.Tick = null;
        }

        public Tick Tick { get; set; }

        public override string ResponseSerialize()
        {
            if (this.Tick == null)
                return string.Empty;
            return TickImpl.Serialize(this.Tick);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.Tick = null;
                return;
            }
            this.Tick = TickImpl.Deserialize(content);
        }
    }


    /// <summary>
    /// 查交易账户请求
    /// </summary>
    public class XQryAccountRequest : RequestPacket
    {
        public XQryAccountRequest()
        {
            _type = MessageTypes.XQRYACCOUNT;
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
    /// 查询交易账户回报
    /// </summary>
    public class RspXQryAccountResponse : RspResponsePacket
    {
        public RspXQryAccountResponse()
        {
            _type = MessageTypes.XACCOUNTRESPONSE;
            this.Account = null;
        }

        public AccountLite Account { get; set; }

        public override string ResponseSerialize()
        {
            if (this.Account == null)
                return string.Empty;
            return AccountLite.Serialize(this.Account);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                this.Account = null;
            else
                this.Account = AccountLite.Deserialize(content);
        }
    }
}
