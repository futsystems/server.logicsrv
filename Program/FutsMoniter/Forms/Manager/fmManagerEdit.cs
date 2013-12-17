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


namespace FutsMoniter
{
    public partial class fmManagerEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmManagerEdit()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(type).BindDataSource(MoniterUtils.GetManagerTypeCBList());
        }


        System.Text.RegularExpressions.Regex reglogin = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");

        ManagerSetting _manger = null;
        public void SetManger(ManagerSetting mgr)
        {
            _manger = mgr;
            this.Text = " 编辑管理员:" + _manger.Name;
            this.id.Text = _manger.ID.ToString();
            this.mgr_fk.Text = _manger.mgr_fk.ToString();
            this.login.Text = _manger.Login;
            this.name.Text = _manger.Name;
            this.mobile.Text = _manger.Mobile;
            this.qq.Text = _manger.QQ;
            this.acclimit.Value = _manger.AccLimit;
            this.type.Enabled = false;
            this.login.Enabled = false;
            this.acclimit.Enabled = false;

            //如果是代理商则可以修改帐户数量限制 同时设定限制为自己的限制 给代理的客户数量不能超过过自己的限制
            if (_manger.Type == QSEnumManagerType.AGENT)
            {
                this.acclimit.Enabled = true;
                this.acclimit.Maximum = Globals.Manager.AccLimit;
            }
        }
        //public Manager Manager
        //{
        //    set
        //    {
        //        _manger = value;
        //        this.Text = " 编辑管理员:" + manger.Name;
        //        this.id.Text = manger.ID.ToString();
        //        this.mgr_fk.Text = manger.mgr_fk.ToString();
        //        this.login.Text = manger.Login;
        //        this.name.Text = manger.Name;
        //        this.mobile.Text = manger.Mobile;
        //        this.qq.Text = manger.QQ;
        //        this.acclimit.Value = manger.AccLimit;
        //        this.type.Enabled = false;
        //        this.login.Enabled = false;
        //        this.acclimit.Enabled = false;

        //        //如果是代理商则可以修改帐户数量限制 同时设定限制为自己的限制 给代理的客户数量不能超过过自己的限制
        //        if (manger.Type == QSEnumManagerType.AGENT)
        //        {
        //            this.acclimit.Enabled = true;
        //            this.acclimit.Maximum = Globals.Manager.AccLimit;
        //        }
        //    }
        //}

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Globals.RightAddManger)
            {
                fmConfirm.Show("没有添加柜员的权限");
            }
            if (_manger == null)
            {
                Manager m = new Manager();
                m.Type = (QSEnumManagerType)type.SelectedValue;
                m.Login = this.login.Text;
                if (string.IsNullOrEmpty(m.Login))
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请输入登入名");
                    return;
                }
                if (!InputReg.Login.IsMatch(m.Login))
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("登入ID只能包含数字和字母,-");
                    return;
                }
                if (m.Login.Length > 12)
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("登入名长度不能大于12位");
                    return;
                }
                m.Name = this.name.Text;
                m.Mobile = this.mobile.Text;
                m.QQ = this.qq.Text;
                m.AccLimit = (int)this.acclimit.Value;
                if (Globals.Manager.Type == QSEnumManagerType.ROOT)
                {
                    if (m.Type == QSEnumManagerType.AGENT)//如果添加代理则mgr_fk=0
                    {
                        m.mgr_fk = 0;
                    }
                }
                if (Globals.Manager.Type == QSEnumManagerType.AGENT)
                {
                    m.mgr_fk = Globals.Manager.mgr_fk;
                }
                if (fmConfirm.Show("确认添加管理员信息?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateManager(m);
                }

            }
            else
            {
                //更新
                _manger.Login = this.login.Text;
                _manger.Name = this.name.Text;
                _manger.Mobile = this.mobile.Text;
                _manger.QQ = this.qq.Text;

                if (_manger.Type == QSEnumManagerType.AGENT)
                {
                    _manger.AccLimit = (int)this.acclimit.Value;
                }

                MessageBox.Show("manager id:" + _manger.ID.ToString());
                if (fmConfirm.Show("确认更新管理员信息?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateManager(_manger);
                }

            }
            this.Close();
        }
    }
}
