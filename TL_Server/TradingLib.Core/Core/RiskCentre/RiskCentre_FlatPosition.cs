using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace TradingLib.Core
{
    internal class PositionFlatSet
    {
        public QSEnumOrderSource Source { get; set; }

        public string Comment { get; set; }
        /// <summary>
        /// 待平持仓
        /// </summary>
        public Position Position { get; set; }
        /// <summary>
        /// 平仓指令发送时间
        /// </summary>
        public DateTime SentTime { get; set; }
        /// <summary>
        /// 平仓指令发送次数
        /// </summary>
        public int FireCount { get; set; }


        public long OrderID { get; set; }
        public PositionFlatSet(Position pos, QSEnumOrderSource source, string comment)
        {
            Position = pos;
            SentTime = DateTime.Now;
            FireCount = 1;
            Source = source;
            Comment = comment;
        }

        public override string ToString()
        {
            return "Pos:" + Position.Account + "-" + Position.Symbol + " OID:" + OrderID.ToString() + " FireCount:" + FireCount.ToString() + " SentTime:" + SentTime.ToString();
        }
    }

    public partial class RiskCentre
    {
        //待平仓列表,主要包含系统尾盘集中强平,系统内部风控强平等形成的平仓指令
        ThreadSafeList<PositionFlatSet> posflatlist = new ThreadSafeList<PositionFlatSet>();
        
        #region【持仓强平的循环检查与维护】

        bool IsPosFlatPending(Position pos)
        {

            foreach (PositionFlatSet ps in posflatlist)
            {
                if (PosKey(ps.Position).Equals(PosKey(pos)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 平掉某个账户的所有仓位(风控里面的 强平并冻结账户)
        /// 注,这里需要封装发单方式,若系统还有未成交的合约,我们需要先撤掉所有委托,然后再发新的委托
        /// 注:Brokerrouter内部 市价平仓有自动撤单机制,平仓是市价委托 因此可以自动提交由br进行撤单并平仓
        /// 但是平仓后 原来的建仓委托单可能没有被撤出，从而再次建仓。
        /// 1.指定要平仓的账户
        /// 2.指定该强平操作所产生委托所标识的Source
        /// 3.对该委托的标注
        /// 4.指定是否需要撤销该帐户的其他所有委托
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public  void FlatPosition(string accid, QSEnumOrderSource source, string comment = "系统强平")
        {
            debug("平掉账户:" + accid + "所有仓位", QSEnumDebugLevel.INFO);
            foreach (Position pos in _clearcentre.getPositions(accid))//遍历该账户的所有仓位 若不是空仓则市价平仓
            {
                if (!pos.isFlat)
                {
                    FlatPosition(pos, source, comment);
                }
                Thread.Sleep(100);
            }
        }


        /// <summary>
        /// 通过风控中心平掉某个持仓
        /// </summary>
        /// <param name="pos">持仓对象</param>
        /// <param name="ourdersource">委托源</param>
        /// <param name="comment">平仓备注</param>
        /// <returns></returns>
        public void FlatPosition(Position pos, QSEnumOrderSource ordersource, string comment)
        {
            FlatPosition(pos, ordersource, comment, true);
        }


        long FlatPosition(Position pos, QSEnumOrderSource ordersource, string comment,bool first)
        {
            debug("RiskCentre Flatpostion:" + pos.ToString(), QSEnumDebugLevel.INFO);
            //如果该持仓已经在平仓队列中并且是标注为第一次发送强平指令则直接返回(表面已经触发过强平指令)
            if (first &&IsPosFlatPending(pos))
                return 0;

            //生成市价委托
            Order o = new MarketOrderFlat(pos);
            o.Account = pos.Account;
            o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
            o.OrderSource = ordersource;
            o.comment = comment.Replace(',','_');
            //o.price = 2500;//模拟不成交延迟撤单的情况

            //绑定委托编号 用于提前获得系统唯一OrderID 方便撤单
            if (AssignOrderIDEvent != null)
                AssignOrderIDEvent(ref o);

            BasicTracker.SymbolTracker.TrckerOrderSymbol(o);
            //对外发送委托
            debug("对外发送委托:" + o.ToString(),QSEnumDebugLevel.INFO);
            SendOrder(o);
            if (first)
            {
                //将持仓加入监控列表
                PositionFlatSet ps = new PositionFlatSet(pos, ordersource, comment);
                ps.OrderID = o.id;
                posflatlist.Add(ps);

            }
            return o.id;
        }


        /// <summary>
        /// 持仓主键
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        string PosKey(Position pos)
        {
            return pos.Account + "-" + pos.Symbol;
        }


        int SENDORDERDELAY = 3;
        int SENDORDERRETRY = 3;
        bool waitforcancel = true;
        /// <summary>
        /// 监控持仓列表,用于观察委托是否正常平仓
        /// </summary>
        
        void ProcessPositionFlat()
        {
            //debug("检查待平仓列表...", QSEnumDebugLevel.INFO);
            List<PositionFlatSet> deletelist = new List<PositionFlatSet>();
            foreach (PositionFlatSet ps in posflatlist)
            {
                //如果持仓已经平掉 则加入待删除列表
                if (ps.Position.isFlat)
                {
                    debug("Pos"+ps.Position.Account +"-"+ps.Position.Symbol +" 已经平掉,从缓存中移除", QSEnumDebugLevel.INFO);
                    deletelist.Add(ps);
                }
                else//持仓未平掉则 检查平仓间隔/平仓次数/触发报警
                {
                    if (DateTime.Now.Subtract(ps.SentTime).TotalSeconds >= SENDORDERDELAY && ps.FireCount < SENDORDERRETRY)
                    {
                        
                        if (ps.OrderID > 0)
                        {
                            debug(PosKey(ps.Position) + " 时间超过3秒仍然没有平掉持仓,取消该委托", QSEnumDebugLevel.INFO);
                            CancelOrder(ps.OrderID);
                            if (waitforcancel)
                                continue;
                        }
                        else
                        {
                            debug(PosKey(ps.Position) + " 原有委托已撤掉,重新平仓", QSEnumDebugLevel.INFO);
                            ps.FireCount++;
                            ps.SentTime = DateTime.Now;
                            ps.OrderID = FlatPosition(ps.Position, ps.Source, ps.Comment, false);
                            continue;
                        }
                    }
                    if (ps.FireCount == SENDORDERRETRY)//报警的时间设定
                    {
                        debug(PosKey(ps.Position) + " 平仓次数超过" + SENDORDERRETRY, QSEnumDebugLevel.INFO);
                        if (ps.OrderID != 0)
                        {
                            CancelOrder(ps.OrderID);
                            
                        }
                        //对外进行未正常平仓报警
                        
                    }
                }
            }


            foreach (PositionFlatSet ps in deletelist)
            {
                posflatlist.Remove(ps);
            }
        }


        #endregion
    }
}
