using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 跟从账户
    /// 分账户ExCore内部建立逻辑服务器接收前置保单
    /// 主账户监控ExCore内部负责交换BrokerRouter和其他组件信息
    /// 跟单账户 通过ExCore进行发单
    /// </summary>
    public class FollowAccount
    {
        
        /// <summary>
        /// 静态函数
        /// 创建FollowAccount
        /// 这里需要对account进行相关判断和检查
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static FollowAccount CreateFollowAccount(string account)
        {
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                return new FollowAccount(acct);
            }
            return null;
        }

        /// <summary>
        /// 获得委托回报
        /// </summary>
        public event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 获得成交回报
        /// </summary>
        public event FillDelegate GotFillEvent;

        IAccount _account = null;
        public FollowAccount(IAccount acct)
        {
            _account = acct;
            //_account.GotFillEvent += new FillDelegate(_account_GotFillEvent);
            //_account.GotOrderEvent += new OrderDelegate(_account_GotOrderEvent);
        }

        void _account_GotOrderEvent(Order order)
        {
            if (this.GotOrderEvent != null)
            {
                this.GotOrderEvent(order);
            }
        }

        void _account_GotFillEvent(Trade t)
        {
            if (this.GotFillEvent != null)
            {
                this.GotFillEvent(t);
            }
        }


        /// <summary>
        /// 跟单账户标识
        /// </summary>
        public string ID
        {
            get
            {
                return _account.ID;
            }
        }
        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            o.Account = this._account.ID;

            TLCtxHelper.ModuleExCore.SendOrderInternal(o);
        }

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="oid"></param>
        public void CancelOrder(long oid)
        {
            TLCtxHelper.ModuleExCore.CancelOrder(oid);
        }

        /// <summary>
        /// 底层账户
        /// </summary>
        public IAccount Account { get { return _account; } }
        /// <summary>
        /// 所有发送的委托
        /// </summary>
        public IEnumerable<Order> Orders { get { return _account.Orders; } }

        /// <summary>
        /// 所有成交列表
        /// </summary>
        public IEnumerable<Trade> Trades { get { return _account.Trades; } }

        /// <summary>
        /// 所有持仓
        /// </summary>
        public IEnumerable<Position> Positions { get { return _account.Positions; } }

        /// <summary>
        /// 所有多头持仓
        /// </summary>
        public IEnumerable<Position> PositonsLong { get { return _account.PositionsLong; } }

        /// <summary>
        /// 所有空头持仓
        /// </summary>
        public IEnumerable<Position> PositionsShort { get { return _account.PositionsShort; } }

        /// <summary>
        /// 通过某个委托ID获得跟单账户的委托
        /// 如果委托为空，或者不是该跟单账户发送的委托则返回null
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Order SentOrder(long oid)
        {
            Order o = TLCtxHelper.ModuleClearCentre.SentOrder(oid);
            if (o == null) return null;
            if (o.Account == _account.ID)
                return o;
            return null;
        }


    }
}
