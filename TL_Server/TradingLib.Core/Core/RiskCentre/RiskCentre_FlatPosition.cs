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
        /// <summary>
        /// 委托来源
        /// </summary>
        public QSEnumOrderSource Source { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ForceCloseReason { get; set; }

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

        /// <summary>
        /// 发送的平仓委托OrderID
        /// </summary>
        public long OrderID { get; set; }

        /// <summary>
        /// 强迫异常或失效
        /// </summary>
        public bool FlatFailed { get; set; }

        public PositionFlatSet(Position pos, QSEnumOrderSource source, string closereason)
        {
            Position = pos;
            SentTime = DateTime.Now;
            FireCount = 1;
            Source = source;
            ForceCloseReason = closereason;
            FlatFailed = false;
        }

        public override string ToString()
        {
            return "Pos:" + Position.Account + "-" + Position.Symbol + " OID:" + OrderID.ToString() + " FireCount:" + FireCount.ToString() + " SentTime:" + SentTime.ToString();
        }
    }

    public partial class RiskCentre
    {

        /// <summary>
        /// 强迫成功事件
        /// </summary>
        public event PositionDelegate GotFlatSuccessEvent;

        /// <summary>
        /// 强迫异常事件
        /// </summary>
        public event PositionDelegate GotFlatFailedEvent;






        //待平仓列表,主要包含系统尾盘集中强平,系统内部风控强平等形成的平仓指令
        ThreadSafeList<PositionFlatSet> posflatlist = new ThreadSafeList<PositionFlatSet>();
        
        #region【持仓强平的循环检查与维护】

        /// <summary>
        /// 检查某个持仓是否在强平队列
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsPosFlatPending(Position pos)
        {

            string key = pos.GetPositionKey();

            foreach (PositionFlatSet ps in posflatlist)
            {
                if (ps.Position.GetPositionKey().Equals(key))
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
        public void FlatPosition(string accid, QSEnumOrderSource source, string closereason = "系统强平")
        {
            //debug("平掉账户:" + accid + "所有仓位", QSEnumDebugLevel.INFO);
            foreach (Position pos in _clearcentre.getPositions(accid))//遍历该账户的所有仓位 若不是空仓则市价平仓
            {
                if (!pos.isFlat)
                {
                    FlatPosition(pos, source, closereason);
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
        public void FlatPosition(Position pos, QSEnumOrderSource ordersource, string closereason = "系统强平")
        {
            FlatPosition(pos, ordersource, closereason, true);
        }


        long FlatPosition(Position pos, QSEnumOrderSource ordersource, string closereason, bool first)
        {
            //如果该持仓已经在平仓队列中并且是标注为第一次发送强平指令则直接返回(表面已经触发过强平指令)
            if (first &&IsPosFlatPending(pos))
                return 0;

            debug("RiskCentre Flatpostion:" + pos.ToString(), QSEnumDebugLevel.INFO);
            //生成市价委托
            Order o = new MarketOrderFlat(pos);
            o.Account = pos.Account;
            o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
            o.OrderSource = ordersource;
            o.ForceClose = true;
            o.ForceCloseReason = closereason;
            
            //o.price = 2500;//模拟不成交延迟撤单的情况

            //绑定委托编号 用于提前获得系统唯一OrderID 方便撤单
            if (AssignOrderIDEvent != null)
                AssignOrderIDEvent(ref o);

            BasicTracker.SymbolTracker.TrckerOrderSymbol(o);
            //对外发送委托
            SendOrder(o);

            //第一次强平 将持仓信组成flatset放入系统队列
            if (first)
            {
                //将持仓加入监控列表
                PositionFlatSet ps = new PositionFlatSet(pos, ordersource, closereason);
                ps.OrderID = o.id;
                posflatlist.Add(ps);
            }
            return o.id;
        }

        int SENDORDERDELAY = 3;
        int SENDORDERRETRY = 3;
        bool waitforcancel = true;

        
        /// <summary>
        /// 监控强平持仓列表,用于观察委托是否正常平仓
        /// 强平是一个触发过程
        /// 系统内部的帐户风控规则检查或者定时检查 扫描到需要强平的仓位或者帐户就会进行强平
        /// 执行强平后 对应的持仓将进入队列进行监控处理 直到强平成功或者异常
        /// </summary>
        void ProcessPositionFlat()
        {
            //debug("检查待平仓列表...", QSEnumDebugLevel.INFO);
            
            foreach (PositionFlatSet ps in posflatlist)
            {
                //如果持仓已经平掉 则加入待删除列表
                //如果持仓平调 但是在平调后立马再次开仓，此时PostioinFlat并没有从队列中删除，当再次扫描到该持仓时 系统会认为该持仓没有被及时平调,从而尝试撤单并重新强平，最后单子又无法撤单成功
                //需要监听positionround closed event 然后将列表删除
                //if (ps.Position.isFlat)
                //{
                //    debug("Position:"+ps.Position.GetPositionKey() +" 已经平掉,从队列中移除", QSEnumDebugLevel.INFO);
                //    //deletelist.Add(ps);
                //}
                //else//持仓未平掉则 检查平仓间隔/平仓次数/触发报警
                if(!ps.Position.isFlat)//如果有持仓 则检查时间 进行撤单并再次强平
                {
                    if (DateTime.Now.Subtract(ps.SentTime).TotalSeconds >= SENDORDERDELAY && ps.FireCount < SENDORDERRETRY)
                    {
                        
                        if (ps.OrderID > 0)
                        {
                            debug(ps.Position.GetPositionKey() + " 时间超过3秒仍然没有平掉持仓,取消该委托", QSEnumDebugLevel.INFO);
                            CancelOrder(ps.OrderID);
                            if (waitforcancel)
                                continue;
                        }
                        else
                        {
                            debug(ps.Position.GetPositionKey() + " 原有委托已撤掉,重新平仓", QSEnumDebugLevel.INFO);
                            ps.FireCount++;
                            ps.SentTime = DateTime.Now;
                            ps.OrderID = FlatPosition(ps.Position, ps.Source, ps.ForceCloseReason, false);
                            continue;
                        }
                    }
                    if (ps.FireCount == SENDORDERRETRY && (!ps.FlatFailed))//报警的时间设定
                    {
                        debug(ps.Position.GetPositionKey() + " 平仓次数超过" + SENDORDERRETRY +" 出发警告通知", QSEnumDebugLevel.INFO);
                        if (ps.OrderID != 0)
                        {
                            CancelOrder(ps.OrderID);
                        }
                        ps.FlatFailed = true;
                        //强迫异常后冻结交易帐户 等待处理 这里需要对外触发事件 相关扩展模块监听后进行通知
                        IAccount account = _clearcentre[ps.Position.Account];
                        if (account != null)
                        {
                            account.InactiveAccount();
                        }
                        //对外进行未正常平仓报警
                        if (GotFlatFailedEvent != null)
                        {
                            GotFlatFailedEvent(ps.Position);
                        }
                        
                    }
                }
            }


           
        }

        /// <summary>
        /// 保证金检查,当保证金超过可用资金时执行强制减仓
        /// 这里需要结合期货业务规则进行设计
        /// </summary>
        void ProcessPositionOff()
        { 
        
        }
        #endregion
    }
}
