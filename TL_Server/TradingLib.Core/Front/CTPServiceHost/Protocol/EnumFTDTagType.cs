using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTPService
{
    public enum EnumFTDTagType
    {
        /// <summary>
        /// 丢弃不处理该标志
        /// 0
        /// </summary>
        FTDTagNone = 0, 
        /// <summary>
        /// 时间戳
        /// 4
        /// </summary>
        FTDTagDatetime,
        /// <summary>
        /// 信息压缩方法
        /// 1
        /// </summary>
        FTDTagCompressMethod,
        /// <summary>
        /// 发送端状态
        /// 1
        /// </summary>
        FTDTagSessionState,
        /// <summary>
        /// 交易日状态
        /// 4
        /// </summary>
        FTDTagTradedate,

        /// <summary>
        /// 心跳
        /// 0
        /// </summary>
        FTDTagKeepAlive,

        /// <summary>
        /// 
        /// </summary>
        FTDTagTarget,

        /// <summary>
        /// 建立Socket连接后的第一个初始化数据包
        /// </summary>
        FTDTagRegister=7,

        /// <summary>
        /// 未知
        /// </summary>
        FTDTagUnknown,
    }
}
