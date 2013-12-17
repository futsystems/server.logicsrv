using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Mixins.LitJson;

namespace TradingLib.Mixins
{
    public class JsonReply
    {
        public int Code;
        public string Message;
        public object Playload;

        public static JsonReply ErrorReply(int code = 1, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "Error:" + code.ToString();
            }
            return new JsonReply(code, message, null);
        }

        public static JsonReply SuccessReply(object playload = null,int code = 0, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "Success:" + code.ToString();
            }
            return new JsonReply(code, message, playload);
        }

        /// <summary>
        /// 将json解析成JsonReply<T>
        /// 可以快速获得Playload对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JsonReply<T> ParseReply<T>(string json)
        {
            return TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonReply<T>>(json);
        }

        public JsonReply(int code, string message, object obj=null)
        {
            this.Code = code;
            this.Message = message;
            this.Playload = obj;
        }

        public string ToJson()
        {
            return JsonMapper.ToJson(this);
        }
    }
    /// <summary>
    /// 系统产生的一个json格式的回报
    /// Result 0标识正常 1标识错误
    /// Code 系统内部定义的Reply类别码
    /// Message 为错误信息
    /// </summary>
    public class JsonReply<T>
    {
        public int Code;
        public string Message;
        public T Playload;

        public JsonReply()
        {
            Code = 0;
            Message = string.Empty;
            Playload = default(T);
        }
        public JsonReply(int code, string message, T playload)
        {
            Code = code;
            Message = message;
            Playload = playload;
        }

        /// <summary>
        /// 将jsonreply 序列化成 json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonMapper.ToJson(this);
        }
    }

}
