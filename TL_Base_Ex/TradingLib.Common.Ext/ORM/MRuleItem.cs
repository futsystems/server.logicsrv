using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MRuleItem:MBase
    {
        /// <summary>
        /// 返回帐户类别列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RuleItem> SelectRuleItem()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM cfg_rule";
                IEnumerable<RuleItem> result = db.Connection.Query<RuleItem>(query);
                return result;
            }
        }

        /// <summary>
        /// 返回某个帐户的风控规则设置
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RuleItem> SelectRuleItem(string account,QSEnumRuleType type)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM cfg_rule WHERE account = '{0}' and ruletype = '{1}'",account,type.ToString());
                IEnumerable<RuleItem> result = db.Connection.Query<RuleItem>(query);
                return result;
            }
        }

        /// <summary>
        /// 加载所有账户的风控规则设置
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RuleItem> SelectAllRuleItems()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_rule";
                IEnumerable<RuleItem> result = db.Connection.Query<RuleItem>(query);
                return result;
            }
        }

        /// <summary>
        /// 更新风控规则项目
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool UpdateRuleItem(RuleItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_rule SET compare='{0}',value='{1}',symbolset='{2}',enable='{3}' WHERE id='{4}'",item.Compare,item.Value,item.SymbolSet,item.Enable?1:0,item.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 插入风控规则项目
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool InsertRuleItem(RuleItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into cfg_rule (`account`,`rulename`,`compare`,`value`,`symbolset`,`ruletype`,`enable`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",item.Account,item.RuleName,item.Compare,item.Value,item.SymbolSet,item.RuleType,item.Enable?1:0);//orderpostflag`,`forceclose`,`hedgeflag`,`orderref`,`orderexchid`,`orderseq`
                
                int row = db.Connection.Execute(query);

                SetIdentity(db.Connection, id => item.ID = id, "id", "cfg_rule");

                return row > 0;
            }
        }

        /// <summary>
        /// 删除风控规则
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool DelRulteItem(RuleItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM cfg_rule WHERE id = '{0}'",item.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }
    }
}
