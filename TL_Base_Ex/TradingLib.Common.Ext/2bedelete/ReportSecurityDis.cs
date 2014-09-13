using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common.Report
{
    /*
    /// <summary>
    /// 新建一个类 继承自dictionary 映射了SecurityCode -> 该交易品种的 交易统计信息
    /// </summary>
    public class SecurityDisMap : Dictionary<string,SecurityDis>
    {
        string Account { get; set; }//信息所属交易账户
        public override string ToString()
        {
            string s = string.Empty;
            foreach (string key in this.Keys)
            {
               s +="$"+ key+":"+this[key].ToString();//单条记录用$分割,记录内 key:记录信息
            }

            return Account + "#" + s;
        }

        public static SecurityDisMap FromString(string msg)
        {
            SecurityDisMap map = new SecurityDisMap();
            string[] p = msg.Split('#');//分割得到#
            string account = p[0];
            map.Account = account;
            string[] v = p[1].Split('$');//分割成品种信息的单条记录
            foreach (string temp in v)
            {
                string[] t = temp.Split(':');
                string seccode = t[0];
                if (map.Keys.Contains(seccode)) continue;
                map.Add(seccode, SecurityDis.FromString(t[1]));
                
            }
            return map;
        }
        
    }



    /// <summary>
    /// 以品种分类得到基本信息
    /// </summary>
    public class SecurityDis
    {
        public string SecurityCode { get; set; }//品种代码
        public int NumTrade { get; set; }//交易次数
        public int NumLong { get; set; }//多头次数
        public int NumLongWin { get; set; }//多头盈利次数
        public int NumShort { get; set; }//空头次数
        public int NumShortWin { get; set; }//空头盈利次数


        public double SumWinValue { get; set; }//累计盈利
        public double SumLongWinValue { get; set; }//累计多单盈利
        public double SumLossValue { get; set; }//累计亏损
        public double SumLongLossValue { get; set; }//累计多单亏损
        public double SumCommssion { get; set; }//累计手续费



        public override string ToString()
        {
            string d = ",";
            string s = string.Empty;
            s += SecurityCode + d;
            s+= NumTrade.ToString()+d;
            s+= NumLong.ToString()+d;
            s+= NumLongWin.ToString() +d;
            s+= NumShort.ToString()+d;
            s+= NumShortWin.ToString()+d;

            s += SumWinValue.ToString() + d;
            s += SumLongWinValue.ToString() + d;
            s += SumLossValue.ToString() + d;
            s += SumLongLossValue.ToString() + d;
            s += SumCommssion.ToString() + d;

            return s;
        }

        public static SecurityDis FromString(string msg)
        {
            SecurityDis temp = new SecurityDis();
            return temp;
        }

    }***/
}
