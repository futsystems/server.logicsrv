using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using DotLiquid;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.RechargeOnLine
{
    public class TemplateHelper
    {
        string _httpDirectory = string.Empty;
        string _templateDirectory = string.Empty;
        Dictionary<string, Template> templatemap = new Dictionary<string, Template>();
        public TemplateHelper(string httpDirectory)
        {
            _httpDirectory = httpDirectory;
            string path = Util.GetResourceDirectory(_httpDirectory);
            _templateDirectory = Path.Combine(new string[] { path, "Templates" });
            Util.Debug("Template directory at:" + _templateDirectory);
            LoadALLTemplate(_templateDirectory);
        }

        /// <summary>
        /// 用字典渲染模板
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dict"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool Render(string key, Dictionary<string, object> dict,out string body)
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
                Util.Debug("template render error:" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 从对象渲染模板
        /// </summary>
        /// <param name="template"></param>
        /// <param name="obj"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool Render(string key, Drop obj, out string body)
        {
            Template template = this[key];
            body = string.Empty;
            try
            {
                if (template == null)
                    return false;
                body = template.Render(Hash.FromAnonymousObject(new { data = obj }));
                return true;
            }
            catch (Exception ex)
            {
                Util.Debug("template render error:" + ex.ToString());
                return false;
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
                if (templatemap.Keys.Contains(tkey))
                {
                    return templatemap[tkey];
                }
                return null;
            }
        }

        /// <summary>
        /// 加载某个目录下的所有模板
        /// </summary>
        /// <param name="path"></param>
        void LoadALLTemplate(string path)
        { 
            if (Directory.Exists(path))
             {
                
                //文件夹及子文件夹下的所有文件的全路径
                string[] files = Directory.GetFiles(path, "*.html", SearchOption.TopDirectoryOnly);
                foreach(string file in files)
                {
                    templatemap.Add(Path.GetFileNameWithoutExtension(file).ToUpper(), LoadTempalte(file));
                    Util.Debug("short file:" + Path.GetFileNameWithoutExtension(file));
                }
            }
        }
        /// <summary>
        /// 从某个文件生成一个template
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public Template LoadTempalte(string fn)
        {
            return Template.Parse(File.ReadAllText(fn, Encoding.UTF8));
        }
    }
}
