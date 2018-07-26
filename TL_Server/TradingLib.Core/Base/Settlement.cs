using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.Core
{
    /// <summary>
    /// 结算辅助类
    /// 用于生成结算单
    /// </summary>
    public class SettlementFactory
    {
        private static string padLeftEx(string str, int totalByteCount)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())//判断字符是2位还是1位 如果是2位就将移位+1
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }
            string w = str.PadRight(totalByteCount - dcount);
            return w;
        }


        private static string padRightEx(string str, int totalByteCount)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())//判断字符是2位还是1位 如果是2位就将移位+1
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }

            string w = str.PadLeft(totalByteCount - dcount);
            return w;
        }

        private static string padCenterEx(string str, int totalByteCount)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())//判断字符是2位还是1位 如果是2位就将移位+1
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }
            int strcnt = dcount + str.Length;
            int remaincnt = totalByteCount - strcnt;
            int leftcnt = remaincnt/2;
            return str.PadLeft(leftcnt + strcnt - dcount).PadRight(totalByteCount - dcount);
        }

        public static string FieldName(string field, int width)
        {
            return padRightEx(field, width);
        }



        static string line = "------------------------------------------------------------------------------------------------------";
        static int fieldwidth_total = 15;
        static int field_blank = 4;
        static string filed_blank_str = new string(' ', field_blank);

        static string tp_total = "{0}{1,10:F2}"+filed_blank_str+"{2}{3,10:F2}"+filed_blank_str+"{4}{5,0:F2}";
        static string tp_onefield = "{0}{1,10:F2}";
        static string tp_twofield = "{0}{1,10:F2}" + filed_blank_str + "{2}{3,10:F2}";


        static string NewLine = "";

        static int section_location = 50;
        static string SectionName(string name)
        {
            return string.Format("{0," + section_location.ToString() + "}", name);
        }

        static ConfigDB _cfgdb = null;
        static string comment = string.Empty;
        static string header1 = string.Empty;
        static SettlementFactory()
        {
            _cfgdb = new ConfigDB("SettlementFactory");
            if (!_cfgdb.HaveConfig("Comment"))
            {
                _cfgdb.UpdateConfig("Comment", QSEnumCfgType.String, "对结算单如有疑问，请于下一交易日上午11:00以前到结算部查询，过期责任自负！！！", "结算单最后免责声明");
            }
            comment = _cfgdb["Comment"].AsString();

            if (!_cfgdb.HaveConfig("Header1"))
            {
                _cfgdb.UpdateConfig("Header1", QSEnumCfgType.String, "交易结算单(盯市)","结算表第一行表头");
            }
            header1 = _cfgdb["Header1"].AsString();
            
        }
        public static string Line(int num)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                sb.Append("-");
            }
            return sb.ToString();
        }
        const int len_EXCH = 10;
        const int len_SECURITY = 12;
        const int len_SYMBOL = 10;
        const int len_DATE = 8;
        const int len_TBMM = 5;
        const int len_SIZE = 6;
        const int len_PRICE = 12;
        const int len_MARGIN = 12;
        const int len_PROFIT = 12;
        const int len_TURNOVER = 13;
        const int len_COMMISSION = 12;
        const int len_SEQID = 8;

        static Dictionary<string, string> symFormatMap = new Dictionary<string, string>();

        static string GetPriceFormat(string symbol)
        {
            string format = string.Empty;
            if (symFormatMap.TryGetValue(symbol, out format))
            {
                return format;
            }
            else
            {
                string sec = SymbolImpl.ParseSecCode(symbol);
                SecurityFamily s = BasicTracker.SecurityTracker[1, sec];
                if (s == null)
                {
                    format = "{0:F2}";
                }
                else
                {
                    format = s.GetPriceFormat();
                }
                symFormatMap.Add(symbol, format);
                return format;
            }
        }

        static ILog logger = LogManager.GetLogger("SettlementFactory");
        public static List<string> GenSettlementFile(AccountSettlement s, IAccount account)
        {
            try
            {
                

                List<string> settlelist = new List<string>();

                //查询历史持仓 计算保证金占用
                //IList<SettlePosition> positions = ORM.MTradingInfo.SelectHistPositions(s.Account, s.Settleday, s.Settleday);
                //成交明细
                IEnumerable<Trade> trades = ORM.MTradingInfo.SelectTrades(s.Account, s.Settleday, s.Settleday);
                //持仓明细
                IEnumerable<PositionDetail> positiondetails = ORM.MSettlement.SelectPositionDetails(s.Account, s.Settleday);

                //平仓明细
                IEnumerable<PositionCloseDetail> positionclose = ORM.MSettlement.SelectPositionCloseDetail(s.Account, s.Settleday);

                AccountProfile profile = BasicTracker.AccountProfileTracker[account.ID];
                string brokername = string.IsNullOrEmpty(profile.Broker) ? "" : profile.Broker;
                string custname = string.IsNullOrEmpty(profile.Name) ? account.ID : profile.Name;
                decimal margin = 0;// positiondetails.Sum(pos => pos.Margin);
                foreach (var pd in positiondetails)
                {
                    SecurityFamily sec = account.GetSecurity(pd.SecCode);
                    if (sec != null)
                    {
                        margin += account.GetExchangeRate(s.Settleday, sec) * pd.Margin;
                    }
                }

                settlelist.Add(NewLine);
                settlelist.Add(SectionName(brokername));
                settlelist.Add(line);
                settlelist.Add(SectionName(header1));
                settlelist.Add(string.Format("{0}{1,10}      {2}{3,10}      {4}{5,10}", padRightEx("客户号:", 10), s.Account, padRightEx("客户名称:", 10), custname, padRightEx("日期:", 10), s.Settleday));
                settlelist.Add(NewLine);
                settlelist.Add(NewLine);
                settlelist.Add(SectionName("资金状况"));
                settlelist.Add(line);
                settlelist.Add(string.Format(tp_total, FieldName("上日结存:", fieldwidth_total), s.LastEquity, FieldName("当日结存:", fieldwidth_total), s.EquitySettled, FieldName("可用资金:", fieldwidth_total), s.EquitySettled - margin));
                settlelist.Add(string.Format(tp_total, FieldName("出入金:", fieldwidth_total), s.CashIn - s.CashOut, FieldName("客户权益:", fieldwidth_total), s.EquitySettled, FieldName("风险度:", fieldwidth_total), s.EquitySettled != 0 ? margin / s.EquitySettled : 0));
                settlelist.Add(string.Format(tp_total, FieldName("手续费:", fieldwidth_total), s.Commission, FieldName("总保证金占用:", fieldwidth_total), margin, FieldName("追加保证金:", fieldwidth_total), 0));
                settlelist.Add(string.Format(tp_total, FieldName("平仓盈亏:", fieldwidth_total), s.CloseProfitByDate, FieldName("持仓盯市盈亏:", fieldwidth_total), s.PositionProfitByDate, FieldName("交割保证金:", fieldwidth_total), 0));
                settlelist.Add(string.Format(tp_onefield, FieldName("可提资金", fieldwidth_total), s.EquitySettled - margin));
                settlelist.Add(string.Format(tp_onefield, FieldName("总盈亏", fieldwidth_total), s.CloseProfitByDate + s.PositionProfitByDate - s.Commission));
                settlelist.Add(NewLine);
                settlelist.Add(NewLine);

                #region 输出成交明细
                if (trades.Count() > 0)
                {
                    int ln = 132;
                    string sline = Line(ln);
                    int i = 0;
                    int size = 0;
                    decimal tunover = 0;
                    decimal commission = 0;
                    decimal profit = 0;

                    settlelist.Add(SectionName("成交明细"));
                    settlelist.Add(sline);
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|",
                        padCenterEx("成交日期", len_DATE),
                        padCenterEx("交易所", len_EXCH),
                        padCenterEx("品种", len_SECURITY),
                        padCenterEx("合约", len_SYMBOL),
                        padCenterEx("买/卖", len_TBMM),
                        padCenterEx("投/保", len_TBMM),
                        padCenterEx("成交价", len_PRICE),
                        padCenterEx("手数", len_SIZE),
                        padCenterEx("成交额", len_TURNOVER),
                        padCenterEx("开/平", len_TBMM),
                        padCenterEx("手续费(基)", len_PRICE),
                        padCenterEx("平仓盈亏(基)", len_COMMISSION),
                        padCenterEx("成交序号", len_SEQID)
                        ));
                    settlelist.Add(sline);

                    foreach (Trade t in trades)
                    {
                        string fmt = GetPriceFormat(t.Symbol);
                        SecurityFamily sym = account.GetSecurity(t.SecurityCode);
                        decimal rate = account.GetExchangeRate(s.Settleday, sym);
                        i++;
                        size += Math.Abs(t.xSize);
                        tunover += sym.GetMultiple() * t.xPrice * Math.Abs(t.xSize);
                        commission += t.Commission * rate;
                        profit += t.Profit * rate;

                        settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|",
                            padCenterEx(t.xDate.ToString(), len_DATE),
                            padCenterEx(BasicTracker.ExchagneTracker.GetExchangeTitle(t.Exchange), len_EXCH),
                            padCenterEx(sym.GetSecurityName(), len_SECURITY),
                            padCenterEx(t.Symbol, len_SYMBOL),
                            padLeftEx((t.xSize > 0 ? "买" : " 卖"), len_TBMM),
                            padLeftEx("投", len_TBMM),
                            padCenterEx(t.xPrice.ToFormatStr(), len_PRICE),
                            padRightEx(t.UnsignedSize.ToString(), len_SIZE),
                            padRightEx((sym.GetMultiple() * t.xPrice * Math.Abs(t.xSize)).ToFormatStr(fmt), len_TURNOVER),
                            padLeftEx(GetCombFlag(t.OffsetFlag), len_TBMM),
                            padRightEx((t.Commission * rate).ToFormatStr(), len_PRICE),
                            padRightEx((t.Profit * rate).ToFormatStr(), len_COMMISSION),
                            padRightEx(t.TradeID, len_SEQID)

                            ));
                    }

                    settlelist.Add(sline);
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|",
                            padLeftEx("共" + i.ToString() + "条", len_DATE),
                            padCenterEx("", len_EXCH),
                            padCenterEx("", len_SECURITY),
                            padCenterEx("", len_SYMBOL),
                            padCenterEx("", len_TBMM),
                            padCenterEx("", len_TBMM),
                            padCenterEx("", len_PRICE),
                            padRightEx(size.ToString(), len_SIZE),
                            padRightEx(tunover.ToFormatStr(), len_TURNOVER),
                            padCenterEx("", len_TBMM),
                            padRightEx(commission.ToFormatStr(), len_PRICE),
                            padRightEx(profit.ToFormatStr(), len_COMMISSION),
                            padRightEx("", len_SEQID)
                        ));
                    settlelist.Add(sline);
                    settlelist.Add(NewLine);
                    settlelist.Add(NewLine);
                }
                #endregion

                #region 输出平仓明细
                if (positionclose.Count() > 0)//平仓成交数量大于0 则输出明细
                {
                    int ln = 113;
                    string sline = Line(ln);

                    int i = 0;
                    int size = 0;
                    decimal profit = 0;
                    
                    settlelist.Add(SectionName("平仓明细"));
                    settlelist.Add(sline);
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|",
                        padCenterEx("平仓日期", len_DATE),
                        padCenterEx("交易所", len_EXCH),
                        padCenterEx("品种", len_SECURITY),
                        padCenterEx("合约", len_SYMBOL),
                        padCenterEx("开仓日期", len_DATE),
                        padCenterEx("买/卖", len_TBMM),
                        padCenterEx("手数", len_SIZE),
                        padCenterEx("开仓价", len_PRICE),
                        padCenterEx("昨结算", len_PRICE),
                        padCenterEx("成交价", len_PRICE),
                        padCenterEx("平仓盈亏(基)", len_PROFIT)
                        ));
                    settlelist.Add(sline);

                    foreach (PositionCloseDetail t in positionclose)
                    {
                        SecurityFamily sym = account.GetSecurity(t.SecCode);
                        decimal rate = account.GetExchangeRate(s.Settleday, sym);
                        string fmt = GetPriceFormat(t.Symbol);
                        i++;
                        size += t.CloseVolume;
                        profit += t.CloseProfitByDate * rate;
                        settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|",
                            padCenterEx(t.CloseDate.ToString(), len_DATE),
                            padCenterEx(BasicTracker.ExchagneTracker.GetExchangeTitle(t.Exchange), len_EXCH),
                            padCenterEx(sym.GetSecurityName(), len_SECURITY),
                            padCenterEx(t.Symbol, len_SYMBOL),
                            padCenterEx(t.OpenDate.ToString(), len_DATE),
                            padLeftEx((t.Side ? "买" : " 卖"), len_TBMM),
                            padRightEx(t.CloseVolume.ToString(), len_SIZE),
                            padCenterEx(t.OpenPrice.ToFormatStr(fmt), len_PRICE),
                            padCenterEx(t.LastSettlementPrice.ToFormatStr(), len_PRICE),
                            padCenterEx(t.ClosePrice.ToFormatStr(fmt), len_PRICE),
                            padRightEx((t.CloseProfitByDate * rate).ToFormatStr(fmt), len_PROFIT)
                            ));
                    }
                    settlelist.Add(sline);
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|",
                            padLeftEx("共" + i.ToString() + "条", len_DATE),
                            padCenterEx("", len_EXCH),
                            padCenterEx("", len_SECURITY),
                            padCenterEx("", len_SYMBOL),
                            padCenterEx("", len_DATE),
                            padCenterEx("", len_TBMM),
                            padCenterEx(size.ToString(), len_SIZE),
                            padCenterEx("", len_PRICE),
                            padCenterEx("", len_PRICE),
                            padCenterEx("", len_PRICE),
                            padRightEx(profit.ToFormatStr(), len_PROFIT)

                            ));
                    settlelist.Add(sline);
                    settlelist.Add(NewLine);
                    settlelist.Add(NewLine);
                }
                #endregion

                #region 输出持仓明细
                if (positiondetails.Count() > 0)
                {
                    int ln = 142;
                    string sline = Line(ln);

                    int i = 0;
                    int size = 0;
                    decimal unpl = 0;
                    decimal unplbydate = 0;
                    decimal hmargin = 0;
                    settlelist.Add(SectionName("持仓明细"));
                    settlelist.Add(sline);
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|",
                        padCenterEx("交易所", len_EXCH),
                        padCenterEx("品种", len_SECURITY),
                        padCenterEx("合约", len_SYMBOL),
                        padCenterEx("开仓日期", len_DATE),
                        padCenterEx("投/保", len_TBMM),
                        padCenterEx("买/卖", len_TBMM),
                        padCenterEx("持仓量", len_SIZE),
                        padCenterEx("开仓价", len_PRICE),
                        padCenterEx("昨结算", len_PRICE),
                        padCenterEx("今结算", len_PRICE),
                        padCenterEx("浮动盈亏", len_PROFIT),
                        padCenterEx("盯市盈亏", len_PROFIT),
                        padCenterEx("保证金", len_MARGIN)
                        ));
                    settlelist.Add(sline);
                    foreach (PositionDetail pd in positiondetails)
                    {
                        SecurityFamily sym = account.GetSecurity(pd.SecCode);
                        decimal rate = account.GetExchangeRate(s.Settleday, sym);
                        i++;
                        size += pd.Volume;
                        unpl += 0;
                        unplbydate += pd.PositionProfitByDate;
                        hmargin += pd.Margin;
                        string fmt = GetPriceFormat(pd.Symbol);
                        settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|",
                            padCenterEx(BasicTracker.ExchagneTracker.GetExchangeTitle(pd.Exchange), len_EXCH),
                            padCenterEx(sym.GetSecurityName(), len_SECURITY),
                            padCenterEx(pd.Symbol, len_SYMBOL),
                            padCenterEx(pd.OpenDate.ToString(), len_DATE),
                            padLeftEx("投", len_TBMM),
                            padLeftEx((pd.Side ? "买" : " 卖"), len_TBMM),
                            padRightEx(pd.Volume.ToString(), len_SIZE),
                            padCenterEx(pd.OpenPrice.ToFormatStr(fmt), len_PRICE),
                            padCenterEx(pd.LastSettlementPrice.ToFormatStr(fmt), len_PRICE),
                            padCenterEx(pd.SettlementPrice.ToFormatStr(fmt), len_PRICE),
                            padRightEx("0", len_PROFIT),
                            padRightEx((pd.PositionProfitByDate * rate).ToFormatStr(), len_PROFIT),
                            padRightEx(pd.Margin.ToFormatStr(), len_MARGIN),
                            padRightEx(pd.Margin.ToFormatStr(), len_MARGIN)
                            ));
                    }
                    settlelist.Add(sline);
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|",
                        padLeftEx("共" + i.ToString() + "条", len_EXCH),
                        padCenterEx("", len_SECURITY),
                        padCenterEx("", len_SYMBOL),
                        padCenterEx("", len_DATE),
                        padCenterEx("", len_TBMM),
                        padCenterEx("", len_TBMM),
                        padRightEx(size.ToString(), len_SIZE),
                        padCenterEx("", len_PRICE),
                        padCenterEx("", len_PRICE),
                        padCenterEx("", len_PRICE),
                        padCenterEx("", len_PROFIT),
                        padCenterEx("", len_PROFIT),
                        padCenterEx("", len_MARGIN)

                        //padRightEx(unpl.ToFormatStr(), len_PROFIT),
                        //padRightEx(unplbydate.ToFormatStr(), len_PROFIT),
                        //padRightEx(hmargin.ToFormatStr(), len_MARGIN)
                        ));
                    settlelist.Add(sline);
                    settlelist.Add(NewLine);
                    settlelist.Add(NewLine);
                }
                #endregion

                #region 输出持仓汇总
                if (positiondetails.Count() > 0)
                {
                    int ln = 122;
                    string sline = Line(ln);

                    int i = 0;
                    int lsize = 0;
                    int ssize = 0;
                    decimal profit = 0;
                    decimal tmargin = 0;

                    settlelist.Add(SectionName("持仓汇总"));
                    settlelist.Add(sline);
                   
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|",
                        padCenterEx("合约", len_SYMBOL),
                        padCenterEx("买持", len_SIZE),
                        padCenterEx("买均价", len_PRICE),
                        padCenterEx("卖持", len_SIZE),
                        padCenterEx("卖均价", len_PRICE),
                        padCenterEx("昨结算", len_PRICE),
                        padCenterEx("今结算", len_PRICE),
                        padCenterEx("盯市盈亏", len_PROFIT),
                        padCenterEx("保证金占用", len_MARGIN),
                        padCenterEx("保证金(基)", len_MARGIN),
                        padCenterEx("投/保", len_TBMM)
                        ));
                    settlelist.Add(sline);

                    Dictionary<string, List<PositionDetail>> ret = GenPositionDetailMap(positiondetails);
                    foreach (string key in ret.Keys)
                    {
                        List<PositionDetail> list = ret[key];
                        if (list.Count > 0)
                        {

                            PositionDetail pd = list[0];
                            string fmt = GetPriceFormat(pd.Symbol);
                            SecurityFamily sym = account.GetSecurity(pd.SecCode);
                            decimal rate = account.GetExchangeRate(s.Settleday, sym);

                            int longsize = list.Where(pos => pos.Side).Sum(pos => pos.Volume);
                            int shortsize = list.Where(pos => !pos.Side).Sum(pos => pos.Volume);
                            decimal settleunpl = list.Sum(pos => pos.PositionProfitByDate);
                            decimal lmargin = list.Sum(pos => pos.Margin);
                            decimal longprice = 0;
                            decimal shortprice = 0;
                            if (longsize != 0)
                            {
                                longprice = list.Where(pos => pos.Side).Sum(pos => pos.Volume * pos.OpenPrice) / longsize;
                            }
                            if (shortsize != 0)
                            {
                                shortprice = list.Where(pos => !pos.Side).Sum(pos => pos.Volume * pos.OpenPrice) / shortsize;
                            }
                            settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|",
                            padCenterEx(pd.Symbol, len_SYMBOL),
                            padRightEx(longsize.ToString(), len_SIZE),
                            padRightEx(longprice.ToFormatStr(fmt), len_PRICE),
                            padRightEx(shortsize.ToString(), len_SIZE),
                            padRightEx(shortprice.ToFormatStr(fmt), len_PRICE),
                            padCenterEx(pd.LastSettlementPrice.ToFormatStr(fmt), len_PRICE),
                            padCenterEx(pd.SettlementPrice.ToFormatStr(fmt), len_PRICE),
                            padRightEx((settleunpl * rate).ToFormatStr(), len_PROFIT),
                            padRightEx(lmargin.ToFormatStr(), len_MARGIN),
                            padRightEx((lmargin * rate).ToFormatStr(), len_MARGIN),
                            padLeftEx("投", len_TBMM)
                        ));
                            i++;
                            lsize += longsize;
                            ssize += shortsize;
                            profit += settleunpl;
                            tmargin += lmargin*rate;
                        }


                    }
                    settlelist.Add(sline);
                    settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|",
                        padLeftEx("共" + i.ToString() + "条", len_SYMBOL),
                        padRightEx(lsize.ToString(), len_SIZE),
                        padCenterEx("", len_PRICE),
                        padRightEx(ssize.ToString(), len_SIZE),
                        padCenterEx("", len_PRICE),
                        padCenterEx("", len_PRICE),
                        padCenterEx("", len_PRICE),
                        padRightEx("", len_PROFIT),
                        padRightEx("", len_MARGIN),
                        padRightEx(tmargin.ToFormatStr(), len_MARGIN),
                        padCenterEx("", len_TBMM)
                        ));
                    settlelist.Add(sline);
                    settlelist.Add(NewLine);
                    settlelist.Add(NewLine);
                }
                #endregion


                settlelist.Add(comment);


                //settlelist.Add(string.Format(tp_total, FieldName("权利金收支:", fieldwidth_total), 500, FieldName("本币证金占用:", fieldwidth_total), 3434, FieldName("交割保证金:", fieldwidth_total), 98088));
                //settlelist.Add(string.Format(tp_total, FieldName("平仓盈亏:", fieldwidth_total), 500, FieldName("质押保证金占用:", fieldwidth_total), 3434, FieldName("交割手续费:", fieldwidth_total), 98088));

                //settlelist.Add(string.Format(tp_total, FieldName("持仓盯市盈亏:", fieldwidth_total), 500, FieldName("质押金:", fieldwidth_total), 3434, FieldName("行权手续费:", fieldwidth_total), 98088));
                //settlelist.Add(string.Format(tp_total, FieldName("执行盈亏:", fieldwidth_total), 500, FieldName("在途资金:", fieldwidth_total), 3434, FieldName("货币质押可用:", fieldwidth_total), 98088));
                //settlelist.Add(string.Format(tp_onefield, FieldName("基础保证金", fieldwidth_total), 333));
                //settlelist.Add(string.Format(tp_onefield, FieldName("今日货币质押", fieldwidth_total), 333));
                //settlelist.Add(string.Format(tp_onefield, FieldName("上日货币质押", fieldwidth_total), 333));
                //settlelist.Add(string.Format(tp_total, FieldName("可提资金:", fieldwidth_total), 500, FieldName("当日期权市值:", fieldwidth_total), 3434, FieldName("交易所保证金:", fieldwidth_total), 98088));
                //settlelist.Add(string.Format(tp_twofield, FieldName("总盈亏:", fieldwidth_total), 500, FieldName("客户市值权益:", fieldwidth_total), 3434));

                return settlelist;
            }
            catch (Exception ex)
            { 
                
            }
            return new List<string>();
            
        }

        static Dictionary<string, List<PositionDetail>> GenPositionDetailMap(IEnumerable<PositionDetail> poslist)
        {
            Dictionary<string, List<PositionDetail>> ret = new Dictionary<string, List<PositionDetail>>();
            foreach (PositionDetail pd in poslist)
            {
                if (!ret.Keys.Contains(pd.Symbol))
                {
                    ret.Add(pd.Symbol, new List<PositionDetail>());
                }
                ret[pd.Symbol].Add(pd);
            }
            return ret;
        }


        static string GetCombFlag(QSEnumOffsetFlag op)
        {
            if (op == QSEnumOffsetFlag.OPEN)
            {
                return "开仓";
            }
            else if (op == QSEnumOffsetFlag.UNKNOWN)
            {
                return "未知";
            }
            else
            {
                return "平仓";
            }
                
                
                

        }
    }
}
