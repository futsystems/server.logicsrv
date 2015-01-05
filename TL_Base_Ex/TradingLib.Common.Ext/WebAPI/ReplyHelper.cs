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


//namespace TradingLib.Common
//{
//    /// <summary>
//    /// 系统产生的一个json格式的回报
//    /// Result 0标识正常 1标识错误
//    /// Code 系统内部定义的Reply类别码
//    /// Message 为错误信息
//    /// </summary>
//    public class JsonReply
//    {
//        public int Result;
//        public ReplyType Code;
//        public string Message;

//        public JsonReply(int result, ReplyType code, string message)
//        {
//            Result = result;
//            Code = code;
//            Message = message;
//        }

//        /// <summary>
//        /// 将jsonreply 序列化成 json字符串
//        /// </summary>
//        /// <returns></returns>
//        public string ToJson()
//        {
//            return JsonMapper.ToJson(this);
//        }

//        /// <summary>
//        /// 生成一个错误回报
//        /// </summary>
//        /// <param name="code"></param>
//        /// <param name="message"></param>
//        /// <returns></returns>
//        public static JsonReply GenericError(ReplyType code, string message = "")
//        {
//            if(string.IsNullOrEmpty(message))
//            {
//                message = Util.GetEnumDescription(code);
//            }
//            return new JsonReply(1, code, message);
//        }

//        /// <summary>
//        /// 生成一个成功回报
//        /// </summary>
//        /// <param name="code"></param>
//        /// <param name="message"></param>
//        /// <returns></returns>
//        public static JsonReply GenericSuccess(ReplyType code, string message = "")
//        {
//            if (string.IsNullOrEmpty(message))
//            {
//                message = Util.GetEnumDescription(code);
//            }
//            return new JsonReply(0, code, message);
//        }
//    }





//    public class ReplyHelper
//    {
//        /// <summary>
//        /// 无法找到对应的模块
//        /// </summary>
//        public static string Error_ModuleNotFund = JsonReply.GenericError(ReplyType.ModuleNotFund).ToJson();
//        /// <summary>
//        /// 无法找到对应的方法
//        /// </summary>
//        public static string Error_MethodNotFund = JsonReply.GenericError(ReplyType.MethodNotFund).ToJson();

//        /// <summary>
//        /// 服务端Command执行错误
//        /// </summary>
//        public static string Error_CommandExecute = JsonReply.GenericError(ReplyType.CommandExecuteError).ToJson();

//        /// <summary>
//        /// 服务端执行异常
//        /// </summary>
//        public static string Error_ServerSide = JsonReply.GenericError(ReplyType.ServerSideError).ToJson();

//        /// <summary>
//        /// WebMessage处理器未绑定
//        /// </summary>
//        public static string Error_WebMsgHandlerNotBind = JsonReply.GenericError(ReplyType.WebMsgHandlerNotBind).ToJson();

//        /// <summary>
//        /// 查询的交易帐户不存在
//        /// </summary>
//        public static string Error_AccountNotFound = JsonReply.GenericError(ReplyType.AccountNotFound).ToJson();

//        /// <summary>
//        /// 服务端执行正常
//        /// </summary>
//        public static string Success_Generic = JsonReply.GenericSuccess(ReplyType.Success).ToJson();

//        /// <summary>
//        /// 指定的websock connection uuid不存在
//        /// </summary>
//        public static string Error_WebSockUUIDNotFound = JsonReply.GenericError(ReplyType.WebSockUUIDNotFound).ToJson();

//        /*
//       public static string Reply2Json(object obj)
//       {
//           return JsonMapper.ToJson(obj);
//       }

       
//       public static JsonData GenericErrorJ(ReplyType code, string message = "")
//       {
//           //return JsonData GenericError(code, message);
//           JsonData d = new JsonData();
//           d["Result"] = 1;
//           d["Code"] = int(code);
//           d["Message"] = string.IsNullOrEmpty(message)? LibUtil.GetEnumDescription(code):message;
//           return d;
//       }



//       public static JsonData GenericSuccessJ(ReplyType code, string message = "")
//       {
//           //return JsonData GenericError(code, message);
//           JsonData d = new JsonData();
//           d["Result"] = 1;
//           d["Code"] = int(code);
//           d["Message"] = string.IsNullOrEmpty(message)? LibUtil.GetEnumDescription(code):message;
//           return d;
//       }
//        * **/

//        public static string ReplyObject(object obj)
//        {
//            JsonWriter w = NewJWriterWithReply(JsonReply.GenericSuccess(ReplyType.Success));
//            FillJWriter(obj, w);
//            EndWriter(w);

//            return w.ToString();
//        }


//        /// <summary>
//        /// 将对象放在属性name下，形成对象嵌套
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="obj"></param>
//        /// <param name="writer"></param>
//        public static void FillJWriter(object obj, JsonWriter writer,string name="Playload")
//        {
//            writer.WritePropertyName(name);
//            JsonMapper.ToJson(obj, writer);
//        }


//        /// <summary>
//        /// 将一个对象数组以name为属性,填充到json字符串中
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="objs"></param>
//        /// <param name="writer"></param>
//        public static void FillJWriter(object[] objs, JsonWriter writer, string name = "Playload")
//        {
//            writer.WritePropertyName(name);
//            writer.WriteArrayStart();
//            foreach (object obj in objs)
//            {
//                JsonMapper.ToJson(obj, writer);
//            }
//            writer.WriteArrayEnd();
//        }

//        /// <summary>
//        /// 将reply 填充到json首层
//        /// </summary>
//        /// <param name="reply"></param>
//        /// <param name="writer"></param>
//        public static void FillJWriter(JsonReply reply, JsonWriter writer)
//        {
//            writer.WritePropertyName("Result");
//            writer.Write(reply.Result);
//            writer.WritePropertyName("Code");
//            writer.Write((int)reply.Code);
//            writer.WritePropertyName("Message");
//            writer.Write(reply.Message);
            
//        }

//        /// <summary>
//        /// 将ISession中的module cmdstr填充到json中
//        /// </summary>
//        /// <param name="session"></param>
//        /// <param name="writer"></param>
//        public static void FillJWriter(ISession session, JsonWriter writer)
//        {
//            writer.WritePropertyName("ContribID");
//            writer.Write(session.ContirbID);
//            writer.WritePropertyName("CMD");
//            writer.Write(session.CMDStr);
//        }



        
//        /// <summary>
//        /// 生成一个jsonwriter 并写入InfoType字段
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        static JsonWriter NewJWriterInfo(InfoType type)
//        {
//            JsonWriter w = NewJWriter();
//            w.WritePropertyName("InfoType");
//            w.Write(type.ToString());
//            return w;
//        }

//        /// <summary>
//        /// 生成基本的InfoJson {InfoType,Playload}
//        /// 并不结束jsonwriter, 还可以填充其他对象
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public static JsonWriter NewJWriterInfo(InfoType type, object obj)
//        {
//            JsonWriter w = NewJWriterInfo(type);
//            FillJWriter(obj, w);
//            //EndWriter(w);
//            return w;
//        }

//        /// <summary>
//        /// 生成websock 请求消息，用于推送到特定的websock 
//        /// </summary>
//        /// <param name="uuid"></param>
//        /// <param name="type"></param>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public static JsonWriter NewJWriterWebSockTopic(string uuid, InfoType type, object obj)
//        {
//            JsonWriter w = NewJWriterInfo(InfoType.WebSockTopic);
//            w.WritePropertyName("UUID");
//            w.Write(uuid);
//            w.WritePropertyName("SubType");
//            w.Write(type.ToString());
//            FillJWriter(obj, w);
//            return w;
//        }
//        /// <summary>
//        /// 生成某个类型的Info jsonstring
//        /// 
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public static string InfoObject(InfoType type, object obj)
//        {
//            JsonWriter w = NewJWriterInfo(type, obj);
//            EndWriter(w);
//            return w.ToString();
//        }

//        public static string InfoObjectWebSockTopic(string uuid, InfoType type, object obj)
//        {
//            JsonWriter w = NewJWriterWebSockTopic(uuid, type, obj);
//            EndWriter(w);
//            return w.ToString();
//        }

//        public static JsonWriter NewJWriterSuccess()
//        {
//            return NewJWriterWithReply(JsonReply.GenericSuccess(ReplyType.Success));
//        }

//        public static JsonWriter NewJWriterError()
//        {
//            return NewJWriterWithReply(JsonReply.GenericError(ReplyType.Error));
//        }

//        /// <summary>
//        /// 生成一个jsonwriter 同时用reply进行填充
//        /// </summary>
//        /// <param name="reply"></param>
//        /// <returns></returns>
//        public static JsonWriter NewJWriterWithReply(JsonReply reply)
//        {
//            JsonWriter w = NewJWriter();
//            FillJWriter(reply, w);
//            return w;
//        }

//        /// <summary>
//        /// 返回一个包含了session的
//        /// </summary>
//        /// <param name="session"></param>
//        /// <returns></returns>
//        public static JsonWriter NewJWriterWithSession(ISession session)
//        {
//            JsonWriter w = NewJWriter();
//            FillJWriter(session, w);
//            return w;
//        }

        


//        public static JsonWriter NewJWriter()
//        {
//            JsonWriter w = new JsonWriter();
//            w.WriteObjectStart();
//            return w;
//        }

//        public static void EndWriter(JsonWriter w)
//        {
//            w.WriteObjectEnd();
            
//        }

        

        
//    }
//}
