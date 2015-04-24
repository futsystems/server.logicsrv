using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class CommissionTemplateSetting
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


    /// <summary>
    /// 手续费模板
    /// </summary>
    public class CommissionTemplate : CommissionTemplateSetting
    {
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

        /// <summary>
        /// 判断是否存在某个品种的模板项目
        /// </summary>
        /// <param name="code"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public bool HaveTemplateItem(string code, int month=0)
        {
            if (month == 0)
            {
                return _itemamp.Keys.Contains(string.Format("{0}-1", code));
            }
            else
            {
                return _itemamp.Keys.Contains(string.Format("{0}-{1}", code, month));
            }
        }



    }
}
