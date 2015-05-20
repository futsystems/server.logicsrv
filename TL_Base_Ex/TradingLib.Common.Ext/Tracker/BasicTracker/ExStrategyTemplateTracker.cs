using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 算法模板维护器
    /// 维护了所有分区的计算策略模板
    /// </summary>
    public class ExStrategyTemplateTracker
    {
        ConcurrentDictionary<int, ExStrategyTemplate> exStrategyTemplateMap = new ConcurrentDictionary<int, ExStrategyTemplate>();
        ConcurrentDictionary<int, ExStrategy> exStrategyMap = new ConcurrentDictionary<int, ExStrategy>();

        /// <summary>
        /// 
        /// </summary>
        public ExStrategyTemplateTracker()
        {
            foreach (ExStrategyTemplate tmp in ORM.MExStrategy.SelectExStrategyTemplates())
            {
                exStrategyTemplateMap.TryAdd(tmp.ID, tmp);
            }

            foreach (ExStrategy strategy in ORM.MExStrategy.SelectExStrategyTemplateItems())
            {
                exStrategyMap.TryAdd(strategy.ID, strategy);

                ExStrategyTemplate t = this[strategy.Template_ID];
                if (t != null)
                {
                    t.ExStrategy = strategy;
                }
            }

        }

        /// <summary>
        /// 计算策略维护器
        /// 通过数据库全局ID获得对应的策略模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExStrategyTemplate this[int id]
        {
            get
            {
                ExStrategyTemplate t = null;
                if (exStrategyTemplateMap.TryGetValue(id, out t))
                {
                    return t;
                }
                return null;
            }
        }

        /// <summary>
        /// 返回所有策略模板
        /// </summary>
        public IEnumerable<ExStrategyTemplate> ExStrategyTemplates
        {
            get
            {
                return exStrategyTemplateMap.Values;
            }
        }

        /// <summary>
        /// 更新策略模板信息
        /// </summary>
        /// <param name="t"></param>
        public void UpdateExStrategyTemplate(ExStrategyTemplateSetting t)
        {
            ExStrategyTemplate target = null;
            
            //存在对应的template则进行更新
            if (exStrategyTemplateMap.TryGetValue(t.ID, out target))
            {
                target.Name = t.Name;
                target.Description = t.Description;
                ORM.MExStrategy.UpdateExStrategyTemplate(target);
            }
            else
            {
                //插入新的计算策略模板
                target = new ExStrategyTemplate();
                target.Name = t.Name;
                target.Description = t.Description;
                target.Domain_ID = t.Domain_ID;

                ORM.MExStrategy.InsertExStrategyTemplate(target);
                //放入内存数据结构
                exStrategyTemplateMap.TryAdd(target.ID, target);
                //数据库插入后获得的全局ID赋值回传
                t.ID = target.ID;

                ExStrategy item = new ExStrategy();
                item.Template_ID = t.ID;
                this.UpdateExStrategy(item);

            }
        }

        /// <summary>
        /// 更新策略模板项目
        /// </summary>
        /// <param name="item"></param>
        public void UpdateExStrategy(ExStrategy item)
        {
            ExStrategy target = null;
            //更新
            if (exStrategyMap.TryGetValue(item.ID, out target))
            {
                target.MarginPrice = item.MarginPrice;
                target.IncludeCloseProfit = item.IncludeCloseProfit;
                target.IncludePositionProfit = item.IncludePositionProfit;
                target.Algorithm = item.Algorithm;
                target.SideMargin = item.SideMargin;
                target.CreditSeparate = item.CreditSeparate;
                target.PositionLock = item.PositionLock;
                target.EntrySlip = item.EntrySlip;
                target.ExitSlip = item.ExitSlip;

                ORM.MExStrategy.UpdateExStrategyTemplateItem(target);
            }
            else
            {
                target = new ExStrategy();

                target.MarginPrice = item.MarginPrice;
                target.IncludeCloseProfit = item.IncludeCloseProfit;
                target.IncludePositionProfit = item.IncludePositionProfit;
                target.Algorithm = item.Algorithm;
                target.SideMargin = item.SideMargin;
                target.CreditSeparate = item.CreditSeparate;
                target.PositionLock = item.PositionLock;
                target.EntrySlip = item.EntrySlip;
                target.ExitSlip = item.ExitSlip;

                target.Template_ID = item.Template_ID;
                ORM.MExStrategy.InsertExStrategyTemplateItem(target);
                item.ID = target.ID;

                
                //加入到内存数据结构
                exStrategyMap.TryAdd(target.ID, target);

                ExStrategyTemplate t = this[target.Template_ID];
                if (t != null)
                {
                    t.ExStrategy = target;
                }
            }
        }
    }


}
