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
    public partial class fmManagerCentre : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmManagerCentre()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();


            

            this.Load += new EventHandler(fmManagerCentre_Load);
            

        }

        void fmManagerCentre_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            this.FormClosing += new FormClosingEventHandler(fmManagerCentre_FormClosing);
            this.mgrgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(mgrgrid_RowPrePaint);
            foreach (Manager m in Globals.BasicInfoTracker.Managers)
            {
                this.GotManager(m);
            }
        }

        public void OnInit()
        {
            Globals.BasicInfoTracker.GotManagerEvent += new Action<Manager>(GotManager);
        }

        public void OnDisposed()
        {
            Globals.BasicInfoTracker.GotManagerEvent -= new Action<Manager>(GotManager);
        }
        void mgrgrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        void AddManager_Click(object sender, EventArgs e)
        {
            Globals.Debug("manger type:" + Globals.Manager.Type.ToString());
            if (!Globals.RightAddManger)
            {
                fmConfirm.Show("没有添加柜员的权限");
                return;
            }
            fmManagerEdit fm = new fmManagerEdit();
            fm.Show();
        }

        void EditManager_Click(object sender, EventArgs e)
        {
            int id = CurrentManagerID;
            Manager manger = Globals.BasicInfoTracker.GetManager(id);
            if (manger != null)
            {
                if (manger.Type == QSEnumManagerType.ROOT)
                {
                    fmConfirm.Show("超级管理员不允许编辑!");
                    return;
                }

                fmManagerEdit fm = new fmManagerEdit();
                fm.Manager = manger;
                fm.ShowDialog();
            }
            else
            {
                fmConfirm.Show("请选择需要编辑的管理员");
            }
        }

        void ActiveManager_Click(object sender, EventArgs e)
        {
            int id = CurrentManagerID;
            Manager manger = Globals.BasicInfoTracker.GetManager(id);
            if (manger == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择管理员");
                return;
            }
            if (fmConfirm.Show("确认激活管理员:" + manger.Login)== System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqActiveManger(manger.ID);
            }
        }

        void InactiveManager_Click(object sender, EventArgs e)
        {
            int id = CurrentManagerID;
            Manager manger = Globals.BasicInfoTracker.GetManager(id);
            if (manger == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择管理员");
                return;
            }

            if (fmConfirm.Show("确认冻结管理员:" + manger.Login) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqInactiveManger(manger.ID);
            }
        }
        //得到当前选择的行号
        private int CurrentManagerID
        {
            get
            {
                int row = mgrgrid.SelectedRows.Count > 0 ? mgrgrid.SelectedRows[0].Index : -1;
                if (row>= 0)
                {
                    return int.Parse(mgrgrid[0,row].Value.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        Image getStatusImage(bool execute)
        {
            if (execute)
                return (Image)Properties.Resources.account_go;
            else
                return (Image)Properties.Resources.account_stop;

        }



        #region 显示字段

        const string ID = "全局ID";
        const string LOGIN = "登入名";
        const string MGRTYPE = "MGRTYPE";
        const string MGRTYPESTR = "管理员类型";
        const string NAME = "姓名";
        const string MOBILE = "手机";
        const string QQ = "QQ号码";
        const string ACCNUMLIMIT = "帐户数量限制";

        const string MGRFK = "管理域ID";
        const string STATUS = "状态";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = mgrgrid;

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


            grid.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();

            grid.ContextMenuStrip.Items.Add("添加", null, new EventHandler(AddManager_Click));
            grid.ContextMenuStrip.Items.Add("编辑", null, new EventHandler(EditManager_Click));
            grid.ContextMenuStrip.Items.Add("激活管理员", null, new EventHandler(ActiveManager_Click));
            grid.ContextMenuStrip.Items.Add("冻结管理员", null, new EventHandler(InactiveManager_Click));

        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ID);//
            gt.Columns.Add(LOGIN);//
            gt.Columns.Add(MGRTYPE);//
            gt.Columns.Add(MGRTYPESTR);
            gt.Columns.Add(NAME);//
            gt.Columns.Add(MOBILE);//
            gt.Columns.Add(QQ);//
            gt.Columns.Add(ACCNUMLIMIT);//
            gt.Columns.Add(MGRFK);//

            gt.Columns.Add(STATUS,typeof(Image));//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = mgrgrid;


            datasource.DataSource = gt;
            //datasource.Filter=""
            grid.DataSource = datasource;
            datasource.Sort = ID + " ASC";

            grid.Columns[MGRTYPE].Visible = false;
            grid.Columns[ID].Visible = false;
        }

        private void fmManagerCentre_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        Dictionary<int, int> mgridmap = new Dictionary<int, int>();

        int MangerIdx(int id)
        {
            int rowid = -1;
            if (mgridmap.TryGetValue(id, out rowid))
            {
                return rowid;
            }
            else
            {
                return -1;
            }
        }

        public void GotManager(Manager manger)
        {
            //如果获得的ManagerID和登入回报的ID一致 则表明该Manger是自己 在列表中不显示
            if (manger.ID.Equals(Globals.LoginResponse.MGRID)) return;
            if (InvokeRequired)
            {
                Invoke(new Action<Manager>(GotManager), new object[] { manger });
            }
            else
            {
                Globals.Debug("got mangaer:" + manger.ID.ToString());
                int r = MangerIdx(manger.ID);
                //添加
                if (r == -1)
                {
                    Globals.Debug("add row");
                    gt.Rows.Add(manger.ID);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][LOGIN] = manger.Login;
                    gt.Rows[i][MGRTYPESTR] = (manger.Type == QSEnumManagerType.AGENT ? "" : "员工-") + Util.GetEnumDescription(manger.Type);
                    gt.Rows[i][MGRTYPE] = manger.Type;
                    gt.Rows[i][NAME] = manger.Name;
                    gt.Rows[i][MOBILE] = manger.Mobile;
                    gt.Rows[i][QQ] = manger.QQ;
                    gt.Rows[i][ACCNUMLIMIT] = manger.Type == QSEnumManagerType.AGENT ? manger.AccLimit.ToString() : "";
                    gt.Rows[i][MGRFK] = manger.mgr_fk;

                    gt.Rows[i][STATUS] = getStatusImage(manger.Active);
                    mgridmap.Add(manger.ID, i);//记录全局ID和table序号的映射

                }
                else
                {
                    Globals.Debug("update manager:" + manger.Active.ToString());
                    int i = r;
                    gt.Rows[i][LOGIN] = manger.Login;
                    gt.Rows[i][MGRTYPESTR] = Util.GetEnumDescription(manger.Type);
                    gt.Rows[i][MGRTYPE] = manger.Type;
                    gt.Rows[i][NAME] = manger.Name;
                    gt.Rows[i][MOBILE] = manger.Mobile;
                    gt.Rows[i][QQ] = manger.QQ;
                    gt.Rows[i][ACCNUMLIMIT] = manger.AccLimit;
                    gt.Rows[i][MGRFK] = manger.mgr_fk;
                    gt.Rows[i][STATUS] = getStatusImage(manger.Active);

                }
            }
        }
    }
}
