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
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class fmAccountCashReq : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmAccountCashReq()
        {
            InitializeComponent();

            this.Load += new EventHandler(fmAccountCashReq_Load);
        }

        void fmAccountCashReq_Load(object sender, EventArgs e)
        {
            //全局事件回调
            Globals.RegIEventHandler(this);

            //
            btnAccountCashReq.Click += new EventHandler(btnAccountCashReq_Click);

            if (Globals.EnvReady)
            {
                //Globals.TLClient.ReqQryAgentCashopOperationTotal();
                Globals.TLClient.ReqQryAccountCashopOperationTotal();
            }
        }

        void btnAccountCashReq_Click(object sender, EventArgs e)
        {
            fmAccountCashOperation fm = new fmAccountCashOperation();
            fm.Show();
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryAccountCashOperationTotal", this.OnQryAccountCashOperationTotal);//查询交易帐户出入金请求
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryAccountCashOperationTotal", this.OnQryAccountCashOperationTotal);//查询交易帐户出入金请求
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

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

        void OnNotifyCashOperation(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());

                if (obj.mgr_fk > 0)
                {
                    //不响应代理上出入金请求
                    return;
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
