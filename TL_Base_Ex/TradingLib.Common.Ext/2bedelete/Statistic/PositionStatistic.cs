using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Collections.Concurrent;

namespace TradingLib.Common
{
    /*
    /// <summary>
    /// 仓位市场快照,用于获得某个时间点的仓位快照
    /// </summary>
    public class PositionStatistic : AccountsSet
    {
        //protected ClearCentreBase _clearcentre;
        public PositionStatistic(ClearCentreBase cc)
        {
            //_clearcentre = cc;

        }
        //仓位我们需要按照合约来进行统计 某个合约 不同的账户有多仓 或者 空仓。分别记录某个合约当前多头与空头

        public event SymPosStatisticAddDel SendSymPosStaAddEvent;
        ConcurrentDictionary<string, ISymbolPositionStatistic> _statisticMap = new ConcurrentDictionary<string, ISymbolPositionStatistic>();


        /// <summary>
        /// 返回统计的所有symbol
        /// </summary>
        public string[] Symbols
        {
            get
            {
                return _statisticMap.Keys.ToArray();
            }

        }
        public ISymbolPositionStatistic this[string symbol]
        {
            get
            {
                ISymbolPositionStatistic re = null;
                if (_statisticMap.TryGetValue(symbol, out re))
                    return re;
                else
                    return null;
            }
        }
        public override void SetAccounts(IAccount[] list)
        {
            base.SetAccounts(list);
            _statisticMap.Clear();//清空缓存数据 下次刷新数据的时候就会重新建立对应的 ISymbolPositionStatistic (新增ISymbolPositionStatistic的时候会触发控件添加对应的view事件)

        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        public void Refresh()
        {
            //将缓存中的数据重置
            foreach (string key in _statisticMap.Keys)
            {
                _statisticMap[key].Reset();
            }

            //遍历所有账户下面的所有持仓,进行统计分析
            for (int i = 0; i < this.Count; i++)
            {
                IAccount a = this[i];
                //遍历账户下所有的持仓
                foreach (Position p in a.Positions)
                {
                    //将持仓归集到对应的仓位统计器中
					getSymPosStatistic(p.Symbol).GotPosition(p);
                }
            }
        }

        ISymbolPositionStatistic getSymPosStatistic(string symbol)
        {
            ISymbolPositionStatistic re = null;
            //若系统没有记录该symbol则新增该symbol
            if (!_statisticMap.TryGetValue(symbol, out re))
            {
                ISymbolPositionStatistic sps = new SymbolPositionStatistic(symbol);
                _statisticMap.TryAdd(symbol, sps);
                if (SendSymPosStaAddEvent != null)
                    SendSymPosStaAddEvent(sps);
            }
            return _statisticMap[symbol];
        }

        public void Display()
        {
            debug("-------------账户集持仓分析-----------------------------");
            foreach (string key in _statisticMap.Keys)
            {
                debug(_statisticMap[key].ToString());
            }
            debug("-------------------------------------------------------");
        }


    }

    /// <summary>
    /// 某个symbol的仓位快照,用于统计某个时间点的仓位情况 多空 人数 手术 成本 注:这个统计是市场的一个时间点的切片数据
    /// 将一个账户集中 某个合约的持有仓位进行分类,多方 空方 多方人数 空方人数 多方成本 空方成本
    /// </summary>
    public class SymbolPositionStatistic : ISymbolPositionStatistic
    {
        public SymbolPositionStatistic(string symbol)
        {
            _symbol = symbol;
            _longaccnum = 0;
            _shortaccnum = 0;
            _lpt = new PositionImpl();
            _spt = new PositionImpl();
        }
        string _symbol;//该统计的合约
        public string Symbol { get { return _symbol; } set { _symbol = value; } }
        int _longaccnum;
        public int LongAccountNum { get { return _longaccnum; } set { _longaccnum = value; } }//持有头单账户数量
        int _shortaccnum;
        public int ShortAccountNum { get { return _shortaccnum; } set { _shortaccnum = value; } }//持有空单账户数量

        public int LongPositionSize { get { return _lpt.Size; } }//多头仓位
        public decimal LongPositionPrice { get { return _lpt.AvgPrice; } }


        public int ShortPositionSize { get { return _spt.Size; } }//空头仓位
        public decimal ShortPositionPrice { get { return _spt.AvgPrice; } }

        Position _lpt;
        Position _spt;

        public void Reset()
        {
            _longaccnum = 0;
            _shortaccnum = 0;
            _lpt = new PositionImpl();
            _spt = new PositionImpl();
        }
        public void GotPosition(Position p)
        {
            if (p.isFlat) return;
            if (p.isLong)
                GotLongPosition(p);
            if (p.isShort)
                GotShortPosition(p);


        }
        void GotLongPosition(Position p)
        {
            //多头仓位 账户+1
            LongAccountNum += 1;
            Trade fill = p.ToTrade();
            fill.Account = _symbol;
            _lpt.Adjust(fill);

        }

        void GotShortPosition(Position p)
        {
            //空头仓位 账户+1
            ShortAccountNum += 1;
            Trade fill = p.ToTrade();
            fill.Account = _symbol;
            _spt.Adjust(fill);
        }

        public override string ToString()
        {
            return "[" + Symbol + "]" + " 多/空:" + LongAccountNum.ToString() + "/" + ShortAccountNum.ToString() + " 手数:" + LongPositionSize.ToString() + "/" + ShortPositionSize.ToString() + " 成本:" + LongPositionPrice.ToString() + "/" + ShortPositionPrice.ToString();

        }

    }
     * 
     * ***/
}
