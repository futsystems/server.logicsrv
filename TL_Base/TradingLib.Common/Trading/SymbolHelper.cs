using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class SymbolHelper
    {
        /// <summary>
        /// IF1301通过解析symol来得到对应的合约品种代码
        /// 数字之前的所有字母为SecurityCode品种代码
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string genSecurityCode(string symbol)
        {
            string code = string.Empty;
            Char[] p = symbol.ToCharArray();
            for (int i = 0; i < symbol.Length; i++)
            {
                int tempChar = -1;
                try
                {
                     tempChar = int.Parse(p[i].ToString());
                }
                catch (Exception e)
                {
                    tempChar = -1;
                }
                //MessageBox.Show("tempChar:" + tempChar.ToString() +" |"+p[i].ToString());
                if (tempChar >= 0 && tempChar <= 9)
                {
                    return code;
                }
                else
                {
                    code += (char)p[i];
                }
            }
            return code;
        }
        //针对不同的交易所返回月份代码 每个交易所的月份代码有所不同
        public static string genExpireCode(SecurityFamily sec, int monthcode)
        {
            //DateTime dt = Util.FT2DT(fastdate);

            switch (sec.Exchange.Index)
            { 
                case "CN_CZCE":
                    return (monthcode-201000).ToString();
                case "CN_CFFEX":
                    return (monthcode-200000).ToString();
                case "CN_SHFE":
                    return (monthcode-200000).ToString();
                case "CN_DCE":
                    return (monthcode - 200000).ToString();
                case "CN_SHFE_24H":
                    return (monthcode - 200000).ToString();
                default:
                    return monthcode.ToString();

            }

        }
    }
}
