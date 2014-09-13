using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public class SettlementHelper
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
        public static List<string> GenSettlementFile(Settlement s)
        {
            List<string> settlelist = new List<string>();

            IList<SettlePosition> positions = ORM.MTradingInfo.SelectHistPositions(s.Account, s.SettleDay, s.SettleDay);
            decimal margin=0;
            foreach(SettlePosition pos in positions)
            {
                margin += Math.Abs(pos.Size * pos.SettlePrice * pos.Multiple * 0.1M);
            }
            settlelist.Add(NewLine);
            settlelist.Add(SectionName("华西期货有限公司"));
            settlelist.Add(line);
            settlelist.Add(SectionName("交易结算单（盯市）"));
            settlelist.Add(string.Format("{0}{1,10}      {2}{3,10}      {4}{5,10}", padRightEx("客户号:", 10),s.Account, padRightEx("客户名称:", 10), "钱波", padRightEx("日期:", 10),s.SettleDay));
            settlelist.Add(NewLine);
            settlelist.Add(NewLine);
            settlelist.Add(SectionName("资金状况"));
            settlelist.Add(line);
            settlelist.Add(string.Format(tp_total, FieldName("上日结存:", fieldwidth_total), s.LastEqutiy, FieldName("当日结存:", fieldwidth_total), s.NowEquity, FieldName("可用资金:", fieldwidth_total), s.NowEquity-margin));
            settlelist.Add(string.Format(tp_total, FieldName("出入金:", fieldwidth_total), s.CashIn-s.CashOut, FieldName("客户权益:", fieldwidth_total), s.NowEquity, FieldName("风险度:", fieldwidth_total), margin/s.NowEquity));
            settlelist.Add(string.Format(tp_total, FieldName("手续费:", fieldwidth_total), s.Commission, FieldName("总保证金占用:", fieldwidth_total), margin, FieldName("追加保证金:", fieldwidth_total), 0));
            settlelist.Add(string.Format(tp_total, FieldName("平仓盈亏:", fieldwidth_total), s.RealizedPL, FieldName("持仓盯市盈亏:", fieldwidth_total), s.UnRealizedPL, FieldName("交割保证金:", fieldwidth_total), 0));
            settlelist.Add(string.Format(tp_onefield, FieldName("可提资金", fieldwidth_total), s.NowEquity-margin));
            settlelist.Add(string.Format(tp_onefield, FieldName("总盈亏", fieldwidth_total), s.RealizedPL+s.UnRealizedPL-s.Commission));
            settlelist.Add(NewLine);
            settlelist.Add(NewLine);
            settlelist.Add(SectionName("成交明细"));
            settlelist.Add(line);
            //settlelist.Add("|成交日期|交易所|品种|交割期|买卖|投保|成交价|手数|成交额|开平|手续费|平仓盈亏|成交序号".Replace('|', '*'));
            settlelist.Add(string.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|", padRightEx("成交日期",10), padRightEx("交易所",8), padRightEx("品种",20), padRightEx("交割期",8), padRightEx("买卖",4), padRightEx("投保",4), padRightEx("成交价",8), padRightEx("手数",4), padRightEx("成交额",10), padRightEx("开平",4), padRightEx("手续费",8), padRightEx("平仓盈亏",10), padRightEx("成交序号",10)).Replace('|', '*'));
            foreach (Trade t in ORM.MTradingInfo.SelectHistTrades(s.Account, s.SettleDay, s.SettleDay))
            {

                settlelist.Add(string.Format(" {0} {1} {2} {3} {4} {5} {6,8:F2} {7,4} {8,10:F2} {9} {10,6:F2} {11,10:F2} {12,10}", t.xdate.ToString().PadRight(10), padRightEx(BasicTracker.ExchagneTracker.GetExchangeTitle(t.Exchange), 8), padRightEx(BasicTracker.SecurityTracker.GetSecurityName(t.SecurityCode), 20), "201415".PadRight(8), padRightEx((t.xsize > 0 ? "买" : " 卖"), 4), padRightEx("投", 4), t.xprice, Math.Abs(t.xsize), BasicTracker.SecurityTracker.GetMultiple(t.SecurityCode)*t.xprice*Math.Abs(t.xsize), padRightEx(GetCombFlag(t.PositionOperation), 4), t.Commission, t.Profit,10101));
            }

            settlelist.Add(NewLine);
            settlelist.Add(NewLine);
            settlelist.Add(SectionName("持仓汇总"));
            settlelist.Add(line);

            settlelist.Add("|   合约   |买持| 买均价 |卖持| 卖均价 | 昨结算 | 今结算 |盯市盈亏|保证金占用|投保".Replace('|', '*'));
            foreach (SettlePosition pos in positions)
            {
                settlelist.Add(string.Format(" {0,-10} {1,4} {2,8:F2} {3,4} {4,8:F2} {5,8:F2} {6,8:F2} {7,8:F2} {8,10:F2} {9,4}", pos.Symbol, pos.Size > 0 ? pos.Size : 0, pos.Size > 0 ? pos.AVGPrice : 0, pos.Size < 0 ? pos.Size : 0, pos.Size < 0 ? pos.AVGPrice : 0,0,pos.SettlePrice,pos.Size*(pos.SettlePrice-pos.AVGPrice)*pos.Multiple,Math.Abs(pos.AVGPrice*pos.Size*pos.Multiple*0.1M),"投"));
            }
            settlelist.Add(NewLine);
            settlelist.Add(NewLine);
            settlelist.Add("对结算单如有疑问，请于下一加以日上午11:00以前到结算部查询，过期责任自负！！！");


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


        static string GetCombFlag(QSEnumPosOperation op)
        {
            switch (op)
            { 
                case QSEnumPosOperation.AddPosition:
                case QSEnumPosOperation.EntryPosition:
                    return "开";
                case QSEnumPosOperation.DelPosition:
                case QSEnumPosOperation.ExitPosition:
                    return "平";
                default:
                    return "";
            }
        }
    }
}
