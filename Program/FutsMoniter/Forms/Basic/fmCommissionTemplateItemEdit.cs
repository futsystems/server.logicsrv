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


namespace FutsMoniter
{
    public partial class fmCommissionTemplateItemEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmCommissionTemplateItemEdit()
        {
            InitializeComponent();
            bymoney.Checked = true;
            Factory.IDataSourceFactory(chargetype).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumChargeType>(false));
            for (int i = 1; i <=12; i++)
            {
                month.Items.Add(i);
            }
            month.SelectedIndex = 0;
            PrepareInput();
            this.Load += new EventHandler(fmCommissionTemplateItemEdit_Load);
        }


        void fmCommissionTemplateItemEdit_Load(object sender, EventArgs e)
        {
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            bymoney.CheckedChanged += new EventHandler(bymoney_CheckedChanged);
            byvolume.CheckedChanged += new EventHandler(byvolume_CheckedChanged);

        }

        void PrepareInput()
        {
            if (byvolume.Checked)
            {
                openbymoney.Value = 0;
                openbymoney.Enabled = false;
                closetodaybymoney.Value = 0;
                closetodaybymoney.Enabled = false;
                closebymoney.Value = 0;
                closebymoney.Enabled = false;

                openbyvolume.Enabled = true;
                closetodaybyvolume.Enabled = true;
                closebyvolume.Enabled = true;
            }

            if (bymoney.Checked)
            {
                openbyvolume.Value = 0;
                closetodaybyvolume.Value = 0;
                closebyvolume.Value = 0;

                openbyvolume.Enabled = false;
                closetodaybyvolume.Enabled = false;
                closebyvolume.Enabled = false;

                openbymoney.Enabled = true;
                closetodaybymoney.Enabled = true;
                closebymoney.Enabled = true;
            }
        }
        void byvolume_CheckedChanged(object sender, EventArgs e)
        {
            PrepareInput();
        }

        void bymoney_CheckedChanged(object sender, EventArgs e)
        {
            PrepareInput();
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (openbymoney.Value >= 1 || closetodaybymoney.Value >= 1 || closebymoney.Value >= 1)
            {
                MoniterUtils.WindowMessage("按金额手续费率必须小于1");
                return;
            }
            //if (openbymoney.Value == 0)
            //{
            //    if ((closebymoney.Value != 0) || (closetodaybymoney.Value != 0))
            //    { 
            //        MoniterUtils.WindowMessage("按金额")
            //    }
            //}
            if (_item != null)//更新
            {
                //_item.Template_ID = _templateid;
                _item.OpenByMoney = openbymoney.Value;
                _item.OpenByVolume = openbyvolume.Value;
                _item.CloseTodayByMoney = closetodaybymoney.Value;
                _item.CloseTodayByVolume = closetodaybyvolume.Value;
                _item.CloseByMoney = closebymoney.Value;
                _item.CloseByVolume = closebyvolume.Value;
                _item.ChargeType = (QSEnumChargeType)chargetype.SelectedValue;
                _item.Month = month.SelectedIndex + 1;
                Globals.TLClient.ReqUpdateCommissionTemplateItem(_item);

            }
            else//添加
            {
                CommissionTemplateItemSetting item = new CommissionTemplateItemSetting();
                item.ChargeType = (QSEnumChargeType)chargetype.SelectedValue;
                item.Code = code.Text;
                item.Month = 1;
                item.OpenByMoney = openbymoney.Value;
                item.OpenByVolume = openbyvolume.Value;
                item.CloseTodayByMoney = closetodaybymoney.Value;
                item.CloseTodayByVolume = closetodaybyvolume.Value;
                item.CloseByMoney = closebymoney.Value;
                item.CloseByVolume = closebyvolume.Value;
                item.ChargeType = (QSEnumChargeType)chargetype.SelectedValue;
                item.Month = month.SelectedIndex+1;
                item.Template_ID = _templateid;
                Globals.TLClient.ReqUpdateCommissionTemplateItem(item);
                
            }

            this.Close();
        }

        int _templateid = 0;
        public void SetCommissionTemplateID(int id)
        {
            _templateid = id;
        }

        CommissionTemplateItemSetting _item = null;
        public void SetCommissioinTemplateItem(CommissionTemplateItemSetting item)
        {
            _item = item;
            id.Text = _item.ID.ToString();
            code.Text = _item.Code;
            openbymoney.Value = _item.OpenByMoney;
            openbyvolume.Value = _item.OpenByVolume;
            closetodaybymoney.Value = _item.CloseTodayByMoney;
            closetodaybyvolume.Value = _item.CloseTodayByVolume;
            closebymoney.Value = _item.CloseByMoney;
            closebyvolume.Value = _item.CloseByVolume;
            month.SelectedIndex = _item.Month-1;

            code.Enabled = false;
            month.Enabled = false;

            if (_item.OpenByVolume == 0)
            {
                bymoney.Checked = true;
            }
            else
            {
                byvolume.Checked = true;
            }
            PrepareInput();
        }
    }
}
