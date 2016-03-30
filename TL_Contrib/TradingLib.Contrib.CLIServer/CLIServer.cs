using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.CLI
{
    [ContribAttr(CLIServer.ContribName,"命令行服务","命令行服务用于提供Telent接入的命令行管理交互,管理员通过命令行可以进行服务端的管理,查看,以及调试等工作")]
    public class CLIServer:BaseSrvObject,IContrib
    {
        const string ContribName = "CLIServer";
        TelnetServer _telentserver;
        ConfigDB _cfgdb;

        int _port = 8888;
        public CLIServer()
            : base(CLIServer.ContribName)
        {
            
            
        }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            logger.Info("CLIServer Loading....");
            _cfgdb = new ConfigDB(CLIServer.ContribName);
            if (!_cfgdb.HaveConfig("cmdport"))
            {
                _cfgdb.UpdateConfig("cmdport", QSEnumCfgType.Int, 8888, "Telnet命令行端口");
            }


            _port = _cfgdb["cmdport"].AsInt();
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            base.Dispose();
            _telentserver.Dispose();
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            logger.Info("启动命令行服务");
            _telentserver = new TelnetServer(_port, null);
            _telentserver.Start();

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }

        [ContribCommandAttr(QSEnumCommandSource.CLI,"echo","echo - just check clicommand is working","")]
        public string Echo()
        {
            return "CLI Command Working";
        }

        [ContribCommandAttr(
            QSEnumCommandSource.CLI, //来源命令行
            "help", //操作码
            "help - list  cli command lsit", //帮助
            "")]
        public string DemoCLI()
        {
            return TLCtxHelper.Ctx.PrintCLICommandList();
        }





        [ContribCommandAttr(QSEnumCommandSource.CLI, "cmdlist", "cmdlist - list all the contrib command list", "")]
        public string CmdList()
        {
            return TLCtxHelper.Ctx.PrintCommandList();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "eventlist", "eventlist - list all the event list", "")]
        public string EventList()
        {
            return TLCtxHelper.Ctx.PrintEventList();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "handlerlist", "handlerlist - list all the event handler", "")]
        public string EventHandlerList()
        {
            return TLCtxHelper.Ctx.PrintEventHandler();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "api", "api [contrib] [cmd] - print the api doc for the contrib cmd", "")]
        public string API(string contrib, string cmd)
        {
            return TLCtxHelper.Ctx.PrintCommandAPI(contrib, cmd);
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "print", "print [srvobj/core/contrib] - print infomation ", "")]
        public string Print(string section)
        {
            if (section.Equals("srvobj"))
            {
                return TLCtxHelper.Ctx.PrintBaseObjectList();
            }
            if (section.Equals("contrib"))
            {
                return TLCtxHelper.Ctx.PrintContribList();
            }
            if (section.Equals("core"))
            {
                return TLCtxHelper.Ctx.PrintCoreList();
            }
            return null;
        }



        [ContribCommandAttr(QSEnumCommandSource.CLI, "pcontrib", "pcontrib [contribId] - print contrib information", "")]
        public string PrintContrib(string contrib)
        {
            Util.Debug(" run cli command:pcontrib");
            return TLCtxHelper.Ctx.PrintContrib(contrib);
        }

        //[ContribCommandAttr(QSEnumCommandSource.CLI, "ptask", "ptask - print task list", "")]
        //public string PrintTask()
        //{
        //    return TLCtxHelper.Ctx.PrintTask();
        //}

        [ContribCommandAttr(QSEnumCommandSource.CLI, "profile", "profile - print profile information", "")]
        public static string Profile()
        {
            return TLCtxHelper.Profiler.GetStatsString();
        }
        [ContribCommandAttr(QSEnumCommandSource.CLI, "info", "info[all/core/contrib] - print the basic information for system", "")]
        public string Info(string section)
        {
            StringBuilder sb = new StringBuilder();
            if (section.Equals("all"))
            {
                sb.Append((CliUtils.SECPRIFX + " Welcome To TL TradingSystem ").PadRight(CliUtils.SECNUM, CliUtils.SECCHAR) + System.Environment.NewLine);
                sb.Append("Version 1.0.0" + System.Environment.NewLine);
            }
            else if (section.Equals("core"))
            {
                sb.Append("There are some core components in  system:" + System.Environment.NewLine);
                sb.Append("##CoreMsgRouter:" + System.Environment.NewLine);
                sb.Append("  this message router is used for message exchagne,it will exhange message betwoeen Client and System Components" + System.Environment.NewLine);
                sb.Append("such as BrokerRouter/DataFeedRouter/ClearCentre/RiskCentre and many other contrib components." + System.Environment.NewLine);
                sb.Append("##MgrMsgRouter:" + System.Environment.NewLine);
                sb.Append("  this message router is used for manager message exchange,manager endpoint will communicate with it.via that router" + System.Environment.NewLine);
                sb.Append("we can add account/view account information/and do many sutff as we support in our system." + System.Environment.NewLine);
                sb.Append("##ClearCentre:" + System.Environment.NewLine);
                sb.Append(" ClearCentre hold all the trading information for account,it support account operation/deal with database/close open market/" + System.Environment.NewLine);
                sb.Append("settlement/ and many trading staff" + System.Environment.NewLine);
                sb.Append("##RiskCentre" + System.Environment.NewLine);
                sb.Append(" riskcentre do the risk control of trading,such as order check/account check/riskrule setting/server side loss proft strategy" + System.Environment.NewLine);
                sb.Append("##DataFeedRouter" + System.Environment.NewLine);
                sb.Append("##BrokerRouter" + System.Environment.NewLine);
                sb.Append("##TaskCentre" + System.Environment.NewLine);
                sb.Append("##ReportCentre" + System.Environment.NewLine);
                sb.Append(System.Environment.NewLine);
                sb.Append("now we try to make core much more 'small' and stable,we use contrib to expand the function of system" + System.Environment.NewLine);
            }
            return sb.ToString();
        }

    }
}
