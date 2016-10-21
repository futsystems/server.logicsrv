﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{

    /// <summary>
    /// 分时数据缓存
    /// </summary>
    public class MinuteDataCache
    {

        
        public MinuteDataCache(Symbol symbol,MarketDay marketday,bool restored=false)
        {
            this.Symbol = symbol;
            this.MinuteDataMap = new Dictionary<long, MinuteData>();
            this.MarketDay = marketday;
            this._restored = restored;
            this.TradingDay = marketday.TradingDay;

            int i=0;
            //初始化所有分时数据
            foreach (var session in this.MarketDay.MarketSessions)
            {
                for (DateTime d = session.Start.AddMinutes(1); d <= session.End; d = d.AddMinutes(1))
                {
                    long key = d.ToTLDateTime();
                    this.MinuteDataMap.Add(key, new MinuteData(d.ToTLDate(), d.ToTLTime(),-1, 0, 0));
                    locationMap.Add(key, i);
                    i++;
                }
            }
        }

        

        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// MarketDay
        /// </summary>
        public MarketDay MarketDay { get; set; }

        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay { get; set; }

        /// <summary>
        /// 某个交易日的分时数据
        /// </summary>
        public Dictionary<long, MinuteData> MinuteDataMap { get; set; }

        Dictionary<long, int> locationMap = new Dictionary<long, int>();

        bool _restored = false;
        object _object = new object();


        long _latestBarKey = 0;
        public void On1MinBarClosed(Bar bar)
        {
            if (_restored)
            {
                this.GotBar(bar);
            }
        }

        public void On1MinPartialBarUpdate(Bar bar)
        {
            if (_restored)
            {
                this.GotBar(bar);
            }
        }

        /// <summary>
        /// 恢复1分钟Bar数据
        /// 启动后 当行情源1分钟过去之后 系统开始恢复历史Tick数据
        /// 恢复完毕后再开始接受实时数据
        /// </summary>
        /// <param name="k"></param>
        public void RestoreMinuteData(List<BarImpl> barList)
        {
                foreach (var bar in barList)
                {
                    GotBar(bar);
                }
                _restored = true;
        }

        /// <summary>
        /// 实时Bar数据生成服务 需要过滤非1分钟周期的数据 否则1小时Bar进来会将最近的事件更新到1小时周期上， 获得图像就是右侧有一条水平线
        /// </summary>
        /// <param name="bar"></param>
        void GotBar(Bar bar)
        {
            MinuteData target = null;
            //找到对应的分时数据更新数值
            long key = bar.EndTime.ToTLDateTime();
            if (this.MinuteDataMap.TryGetValue(bar.EndTime.ToTLDateTime(), out target))
            {
                target.Close = bar.Close;
                target.Vol = bar.Volume;
                target.AvgPrice = 0;

                if (key > _latestBarKey)
                {
                    _latestBarKey = key;
                }
                //获得当前序号
                int idx = locationMap[key];

                //当天第一分钟直接赋值,其余的需要检查前面的数据 是否为空
                if (idx != 0)
                {
                    List<MinuteData> zeroDataList = new List<MinuteData>();
                    MinuteData firstValueData = null;
                    for (int i = idx - 1; i >= 0; i--)
                    {
                        MinuteData md = this.MinuteDataMap.ElementAt(i).Value;
                        if (md.Vol == 0)
                        {
                            zeroDataList.Add(md);
                        }
                        else //中间没有间隔有数据记录，前面的空的数据对象已被填充，遇到有数据的直接跳出
                        {
                            firstValueData = md;
                            break;
                        }
                    }

                    //如果遍历到第一个数据都没有有效数据 则取当前分时为有效数据更新前面的空缺数据
                    if (firstValueData == null)
                    {
                        firstValueData = target;
                    }
                    if (firstValueData != null)
                    {
                        foreach (var d in zeroDataList)
                        {
                            d.Close = firstValueData.Close;
                        }
                    }
                    //else//当前值前面所有的数据都没有 则取昨日收盘价/结算价
                    //{

                    //    Tick snapshot = Global.TickTracker[this.Symbol.Exchange, this.Symbol.Symbol];
                    //    if (snapshot != null)
                    //    {
                    //        //获得昨日收盘价/结算价
                    //        decimal price = GetYdPrice(snapshot,this.Symbol);
                    //        foreach (var d in zeroDataList)
                    //        {
                    //            d.Close = (double)price;
                    //        }
                    //    }
                    //}
                }
            }
        }

        //decimal GetYdPrice(Tick k, Symbol symbol)
        //{
        //    switch (symbol.SecurityType)
        //    { 
        //        case SecurityType.FUT:
        //            return k.PreSettlement;
        //        case SecurityType.STK:
        //            return k.PreClose;
        //        default:
        //            return k.PreClose;
        //    }
        //    //如果是新上合约 没有昨日收盘价格或结算价 则取今日开盘价
        //}

        /// <summary>
         /// 查询成交数据
         /// </summary>
         /// <param name="startIndex"></param>
         /// <param name="count"></param>
         /// <returns></returns>
        public List<MinuteData> QryMinuteDate(DateTime start)
        {
            if (start == DateTime.MinValue)
            {
                return this.MinuteDataMap.Where(d => d.Key <= _latestBarKey).Select(d => d.Value).ToList();
            }
            else
            {
                return this.MinuteDataMap.Where(d => d.Key <= _latestBarKey).Select(d => d.Value).Where(d => d.DateTime >= start).ToList();
                
            }
        }

    }
}