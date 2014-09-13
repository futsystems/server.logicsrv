using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.TNetStrings;

namespace TradingLib.Mixins
{
    public class JsonRequest
    {
        public string Module;
        public string Method;
        public string Args;
        public string Token;

        public JsonRequest(string method, string module = "main", string args="",string token = "")
        {
            Module = module;
            Method = method;
            Args = args;
            Token = token;
        }

        /// <summary>
        /// 将jsonrequest对象转换成json string
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public  static string ToJson(JsonRequest request)
        {
            return JsonMapper.ToJson(request);
        }
        /// <summary>
        /// 生成json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonRequest.ToJson(this);
        }
        /// <summary>
        /// 将json string解析成jsondata
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static JsonData ToObject(string request)
        {
            return JsonMapper.ToObject(request);
        }

        public static JsonRequest Request(string method="", string module = "main",string token = "")
        {
            return new JsonRequest(method, module,null, token);
        }

        /// <summary>
        /// 设定方法名
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public JsonRequest SetMthod(string method)
        {
            this.Method = method;
            return this;
        }
        /// <summary>
        /// 设定模块名
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public JsonRequest SetModule(string module)
        {
            this.Module = module;
            return this;
        }

        /// <summary>
        /// 设定Token值
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public JsonRequest SetToken(string token)
        {
            this.Token = token;
            return this;
        }

        /// <summary>
        /// 将参数字符串传入 并通过Tnetstring打包
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public JsonRequest SetArgs(string[] args)
        { 
            string argstr=string.Empty;
            foreach(string arg in args)
            {
                argstr += Tnetstring.TDump(new TnetString(arg));
            }
            this.Args = argstr;
            return this;
        }

    }
}
