using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutsMoniter.Common;

namespace FutsMoniter
{
    partial class MainForm
    {
        /// <summary>
        /// 初始化成功后回调 用于初始化数据
        /// </summary>
        public void OnInitFinished()
        {
            Globals.Debug("Manager Name:" + Globals.Manager.Name + " BaseFK:" + Globals.Manager.mgr_fk + " MgrType:" + Globals.Manager.Type.ToString());

            if (!Globals.Manager.RightRootDomain())
            {
                //btnGPSystem.Enabled = false;
                //btnGPSymbol.Enabled = false;
            }

        }
    }
}
