using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    public class PluginFinderWrapper
    {
        public PluginFinder finder;
        private AppDomain finderDomain;
        private bool _createDomain = false;

        void CreateFinder()
        {
            if (finder == null)
            {
                if (this._createDomain)
                {
                    AppDomainSetup info = new AppDomainSetup
                    {
                        ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                    };
                    string text1 = "App base:    " + info.ApplicationBase + "\r\nPrivate bin: " + info.PrivateBinPath;
                    this.finderDomain = AppDomain.CreateDomain("Plugin Finder AppDomain", null, info);
                    this.finder = (PluginFinder)this.finderDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(PluginFinder).FullName);
                }
                else
                {
                    this.finder = new PluginFinder();
                }
            }
        }

        public List<Type> LoadBrokerType()
        {
            this.CreateFinder();
            return this.finder.LoadBrokerType();
        }

        public List<Type> LoadDataFeedType()
        {
            this.CreateFinder();
            return this.finder.LoadDataFeedType();
        }

        public List<Type> LoadAccountType()
        {
            this.CreateFinder();
            return this.finder.LoadAccountType();
        }

        public List<Type> LoadAccountRule()
        {
            this.CreateFinder();
            return this.finder.LoadAccountRule();
        }

        public List<Type> LoadOrderRule()
        {
            this.CreateFinder();
            return this.finder.LoadOrderRule();
        }



        public List<IContribPlugin> LoadContribList()
        {
            this.CreateFinder();
            return this.finder.LoadContribList();
        }

        public IContribPlugin LoadContribPlugin(string id)
        {
            this.CreateFinder();
            return this.finder.LoadContribPlugin(id);
        }
        public  IContrib ConstructContrib(string className)
        { 
            this.CreateFinder();
            return this.finder.ConstructContrib(className);

        }

        public List<ContribCommandInfo> FindContribCommand(object obj)
        {
            this.CreateFinder();
            return this.finder.FindContribCommand(obj);
        }
        public List<ContribEventInfo> FindContribEvent(object obj)
        {
            this.CreateFinder();
            return this.finder.FindContribEvent(obj);
        }
        public List<TaskInfo> FindContribTask(object obj)
        {
            this.CreateFinder();
            return this.finder.FindContribTask(obj);
        }

        public List<MethodArgument> GetArgumentList(MethodInfo mi)
        {
            this.CreateFinder();
            return this.finder.GetArgumentList(mi);
        }

        public object[] ParseMethodArgs(List<MethodArgument> args)
        {
            this.CreateFinder();
            return this.finder.ParseMethodArgs(args);
        }
    }
}
