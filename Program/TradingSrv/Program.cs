using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Threading;
using Common.Logging;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;
using TradingLib.ORM;
using Autofac;
using Autofac.Configuration;
using System.Security;
using System.Security.Cryptography;

using System.Diagnostics;



namespace TraddingSrvCLI
{
    class Program
    {
        
        const string PROGRAME = "LogicSrv";
        static ILog logger = LogManager.GetLogger(PROGRAME);

        //将字符串经过md5加密，返回加密后的字符串的小写表示
        public static string Md5Encrypt(string strToBeEncrypt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strToBeEncrypt);
            Byte[] TargetData = md5.ComputeHash(FromData);
            string Byte2String = "";
            for (int i = 0; i < TargetData.Length; i++)
            {
                Byte2String += TargetData[i].ToString("x2");
            }
            return Byte2String.ToLower();
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                LoadLicenseConfig();

                FileVersionInfo info = FileVersionInfo.GetVersionInfo("LogicSrv.exe");
                string version = info.FileMajorPart + "." + info.FileMinorPart + "." + info.FileBuildPart;

                

                logger.Status("Database", "INIT");
                //读取配置文件 初始化数据库参数 系统其余设置均从数据库中加载

                if (License.Status.Licensed)
                {
                    DBHelper.InitDBConfig(LicenseConfig.Instance.DBHost, 3306,LicenseConfig.Instance.DBName,LicenseConfig.Instance.DBUser,LicenseConfig.Instance.DBPass);
                }
                else
                {
                    ConfigFile _configFile = ConfigFile.GetConfigFile();
                    DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());
                }
                MSystem.UpdateVersion(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart);
                MSystem.UpdateDeployID(LicenseConfig.Instance.Deploy);


                //加载配置文件并生成容器
                var builder = new ContainerBuilder();
                builder.RegisterModule(new ConfigurationSettingsReader(TLCtxHelper.Version.ProductType.ToString(), Util.GetConfigFile("autofac.xml")));

                using (var container = builder.Build())
                {
                    using (var scope = container.BeginLifetimeScope())
                    {
                        //注册全局scope 用于动态生成组件
                        TLCtxHelper.RegisterScope(scope);
                        //update domain expire date information
                        BasicTracker.DomainTracker.UpdateLicenseExpire(LicenseConfig.Instance.Expire);
                        //update account limit
                        BasicTracker.DomainTracker.UpdateLicenseAccountLimit(LicenseConfig.Instance.AccountCNT);
                        //update agent limit
                        BasicTracker.DomainTracker.UpdateLicenseAgentLimit(LicenseConfig.Instance.AgentCNT);


                        using (var coreMgr = scope.Resolve<ICoreManager>())//1.核心模块管理器,加载核心服务组件
                        {
                            coreMgr.Init();
                            using (var connectorMgr = scope.Resolve<IConnectorManager>())//2.路由管理器,绑定核心部分的数据与成交路由,并加载Connector
                            {
                                connectorMgr.Init();

                                ////////////////////////////////// Stat Section
                                //0.启动扩展服务
                                //contribMgr.Start();

                                //1.待所有服务器启动完毕后 启动核心服务
                                coreMgr.Start();

                                //3.绑定扩展模块调用事件
                                TLCtxHelper.BindContribEvent();

                                //启动连接管理器 启动通道
                                connectorMgr.Start();

                                //最后确认主备机服务状态，并启用全局状态标识，所有的消息接收需要该标识打开,否则不接受任何操作类的消息
                                TLCtxHelper.IsReady = true;

                                TLCtxHelper.PrintVersion();

                                while (true)
                                {
                                    Thread.Sleep(1000);
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error:" + ex.ToString());
            }
        }


        static void LoadLicenseConfig()
        {
            logger.Info(string.Format("License status:{0} hardware:{1} expire:{2}", License.Status.Licensed, License.Status.License_HardwareID, License.Status.Expiration_Date));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < License.Status.KeyValueList.Count; i++)
            {
                string key = License.Status.KeyValueList.GetKey(i).ToString();
                string value = License.Status.KeyValueList.GetByIndex(i).ToString();
                dict.Add(key, value);
            }

            string tmp = "";
            if (dict.TryGetValue("deploy", out tmp))
            {
                LicenseConfig.Instance.Deploy = tmp;
            }
            if (dict.TryGetValue("cnt_counter", out tmp))
            {
                LicenseConfig.Instance.DomainCNT = int.Parse(tmp);
            }
            if (dict.TryGetValue("cnt_account", out tmp))
            {
                LicenseConfig.Instance.AccountCNT = int.Parse(tmp);
            }
            if (dict.TryGetValue("cnt_agent", out tmp))
            {
                LicenseConfig.Instance.AgentCNT = int.Parse(tmp);
            }
            if (dict.TryGetValue("enable_api", out tmp))
            {
                LicenseConfig.Instance.EnableAPI = tmp == "1" ? true : false;
            }
            if (dict.TryGetValue("enable_app", out tmp))
            {
                LicenseConfig.Instance.EnableAPP = tmp == "1" ? true : false;
            }
            if (dict.TryGetValue("expire", out tmp))
            {
                LicenseConfig.Instance.Expire = Util.ToDateTime(int.Parse(tmp), 0);
            }
            if (dict.TryGetValue("db_host", out tmp))
            {
                LicenseConfig.Instance.DBHost = tmp;
            }
            if (dict.TryGetValue("db_name", out tmp))
            {
                LicenseConfig.Instance.DBName = tmp;
            }
            if (dict.TryGetValue("db_user", out tmp))
            {
                LicenseConfig.Instance.DBUser = tmp;
            }
            if (dict.TryGetValue("db_pass", out tmp))
            {
                LicenseConfig.Instance.DBPass = tmp;
            }



        }
 
       
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            logger.Error("UnhandledException:" + ex.ToString());
        }
    }

}
