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
        /// 登入成功基础数据加载完毕后再执行OnInit
        /// </summary>
        public void OnInit()
        {
            Globals.Debug("Set control visable via UIAccess");

            //根据权限设置界面菜单权限

            //日志窗口
            kryptonRibbonQATButton_debug.Visible = true;//Globals.LoginResponse.Domain.Super;

            //分区管理窗口
            kryptonContextMenuItem_Domain.Visible = Globals.Domain.Super;

            //接口设置
            kryptonRibbonGroupButton_interfacelist.Visible = Globals.Domain.Super;
            
            //系统 开启关闭清算中心 默认行情和交易通道 超级管理员或者是独立部署的管理员
            kryptonRibbonGroupButton_OpenClearCentre.Visible = Globals.Domain.Super || Globals.Domain.Dedicated;
            kryptonRibbonGroupButton_CloseClearCentre.Visible = Globals.Domain.Super || Globals.Domain.Dedicated;
            kryptonRibbonGroup1.Visible = Globals.Domain.Super || Globals.Domain.Dedicated;
            kryptonRibbonGroupButton_tickpaper.Visible = Globals.Domain.Super || Globals.Domain.Dedicated;


            //超级管理员 可以查看所有界面
            if (!(Globals.Domain.Super&&Globals.Manager.IsRoot()))
            {
                if (Globals.TLClient.ServerVersion.ProductType == QSEnumProductType.CounterSystem)
                {
                    //系统管理
                    if (!Globals.Manager.IsRoot())//不是管理员则系统管理tab不可见
                    {
                        tabSystem.Visible = false;
                    }
                    else
                    {
                        //管理员可以查看实盘帐户列表和系统状态
                        kryptonRibbonGroupButton_connectorlist.Visible = Globals.Manager.IsRoot();//实盘帐号
                        kryptonRibbonGroupButton_SystemStatus.Visible = Globals.Manager.IsRoot();//系统状态

                        //权限模板 管理员查看
                        kryptonRibbonGroupButton_permissiontmp.Visible = Globals.Manager.IsRoot();
                        kryptonRibbonGroup13.Visible = kryptonRibbonGroupButton_permissiontmp.Visible;
                    }

                    //------------------------基础数据 -------------------------------------------------------
                    //基础数据 代理也可见 且不用设置权限限制



                    //}




                    //------------------------柜员管理-------------------------------------------------------
                    //管理员才有权设置权限
                    kryptonRibbonGroupButton_PermissionAgent.Visible = Globals.Manager.IsRoot();
                    //}

                    //------------------------财务管理-------------------------------------------------------
                    //代理才有权限看到 代理财务帐户
                    kryptonRibbonGroupButton_FinanceManagement.Visible = Globals.Manager.IsAgent();

                    //管理员才可以进行出纳管理或修改收款方式
                    kryptonRibbonGroupButton_CasherManagement.Visible = Globals.Manager.IsRoot();
                    kryptonRibbonGroupButton_ReceiveBank.Visible = Globals.Manager.IsRoot();

                    kryptonRibbonGroup12.Visible = kryptonRibbonGroupButton_CasherManagement.Visible || kryptonRibbonGroupButton_ReceiveBank.Visible;


                    //离线出入金模块
                    if (!Globals.Domain.Module_PayOnline)
                    {
                        kryptonRibbonGroupButton_payonline.Visible = false;
                    }
                    //代理模块
                    if (!Globals.Domain.Module_Agent)//没有代理模块
                    {

                        kryptonRibbonGroupButton_permissiontmp.Visible = false;//权限模板设置

                        //-----------------------柜员管理----------------------------------------
                        kryptonRibbonGroupButton_AgentCost.Visible = false;//代理资费设置
                        kryptonRibbonGroupButton_PermissionAgent.Visible = false;//代理模板设置

                        //-----------------------财务管理----------------------------------------
                        kryptonRibbonGroupButton_FinanceManagement.Visible = false;//代理收益帐户


                        //-----------------------记录与报表--------------------------------------
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
                    if (!Globals.Domain.Router_Live)
                    {
                        kryptonRibbonGroupButton_connectorlist.Visible = false;
                        kryptonRibbonGroup2.Visible = kryptonRibbonGroupButton_connectorlist.Visible || kryptonRibbonGroupButton_tickpaper.Visible || kryptonRibbonGroupButton_interfacelist.Visible;
                    }
                }

                

            }

            if (Globals.TLClient.ServerVersion.ProductType == QSEnumProductType.VendorMoniter)
            {

                tabAgent.Visible = false;//柜员tab不可见
                tabFinance.Visible = false;//财务管理tab不可见
                tabHistQuery.Visible = false;//历史查询tab不可见
                kryptonRibbonGroupButton_interfacelist.Visible = false;
                kryptonRibbonGroupButton_connectorlist.Visible = false;
            }


           
            //更新过期提醒
            SetExpireStatus();

            //加载管理控件
            this.LoadMoniterControl();
            //初始化管理端Page页面
            InitPage();

            //初始化后台woker用于弹窗提示
            InitBW();
            //操作回报消息 弹窗提示
            Globals.LogicEvent.GotRspInfoEvent += new Action<TradingLib.API.RspInfo>(OnRspInfo);
                 
        }
        public void OnDisposed()
        {

        }


    }
}
