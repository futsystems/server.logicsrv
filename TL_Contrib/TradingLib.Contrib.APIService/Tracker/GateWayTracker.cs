using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class GateWayTracker
    {
        Dictionary<int, GateWayConfig> configMap = new Dictionary<int, GateWayConfig>();
        
        //domain_id与网关对象映射
        Dictionary<int, GateWayBase> gatewayMap = new Dictionary<int, GateWayBase>();

        public GateWayTracker()
        {
            foreach (var config in ORM.MGateWayConfig.SelectGateWayConfig())
            {
                configMap.Add(config.ID, config);
                GateWayBase tmp = GateWayBase.CreateGateWay(config);
                if (tmp != null)
                {
                    gatewayMap.Add(config.Domain_ID, tmp);
                }
            }
        }
        /// <summary>
        /// 获得天付宝网关
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GateWayBase> GetTFBGateways()
        {
            return gatewayMap.Values.Where(gw => gw.GateWayType == QSEnumGateWayType.TFBPay);
        }

        public GateWayConfig GetGateWayConfig(int id)
        {
            GateWayConfig target = null;
            if (configMap.TryGetValue(id, out target))
            {
                return target;
            }
            return null;
        }
        public GateWayBase GetDomainGateway(int domain_id)
        {
            GateWayBase target = null;
            if (gatewayMap.TryGetValue(domain_id, out target))
            {
                return target;
            }
            return target;
        }

        public void UpdateGateWayConfig(GateWayConfig config)
        {
            GateWayConfig target = null;
            if (configMap.TryGetValue(config.ID, out target))
            {
                target.Avabile = config.Avabile;

                bool needCreate = false;
                needCreate = (needCreate || (target.GateWayType != config.GateWayType));
                target.GateWayType = config.GateWayType;
                needCreate = (needCreate || target.Config != config.Config);
                target.Config = config.Config;

                ORM.MGateWayConfig.UpdateGateWayConfig(target);
                if (needCreate)
                {
                    GateWayBase tmp = GateWayBase.CreateGateWay(target);
                    gatewayMap[config.Domain_ID] = tmp;
                }
            }
            else
            {
                target = new GateWayConfig();
                target.Domain_ID = config.Domain_ID;
                target.Avabile = config.Avabile;
                target.GateWayType = config.GateWayType;
                target.Config = config.Config;
                ORM.MGateWayConfig.InsertGateWayConfig(target);

                config.ID = target.ID;
                configMap.Add(target.ID, target);
                GateWayBase tmp = GateWayBase.CreateGateWay(target);
                gatewayMap[config.Domain_ID] = tmp;
            }
        }
    }
}
