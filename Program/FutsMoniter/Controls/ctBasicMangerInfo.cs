using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class ctBasicMangerInfo : UserControl,IEventBinder
    {
        public ctBasicMangerInfo()
        {
            InitializeComponent();
            this.btnChangePass.Click +=new EventHandler(btnChangePass_Click);
        }

        public void OnInit()
        {
            InitAgentList();
        }

        public void OnDisposed()
        { 
            
        }

        void InitAgentList()
        {
            lbbasemgrfk.Text = Globals.LoginResponse.BaseMGRFK.ToString();
            lblogin.Text = Globals.LoginResponse.LoginID;
            lbname.Text = Globals.LoginResponse.Name;
            lbmobile.Text = Globals.LoginResponse.Mobile;
            lbqq.Text = Globals.LoginResponse.QQ;
            lbrole.Text = Util.GetEnumDescription(Globals.LoginResponse.ManagerType);
        }

        /// <summary>
        /// 响应环境初始化完成事件 用于在环境初始化之前创立的空间获得对应的基础数据
        /// </summary>
        public void OnInitFinished()
        {
            InitAgentList();
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            fmChangePasswordAgent fm = new fmChangePasswordAgent();
            fm.ShowDialog();
        }
    }
}
