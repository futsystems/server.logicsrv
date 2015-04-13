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
    public enum TemplateEditType
    { 
        Margin,
        Commission,
        Strategy
    }
    public partial class fmTemplateEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        TemplateEditType _type;
        public fmTemplateEdit(TemplateEditType type)
        {
            _type = type;
            InitializeComponent();
            this.Load += new EventHandler(fmCommissionTemplateEdit_Load);
        }

        void fmCommissionTemplateEdit_Load(object sender, EventArgs e)
        {
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        CommissionTemplateSetting _commissionTemplate = null;
        MarginTemplateSetting _marginTemplate = null;
        ExStrategyTemplateSetting _exstrategyTemplate = null;

        bool isedit = false;
        public void SetTemplate(object t)
        {
            isedit = true;
            if (_type == TemplateEditType.Commission)
            {
                if(t is CommissionTemplateSetting)
                {
                    _commissionTemplate = t as CommissionTemplateSetting;
                    name.Text = _commissionTemplate.Name;
                    desp.Text = _commissionTemplate.Description;
                    id.Text = _commissionTemplate.ID.ToString();
                    this.Text = "编辑手续费模板";
                }
                else
                {
                    MoniterUtils.WindowMessage("请设定要编辑的手续费模板");
                }
            }
            else if (_type == TemplateEditType.Margin)
            {
                if (t is MarginTemplateSetting)
                {
                    _marginTemplate = t as MarginTemplateSetting;
                    name.Text = _marginTemplate.Name;
                    desp.Text = _marginTemplate.Description;
                    id.Text = _marginTemplate.ID.ToString();
                    this.Text = "编辑保证金模板";
                }
                else
                {
                    MoniterUtils.WindowMessage("请设定要编辑的保证金模板");
                }
            }
            else if (_type == TemplateEditType.Strategy)
            {
                if (t is ExStrategyTemplateSetting)
                {
                    _exstrategyTemplate = t as ExStrategyTemplateSetting;
                    name.Text = _exstrategyTemplate.Name;
                    desp.Text = _exstrategyTemplate.Description;
                    id.Text = _exstrategyTemplate.ID.ToString();
                    this.Text = "标记交易参数模板";
                }
            }

           
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_type == TemplateEditType.Commission)
            {
                if (_commissionTemplate == null)
                {
                    CommissionTemplateSetting target = new CommissionTemplateSetting();
                    target.Name = name.Text;
                    target.Description = desp.Text;

                    Globals.TLClient.ReqUpdateCommissionTemplate(target);
                }
                else
                {
                    _commissionTemplate.Name = name.Text;
                    _commissionTemplate.Description = desp.Text;
                    Globals.TLClient.ReqUpdateCommissionTemplate(_commissionTemplate);
                }
            }
            else if (_type == TemplateEditType.Margin)
            {
                if (_marginTemplate == null)
                {
                    MarginTemplateSetting target = new MarginTemplateSetting();
                    target.Name = name.Text;
                    target.Description = desp.Text;

                    Globals.TLClient.ReqUpdateMarginTemplate(target);
                }
                else
                {
                    _marginTemplate.Name = name.Text;
                    _marginTemplate.Description = desp.Text;
                    Globals.TLClient.ReqUpdateMarginTemplate(_marginTemplate);
                }
            }
            else if (_type == TemplateEditType.Strategy)
            {
                if (_exstrategyTemplate == null)
                {
                    ExStrategyTemplateSetting target = new ExStrategyTemplateSetting();
                    target.Name = name.Text;
                    target.Description = desp.Text;

                    Globals.TLClient.ReqUpdateExStrategyTemplate(target);
                }
                else
                {
                    _exstrategyTemplate.Name = name.Text;
                    _exstrategyTemplate.Description = desp.Text;

                    Globals.TLClient.ReqUpdateExStrategyTemplate(_exstrategyTemplate);
                }
            }

            this.Close();
        }
    }
}
