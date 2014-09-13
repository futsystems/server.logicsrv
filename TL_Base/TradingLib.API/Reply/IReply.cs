using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IReply
    {
        /// <summary>
        /// 结果标识0成功，1错误
        /// </summary>
        int Result { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        ReplyType Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        string Message { get; set; }
    }
}
