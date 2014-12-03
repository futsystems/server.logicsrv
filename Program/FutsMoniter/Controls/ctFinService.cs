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
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class ctFinService : UserControl, IEventBinder
    {
        public ctFinService()
        {
            InitializeComponent();

            btnChangeServicePlan.Click +=new EventHandler(btnChangeServicePlan_Click);
            btnUpdateArgs.Click +=new EventHandler(btnUpdateArgs_Click);
            btnDeleteFinService.Click +=new EventHandler(btnDeleteFinService_Click);

            //执行事件订阅
            Globals.RegIEventHandler(this);
        }

        #region IEventBinder
        public void OnInit()
        {
            Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "QryFinService", this.OnQryFinService);//查询配资服务
            Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "QryFinServicePlan", this.OnQryServicePlan);//查询配资服务计划
            Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "UpdateArguments", this.OnQryFinService);//更新参数
            Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "ChangeServicePlane", this.OnQryFinService);//修改服务计划
            Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "DeleteServicePlane", this.OnQryFinService);//删除服务
        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "QryFinService", this.OnQryFinService);//查询配资服务
            Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "QryFinServicePlan", this.OnQryServicePlan);//查询配资服务计划
            Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "UpdateArguments", this.OnQryFinService);//更新参数
            Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "ChangeServicePlane", this.OnQryFinService);//修改服务计划
            Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "DeleteServicePlane", this.OnQryFinService);//删除服务
        }
        #endregion



        JsonWrapperFinServiceStub finservice = null;
        JsonWrapperServicePlane[] serviceplans = null;

        IAccountLite _account = null;
        /// <summary>
        /// 设定当前交易帐号
        /// 交易帐号改变就需要查询该帐号的配资服务
        /// </summary>
        //public IAccountLite CurrentAccount
        //{
        //    get
        //    {
        //        return _account;
        //    }
        //    set
        //    {
                
        //    }
        //}

        /// <summary>
        /// 响应交易帐户选中事件
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountSelected(IAccountLite account)
        {
            _account = account;
            finservice = null;//重置配资服务

            if (!Globals.EnvReady) return;
            if (!Globals.UIAccess.fun_tab_finservice) return;
            //如果服务计划没有获取 则请求服务计划
            if (serviceplans == null)
            {
                Globals.TLClient.ReqQryServicePlan();
            }
            //请求交易帐户的配资服务
            if (_account != null)
            {
                Globals.TLClient.ReqQryFinService(_account.Account);
            }
        }

        public void PrepareServicePlan()
        {
            if (serviceplans == null)
            {
                Globals.TLClient.ReqQryServicePlan();
            }
        }


        public void OnQryFinService(string jsonstr)
        {
            Globals.Debug("ctFinService got json ret:" + jsonstr);

            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperFinServiceStub obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperFinServiceStub>(jd["Playload"].ToJson());
                GotFinServiceStub(obj);
                finservice = obj;

                StubShow();

            }
            else//如果没有配资服
            {
                finservice = null;
                NoStubShow();
            }
        }

        /// <summary>
        /// 获得服务计划
        /// </summary>
        /// <param name="jsonstr"></param>
        public void OnQryServicePlan(string jsonstr)
        {
            Globals.Debug("ctFinService got json ret:" + jsonstr);
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                serviceplans = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperServicePlane[]>(jd["Playload"].ToJson());
            }
        }

        delegate void JsonWrapperFinServiceStubDel(JsonWrapperFinServiceStub stub);
        void GotFinServiceStub(JsonWrapperFinServiceStub stub)
        {
            if (InvokeRequired)
            {
                Invoke(new JsonWrapperFinServiceStubDel(GotFinServiceStub), new object[] { stub });
            }
            else
            {
                this.lbaccount.Text = stub.Account;
                this.lbsptitle.Text = stub.ServicePlaneName;
                this.lbstatus.Text = stub.Active ? "激活" : "冻结";
                this.lbchargetype.Text = stub.FinService.ChargeType;
                this.lbcollecttype.Text = stub.FinService.CollectType;

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
                    Globals.TLClient.ReqUpdateFinServiceArgument(TradingLib.Mixins.LitJson.JsonMapper.ToJson(finservice));
                }
            }
        }

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
    }
}
