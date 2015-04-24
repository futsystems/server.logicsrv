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

        /// <summary>
        /// 返回所有手续费模板项目
        /// </summary>
        public IEnumerable<CommissionTemplateItem> CommissionTemplateItems
        {
            get
            {
                return commissionTemplateItemMap.Values;
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
                            item.Percent = 0;
                            ORM.MCommission.InsertCommissionTemplateItem(item);

                            //加入到内存数据结构
                            commissionTemplateItemMap.TryAdd(item.ID, item);
                            target.AddItem(item);
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// 向某个模板中添加某个品种的默认模板项
        /// </summary>
        /// <param name="tmp"></param>
        /// <param name="sec"></param>
        public void AddDefaultTemplateItem(CommissionTemplate tmp, SecurityFamilyImpl sec)
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
                item.Template_ID = tmp.ID;
                item.Percent = 0;
                ORM.MCommission.InsertCommissionTemplateItem(item);

                //加入到内存数据结构
                commissionTemplateItemMap.TryAdd(item.ID, item);
                tmp.AddItem(item);
            }
        }
        /// <summary>
        /// 更新某个手续费模板项目
        /// </summary>
        /// <param name="item"></param>
        public void UpdateCommissionTemplateItem(CommissionTemplateItemSetting item)
        {
            CommissionTemplateItem target = null;
            if (commissionTemplateItemMap.TryGetValue(item.ID, out target))
            {
                target.ChargeType = item.ChargeType;
                target.OpenByMoney = item.OpenByMoney;
                target.OpenByVolume = item.OpenByVolume;
                target.CloseByMoney = item.CloseByMoney;
                target.CloseByVolume = item.CloseByVolume;
                target.CloseTodayByMoney = item.CloseTodayByMoney;
                target.CloseTodayByVolume = item.CloseTodayByVolume;
                target.Percent = item.Percent;
                //更新数据库
                ORM.MCommission.UpdateCommissionTemplateItem(target);
            }
            else
            {
                target = new CommissionTemplateItem();
                target.Code = item.Code;
                target.Month = item.Month;
                target.OpenByMoney = item.OpenByMoney;
                target.OpenByVolume = item.OpenByVolume;
                target.CloseTodayByMoney = item.CloseTodayByMoney;
                target.CloseTodayByVolume = item.CloseTodayByVolume;
                target.CloseByMoney = item.CloseByMoney;
                target.CloseByVolume = item.CloseByVolume;
                target.Percent = item.Percent;
                target.Template_ID = item.Template_ID;
                target.ChargeType = item.ChargeType;

                ORM.MCommission.InsertCommissionTemplateItem(target);
                item.ID = target.ID;

                //加入到内存数据结构
                commissionTemplateItemMap.TryAdd(target.ID, target);

                CommissionTemplate template = BasicTracker.CommissionTemplateTracker[target.Template_ID];
                if (template != null)
                {
                    template.AddItem(target);
                }
            }
        }
    }
}
