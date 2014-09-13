//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.MySql;

//namespace TradingLib.Core
//{
//    public partial class ClearCentre
//    {

//        #region 清算中心 开启 关闭 以及假期
//        const string OpenTime = "8:55";
//        const string CloseTime = "15:15";
//        const string NightOpenTime = "20:55";
//        const string NightClosedTime = "23:59";
//        const string NightOpenTime2 = "00:01";
//        const string NightClosedTime2 = "02:30";

//        #region 清算中心状态
//        QSEnumClearCentreStatus oldstatus = QSEnumClearCentreStatus.UNKNOWN;
//        QSEnumClearCentreStatus _status = QSEnumClearCentreStatus.UNKNOWN;
//        /// <summary>
//        /// 清算中心状态
//        /// </summary>
//        public QSEnumClearCentreStatus Status
//        {
//            get { return _status; }
//            set
//            {
//                oldstatus = Status;//先保存原先状态
//                _status = value;//再设定新的状态
//                //更新当前状态 
//                UpdateStatus();

//                //对外触发清算中心状态改变事件
//                //NotifyClearCentreStatus();
//            }
//        }

//        /// <summary>
//        /// 清算中心状态变化事件对外通知当前清算中心状态
//        /// </summary>
//        //public event ClearCentreStatusDel SendClearCentreStatus;
//        //void NotifyClearCentreStatus()
//        //{
//        //    if (SendClearCentreStatus != null)
//        //        SendClearCentreStatus(_status);
//        //}

//        #endregion
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
//                bool day = LibUtil.IsInPeriod(Convert.ToDateTime(OpenTime), Convert.ToDateTime(CloseTime));
//                bool nigth1 = LibUtil.IsInPeriod(Convert.ToDateTime(NightOpenTime), Convert.ToDateTime(NightClosedTime));
//                bool night2 = LibUtil.IsInPeriod(Convert.ToDateTime(NightOpenTime2), Convert.ToDateTime(NightClosedTime2));

//                if (day || nigth1 || night2)
//                    Status = QSEnumClearCentreStatus.CCOPEN;
//                else
//                    Status = QSEnumClearCentreStatus.CCCLOSE;
//            }
//        }


//        /// <summary>
//        /// 用于检查当前日期 判断是否为交易日,如果是交易日设定 可交易 和当前交易日
//        /// 每天0:05分进行判断
//        /// 1.首次启动调用
//        /// 2.设定假日调用
//        /// 3.每日凌晨调用 
//        /// </summary>
//        //void DateCheck()
//        //{
//        //    IsTradeDay = true;
//        //    //周末为非交易日
//        //    DayOfWeek dw = CurrentDay.DayOfWeek;
//        //    if (dw == DayOfWeek.Saturday || dw == DayOfWeek.Sunday)
//        //    {
//        //        IsTradeDay = false;
//        //    }
//        //    //如果为假日 则为非交易日
//        //    if (_holiday != null && _holiday.IsInHoliday(CurrentDay))
//        //    {
//        //        IsTradeDay = false;
//        //    }
//        //    UpdateTradingDate();//对外触发交易日信息
//        //}
        
        
//        /// <summary>
//        /// 清算中心交易日变化事件
//        /// </summary>
//        //public event VoidDelegate SendTradingDateUpdate;
//        //void UpdateTradingDate()
//        //{
//        //    if (SendTradingDateUpdate != null)
//        //        SendTradingDateUpdate();
//        //}

//        ///// <summary>
//        ///// 上一个结算日
//        ///// </summary>
//        //public DateTime LastSettleDay { get; set; }

//        ///// <summary>
//        ///// 当前交易日
//        ///// </summary>
//        ////每天凌晨自动将日期赋值
//        //DateTime _currentday;
//        ////每个临晨进行日期切换,同时做日期检查,用于判断当天是否是交易日
//        //public DateTime CurrentDay { get { return _currentday; } set { _currentday = value; this.DateCheck(); } }//赋值后进行一次日期检查
//        ///// <summary>
//        ///// 重置清算中心日期
//        ///// </summary>
//        ///// <returns></returns>
//        //public void NewDay()
//        //{
//        //    DateTime n = DateTime.Now;
//        //    //如果上一天为交易日 则设定该日期为最后结算日
//        //    if (IsTradeDay)
//        //    {
//        //        LastSettleDay = CurrentDay;
//        //    }
//        //    //设定完毕后更新当前交易日
//        //    CurrentDay = new DateTime(n.Year, n.Month, n.Day);
//        //    //重新加载交易所信息 用于更新交易所的session(判断是否是开市时间)
//        //    //ReloadExchangeMap();
//        //}
//        //bool _istradeday = true;
//        ///// <summary>
//        ///// 当前日期是否可以交易 有自动的周末判断和人工假期设定判断
//        ///// </summary>
//        //public bool IsTradeDay { get { return _istradeday; } set { _istradeday = value; } }

//        //Holiday _holiday = null;
//        ///// <summary>
//        ///// 设定最近假日信息
//        ///// </summary>
//        //public Holiday LatestHoliday { set { _holiday = value; DateCheck(); } }
//        //public string GetHolidayInfo()
//        //{
//        //    if (_holiday == null)
//        //    {
//        //        return "无假日";

//        //    }
//        //    else
//        //        return _holiday.ToString();

//        //}



//        #endregion
//    }
//}
