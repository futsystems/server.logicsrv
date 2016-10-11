using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Common.DataFarm;
using TradingLib.ORM;
namespace TL_Test
{
    [TestFixture]
    public class TextMarketDay
    {


        /// <summary>
        /// 获得当前MarketDay
        /// 逻辑
        /// 1.如果当天是交易日 且在开盘与收盘之间则返回 当前交易日对应的MarketDay
        /// 2.如果不是交易日 如果离开下个交易开盘5分钟之内 则为下个交易日，否则为上个交易日
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="exTime"></param>
        /// <returns></returns>
        MarketDay GetCurrentMarketDay(Symbol symbol, DateTime exTime)
        {
            Dictionary<int, MarketDay> mdmap = symbol.SecurityFamily.GetMarketDays(exTime,10);

            MarketDay current = null;
            MarketDay nextMarketDay = symbol.SecurityFamily.GetNextMarketDay(exTime);
            MarketDay lastMarketDay = symbol.SecurityFamily.GetLastMarketDay(exTime);
            //当天不是交易日
            if (!mdmap.TryGetValue(exTime.ToTLDate(), out current))
            {
                //离下一个开盘时间小于5分钟 则current设定为nextMarketDay 否则就为上一个MarketDay
                if (nextMarketDay.MarketOpen.Subtract(exTime).TotalMinutes < 5)
                {
                    current = nextMarketDay;
                }
                else
                {
                    current = lastMarketDay;
                }
            }
            else
            {   //当前是交易日 且离开盘大于5分钟 则取上一个交易日
                if (current.MarketOpen.Subtract(exTime).TotalMinutes >= 5)
                {
                    current = lastMarketDay;
                }
                //离下一个开盘小于5分钟 则为下一个交易日 
                if (nextMarketDay.MarketOpen.Subtract(exTime).TotalMinutes < 5)
                {
                    current = nextMarketDay;
                }
            }

            //离开盘时间大于5分钟 则current设定为LastMarketDay
            if (current.MarketOpen.Subtract(exTime).TotalMinutes >= 5)
            {
                current = symbol.SecurityFamily.GetLastMarketDay(exTime);
            }

            return current;
        }
        [Test]
        public void Text_MarketDay_CL()
        {
            DBHelper.InitDBConfig("127.0.0.1", 3306, "db-market", "root", "123456");
            Symbol symbol = MDBasicTracker.SymbolTracker["NYMEX", "CLX6"];
            Assert.NotNull(symbol);

            //正常交易日
            DateTime exTime = new DateTime(2016, 10, 13, 10, 01, 01);
            MarketDay nextMarketDay = symbol.SecurityFamily.GetNextMarketDay(exTime);
            Console.WriteLine(string.Format("{0} Next -> {1}", exTime, nextMarketDay));
            Assert.AreEqual(nextMarketDay.TradingDay, 20161014);

            MarketDay lastMarketDay = symbol.SecurityFamily.GetLastMarketDay(exTime);
            Console.WriteLine(string.Format("{0} Last -> {1}", exTime, lastMarketDay));
            Assert.AreEqual(lastMarketDay.TradingDay, 20161012);

            //星期五
            exTime = new DateTime(2016, 10, 14, 10, 01, 01);
            nextMarketDay = symbol.SecurityFamily.GetNextMarketDay(exTime);
            Console.WriteLine(string.Format("{0} Next -> {1}", exTime, nextMarketDay));
            Assert.AreEqual(nextMarketDay.TradingDay, 20161017);//星期一

            lastMarketDay = symbol.SecurityFamily.GetLastMarketDay(exTime);
            Console.WriteLine(string.Format("{0} Last -> {1}", exTime, lastMarketDay));
            Assert.AreEqual(lastMarketDay.TradingDay, 20161013);//星期四
        }

        [Test]
        public void Text_CurrentMarketDay_CL()
        {
            DBHelper.InitDBConfig("127.0.0.1", 3306, "db-market", "root", "123456");
            Symbol symbol = MDBasicTracker.SymbolTracker["NYMEX", "CLX6"];
            
            Assert.NotNull(symbol);


            //当前时间为交易日 且在盘中
            DateTime exTime = new DateTime(2016, 10, 13, 10, 01, 01);

            MarketDay md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 交易日盘中",exTime,md));
            Assert.AreEqual(md.TradingDay, 20161013);

            //当前时间为交易日 且在收盘后
            exTime = new DateTime(2016, 10, 13, 17, 01, 01);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 交易日收盘后1分钟", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161013);

            //当前时间为交易日 且在开盘前5分钟
            exTime = new DateTime(2016, 10, 13, 17, 54, 59);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 下个交易日开盘前", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161013);

            //当前时间为交易日 且在开盘前5分钟 进入预备时间
            exTime = new DateTime(2016, 10, 13, 17, 55, 01);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 下个交易日预备时间", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161014);

            //星期五盘中
            exTime = new DateTime(2016, 10, 14, 16, 30, 01);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 星期五盘中", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161014);

            //星期五收盘后
            exTime = new DateTime(2016, 10, 14, 17, 30, 01);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 星期五收盘后", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161014);

            //星期五午夜
            exTime = new DateTime(2016, 10, 14, 23,59, 59);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 星期五午夜", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161014);

            //星期六午夜
            exTime = new DateTime(2016, 10, 15, 23, 59, 59);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 星期六午夜", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161014);


            //星期天下午开盘前
            exTime = new DateTime(2016, 10, 16, 17, 45, 00);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 星期天下午开盘前", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161014);

            //星期天下午开盘前 预备时间
            exTime = new DateTime(2016, 10, 16, 17, 55, 01);
            md = GetCurrentMarketDay(symbol, exTime);
            Console.WriteLine(string.Format("{0} -> {1} 星期天预备时间进入星期一", exTime, md));
            Assert.AreEqual(md.TradingDay, 20161017);

            ////星期天下午开盘前 预备时间
            //exTime = new DateTime(2016, 10, 10, 17, 58, 01);
            //md = GetCurrentMarketDay(symbol, exTime);
            //Console.WriteLine(string.Format("{0} -> {1}", exTime, md));
            //Assert.AreEqual(md.TradingDay, 20161017);
        }
    }
}
