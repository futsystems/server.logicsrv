using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MStrategy: MBase
    {
        /// <summary>
        /// 查找所有跟单配置信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FollowStrategyConfig> SelectFollowStrategyConfigs()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM follow_strategys";
                return db.Connection.Query<FollowStrategyConfig>(query, null);
            }
        }

        /// <summary>
        /// 插入跟单策略配置
        /// </summary>
        /// <param name="cfg"></param>
        public static void InsertFollowStrategyConfig(FollowStrategyConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO follow_strategys (`token`,`desp`,`followdirection`,`followpower`,`entrypricetype`,`entryoffsetticks`,`entrypendingthresholdtype`,`entrypendingthresholdvalue`,`entrypendingoperationtype`,`exitpricetype`,`exitoffsetticks`,`exitpendingthreadsholdtype`,`exitpendingthresholdvalue`,`exitpendingoperationtype`,`account`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", cfg.Token, cfg.Desp, cfg.FollowDirection, cfg.FollowPower, cfg.EntryPriceType, cfg.EntryOffsetTicks, cfg.EntryPendingThresholdType, cfg.EntryPendingThresholdValue, cfg.EntryPendingOperationType, cfg.ExitPriceType, cfg.ExitOffsetTicks, cfg.ExitPendingThreadsholdType, cfg.ExitPendingThresholdValue, cfg.ExitPendingOperationType, cfg.Account);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => cfg.ID = id, "id", "follow_strategys");
            }
        }

        /// <summary>
        /// 更新跟单策略配置信息
        /// 
        /// </summary>
        /// <param name="cfg"></param>
        public static void UpdateFollowStrategyConfig(FollowStrategyConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE follow_strategys SET desp='{0}',entrypricetype='{1}',entryoffsetticks='{2}',entrypendingthresholdtype='{3}',entrypendingthresholdvalue='{4}',entrypendingoperationtype='{5}',exitpricetype='{6}',exitoffsetticks='{7}',exitpendingthreadsholdtype='{8}',exitpendingthresholdvalue='{9}',exitpendingoperationtype='{10}' WHERE id='{11}'", cfg.Desp, cfg.EntryPriceType, cfg.EntryOffsetTicks, cfg.EntryPendingThresholdType, cfg.EntryPendingThresholdValue, cfg.EntryPendingOperationType, cfg.ExitPriceType, cfg.ExitOffsetTicks, cfg.ExitPendingThreadsholdType, cfg.ExitPendingThresholdValue, cfg.ExitPendingOperationType,cfg.ID);
                db.Connection.Execute(query);
            }
        }
    }
}
