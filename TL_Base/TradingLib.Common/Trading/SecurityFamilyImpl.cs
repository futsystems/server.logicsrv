using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 证券品种簇定义
    /// </summary>
    public class SecurityFamilyImpl:SecurityFamily
    {
        public int ID { get; set; }

        /// <summary>
        /// 品种代号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 品种名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 货币类别
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 品种类别
        /// </summary>
        public SecurityType Type { get; set; }

        /// <summary>
        /// 所属交易所
        /// </summary>
        public IExchange Exchange { get; set; }


        /// <summary>
        /// 乘数
        /// </summary>
        public int Multiple { get; set; }


        /// <summary>
        /// 最小价格变动
        /// </summary>
        public decimal PriceTick { get; set; }

        /// <summary>
        /// 是否可交易
        /// </summary>
        public bool Tradeable { get; set; }

        /// <summary>
        /// 底层证券
        /// 某个衍生品证券会依赖于底层证券
        /// 比如沪深300股指期货依赖于沪深300，沪深300股指期权依赖于沪深300
        /// 沪深300不可交易，而起衍生品证券可以进行交易
        /// </summary>
        public SecurityFamily UnderLaying { get; set; }

        /// <summary>
        /// 开仓手续费
        /// </summary>
        public decimal EntryCommission { get; set; }

        /// <summary>
        /// 平仓手续费
        /// </summary>
        public decimal ExitCommission { get; set; }

        /// <summary>
        /// 平今手续费
        /// </summary>
        public decimal ExitCommissionToday { get; set; }

        /// <summary>
        /// 保证金比例
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// 额外保证金字段
        /// 用于在基本保证金外提供额外质押
        /// </summary>
        public decimal ExtraMargin { get; set; }

        /// <summary>
        /// 过夜保证金,如果需要过夜则需要提供Maintance保证金
        /// </summary>
        public decimal MaintanceMargin { get; set; }

        int _domainid = 0;
        public int Domain_ID { get { return _domainid; } set { _domainid = value; } }

        public bool IsValid
        {
            get
            {
                return (!string.IsNullOrEmpty(this.Code));
            }
        }
        public override string ToString()
        {
            return ID.ToString() + " Code:" + Code.ToString() + " Name:" + Name.ToString() + " Currency:" + Currency.ToString() + " Exch:" + Util.SafeToString(Exchange) + " Mutil:" + Multiple.ToString() + " PriceTick:" + PriceTick.ToString() + " Tradeable:" + Tradeable.ToString() + " Underlaying:" + Util.SafeToString(UnderLaying) + " EntryC:" + EntryCommission.ToString() + " ExitC:" + ExitCommission.ToString() + " Margin:" + Margin.ToString() + " underlaying_fk:" + Util.SafeToString(underlaying_fk); 
        }

        /// <summary>
        /// 交易时间段 开市时间
        /// </summary>
        public IMarketTime MarketTime { get; set; }


        /// <summary>
        /// 该品种对应的行情源
        /// </summary>
        public QSEnumDataFeedTypes DataFeed { get; set; }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';

            sb.Append(this.ID.ToString());
            sb.Append(d);
            sb.Append(this.Code);
            sb.Append(d);
            sb.Append(this.Name);
            sb.Append(d);
            sb.Append(this.Currency.ToString());
            sb.Append(d);
            sb.Append(this.Type.ToString());
            sb.Append(d);
            sb.Append(this.Multiple.ToString());//5
            sb.Append(d);
            sb.Append(this.PriceTick.ToString());
            sb.Append(d);
            sb.Append(this.Tradeable.ToString());
            sb.Append(d);
            sb.Append(this.EntryCommission.ToString());
            sb.Append(d);
            sb.Append(this.ExitCommission.ToString());
            sb.Append(d);
            sb.Append(this.Margin.ToString());//10
            sb.Append(d);
            sb.Append(this.ExtraMargin.ToString());
            sb.Append(d);
            sb.Append(this.MaintanceMargin.ToString());
            sb.Append(d);
            sb.Append(this.exchange_fk.ToString());//exchange
            sb.Append(d);
            sb.Append(this.underlaying_fk.ToString());//securityfamily
            sb.Append(d);
            sb.Append(this.mkttime_fk.ToString());//markettime
            sb.Append(d);
            sb.Append(this.ExitCommissionToday);
            sb.Append(d);
            sb.Append(this.DataFeed);
            
            return sb.ToString();
        }

        //对象的嵌套是在对象初始化时候通过fk获得的
        public int exchange_fk { get; set; }
        public int ExchangeFK { get { return this.Exchange != null ? (this.Exchange as Exchange).ID : 0; } }
        public int underlaying_fk{get;set;}
        public int UnderLayingFK { get { return this.UnderLaying != null ? (this.UnderLaying as SecurityFamilyImpl).ID : 0; } }
        public int mkttime_fk{get;set;}
        public int MarketTimeFK { get { return this.MarketTime != null ? (this.MarketTime as MarketTime).ID : 0; } }
        
        public void Deserialize(string content)
        {
            string[] rec = content.Split(',');
            this.ID = int.Parse(rec[0]);
            this.Code = rec[1];
            this.Name = rec[2];
            this.Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), rec[3]);
            this.Type = (SecurityType)Enum.Parse(typeof(SecurityType), rec[4]);
            this.Multiple = int.Parse(rec[5]);
            this.PriceTick = decimal.Parse(rec[6]);
            this.Tradeable = bool.Parse(rec[7]);
            this.EntryCommission = decimal.Parse(rec[8]);
            this.ExitCommission = decimal.Parse(rec[9]);
            this.Margin = decimal.Parse(rec[10]);
            this.ExtraMargin = decimal.Parse(rec[11]);
            this.MaintanceMargin = decimal.Parse(rec[12]);
            this.exchange_fk = int.Parse(rec[13]);
            this.underlaying_fk = int.Parse(rec[14]);
            this.mkttime_fk = int.Parse(rec[15]);
            this.ExitCommissionToday = decimal.Parse(rec[16]);
            this.DataFeed = (QSEnumDataFeedTypes)Enum.Parse(typeof(QSEnumDataFeedTypes), rec[17]);
        }
    }
}
