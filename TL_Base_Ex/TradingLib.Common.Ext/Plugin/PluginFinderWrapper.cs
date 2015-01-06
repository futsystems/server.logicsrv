using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    public class PluginFinderWrapper:IDisposable
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

        public void Dispose()
        {
            if (finder != null)
            {
                finder = null;
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

        public List<MethodInfo> FindMethod<T>(object obj) where T : Attribute
        {
            this.CreateFinder();
            return this.finder.FindMethod<T>(obj);
        }

        public List<PropertyInfo> FindProperty<T>(object obj) where T : Attribute
        {
            this.CreateFinder();
            return this.finder.FindProperty<T>(obj);
        }
        public List<PropertyInfo> FindProperty<T>(Type type) where T : Attribute
        {
            this.CreateFinder();
            return this.finder.FindProperty<T>(type);
        }

        /// <summary>
        /// 获得某个方法的参数列表
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public List<MethodArgument> GetArgumentList(MethodInfo mi)
        {
            this.CreateFinder();
            return this.finder.GetArgumentList(mi);
        }

        /// <summary>
        /// 将参数转换成 object[] 预备调用方法
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object[] ParseMethodArgs(List<MethodArgument> args)
        {
            this.CreateFinder();
            return this.finder.ParseMethodArgs(args);
        }

        public IList<Type> GetImplementors(string path, Type needtype)
        {
            this.CreateFinder();
            return this.finder.GetImplementors(path, needtype);
        }
    }
}
