using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class DefaultSymSetting
    {

        string _symcode = "";
        decimal _profit = 0;
        decimal _loss = 0;
        int _size = 0;

        public string SymCode { get { return _symcode; } set { _symcode = value; } }
        public decimal Profit { get { return _profit; } set { _profit = value; } }
        public decimal Loss { get { return _loss; } set { _loss = value; } }

        public int Size { get { return _size; } set { _size = value; } }

        /// <summary>
        /// 从配置文件加载对象
        /// </summary>
        /// <param name="symcode"></param>
        /// <param name="loss"></param>
        /// <param name="profit"></param>
        /// <param name="size"></param>
        public DefaultSymSetting(string symcode, string loss, string profit, string size)
        {
            _symcode = symcode;

            decimal d=0;
            int i=0;

            if(Decimal.TryParse(loss, out d))
                _loss=d;
            if (Decimal.TryParse(profit, out d))
                _profit = d;
            if (Int32.TryParse(size, out i))
                _size = i;
        }


    }
}
