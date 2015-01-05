using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using TradingLib.Mixins.JsonObject;


namespace FutsMoniter
{
    public partial class fmCashCentre : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmCashCentre()
        {
            InitializeComponent();

            this.Load += new EventHandler(CasherMangerForm_Load);

        }

        void CasherMangerForm_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }



        public void OnInit()
        {

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryAgentCashOperationTotal", this.OnQryAgentCashOperationTotal);//查询代理出入金请求
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryAccountCashOperationTotal", this.OnQryAccountCashOperationTotal);//查询交易帐户出入金请求

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "RequestCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "CancelCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "RejectCashOperation", this.OnAccountCashOperation);

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "ConfirmAccountCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "CancelAccountCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "RejectAccountCashOperation", this.OnAccountCashOperation);

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

            Globals.TLClient.ReqQryAgentCashopOperationTotal();
            Globals.TLClient.ReqQryAccountCashopOperationTotal();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryAgentCashOperationTotal", this.OnQryAgentCashOperationTotal);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryAccountCashOperationTotal", this.OnQryAccountCashOperationTotal);//查询交易帐户出入金请求


            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "RequestCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "CancelCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "RejectCashOperation", this.OnAccountCashOperation);

            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "ConfirmAccountCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "CancelAccountCashOperation", this.OnAccountCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "RejectAccountCashOperation", this.OnAccountCashOperation);

            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

            
        }

        void OnQryAgentCashOperationTotal(string jsonstr)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperCashOperation[] objs = MoniterUtils.ParseJsonResponse<JsonWrapperCashOperation[]>(jsonstr);
            if (objs!=null)
            {
                //JsonWrapperCashOperation[] objs = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation[]>(jd["Playload"].ToJson());
                foreach (JsonWrapperCashOperation op in objs)
                {
                    ctCashOperationAgent.GotJsonWrapperCashOperation(op);
                }
            }
            else//如果没有配资服
            {

            }
        }

        void OnQryAccountCashOperationTotal(string jsonstr)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperCashOperation[] objs = MoniterUtils.ParseJsonResponse<JsonWrapperCashOperation[]>(jsonstr);
            if (objs != null)
            {
                //JsonWrapperCashOperation[] objs = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation[]>(jd["Playload"].ToJson());
                foreach (JsonWrapperCashOperation op in objs)
                {
                    ctCashOperationAccount.GotJsonWrapperCashOperation(op);
                }
            }
            else//如果没有配资服
            {

            }
        }



        void OnAgentCashOperation(string jsonstr)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperCashOperation obj = MoniterUtils.ParseJsonResponse<JsonWrapperCashOperation>(jsonstr);
            if (obj != null)
            {
                //JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());
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

        void OnAccountCashOperation(string jsonstr)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperCashOperation obj = MoniterUtils.ParseJsonResponse<JsonWrapperCashOperation>(jsonstr);

            if (obj!= null)
            {
                //JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());
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


        void OnNotifyCashOperation(string jsonstr)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperCashOperation obj = MoniterUtils.ParseJsonResponse<JsonWrapperCashOperation>(jsonstr);
            if (obj != null)
            {
                //JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());

                if (obj.mgr_fk > 0)
                {
                    ctCashOperationAgent.GotJsonWrapperCashOperation(obj);
                }
                else
                {
                    ctCashOperationAccount.GotJsonWrapperCashOperation(obj);
                }
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
