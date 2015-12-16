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
using TradingLib.Protocol;

namespace FutsMoniter
{
    public partial class fmLoginInfo : ComponentFactory.Krypton.Toolkit.KryptonForm, IEventBinder
    {
        public fmLoginInfo()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmDomainRootLoginInfo_Load);
        }

        void fmDomainRootLoginInfo_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {
            if (_domain != null)
            {
                Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryDomainRootLoginInfo", this.OnLoginInfo);
                Globals.TLClient.ReqQryDomainRootLoginInfo(_domain.ID);
            }

            if (_manager != null)
            {
                Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryManagerLoginInfo", this.OnLoginInfo);
                Globals.TLClient.ReqQryManagerLoginInfo(_manager.ID);
            }

            if (_account != null)
            {
                Globals.LogicEvent.RegisterCallback("AccountManager", "QryAccountLoginInfo", this.OnLoginInfo);
                Globals.TLClient.ReqQryAccountLoginInfo(_account.Account);
            }

        }

        public void OnDisposed()
        {
            if (_domain != null)
            {
                Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryDomainRootLoginInfo", this.OnLoginInfo);
            }
            if (_manager != null)
            {
                Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryManagerLoginInfo", this.OnLoginInfo);
            }

            if (_account != null)
            {
                Globals.LogicEvent.UnRegisterCallback("AccountManager", "QryAccountLoginInfo", this.OnLoginInfo);
            }
        }

        void OnLoginInfo(string json, bool islast)
        {
            LoginInfo info = MoniterUtils.ParseJsonResponse<LoginInfo>(json);
            if (json != null)
            {
                loginname.Text = info.LoginID;
                password.Text = info.Pass;
            }
        }
        DomainImpl _domain = null;
        public void SetDomain(DomainImpl domain)
        {
            _domain = domain;
        }

        ManagerSetting _manager = null;

        public void SetManager(ManagerSetting manger)
        {
            _manager = manger;
        }

        AccountLite _account = null;
        public void SetAccount(AccountLite account)
        {
            _account = account;
        }
    }
}
