using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common.Report
{
    /*
    public class ReportWLLSDis
    {

        
    }


    /// <summary>
    /// 记录某个账户交易信息多空盈亏,多头盈亏比例 空头盈亏分布  盈利多空分布  亏损多空分布
    /// </summary>
    public class WLSideDIS
    {
        public string Account { get; set; }
        //用于计算多单与空单的 胜率 多单胜率 空单胜率
        public int Long_Win_Num { get; set; }//多头盈利次数
        public int Long_Loss_Num { get; set; }//多头亏损次数
        public int Short_Win_Num { get; set; }//空头盈利次数
        public int Short_Loss_Num { get; set; }//空头亏损次数

        //用于计算盈利和亏损的多空占比(金额)
        public double Loss_Short_Value { get; set; }//空单损失金额
        public double Loss_Long_Value { get; set; }//多单损失金额
        public double Win_Short_Value { get; set; }//空单盈利金额
        public double Win_Long_Value { get; set; }//多单盈利金额


        public override string ToString()
        {
            string d = ",";
            string s = string.Empty;
            s += Long_Win_Num.ToString() + d;
            s += Long_Loss_Num.ToString() + d;
            s += Short_Win_Num.ToString() + d;
            s += Short_Loss_Num.ToString() + d;
            s += Win_Long_Value.ToString() + d;
            s += Win_Short_Value.ToString() + d;
            s += Loss_Long_Value.ToString() + d;
            s += Loss_Short_Value.ToString() + d;
            return s;
        }

        public static WLSideDIS FromString(string msg)
        {
            string[] p = msg.Split(',');
            WLSideDIS t = new WLSideDIS();
            t.Long_Win_Num = int.Parse(p[0]);
            t.Long_Loss_Num = int.Parse(p[1]);
            t.Short_Win_Num = int.Parse(p[2]);
            t.Short_Loss_Num = int.Parse(p[3]);

            t.Win_Long_Value = double.Parse(p[4]);
            t.Win_Short_Value = double.Parse(p[5]);
            t.Loss_Long_Value = double.Parse(p[6]);
            t.Loss_Short_Value = double.Parse(p[7]);
            return t;
        }
    }***/
}
