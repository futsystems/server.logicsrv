using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouter
    {
        /// <summary>
        /// 获得模拟成交Broker
        /// </summary>
        /// <returns></returns>
        IBroker GetSimBroker()
        {
            return TLCtxHelper.Ctx.RouterManager.DefaultSimBroker;
        }


        /// <summary>
        /// 查找委托对应的交易通道
        /// 如果委托对应的account实体不存在则返回null
        /// 如果帐户设置的是模拟路由则直接返回模拟成交接口
        /// 如果帐户设置的实盘目录，则查找帐户的路由组设置，如果路由组没有设置则返回null
        /// 如果设置了路由组则通过路由组来返回Broker，开仓与平仓返回Broker的方式不同，开仓是按照策略进行选择，平仓是在判定是否需要分拆后直接按Broker token进行选择
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public IBroker SelectBroker(Order o)
        {
            IAccount account = _clearCentre[o.Account];
            if (account == null) return null;
            //如果设定的路由类别为模拟 则返回模拟成交接口 如果是路由类别为实盘 则通过RouterGroup返回对应的路由
            if (account.OrderRouteType == QSEnumOrderTransferType.SIM)
            {
                return GetSimBroker();
            }//实盘路由通过RouterGroup返回
            else
            {
                RouterGroup rg = BasicTracker.RouterGroupTracker[account.RG_FK]; //这里需要做个鉴权 帐户设置的路由组的domain_id与帐户所属domain_id一致
                //没有设定路由组则返回null
                if (rg == null)
                {
                    debug(string.Format("account:{0} have not set router gorup fk:{1}", account.ID, account.RG_FK), QSEnumDebugLevel.WARNING);
                    return null;
                }
                //开仓委托按委托 通过RouterGroup的路由策略返回委托
                if (o.IsEntryPosition)
                {
                    return rg.GetBroker(o);
                }
                else//平仓委托按委托中的Broker字段返回
                {
                    return rg.GetBroker(o.Broker);
                }
            }
        }
    }
}
