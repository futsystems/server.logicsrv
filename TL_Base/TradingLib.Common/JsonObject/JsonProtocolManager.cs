using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Mixins.JsonObject
{
    /// <summary>
    /// 银行
    /// </summary>
    public class JsonWrapperBank
    {
        /// <summary>
        /// 银行编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 代理 银行信息
    /// </summary>
    public class JsonWrapperBankAccount
    {
        /// <summary>
        /// 主域ID
        /// </summary>
        public int mgr_fk { get; set; }


        public int bank_id {get;set;}
        public JsonWrapperBank Bank { get; set; }

        

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 银行帐户号码
        /// </summary>
        public string Bank_AC { get; set; }

        /// <summary>
        /// 开户支行信息
        /// </summary>
        public string Branch { get; set; }


    }


    /// <summary>
    /// 代理财务信息 精简
    /// </summary>
    public class JsonWrapperAgentFinanceInfoLite
    {
        /// <summary>
        /// 主域编号
        /// </summary>
        public int BaseMGRFK { get; set; }

        /// <summary>
        /// 当前权益信息
        /// </summary>
        public JsonWrapperAgentBalance Balance { get; set; }

        /// <summary>
        /// 待处理提现金额
        /// </summary>
        public decimal PendingDeposit { get; set; }

        /// <summary>
        /// 待处理充值金额
        /// </summary>
        public decimal PendingWithDraw { get; set; }

        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal CashIn { get; set; }


        /// <summary>
        /// 提现记录
        /// </summary>
        public decimal CashOut { get; set; }


    }


    public class JsonWrapperAgentPaymentInfo
    {
        /// <summary>
        /// 主域编号
        /// </summary>
        public int BaseMGRFK { get; set; }

        /// <summary>
        /// 代理姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 代理手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 代理QQ号码
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 银行帐户信息
        /// </summary>
        public JsonWrapperBankAccount BankAccount { get; set; }
    }
    /// <summary>
    /// 代理财务信息 全面
    /// </summary>
    public class JsonWrapperAgentFinanceInfo
    {

        /// <summary>
        /// 主域编号
        /// </summary>
        public int BaseMGRFK { get; set; }

        /// <summary>
        /// 银行帐户信息
        /// </summary>
        public JsonWrapperBankAccount BankAccount { get; set; }

        /// <summary>
        /// 当前权益信息
        /// </summary>
        public JsonWrapperAgentBalance Balance { get; set; }

        /// <summary>
        /// 待处理提现金额
        /// </summary>
        public decimal PendingDeposit { get; set; }

        /// <summary>
        /// 待处理充值金额
        /// </summary>
        public decimal PendingWithDraw { get; set; }


        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal CashIn { get; set; }


        /// <summary>
        /// 提现记录
        /// </summary>
        public decimal CashOut { get; set; }

        /// <summary>
        /// Balance对应的最近结算信息
        /// </summary>
        public JsonWrapperAgentSettle LastSettle { get; set; }


        /// <summary>
        /// 近期出入金操作
        /// </summary>
        public JsonWrapperCashOperation[] LatestCashOperations { get; set; }
    }

    /// <summary>
    /// 代理帐户余额记录
    /// </summary>
    public class JsonWrapperAgentBalance
    {

        /// <summary>
        /// 主域编号
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 当前权益
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }
    }


    /// <summary>
    /// 代理结算记录
    /// </summary>
    public class JsonWrapperAgentSettle
    {
        /// <summary>
        /// 主域编号
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 直客收入
        /// </summary>
        public decimal Profit_Fee { get; set; }


        /// <summary>
        /// 代理佣金收入
        /// </summary>
        public decimal Profit_Commission { get; set; }


        /// <summary>
        /// 资金流入 入金
        /// </summary>
        public decimal CashIn { get; set; }


        /// <summary>
        /// 资金流出 出金
        /// </summary>
        public decimal CashOut { get; set; }


        /// <summary>
        /// 上日权益
        /// </summary>
        public decimal LastEquity { get; set; }


        /// <summary>
        /// 结算后权益
        /// </summary>
        public decimal NowEquity { get; set; }
    }

    /// <summary>
    /// 交易帐户对应的银行帐户信息
    /// </summary>
    public class JsonWrapperAccountBankAC : JsonObjectBase
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string Bank { get; set; }


        /// <summary>
        /// 银行帐户
        /// </summary>
        public string BankAC { get; set; }

        /// <summary>
        /// 开户分行
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// 代理信息
        /// </summary>
        public string AgentInfo { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }
    }
    /// <summary>
    /// 收款银行
    /// </summary>
    public class JsonWrapperReceivableAccount : JsonObjectBase
    {
        /// <summary>
        /// 收款银行ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        public string Bank_AC { get; set; }

        /// <summary>
        /// 分行地址
        /// </summary>
        public string Branch { get; set; }
    }


    /// <summary>
    /// 出入金操作
    /// </summary>
    public class JsonWrapperCashOperation : JsonObjectBase
    {
        /// <summary>
        /// 主域ID
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public long DateTime { get; set; }

        /// <summary>
        /// 资金操作
        /// </summary>
        public QSEnumCashOperation Operation { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public QSEnumCashInOutStatus Status { get; set; }


        /// <summary>
        /// 出入金操作来源
        /// </summary>
        public QSEnumCashOPSource Source { get; set; }


        /// <summary>
        /// 收款银行ID
        /// 0表示第三方支付
        /// </summary>
        public string  RecvInfo { get; set; }
    }

    public class JsonWrapperCasnTrans
    {
        public int ID { get; set; }
        /// <summary>
        /// 管理主域ID
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }


        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 金额 带有方向
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 对应流水
        /// </summary>
        public string TransRef { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Comment { get; set; }
    }


    /// <summary>
    /// 管理端界面权限
    /// 用于控制管理端的界面元素的现实
    /// </summary>
    public class UIAccess : JsonObjectBase
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 界面权限名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string desp { get; set; }
        /// <summary>
        /// 日志窗口显示
        /// </summary>
        [PermissionFieldAttr("日志窗口","用户查看后台通讯日志")]
        public bool fm_debug { get; set; }


        /// <summary>
        /// 头部导航 系统管理
        /// </summary>
        [PermissionFieldAttr("导航-系统设置","用于查看系统状态和设置")]
        public bool nav_system { get; set; }


        /// <summary>
        /// 系统管理-开启清算中心
        /// </summary>
        [PermissionFieldAttr("导航-开启清算中心", "用于打开清算中心，接受委托")]
        public bool nav_system_ccopen { get; set; }

        /// <summary>
        /// 系统管理-关闭清算中心
        /// </summary>
        [PermissionFieldAttr("导航-关闭清算中心", "用于关闭清算中心，拒绝所有委托")]
        public bool nav_system_ccclose { get; set; }


        /// <summary>
        /// 系统管理-路由列表
        /// </summary>
        [PermissionFieldAttr("导航-路由列表", "查看行情与数据通道，启动与关闭")]
        public bool nav_system_router { get; set; }

        /// <summary>
        /// 系统管理-内核状态
        /// </summary>
        [PermissionFieldAttr("导航-内核状态", "用于查看系统状态")]
        public bool nav_system_corestatus { get; set; }

        /// <summary>
        /// 基础数据
        /// </summary>
        [PermissionFieldAttr("导航-基础数据", "用于查看和设置基础数据")]
        public bool nav_basic { get; set; }

        /// <summary>
        /// 基础数据-交易时间段
        /// </summary>
        [PermissionFieldAttr("导航-交易时间", "查看市场交易时间段列表")]
        public bool nav_basic_mktime { get; set; }

        /// <summary>
        /// 基础数据-交易所
        /// </summary>
        [PermissionFieldAttr("导航-交易所", "查看支持的交易所列表")]
        public bool nav_basic_exchange { get; set; }

        /// <summary>
        /// 基础数据-品种管理
        /// </summary>
        [PermissionFieldAttr("导航-品种", "查看或设置品种信息")]
        public bool nav_basic_security { get; set; }

        /// <summary>
        /// 基础数据-合约管理
        /// </summary>
        [PermissionFieldAttr("导航-合约", "查看或设置合约信息")]
        public bool nav_basic_symbol { get; set; }

        /// <summary>
        /// 基础数据-权限模板
        /// </summary>
        [PermissionFieldAttr("导航-权限模板", "查看或设置权限模板")]
        public bool nav_basic_permissiontemplate { get; set; }
        /// <summary>
        /// 柜员管理
        /// </summary>
        [PermissionFieldAttr("导航-柜员管理", "管理下级柜员")]
        public bool nav_manager { get; set; }

        /// <summary>
        /// 柜员管理-柜员列表
        /// </summary>
        [PermissionFieldAttr("导航-柜员列表", "查看下级柜员列表")]
        public bool nav_manager_management { get; set; }

        /// <summary>
        /// 柜员管理-资费管理
        /// </summary>
        [PermissionFieldAttr("导航-柜员费率设置", "查看或设置下级柜员费率")]
        public bool nav_manager_feeconfig { get; set; }

        /// <summary>
        /// 柜员管理-权限设定
        /// </summary>
        [PermissionFieldAttr("导航-柜员权限设置", "查看或设置官员权限")]
        public bool nav_manager_permissionagent { get; set; }
        /// <summary>
        /// 财务管理
        /// </summary>
        [PermissionFieldAttr("导航-财务管理", "财务管理")]
        public bool nav_finance { get; set; }

        /// <summary>
        /// 财务管理-财务中心
        /// </summary>
        [PermissionFieldAttr("导航-财务中心", "管理代理的财务帐户,用于提交出入金")]
        public bool nav_finance_fincentre { get; set; }

        /// <summary>
        /// 财务管理-在线支付
        /// </summary>
        [PermissionFieldAttr("导航-在线出入金", "方位平台在线出入金平台")]
        public bool nav_finance_payonline { get; set; }

        /// <summary>
        /// 财务管理-出纳管理
        /// </summary>
        [PermissionFieldAttr("导航-出纳中心", "执行出入金请求操作")]
        public bool nav_finance_cashercentre { get; set; }

        /// <summary>
        /// 记录与报表
        /// </summary>
        [PermissionFieldAttr("导航-记录与报表", "记录和报表")]
        public bool nav_report { get; set; }

        /// <summary>
        /// 记录报表-记录查询
        /// </summary>
        [PermissionFieldAttr("导航-交易记录查询", "查询交易帐户的交易记录")]
        public bool nav_report_acchistinfo { get; set; }

        /// <summary>
        /// 记录报表-出入金查询
        /// </summary>
        [PermissionFieldAttr("导航-出入金查询", "查询交易帐户的出入金记录")]
        public bool nav_report_acccashtrans { get; set; }

        /// <summary>
        /// 记录报表-结算单
        /// </summary>
        [PermissionFieldAttr("导航-结算单查询", "查询交易帐户的结算单")]
        public bool nav_report_accsettlement { get; set; }


        /// <summary>
        /// 记录报表-代理出入金
        /// </summary>
        [PermissionFieldAttr("导航-代理出入金擦和讯", "查询代理出入金记录")]
        public bool nav_report_agentcashtrans { get; set; }


        /// <summary>
        /// 记录报表-代理结算
        /// </summary>
        [PermissionFieldAttr("导航-代理结算单查询", "查询代理结算单记录")]
        public bool nav_report_agentsettlement { get; set; }

        /// <summary>
        /// 代理分润
        /// </summary>
        [PermissionFieldAttr("导航-代理分润", "查询代理利润结算报表")]
        public bool nav_report_agentreport { get; set; }

        /// <summary>
        /// 帐户列表-路由类别
        /// </summary>
        [PermissionFieldAttr("监控-路由信息", "查看帐户路由信息")]
        public bool moniter_router { get; set; }


        /// <summary>
        /// 帐户列表-帐户类型
        /// </summary>
        [PermissionFieldAttr("监控-帐户类别", "查看帐户类别")]
        public bool moniter_acctype { get; set; }


        /// <summary>
        /// 帐户编辑-设置
        /// </summary>
        [PermissionFieldAttr("帐户编辑-帐户设置", "设置帐户属性")]
        public bool moniter_tab_config { get; set; }

        /// <summary>
        /// 帐户编辑-设置-帐户冻结
        /// </summary>
        [PermissionFieldAttr("帐户编辑-帐户设置-冻结", "帐户属性tab页中的冻结操作按钮")]
        public bool moniter_tab_config_inactive { get; set; }

        /// <summary>
        /// 帐户编辑-财务
        /// </summary>
        [PermissionFieldAttr("帐户编辑-财务操作", "查看财务信息和出入金操作")]
        public bool moniter_tab_finance { get; set; }

        /// <summary>
        /// 帐户列表-委托风控规则
        /// </summary>
        [PermissionFieldAttr("帐户编辑-委托风控", "设置委托风控规则")]
        public bool moniter_tab_orderrule { get; set; }

        /// <summary>
        /// 帐户列表-委托风控规则
        /// </summary>
        [PermissionFieldAttr("帐户编辑-帐户风控", "设置帐户风控规则")]
        public bool moniter_tab_accountrule { get; set; }

        /// <summary>
        /// 帐户列表-保证金手续费
        /// </summary>
        [PermissionFieldAttr("帐户编辑-保证金手续费", "设置帐户保证金和手续费")]
        public bool moniter_tab_margincommissoin { get; set; }

        /// <summary>
        /// 帐户列表-菜单编辑帐户
        /// </summary>
        [PermissionFieldAttr("监控菜单-编辑帐户", "编辑帐户")]
        public bool moniter_menu_editaccount { get; set; }

        /// <summary>
        /// 帐户列表-菜单修改交易密码
        /// </summary>
        [PermissionFieldAttr("监控菜单-修改密码", "修改密码")]
        public bool moniter_menu_changepass { get; set; }


        /// <summary>
        /// 帐户列表-菜单修改投资者信息
        /// </summary>
        [PermissionFieldAttr("监控菜单-修改投资者信息", "修改投资者信息")]
        public bool moniter_menu_changeinvestor { get; set; }

        /// <summary>
        /// 帐户列表-菜单查询交易记录
        /// </summary>
        [PermissionFieldAttr("监控菜单-查询历史记录", "查询历史记录")]
        public bool moniter_menu_queryhist { get; set; }


        /// <summary>
        /// 删除交易帐户
        /// </summary>
        [PermissionFieldAttr("监控菜单-删除交易帐户", "删除交易帐户")]
        public bool moniter_menu_delaccount { get; set; }


        /// <summary>
        /// 日内持仓明细查看模块中的平仓或反手操作
        /// </summary>
        [PermissionFieldAttr("交易记录监控-交易类操作", "后台平仓或反手")]
        public bool fun_info_operation { get; set; }
        /// <summary>
        /// 下单面板
        /// </summary>
        [PermissionFieldAttr("功能面板-下单面板", "管理端执行下单操作")]
        public bool fun_tab_placeorder { get; set; }

        /// <summary>
        /// 下单面板-插入成交
        /// </summary>
        [PermissionFieldAttr("功能面板-下单面板-插入成交", "管理端执行插入成交调试操作")]
        public bool fun_tab_placeorder_insert { get; set; }

        /// <summary>
        /// 配资模块面板
        /// </summary>
        [PermissionFieldAttr("功能面板-配资模块", "配资模块")]
        public bool fun_tab_finservice { get; set; }

        /// <summary>
        /// 财务信息面板
        /// </summary>
        [PermissionFieldAttr("功能面板-财务信息", "查询帐户财务状况")]
        public bool fun_tab_financeinfo { get; set; }


        /// <summary>
        /// 是否可以添加实盘帐户
        /// </summary>
        [PermissionFieldAttr("帐户类别-实盘", "是否可以添加实盘帐户")]
        public bool acctype_live { get; set; }

        /// <summary>
        /// 是否可以添加模拟帐户
        /// </summary>
        [PermissionFieldAttr("帐户类别-模拟", "是否可以添加模拟帐户")]
        public bool acctype_sim { get; set; }

        /// <summary>
        /// 是否可以添加交易员帐户
        /// </summary>
        [PermissionFieldAttr("帐户类别-交易员", "是否可以添加交易员帐户")]
        public bool acctype_dealer { get; set; }
    }
}
