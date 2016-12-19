using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public partial class DataServerBase
    {

        EodDataService eodservice = null;
        public void InitEodService()
        {
            string path = this.ConfigFile["TickPath"].AsString();
            logger.Info("[Init EOD Service]");
            eodservice = new EodDataService(GetHistDataSotre(), path,_syncdb);
            eodservice.EodBarResotred += new Action<Symbol, IEnumerable<BarImpl>>(eodservice_EodBarResotred);
            eodservice.EodBarClose += new Action<EodBarEventArgs>(eodservice_EodBarClose);
            eodservice.EodBarUpdate += new Action<EodBarEventArgs>(eodservice_EodBarUpdate);

            eodservice.SecurityEntryMarketDay += new Action<SecurityFamily, MarketDay>(eodservice_SecurityEntryMarketDay);
            eodservice.SymbolExpiredEvent += new Action<Symbol, Symbol>(eodservice_SymbolExpiredEvent);
        }

        void eodservice_SymbolExpiredEvent(Symbol arg1, Symbol arg2)
        {
            logger.Info(string.Format("Symbol:{0} Expied, new symbol created:{1}", arg1.Symbol, arg2.Symbol));
            IExchange exch = MDBasicTracker.ExchagneTracker[arg1.Exchange];
            List<Symbol> list = new List<Symbol>() { arg2};
            if (_defaultFeed != null)
            {
                _defaultFeed.RegisterSymbols(exch, list);
            }
        }


        void eodservice_SecurityEntryMarketDay(SecurityFamily arg1, MarketDay arg2)
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

        //public void StartEodService()
        //{
        //    eodservice.RestoreTickBakcground();
        //}
        void eodservice_EodBarUpdate(EodBarEventArgs obj)
        {
            //throw new NotImplementedException(); ?日线数据是否需要每次都去更新下该值，日线只要当天绑定一个PartialBar即可
            this.UpdateRealPartialBar(obj.Symbol, obj.EodPartialBar);
        }

        void eodservice_EodBarClose(EodBarEventArgs obj)
        {
            //throw new NotImplementedException();
            this.UpdateBar(obj.Symbol, obj.EodPartialBar);
        }

        void eodservice_EodBarResotred(Symbol arg1, IEnumerable<BarImpl> arg2)
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
