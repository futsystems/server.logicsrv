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
using FutSystems.GUI;
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;


namespace FutsMoniter
{
    public partial class fmRecvBankEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmRecvBankEdit()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmRecvBankEdit_Load);
        }

        void fmRecvBankEdit_Load(object sender, EventArgs e)
        {
            
            WireEvent();
        }

        void WireEvent()
        {
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_bank == null)
            {
                if (fmConfirm.Show("确认添加银行帐号?") == System.Windows.Forms.DialogResult.Yes)
                {
                    JsonWrapperReceivableAccount bank = new JsonWrapperReceivableAccount();
                    bank.Name = name.Text;
                    bank.Bank_AC = bankac.Text;
                    bank.Bank_ID = ctBankList1.BankSelected;
                    bank.Branch = branch.Text;

                    Globals.TLClient.ReqUpdateRecvBank(bank);
                    this.Close();
                }
            }
            else
            {
                if (fmConfirm.Show("确认更新银行帐号?") == System.Windows.Forms.DialogResult.Yes)
                {
                    
                    _bank.Name = name.Text;
                    _bank.Bank_AC = bankac.Text;
                    _bank.Bank_ID = ctBankList1.BankSelected;
                    _bank.Branch = branch.Text;

                    Globals.TLClient.ReqUpdateRecvBank(_bank);
                    this.Close();
                }
            }
        }
        JsonWrapperReceivableAccount _bank;
        public void SetRecvBank(JsonWrapperReceivableAccount bank)
        {
            this.Text = "编辑银行帐号";

            _bank = bank;

            id.Text = bank.ID.ToString();
            name.Text = bank.Name;
            branch.Text = bank.Branch;
            bankac.Text = bank.Bank_AC;
            ctBankList1.BankSelected = _bank.Bank_ID;
            
        }


    }
}
