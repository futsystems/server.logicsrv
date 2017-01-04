using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 在风控中心维护的强平或者撤单任务中分以下几个情况
    /// 1.强平某个持仓，该动作会将该持仓对应合约的所有待成交委托撤掉，然后再进行强平，强平成功后从队列删除
    /// 2.撤单后或强平，撤掉一组委托，待撤单成功后按设置生成对应的强平仓位的事务加入队列
    /// </summary>
    internal enum QSEnumRiskTaskType
    {
        /// <summary>
        /// 强平仓位
        /// 包含自动检测到的对应待成交委托
        /// </summary>
        FlatPosition,

        /// <summary>
        /// 撤单 单纯撤单不涉及强平
        /// </summary>
        CancelOrder,

        /// <summary>
        /// 强平所有持仓 先撤单然后再强平
        /// </summary>
        FlatAllPositions,
    }


    internal enum QSEnumRiskTaskStatus
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        Inited,
        /// <summary>
        /// 子任务已创建
        /// </summary>
        SubTaskGenerated,
        /// <summary>
        /// 撤单已发送
        /// </summary>
        CancelSent,
        /// <summary>
        /// 撤单超时
        /// </summary>
        CancelTimeOut,
        /// <summary>
        /// 撤单完成
        /// </summary>
        CancelDone,
        /// <summary>
        /// 强平发送
        /// </summary>
        FlatSent,
        /// <summary>
        /// 强平超时
        /// </summary>
        FlatTimeOut,
        /// <summary>
        /// 强平完成
        /// </summary>
        FlatDone,
    }

    internal class RiskTaskSet
    {
        /// <summary>
        /// 对应的交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 风控操作任务类型
        /// </summary>
        public QSEnumRiskTaskType TaskType { get; set; }

        /// <summary>
        /// 委托来源
        /// </summary>
        public QSEnumOrderSource Source { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ForceCloseReason { get; set; }


        QSEnumRiskTaskStatus _status = QSEnumRiskTaskStatus.Inited;
        /// <summary>
        /// 任务状态
        /// </summary>
        public QSEnumRiskTaskStatus TaskStatus
        {
            get { return _status; }
            set { _status = value; }
        }

        #region 持仓数据
        /// <summary>
        /// 待平持仓
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 是否需要强平持仓
        /// </summary>
        public bool NeedFlatPosition
        {
            get { return this.Position != null; }
        }

        /// <summary>
        ///  平仓数量
        /// </summary>
        public int FlatSize { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int RemainSize { get; set; }


        /// <summary>
        /// 发送的平仓委托OrderID
        /// </summary>
        public List<long> FlatOrderIDList { get; set; }


        /// <summary>
        /// 平仓指令发送时间
        /// </summary>
        public DateTime SentFlatTime { get; set; }

        /// <summary>
        /// 平仓指令发送次数
        /// </summary>
        public int FlatCount { get; set; }


        #endregion

        #region 撤单数据
        /// <summary>
        /// 待成交委托列表
        /// </summary>
        List<long> _pendingorders = new List<long>();

        /// <summary>
        /// 待撤单委托编号
        /// 需要在平仓前撤掉的委托
        /// </summary>
        public List<long> PendingOrders
        {
            get
            {
                return _pendingorders;
            }

            set
            {
                _pendingorders = value;
            }
        }

        /// <summary>
        /// 是否需要取消委托
        /// </summary>
        public bool NeedCancelOrders
        {
            get
            {
                return _pendingorders.Count > 0;
            }
        }

        /// <summary>
        /// 取消委托发送时间
        /// </summary>
        public DateTime SentCancelTime { get; set; }

        /// <summary>
        /// 撤单次数
        /// </summary>
        public int CancelCount { get; set; }

        #endregion


        #region FlatAll数据
        List<Position> _pendingpositions = new List<Position>();
        /// <summary>
        /// 待强平持仓
        /// </summary>
        public List<Position> PendingPositionFlat
        {
            get
            {
                return _pendingpositions;
            }

            set
            {
                _pendingpositions = value;
            }
        }

        /// <summary>
        /// 是否需要生成子任务平仓
        /// </summary>
        public bool NeedGenerateSubTask
        {
            get { return _pendingpositions.Count > 0; }
        }


        /// <summary>
        /// 由该任务开启的子任务
        /// </summary>
        public List<RiskTaskSet> SubTask { get; set; }
        #endregion

        public RiskTaskSet(string account, Position pos, List<long> pendingOrdres, int sizeFlat, QSEnumOrderSource source, string closeReason)
        {
            //设定平仓任务标识
            this.TaskType = QSEnumRiskTaskType.FlatPosition;

            this.Account = pos.Account;
            this.PendingOrders = pendingOrdres;
            this.Position = pos;
            //设定平仓数量以及剩余数量
            this.FlatSize = sizeFlat > pos.UnsignedSize ? pos.UnsignedSize : sizeFlat;
            this.RemainSize = pos.UnsignedSize - this.FlatSize;



            this.SentFlatTime = DateTime.Now;
            this.SentCancelTime = DateTime.Now;
            this.FlatCount = 0;
            this.CancelCount = 0;

            this.Source = source;
            this.ForceCloseReason = closeReason;
            this.SubTask = new List<RiskTaskSet>();
            this.FlatOrderIDList = new List<long>();

        }


        /// <summary>
        /// 先撤一组委托 然后再强平一组持仓，
        /// pendingpostions可以为
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="pendingpositons"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public RiskTaskSet(string account, List<long> orders, List<Position> pendingpositons, QSEnumOrderSource source, string closereason)
        {
            TaskType = QSEnumRiskTaskType.FlatAllPositions;

            this.Account = account;
            this.PendingOrders = orders;
            this.PendingPositionFlat = pendingpositons;

            this.SentFlatTime = DateTime.Now;
            this.SentCancelTime = DateTime.Now;
            this.FlatCount = 0;
            this.CancelCount = 0;

            this.Source = source;
            this.ForceCloseReason = closereason;
            this.SubTask = new List<RiskTaskSet>();
            this.FlatOrderIDList = new List<long>();
        }

        /// <summary>
        /// 撤掉一组委托事务
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="source"></param>
        /// <param name="closereason"></param>
        public RiskTaskSet(string account, List<long> orders, QSEnumOrderSource source, string closereason)
        {
            TaskType = QSEnumRiskTaskType.CancelOrder;

            this.Account = account;
            PendingOrders = orders;

            this.SentFlatTime = DateTime.Now;
            this.FlatCount = 0;

            this.SentFlatTime = DateTime.Now;
            this.SentCancelTime = DateTime.Now;
            this.FlatCount = 0;
            this.CancelCount = 0;

            this.Source = source;
            this.ForceCloseReason = closereason;
            this.SubTask = new List<RiskTaskSet>();
            this.FlatOrderIDList = new List<long>();
        }

        public override string ToString()
        {
            string msg = string.Empty;
            switch (this.TaskType)
            {
                case QSEnumRiskTaskType.FlatPosition:
                    msg = "Pos:" + this.Position.GetPositionKey() + " Status:" + this.TaskStatus.ToString();
                    break;
                case QSEnumRiskTaskType.CancelOrder:
                case QSEnumRiskTaskType.FlatAllPositions:
                    msg = "Flat All Status:" + this.TaskStatus.ToString();//"CancelSent:"+this.CancelSent.ToString() +" CancelDone:"+this.CancelDone.ToString()+" SubTaskGen:"+this.SubTaskGenerated.ToString();
                    break;
                default:
                    break;
            }
            return this.Account + " " + this.TaskType.ToString() + " SubTaskCnt:" + this.SubTask.Count.ToString() + " | " + msg;
        }

        public string Title
        {
            get
            {
                string msg = string.Empty;
                switch (this.TaskType)
                {
                    case QSEnumRiskTaskType.FlatPosition:
                        msg = string.Format("[{0}-{1}/{2}]", this.Account, this.TaskType.ToString(), this.Position.GetPositionKey());
                        break;
                    case QSEnumRiskTaskType.CancelOrder:
                        msg = string.Format("[{0}-{1}]", this.Account, this.TaskType.ToString());
                        break;
                    case QSEnumRiskTaskType.FlatAllPositions:
                        msg = string.Format("[{0}-{1}]", this.Account, this.TaskType.ToString());
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }
    }
}
