using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Reflection;
using System.IO;

using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    [Serializable]
    public class PluginFinder
    {
        
        string SearchPath = string.Empty;
        public List<Type> DesiredInterfaces;
        public Dictionary<Type, List<Type>> AvailableTypes;
        public Dictionary<Type, Dictionary<string, FinderPluginInfo>> Plugins;
 

        
        public void SetSearchPath(string path)
        {
            this.SearchPath = path;
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        public PluginFinder(bool doLease)
        {
            this.DesiredInterfaces = new List<Type>();
            this.AvailableTypes = new Dictionary<Type, List<Type>>();
            this.Plugins = new Dictionary<Type, Dictionary<string, FinderPluginInfo>>();
            //if (doLease)
            //{
            //    LifetimeServices.LeaseTime = TimeSpan.FromDays(36500.0);
            //}
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(this.CurrentDomain_ReflectionOnlyAssemblyResolve);
            this.Reset();
            this.DesiredInterfaces.Add(typeof(ISeries));
            //this.DesiredInterfaces.Add(typeof(ISeries));
            //this.DesiredInterfaces.Add(typeof(ITrigger));
            //this.DesiredInterfaces.Add(typeof(IAction));
            //this.DesiredInterfaces.Add(typeof(IRiskAssessment));
            this.DesiredInterfaces.Add(typeof(IIndicatorPlugin));
            //this.DesiredInterfaces.Add(typeof(IBarDataStorage));
            //this.DesiredInterfaces.Add(typeof(IRiskAssessmentPlugin));
            this.DesiredInterfaces.Add(typeof(IBackTestReportPlugin));
        }

        private void Reset()
        {
            this.Plugins.Clear();
            this.AvailableTypes.Clear();
            this.Plugins[typeof(ISeries)] = new Dictionary<string, FinderPluginInfo>();//指标插件
            //this.Plugins[typeof(ISeries)] = new Dictionary<string, FinderPluginInfo>();
            //this.Plugins[typeof(ITrigger)] = new Dictionary<string, FinderPluginInfo>();
            //this.Plugins[typeof(IAction)] = new Dictionary<string, FinderPluginInfo>();
            this.Plugins[typeof(IBackTestReportPlugin)] = new Dictionary<string, FinderPluginInfo>();
        }

        /// <summary>
        /// 获得指标参数
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public List<ConstructorArgument> GetArgumentList(Type interfaceType, string className)
        {
            //获得具体的type
            return GetArgumentList(this.GetTypeById(interfaceType, className));
        }

        /// <summary>
        /// 获得指标的输入序列
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public List<SeriesInputAttribute> GetSeriesInputs(Type interfaceType, string className)
        {
            List<SeriesInputAttribute> list = new List<SeriesInputAttribute>((SeriesInputAttribute[])this.GetTypeById(interfaceType, className).GetCustomAttributes(typeof(SeriesInputAttribute), false));
            list.Sort(delegate(SeriesInputAttribute att1, SeriesInputAttribute att2)
            {
                if (att1.Order < att2.Order)
                {
                    return -1;
                }
                if (att1.Order > att2.Order)
                {
                    return 1;
                }
                return 0;
            });
            return list;
        }





        
        /// <summary>
        /// 获得某个type所对应的构造参数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ConstructorArgument> GetArgumentList(Type type)
        {
            List<ConstructorArgument> list = new List<ConstructorArgument>();
            ConstructorInfo info = FindConstructor(type);
            ParameterInfo[] parameters = info.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info2 = parameters[i];
                Type parameterType = info2.ParameterType;
                if (parameterType == typeof(double))
                {
                    ConstructorArgument item = new ConstructorArgument(info2.Name, ConstructorArgumentType.Double)
                    {
                        Order = i + 1,
                        Value = 0
                    };
                    list.Add(item);
                }
                else if (parameterType == typeof(int))
                {
                    ConstructorArgument argument2 = new ConstructorArgument(info2.Name, ConstructorArgumentType.Integer)
                    {
                        Order = i + 1,
                        Value = 0
                    };
                    list.Add(argument2);
                }
                else if (parameterType == typeof(long))
                {
                    ConstructorArgument argument3 = new ConstructorArgument(info2.Name, ConstructorArgumentType.Int64)
                    {
                        Order = i + 1,
                        Value = 0
                    };
                    list.Add(argument3);
                }
                else if (parameterType == typeof(BarDataType))
                {
                    ConstructorArgument argument4 = new ConstructorArgument(info2.Name, ConstructorArgumentType.BarElement)
                    {
                        Order = i + 1,
                        Value = BarDataType.Close
                    };
                    list.Add(argument4);
                }
                else if (parameterType == typeof(ChartPane))
                {
                    ConstructorArgument argument5 = new ConstructorArgument(info2.Name, ConstructorArgumentType.ChartPane)
                    {
                        Order = i + 1
                    };
                    list.Add(argument5);
                }
                else if (parameterType == typeof(string))
                {
                    ConstructorArgument argument6 = new ConstructorArgument(info2.Name, ConstructorArgumentType.String)
                    {
                        Order = i + 1
                    };
                    list.Add(argument6);
                }
                else if (parameterType.IsEnum)
                {
                    ConstructorArgument argument7 = new ConstructorArgument(info2.Name, ConstructorArgumentType.Enum)
                    {
                        Order = i + 1
                    };
                    foreach (string str in Enum.GetNames(parameterType))
                    {
                        object obj2 = Enum.Parse(parameterType, str);
                        argument7.EnumValues[str] = Convert.ChangeType(obj2, Enum.GetUnderlyingType(parameterType));
                        if (argument7.Value == null)
                        {
                            argument7.Value = argument7.EnumValues[str];
                        }
                    }
                    list.Add(argument7);
                }
                else if (parameterType == typeof(bool))
                {
                    ConstructorArgument argument8 = new ConstructorArgument(info2.Name, ConstructorArgumentType.Boolean)
                    {
                        Order = i + 1,
                        Value = false
                    };
                    list.Add(argument8);
                }
            }
            object[] customAttributes = info.GetCustomAttributes(typeof(ConstructorArgument), false);
            for (int j = 0; j < customAttributes.Length; j++)
            {
                ConstructorArgument argument9 = customAttributes[j] as ConstructorArgument;
                if (((argument9 != null) && (argument9.Order >= 1)) && ((argument9.Order <= list.Count) && (list[argument9.Order - 1].Type == argument9.Type)))
                {
                    if (!string.IsNullOrEmpty(argument9.Name))
                    {
                        list[argument9.Order - 1].Name = argument9.Name;
                    }
                    if ((argument9.Value != null) && !argument9.Value.Equals(""))
                    {
                        list[argument9.Order - 1].Value = argument9.Value;
                    }
                }
            }
            return list;
        }


 


        /// <summary>
        /// 加载指标列表
        /// </summary>
        /// <returns></returns>
        public List<IIndicatorPlugin> LoadIndicatorList()
        {
            this.LoadImplementors(typeof(ISeries));
            List<IIndicatorPlugin> list = new List<IIndicatorPlugin>();
            foreach (KeyValuePair<string, FinderPluginInfo> pair in this.Plugins[typeof(ISeries)])
            {
                try
                {
                    list.Add(new AttributePlugin((IndicatorAttribute)pair.Value.attribute));
                    continue;
                }
                catch
                {
                    throw;
                }
            }
            return list;
        }

        /// <summary>
        /// 加载某个指标
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IIndicatorPlugin LoadIndicatorPlugin(string Id)
        {
            foreach (IIndicatorPlugin plugin in this.LoadIndicatorList())
            {
                if (plugin.id() == Id)
                {
                    return plugin;
                }
            }
            return null;
        }

 



        /// <summary>
        /// 构造指标实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <param name="createWrapper"></param>
        /// <returns></returns>
        public ISeries ConstructIndicator(string id, List<ConstructorArgument> args, bool createWrapper)
        {
            Type typeById = this.GetTypeById(typeof(ISeries), id);
            object[] constructorArgs = CreateConstructorArgs(args, typeById);
            return this.ConstructClass<ISeries>(id, constructorArgs, createWrapper);
        }




       




        /// <summary>
        /// 生成一个报告插件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createWrapper"></param>
        /// <returns></returns>
        public IBackTestReportPlugin ConstructBackTestRepoert(string id, bool createWrapper)
        {
            this.GetTypeById(typeof(IBackTestReportPlugin), id);
            return this.ConstructClass<IBackTestReportPlugin>(id, null, createWrapper);
        }


        /// <summary>
        /// 获得某个id对应的插件属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BackTestReportAttribute LoadBackTestPlugin(string id)
        {
            foreach (BackTestReportAttribute attribute in this.LoadBackTestReportList())
            {
                if (attribute.Id == id)
                {
                    return attribute;
                }
            }
            return null;
        }
        /// <summary>
        /// 加载回测报告插件列表
        /// </summary>
        /// <returns></returns>
        public List<BackTestReportAttribute> LoadBackTestReportList()
        {
            this.LoadImplementors(typeof(IBackTestReportPlugin));
            List<BackTestReportAttribute> list = new List<BackTestReportAttribute>();
            foreach (KeyValuePair<string, FinderPluginInfo> pair in this.Plugins[typeof(IBackTestReportPlugin)])
            {
                list.Add((BackTestReportAttribute)pair.Value.attribute);
            }
            return list;
        }

        #region 类构造 以及 构造参数
        /// <summary>
        /// 生成对应的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="constructorArgs"></param>
        /// <param name="createWrapper"></param>
        /// <returns></returns>
        public T ConstructClass<T>(string id, object[] constructorArgs, bool createWrapper) where T : class
        {
            T local = (T)Activator.CreateInstance(this.GetTypeById(typeof(T), id), constructorArgs);
            if (createWrapper)
            {
                if (local is IIndicator)
                {
                    return (new IndicatorWrapper((IIndicator)local) as T);
                }
                /*
                if (local is ISeriesCalculator)
                {
                    return (new SeriesCalculatorWrapper((ISeriesCalculator)local) as T);
                }**/
                /*
                if (local is ITrigger)
                {
                    return (new TriggerWrapper((ITrigger)local) as T);
                }
                if (local is IAction)
                {
                    return (new ActionWrapper((IAction)local) as T);
                }
                if (local is IRiskAssessment)
                {
                    return (new RiskAssessmentWrapper((IRiskAssessment)local) as T);
                }
                if (local is IBarDataStorage)
                {
                    return (new DataStorageWrapper((IBarDataStorage)local) as T);
                }
                if (local is ISystemResultPlugin)
                {
                    local = new SystemResultWrapper((ISystemResultPlugin)local) as T;
                }**/
            }
            return local;
        }

        /// <summary>
        /// 通过类型找到构造信息
        /// 我们需要得到的是不含ISeries的最大构造行数，因为其他少参数的构造函数个可能是某个参数的默认简化
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected static ConstructorInfo FindConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo info = null;
            foreach (ConstructorInfo info2 in constructors)
            {
                if (info == null)
                {
                    info = info2;
                }
                else if (info.GetParameters().Length < info2.GetParameters().Length)
                {
                    bool flag = true;
                    foreach (ParameterInfo info3 in info2.GetParameters())
                    {
                        if (typeof(ISeries).IsAssignableFrom(info3.ParameterType))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        info = info2;
                    }
                }
            }
            return info;
        }


        /// <summary>
        /// 生成构造函数所需要的参数
        /// </summary>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object[] CreateConstructorArgs(List<ConstructorArgument> args, Type type)
        {
            ParameterInfo[] parameters = FindConstructor(type).GetParameters();
            if (args.Count != parameters.Length)
            {
                throw new QSQuantError(string.Concat(new object[] { "The constructor for ", type.FullName, " requires ", parameters.Length, " arguments, but ", args.Count, " were passed in." }));
            }
            object[] objArray = null;//构造参数数组
            if (args.Count > 0)
            {
                objArray = new object[args.Count];
                foreach (ConstructorArgument argument in args)//遍历构造参数列表,生成对应的构造参数数组
                {
                    switch (argument.Type)
                    {
                        case ConstructorArgumentType.String:
                            {
                                string str = argument.Value.ToString();
                                objArray[argument.Order - 1] = str;
                                continue;
                            }
                        case ConstructorArgumentType.Integer:
                            {
                                int num2 = Convert.ToInt32(argument.Value);
                                objArray[argument.Order - 1] = num2;
                                continue;
                            }
                        case ConstructorArgumentType.Double:
                            {
                                double num = Convert.ToDouble(argument.Value);
                                objArray[argument.Order - 1] = num;
                                continue;
                            }
                        case ConstructorArgumentType.BarElement:
                            {
                                BarDataType element = (BarDataType)argument.Value;
                                objArray[argument.Order - 1] = element;
                                continue;
                            }
                        case ConstructorArgumentType.Enum:
                            {
                                objArray[argument.Order - 1] = Enum.ToObject(parameters[argument.Order - 1].ParameterType, argument.Value);
                                continue;
                            }
                        case ConstructorArgumentType.ChartPane:
                        case ConstructorArgumentType.UserDefined:
                            {
                                continue;
                            }
                        case ConstructorArgumentType.Boolean:
                            {
                                objArray[argument.Order - 1] = Convert.ToBoolean(argument.Value);
                                continue;
                            }
                        case ConstructorArgumentType.Int64:
                            {
                                long num3 = Convert.ToInt64(argument.Value);
                                objArray[argument.Order - 1] = num3;
                                continue;
                            }
                    }
                }
            }
            return objArray;
        }


        #endregion

        #region 底层基础函数
        /// <summary>
        /// 通过ID获得 type
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Type GetTypeById(Type interfaceType, string id)
        {   //获得实现某个接口的所有type
            IList<Type> implementors = this.GetImplementors(interfaceType);
            if (this.Plugins.ContainsKey(interfaceType) && this.Plugins[interfaceType].ContainsKey(id))
            {
                return this.Plugins[interfaceType][id].type;
            }
            foreach (Type type in implementors)
            {
                if (type.FullName == id)
                {
                    return type;
                }
            }
            throw new QSQuantError("Unable to find class with id " + id + " implementing type " + interfaceType.FullName);
        }
        //获得实现某个接口的所有type
        public IList<Type> GetImplementors(Type Interface)
        {
            if (!this.AvailableTypes.ContainsKey(Interface))
            {
                this.LoadImplementors(Interface);
            }
            return this.AvailableTypes[Interface];
        }

        //加载某个interface的所有implement
        public void LoadImplementors(Type intface)
        {
            //可用type集合中不存在该interface
            //MessageBox.Show("查找:" + intface.ToString() + " 所有实现者");
            if (!this.AvailableTypes.ContainsKey(intface))
            {
                List<Type> list = new List<Type>();

                //将desiredInterfaces初始化到 avabileTypes
                foreach (Type type in this.DesiredInterfaces)//遍历所有需要的type
                {
                    if (!this.AvailableTypes.ContainsKey(type))//如果可用中不包含该type
                    {
                        list.Add(type);//则记录该type
                    }
                }
                //列表中不包含该type,则增加interface 然后遍历所有的文件查找信息
                if (!list.Contains(intface))
                {
                    list.Add(intface);
                }

                foreach (Type type2 in list)//遍历list中的type
                {
                    //MessageBox.Show("准备查找:"+ type2.ToString() +"所有实现者");
                    if (this.AvailableTypes.ContainsKey(type2))
                    {
                        this.AvailableTypes[type2].Clear();//如果包含该key，则清空对应的list
                    }
                    else
                    {
                        this.AvailableTypes[type2] = new List<Type>();//重新建立该type对应的list
                    }
                    if (this.Plugins.ContainsKey(type2)) //如果有对应的类型的的插件,则先情况该插件list
                    {
                        this.Plugins[type2].Clear();
                    }
                }
                try
                {
                    Dictionary<Type, bool> dictionary = new Dictionary<Type, bool>();
                    List<string> list2 = new List<string>();
                    //MessageBox.Show("searchpath:" + this.SearchPath.ToString());
                    list2.AddRange(Directory.GetFiles(this.SearchPath, "*.dll"));//遍历所有插件文件夹下的文件

                    //string startupPath = Application.StartupPath; //程序启动文件夹 
                    //if (startupPath != this.SearchPath)//同时遍历启动文件夹下面所有的dll文件
                    //{
                    //    list2.AddRange(Directory.GetFiles(startupPath, "*.dll"));
                    //}

                    foreach (string str2 in list2)
                    {
                        try
                        {
                            //加载plugin dll
                            var assembly = Assembly.ReflectionOnlyLoadFrom(str2);
                            AssemblyName assemblyName = AssemblyName.GetAssemblyName(str2);
                            //加载该dll所依赖的dll 否则无法使用导出的Type
                            foreach (var an in assembly.GetReferencedAssemblies())
                            {
                                try
                                {
                                    Assembly.ReflectionOnlyLoad(an.FullName);
                                }
                                catch
                                {
                                    Assembly.ReflectionOnlyLoadFrom(Path.Combine(Path.GetDirectoryName(str2), an.Name + ".dll"));
                                }
                            }

                            foreach (Type type3 in assembly.GetExportedTypes())
                            {
                                bool flag = false;
                                //MessageBox.Show("type in dll:" + type3.ToString());
                                foreach (Type type4 in list)
                                {
                                    //如果文件中的不是abstract并且 继承了 我们需要的list中的type,则标记为true
                                    if (!type3.IsAbstract && (type3.GetInterface(type4.FullName) != null))
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    Assembly assembly2 = Assembly.Load(assemblyName);//加载该程序集,并且将该type标记为true
                                    dictionary[assembly2.GetType(type3.FullName)] = true;
                                }
                            }
                            continue;
                        }
                        catch (ReflectionTypeLoadException exception)
                        {
                            string message = exception.Message;
                            continue;
                        }
                        catch (BadImageFormatException exception2)
                        {
                            string text2 = exception2.Message;
                            continue;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    //重新遍历我们所需要的类型
                    foreach (Type type5 in list)
                    {
                        foreach (Type type6 in dictionary.Keys)//查看dictionary中储存的类型
                        {
                            //如果字典中的类型继承了我们需要的接口,则avabiletypes[] 其储存了支持某个接口的所有类型
                            if (type6.GetInterface(type5.FullName) != null)
                            {
                                this.AvailableTypes[type5].Add(type6);
                            }
                        }
                        //如果插件中还包含了该我们需要的类型
                        if (this.Plugins.ContainsKey(type5))
                        {
                            foreach (Type type7 in this.AvailableTypes[type5])//遍历该接口对应的可用类型
                            {
                                try
                                {
                                    //新建对应的插件信息
                                    FinderPluginInfo info = new FinderPluginInfo()
                                    {
                                        type = type7
                                    };

                                    //从该类别中获得客户自定义的插件属性
                                    QuantShopObjectAttribute customAttribute = (QuantShopObjectAttribute)Attribute.GetCustomAttribute(type7, typeof(QuantShopObjectAttribute));
                                    if (customAttribute != null)//如果属性存在,则建立对应的plugininfo然后储存在插件数据结构中
                                    {
                                        if ((customAttribute.Id == null) || (customAttribute.Id.Length == 0))
                                        {
                                            customAttribute.Id = type7.FullName;
                                        }
                                        info.attribute = customAttribute;
                                        this.Plugins[type5][customAttribute.Id] = info;
                                    }
                                    continue;
                                }
                                catch (AmbiguousMatchException)
                                {
                                    continue;
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                            continue;
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                }
            }
        }

        #endregion 
 


 

 


 


    }
}
