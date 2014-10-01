using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{
    public interface IBarDataRetrieval
    {
        // Methods
        IQService GetService();
        /// <summary>
        /// 获得一个时间段的历史数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="frequency"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        void RetrieveData(Security symbol,BarFrequency freq, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 获得历史数据回报事件
        /// </summary>
        event BarDelegate GotHistBarEvent;
    }
}
