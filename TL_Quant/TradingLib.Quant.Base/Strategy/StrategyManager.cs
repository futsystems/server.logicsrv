using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;

namespace TradingLib.Quant.Base
{
    public class StrategyManager
    {
        public event StrategySetupDel AddStrategySetupEvent;

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        StrategyLoader _strategyloader;
        string _appdatapath;
        string _strategypath;
        bool _explicitload = false;
        SynchronizationContext _currentContext;

        List<StrategySetup> _strategylist;

        public StrategyManager(SynchronizationContext synchContext, string appDataPath, string strategyPath, bool explicitLoad)
        {
            _currentContext = synchContext;
            _appdatapath = BaseGlobals.AppDataDirectory;
            _strategypath = BaseGlobals.StrategyDirectory;
            _explicitload = explicitLoad;

            if (!string.IsNullOrEmpty(strategyPath))
                _appdatapath = appDataPath;
            if (!string.IsNullOrEmpty(strategyPath))
                _strategypath = strategyPath;

            _strategyloader = new StrategyLoader(synchContext, _strategypath, _explicitload);

            _strategylist = new List<StrategySetup>();

            if (!explicitLoad)
            {
                this.LoadStrategyProjects();
            }
            
        }

        /// <summary>
        /// 获得所有加载的策略工程列表
        /// </summary>
        /// <returns></returns>
        public List<StrategySetup> GetLoadedStrategyProject()
        {
            return _strategylist;
        }

        string errorText=string.Empty;
        public bool AddStrategyProject(StrategySetup newStrategy)
        {
            //策略检查
            if (string.IsNullOrEmpty(newStrategy.FriendlyName))
            {
                this.errorText = "策略工程名不能为空或者null";
                return false;
            }
            if (!_strategyloader.IsExistStrategy(newStrategy.StrategyClassName))
            {
                this.errorText = "策略类不存在";
                return false;
            }

            foreach(StrategySetup s in _strategylist)
            {
                if (s.FriendlyName == newStrategy.FriendlyName)
                {
                    this.errorText = "已经存在同名策略实例,请用唯一的名称标示";
                    return false;
                }
            }
            List<StrategySetup> newstrategylist = new List<StrategySetup>(_strategylist);
            newstrategylist.Add(newStrategy);
            bool re = this.SaveStrategyProjects(newstrategylist);
            if(re)
            {
                try
                {
                    //if (AddStrategySetupEvent != null)
                    //    AddStrategySetupEvent(newStrategy);
                }
                catch (Exception ex)
                {
                    this.errorText = ex.ToString();
                }
                
            }
            return re;
        }

        /// <summary>
        /// 删除所有策略工程
        /// </summary>
        /// <returns></returns>
        public bool ClearStrategyProject()
        {
            List<StrategySetup> newServices = new List<StrategySetup>();
            return this.SaveStrategyProjects(newServices);
        }

        /// <summary>
        /// 删除某个策略工程
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        public bool RemoveStrategyProject(string friendlyName)
        {
            this.errorText = "";
            StrategySetup item = this.FindStrategy(friendlyName);
            if (item == null)
            {
                this.errorText = "A strategy with the specified name does not exist.";
                return false;
            }
            List<StrategySetup> newServices = new List<StrategySetup>(this._strategylist);
            newServices.Remove(item);
            return this.SaveStrategyProjects(newServices);
        }

        /// <summary>
        /// 查找某个服务
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        private StrategySetup FindStrategy(string friendlyName)
        {
            foreach (StrategySetup setup in this._strategylist)
            {
                if (setup.FriendlyName == friendlyName)
                {
                    return setup;
                }
            }
            return null;
        }

        public StrategyInfo GetStrategyInfo(string friendlyName)
        {
            StrategySetup ss = this.GetStrategySetup(friendlyName);
            if (ss != null)
            {
                return _strategyloader.GetStrategyInfo(ss.StrategyClassName);
            }
            return null;
        }
        public StrategySetup GetStrategySetup(string friendlyName)
        {
            this.errorText = "";
            StrategySetup setup = this.FindStrategy(friendlyName);
            if (setup == null)
            {
                this.errorText = "A strategy with the specified name does not exist.";
                return null;
            }
            //检查是否可以加载该类？
            if (!string.IsNullOrEmpty(setup.StrategyClassName))
            {
                StrategyInfo serviceInfo = this._strategyloader.GetStrategyInfo(setup.StrategyClassName);
                if (serviceInfo == null)
                {
                    this.errorText = "Unable to load strategy info for this strategyproject: " + setup.StrategyClassName;
                    return null;
                }
                else
                    return setup;
            }
            else
            {
                return null;
            }
        }

        bool _loadSuccess = false;
        /// <summary>
        /// 加载所有服务
        /// </summary>
        private void LoadStrategyProjects()
        {
            this.errorText = "";
            this._loadSuccess = false;
            TextReader textReader = null;
            try
            {
                string path = Path.Combine(this._appdatapath, BaseGlobals.StrategyConfigFileName);
                if (!File.Exists(path))
                {
                    StreamWriter writer = File.CreateText(path);
                    //writer.Write(ServiceGlobals.DefaultServiceSetup);
                    //writer.Close();
                }
                textReader = new StreamReader(path);
                XmlSerializer serializer = new XmlSerializer(typeof(List<StrategySetup>));
                this._strategylist = (List<StrategySetup>)serializer.Deserialize(textReader);
                //this.UpgradeServices();
                this._loadSuccess = true;
            }
            catch (FileNotFoundException exception)
            {
                //Trace.WriteLine("Service config file not found: " + exception.Message);
                this._strategylist.Clear();
                this._loadSuccess = true;
            }
            catch (Exception exception2)
            {
                this._loadSuccess = false;
                this._strategylist.Clear();
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
                this._strategyloader.RefreshStrategys();
            }
            this.LoadStrategyProjects();
            return this._loadSuccess;
        }
        /// <summary>
        /// 保存策略安装配置
        /// </summary>
        /// <param name="newServices"></param>
        /// <returns></returns>
        private bool SaveStrategyProjects(List<StrategySetup> newStrategys)
        {
            TextWriter textWriter = null;
            
            try
            {
                string file = Path.Combine(this._appdatapath, "StrategySettings.xml");
                textWriter = new StreamWriter(file);
                debug("保存策略配置到文件:" + file);
                new XmlSerializer(typeof(List<StrategySetup>)).Serialize(textWriter, newStrategys);
            }
            catch (Exception exception)
            {
                //TraceHelper.DumpExceptionToTrace(exception);
                //Trace.WriteLine("Unable to save service list.");
                this.errorText = exception.Message;
                debug(this.errorText);
                return false;
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
            this._strategylist = newStrategys;
            return true;
        }
        public string ErrorText
        {
            get
            {
                return this.errorText;
            }
        }

    }
}
