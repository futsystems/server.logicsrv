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


               

                logger.Info("********* start core daemon *********");
                System.OperatingSystem osInfo = System.Environment.OSVersion;
                System.PlatformID platformID = osInfo.Platform;
                Console.WriteLine(platformID.ToString());

                Util.StatusSection("Database", "INIT", QSEnumInfoColor.INFOGREEN, true);
                //读取配置文件 初始化数据库参数 系统其余设置均从数据库中加载
                ConfigFile _configFile = ConfigFile.GetConfigFile();
                DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());
                
                //加载配置文件并生成容器
                var builder = new ContainerBuilder();
                builder.RegisterModule(new ConfigurationSettingsReader(TLCtxHelper.Version.ProductType.ToString(), Util.GetConfigFile("autofac.xml")));

                using (var container = builder.Build())
                {
                    using (var scope = container.BeginLifetimeScope())
                    {
                        //注册全局scope 用于动态生成组件
                        TLCtxHelper.RegisterScope(scope);

                        using (var coreMgr = scope.Resolve<ICoreManager>())//1.核心模块管理器,加载核心服务组件
                        {
                            coreMgr.Init();
                            using (var connectorMgr = scope.Resolve<IConnectorManager>())//2.路由管理器,绑定核心部分的数据与成交路由,并加载Connector
                            {
                                connectorMgr.Init();
                                using (var contribMgr = scope.Resolve<IContribManager>())//3.扩展模块管理器 加载扩展模块,启动扩展模块
                                {
                                    contribMgr.Init();
                                    contribMgr.Load();

                                    ////////////////////////////////// Stat Section
                                    //0.启动扩展服务
                                    contribMgr.Start();

                                    //1.待所有服务器启动完毕后 启动核心服务
                                    coreMgr.Start();

                                    //3.绑定扩展模块调用事件
                                    TLCtxHelper.BindContribEvent();

                                    //启动连接管理器 启动通道
                                    connectorMgr.Start();

                                    //解析版本信息
                                    TLCtxHelper.ParseVersion();
                                    //最后确认主备机服务状态，并启用全局状态标识，所有的消息接收需要该标识打开,否则不接受任何操作类的消息
                                    TLCtxHelper.IsReady = true;

                                    TLCtxHelper.PrintVersion();

                                    //TLCtxHelper.Worker.StartWorker();
                                    while (true)
                                    {
                                        Thread.Sleep(1000);
                                    }
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

 
       
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            logger.Error("UnhandledException:" + ex.ToString());
        }
    }

}
