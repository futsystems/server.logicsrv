using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;

using FutsMoniter.Common;

namespace FutsMoniter
{
    partial class MainForm
    {
        /// <summary>
        /// 初始化成功后回调 用于初始化数据
        /// </summary>
        public void OnInitFinished()
        {
            Globals.Debug("Manager Name:" + Globals.Manager.Name + " BaseFK:" + Globals.Manager.mgr_fk + " MgrType:" + Globals.Manager.Type.ToString());

            //if (!Globals.Manager.RightRootDomain())
            //{
            //    //btnGPSystem.Enabled = false;
            //    //btnGPSymbol.Enabled = false;
            //}

        }

        void OnRspInfo(TradingLib.API.RspInfo info)
        {
            //将RspInfo写入缓存 等待后台线程进行处理
            infobuffer.Write(info);
        }

        public void OnInit()
        {
            Globals.Debug("Set control visable via UIAccess");

            //分区管理窗口
            kryptonContextMenuItem_Domain.Visible = Globals.LoginResponse.Domain.Super;

            //接口设置
            kryptonRibbonGroupButton_interfacelist.Visible = Globals.LoginResponse.Domain.Super;
            
            //系统 开启关闭清算中心 默认行情和交易通道
            kryptonRibbonGroupButton_OpenClearCentre.Visible = Globals.LoginResponse.Domain.Super || Globals.Domain.Dedicated;
            kryptonRibbonGroupButton_CloseClearCentre.Visible = Globals.LoginResponse.Domain.Super || Globals.Domain.Dedicated;
            kryptonRibbonGroupButton_tickpaper.Visible = Globals.Domain.Super || Globals.Domain.Dedicated;
            //ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("defaultrouter:" + kryptonRibbonGroupButton_tickpaper.Visible.ToString());
            kryptonRibbonGroup1.Visible = Globals.LoginResponse.Domain.Super || Globals.Domain.Dedicated;

            //日志窗口
            kryptonRibbonQATButton_debug.Visible = Globals.LoginResponse.Domain.Super;

            Globals.LogicEvent.GotRspInfoEvent += new Action<TradingLib.API.RspInfo>(OnRspInfo);

            //超级管理员 可以查看所有界面
            if (!(Globals.LoginResponse.Domain.Super&&Globals.Manager.IsRoot()))
            {
                //系统管理
                if (!Globals.UIAccess.nav_system || (!Globals.Manager.IsRoot()))//只有管理员才可以查看
                {
                    tabSystem.Visible = false;
                }
                else
                {
                    //kryptonRibbonGroupButton_RouterList.Visible = Globals.UIAccess.nav_system_router;
                    
                    kryptonRibbonGroupButton_connectorlist.Visible = Globals.Manager.IsRoot();//实盘帐号

                    kryptonRibbonGroupButton_SystemStatus.Visible = Globals.Manager.IsRoot();//系统状态
                }

                //基础数据
                if (!Globals.UIAccess.nav_basic)
                {
                    tabBasicConfig.Visible = false;
                }
                else
                {
                    //交易所与合约
                    kryptonRibbonGroupButton_Mktime.Visible = Globals.UIAccess.nav_basic_mktime;
                    kryptonRibbonGroupButton_Exchange.Visible = Globals.UIAccess.nav_basic_exchange;
                    kryptonRibbonGroupButton_Security.Visible = Globals.UIAccess.nav_basic_security;
                    kryptonRibbonGroupButton_Symbol.Visible = Globals.UIAccess.nav_basic_symbol;

                    //模块设置
                    kryptonRibbonGroupButton_PermissionTemplate.Visible = Globals.Manager.IsRoot() && Globals.UIAccess.nav_basic_permissiontemplate;
                    kryptonRibbonGroup5.Visible = Globals.UIAccess.nav_basic_permissiontemplate;
                }

                //柜员管理
                if (!Globals.UIAccess.nav_manager)
                {
                    tabAgent.Visible = false;
                }
                else
                {
                    //柜员管理
                    kryptonRibbonGroupButton_AgentManagement.Visible = Globals.UIAccess.nav_manager_management;
                    //资费设置
                    kryptonRibbonGroupButton_AgentCost.Visible = Globals.UIAccess.nav_manager_feeconfig;
                    //管理员才有权设置权限
                    kryptonRibbonGroupButton_PermissionAgent.Visible = Globals.Manager.IsRoot() && Globals.UIAccess.nav_manager_permissionagent;
                }

                //财务管理
                if (!Globals.UIAccess.nav_finance)
                {
                    tabFinance.Visible = false;
                }
                else
                {
                    //代理财务帐户
                    kryptonRibbonGroupButton_FinanceManagement.Visible = Globals.Manager.IsAgent() && Globals.UIAccess.nav_finance_fincentre;
                    //离线入金
                    kryptonRibbonGroupButton_AccountCashreq.Visible = Globals.UIAccess.nav_finance_accountcashreq;
                    //在线支付
                    kryptonRibbonGroupButton_payonline.Visible = Globals.Domain.Module_PayOnline && Globals.UIAccess.nav_finance_payonline;

                    //管理员才可以进行出纳管理或修改收款方式
                    kryptonRibbonGroupButton_CasherManagement.Visible = Globals.Manager.IsRoot() && Globals.UIAccess.nav_finance_cashercentre;
                    kryptonRibbonGroupButton_ReceiveBank.Visible = Globals.Manager.IsRoot();

                    kryptonRibbonGroup12.Visible = kryptonRibbonGroupButton_CasherManagement.Visible || kryptonRibbonGroupButton_ReceiveBank.Visible;

                }

                //报表管理
                if (!Globals.UIAccess.nav_report)
                {
                    tabHistQuery.Visible = false;
                }
                else
                {
                    kryptonRibbonGroupButton_QueryExHist.Visible = Globals.UIAccess.nav_report_acchistinfo;
                    kryptonRibbonGroupButton_QueryCashTransAccount.Visible = Globals.UIAccess.nav_report_acccashtrans;
                    kryptonRibbonGroupButton_QuerySettleAccount.Visible = Globals.UIAccess.nav_report_accsettlement;
                    kryptonRibbonGroup6.Visible = Globals.UIAccess.nav_report_acchistinfo || Globals.UIAccess.nav_report_acccashtrans || Globals.UIAccess.nav_report_accsettlement;

                    kryptonRibbonGroupButton_QueryCashTransAgent.Visible = Globals.UIAccess.nav_report_agentcashtrans;
                    kryptonRibbonGroupButton_QuerySettleAgent.Visible = Globals.UIAccess.nav_report_agentsettlement;
                    kryptonRibbonGroup7.Visible = Globals.UIAccess.nav_report_agentcashtrans || Globals.UIAccess.nav_report_agentsettlement;

                    kryptonRibbonGroupButton_QueryAgentProfit.Visible = Globals.UIAccess.nav_report_agentreport;
                    kryptonRibbonGroup8.Visible = Globals.UIAccess.nav_report_agentreport;
                }


                //域权限最终控制

                //代理管理
                //tabAgent.Visible = !Globals.LoginResponse.Domain.Module_Agent ? false : tabAgent.Visible;
                
                //代理模块禁止
                if (!Globals.Domain.Module_Agent)
                {
                    kryptonRibbonGroupButton_PermissionTemplate.Visible = false;//权限模板设置

                    kryptonRibbonGroupButton_AgentCost.Visible = false;//代理资费设置
                    kryptonRibbonGroupButton_PermissionAgent.Visible = false;//代理模板设置

                    kryptonRibbonGroupButton_FinanceManagement.Visible = false;//代理收益帐户

                    //代理商报表
                    kryptonRibbonGroupButton_QueryCashTransAgent.Visible = false;
                    kryptonRibbonGroupButton_QuerySettleAgent.Visible = false;
                    kryptonRibbonGroup7.Visible = false;

                    //代理分润查询
                    kryptonRibbonGroupButton_QueryAgentProfit.Visible = false;
                    kryptonRibbonGroup8.Visible = false;
           
                }
                else
                {
                    //如果是代理 且没有开启多级代理则无需显示代理资费设置
                    if (!Globals.Domain.Module_SubAgent && Globals.Manager.IsAgent())
                    {
                        kryptonRibbonGroupButton_AgentCost.Visible = false;//代理资费设置
                    }
                }

                //没有启用实盘交易
                if(!Globals.Domain.Router_Live)
                {
                    kryptonRibbonGroupButton_connectorlist.Visible = false;
                }

            }
                 
        }
        public void OnDisposed()
        {

        }


    }
}
