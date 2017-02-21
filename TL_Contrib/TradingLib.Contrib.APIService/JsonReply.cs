using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class JsonReply
    {
        public int Code;
        public string Message;
        public object Payload;

        

        public JsonReply(int code, string message, object obj = null)
        {
            this.Code = code;
            this.Message = message;
            this.Payload = obj;
        }
    }

    ///// <summary>
    ///// 系统产生的一个json格式的回报
    ///// Result 0标识正常 1标识错误
    ///// Code 系统内部定义的Reply类别码
    ///// Message 为错误信息
    ///// </summary>
    //public class JsonReply<T>
    //{
    //    public int Code;
    //    public string Message;
    //    public T Payload;

    //    public JsonReply()
    //    {
    //        Code = 0;
    //        Message = string.Empty;
    //        Payload = default(T);
    //    }
    //    public JsonReply(int code, string message, T playload)
    //    {
    //        Code = code;
    //        Message = message;
    //        Payload = playload;
    //    }
    //}

}
