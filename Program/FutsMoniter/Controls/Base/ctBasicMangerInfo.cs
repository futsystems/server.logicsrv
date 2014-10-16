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
    public partial class ctBasicMangerInfo : UserControl
    {
        public ctBasicMangerInfo()
        {
            InitializeComponent();
            //如果已经初始化完成 则直接读取数据填充 否则将资金放入事件回调中
            if (Globals.EnvReady)
            {
                InitAgentList();
            }
            else
            {
                if (Globals.CallbackCentreReady)
                {
                    Globals.RegInitCallback(OnInitFinished);
                }
            }
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

        private void btnChagnePass_Click(object sender, EventArgs e)
        {
            UpdatePassForm fm = new UpdatePassForm();
            fm.ShowDialog();
        }
    }
}
