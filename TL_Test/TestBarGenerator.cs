using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.Common;
using TradingLib.ORM;
namespace TL_Test
{
    [TestFixture]
    public class TestBarGenerator
    {
        

        [Test]
        public void Text_BarFrequency()
        {
            DBHelper.InitDBConfig("127.0.0.1", 3306, "db-market", "root", "123456");
            Symbol symbol = MDBasicTracker.SymbolTracker["NYMEX", "CLX6"];

            BarFrequency bfreq = new BarFrequency(BarInterval.CustomTime,60);

            BarGenerator bg = new BarGenerator(symbol, bfreq, BarConstructionType.Trade);

            Tick timek = new TickImpl(DateTime.Now);
            
            Tick k = new TickImpl();
            k.Date = 20160323;
            k.Time = 143001;

            k.AskPrice = 2001;
            k.AskSize = 1;

            k.BidPrice = 1999;
            k.BidSize = 1;

            k.Trade = 2000;
            k.Size = 10;

            //构造后默认PartialBar不为Null
            Assert.NotNull(bg.PartialBar);
            //初始化后默认的BarStartTime为MinValue
            Assert.True(bg.BarEndTime == DateTime.MinValue);

            Assert.AreEqual(0, bg.PartialBar.Open);
            Assert.False(bg.TickWareSent);
            Assert.False(bg.Updated);

            //处理时间Tick tick与update标记均为False
            bg.ProcessTick(timek);
            Assert.False(bg.TickWareSent);
            Assert.False(bg.Updated);

            bg.ProcessTick(k);
            Assert.AreEqual(2000, bg.PartialBar.Open);
            Assert.AreEqual(2000, bg.PartialBar.High);
            Assert.AreEqual(2000, bg.PartialBar.Close);
            Assert.AreEqual(2000, bg.PartialBar.Low);
            Assert.AreEqual(2001, bg.PartialBar.Ask);
            Assert.AreEqual(1999, bg.PartialBar.Bid);
            Assert.AreEqual(10, bg.PartialBar.Volume);

            Assert.True(bg.TickWareSent);
            Assert.True(bg.Updated);

            //hit high
            k.Trade = 2001;
            k.Size = 2;
            bg.ProcessTick(k);
            Assert.AreEqual(2000, bg.PartialBar.Open);
            Assert.AreEqual(2001, bg.PartialBar.High);
            Assert.AreEqual(2001, bg.PartialBar.Close);
            Assert.AreEqual(2000, bg.PartialBar.Low);
            Assert.AreEqual(2001, bg.PartialBar.Ask);
            Assert.AreEqual(1999, bg.PartialBar.Bid);
            Assert.AreEqual(12, bg.PartialBar.Volume);

            //hit low
            k.Trade = 1990;
            k.Size = 10;
            bg.ProcessTick(k);
            Assert.AreEqual(2000, bg.PartialBar.Open);
            Assert.AreEqual(2001, bg.PartialBar.High);
            Assert.AreEqual(1990, bg.PartialBar.Close);
            Assert.AreEqual(1990, bg.PartialBar.Low);
            Assert.AreEqual(2001, bg.PartialBar.Ask);
            Assert.AreEqual(1999, bg.PartialBar.Bid);
            Assert.AreEqual(22, bg.PartialBar.Volume);


            //hit close
            k.Trade = 1000;
            k.Size = 10;
            bg.ProcessTick(k);


            //触发一个新Bar后 BarGenerator生成一个新的PartialBar
            DateTime nextround = TimeFrequency.BarEndTime(DateTime.Now, TimeSpan.FromSeconds(60));
            bg.SendNewBar(nextround);

            Assert.False(bg.TickWareSent);
            Assert.False(bg.Updated);

            //下一个Bar的起始时间为上一个Bar的开始时间
            Assert.AreEqual(nextround, bg.PartialBar.EndTime);

            //同时将上一个Bar的收盘价更新到当前Bar
            Assert.AreEqual(1000, bg.PartialBar.Open);
            Assert.AreEqual(1000, bg.PartialBar.High);
            Assert.AreEqual(1000, bg.PartialBar.Close);
            Assert.AreEqual(1000, bg.PartialBar.Low);
            Assert.AreEqual(2001, bg.PartialBar.Ask);
            Assert.AreEqual(1999, bg.PartialBar.Bid);
            Assert.AreEqual(0, bg.PartialBar.Volume);

            k.AskPrice = 2000;
            k.AskSize = 1;

            k.BidPrice = 1990;
            k.BidSize = 1;

            k.Trade = 1995;
            k.Size = 10;
            bg.ProcessTick(k);
            Assert.AreEqual(1995, bg.PartialBar.Open);
            Assert.AreEqual(1995, bg.PartialBar.High);
            Assert.AreEqual(1995, bg.PartialBar.Close);
            Assert.AreEqual(1995, bg.PartialBar.Low);
            Assert.AreEqual(2000, bg.PartialBar.Ask);
            Assert.AreEqual(1990, bg.PartialBar.Bid);
            Assert.AreEqual(10, bg.PartialBar.Volume);

            //hit high
            k.Trade = 2001;
            k.Size = 2;
            bg.ProcessTick(k);
            Assert.AreEqual(1995, bg.PartialBar.Open);
            Assert.AreEqual(2001, bg.PartialBar.High);
            Assert.AreEqual(2001, bg.PartialBar.Close);
            Assert.AreEqual(1995, bg.PartialBar.Low);
            Assert.AreEqual(2000, bg.PartialBar.Ask);
            Assert.AreEqual(1990, bg.PartialBar.Bid);
            Assert.AreEqual(12, bg.PartialBar.Volume);

            //hit low
            k.Trade = 1980;
            k.Size = 10;
            bg.ProcessTick(k);
            Assert.AreEqual(1995, bg.PartialBar.Open);
            Assert.AreEqual(2001, bg.PartialBar.High);
            Assert.AreEqual(1980, bg.PartialBar.Close);
            Assert.AreEqual(1980, bg.PartialBar.Low);
            Assert.AreEqual(2000, bg.PartialBar.Ask);
            Assert.AreEqual(1990, bg.PartialBar.Bid);
            Assert.AreEqual(22, bg.PartialBar.Volume);
        }


        [Test]
        public void Text_FrequencyManager()
        {
            DBHelper.InitDBConfig("127.0.0.1",3306,"db-market","root","123456");
            Symbol symbol = MDBasicTracker.SymbolTracker["NYMEX", "CLX6"];
            //初始化FrequencyPlugin
            TimeFrequency tm = new TimeFrequency(new BarFrequency(BarInterval.CustomTime, 60));
            Dictionary<Symbol, BarConstructionType> map = new Dictionary<Symbol, BarConstructionType>();
            //Symbol symbol = new SymbolImpl();
            //symbol.Symbol = "CNH6";

            map.Add(symbol, BarConstructionType.Trade);
            //map.Add(symbol2, BarConstructionType.Trade);
            FrequencyManager fm = new FrequencyManager("debug",QSEnumDataFeedTypes.DEFAULT);
            fm.RegisterAllBasicFrequency();
            fm.RegisterSymbol(symbol);


            FrequencyManager.FreqKey key = new FrequencyManager.FreqKey(tm, symbol);

            Frequency frequency = fm.GetFrequency(symbol, tm.BarFrequency);

            Assert.AreEqual(0, frequency.WriteableBars.Count);


            Tick k = new TickImpl();
            k.Exchange = symbol.Exchange;
            k.Symbol = symbol.Symbol;
            k.UpdateType = "X";

            k.Date = 20160323;
            k.Time = 143000;

            k.AskPrice = 2001;
            k.AskSize = 1;

            k.BidPrice = 1999;
            k.BidSize = 1;

            k.Trade = 2001;
            k.Size = 10;

            fm.ProcessTick(k);
            //处理tick后 当前时间更新为该合约时间
            //Assert.AreEqual(k.DateTime(), fm.CurrentTime);
            fm.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(fm_NewFreqKeyBarEvent);

            //143000开始 143059结束,143100代表一个新的Bar开始 00-59为一个周期
            k.Time = 143059;
            fm.ProcessTick(k);
            Assert.AreEqual(0, frequency.WriteableBars.Count);
            //Console.WriteLine("Bar:" + frequency.Bars.Current);


            //新开始一个Bar Hit Open
            k.Time = 143100;
            k.Trade = 2000;
            fm.ProcessTick(k);
            //有对应的Bar生成 frequency.WriteableBars增加1
            Assert.AreEqual(1, frequency.WriteableBars.Count);
            //Console.WriteLine("Bar:" + frequency.Bars.Current);
            Assert.AreEqual(2000, frequency.WriteableBars.PartialItem.Open);
            Assert.AreEqual(2000, frequency.WriteableBars.PartialItem.High);
            Assert.AreEqual(2000, frequency.WriteableBars.PartialItem.Low);
            Assert.AreEqual(2000, frequency.WriteableBars.PartialItem.Close);

            k.Time = 143159;
            fm.ProcessTick(k);
            Assert.AreEqual(1, frequency.WriteableBars.Count);

            //Hit High
            k.Trade = 2002;
            fm.ProcessTick(k);
            Assert.AreEqual(1, frequency.WriteableBars.Count);
            Assert.AreEqual(2000, frequency.WriteableBars.PartialItem.Open);
            Assert.AreEqual(2002, frequency.WriteableBars.PartialItem.High);
            Assert.AreEqual(2000, frequency.WriteableBars.PartialItem.Low);
            Assert.AreEqual(2002, frequency.WriteableBars.PartialItem.Close);

            //Hit Low
            k.Trade = 1990;
            fm.ProcessTick(k);
            Assert.AreEqual(1, frequency.WriteableBars.Count);
            Assert.AreEqual(2000, frequency.WriteableBars.PartialItem.Open);
            Assert.AreEqual(2002, frequency.WriteableBars.PartialItem.High);
            Assert.AreEqual(1990, frequency.WriteableBars.PartialItem.Low);
            Assert.AreEqual(1990, frequency.WriteableBars.PartialItem.Close);



            k.Time = 143200;
            fm.ProcessTick(k);
            //有对应的Bar生成 frequency.WriteableBars增加1
            Assert.AreEqual(2, frequency.WriteableBars.Count);
            Console.WriteLine("Bar:"+frequency.Bars.Current);

            Assert.AreEqual(2000, frequency.WriteableBars.Current.Open);
            Assert.AreEqual(2002, frequency.WriteableBars.Current.High);
            Assert.AreEqual(1990, frequency.WriteableBars.Current.Low);
            Assert.AreEqual(1990, frequency.WriteableBars.Current.Close);

            //Assert.AreEqual(false, frequency.WriteableBars.HasPartialItem);
            Assert.AreEqual(1990, frequency.WriteableBars.PartialItem.Open);
            Assert.AreEqual(1990, frequency.WriteableBars.PartialItem.High);
            Assert.AreEqual(1990, frequency.WriteableBars.PartialItem.Low);
            Assert.AreEqual(1990, frequency.WriteableBars.PartialItem.Close);
            //frequency.Bars.PartialItem
            

        }

        void fm_NewFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
            
        }
    }
}
