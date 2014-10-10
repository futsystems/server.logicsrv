using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    public partial class ctOptimizationProgress : UserControl
    {
        public ctOptimizationProgress(int id)
        {
            InitializeComponent();
            worker.Text = "线程#" + id;

        }

        public void GotProgressItem(OptimizationPlugin.ProgressItem item)
        {
            message.Text = item.Text;
            int p = (int)(item.Progress * 100);
            //debug(p.ToString() +"cu:" + currentItem.ToString() + " to:" + totalItems.ToString());
            if (p > 100) p = 100;
            if (p < 0) p = 0;

            progress.Value = p;
            progress.Invalidate();
        }
    }
}
