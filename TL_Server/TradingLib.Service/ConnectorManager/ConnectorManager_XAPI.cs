using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;
using TradingLib.BrokerXAPI;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        /// <summary>
        /// 验证成交接口设置
        /// </summary>
        void ValidBrokerInterface()
        {
            debug("Valid BrokerInterface c++ DLL config....", QSEnumDebugLevel.MUST);
            foreach (BrokerInterface itface in ConnectorConfigTracker.BrokerInterfaces)
            {
                bool ret = BrokerXAPIHelper.ValidBrokerInterface(itface);
                //通过加载测试
                if (ret)
                    itface.IsValid = true;
                debug("Interface:" + itface.Name + " valid:" + itface.IsValid.ToString(), QSEnumDebugLevel.INFO);
            }
            foreach (BrokerConfig cfg in ConnectorConfigTracker.BrokerConfigs)
            {
                if (cfg.Interface == null)
                    continue;
                if (!cfg.Interface.IsValid)
                    continue;
                debug("Config Token:" + cfg.Token + " Name:" + cfg.Name + " SrvIP:" + cfg.srvinfo_ipaddress + " LoginID:" + cfg.usrinfo_userid, QSEnumDebugLevel.INFO);
            }
        }

        Dictionary<string, TLBroker> xapibrokermap = new Dictionary<string, TLBroker>();
        /// <summary>
        /// 加载BrokerXAPI底层成交接口
        /// </summary>
        void LoadBrokerXAPI()
        {
            foreach (BrokerConfig cfg in ConnectorConfigTracker.BrokerConfigs)
            {
                if (!cfg.Interface.IsValid)
                    continue;

                //创建Broker对象
                TLBroker broker = BrokerXAPIHelper.CreateBroker(cfg.Interface);
                //判断是否实现IBroker接口
                if (!(broker is IBroker))
                {
                    continue;
                }

                broker.SendLogItemEvent += new ILogItemDel(Util.Log);

                //设定服务端连接信息和用户登入信息
                broker.SetServerInfo(BrokerXAPIHelper.GenServerInfo(cfg));
                broker.SetUserInfo(BrokerXAPIHelper.GenUserInfo(cfg));
                broker.BrokerToken = cfg.Token;

                IBroker brokerinterface = broker as IBroker;
                //
                xapibrokermap.Add(cfg.Token, broker);
                brokerInstList.Add(cfg.Token, brokerinterface);

                //绑定状态事件
                broker.Connected += (IConnecter b) =>
                {
                    Util.Debug("Broker:" + broker.BrokerToken + " Connected");
                    if (BrokerConnectedEvent != null)
                        BrokerConnectedEvent(b);
                };
                broker.Disconnected += (IConnecter b) =>
                {
                    Util.Debug("Broker:" + broker.BrokerToken + " Disconnected");
                    if (BrokerDisconnectedEvent != null)
                        BrokerDisconnectedEvent(b);
                };
                //将broker的交易类事件绑定到路由内 然后通过路由转发到交易消息服务
                BindBrokerIntoRouter(broker);

                //broker.Start();
            }
        
        }
    }
}
