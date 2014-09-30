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

        /// <summary>
        /// 注入强平任务
        /// </summary>
        /// <param name="flattime"></param>
        /// <param name="mts"></param>
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
            debug("执行强平任务 对日内帐户执行撤单并强平持仓,强平时间点:" + flattime.ToString(), QSEnumDebugLevel.INFO);
            //1.遍历所有pending orders 如果委托对应的帐户是日内交易并且该委托需要在该强平时间点撤单 则执行撤单
            //查询所有待成交委托 且该委托合约在对应的强平时间点 撤掉将当前强平时间点的所有委托
            foreach (Order od in _clearcentre.TotalOrders.Where(o => o.IsPending() && IsSymbolWithMarketTime(o.oSymbol, mts)))
            { 
                IAccount acc = null;
                if (_clearcentre.HaveAccount(od.Account, out acc))
                {
                    if (!acc.IntraDay) continue;
                    {
                        //取消委托
                        CancelOrder(od, QSEnumOrderSource.RISKCENTRE, "尾盘强平");
                    }
                }
            }

            //2.遍历所有持仓 进行强平
            foreach (Position pos in _clearcentre.TotalPositions)
            {
                IAccount acc = null;
                if (_clearcentre.HaveAccount(pos.Account, out acc))
                {
                    if (!acc.IntraDay) continue;//如果隔夜账户,则不用平仓
                    if (!pos.isFlat && IsSymbolWithMarketTime(pos.oSymbol,mts))//如果有持仓 并且持仓合约绑定在对应的市场交易时间上
                    {
                        this.FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "尾盘强平");
                    }
                    Thread.Sleep(50);
                }
            }
        }
    }
}
