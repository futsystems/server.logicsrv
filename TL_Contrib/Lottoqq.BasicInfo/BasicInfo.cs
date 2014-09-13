using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;


namespace Lottoqq.BasicInfo
{
    /// <summary>
    /// 基本信息查询模块
    /// 用于给客户端查询交易所,品种,合约等基本交易数据
    /// </summary>
    [ContribAttr("BasicInfo", "基本信息查询模块", "用于查询交易所信息,证券品种,合约等信息")]
    public class BasicInfoServer : ContribSrvObject, IContrib
    {

        public BasicInfoServer()
            : base("BasicInfo")
        {

        }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {


        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            base.Dispose();
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }

        JsonWrapperExchange[] exchangelist = null;
        ConcurrentDictionary<string,JsonWrapperSecurityLite[]> exsecmap = new ConcurrentDictionary<string,JsonWrapperSecurityLite[]>();
        ConcurrentDictionary<string, JsonWrapperSymbolLite[]> secsymbolmap = new ConcurrentDictionary<string, JsonWrapperSymbolLite[]>();
        JsonWrapperSymbol[] symlist = null;
        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "qryexlist",
            "qryexlist - 查询交易所列表",
            "查询所有交易所")]
        public void CTE_QryExchangeList(ISession session)
        {
            debug("qryexlist", QSEnumDebugLevel.INFO);
            if (exchangelist == null)
            {
                exchangelist =(from ex in BasicTracker.ExchagneTracker.Exchanges
                select new JsonWrapperExchange(ex)).ToArray();
            }
            SendJsonObjs(session, exchangelist);
        }


        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "qryexsec",
            "qryexsec - 查询交易所所有品种",
            "查询交易所所有品种")]
        public void CTE_QryExSecurity(ISession session, string exchagneindex)
        { 
            //如果缓存中不存在交易所的合约列表
            if(!exsecmap.Keys.Contains(exchagneindex))
            {
                JsonWrapperSecurityLite[] seclist = (from sec in BasicTracker.SecurityTracker.Securities
                                             where sec.Exchange.Index.Equals(exchagneindex)
                                             select new JsonWrapperSecurityLite(sec)).ToArray();
                exsecmap.TryAdd(exchagneindex, seclist);
            }

            SendJsonObj(session, exsecmap[exchagneindex]);
        }

        [ContribCommandAttr(
           QSEnumCommandSource.MessageExchange,
           "qrysec",
           "qrysec - 查询某个品种的具体信息",
           "查询某个品种的具体信息")]
        public void CTE_QrySecurity(ISession sesion, string seccode)
        {
            if (seccode.ToUpper().Equals("ALL"))
            {
                foreach (SecurityFamily sec in BasicTracker.SecurityTracker.Securities)
                {
                    SendJsonObj(sesion, new JsonWrapperSecurity(sec));
                }
                SendJsonObj(sesion, "End" as object);
            }
            else
            {
                SecurityFamily sec = BasicTracker.SecurityTracker[seccode];
                if (sec == null) return;
                SendJsonObj(sesion, new JsonWrapperSecurity(sec));
            }
        }


        [ContribCommandAttr(
           QSEnumCommandSource.MessageExchange,
           "qrysecsym",
           "qrysecsym - 查询某个品种的所有合约",
           "查询某个品种的所有合约")]
        public void CTE_QrySecSymbol(ISession session, string seccode)
        {
            if (!secsymbolmap.Keys.Contains(seccode))
            {
                JsonWrapperSymbolLite[] symblist = (from sym in BasicTracker.SymbolTracker.Symbols
                                                    where sym.SecurityFamily.Code.Equals(seccode)
                                                    select new JsonWrapperSymbolLite(sym)).ToArray();
                secsymbolmap.TryAdd(seccode, symblist);                          
            }
            foreach (JsonWrapperSymbolLite symlite in secsymbolmap[seccode])
            {
                SendJsonObj(session, symlite);
            }
            SendJsonObj(session, "End" as object);
            
            
        }

        [ContribCommandAttr(
           QSEnumCommandSource.MessageExchange,
           "qrysym",
           "qrysym - 查询某个合约的所有字段属性",
           "查询某个合约的所有字段属性")]
        public void CTE_QrySymbol(ISession session, string symbol)
        {
            debug("qrysymbol called, symbol:" + symbol, QSEnumDebugLevel.INFO);
            if (symbol.ToUpper().Equals("ALL"))
            {
                if (symlist == null)
                {
                    symlist = (from sym in BasicTracker.SymbolTracker.Symbols
                               select new JsonWrapperSymbol(sym)).ToArray();
                }
                
                //foreach (JsonWrapperSymbol s in symlist)
                //{
                    MultiSendJsonObjs(session, symlist,false);
                //}
                SendJsonObj(session, "End" as object);
            }
            else
            {
                Symbol s = BasicTracker.SymbolTracker[symbol];
                if (s == null) return;
                SendJsonObj(session, new JsonWrapperSymbol(s),true);
            }
        }


        [ContribCommandAttr(
           QSEnumCommandSource.MessageExchange,
           "qrytime",
           "qrytime - 查询服务端时间",
           "查询服务端时间")]
        public void CTE_QrySrvTime(ISession session)
        {
            SendJsonObj(session, new JsonWrapperDateTime());
            
        }



        /*
        /// <summary>
        /// 查询某个合约所对应的所有期权合约
        /// </summary>
        [ContribCommandAttr(
           QSEnumCommandSource.MessageExchange,
           "qryderivativesymbol",
           "qrytime - 查询服务端时间",
           "查询服务端时间")]
        public void CTE_QrySymbolList(ISession session,string symbol)
        {
            Symbol s = BasicTracker.SymbolTracker[symbol];
            if (s == null) return;

            SecurityFamily[] familys = BasicTracker.SecurityTracker.GetUnderlayedOn(s.SecurityFamily);

            debug("symbol date:" + s.Date.ToString());

            foreach (SecurityFamily sf in familys)
            {
                debug("family:" + sf.Code);
            }

        
        }**/
        

    }

    internal class JsonWrapperDateTime
    {
        DateTime _now;
        public JsonWrapperDateTime()
        {
            _now = DateTime.Now;
        }
        public int TLDate
        {
            get
            {
                return Util.ToTLDate(_now);
            }
        }
        public int TLTime
        {
            get
            {
                return Util.ToTLTime(_now);
            }
        }

        public long TLDateTime
        {
            get
            {
                return Util.ToTLDateTime(_now);
            }
        }
        
        
    }
    internal class JsonWrapperSymbol
    {
        Symbol _sym;
        public JsonWrapperSymbol(Symbol symbol)
        {
            _sym = symbol;
        }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get { return _sym.Symbol; } }

        /// <summary>
        /// 品种代码
        /// </summary>
        public string Security { get { return _sym.SecurityFamily.Code; } }

        //手续费 保证金
        /// <summary>
        /// 开仓手续费
        /// </summary>
        public decimal EntryCommission { get { return _sym.EntryCommission; } }
        /// <summary>
        /// 平仓手续费
        /// </summary>
        public decimal ExitCommission { get { return _sym.ExitCommission; } }
        /// <summary>
        /// 保证金
        /// </summary>
        public decimal Margin { get { return _sym.Margin; } }
        /// <summary>
        /// 额外保证金
        /// </summary>
        public decimal ExtraMargin { get { return _sym.ExtraMargin; } }
        /// <summary>
        /// 过夜保证金
        /// </summary>
        public decimal MaintanceMargin { get { return _sym.MaintanceMargin; } }

        /// <summary>
        /// 乘数
        /// </summary>
        public decimal Multiple { get { return _sym.Multiple; } }

        /// <summary>
        /// 最小价格变动
        /// </summary>
        public decimal PriceTick { get { return _sym.SecurityFamily.PriceTick; } }

        /// <summary>
        /// 货币 参考接口头文件
        /// </summary>
        public string Currency { get { return _sym.Currency.ToString(); } }

        /// <summary>
        /// 类型 参考接口头文件
        /// </summary>
        public string SecType { get { return _sym.SecurityType.ToString(); } }

        /// <summary>
        /// 全称
        /// </summary>
        public string FullName { get { return _sym.FullName; } }
        /// <summary>
        /// 底层实际合约(用于衍生合约乐透)
        /// </summary>
        public string ULSymbol { get { if (_sym.ULSymbol != null) return _sym.ULSymbol.FullName; return ""; } }

        /// <summary>
        /// 交易所代码
        /// </summary>
        public string ExchCode {
            get{ 
                if(_sym.SecurityFamily.Exchange != null)
                {
                    return _sym.SecurityFamily.Exchange.EXCode;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 交易所编号
        /// </summary>
        public string ExchIndex {
            get
            {
                if (_sym.SecurityFamily.Exchange != null)
                {
                    return _sym.SecurityFamily.Exchange.Index;
                }
                else
                {
                    return "";
                }
            }
            
        }

        /// <summary>
        /// 过期月份
        /// </summary>
        public int ExpireMonth {

            get
            {
                return _sym.ExpireMonth;
            }
        }

        /// <summary>
        /// 过期日
        /// </summary>
        public int ExpreDate {
            get
            {
                return _sym.ExpireDate;
            }
        }

        /// <summary>
        /// 期权方向
        /// </summary>
        public string OptionSide
        {
            get
            {
                if (_sym.SecurityType == SecurityType.OPT)
                {
                    return _sym.OptionSide.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 执行价格
        /// </summary>
        public decimal Strick
        { 
            get
            {
                if (_sym.SecurityType == SecurityType.OPT)
                {
                    return _sym.Strike;
                }
                else
                {
                    return _sym.Strike;
                }

            }
        }

        /// <summary>
        /// 底层合约
        /// </summary>
        public string UnderlayingSymbol
        {
            get
            {
                if (_sym.UnderlayingSymbol != null)
                {
                    return _sym.UnderlayingSymbol.Symbol;
                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 底层合约称呼苏
        /// </summary>
        public int UnderlayMultiple 
        {
            get
            {
                if (_sym.UnderlayingSymbol != null)
                {
                    return _sym.UnderlayingSymbol.Multiple;
                }
                else
                {
                    return 0;
                }
            }
        }



        /// <summary>
        /// 底层合约最小价格变动
        /// </summary>
        public decimal UnderlayPriceTick
        {
            get
            {
                if (_sym.UnderlayingSymbol != null)
                {
                    return _sym.UnderlayingSymbol.SecurityFamily.PriceTick;
                }
                else
                {
                    return 0;
                }
            }

        }

    }

    internal class JsonWrapperSymbolLite
    {
        Symbol _sym;
        public JsonWrapperSymbolLite(Symbol symbol)
        {
            _sym = symbol;
        }

        public string Symbol {get{return _sym.Symbol;}}
        public string Security {get{return _sym.SecurityFamily.Code;}}
    }

    internal class JsonWrapperSecurity
    {
        SecurityFamily _secfam;
        public JsonWrapperSecurity(SecurityFamily sec)
        {
            _secfam = sec;

        }

        public string Code { get { return _secfam.Code; } }
        public string Name { get { return _secfam.Name; } }
        public string Currency { get { return _secfam.Currency.ToString(); } }
        public string SecType { get { return _secfam.Type.ToString(); } }
        public string Exchange { get { return _secfam.Exchange.Index; } }
        public int Multiple { get { return _secfam.Multiple; } }
        public decimal PriceTick { get { return _secfam.PriceTick; } }
        public string UnderLaying { get { if (_secfam.UnderLaying != null) return _secfam.UnderLaying.Code; return ""; } }
        public decimal EntryCommission { get { return _secfam.EntryCommission; } }
        public decimal ExitCommission { get { return _secfam.ExitCommission; } }
        public decimal Margin { get { return _secfam.Margin; } }
        public decimal ExtraMargin { get { return _secfam.ExtraMargin; } }
        public decimal MaintanceMargin { get { return _secfam.MaintanceMargin; } }

    }


    internal class JsonWrapperSecurityLite
    {
        SecurityFamily _secfam;
        public JsonWrapperSecurityLite(SecurityFamily sec)
        {
            _secfam = sec;
            
        }

        public string Code { get { return _secfam.Code; } }

        public string Exchange { get { return _secfam.Exchange.Index; } }
    }
    internal class JsonWrapperExchange
    {
        IExchange _exchange;
        public JsonWrapperExchange(IExchange ex)
        {
            _exchange = ex;
        }

        public string Name { get { return _exchange.Name; } }

        public string Country { get { return _exchange.Country.ToString(); } }

        public string Index { get { return _exchange.Index; } }

        public string ExCode { get { return _exchange.EXCode; } }
    }
}
