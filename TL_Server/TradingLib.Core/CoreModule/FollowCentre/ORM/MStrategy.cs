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
                string query = string.Format("INSERT INTO follow_strategys (`token`,`desp`,`followdirection`,`followpower`,`entrypricetype`,`entryoffsetticks`,`entrypendingthresholdtype`,`entrypendingthresholdvalue`,`entrypendingoperationtype`,`exitpricetype`,`exitoffsetticks`,`exitpendingthreadsholdtype`,`exitpendingthresholdvalue`,`exitpendingoperationtype`,`account`,`domain_id`,`secfilter`,`timefilter`,`sizefilter`,`stopenable`,`stopvalue`,`stopvaluetype`,`profit1enable`,`profit1value`,`profit1valuetype`,`profit2enable`,`profit2value1`,`profit2trailing1`,`profit2value1type`,`profit2value2`,`profit2trailing2`,`profit2value2type`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')", cfg.Token, cfg.Desp, cfg.FollowDirection, cfg.FollowPower, cfg.EntryPriceType, cfg.EntryOffsetTicks, cfg.EntryPendingThresholdType, cfg.EntryPendingThresholdValue, cfg.EntryPendingOperationType, cfg.ExitPriceType, cfg.ExitOffsetTicks, cfg.ExitPendingThreadsholdType, cfg.ExitPendingThresholdValue, cfg.ExitPendingOperationType, cfg.Account, cfg.Domain_ID, cfg.SecFilter, cfg.TimeFilter, cfg.SizeFilter, cfg.StopEnable ? 1 : 0, cfg.StopValue, cfg.StopValueType, cfg.Profit1Enable ? 1 : 0, cfg.Profit1Value, cfg.Profit1ValueType, cfg.Profit2Enable ? 1 : 0, cfg.Profit2Value1, cfg.Profit2Trailing1, cfg.Profit2Value1Type, cfg.Profit2Value2, cfg.Profit2Trailing2, cfg.Profit2Value2Type);
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
                string query = string.Format("UPDATE follow_strategys SET desp='{0}',entrypricetype='{1}',entryoffsetticks='{2}',entrypendingthresholdtype='{3}',entrypendingthresholdvalue='{4}',entrypendingoperationtype='{5}',exitpricetype='{6}',exitoffsetticks='{7}',exitpendingthreadsholdtype='{8}',exitpendingthresholdvalue='{9}',exitpendingoperationtype='{10}' ,secfilter='{11}',timefilter='{12}',sizefilter='{13}',stopenable='{14}',stopvalue='{15}',stopvaluetype='{16}',profit1enable='{17}',profit1value='{18}',profit1valuetype='{19}',profit2enable='{20}',profit2value1='{21}',profit2trailing1='{22}',profit2value1type='{23}',profit2value2='{24}',profit2trailing2='{25}',profit2value2type='{26}',followpower='{27}' WHERE id='{28}'", cfg.Desp, cfg.EntryPriceType, cfg.EntryOffsetTicks, cfg.EntryPendingThresholdType, cfg.EntryPendingThresholdValue, cfg.EntryPendingOperationType, cfg.ExitPriceType, cfg.ExitOffsetTicks, cfg.ExitPendingThreadsholdType, cfg.ExitPendingThresholdValue, cfg.ExitPendingOperationType, cfg.SecFilter, cfg.TimeFilter, cfg.SizeFilter, cfg.StopEnable ? 1 : 0, cfg.StopValue, cfg.StopValueType, cfg.Profit1Enable ? 1 : 0, cfg.Profit1Value, cfg.Profit1ValueType, cfg.Profit2Enable ? 1 : 0, cfg.Profit2Value1, cfg.Profit2Trailing1, cfg.Profit2Value1Type, cfg.Profit2Value2, cfg.Profit2Trailing2, cfg.Profit2Value2Type, cfg.FollowPower,cfg.ID);
                db.Connection.Execute(query);
            }
        }
    }
}
