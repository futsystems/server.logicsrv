using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    /// <summary>
    /// 仓位开平来回,从建仓开始 经过 加仓/减仓 最后 平掉 为一个仓位操作回合.系统对选手的考核通过仓位操作回合来进行
    /// 计算选手操作次数,胜率,平均持仓周期等数据
    /// </summary>
    public class PositionRound :IPositionRound
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        /// <summary>
        /// 储存了成交序列,按照该序列的成交 完成了一个positionround
        /// </summary>
        ThreadSafeList<PositionTransaction> _postransactionlist = new ThreadSafeList<PositionTransaction>();


        
        public PositionRound(string account,Symbol symbol)
        {
            Account = account;//记录账户
            oSymbol = symbol;
        }


        /// <summary>
        /// 对应合约对象
        /// </summary>
        public Symbol oSymbol { get; set; }

        /// <summary>
        /// 品种类被
        /// </summary>
        public SecurityType Type { get { return oSymbol.SecurityType; } }

        /// <summary>
        /// 乘数
        /// </summary>
        public int Multiple { get { return oSymbol.Multiple; } }

        public void SetOpen()
        {
            _opened = true;
        }


        bool _opened = false;
        /// <summary>
        /// 开仓回合开启标志
        /// </summary>
        public bool IsOpened { get { return _opened; } }

        bool _closed = false;
        /// <summary>
        /// 开仓回合关闭标志
        /// </summary>
        public bool IsClosed { get { return _closed; } }

       
        /// <summary>
        /// rpt获得一个positiontrans
        /// 持仓回合获得一个持仓成交记录
        /// </summary>
        /// <param name="postrans"></param>
        /// <returns></returns>
        public bool GotPositionTransaction(PositionTransaction postrans)
        {
            try
            {
                //回合记录已经关闭 则不再记录其他position
                if (IsClosed)
                {
                    //debug("回合关闭 拒绝记录");
                    return false;
                }
                //没有开启则忽略除开仓以外的其他仓位操作,没有初始开仓记录 则平仓 加仓 减仓 均不做记录
                if (!IsOpened && postrans.PosOperation != QSEnumPosOperation.EntryPosition)
                {
                    //debug("仓位没有开启,拒绝其他操作");
                    return false;//没有开仓 
                }
                //已经开仓,则忽略开仓记录
                if (IsOpened && postrans.PosOperation == QSEnumPosOperation.EntryPosition)
                {
                    //debug("仓位已经开启,拒绝开启操作");
                    return false;//已经开仓 又收到entrypostion操作
                }
                //以上为positionround的对成交的过滤

                //开仓
                if (postrans.PosOperation == QSEnumPosOperation.EntryPosition)
                {
                    //debug("仓位开启");
                    _opened = true;//标记已开仓
                    Side = postrans.Size > 0 ? true : false;//标记多空方向
                    _entrytime = postrans.Time;//记录开仓时间
                }

                //建仓或者加仓
                if ((postrans.PosOperation == QSEnumPosOperation.EntryPosition) || (postrans.PosOperation == QSEnumPosOperation.AddPosition))
                {
                    //debug("仓位增加");
                    int oldsize = _entrysize;
                    _entrysize += postrans.Size;//数量增加
                    //_entrycommission += postrans.Commission;//累加手续费
                    _entryprice = (oldsize * _entryprice + postrans.Size * postrans.Price) / Convert.ToDecimal(_entrysize);//计算均价
                }

                //平仓或者减仓
                if ((postrans.PosOperation == QSEnumPosOperation.ExitPosition) || (postrans.PosOperation == QSEnumPosOperation.DelPosition))
                {
                    //debug("仓位减少");
                    int oldsize = _exitsize;
                    _exitsize += postrans.Size;//数量累加
                    //_exitcommission += postrans.Commission;//累加手续费
                    _exitprice = (oldsize * _exitprice + postrans.Size * postrans.Price) / Convert.ToDecimal(_exitsize);//计算均价
                }

                //平仓
                if (postrans.PosOperation == QSEnumPosOperation.ExitPosition)
                {
                    //debug("仓位关闭");
                    _closed = true;
                    _opened = false;
                    _exittime = postrans.Time;
                }

                //记录成交记录所记录的最高价与最低价
                _highest = Math.Max(_highest, postrans.Highest);
                _lowest = Math.Min(_lowest, postrans.Lowest);

                //保存仓位变动数据
                _postransactionlist.Add(new PositionTransaction(postrans));
                return true;
            }
            catch (Exception ex)
            {
                debug("some error:" + ex.ToString());
                return false;
            }           
        }

        /// <summary>
        /// 获得positiontransaction key
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetPRKey(PositionTransaction p)
        {
            return p.Account + "-" + p.Symbol;
        }

        /// <summary>
        /// 获得position key
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string GetPRKey(Position position)
        {
			return position.Account + "-" + position.Symbol;
        }

        public string PRKey
        {
            get {
                return Account+ "-" + Symbol;
            }
        }
        
        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get { return oSymbol.Symbol; }}

        /// <summary>
        /// 品种
        /// </summary>
        public string Security { get { return oSymbol.SecurityFamily.Code; } }

        /// <summary>
        /// 多空
        /// </summary>
        public bool Side { get; set; }

        /// <summary>
        /// 当前持仓数量
        /// </summary>
        public int HoldSize { get { return (_entrysize + _exitsize); } }

        int _entrysize = 0;
        /// <summary>
        /// 总建仓数量
        /// </summary>
        public int EntrySize { get { return _entrysize; } set { _entrysize = value; } }

        DateTime _entrytime;
        /// <summary>
        /// 开仓时间
        /// </summary>
        public DateTime EntryTime { get { return _entrytime; } set { _entrytime = value; } }

        decimal _entryprice;
        /// <summary>
        /// 开仓价
        /// </summary>
        public decimal EntryPrice { get { return _entryprice; } set { _entryprice = value; } }

        //decimal _entrycommission = 0;
        /// <summary>
        /// 开仓手续费 通过累加所有开仓操作的手续费来得到 累计的开仓手续费
        /// </summary>
        public decimal EntryCommission { get {

            decimal total = 0;
            foreach (PositionTransaction pt in _postransactionlist)
            {
                if (pt.PosOperation == QSEnumPosOperation.EntryPosition || pt.PosOperation == QSEnumPosOperation.AddPosition)
                {
                    total += pt.Commission;
                }
            }
            return total;


            }}

        int _exitsize = 0;
        /// <summary>
        /// 总平仓数量
        /// </summary>
        public int ExitSize { get { return _exitsize; } set { _exitsize = value; } }
        DateTime _exittime;
        /// <summary>
        /// 平仓时间
        /// </summary>
        public DateTime ExitTime { get { return _exittime; } set { _exittime = value; } }

        decimal _exitprice;
        /// <summary>
        /// 平仓价格
        /// </summary>
        public decimal ExitPrice { get { return _exitprice; } set { _exitprice = value; } }


        /// <summary>
        /// 总平仓手续费
        /// </summary>
        public decimal ExitCommission { get {

            decimal total = 0;
            foreach (PositionTransaction pt in _postransactionlist)
            {
                if (pt.PosOperation == QSEnumPosOperation.ExitPosition || pt.PosOperation == QSEnumPosOperation.DelPosition)
                {
                    total += pt.Commission;
                }
            }
            return total;

            } } 



        decimal _highest = decimal.MinValue;
        public decimal Highest { get { return _highest==decimal.MinValue ?EntryPrice:_highest; }  }

        decimal _lowest = decimal.MaxValue;
        public decimal Lowest { get { return _lowest==decimal.MaxValue?EntryPrice:_lowest; } }


        //当计算仓位操作回合的时候 我们进行仓位操作是否closed的判断,如果没有closed则直接返回0
        /// <summary>
        /// 平均每手盈亏点数
        /// </summary>
        public decimal Points { get { return IsClosed ?((_entryprice - _exitprice) * (this.Side == true ? -1 : 1) ): 0; } }
        
        /// <summary>
        /// 单个回合总共盈亏点数
        /// </summary>
        public decimal TotalPoints { get { return IsClosed ?(Points * Math.Abs(this.EntrySize)):0; } }

        /// <summary>
        /// 盈亏(不含手续费)
        /// </summary>
        public decimal Profit { get { return IsClosed ?(TotalPoints * this.Multiple):0; } }

        /// <summary>
        /// 累计手续费
        /// </summary>
        public decimal Commissoin { get { return Math.Abs(EntryCommission) + Math.Abs(ExitCommission); } }

        /// <summary>
        /// 净盈亏
        /// </summary>
        public decimal NetProfit { get { return IsClosed ?(Profit - Commissoin):0; } }


        /// <summary>
        /// 盈亏标识
        /// </summary>
        public bool WL { get { return IsClosed ?((NetProfit >= 0)):true; } }

        /// <summary>
        /// 净交易数量
        /// </summary>
        public int Size { get { return Math.Abs(this.EntrySize); } }


        /// <summary>
        /// 判断一个持仓数据和PR数据是否吻合,account/symbol/holdsize
        /// PR的成本为所有开仓数据的加权成本，Position反应的是当前持仓的一个持有成本。当持有过程中发生过加减仓操作 价格数据就不吻合了
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool EqualPosition(Position p)
        {
            if (p.Account == Account && p.Symbol == Symbol && p.Size == HoldSize) return true;
            return false;
            
        }

        public override string ToString()
        {
                //return Account + "," + Symbol + "," +Security+","+ EntryTime.ToString() + "," + EntrySize.ToString() + "," + EntryPrice.ToString() + "," + ExitTime.ToString() + "," + ExitSize.ToString() + "," + ExitPrice.ToString() + "," + Highest.ToString() + "," + Lowest.ToString() + "," + HoldSize.ToString()+","+EntryCommission.ToString()+","+ExitCommission.ToString()+","+Side.ToString()+","+WL.ToString()+","+Points.ToString()+","+TotalPoints.ToString()+","+Profit.ToString()+","+Commissoin.ToString()+","+NetProfit.ToString();
                string nm = Account + "_" + Symbol + " 方向:" + Side.ToString() + " 开仓:" + EntryTime.ToString() + "," + EntrySize.ToString() + "," + LibUtil.FormatDisp(EntryPrice) + " 平仓:" + ExitTime.ToString() + "," + ExitSize.ToString() + "," + LibUtil.FormatDisp(ExitPrice) + " 最高:" + LibUtil.FormatDisp(Highest) + " 最底:" + LibUtil.FormatDisp(Lowest) + " 持有数量:" + HoldSize.ToString() + " 开仓手续费:" + LibUtil.FormatDisp(EntryCommission) + " 平仓手续费:" + LibUtil.FormatDisp(ExitCommission);

                if (IsClosed)
                    nm = nm + "盈亏:" + WL.ToString() + " 总点数:" + LibUtil.FormatDisp(TotalPoints) + " 总盈利:" + LibUtil.FormatDisp(Profit) + " 总手续费:" + LibUtil.FormatDisp(Commissoin) + " 净利润:" + LibUtil.FormatDisp(NetProfit);
                return nm;
        }


    }

    /// <summary>
    /// 交易回合记录器,用于记录交易员的成交回合,一次开 平回合
    /// 
    /// </summary>
    [Serializable]
    public class PositionRoundTracker
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        public event Str2SymbolDel FindSymbolEvent;
        Symbol FindSymbol(string symbol)
        {
            if (FindSymbolEvent != null)
                return FindSymbolEvent(symbol);
            return null;
        }
        /*
        public event FindSecurity FindSecurityEvent;
        Security findSecurity(string symbol)
        {
            if (FindSecurityEvent != null)
                return FindSecurityEvent(symbol);
            return null;
        }**/

        ConcurrentDictionary<string, PositionRound> _roundmap = new ConcurrentDictionary<string, PositionRound>();
        ThreadSafeList<PositionRound> _roundlog = new ThreadSafeList<PositionRound>();

        /// <summary>
        /// 返回已经关闭的持仓操作回合
        /// </summary>
        public PositionRound[] RoundClosed
        {
            get {
                //FinishGotPosTrans();
                return _roundlog.ToArray();
            }
        }
        /// <summary>
        /// PositionTrans填充结束后 运行,检测当前map中的positionround,将closed的round放置到roundlog,并从roundmap中移除
        /// roundlog则插入到对应的历史记录,roundmap则用于下次启动的时候从数据库进行加载(隔夜操作)
        /// 仓位每日按照收盘价格进行结算并结转盈亏体现到当前权益,账户持仓类表则根据结算价进行成本变更.
        /// positionround则不进行成本变更,posiitonround体现的是真实的开仓与平仓操作
        /// </summary>
        /*
        void FinishGotPosTrans()
        {
            List<string> closed = new List<string>();
            foreach (string key in _roundmap.Keys)
            {
                if (_roundmap[key].IsClosed)
                {
                    _roundlog.Add(_roundmap[key]);
                    closed.Add(key);
                }
            }
            foreach (string key in closed)
            {
                _roundmap.Remove(key);
            }
        }**/

        /// <summary>
        /// 返回仍然打开的持仓操作回合
        /// </summary>
        public PositionRound[] RoundOpened
        {
            get
            {
                //FinishGotPosTrans();
                return _roundmap.Values.ToArray();
            }
        }

        public void Display()
        {
            foreach (PositionRound pr in _roundlog)
            {
                debug(pr.ToString());
            }
        }

        public void DisplayAll()
        {
            Display();
            foreach(string key in _roundmap.Keys)
            {
                debug(_roundmap[key].ToString());
            }
        }

        //public string genKey(Position p)
        //{
        //    return p.Account + "-" + p.symbol;
        //}
        /// <summary>
        /// 获得某个key对应的开启的仓位操作记录
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public PositionRound this[string key]
        {
            get
            {
                PositionRound pr;
                if (_roundmap.TryGetValue(key, out pr))
                {
                    if (pr.IsOpened) return pr;
                    return null;
                }
                return null;
            }
        }

        /// <summary>
        /// 每日收盘后将position hold数据与PR数据进行同步 account/symbol/数量 进行一致性校验
        /// </summary>
        /// <param name="hold"></param>
        public void SyncPositionHold(Position[] hold)
        {
            //debug("同步positonhold...");
            List<string> keylist = new List<string>();
            //1.将当前PR数据与posiiton hold数据进行矫正,错误的修改,缺失的添加
            foreach (Position p in hold)
            {
                SyncPosition(p);
                keylist.Add(PositionRound.GetPRKey(p));//将同步过的key进行记录
                //debug("同步positon:"+p.ToString());
            }

            //2.将position hold列表外的所有PR数据删除 完成 position hold 与 PositionRound的数据之间的一致性同步
            //对别数据然后进行数据同步
            IncludeKeyList(keylist);
        }
        /// <summary>
        /// 同步持仓数据和PR数据,PositionRound记录了账户的开平回合记录,是交易来回的记录
        /// 持仓数据时所有交易累加到当前的状态信息,
        /// 所有数据正确，持仓数据和仓位操作数据应该对应,有持仓表明没有平仓，就应该有对应的PositionRound信息
        /// 同步某个持仓数据,当结算结束后,将PR数据和当前positionhold进行同步,
        /// 目的1.每天调整PR错误，尽可能将PR数据和position数据吻合,这样PR数据就精确 计算出来的收益与指标均正确
        /// 2.保持数据库所有数据统一
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool SyncPosition(Position p)
        {
            //debug("同步position:" + p.ToString());
            string key = PositionRound.GetPRKey(p);
            PositionRound pr;
            //positionround open 记录中包含对应的pr数据则进行数据比对并调整
            if (_roundmap.TryGetValue(key, out pr))
            {
                if (pr.EqualPosition(p))
                {
                    return true;//如果数据吻合则直接返回

                }
                else
                {
                    _roundmap[key] = Posiotn2PR(p);
                    return false;
                }

            }
            else //不存在 则通过Position数据生成一条数据
            {
                
                _roundmap.TryAdd(key, Posiotn2PR(p));
                //debug("增加PR数据");
                return false;
            }
        }
        /// <summary>
        /// 将key列表外的open PR数据排除  并返回我们排除的列表
        /// </summary>
        /// <param name="keylist"></param>
        List<string> IncludeKeyList(List<string> keylist)
        {
            //debug("includekeylist keynum:"+_roundmap.Keys.Count.ToString());
            List<string> removelist = new List<string>();
            foreach(string key in _roundmap.Keys)
            {
                //debug("start check key:" + key.ToString());
                try
                {
                    if(!keylist.Contains(key))//position hold列表中没有该仓位 则记录，我们需要删除以和positionhold进行同步
                    {
                        removelist.Add(key);
                        //debug("pr ok here");
                    }
                }
                catch (Exception ex)
                {
                    debug("error:" + ex.ToString());
                }
                //debug("include key:"+key.ToString());

            }
            foreach(string key in removelist)
            {
                //debug("删除key:"+key);
                PositionRound removedpr;
                _roundmap.TryRemove(key,out removedpr);
            }
            return removelist;
        }
        /// <summary>
        /// 将某个position转换成对应的PR数据,在转换的过程中 加减仓数据会丢失，只是针对当前的持仓状态进行的PR数据复原
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        PositionRound Posiotn2PR(Position p)
        {
            Symbol sym = FindSymbol(p.Symbol);
            if (sym == null) return null;
            PositionRound pr = new PositionRound(p.Account,p.oSymbol);
            //pr.EntryTime  = p.
            pr.EntrySize = p.Size;
            pr.EntryPrice = p.AvgPrice;
            //pr.ExitCommission = 0;
            pr.SetOpen();//设定该pr为开启状态
            return pr;
        }

        

        /// <summary>
        /// 将positionround list恢复到 roundmap中去
        /// </summary>
        /// <param name="prlist"></param>
        public void RestorePositionRounds(IList<PositionRound> prlist)
        {
            foreach (PositionRound pr in prlist)
            {
                string key = pr.PRKey;
                if (!_roundmap.ContainsKey(key))
                {
                   
                    _roundmap.TryAdd(key, pr);
                }
            }
            
        }

        int wrongpt = 0;
        /// <summary>
        /// 记录一条持仓操作记录数据
        /// </summary>
        /// <param name="p"></param>
        public IPositionRound GotPositionTransaction(PositionTransaction p)
        {
            string key = PositionRound.GetPRKey(p);//p.Account + "-" + p.Symbol;//PositionTransaction key值用于索引唯一的account-symbol positionround维护

            PositionRound pr = null;
            if (_roundmap.TryGetValue(key, out pr))//存在对应的key
            {
                //存在则需要进行判断是否closed,如果closed则我们将closed的记录放入log.然后新建一个空PositionRound
                //if (pr.IsClosed)
                //{
                //    _roundlog.Add(pr);//如果已经平仓,则将该positionround数据添加到log中
                //    pr = new PositionRound(p.Account, p.Symbol, p.Security, p.Multiple);
                //    //pr.SendDebugEvent  +=new DebugDelegate(debug);
                //    _roundmap[key] = pr;
                //    if(!pr.GotPositionTransaction(p)) wrongpt++;
                //}
                //else
                //{
                if(!pr.GotPositionTransaction(p)) wrongpt++;
                //}
            }
            else//不存在key则新建一个
            {
                pr =  new PositionRound(p.Account,p.oSymbol);
                _roundmap.TryAdd(key,pr);
                //pr.SendDebugEvent += new DebugDelegate(debug);
                if(!pr.GotPositionTransaction(p)) wrongpt++;
            }

            //检查持仓是否关闭,如果关闭则对外触发相应的事件
            if (pr.IsClosed)
            {
                _roundlog.Add(pr);//如果已经平仓,则将该positionround数据添加到log中
                //同时新建一个positionround添加到roundmap中,用于等待新的成交进入(初始状态是未开启)
                //pr = new PositionRound(p.Account, p.Symbol, p.Security, p.Multiple);
                PositionRound prremoved;
                _roundmap.TryRemove(key, out prremoved);
                //pr.SendDebugEvent  +=new DebugDelegate(debug);
            }
            return pr;
        }
        

        public void Clear()
        {
            _roundlog.Clear();
            _roundmap.Clear();
        }

    }
}
