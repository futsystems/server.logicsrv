﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    public class BarMerger
    {
        /// <summary>
        /// 将1分钟Bar生成不同频率的Bar
        /// </summary>
        /// <param name="source"></param>
        /// <param name="span"></param>
        public static List<BarImpl> Merge(IEnumerable<BarImpl> source, TimeSpan span)
        {
            //List<DateTime> targetEnd = new List<DateTime>();
            //DateTime end = DateTime.MinValue;
            //获得Bar时间对应周期的结束时间
            //foreach (var bar in source)
            //{
            //    targetEnd.Add(TimeFrequency.BarEndTime(bar.EndTime.AddMinutes(-1), span));
            //}


            int j = -1;
            List<BarImpl> target = new List<BarImpl>();
            BarImpl tmp = null;
            BarImpl sbar = null;
            DateTime currentEnd = DateTime.MinValue;
            DateTime targetEnd = DateTime.MinValue;
            for (int i=0;i<source.Count();i++)
            {
                sbar = source.ElementAt(i);
                targetEnd = TimeFrequency.BarEndTime(sbar.EndTime.AddMinutes(-1), span);
                if (currentEnd != targetEnd)
                {
                    j++;
                    //正常跨越了一个Bar 则标记上一个Bar为完整 BarImpl增加一个MergeComplete标记
                    //if (tmp != null)
                    //{
                    //    tmp.MergeComplete = true;
                    //}
                    tmp = new BarImpl();
                    tmp.EndTime = targetEnd;
                    target.Add(tmp);
                    tmp.Open = sbar.Open;
                    tmp.High = sbar.Open;
                    tmp.Low = sbar.Low;
                    tmp.Close = sbar.Close;
                }
                tmp = target[j];
                tmp.High = Math.Max(tmp.High, sbar.High);
                tmp.Low = Math.Min(tmp.Low, sbar.Low);
                tmp.Close = sbar.Close;
                tmp.Volume += sbar.Volume;

                //if (sbar.EndTime == targetEnd)//如果source bar对应的结束时间与目标周期的结束时间一致,则该Bar结束
                //{
                //    tmp.MergeComplete = true;
                //}

                currentEnd = targetEnd;
            }
            //tmp = target.Last();
            return target;
        }
    }
}
