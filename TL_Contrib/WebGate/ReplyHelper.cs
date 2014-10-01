using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;

namespace WebGate
{
    public class ReplyHelper
    {

        static Dictionary<string, string> codemap;

        static void InitCodeMap()
        {
            codemap = new Dictionary<string, string>();
            codemap.Add("r100", "WebAPIServer do not bind");//函数调用格式错误
            codemap.Add("r101", "ServerSide Error");//服务端处理异常
            codemap.Add("r102", "Operation Error");//

            codemap.Add("r200", "Request  form error:Module|Function|arg1,arg2,arg3... or Function|arg1,arg2,arg3...");//函数调用格式错误
            codemap.Add("r201", "Function Arguments Error");//函数调用参数错误
            
            codemap.Add("r202", "Add Sim Account Error");//添加帐户异常

           
        }

        /// <summary>
        /// 服务端时间没有绑定
        /// </summary>
        public static string ER_APIHANDLE_NOTBIND = RepServerAPINotBind();
        
        /// <summary>
        /// 服务端处理异常错误
        /// </summary>
        public static string ER_SERVER_ERROR = RepServerError();

        /// <summary>
        /// 请求格式错误
        /// </summary>
        public static string ER_REQUEST_FORM_ERROR = RequestFormError();

        static Dictionary<string, string> CodeMap
        {
            get
            {
                if (codemap == null)
                    InitCodeMap();
                return codemap;
            }
        }

        public static string RepServerAPINotBind()
        {
            ErrorReply r = new ErrorReply("r100", CodeMap["r100"]);
            return JsonMapper.ToJson(r);
        }
        public static string RepServerError()
        {
            ErrorReply r = new ErrorReply("r101", CodeMap["r101"]);
            return JsonMapper.ToJson(r);
        }



        /// <summary>
        /// 请求格式错误
        /// </summary>
        /// <returns></returns>
        public static string RequestFormError()
        {
            ErrorReply r = new ErrorReply("r200", CodeMap["r200"]);
            return JsonMapper.ToJson(r);
        }
        /// <summary>
        /// 请求参数错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string RequestArgumentsError(string message="")
        {
            ErrorReply r = new ErrorReply("r201",string.IsNullOrEmpty(message)?CodeMap["r201"]:message);
            return JsonMapper.ToJson(r);
        }

        public static string AddSimAccountError(string message="")
        {
             ErrorReply r = new ErrorReply("r202",string.IsNullOrEmpty(message)?CodeMap["r202"]:message);
            return JsonMapper.ToJson(r);
        }

        public static string OperationError(string message = "")
        {
            ErrorReply r = new ErrorReply("r102", string.IsNullOrEmpty(message) ? CodeMap["r102"] : message);
            return JsonMapper.ToJson(r);
        }


        /// <summary>
        /// 返回操作成功标识,返回类型为JsonData,用于嵌入其他回报值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonData OKReplyJ(string message="Operate Success")
        {
            JsonData data = new JsonData();
            data["Result"] = 0;
            data["Message"] = message;
            return data;
        }
    }


    public struct OKReplay
    {
        public int Result;
        public string Message;

        public OKReplay(string message)
        {
            Result = 0;
            Message = message;
        }
    }


    /// <summary>
    /// 系统产生的一个错误汇报
    /// Result 0标识正常 1标识错误
    /// Code 系统内部定义的错误代码表
    /// Message 为错误信息
    /// </summary>
    public struct ErrorReply
    {
        public int Result;
        public string Code;
        public string Message;
        public ErrorReply(string code, string message)
        {
            Result = 1;
            Code = code;
            Message = message;
        }
    }
}
