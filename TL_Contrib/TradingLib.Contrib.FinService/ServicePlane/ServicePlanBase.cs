using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.FinService
{
    /// <summary>
    /// 代理间分润计算公式
    /// 用于提供代理分润公式 然后递归代理进行计算
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="agent"></param>
    /// <returns></returns>
    public delegate decimal AgentCommissionDel(Manager agent, Manager parent);

    /// <summary>
    /// 直客分润计算公式
    /// 用于提供直接客户与代理间的利润计算
    /// </summary>
    /// <param name="totalfee"></param>
    /// <param name="agentfee"></param>
    /// <param name="comment"></param>
    public delegate void FeeChargeDel(decimal totalfee, decimal agentfee, AgentCommissionDel func,string comment);
    /// <summary>
    /// 所有服务计划的父类
    /// 用于定义服务计划的框架
    /// </summary>
    public abstract class ServicePlanBase : IFinService
    {
        protected string SPNAME = "股指专项";
        public event FeeChargeDel GotFeeChargeEvent;

        public ServicePlanBase()
        {

        }

        IAccount _account = null;
        /// <summary>
        /// 交易帐号对象属性 初始化时候需要进行绑定 避免每次运算查找帐户对象
        /// </summary>
        public IAccount Account { get { return _account; } }
        public void BindAccount(IAccount acc)
        {
            _account = acc;
        }


        /// <summary>
        /// 对应的服务计划外键
        /// </summary>
        public int ServicePlanFK { get; set; }

        /// <summary>
        /// 服务ID
        /// </summary>
        public int ServiceID { get; set; }
        /// <summary>
        /// 按照即定的业务逻辑
        /// 当需要收费时触发收费事件
        /// 收费事件只需要提供累计收费多少，代理成本是多少
        /// 其余参数均有框架提供
        /// </summary>
        /// <param name="totalfee"></param>
        /// <param name="agentfee"></param>
        protected void FeeCharge(decimal totalfee, decimal agentfee, AgentCommissionDel func, string comment)
        {
            GotFeeChargeEvent(totalfee, agentfee, func, comment);
        }
        /// <summary>
        /// 费用计算方式
        /// </summary>
        protected EnumFeeChargeType _chargetype = EnumFeeChargeType.BYTrade;
        public EnumFeeChargeType ChargeType { get { return _chargetype; } }

        /// <summary>
        /// 费用收集方式
        /// </summary>
        protected EnumFeeCollectType _collecttype = EnumFeeCollectType.CollectInTrading;
        public EnumFeeCollectType CollectType { get { return _collecttype; } }

        /// <summary>
        /// 外部程序用于获得该服务计划的参数
        /// </summary>
        public Dictionary<string, Argument> AccountArgumentMap { get { return accountargmap; } }

        /// <summary>
        /// 交易帐户参数列表
        /// </summary>
        Dictionary<string, Argument> accountargmap = new Dictionary<string, Argument>();

        /// <summary>
        /// 代理帐户参数列表
        /// </summary>
        Dictionary<string, Argument> agentargmap = new Dictionary<string, Argument>();



        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="args"></param>
        public virtual void InitArgument(Dictionary<string, Argument> accountarg, Dictionary<string, Argument> agentarg)
        {
            accountargmap = accountarg;
            agentargmap = agentarg;
            //调用反射自动进行参数赋值
            FinTracker.ServicePlaneTracker.SetArgument(this, accountarg, agentarg);

        }

        #region 事件响应 执行计费收集 这里可以进行进一步的扩展 针对不同的计费方式 监听不同的事件 进行计费
        /// <summary>
        /// 响应成交事件
        /// 比如调整手续费等
        /// </summary>
        /// <param name="t"></param>
        public virtual void OnTrade(Trade t)
        {

        }

        /// <summary>
        /// 响应成交回合
        /// 比如按照成交回合进行收费
        /// 亏损不收钱 盈利收钱
        /// </summary>
        /// <param name="round"></param>
        public virtual void OnRound(IPositionRound round)
        {


        }

        /// <summary>
        /// 响应每日收盘事件
        /// 当每天收盘后触发
        /// 用于按天或周期性收费
        /// 或者按当日盈利综合收费等
        /// </summary>
        public virtual void OnSettle()
        {

        }


        /// <summary>
        /// 响应成交手续费调整
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public virtual decimal OnAdjustCommission(Trade t, IPositionRound pr)
        {
            return t.Commission;
        }
        #endregion



        /// <summary>
        /// 调整收费项目
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="positionround"></param>
        /// <returns></returns>
        //public virtual decimal AdjustCommission(Trade fill, IPositionRound positionround)
        //{
        //    return fill.Commission;
        //}

        /// <summary>
        /// 获得可用配资额度，用于在标准资金上加入可用资金额度 实现配资逻辑
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public virtual decimal AdjustFinAmmount(decimal ammount)
        {
            return ammount;
        }


        #region 业务逻辑部分
        /*
         * 帐户业务逻辑和服务业务逻辑的分析
         * 帐户基础业务逻辑可以被实现的特例所覆写，达到修改逻辑的目的
         * 在帐户业务逻辑覆写过程中可以通过查询帐户相关服务并调用服务实现的业务逻辑从而实现绑定服务
         * 修改业务逻辑的功能
         * 
         * 服务的业务逻辑不存在与帐户间的覆写逻辑，只有当被调用时候才会应用服务的业务逻辑
         * 因此服务的业务逻辑默认是严格限制的。单确认要用到某个服务的相关业务逻辑接口时才会去实现
         * 
         * 
         * 
         * 
         * 
         * */
        /// <summary>
        /// 检查合约交易权限
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 保证金检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool CanTakeOrder(Order o, out string msg)
        {
            msg = string.Empty;
            return false;
        }

        /// <summary>
        /// 获得帐户某个合约的可用资金
        /// 在进行保证金检查时需要查询某个合约的可用资金
        /// 在业务逻辑覆写时 通过服务对应的结构对外暴露
        /// 然后在account主逻辑中进行调用
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual decimal GetFundAvabile(Symbol symbol)
        {
            return 0;
        }

        /// <summary>
        /// 计算通过配资服务后某个合约的可开手数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual int CanOpenSize(Symbol symbol)
        {
            return 0;
        }
        #endregion


        #region 风控规则

        public virtual void CheckAccount()
        {

        }

        DateTime _firetime = DateTime.Now;
        int _timediff = 5;
        /// <summary>
        /// 触发强平，这里启用了过滤
        /// 在4秒内 触发2次执行强平，防止第一次计算错误造成误差
        /// </summary>
        /// <param name="reason"></param>
        protected virtual void FireFlatPosition(string reason)
        {
            //如果5秒内触发过2此强平信号，则认为该计算是正确的 可以排除第一次计算出现当前权益为负数的情况[待查]
            if (DateTime.Now.Subtract(_firetime).TotalSeconds < _timediff)
            {
                Util.Debug("5秒内触发2次强平信号,执行强平");
                this.Account.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, "配资服务强平:" + reason);
                this.Account.InactiveAccount();
            }
            else
            {
                Util.Debug("距离上次触发时间超过5秒,重置触发时间");
                //如果离上一次触发大于5秒，则重置促发时间
                _firetime = DateTime.Now;
            }
                
        }
        #endregion


        /// <summary>
        /// 调整配资服务
        /// </summary>
        public virtual void AdjustOmCashOperation(JsonWrapperCashOperation op)
        {

        }
    }
}
