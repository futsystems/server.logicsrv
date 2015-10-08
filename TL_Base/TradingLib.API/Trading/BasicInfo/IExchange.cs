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
        /// 交易所对应的时区信息
        /// </summary>
        string TimeZone { get; set; }


        /// <summary>
        /// 品种对象绑定交易所对象，判断品种是否可以交易 需要获得交易所时间，交易所时间需要用交易所时区对象将时间进行转换
        /// </summary>
        TimeZoneInfo TimeZoneInfo { get; }

        /// <summary>
        /// 收盘时间
        /// </summary>
        int CloseTime { get; set; }
        /// <summary>
        /// 交易日历
        /// </summary>
        string Calendar { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        string Title { get; set; }
        
    }
}
