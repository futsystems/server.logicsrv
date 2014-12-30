//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using TradingLib.API;


//namespace TradingLib.Common
//{
//    public class ConfigDB
//    {
//        string _module = "";

//        Dictionary<string, ConfigItem> configmap = new Dictionary<string, ConfigItem>();
//        public ConfigDB(string module = "demo")
//        {
//            _module = module;
//            //加载配置文件
//            ReloadConfig();
//        }

//        /// <summary>
//        /// 获得某个参数的配置对象
//        /// </summary>
//        /// <param name="cfgname"></param>
//        /// <returns></returns>
//        public ConfigItem this[string cfgname]
//        {
//            get
//            {
//                ConfigItem item = null;
//                if (this.configmap.TryGetValue(cfgname,out item))
//                {
//                    return item;
//                }
//                return null;
//            }
//        }

//        /// <summary>
//        /// 检查是否含有某个参数名称
//        /// </summary>
//        /// <param name="cfgname"></param>
//        /// <returns></returns>
//        public bool HaveConfig(string cfgname)
//        {
//            return configmap.Keys.Contains(cfgname);
//        }
//        /// <summary>
//        /// 重新加载配置文件
//        /// </summary>
//        public void ReloadConfig()
//        {
//            configmap.Clear();
//            //生成配置名和配置对象的映射Map
//            foreach (ConfigItem cfg in ORM.MConfig.SelectConfigItem(_module))
//            {
//                configmap.Add(cfg.CfgName, cfg);
//            }
//        }


//        /// <summary>
//        /// 更新某个名称的参数
//        /// 如果该参数不存在则插入该参数
//        /// </summary>
//        /// <param name="cfgname"></param>
//        /// <param name="type"></param>
//        /// <param name="value"></param>
//        /// <param name="description"></param>
//        public void UpdateConfig(string cfgname, QSEnumCfgType type, object value,string description = "")
//        {
//            ConfigItem item = new ConfigItem();
//            item.CfgName = cfgname;
//            item.CfgType = type;
//            item.ModuleID = _module;
//            item.Description = description;
//            item.CfgValue = value.ToString();

//            if (configmap.Keys.Contains(cfgname))
//            {
//                configmap[cfgname] = item;
//                ORM.MConfig.UpdateConfigItem(item);
//            }
//            else
//            {
//                configmap[cfgname] = item;
//                ORM.MConfig.InsertConfigItem(item);
//            }
//        }

//        public string CfgString
//        {
//            get
//            {
//                StringBuilder sb = new StringBuilder();
//                foreach (ConfigItem item in configmap.Values)
//                {
//                    sb.Append(item.CfgName + " " + item.CfgType.ToString() + " " + item.CfgValue + " " + item.Description + Environment.NewLine);
//                }
//                return sb.ToString();
//            }
//        }


//    }
//}
