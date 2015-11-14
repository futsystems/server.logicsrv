using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{

    /// <summary>
    /// 管理ServiceHost
    /// </summary>
    public partial class DataServer
    {

        private readonly string _ServiceHostFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceHost");
        private readonly List<IServiceHost> _serviceHosts = new List<IServiceHost>();
        
        /// <summary>
        /// 加载ServiceHost
        /// </summary>
        void LoadServiceHosts()
        {
            string[] aDLLs = null;

            try
            {
                aDLLs = Directory.GetFiles(_ServiceHostFolder, "*.dll");
            }
            catch (Exception ex)
            {
                logger.Error("Load ServiceHost Error:" + ex.ToString());
            }
            if (aDLLs.Length == 0)
                return;

            foreach (string item in aDLLs)
            {
                Assembly aDLL = Assembly.UnsafeLoadFrom(item);
                Type[] types = aDLL.GetTypes();

                foreach (Type type in types)
                {
                    try
                    {
                        
                        //connection service must support IDataServerServiceHost interface
                        if (type.GetInterface("TradingLib.DataFarm.API.IServiceHost") != null)
                        {
                            object o = Activator.CreateInstance(type);

                            if (o is DataFarm.API.IServiceHost)
                            {
                                _serviceHosts.Add(o as DataFarm.API.IServiceHost);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 启动ServiceHost
        /// </summary>
        void StartServiceHosts()
        {
            logger.Info("Start ServiceHosts....");
            foreach (var h in _serviceHosts)
            {
                StartServiceHost(h);
            }
        }

        /// <summary>
        /// 启动某个ServiceHost
        /// 绑定ServiceHost
        /// </summary>
        /// <param name="host"></param>
        void StartServiceHost(IServiceHost host)
        {
            host.SessionCreatedEvent += new Action<IServiceHost, IConnection>(OnSessionCreatedEvent);
            host.SessionClosedEvent += new Action<IServiceHost, IConnection>(OnSessionClosedEvent);
            host.RequestEvent += new Action<IServiceHost, IConnection, IPacket>(OnRequestEvent);    //(OnRequestEvent);
            host.ServiceEvent += new Func<IServiceHost, IPacket, IPacket>(OnServiceEvent);
            host.Start();
        }

        


        
    }
}
