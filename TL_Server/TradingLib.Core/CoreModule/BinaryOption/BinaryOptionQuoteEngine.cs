using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using Common.Logging;

namespace TradingLib.Common
{
    /// <summary>
    /// 二元期权合约与报价引擎
    /// 用于定时生成二元期权同时按一定的规则生成对应的报价和参数
    /// </summary>
    public class BinaryOptionQuoteEngine:BaseSrvObject
    {
        const string CoreName = "BinaryOptionQuoteEngine";
        public string CoreId { get { return this.PROGRAME; } }

        //底层合约map 用于记录可交易的资产
        ConcurrentDictionary<string, Symbol> symbolmap = new ConcurrentDictionary<string, Symbol>();
        //二元期权合约map 用于维护当前可交易资产
        ConcurrentDictionary<string, BinaryOption> optionmap = new ConcurrentDictionary<string, BinaryOption>();

        //已经生成二元期权的 时间类别-到期时间 列表 MIN4-BornTime-ExpireTime
        List<string> _generatedtime = new List<string>();

        bool IsGenerated(EnumBinaryOptionTimeSpan type, long borntime, long expiretime)
        {
            string key = "{0}-{1}-{2}".Put(type, borntime, expiretime);
            if (_generatedtime.Contains(key)) return true;
            return false;
        }

        public BinaryOptionQuoteEngine()
        { 
            
            ITask task = new TaskProc("","定时生成二元期权合约","0 * * * * ?",OnSchedule);
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);

            foreach (var sym in BasicTracker.DomainTracker[1].GetSymbols())
            {
                symbolmap.TryAdd(sym.Symbol, sym);
            }
        }

        void OnSchedule()
        {
            DateTime now = DateTime.Now;
            long dt = now.ToTLDateTime();
            logger.Info("Schedule:" + dt);
            //遍历所有交易合约 生成合约
            Dictionary<EnumBinaryOptionTimeSpan, bool> flag = GetTimeSpanGenFlag(now);
            foreach (var sym in this.Symbols)
            {
                GenerateBinaryOptionCallPut(sym,flag);
                //如果可以交易其他类型的二元期权 在此处加入生成函数
            }
        }

        #region 生成二元期权合约
        /// <summary>
        /// 获得某个时间点 是否需要生成二元期权的Map
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        Dictionary<EnumBinaryOptionTimeSpan, bool> GetTimeSpanGenFlag(DateTime now)
        {
            Dictionary<EnumBinaryOptionTimeSpan, bool> tmp = new Dictionary<EnumBinaryOptionTimeSpan, bool>();
            foreach(var type in Enum.GetValues(typeof(EnumBinaryOptionTimeSpan)))
            {
                EnumBinaryOptionTimeSpan tstype = (EnumBinaryOptionTimeSpan)type;
                int val = (int)tstype;
                tmp[tstype] = now.Minute % val == 0;
            }
            return tmp;
        }

        /// <summary>
        /// 生成CallPut合约
        /// </summary>
        /// <param name="sym"></param>
        void GenerateBinaryOptionCallPut(Symbol sym,Dictionary<EnumBinaryOptionTimeSpan, bool> flag)
        {
            foreach(var key in flag.Keys)
            {
                if(flag[key])
                {
                    //检查该品种 在对应时间间隔之是否处于交易状态,如果底层品种不交易则不生成对应二元期权
                    int minute = (int)key;
                    bool open = sym.SecurityFamily.IsMarketOpenAfterTime(TimeSpan.FromMinutes(minute));
                    if (open)
                    {
                        BinaryOption option = new BinaryOptionCallPut(sym.Symbol, key, 0.7M);
                        logger.Info("Option Generated:" + option.ToString());
                        optionmap.TryAdd(option.ContractID, option);
                    }
                }
            }
        }

        #endregion


        #region 数据集访问
        /// <summary>
        /// 返回所有底层可交易资产合约
        /// </summary>
        public IEnumerable<Symbol> Symbols
        {
            get { return symbolmap.Values; }
        }


        /// <summary>
        /// 返回所有未过期二元期权
        /// </summary>
        IEnumerable<BinaryOption> BinaryOptions
        {
            get
            {
                long now = Util.ToTLDateTime();
                return optionmap.Values.Where(o => o.IsExpired(now));
            }
        }

        /// <summary>
        /// 返回所有二元期权
        /// </summary>
        public IEnumerable<BinaryOption> GetBinaryOptions()
        {
            return this.BinaryOptions;
        }

        /// <summary>
        /// 获得某种类别的二元期权
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<BinaryOption> GetBinaryOptions(EnumBinaryOptionType type)
        {
            return BinaryOptions.Where(o => o.OptionType == type);
        }


        /// <summary>
        /// 返回某个合约某种类别所有二元期权
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<BinaryOption> GetBinaryOptions(string symbol, EnumBinaryOptionType type)
        {
            return BinaryOptions.Where(o => o.OptionType == type && o.Symbol == symbol);
        }


        /// <summary>
        /// 查找某个合约 某种类别 某个时间间隔 某个到期时间 的二元期权
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="tstype"></param>
        /// <param name="expiretime"></param>
        /// <returns></returns>
        public BinaryOption GetBinaryOption(string symbol,EnumBinaryOptionType type,EnumBinaryOptionTimeSpan tstype, long expiretime)
        {
            string contractid = "{0}-{1}-{2}-{3}".Put(symbol, type, tstype, expiretime);
            BinaryOption option = null;
            if (optionmap.TryGetValue(contractid, out option))
            {
                return option;
            }
            return null;
        }

        #endregion

    }
}
