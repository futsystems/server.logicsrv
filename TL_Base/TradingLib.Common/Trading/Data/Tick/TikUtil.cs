using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class TikUtil
    {
        /// <summary>
        /// converts EPF files to tradelink tick files in current directory
        /// </summary>
        /// <param name="args"></param>
        public static void EPF2TIK(string[] args)
        {
            // get a list of epf files
            foreach (string file in args)
            {
                //SecurityImpl sec = SecurityImpl.FromTIK(file);
                //sec.HistSource.gotTick += new TickDelegate(HistSource_gotTick);
                //_tw = new TikWriter(sec.Symbol);
                //while (sec.NextTick())
                //_tw.Close();
            }
        }

        public static bool IsFileWritetable(string path)
        {
            FileStream stream = null;

            try
            {
                if (!System.IO.File.Exists(path))
                    return true;
                System.IO.FileInfo file = new FileInfo(path);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return true;
        }

        static void HistSource_gotTick(TradingLib.API.Tick t)
        {
            _tw.newTick((TickImpl)t);
        }

        private static TikWriter _tw;

        /// <summary>
        /// finds a group of files with a given extension
        /// </summary>
        /// <param name="EXT"></param>
        /// <returns></returns>
        public static string[] GetFiles() { return GetFiles(Util.TLTickDir, TikConst.WILDCARD_EXT); }
        public static string[] GetFiles(string EXT) { return GetFiles(Environment.CurrentDirectory, EXT); }
        public static string[] GetFiles(string path, string EXT)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fis = di.GetFiles(EXT);
            List<string> names = new List<string>();
            foreach (FileInfo fi in fis)
                names.Add(fi.FullName);
            return names.ToArray();


        }

        /// <summary>
        /// get tick files created today from default folder
        /// </summary>
        /// <returns></returns>
        //public static List<string> GetFilesFromDate() { return GetFilesFromDate(Util.TLTickDir, Util.ToTLDate()); }
        /// <summary>
        /// get tick files created today
        /// </summary>
        /// <param name="tickfolder"></param>
        /// <returns></returns>
        //public static List<string> GetFilesFromDate(string tickfolder) { return GetFilesFromDate(tickfolder, Util.ToTLDate()); }
        /// <summary>
        /// get tick files created on a certain date
        /// </summary>
        /// <param name="tickfolder"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        //public static List<string> GetFilesFromDate(string tickfolder, int date)
        //{
        //    string[] files = TikUtil.GetFiles(tickfolder, TikConst.WILDCARD_EXT);
        //    List<string> matching = new List<string>();
        //    foreach (string file in files)
        //    {
        //        SecurityImpl sec = SecurityImpl.SecurityFromFileName(file);
        //        string symfix = System.IO.Path.GetFileNameWithoutExtension(sec.Name);
        //        if (sec.Date == date)
        //            matching.Add(file);
        //    }
        //    return matching;
        //}


        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="ticks"></param>
        public static bool TicksToFile(Tick[] ticks) { return TicksToFile(ticks, null); }
        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="debs"></param>
        /// <returns></returns>
        public static bool TicksToFile(Tick[] ticks, DebugDelegate debs)
        {
            try
            {
                TikWriter tw = new TikWriter();
                foreach (Tick k in ticks)
                    tw.newTick(k);
                tw.Close();
                if (debs != null)
                    debs(tw.RealSymbol + " saved " + tw.Count + " ticks to: " + tw.Filepath);

            }
            catch (Exception ex)
            {
                if (debs != null)
                    debs(ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }
        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="debs"></param>
        /// <param name="tw"></param>
        /// <returns></returns>
        public static bool TicksToFile(Tick[] ticks, DebugDelegate debs, TikWriter tw)
        {
            try
            {
                foreach (Tick k in ticks)
                    tw.newTick(k);
                tw.Close();
                if (debs != null)
                    debs(tw.RealSymbol + " saved " + tw.Count + " ticks to: " + tw.Filepath);

            }
            catch (Exception ex)
            {
                if (debs != null)
                    debs(ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }
        /// <summary>
        /// create ticks from bars on default interval
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        //public static Tick[] Barlist2Tick(BarList bl)
        //{
        //    List<Tick> k = new List<Tick>(bl.Count * 4);
        //    foreach (Bar b in bl)
        //        k.AddRange(BarImpl.ToTick(b));
        //    return k.ToArray();
        //}

        public static DateTime DateTime(this Tick k)
        {
            return Util.ToDateTime(k.Date, k.Time);
        }

        /// <summary>
        /// 是否
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsTrade(this Tick k)
        { 
            return k.Trade!=0 && k.Size>0;
        }

        /// <summary>
        /// 是否有Bid数据
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool HasBid(this Tick k)
        {
            return k.BidPrice != 0 && k.BidSize != 0;
        }

        /// <summary>
        /// 是否有Ask数据
        /// </summary>
        public static bool HasAsk(this Tick k)
        {
            return k.AskPrice != 0 && k.AskSize != 0;
        }

        public static bool IsFullQuote(this Tick k)
        {
            return k.HasAsk() && k.HasBid();
        }


        /// <summary>
        /// 是否是指数
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsIndex(this Tick k)
        {
            return k.Size < 0;
        }

        /// <summary>
        /// 是否有Tick数据 Trade/Quote
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool HasTick(this Tick k)
        {
            return k.IsTrade() || k.HasAsk() || k.HasBid();
        }

        /// <summary>
        /// 是否是统计信息
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsSummary(this Tick k)
        {
            return k.Type == EnumTickType.SUMMARY;
        }

        /// <summary>
        /// 是否是快照数据
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsSnapshot(this Tick k)
        {
            return k.Type == EnumTickType.SNAPSHOT;
        }

        /// <summary>
        /// Tick数据是否有效
        /// 1.Snapshot 快照数据
        /// 2.Trade Quote 数据 HasTick()
        /// 3.Summary 统计数据
        /// 4.Index 指数数据 没有Trade 没有盘口 只有Trade数据
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsValid(this Tick k)
        {
            //合约不为空 有Trade或Quote数据
            return (!string.IsNullOrEmpty(k.Symbol)) && (k.IsSnapshot() || k.HasTick() || k.IsSummary() || k.IsIndex());
        }

        /// <summary>
        /// 用Tick数据k 更新快照snapshot
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="k"></param>
        public static void UpdateSnapshot(this Tick snapshot, Tick k)
        {
            if (snapshot.Symbol != k.Symbol) return;

            snapshot.Date = k.Date;
            snapshot.Time = k.Time;

            switch (k.Type)
            {
                case EnumTickType.TRADE:
                    {
                        if (k.IsTrade())
                        {
                            snapshot.Trade = k.Trade;
                            snapshot.Size = k.Size;
                            snapshot.Exchange = k.Exchange;
                        }
                        break;
                    }
                case EnumTickType.QUOTE:
                    {
                        if (k.HasAsk())
                        {
                            snapshot.AskPrice = k.AskPrice;
                            snapshot.AskSize = k.AskSize;
                            snapshot.AskExchange = k.AskExchange;
                        }
                        if (k.HasBid())
                        {
                            snapshot.BidPrice = k.BidPrice;
                            snapshot.BidSize = k.BidSize;
                            snapshot.BidExchange = k.BidExchange;
                            snapshot.BidPrice = k.BidPrice;
                        }
                        break;
                    }
                case EnumTickType.LEVEL2:
                    {
                        if (k.HasAsk())
                        {
                            snapshot.AskPrice = k.AskPrice;
                            snapshot.AskSize = k.AskSize;
                            snapshot.AskExchange = k.AskExchange;
                        }
                        if (k.HasBid())
                        {
                            snapshot.BidPrice = k.BidPrice;
                            snapshot.BidSize = k.BidSize;
                            snapshot.BidExchange = k.BidExchange;
                            snapshot.BidPrice = k.BidPrice;
                        }
                        snapshot.Depth = k.Depth;
                        break;
                    }
                case EnumTickType.SUMMARY:
                    {
                        snapshot.Vol = k.Vol;
                        snapshot.Open = k.Open;
                        snapshot.High = k.High;
                        snapshot.Low = k.Low;
                        snapshot.PreOpenInterest = k.PreOpenInterest;
                        snapshot.OpenInterest = k.OpenInterest;
                        snapshot.PreSettlement = k.PreSettlement;
                        snapshot.Settlement = k.Settlement;
                        snapshot.UpperLimit = k.UpperLimit;
                        snapshot.LowerLimit = k.LowerLimit;
                        snapshot.PreClose = k.PreClose;
                        break;
                    }
                case EnumTickType.SNAPSHOT:
                    {
                        if (k.IsTrade())
                        {
                            snapshot.Trade = k.Trade;
                            snapshot.Size = k.Size;
                            snapshot.Exchange = k.Exchange;
                        }

                        if (k.HasAsk())
                        {
                            snapshot.AskPrice = k.AskPrice;
                            snapshot.AskSize = k.AskSize;
                            snapshot.AskExchange = k.AskExchange;
                        }
                        if (k.HasBid())
                        {
                            snapshot.BidPrice = k.BidPrice;
                            snapshot.BidSize = k.BidSize;
                            snapshot.BidExchange = k.BidExchange;
                            snapshot.BidPrice = k.BidPrice;
                        }


                        snapshot.Vol = k.Vol;
                        snapshot.Open = k.Open;
                        snapshot.High = k.High;
                        snapshot.Low = k.Low;
                        snapshot.PreOpenInterest = k.PreOpenInterest;
                        snapshot.OpenInterest = k.OpenInterest;
                        snapshot.PreSettlement = k.PreSettlement;
                        snapshot.Settlement = k.Settlement;
                        snapshot.UpperLimit = k.UpperLimit;
                        snapshot.LowerLimit = k.LowerLimit;
                        snapshot.PreClose = k.PreClose;

                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 获得Tick对应合约的唯一键值
        /// 比如在维护合约快照时 如果深圳和上海2个相同代码的合约行情到达，就需要区分交易所
        /// 因此需要加入交易所来形成唯一键
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string GetSymbolUniqueKey(this Tick k)
        {
            return string.Format("{0}-{1}", k.Exchange, k.Symbol);
        }
    }
}