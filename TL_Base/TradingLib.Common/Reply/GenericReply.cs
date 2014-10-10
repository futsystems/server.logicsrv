using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class GenericReply : IReply
    {
        /// <summary>
        /// 结果标识0成功，1错误
        /// </summary>
        public int Result { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public ReplyType Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
