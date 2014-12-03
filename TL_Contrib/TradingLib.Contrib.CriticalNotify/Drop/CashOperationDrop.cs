using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using DotLiquid;
using TradingLib.Mixins.JsonObject;



namespace TradingLib.Contrib.NotifyCentre
{

    /// <summary>
    /// 出入金操作的Drop
    /// 不同的事件会有不同的Drop 用于输出我们需要的字段和属性
    /// 然后对应查询不同的通知对象类别获得通知信息列表 进行通知
    /// </summary>
    public class CashOperationEmailDrop : EmailDrop
    {
        JsonWrapperCashOperation _cashop;
        Manager manager = null;
        public CashOperationEmailDrop(JsonWrapperCashOperation op)
            : base("CashOperation")
        {
            _cashop = op;
            if (_cashop.mgr_fk > 0)
            {
                manager = BasicTracker.ManagerTracker[_cashop.mgr_fk];
                if (manager == null)
                    throw new ArgumentNullException();
            }
            
        }

        /// <summary>
        /// 编号
        /// </summary>
        public string Ref { get { return _cashop.Ref; } }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get { return _cashop.Amount; } }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get { return Util.GetEnumDescription(_cashop.Status); } }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get { return Util.GetEnumDescription(_cashop.Operation); } }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get { return _cashop.Account; } }

        /// <summary>
        /// 代理姓名
        /// </summary>
        public string Agent { 
            get 
            {
                return manager != null ? manager.Name : ""; 
            } 
        }

        /// <summary>
        /// 时间
        /// </summary>
        public string Datetime { get { return Util.ToDateTime(_cashop.DateTime).ToString("yy-MM-dd HH:mm:ss"); } }



        public override EnumNotifeeType[] GetNotifyTargets()
        {
            return new EnumNotifeeType[] { EnumNotifeeType.Account,EnumNotifeeType.Agent,EnumNotifeeType.Cashier,EnumNotifeeType.Accountant};
        }

        /// <summary>
        /// 获得通知列表
        /// 不同的通知模板有不同的订阅者
        /// 这里可以按照一定的方式来获得收件人
        /// </summary>
        /// <returns></returns>
        public override string[] GetNotifyList(EnumNotifeeType type)
        {
            switch (type)
            {
                case EnumNotifeeType.Account:
                    {
                        //获得交易帐户的邮件地址
                        AccountContact ct = ContactTracker.GetAccountContract(this.Account);
                        if (ct != null && ct.IsEmailValid)
                        {
                            return new string[] { ct.Email };
                        }
                        else
                        {
                            return null;
                        }
                    }
                case EnumNotifeeType.Agent:
                    {
                        //获得代理的邮件地址列表
                        if (manager == null)
                            return null;
                        else
                            return new string[] { manager.BaseManager.QQ + "@qq.com" };
                    }
                case EnumNotifeeType.Cashier://手工入金 提交时 通知出纳
                    {
                        //返回出纳所有有效的邮件地址 同时是新开的出入金请求
                        if (_cashop.Source != QSEnumCashOPSource.Online && _cashop.Status == QSEnumCashInOutStatus.PENDING)
                        {
                            return ContactTracker.GetNotifeeContact(type).Where(ct => ct.IsEmailValid).Select(ct2 => ct2.Email).ToArray();
                        }
                        else
                        {
                            return null;
                        }
                    }
                case EnumNotifeeType.Accountant://任何出入金 确认时 通知财务
                    {
                        Util.Debug("op status:" + _cashop.Status.ToString());
                        if (_cashop.Status == QSEnumCashInOutStatus.CONFIRMED)
                        {
                            return ContactTracker.GetNotifeeContact(type).Where(ct => ct.IsEmailValid).Select(ct2 => ct2.Email).ToArray();
                        }
                        else
                        {
                            return null;
                        }
                    }
                default:
                    return null;
            }
        }



    }
}
