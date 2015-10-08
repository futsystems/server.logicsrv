using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TradingLib.API;
using TradingLib.Common;


namespace TL_Test
{
    [TestFixture]
    public class TestMarketTime
    {
        TradingRange range_sameday = new TradingRangeImpl(DayOfWeek.Monday, 90000, DayOfWeek.Monday, 120000);//星期一 9:00:00到12:00:00
        
        DateTime t1 = new DateTime(2015, 10, 8, 8, 0, 0);//星期四八点
        DateTime t2_1 = new DateTime(2015, 10, 5, 8, 0, 0);//星期一 8:00:00
        DateTime t2_2 = new DateTime(2015, 10, 5, 9, 0, 0);//星期一 9:00:00
        DateTime t2_3 = new DateTime(2015, 10, 5, 9, 0, 01);//星期一 9:00:01
        DateTime t2_4 = new DateTime(2015, 10, 5, 12, 0, 0);//星期一 12:00:00
        DateTime t2_5 = new DateTime(2015, 10, 5, 12, 0, 01);//星期一12:00:01

        DateTime t2_6 = new DateTime(2015, 10, 5, 22, 0, 0);//星期一 22:00:00


        DateTime t3_1 = new DateTime(2015, 10, 6, 8, 0, 0);//星期二 8:00:00
        DateTime t3_2 = new DateTime(2015, 10, 6, 9, 0, 0);//星期二 9:00:00
        DateTime t3_3 = new DateTime(2015, 10, 6, 20, 0, 0);//星期二 20:00:00
        DateTime t3_4 = new DateTime(2015, 10, 6, 22, 0, 0);//星期二 22:00:00

        /// <summary>
        /// 判定一个时间是否在交易小节之内
        /// 交易小节为某一天的一个时间段
        /// </summary>
        [Test]
        public void InRange_OneDay()
        {
            

            bool inrange = false;

            inrange = range_sameday.IsInRange(t2_1);
            Assert.False(inrange);
            inrange = range_sameday.IsInRange(t2_2);
            Assert.True(inrange);
            inrange = range_sameday.IsInRange(t2_3);
            Assert.True(inrange);
            inrange = range_sameday.IsInRange(t2_4);
            Assert.True(inrange);
            inrange = range_sameday.IsInRange(t2_5);
            Assert.False(inrange);
        }


        TradingRange range_mutilday = new TradingRangeImpl(DayOfWeek.Monday, 90000, DayOfWeek.Tuesday, 200000);//星期一 9:0000 到 星期二 20:00:00
        /// <summary>
        /// 判定一个时间是否在交易小节内，
        /// 交易小节横跨了几天
        /// </summary>
        [Test]
        public void InRange_MutilDay()
        {
            bool inrange = false;
            inrange = range_mutilday.IsInRange(t2_1);
            Assert.False(inrange);
            inrange = range_mutilday.IsInRange(t2_2);
            Assert.True(inrange);
            inrange = range_mutilday.IsInRange(t2_3);
            Assert.True(inrange);
            inrange = range_mutilday.IsInRange(t2_4);
            Assert.True(inrange);
            inrange = range_mutilday.IsInRange(t2_5);
            Assert.True(inrange);
            inrange = range_mutilday.IsInRange(t2_6);
            Assert.True(inrange);

            inrange = range_mutilday.IsInRange(t3_1);
            Assert.True(inrange);
            inrange = range_mutilday.IsInRange(t3_2);
            Assert.True(inrange);
            inrange = range_mutilday.IsInRange(t3_3);
            Assert.True(inrange);
            inrange = range_mutilday.IsInRange(t3_4);
            Assert.False(inrange);

        }

        TradingRange range_mutilday_sunday = new TradingRangeImpl(DayOfWeek.Saturday, 90000, DayOfWeek.Monday, 200000);//星期六 9:00:00 到 星期一 20:00:00

        DateTime t4_1 = new DateTime(2015, 10, 10, 8, 0, 0);//星期六 8:00:00
        DateTime t4_2 = new DateTime(2015, 10, 10, 9, 0, 0);//星期六 9:00:00
        DateTime t4_3 = new DateTime(2015, 10, 10, 23, 59, 59);//星期六 23:59:59
        DateTime t4_4 = new DateTime(2015, 10, 11, 0, 0, 0);//星期日 0:0:0
        DateTime t4_5 = new DateTime(2015, 10, 11, 9, 0, 0);//星期日 9:0:0
        DateTime t4_6 = new DateTime(2015, 10, 12, 9, 0, 0);//星期一 9:0:0
        DateTime t4_7 = new DateTime(2015, 10, 12, 21, 0, 0);//星期一 21:0:0

        [Test]
        public void InRange_MutilDay_sunday()
        {
            bool inrange = false;
            inrange = range_mutilday_sunday.IsInRange(t4_1);
            Assert.False(inrange);
            inrange = range_mutilday_sunday.IsInRange(t4_2);
            Assert.True(inrange);
            inrange = range_mutilday_sunday.IsInRange(t4_3);
            Assert.True(inrange);
            inrange = range_mutilday_sunday.IsInRange(t4_4);
            Assert.True(inrange);
            inrange = range_mutilday_sunday.IsInRange(t4_5);
            Assert.True(inrange);
            inrange = range_mutilday_sunday.IsInRange(t4_6);
            Assert.True(inrange);
            inrange = range_mutilday_sunday.IsInRange(t4_7);
            Assert.False(inrange);
        }

        //开始和结束在同一天
        TradingRange range_sameday_t = new TradingRangeImpl(DayOfWeek.Tuesday, 90000, DayOfWeek.Tuesday, 200000);
        TradingRange range_sameday_t1 = new TradingRangeImpl(DayOfWeek.Tuesday, 90000, DayOfWeek.Tuesday, 200000, QSEnumRangeSettleFlag.T1);
        
        DateTime t5_1 = new DateTime(2015, 10, 12, 12, 0, 0);//星期一 12:0:0
        DateTime t5_2 = new DateTime(2015, 10, 13, 12, 0, 0);//星期二 12:0:0

        
        //跨越周末

        [Test]
        public void TradingDay_Sameday()
        {
            DateTime dt = DateTime.Now;
            //dt = range_01.TradingDay(t5_1);

            dt = range_sameday_t.TradingDay(t5_2);
            Assert.AreEqual(Util.ToTLDate(dt), 20151013);

            dt = range_sameday_t1.TradingDay(t5_2);
            Assert.AreEqual(Util.ToTLDate(dt), 20151014);

            
        }

        //跨越多个交易日
        TradingRange range_mutilday_t = new TradingRangeImpl(DayOfWeek.Tuesday, 210000, DayOfWeek.Wednesday, 20000, QSEnumRangeSettleFlag.T);
        TradingRange range_mutilday_t1 = new TradingRangeImpl(DayOfWeek.Tuesday, 210000, DayOfWeek.Wednesday, 20000, QSEnumRangeSettleFlag.T1);
        DateTime t5_3 = new DateTime(2015, 10, 13, 21, 1, 0);//星期二 21:01:0 在前一个日期
        DateTime t5_4 = new DateTime(2015, 10, 14, 1, 0, 0);//星期三 1:00:00 在后一个日期

        [Test]
        public void TradingDay_Mutilday()
        {
            DateTime dt = DateTime.Now;
            dt = range_mutilday_t.TradingDay(t5_3);
            Assert.AreEqual(Util.ToTLDate(dt), 20151013);

            dt = range_mutilday_t.TradingDay(t5_4);
            Assert.AreEqual(Util.ToTLDate(dt), 20151013);


            dt = range_mutilday_t1.TradingDay(t5_3);
            Assert.AreEqual(Util.ToTLDate(dt), 20151014);

            dt = range_mutilday_t1.TradingDay(t5_4);
            Assert.AreEqual(Util.ToTLDate(dt), 20151014);
        }

        //跨越周末 周五的T1交易日 顺延到星期一
        TradingRange range_week_t = new TradingRangeImpl(DayOfWeek.Friday, 210000, DayOfWeek.Saturday, 20000, QSEnumRangeSettleFlag.T);
        TradingRange range_week_t1 = new TradingRangeImpl(DayOfWeek.Friday, 210000, DayOfWeek.Saturday, 20000, QSEnumRangeSettleFlag.T1);
        DateTime t5_5 = new DateTime(2015, 10, 16, 21, 1, 0);//星期二 21:01:0 在前一个日期
        DateTime t5_6 = new DateTime(2015, 10, 17, 1, 0, 0);//星期三 1:00:00 在后一个日期
        [Test]
        public void TradingDay_Weekend()
        {
            DateTime dt = DateTime.Now;
            dt = range_week_t.TradingDay(t5_5);
            Assert.AreEqual(Util.ToTLDate(dt), 20151016);

            dt = range_week_t.TradingDay(t5_6);
            Assert.AreEqual(Util.ToTLDate(dt), 20151016);


            dt = range_week_t1.TradingDay(t5_5);
            Assert.AreEqual(Util.ToTLDate(dt), 20151019);

            dt = range_week_t1.TradingDay(t5_6);
            Assert.AreEqual(Util.ToTLDate(dt), 20151019);
        }


        TradingRange range_sunday_t1 = new TradingRangeImpl(DayOfWeek.Sunday, 170000, DayOfWeek.Monday, 160000, QSEnumRangeSettleFlag.T);
        DateTime t5_7 = new DateTime(2015, 10, 11, 17, 0, 1);//星期日 17:00:01 在前一个日期
        DateTime t5_8 = new DateTime(2015, 10, 12, 15, 0, 1);//星期一 15:00:01 在后一个日期
        [Test]
        public void TradingDay_Sunday()
        {
            DateTime dt = DateTime.Now;
            dt = range_sunday_t1.TradingDay(t5_7);
            Assert.AreEqual(Util.ToTLDate(dt), 20151012);

            dt = range_sunday_t1.TradingDay(t5_8);
            Assert.AreEqual(Util.ToTLDate(dt), 20151012);
        }
    }
}
