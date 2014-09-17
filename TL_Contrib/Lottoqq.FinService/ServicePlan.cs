using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.FinService
{

    public delegate void FeeChargeDel(decimal totalfee,decimal agentfee);
    /// <summary>
    /// 所有服务计划的父类
    /// 用于定义服务计划的框架
    /// </summary>
    public abstract class ServicePlanBase:IFinService
    {

        public event FeeChargeDel GotFeeChargeEvent;

        /// <summary>
        /// 按照即定的业务逻辑
        /// 当需要收费时触发收费事件
        /// 收费事件只需要提供累计收费多少，代理成本是多少
        /// 其余参数均有框架提供
        /// </summary>
        /// <param name="totalfee"></param>
        /// <param name="agentfee"></param>
        protected void FeeCharge(decimal totalfee, decimal agentfee)
        {
            GotFeeChargeEvent(totalfee, agentfee);
        }
        /// <summary>
        /// 费用计算方式
        /// </summary>
        EnumFeeChargeType _chargetype = EnumFeeChargeType.BYTrade;
        public EnumFeeChargeType ChargeType { get { return _chargetype; } }

        /// <summary>
        /// 费用收集方式
        /// </summary>
        EnumFeeCollectType _collecttype = EnumFeeCollectType.CollectInTrading;
        public EnumFeeCollectType CollectType { get { return _collecttype; } }

        /// <summary>
        /// 交易帐户参数列表
        /// </summary>
        Dictionary<string, Argument> accountargmap = new Dictionary<string, Argument>();

        /// <summary>
        /// 代理帐户参数列表
        /// </summary>
        Dictionary<string, Argument> agentargmap = new Dictionary<string, Argument>();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="args"></param>
        public  ServicePlanBase()
        {


        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="args"></param>
        public virtual void InitArgument(Dictionary<string, Argument> accountarg, Dictionary<string, Argument> agentarg)
        {
            accountargmap = accountarg;
            agentargmap = agentarg;

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

        #endregion

        

        /// <summary>
        /// 调整收费项目
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="positionround"></param>
        /// <returns></returns>
        public virtual decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission;
        }

        /// <summary>
        /// 获得可用配资额度，用于在标准资金上加入可用资金额度 实现配资逻辑
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public virtual decimal AdjustFinAmmount(decimal ammount)
        {
            return ammount;
        }


    }

    /// <summary>
    /// 覆写相关函数 实现对应的逻辑
    /// </summary>
    public class SPSpecialIF : ServicePlanBase
    {

        ArgumentPair _wincharge = null;

        /// <summary>
        /// 盈利回合收费
        /// </summary>
        [ArgumentAttribute("WinCharge", EnumArgumentType.DECIMAL, 200, 100)]
        public ArgumentPair WinCharge { get; set; }

        /// <summary>
        /// 亏损回合收费
        /// </summary>
        [ArgumentAttribute("LossCharge", EnumArgumentType.DECIMAL, 100, 50)]
        public ArgumentPair LossCharge { get; set; }
        

        //public decimal AccountWinCharge { get { return _accountwincharge; } }
        //decimal _accountwincharge = 200;

        //[ArgumentAttribute("AcctLossCharge", EnumArgumentType.DECIMAL, 100, 50)]
        //public decimal AccountLossCharge { get { return _accountlosscharge; } }
        //decimal _accountlosscharge = 100;

        //[ArgumentAttribute("AgtWinCharge", EnumArgumentType.DECIMAL, 100, 50)]
        //public decimal AgentWinCharge { get { return _agentwincharge; } }
        //decimal _agentwincharge = 100;

        //[ArgumentAttribute("AgtLossCharge", EnumArgumentType.DECIMAL, 100, 50)]
        //public decimal AgentLossCharge { get { return _agentlosscharge; } }
        //decimal _agentlosscharge = 50;
        public override void InitArgument(Dictionary<string, Argument> accountarg, Dictionary<string, Argument> agentarg)
        {
            base.InitArgument(accountarg, agentarg);
            LibUtil.Debug("调用服务计划的参数初始化");
            //将参数加载到内存
            
        }


        /// <summary>
        /// 响应持仓回合事件
        /// 当一次开仓 平仓结束后触发该调用
        /// </summary>
        /// <param name="round"></param>
        public override void OnRound(IPositionRound round)
        {
            decimal totalfee = 0;
            decimal agentfee = 0;
            //盈利
            if (round.Profit>0)
            {
                totalfee = round.Size * this.WinCharge.AccountArgument.AsDecimal();
                agentfee = round.Size * this.WinCharge.AgentArgument.AsDecimal();
            }
            //亏损
            else
            {
                totalfee = round.Size * this.LossCharge.AccountArgument.AsDecimal();
                agentfee = round.Size * this.LossCharge.AgentArgument.AsDecimal(); ;
            }

            //进行收费记录
            FeeCharge(totalfee, agentfee);
        }
        
    }
}
