using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib;
using TradingLib.Mixins.JsonObject;
using TradingLib.Mixins.LitJson;

namespace FutsMoniter
{
    public partial class ctFinService : UserControl
    {
        public ctFinService()
        {
            InitializeComponent();
            //EmptyShow();
            //oppanel.Visible = false;
        }

        public void OnQryFinService(string jsonstr)
        {
            Globals.Debug("ctFinService got json ret:" + jsonstr);

            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperFinServiceStub obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperFinServiceStub>(jd["Playload"].ToJson());
                //Globals.Debug("playload:" + JsonMapper.ToJson(obj));
                GotFinServiceStub(obj);
                //oppanel.Visible = true;
            }
            else
            {
                EmptyShow();
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
        void EmptyShow()
        {
            this.lbaccount.Text = "--";
            this.lbsptitle.Text = "--";
            this.lbstatus.Text = "--";
            this.lbchargetype.Text = "--";
            this.lbcollecttype.Text = "--";
            this.tableLayoutPanel.Controls.Clear();
            //this.oppanel.Visible = false;
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
    }

    

}
