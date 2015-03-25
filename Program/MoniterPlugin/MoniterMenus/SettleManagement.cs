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
    /// <summary>
    /// 结算管理
    /// 绑定到对应的菜单项 用于调出对应的管理模块进行操作
    /// </summary>
    [MoniterCommandAttr("51E22D75-F1E2-4032-8121-C8C3B720F5AF", "demo", "测试菜单")]
    public class SettleManagement:MoniterCommand
    {

        public override void OnCommand(object sender, EventArgs e)
        {
            MessageBox.Show("first command");
        }
    }
}
