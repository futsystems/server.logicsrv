using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class fmBindConnector :ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmBindConnector()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmChangeRouter_Load);
            //this.ctRouterGroupList1.RouterGroupInitEvent += new VoidDelegate(ctRouterGroupList1_RouterGroupInitEvent);
            
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("BrokerRouterPassThrough", "QryAccountConnectorPair", this.OnAccountConnecorPair);
            Globals.LogicEvent.RegisterCallback("BrokerRouterPassThrough", "QryAvabileConnectors", this.OnAvabileConnectors);

            Globals.TLClient.ReqQryAccountConnectorPair(_account.Account);
            Globals.TLClient.ReqQryAvabileConnectors();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("BrokerRouterPassThrough", "QryAccountConnectorPair", this.OnAccountConnecorPair);
            Globals.LogicEvent.UnRegisterCallback("BrokerRouterPassThrough", "QryAvabileConnectors", this.OnAvabileConnectors);
        }


        void OnAvabileConnectors(string json, bool islast)
        {
            cbConnectorList.Items.Clear();

            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(json)["Payload"];

            ArrayList list = new ArrayList();

            foreach (TradingLib.Mixins.Json.JsonData connector in data)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = string.Format("{0}-{1}", connector["Token"].ToString(), connector["LoginID"].ToString());
                vo.Value = int.Parse(connector["ConnectorID"].ToString());
                list.Add(vo);
                Globals.Debug("????????????????? got avabile list:" + connector["Token"].ToString());
            }
            Factory.IDataSourceFactory(cbConnectorList).BindDataSource(list);
        
        }
        void OnAccountConnecorPair(string json,bool islast)
        {
            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(json)["Payload"];

            string account = data["Account"].ToString();
            int connector_id = int.Parse(data["ConnectorID"].ToString());
            string token = data["Token"].ToString();

            cutrgname.Text = string.IsNullOrEmpty(token) ? "未绑定" : token;
            if (string.IsNullOrEmpty(token))
            {
                btnDelAccountConnector.Enabled = false;
            }

            Globals.Debug("????????????????? got account pair:" + json);
        }


        void fmChangeRouter_Load(object sender, EventArgs e)
        {
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            btnDelAccountConnector.Click += new EventHandler(btnDelAccountConnector_Click);
            Globals.RegIEventHandler(this);
        }

        void btnDelAccountConnector_Click(object sender, EventArgs e)
        {
            string msg = string.Format("确认从帐户:{0} 解绑主帐户:{1}", _account.Account,cutrgname.Text);
            if (ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show(msg, "确认操作", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqDelAccountConnector(_account.Account);
                this.Close();
            }
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            var data = (ValueObject<int>)cbConnectorList.SelectedItem;

            string msg = string.Format("确认绑定主帐户:{0}到帐户:{1}",data.Name,_account.Account);

            if (ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show(msg,"确认操作",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqUpdateAccountConnector(_account.Account, data.Value);
                this.Close();
            }
        }

        AccountLite _account = null;
        public void SetAccount(AccountLite account)
        {
            _account = account;
            this.Text = string.Format("修改帐户[{0}]主帐户绑定", _account.Account);
            //RouterGroupSetting rg = Globals.BasicInfoTracker.GetRouterGroup(_account.RG_ID);
            //cutrgname.Text = (rg != null ? rg.Name : "未设置");
        }




    }
}
