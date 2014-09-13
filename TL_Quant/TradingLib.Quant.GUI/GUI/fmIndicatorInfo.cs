using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.GUI
{
    public partial class fmIndicatorInfo : KryptonForm
    {
        public fmIndicatorInfo()
        {
            InitializeComponent();
        }

        public void ViewIndicator(IIndicatorPlugin plugin)
        {
            try
            {
                name.Text = plugin.GetName();
                author.Text = plugin.GetAuthor();
                group.Text = plugin.GetGroupName();
                company.Text = plugin.GetCompanyName();
                desp.Text = plugin.GetDescription();

            }
            catch (Exception ex)
            { 
                
            }
        }
    }
}
