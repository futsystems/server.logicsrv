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
        /// 初始化强平任务 生成强平任务 注入到调度系统
        /// </summary>
        void InitFlatTask()
        {
            logger.Info("初始化日内强平任务");
            Dictionary<DateTime, List<SecurityFamily>> closetimemap = new Dictionary<DateTime, List<SecurityFamily>>();

            //遍历所有品种
            foreach (var sec in BasicTracker.DomainTracker.SuperDomain.GetSecurityFamilies())
            {
                if (sec.MarketTime == null)
                    continue;
                DateTime extime = Util.ToDateTime(sec.Exchange.GetExchangeTime().ToTLDate(), sec.MarketTime.CloseTime);

                DateTime systime = sec.Exchange.ConvertToSystemTime(extime);

                if (!closetimemap.Keys.Contains(systime))
                {
                    closetimemap.Add(systime, new List<SecurityFamily>());
                }
                closetimemap[systime].Add(sec);
            }

            foreach (var key in closetimemap.Keys)
            {
                RegisterMarketCloseFlatTask(key, closetimemap[key]);
            }

        }

        void RegisterMarketCloseFlatTask(DateTime closetime, List<SecurityFamily> list)
        {
            logger.Info("注册收盘强平任务,收盘时间:" + closetime.ToString("HH:mm:ss"));
            DateTime flattime = closetime.AddMinutes(-1*GlobalConfig.FlatTimeAheadOfMarketClose);//提前5分钟强平
            TaskProc task = new TaskProc(this.UUID, "收盘前强平-" + flattime.ToString("HH:mm:ss"), flattime.Hour, flattime.Minute, flattime.Second, delegate() { FlatPositoinBeforeClose(closetime, list); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);

        }

        void FlatPositoinBeforeClose(DateTime close, List<SecurityFamily> list)
        {
            logger.Info("执行收盘前强平操作,对应收盘时间:" + close.ToString("HH:mm:ss"));
            //将品种列表中的pending委托 撤单
            foreach (var o in TLCtxHelper.ModuleClearCentre.TotalOrders.Where(o => o.IsPending() && list.Any(sec => sec.Code == o.oSymbol.SecurityFamily.Code)))
            {
                IAccount acc = TLCtxHelper.ModuleAccountManager[o.Account];
                if (acc != null)
                {
                    if (!acc.IntraDay) continue;
                    CancelOrder(o, QSEnumOrderSource.RISKCENTRE, "尾盘强平");
                    Thread.Sleep(50);
                }
            }

            //等待1秒后再进行强平持仓
            Util.sleep(3000);
            //2.遍历所有持仓 进行强平
            foreach (Position pos in TLCtxHelper.ModuleClearCentre.TotalPositions.Where(p => !p.isFlat && list.Any(sec => sec.Code == p.oSymbol.SecurityFamily.Code)))
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
