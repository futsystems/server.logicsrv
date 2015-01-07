using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.ServiceManager
{
    /// <summary>
    /// 扩展模块管理器
    /// </summary>
    public class ContribManager : BaseSrvObject, IServiceManager,IDisposable
    {
        const string SMGName = "ContribManager";
        ConcurrentDictionary<string, IContrib> contribmap = new ConcurrentDictionary<string, IContrib>();

        public string ServiceMgrName { get { return SMGName; } }
        public ContribManager()
            : base(SMGName)
        { 
            
        }
        /// <summary>
        /// 模块初始化
        /// </summary>
        public void Init()
        {
            Util.InitStatus(this.PROGRAME, true);
            //1.从配置文件中加载扩展模块列表
            List<string> contribList = GetContribList();

            //2.从扩展模块列表中加载扩展模块
            foreach (string contribName in contribList)
            {
                InitContrib(contribName);
            }
        }

        void InitContrib(string className)
        {
            IContribPlugin plugin = PluginHelper.LoadContribPlugin(className);
            if (plugin == null)
            {
                TLCtxHelper.Ctx.debug("扩展模块:" + className + "不存在,请提供正确的扩展模块名称");
                return;
            }
            if (contribmap.Keys.Contains(className))
            {
                Util.Debug("扩展模块:" + className + "已经加载,请勿重复加载");
                return;
            }

            IContrib contrib = PluginHelper.ConstructContrib(plugin.ContribClassName);
            if (contrib == null)
            {
                TLCtxHelper.Ctx.debug("扩展模块:" + className + "加载失败,请查询相关日志");
                return;
            }
            contribmap.TryAdd(className, contrib);
        }

        /// <summary>
        /// 模块加载
        /// </summary>
        public void Load()
        {
            Util.LoadStatus(this.PROGRAME, true);
            foreach (string key in contribmap.Keys)
            {
                debug("[LOAD CONTRIB] " + key,QSEnumDebugLevel.INFO);
                try
                {
                    contribmap[key].OnLoad();
                }
                catch (Exception ex)
                {
                    debug("load:" + key + "error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                }
            }
        }
        /// <summary>
        /// 模块销毁
        /// </summary>
        public void Destory()
        {
            Util.DestoryStatus(this.PROGRAME, true);
            foreach (string key in contribmap.Keys)
            {
                debug("[RELEASE CONTRIB] " + key, QSEnumDebugLevel.INFO);
                try
                {
                    contribmap[key].OnDestory();
                }
                catch (Exception ex)
                {
                    debug("release:" + key + "error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                }
            }
        }

        /// <summary>
        /// 模块启动
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME, true);
            foreach (string key in contribmap.Keys)
            {
                debug(string.Format("[START CONTRIB] {0}", key), QSEnumDebugLevel.INFO);
                try
                {
                    contribmap[key].Start();
                }
                catch (Exception ex)
                {
                    Util.Debug("start:" + key + "error:" + ex.ToString());
                }
            
            }
        }

        /// <summary>
        /// 模块停止
        /// </summary>
        public void Stop()
        {
            Util.StopStatus(this.PROGRAME, true);
            foreach (string key in contribmap.Keys)
            {
                debug(string.Format("[STOP CONTRIB] {0}", key), QSEnumDebugLevel.INFO);
                try
                {
                    contribmap[key].Stop();
                }
                catch (Exception ex)
                {
                    Util.Debug("stop:" + key + "error:" + ex.ToString());
                }
            }
        }

        private static List<string> GetContribList()
        {
            List<string> list = new List<string>();
            try
            {                     
                string fn = Util.GetConfigFile("contriblist.cfg");
                //实例化一个文件流--->与写入文件相关联  
                using (FileStream fs = new FileStream(fn, FileMode.Open))
                {
                    //实例化一个StreamWriter-->与fs相关联  
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        while (sw.Peek() > 0)
                        {
                            string str = sw.ReadLine();
                            if (string.IsNullOrEmpty(str) || str.StartsWith(";"))
                            {
                                continue;
                            }
                            list.Add(str);
                        }
                        sw.Close();
                    }
                    fs.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }
    }
}
