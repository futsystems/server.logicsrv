using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 手续费模板维护器
    /// </summary>
    public class CommissionTemplateTracker
    {
        ConcurrentDictionary<int, CommissionTemplate> commissionTemplateMap = new ConcurrentDictionary<int, CommissionTemplate>();
        ConcurrentDictionary<int, CommissionTemplateItem> commissionTemplateItemMap = new ConcurrentDictionary<int, CommissionTemplateItem>();

        public CommissionTemplateTracker()
        {
            foreach (CommissionTemplate t in ORM.MCommission.SelectCommissionTemplates())
            {
                commissionTemplateMap.TryAdd(t.ID, t);
            }

            foreach (CommissionTemplateItem item in ORM.MCommission.SelectCommissionTemplateItems())
            {
                commissionTemplateItemMap.TryAdd(item.ID, item);

                CommissionTemplate tmp = this[item.TemplateID];
                if (tmp != null)
                {
                    tmp.AddItem(item);
                }
            }
            //将手续费模板项目 注入到模板中
        }

        /// <summary>
        /// 按照数据库ID获得手续费模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CommissionTemplate this[int id]
        {
            get
            {
                CommissionTemplate t = null;
                if (commissionTemplateMap.TryGetValue(id, out t))
                {
                    return t;
                }
                return null;
            }
        }
    }
}
