using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 保证金模板维护器
    /// </summary>
    public class MarginTemplateTracker
    {
        ConcurrentDictionary<int, MarginTemplate> marginTemplateMap = new ConcurrentDictionary<int, MarginTemplate>();
        ConcurrentDictionary<int, MarginTemplateItem> marginTemplateItemMap = new ConcurrentDictionary<int, MarginTemplateItem>();

        public MarginTemplateTracker()
        { 
            foreach(MarginTemplate t in ORM.MMargin.SelectMarginTemplates())
            {
                marginTemplateMap.TryAdd(t.ID, t);
            }

            foreach (MarginTemplateItem item in ORM.MMargin.SelectMarginTemplateItems())
            {
                marginTemplateItemMap.TryAdd(item.ID, item);

                MarginTemplate template = this[item.Template_ID];
                if (template != null)
                {
                    template.AddItem(item);
                }
            }
        }

        public MarginTemplate this[int id]
        {
            get
            {
                MarginTemplate template = null;
                if (marginTemplateMap.TryGetValue(id, out template))
                {
                    return template;
                }
                return null;
            }
        }

        public IEnumerable<MarginTemplate> MarginTemplates
        {
            get
            {
                return marginTemplateMap.Values;
            }
        }

        public IEnumerable<MarginTemplateItem> MarginTemplateItems
        {
            get
            {
                return marginTemplateItemMap.Values;
            }
        }

        /// <summary>
        /// 删除某个保证金模板
        /// </summary>
        /// <param name="template_id"></param>
        public void DeleteMarginTemplate(int template_id)
        {
            MarginTemplate target = null;
            //存在对应的手续费模板
            if (marginTemplateMap.TryGetValue(template_id, out target))
            {
                ORM.MMargin.DeleteMarginTemplate(template_id);

                //删除模板对象
                MarginTemplate remove_template = null;
                marginTemplateMap.TryRemove(template_id, out remove_template);//删除模板对象

                
                if (remove_template != null)
                {
                    MarginTemplateItem remove_item = null;
                    //删除所有模板项
                    foreach (var item in remove_template.MarginTemplateItems)
                    {
                        marginTemplateItemMap.TryRemove(item.ID, out remove_item);
                    }
                }

            }
        }
        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="t"></param>
        public void UpdateMarginTemplate(MarginTemplateSetting t)
        {
            MarginTemplate target = null;
            if (marginTemplateMap.TryGetValue(t.ID, out target))
            {
                //更新
                target.Name = t.Name;
                target.Description = t.Description;
                ORM.MMargin.UpdateMarginTemplate(target);
            }
            else
            {
                target = new MarginTemplate();
                target.Name = t.Name;
                target.Description = t.Description;
                target.Domain_ID = t.Domain_ID;
                target.Manager_ID = t.Manager_ID;

                ORM.MMargin.InsertMarginTemplate(target);
                marginTemplateMap.TryAdd(target.ID, target);
                t.ID = target.ID;

                Domain domain = BasicTracker.DomainTracker.SuperDomain;
                if (domain != null)
                {
                    foreach (SecurityFamilyImpl sec in domain.GetSecurityFamilies().Where(sec => sec.Type == SecurityType.FUT))
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            MarginTemplateItem item = new MarginTemplateItem();
                            item.Code = sec.Code;
                            item.Month = i;
                            item.MarginByMoney = 0;
                            item.MarginByVolume = 0;
                            item.Percent = 0;
                            item.ChargeType = QSEnumChargeType.Relative;
                            item.Template_ID = target.ID;
                            item.Percent = 0;
                            ORM.MMargin.InsertMarginTemplateItem(item);
                            //加入到内存数据结构
                            marginTemplateItemMap.TryAdd(item.ID, item);
                            target.AddItem(item);
                        }
                    }
                }
            }
        }

        public void UpdateMarginTemplateItem(MarginTemplateItemSetting item)
        {
            MarginTemplateItem target = null;
            if (marginTemplateItemMap.TryGetValue(item.ID, out target))
            {
                target.MarginByMoney = item.MarginByMoney;
                target.MarginByVolume = item.MarginByVolume;
                target.Percent = item.Percent;
                target.ChargeType = item.ChargeType;

                ORM.MMargin.UpdateMarginTemplateItem(target);
            }
            else
            {
                target = new MarginTemplateItem();
                target.ChargeType = item.ChargeType;
                target.Code = item.Code;
                target.Month = item.Month;
                target.MarginByMoney = item.MarginByMoney;
                target.MarginByVolume = item.MarginByVolume;
                target.Percent = item.Percent;
                target.Template_ID = item.Template_ID;

                ORM.MMargin.InsertMarginTemplateItem(target);
                item.ID = target.ID;

                marginTemplateItemMap.TryAdd(target.ID, target);

                MarginTemplate template = this[target.Template_ID];
                if (template != null)
                {
                    template.AddItem(target);
                }
            }
        }

        /// <summary>
        /// 为某个保证金模板增加某个品种的保证金模板项目
        /// </summary>
        /// <param name="tmp"></param>
        /// <param name="sec"></param>
        public void AddDefaultTemplateItem(MarginTemplate tmp, SecurityFamilyImpl sec)
        {
            for (int i = 1; i <= 12; i++)
            {
                MarginTemplateItem item = new MarginTemplateItem();
                item.Code = sec.Code;
                item.Month = i;
                item.MarginByMoney = 0;
                item.MarginByVolume = 0;
                item.Percent = 0;
                item.ChargeType = QSEnumChargeType.Relative;
                item.Template_ID = tmp.ID;
                item.Percent = 0;
                ORM.MMargin.InsertMarginTemplateItem(item);
                //加入到内存数据结构
                marginTemplateItemMap.TryAdd(item.ID, item);
                tmp.AddItem(item);
            }
        }
    }
}
