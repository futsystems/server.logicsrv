using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{

    public enum QSEnumResumeStatus
    { 
        BEGIN,//开始恢复数据
        END,//恢复数据结束
    }
    

    /// <summary>
    /// 系统服务标识
    /// 每个连接上来登入时候会提供服务标识，
    /// 用于标注某个连接是提供什么样的服务
    /// </summary>
    public enum QSEnumTLServiceType
    {
        [Description("模拟比赛服务")]
        Service_Race=0,
        [Description("配资服务")]
        Service_FinService=1,
    }

    //枚举了tradingsession的工作模式 设定当前工作模式是恢复模式还是交易模式
    //当QStrading登入的时候会自动获取当日的成交明细
    public enum QSEnumTradingSessinMode
    { 
        NOTLOGIN,//未登入
        RESUME,//恢复数据状态
        WORK,//工作状态
    
    }
    public enum TimeSalesType
    {
        Window,
        ChartRight,
    }
    /*
    public enum ExecutionType
    { 
        Sim,
        Real,
    }**/

    /// <summary>
    /// 规则集所使用的比较规则
    /// </summary>
    public enum QSEnumCompareType
    {
        [Description("大于")]
        Greater,
        [Description("大于等于")]
        GreaterEqual,
        [Description("小于")]
        Less,
        [Description("小于等于")]
        LessEqual,
        [Description("等于")]
        Equals,
        [Description("之内")]
        In,
        [Description("除外")]
        Out,
    }
    
    public enum PositionDataDriver
    {
        [Description("Tick")]
        Tick,
        [Description("1分钟")]
        min_1,
        [Description("3分钟")]
        min_3,
        [Description("5分钟")]
        min_5,
        [Description("15分钟")]
        min_15,
        [Description("30分钟")]
        min_30,
    }

    //指标分析中的交汇方式
    public enum EnumCross
    {
        [Description("上穿")]
        Above,
        [Description("下穿")]
        Below,
        [Description("无动作")]
        None
    }

 

 


    public enum BarDataType
    {
        [Description("时间")]
        Date,
        [Description("开盘价")]
        Open,
        [Description("最高价")]
        High,
        [Description("最低价")]
        Low,
        [Description("收盘价")]
        Close,
        [Description("成交量")]
        Volume,
       
        
    }

    //判断控件是用在服务端还是客户端用于生成不同的菜单和提供不同的功能
    public enum QSEnumGUISide
    { 
        Server,
        Client,
        
    }


    /// <summary>
    /// 清算中心状态
    /// </summary>
    public enum QSEnumClearCentreStatus
    {
        [Description("状态未知")]
        UNKNOWN,
        /// <summary>
        /// 清算中心初始化
        /// </summary>
        [Description("初始化模式")]
        CCINIT,
        [Description("初始化完毕")]
        CCINITFINISH,
        /// 清算中心从数据库恢复交易信息
        /// </summary>
        [Description("正在恢复")]
        CCRESTORE,
        [Description("恢复完毕")]
        CCRESTOREFINISH,
        /// <summary>
        /// 清算中心开启,可以进行交易处理
        /// </summary>
        [Description("开启模式")]
        CCOPEN,
        /// <summary>
        /// 清算中心关闭
        /// </summary>
        [Description("关闭模式")]
        CCCLOSE,
        /// <summary>
        /// 清算中心结算
        /// </summary>
        [Description("结算模式")]
        CCSETTLE,
        [Description("结算完毕")]
        CCSETTLEFINISH,
        [Description("数据检验")]
        CCDATACHECK,
        [Description("数据检验完毕")]
        CCDATACHECKFINISH,

        [Description("数据保存")]
        CCDATASAVE,
        [Description("数据保存完毕")]
        CCDATASAVEFINISH,

        [Description("重置模式")]
        CCRESET,
        [Description("重置完毕")]
        CCRESETFINISH,
    }
    
    
    //定义策略运行模式
    public enum QSEnumStrategyMode
    {
        [Description("回测")]
        HIST,
        [Description("实盘")]
        LIVE,
    }


    /// <summary>
    /// 现金操作 入金 出金
    /// </summary>
    public enum QSEnumCashOperation
    {
        [Description("入金")]
        CashIn,
        [Description("出金")]
        CashOut,
    }
    /*
    public enum QSEnumTIF
    { 

        DAY,//日内有效

    }**/
   
    /// <summary>
    /// 标注账户在大赛过程中所处的状态,淘汰,未报名,初赛,复赛等...
    /// </summary>
    public enum QSEnumAccountRaceStatus
    {
        [Description("未报名")]
        NORACE=100,//未加入比赛(选手未申请加入比赛)
        [Description("正在初赛")]
        INPRERACE=101,//初赛(选手申请加入比赛后的第一轮比赛,符合条件则进入复赛,不符合条件则淘汰(选手可以申请加入下一届比赛,资金复位))

        [Description("正在复赛")]
        INSEMIRACE=200,//复赛(选手进入复赛后,符合条件则进入实盘,不符合条件则淘汰进入复赛)
        [Description("淘汰")]
        ELIMINATE,//淘汰
        [Description("实盘1级")]
        INREAL1,//晋级实盘25万
        [Description("实盘2级")]
        INREAL2,//晋级实盘50万
        [Description("实盘3级")]
        INREAL3,//晋级实盘100万
        [Description("实盘4级")]
        INREAL4,//...
        [Description("实盘5级")]
        INREAL5,
        [Description("顶级阶段")]
        TOP,//最终比赛状态达到realx,没有办法再次升级了

        [Description("淘汰")]
        DDELIMINATE = 300,//如果实盘选手未达到比赛条件，则系统会淘汰该实盘选手
        [Description("实盘稳定组审核")]
        INREALSTABLE_Check,//晋级实盘25万
        [Description("实盘稳定组")]
        INREALSTABLE,//晋级实盘25万
        [Description("实盘波动组审核")]
        INREALUndulate_Check,//晋级实盘50万
        [Description("实盘波动组")]
        INREALUndulate,//晋级实盘50万

    }
    /// <summary>
    /// 定义了比赛类型,初赛,复赛,实盘25,实盘50,实盘100,实盘200,实盘300
    /// </summary>
    public enum QSEnumRaceType
    {
        [Description("初赛")]
        PRERACE=100,//初赛
        [Description("复赛")]
        SEMIRACE,//复赛
        [Description("实盘1级")]
        REAL1,//实盘1
        [Description("实盘2级")]
        REAL2,//实盘2
        [Description("实盘3级")]
        REAL3,//实盘3
        [Description("实盘4级")]
        REAL4,//实盘4
        [Description("实盘5级")]
        REAL5,//实盘5
    }
    /// <summary>
    /// 比赛考核结果 保留,晋级,淘汰
    /// </summary>
    public enum QSEnumRaceCheckResult
    { 

        STAY,//任然处于比赛当中
        PROMOT,//晋级
        ELIMINATE,//淘汰
    }
    /*
     * 关于选拔机制的思考
     * 初赛,复赛,盈利达到一定收益率后晋级(加入比赛以后开始计算),则需要记录加入比赛日期
     * 每天结算完毕后,计算收益率然后更新选手的状态,淘汰还是晋级亏损10%淘汰,需要重新报名比赛
     * 进入实盘后累计盈利40%以后可以晋级,
     * 实盘亏算10%淘汰。不同级别的实盘淘汰进入初赛或者是复赛或者实盘
     * 
     * 晋级次序
     * INPREACE 盈利20%进入INSEMIRACE,亏损10%淘汰进入报名初赛
     * INSEMIRACE 盈利20%进入INREAL25,亏算10%淘汰进入报名初赛
     * INREAL25  盈利40%进入INREAL50,亏损10%淘汰进入报名初赛
     * INREAL50  盈利40%进入INREAL100,亏损10%淘汰进入报名初赛
     * INREAL100 盈利40%进入INREAL200,亏损10%淘汰进入复赛
     * INREAL200 盈利40%进入INREAL300,亏损10%淘汰进入INREAL50
     * 
     */

    /// <summary>
    /// 委托单转发类型,转发到实盘交易接口或者模拟交易接口
    /// </summary>
    public enum QSEnumOrderTransferType
    {
        [Description("实盘")]
        LIVE,
        [Description("模拟")]
        SIM,
    }

    /// <summary>
    /// Tick数据类别,用于区分不同的tick数据,减少tick数据的重复发送
    /// </summary>
    public enum QSEnumTickType
    {
        ALL,//包含所有数据
        BASE,//包含基本的trade ask bid 数据
        HOLC,//日内高开低收数据变化,开盘价固定,日内创新高 或者 新低的时候才有必要发送,收盘价就是当时的价格
    }

    /// <summary>
    /// 枚举了报表类型
    /// </summary>
    public enum QSEnumReportType
    { 
        DailySummary,//每日汇总
        TotalProfit,//总累计盈亏
    
    }
    /// <summary>
    /// 服务器模式 单机,分布
    /// </summary>
    public enum QSEnumServerMode
    { 
        StandAlone,//单机部署,TradingServer在单机模式下会同时启动数据服务和交易服务
        Distributed,//分布式部署,TradingServer在分布式下只运行交易服务,数据服务由单独的服务器提供
    }
    /// <summary>
    /// TLserver的服务模式,提供交易系统服务,提供管理信息服务 管理信息服务不用流控
    /// </summary>
    public enum QSEnumTLMode
    { 
        TradingSrv,
        MgrSrv,
    }

    /// <summary>
    /// 枚举日志输出级别
    /// </summary>
    public enum QSEnumDebugLevel
    { 
        VERB=5,//verbose输出
        DEBUG=4,//调试输出
        INFO=3,//消息
        WARNING=2,//警告
        ERROR=1,//错误
        MUST=0,//系统必须输出的信息

    }
    /// <summary>
    /// 枚举 ticket信息的发布源
    /// </summary>
    public enum QSEnumTicketSource
    {
        [Description("未知")]
        Unknown,
        [Description("系统")]
        System,//系统级
        [Description("交易账户")]
        Account,//交易客户端
        [Description("客户端")]
        Customer,//管理客户端
        [Description("交易服务")]
        TradingSrv,//交易服务
        [Description("清算中心")]
        ClearCentre,//清算中心
        [Description("风控中心")]
        RiskCentre,//风控中心
        [Description("比赛中心")]
        RaceCentre,//比赛中心
        [Description("任务中心")]
        TaskCentre,//任务中心
        [Description("管理服务")]
        ManagerSrv,//管理服务
    }


    public enum QSEnumGender
    {
        [Description("男")]
        M,//男
        [Description("女")]
        F,//女
    }

    /// <summary>
    /// 止损 止盈方式
    /// </summary>
    public enum StopOffsetType
    {
        [Description("固定点数")]
        POINTS,
        [Description("固定价格")]
        PRICE,
        [Description("百分比")]
        PERCENT,
    }

    /// <summary>
    /// 止损 止盈方式
    /// </summary>
    public enum ProfitOffsetType
    {
        [Description("固定点数")]
        POINTS,
        [Description("固定价格")]
        PRICE,
        [Description("百分比")]
        PERCENT,
        [Description("跟踪")]
        TRAILING,
    }

    /// <summary>
    /// 止盈与止损方式
    /// </summary>
    public enum QSEnumPositionOffsetType
    {
        [Description("固定价格")]
        PRICE,
        [Description("固定点数")]
        POINTS,
        [Description("百分比")]
        PERCENT,
        [Description("跟踪")]
        TRAILING,
    }

    public enum QSEnumPositionOffsetDirection
    {
        [Description("止盈参数")]
        PROFIT,
        [Description("止损参数")]
        LOSS,
    }

    //加载账户类型
    public enum QSEnumAccountLoadMode
    {
        [Description("模拟与实盘")]
        ALL,//加载所有账户
        [Description("模拟")]
        SIM,//模拟账户
        [Description("实盘")]
        REAL,//实盘账户
    }
    /// <summary>
    /// 管理员类别 赋予不同的管理权限
    /// </summary>
    public enum QSEnumCustomerType
    {
        [Description("管理员")]
        ADMIN,//管理员拥有最高权限
        [Description("风控员")]
        RISKADMIN,//风控员,可以设置风控设置,对客户的账户执行强平
        [Description("财务管理员")]
        ACCOUNTADMIN,//财务管理员,可以增加账户,出入金等操作
        [Description("查看员")]
        VIEWER,//财务管理员,可以增加账户,出入金等操作
    }

    /// <summary>
    /// 融资服务费率计划类别
    /// </summary>
    public enum QSEnumFinServiceType
    {
        [Description("日利息")]
        INTEREST,//日利息 费用按照融资额的日利息进行收取 
        [Description("手续费加成")]
        COMMISSION,//手续费加成 费用按照手续费的加成进行计算,N%的手续费加成
        [Description("盈利提成")]
        BONUS,//分红计划 费用按照盈利日分红计算,不盈利不收费,盈利了收取一定比例的分成
        [Description("夜盘特价")]
        SPECIAL_NIGHT,//夜盘特价,为了充分利用资金，可以将夜盘资金进行充分利用
        [Description("股指专户")]
        SPECIAL_IF,//股指专户
        [Description("日盘特价")]
        SPECIAL_DAY,//股指专户
        [Description("股指固定保证金")]
        SPECIAL_IF_FJ,//股指专户
    }
}
