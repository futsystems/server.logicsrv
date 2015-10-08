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
            logger.Info("初始化日内强平任务");
            Dictionary<int, List<MarketTime>> flatlist = new Dictionary<int, List<MarketTime>>();
            //foreach (FlatTimeMarketTimePair p in BasicTracker.MarketTimeTracker.GetFlatTimeMarketTimePairs())
            //{
            //    //debug("xxxxxxxxxxxxx flattime:" + p.FlatTime.ToString() + " marketime id:" + p.MarketTime.ID, QSEnumDebugLevel.INFO);
            //    if (!flatlist.Keys.Contains(p.FlatTime))
            //    {
            //        flatlist[p.FlatTime] = new List<MarketTime>();
            //    }
            //    flatlist[p.FlatTime].Add(p.MarketTime);
            //}

            //foreach (int flattime in flatlist.Keys)
            //{
            //    InjectTask(flattime, flatlist[flattime].ToArray());
            //}

            

        }

        //2:30 到期合约执行强平
        [TaskAttr("强平-合约交割",14,50,0, "合约交割日执行强平")]
        public void Task_FlatPositionViaExpiredDate()
        {

            foreach (Position pos in TLCtxHelper.ModuleClearCentre.TotalPositions.Where(p => !p.isFlat && p.oSymbol.IsExpiredToday()))
            {
                this.FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "合约交割强平");
                Thread.Sleep(50);
            }
        }

        
        /// <summary>
        /// 注入强平任务
        /// </summary>
        /// <param name="flattime"></param>
        /// <param name="mts"></param>
        void InjectTask(int flattime, MarketTime[] mts)
        {
            //注入强平任务
            logger.Info("注入强平任务,强平时间点:" + flattime);
            DateTime t = Util.ToDateTime(Util.ToTLDate(), flattime);
            TaskProc task = new TaskProc(this.UUID, "强平-日内" + flattime.ToString(), t.Hour, t.Minute, t.Second, delegate() { FlatPositionViaMarketTime(flattime,mts); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
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
            logger.Info("执行强平任务 对日内帐户执行撤单并强平持仓,强平时间点:" + flattime.ToString());
            //1.遍历所有pending orders 如果委托对应的帐户是日内交易并且该委托需要在该强平时间点撤单 则执行撤单
            //查询所有待成交委托 且该委托合约在对应的强平时间点 撤掉将当前强平时间点的所有委托
            foreach (Order od in TLCtxHelper.ModuleClearCentre.TotalOrders.Where(o => o.IsPending() && IsSymbolWithMarketTime(o.oSymbol, mts)))
            {
                logger.Info("symbol:" + od.Symbol + "order status:" + od.IsPending().ToString() + " withmarkettime:" + IsSymbolWithMarketTime(od.oSymbol, mts));
                //if (od.IsPending() && IsSymbolWithMarketTime(od.oSymbol, mts))
                //{
                IAccount acc = TLCtxHelper.ModuleAccountManager[od.Account];
                if (acc != null)
                {
                    if (!acc.IntraDay) continue;
                    {
                        //取消委托
                        CancelOrder(od, QSEnumOrderSource.RISKCENTRE, "尾盘强平");
                    }
                }
            }

            //等待1秒后再进行强平持仓
            Util.sleep(3000);
            //2.遍历所有持仓 进行强平
            foreach (Position pos in TLCtxHelper.ModuleClearCentre.TotalPositions.Where(p => !p.isFlat && IsSymbolWithMarketTime(p.oSymbol, mts)))
            {
                IAccount acc = TLCtxHelper.ModuleAccountManager[pos.Account];
                if (acc != null)
                {
                    if (!acc.IntraDay) continue;//如果隔夜账户,则不用平仓
                    this.FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "尾盘强平");
                    Thread.Sleep(50);
                }
            }
        }
    }
}
