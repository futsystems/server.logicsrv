using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.DataFarm.Common
{
    public class DFStatistic
    {

        /// <summary>
        /// 实时行情发送次数
        /// </summary>
        public long TickSendCnt;

        /// <summary>
        /// 分时数据发送次数
        /// </summary>
        public long MinuteDataSendCnt;

        /// <summary>
        /// Bar数据发送次数
        /// </summary>
        public long BarDataSendCnt;

        /// <summary>
        /// 分比数据发送次数
        /// </summary>
        public long TradeSplitSendCnt;

        /// <summary>
        /// 
        /// </summary>
        public long PriceVolSendCnt;


        public long OtherPktSendCnt;
        /// <summary>
        /// 实时行情发送数据大小
        /// </summary>
        public long TickSendSize;

        /// <summary>
        /// 分时数据发送数据大小
        /// </summary>
        public long MinuteDataSendSize;

        /// <summary>
        /// Bar数据发送数据大小
        /// </summary>
        public long BarDataSendSize;

        /// <summary>
        /// 分笔数据发送大小
        /// </summary>
        public long TradeSplitSendSize;


        public long PriceVolSendSize;

        public long OtherPktSendSize;
    }
}
