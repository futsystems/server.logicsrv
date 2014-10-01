using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using TradingLib.API;


namespace TradingLib.Quant.Base
{
    public class StrategyLoader
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        private SynchronizationContext _synchContext;
        //private Dictionary<string, string> barStorageAssemblyMap;
        //策略类型->constructor映射
        //private Dictionary<string, ConstructorInfo> strategyConstructorMap = new Dictionary<string, ConstructorInfo>();
        //private Dictionary<string, string> strategyFileMap = new Dictionary<string, string>();
        //private Dictionary<string, Type> strategyFullnameTypeMap = new Dictionary<string, Type>();
        //private Dictionary<string, IStrategy> strategyFullNameInstanceMap = new Dictionary<string, IStrategy>();
        private Dictionary<string, StrategyInfo> straegyInfoMap = new Dictionary<string, StrategyInfo>();
        private bool explicitLoad=false;
        //private Dictionary<string, string> riskAssessmentAssemblyMap;
        private string searchPath;
        //private Dictionary<IQService, string> serviceAssemblyMap;
        //private List<IStrategy> strategys;

        public StrategyLoader(SynchronizationContext synchContext)
            : this(synchContext, null, false)
        { }

        public StrategyLoader(SynchronizationContext synchContext, string searchPath, bool explicitLoad)
        {
            //this.strategys = new List<IStrategy>();
            //this.serviceAssemblyMap = new Dictionary<IQService, string>();
            //this.barStorageAssemblyMap = new Dictionary<string, string>();
            //this.riskAssessmentAssemblyMap = new Dictionary<string, string>();
            this.searchPath = BaseGlobals.StrategyDirectory;
            this._synchContext = synchContext;
            if (!string.IsNullOrEmpty(searchPath))
            {
                this.SetSearchPath(searchPath);
            }
            if (!explicitLoad)
            {
                this.RefreshStrategys();
            }
        }
        public void SetSearchPath(string searchPath)
        {
            this.searchPath = searchPath;
        }
        /// <summary>
        /// 获得所有策略列表
        /// </summary>
        /// <returns></returns>
        public List<StrategyInfo> GetAvabileStrategies()
        {
            return new List<StrategyInfo>(straegyInfoMap.Values.ToArray());
        }

        /// <summary>
        /// 检查某个类名的Strategy是否存在
        /// </summary>
        /// <param name="strategyClassName"></param>
        /// <returns></returns>
        public bool IsExistStrategy(string strategyClassName)
        {
            return straegyInfoMap.Keys.Contains(strategyClassName);
        }
        /// <summary>
        /// 通过摸个类名称获得策略信息
        /// 策略信息包含类名/文件名/类型/构造信息等
        /// </summary>
        /// <param name="strategyClassName"></param>
        /// <returns></returns>
        public StrategyInfo GetStrategyInfo(string strategyClassName)
        {
            if (straegyInfoMap.Keys.Contains(strategyClassName))
                return straegyInfoMap[strategyClassName];
            return null;
            
        }
        public void ClearStrategys()
        {
            straegyInfoMap.Clear();
        }
        /// <summary>
        /// 刷新所有策略
        /// </summary>
        public void RefreshStrategys()
        {
            this.ClearStrategys();
            if (string.IsNullOrEmpty(this.searchPath))
            {
                this.searchPath = BaseGlobals.StrategyDirectory;
            }

            foreach (string str in Directory.GetFiles(this.searchPath, "*.dll"))
            {
                //MessageBox.Show("filename:" + str);
                debug("dll filenmae:" + str);
                try
                {
                    foreach (Type type in Assembly.Load(AssemblyName.GetAssemblyName(str)).GetTypes())
                    {
                        //MessageBox.Show("Type:" + type.FullName);
                        //debug("got IStrategy type:" + type.FullName);
                        if ((type.GetInterface("IStrategy") != null) && !type.IsAbstract)
                        {
                            //获得对应的无参数实例化构造信息
                            ConstructorInfo coninfo  = type.GetConstructor(Type.EmptyTypes);
                            if (coninfo != null)
                            {
                                string typefullname = type.FullName;
                                StrategyInfo strainfo = new StrategyInfo();
                                strainfo.Constructor = coninfo;
                                strainfo.FileName = str;
                                strainfo.StrategyClassName = typefullname;
                                strainfo.StrategyType = type;

                                //debug("got IStrategy type:" + typefullname);
                                straegyInfoMap.Add(typefullname, strainfo);
                                //strategyConstructorMap.Add(typefullname, coninfo);//类型->构造信息映射
                                //strategyFileMap.Add(typefullname, str);//类型->文件名映射
                                //strategyFullnameTypeMap.Add(typefullname, type);//

                                //IStrategy item = (IStrategy)Activator.CreateInstance(type);
                                //strategyFullNameInstanceMap.Add(typefullname,item);
                            }

                           // FieldInfo[]  fields =  type.GetFields();
                           // debug("Type:" + type.FullName + " Have Fields:" + fields.Length.ToString());
                           // MessageBox.Show("Type:" + type.FullName + " Have Fields:" + fields.Length.ToString() +fields[0].Name);
                        }
                    }
                }

                catch (Exception exception)
                {
                    //TraceHelper.DumpExceptionToTrace(exception);
                    //Trace.WriteLine("Error loading Service: " + str);
                    debug(exception.ToString());
                }
            }
        }

    }


}
