using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class ConfigTemplateTracker
    {
        ConcurrentDictionary<int, ConfigTemplate> configTemplateMap = new ConcurrentDictionary<int, ConfigTemplate>();

        public ConfigTemplateTracker()
        {
            foreach (ConfigTemplate t in ORM.MConfigTemplate.SelectConfigTemplates())
            {
                configTemplateMap.TryAdd(t.ID, t);
            }

        }

        public IEnumerable<ConfigTemplate> ConfigTemplates { get { return configTemplateMap.Values; } }
        /// <summary>
        /// 获取配置模板对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConfigTemplate this[int id]
        {
            get
            {
                ConfigTemplate target = null;
                if (configTemplateMap.TryGetValue(id, out target))
                {
                    return target;
                }
                else
                {
                    return null;
                }

            }
        }


        public void DeleteConfigTemplate(int id)
        {
            ConfigTemplate target = null;
            if (configTemplateMap.TryRemove(id, out target))
            {
                ORM.MConfigTemplate.DeleteConfigTemplate(id);
            }
        }
        /// <summary>
        /// 更新配置模板
        /// </summary>
        /// <param name="t"></param>
        public void UpdateConfigTemplate(ConfigTemplate t)
        {
            ConfigTemplate target = null;
            if (configTemplateMap.TryGetValue(t.ID, out target))
            {
                target.Description = t.Description;
                target.Margin_ID = t.Margin_ID;
                target.Commission_ID = t.Commission_ID;
                target.ExStrategy_ID = t.ExStrategy_ID;

                ORM.MConfigTemplate.UpdateConfigTemplate(target);
            }
            else
            {
                target = new ConfigTemplate();
                target.Domain_ID = t.Domain_ID;
                target.Manager_ID = t.Manager_ID;
                target.Name = t.Name;
                target.Description = t.Description;
                target.Margin_ID = t.Margin_ID;
                target.Commission_ID = t.Commission_ID;
                target.ExStrategy_ID = t.ExStrategy_ID;

                ORM.MConfigTemplate.InsertConfigTemplate(target);
                configTemplateMap.TryAdd(target.ID, target);

                t.ID = target.ID;
            }
        }
    }
}
