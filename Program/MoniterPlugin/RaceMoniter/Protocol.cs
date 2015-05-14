using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.Protocol
{
    /// <summary>
    /// 所处比赛级别
    /// </summary>
    public enum QSEnumRaceType
    {
        [Description("初赛")]
        PRERACE = 0,
        [Description("复赛")]
        SEMIRACE = 1,
        [Description("实盘1级")]
        REAL1 = 2,
        [Description("实盘2级")]
        REAL2 = 3,
        [Description("实盘3级")]
        REAL3 = 4,
        [Description("实盘4级")]
        REAL4 = 5,
        [Description("实盘5级")]
        REAL5 = 6,
    }

    /// <summary>
    /// 比赛检查结果
    /// 晋级，淘汰，保留
    /// </summary>
    public enum QSEnumRaceCheckResult
    {
        STAY = 0,
        PROMOT = 1,
        ELIMINATE = 2,
    }

    /// <summary>
    /// 交易帐户的比赛状态
    /// </summary>
    public enum QSEnumAccountRaceStatus
    {
        [Description("未报名")]
        NORACE = 0,
        [Description("正在初赛")]
        INPRERACE = 1,
        [Description("正在复赛")]
        INSEMIRACE = 2,
        [Description("淘汰")]
        ELIMINATE = 3,
        [Description("实盘1级")]
        INREAL1 = 4,
        [Description("实盘2级")]
        INREAL2 = 5,
        [Description("实盘3级")]
        INREAL3 = 6,
        [Description("实盘4级")]
        INREAL4 = 7,
        [Description("实盘5级")]
        INREAL5 = 8,
        [Description("顶级阶段")]
        TOP = 9,
    }


    public class RaceSetting
    {

        /// <summary>
        /// 比赛session标识
        /// </summary>
        public string RaceID { get; set; }

        /// <summary>
        /// 比赛级别
        /// </summary>
        public QSEnumRaceType RaceType { get; set; }

        /// <summary>
        /// 参赛总人数(包含淘汰与晋级)
        /// </summary>
        public int EntryNum { get; set; }

        /// <summary>
        /// 淘汰人数
        /// </summary>
        public int EliminateNum { get; set; }

        /// <summary>
        /// 晋级人数
        /// </summary>
        public int PromotNum { get; set; }

        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 报名开始时间
        /// </summary>
        public long BeginSignTime { get; set; }

        /// <summary>
        /// 报名结束时间
        /// </summary>
        public long EndSignTime { get; set; }

    }

    public class RaceServiceSetting
    {
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Acct { get; set; }

        /// <summary>
        /// 比赛编号
        /// </summary>
        public string RaceID { get; set; }

        /// <summary>
        /// 参赛时间
        /// </summary>
        public long EntryTime { get; set; }

        /// <summary>
        /// 比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus RaceStatus { get; set; }

        /// <summary>
        /// 考核时间
        /// </summary>
        public long ExamineTime { get; set; }


        /// <summary>
        /// 考核权益
        /// 初赛或者复赛会进行盈利折算
        /// </summary>
        public decimal ExamineEquity { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsAvabile { get; set; }
    }

}
