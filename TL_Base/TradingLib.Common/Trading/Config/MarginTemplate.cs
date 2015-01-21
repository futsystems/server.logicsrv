﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
namespace TradingLib.Common
{
    public class MarginTemplateSetting
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 域ID
        /// </summary>
        public int Domain_ID { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class MarginTemplate : MarginTemplateSetting
    {

        public IEnumerable<MarginTemplateItem> MarginTemplateItems { get { return _itemmap.Values; } }

        Dictionary<string, MarginTemplateItem> _itemmap = new Dictionary<string, MarginTemplateItem>();

        /// <summary>
        /// 添加保证金项目
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(MarginTemplateItem item)
        {
            if (_itemmap.Keys.Contains(item.GetItemKey()))
            {
                _itemmap[item.GetItemKey()] = item;
            }
            else
            {
                _itemmap.Add(item.GetItemKey(), item);
            }
        }

        /// <summary>
        /// 获得某个品种某个月份的 保证金模板项目
        /// </summary>
        /// <param name="code"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public MarginTemplateItem this[string code, int month]
        {
            get
            {
                MarginTemplateItem item = null;
                string key = string.Format("{0}-{1}", code, month);
                if (_itemmap.TryGetValue(key, out item))
                {
                    return item;
                }
                return null;
            }
        }
    }
}