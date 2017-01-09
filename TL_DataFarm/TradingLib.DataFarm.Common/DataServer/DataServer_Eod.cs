using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public partial class DataServer
    {

        EodDataService eodservice = null;
        public void InitEodService()
        {
            string path = this.ConfigFile["TickPath"].AsString();
            logger.Info("[Init EOD Service]");
            eodservice = new EodDataService(GetHistDataSotre(), path,_syncdb);
            eodservice.EodBarResotred += new Action<Symbol, IEnumerable<BarImpl>>(OnEodBarResotred);
            eodservice.EodBarClose += new Action<EodBarEventArgs>(OnEodBarClose);
            eodservice.EodBarUpdate += new Action<EodBarEventArgs>(OnEodBarUpdate);

            eodservice.SecurityEntryMarketDay += new Action<SecurityFamily, MarketDay>(OnSecurityEntryMarketDay);
            eodservice.SymbolExpiredEvent += new Action<Symbol, Symbol>(OnSymbolExpiredEvent);
        }

        void OnSymbolExpiredEvent(Symbol arg1, Symbol arg2)
        {
            logger.Info(string.Format("Symbol:{0} Expied, new symbol created:{1}", arg1.Symbol, arg2.Symbol));
            Exchange exch = MDBasicTracker.ExchagneTracker[arg1.Exchange];
            List<Symbol> list = new List<Symbol>() { arg2};
            if (_defaultFeed != null)
            {
                _defaultFeed.RegisterSymbols(exch, list);
            }
        }


        void OnSecurityEntryMarketDay(SecurityFamily arg1, MarketDay arg2)
        {
            //重置快照数据
            foreach (var snapshot in Global.TickTracker.TickSnapshots)
            {
                Symbol symbol = MDBasicTracker.SymbolTracker[snapshot.Exchange, snapshot.Symbol];
                if (symbol == null) continue;

                //属于对应品种
                if (symbol.SecurityFamily.Code == arg1.Code)
                {
                                                                                       
                    decimal lastclose = snapshot.Trade;//昨日收盘价 为最后一个成交价
                    //根据交易所不同盘中/盘后会给出当前交易日的结算价与持仓统计
                    decimal lastsettle = snapshot.Settlement;
                    int lastoi = snapshot.OpenInterest;
                    DateTime dt = GetDataFeedTime(symbol.SecurityFamily.Exchange.DataFeed).CurrentTime;

                    snapshot.Reset();
                    if (dt != null)
                    {
                        snapshot.Date = dt.ToTLDate();
                        snapshot.Time = dt.ToTLTime();
                    }
                    snapshot.PreClose = lastclose;
                    snapshot.PreSettlement = lastsettle;
                    snapshot.PreOpenInterest = lastoi;

                    NotifyTick2Connections(snapshot);
                }
            }
        }

        /// <summary>
        /// 重置所有快照数据
        /// </summary>
        void ResetAllSnapshot()
        {
            //重置快照数据
            foreach (var snapshot in Global.TickTracker.TickSnapshots)
            {
                Symbol symbol = MDBasicTracker.SymbolTracker[snapshot.Exchange, snapshot.Symbol];
                if (symbol == null) continue;

                decimal lastclose = snapshot.Trade;//昨日收盘价 为最后一个成交价
                //根据交易所不同盘中/盘后会给出当前交易日的结算价与持仓统计
                decimal lastsettle = snapshot.Settlement;
                int lastoi = snapshot.OpenInterest;
                lastsettle = 100;
                DateTime dt = GetDataFeedTime(symbol.SecurityFamily.Exchange.DataFeed).CurrentTime;

                snapshot.Reset();
                if (dt != null)
                {
                    snapshot.Date = dt.ToTLDate();
                    snapshot.Time = dt.ToTLTime();
                }
                snapshot.PreClose = lastclose;
                snapshot.PreSettlement = lastsettle;
                snapshot.PreOpenInterest = lastoi;

                NotifyTick2Connections(snapshot);
            }
        }

        /// <summary>
        /// EOD PartialBar更新
        /// </summary>
        /// <param name="obj"></param>
        void OnEodBarUpdate(EodBarEventArgs obj)
        {
            //此处采用每次更新 日线数据更新时需要强制更新日级别以上数据 比如周线 月线等(EOD PartialBar在一个交易日内不会发生对象变化)
            this.UpdateRealPartialBar(obj.Symbol, obj.EodPartialBar);
        }

        /// <summary>
        /// EODBar关闭 更新数据集
        /// </summary>
        /// <param name="obj"></param>
        void OnEodBarClose(EodBarEventArgs obj)
        {
            this.UpdateBar(obj.Symbol, obj.EodPartialBar);
        }

        /// <summary>
        /// 1分钟Bar数据合并生成EODBar数据 例如Eod没有保存 多个交易日之后 则之间空缺的EOD Bar数据由1分钟Bar数据合并生成
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void OnEodBarResotred(Symbol arg1, IEnumerable<BarImpl> arg2)
        {
            foreach (var bar in arg2)
            {
                this.UpdateBar(arg1, bar);
            }
        }

        void EodServiceProcessTick(Tick k)
        {
            eodservice.OnTick(k);
        }
    }
}
