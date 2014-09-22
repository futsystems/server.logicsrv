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
        public event FeeChargeItemDel GotFeeChargeItemEvent;
        Dictionary<string, FinServiceStub> finservicemap = new Dictionary<string, FinServiceStub>();

        public FinServiceTracker()
        {
            foreach (FinServiceStub stub in ORM.MFinService.SelectFinService())
            {
                //初始化FinService服务
                stub.InitFinService();
                if (!stub.IsValid)
                {
                    LibUtil.Debug("FinService:" + stub.ToString() + " is not valid, drop it");
                }
                finservicemap.Add(stub.Acct, stub);
            }
        }

        /// <summary>
        /// 通过交易帐号 获得FinServiceStub对象
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public FinServiceStub this[string account]
        {
            get
            {
                FinServiceStub stub = null;
                if(finservicemap.TryGetValue(account,out stub))
                {
                    return stub;
                }
                return null;
            }
        }

        public FinServiceStub[] ToArray()
        {
            return finservicemap.Values.ToArray();
        }

        public void GotFeeChargeItem(FeeChargeItem item)
        {
            if (GotFeeChargeItemEvent != null)
                GotFeeChargeItemEvent(item);
        }
    }


    /// <summary>
    /// 配资服务
    /// 收费部分
    /// 收费的实质就是记录一共收了多少钱，给代理是多少成本，代理获得的利润是多少
    /// </summary>
    public class FinServiceStub : IAccountService
    {

        /// <summary>
        /// 数据库ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 交易帐户ID
        /// </summary>
        public string Acct { get; set; }

        /// <summary>
        /// 返回代理ID
        /// </summary>
        public int AgentID { get { return Account != null ? Account.Mgr_fk : 0; } }

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

        public override string ToString()
        {
            return "ID:" + this.ID.ToString() + " Account:" + this.Account + " serviceplan_fk:" + serviceplan_fk.ToString() + " active:" + this.Active.ToString() + " ModifiedTime:" + this.ModifiedTime.ToString();
        }
        /// <summary>
        /// 初始化配资服务
        /// </summary>
        public void InitFinService()
        {
            LibUtil.Debug("初始化配资服务项,account:" + this.Acct);
            //1.绑定交易帐号
            this.Account = TLCtxHelper.CmdAccount[this.Acct];

            bool ret = TLCtxHelper.CmdAccount.HaveAccount(this.Acct);

            //1.调用服务生成工厂 生成对应的 IFinService
            _finservice = ServiceFactory.GenFinService(this);
            
            ServicePlanBase baseobj = _finservice as ServicePlanBase;
            if (baseobj != null)
            {
                baseobj.GotFeeChargeEvent += new FeeChargeDel(ChargeFee);
                baseobj.BindAccount(this.Account);
                this.Account.BindService(this);//将服务绑定到帐户
            }

            //this.FinService = _finservice;

            
        }

        /// <summary>
        /// 配资服务是否有效
        /// 有效必须要有Account对象 IFinService对象
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get
            {
                return (this.FinService != null) && (this.Account != null);
            }
        }

        #region 计费事件响应 将具体的事件通过FinService进行响应

        /// <summary>
        /// 响应清算中心的手续费调整事件
        /// 用于调整某个成交的手续费
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pr"></param>
        public decimal OnAdjustCommission(Trade t, IPositionRound pr)
        {
            return this.FinService.OnAdjustCommission(t, pr);
        }
        /// <summary>
        /// 响应成交事件
        /// 比如调整手续费等
        /// </summary>
        /// <param name="t"></param>
        public  void OnTrade(Trade t)
        {
            this.FinService.OnTrade(t);
        }

        /// <summary>
        /// 响应成交回合
        /// 比如按照成交回合进行收费
        /// 亏损不收钱 盈利收钱
        /// </summary>
        /// <param name="round"></param>
        public  void OnRound(IPositionRound round)
        {
            this.FinService.OnRound(round);
        }

        /// <summary>
        /// 响应每日收盘事件
        /// 当每天收盘后触发
        /// 用于按天或周期性收费
        /// 或者按当日盈利综合收费等
        /// </summary>
        public  void OnSettle()
        {
            this.FinService.OnSettle();
        }
        #endregion



        /// <summary>
        /// 进行系统收费
        /// 将收费记录记录到数据库
        /// 累计收费多少，代理收费多少
        /// </summary>
        /// <param name="totalfee"></param>
        /// <param name="agentfee"></param>
        void ChargeFee(decimal totalfee, decimal agentfee,string comment)
        {
            FeeChargeItem item = new FeeChargeItem();
            item.Account = this.Acct;
            item.TotalFee = totalfee;
            item.AgentFee = agentfee;
            item.AgetProfit = item.TotalFee - item.AgentFee;
            item.ChargeType = this.FinService.ChargeType;
            item.CollectType = this.FinService.CollectType;
            item.serviceplan_fk = this.serviceplan_fk;
            item.Agent_fk = this.AgentID;
            item.Comment = comment;
            FinTracker.FinServiceTracker.GotFeeChargeItem(item);
        }


        #region AccountService接口
        /// <summary>
        /// 返回AccountService的唯一标识
        /// </summary>
        public string SN { get { return "FinService"; } }

        /// <summary>
        /// 该服务所绑定的Account
        /// </summary>
        public IAccount Account { get; set; }


        /// <summary>
        /// 是否可以交易某个合约
        /// 限定合约部分
        /// 比如秘籍级别与衍生证券登记的关系
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            msg = string.Empty;
            return _finservice.CanTradeSymbol(symbol, out msg);
        }


        /// <summary>
        /// 检查是否可以接受委托
        /// 这样就可以绕过保证金检查,比如实现1000元开一手股指
        /// 保证金计算部分
        /// 异化的保证金计算
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool CanTakeOrder(Order o, out string msg)
        {
            msg = string.Empty;
            return _finservice.CanTakeOrder(o, out msg);
        }


        



        /// <summary>
        /// 返回帐户可某个合约的手数
        /// 逻辑中包含一些特殊的保证金处理
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int CanOpenSize(Symbol symbol)
        {
            return _finservice.CanOpenSize(symbol);
        }


        /// <summary>
        /// 获得某个合约的可用资金
        /// 1万配资10完的配资服务 需要返回不同于帐户资金的资金
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetFundAvabile(Symbol symbol)
        {
            return _finservice.GetFundAvabile(symbol);
        }

        /// <summary>
        /// 当前服务是否可用
        /// 检查服务是否处于激活状态
        /// 检查服务是否valid,有oaccount对象以及finservice对象
        /// </summary>
        public bool IsAvabile 
        {
            get
            {
                return this.IsValid && this.Active;
            }
        
        }

        #endregion

    }
}
