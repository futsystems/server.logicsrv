using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.FinService
{

    public class ArgumentAccountTracker
    {
        /// <summary>
        /// 服务fk与参数列表的字典映射
        /// </summary>
        Dictionary<int, Dictionary<string, ArgumentAccount>> serviceArgMap = new Dictionary<int, Dictionary<string, ArgumentAccount>>();

        public ArgumentAccountTracker()
        {
            foreach (ArgumentAccount arg in ORM.MArgumentBase.SelectArgumentAccount())
            {
                if (!serviceArgMap.Keys.Contains(arg.service_fk))
                {
                    serviceArgMap[arg.service_fk] = new Dictionary<string, ArgumentAccount>();
                }

                serviceArgMap[arg.service_fk][arg.Name] = arg;
            }
        }

        /// <summary>
        /// 查找某个服务的参数字典
        /// </summary>
        /// <param name="service_fk"></param>
        /// <returns></returns>
        public Dictionary<string, ArgumentAccount> GetServiceArgument(int service_fk)
        {
            if (serviceArgMap.Keys.Contains(service_fk))
            {
                return serviceArgMap[service_fk];
            }
            return null;
        }
    }


    public class ArgumentAgentTracker
    {
        /// <summary>
        /// 代理参数字典
        /// 按代理编号进行一级映射，然后在通过serviceplane_fk进行二级映射 然后在单个serviceplane中再进行name-argument映射
        /// </summary>
        Dictionary<int, Dictionary<int, Dictionary<string, ArgumentAgent>>> agentArgMap = new Dictionary<int, Dictionary<int, Dictionary<string, ArgumentAgent>>>();


        public ArgumentAgentTracker()
        {
            foreach (ArgumentAgent arg in ORM.MArgumentBase.SelectArgumentAgent())
            {
                if (!agentArgMap.Keys.Contains(arg.agent_fk))
                {
                    agentArgMap[arg.agent_fk] = new Dictionary<int, Dictionary<string, ArgumentAgent>>();
                }
                if (!agentArgMap[arg.agent_fk].Keys.Contains(arg.serviceplane_fk))
                {
                    agentArgMap[arg.agent_fk][arg.serviceplane_fk] = new Dictionary<string, ArgumentAgent>();
                }
                agentArgMap[arg.agent_fk][arg.serviceplane_fk][arg.Name] = arg;
            }
        }


        /// <summary>
        /// 获得某个代理的参数字典
        /// 字典以 服务计划-参数字典 绑定映射关系
        /// </summary>
        /// <param name="agent_fk"></param>
        /// <returns></returns>
        public Dictionary<string, ArgumentAgent> GetAgentArgument(int agent_fk,int serviceplan_fk)
        {
            if (agentArgMap.Keys.Contains(agent_fk))
            {
                if (agentArgMap[agent_fk].Keys.Contains(serviceplan_fk))
                {
                    return agentArgMap[agent_fk][serviceplan_fk];
                }
            }
            return null;
        }
    }


    public class ArgumentBaseTracker
    {
        /// <summary>
        /// 维护代理商的基准参数
        /// serviceplane_fk 与 一组参数字典 进行映射 
        /// 通过serviceplane_fk可以获得一个参数字典 然后用于参数加载
        /// </summary>
        Dictionary<int, Dictionary<string, ArgumentBase>> agentArgMap = new Dictionary<int, Dictionary<string, ArgumentBase>>();


        /// <summary>
        /// 交易帐户基准参数
        /// 通过serviceplane_fk与一组参数字典 进行映射
        /// </summary>
        Dictionary<int, Dictionary<string, ArgumentBase>> accountArgMap = new Dictionary<int, Dictionary<string, ArgumentBase>>();


        public ArgumentBaseTracker()
        {
            //从数据库加载ArgumentBase并组织到内存结构
            foreach (ArgumentBase arg in ORM.MArgumentBase.SelectArgumentBase())
            {
                switch (arg.ArgClass)
                {
                    case EnumArgumentClass.Agent:
                        {
                            RangeArg(agentArgMap, arg);
                            break;
                        }
                    case EnumArgumentClass.Account:
                        {
                            RangeArg(accountArgMap, arg);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 返回代理上参数字典
        /// </summary>
        public Dictionary<int, Dictionary<string, ArgumentBase>> AgentArgument
        {
            get
            {
                return agentArgMap;
            }
        }

        /// <summary>
        /// 返帐户参数字典
        /// </summary>
        public Dictionary<int, Dictionary<string, ArgumentBase>> AccountArgument
        {
            get
            {
                return accountArgMap;
            }
        }

        /// <summary>
        /// 通过服务计划ID查找代理基准参数字典
        /// </summary>
        /// <param name="serviceplane_fk"></param>
        /// <returns></returns>
        public Dictionary<string, ArgumentBase> GetAgentArgument(int serviceplane_fk)
        {
            return agentArgMap[serviceplane_fk];
        }

        /// <summary>
        /// 通过服务计划ID查找帐户参数字典
        /// </summary>
        /// <param name="serviceplane_fk"></param>
        /// <returns></returns>
        public Dictionary<string, ArgumentBase> GetAccountArgument(int serviceplane_fk)
        {
            return accountArgMap[serviceplane_fk];
        }


        void RangeArg(Dictionary<int, Dictionary<string, ArgumentBase>> argmap, ArgumentBase arg)
        {
            //如果字典中没有serviceplane_fk 则添加 然后在serviceplane_fk对应的字段中加入name-arg对
            if (!argmap.Keys.Contains(arg.serviceplane_fk))
            {
                argmap.Add(arg.serviceplane_fk, new Dictionary<string, ArgumentBase>());
            }

            argmap[arg.serviceplane_fk][arg.Name] = arg;

        }

    }
    public class ArgumentTracker
    {

    }
}
