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
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;

namespace FutsMoniter
{
    public partial class fmDomainEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmDomainEdit()
        {
            InitializeComponent();
            this.Text = "添加域分区";
        }

        DomainImpl _domain = null;
        public void SetDomain(DomainImpl domain)
        {
            _domain = domain;
            domainid.Text = _domain.ID.ToString();
            name.Text = _domain.Name;
            linkman.Text = _domain.LinkMan;
            mobile.Text = _domain.Mobile;
            qq.Text = _domain.QQ;
            email.Text = _domain.Email;

            dateexpired.Value = _domain.DateExpired==0? (DateTime.Now+ new TimeSpan(1000,0,0,0,0)):Util.ToDateTime(_domain.DateExpired, 1);
            acclimit.Value = _domain.AccLimit==0?acclimit.Maximum:_domain.AccLimit;
            routergrouplimit.Value = _domain.RouterGroupLimit == 0 ? routergrouplimit.Maximum : _domain.RouterGroupLimit;
            routeritemlimit.Value = _domain.RouterItemLimit == 0 ? routeritemlimit.Maximum : _domain.RouterItemLimit;
            this.Text = "编辑域分区:" + _domain.Name;

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_domain != null)
            {
                _domain.Name = name.Text;
                _domain.LinkMan = linkman.Text;
                _domain.Mobile = mobile.Text;
                _domain.QQ = qq.Text;
                _domain.Email = email.Text;

                _domain.DateExpired = Util.ToTLDate(dateexpired.Value);
                _domain.AccLimit = (int)acclimit.Value;
                _domain.RouterGroupLimit = (int)routergrouplimit.Value;
                _domain.RouterItemLimit = (int)routeritemlimit.Value;
                if (fmConfirm.Show("确认更新分区设置?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateDomain(_domain);
                    this.Close();
                }
            }
            else
            {
                _domain = new DomainImpl();
                _domain.Name = name.Text;
                _domain.LinkMan = linkman.Text;
                _domain.Mobile = mobile.Text;
                _domain.QQ = qq.Text;
                _domain.Email = email.Text;

                _domain.DateExpired = Util.ToTLDate(dateexpired.Value);
                _domain.AccLimit = (int)acclimit.Value;
                _domain.RouterGroupLimit = (int)routergrouplimit.Value;
                _domain.RouterItemLimit = (int)routeritemlimit.Value;
                if (fmConfirm.Show("确认添加分区?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateDomain(_domain);
                    this.Close();
                }
            }

        }
    }
}
