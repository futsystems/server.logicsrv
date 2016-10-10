using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    
    internal class EodBarStruct
    {
        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// Eod日内Bar数据
        /// </summary>
        public BarImpl EODBar { get; set; }

        /// <summary>
        /// 已关闭的1分钟Bar成交量累加
        /// </summary>
        public int ClosedVol { get; set; }
    }


    /// <summary>
    /// 日线级别数据服务
    /// 日内数据通过Tick数据驱动，日线级别的数据直接通过分钟线进行驱动即可
    /// 如果通过Tick驱动则会进行大量的数据运算与交易日判定，通过1分钟线数据进行驱动则数据储量减少到原来的1/60可以满足要求
    /// 
    /// 关于交易日处理
    /// 1.定时任务在交易所收盘后 执行收盘操作 更新交易日,结算价,持仓量等信息更新
    /// 2.分钟数据关闭时 计算结算日
    /// </summary>
    public class EodDataService
    {

        /// <summary>
        /// 日线数据更新事件
        /// </summary>
        public event Action<EodBarEventArgs> EodBarUpdate;

        /// <summary>
        /// 日线数据关闭
        /// 该事件由定时任务根据交易所收盘时间进行触发
        /// </summary>
        public event Action<EodBarEventArgs> EodBarClose;

        /// <summary>
        /// 保存当前交易日日线数据
        /// </summary>
        Dictionary<string, EodBarStruct> eodBarMap = new Dictionary<string, EodBarStruct>();


        /// <summary>
        /// 维护交易所对应交易日
        /// </summary>
        Dictionary<string, int> tradingdayMap = new Dictionary<string, int>();

        public EodDataService()
        {
            //获得当前交易日
            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            { 
                
            }
        
        }

        /// <summary>
        /// 判定某个合约交易所当前时间对应交易日
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int GetTradingDay(SecurityFamily sec)
        {
            return GetTradingDay(sec, sec.Exchange.GetExchangeTime());
        }

        /// <summary>
        /// 判定某个合约交易日信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="exTime"></param>
        /// <returns></returns>
        public int GetTradingDay(SecurityFamily sec, DateTime exTime)
        {
            TradingRange range = sec.MarketTime.JudgeRange(exTime);//根据交易所时间判定当前品种所属交易小节
            if (range == null) return 0;
            DateTime tradingday = range.TradingDay(exTime);
            //if (sec.Exchange.IsInHoliday(tradingday)) return 0;
            return tradingday.ToTLDate();
        }


        /// <summary>
        /// 判定交易日
        /// 00:00:00->夜盘收盘
        /// 日盘开盘->日盘收盘 交易所收盘
        /// 夜盘开盘->00:00:00
        /// 
        /// 00:00:00->MarketClose T         当天凌晨->收盘 交易日为当天
        /// MarketClose->12:59:59 T+1       收盘->12:59:59 交易日为下一个交易
        /// 
        /// </summary>
        /// <param name="exTime"></param>
        /// <returns></returns>
        //public int GetTradingDay(DateTime exTime)
        //{
            
        //}

        //IExchange exchange = sec.Exchange;
        //DateTime extime = exchange.GetExchangeTime();//获得交易所时间
        //TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节
        // settleday = 0;
        //    if (range == null)
        //    {
        //        settleday = 0;
        //        return QSEnumActionCheckResult.RangeNotExist;
        //    }

        //    //获得当前交易小节所属交易日 如果该交易日放假
        //    DateTime tradingday = range.TradingDay(extime);
        //    if (exchange.IsInHoliday(tradingday))
        //    {
        //        settleday = 0;
        //        return QSEnumActionCheckResult.InHoliday;
        //    }


        /// <summary>
        /// 1分钟Bar数据更新时 更新当前日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bar"></param>
        public void On1MinPartialBar(Symbol symbol, BarImpl bar)
        {
            EodBarStruct eod = null;
            if (eodBarMap.TryGetValue(symbol.UniqueKey, out eod))
            {
                eod.EODBar.High = Math.Max(eod.EODBar.High, bar.High);
                eod.EODBar.Low = Math.Max(eod.EODBar.Low, bar.Low);
                eod.EODBar.Close = bar.Close;
                eod.EODBar.Volume = eod.ClosedVol + bar.Volume;

                //触发Eod更新事件 用于更新到BarList
                UpdateEodPartialBar(eod);
            }
        }

        /// <summary>
        /// 1分钟Bar关闭后 更新当前日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bar"></param>
        public void On1MinBar(Symbol symbol, BarImpl bar)
        {
            EodBarStruct eod = null;
            if (eodBarMap.TryGetValue(symbol.UniqueKey, out eod))
            {
                eod.EODBar.High = Math.Max(eod.EODBar.High, bar.High);
                eod.EODBar.Low = Math.Max(eod.EODBar.Low, bar.Low);
                eod.EODBar.Close = bar.Close;
                eod.EODBar.Volume = eod.ClosedVol + bar.Volume;
                eod.ClosedVol = eod.ClosedVol + bar.Volume;//将当前1分钟的成交量计入ClosedVol
                UpdateEodPartialBar(eod);
            }
        }

        /// <summary>
        /// 更新日级别Bar数据
        /// </summary>
        /// <param name="eod"></param>
        void UpdateEodPartialBar(EodBarStruct eod)
        {
            if (EodBarUpdate != null)
            {
                EodBarUpdate(new EodBarEventArgs(eod.Symbol, eod.EODBar));
            }
        }

        /// <summary>
        /// 关闭日级别Bar数据 
        /// </summary>
        /// <param name="eod"></param>
        void CloseEodPartialBar(EodBarStruct eod)
        {
            if (EodBarClose != null)
            {
                EodBarClose(new EodBarEventArgs(eod.Symbol, eod.EODBar));
            }
        
        }

    }
}
