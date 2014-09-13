using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;



namespace TradingLib.ServiceManager
{
    public partial class CoreManager
    {
        private void InitEmailServer()
        {
            debug("7.初始化MailServer");
            /*
            _emailsrv = new EmailServer(config.SMTPServer, config.EmailFrom, config.EmailUser, config.EmailPass, config.AdminEmail);
            //_emailsrv.SendDebugEvent += new DebugDelegate(mgrdebug);

            //核心组件
            _srv.SendEmailEvent += new EmailDel(_emailsrv.SendEmail);
            _clearCentre.SendEmailEvent += new EmailDel(_emailsrv.SendEmail);
            _riskCentre.SendEmailEvent += new EmailDel(_emailsrv.SendEmail);

            //功能组件发送邮件的事件绑定
            if (_racecentre != null)
                _racecentre.SendEmailEvent += new EmailDel(_emailsrv.SendEmail);

            if (_fincentre != null)
                _fincentre.SendEmailEvent += new EmailDel(_emailsrv.SendEmail);

            if (_webmanager != null)
                _webmanager.SendEmailEvent += new EmailDel(_emailsrv.SendEmail);
            if (_infosrv != null)
                _infosrv.SendEmailEvent += new EmailDel(_emailsrv.SendEmail);

            //_emailsrv.Start();**/
        }

        //初始化报告中心,报告中心负责采集各个组件的运行状态与数据
        private void InitReportCentre()
        {
            /*
            debug("8.初始化ReportCentre");
            _reportcentre = new ReportCentre(_srv, _clearCentre, qsmgrSrv, _fincentre, config.DBServer, config.DBUser, config.DBPass);
            //_reportcentre.SendDebugEvent += new DebugDelegate(debug);
            qsmgrSrv.BindReport(_reportcentre);//为管理中心绑定 报告中心

            //将全局日志记录系统绑定到report对应的函数
            //LibUtil.SendLogEvent += new LogDelegate(_reportcentre.NewLog);

            //推送服务器健康状态数据
            _reportcentre.SendHealthEvent += (IHealthInfo info) =>
            {
                qsmgrSrv.newHealth(info);
                if (SendHealthEvent != null)
                    SendHealthEvent(info);
            };

            _reportcentre.SendWebStatisticEvent += (IWebStatistic w) =>
            {
                if (_infosrv != null)
                    _infosrv.newStatistic(w);
            };

            _riskCentre.ClientSessionEvent += new ClientTrackerInfoSessionDel(_reportcentre.SaveClientSession);

            //_reportcentre.Start();
             * 
             * */

        }

        //初始化任务中心
        //private void InitTaskCentre()
        //{
        //    debug("9.初始化任务中心");
        //    _taskcentre = new TaskCentre();
            //_taskcentre.SendDebugEvent += new DebugDelegate(debug);
            //_taskcentre.Start();
            /*
            //0.每天夜盘开盘前重新加载配资服务
            if (config.Enable_FinService && finmode_loaded)
            {
                TaskCentre.RegisterTask(new TaskProc("重置配资服务",20, 30, 5, _fincentre.Reset));
            }

            //1.夜盘交易过程 //////////////////////////////////////////////////////////////////////////////////////////////////
            //20:40:05晚上重启模拟交易引擎,FastTick连接,CTP交易接口,FastTick远端DataFeed重启
            TaskCentre.RegisterTask(new TaskProc("重启数据与交易通道",20, 45, 5, this.Task_Ready_Reset));//
            //每天晚上8:30:05打开清算中心,准备进行夜盘交易
            *TaskCentre.RegisterTask(new TaskProc("开启清算中心",20, 50, 5, _clearCentre.Task_OpenClearCentre));


            //00:00:05 每日凌晨 更新清算中心当前日期
            *TaskCentre.RegisterTask(new TaskProc("更新交易日期",0, 0, 1, _clearCentre.NewDay));

            //2:28:05执行定时平商品仓(金 银) 撤单与平仓需要有时间间隔,否则平仓时 撤单还没有执行导致系统无法正常平仓
            ×TaskCentre.RegisterTask(new TaskProc("夜盘商品收盘撤单",2, 30 - CoreGlobal.ClearPosBeforeMarketClose, 5, _clearCentre.Task_ClearOrders_Community));//全撤委托 这个时候只有金 银可以交易
            ×TaskCentre.RegisterTask(new TaskProc("夜盘商品收盘平仓",2, 30 - CoreGlobal.ClearPosBeforeMarketClose, 35, _clearCentre.Task_FlatPosition_Community));//全平仓位 这个时候只有金 银可以交易

            //2:32:05执行强制清仓,用于清除交易时间没有正确平仓的仓位 用于维护数据一致性
            *TaskCentre.RegisterTask(new TaskProc("夜盘处理错误持仓",2, 32, 5, _clearCentre.Task_FlatResidue));//清除错误仓位

            //2:35:05执行关闭清算中心，系统将拒绝任何委托信息
            *TaskCentre.RegisterTask(new TaskProc("夜盘关闭清算中心",2, 35, 5, _clearCentre.Task_CloseClearCentre));

            //2.日盘交易过程 //////////////////////////////////////////////////////////////////////////////////////////////////
            //8:45:05早上重启模拟交易引擎,FastTick连接,CTP交易接口,FastTick远端DataFeed重启
            TaskCentre.RegisterTask(new TaskProc("开盘前重启交易与数据通道",08, 45, 5, this.Task_Ready_Reset));//
            //08:50:05 开启清算中心，准备就收客户端委托
            *TaskCentre.RegisterTask(new TaskProc("开启清算中心",08, 50, 5, _clearCentre.Task_OpenClearCentre));//设定服务状态为可用,客户端可以进行交易

            //14:58:05执行定时平商品仓 撤单与平仓需要有时间间隔,否则平仓时 撤单还没有执行导致系统无法正常平仓
            TaskCentre.RegisterTask(new TaskProc("商品收盘撤单",14, 60 - CoreGlobal.ClearPosBeforeMarketClose, 5, _clearCentre.Task_ClearOrders_Community));//全撤委托
            TaskCentre.RegisterTask(new TaskProc("商品收盘平仓",14, 60 - CoreGlobal.ClearPosBeforeMarketClose, 35, _clearCentre.Task_FlatPosition_Community));//全平仓位

            //15:13:05执行定时平股指仓
            TaskCentre.RegisterTask(new TaskProc("股指收盘撤单", 15, 15 - CoreGlobal.ClearPosBeforeMarketClose, 5, _clearCentre.Task_ClearOrders_IF));//全撤委托
            TaskCentre.RegisterTask(new TaskProc("股指收盘平仓", 15, 15 - CoreGlobal.ClearPosBeforeMarketClose, 35, _clearCentre.Task_FlatPosition_IF));//全平仓位

            //15:18:05执行强制清仓,用于清除交易时间没有正确平仓的仓位 用于维护数据一致性
            TaskCentre.RegisterTask(new TaskProc("盘后处理错误持仓", 15, 18, 5, _clearCentre.Task_FlatResidue));//清除错误仓位

            //15:20:05执行关闭清算中心，系统将拒绝任何委托信息
            TaskCentre.RegisterTask(new TaskProc("关闭清算中心", 15, 20, 5, _clearCentre.Task_CloseClearCentre));


            //3.当天数据保存 ////////////////////////////////////////////////////////////////////////////////////////////////
            //15:21:05执行运营数据记录,记录当天的运营数据
            if ((config.LoadMode == QSEnumAccountLoadMode.REAL) || (config.LoadMode == QSEnumAccountLoadMode.ALL))
            { 
                //TaskCentre.RegisterTask(new TaskProc("保存运营统计数据",15, 21, 5, _reportcentre.SaveReport));
            }

            //15:22:05 执行数据检验
            TaskCentre.RegisterTask(new TaskProc("执行数据检验",15, 22, 35, _clearCentre.Task_DataCheck));

            //15:24:05 执行数据清理
            TaskCentre.RegisterTask(new TaskProc("执行数据清理",15, 24, 35, _clearCentre.Task_DataClean));

            //15:10:05 执行数据转储 保存交易记录
            TaskCentre.RegisterTask(new TaskProc("保存交易记录",15, 25, 5, _clearCentre.Task_DataStore));


            //4.结算处理 ////////////////////////////////////////////////////////////////////////////////////////////////////
            //15:30分执行配资扣费程序 如果加载的实盘账户,则我们需要定时进行配资结算 配资扣费程序扣费从账户中以出金的方式扣除,扣费后会进行配资额度调整,会导致计算的费用不正确,因此运营数据记录是在配资结算之前进行记录的
            if (config.Enable_FinService && finmode_loaded)
            {
                if ((config.LoadMode == QSEnumAccountLoadMode.REAL) || (config.LoadMode == QSEnumAccountLoadMode.ALL))
                    TaskCentre.RegisterTask(new TaskProc("配资结算",15, 30, 05, _fincentre.SettleFinServices));
            }

            //16:20:05执行定时结算,结算与重置 之间不可进行财务操作结算后 权益为22万，入金2万则当前权益是24万，重置时会将当前权益作为 昨日权益,并且重新加载结算后的出入金操作,导致出入金操作重复被计算
            TaskCentre.RegisterTask(new TaskProc("系统结算",15, 40, 5, _clearCentre.Task_SettleAccount));//结算时设定服务状态为结算

            //注在系统结算后,重置之前,我们不进行出入金操作
            //16:25:05执行系统重置
            TaskCentre.RegisterTask(new TaskProc("系统重置",15, 50, 5, this.Task_Settle_Reset));
            //系统重置 和结算的关系,结算的将权益更新后,比赛检查会产生权益重置的资金操作,如果重置在比赛结算后,会造成数据不一致,结算是权益22500，比赛检查时重置为25000，重置时 now equity=25000,就会被更新为昨日权益 其实昨日权益为22500
            //并且在 结算账户 和 重置账户之间 不能进行出入金操作,否则会造成昨日权益不正确的问题
            //16:25:05执行比赛考核 比赛考核需要使用折算盈利,折算盈利需要每天结算以后并且充值结算计算flag,重新计算折算收益才可以得到当天结算后的准确折算收益
            //在考核比赛前需要重置账户,用于更新账户的昨日,当前权益,折算权益等(折算权益之计算一次，重置后重新折算) 在比赛考核完毕后 需要再次重置账户,用于重新计算折算权益,否则会出现账户多次晋级的问题


            if (config.Enable_Race && racemode_loaded)
            {
                //5.比赛结算及比赛统计数据 ////////////////////////////////////////////////////////////////////////////////////////
                TaskCentre.RegisterTask(new TaskProc("比赛考核",16, 0, 5, _racecentre.Task_CheckRace));
                
                //16:15分进行系统比赛数据计算,更新排名次序 交易服务程序不能删除，他们只负责写入，因为同时存在实盘 比赛2个程序在运行
                TaskCentre.RegisterTask(new TaskProc("清空比赛统计数据",16, 15, 5, _racecentre.Task_ClearRaceStatistic));
                //
                TaskCentre.RegisterTask(new TaskProc("执行比赛统计计算",16, 16, 5, _racecentre.Task_GenRaceStatistic));
            }

            //17:00:05执行单日交易记录临时表清空
            TaskCentre.RegisterTask(new TaskProc("清空日内交易数据",17, 00, 5, _clearCentre.Task_CleanAfterReset));
            **/
        //}
    }
}
