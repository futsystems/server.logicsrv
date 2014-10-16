using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.FinService
{
    public class FinServiceTracker : IEnumerable,IEnumerable<FinServiceStub>
    {
        public event FeeChargeItemDel GotFeeChargeItemEvent;
        Dictionary<string, FinServiceStub> finservicemap = new Dictionary<string, FinServiceStub>();

        public FinServiceTracker()
        {
            foreach (FinServiceStub stub in ORM.MFinService.SelectFinService())
            {
                LoadStub(stub);
            }
        }

        /// <summary>
        /// 加载配资服务
        /// </summary>
        /// <param name="stub"></param>
        void LoadStub(FinServiceStub stub)
        {
            //初始化FinService服务
            stub.InitFinService();
            if (!stub.IsValid)
            {
                Util.Debug("FinService:" + stub.ToString() + " is not valid, drop it");
                return;
            }
            if (finservicemap.Keys.Contains(stub.Acct))
            {
                Util.Debug("account:"+stub.Acct+"already load finservice,drop newer");
                return;
            }
            finservicemap.Add(stub.Acct, stub);
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

        /// <summary>
        /// 判断某个帐户是否有配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool HaveFinService(string account)
        {
            if (finservicemap.Keys.Contains(account))
                return true;
            return false;
        }

        /// <summary>
        /// 为某个交易帐号添加 某个服务计划 sp_fk
        /// </summary>
        /// <param name="account"></param>
        /// <param name="sp_fk"></param>
        public void AddFinService(string account,int sp_fk)
        {
            FinServiceStub stub = new FinServiceStub();
            //配资服务帐号
            stub.Acct = account;
            //是否激活
            stub.Active = true;
            //修改时间
            stub.ModifiedTime = Util.ToTLDateTime();
            //服务计划fk
            stub.serviceplan_fk = sp_fk;

            //插入到数据库
            ORM.MFinService.InsertFinService(stub);

            //加载FinServiceStub
            LoadStub(stub);           
        }

        /// <summary>
        /// 删除某个配资服务
        /// </summary>
        /// <param name="service_id"></param>
        public void DeleteFinService(FinServiceStub stub)
        {
            //将服务从交易帐户上注销
            stub.Account.UnBindService(stub);

            //内存清空相关记录
            if (finservicemap.Keys.Contains(stub.Acct))
            {
                finservicemap.Remove(stub.Acct);
            }

            //数据库删除记录
            ORM.MFinService.DeleteFinService(stub);
        }


        public FinServiceStub[] ToArray()
        {
            return finservicemap.Values.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        IEnumerator<FinServiceStub> IEnumerable<FinServiceStub>.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<FinServiceStub> GetEnumerator()
        {
            foreach (FinServiceStub stub in finservicemap.Values.ToArray())
                yield return stub;
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
        /// 加载配资服务参数
        /// </summary>
        public void LoadArgument()
        {
            
            //获得对应的参数参数
            Dictionary<string, Argument> agentarg = FinTracker.ArgumentTracker.GetAgentArgument(this.AgentID, this.serviceplan_fk);
            Dictionary<string, Argument> accountarg = FinTracker.ArgumentTracker.GetAccountArgument(this);

            ServicePlanBase baseobj = _finservice as ServicePlanBase;
            baseobj.InitArgument(accountarg, agentarg);
        }
        /// <summary>
        /// 初始化配资服务
        /// </summary>
        public void InitFinService()
        {
            //LibUtil.Debug("初始化配资服务项,account:" + this.Acct);
            //1.预检查
            this.Account = TLCtxHelper.CmdAccount[this.Acct];//如果没有对应的交易帐号 则直接返回
            if (this.Account == null) return;
            Type type = FinTracker.ServicePlaneTracker.GetFinServiceType(this.serviceplan_fk);
            if (type == null) return;//如果没有获得对应的类型 则直接返回

            //2.生成对应的IFinService
            _finservice  = (IFinService)Activator.CreateInstance(type);
            
            //3.绑定收费事件,绑定交易帐号,同时将服务绑定到对应的交易帐号对象上
            ServicePlanBase baseobj = _finservice as ServicePlanBase;
            if (baseobj != null)
            {
                //加载配资服务参数
                this.LoadArgument();
                baseobj.ServicePlanFK = this.serviceplan_fk;
                //绑定输出计费事件
                baseobj.GotFeeChargeEvent += new FeeChargeDel(ChargeFee);
                //绑定交易帐号
                baseobj.BindAccount(this.Account);
                //将服务绑定到帐户
                this.Account.BindService(this);
            }
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
        void ChargeFee(decimal totalfee, decimal agentfee,AgentCommissionDel func, string comment)
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
            item.Settleday = TLCtxHelper.CmdSettleCentre.CurrentTradingday;
            

            //定义了代理之间的分润计算
            if (func != null)
            {
                Manager agent = BasicTracker.ManagerTracker[item.Agent_fk];//客户的直接代理

                //递归代理的父代理 计算对应的分润
                while (agent.ParentManager.Type != QSEnumManagerType.ROOT)
                {
                    decimal commission = func(agent, agent.ParentManager);
                    Util.Debug("代理:" + agent.mgr_fk.ToString() + " 的父代理:" + agent.ParentManager.mgr_fk.ToString() + " 收入:" + commission.ToString());
                    //插入代理分润数据
                    CommissionItem commissionitem = new CommissionItem()
                    {
                        Settleday = item.Settleday,
                        Agent_FK = agent.parent_fk,
                        SubAgent_FK = agent.mgr_fk,
                        Commission = commission,
                    };
                    item.AppendCommissionItem(commissionitem);
                    agent = agent.ParentManager;
                }
            }

            FinTracker.FinServiceTracker.GotFeeChargeItem(item);
        }



        /// <summary>
        /// 执行帐户分控检查
        /// </summary>
        public  void CheckAccount()
        {
            _finservice.CheckAccount();
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


        #region 服务查询和设置
        /// <summary>
        /// 查询服务状态和参数
        /// </summary>
        /// <returns></returns>
        public string QryService()
        {
            return _finservice.ToString();   
        }

        /// <summary>
        /// 设置服务状态和参数
        /// </summary>
        /// <param name="cfg"></param>
        public void SetService(string cfg)
        {

        }

        #endregion

    }
}
