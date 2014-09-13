using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TradingLib.API;
using TradingLib.Common;
using System.Data;


namespace TradingLib.Contrib
{



    /* 融资以服务的形式进行
     * 账户按照自己的需要订购某个服务,
     * 账户通过融资服务的绑定来获得对应的数据
     * 服务同时描述了对应的费率计划,结算时按照服务的不同进行收费
     * 
     * */
    /// <summary>
    /// 融资服务
    /// 每个账户有自己的finserver设定,FinService是根据具体的设定来生成的
    /// 每个融资服务作为一个数据条目记录于数据库,服务加载的时候从数据库加载对应的服务生成服务实例，然后用于清算中心获得对应的
    /// 融资额度，开放开仓权限
    /// </summary>
    public class FinService:IFinService
    {
        public FinService(IAccount account,decimal ammount,IRatePlan rateplan,decimal discount,bool active,string agentcode)
        {
            _account = account;//对应的交易帐户
            _finammount = ammount;//配资额度
            _rateplan = rateplan;//收费计划
            _discount = discount;//折扣
            _active = active;//服务是否激活
            _agentcode = agentcode;//居间人代码
        }
        string _agentcode = "0";
        /// <summary>
        /// 配资服务代理编号
        /// </summary>
        public string AgentCode { get { return _agentcode; } set { _agentcode = value; } }

        IAccount _account = null;
        /// <summary>
        /// 该服务所绑定的账户
        /// </summary>
        public IAccount Account { get { return _account; } set { _account = value; } }

        decimal _finammount = 0;
        /// <summary>
        /// 融资额度
        /// </summary>
        public decimal FinAmmount { get { return _finammount; } set { _finammount = value; } }

        IRatePlan _rateplan = null;
        /// <summary>
        /// 费率计划
        /// 系统设定了若干费率计划,然后订购融资服务的时候可以选择对应的费率计划,当天的使用费用由费率计划计算得出
        /// 并且计算得到当日费用后，结算进入对应的记录表格
        /// </summary>
        public IRatePlan RatePlan { get { return _rateplan; } set { _rateplan = value; } }

        decimal _discount=1.0M;
        /// <summary>
        /// 费率折扣,在标准计费模式上进行的费率折扣,统一使用折扣模式来进行费率的让渡,标准费率不再做修改
        /// </summary>
        public decimal Discount { get { return _discount; } set { _discount = value; } }


        bool _active = false;
        /// <summary>
        /// 服务是否生效,当订购完服务后,汇款入金成功,则系统会至服务生效
        /// </summary>
        public bool Active { get { return _active; } set { _active = value; } }

        /// <summary>
        /// 获得服务当前可用金额,该金额与账户cash形成对应的购买力
        /// </summary>
        /// <returns></returns>
        public decimal GetAvabileAmmount()
        {
            if (!_active) return 0;//服务不可用则返回可用金额为0
            if (_rateplan == null) return _finammount;
            return _rateplan.AdjustFinAmmount(_finammount);
        }

        /// <summary>
        /// 调整手续费,每种计费模式可以在标准手续费基础上进行手续费调整
        /// </summary>
        /// <param name="size"></param>
        /// <param name="commission"></param>
        /// <returns></returns>
        public decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            if (_rateplan == null) return fill.Commission;
            return _rateplan.AdjustCommission(fill,positionround);
        }

        /// <summary>
        /// 计算收费,由标准收费×对应的折扣率来获得费用
        /// </summary>
        /// <returns></returns>
        public decimal GetRate()
        {
            if (!Active) return 0;//如果服务没有激活 则收费为0
            return this.RatePlan.CalRate(this.Account, this.FinAmmount)*_discount;
        }

        public override string ToString()
        {
            return "账户:"+_account.ID + " 额度:" + _finammount.ToString() + " 费率折扣:" + _discount.ToString() + " 计费:" + _rateplan.Type.ToString();
        }
    }

    


   

}
