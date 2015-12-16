using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    internal delegate  void AsyncBrokerOperationDel(string token);

    public partial class BrokerRouterPassThrough
    {
        /// <summary>
        /// 清空某个交易帐户的交易记录
        /// </summary>
        /// <param name="account"></param>
        void ClearAccountTradingInfo(IAccount account)
        { 
            //1.将交易路由内的委托缓存中的属于该交易账户的记录删除
            Order tmp = null;
            //orders = localOrderID_map.Values.Where(o => o.Account == account.ID).ToArray();
            foreach (Order o in account.Orders)
            {
                localOrderID_map.TryRemove(o.BrokerLocalOrderID, out tmp);
                remoteOrderID_map.TryRemove(o.BrokerRemoteOrderID, out tmp);
            }

            //2.清空重置清算中心该帐户
            TLCtxHelper.ModuleClearCentre.ResetAccount(account);
        }


        void AsyncRestartBroker(string token)
        {
            AsyncBrokerOperationDel cb = new AsyncBrokerOperationDel(this.RestartBroker);
            cb.BeginInvoke(token, null, null);
        }

        /// <summary>
        /// 重启成交接口
        /// </summary>
        /// <param name="token"></param>
        void RestartBroker(string token)
        {

            IBroker b = TLCtxHelper.ServiceRouterManager.FindBroker(token);
            if (b == null)//未找到
            {
                throw new FutsRspError("通道不存在");
            }
            if (b.IsLive)//已经启动则停止
            {
                b.Stop();
            }
            string msg = string.Empty;
            bool s = b.Start(out msg);
            if (!s)
            {
                throw new FutsRspError(msg);
            }
        }

        void AsyncStopBroker(string token)
        {
            AsyncBrokerOperationDel cb = new AsyncBrokerOperationDel(this.StopBroker);
            cb.BeginInvoke(token, null, null);
        }

        void StopBroker(string token)
        {
            IBroker b = TLCtxHelper.ServiceRouterManager.FindBroker(token);
            if (b == null)//未找到
            {
                throw new FutsRspError("通道不存在");
            }
            if (b.IsLive)//已经启动则停止
            {
                b.Stop();
            }
        }

        void AsyncStartBroker(string token)
        {
            AsyncBrokerOperationDel cb = new AsyncBrokerOperationDel(this.StartBroker);
            cb.BeginInvoke(token, null, null);
        }

        void StartBroker(string token)
        {
            IBroker b = TLCtxHelper.ServiceRouterManager.FindBroker(token);
            if (b == null)//未找到
            {
                throw new FutsRspError("通道不存在");
            }
            if (!b.IsLive)//已经启动则停止
            {
                b.Start();
            }
        }

    }
}
