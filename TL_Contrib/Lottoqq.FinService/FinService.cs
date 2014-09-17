using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.FinService
{
    public class FinServiceTracker
    {
        Dictionary<string, FinServiceStub> finservicemap = new Dictionary<string, FinServiceStub>();

        public FinServiceTracker()
        { 
            
        }
    }
    /// <summary>
    /// 配资服务
    /// 收费部分
    /// 收费的实质就是记录一共收了多少钱，给代理是多少成本，代理获得的利润是多少
    /// </summary>
    public class FinServiceStub
    {

        /// <summary>
        /// 数据库ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 交易帐户ID
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 服务计划外键
        /// </summary>
        public int serviceplan_fk { get; set; }


        IFinService _finservice= null;
        /// <summary>
        /// 配资服务实例
        /// 用于调用响应和计算
        /// </summary>
        public IFinService FinService { get { return _finservice; }  }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public long ModifiedTime { get; set; }


        /// <summary>
        /// 初始化配资服务
        /// </summary>
        public void InitFinService()
        { 
            //1.调用服务生成工厂 生成对应的 IFinService
            _finservice = ServiceFactory.GenFinService(this);
            ServicePlanBase baseobj = _finservice as ServicePlanBase;
            if (baseobj != null)
            {
                baseobj.GotFeeChargeEvent += new FeeChargeDel(ChargeFee);
            }
        }

        #region 计费事件响应 将具体的事件通过FinService进行响应
        /// <summary>
        /// 响应成交事件
        /// 比如调整手续费等
        /// </summary>
        /// <param name="t"></param>
        public  void OnTrade(Trade t)
        {
            if (this.FinService.ChargeType == EnumFeeChargeType.BYTrade)
            {
                this.FinService.OnTrade(t);
            }
        }

        /// <summary>
        /// 响应成交回合
        /// 比如按照成交回合进行收费
        /// 亏损不收钱 盈利收钱
        /// </summary>
        /// <param name="round"></param>
        public  void OnRound(IPositionRound round)
        {
            if (this.FinService.ChargeType == EnumFeeChargeType.BYRound)
            {
                this.FinService.OnRound(round);
            }
        }

        /// <summary>
        /// 响应每日收盘事件
        /// 当每天收盘后触发
        /// 用于按天或周期性收费
        /// 或者按当日盈利综合收费等
        /// </summary>
        public  void OnSettle()
        {
            if (this.FinService.ChargeType == EnumFeeChargeType.BYTime)
            {
                this.FinService.OnSettle();
            }
        }
        #endregion



        /// <summary>
        /// 进行系统收费
        /// 将收费记录记录到数据库
        /// </summary>
        /// <param name="totalfee"></param>
        /// <param name="agentfee"></param>
        void ChargeFee(decimal totalfee, decimal agentfee)
        {
            FeeChargeItem item = new FeeChargeItem();
            item.Account = this.Account;
            item.TotalFee = totalfee;
            item.AgentFee = agentfee;
            item.AgetProfit = item.TotalFee - item.AgetProfit;
            item.ChargeType = this.FinService.ChargeType;
            item.CollectType = this.FinService.CollectType;
            item.serviceplan_fk = this.serviceplan_fk;

            
        }

    }
}
