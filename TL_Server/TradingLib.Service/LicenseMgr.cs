using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using License;

namespace TradingLib.ServiceManager
{
    /*
     * 系统通过识别授权文件的版本类别从而在启动的时候设定相应功能
     * 1.版本类别 版本类别是描述软件系统主体功能的,目前系统可以分为核心功能,扩展功能,与附加功能
     * 核心功能(交易模块,风控模块,清算模块,管理模块)这3个模块构成了系统的核心,代表了一个基本的分账户交易系统的底层
     * 扩展功能(配资模块,比赛模块,)是在核心功能基础上构建的应用模块,可以实现配资管理,比赛管理等
     * 辅助功能(信息中心,web管理模块)主要是扩展了系统的功能与web集成 实现用户注册,记录查询等
     * 
     * 在以上基础上我们整理出如下版本
     * A:分账户版       包含核心分账户功能
     * AN:分账户网络版   包含核心分账户功能与网站注册与查询功能 
     * B:配资版         包含配资功能
     * BN:配资网络版
     * 
     * 2.核心功能限制
     * a.交易并发 同时可以在线的并发数
     * b.
     * 
     * 版本标识:
     * A-20
     * */
    /// <summary>
    /// 授权许可管理
    /// 通过读取授权许可,然后获得对应的模块参数,将模块参数导入
    /// lib,core等组件的全局参数
    /// 系统通过相关全局参数进行运行
    /// </summary>
    public class LicenseMgr
    {

        Dictionary<string, string> licvalue = new Dictionary<string, string>();

        ModuleEnable moduleswitch = null;

        public ModuleEnable ModuleEnable { get { return moduleswitch; } }

        public LicenseMgr()
        {

            //从授权文件获得软件版本以及相关运行参数
            /* Check first if a valid license file is found */
            if (License.Status.Licensed)
            {
                /* Read additional license information */
                for (int i = 0; i < License.Status.KeyValueList.Count; i++)
                {
                    string key = License.Status.KeyValueList.GetKey(i).ToString();
                    string value = License.Status.KeyValueList.GetByIndex(i).ToString();
                    licvalue[key] = value;
                }
            }
            //通过版本代码来生成附加模块的激活标识
            switch (VerCode)
            {
                //基本分账户版
                case "A":
                    moduleswitch = new ModuleEnable(false, false, false, false);
                    break;
                case "AN":
                    moduleswitch = new ModuleEnable(false, true, true, false);
                    break;
                case "ANA":
                    moduleswitch = new ModuleEnable(false, true, true, false);
                    _enableaccess = true;//允许前置接入
                    break;
                //配资版
                case "B":
                    moduleswitch = new ModuleEnable(true, false, false, false);
                    break;
                case "BN":
                    moduleswitch = new ModuleEnable(true, true, true, false);
                    break;
                case "BNA":
                    moduleswitch = new ModuleEnable(true, true, true, false);
                    _enableaccess = true;//允许前置接入
                    break;
                case "DEV":
                    moduleswitch = new ModuleEnable(true, true, true, true);
                    _enableaccess = true;//允许前置接入
                    break;
                default:
                    moduleswitch = new ModuleEnable(false, false, false, false);
                    break;
            }
        }

        /// <summary>
        /// 实盘与模拟单独加载
        /// </summary>
        public bool EnableLoadMode
        {
            get
            {
#if DEV
                return true;
#else
                if (licvalue.Keys.Contains("LoadMode"))
                {
                    return licvalue["LoadMode"].Equals("1");
                }
                else
                {
                    return false;
                }
#endif
            }
        }
        bool _enableaccess = false;
        /// <summary>
        /// 是否允许前置接入
        /// </summary>
        public bool EnableAccess { get { return _enableaccess; } }
        /// <summary>
        /// 软件版本码,版本码决定软件运行时加载的模块
        /// </summary>
        public string VerCode {
            get
            {
#if DEV
                return "DEV";
#else
                if (licvalue.Keys.Contains("VerCode"))
                    return licvalue["VerCode"];
                else
                    return "A";
#endif
            }
        
        }

        /// <summary>
        /// 版本代码对应的版本名称
        /// </summary>
        public string VerName
        {
            get
            {
                switch (VerCode)
                { 
                    case "A":
                        return "分账户版[A]";
                    case "AN":
                        return "分账户版[AN]";
                    case "ANA":
                        return "分账户版[ANA]";
                    case "B":
                        return "配资版[B]";
                    case "BN":
                        return "配资版[BN]";
                    case "BNA":
                        return "配资版[BNA]";
                    case "DEV":
                        return "开发版本[DEV]";
                    default:
                        return "分账户版[X]";
                }
            }
        }
        /// <summary>
        /// 授权给某个公司或个人
        /// </summary>
        public string Company
        {
            get
            {
#if DEV
                return "futsystems";
#else
                if (licvalue.Keys.Contains("Company"))
                    return licvalue["Company"];
                else
                    return "测试用户";
#endif
            }
        }
        /// <summary>
        /// 软件到期日
        /// </summary>
        public DateTime ExpDate
        {
            get 
            {
                if (License.Status.Expiration_Date_Lock_Enable)
                    return License.Status.Expiration_Date;
                else
                    return new DateTime(2100, 1, 1);
            }
        }

        public string HardID
        {
            get
            {
                return License.Status.HardwareID;
            }
        }

        public bool HardLock
        {
            get
            {
                return License.Status.Hardware_Lock_Enabled;
            }
        }
        /// <summary>
        /// 系统支持的在线并发数,用于设定可以支持多少交易账户同时在线交易
        /// </summary>
        public int ConcurrentUser
        {
            get
            {
#if DEV
                return 10000;
#else
                if (licvalue.Keys.Contains("ConcurrentUser"))
                {
                    try
                    {
                        int c = Convert.ToInt16(licvalue["ConcurrentUser"]);
                        return c;
                    }
                    catch (Exception ex)
                    {
                        return 5;
                    }
                }
                return 5;
#endif
            }
        }

    }
    /// <summary>
    /// 模块运行标志,指示某个模块是否开启
    /// </summary>
    public class ModuleEnable
    {
        public ModuleEnable(bool enfin, bool enwebmgr, bool eninfo, bool enrace)
        {
            this.Enable_FinService = enfin;
            this.Enable_WebMgr = enwebmgr;
            this.Enable_InfoCentre = eninfo;
            this.Enable_Race = enrace;
        }
        public bool Enable_Race { get; private set; }//比赛模块
        public bool Enable_FinService { get; private set; }//配资本模块
        public bool Enable_WebMgr { get; private set; }//在线管理模块
        public bool Enable_InfoCentre { get; private set; }//信息中心模块

    }

    /// <summary>
    /// 所有模块的运行状态
    /// </summary>
    public interface IModuleRunStatus
    {
        
        bool Run_TradingSrv { get; }
        bool Run_RiskCentre { get; }
        bool Run_ClearCentre { get;}
        bool Run_Managerment { get;}

        bool Run_RaceCentre { get;}
        bool Run_FinService { get;}

        bool Run_Information { get; }
        bool Run_WebRep { get;}
    
    }
}
