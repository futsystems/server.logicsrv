﻿using System;
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


        string _uniqueKey = string.Empty;
        /// <summary>
        /// 唯一键
        /// </summary>
        public string UniqueKey
        {
            get 
            {
                return string.Format("{0}-{1}",Exchange,Symbol);
            }
        }


        string _symbol = string.Empty;
        /// <summary>
        /// 合约代码
        /// </summary>
        public string Symbol {get;set;}

        /// <summary>
        /// 合约名称
        /// </summary>
        public string Name { get; set; }


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

        public decimal _exitcommissiontoday = 0;
        /// <summary>
        /// 平仓手续费
        /// </summary>
        public decimal ExitCommissionToday
        {
            get
            {

                if (_exitcommissiontoday < 0)
                {
                    if (SecurityFamily != null)
                    {
                        return SecurityFamily.ExitCommissionToday;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return _exitcommissiontoday;
                }

            }
            set
            {
                _exitcommissiontoday = value;
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
                    return SecurityFamily.Multiple;
                }
                else
                {
                    return 1;
                }
            }
        }

        public SecurityType SecurityType { get { return SecurityFamily != null ? SecurityFamily.Type : SecurityType.NIL; } }

        public CurrencyType Currency { get { return SecurityFamily != null ? SecurityFamily.Currency : CurrencyType.RMB; } }

        public string Exchange { get { return SecurityFamily != null ? SecurityFamily.Exchange.EXCode : ""; } }


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


        string _month = "01";
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get { return _month; } set { _month = value; } }


        QSEnumSymbolType _symboltype = QSEnumSymbolType.Standard;
        /// <summary>
        /// 合约类别
        /// </summary>
        public QSEnumSymbolType SymbolType { get { return _symboltype; } set { _symboltype = value; } }

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
            //过期日未0 则该合约永不过期
            if (this.ExpireDate == 0)
            {
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



        //TODO 统一通过全名函数来获得合约全名 比如在合约注册时所用
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
        /// 是否可以进行交易
        /// 这里Tradeable是指检查品种与合约的全局设定
        /// </summary>
        public bool IsTradeable
        {
            get
            {
                //Sec为空 不可交易
                if (this.SecurityFamily == null) return false;
                //NIL IDX不可交易
                if (this.SecurityFamily.Type == SecurityType.NIL) return false;
                if (this.SecurityFamily.Type == SecurityType.IDX) return false;
                //底层品种存在 底层品种可交易 当前合约可交易 是可交易的完备条件
                if (this.Tradeable &&  this.SecurityFamily.Tradeable)
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

        #region 辅助函数 转换国外合约月份字母
        public static string MonthLetter2Num(string month)
        {
            if (month == "F")
            {
                return "01";
            }
            else if (month == "G")
            {
                return "02";
            }
            else if (month == "H")
            {
                return "03";
            }
            else if (month == "J")
            {
                return "04";
            }
            else if (month == "K")
            {
                return "05";
            }
            else if (month == "M")
            {
                return "06";
            }
            else if (month == "N")
            {
                return "07";
            }
            else if (month == "Q")
            {
                return "08";
            }
            else if (month == "U")
            {
                return "09";
            }
            else if (month == "V")
            {
                return "10";
            }
            else if (month == "X")
            {
                return "11";
            }
            else if (month == "Z")
            {
                return "12";
            }
            else
            {
                throw new ArgumentException("Month must in (FGHJKMNQUVXZ)");
            }
        }

        /// <summary>
        /// 月份数字转换成字符
        /// </summary>
        /// <returns></returns>
        public static string MonthNum2Letter(string month)
        {
            if (month == "01")
            {
                return "F";
            }
            else if (month == "02")
            {
                return "G";
            }
            else if (month == "03")
            {
                return "H";
            }
            else if (month == "04")
            {
                return "J";
            }
            else if (month == "05")
            {
                return "K";
            }
            else if (month == "06")
            {
                return "M";
            }
            else if (month == "07")
            {
                return "N";
            }
            else if (month == "08")
            {
                return "Q";
            }
            else if (month == "09")
            {
                return "U";
            }
            else if (month == "10")
            {
                return "V";
            }
            else if (month == "11")
            {
                return "X";
            }
            else if (month == "12")
            {
                return "Z";
            }
            else
            {
                throw new ArgumentException("Month must in (01,02....12)");
            }
        }
        #endregion


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
            sb.Append(this.Month);//过期月份
            sb.Append(d);
            sb.Append(this.Tradeable.ToString());//该合约是否允许交易
            sb.Append(d);
            sb.Append(this.SymbolType);
            sb.Append(d);
            sb.Append(this.Name);
            sb.Append(d);
            sb.Append(this._exitcommissiontoday);

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
            this.Month = rec[13];
            this.Tradeable = bool.Parse(rec[14]);
            this.SymbolType = (QSEnumSymbolType)Enum.Parse(typeof(QSEnumSymbolType), rec[15]);
            this.Name = rec[16];
            this._exitcommissiontoday = decimal.Parse(rec[17]);
        }

        /// <summary>
        /// 从到期日获得该合约月份
        /// </summary>
        /// <param name="expiredate"></param>
        /// <returns></returns>
        public static string GetMonthFromExpireDate(int expiredate)
        {
            return expiredate.ToString().Substring(4,2);
        }
    }
}
