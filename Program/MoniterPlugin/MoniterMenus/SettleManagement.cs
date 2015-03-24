using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MoniterControl;

namespace MoniterMenus
{
    [MoniterCommandAttr("", "demo", "测试菜单")]
    public class SettleManagement:MoniterCommand
    {

        public override void OnCommand(object sender, EventArgs e)
        {
            MessageBox.Show("first command");
        }
    }
}
