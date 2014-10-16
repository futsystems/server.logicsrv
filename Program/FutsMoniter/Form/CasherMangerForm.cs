using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class CasherMangerForm : Telerik.WinControls.UI.RadForm
    {
        public CasherMangerForm()
        {
            InitializeComponent();
            if (Globals.CallbackCentreReady)
            {
                if (Globals.CallbackCentreReady)
                {
                    //Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryFinanceInfo", this.OnQryAgentFinanceInfo);
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryAgentCashOperationTotal", this.OnQryAgentCashOperationTotal);//查询代理出入金请求
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryAccountCashOperationTotal", this.OnQryAccountCashOperationTotal);//查询交易帐户出入金请求

                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "RequestCashOperation", this.OnCashOperation);
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnCashOperation);
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "CancelCashOperation", this.OnCashOperation);
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "RejectCashOperation", this.OnCashOperation);

                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "ConfirmAccountCashOperation", this.OnAccountCashOperation);
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "CancelAccountCashOperation", this.OnAccountCashOperation);
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "RejectAccountCashOperation", this.OnAccountCashOperation);
                    

                }
            }
            this.FormClosing += new FormClosingEventHandler(CasherMangerForm_FormClosing);
            this.Load += new EventHandler(CasherMangerForm_Load);

        }

        void CasherMangerForm_Load(object sender, EventArgs e)
        {
            if (Globals.EnvReady)
            {

                Globals.TLClient.ReqQryAgentCashopOperationTotal();
                Globals.TLClient.ReqQryAccountCashopOperationTotal();
            }
        }

        void CasherMangerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryAgentCashOperationTotal", this.OnQryAgentCashOperationTotal);
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryAccountCashOperationTotal", this.OnQryAccountCashOperationTotal);//查询交易帐户出入金请求


            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "RequestCashOperation", this.OnCashOperation);
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnCashOperation);
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "CancelCashOperation", this.OnCashOperation);
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "RejectCashOperation", this.OnCashOperation);

            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "ConfirmAccountCashOperation", this.OnAccountCashOperation);
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "CancelAccountCashOperation", this.OnAccountCashOperation);
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "RejectAccountCashOperation", this.OnAccountCashOperation);
                    
        }

        void OnQryAgentCashOperationTotal(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation[] objs = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation[]>(jd["Playload"].ToJson());
                foreach (JsonWrapperCashOperation op in objs)
                {
                    ctCashOperation1.GotJsonWrapperCashOperation(op);
                }
            }
            else//如果没有配资服
            {

            }
        }

        void OnQryAccountCashOperationTotal(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation[] objs = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation[]>(jd["Playload"].ToJson());
                foreach (JsonWrapperCashOperation op in objs)
                {
                    ctCashOperationAccount.GotJsonWrapperCashOperation(op);
                }
            }
            else//如果没有配资服
            {

            }
        }



        void OnCashOperation(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());
                ctCashOperation1.GotJsonWrapperCashOperation(obj);
            }
            else//如果没有配资服
            {

            }
            if (Globals.EnvReady)
            {
                //Globals.TLClient.ReqQryAgentFinanceInfoLite();
            }
        }

        void OnAccountCashOperation(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());
                ctCashOperationAccount.GotJsonWrapperCashOperation(obj);
            }
            else//如果没有配资服
            {

            }
            if (Globals.EnvReady)
            {
                //Globals.TLClient.ReqQryAgentFinanceInfoLite();
            }
        }

    }
}
