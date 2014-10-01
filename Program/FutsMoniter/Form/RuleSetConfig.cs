using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class RuleSetConfig : Telerik.WinControls.UI.RadForm
    {
        public IAccountLite Account { get; set; }

        RuleClassItem _ruleclass = null;
        public RuleClassItem RuleClass { get { return _ruleclass; }

            set
            {
                _ruleclass = value;
                ruletitle.Text = _ruleclass.Title;
                rulecheckargname.Text = _ruleclass.ValueName;
                Globals.Debug("rule desp:" + _ruleclass.Description);
                description.Text = _ruleclass.Description;
            }
        
        }

        RuleItem _rule = null;
        public RuleItem Rule { get { return _rule; }
            set
            {
                _rule = value;
                RuleClassItem klass = Globals.BasicInfoTracker.GetRuleItemClass(_rule);
                ruletitle.Text = klass.Title;
                rulecheckargname.Text = klass.ValueName;
                description.Text = klass.Description;

                argvalue.Text = _rule.Value;
                comparetype.SelectedValue = _rule.Compare;
                symbolset.Text = _rule.SymbolSet;
            }
        }


        public RuleSetConfig()
        {
            InitializeComponent();
            //_ruleclass = new RuleClassItem();
            Factory.IDataSourceFactory(comparetype).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumCompareType>());
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("rule==null:" + (this.Rule == null).ToString() + " ruleclass==null:" + (this.RuleClass == null).ToString());
            if (this.Rule == null)
            {
                RuleItem item = new RuleItem();
                item.Account = this.Account.Account;
                item.Compare = (QSEnumCompareType)comparetype.SelectedValue;
                item.Enable = true;
                item.RuleName = _ruleclass.ClassName;
                item.RuleType = _ruleclass.Type;
                item.SymbolSet = symbolset.Text;
                item.Value = argvalue.Text;

                if (fmConfirm.Show("确添加帐户风控规则?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateRuleSet(item);
                }
            }
            else if (RuleClass == null)
            {
                this.Rule.SymbolSet = symbolset.Text;
                this.Rule.Compare = (QSEnumCompareType)comparetype.SelectedValue;
                this.Rule.Value = argvalue.Text;

                if (fmConfirm.Show("确认更新帐户风控规则?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateRuleSet(this.Rule);
                }
            }
            //MessageBox.Show("it is here");
            this.Close();
        }
    }
}
