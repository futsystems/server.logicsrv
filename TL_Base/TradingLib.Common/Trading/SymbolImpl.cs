using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 合约实现
    /// </summary>
    public class SymbolImpl:Symbol
    {
        /// <summary>
        /// 数据库序号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 合约代码
        /// </summary>
        public string Symbol { get; set; }

        //品种外键
        public int security_fk { get; set; }
        /// <summary>
        /// 该合约属于哪个品种
        /// </summary>
        public SecurityFamily SecurityFamily { get; set; }

        //异化合约底层外键
        public int underlaying_fk { get; set; }
        /// <summary>
        /// 异化合约的底层合约对象
        /// </summary>
        public Symbol ULSymbol { get; set; }


        //底层合约外键
        public int underlayingsymbol_fk { get; set; }
        /// <summary>
        /// 底层合约 常规合约中 期权合约的底层标的
        /// </summary>
        public Symbol UnderlayingSymbol { get; set; }


        public decimal _entrycommission = 0;
        /// <summary>
        /// 开仓手续费
        /// </summary>
        public decimal EntryCommission
        {
            get { 
                if (_entrycommission < 0)
                {
                    if (SecurityFamily != null)
                    {
                        return SecurityFamily.EntryCommission;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return _entrycommission;
                }
            }

            set
            {
                _entrycommission = value;
            }
        }


        public decimal _exitcommission=0;
        /// <summary>
        /// 平仓手续费
        /// </summary>
        public decimal ExitCommission
        {
            get {

                if (_exitcommission < 0)
                {
                    if (SecurityFamily != null)
                    {
                        return SecurityFamily.ExitCommission;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return _exitcommission;
                }
            
            }
            set
            {
                _exitcommission = value;
            }
        }


        public decimal _margin = 0;
        /// <summary>
        /// 保证金比例
        /// 如果设定了保证金比例则取品种定义的保证金
        /// 如果品种的保证金没有定义则默认返回1,交易为全额保证金
        /// 异化合约直接设置合约的保证金信息
        /// </summary> 
        public decimal Margin
        {
            get
            {
                if (_margin < 0)
                {
                    //品种不为null
                    if (SecurityFamily != null)
                    {
                        return SecurityFamily.Margin > 0 ? SecurityFamily.Margin : 1;
                    }
                    return 1;
                }
                else
                {
                    return _margin;
                }
            }
            set
            {
                _margin = value;
            } 
        }

        public decimal _extramargin = 0;
        /// <summary>
        /// 额外保证金字段
        /// 用于在基本保证金外提供额外质押
        /// </summary>
        public decimal ExtraMargin
        {
            get
            {
                if (_extramargin < 0)
                {
                    if (SecurityFamily != null)
                    {
                        return SecurityFamily.Margin > 0 ? SecurityFamily.Margin : 0;
                    }
                    return 0;
                }
                else
                {
                    return _extramargin;
                }
                
            }
            set { _extramargin = value; } 
        }

        public decimal _maintancemargin = 0;
        /// <summary>
        /// 过夜保证金,如果需要过夜则需要提供Maintance保证金
        /// </summary>
        public decimal MaintanceMargin
        {
            get
            {
                if (_maintancemargin < 0)
                {
                    if (SecurityFamily != null)
                    {
                        return SecurityFamily.Margin > 0 ? SecurityFamily.Margin : 0;
                    }
                    return 0;
                }
                else
                {
                    return _maintancemargin;
                }
            }
            set { _maintancemargin = value; }
        }


        /// <summary>
        /// 期权 方向
        /// </summary>
        public QSEnumOptionSide OptionSide { get; set; }

        /// <summary>
        /// 期权中的行权价
        /// </summary>
        public decimal Strike { get; set; }

        /// <summary>
        /// 到期日,比如期货合约的交割日,期权合约的行权日
        /// </summary>
        public int ExpireDate { get; set; }


        /// <summary>
        /// 获得合约乘数
        /// 异化合约乘数为底层合约乘数
        /// </summary>
        public int Multiple
        {
            get
            {
                if (SecurityFamily != null)
                {
                    //如果该合约是异化合约,则其乘数参数为底层合约的参数
                    if (SecurityFamily.Type == API.SecurityType.INNOV)
                    {
                        return ULSymbol != null ? ULSymbol.Multiple : 1;
                    }
                    return SecurityFamily.Multiple;
                }
                else
                {
                    return 1;
                }
            }
        }
       
        /// <summary>
        /// 底层行情合约
        /// 用于异化证券的合约转换
        /// </summary>
        public string TickSymbol
        {
            get
            {
                //是否设定底层合约族
                if (SecurityFamily != null)
                {
                    //只有异化合约的底层symbol代表其取Tick值，其余底层依赖关系
                    if (SecurityFamily.Type == API.SecurityType.INNOV)
                    {
                        if (ULSymbol != null)
                        {
                            return ULSymbol.Symbol;
                        }
                        return Symbol;
                    }
                    else
                    {
                        return Symbol;
                    }
                }
                else
                {
                    return Symbol;
                }
            }
        }

        public SecurityType SecurityType { get { return SecurityFamily != null ? SecurityFamily.Type : SecurityType.NIL; } }

        public CurrencyType Currency { get { return SecurityFamily != null ? SecurityFamily.Currency : CurrencyType.RMB; } }

        public string Exchange { get { return SecurityFamily != null ? SecurityFamily.Exchange.Index : ""; } }


        bool _tradeable = false;
        /// <summary>
        /// 是否可交易 设置中设定的可交易标识
        /// </summary>
        public bool Tradeable
        {
            get { return _tradeable; }
            set { _tradeable = value; }
        }

        int _domainid = 0;
        public int Domain_ID { get { return _domainid; } set { _domainid = value; } }

        /// <summary>
        /// 检查合约是否是开市时间
        /// </summary>
        public bool IsMarketTime
        {
            get
            {
                if (SecurityFamily != null)
                {
                    //异化合约返回底层时间
                    if (SecurityFamily.Type == API.SecurityType.INNOV)
                    {
                        if (ULSymbol == null)
                        {
                            return false;
                        }
                        return ULSymbol.IsMarketTime;
                    }
                    return SecurityFamily.IsMarketTime;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 是否处于强平时间
        /// </summary>
        public bool IsFlatTime
        {
            get
            {
                if (SecurityFamily != null)
                {
                    //异化合约返回底层时间
                    if (SecurityFamily.Type == API.SecurityType.INNOV)
                    {
                        if (ULSymbol == null)
                        {
                            return false;
                        }
                        return ULSymbol.IsFlatTime;
                    }
                    return SecurityFamily.IsFlatTime;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 该合约是否有效 如果没有底层证券品种信息则该合约无效
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (SecurityFamily == null) return false;

                return true;
            }
        }

        /// <summary>
        /// 判断该合约是否过期
        /// </summary>
        public bool IsExpired(int date)
        {

            if (this.ExpireDate == 0)
            {
                //如果没有结算日信息则不过气
                return false;
            }
            else
            {
                return date > this.ExpireDate ? true : false;
            }
        }

        public string FullName
        {
            get
            {
                return Serialize(this);
            }
        }




        public static string Serialize(Symbol symbol)
        {
            List<string> p = new List<string>();
            p.Add(symbol.Symbol);
            //opt or fop
            if (symbol.SecurityType == SecurityType.OPT || symbol.SecurityType == SecurityType.FOP)
            {
                //添加日期
                if (symbol.ExpireDate != 0)
                    p.Add(symbol.ExpireDate.ToString());
                //添加call put
                if (symbol.SecurityType == SecurityType.OPT)
                    p.Add(symbol.OptionSide.ToString());
                //添加 行权价格
                if (symbol.Strike != 0)
                    p.Add(symbol.Strike.ToString("F4"));
            }
            //any exchagne info
            //if (symbol.HasExchange)
            //    p.Add(symbol.Exchange);
            //any security type info
            if ((symbol.SecurityType != SecurityType.NIL) && (symbol.SecurityType != SecurityType.STK))
                p.Add(symbol.SecurityType.ToString());
            return string.Join(" ", p.ToArray());
        }

        /// <summary>
        /// 是否可以进行交易 这里需要加入更多完备性检查
        /// </summary>
        public bool IsTradeable
        {
            get
            {
                //底层品种存在 底层品种可交易 当前合约可交易 是可交易的完备条件
                if (this.Tradeable && this.SecurityFamily != null && this.SecurityFamily.Tradeable)
                {
                    return true;
                }
                return false;
            }
        }

        public override string ToString()
        {
            return "Symbol:" + Symbol + " Security:" + Util.SafeToString(SecurityFamily) + " entrycommision:" + EntryCommission.ToString() + " exitcommission:" + ExitCommission.ToString() + " SecurityID:" + security_fk.ToString();
        }


        //int _expiremonth = 0;
        //public int ExpireMonth { get { return _expiremonth; } set { _expiremonth = value; } }

        //public int ExpireMonth { get; set; }


        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.ID.ToString());
            sb.Append(d);
            sb.Append(this.Symbol);
            sb.Append(d);
            sb.Append(this._entrycommission.ToString());
            sb.Append(d);
            sb.Append(this._exitcommission.ToString());
            sb.Append(d);
            sb.Append(this._margin.ToString());
            sb.Append(d);
            sb.Append(this._extramargin.ToString());
            sb.Append(d);
            sb.Append(this._maintancemargin.ToString());
            sb.Append(d);
            sb.Append(this.Strike.ToString());//执行价
            sb.Append(d);
            sb.Append(this.OptionSide.ToString());//期权方向
            sb.Append(d);
            sb.Append(this.ExpireDate.ToString());//到期日期
            sb.Append(d);
            sb.Append(this.security_fk.ToString());//品种外键
            sb.Append(d);
            sb.Append(this.underlaying_fk.ToString());//异化合约底层合约外键
            sb.Append(d);
            sb.Append(this.underlayingsymbol_fk.ToString());//底层合约外键
            sb.Append(d);
            sb.Append("0");//过期月份
            sb.Append(d);
            sb.Append(this.Tradeable.ToString());//该合约是否允许交易
            

            return sb.ToString();
        }

        public void Deserialize(string content)
        {
            string[] rec = content.Split(',');
            this.ID = int.Parse(rec[0]);
            this.Symbol = rec[1];
            this._entrycommission = decimal.Parse(rec[2]);
            this._exitcommission = decimal.Parse(rec[3]);
            this._margin = decimal.Parse(rec[4]);
            this._extramargin = decimal.Parse(rec[5]);
            this._maintancemargin = decimal.Parse(rec[6]);
            this.Strike = decimal.Parse(rec[7]);
            this.OptionSide = (QSEnumOptionSide)Enum.Parse(typeof(QSEnumOptionSide),rec[8]);
            this.ExpireDate = int.Parse(rec[9]);
            this.security_fk = int.Parse(rec[10]);
            this.underlaying_fk = int.Parse(rec[11]);
            this.underlayingsymbol_fk = int.Parse(rec[12]);
            //this.ExpireMonth = int.Parse(rec[13]);
            this.Tradeable = bool.Parse(rec[14]);
        }
    }
}
