﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    internal class SystemInformation
    {

        public SystemInformation()
        {
            this.LastSettleday = 19700101;
            this.Maj = 0;
            this.Min = 0;
            this.Fix = 0;
            this.Date = 0;
            this.UpdateDate = 0;
            this.ProductType = QSEnumProductType.CounterSystem;
            this.DeployID = "";
        }

        /// <summary>
        /// 上次结算日
        /// </summary>
        public int LastSettleday{ get; set; }

        /// <summary>
        /// 主版本号
        /// 功能模块有大的变动，比如增加多个模块或者整体架构发生变化
        /// </summary>
        public int Maj { get; set; }

        /// <summary>
        /// 次版本号
        /// 和主版本相对而言，次版本号的升级对应的只是局部的变动。
        /// 但该局部的变动造成了程序和以前版本不能兼容，或者对该程序以前的协作关系产生了破坏，
        /// 或者是功能上有大的改进或增强。
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// 修订版本号
        /// 局部的变动，主要是局部函数的功能改进，或者bug的修正，或者功能的扩充。
        /// </summary>
        public int Fix { get; set; }

        /// <summary>
        /// 日期
        /// 版本生成日期
        /// </summary>
        public int Date { get; set; }

        /// <summary>
        /// 系统更新日期
        /// </summary>
        public int UpdateDate { get; set; }

        /// <summary>
        /// 产品类别
        /// </summary>
        public QSEnumProductType ProductType { get; set; }

        /// <summary>
        /// 部署编号
        /// </summary>
        public string DeployID { get; set; }
    }

    internal class positionroundinfo
    {
        public string Account { get; set; }
        public string Symbol { get; set; }
        public bool Side { get; set; }

        public long EntryTime { get; set; }
        public decimal EntryPrice { get; set; }
        public int EntrySize { get; set; }

        public long ExitTime { get; set; }
        public decimal ExitPrice { get; set; }
        public int ExitSize { get; set; }

        public decimal Highest { get; set; }
        public decimal Lowest { get; set; }
        
        
    }

    internal class positionfields
    {
        public string Account { get; set; }
        public string Symbol { get; set; }
        public int Size { get; set; }
        public decimal AVGPrice { get; set; }
        public int Settleday { get; set; }
        public decimal SettlePrice { get; set; }

    }

    internal class EntityCount
    {
        public int Count { get; set; }
    }

    internal class orderseq
    {
        public int MaxOrderSeq { get; set; }
    }

    internal class tradeid
    {
        public string MaxTradeID { get; set; }
    }
    public class MTradingInfo:MBase
    {

        /// <summary>
        /// 插入委托
        /// </summary>
        /// <param name="o"></param>
        public static bool InsertOrder(Order o)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Empty;
                if (o.FatherBreed != null)
                {
                    query = String.Format("Insert into tmp_orders (`settleday`,`id`,`account`,`date`,`time`,`symbol`,`localsymbol`,`timeinforce`,`offsetflag`,`hedgeflag`,`size`,`totalsize`,`filledsize`,`side`,`limitprice`,`stopprice`,`trail`,`exchange`,`securitytype`,`currency`,`ordersource`,`forceclose`,`forceclosereason`,`status`,`comment`,`broker`,`brokerlocalorderid`,`brokerremoteorderid`,`orderseq`,`orderref`,`ordersysid`,`sessionidi`,`frontidi`,`fatherid`,`breed`,`fatherbreed`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}')",o.SettleDay, o.id, o.Account, o.Date, o.Time, o.Symbol, o.LocalSymbol, o.TimeInForce, o.OffsetFlag, o.HedgeFlag, o.Size, o.TotalSize, o.FilledSize, o.Side ? 1 : 0, o.LimitPrice, o.StopPrice, o.trail, o.Exchange, o.SecurityType, o.Currency, o.OrderSource, o.ForceClose ? 1 : 0, o.ForceCloseReason, o.Status, o.Comment, o.Broker, o.BrokerLocalOrderID, o.BrokerRemoteOrderID, o.OrderSeq, o.OrderRef, o.OrderSysID, o.SessionIDi, o.FrontIDi, o.FatherID, o.Breed, o.FatherBreed);
                }
                else
                {
                    query = String.Format("Insert into tmp_orders (`settleday`,`id`,`account`,`date`,`time`,`symbol`,`localsymbol`,`timeinforce`,`offsetflag`,`hedgeflag`,`size`,`totalsize`,`filledsize`,`side`,`limitprice`,`stopprice`,`trail`,`exchange`,`securitytype`,`currency`,`ordersource`,`forceclose`,`forceclosereason`,`status`,`comment`,`broker`,`brokerlocalorderid`,`brokerremoteorderid`,`orderseq`,`orderref`,`ordersysid`,`sessionidi`,`frontidi`,`fatherid`,`breed`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}')", o.SettleDay, o.id, o.Account, o.Date, o.Time, o.Symbol, o.LocalSymbol, o.TimeInForce, o.OffsetFlag, o.HedgeFlag, o.Size, o.TotalSize, o.FilledSize, o.Side ? 1 : 0, o.LimitPrice, o.StopPrice, o.trail, o.Exchange, o.SecurityType, o.Currency, o.OrderSource, o.ForceClose ? 1 : 0, o.ForceCloseReason, o.Status, o.Comment, o.Broker, o.BrokerLocalOrderID, o.BrokerRemoteOrderID, o.OrderSeq, o.OrderRef, o.OrderSysID, o.SessionIDi, o.FrontIDi, o.FatherID, o.Breed);
                }
                return db.Connection.Execute(query) > 0;
            }
        }

        public static int MaxOrderSeq()
        {
            using (DBMySql db = new DBMySql())
            {
                string query_count = string.Format("SELECT count(pk_id) as count FROM tmp_orders");
                EntityCount num = db.Connection.Query<EntityCount>(query_count).SingleOrDefault();
                if (num.Count == 0)
                {
                    return 1000;
                }
                else
                {
                    string query_seq = string.Format("SELECT MAX(orderseq) as MaxOrderSeq FROM tmp_orders WHERE settleday = {0}", TLCtxHelper.ModuleSettleCentre.Tradingday);
                    orderseq seq = db.Connection.Query<orderseq>(query_seq).SingleOrDefault();
                    return seq.MaxOrderSeq;
                }
            }
        }

        public static int MaxTradeID()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT MAX(tradeid) as MaxTradeID FROM tmp_trades WHERE settleday = {0} AND breed='{1}'", TLCtxHelper.ModuleSettleCentre.Tradingday, QSEnumOrderBreedType.ACCT);
                tradeid id = db.Connection.Query<tradeid>(query).SingleOrDefault();
                int maxid = 0;
                if (int.TryParse(id.MaxTradeID, out maxid))
                    return maxid;
                else
                    return 0;
            }
        }
        /// <summary>
        /// 更新委托
        /// 更新内容
        /// size
        /// filledsize
        /// status
        /// comment
        /// broker
        /// brokerlocalorderid
        /// brokerremoteorderid
        /// 
        /// ordersysid
        /// orderseq由全局统一分配
        /// orderref由客户端提供
        /// front,session下单时确定
        /// </summary>
        /// <param name="o"></param>
        public static bool UpdateOrderStatus(Order o)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE tmp_orders SET size = '{0}',filledsize = '{1}',status='{2}' ,comment='{3}',broker='{4}',brokerlocalorderid='{5}' ,brokerremoteorderid='{6}' ,ordersysid='{7}'  WHERE id = '{8}'",o.Size,o.FilledSize,o.Status,o.Comment,o.Broker,o.BrokerLocalOrderID,o.BrokerRemoteOrderID,o.OrderSysID,o.id);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新翻转后的委托对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static void UpdateOrderReversed(Order o)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE tmp_orders SET size = '{0}',filledsize = '{1}',totalsize = '{2}',side = '{3}' WHERE id = '{4}'", o.Size, o.FilledSize, o.TotalSize, o.Side ? 1 : 0, o.id);
                db.Connection.Execute(query);
            }
        }

        public static void MarkOrderSettled(Order o)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE tmp_orders SET settled='{0}' where id = '{1}'", o.Settled ? 1 : 0, o.id);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 获得最近结算日的下一个结算日的委托数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Order> SelectOrdersUnSettled(int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                if (tradingday == 0)
                {
                    string query = string.Format("SELECT * FROM  tmp_orders  WHERE  settled='{0}' AND breed='{1}'", 0, QSEnumOrderBreedType.ACCT);
                    return db.Connection.Query<OrderImpl>(query);
                }
                else
                {
                    string query = string.Format("SELECT * FROM  tmp_orders  WHERE  settled='{0}' AND breed='{1}' AND settleday='{2}'", 0, QSEnumOrderBreedType.ACCT,tradingday);
                    return db.Connection.Query<OrderImpl>(query);
                }
            }
        }

        /// <summary>
        /// 恢复日内Broker侧委托数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Order> SelectBrokerOrders()
        {
            int settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            return SelectOrders(settleday, settleday,QSEnumOrderBreedType.BROKER);
        }

        /// <summary>
        /// 获得日内Router侧的委托数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Order> SelectRouterOrders()
        {
            int settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            return SelectOrders(settleday, settleday, QSEnumOrderBreedType.ROUTER);
        }
        /// <summary>
        /// 搜索某个结算时间段的委托记录
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<Order> SelectOrders(int begin, int end,QSEnumOrderBreedType breed= QSEnumOrderBreedType.ACCT)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND breed='{3}'", "tmp_orders", begin, end, breed);
                List<Order> orders = db.Connection.Query<OrderImpl>(query).ToList<Order>();

                string query2 = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND breed='{3}'", "log_orders", begin, end, breed);
                List<Order> orders2 = db.Connection.Query<OrderImpl>(query2).ToList<Order>();
                
                //合并委托记录
                orders.Union(orders2, new OrderCompare());
                return orders;
            }
        }



        /// <summary>
        /// 查询历史委托
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IList<Order> SelectHistOrders(string account,int begin,int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query2 = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND account='{3}'", "log_orders",begin, end,account);
                List<Order> orders2 = db.Connection.Query<OrderImpl>(query2).ToList<Order>();

                return orders2;
            }
        }


        /// <summary>
        /// 插入成交数据
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool InsertTrade(Trade f)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into tmp_trades (`settleday`,`id`,`account`,`xdate`,`xtime`,`symbol`,`side`,`xsize`,`xprice`,`offsetflag`,`hedgeflag`,`commission`,`profit`,`exchange`,`securitytype`,`securitycode`,`currency`,`broker`,`brokerlocalorderid`,`brokerremoteorderid`,`brokertradeid`,`tradeid`,`orderseq`,`orderref`,`ordersysid`,`positionoperation`,`fatherid`,`breed`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}')", f.SettleDay, f.id, f.Account, f.xDate, f.xTime, f.Symbol, f.Side ? 1 : 0, f.xSize, f.xPrice, f.OffsetFlag, f.HedgeFlag, f.Commission, f.Profit, f.Exchange, f.SecurityType, f.SecurityCode, f.Currency, f.Broker, f.BrokerLocalOrderID, f.BrokerRemoteOrderID, f.BrokerTradeID, f.TradeID, f.OrderSeq, f.OrderRef, f.OrderSysID, f.PositionOperation, f.FatherID, f.Breed);
                return  db.Connection.Execute(query) >0;

            }

        }


        /// <summary>
        /// 更新反转成交
        /// </summary>
        /// <param name="f"></param>
        public static void UpdateTradeReversed(Trade f)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE tmp_trades SET side='{0}' ,xsize='{1}',profit='{2}' WHERE settleday = '{3}' AND tradeid='{4}'", f.Side ? 1 : 0, f.xSize,f.Profit,f.SettleDay, f.TradeID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 标记成交已结算
        /// </summary>
        /// <param name="f"></param>
        public static void MarkeTradeSettled(Trade f)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE tmp_trades SET settled='{0}' WHERE settleday = '{1}' AND tradeid='{2}'", f.Settled ? 1 : 0, f.SettleDay, f.TradeID);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 获得最近结算日的下一个结算日的成交数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Trade> SelectTradesUnSettled(int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                if (tradingday == 0)
                {
                    string query = string.Format("SELECT * FROM  tmp_trades  WHERE  settled='{0}' AND breed='{1}'", 0, QSEnumOrderBreedType.ACCT);
                    return db.Connection.Query<TradeImpl>(query);
                }
                else
                {
                    string query = string.Format("SELECT * FROM  tmp_trades  WHERE  settled='{0}' AND breed='{1}' AND settleday='{2}'", 0, QSEnumOrderBreedType.ACCT,tradingday);
                    return db.Connection.Query<TradeImpl>(query);
                }
            }
        }


        /// <summary>
        /// 获得最近结算日的所有成交侧 成交数据
        /// </summary>
        /// <returns></returns>
        public static IList<Trade> SelectBrokerTrades()
        {
            int settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            return SelectTrades(settleday, settleday,QSEnumOrderBreedType.BROKER);
        }

        /// <summary>
        /// 搜索某个时间段的委托记录
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IList<Trade> SelectTrades(int begin, int end,QSEnumOrderBreedType breed= QSEnumOrderBreedType.ACCT)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND breed='{3}'", "tmp_trades", begin, end, breed);
                List<Trade> trades = db.Connection.Query<TradeImpl>(query).ToList<Trade>();

                string query2 = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND breed='{3}'", "log_trades", begin, end, breed);
                List<Trade> trades2 = db.Connection.Query<TradeImpl>(query2).ToList<Trade>();

                //trades.AddRange(trades2);
                trades.Union(trades2, new TradeCompare());
                return trades;
            }
        }

        /// <summary>
        /// 查询历史委托
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IList<Trade> SelectHistTrades(string account, int begin, int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND account='{3}'", "log_trades", begin, end, account);
                List<Trade> trades = db.Connection.Query<TradeImpl>(query).ToList<Trade>();

                return trades;
            }
        }

        /// <summary>
        /// 插入委托操作
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool InsertOrderAction(OrderAction action)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into tmp_orderactions (`account`,`actionflag`,`orderid`,`sessionid`,`orderref`,`exchange`,`orderexchid`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", action.Account, action.ActionFlag, action.OrderID, action.SessionID, action.OrderRef, action.Exchagne, action.OrderExchID, TLCtxHelper.ModuleSettleCentre.Tradingday);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 获得当前交易日的委托操作
        /// </summary>
        /// <returns></returns>
        public static IList<OrderAction> SelectOrderActions()
        {
            int settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            return SelectOrderActions(settleday, settleday);
        }


        /// <summary>
        /// 查询某个时间段的委托操作
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="islog"></param>
        /// <returns></returns>
        public static IList<OrderAction> SelectOrderActions(int begin, int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}'", "tmp_orderactions", begin, end);
                List<OrderAction> actions = db.Connection.Query<OrderActionImpl>(query).ToList<OrderAction>();

                string query2 = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}'", "log_orderactions", begin, end);
                List<OrderAction> actions2 = db.Connection.Query<OrderActionImpl>(query2).ToList<OrderAction>();

                actions.AddRange(actions2);
                return actions;
            }
            
        }


        //static Position posfields2position(positionfields fields)
        //{
        //    Position pos = new PositionImpl(fields.Symbol, fields.SettlePrice, fields.Size, 0, fields.Account,fields.Size>0?QSEnumPositionDirectionType.Long:QSEnumPositionDirectionType.Short);
        //    return pos;
        //}

        static PositionRoundImpl PRInfo2PositionRound(positionroundinfo info)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[info.Account];
            if (account == null)
                return null;
            PositionRoundImpl pr = new PositionRoundImpl(info.Account, account.Domain.GetSymbol(info.Symbol), info.Side);

            pr.EntryTime = info.EntryTime;
            pr.EntrySize = info.EntrySize;
            pr.EntryPrice = info.EntryPrice;

            pr.ExitTime = info.ExitTime;
            pr.ExitSize = info.ExitSize;
            pr.ExitPrice = info.ExitPrice;
            if (pr.HoldSize != 0)
            {
                pr.SetOpen();
            }
            return pr;

        }

        public static IEnumerable<PositionRoundImpl> SelectHoldPositionRounds(int lastsettleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT account,symbol,side,entrytime,entryprice,entrysize,exittime,exitprice,exitsize,highest,lowest FROM  hold_postransactions WHERE settleday = {0}", lastsettleday);
                return db.Connection.Query<positionroundinfo>(query).Select(prinfo => { return PRInfo2PositionRound(prinfo); });//(from prinfo in db.Connection.Query<positionroundinfo>(query).ToArray()
                                                      //select PRInfo2PositionRound(prinfo)).ToArray<PositionRoundImpl>();
                //return prs;
            }
        }



        /// <summary> 
        /// 插入取消数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static bool InsertCancel(int date, int time, long oid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into cancles (`date`,`time`,`ordid`) values('{0}','{1}','{2}')", date.ToString(), time.ToString(), oid.ToString());
                return db.Connection.Execute(query) > 0;
            }

        }
        /// <summary>
        /// 插入关闭的持仓回合数据
        /// </summary>
        /// <param name="pr"></param>
        /// <returns></returns>
        public static bool InsertPositionRound(PositionRound pr)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into tmp_postransactions (`account`,`symbol`,`security`,`multiple`,`entrytime`,`entrysize`,`entryprice`,`entrycommission`,`exittime`,`exitsize`,`exitprice`,`exitcommission`,`highest`,`lowest`,`size`,`holdsize`,`side`,`wl`,`totalpoints`,`profit`,`commission`,`netprofit`,`type`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',{23})", pr.Account, pr.Symbol, pr.Security, pr.Multiple, pr.EntryTime, pr.EntrySize, pr.EntryPrice, pr.EntryCommission, pr.ExitTime, pr.ExitSize, pr.ExitPrice, pr.ExitCommission, pr.Highest, pr.Lowest, pr.Size, pr.HoldSize, pr.Side ? 1 : 0, pr.WL ? 1 : 0, pr.TotalPoints, pr.Profit, pr.Commissoin, pr.NetProfit, pr.Type, TLCtxHelper.ModuleSettleCentre.Tradingday);
                return db.Connection.Execute(query) > 0;
            }
        }

        

       

        #region 日内数据转储与清空

        /// <summary>
        /// 转储orders
        /// </summary>
        /// <returns></returns>
        public static void DumpSettledOrders(out int rows, int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                rows = 0;
                string query = String.Format("replace into log_orders select * from tmp_orders WHERE settleday<={0} and settled=1", tradingday);
                rows = db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 转储成交数据
        /// </summary>
        /// <returns></returns>
        public static void DumpSettledTrades(out int rows, int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                rows = 0;
                string query = String.Format("replace into log_trades select * from tmp_trades WHERE settleday<={0} and settled=1", tradingday);
                rows = db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 转储日内取消数据到历史取消表
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static void DumpSettledOrderActions(out int rows, int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                rows = 0;
                string query = String.Format("replace into log_orderactions select * from tmp_orderactions where settleday<={0}", tradingday);
                rows = db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 删除所有已结算数据
        /// 需要加入删除前置判断
        /// </summary>
        /// <param name="tradingday"></param>
        public static void DeleteSettledTradingInfo()
        {
            using (DBMySql db = new DBMySql())
            {
                int tradingday = TLCtxHelper.ModuleSettleCentre.LastSettleday;

                string query = String.Format("DELETE FROM tmp_orders WHERE settleday <={0} AND settled=1", tradingday);
                db.Connection.Execute(query);

                query = String.Format("DELETE FROM tmp_trades WHERE settleday <={0} AND settled=1", tradingday);
                db.Connection.Execute(query);

                query = String.Format("DELETE FROM tmp_orderactions WHERE settleday <={0}", tradingday);
                db.Connection.Execute(query);

            }
        }

        /// <summary>
        /// 删除日内委托数据
        /// </summary>
        /// <returns></returns>
        public static bool ClearIntradayOrders(int tradingday=0)
        {
            using (DBMySql db = new DBMySql())
            {
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.ModuleSettleCentre.Tradingday);
                string query = String.Format("delete from tmp_orders where settleday ={0}",tradingday);
                return db.Connection.Execute(query) >= 0;
            }
        }

        

        /// <summary>
        /// 清空日内成交数据
        /// </summary>
        /// <returns></returns>
        public static bool ClearIntradayTrades(int tradingday = 0)
        {
            using (DBMySql db = new DBMySql())
            {
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.ModuleSettleCentre.Tradingday);
                string query = String.Format("delete from tmp_trades where settleday={0}",tradingday);
                return db.Connection.Execute(query)>=0;
            }
        }

        

        /// <summary>
        /// 清空日内取消数据
        /// </summary>
        /// <returns></returns>
        public static bool ClearIntradayOrderActions(int tradingday=0)
        {
            using (DBMySql db = new DBMySql())
            {
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.ModuleSettleCentre.Tradingday);
                string query = String.Format("delete from tmp_orderactions where settleday={0}",tradingday);
                return db.Connection.Execute(query)>=0;
            }
        }
        

        

        /// <summary>
        /// 清空日内交易回合记录
        /// </summary>
        /// <returns></returns>
        public static bool ClearIntradayPosTransactions(int tradingday=0)
        {
            using (DBMySql db = new DBMySql())
            {
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.ModuleSettleCentre.Tradingday);
                string query = String.Format("delete from tmp_postransactions where settleday={0}",tradingday);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 清空隔夜交易回合记录
        /// 交易回合记录处于Open状态
        /// </summary>
        /// <returns></returns>
        public static bool ClearPosTransactionsOpened()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("delete from hold_postransactions");
                return db.Connection.Execute(query) >= 0;
            }
        }
        #endregion


        #region 未结算委托数量查询

        /// <summary>
        /// 查询帐户未结算委托数量
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static int GetUnsettledAcctOrderNum(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query_count = string.Format("SELECT count(pk_id) as count FROM tmp_orders WHERE settleday<='{0}' AND breed='ACCT' AND settled=0",settleday);
                EntityCount num = db.Connection.Query<EntityCount>(query_count).SingleOrDefault();
                return num.Count;
            }
        }

        /// <summary>
        /// 查询帐户未结算成交数量
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static int GetUnsettledAcctTradeNum(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query_count = string.Format("SELECT count(pk_id) as count FROM tmp_trades WHERE settleday<='{0}' AND breed='ACCT' AND settled=0", settleday);
                EntityCount num = db.Connection.Query<EntityCount>(query_count).SingleOrDefault();
                return num.Count;
            }
        }


        /// <summary>
        /// 查询Broker未结算委托数量
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static int GetUnsettledBrokerOrderNum(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query_count = string.Format("SELECT count(pk_id) as count FROM tmp_orders WHERE settleday<='{0}' AND breed='BROKER' AND settled=0", settleday);
                EntityCount num = db.Connection.Query<EntityCount>(query_count).SingleOrDefault();
                return num.Count;
            }
        }

        /// <summary>
        /// 获得某个交易日所有日内委托数量
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static int GetInterdayOrderNum(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query_count = string.Format("SELECT count(pk_id) as count FROM tmp_orders WHERE settleday='{0}'", settleday);
                EntityCount num = db.Connection.Query<EntityCount>(query_count).SingleOrDefault();
                return num.Count;
            }
        }

        /// <summary>
        /// 获得某个交易日所有日内成交数量
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static int GetInterdayTradeNum(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query_count = string.Format("SELECT count(pk_id) as count FROM tmp_trades WHERE settleday='{0}'", settleday);
                EntityCount num = db.Connection.Query<EntityCount>(query_count).SingleOrDefault();
                return num.Count;
            }
        }


        #endregion

    }
}
