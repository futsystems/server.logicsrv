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
    public partial class fmExStrategy : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmExStrategy()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(margin).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumMarginStrategy>());
            Factory.IDataSourceFactory(avabilefund).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAvabileFundStrategy>());

            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            this.Load += new EventHandler(fmCommission_Load);
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {

            if (_current == null)
            { 
                MoniterUtils.WindowMessage("请选择要编辑的交易参数模板");
            }

            _current.SideMargin = sidemargin.Checked;
            _current.CreditSeparate = creditseparate.Checked;
            _current.Margin = (QSEnumMarginStrategy)margin.SelectedValue;
            _current.AvabileFund = (QSEnumAvabileFundStrategy)avabilefund.SelectedValue;
            _current.PositionLock = poslock.Checked;

            Globals.TLClient.ReqUpdateExStrategyTemplateItem(_current);
        }

        void fmCommission_Load(object sender, EventArgs e)
        {
            this.templatelist.ContextMenuStrip = new ContextMenuStrip();
            this.templatelist.ContextMenuStrip.Items.Add("添加模板", null, new EventHandler(Add_Click));
            this.templatelist.ContextMenuStrip.Items.Add("修改模板", null, new EventHandler(Edit_Click));
            this.templatelist.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            this.templatelist.ContextMenuStrip.Items.Add("加载数据", null, new EventHandler(Qry_Click));

            //commissionGrid.ContextMenuStrip = new ContextMenuStrip();
            //commissionGrid.ContextMenuStrip.Items.Add("添加模板项目", null, new EventHandler(AddItem_Click));

            Globals.RegIEventHandler(this);
            //this.templatelist.ContextMenuStrip.Items.Add("添加模板", null, new EventHandler(Add_Click));
            //commissionGrid.DoubleClick += new EventHandler(commissionGrid_DoubleClick);
            //commissionGrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(commissionGrid_RowPrePaint);
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
            ExStrategyTemplateSetting t = templatelist.SelectedItem as ExStrategyTemplateSetting;
            if (t == null)
            {
                MoniterUtils.WindowMessage("请选择交易参数模板");
                return;
            }
            ClearItem();
            templatename.Text = t.Name;
            templateid.Text = t.ID.ToString();
            Globals.TLClient.ReqQryExStrategyTemplateItem(t.ID);
        }
        void Add_Click(object sender, EventArgs e)
        {
            fmTemplateEdit fm = new fmTemplateEdit(TemplateEditType.Strategy);
            fm.ShowDialog();
        }
        void Edit_Click(object sender, EventArgs e)
        {
            ExStrategyTemplateSetting t = templatelist.SelectedItem as ExStrategyTemplateSetting;
            fmTemplateEdit fm = new fmTemplateEdit(TemplateEditType.Strategy);

            fm.SetTemplate(t);
            fm.ShowDialog();
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryExStrategyTemplate", this.OnQryExStrategyTemplate);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyExStrategyTemplate", this.OnNotifyExStrategyTemplate);

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryExStrategyTemplateItem", this.OnQryExStrategyTemplateItem);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyExStrategyTemplateItem", this.OnNotifyExStrategyTemplateItem);
            Globals.TLClient.ReqQryExStrategyTemplate();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryExStrategyTemplate", this.OnQryExStrategyTemplate);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyExStrategyTemplate", this.OnNotifyExStrategyTemplate);

            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryExStrategyTemplateItem", this.OnQryExStrategyTemplateItem);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyExStrategyTemplateItem", this.OnNotifyExStrategyTemplateItem);
        }


        void ClearItem()
        {
            //commissionGrid.DataSource = null;
            //itemrowmap.Clear();
            //itemmap.Clear();
            //gt.Rows.Clear();
            //BindToTable();
        }
        ExStrategy _current = null;

        void OnQryExStrategyTemplateItem(string json, bool islast)
        {
            ExStrategy item = MoniterUtils.ParseJsonResponse<ExStrategy>(json);
            if (item != null)
            {
                GotExStrategy(item);
            }
        }

        void OnNotifyExStrategyTemplateItem(string json, bool islast)
        {
            ExStrategy obj = MoniterUtils.ParseJsonResponse<ExStrategy>(json);
            if (obj != null)
            {
                GotExStrategy(obj);
            }
        }

        void GotExStrategy(ExStrategy item)
        {
            margin.SelectedValue = item.Margin;
            avabilefund.SelectedValue = item.AvabileFund;
            sidemargin.Checked = item.SideMargin;
            creditseparate.Checked = item.CreditSeparate;
            poslock.Checked = item.PositionLock;
            _current = item;
        }


        Dictionary<int, ExStrategyTemplateSetting> templatemap = new Dictionary<int, ExStrategyTemplateSetting>();
        void OnQryExStrategyTemplate(string json, bool islast)
        {
            ExStrategyTemplateSetting[] list = MoniterUtils.ParseJsonResponse<ExStrategyTemplateSetting[]>(json);
            if (list != null)
            {
                foreach (ExStrategyTemplateSetting t in list)
                {
                    templatemap.Add(t.ID, t);
                    templatelist.Items.Add(t);
                }
            }
        }

        void OnNotifyExStrategyTemplate(string json, bool islast)
        {
            ExStrategyTemplateSetting obj = MoniterUtils.ParseJsonResponse<ExStrategyTemplateSetting>(json);
            if (obj != null)
            {
                ExStrategyTemplateSetting target = null;
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



    }
}
