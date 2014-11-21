using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Mixins.LitJson;

namespace TradingLib.Mixins
{
    /// <summary>
    /// 系统产生的一个json格式的回报
    /// Result 0标识正常 1标识错误
    /// Code 系统内部定义的Reply类别码
    /// Message 为错误信息
    /// </summary>
    public class JsonReply
    {
        public int Code;
        public string Message;
        public object Playload;

        public JsonReply()
        {
            Code = 0;
            Message = string.Empty;
            Playload = null;
        }
        public JsonReply(int code, string message,object playload)
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

        /// <summary>
        /// 生成一个错误回报
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonReply GenericError(int code=1, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "Error:" + code;
            }
            return new JsonReply(code, message,null);
        }

        /// <summary>
        /// 生成一个成功回报
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonReply GenericSuccess(int code=0, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "Success:" + code;
            }
            return new JsonReply(code, message,null);
        }

        /// <summary>
        /// 从返回的json字符串中获得playload对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonreply"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static T ParsePlayload<T>(JsonData data,string field="Playload")
        { 
            return TradingLib.Mixins.LitJson.JsonMapper.ToObject<T>(data[field].ToJson());
        }

        /// <summary>
        /// 从返回的json字符串解析成JsonData
        /// </summary>
        /// <param name="jsonreply"></param>
        /// <returns></returns>
        public static JsonData ParseJsonReplyData(string jsonreply)
        {
            return TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonreply);
        }
    }

    /// <summary>
    /// 封装了基本的json对象拼接操作,实现链式操作
    /// </summary>
    public class ReplyWriter : JsonWriter
    {

        public ReplyWriter()
        { 
            
        }

        /// <summary>
        /// 开始Json写入头
        /// </summary>
        /// <returns></returns>
        public ReplyWriter Start()
        {
            this.WriteObjectStart();
            return this;
        }

        /// <summary>
        /// 结束Json写入尾
        /// </summary>
        /// <returns></returns>
        public ReplyWriter End()
        {
            this.WriteObjectEnd();
            return this;
        }

        /// <summary>
        /// 填充jsonreply
        /// </summary>
        /// <param name="reply"></param>
        /// <returns></returns>
        public ReplyWriter FillReply(JsonReply reply)
        { 
            this.WritePropertyName("Code");
            this.Write(reply.Code);
            this.WritePropertyName("Message");
            this.Write(reply.Message);

            return this;
        }

        ///// <summary>
        ///// 填充jsonreply
        ///// </summary>
        ///// <param name="reply"></param>
        ///// <returns></returns>
        //public ReplyWriter FillReply(JsonData jsondata)
        //{
        //    foreach (string key in jsondata.Keys)
        //    {
        //        this.WritePropertyName(key);
        //        this.Write(jsondata[key].ToString());
        //    }
        //    return this;
        //}

        /// <summary>
        /// 以字段objname将obj写入对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objname"></param>
        /// <returns></returns>
        public ReplyWriter Fill(object obj,string objname)
        {
            this.WritePropertyName(objname);
            JsonMapper.ToJson(obj, this);
            return this;
        }

        /// <summary>
        /// 以字段objname将obj数组写入对象
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="objsname"></param>
        /// <returns></returns>
        public ReplyWriter Fill(object[] objs, string objsname)
        {
            this.WritePropertyName(objsname);
            this.WriteArrayStart();
            foreach (object obj in objs)
            {
                JsonMapper.ToJson(obj, this);
            }
            this.WriteArrayEnd();
            return this;
        }

        /// <summary>
        /// 将一个对象写入playload字段
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ReplyWriter FillPlayload(object obj)
        {
            return Fill(obj, "Playload");
        }

        /// <summary>
        /// 将一个对象列表写入playload字段
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public ReplyWriter FillPlayload(object[] objs)
        {
            return Fill(objs, "Playlod");
        }




    }

}
