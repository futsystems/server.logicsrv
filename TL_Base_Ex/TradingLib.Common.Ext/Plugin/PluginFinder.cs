using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class PluginFinder
    {

        /*关于动态获得操作,封装生成CLICommand的过程
         * 1.获得生成的runner实例,通过遍历其方法,获得用CLICommandAttr标识的方法
         * 2.将方法加载后注入Commmand,形成Command对象
         * 
         * IContrib     辅助模块插件
         * IBroker      成交插件
         * IDataFeed    数据源插件
         * IAccountCheck    帐户风控插件
         * IOrderCheck      委托检查插件
         * 
         * **/

        public List<Type> DesiredInterfaces;//需要加载的插件Interface
        public Dictionary<Type, List<Type>> AvailableTypes;//不同Interface所对应的有效插件类别
        public Dictionary<Type, Dictionary<string, FinderPluginInfo>> Plugins;

        List<string> searchPathList = new List<string>();
        public PluginFinder()
        {
            //设定动态dll加载目录
            searchPathList.Add("RuleSet");
            searchPathList.Add("Contrib");
            searchPathList.Add("Connecter");
            searchPathList.Add("Account");

            this.DesiredInterfaces = new List<Type>();
            this.AvailableTypes = new Dictionary<Type, List<Type>>();
            this.Plugins = new Dictionary<Type, Dictionary<string, FinderPluginInfo>>();
            this.Reset();

            this.DesiredInterfaces.Add(typeof(ICore));
            this.DesiredInterfaces.Add(typeof(IContrib));

            this.DesiredInterfaces.Add(typeof(IBroker));
            this.DesiredInterfaces.Add(typeof(IDataFeed));
            this.DesiredInterfaces.Add(typeof(IAccountCheck));
            this.DesiredInterfaces.Add(typeof(IOrderCheck));
            this.DesiredInterfaces.Add(typeof(IAccount));

        }

        private void Reset()
        {
            this.Plugins.Clear();
            this.AvailableTypes.Clear();
            this.Plugins[typeof(ICore)] = new Dictionary<string, FinderPluginInfo>();//核心模块
            this.Plugins[typeof(IContrib)] = new Dictionary<string, FinderPluginInfo>();//扩展模块插件
            //this.Plugins[typeof(ISeries)] = new Dictionary<string, FinderPluginInfo>();
            //this.Plugins[typeof(ITrigger)] = new Dictionary<string, FinderPluginInfo>();
            //this.Plugins[typeof(IAction)] = new Dictionary<string, FinderPluginInfo>();
            //this.Plugins[typeof(IBackTestReportPlugin)] = new Dictionary<string, FinderPluginInfo>();
        }


        public string PrintPlugins()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-------- Desired Interfaces--------" + "\r\n");
            foreach (Type t in DesiredInterfaces)
            {
                sb.Append("Type:" + t.FullName + "\r\n");
            }
            sb.Append("-------- Avabile Types--------" + "\r\n");
            foreach (Type t in AvailableTypes.Keys)
            {
                sb.Append("--------Interface:"+t.FullName +" Type List----"+"\r\n");
                foreach (Type type in AvailableTypes[t])
                {
                    sb.Append("Type:" + type.FullName + "\r\n");
                }
            }

            foreach (Type t in this.Plugins.Keys)
            {
                sb.Append("--------Interface:" + t.FullName + " Plugin List----" + "\r\n");
                foreach (string s in Plugins[t].Keys)
                {
                    sb.Append("String:" + s + " PluginInfo:" + Plugins[t][s].Attribute.ToString()+"\r\n");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 返回所有成交接口类型
        /// </summary>
        /// <returns></returns>
        public List<Type> LoadBrokerType()
        {
            this.LoadImplementors(typeof(IBroker));
            return AvailableTypes[typeof(IBroker)];
        }

        /// <summary>
        /// 返回所有行情接口类型
        /// </summary>
        /// <returns></returns>
        public List<Type> LoadDataFeedType()
        {
            this.LoadImplementors(typeof(IDataFeed));
            return AvailableTypes[typeof(IDataFeed)];
        }

        /// <summary>
        /// 加载所有帐户类型
        /// </summary>
        /// <returns></returns>
        public List<Type> LoadAccountType()
        {
            this.LoadImplementors(typeof(IAccount));
            return AvailableTypes[typeof(IAccount)];
        }

        public List<Type> LoadAccountRule()
        {
            this.LoadImplementors(typeof(IAccountCheck));
            return AvailableTypes[typeof(IAccountCheck)];
        }

        public List<Type> LoadOrderRule()
        {
            this.LoadImplementors(typeof(IOrderCheck));
            return AvailableTypes[typeof(IOrderCheck)];
        }

        /// <summary>
        /// 获得所有扩展模块插件
        /// </summary>
        /// <returns></returns>
        public List<IContribPlugin> LoadContribList()
        {
            this.LoadImplementors(typeof(IContrib));
            List<IContribPlugin> list = new List<IContribPlugin>();
            foreach (KeyValuePair<string, FinderPluginInfo> pair in this.Plugins[typeof(IContrib)])
            {
                try
                {
                    list.Add(new AttrPluginContrib((ContribAttr)pair.Value.Attribute));
                }
                catch
                {
                    throw;
                }
            }
            return list;
        }

        public IContribPlugin LoadContribPlugin(string id)
        {
            foreach (IContribPlugin plugin in LoadContribList())
            {
                if (plugin.Id == id)
                    return plugin;
            }
            return null;
            
        }



        /// <summary>
        /// 生成某个扩展模块插件的实例
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public IContrib ConstructContrib(string className)
        {
            Type type = this.GetTypeById(typeof(IContrib), className);
            return this.ConstructClass<IContrib>(className);
        }


        /// <summary>
        /// 无参数构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T ConstructClass<T>(string id) where T : class
        {
            return ConstructClass<T>(id, null, false);
        }
        /// <summary>
        /// 创建某个类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="constructorArgs"></param>
        /// <param name="createWrapper"></param>
        /// <returns></returns>
        public T ConstructClass<T>(string id, object[] constructorArgs, bool createWrapper) where T : class
        {
            T local = (T)Activator.CreateInstance(this.GetTypeById(typeof(T), id), constructorArgs);
            //是否给该对象进行wrapper,可以通过wrapper对插件进行操作整形比如增加异常捕捉等,将插件wrapper系统安全采用的方式进行调用
            if (createWrapper)
            { 
            
            }
            return local;
        }


        /// <summary>
        /// 通过ID获得 type
        /// 类的全名
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Type GetTypeById(Type interfaceType, string id)
        {   //获得实现某个接口的所有type
            IList<Type> implementors = this.GetImplementors(interfaceType);
            //首先从插件字典中寻找
            if (this.Plugins.ContainsKey(interfaceType) && this.Plugins[interfaceType].ContainsKey(id))
            {
                return this.Plugins[interfaceType][id].Type;
            }
            //然后遍历所有的avabile对象然后找到对应的类型
            foreach (Type type in implementors)
            {
                if (type.FullName == id)
                {
                    return type;
                }
            }
            throw new Exception("Unable to find class with id " + id + " implementing type " + interfaceType.FullName);
        }

        /// <summary>
        /// 获得实现某个接口的所有实现
        /// </summary>
        /// <param name="Interface"></param>
        /// <returns></returns>
        public IList<Type> GetImplementors(Type Interface)
        {
            if (!this.AvailableTypes.ContainsKey(Interface))
            {
                this.LoadImplementors(Interface);
            }
            return this.AvailableTypes[Interface];
        }
        /// <summary>
        /// 从程序目录加载某个接口的实现
        /// </summary>
        /// <param name="intface"></param>
        public void LoadImplementors(Type intface)
        {
            //TLCtxHelper.Debug("loadimplementors:" + intface.ToString());

            //可用type集合中不存在该interface
            if (!this.AvailableTypes.ContainsKey(intface))
            {
                List<Type> list = new List<Type>();

                foreach (Type type in this.DesiredInterfaces)
                {
                    if (!this.AvailableTypes.ContainsKey(type))
                    {
                        list.Add(type);
                    }
                }

                //获得我们所要查找的intface列表
                if (!list.Contains(intface))
                    list.Add(intface);

                foreach (Type t in list)
                {
                    if (this.AvailableTypes.ContainsKey(t))
                    {
                        this.AvailableTypes[t].Clear();//如果包含该key则清空
                    }
                    else
                    {
                        this.AvailableTypes[t] = new List<Type>();//如果不包含则新增key
                    }

                    if (Plugins.ContainsKey(t))
                    {
                        Plugins[t].Clear();
                    }
                }

                try
                {
                    Dictionary<Type, bool> dictionary = new Dictionary<Type, bool>();
                    List<string> dllfilelist = new List<string>();
                    //遍历搜索路径 获得所有dll文件
                    foreach (string path in searchPathList)
                    {
                        dllfilelist.AddRange(Directory.GetFiles(path, "*.dll"));
                    }

                    foreach (string dllfile in dllfilelist)
                    {
                        //TLCtxHelper.Debug(ExUtil.SectionHeader(" Dll List "));
                        //TLCtxHelper.Debug("Dll File:" + dllfile);
                        try
                        {
                            var assembly = Assembly.ReflectionOnlyLoadFrom(dllfile);
                            AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllfile);
                            foreach (var an in assembly.GetReferencedAssemblies())
                            {
                                try
                                {
                                    Assembly.ReflectionOnlyLoad(an.FullName);
                                }
                                catch
                                {
                                    Assembly.ReflectionOnlyLoadFrom(Path.Combine(Path.GetDirectoryName(dllfile), an.Name + ".dll"));
                                }
                            }
                            foreach (Type type in assembly.GetExportedTypes())
                            {
                                bool flag = false;
                                foreach (Type needType in list)
                                {
                                    //程序集中的type不是抽象函数并且其实现了needType接口,则标记为有效
                                    if (!type.IsAbstract && type.GetInterface(needType.FullName) != null)
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    Assembly a = Assembly.Load(assemblyName);
                                    dictionary[a.GetType(type.FullName)] = true;//标记该类型被加载

                                }
                            }
                            continue;


                        }
                        catch (Exception ex)
                        {
                            TLCtxHelper.Debug("LoadImplementors error" + ex.ToString());
                        }
                    }

                    //重新遍历我们所需要的类型
                    foreach (Type type in list)
                    {
                        foreach (Type typeloaded in dictionary.Keys)
                        {
                            if (typeloaded.GetInterface(type.FullName) != null)
                            {
                                this.AvailableTypes[type].Add(typeloaded);
                            }
                        }

                        //如果插件列表中还包含了我们所需要的类型
                        if (this.Plugins.ContainsKey(type))
                        {
                            foreach (Type avabileType in this.AvailableTypes[type])//遍历该接口所提供的可用类型
                            { 
                                //创建插件信息
                                try
                                {
                                    FinderPluginInfo info = new FinderPluginInfo()
                                    {
                                        Type = avabileType
                                    };

                                    //从该类别中获得客户自定义的插件属性
                                    //从TLObjectAttribute集成的特性属于插件体系
                                    //TLCtxHelper.Debug("find tlobjectattribute for plugin");
                                    TLObjectAttribute customAttribute = (TLObjectAttribute)Attribute.GetCustomAttribute(avabileType, typeof(TLObjectAttribute));
                                    if (customAttribute != null)//如果属性存在,则建立对应的plugininfo然后储存在插件数据结构中
                                    {
                                        //TLCtxHelper.Debug("find tlobjectattribute for plugin");
                                        if ((customAttribute.Id == null) || (customAttribute.Id.Length == 0))
                                        {
                                            customAttribute.Id = avabileType.FullName;
                                        }
                                        info.Attribute = customAttribute;
                                        this.Plugins[type][customAttribute.Id] = info;
                                    }
                                    continue;
                                }
                                catch (Exception ex)
                                { 
                                    
                                }
                            
                            }
                        }
                    }
                }
                catch (Exception ex)
                { 
                    
                }
                
            }
        }

        /// <summary>
        /// 获得某个函数的参数列表
        /// MethodInfo中Order是从1开始的
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public  List<MethodArgument> GetArgumentList(MethodInfo mi)
        {
            List<MethodArgument> list = new List<MethodArgument>();
            ParameterInfo[] parameters = mi.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info2 = parameters[i];
                Type parameterType = info2.ParameterType;

                if (parameterType == typeof(ISession))
                {
                    MethodArgument session = new MethodArgument(info2.Name, QSEnumMethodArgumentType.ISession)
                    {
                        Order = i+1,
                        Value = null
                    };
                    list.Add(session);
                }else if (parameterType == typeof(double))
                {
                    MethodArgument argument1 = new MethodArgument(info2.Name, QSEnumMethodArgumentType.Double)
                    {
                        Order = i + 1,
                        Value = 0
                    };
                    list.Add(argument1);
                }
                else if (parameterType == typeof(int))
                {
                    MethodArgument argument2 = new MethodArgument(info2.Name, QSEnumMethodArgumentType.Integer)
                    {
                        Order = i + 1,
                        Value = 0
                    };
                    list.Add(argument2);
                }
                else if (parameterType == typeof(long))
                {
                    MethodArgument argument3 = new MethodArgument(info2.Name, QSEnumMethodArgumentType.Int64)
                    {
                        Order = i + 1,
                        Value = 0
                    };
                    list.Add(argument3);
                }
                else if (parameterType == typeof(string))
                {
                    MethodArgument argument6 = new MethodArgument(info2.Name, QSEnumMethodArgumentType.String)
                    {
                        Order = i + 1
                    };
                    list.Add(argument6);
                }
                else if (parameterType.IsEnum)
                {
                    MethodArgument argument7 = new MethodArgument(info2.Name, QSEnumMethodArgumentType.Enum)
                    {
                        Order = i + 1,
                        ArgType = parameterType
                    };

                    foreach (string str in Enum.GetNames(parameterType))
                    {
                        object obj2 = Enum.Parse(parameterType, str);
                        argument7.EnumValues[str] = Convert.ChangeType(obj2, Enum.GetUnderlyingType(parameterType));
                        //TLCtxHelper.Debug("**************enum argument list,str:" + str + "  value:" + argument7.EnumValues[str].ToString());
                        //if (argument7.Value == null)
                        //{
                        //    argument7.Value = argument7.EnumValues[str];
                        //}
                    }
                    list.Add(argument7);
                }
                else if (parameterType == typeof(bool))
                {
                    MethodArgument argument8 = new MethodArgument(info2.Name, QSEnumMethodArgumentType.Boolean)
                    {
                        Order = i + 1,
                        Value = false
                    };
                    list.Add(argument8);
                }
                else if (parameterType == typeof(decimal))
                { 
                    MethodArgument argument9 = new MethodArgument(info2.Name, QSEnumMethodArgumentType.Decimal)
                    {
                        Order = i + 1,
                        Value = 0
                    };
                    list.Add(argument9);
                }

            }
            //将自定义的参数特性设置到自动搜索出的对象
            object[] customAttributes = mi.GetCustomAttributes(typeof(MethodArgument), false);
            for (int j = 0; j < customAttributes.Length; j++)
            {
                MethodArgument custattr = customAttributes[j] as MethodArgument;
                if (((custattr != null) && (custattr.Order >= 1)) && ((custattr.Order <= list.Count) && (list[custattr.Order - 1].Type == custattr.Type)))
                {
                    if (!string.IsNullOrEmpty(custattr.Name))
                    {
                        list[custattr.Order - 1].Name = custattr.Name;
                    }
                    if ((custattr.Value != null) && !custattr.Value.Equals(""))
                    {
                        list[custattr.Order - 1].Value = custattr.Value;
                    }
                    if ((custattr.Value != null) && !custattr.Value.Equals(""))
                    {
                        list[custattr.Order - 1].Description = custattr.Description;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 将MethodArguments中的值转换成对应的类型 返回object[]
        /// 用于传递给函数作为参数进行调用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object[] ParseMethodArgs(List<MethodArgument> args)
        { 
            object[] objArray = null;
                if(args.Count>0)
                {
                    objArray = new object[args.Count];
                    
                    //遍历所有的函数参数,然后生成对应的函数参宿列表对象
                    foreach (MethodArgument argument in args)
                    {
                        switch (argument.Type)
                        {
                            case QSEnumMethodArgumentType.ISession:
                                {
                                    objArray[argument.Order - 1] = argument.Value as ISession;
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Boolean:
                                {
                                    objArray[argument.Order - 1] = Convert.ToBoolean(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Decimal:
                                {
                                    objArray[argument.Order - 1] = Convert.ToDecimal(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Double:
                                {
                                    objArray[argument.Order - 1] = Convert.ToDouble(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Enum:
                                {
                                    //TLCtxHelper.Debug("--------- prase method ,type:" + argument.ArgType.ToString()+" value:"+argument.Value);
                                    //Enum.TryParse<>(argument.Value,)
                                    int i = (int)argument.EnumValues[argument.Value.ToString()];
                                    objArray[argument.Order - 1] = Enum.ToObject(argument.ArgType,i);//Enum.ToObject(argument. [argument.Order - 1].ParameterType, argument.Value); ;//Enum.ToObject(argument.Order - 1].ParameterType, argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Int64:
                                {
                                    objArray[argument.Order - 1] = Convert.ToInt64(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Integer:
                                {
                                    objArray[argument.Order - 1] = Convert.ToInt32(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.String:
                                {
                                    objArray[argument.Order - 1] = Convert.ToString(argument.Value);
                                    continue;
                                }
                        }
                    }
                }
                return objArray;
        }


        public  object[] CreateMethodArgs(List<MethodArgument> args, MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (args.Count != parameters.Length)
            {
                TLCtxHelper.Debug("args count do not math method's parameters count");

            }
            object[] objArray = null;
            {
                if(args.Count>0)
                {
                    objArray = new object[args.Count];
                    
                    //遍历所有的函数参数,然后生成对应的函数参宿列表对象
                    foreach (MethodArgument argument in args)
                    {
                        switch (argument.Type)
                        {
                            case QSEnumMethodArgumentType.Boolean:
                                {
                                    objArray[argument.Order - 1] = Convert.ToBoolean(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Decimal:
                                {
                                    objArray[argument.Order - 1] = Convert.ToDecimal(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Double:
                                {
                                    objArray[argument.Order - 1] = Convert.ToDouble(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Enum:
                                {
                                    objArray[argument.Order - 1] = Enum.ToObject(parameters[argument.Order - 1].ParameterType, argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Int64:
                                {
                                    objArray[argument.Order - 1] = Convert.ToInt64(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.Integer:
                                {
                                    objArray[argument.Order - 1] = Convert.ToInt32(argument.Value);
                                    continue;
                                }
                            case QSEnumMethodArgumentType.String:
                                {
                                    objArray[argument.Order - 1] = Convert.ToString(argument.Value);
                                    continue;
                                }
                        }
                    }
                }
            }
            return objArray;
        }
        /// <summary>
        /// 找到某个对象中用某个Attribute标注的所有方法
        /// </summary>
        /// <typeparam name="T">自定义特新</typeparam>
        /// <param name="obj">对象</param>
        public  List<MethodInfo> FindMethod<T>(object obj) where T:Attribute
        {
            List<MethodInfo> list = new List<MethodInfo>();
            Type type = obj.GetType();
            MethodInfo[] methodInfos =  type.GetMethods();
            //查看methodinfo中是否有用T标记的方法
            foreach (MethodInfo mi in methodInfos)
            {
                T attr = (T)Attribute.GetCustomAttribute(mi, typeof(T));
                if (attr != null)
                {
                    //TLCtxHelper.Debug(mi.Name);
                    list.Add(mi);
                }
            }
            return list;
        }

        /// <summary>
        /// 查找对象中用某个attr标注的所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<PropertyInfo> FindProperty<T>(object obj) where T : Attribute
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            Type type = obj.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo pi in propertyInfos)
            {
                T attr = (T)Attribute.GetCustomAttribute(pi, typeof(T));
                if (attr != null)
                {
                    list.Add(pi);
                }
            }
            return list;
        }

        public List<PropertyInfo> FindProperty<T>(Type type) where T : Attribute
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo pi in propertyInfos)
            {
                T attr = (T)Attribute.GetCustomAttribute(pi, typeof(T));
                if (attr != null)
                {
                    list.Add(pi);
                }
            }
            return list;
        }

        /// <summary>
        /// 获得某个对象的扩展模块方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<ContribCommandInfo> FindContribCommand(object obj)
        {
            List<ContribCommandInfo> list = new List<ContribCommandInfo>();
            Type type = obj.GetType();
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo mi in methodInfos)
            {
                ContribCommandAttr[] attrs = (ContribCommandAttr[])Attribute.GetCustomAttributes(mi, typeof(ContribCommandAttr));
                if (attrs !=null && attrs.Length>=1)
                {
                    foreach (ContribCommandAttr attr in attrs)
                    {
                        list.Add(new ContribCommandInfo(mi, attr, obj));
                    }
                    /*
                    list.Add(new ContribCommandInfo(mi, attr,obj));
                    
                    List<MethodArgument> args = GetArgumentList(mi);
                    foreach (MethodArgument arg in args)
                    {
                        TLCtxHelper.Debug(arg.ToString());
                    }**/
                }
            }
            return list;
        }

        public List<ContribEventInfo> FindContribEvent(object obj)
        {
            List<ContribEventInfo> list = new List<ContribEventInfo>();
            Type type = obj.GetType();
            EventInfo[] eventInfos = type.GetEvents();
            foreach (EventInfo ei in eventInfos)
            {
                ContribEventAttr[] attrs = (ContribEventAttr[])Attribute.GetCustomAttributes(ei, typeof(ContribEventAttr));
                if(attrs !=null && attrs.Length>=1)
                {
                    foreach (ContribEventAttr attr in attrs)
                    {
                        list.Add(new ContribEventInfo(ei, attr,obj));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获得某个对象的扩展模块任务
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<TaskInfo> FindContribTask(object obj)
        {
            List<TaskInfo> list = new List<TaskInfo>();
            Type type = obj.GetType();
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo mi in methodInfos)
            {
                TaskAttr[] attrs = (TaskAttr[])Attribute.GetCustomAttributes(mi, typeof(TaskAttr));
                if(attrs!=null && attrs.Length>=1)
                {
                    foreach(TaskAttr attr in attrs)
                    {
                        list.Add(new TaskInfo(mi,attr));
                    }
                }
            }
            return list;
        }



        /// <summary>
        /// 从某个文件夹加载实现某个接口的类型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Interface"></param>
        /// <returns></returns>
        public IList<Type> GetImplementors(string path, Type needtype)
        {
            //遍历搜索路径 获得所有dll文件
            List<string> dllfilelist = new List<string>();
            dllfilelist.AddRange(Directory.GetFiles(path, "*.dll"));

            List<Type> types = new List<Type>();
            foreach (string dllfile in dllfilelist)
            {
                //TLCtxHelper.Debug(ExUtil.SectionHeader(" Dll List "));
                //TLCtxHelper.Debug("Dll File:" + dllfile);
                try
                {
                    var assembly = Assembly.ReflectionOnlyLoadFrom(dllfile);
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllfile);
                    foreach (var an in assembly.GetReferencedAssemblies())
                    {
                        try
                        {
                            Assembly.ReflectionOnlyLoad(an.FullName);
                        }
                        catch
                        {
                            Assembly.ReflectionOnlyLoadFrom(Path.Combine(Path.GetDirectoryName(dllfile), an.Name + ".dll"));
                        }
                    }
                    foreach (Type type in assembly.GetExportedTypes())
                    {
                        //程序集中的type不是抽象函数并且其实现了needType接口,则标记为有效
                        if (!type.IsAbstract && type.GetInterface(needtype.FullName) != null)
                        {
                            Assembly a = Assembly.Load(assemblyName);
                            types.Add(a.GetType(type.FullName));
                        }

                    }
                }
                catch (Exception ex)
                {
                    TLCtxHelper.Debug("GetImplementors error" + ex.ToString());
                }
            }
            return types;
        }
    }
}
