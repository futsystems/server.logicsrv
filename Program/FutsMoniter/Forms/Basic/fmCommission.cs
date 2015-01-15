﻿using System;
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
    public partial class fmCommission : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmCommission()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
            this.Load += new EventHandler(fmCommission_Load);
        }

        void fmCommission_Load(object sender, EventArgs e)
        {
            this.templatelist.ContextMenuStrip = new ContextMenuStrip();
            this.templatelist.ContextMenuStrip.Items.Add("添加模板", null, new EventHandler(Add_Click));
            this.templatelist.ContextMenuStrip.Items.Add("修改模板", null, new EventHandler(Edit_Click));
            this.templatelist.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            this.templatelist.ContextMenuStrip.Items.Add("加载模板", null, new EventHandler(Qry_Click));

            //this.templatelist.MouseDoubleClick += new MouseEventHandler(templatelist_MouseDoubleClick);
            Globals.RegIEventHandler(this);
            //this.templatelist.ContextMenuStrip.Items.Add("添加模板", null, new EventHandler(Add_Click));
        }

        void templatelist_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CommissionTemplateSetting t = templatelist.SelectedItem as CommissionTemplateSetting;
            templatename.Text = t.Name;
        }

        void Qry_Click(object sender, EventArgs e)
        {
            CommissionTemplateSetting t = templatelist.SelectedItem as CommissionTemplateSetting;
            if (t == null)
            {
                MoniterUtils.WindowMessage("请选择手续费模板");
                return;
            }
            templatename.Text = t.Name;
            Globals.TLClient.ReqQryCommissionTemplateItem(t.ID);
        }
        void Add_Click(object sender, EventArgs e)
        {
            fmCommissionTemplateEdit fm = new fmCommissionTemplateEdit();
            fm.ShowDialog();
        }
        void Edit_Click(object sender, EventArgs e)
        {
            CommissionTemplateSetting t = templatelist.SelectedItem as CommissionTemplateSetting;
            fmCommissionTemplateEdit fm = new fmCommissionTemplateEdit();

            fm.SetCommissionTemplate(t);
            fm.ShowDialog();
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryCommissionTemplate", this.OnQryCommissionTemplate);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyCommissionTemplate", this.OnNotifyCommissionTemplate);

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryCommissionTemplateItem", this.OnQryCommissionTemplateItem);
            Globals.TLClient.ReqQryCommissionTemplate();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryCommissionTemplate", this.OnQryCommissionTemplate);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyCommissionTemplate", this.OnNotifyCommissionTemplate);

            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryCommissionTemplateItem", this.OnQryCommissionTemplateItem);
        }


        Dictionary<int, int> itemrowmap  = new Dictionary<int, int>();
        Dictionary<int, CommissionTemplateItemSetting> itemmap = new Dictionary<int, CommissionTemplateItemSetting>();

        int ItemIdx(int id)
        {
            int rowid = -1;
            if (itemrowmap.TryGetValue(id, out rowid))
            {
                return rowid;
            }
            else
            {
                return -1;
            }
        }

        void OnQryCommissionTemplateItem(string json)
        {
            CommissionTemplateItemSetting[] list = MoniterUtils.ParseJsonResponse<CommissionTemplateItemSetting[]>(json);
            if (list != null)
            {
                foreach (CommissionTemplateItemSetting item in list)
                {
                    GotCommissionTemplateItem(item);
                }
            }
        }

        void GotCommissionTemplateItem(CommissionTemplateItemSetting item)
        {
            if (this.InvokeRequired)
            {
                Invoke(new Action<CommissionTemplateItemSetting>(GotCommissionTemplateItem), new object[] { item });
            }
            else
            {
                int r = ItemIdx(item.ID);
                if (r == -1)
                {
                    gt.Rows.Add(item.ID);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][CODE] = item.Code;
                    gt.Rows[i][MONTH] = item.Month;
                    gt.Rows[i][OPENBYMONEY] = item.OpenByMoney;
                    gt.Rows[i][OPENBYVOLUME] = item.OpenByVolume;
                    gt.Rows[i][CLOSETODAYBYMONEY] = item.CloseTodayByMoney;
                    gt.Rows[i][CLOSETODAYBYVOLUME] = item.CloseTodayByVolume;
                    gt.Rows[i][CLOSEBYMONEY] = item.CloseByMoney;
                    gt.Rows[i][CLOSEBYVOLUME] = item.CloseByVolume;

                    itemmap.Add(item.ID, item);
                    itemrowmap.Add(item.ID, i);

                }
                else
                { 
                    
                }
                
            }
        }

        Dictionary<int, CommissionTemplateSetting> templatemap = new Dictionary<int, CommissionTemplateSetting>();
        void OnQryCommissionTemplate(string json)
        {
            CommissionTemplateSetting[] list = MoniterUtils.ParseJsonResponse<CommissionTemplateSetting[]>(json);
            if (list != null)
            {
                foreach (CommissionTemplateSetting t in list)
                {
                    templatemap.Add(t.ID, t);
                    templatelist.Items.Add(t);
                }
            }
        }

        void OnNotifyCommissionTemplate(string json)
        {
            CommissionTemplateSetting obj = MoniterUtils.ParseJsonResponse<CommissionTemplateSetting>(json);
            if (obj != null)
            {
                CommissionTemplateSetting target = null;
                if (templatemap.TryGetValue(obj.ID,out target))
                {
                    target.Name = obj.Name;
                    target.Description = obj.Description;
                    templatelist.Refresh();
                }
                else
                {
                    templatemap.Add(obj.ID, obj);
                    templatelist.Items.Add(obj);
                }
            }
        }




        #region 表格
        #region 显示字段

        const string ID = "全局ID";
        const string CODE = "品种";
        const string MONTH = "月份";
        const string OPENBYMONEY = "开仓手续费(金额)";
        const string OPENBYVOLUME = "开仓手续费(手数)";
        const string CLOSETODAYBYMONEY = "平今手续费(金额)";
        const string CLOSETODAYBYVOLUME = "平今手续费(手数)";
        const string CLOSEBYMONEY = "平仓手续费(金额)";
        const string CLOSEBYVOLUME = "平仓手续费(手数)";
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = commissionGrid;

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeight = 25;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            grid.StateCommon.Background.Color1 = Color.WhiteSmoke;
            grid.StateCommon.Background.Color2 = Color.WhiteSmoke;

        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ID);//
            gt.Columns.Add(CODE);//
            gt.Columns.Add(MONTH);//
            gt.Columns.Add(OPENBYMONEY);//
            gt.Columns.Add(OPENBYVOLUME);//
            gt.Columns.Add(CLOSETODAYBYMONEY);//
            gt.Columns.Add(CLOSETODAYBYVOLUME);//
            gt.Columns.Add(CLOSEBYMONEY);//
            gt.Columns.Add(CLOSEBYVOLUME);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = commissionGrid;
            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //grid.Columns[MTID].Width = 80;
            //grid.Columns[MTNAME].Width = 200;

        }





        #endregion


    }
}
