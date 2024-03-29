﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Contrib.FinService
{


    public class ArgumentTracker
    {
        ArgumentAccountTracker _acctargtracker = new ArgumentAccountTracker();
        ArgumentAgentTracker _agtargttracker = new ArgumentAgentTracker();
        ArgumentBaseTracker _baseargtracker = new ArgumentBaseTracker();

        public Argument GetAgentArgument(int agent_fk, int serviceplan_fk, string argname)
        {
            Dictionary<string, Argument> argmap = GetAgentArgument(agent_fk, serviceplan_fk);
            return argmap[argname];
        }
        /// <summary>
        /// 获得某个finservicestub的代理参数
        /// </summary>
        /// <param name="agent_fk"></param>
        /// <param name="serviceplan_fk"></param>
        /// <returns></returns>
        public Dictionary<string, Argument> GetAgentArgument(int agent_fk, int serviceplan_fk)
        {
            //LibUtil.Debug("get agent argument,agent_fk:" + agent_fk.ToString() + " serviceplan_fk:" + serviceplan_fk.ToString());
            Dictionary<string, Argument> agtarg = _agtargttracker.GetAgentArgument(agent_fk, serviceplan_fk).ToDictionary(c=>c.Key,c=>c.Value as Argument);
            Dictionary<string, Argument> basearg = _baseargtracker.GetAgentArgument(serviceplan_fk).ToDictionary(c => c.Key, c => c.Value as Argument);

            Dictionary<string, Argument> ret = MergeDict(new Dictionary<string, Argument>[] { agtarg, basearg });
            //foreach (Argument arg in ret.Values)
            //{
            //    LibUtil.Debug("arg:" + arg.ToString());
            //}
            return ret;
        }


        /// <summary>
        /// 获得某个finservicestub的交易帐户参数
        /// </summary>
        /// <param name="stub"></param>
        /// <returns></returns>
        public Dictionary<string, Argument> GetAccountArgument(FinServiceStub stub)
        {
            Dictionary<string, Argument> acctarg = _acctargtracker.GetServiceArgument(stub.ID).ToDictionary(c => c.Key, c => c.Value as Argument);
            Dictionary<string, Argument> basearg = _baseargtracker.GetAccountArgument(stub.serviceplan_fk).ToDictionary(c => c.Key, c => c.Value as Argument);
            Dictionary<string, Argument> ret = MergeDict(new Dictionary<string, Argument>[] { acctarg, basearg });
            //foreach (Argument arg in ret.Values)
            //{
            //    LibUtil.Debug("arg:" + arg.ToString());
            //}
            return ret;
        }


        /// <summary>
        /// 合并Dict,制定后覆盖还是前覆盖
        /// 当列表中的字典有相同字段时,后覆盖则后面的相同字段会覆盖前面的字段
        /// 默认使用第一个值
        /// </summary>
        /// <param name="dictlist"></param>
        /// <returns></returns>
        static Dictionary<string, Argument> MergeDict(ICollection<Dictionary<string, Argument>> dictlist, bool userfirst = true)
        {
            Dictionary<string, Argument> tmp = new Dictionary<string, Argument>();
            IEnumerable<KeyValuePair<string, Argument>> tmpkvpair = new Dictionary<string, Argument>();

            foreach (Dictionary<string, Argument> dict in dictlist)
            {

                tmpkvpair = tmpkvpair.Concat(dict);
            }
            if (!userfirst)
            {
                tmp = tmpkvpair.GroupBy(s => s.Key).ToDictionary(s => s.Key, s => s.Last().Value);
            }
            else
            {
                tmp = tmpkvpair.GroupBy(s => s.Key).ToDictionary(s => s.Key, s => s.First().Value);
            }
            return tmp;
        }

        /// <summary>
        /// 同步服务计划参数属性到数据库
        /// </summary>
        /// <param name="serviceplan_fk"></param>
        /// <param name="attr"></param>
        public void UpdateArgumentBase(int serviceplan_fk, ArgumentAttribute attr)
        {
            _baseargtracker.UpdateArgumentBase(serviceplan_fk, attr);
        }


        /// <summary>
        /// 更新某个配资服务的参数
        /// </summary>
        /// <param name="service_fk"></param>
        /// <param name="args"></param>
        public void UpdateArgumentAccount(int service_fk, JsonWrapperArgument[] args)
        {
            _acctargtracker.UpdateArgument(service_fk, args);
        }

        /// <summary>
        /// 更新帐户某个参数
        /// </summary>
        /// <param name="service_fk"></param>
        /// <param name="arg"></param>
        public void UpdateArgumentAccount(int service_fk, Argument arg)
        {
            _acctargtracker.UpdateArgument(service_fk, arg);
        }

        /// <summary>
        /// 更新某个代理某个服务计划的参数
        /// </summary>
        /// <param name="agent_fk"></param>
        /// <param name="serviceplan_fk"></param>
        /// <param name="args"></param>
        public void UpdateArgumentAgent(int agent_fk,int serviceplan_fk,JsonWrapperArgument[] args)
        {
            _agtargttracker.UpdateArgument(agent_fk, serviceplan_fk, args);
        }
    }

    /// <summary>
    /// 交易帐户的参数维护其
    /// </summary>
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
        /// service_fk为某帐户添加的配资服务记录全局ID
        /// </summary>
        /// <param name="service_fk"></param>
        /// <returns></returns>
        public Dictionary<string, ArgumentAccount> GetServiceArgument(int service_fk)
        {
            if (serviceArgMap.Keys.Contains(service_fk))
            {
                return serviceArgMap[service_fk];
            }
            return new Dictionary<string,ArgumentAccount>();
        }


        /// <summary>
        /// 更新service_fk对应的配资参数
        /// </summary>
        /// <param name="service_fk"></param>
        /// <param name="args"></param>
        public void UpdateArgument(int service_fk,JsonWrapperArgument[] args)
        {
            if (!serviceArgMap.Keys.Contains(service_fk))
            {
                serviceArgMap.Add(service_fk, new Dictionary<string, ArgumentAccount>());
            }

            Dictionary<string, ArgumentAccount> argmap = serviceArgMap[service_fk];

            foreach (JsonWrapperArgument arg in args)
            {
                ArgumentAccount newarg = new ArgumentAccount
                {
                    Name = arg.ArgName,
                    Value = arg.ArgValue,
                    Type = (EnumArgumentType)Enum.Parse(typeof(EnumArgumentType),arg.ArgType),
                    service_fk = service_fk
                };
                if (argmap.Keys.Contains(arg.ArgName))
                {
                    //如果参数发生变化则进行数据库更新
                    ArgumentAccount target = argmap[arg.ArgName];
                    if (!target.Value.Equals(newarg.Value))
                    {
                        Util.Debug("arg changed ,update database arg:" + newarg.Name);
                        //更新数据库
                        ORM.MArgumentBase.UpdateArgumentAccount(newarg);
                        //更新内存
                        target.Value = newarg.Value;
                    }
                }
                else//如果内存中没有加载该帐户的参数则直接进行数据库更新
                {
                    Util.Debug("database have no arg:" + newarg.Name + " update directly");
                    //更新数据库
                    ORM.MArgumentBase.UpdateArgumentAccount(newarg);
                    //插入新的内存参数
                    argmap[arg.ArgName] = newarg;
                }
            }
        }

        public void UpdateArgument(int service_fk,Argument arg)
        {
            if (!serviceArgMap.Keys.Contains(service_fk))
            {
                serviceArgMap.Add(service_fk, new Dictionary<string, ArgumentAccount>());
            }

            Dictionary<string, ArgumentAccount> argmap = serviceArgMap[service_fk];

           
                ArgumentAccount newarg = new ArgumentAccount
                {
                    Name = arg.Name,
                    Value = arg.Value,
                    Type = arg.Type,
                    service_fk = service_fk
                };
                if (argmap.Keys.Contains(arg.Name))
                {
                    //如果参数发生变化则进行数据库更新
                    ArgumentAccount target = argmap[arg.Name];
                    if (!target.Value.Equals(newarg.Value))
                    {
                        Util.Debug("arg changed ,update database arg:" + newarg.Name);
                        //更新数据库
                        ORM.MArgumentBase.UpdateArgumentAccount(newarg);
                        //更新内存
                        target.Value = newarg.Value;
                    }
                }
                else//如果内存中没有加载该帐户的参数则直接进行数据库更新
                {
                    Util.Debug("database have no arg:" + newarg.Name + " update directly");
                    //更新数据库
                    ORM.MArgumentBase.UpdateArgumentAccount(newarg);
                    //插入新的内存参数
                    argmap[arg.Name] = newarg;
                }
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
                if (!agentArgMap[arg.agent_fk].Keys.Contains(arg.serviceplan_fk))
                {
                    agentArgMap[arg.agent_fk][arg.serviceplan_fk] = new Dictionary<string, ArgumentAgent>();
                }
                agentArgMap[arg.agent_fk][arg.serviceplan_fk][arg.Name] = arg;
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
            return new Dictionary<string,ArgumentAgent>();
        }

        /// <summary>
        /// 更新某个代理某个服务计划的参数
        /// </summary>
        /// <param name="agent_fk"></param>
        /// <param name="serviceplan_fk"></param>
        /// <param name="args"></param>
        public void UpdateArgument(int agent_fk, int serviceplan_fk, JsonWrapperArgument[] args)
        {
            //如果内存中没有该代理的数据集 建立数据集
            if (!agentArgMap.Keys.Contains(agent_fk))
            {
                agentArgMap.Add(agent_fk, new Dictionary<int, Dictionary<string, ArgumentAgent>>());
            }
            //获得代理的配资服务计划参数 代理有多个配资服务计划的参数
            Dictionary<int, Dictionary<string, ArgumentAgent>> agentspargmap = agentArgMap[agent_fk];

            if (!agentspargmap.Keys.Contains(serviceplan_fk))
            {
                agentspargmap.Add(serviceplan_fk, new Dictionary<string, ArgumentAgent>());
            }

            Dictionary<string, ArgumentAgent> argmap = agentspargmap[serviceplan_fk];

            foreach (JsonWrapperArgument arg in args)
            {
                ArgumentAgent newarg = new ArgumentAgent
                {
                    Name = arg.ArgName,
                    Value = arg.ArgValue,
                    Type = (EnumArgumentType)Enum.Parse(typeof(EnumArgumentType), arg.ArgType),
                    serviceplan_fk = serviceplan_fk,
                    agent_fk =agent_fk,
                };
                if (argmap.Keys.Contains(arg.ArgName))
                {
                    //如果参数发生变化则进行数据库更新
                    ArgumentAgent target = argmap[arg.ArgName];
                    if (!target.Value.Equals(newarg.Value))
                    {
                        Util.Debug("arg changed ,update database arg:" + newarg.Name);
                        //更新数据库
                        ORM.MArgumentBase.UpdateArgumentAgent(newarg);
                        //更新内存
                        target.Value = newarg.Value;
                    }
                }
                else//如果内存中没有加载该帐户的参数则直接进行数据库更新
                {
                    Util.Debug("database have no arg:" + newarg.Name + " update directly");
                    //更新数据库
                    ORM.MArgumentBase.UpdateArgumentAgent(newarg);
                    //插入新的内存参数
                    argmap[arg.ArgName] = newarg;
                }
            }
            
            
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
            if (!argmap.Keys.Contains(arg.serviceplan_fk))
            {
                argmap.Add(arg.serviceplan_fk, new Dictionary<string, ArgumentBase>());
            }

            argmap[arg.serviceplan_fk][arg.Name] = arg;
        }


        /// <summary>
        /// 从程序集加载参数属性并更新到数据库
        /// 将服务计划的默认数据更新到数据库
        /// </summary>
        /// <param name="serviceplane_fk"></param>
        /// <param name="attr"></param>
        public void UpdateArgumentBase(int serviceplane_fk, ArgumentAttribute attr)
        {
            if (!agentArgMap.Keys.Contains(serviceplane_fk))
            {
                agentArgMap.Add(serviceplane_fk, new Dictionary<string, ArgumentBase>());
            }
            if (!accountArgMap.Keys.Contains(serviceplane_fk))
            {
                accountArgMap.Add(serviceplane_fk, new Dictionary<string, ArgumentBase>());
            }

            string argname = attr.Name;
            //不存在argname 插入对应的基准参数
            if (!agentArgMap[serviceplane_fk].Keys.Contains(argname))
            {
                ArgumentBase arg = new ArgumentBase();
                arg.ArgClass = EnumArgumentClass.Agent;
                arg.Name = argname;
                arg.serviceplan_fk = serviceplane_fk;
                arg.Type = attr.ArgType;
                arg.Value = attr.AgentValue.Value;
                ORM.MArgumentBase.InsertArgumentBase(arg);
                agentArgMap[serviceplane_fk][argname] = arg;
            }
            if (!accountArgMap[serviceplane_fk].Keys.Contains(argname))
            {
                ArgumentBase arg = new ArgumentBase();
                arg.ArgClass = EnumArgumentClass.Account;
                arg.Name = argname;
                arg.serviceplan_fk = serviceplane_fk;
                arg.Type = attr.ArgType;
                arg.Value = attr.AccountValue.Value;
                ORM.MArgumentBase.InsertArgumentBase(arg);
                accountArgMap[serviceplane_fk][argname] = arg;
            }
        }

    }

}
