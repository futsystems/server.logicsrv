using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class FeeConfigForm : Telerik.WinControls.UI.RadForm
    {
        public FeeConfigForm()
        {
            InitializeComponent();

           
        }

        private void FeeConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }


        ~FeeConfigForm()
        {
            Globals.Debug("Finialze called ~~~~~~~~");
        }


        

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            
        }

        
    }
}
