using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer;
using DotLiquid;


namespace TradingLib.Contrib.RechargeOnLine
{
    public static class ResponseHelper
    {

        public static Dictionary<string, object> GenErrorMessage(string message,int id=404)
        {
            Dictionary<string, object> errormessage = new Dictionary<string, object>();
            errormessage.Add("message", message);
            errormessage.Add("id", id);
            return errormessage;
        }


        public static void PageTemplate(this IResponse response, string template, Dictionary<string, object> dict)
        {
            string body = string.Empty;
            bool renderret = GWGlobals.TemplateHelper.Render(template, dict, out body);
            //模板渲染成功
            if (renderret)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(body);
                response.Body.Write(buffer, 0, buffer.Length);
            }
            else
            {
                PageError(response, "服务端维护,请稍后再试!");
            }
        }

        public static void PageTemplate(this IResponse response, string template, Drop drop)
        {
            string body = string.Empty;
            bool renderret = GWGlobals.TemplateHelper.Render(template, drop, out body);
            //模板渲染成功
            if (renderret)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(body);
                response.Body.Write(buffer, 0, buffer.Length);
            }
            else
            {
                PageError(response, "服务端维护,请稍后再试!");
            }
        }

        public static void PageError(this IResponse response ,string message)
        {
            string body = string.Empty;
            bool renderret = GWGlobals.TemplateHelper.Render("error", GenErrorMessage(message), out body);
            if (renderret)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(body);
                response.Body.Write(buffer, 0, buffer.Length);
            }
            else
            {
                //显示静态错误页面
                response.Redirect("/static_error.html");
            }
        }
    }
}
