using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    


    public class MFollowItem:MBase
    {
        /// <summary>
        /// 插入跟单项目
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        public static void InsertFollowItem(FollowItemData item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_items (`followkey`,`strategyid`,`signalid`,`signaltradeid`,`opentradeid`,`closetradeid`,`stage`,`followside`,`followpower`,`settleday`,`triggertype`,`exchange`,`symbol`,`followsize`,`eventtype`,`comment`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", item.FollowKey, item.StrategyID, item.SignalID, item.SignalTradeID, item.OpenTradeID, item.CloseTradeID, item.Stage, item.FollowSide ? 1 : 0, item.FollowPower, item.Settleday, item.TriggerType, item.Exchange, item.Symbol, item.FollowSize, item.EventType,item.Comment);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新跟单项
        /// </summary>
        /// <param name="item"></param>
        public static void UpdateFollowItem(FollowItemData item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE follow_items SET stage='{0}',comment='{2}'  WHERE followkey='{1}'", item.Stage, item.FollowKey,item.Comment);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入跟单项委托关系
        /// </summary>
        /// <param name="fo"></param>
        public static void InsertFollowItemOrder(FollowItemOrder fo)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_item_order_map (`followkey`,`orderid`,`settleday`) values('{0}','{1}','{2}')",fo.FollowKey,fo.OrderID,fo.Settleday);
                db.Connection.Execute(query);
            }
        }



        /// <summary>
        /// 插入跟单项目成交关系
        /// </summary>
        /// <param name="ft"></param>
        public static void InsertFollowItemTrade(FollowItemTrade ft)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_item_trade_map (`followkey`,`tradeid`,`settleday`) values('{0}','{1}','{2}')", ft.FollowKey, ft.TradeID,ft.Settleday);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 从数据库加载所有跟单项目数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FollowItemData> SelectFollowItemData()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM follow_items";
                return db.Connection.Query<FollowItemData>(query, null);
            }
        }

        /// <summary>
        /// 从数据库加载所有跟单项委托关系
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FollowItemOrder> SelectFollowItemOrder()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM follow_item_order_map";
                return db.Connection.Query<FollowItemOrder>(query, null);
            }
        }


        /// <summary>
        /// 从数据库加载所有跟单项成交关系
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FollowItemTrade> SelectFollowItemTrade()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM follow_item_trade_map";
                return db.Connection.Query<FollowItemTrade>(query, null);
            }
        }


        /// <summary>
        /// 转储日内记录到对应的log表
        /// </summary>
        public static void DumpInterdayFollowInfos()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "replace into follow_items_log select * from follow_items";
                db.Connection.Execute(query);

                query = "replace into follow_item_trade_map_log select * from follow_item_trade_map";
                db.Connection.Execute(query);

                query = "replace into follow_item_order_map_log select * from follow_item_order_map";
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 清空日内跟单记录表
        /// </summary>
        public static void ClearInterdayFollowInfos()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "delete from follow_items";
                db.Connection.Execute(query);

                query = "delete from follow_item_trade_map";
                db.Connection.Execute(query);

                query = "delete from follow_item_order_map";
                db.Connection.Execute(query);
            }
        }

    }
}
