using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Strategy.GUI
{
    public partial class MemoTargetProfit : KryptonForm
    {
        public MemoTargetProfit()
        {
            InitializeComponent();
        }

        const int WM_NCLBUTTONDBLCLK = 0x00A3;
        //private bool _winhide = false;
        private int _winheight = 0;
        protected override void WndProc(ref Message m)
        {

            if (m.WParam.ToInt32() == 2 && m.Msg == WM_NCLBUTTONDBLCLK)
            {
                return;
            }
            base.WndProc(ref m);
        }
    }
}
