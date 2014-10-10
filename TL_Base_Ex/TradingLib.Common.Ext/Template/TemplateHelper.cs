using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using DotLiquid;


//namespace TradingLib.Common
//{
//    public class User : Drop
//    {
//        public string Name { get; set; }
//        public List<Task> Tasks { get; set; }
//    }

//    public class Task : Drop
//    {
//        public string Name { get; set; }
//    }


//    public class TemplateHelper
//    {


//        public static string RenderSettlementInfo(SettlementInfo info)
//        {
//            Template t = LoadTemplate("settlement");
//            if (t != null)
//            {
//                string content = TemplateRender(t, info);
//                //TLCtxHelper.Debug("content:" + content);
//                //TLCtxHelper.Debug(string.Format("account:{0} lastequity:{1} settleday:{2}", info.Account, info.LastEquity, info.SettleDay));
//                return content;
//            }
//            else
//            {
//                return "template not exist";
//            }
//        }


        
//        /// <summary>
//        /// 渲染模板
//        /// </summary>
//        /// <param name="template"></param>
//        /// <param name="dict"></param>
//        /// <returns></returns>
//        static string TemplateRender(Template template,object obj)
//        {
//            try
//            {
//                return template.Render(Hash.FromAnonymousObject(new { settleinfo = obj }));
//            }
//            catch (Exception ex)
//            {
//                TLCtxHelper.Debug("template render error:" + ex.ToString());
//                return "";
//            }
//        }


//        static string GetTemplateFile(string name)
//        {
//            string path = AppDomain.CurrentDomain.BaseDirectory;
//            string filepath = Path.Combine(new string[] { path, "config", "templates", name+".tpl" });
//            return filepath;
//        }

//        /// <summary>
//        /// 加载模板
//        /// </summary>
//        /// <param name="fn"></param>
//        /// <returns></returns>
//        static Template LoadTemplate(string name)
//        {
//            string filename = GetTemplateFile(name);
//            TLCtxHelper.Ctx.debug("template file name:" + filename);
//            if (File.Exists(filename))
//            {
//                return Template.Parse(File.ReadAllText(filename, Encoding.UTF8));
//            }
//            else
//            {
//                return null;
//            }
//        }
//    }
//}
