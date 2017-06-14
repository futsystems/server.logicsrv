using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;

namespace TradingLib.Quant.GUI
{
    public partial class fmPropertiesWindow : DockContent
    {
        public fmPropertiesWindow()
        {
            InitializeComponent();
            this.FormClosing +=new FormClosingEventHandler(fmPropertiesWindow_FormClosing);
        }

        void  fmPropertiesWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
 	        e.Cancel = true;
        }

         public void ShowIndicatorProperties(IndicatorInfo info)
        {
            this.ShowIndicatorProperties(info, null, null, null);
        }
        public void ShowIndicatorProperties(IndicatorInfo info, IList<string> availableInputs, IndicatorInfo.ChangeDelegate changeHandler, string objectOwner)
        {
            propertiesWindow1.ShowIndicatorProperties(info,availableInputs,changeHandler,objectOwner);

        }

        public void ShowStrategyProjectProperties(StrategySetting setting)
        {
            propertiesWindow1.ShowStrategyProject(setting);
        }
    }
}
