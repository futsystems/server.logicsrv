using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 手续费模板
    /// </summary>
    public class CommissionTemplate
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
        /// 模板ID
        /// </summary>
        public int Template_ID { get; set; }

        public IEnumerable<CommissionTemplateItem> CommissionItems { get { return _itemamp.Values; } }

        Dictionary<string, CommissionTemplateItem> _itemamp = new Dictionary<string, CommissionTemplateItem>();

        /// <summary>
        /// 添加手续费项目
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(CommissionTemplateItem item)
        {
            if (_itemamp.Keys.Contains(item.GetItemKey()))
            {
                _itemamp[item.GetItemKey()] = item;
            }
            else
            {
                _itemamp.Add(item.GetItemKey(), item);
            }
        }

        /// <summary>
        /// 获得手续费模板中的某个模板项，模板项按品种-月份 进行索引
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CommissionTemplateItem this[string code,int month]
        {
            get
            {
                CommissionTemplateItem item = null;
                string key = string.Format("{0}-{1}", code, month);
                if (_itemamp.TryGetValue(key, out item))
                {
                    return item;
                }
                return null;
            }
        }

    }
}
