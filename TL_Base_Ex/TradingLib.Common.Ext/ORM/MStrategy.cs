﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MExStrategy : MBase
    {
        /// <summary>
        /// 插入计算策略模板
        /// </summary>
        /// <param name="t"></param>
        public static void InsertExStrategyTemplate(ExStrategyTemplate t)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_strategy_template (`name`,`description`,`domain_id`,`manager_id`) VALUES ( '{0}','{1}','{2}','{3}')", t.Name, t.Description, t.Domain_ID,t.Manager_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => t.ID = id, "id", "cfg_strategy_template");
            }
        }

        /// <summary>
        /// 更新计算策略模板
        /// </summary>
        /// <param name="t"></param>
        public static void UpdateExStrategyTemplate(ExStrategyTemplate t)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_strategy_template SET name='{0}',description='{1}' WHERE id='{2}'", t.Name, t.Description, t.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某个手续费模板
        /// </summary>
        /// <param name="template_id"></param>
        public static void DeleteExStrategyTemplate(int template_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM cfg_strategy_template WHERE id={0}", template_id);
                db.Connection.Execute(query);

                query = string.Format("DELETE FROM cfg_strategy WHERE template_id={0}", template_id);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 获得所有计算策略模板
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ExStrategyTemplate> SelectExStrategyTemplates()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_strategy_template";
                return db.Connection.Query<ExStrategyTemplate>(query);
            }
        }


        /// <summary>
        /// 插入计算策略模板项
        /// </summary>
        /// <param name="item"></param>
        public static void InsertExStrategyTemplateItem(ExStrategy item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_strategy (`marginprice`,`includecloseprofit`,`includepositionprofit`,`algorithm`,`sidemargin`,`creditseparate`,`positionlock`,`entryslip`,`exitslip` ,`limitcheck` ,`probability`,`template_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", item.MarginPrice, item.IncludeCloseProfit ? 1 : 0, item.IncludePositionProfit ? 1 : 0, item.Algorithm, item.SideMargin ? 1 : 0, item.CreditSeparate ? 1 : 0, item.PositionLock ? 1 : 0, item.EntrySlip, item.ExitSlip, item.LimitCheck ? 1 : 0, item.Probability, item.Template_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => item.ID = id, "id", "cfg_strategy");
            }
        }

        /// <summary>
        /// 获得所有计算策略模板项
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ExStrategy> SelectExStrategyTemplateItems()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_strategy";
                return db.Connection.Query<ExStrategy>(query);
            }
        }

        /// <summary>
        /// 更新计算策略模板项目
        /// </summary>
        /// <param name="item"></param>
        public static void UpdateExStrategyTemplateItem(ExStrategy item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_strategy SET marginprice='{0}',includecloseprofit='{1}' ,includepositionprofit='{2}' ,algorithm='{3}' ,sidemargin='{4}' ,creditseparate='{5}' ,positionlock='{6}',entryslip='{7}' ,exitslip='{8}',limitcheck='{9}' ,probability='{10}' WHERE id='{11}'", item.MarginPrice, item.IncludeCloseProfit ? 1 : 0, item.IncludePositionProfit ? 1 : 0, item.Algorithm, item.SideMargin ? 1 : 0, item.CreditSeparate ? 1 : 0, item.PositionLock ? 1 : 0, item.EntrySlip, item.ExitSlip, item.LimitCheck?1:0,item.Probability,item.ID);
                db.Connection.Execute(query);
            }
        }

    }
}