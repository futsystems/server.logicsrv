﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class ctAgentSPArgConfig : UserControl
    {
        public ctAgentSPArgConfig()
        {
            InitializeComponent();
            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "QryAgentSPArg", this.OnQrySPAgentArg);
            }
            this.Disposed += new EventHandler(ctAgentSPArgConfig_Disposed);
        }

        void ctAgentSPArgConfig_Disposed(object sender, EventArgs e)
        {
            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "QryAgentSPArg", this.OnQrySPAgentArg);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (spagentarg != null)
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
                if (fmConfirm.Show("确认更新帐户:" + ctAgentList1.CurrentAgentFK + "的配资服务参数为当前设置?") == DialogResult.Yes)
                {
                    //Globals.TLClient.ReqUpdateFinServiceArgument(TradingLib.Mixins.LitJson.JsonMapper.ToJson(finservice));
                    Globals.TLClient.ReqUpdateSPAgentArg(TradingLib.Mixins.LitJson.JsonMapper.ToJson(spagentarg));

                }
            }
        }

        JsonWrapperServicePlanAgentArgument spagentarg = null;

        void OnQrySPAgentArg(string jsonstr)
        {
            Globals.Debug("FeeConfigForm got json ret:" + jsonstr);

            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperServicePlanAgentArgument obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperServicePlanAgentArgument>(jd["Playload"].ToJson());
                GotSPAgentArg(obj);
                lbsptitle.Text = obj.Title;
                lbchargetype.Text = obj.ChargeType;
                lbcollecttype.Text = obj.CollectType;
                spagentarg = obj;
            }
            else//如果没有配资服
            {
                //finservice = null;
                //NoStubShow();
                spagentarg = null;
            }
        }

        delegate void del1(JsonWrapperServicePlanAgentArgument arg);
        void GotSPAgentArg(JsonWrapperServicePlanAgentArgument arg)
        {
            if (InvokeRequired)
            {
                Invoke(new del1(GotSPAgentArg), new object[] { arg });
            }
            else
            {
                InitArgs(arg.Arguments);
            }
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


        private void ctFinServicePlanList1_ServicePlanSelectedChangedEvent()
        {
            RefreshSPAgentArg();
        }

        private void ctAgentList1_AgentSelectedChangedEvent()
        {
            RefreshSPAgentArg();
        }

        /// <summary>
        /// 重新请求代理服务计划参数
        /// </summary>
        void RefreshSPAgentArg()
        {
            Globals.TLClient.ReqQrySPAgentArg(ctAgentList1.CurrentAgentFK, ctFinServicePlanList1.CurrentServicePlanFK);
        }

    }
}
