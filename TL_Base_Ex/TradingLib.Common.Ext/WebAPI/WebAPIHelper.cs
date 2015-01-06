using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.Json;

namespace TradingLib.Common
{
    public class WebAPIHelper
    {
        /// <summary>
        /// 返回操作成功回报
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonReply ReplySuccess(string message="Success")
        {
            return new JsonReply(0, message);
        }
        /// <summary>
        /// 用错误标识生成JsonReply
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static JsonReply ReplyError(string key)
        {
            return CreateJsonReply(RspInfoTracker.WebAPIRspInfo[key]);
        }

        /// <summary>
        /// 用错误代码生成JsonReply
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static JsonReply ReplyError(int code)
        {
            return CreateJsonReply(RspInfoTracker.WebAPIRspInfo[code]);
        }

        /// <summary>
        /// 将对象填充到playload中 生成JsonReply
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static JsonReply ReplyObject(object obj)
        {
            return new JsonReply(0, "", obj);
        }

        /// <summary>
        /// 通过XMLRspInfo生成对应的JsonReply
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static JsonReply CreateJsonReply(XMLRspInfo info)
        {
            return new JsonReply(info.Code, info.Message);
        }
    }
}
