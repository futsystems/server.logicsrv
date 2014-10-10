﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class ServicePlanChangeForm : Telerik.WinControls.UI.RadForm
    {
        public ServicePlanChangeForm()
        {
            InitializeComponent();
        }

        JsonWrapperFinServiceStub stub = null;
        public JsonWrapperFinServiceStub FinServiceStub
        {
            get
            {
                return stub;
            }
            set
            {
                stub = value;
                if(stub != null)
                {
                    lbcurrentsp.Text = stub.ServicePlaneName;
                }
                else
                {
                    lbcurrentsp.Text = "未开通配资服务";
                }
            }
        }

        IAccountLite _account = null;
        public IAccountLite Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                lbaccount.Text = _account.Account;
            }
        }

        public void SetServicePlans(JsonWrapperServicePlane[] plans)
        { 
            ArrayList list = new ArrayList();
            foreach (JsonWrapperServicePlane sp in plans)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = sp.Title;
                vo.Value = sp.ID;
                list.Add(vo);
            }

            Factory.IDataSourceFactory(cbServicePlan).BindDataSource(list);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            JsonWrapperChgServicePlaneRequest request = new JsonWrapperChgServicePlaneRequest();
            request.Account = _account.Account;
            request.ServicePlaneFK = int.Parse(cbServicePlan.SelectedValue.ToString());

            if (stub == null)
            {
                if (fmConfirm.Show("确认为帐户:" + _account.Account + " 添加配资服务:【" + cbServicePlan.SelectedItem.Text + "】? 添加成功后请修改服务参数!") == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                if (request.ServicePlaneFK == stub.ServicePlaneFK)
                {
                    fmConfirm.Show("当前服务即为:【" + stub.ServicePlaneName + "】请选择不同的服务计划类型!");
                    return;
                }
                if (fmConfirm.Show("确认将帐户:" + _account.Account + "的配资服务:【" + stub.ServicePlaneName + "】修改为:【" + cbServicePlan.SelectedItem.Text + "】? 修改配资服务将删除原来的参数设置,修改成功后请修改服务参数!") == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            Globals.TLClient.ReqChangeFinService(TradingLib.Mixins.LitJson.JsonMapper.ToJson(request));
            this.Close();
        }
    }
}
