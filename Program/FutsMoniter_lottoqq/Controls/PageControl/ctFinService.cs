using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class ctFinService : UserControl, IEventBinder
    {
        public ctFinService()
        {
            InitializeComponent();
            this.Load += new EventHandler(ctFinService_Load);
        }

        void ctFinService_Load(object sender, EventArgs e)
        {
            NoStubShow();

            btnChangeServicePlan.Click += new EventHandler(btnChangeServicePlan_Click);
            btnUpdateArgs.Click += new EventHandler(btnUpdateArgs_Click);
            btnDeleteFinService.Click += new EventHandler(btnDeleteFinService_Click);

            //执行事件订阅
            Globals.RegIEventHandler(this);
        }

        #region IEventBinder
        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("FinServiceCentre", "QryFinService", this.OnQryFinService);//查询配资服务
            Globals.LogicEvent.RegisterCallback("FinServiceCentre", "QryFinServicePlan", this.OnQryServicePlan);//查询配资服务计划
            Globals.LogicEvent.RegisterCallback("FinServiceCentre", "UpdateArguments", this.OnQryFinService);//更新参数
            Globals.LogicEvent.RegisterCallback("FinServiceCentre", "ChangeServicePlane", this.OnQryFinService);//修改服务计划
            Globals.LogicEvent.RegisterCallback("FinServiceCentre", "DeleteServicePlane", this.OnDeleteFinService);//删除服务
            Globals.LogicEvent.GotAccountSelectedEvent += new Action<AccountLite>(OnAccountSelected);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("FinServiceCentre", "QryFinService", this.OnQryFinService);//查询配资服务
            Globals.LogicEvent.UnRegisterCallback("FinServiceCentre", "QryFinServicePlan", this.OnQryServicePlan);//查询配资服务计划
            Globals.LogicEvent.UnRegisterCallback("FinServiceCentre", "UpdateArguments", this.OnQryFinService);//更新参数
            Globals.LogicEvent.UnRegisterCallback("FinServiceCentre", "ChangeServicePlane", this.OnQryFinService);//修改服务计划
            Globals.LogicEvent.UnRegisterCallback("FinServiceCentre", "DeleteServicePlane", this.OnDeleteFinService);//删除服务
            Globals.LogicEvent.GotAccountSelectedEvent -= new Action<AccountLite>(OnAccountSelected);
      
        }
        #endregion



        JsonWrapperFinServiceStub finservice = null;
        JsonWrapperServicePlane[] serviceplans = null;

        AccountLite _account = null;

        /// <summary>
        /// 响应交易帐户选中事件
        /// </summary>
        /// <param name="account"></param>
        void OnAccountSelected(AccountLite account)
        {
            if (account == null) return;
            _account = account;
            finservice = null;//重置配资服务
            NoStubShow();

            if (!Globals.Domain.Module_FinService) return;

            //如果服务计划没有获取 则请求服务计划
            if (serviceplans == null)
            {
                Globals.TLClient.ReqQryServicePlan();
            }

            if (Globals.Domain.Module_FinService)
            {
                //请求交易帐户的配资服务
                Globals.TLClient.ReqQryFinService(_account.Account);
            }
        }

        void OnDeleteFinService(string json)
        {
            JsonWrapperFinServiceStub stub = MoniterUtils.ParseJsonResponse<JsonWrapperFinServiceStub>(json);
            if (stub == null)
            {
                finservice = null;
                NoStubShow();
            }
        }


        void OnQryFinService(string jsonstr)
        {
            JsonWrapperFinServiceStub obj = MoniterUtils.ParseJsonResponse<JsonWrapperFinServiceStub>(jsonstr);
            if (obj != null)
            {
                GotFinServiceStub(obj);
                finservice = obj;
                StubShow();
            }
        }

        /// <summary>
        /// 获得服务计划
        /// </summary>
        /// <param name="jsonstr"></param>
        void OnQryServicePlan(string jsonstr)
        {
            JsonWrapperServicePlane[] splist = MoniterUtils.ParseJsonResponse<JsonWrapperServicePlane[]>(jsonstr);
            serviceplans =splist;
        }

        void GotFinServiceStub(JsonWrapperFinServiceStub stub)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<JsonWrapperFinServiceStub>(GotFinServiceStub), new object[] { stub });
            }
            else
            {
                this.lbaccount.Text = stub.Account;
                this.lbsptitle.Text = stub.ServicePlaneName;
                this.lbstatus.Text = stub.Active ? "激活" : "冻结";
                this.lbchargetype.Text = stub.FinService.ChargeType;
                this.lbcollecttype.Text = stub.FinService.CollectType;
                this.lbforceclose.Text = stub.ForceClose ? "触发" : "未触发";
                InitArgs(stub.FinService.Arguments);
            }
        }



        void NoStubShow()
        {
            this.lbaccount.Text = "--";
            this.lbsptitle.Text = "服务未开通";
            this.lbstatus.Text = "--";
            this.lbchargetype.Text = "--";
            this.lbcollecttype.Text = "--";
            btnChangeServicePlan.Text = "添加配资服务";
            this.tableLayoutPanel.Controls.Clear();
            btnChangeStatus.Enabled = false;
            btnUpdateArgs.Enabled = false;
            btnDeleteFinService.Enabled = false;

        }

        void StubShow()
        {
            btnChangeServicePlan.Text = "修改配资服务";
            btnChangeStatus.Enabled = true;
            btnUpdateArgs.Enabled = true;
            btnDeleteFinService.Enabled = true;
        }



        void InitArgs(JsonWrapperArgument[] args)
        {
            tableLayoutPanel.Controls.Clear();

            foreach (JsonWrapperArgument arg in args)
            {
                ctTLEdit edit = new ctTLEdit();
                edit.Argument = arg;
                tableLayoutPanel.Controls.Add(edit);
            }
        }


        #region 事件
        /// <summary>
        /// 更新服务参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateArgs_Click(object sender, EventArgs e)
        {

            if (finservice != null)
            {
                foreach (Control c in tableLayoutPanel.Controls)
                {
                    ctTLEdit edit = c as ctTLEdit;
                    if (!edit.ParseArg())
                    {
                        fmConfirm.Show("参数:" + edit.Argument.ArgTitle + " 设置无效,请输入有效参数!");
                        return;
                    }
                }
                if (fmConfirm.Show("确认更新帐户:" + _account.Account + "的配资服务参数为当前设置?") == DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateFinServiceArgument(TradingLib.Mixins.Json.JsonMapper.ToJson(finservice));
                }
            }
        }
        /// <summary>
        /// 修改服务计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeServicePlan_Click(object sender, EventArgs e)
        {
            if (_account == null)
            {
                fmConfirm.Show("请选择交易帐号");
                return;
            }
            if (serviceplans == null || serviceplans.Length == 0)
            {
                fmConfirm.Show("获取服务计划异常!");
                return;
            }

            fmChangeServicePlan fm = new fmChangeServicePlan();
            fm.Text = btnChangeServicePlan.Text;
            fm.FinServiceStub = finservice;
            fm.Account = _account;
            //fm.SetServicePlans(serviceplans);
            fm.ShowDialog();
        }

        /// <summary>
        /// 删除服务计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteFinService_Click(object sender, EventArgs e)
        {
            if (_account == null)
            {
                fmConfirm.Show("请选择交易帐号");
                return;
            }
            if (fmConfirm.Show("确认删除帐户:" + _account.Account + "的配资服务?") == DialogResult.Yes)
            {
                Globals.TLClient.ReqDeleteFinService(_account.Account);
            }
        }
        #endregion

    }
}
