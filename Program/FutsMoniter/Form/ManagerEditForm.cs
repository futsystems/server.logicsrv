using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class ManagerEditForm : Telerik.WinControls.UI.RadForm
    {
        Manager manger = null;
        public Manager Manager
        {
            set
            {
                manger = value;
                this.Text = " 编辑管理员:" + manger.Name;
                this.id.Text = manger.ID.ToString();
                this.mgr_fk.Text = manger.mgr_fk.ToString();
                this.login.Text = manger.Login;
                this.name.Text = manger.Name;
                this.mobile.Text = manger.Mobile;
                this.qq.Text = manger.QQ;
                this.acclimit.Value = manger.AccLimit;
                this.type.Enabled = false;
                this.acclimit.Enabled = false;
                if (manger.Type == QSEnumManagerType.AGENT)
                {
                    this.acclimit.Enabled = true;
                }
            }
        }
        public ManagerEditForm()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(type).BindDataSource(Utils.GetManagerTypeCBList());
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (manger == null)
            {
                Manager m = new Manager();
                m.Type = (QSEnumManagerType)type.SelectedValue;
                m.Login = this.login.Text;
                m.Name = this.name.Text;
                m.Mobile = this.mobile.Text;
                m.QQ = this.qq.Text;
                m.AccLimit = (int)this.acclimit.Value;
                if (fmConfirm.Show("确认添加管理员信息?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateManager(m);
                }

            }
            else
            {
                //更新
                manger.Login = this.login.Text;
                manger.Name = this.name.Text;
                manger.Mobile = this.mobile.Text;
                manger.QQ = this.qq.Text;

                if (manger.Type == QSEnumManagerType.AGENT)
                {
                    manger.AccLimit = (int)this.acclimit.Value;
                }

                if (fmConfirm.Show("确认更新管理员信息?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateManager(manger);
                }
               
            }
            this.Close();
        }
    }
}
