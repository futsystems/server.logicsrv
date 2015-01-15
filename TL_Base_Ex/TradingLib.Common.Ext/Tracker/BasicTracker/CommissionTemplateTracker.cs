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

                CommissionTemplate tmp = this[item.Template_ID];
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

        /// <summary>
        /// 返回所有手续费模板
        /// </summary>
        public IEnumerable<CommissionTemplate> CommissionTemplates
        {
            get
            {
                return commissionTemplateMap.Values;
            }
        }

        public void UpdateCommissionTemplate(CommissionTemplateSetting t)
        {
            CommissionTemplate target = null;
            if (commissionTemplateMap.TryGetValue(t.ID,out target))
            {
                //更新
                target.Name = t.Name;
                target.Description = t.Description;
                ORM.MCommission.UpdateCommissionTemplate(target);
            }
            else
            {
                //插入新的手续费模板
                target = new CommissionTemplate();
                target.Name = t.Name;
                target.Description = t.Description;
                target.Domain_ID = t.Domain_ID;

                ORM.MCommission.InsertCommissionTemplate(target);
                //放入内存数据结构
                commissionTemplateMap.TryAdd(target.ID, target);
                //插入原始数据
                t.ID = target.ID;

                Domain domain = BasicTracker.DomainTracker.SuperDomain;
                if (domain != null)
                {
                    foreach (SecurityFamilyImpl sec in domain.GetSecurityFamilies().Where(sec=>sec.Type == SecurityType.FUT))
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            CommissionTemplateItem item = new CommissionTemplateItem();
                            item.Code = sec.Code;
                            item.Month = i;
                            item.OpenByMoney = 0;
                            item.OpenByVolume = 0;
                            item.CloseByMoney = 0;
                            item.CloseByVolume = 0;
                            item.CloseTodayByMoney = 0;
                            item.CloseTodayByVolume = 0;
                            item.ChargeType = QSEnumChargeType.Relative;
                            item.Template_ID = target.ID;
                            ORM.MCommission.InsertCommissionTemplateItem(item);
                            target.AddItem(item);
                        }
                    }
                }
                
            }
        }
    }
}
