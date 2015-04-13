using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
    public partial class fmDomain : ComponentFactory.Krypton.Toolkit.KryptonForm, IEventBinder
    {
        public fmDomain()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            this.Load += new EventHandler(fmDomain_Load);
        }

        void fmDomain_Load(object sender, EventArgs e)
        {
            WireEvent();
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryDomain();
            }
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            domaingrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(domaingrid_RowPrePaint);
        }

        void domaingrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }


        //得到当前选择的行号
        private DomainImpl CurrentDomain
        {
            get
            {
                int row = domaingrid.SelectedRows.Count > 0 ? domaingrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id = int.Parse(domaingrid[0, row].Value.ToString());

                    if (domainmap.Keys.Contains(id))
                        return domainmap[id];
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryDomain", this.OnQryDomain);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyDomain", this.OnNotifyDomain);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryDomain", this.OnQryDomain);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyDomain", this.OnNotifyDomain);
       
        }

        bool _godomain = false;
        void OnQryDomain(string jsonstr)
        {
            DomainImpl[] objs = MoniterUtils.ParseJsonResponse<DomainImpl[]>(jsonstr);
            if (objs != null)
            {
                foreach (DomainImpl op in objs.OrderBy(d=>d.ID))
                {
                    InvokeGotDomain(op);
                }
                _godomain = true;
            }
            else//如果没有配资服
            {

            }
        }

        void OnNotifyDomain(string json)
        {
            DomainImpl obj = MoniterUtils.ParseJsonResponse<DomainImpl>(json);
            if (obj != null)
            {
                InvokeGotDomain(obj);
            }
            else//如果没有配资服
            {

            }
        }


        ConcurrentDictionary<int, DomainImpl> domainmap = new ConcurrentDictionary<int, DomainImpl>();
        ConcurrentDictionary<int, int> domainrowid = new ConcurrentDictionary<int, int>();

        int DomainIdx(int id)
        {
            int rowid = -1;
            if (!domainrowid.TryGetValue(id, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }

        void InvokeGotDomain(DomainImpl domain)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DomainImpl>(InvokeGotDomain), new object[] { domain });
            }
            else
            {
                int r = DomainIdx(domain.ID);
                if (r == -1)
                {
                    gt.Rows.Add(domain.ID);
                    int i = gt.Rows.Count - 1;

                    gt.Rows[i][NAME] = domain.Name;
                    gt.Rows[i][LINKMAN] = domain.LinkMan;
                    gt.Rows[i][DATECREATED] = domain.DateCreated;
                    gt.Rows[i][DATEEXPIRED] = domain.DateExpired;
                    gt.Rows[i][VENDORLIMIT] = domain.VendorLimit;
                    gt.Rows[i][ACCLIMIT] = domain.AccLimit;
                    gt.Rows[i][ROUTERGROUPLIMIT] = domain.RouterGroupLimit;
                    gt.Rows[i][ROUTERITEMLIMIT] = domain.RouterItemLimit;


                    domainrowid.TryAdd(domain.ID, i);
                    domainmap.TryAdd(domain.ID, domain);
                }
                else
                {
                    gt.Rows[r][NAME] = domain.Name;
                    gt.Rows[r][LINKMAN] = domain.LinkMan;

                    gt.Rows[r][DATEEXPIRED] = domain.DateExpired;
                    gt.Rows[r][VENDORLIMIT] = domain.VendorLimit;
                    gt.Rows[r][ACCLIMIT] = domain.AccLimit;
                    gt.Rows[r][ROUTERGROUPLIMIT] = domain.RouterGroupLimit;
                    gt.Rows[r][ROUTERITEMLIMIT] = domain.RouterItemLimit;

                    domainmap[domain.ID] = domain;
                }

            }
        }



        #region 表格
        #region 显示字段

        const string DOMAINID = "域ID";
        const string NAME = "名称";
        const string LINKMAN = "联系人";
        const string DATECREATED = "创建日";
        const string DATEEXPIRED = "到期日";
        const string VENDORLIMIT = "实盘帐户";
        const string ACCLIMIT = "分帐户";
        const string ROUTERGROUPLIMIT = "路由组";
        const string ROUTERITEMLIMIT = "路由项目";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = domaingrid;

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

            grid.ContextMenuStrip = new ContextMenuStrip();
            grid.ContextMenuStrip.Items.Add("添加Domain", null, new EventHandler(AddDomain_Click));
            grid.ContextMenuStrip.Items.Add("修改Domain", null, new EventHandler(EditDomain_Click));
            grid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            grid.ContextMenuStrip.Items.Add("查看管理员密码", null, new EventHandler(QryDomainRootPass_Click));
        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(DOMAINID);//0
            gt.Columns.Add(NAME);//1
            gt.Columns.Add(LINKMAN);//1
            gt.Columns.Add(DATECREATED);
            gt.Columns.Add(DATEEXPIRED);
            gt.Columns.Add(VENDORLIMIT);
            gt.Columns.Add(ACCLIMIT);//1
            gt.Columns.Add(ROUTERGROUPLIMIT);//1
            gt.Columns.Add(ROUTERITEMLIMIT);//1
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = domaingrid;

            datasource.DataSource = gt;
            //datasource.Sort = DOMAINID;

            grid.DataSource = datasource;

            grid.Columns[DOMAINID].Width = 30;
            grid.Columns[NAME].Width = 100;
            //grid.Columns[ISXAPI].Width = 50;
            //grid.Columns[TYPE].Width = 50;
            grid.Columns[DATECREATED].Width = 60;
            grid.Columns[DATEEXPIRED].Width = 60;

            grid.Columns[VENDORLIMIT].Width = 60;
            grid.Columns[ACCLIMIT].Width = 60;
            grid.Columns[ROUTERGROUPLIMIT].Width = 60;
            grid.Columns[ROUTERITEMLIMIT].Width = 60;
            /*
            datasource.Sort = ACCOUNT + " ASC";
            

            accountgrid.Columns[EXECUTE].IsVisible = false;
            accountgrid.Columns[ROUTE].IsVisible = false;
            accountgrid.Columns[LOGINSTATUS].IsVisible = false;

            accountgrid.Columns[ACCOUNT].Width = 60;
            accountgrid.Columns[ROUTEIMG].Width = 20;
            accountgrid.Columns[EXECUTEIMG].Width = 20;
            accountgrid.Columns[PROFITLOSSIMG].Width = 20;
            accountgrid.Columns[LOGINSTATUSIMG].Width = 20;
            accountgrid.Columns[ADDRESS].Width = 120;**/

            for (int i = 0; i < gt.Columns.Count; i++)
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }





        #endregion


        void AddDomain_Click(object sender, EventArgs e)
        {
            fmDomainEdit fm = new fmDomainEdit();
            fm.Show();
        }

        /// <summary>
        /// 查询域RootPass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void QryDomainRootPass_Click(object sender, EventArgs e)
        {
            if(CurrentDomain != null)
            {
                fmLoginInfo fm = new fmLoginInfo();
                fm.SetDomain(CurrentDomain);
                fm.ShowDialog();
            }
        }


        void EditDomain_Click(object sender, EventArgs e)
        {
            DomainImpl domain = CurrentDomain;
            if (domain == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择要编辑分区");
                return;
            }
            fmDomainEdit fm = new fmDomainEdit();
            fm.SetDomain(domain);
            fm.Show();
        }

    }
}
