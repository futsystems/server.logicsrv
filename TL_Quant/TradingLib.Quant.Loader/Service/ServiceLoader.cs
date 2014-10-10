using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.Loader
{
    public class ServiceLoader
    {
        // Fields
        private SynchronizationContext _synchContext;
        private Dictionary<string, string> barStorageAssemblyMap;
        private bool explicitLoad;
        private Dictionary<string, string> riskAssessmentAssemblyMap;
        private string searchPath;
        private Dictionary<IQService, string> serviceAssemblyMap;
        private List<IQService> services;

        // Methods
        public ServiceLoader(SynchronizationContext synchContext)
            : this(synchContext, null)
        {
        }

        public ServiceLoader(SynchronizationContext synchContext, string searchPath)
            : this(synchContext, searchPath, false)
        {
        }

        public ServiceLoader(SynchronizationContext synchContext, string searchPath, bool explicitLoad)
        {
            this.services = new List<IQService>();
            this.serviceAssemblyMap = new Dictionary<IQService, string>();
            this.barStorageAssemblyMap = new Dictionary<string, string>();
            this.riskAssessmentAssemblyMap = new Dictionary<string, string>();
            this.searchPath = "";
            this._synchContext = synchContext;
            if (!string.IsNullOrEmpty(searchPath))
            {
                this.SetSearchPath(searchPath);
            }
            if (!explicitLoad)
            {
                this.RefreshServices();
            }
        }

        private void ClearServices()
        {
            foreach (IQService service in this.services)
            {
                try
                {
                    service.Dispose();
                    continue;
                }
                catch (Exception exception)
                {
                    //TraceHelper.DumpExceptionToTrace(exception);
                    //Trace.WriteLine("Error disposing service.");
                    continue;
                }
            }
            this.services.Clear();
            this.serviceAssemblyMap.Clear();
            this.riskAssessmentAssemblyMap.Clear();
            this.barStorageAssemblyMap.Clear();
        }
        /// <summary>
        /// 获得可用服务
        /// </summary>
        /// <returns></returns>
        public List<ServiceInfo> GetAvailableServices()
        {
            List<ServiceInfo> list = new List<ServiceInfo>();
            foreach (IQService service in this.services)
            {
                try
                {
                    //MessageBox.Show(service.id());
                    ServiceInfo item = new ServiceInfo
                    {
                        Name = service.ServiceName(),
                        id = service.id(),
                        Description = service.Description(),
                        Author = service.Author(),
                        BarDataAvailable = service.HisDataAvailable,
                        TickDataAvailable = service.TickDataAvailable,
                        AssemblyName = this.serviceAssemblyMap[service],
                        BrokerFunctionsAvailable = service.BrokerExecutionAvailable
                    };
                    if (item.BrokerFunctionsAvailable)
                    {
                        item.IsSimBroker = !service.GetBrokerInterface().IsLiveBroker();
                    }
                    else
                    {
                        item.IsSimBroker = false;
                    }
                    item.SupportsMultipleInstances = service.SupportsMultipleInstances();
                    list.Add(item);
                    continue;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                    //TraceHelper.DumpExceptionToTrace(exception);
                    //Trace.WriteLine("Error getting service info.");
                    continue;
                }
            }
            return list;
        }

        /// <summary>
        /// 通过某个id得到服务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceInfo GetServiceInfo(string id)
        {
            foreach (ServiceInfo info in this.GetAvailableServices())
            {
                if (info.id == id)
                {
                    return info;
                }
            }
            return null;
        }
        /// <summary>
        /// 加载某个ID的服务 实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQService LoadService(string id)
        {
            //MessageBox.Show("load service id:" + id);
            foreach (IQService service in this.services)
            {
                if (service.id() == id)
                {
                    return new ServiceWrapper((IQService)Activator.CreateInstance(service.GetType()), this._synchContext);
                }
            }
            return null;
        }

        /// <summary>
        /// 加载一个单独的service 该service加载后不放入loader缓存，
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public IQService LoadSingleService(ServiceSetup setup)
        {
            IQService service = null;
            string assemblyName = setup.AssemblyName;
            if (!assemblyName.ToLower().EndsWith(".dll"))
            {
                assemblyName = assemblyName + ".dll";
            }
            string assemblyFile = Path.Combine(this.searchPath, assemblyName);
            //MessageBox.Show("AssemblyFile:" + assemblyFile);
            try
            {
                foreach (Type type in Assembly.Load(AssemblyName.GetAssemblyName(assemblyFile)).GetTypes())
                {
                    //MessageBox.Show("it is here:" + type.FullName + "   " + (type.GetInterface("IQService") != null).ToString());
                    if ((type.GetInterface("IQService") != null) && !type.IsAbstract)
                    {
                        IQService service2 = (IQService)Activator.CreateInstance(type);
                        if (service2.id() == setup.id)
                        {
                            service = new ServiceWrapper(service2, this._synchContext);
                            if (service != null)
                            {
                                service.ServerAddress = setup.ServerAddress;
                                service.Port = setup.Port;
                                service.UserName = setup.Username;
                                service.Password = setup.Password;
                                service.Initialize(setup.CustomSettings);
                            }
                            return service;
                        }
                    }
                }
            }
            catch (MissingMethodException)
            {
            }
            catch (Exception exception)
            {
                //TraceHelper.DumpExceptionToTrace(exception);
                //Trace.WriteLine("Error loading single Service: " + assemblyFile);
            }
            return null;
        }

        /// <summary>
        /// 刷新所有服务
        /// </summary>
        public void RefreshServices()
        {
            this.ClearServices();
            if (string.IsNullOrEmpty(this.searchPath))
            {
                this.searchPath = ServiceGlobals.PluginDirectory;
            }

            foreach (string str in Directory.GetFiles(this.searchPath, "*.dll"))
            {
                //MessageBox.Show("filename:" + str);
                try
                {
                    foreach (Type type in Assembly.Load(AssemblyName.GetAssemblyName(str)).GetTypes())
                    {
                        //MessageBox.Show("Type:" + type.FullName);
                        if ((type.GetInterface("IQService") != null) && !type.IsAbstract)
                        {
                            //MessageBox.Show("type:" + type.FullName);
                            IQService item = (IQService)Activator.CreateInstance(type);
                            this.services.Add(item);
                            this.serviceAssemblyMap.Add(item, Path.GetFileName(str));
                        }
                        if ((type.GetInterface("IBarDataStorage") != null) && !type.IsAbstract)
                        {
                            //this.barStorageAssemblyMap.Add(((IBarDataStorage)Activator.CreateInstance(type)).id(), Path.GetFileName(str));
                        }
                        if ((type.GetInterface("IRiskAssessmentPlugin") != null) && !type.IsAbstract)
                        {
                            //this.riskAssessmentAssemblyMap.Add(((IRiskAssessmentPlugin)Activator.CreateInstance(type)).id(), Path.GetFileName(str));
                        }
                    }
                }
                catch (MissingMethodException)
                {
                }
                catch (Exception exception)
                {
                    //TraceHelper.DumpExceptionToTrace(exception);
                    //Trace.WriteLine("Error loading Service: " + str);
                }
            }
        }

        public void SetSearchPath(string searchPath)
        {
            this.searchPath = searchPath;
        }

        // Properties
        public Dictionary<string, string> BarStorageAssemblyMap
        {
            get
            {
                return this.barStorageAssemblyMap;
            }
        }

        public Dictionary<string, string> RiskAssessmentAssemblyMap
        {
            get
            {
                return this.riskAssessmentAssemblyMap;
            }
        }
    }


}
