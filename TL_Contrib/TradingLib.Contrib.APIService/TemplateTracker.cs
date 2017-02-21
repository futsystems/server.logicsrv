using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DotLiquid;
using Common.Logging;

namespace TradingLib.Contrib.APIService
{
    public class TemplateTracker
    {
        static ILog logger = LogManager.GetLogger("TemplateTracker");
        string _templateDirectory = string.Empty;
        string _suffix = "html";
        Dictionary<string, Template> templatemap = new Dictionary<string, Template>();

        /// <summary>
        /// 在某个目录下 创建一个模板维护器 加载以suffix为后缀的文件
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static TemplateTracker CreateTemplateTracker(string templatePath, string suffix="html")
        {
            return new TemplateTracker(templatePath, suffix);
        }

        /// <summary>
        /// 返回所有模板键
        /// </summary>
        public IEnumerable<string> TemplateKeys
        {
            get
            {
                return templatemap.Keys.ToArray();
            }
        }
        /// <summary>
        /// 从某个模板目录 加载suffix为扩展文件名的模板
        /// 形成key - template 缓存
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="suffix"></param>
        public TemplateTracker(string templatePath, string suffix)
        {
            _templateDirectory = templatePath;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("template path:" + _templateDirectory);
            Console.ForegroundColor = ConsoleColor.White;
            _suffix = suffix;
            LoadALLTemplate(_templateDirectory);
        }

        /// <summary>
        /// 用字典渲染模板
        /// 渲染成功返回true 渲染失败返回false
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dict"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool Render(string key, Dictionary<string, object> dict, out string body)
        {
            Template template = this[key];
            body = string.Empty;
            try
            {
                if (template == null)
                    return false;
                body = template.Render(Hash.FromDictionary(dict));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 从Drop对象渲染模板
        /// 渲染成功返回true 渲染失败返回false
        /// </summary>
        /// <param name="template"></param>
        /// <param name="obj"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public string Render(string key, Drop obj)
        {
            Template template = this[key.ToUpper()];
            try
            {
                if (template == null)
                    return string.Format("Template:{0} Not Exist", key);
                return template.Render(Hash.FromAnonymousObject(new { data = obj }));
            }
            catch (Exception ex)
            {
                logger.Error("Render Template Error:{0}" + ex.ToString());
                return string.Format("Render Template:{0} Error:{1}", key, ex.ToString());
            }
        }
        /// <summary>
        /// 返回某个key的template
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Template this[string key]
        {
            get
            {
                string tkey = key.ToUpper();
                Template tpl = null;
                if (templatemap.TryGetValue(tkey, out tpl))
                {
                    return tpl;
                }
                return null;
            }
        }


        /// <summary>
        /// 加载某个目录下的所有模板
        /// </summary>
        /// <param name="path"></param>
        private void LoadALLTemplate(string path)
        {
            if (Directory.Exists(path))
            {
                //文件夹及子文件夹下的所有文件的全路径
                string[] files = Directory.GetFiles(path, "*." + _suffix, SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    templatemap.Add(Path.GetFileNameWithoutExtension(file).ToUpper(), LoadTempalte(file));
                }
            }
        }

        /// <summary>
        /// 从某个文件生成一个template
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        private Template LoadTempalte(string fn)
        {
            try
            {
                return Template.Parse(File.ReadAllText(fn, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}