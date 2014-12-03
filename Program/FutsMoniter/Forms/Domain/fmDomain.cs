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
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;

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
            Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryDomain", this.OnQryDomain);
            Globals.CallBackCentre.RegisterCallback("MgrExchServer", "NotifyDomain", this.OnNotifyDomain);
        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryDomain", this.OnQryDomain);
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "NotifyDomain", this.OnNotifyDomain);
       
        }

        bool _godomain = false;
        void OnQryDomain(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                DomainImpl[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<DomainImpl[]>(jd);
                foreach (DomainImpl op in objs)
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
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(json);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                DomainImpl obj = TradingLib.Mixins.JsonReply.ParsePlayload<DomainImpl>(jd);
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

                    gt.Rows[i][DATEEXPIRED] = domain.DateExpired;
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
        const string DATEEXPIRED = "到期日";
        const string ACCLIMIT = "帐户限制";
        const string ROUTERGROUPLIMIT = "路由组限制";
        const string ROUTERITEMLIMIT = "路由项目限制";

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
        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(DOMAINID);//0
            gt.Columns.Add(NAME);//1
            gt.Columns.Add(LINKMAN);//1

            gt.Columns.Add(DATEEXPIRED);
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
            grid.DataSource = datasource;

            grid.Columns[DOMAINID].Width = 50;
            grid.Columns[NAME].Width = 160;
            //grid.Columns[ISXAPI].Width = 50;
            //grid.Columns[TYPE].Width = 50;
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
        }





        #endregion


        void AddDomain_Click(object sender, EventArgs e)
        {
            fmDomainEdit fm = new fmDomainEdit();
            fm.Show();
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
