using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using TradingLib.API;

using TradingLib.Quant.Base;
namespace TradingLib.Quant.Plugin
{
    /// <summary>
    /// 插件管理器,用于动态的加载和创建插件
    /// </summary>
    public sealed class PluginManager
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        // Fields
        private SerializableDictionary<string, FilePluginList> _tokens;
        public static bool _watchingLoadedAssemblies;
        //private static readonly ILog log = LogManager.GetLogger(typeof(PluginManager));

        // Methods
        public PluginManager(string cacheFilename)
        {
            this.CacheFilename = cacheFilename;
            this._tokens = new SerializableDictionary<string, FilePluginList>();
        }

        /// <summary>
        /// 创建一个新的AppDomian,并附加一些私有的程序集路径
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <param name="additionalBinPaths"></param>
        /// <returns></returns>
        public AppDomain CreateAppDomain(string friendlyName, params string[] additionalBinPaths)
        {
            AppDomainSetup info = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = this.PluginDirectory
            };
            foreach (string str in additionalBinPaths)
            {
                info.PrivateBinPath = info.PrivateBinPath + ";" + str;
            }
            PermissionSet grantSet = new PermissionSet(PermissionState.Unrestricted);
            AppDomain domain = AppDomain.CreateDomain(friendlyName, AppDomain.CurrentDomain.Evidence, info, grantSet);
            //AppDomain domain = AppDomain.CreateDomain(friendlyName, AppDomain.CurrentDomain.Evidence, info, grantSet, new StrongName[0]);
            ((InitializeAppDomainStub)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(InitializeAppDomainStub).FullName)).InitializeAppDomain(this.PluginDirectory, this.CacheFilename, PluginGlobals.UserAppDataPath);
            
            return domain;
        }

        public object CreatePlugin(PluginSettings settings)
        {
            return this.CreatePlugin(settings, AppDomain.CurrentDomain).Plugin;
        }

        public PluginReference CreatePlugin(PluginSettings settings, AppDomain targetDomain)
        {
            if (targetDomain == null)
            {
                targetDomain = AppDomain.CurrentDomain;
            }
            return this.CreateStubInDomain(targetDomain).CreatePlugin(settings);
        }

        private PluginManagerStub CreateStubInDomain(AppDomain targetDomain)
        {
            if (targetDomain == AppDomain.CurrentDomain)
            {
                return new PluginManagerStub();
            }
            InitializeAppDomainStub stub2 = (InitializeAppDomainStub)targetDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(InitializeAppDomainStub).FullName);
            if (!stub2.AppDomainIsInitialized())
            {
                throw new QSQuantError("The AppDomain " + targetDomain.FriendlyName + " was not initialized correctly.  AppDomains in RightEdge should be created with PluginManager.CreateAppDomain.");
            }
            return (PluginManagerStub)targetDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(PluginManagerStub).FullName);
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LogAssembly(args.LoadedAssembly);
        }

        public List<PluginToken> GetTokens()
        {
            this.Scan();
            List<PluginToken> list = new List<PluginToken>();
            foreach (FilePluginList list2 in this._tokens.Values)
            {
                list.AddRange(list2.Tokens);
            }
            return list;
        }

        private void LoadCache()
        {
            SerializableDictionary<string, FilePluginList> plugins = new SerializableDictionary<string, FilePluginList>();
            try
            {
                using (XmlReader reader = XmlReader.Create(this.CacheFilename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(PluginCache));
                    PluginCache cache = null;
                    try
                    {
                        cache = (PluginCache)serializer.Deserialize(reader);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    if ((cache != null) && (cache.PluginDirectory == this.PluginDirectory))
                    {
                        plugins = cache.Plugins;
                    }
                }
            }
            catch (FileNotFoundException)
            {
            }
            this._tokens = plugins;
        }

        private static void LogAssembly(Assembly asm)
        {
            string codeBase;
            try
            {
                codeBase = asm.CodeBase;
            }
            catch (Exception exception)
            {
                codeBase = "Could not access CodeBase: " + exception.GetType().FullName;
            }
            string message = string.Format("{2}: Assembly: {0} Location: {1}", asm.FullName, codeBase, AppDomain.CurrentDomain.FriendlyName);
            //log.Debug(message);
        }

        public static void LogLoadedAssemblies()
        {
            if (!_watchingLoadedAssemblies)
            {
                _watchingLoadedAssemblies = true;
                AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(PluginManager.CurrentDomain_AssemblyLoad);
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    LogAssembly(assembly);
                }
            }
        }

        private void SaveCache()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                CloseOutput = true,
                Indent = true
            };
            using (XmlWriter writer = XmlWriter.Create(this.CacheFilename, settings))
            {
                PluginCache o = new PluginCache
                {
                    PluginDirectory = this.PluginDirectory,
                    Plugins = this._tokens
                };
                new XmlSerializer(typeof(PluginCache)).Serialize(writer, o);
            }
        }
        public void Scan()
        {
            Scan(false);
        }
        public void Scan(bool noload = false)
        {

            debug("try to scan....:"+this.PluginDirectory.ToString());
            if(!noload)
                this.LoadCache();
            bool flag = false;
            AppDomain targetDomain = null;
            SerializableDictionary<string, FilePluginList> dictionary = new SerializableDictionary<string, FilePluginList>(this._tokens);
            try
            {
                PluginManagerStub stub = null;
                
                List<string> list = new List<string>(Directory.GetFiles(this.PluginDirectory, "*.dll"));
                foreach (string str in new List<string>(dictionary.Keys))
                {
                    
                    if (!list.Contains(str))
                    {
                        dictionary.Remove(str);
                        flag = true;
                    }
                }
                foreach (string str2 in list)
                {
                    //debug("fileName:" + str2);
                    bool flag2 = true;
                    if (dictionary.ContainsKey(str2))
                    {
                        //debug("check file path");
                        FileInfo info = new FileInfo(Path.Combine(this.PluginDirectory, str2));
                        if ((info.LastWriteTime == dictionary[str2].LastModifiedDate) && (info.Length == dictionary[str2].FileSize))
                        {
                            flag2 = false;
                        }
                    }
                    if (flag2)
                    {
                        if (targetDomain == null)
                        {
                            targetDomain = this.CreateAppDomain("Plugin Manager Stub Domain", new string[0]);
                            stub = this.CreateStubInDomain(targetDomain);
                            //stub.SendDebugEvent +=new DebugDelegate(debug);
                        }
                        debug("Scanning " + str2 + " for plugins...");
                        FileInfo info2 = new FileInfo(str2);//(Path.Combine(this.PluginDirectory, str2));
                        FilePluginList list2 = new FilePluginList
                        {
                            Tokens = stub.ScanFile(str2),
                            Filename = str2,
                            FileSize = info2.Length,
                            LastModifiedDate = info2.LastWriteTime
                        };
                        dictionary[str2] = list2;
                        flag = true;
                    }
                }
            }
            finally
            {
                if (targetDomain != null)
                {
                    try
                    {
                        AppDomain.Unload(targetDomain);
                    }
                    catch (Exception exception)
                    {
                        //TraceHelper.DumpExceptionToTrace(exception);
                        debug(exception.ToString());
                    }
                }
            }
            if (flag)
            {
                this._tokens = dictionary;
                this.SaveCache();
            }
        }

        // Properties
        public string CacheFilename { get; private set; }

        private string PluginDirectory
        {
            get
            {
                //debug("plugins path:" + PluginHelper.pluginpath());
                return PluginGlobals.PluginDirectory;
            }
        }

        // Nested Types
        public class FilePluginList
        {
            // Methods
            public FilePluginList()
            {
                this.Tokens = new List<PluginToken>();
            }

            // Properties
            public string Filename { get; set; }

            public long FileSize { get; set; }

            public DateTime LastModifiedDate { get; set; }

            public List<PluginToken> Tokens { get; set; }
        }

        private sealed class InitializeAppDomainStub : MarshalByRefObject
        {
            // Methods
            public bool AppDomainIsInitialized()
            {
                return ((PluginGlobals.PluginDirectory != null) && (PluginGlobals.PluginManager != null));
            }

            public void InitializeAppDomain(string pluginDirectory, string pluginCacheFilename, string userAppDataPath)
            {
                PluginManager.LogLoadedAssemblies();
                PluginGlobals.PluginDirectory = pluginDirectory;
                PluginGlobals.PluginManager = new PluginManager(pluginCacheFilename);
                //CommonGlobals.UserAppDataPath = userAppDataPath;
            }
        }

        public sealed class PluginCache
        {
            // Properties
            public string PluginDirectory { get; set; }

            public SerializableDictionary<string, PluginManager.FilePluginList> Plugins { get; set; }
        }

        private sealed class PluginManagerStub : MarshalByRefObject
        {
            /*
            public event DebugDelegate SendDebugEvent;
            void debug(string msg)
            {
                if (SendDebugEvent != null)
                    SendDebugEvent(msg);
            }**/
            // Methods
            public PluginReference CreatePlugin(PluginSettings settings)
            {
                object obj2;
                //MessageBox.Show("pluginpath:" + this.PluginDirectory);
                string dllpath = PluginHelper.GetFullFileName(Path.Combine(this.PluginDirectory, settings.PluginToken.AssemblyFilename));
                string dllpath2 = PluginHelper.GetFullFileName(settings.PluginToken.AssemblyFilename);//有些组件是在quant.dll中/并不在plugins插件文件夹

                string filename = string.Empty;
                if (File.Exists(dllpath))
                    filename = dllpath;
                else if (File.Exists(dllpath2))
                    filename = dllpath2;
                //MessageBox.Show("dllpath:" + dllpath + " fillname:" + settings.PluginToken.AssemblyFilename);
               // MessageBox.Show("dllpath2:" + dllpath + " fillname:" + settings.PluginToken.AssemblyFilename);
               // MessageBox.Show("FileName:" + filename + "type full name:" + settings.PluginToken.TypeFullName);

                Type c = Assembly.LoadFile(filename).GetType(settings.PluginToken.TypeFullName);
                //Type c = Assembly.Load(AssemblyName.GetAssemblyName(Path.Combine(this.PluginDirectory, settings.PluginToken.AssemblyFilename))).GetType(settings.PluginToken.TypeFullName);
                //if (typeof(IBarDataStorage).IsAssignableFrom(c))
                //{
                    //IBarDataStorage storage = (IBarDataStorage)Activator.CreateInstance(c);
                    //OldDataStoreWrapper wrapper = new OldDataStoreWrapper
                    //{
                    //    OldStore = storage
                    //};
                    //obj2 = wrapper;
                //}
                //else 
                if (settings.PluginToken.RequiresConstructorArguments)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<ConstructorArgument>));
                    List<ConstructorArgument> args = (List<ConstructorArgument>)serializer.Deserialize(new StringReader(settings.PluginXml));
                    object[] objArray = PluginFinder.CreateConstructorArgs(args, c);
                    obj2 = Activator.CreateInstance(c, objArray);
                }
                else if (string.IsNullOrEmpty(settings.PluginXml))
                {
                    obj2 = Activator.CreateInstance(c);
                }
                else
                {
                    obj2 = new XmlSerializer(c).Deserialize(new StringReader(settings.PluginXml));
                }
                return new PluginReference(obj2);
            }

            private static PluginToken CreatePluginToken(string filename, Type pluginType, Type interfaceType)
            {
                return PluginToken.Create(pluginType, interfaceType);
            }

            public List<PluginToken> ScanFile(string filename)
            {
                //debug("stub scanfile:" + filename.ToString());
                //MessageBox.Show("stub scanfile:" + filename.ToString());
                List<PluginToken> list = new List<PluginToken>();
                try
                {
                    //不加载依赖程序集
                    //foreach (Type type in Assembly.Load(AssemblyName.GetAssemblyName(Path.Combine(this.PluginDirectory, filename)).FullName).GetExportedTypes())
                    //MessageBox.Show("base path:" + PluginHelper.GetFullFileName(filename));
                    //MessageBox.Show("pluginpath000000000:" + filename + " assembly:" + Assembly.LoadFile(filename).FullName);

                    foreach (Type type in Assembly.LoadFile(PluginHelper.GetFullFileName(filename)).GetExportedTypes())
                    {
                        
                        if (!type.IsAbstract)
                        {
                            //MessageBox.Show("type:" + type.ToString() + " is idatasore:" + typeof(IDataStore).IsAssignableFrom(type).ToString()+ " get interface:" + (type.GetInterface(typeof(IDataStore).Name)!=null).ToString());
                            PluginToken item = null;
                            //频率插件
                            if (typeof(FrequencyPlugin).IsAssignableFrom(type))
                            {
                                item = CreatePluginToken(filename, type, typeof(FrequencyPlugin));
                            }
                            //数据读写插件
                            else if (typeof(IDataStore).IsAssignableFrom(type) || typeof(IBarDataStorage).IsAssignableFrom(type))
                            //else if (type.GetInterface(typeof(IDataStore).Name) != null)//查看该类型是否实现了我们需要的接口
                            {
                                //MessageBox.Show("find datastore:" + type.ToString());
                                item = CreatePluginToken(filename, type, typeof(IDataStore));
                            }
                            else if (typeof(OptimizationPlugin).IsAssignableFrom(type))
                            {
                                item = CreatePluginToken(filename, type, typeof(OptimizationPlugin));
                            }
                            if (item != null)
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException exception)
                {
                    string message = exception.Message;
                }
                catch (BadImageFormatException exception2)
                {
                    string text2 = exception2.Message;
                }
                catch (Exception)
                {
                }
                return list;
            }

            // Properties
            public string PluginDirectory
            {
                get
                {
                    return PluginGlobals.PluginDirectory;
                }
            }
        }
    }


}
