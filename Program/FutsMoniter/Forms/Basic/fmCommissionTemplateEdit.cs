using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter
{
    public partial class fmCommissionTemplateEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmCommissionTemplateEdit()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmCommissionTemplateEdit_Load);
        }

        void fmCommissionTemplateEdit_Load(object sender, EventArgs e)
        {
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        CommissionTemplateSetting _template = null;
        public void SetCommissionTemplate(CommissionTemplateSetting t)
        {
            _template = t;
            name.Text = t.Name;
            desp.Text = t.Description;
            id.Text = t.ID.ToString();
            this.Text = "编辑手续费模板";
        }
        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_template == null)
            {
                CommissionTemplateSetting target = new CommissionTemplateSetting();
                target.Name = name.Text;
                target.Description = desp.Text;

                Globals.TLClient.ReqUpdateCommissionTemplate(target);
            }
            else
            {
                _template.Name = name.Text;
                _template.Description = desp.Text;

                Globals.TLClient.ReqUpdateCommissionTemplate(_template);
               
            }

            this.Close();
        }
    }
}
