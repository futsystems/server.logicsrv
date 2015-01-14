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


    }
}
