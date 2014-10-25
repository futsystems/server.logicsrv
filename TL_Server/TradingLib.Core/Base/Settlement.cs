using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 结算辅助类
    /// 用于生成结算单
    /// </summary>
    public class SettlementFactory
    {


         private static string padRightEx(string str, int totalByteCount)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }
            string w = str.PadRight(totalByteCount - dcount);
            return w;
        }

        public static string FieldName(string field, int width)
        {
            return padRightEx(field, width);
        }



        static string line = "----------------------------------------------------------------------------------------------------";
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
        public static List<string> GenSettlementFile(Settlement s,IAccount account)
        {
            List<string> settlelist = new List<string>();

            //查询历史持仓 计算保证金占用
            IList<SettlePosition> positions = ORM.MTradingInfo.SelectHistPositions(s.Account, s.SettleDay, s.SettleDay);
            IList<Trade> trades = ORM.MTradingInfo.SelectHistTrades(s.Account, s.SettleDay, s.SettleDay);

            decimal margin = 0;
            foreach(SettlePosition pos in positions)
            {
                margin += pos.Margin;//结算持仓数据总包含对应的保证金记录 这样下次读取时直接累加即可
            }

            settlelist.Add(NewLine);
            settlelist.Add(SectionName(account.GetCustBroker()));
            settlelist.Add(line);
            settlelist.Add(SectionName(header1));
            settlelist.Add(string.Format("{0}{1,10}      {2}{3,10}      {4}{5,10}", padRightEx("客户号:", 10),s.Account, padRightEx("客户名称:", 10),account.GetCustName(), padRightEx("日期:", 10),s.SettleDay));
            settlelist.Add(NewLine);
            settlelist.Add(NewLine);
            settlelist.Add(SectionName("资金状况"));
            settlelist.Add(line);
            settlelist.Add(string.Format(tp_total, FieldName("上日结存:", fieldwidth_total), s.LastEquity, FieldName("当日结存:", fieldwidth_total), s.NowEquity, FieldName("可用资金:", fieldwidth_total), s.NowEquity - margin));
            settlelist.Add(string.Format(tp_total, FieldName("出入金:", fieldwidth_total), s.CashIn-s.CashOut, FieldName("客户权益:", fieldwidth_total), s.NowEquity, FieldName("风险度:", fieldwidth_total),s.NowEquity!=0 ?margin/s.NowEquity:0));
            settlelist.Add(string.Format(tp_total, FieldName("手续费:", fieldwidth_total), s.Commission, FieldName("总保证金占用:", fieldwidth_total), margin, FieldName("追加保证金:", fieldwidth_total), 0));
            settlelist.Add(string.Format(tp_total, FieldName("平仓盈亏:", fieldwidth_total), s.RealizedPL, FieldName("持仓盯市盈亏:", fieldwidth_total), s.UnRealizedPL, FieldName("交割保证金:", fieldwidth_total), 0));
            settlelist.Add(string.Format(tp_onefield, FieldName("可提资金", fieldwidth_total), s.NowEquity-margin));
            settlelist.Add(string.Format(tp_onefield, FieldName("总盈亏", fieldwidth_total), s.RealizedPL+s.UnRealizedPL-s.Commission));
            settlelist.Add(NewLine);
            settlelist.Add(NewLine);

            
            //输出成交明细
            if (trades.Count > 0)
            {
                settlelist.Add(SectionName("成交明细"));
                settlelist.Add(line);
                //settlelist.Add("|成交日期|交易所|品种|合约|买卖|投保|成交价|手数|成交额|开平|手续费|平仓盈亏|成交序号".Replace('|', '*'));
                settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|", padRightEx("成交日期", 10), padRightEx("交易所", 8), padRightEx("品种", 20), padRightEx("合约", 10), padRightEx("买卖", 4), padRightEx("投保", 4), padRightEx("成交价", 8), padRightEx("手数", 4), padRightEx("成交额", 10), padRightEx("开平", 4), padRightEx("手续费", 8), padRightEx("平仓盈亏", 10), padRightEx("成交序号", 10)).Replace('|', '*'));
                foreach (Trade t in trades)
                {

                    settlelist.Add(string.Format(" {0} {1} {2} {3} {4} {5} {6,8:F2} {7,4} {8,10:F2} {9} {10,6:F2} {11,10:F2} {12,10}", 
                        t.xdate.ToString().PadRight(10), //成交日期
                        padRightEx(BasicTracker.ExchagneTracker.GetExchangeTitle(t.Exchange), 8),//交易所
                        padRightEx(BasicTracker.SecurityTracker.GetSecurityName(t.SecurityCode), 20),//品种
                        padRightEx(t.symbol,10), //合约
                        padRightEx((t.xsize > 0 ? "买" : " 卖"), 4), //买卖 3
                        padRightEx("投", 4),//头保
                        t.xprice, //成交价 6
                        Math.Abs(t.xsize), //手数量 7
                        BasicTracker.SecurityTracker.GetMultiple(t.SecurityCode) * t.xprice * Math.Abs(t.xsize),//成交额
                        padRightEx(GetCombFlag(t.OffsetFlag), 4), //开平
                        t.Commission, //手续费
                        t.Profit,//平仓盈亏
                        t.BrokerKey));//成交序号
                }

                settlelist.Add(NewLine);
                settlelist.Add(NewLine);
            }

            //输出平仓明细
            IEnumerable<Trade> flattrades = trades.Where(t => !t.IsEntryPosition);
            if (flattrades.Count() > 0)//平仓成交数量大于0 则输出明细
            {
                settlelist.Add(SectionName("平仓明细"));
                settlelist.Add(line);
                //settlelist.Add("|平仓日期|交易所|品种|合约|开仓日期|买卖|手数|开仓价|昨结算|成交价格|平仓盈亏|".Replace('|', '*'));
                settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|", padRightEx("平仓日期", 10), padRightEx("交易所", 8), padRightEx("品种", 20), padRightEx("合约", 10), padRightEx("开仓日期", 10), padRightEx("买卖", 4), padRightEx("手数", 4), padRightEx("开仓价",10), padRightEx("昨结算",10), padRightEx("成交价", 10), padRightEx("平仓盈亏", 10)).Replace('|', '*'));
                foreach (Trade t in flattrades)
                {
                                                                   //                               
                    settlelist.Add(string.Format(" {0} {1} {2} {3} {4} {5} {6,4} {7,10:F2} {8,10:F2} {9,10:F2} {10,10:F2}", 
                        t.xdate.ToString().PadRight(10), //平仓日期
                        padRightEx(BasicTracker.ExchagneTracker.GetExchangeTitle(t.Exchange), 8),//交易所
                        padRightEx(BasicTracker.SecurityTracker.GetSecurityName(t.SecurityCode), 20),//品种
                        padRightEx(t.symbol,10), //合约
                        padRightEx("xxxxx",10),//开仓日期
                        padRightEx((t.xsize > 0 ? "买" : " 卖"), 4), //买卖
                        Math.Abs(t.xsize), //手数 6
                        2444.00,//开仓价  7
                        2450.00,//昨结算
                        t.xprice, //成交价
                        t.Profit//平仓盈亏
                        ));
                }
                settlelist.Add(NewLine);
                settlelist.Add(NewLine);
            }


            //输出持仓明细
            if (positions.Count > 0)
            {
                settlelist.Add(SectionName("持仓汇总"));
                settlelist.Add(line);

                settlelist.Add("|   合约   |买持| 买均价 |卖持| 卖均价 | 昨结算 | 今结算 |盯市盈亏|保证金占用|投保".Replace('|', '*'));
                
                Dictionary<string, List<SettlePosition>> ret = GenPositionPairMap(positions);
                foreach (string key in ret.Keys)
                {
                    List<SettlePosition> list = ret[key];
                    if (list.Count > 0)
                    {
                        string symbol = list[0].Symbol;
                        int longsize = list.Where(pos => pos.Size > 0).Sum(pos => pos.Size);
                        decimal longavgprice = longsize>0?list.Where(pos => pos.Size > 0).Sum(pos => pos.Size * pos.AVGPrice) / longsize : 0;
                        int shortsize = list.Where(pos => pos.Size < 0).Sum(pos => Math.Abs(pos.Size));
                        decimal shortavgprice =shortsize>0? list.Where(pos => pos.Size < 0).Sum(pos => Math.Abs(pos.Size) * pos.AVGPrice) / shortsize :0;
                        decimal settleprice = list[0].SettlePrice;
                        decimal settleunpl = list.Sum(pos=> pos.Size * (pos.SettlePrice - pos.AVGPrice) * pos.Multiple);
                        decimal lmargin = list.Sum(pos => pos.Margin);
                        settlelist.Add(string.Format(" {0,-10} {1,4} {2,8:F2} {3,4} {4,8:F2} {5,8:F2} {6,8:F2} {7,8:F2} {8,10:F2} {9,4}", symbol, longsize, longavgprice, shortsize, shortavgprice, 0, settleprice, settleunpl, lmargin, "投"));
                    }
                }

                settlelist.Add(NewLine);
                settlelist.Add(NewLine);
            }
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


        static Dictionary<string, List<SettlePosition>> GenPositionPairMap(IEnumerable<SettlePosition> poslist)
        {
            Dictionary<string, List<SettlePosition>> ret = new Dictionary<string, List<SettlePosition>>();
            foreach (SettlePosition sp in poslist)
            {
                if (!ret.Keys.Contains(sp.SecurityCode))
                {
                    ret.Add(sp.SecurityCode, new List<SettlePosition>());
                }
                ret[sp.SecurityCode].Add(sp);
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
