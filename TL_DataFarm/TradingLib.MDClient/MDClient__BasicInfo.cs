using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.MDClient
{
    /// <summary>
    /// 维护基础数据
    /// </summary>
    public partial class MDClient
    {
        /// <summary>
        /// 市场时间段map
        /// </summary>
        Dictionary<int, MarketTime> markettimemap = new Dictionary<int, MarketTime>();

        /// <summary>
        /// 交易所map
        /// </summary>
        Dictionary<int, Exchange> exchangemap = new Dictionary<int, Exchange>();

        /// <summary>
        /// 品种map
        /// </summary>
        Dictionary<int, SecurityFamilyImpl> securitymap = new Dictionary<int, SecurityFamilyImpl>();

        /// <summary>
        /// 品种名称map
        /// </summary>
        Dictionary<string, SecurityFamilyImpl> securitynamemap = new Dictionary<string, SecurityFamilyImpl>();

        /// <summary>
        /// 合约map
        /// </summary>
        Dictionary<int, SymbolImpl> symbolmap = new Dictionary<int, SymbolImpl>();

        /// <summary>
        /// 合约名称map
        /// </summary>
        Dictionary<string, SymbolImpl> symbolnamemap = new Dictionary<string, SymbolImpl>();


        void OnXQryMarketTimeResponse(RspXQryMarketTimeResponse response)
        {
            if (response.MarketTime!= null)
            {
                MarketTime target = null;
                if (markettimemap.TryGetValue(response.MarketTime.ID, out target))
                {
                    //更新
                    target.Name = response.MarketTime.Name;
                    target.Description = response.MarketTime.Description;
                    target.CloseTime = response.MarketTime.CloseTime;
                    target.DeserializeTradingRange(response.MarketTime.SerializeTradingRange());//将时间小节解析到对象中
                }
                else
                {
                    markettimemap.Add(response.MarketTime.ID, response.MarketTime);
                }
            }
            if (response.IsLast)
            {
                logger.Info("交易时间查询完毕,查询交易所信息");
                QryExchange();
            }
        }

        void OnXQryExchangeResponse(RspXQryExchangeResponse response)
        {
            if (response.Exchange != null)
            {
                Exchange target = null;
                if (exchangemap.TryGetValue(response.Exchange.ID, out target))
                {
                    //更新
                    target.Name = response.Exchange.Name;
                    target.EXCode = response.Exchange.EXCode;
                    target.Country = response.Exchange.Country;
                    target.Title = response.Exchange.Title;
                    target.Calendar = response.Exchange.Calendar;
                    target.CloseTime = response.Exchange.CloseTime;
                }
                else
                {
                    exchangemap.Add(response.Exchange.ID, response.Exchange);
                }
            }
            if (response.IsLast)
            {
                logger.Info("交易所查询完毕,查询品种信息");
                QrySecurity();
            }
        }

        void OnXQrySecurityResponse(RspXQrySecurityResponse response)
        {
            if (response.SecurityFaimly != null)
            {
                SecurityFamilyImpl target = null;
                if (securitymap.TryGetValue(response.SecurityFaimly.ID, out target))
                {
                    //更新
                    target.Code = response.SecurityFaimly.Code;
                    target.Name = response.SecurityFaimly.Name;
                    target.Currency = response.SecurityFaimly.Currency;
                    target.Type = response.SecurityFaimly.Type;

                    target.exchange_fk = response.SecurityFaimly.exchange_fk;
                    //target.Exchange = this.GetExchange(target.exchange_fk);

                    target.mkttime_fk = response.SecurityFaimly.mkttime_fk;
                    //target.MarketTime = this.GetMarketTime(target.mkttime_fk);

                    target.underlaying_fk = response.SecurityFaimly.underlaying_fk;
                    //target.UnderLaying = this.GetSecurity(target.underlaying_fk);

                    target.Multiple = response.SecurityFaimly.Multiple;
                    target.PriceTick = response.SecurityFaimly.PriceTick;
                    target.EntryCommission = response.SecurityFaimly.EntryCommission;
                    target.ExitCommission = response.SecurityFaimly.ExitCommission;
                    target.Margin = response.SecurityFaimly.Margin;
                    target.ExtraMargin = response.SecurityFaimly.ExtraMargin;
                    target.MaintanceMargin = response.SecurityFaimly.MaintanceMargin;
                    target.Tradeable = response.SecurityFaimly.Tradeable;
                }
                else
                {
                    securitymap.Add(response.SecurityFaimly.ID, response.SecurityFaimly);
                }
            }

            if (response.IsLast)
            {
                logger.Info("品种查询完毕,查询合约信息");
                QrySymbol();
            }
        }

        void OnXQrySymbolResponse(RspXQrySymbolResponse response)
        {
            if (response.Symbol != null)
            {
                SymbolImpl target = null;
                if (symbolmap.TryGetValue(response.Symbol.ID, out target))
                {
                    //更新
                    target.Symbol = response.Symbol.Symbol;
                    target.EntryCommission = response.Symbol._entrycommission;
                    target.ExitCommission = response.Symbol._exitcommission;
                    target.Margin = response.Symbol._margin;
                    target.ExtraMargin = response.Symbol._extramargin;
                    target.MaintanceMargin = response.Symbol._maintancemargin;
                    target.Strike = response.Symbol.Strike;
                    target.OptionSide = response.Symbol.OptionSide;
                    target.ExpireDate = response.Symbol.ExpireDate;

                    target.security_fk = response.Symbol.security_fk;
                    target.SecurityFamily = this.GetSecurity(target.security_fk);

                    target.underlaying_fk = response.Symbol.underlaying_fk;
                    target.ULSymbol = this.GetSymbol(target.underlaying_fk);

                    target.underlayingsymbol_fk = response.Symbol.underlayingsymbol_fk;
                    target.UnderlayingSymbol = this.GetSymbol(target.underlayingsymbol_fk);
                    target.Tradeable = response.Symbol.Tradeable;
                }
                else //添加
                {
                    symbolmap.Add(response.Symbol.ID, response.Symbol);
                    response.Symbol.SecurityFamily = this.GetSecurity(response.Symbol.security_fk);
                    response.Symbol.ULSymbol = this.GetSymbol(response.Symbol.underlaying_fk);
                    response.Symbol.UnderlayingSymbol = this.GetSymbol(response.Symbol.underlayingsymbol_fk);
                    symbolnamemap[response.Symbol.Symbol] = response.Symbol;

                }
            }
            if (response.IsLast)
            {
                logger.Info("合约查询完毕,查询隔夜持仓");
                BindData();
                
                //CoreService.TLClient.ReqXQryYDPositon();
                if (!_inited)
                {
                    _inited = true;
                }
            }

        }

        /// <summary>
        /// 绑定对象数据
        /// </summary>
        void BindData()
        {
            foreach (SecurityFamilyImpl target in securitymap.Values)
            {
                target.Exchange = this.GetExchange(target.exchange_fk);
                target.MarketTime = this.GetMarketTime(target.mkttime_fk);
                target.UnderLaying = this.GetSecurity(target.underlaying_fk);
            }

            foreach (SymbolImpl target in symbolmap.Values)
            {
                target.SecurityFamily = this.GetSecurity(target.security_fk);
                target.ULSymbol = this.GetSymbol(target.underlaying_fk);
                target.UnderlayingSymbol = this.GetSymbol(target.underlayingsymbol_fk);
            }
            logger.Info("MarketTime     Num:" + markettimemap.Count.ToString());
            logger.Info("Exchange       Num:" + exchangemap.Count.ToString());
            logger.Info("Security       Num:" + securitymap.Count.ToString());
            logger.Info("Symbol         Num:" + symbolmap.Count.ToString());
        }



        #region 外部访问接口
        /// <summary>
        /// 市场时间段
        /// </summary>
        public IEnumerable<IMarketTime> MarketTimes
        {
            get
            {
                return markettimemap.Values;
            }
        }

        /// <summary>
        /// 交易所
        /// </summary>
        public IEnumerable<IExchange> Exchanges
        {
            get
            {
                return exchangemap.Values;
            }
        }

        /// <summary>
        /// 品种
        /// </summary>
        public IEnumerable<SecurityFamily> Securities
        {
            get
            {
                return securitymap.Values;
            }

        }

        /// <summary>
        /// 合约
        /// </summary>
        public IEnumerable<Symbol> Symbols
        {
            get
            {
                return symbolmap.Values.ToArray();
            }
        }


        private MarketTime GetMarketTime(int id)
        {
            MarketTime mt = null;
            if (markettimemap.TryGetValue(id, out mt))
            {
                return mt;
            }
            return null;
        }

        private Exchange GetExchange(int id)
        {
            Exchange ex = null;
            if (exchangemap.TryGetValue(id, out ex))
            {
                return ex;
            }
            return null;
        }
        private SecurityFamilyImpl GetSecurity(int id)
        {
            SecurityFamilyImpl sec = null;
            if (securitymap.TryGetValue(id, out sec))
            {
                return sec;
            }
            return null;
        }

        public SecurityFamily GetSecurity(string code)
        {
            SecurityFamilyImpl sec = null;
            if (securitynamemap.TryGetValue(code, out sec))
            {
                return sec;
            }
            return null;
        }

        private SymbolImpl GetSymbol(int id)
        {
            SymbolImpl sym = null;
            if (symbolmap.TryGetValue(id, out sym))
            {
                return sym;
            }
            return null;
        }



        public Symbol GetSymbol(string symbol)
        {
            SymbolImpl sym = null;
            if (symbolnamemap.TryGetValue(symbol, out sym))
            {
                return sym;
            }
            return null;
        }




        #endregion



    }
}
