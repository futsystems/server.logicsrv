﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MExStrategy:MBase
    {
        /// <summary>
        /// 插入计算策略模板
        /// </summary>
        /// <param name="t"></param>
        public static void InsertExStrategyTemplate(ExStrategyTemplate t)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_strategy_template (`name`,`description`,`domain_id`) VALUES ( '{0}','{1}','{2}')", t.Name, t.Description, t.Domain_ID);
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
                string query = string.Format("INSERT INTO cfg_strategy (`margin`,`avabilefund`,`sidemargin`,`creditseparate`,`positionlock`,`template_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}')", item.Margin, item.AvabileFund, item.SideMargin ? 1 : 0, item.CreditSeparate ? 1 : 0, item.PositionLock ? 1 : 0, item.Template_ID);
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
                string query = string.Format("UPDATE cfg_strategy SET margin='{0}',avabilefund='{1}' ,sidemargin='{2}' ,creditseparate='{3}' ,positionlock='{4}' WHERE id='{5}'", item.Margin, item.AvabileFund, item.SideMargin ? 1 : 0, item.CreditSeparate ? 1 : 0, item.PositionLock?1:0,item.ID);
                db.Connection.Execute(query);
            }
        }

    }
}
