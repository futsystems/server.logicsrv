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
using TradingLib.Protocol;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class fmMarginTemplateItemEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmMarginTemplateItemEdit()
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
            chargetype.SelectedIndexChanged += new EventHandler(chargetype_SelectedIndexChanged);
            setAllCodeMonth.CheckedChanged += new EventHandler(setAllCodeMonth_CheckedChanged);
            setAllMonth.CheckedChanged += new EventHandler(setAllMonth_CheckedChanged);
        }

        void setAllMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (setAllCodeMonth.Checked)
            {
                setAllCodeMonth.Checked = false;
            }
        }

        void setAllCodeMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (setAllMonth.Checked)
            {
                setAllMonth.Checked = false;
            }
        }

        void chargetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrepareInput();
        }

        void PrepareInput()
        {
            QSEnumChargeType type = (QSEnumChargeType)chargetype.SelectedValue;
            if (type == QSEnumChargeType.Percent)
            {
                gbConfig.Enabled = false;
                gbPercent.Enabled = true;

                marginbyvolume.Value = 0;
                marginbymoney.Value = 0;

            }
            else
            {
                gbConfig.Enabled = true;
                gbPercent.Enabled = false;

                percent.Value = 0;
            }

            if (byvolume.Checked)
            {
                marginbymoney.Value = 0;
                marginbymoney.Enabled = false;
                marginbyvolume.Enabled = true;
            }

            if (bymoney.Checked)
            {
                marginbyvolume.Value = 0;
                marginbyvolume.Enabled = false;
                marginbymoney.Enabled = true;
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
            if (marginbymoney.Value >= 1)
            {
                MoniterUtils.WindowMessage("按金额手续费率必须小于1");
                return;
            }
            if (_item != null)//更新
            {
                _item.MarginByMoney = marginbymoney.Value;
                _item.MarginByVolume = marginbyvolume.Value;
                _item.ChargeType = (QSEnumChargeType)chargetype.SelectedValue;
                _item.Percent = percent.Value;

                MGRMarginTemplateItemSetting cfg = new MGRMarginTemplateItemSetting(_item);
                cfg.SetAllMonth = setAllMonth.Checked;
                cfg.SetAllCodeMonth = setAllCodeMonth.Checked;
                Globals.TLClient.ReqUpdateCommissionTemplateItem(cfg);
            }
            else//添加
            {
                MarginTemplateItemSetting item = new MarginTemplateItemSetting();
                item.ChargeType = (QSEnumChargeType)chargetype.SelectedValue;
                item.Code = code.Text;
                item.Month = 1;
                item.MarginByMoney = marginbymoney.Value;
                item.MarginByVolume = marginbyvolume.Value;
                item.ChargeType = (QSEnumChargeType)chargetype.SelectedValue;
                item.Month = month.SelectedIndex+1;
                item.Template_ID = _templateid;
                item.Percent = percent.Value;

                MGRMarginTemplateItemSetting cfg = new MGRMarginTemplateItemSetting(item);
                cfg.SetAllMonth = setAllMonth.Checked;
                cfg.SetAllCodeMonth = setAllCodeMonth.Checked;
                Globals.TLClient.ReqUpdateCommissionTemplateItem(cfg);
                
            }

            this.Close();
        }

        int _templateid = 0;
        public void SetCommissionTemplateID(int id)
        {
            _templateid = id;
        }

        MarginTemplateItemSetting _item = null;
        public void SetMarginTemplateItem(MarginTemplateItemSetting item)
        {
            _item = item;
            id.Text = _item.ID.ToString();
            code.Text = _item.Code;
            marginbymoney.Value = _item.MarginByMoney;
            marginbyvolume.Value = _item.MarginByVolume;
            
            percent.Value = _item.Percent;
            month.SelectedIndex = _item.Month-1;
            chargetype.SelectedValue = _item.ChargeType;
            code.Enabled = false;
            month.Enabled = false;
            //MessageBox.Show(TradingLib.Mixins.Json.JsonMapper.ToJson(_item));
            if (_item.MarginByVolume == 0)
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
