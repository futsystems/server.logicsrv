using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 跟单项操作
    /// 
    /// </summary>
    public class FollowAction
    {
        public FollowAction(FollowItem item)
        {
            this.FollowItem = item;
            this.ActionType = QSEnumFollowActionType.PlaceOrder;
            this.TargetOrders = new List<Order>();
        }

        /// <summary>
        /// 跟单项
        /// </summary>
        public FollowItem FollowItem { get; set; }

        /// <summary>
        /// 跟单操作类别
        /// 发送委托,取消委托,关闭跟单项等
        /// </summary>
        public QSEnumFollowActionType ActionType { get; set; }


        /// <summary>
        /// 目标委托
        /// </summary>
        public List<Order> TargetOrders { get; set; }

        public string ToString(bool simple=true)
        {
            if (simple)
            {
                return this.ActionType.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("ActionType:{0}", this.ActionType));
                foreach (var o in TargetOrders)
                {
                    sb.Append(o.GetOrderInfo());
                }
                return sb.ToString();
            }
        }
        public override string ToString()
        {
            return this.ToString(false);
        }
    }
}
