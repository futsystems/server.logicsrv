using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class RiskCentre
    {

        /// <summary>
        /// 判断合约是否绑定在某个市场交易时间对象上
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="mt"></param>
        /// <returns></returns>
        bool IsSymbolWithMarketTime(Symbol sym, MarketTime mt)
        {
            if (sym == null)
                return false;

            if (sym.SecurityFamily != null && sym.SecurityFamily.MarketTime != null)
            {
                if (sym.SecurityFamily.MarketTime.Equals(mt))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断某个合约的交易时间对象是否在一个交易时间对象组中
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="mts"></param>
        /// <returns></returns>
        bool IsSymbolWithMarketTime(Symbol sym, MarketTime[] mts)
        {
            foreach (MarketTime mt in mts)
            {
                if (IsSymbolWithMarketTime(sym, mt))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 初始化强平任务 生成强平任务 注入到调度系统
        /// </summary>
        void InitFlatTask()
        {
            debug("初始化日内强平任务", QSEnumDebugLevel.INFO);
            Dictionary<int, List<MarketTime>> flatlist = new Dictionary<int, List<MarketTime>>();
            foreach (FlatTimeMarketTimePair p in BasicTracker.MarketTimeTracker.GetFlatTimeMarketTimePairs())
            {
                //debug("xxxxxxxxxxxxx flattime:" + p.FlatTime.ToString() + " marketime id:" + p.MarketTime.ID, QSEnumDebugLevel.INFO);
                if (!flatlist.Keys.Contains(p.FlatTime))
                {
                    flatlist[p.FlatTime] = new List<MarketTime>();
                    
                }
                flatlist[p.FlatTime].Add(p.MarketTime);
            }

            foreach (int flattime in flatlist.Keys)
            {
                InjectTask(flattime, flatlist[flattime].ToArray());
            }
        }


        void InjectTask(int flattime, MarketTime[] mts)
        {
            //注入强平任务
            debug("注入强平任务,强平时间点:" + flattime,QSEnumDebugLevel.INFO);
            DateTime t = Util.ToDateTime(Util.ToTLDate(), flattime);
            TaskProc task = new TaskProc(this.UUID, "日内强平-" + flattime.ToString(), t.Hour, t.Minute, t.Second, delegate() { FlatPositionViaMarketTime(flattime,mts); });
            TLCtxHelper.Ctx.InjectTask(task);
        }

        string MarketTimeIDstr(MarketTime[] mts)
        {
            string str = string.Empty;
            foreach (MarketTime mt in mts)
            {
                str = str + mt.ID + ",";
            }
            return str;
        }
        /// <summary>
        /// 强平绑定某个市场交易时间的所有持仓
        /// </summary>
        /// <param name="mt"></param>
        void FlatPositionViaMarketTime(int flattime,MarketTime[] mts)
        {
            debug("执行强平任务,强平时间点:" + flattime.ToString() + " 交易时间对象ID:" + MarketTimeIDstr(mts), QSEnumDebugLevel.INFO);
            PositionTracker pt = _clearcentre.DefaultPositionTracker as PositionTracker;
            foreach (Position pos in pt)
            {
                IAccount acc = null;
                if (_clearcentre.HaveAccount(pos.Account, out acc))
                {
                    if (!acc.IntraDay) continue;//如果隔夜账户,则不用平仓
                    if (!pos.isFlat && IsSymbolWithMarketTime(pos.oSymbol,mts))//如果有持仓 并且持仓合约绑定在对应的市场交易时间上
                    {
                        Order o = new MarketOrderFlat(pos);
                        o.Account = pos.Account;
                        o.OrderSource = QSEnumOrderSource.RISKCENTRE;
                        o.comment = "风控日内强平";
                        this.SendOrder(o);
                    }
                    Thread.Sleep(50);
                }
            }
        }



        //#region 定时撤销委托与平仓
        ///// <summary>
        ///// 收盘前平掉所有商品仓位
        ///// </summary>
        //[TaskAttr("FlatPositionCommunity_n", 2, 25, 35, "夜盘收盘前平掉商品持仓")]
        //[TaskAttr("FlatPositionCommunity_d", 14, 55, 35, "夜盘收盘前平掉商品持仓")]
        //public void Task_FlatPosition_Community()
        //{
        //    if (!IsTradingday) return;
        //    PositionTracker pt = _clearcentre.DefaultPositionTracker as PositionTracker;

        //    debug("全平所有账户商品仓位,共计仓位:" + pt.Count.ToString(), QSEnumDebugLevel.INFO);
        //    //遍历所有持仓开始平仓
        //    foreach (Position pos in pt)
        //    {
        //        IAccount acc = null;
        //        if (_clearcentre.HaveAccount(pos.Account, out acc))
        //        {
        //            if (!acc.IntraDay) continue;//如果隔夜账户,则不用平仓
        //            if (!pos.isFlat && pos.oSymbol.SecurityFamily.Code!="IF")
        //            {
        //                Order o = new MarketOrderFlat(pos);
        //                o.Account = pos.Account;
        //                o.OrderSource = QSEnumOrderSource.RISKCENTRE;
        //                o.comment = "风控中心尾盘强平";
        //                this.SendOrder(o);
        //            }
        //            Thread.Sleep(50);
        //        }
        //    }
        //    Notify("全平商品[" + DateTime.Now.ToString() + "]", " ");
        //}

        ///// <summary>
        ///// 收盘前平掉所有股指仓位
        ///// </summary>
        //[TaskAttr("FlatPositionIF", 15, 10, 35, "日盘收盘前平掉股指持仓")]
        //public void Task_FlatPosition_IF()
        //{
        //    if (!IsTradingday) return;
        //    PositionTracker pt = _clearcentre.DefaultPositionTracker as PositionTracker;
        //    debug("全平所有账户股指仓位:" + pt.Count.ToString(), QSEnumDebugLevel.INFO);

        //    foreach (Position pos in pt)
        //    {
        //        IAccount acc = null;
        //        if (_clearcentre.HaveAccount(pos.Account, out acc))
        //        {
        //            if (!acc.IntraDay) continue;//如果隔夜账户,则不用平仓
        //            if (!pos.isFlat && pos.oSymbol.SecurityFamily.Code == "IF")
        //            {
        //                Order o = new MarketOrderFlat(pos);
        //                o.Account = pos.Account;
        //                o.OrderSource = QSEnumOrderSource.CLEARCENTRE;
        //                o.comment = "风控中心尾盘强平";
        //                this.SendOrder(o);
        //            }
        //            Thread.Sleep(50);
        //        }
        //    }
        //    Notify("全平股指[" + DateTime.Now.ToString() + "]", " ");
        //}
        ///// <summary>
        ///// 撤掉所有商品委托
        ///// </summary>
        //[TaskAttr("ClearOrderCommunity_n", 2, 25, 5, "夜盘收盘前撤掉商品委托")]
        //[TaskAttr("ClearOrderCommunity_d", 14, 55, 5, "日盘收盘前撤掉商品委托")]
        //public void Task_ClearOrders_Community()
        //{
        //    if (!IsTradingday && !CoreUtil.IsSat230()) return;
        //    OrderTracker ot = _clearcentre.DefaultOrderTracker as OrderTracker;
        //    debug("全撤所有账户商品委托", QSEnumDebugLevel.INFO);
        //    foreach (Order o in ot.getPendingOrders())
        //    {
        //        if (ot.isPending(o.id) && o.oSymbol.SecurityFamily.Code != "IF")
        //        {
        //            this.CancelOrder(o.id);
        //            Thread.Sleep(10);
        //        }
        //    }
        //    Notify("全撤商品[" + DateTime.Now.ToString() + "]", " ");
        //}
        ///// <summary>
        ///// 撤掉所有股指委托
        ///// </summary>
        //[TaskAttr("ClearOrderIF", 15, 10, 5, "日盘收盘前撤掉股指委托")]
        //public void Task_ClearOrders_IF()
        //{
        //    if (!IsTradingday) return;
        //    OrderTracker ot = _clearcentre.DefaultOrderTracker as OrderTracker;

        //    debug("全撤所有账户股指委托", QSEnumDebugLevel.INFO);
        //    foreach (Order o in ot.getPendingOrders())
        //    {
        //        if (ot.isPending(o.id) && o.oSymbol.SecurityFamily.Code == "IF")
        //        {
        //            this.CancelOrder(o.id);
        //            Thread.Sleep(10);
        //        }
        //    }
        //    Notify("全撤股指[" + DateTime.Now.ToString() + "]", " ");
        //}

        
        //#endregion
    }
}
