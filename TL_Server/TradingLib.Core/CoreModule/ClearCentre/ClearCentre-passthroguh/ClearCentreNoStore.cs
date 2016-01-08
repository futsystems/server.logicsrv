//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;


//namespace TradingLib.Core
//{
//    [CoreAttr(ClearCentrePass.CoreName, "清算中心", "清算中心,用于维护交易帐号,交易记录,保证金核算,系统结算等功能")]
//    public class ClearCentrePass:ClearCentreBase,IModuleClearCentre
//    {
//        const string CoreName = "ClearCentre";
//        public string CoreId { get { return this.PROGRAME; } }

//        public ClearCentrePass()
//            : base(ClearCentrePass.CoreName)
//        { 
            
//        }

//        public IEnumerable<PositionRound> TotalRoundOpend
//        {
//            get
//            {
//                return null;
//            }
//        }


//        #region 清算中心 开启 关闭 以及状态更新
//        const string OpenTime = "8:55";
//        const string CloseTime = "15:15";
//        const string NightOpenTime = "20:55";
//        const string NightClosedTime = "23:59";
//        const string NightOpenTime2 = "00:01";
//        const string NightClosedTime2 = "02:30";

//        QSEnumClearCentreStatus oldstatus = QSEnumClearCentreStatus.UNKNOWN;
//        QSEnumClearCentreStatus _status = QSEnumClearCentreStatus.UNKNOWN;
//        /// <summary>
//        /// 清算中心状态
//        /// </summary>
//        public QSEnumClearCentreStatus Status
//        {
//            get { return _status; }
//            private set
//            {
//                oldstatus = Status;//先保存原先状态
//                _status = value;//再设定新的状态
//                //更新当前状态 
//                UpdateStatus();
//            }
//        }

//        internal override void onGotFill(Trade fill, PositionTransaction postrans)
//        {
            
//        }

//        /// <summary>
//        /// 自动更新清算中心的状态
//        /// 1.清算中心初始化 ×
//        /// 2.清算中心恢复交易数据 ×
//        /// 3.清算中心开启 ×
//        /// 4.清算中心关闭 ×
//        /// 5.清算中心数据检验 
//        /// 6.清算中心保存数据
//        /// 6.清算中心结算
//        /// 7.清算中心重置
//        /// </summary>
//        void UpdateStatus()
//        {

//            //状态由 恢复->恢复完成改变 则我们检查当前的时间,如果是清算中心接收委托事件 则设定对应状态
//            //交易日内 盘中 启动软件
//            if (oldstatus == QSEnumClearCentreStatus.CCRESTORE && Status == QSEnumClearCentreStatus.CCRESTOREFINISH)
//            {
//                bool day = Util.IsInPeriod(Convert.ToDateTime(OpenTime), Convert.ToDateTime(CloseTime));
//                bool nigth1 = Util.IsInPeriod(Convert.ToDateTime(NightOpenTime), Convert.ToDateTime(NightClosedTime));
//                bool night2 = Util.IsInPeriod(Convert.ToDateTime(NightOpenTime2), Convert.ToDateTime(NightClosedTime2));

//                if (day || nigth1 || night2)
//                    Status = QSEnumClearCentreStatus.CCOPEN;
//                else
//                    Status = QSEnumClearCentreStatus.CCCLOSE;
//            }
//        }

//        /// <summary>
//        /// 开启清算中心
//        /// </summary>
//        public void OpenClearCentre()
//        {
//            Status = QSEnumClearCentreStatus.CCOPEN;
//        }
//        /// <summary>
//        /// 关闭清算中心
//        /// </summary>
//        public void CloseClearCentre()
//        {
//            Status = QSEnumClearCentreStatus.CCCLOSE;
//        }
//        #endregion


//        public void Start()
//        {
//            Util.StartStatus(this.PROGRAME);
//            Status = QSEnumClearCentreStatus.CCOPEN;
//        }

//        public void Stop()
//        {
//            Util.StopStatus(this.PROGRAME);
//        }

//        public void Reset()
//        { 
            
//        }

//    }
//}
