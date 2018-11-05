using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;


namespace TradingLib.Contrib.APIService
{
    /// <summary>
    /// APIService
    /// 系统内核提供webmessage server该服务都外提供ReqRep方式的请求，请求形式是JsonRequest,返回形式是JsonResponse
    /// 核心部分不实现api接口调用，统一在APService扩展模块中实现，方便集中维护与管理
    /// 如果其他扩展模块需要对外提供API接口，则在对应的扩展模块中进行实现，这样降低了耦合，更好的实现了模块化
    /// </summary>
    public partial class APIServiceBundle : BaseSrvObject, IModuleAPIService
    {
        const string CoreName = "APIService";
        public string CoreId { get { return CoreName; } }
        HttpServer _httpServer = null;
        ConfigDB _cfgdb;
        string _md5key = "123456";
        int _port = 8080;
        string _address = "127.0.0.1";
        decimal _depositLimit = 50000;
        List<string> cfgSrvIPList = new List<string>();

        string _configAPIServer = "127.0.0.1";
        HttpProxy _httpProxy = null;
        int localAppPort = 80;
        public APIServiceBundle()
            : base(APIServiceBundle.CoreName)
        {
            //从数据库加载参数
            _cfgdb = new ConfigDB(APIServiceBundle.CoreName);
            if (!_cfgdb.HaveConfig("MD5Key"))
            {
                _cfgdb.UpdateConfig("MD5Key", QSEnumCfgType.String, "123456", "MD5Key");
            }
            _md5key = _cfgdb["MD5Key"].AsString();

            if (!_cfgdb.HaveConfig("HttpPort"))
            {
                _cfgdb.UpdateConfig("HttpPort", QSEnumCfgType.Int, "8080", "HttpPort");
            }
            _port = _cfgdb["HttpPort"].AsInt();


            if (!_cfgdb.HaveConfig("HttpAddress"))
            {
                _cfgdb.UpdateConfig("HttpAddress", QSEnumCfgType.String, "127.0.0.1", "HttpAddress");
            }
            _address = _cfgdb["HttpAddress"].AsString();

            if (!_cfgdb.HaveConfig("DepositLimit"))
            {
                _cfgdb.UpdateConfig("DepositLimit", QSEnumCfgType.Decimal, 50000, "单笔入金限额");
            }
            _depositLimit = _cfgdb["DepositLimit"].AsDecimal();

            if (!_cfgdb.HaveConfig("ConfigServer"))
            {
                _cfgdb.UpdateConfig("ConfigServer", QSEnumCfgType.String, "47.89.14.9", "ConfigServer");
            }
            if (!_cfgdb.HaveConfig("ConfigAPIServer"))
            {
                _cfgdb.UpdateConfig("ConfigAPIServer", QSEnumCfgType.String, "47.99.155.187", "ConfigAPIServer");
            }

            _configAPIServer = _cfgdb["ConfigAPIServer"].AsString();

            if (!_cfgdb.HaveConfig("LocalAppPort"))
            {
                _cfgdb.UpdateConfig("LocalAppPort", QSEnumCfgType.Int, 80, "LocalAppPort");
            }
            localAppPort = _cfgdb["LocalAppPort"].AsInt();


            try
            {
                //add vpn address
                APIGlobal.ConfigServerIPList.Add("139.196.49.200");
                APIGlobal.ConfigServerIPList.Add(_cfgdb["ConfigServer"].AsString());

                logger.Info("cfg server list:" + string.Join(",", APIGlobal.ConfigServerIPList.ToArray()));
            }
            catch (Exception ex)
            {
                logger.Error("Get Config Server Address Error:" + ex.ToString());
            }

            APIGlobal.BaseUrl = string.Format("http://{0}:{1}", _address, _port);
            APIGlobal.LocalIPAddress = _address;

            
            


            TLCtxHelper.EventSystem.CashOperationProcess += new Func<CashOperationRequest, bool>(EventSystem_CashOperationProcess);
        
        

        }

        bool EventSystem_CashOperationProcess(CashOperationRequest arg)
        {

            var account = TLCtxHelper.ModuleAccountManager[arg.Account];
            if (account == null)
            {
                
                arg.ProcessComment = "交易账户不存在";
                return false;
            }

            var gateway = APITracker.GateWayTracker.GetDomainGateway(account.Domain.ID);
            if (gateway == null)
            {
                arg.ProcessComment = "未设置支付网关";
                return false;
            }

            if (!gateway.Avabile)
            {
                arg.ProcessComment  = "支付网关未启用";
                return false;
            }

            //出金
            if (arg.Amount < 0)
            {
                IEnumerable<CashOperation> pendingWithdraws = ORM.MCashOperation.SelectPendingCashOperation(account.ID).Where(c => c.OperationType == QSEnumCashOperation.WithDraw);
                if (pendingWithdraws.Count() > 0)
                {
                    arg.ProcessComment = "上次出金请求仍在处理中";
                    return false;
                }

                if (account.GetPositionsHold().Count() > 0 || account.GetPendingOrders().Count() > 0)
                {
                    arg.ProcessComment = "交易账户有持仓或挂单,无法执行出金";
                    return false;
                }

                var rate = account.GetExchangeRate(CurrencyType.RMB);//计算RMB汇率系数
                var amount = Math.Abs(arg.Amount) * rate;
                if (amount > account.NowEquity)
                {
                    arg.ProcessComment = "出金金额大于账户权益";
                    return false;
                }
            }

            CashOperation operation = null;
            //输入参数验证完毕
            operation = new CashOperation();
            operation.BusinessType = EnumBusinessType.Normal;
            operation.Account = account.ID;
            operation.Amount = Math.Abs(arg.Amount);
            operation.DateTime = Util.ToTLDateTime();
            operation.GateWayType = (QSEnumGateWayType)(-1);
            operation.OperationType = arg.Amount < 0 ? QSEnumCashOperation.WithDraw : QSEnumCashOperation.Deposit;
            operation.Ref = APITracker.NextRef;
            operation.Domain_ID = account.Domain.ID;

            ORM.MCashOperation.InsertCashOperation(operation);
            TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());

            arg.RefID = operation.Ref;
            return true;

        }


        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad() 
        {
            logger.Info("APIServiceBundle is loading ......");
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory() { }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start() 
        {
            logger.StatusStart(this.PROGRAME);
            _httpServer = new HttpServer(_address,_port);
            _httpServer.Start();

            _httpProxy = new HttpProxy(localAppPort, _configAPIServer);
            _httpProxy.Start();
           
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() 
        {
            logger.StatusStop(this.PROGRAME);
            _httpServer.Stop();
        
        }

    }
}
