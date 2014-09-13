using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.API
{
    public interface IExchange
    {
        /// <summary>
        /// 交易所代码
        /// </summary>
        string EXCode { get; set; }
        /// <summary>
        /// 交易所名称
        /// </summary>
        string Name {get;set;}
        /// <summary>
        /// 所在国家
        /// </summary>
        Country Country {get;set; }
        /// <summary>
        /// 交易所编号 
        /// 国家+交易所代码
        /// </summary>
        string Index { get; }

        /// <summary>
        /// 简称
        /// </summary>
        string Title { get; set; }
        
    }
}
