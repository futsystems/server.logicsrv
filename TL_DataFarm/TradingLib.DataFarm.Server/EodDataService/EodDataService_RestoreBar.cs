using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.Common.DataFarm
{

    public partial class EodDataService
    {
        void RestoreBar(RestoreTask task)
        {
            try
            {

                //加载从最后一个EodBar结束时间以来的1分钟数据 合并成EodBar数据
                IEnumerable<BarImpl> list = _store.QryBar(task.oSymbol, BarInterval.CustomTime, 60, task.EodHistBarEndTradingDay + 1, int.MaxValue, 0, 0, false);
                IEnumerable<BarImpl> eodlist = BarMerger.MergeEOD(list);
                logger.Info(string.Format("Symbol:{0} Restore Bar from:{1} cnt:{2}", task.oSymbol.Symbol, task.EodHistBarEndTradingDay, eodlist.Count()));
                //数据恢复后 日线数据最后一条数据最关键，该数据如果在收盘时刻启动则日线完备，如果在盘中启动则日线不完备
                //如果数据操作由延迟，导致已经有完整的1分钟Bar数据到达，而日线数据还没有回复完毕，则我们将1分钟数据先放到list中，待日线数据恢复完毕后再用该数据执行驱动 PartialBar则只要更新最后一个
                //1分钟的Bar数据只是用来更新EOD的高开低收 以及没有实时行情时候的成交量数据 待回复完毕后由实时Tick负责更新


                //将除了最后一条数据之前的数据统一对外发送到BarList 更新数据集并保存 用最后一条数据 创建EodBarStruct放入map
                //如果最后一个Bar已经完备，则新生成的1分钟Bar 会导致关闭该Bar 如果在同一个交易日内则进行数据更新直至收盘

                //将恢复完毕的日级别数据 发送到Barlist
                if (EodBarResotred != null)
                {
                    EodBarResotred(task.oSymbol, eodlist.Take(Math.Max(0, eodlist.Count() - 1)));//最后一个Bar不更新到缓存 放入EodBarStruct 作为EODPartial来处理,通过1分钟K线的处理来决定是否关闭该EODBar
                }

                //用最后一个Eod 创建struct
                BarImpl lastbar = eodlist.LastOrDefault();
                EodBarStruct st = null;
                if (lastbar != null)
                {
                    st = new EodBarStruct(task.oSymbol, lastbar, lastbar.Volume);
                }
                else
                {
                    st = new EodBarStruct(task.oSymbol, null, 0);
                }
                //将EODStruct放入Map
                eodBarMap[task.oSymbol.UniqueKey] = st;

             
                //合并实时系统产生的数据
                //如果操作执行完成前已经有最新的Bar数据到达，则将这些没有处理的数据应用到当前EODPartial
                List<BarImpl> minbarlist = null;
                if (eodPendingMinBarMap.TryGetValue(task.oSymbol.UniqueKey, out minbarlist))
                {
                    foreach (var bar in minbarlist.ToArray())
                    {
                        On1MinBarClose(task.oSymbol, bar);
                    }
                    eodPendingMinBarMap.Remove(task.oSymbol.UniqueKey);
                }

                BarImpl partialBar = null;
                if (eodPendingMinPartialBarMap.TryGetValue(task.oSymbol.UniqueKey, out partialBar))
                {
                    On1MinPartialBarUpdate(task.oSymbol, partialBar);
                    eodPendingMinPartialBarMap.Remove(task.oSymbol.UniqueKey);
                }
                //停盘时刻 Bar和PartialBar都没有 OnTick中需要执行 UpdateEodPartialBar(eod);否则数据集中没有PartialBar导致 无法获得当天的日线数据 由于PartialBar为同一个，所以第一次UpdateEodPartialBar(eod)之后 后续的update只是做相同的赋值操作
                //数据恢复完毕后
                st.Restored = true;
                UpdateEodPartialBar(st);
            }
            catch (Exception ex)
            {
                task.IsEODRestoreSuccess = false;
                logger.Error(string.Format("Symbol:{0} EOD Restore Error:{1}", task.oSymbol.Symbol, ex.ToString()));
            }
        }
    }
}
