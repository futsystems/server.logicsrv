using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    internal class systeminformation
    {
        public systeminformation()
        {
            LastSettleday = 19700101;
        }
        public int LastSettleday{ get; set; }
    }

    internal class positionroundinfo
    {
        public string Account { get; set; }
        public string Symbol { get; set; }

        public DateTime EntryTime { get; set; }
        public decimal EntryPrice { get; set; }
        public int EntrySize { get; set; }

        public DateTime ExitTime { get; set; }
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

    internal class ordersnum
    {
        public int Count { get; set; }
    }

    internal class orderseq
    {
        public int MaxOrderSeq { get; set; }
    }
    public class MTradingInfo:MBase
    {

        public static int GetLastSettleday()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM system";
                systeminformation info = db.Connection.Query<systeminformation>(query).SingleOrDefault<systeminformation>();
                return info.LastSettleday;
            }
        }


        /// <summary>
        /// 插入委托
        /// </summary>
        /// <param name="o"></param>
        public static bool InsertOrder(Order o)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into tmp_orders (`symbol`,`size`,`price`,`stopp`,`comment`,`exchange`,`account`,`securitytype`,`currency`,`localsymbol`,`id`,`tif`,`date`,`time`,`trail`,`broker`,`brokerkey`,`status`,`ordersource`,`settleday`,`offsetflag`,`forceclose`,`hedgeflag`,`orderref`,`orderexchid`,`orderseq`,`totalsize`,`filled`,`frontidi`,`sessionidi`,`side`,`forceclosereason`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')", o.symbol, (o.UnsignedSize * (o.side ? 1 : -1)).ToString(), o.price.ToString(), o.stopp.ToString(), o.comment, o.Exchange, o.Account, o.SecurityType.ToString(), o.Currency.ToString(), o.LocalSymbol, o.id.ToString(), o.TIF, o.date.ToString(), o.time.ToString(), o.trail.ToString(), o.Broker, o.BrokerKey, o.Status.ToString(), o.OrderSource.ToString(), TLCtxHelper.Ctx.SettleCentre.CurrentTradingday, o.OffsetFlag, (o.ForceClose == true ? 1: 0), o.HedgeFlag, o.OrderRef, o.OrderExchID, o.OrderSeq, o.TotalSize, o.Filled, o.FrontIDi, o.SessionIDi, o.side ? 1 : 0,o.ForceCloseReason);
                //TLCtxHelper.Ctx.debug("query:" + query);
                return db.Connection.Execute(query) > 0;
            }
        }

        public static int MaxOrderSeq()
        {
            using (DBMySql db = new DBMySql())
            {
                string query_count = string.Format("SELECT count(pk_id) as count FROM tmp_orders");
                ordersnum num = db.Connection.Query<ordersnum>(query_count).SingleOrDefault();
                if (num.Count == 0)
                {
                    return 1000;
                }
                else
                {
                    string query_seq = string.Format("SELECT MAX(orderseq) as MaxOrderSeq FROM tmp_orders WHERE settleday = {0}", TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
                    orderseq seq = db.Connection.Query<orderseq>(query_seq).SingleOrDefault();
                    return seq.MaxOrderSeq;
                }
            }
        }
        /// <summary>
        /// 更新委托
        /// </summary>
        /// <param name="o"></param>
        public static bool UpdateOrderStatus(Order o)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE tmp_orders SET status = '{0}',broker = '{1}',brokerkey='{2}' ,size='{3}',filled='{4}' ,orderexchid='{5}' ,comment='{6}'  WHERE id = '{7}'", o.Status.ToString(), o.Broker, o.BrokerKey, o.size, o.Filled, o.OrderExchID,o.comment, o.id);
                return db.Connection.Execute(query) >= 0;
            }
        }


        public static IList<Order> SelectOrders()
        {
            int tradingday = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;
            if (tradingday == 0)
            {
                throw new Exception("not valid current trading day!");
            }
            return SelectOrders(tradingday,tradingday);
        }

        /// <summary>
        /// 搜索某个结算时间段的委托记录
        /// 
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IList<Order> SelectOrders(int begin, int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}'", "tmp_orders", begin, end);
                TLCtxHelper.Debug(query);
                List<Order> orders = db.Connection.Query<OrderImpl>(query).ToList<Order>();

                string query2 = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}'", "log_orders", begin, end);
                TLCtxHelper.Debug(query);
                List<Order> orders2 = db.Connection.Query<OrderImpl>(query2).ToList<Order>();
                
                //合并委托记录
                orders.AddRange(orders2);

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
                string query = String.Format("Insert into tmp_trades (`id`,`xsize`,`xprice`,`xdate`,`xtime`,`symbol`,`account`,`fillid`,`broker`,`brokerkey`,`positionoperation`,`commission`,`settleday`,`orderref`,`orderexchid`,`orderseq`,`hedgeflag`,`profit`,`securitytype`,`currency`,`exchange`,`securitycode`,`offsetflag`,`side`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}')", f.id.ToString(), f.xsize.ToString(), f.xprice.ToString(), f.xdate.ToString(), f.xtime.ToString(), f.symbol.ToString(), f.Account.ToString(), f.id.ToString(), f.Broker, f.BrokerKey, f.PositionOperation.ToString(), f.Commission.ToString(), TLCtxHelper.Ctx.SettleCentre.CurrentTradingday, f.OrderRef, f.OrderExchID, f.OrderSeq, f.HedgeFlag, f.Profit, f.SecurityType, f.Currency, f.Exchange, f.SecurityCode, f.OffsetFlag,f.side?1:0);
                return  db.Connection.Execute(query) >0;

            }

        }

        public static IList<Trade> SelectTrades()
        {
            int tradingday = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;
            if (tradingday == 0)
            {
                throw new Exception("not valid current trading day!");
            }
            return SelectTrades(tradingday, tradingday);
        }

        /// <summary>
        /// 搜索某个时间段的委托记录
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IList<Trade> SelectTrades(int begin, int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}'", "tmp_trades", begin, end);
                List<Trade> trades = db.Connection.Query<TradeImpl>(query).ToList<Trade>();

                string query2 = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}'", "log_trades", begin, end);
                List<Trade> trades2 = db.Connection.Query<TradeImpl>(query2).ToList<Trade>();

                trades.AddRange(trades2);
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
                string query = String.Format("Insert into tmp_orderactions (`account`,`actionflag`,`orderid`,`sessionid`,`orderref`,`exchange`,`orderexchid`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", action.Account,action.ActionFlag,action.OrderID,action.SessionID,action.OrderRef,action.Exchagne,action.OrderExchID,TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 获得当前交易日的委托操作
        /// </summary>
        /// <returns></returns>
        public static IList<OrderAction> SelectOrderActions()
        {
            int tradingday = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;
            if (tradingday == 0)
            {
                throw new Exception("not valid current trading day!");
            }
            return SelectOrderActions(tradingday, tradingday);
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


        static Position posfields2position(positionfields fields)
        {
            Position pos = new PositionImpl(fields.Symbol, fields.SettlePrice, fields.Size, 0, fields.Account,fields.Size>0?QSEnumPositionDirectionType.Long:QSEnumPositionDirectionType.Short);
            return pos;
        }
        /// <summary>
        /// 获得所有隔夜持仓
        /// </summary>
        /// <returns></returns>
        public static IList<Position> SelectHoldPositions(int lastsettleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  hold_positions WHERE settleday = {0}",lastsettleday);
                IList<Position> positions = (from fields in (db.Connection.Query<positionfields>(query).ToArray())
                                             select posfields2position(fields)).ToArray();
                return positions;
            }
        }


        /// <summary>
        /// 查询历史委托
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IList<SettlePosition> SelectHistPositions(string account, int begin, int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND account='{3}'", "hold_positions", begin, end, account);
                List<SettlePosition> poslist = db.Connection.Query<SettlePosition>(query).ToList<SettlePosition>();

                return poslist;
            }
        }


        static PositionRound PRInfo2PositionRound(positionroundinfo info)
        {
            PositionRound pr = new PositionRound(info.Account, BasicTracker.SymbolTracker[info.Symbol]);

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
            pr.Side = pr.EntrySize > 0;
            return pr;

        }

        public static IList<PositionRound> SelectHoldPositionRounds(int lastsettleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT account,symbol,entrytime,entryprice,entrysize,exittime,exitprice,exitsize,highest,lowest FROM  hold_postransactions WHERE settleday = {0}", lastsettleday);
                IList<PositionRound> prs = (from prinfo in db.Connection.Query<positionroundinfo>(query).ToArray()
                                                   select PRInfo2PositionRound(prinfo)).ToArray<PositionRound>();
                return prs;
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
        public static bool InsertPositionRound(IPositionRound pr)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into tmp_postransactions (`account`,`symbol`,`security`,`multiple`,`entrytime`,`entrysize`,`entryprice`,`entrycommission`,`exittime`,`exitsize`,`exitprice`,`exitcommission`,`highest`,`lowest`,`size`,`holdsize`,`side`,`wl`,`totalpoints`,`profit`,`commission`,`netprofit`,`type`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',{23})", pr.Account, pr.Symbol, pr.Security, pr.Multiple.ToString(), pr.EntryTime.ToString(), pr.EntrySize.ToString(), pr.EntryPrice.ToString(), pr.EntryCommission.ToString(), pr.ExitTime.ToString(), pr.ExitSize.ToString(), pr.ExitPrice.ToString(), pr.ExitCommission.ToString(), pr.Highest.ToString(), pr.Lowest.ToString(), pr.Size.ToString(), pr.HoldSize.ToString(), pr.Side.ToString(), pr.WL.ToString(), pr.TotalPoints.ToString(), pr.Profit.ToString(), pr.Commissoin.ToString(), pr.NetProfit.ToString(), pr.Type.ToString(),TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
                return db.Connection.Execute(query) > 0;
            }
        }

        

       

        #region 日内数据转储与清空
        /// <summary>
        /// 删除日内委托数据
        /// </summary>
        /// <returns></returns>
        public static bool ClearIntradayOrders(int tradingday=0)
        {
            using (DBMySql db = new DBMySql())
            {
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
                string query = String.Format("delete from tmp_orders where settleday ={0}",tradingday);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 转储orders
        /// </summary>
        /// <returns></returns>
        public static bool DumpIntradayOrders(out int rows)
        {
            using (DBMySql db = new DBMySql())
            {
                rows = 0;
                string query = String.Format("replace into log_orders select * from tmp_orders");
                rows = db.Connection.Execute(query);
                return rows>= 0;
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
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
                string query = String.Format("delete from tmp_trades where settleday={0}",tradingday);
                return db.Connection.Execute(query)>=0;
            }
        }

        /// <summary>
        /// 转储成交数据
        /// </summary>
        /// <returns></returns>
        public static bool DumpIntradayTrades(out int rows)
        {
            using (DBMySql db = new DBMySql())
            {
                rows = 0;
                string query = String.Format("replace into log_trades select * from tmp_trades");
                rows = db.Connection.Execute(query);
                return rows >= 0;
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
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
                string query = String.Format("delete from tmp_orderactions where settleday={0}",tradingday);
                return db.Connection.Execute(query)>=0;
            }
        }
        /// <summary>
        /// 转储日内取消数据到历史取消表
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static bool DumpIntradayOrderActions(out int rows)
        {
            using (DBMySql db = new DBMySql())
            {
                rows = 0;
                string query = String.Format("replace into log_orderactions select * from tmp_orderactions");
                rows = db.Connection.Execute(query);
                return rows >= 0;
            }
        }

        /// <summary>
        /// 转储日内交易回合记录
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static bool DumpIntradayPosTransactions(out int rows)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("replace into log_postransactions select * from tmp_postransactions");
                rows = db.Connection.Execute(query);
                return rows >= 0;
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
                tradingday = (tradingday != 0 ? tradingday : TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
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

    }
}
