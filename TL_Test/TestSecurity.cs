using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;

namespace TL_Test
{
    [TestFixture]
    public class TestSecurity
    {

        [Test]
        public void RB()
        {
            DBHelper.InitDBConfig("127.0.0.1", 3306, "db-2-0-w", "root", "123456");
            SecurityFamily sec = BasicTracker.SecurityTracker[1, "rb"];
            Assert.NotNull(sec);

            Calendar calendar = sec.Exchange.GetCalendar();
            foreach (var c in calendar.GetHolidays())
            {
                Console.WriteLine(c);
            }
            int tradingday = 0;
            //正常交易日8：59 未开盘
            DateTime t1 = new DateTime(2018, 12, 14, 8, 59, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //正常交易日9：00 开盘
            t1 = new DateTime(2018, 12, 14, 9, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181214, tradingday);

            //正常交易日14：00 开盘
            t1 = new DateTime(2018, 12, 14, 14, 59, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181214, tradingday);

            //正常交易日15：01 收盘
            t1 = new DateTime(2018, 12, 14, 15, 1, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //正常交易日22：01 夜盘 计入下一个交易日 注意夜盘跨越了凌晨
            t1 = new DateTime(2018, 12, 21, 22, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181224, tradingday);

            //遇到下个交易日放假 则当前夜盘不交易
            t1 = new DateTime(2018, 12, 28, 22, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //放假夜盘也不交易
            t1 = new DateTime(2018, 12, 31, 22, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //放假白天不交易
            t1 = new DateTime(2019, 1, 1, 10, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //当天放假，下个交易日正常交易，当天夜盘 也不交易
            t1 = new DateTime(2019, 1, 1, 22, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //正常交易日 9点正常开盘
            t1 = new DateTime(2019, 1, 2, 9, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190102, tradingday);

        }
        [Test]
        public void HSI()
        {
            DBHelper.InitDBConfig("127.0.0.1", 3306, "db-2-0-w", "root", "123456");
            SecurityFamily sec = BasicTracker.SecurityTracker[1, "HSI"];
            Assert.NotNull(sec);

            Calendar calendar = sec.Exchange.GetCalendar();
            foreach (var c in calendar.GetHolidays())
            {
                Console.WriteLine(c);
            }

            int tradingday = 0;
            //2018 12 25/26放假

            //正常交易日9：14 未开盘
            DateTime t1 = new DateTime(2018, 12,24, 9,14, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //正常交易日9：15 开盘
            t1 = new DateTime(2018, 12, 24, 9,15, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181224, tradingday);

            //正常交易日16：29 开盘
            t1 = new DateTime(2018, 12, 24, 16, 29, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181224, tradingday);

            //正常交易日16：31 收盘
            t1 = new DateTime(2018, 12, 24, 16, 31, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //25 26放假 不影像当前交易日的夜盘 夜盘进入 顺延的下一个交易日
            //正常交易日17：16 开盘
            t1 = new DateTime(2018, 12, 24, 17, 16, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181227, tradingday);

            //正常交易日23：00 开盘
            t1 = new DateTime(2018, 12, 24, 23, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181227, tradingday);

            //24号交易 25号凌晨1：00 开盘
            t1 = new DateTime(2018, 12, 25, 1, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181227, tradingday);

            //周五次日22点和凌晨1：00 开盘
            t1 = new DateTime(2018, 12, 28, 22, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181231, tradingday);

            t1 = new DateTime(2018, 12, 29, 1, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181231, tradingday);
        }
        [Test]
        public void CL()
        {
            DBHelper.InitDBConfig("127.0.0.1", 3306, "db-2-0-w", "root", "123456");
            SecurityFamily sec = BasicTracker.SecurityTracker[1, "CL"];
            Assert.NotNull(sec);

            Calendar calendar = sec.Exchange.GetCalendar();
            foreach (var c in calendar.GetHolidays())
            {
                Console.WriteLine(c);
            }

            //测试时间为当地时间
            int tradingday = 0;
            //原油冬令电子盘 07:00-06:00
            //星期一早上6点50分没有开盘
            DateTime t1 = new DateTime(2019, 1, 7, 6, 50, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //星期一早上7点1分开盘
            t1 = new DateTime(2019, 1, 7, 7, 1, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //星期一15：00开盘
            t1 = new DateTime(2019, 1, 7, 15, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //星期一20：00开盘
            t1 = new DateTime(2019, 1, 7, 20, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //星期二4：00开盘
            t1 = new DateTime(2019, 1, 8, 4, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //星期二6：00开盘
            t1 = new DateTime(2019, 1, 8,6, 0, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //星期二6：01收盘
            t1 = new DateTime(2019, 1, 8, 6, 1, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);


            //节假日
            //20191226
            t1 = new DateTime(2018, 12, 25, 7, 1, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            t1 = new DateTime(2018, 12, 26, 7, 1, 0, DateTimeKind.Local);
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20181226, tradingday);
        }

        [Test]
        public void DAX()
        {
            DBHelper.InitDBConfig("127.0.0.1", 3306, "db-2-0-w", "root", "123456");
            SecurityFamily sec = BasicTracker.SecurityTracker[1, "DAX"];
            Assert.NotNull(sec);

            Exchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            Console.WriteLine("checck trading time");
            int tradingday = 0;
            //早上8点没有开盘
            DateTime t1 = new DateTime(2019, 1, 7, 8, 0, 0, DateTimeKind.Local);//星期一 08:00:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //早上8点14
            t1 = new DateTime(2019, 1, 7, 8, 14, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0,tradingday);
            
            //早上8点15
            t1 = new DateTime(2019, 1, 7, 8, 15, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //晚上8点15
            t1 = new DateTime(2019, 1, 7, 20, 15, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //次日凌晨4点
            t1 = new DateTime(2019, 1, 8, 4, 0, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //次日凌晨5点
            t1 = new DateTime(2019, 1, 8, 5, 0, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190107, tradingday);

            //次日凌晨5点01收盘
            t1 = new DateTime(2019, 1, 8, 5, 1, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);


            //次日早上8点再次开盘
            t1 = new DateTime(2019, 1, 8, 8, 10, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            t1 = new DateTime(2019, 1, 8, 8, 15, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190108, tradingday);

            //周六凌晨4点交易
            t1 = new DateTime(2019, 1, 12, 4, 0, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190111, tradingday);

            //周六凌晨5点1分收盘
            t1 = new DateTime(2019, 1, 12,5,1, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //周六不交易
            t1 = new DateTime(2019, 1,12, 9, 0, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //周日不交易
            t1 = new DateTime(2019, 1, 13, 9, 0, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //周日晚上不交易
            t1 = new DateTime(2019, 1, 13, 20, 0, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //周一晚上不交易
            t1 = new DateTime(2019, 1, 14, 5, 0, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(0, tradingday);

            //周一早上在开盘
            t1 = new DateTime(2019, 1, 14, 8, 15, 0, DateTimeKind.Local);//星期一 08:31:00
            tradingday = MarketTimeCheck(sec, t1);
            Assert.AreEqual(20190114, tradingday);
        }



        int MarketTimeCheck(SecurityFamily sec,DateTime localTime)
        {
            var extime = sec.Exchange.ConvertToExchangeTime(localTime);

            return sec.MarketTimeCheck(extime);
        }
    }
}
