//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    /// <summary>
//    /// 持仓回合维护器
//    /// 用于监控一个持仓对象的开平周期,
//    /// 从开始持有仓位到所有持仓平掉归零的一个持仓操作过程
//    /// </summary>
//    public class PositionRoundTracker
//    {
//        /// <summary>
//        /// 处于开启过程中的持仓回合
//        /// </summary>
//        ConcurrentDictionary<string, PositionRoundImpl> _roundmap = new ConcurrentDictionary<string, PositionRoundImpl>();
        
//        /// <summary>
//        /// 已经关闭的持仓回合
//        /// </summary>
//        ThreadSafeList<PositionRoundImpl> _roundlog = new ThreadSafeList<PositionRoundImpl>();

//        /// <summary>
//        /// 返回已经关闭的持仓操作回合
//        /// </summary>
//        public IEnumerable<PositionRoundImpl> RoundClosed
//        {
//            get
//            {
//                return _roundlog.ToArray();
//            }
//        }
//        /// <summary>
//        /// PositionTrans填充结束后 运行,检测当前map中的positionround,将closed的round放置到roundlog,并从roundmap中移除
//        /// roundlog则插入到对应的历史记录,roundmap则用于下次启动的时候从数据库进行加载(隔夜操作)
//        /// 仓位每日按照收盘价格进行结算并结转盈亏体现到当前权益,账户持仓类表则根据结算价进行成本变更.
//        /// positionround则不进行成本变更,posiitonround体现的是真实的开仓与平仓操作
//        /// </summary>
//        /*
//        void FinishGotPosTrans()
//        {
//            List<string> closed = new List<string>();
//            foreach (string key in _roundmap.Keys)
//            {
//                if (_roundmap[key].IsClosed)
//                {
//                    _roundlog.Add(_roundmap[key]);
//                    closed.Add(key);
//                }
//            }
//            foreach (string key in closed)
//            {
//                _roundmap.Remove(key);
//            }
//        }**/

//        /// <summary>
//        /// 返回仍然打开的持仓操作回合
//        /// </summary>
//        public IEnumerable<PositionRound> RoundOpened
//        {
//            get
//            {
//                return _roundmap.Values;
//            }
//        }

//        //public void Display()
//        //{
//        //    foreach (PositionRound pr in _roundlog)
//        //    {
//        //        Util.Debug(pr.ToString());
//        //    }
//        //}

//        //public void DisplayAll()
//        //{
//        //    Display();
//        //    foreach (string key in _roundmap.Keys)
//        //    {
//        //        Util.Debug(_roundmap[key].ToString());
//        //    }
//        //}

       

//        /// <summary>
//        /// 获得某个key对应的开启的仓位操作记录
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public PositionRoundImpl this[string key]
//        {
//            get
//            {
//                PositionRoundImpl pr;
//                if (_roundmap.TryGetValue(key, out pr))
//                {
//                    if (pr.IsOpened) return pr;
//                    return null;
//                }
//                return null;
//            }
//        }

//        /// <summary>
//        /// 每日收盘后将position hold数据与PR数据进行同步 account/symbol/数量 进行一致性校验
//        /// </summary>
//        /// <param name="hold"></param>
//        public void SyncPositionHold(IEnumerable<Position> hold)
//        {
//            //debug("同步positonhold...");
//            List<string> keylist = new List<string>();
//            //1.将当前PR数据与posiiton hold数据进行矫正,错误的修改,缺失的添加
//            foreach (Position p in hold)
//            {
//                SyncPosition(p);
//                keylist.Add(PositionRoundImpl.GetPRKey(p));//将同步过的key进行记录
//                //debug("同步positon:"+p.ToString());
//            }

//            //2.将position hold列表外的所有PR数据删除 完成 position hold 与 PositionRound的数据之间的一致性同步
//            //对别数据然后进行数据同步
//            IncludeKeyList(keylist);
//        }

//        /// <summary>
//        /// 同步持仓数据和PR数据,PositionRound记录了账户的开平回合记录,是交易来回的记录
//        /// 持仓数据是所有交易累加到当前的持仓状态状态信息,
//        /// 所有数据正确，持仓数据和仓位操作数据应该对应,有持仓表明没有平仓，就应该有对应的PositionRound信息
//        /// 同步某个持仓数据,当结算结束后,将PR数据和当前positionhold进行同步,
//        /// 目的1.每天调整PR错误，尽可能将PR数据和position数据吻合,这样PR数据就精确 计算出来的收益与指标均正确
//        /// 2.保持数据库所有数据统一
//        /// </summary>
//        /// <param name="p"></param>
//        /// <returns></returns>
//        bool SyncPosition(Position p)
//        {
//            //debug("同步position:" + p.ToString());
//            string key = PositionRoundImpl.GetPRKey(p);
//            PositionRoundImpl pr;
//            //positionround open 记录中包含对应的pr数据则进行数据比对并调整
//            if (_roundmap.TryGetValue(key, out pr))
//            {
//                if (pr.EqualPosition(p))
//                {
//                    return true;//如果数据吻合则直接返回

//                }
//                else
//                {
//                    _roundmap[key] = Posiotn2PR(p);
//                    return false;
//                }

//            }
//            else //不存在 则通过Position数据生成一条数据
//            {
//                _roundmap.TryAdd(key, Posiotn2PR(p));
//                return false;
//            }
//        }

//        /// <summary>
//        /// 将key列表外的open PR数据排除  并返回我们排除的列表
//        /// </summary>
//        /// <param name="keylist"></param>
//        List<string> IncludeKeyList(List<string> keylist)
//        {
//            //debug("includekeylist keynum:"+_roundmap.Keys.Count.ToString());
//            List<string> removelist = new List<string>();
//            foreach (string key in _roundmap.Keys)
//            {
//                //debug("start check key:" + key.ToString());
//                try
//                {
//                    if (!keylist.Contains(key))//position hold列表中没有该仓位 则记录，我们需要删除以和positionhold进行同步
//                    {
//                        removelist.Add(key);
//                        //debug("pr ok here");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Util.Debug("error:" + ex.ToString());
//                }
//                //debug("include key:"+key.ToString());

//            }
//            foreach (string key in removelist)
//            {
//                //debug("删除key:"+key);
//                PositionRoundImpl removedpr;
//                _roundmap.TryRemove(key, out removedpr);
//            }
//            return removelist;
//        }

//        /// <summary>
//        /// 将某个position转换成对应的PR数据,在转换的过程中 加减仓数据会丢失，只是针对当前的持仓状态进行的PR数据复原
//        /// </summary>
//        /// <param name="p"></param>
//        /// <returns></returns>
//        PositionRoundImpl Posiotn2PR(Position p)
//        {
//            PositionRoundImpl pr = new PositionRoundImpl(p.Account, p.oSymbol, p.isLong);
//            //pr.EntryTime  = p.
//            pr.EntrySize = p.Size;
//            pr.EntryPrice = p.AvgPrice;
//            //pr.ExitCommission = 0;
//            pr.SetOpen();//设定该pr为开启状态
//            return pr;
//        }



//        /// <summary>
//        /// 将positionround list恢复到 roundmap中去
//        /// </summary>
//        /// <param name="prlist"></param>
//        public void RestorePositionRounds(IEnumerable<PositionRoundImpl> prlist)
//        {
//            foreach (PositionRoundImpl pr in prlist)
//            {
//                if (pr == null) continue;
//                if (!pr.IsValid) continue;//如果positionround无效 则不加载到维护器中
//                string key = pr.PRKey;
//                if (!_roundmap.ContainsKey(key))
//                {

//                    _roundmap.TryAdd(key, pr);
//                }
//            }

//        }

//        int wrongpt = 0;
//        /// <summary>
//        /// 记录一条持仓操作记录数据
//        /// </summary>
//        /// <param name="p"></param>
//        public PositionRound GotPositionTransaction(PositionTransaction p)
//        {
//            string key = PositionRoundImpl.GetPRKey(p);

//            PositionRoundImpl pr = null;
//            if (_roundmap.TryGetValue(key, out pr))//存在对应的key
//            {
//                if (!pr.GotPositionTransaction(p)) wrongpt++;
//            }
//            else//不存在key则新建一个
//            {
//                pr = new PositionRoundImpl(p.Account, p.oSymbol, p.Trade.PositionSide);
//                _roundmap.TryAdd(key, pr);
//                if (!pr.GotPositionTransaction(p)) wrongpt++;
//            }

//            //检查持仓是否关闭
//            if (pr.IsClosed)
//            {
//                //如果已经平仓,则将该positionround数据添加到log中并且从map中移除该pr记录
//                _roundlog.Add(pr);

//                PositionRoundImpl prremoved;
//                _roundmap.TryRemove(key, out prremoved);
//            }
//            return pr;
//        }

//        public void Clear()
//        {
//            _roundlog.Clear();
//            _roundmap.Clear();
//        }

//    }
//}
