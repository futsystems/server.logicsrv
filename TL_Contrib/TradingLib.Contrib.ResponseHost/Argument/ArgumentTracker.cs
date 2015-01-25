using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.ResponseHost
{
    /// <summary>
    /// 策略参数维护器
    /// 用于获取策略参数或更新策略参数
    /// </summary>
    public class ArgumentTracker
    {
        ArgumentBaseTracker basetracker = new ArgumentBaseTracker();
        ArgumentInstanceTracker instancetracker = new ArgumentInstanceTracker();

        public Dictionary<string, Argument> GetInstanceArgument(int template_id,int instance_id)
        {
            Dictionary<string, Argument> basearg = basetracker.GetArgument(template_id).ToDictionary(c => c.Key, c => c.Value as Argument);
            Dictionary<string, Argument> instancearg = instancetracker.GetArgumentInstance(instance_id).ToDictionary(c => c.Key, c => c.Value as Argument);

            Dictionary<string, Argument> ret = MergeDict(new Dictionary<string, Argument>[] { instancearg, basearg });
            //foreach (Argument arg in ret.Values)
            //{
            //    LibUtil.Debug("arg:" + arg.ToString());
            //}
            return ret;
        }

        /// <summary>
        /// 合并Dict,指定后覆盖还是前覆盖
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
        /// 更新策略模板默认参数
        /// </summary>
        /// <param name="template_id"></param>
        /// <param name="attr"></param>
        public void UpdateArgumentBase(int template_id, ArgumentAttribute attr)
        {
            basetracker.UpdateArgumentBase(template_id, attr);
        }

        public void UpdateArgumentInstance(int instance, Argument arg)
        { 
        
        }


    }

    public class ArgumentInstanceTracker
    {
        Dictionary<int, Dictionary<string, ArgumentInstance>> argumentInstanceMap = new Dictionary<int, Dictionary<string, ArgumentInstance>>();
        public ArgumentInstanceTracker()
        {
            foreach (ArgumentInstance arg in ORM.MArgument.SelectArgumentInstance())
            {
                if (!argumentInstanceMap.Keys.Contains(arg.Instance_ID))
                {
                    argumentInstanceMap.Add(arg.Instance_ID, new Dictionary<string, ArgumentInstance>());
                }
                argumentInstanceMap[arg.Instance_ID][arg.Name] = arg;
            }
        }

        /// <summary>
        /// 获得策略实例参数
        /// </summary>
        /// <param name="instance_id"></param>
        /// <returns></returns>
        public Dictionary<string, ArgumentInstance> GetArgumentInstance(int instance_id)
        {
            if (argumentInstanceMap.Keys.Contains(instance_id))
            {
                return argumentInstanceMap[instance_id];
            }
            return new Dictionary<string, ArgumentInstance>();
        }

        /// <summary>
        /// 更新策略实例参数
        /// </summary>
        public void UpdateArgument()
        { 
        
        }
    }
    /// <summary>
    /// 策略模板基础参数维护器
    /// </summary>
    public class ArgumentBaseTracker
    {
        Dictionary<int, Dictionary<string, ArgumentBase>> argumentBaseMap = new Dictionary<int, Dictionary<string, ArgumentBase>>();

        public ArgumentBaseTracker()
        {
            foreach (ArgumentBase arg in ORM.MArgument.SelectArgumentBase())
            {
                if (!argumentBaseMap.Keys.Contains(arg.Template_ID))
                {
                    argumentBaseMap.Add(arg.Template_ID, new Dictionary<string, ArgumentBase>());
                }
                argumentBaseMap[arg.Template_ID][arg.Name] = arg;
            }
        }

        public Dictionary<int, Dictionary<string, ArgumentBase>> ArgumentBase
        {
            get
            {
                return argumentBaseMap;
            }
        }

        /// <summary>
        /// 获得某个策略模板的参数字典
        /// </summary>
        /// <param name="template_id"></param>
        /// <returns></returns>
        public Dictionary<string, ArgumentBase> GetArgument(int template_id)
        {
            return argumentBaseMap[template_id];
        }

        /// <summary>
        /// 更新策略模板的默认参数
        /// 从策略程序集加载参数属性并更新到数据库
        /// </summary>
        public void UpdateArgumentBase(int template_id,ArgumentAttribute attr)
        {
            if (!argumentBaseMap.Keys.Contains(template_id))
            {
                argumentBaseMap.Add(template_id, new Dictionary<string, ArgumentBase>());
            }

            ArgumentBase target = null;
            //不存在对应的默认参数 写入数据库
            if (!argumentBaseMap[template_id].TryGetValue(attr.Name,out target))
            {
                target = new ArgumentBase();
                target.Name = attr.Name;
                target.Template_ID = template_id;
                target.Type = attr.ArgType;
                target.Value = attr.ArgumentValue.Value;
                ORM.MArgument.InsertArgumentBase(target);
                argumentBaseMap[template_id][target.Name] = target;
            }        
        }
    }
}
