using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.Loader
{
    public class ServiceManager
    {
        // Fields
        private string _appDataPath;
        private Dictionary<string, IQService> _loadedServices;
        private ServiceLoader _loader;
        private bool _loadSuccess;
        private string _pluginPath;
        private List<ServiceSetup> _services;
        private SynchronizationContext _synchContext;
        private string errorText;
        private bool explicitLoad;
        private const string TWSActiveXID = "{1DAB4AE4-E475-4621-A293-956308FC2351}";
        private const string TWSCSharpID = "{B03427B2-5405-4686-A922-F888836C19BC}";

        // Methods
        public ServiceManager(SynchronizationContext synchContext, string appDataPath)
            : this(synchContext, appDataPath, null)
        {
        }

        public ServiceManager(SynchronizationContext synchContext, string appDataPath, string pluginPath)
            : this(synchContext, appDataPath, pluginPath, false)
        {
        }

        public ServiceManager(SynchronizationContext synchContext, string appDataPath, string pluginPath, bool explicitLoad)
        {
            this._services = new List<ServiceSetup>();
            this._loadedServices = new Dictionary<string, IQService>();
            this.errorText = "";
            this._pluginPath = "";
            this._synchContext = synchContext;
            this.explicitLoad = explicitLoad;
            this._appDataPath = appDataPath;
            if (!string.IsNullOrEmpty(pluginPath))
            {
                this._pluginPath = pluginPath;
            }
            else
            {
                this._pluginPath = PluginGlobals.PluginDirectory;
            }
            if (!explicitLoad)
            {
                this.LoadServices();
            }
        }

        /// <summary>
        /// 新增一个服务
        /// </summary>
        /// <param name="newService"></param>
        /// <returns></returns>
        public bool AddService(ServiceSetup newService)
        {
            this.errorText = "";
            foreach (ServiceSetup setup in this._services)
            {
                if (setup.FriendlyName == newService.FriendlyName)
                {
                    this.errorText = "A service with the same friendly name already exists.";
                    return false;
                }
            }
            List<ServiceSetup> newServices = new List<ServiceSetup>(this._services) {
            newService
             };
            return this.SaveServices(newServices);
        }
        /// <summary>
        /// 查找某个服务
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        private ServiceSetup FindService(string friendlyName)
        {
            foreach (ServiceSetup setup in this._services)
            {
                if (setup.FriendlyName == friendlyName)
                {
                    return setup;
                }
            }
            return null;
        }
        /// <summary>
        /// 通过ID得到插件程序集
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetPluginAssemblyById(string id)
        {
            if (this.Loader.BarStorageAssemblyMap.ContainsKey(id))
            {
                return this.Loader.BarStorageAssemblyMap[id];
            }
            if (this.Loader.RiskAssessmentAssemblyMap.ContainsKey(id))
            {
                return this.Loader.RiskAssessmentAssemblyMap[id];
            }
            return "";
        }

        /// <summary>
        /// 获得某个service的服务信息
        /// </summary>
        /// <param name="FriendlyName"></param>
        /// <returns></returns>
        public ServiceInfo GetServiceInfo(string FriendlyName)
        {
            this.errorText = "";
            ServiceSetup setup = this.FindService(FriendlyName);
            if (setup == null)
            {
                this.errorText = "A service with the specified name does not exist.";
                return null;
            }
            ServiceInfo serviceInfo = this.Loader.GetServiceInfo(setup.id);
            if (serviceInfo == null)
            {
                this.errorText = "Unable to load service info for service: " + setup.ServicePluginName;
                return null;
            }
            return serviceInfo;
        }

        /// <summary>
        /// 获得某个服务的setup info
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        public ServiceSetup GetServiceSetup(string friendlyName)
        {
            this.errorText = "";
            ServiceSetup setup = this.FindService(friendlyName);
            if (setup == null)
            {
                this.errorText = "A service with the specified name does not exist.";
                return null;
            }
            if (string.IsNullOrEmpty(setup.AssemblyName))
            {
                ServiceInfo serviceInfo = this.Loader.GetServiceInfo(setup.id);
                if (serviceInfo == null)
                {
                    this.errorText = "Unable to load service info for service: " + setup.ServicePluginName;
                    return null;
                }
                setup.AssemblyName = serviceInfo.AssemblyName;
            }
            return setup;
        }

        /// <summary>
        /// 加载某个服务
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        public IQService LoadService(string friendlyName)
        {
            this.errorText = "";
            ServiceSetup setup = this.FindService(friendlyName);
            if (setup == null)
            {
                this.errorText = "A service with the specified name (" + friendlyName + ") does not exist.  It may have been renamed.  Please set the new service name by configuring this folder.";
                return null;
            }
            IQService service = this.Loader.LoadService(setup.id);
            if (service == null)
            {
                this.errorText = "The service plugin was not found.  Plugin name: " + setup.ServicePluginName;
                return null;
            }
            service.ServerAddress = setup.ServerAddress;
            service.Port = setup.Port;
            service.UserName = setup.Username;
            service.Password = setup.Password;
            service.Initialize(setup.CustomSettings);
            return service;
        }
        /// <summary>
        /// 加载所有服务
        /// </summary>
        private void LoadServices()
        {
            this.errorText = "";
            this._loadSuccess = false;
            TextReader textReader = null;
            try
            {
                string path = Path.Combine(this._appDataPath, "ServiceSettings.xml");
                if (!File.Exists(path))
                {
                    StreamWriter writer = File.CreateText(path);
                    writer.Write(ServiceGlobals.DefaultServiceSetup);
                    writer.Close();
                }
                textReader = new StreamReader(path);
                XmlSerializer serializer = new XmlSerializer(typeof(List<ServiceSetup>));
                this._services = (List<ServiceSetup>)serializer.Deserialize(textReader);
                //this.UpgradeServices();
                this._loadSuccess = true;
            }
            catch (FileNotFoundException exception)
            {
                //Trace.WriteLine("Service config file not found: " + exception.Message);
                this._services.Clear();
                this._loadSuccess = true;
            }
            catch (Exception exception2)
            {
                this._loadSuccess = false;
                this._services.Clear();
                //TraceHelper.DumpExceptionToTrace(exception2);
                //Trace.WriteLine("Unable to load service list.");
                this.errorText = exception2.Message;
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Close();
                }
            }
        }
        /// <summary>
        /// 从新加载服务
        /// </summary>
        /// <param name="fromDisk"></param>
        /// <returns></returns>
        public bool ReloadServices(bool fromDisk)
        {
            if (fromDisk)
            {
                this.Loader.RefreshServices();
            }
            this.LoadServices();
            return this._loadSuccess;
        }
        /// <summary>
        /// 删除某个服务
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        public bool RemoveService(string friendlyName)
        {
            this.errorText = "";
            ServiceSetup item = this.FindService(friendlyName);
            if (item == null)
            {
                this.errorText = "A service with the specified name does not exist.";
                return false;
            }
            List<ServiceSetup> newServices = new List<ServiceSetup>(this._services);
            newServices.Remove(item);
            return this.SaveServices(newServices);
        }
        /// <summary>
        /// 保存服务配置
        /// </summary>
        /// <param name="newServices"></param>
        /// <returns></returns>
        private bool SaveServices(List<ServiceSetup> newServices)
        {
            TextWriter textWriter = null;
            try
            {
                textWriter = new StreamWriter(Path.Combine(this._appDataPath, "ServiceSettings.xml"));
                new XmlSerializer(typeof(List<ServiceSetup>)).Serialize(textWriter, newServices);
            }
            catch (Exception exception)
            {
                //TraceHelper.DumpExceptionToTrace(exception);
                //Trace.WriteLine("Unable to save service list.");
                this.errorText = exception.Message;
                return false;
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
            this._services = newServices;
            return true;
        }
        /// <summary>
        /// 更新某个服务
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public bool UpdateService(ServiceSetup setup)
        {
            this.errorText = "";
            ServiceSetup item = this.FindService(setup.FriendlyName);
            if (item == null)
            {
                this.errorText = "A service with the specified name does not exist.";
                return false;
            }
            List<ServiceSetup> newServices = new List<ServiceSetup>(this._services);
            newServices.Remove(item);
            newServices.Add(setup);
            return this.SaveServices(newServices);
        }
        /*
        private void UpgradeServices()
        {
            foreach (ServiceSetup setup in this._services)
            {
                if (setup.id == "{1DAB4AE4-E475-4621-A293-956308FC2351}")
                {
                    setup.id = "{B03427B2-5405-4686-A922-F888836C19BC}";
                }
            }
        }
        **/
        // Properties
        public string ErrorText
        {
            get
            {
                return this.errorText;
            }
        }

        private ServiceLoader Loader
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new ServiceLoader(this._synchContext, this._pluginPath);
                    if (!this.explicitLoad)
                    {
                        this._loader.RefreshServices();
                    }
                }
                return this._loader;
            }
        }

        public bool LoadSuccess
        {
            get
            {
                return this._loadSuccess;
            }
        }

        public List<ServiceSetup> Services
        {
            get
            {
                List<ServiceSetup> list = new List<ServiceSetup>();
                foreach (ServiceSetup setup in this._services)
                {
                    list.Add(new ServiceSetup(setup));
                }
                return list;
            }
        }

        // Nested Types
        private class LoadedService
        {
            // Fields
            private string FriendlyName;
        }
    }


}
