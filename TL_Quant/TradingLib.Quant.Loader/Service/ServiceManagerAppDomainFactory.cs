using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Reflection;
using System.Threading;

using TradingLib.Quant.Base;

namespace TradingLib.Quant.Loader
{
    public sealed class ServiceManagerAppDomainFactory : ServiceAppDomainFactory
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        // Fields
        private string _appDataPath;
        private string _pluginPath;
        private string _serviceName;
        private ServiceSetup _serviceSetup;
        //private static readonly ILog log = LogManager.GetLogger(typeof(ServiceManagerAppDomainFactory));

        // Methods
        /// <summary>
        /// 通过新建的ServiceSetup来生成对应的服务
        /// </summary>
        /// <param name="serviceSetup"></param>
        /// <param name="appDataPath"></param>
        /// <param name="pluginPath"></param>
        public ServiceManagerAppDomainFactory(ServiceSetup serviceSetup, string appDataPath, string pluginPath)
        {
            this._serviceSetup = serviceSetup;
            this._appDataPath = appDataPath;
            this._pluginPath = pluginPath;
        }
        /// <summary>
        /// 通过FriendlyName生成对应的Factory来生成服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="appDataPath"></param>
        /// <param name="pluginPath"></param>
        public ServiceManagerAppDomainFactory(string serviceName, string appDataPath, string pluginPath)
        {
            this._serviceName = serviceName;
            this._appDataPath = appDataPath;
            this._pluginPath = pluginPath;
        }

        /// <summary>
        /// 不提供任何service信息,则加载paperbroker
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="appDataPath"></param>
        /// <param name="pluginPath"></param>
        public ServiceManagerAppDomainFactory(string appDataPath, string pluginPath)
        {
            this._appDataPath = appDataPath;
            this._pluginPath = pluginPath;
        }

        public override ServiceFactory CreateFactoryInDomain(AppDomain domain)
        {
            if (this._serviceSetup != null)
            {
                debug("CreateFactoryInDomain code path1");
                return (ServiceFactory)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ServiceManagerFactory).FullName, false, BindingFlags.Default, null, new object[] { this._serviceSetup, this._appDataPath, this._pluginPath }, null, null, null);
            }
            debug("CreateFactoryInDomain code path 2");
            if (this._serviceName == null)
            {
                this._serviceName = string.Empty;
            }
            return (ServiceFactory)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ServiceManagerFactory).FullName, false, BindingFlags.Default, null, new object[] { this._serviceName, this._appDataPath, this._pluginPath }, null, null, null);
        }
    }

    internal sealed class ServiceManagerFactory : ServiceFactory
    {
        // Fields
        private string _appDataPath;
        private ServiceLoader _loader;
        private ServiceManager _manager;
        private string _pluginPath;
        private string _serviceName;
        private ServiceSetup _serviceSetup;
        private bool loadExplicit;

        // Methods
        public ServiceManagerFactory(ServiceSetup serviceSetup, string appDataPath, string pluginPath)
        {
            this._serviceSetup = serviceSetup;
            this._appDataPath = appDataPath;
            this._pluginPath = pluginPath;
            this.loadExplicit = true;
        }

        public ServiceManagerFactory(string serviceName, string appDataPath, string pluginPath)
        {
            this._serviceName = serviceName;
            this._appDataPath = appDataPath;
            this._pluginPath = pluginPath;
        }

        public override IQService CreateService()
        {
            IQService service;
            SynchronizationContext current = SynchronizationContext.Current;
            //检测是否提供了friendlyName ,通过服务名来进行加载
            if (!string.IsNullOrEmpty(this._serviceName))
            {
                service = this.Manager.LoadService(this._serviceName);//通过friendname进行加载
                if (service == null)
                {
                    throw new QSQuantError(this.Manager.ErrorText);
                }   
                return service;
            }
            //如果有对应的servicesetup则通过该servicesetup进行加载
            if (this._serviceSetup != null)
            {
                service = this.Loader.LoadSingleService(this._serviceSetup);
            }
            else//以上2中情况之外加载paperbroker
            {
                service = this.Loader.LoadService("{2ED4C20C-279A-A615-F2C6-437CC426A075}");//通过guid进行加载
            }
            if (service == null)
            {
                throw new QSQuantError("Unable to create paper trade broker.");
            }
            return service;
        }

        // Properties
        private ServiceLoader Loader
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new ServiceLoader(SynchronizationContext.Current, this._pluginPath, true);
                    if (!this.loadExplicit)
                    {
                        this._loader.RefreshServices();
                    }
                }
                return this._loader;
            }
        }

        private ServiceManager Manager
        {
            get
            {
                if (this._manager == null)
                {
                    this._manager = new ServiceManager(SynchronizationContext.Current, this._appDataPath, this._pluginPath);
                }
                return this._manager;
            }
        }
    }




}
