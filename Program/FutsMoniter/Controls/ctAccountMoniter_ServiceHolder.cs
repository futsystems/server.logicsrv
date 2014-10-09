using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;


namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier
    {
        void ServiceTabRefresh()
        {
            ServiceTabHolder_SelectedPageChanged(null, null);
        }

        /// <summary>
        /// 右下角Tab切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceTabHolder_SelectedPageChanged(object sender, EventArgs e)
        {
            if (ServiceTabHolder.SelectedPage.Name.Equals("FinServicePage"))
            {
                if (AccountSetlected != null)
                {
                    Globals.TLClient.ReqQryFinService(AccountSetlected.Account);
                }
            }
        }
    }
}
