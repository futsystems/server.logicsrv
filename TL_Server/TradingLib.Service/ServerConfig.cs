using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Core;

namespace TradingLib.ServiceManager
{
    public enum RunType
    { 
        RUN,
        DEV
    }
    /// <summary>
    /// 服务配置
    /// </summary>
    public class ServerConfig
    {
        LicenseMgr licmgr;
        ConfigFile _configFile;
        QSEnumAccountLoadMode _loadMode = QSEnumAccountLoadMode.ALL;
        public QSEnumAccountLoadMode LoadMode { get { return _loadMode; } }
        RunType _runType = RunType.DEV;
        public RunType RunType { get { return _runType; } }


        //#region 配置信息的读写
        ///// <summary>
        ///// 保存设置到配置文件
        ///// </summary>
        //public void Save()
        //{
        //    _configFile.Save();
        //}

        ///// <summary>
        ///// 将ConfigFile的索引传递出去
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public CfgValue this[string key]
        //{
        //    get { return _configFile[key]; }
        //}

        ///// <summary>
        ///// 设置对应的配置参数
        ///// </summary>
        ///// <param name="key">参数名称</param>
        ///// <param name="value">参数值</param>
        //public void Set(string key, string value)
        //{
        //    _configFile.Set(key, value);
        //}
        //#endregion 

        //用于设点端口前缀
        int PortPrefix = 10000;
        DebugDelegate _debugfunc;
        void debug(string msg)
        {
            if (_debugfunc != null)
                _debugfunc(msg);
        }
        public ServerConfig(DebugDelegate debugfunc)
        {
            _debugfunc = debugfunc;
            try
            {
                licmgr = new LicenseMgr();

                //加载系统配置文件
                _configFile = ConfigFile.GetConfigFile();

                //设定数据库
                DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());
                
                //设定帐户类型
                AccountHelper.SetAccountType(_configFile["AccountType"].AsString());


                _loadMode = (QSEnumAccountLoadMode)Enum.Parse(typeof(QSEnumAccountLoadMode), _configFile["LoadMode"].AsString());
                _runType = (RunType)Enum.Parse(typeof(RunType), _configFile["RunType"].AsString());
                switch (_loadMode)
                {
                    //实盘与模拟运行在一个进程内,则端口为默认端口 5001，5000，5570，6670
                    case QSEnumAccountLoadMode.ALL:
                        PortPrefix = 0;
                        //LibGlobal.SimLimitReal = true;
                        break;
                    //单独运行实盘 15001，15000，15570，16670
                    case QSEnumAccountLoadMode.REAL:
                        PortPrefix = 10000;
                        //LibGlobal.SimLimitReal = true;
                        break;
                    //单独运行模拟 25001,25000,25570,26670
                    case QSEnumAccountLoadMode.SIM:
                        PortPrefix = 20000;
                        break;
                }
                //初始化lib全局参数
                LibGlobal.InitCTPConfig(_configFile["MarketOrderOffset"].AsInt());



                //模拟成交引擎参数
                LibGlobal.InitSimBrokerConfig(_configFile["PPTUseBidAsk"].AsBool(), _configFile["FillTickByTick"].AsBool());

            }
            catch (Exception ex)
            {
                debug("Generate Server Config Error:" + ex.ToString());
            }
        }

    }
}
