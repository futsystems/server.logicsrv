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
    public partial class fmMargin : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmMargin()
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
            this.templatelist.ContextMenuStrip.Items.Add("加载数据", null, new EventHandler(Qry_Click));

            marginGrid.ContextMenuStrip = new ContextMenuStrip();
            marginGrid.ContextMenuStrip.Items.Add("添加模板项目", null, new EventHandler(AddItem_Click));

            Globals.RegIEventHandler(this);
            //this.templatelist.ContextMenuStrip.Items.Add("添加模板", null, new EventHandler(Add_Click));
            marginGrid.DoubleClick += new EventHandler(commissionGrid_DoubleClick);
            marginGrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(commissionGrid_RowPrePaint);
        }

        void commissionGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }


        void commissionGrid_DoubleClick(object sender, EventArgs e)
        {
            MarginTemplateItemSetting item = GetVisibleCommissionItem(CurrentItemID);
            if (item == null)
            {
                MoniterUtils.WindowMessage("请选择需要编辑的保证金模板项目");
                return;
            }
            fmMarginTemplateItemEdit fm = new fmMarginTemplateItemEdit();
            fm.SetMarginTemplateItem(item);
            fm.ShowDialog();
        }

        //得到当前选择的行号
        private int CurrentItemID
        {
            get
            {
                int row = marginGrid.SelectedRows.Count > 0 ? marginGrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    return int.Parse(marginGrid[0, row].Value.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        MarginTemplateItemSetting GetVisibleCommissionItem(int id)
        {
            MarginTemplateItemSetting item = null;
            if (itemmap.TryGetValue(id, out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }


        void AddItem_Click(object sender, EventArgs e)
        {

            fmCommissionTemplateItemEdit fm = new fmCommissionTemplateItemEdit();
            try
            {
                int id = int.Parse(templateid.Text);
                fm.SetCommissionTemplateID(id);
                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MoniterUtils.WindowMessage("请选择模板");
            }
        }

        void Qry_Click(object sender, EventArgs e)
        {
            MarginTemplateSetting t = templatelist.SelectedItem as MarginTemplateSetting;
            if (t == null)
            {
                MoniterUtils.WindowMessage("请选择保证金模板");
                return;
            }
            ClearItem();
            templatename.Text = t.Name;
            templateid.Text = t.ID.ToString();
            Globals.TLClient.ReqQryMarginTemplateItem(t.ID);
        }
        void Add_Click(object sender, EventArgs e)
        {
            fmTemplateEdit fm = new fmTemplateEdit(TemplateEditType.Margin);
            fm.ShowDialog();
        }
        void Edit_Click(object sender, EventArgs e)
        {
            MarginTemplateSetting t = templatelist.SelectedItem as MarginTemplateSetting;
            fmTemplateEdit fm = new fmTemplateEdit(TemplateEditType.Margin);

            fm.SetTemplate(t);
            fm.ShowDialog();
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryMarginTemplate", this.OnQryMarginTemplate);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyMarginTemplate", this.OnNotifyMarginTemplate);

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryMarginTemplateItem", this.OnQryMarginTemplateItem);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyMarginTemplateItem", this.OnNotifyMarginTemplateItem);
            Globals.TLClient.ReqQryMarginTemplate();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryMarginTemplate", this.OnQryMarginTemplate);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyMarginTemplate", this.OnNotifyMarginTemplate);

            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryMarginTemplateItem", this.OnQryMarginTemplateItem);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyMarginTemplateItem", this.OnNotifyMarginTemplateItem);
        }

        string GetChargeTypeStr(QSEnumChargeType type)
        {
            switch (type)
            { 
                case QSEnumChargeType.Absolute:
                    return "绝对";
                case QSEnumChargeType.Percent:
                    return "百分比";
                case QSEnumChargeType.Relative:
                    return "相对";
                default:
                    return "";
            }
        }
        void ClearItem()
        {
            marginGrid.DataSource = null;
            itemrowmap.Clear();
            itemmap.Clear();
            gt.Rows.Clear();
            BindToTable();
        }


        Dictionary<int, int> itemrowmap  = new Dictionary<int, int>();
        Dictionary<int, MarginTemplateItemSetting> itemmap = new Dictionary<int, MarginTemplateItemSetting>();

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

        void OnQryMarginTemplateItem(string json)
        {
            MarginTemplateItemSetting obj = MoniterUtils.ParseJsonResponse<MarginTemplateItemSetting>(json);
            if (obj != null)
            {
                    GotCommissionTemplateItem(obj);
            }
        }

        void OnNotifyMarginTemplateItem(string json)
        {
            MarginTemplateItemSetting obj = MoniterUtils.ParseJsonResponse<MarginTemplateItemSetting>(json);
            if (obj != null)
            {
                GotCommissionTemplateItem(obj);
            }
        }

        void GotCommissionTemplateItem(MarginTemplateItemSetting item)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<MarginTemplateItemSetting>(GotCommissionTemplateItem), new object[] { item });
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
                    gt.Rows[i][MARGINBYMONEY] = item.MarginByMoney;
                    gt.Rows[i][MARGINBYVOLUME] = item.MarginByVolume;
                    gt.Rows[i][PERCENT] = item.Percent;
                    gt.Rows[i][CHARGETYPE] = GetChargeTypeStr(item.ChargeType);// == QSEnumChargeType.Absolute ? "绝对" : "相对";

                    itemmap.Add(item.ID, item);
                    itemrowmap.Add(item.ID, i);

                }
                else
                {
                    int i = r;
                    gt.Rows[i][MARGINBYMONEY] = item.MarginByMoney;
                    gt.Rows[i][MARGINBYVOLUME] = item.MarginByVolume;
                    gt.Rows[i][PERCENT] = item.Percent;
                    gt.Rows[i][CHARGETYPE] = GetChargeTypeStr(item.ChargeType);// == QSEnumChargeType.Absolute ? "绝对" : "相对";
                    itemmap[item.ID]=item;
                }
            }
        }

        Dictionary<int, MarginTemplateSetting> templatemap = new Dictionary<int, MarginTemplateSetting>();
        void OnQryMarginTemplate(string json)
        {
            MarginTemplateSetting[] list = MoniterUtils.ParseJsonResponse<MarginTemplateSetting[]>(json);
            if (list != null)
            {
                foreach (MarginTemplateSetting t in list)
                {
                    templatemap.Add(t.ID, t);
                    templatelist.Items.Add(t);
                }
            }
        }

        void OnNotifyMarginTemplate(string json)
        {
            MarginTemplateSetting obj = MoniterUtils.ParseJsonResponse<MarginTemplateSetting>(json);
            if (obj != null)
            {
                MarginTemplateSetting target = null;
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
        const string MARGINBYMONEY = "保证金(金额)";
        const string MARGINBYVOLUME = "保证金(手数)";
        const string PERCENT = "上浮率";
        const string CHARGETYPE = "计算方式";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = marginGrid;

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
            gt.Columns.Add(MARGINBYMONEY);//
            gt.Columns.Add(MARGINBYVOLUME);//
            gt.Columns.Add(PERCENT);
            gt.Columns.Add(CHARGETYPE);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = marginGrid;
            datasource.DataSource = gt;
            grid.DataSource = datasource;

            grid.Columns[ID].Width = 60;
            grid.Columns[CODE].Width =60;
            grid.Columns[MONTH].Width = 60;

        }





        #endregion


    }
}
