using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;


namespace TradingLib.Contrib.ResponseHost
{
    /// <summary>
    /// 策略模板维护器
    /// </summary>
    public class ResponseTemplateTracker
    {
        /// <summary>
        /// 数据库对象映射
        /// </summary>
        Dictionary<int, ResponseTemplate> templateIdMap = new Dictionary<int, ResponseTemplate>();
        Dictionary<string, ResponseTemplate> klassNameMap = new Dictionary<string, ResponseTemplate>();

        /// <summary>
        /// 通过反射获得的类型和参数信息
        /// </summary>
        Dictionary<string, Type> responseTypeMap = new Dictionary<string, Type>();
        Dictionary<string, List<PropertyInfo>> responseArgumentMap = new Dictionary<string, List<PropertyInfo>>();


        public ResponseTemplateTracker()
        {
            //1.从数据库加载策略模板 此处不包含实际的类型加载
            foreach (ResponseTemplate resp in ORM.MResponse.SelectResponseTemplate())
            {
                templateIdMap.Add(resp.ID, resp);
                klassNameMap.Add(resp.ClassName, resp);
            }
        }

        public void LoadResponseTemplate()
        {
            //从扩展目录加载所有实现IResponse的类型
            IList<Type> types = PluginHelper.GetImplementors("Response", typeof(IResponse));
            foreach (Type t in types)
            {
                Util.Debug("Load Response Type:" + t.FullName, QSEnumDebugLevel.INFO);
                //同步服务计划 ServicePlane
                InitResponseTemplate(t);
            }
        }

        /// <summary>
        /// 初始化策略模板类型
        /// </summary>
        /// <param name="type"></param>
        void InitResponseTemplate(Type type)
        {
            string fullname = type.FullName;
            //如果数据库中包含了该策略模板，则加载该类型
            if (klassNameMap.Keys.Contains(fullname))
            {
                responseTypeMap[fullname] = type;
            }
            else//如果数据库中不存在 则需要同步数据库信息 然后加入到内存映射
            {
                ResponseBase instance = (ResponseBase)Activator.CreateInstance(type);
                ResponseTemplate template = new ResponseTemplate();
                template.ClassName = type.FullName;
                template.Name = type.Name;
                template.Title = "";

                ORM.MResponse.InsertResponseTemplate(template);

                templateIdMap.Add(template.ID, template);
                klassNameMap.Add(template.ClassName, template);

                responseTypeMap[fullname] = type;
            }

            int template_id = klassNameMap[fullname].ID;
            //查找用argumentattribute标注过的属性
            List<PropertyInfo> propertyInfos = PluginHelper.FindProperty<ArgumentAttribute>(type);
            //将标记的属性放到map中
            responseArgumentMap[fullname] = propertyInfos;

            //遍历每个属性 获得对应的特性对象
            foreach (PropertyInfo pi in propertyInfos)
            {
                ArgumentAttribute attr = (ArgumentAttribute)Attribute.GetCustomAttribute(pi, typeof(ArgumentAttribute));
                //通过参数特新来更新模板默认参数
                Tracker.ArgumentTracker.UpdateArgumentBase(template_id, attr);
            }
        }

        /// <summary>
        /// 设定策略参数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void SetArgument(object obj, Dictionary<string, Argument> args)
        {
            string fullname = obj.GetType().FullName;
            //LibUtil.Debug("setargument,obj fullname:" + fullname);
            if (!responseTypeMap.Keys.Contains(fullname))
            {
                //如果当前数据集没有记录到该对象的类型 则抛出异常
                throw new Exception("unknow responsetemplate type");
            }

            Type type = responseTypeMap[fullname];
            List<PropertyInfo> propertyInfos = responseArgumentMap[fullname];

            foreach (PropertyInfo pi in propertyInfos)
            {
                ArgumentAttribute attr = (ArgumentAttribute)Attribute.GetCustomAttribute(pi, typeof(ArgumentAttribute));
                Argument arg = args[attr.Name];
                if (arg == null)
                {
                    throw new Exception("arg can not be null");
                }
                pi.SetValue(obj, arg, null);
            }
        }

        public ResponseTemplate this[int id]
        {
            get
            {
                if (templateIdMap.Keys.Contains(id))
                {
                    return templateIdMap[id];
                }
                return null;
            }
        }

        public ResponseTemplate this[string classname]
        {
            get
            {
                if (klassNameMap.Keys.Contains(classname))
                {
                    return klassNameMap[classname];
                }
                return null;
            }
        }

        public IEnumerable<ResponseTemplate> ResponseTemplates
        {
            get
            {
                return templateIdMap.Values;
            }
        }


        public IEnumerable<ArgumentAttribute> GetAttribute(object obj)
        {
            string fullname = obj.GetType().FullName;
            if (!klassNameMap.Keys.Contains(fullname))
            {
                throw new Exception("unknow responsetemplate type");
            }
            List<PropertyInfo> propertyInfos = responseArgumentMap[fullname];
            return propertyInfos.Select(pi => (ArgumentAttribute)Attribute.GetCustomAttribute(pi, typeof(ArgumentAttribute)));
        }
        /// <summary>
        /// 获得某个策略模板类型的参数特性
        /// </summary>
        /// <param name="template_id"></param>
        /// <returns></returns>
        public IEnumerable<ArgumentAttribute> GetAttribute(int template_id)
        {
            if (!templateIdMap.Keys.Contains(template_id))
            {
                throw new Exception("unknow responsetemplate type");
            }
            List<PropertyInfo> propertyInfos = responseArgumentMap[templateIdMap[template_id].ClassName];
            return propertyInfos.Select(pi => (ArgumentAttribute)Attribute.GetCustomAttribute(pi, typeof(ArgumentAttribute)));
        }
        /// <summary>
        /// 获得某个策略模板类型
        /// </summary>
        /// <param name="template_id"></param>
        /// <returns></returns>
        public Type GetResponseType(int template_id)
        {
            ResponseTemplate template = this[template_id];
            if (template == null)
                return null;
            Type tout = null;
            if (responseTypeMap.TryGetValue(template.ClassName, out tout))
                return tout;
            return null;
        }

    }
}
