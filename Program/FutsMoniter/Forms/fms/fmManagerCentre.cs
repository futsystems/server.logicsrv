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
    public partial class fmManagerCentre : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmManagerCentre()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            menu.Items.Add("编辑", null, new EventHandler(EditManager_Click));
            menu.Items.Add("添加", null, new EventHandler(AddManager_Click));
            secgrid.ContextMenuStrip = menu;
        }

        ContextMenuStrip menu = new ContextMenuStrip();

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



        //得到当前选择的行号
        private int CurrentManagerID
        {

            get
            {
                int row = secgrid.SelectedRows.Count > 0 ? secgrid.SelectedRows[0].Index : -1;
                if (row>= 0)
                {
                    return int.Parse(secgrid[0,row].Value.ToString());
                }
                else
                {
                    return 0;
                }
            }
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


        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = secgrid;

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
            gt.Columns.Add(LOGIN);//
            gt.Columns.Add(MGRTYPE);//
            gt.Columns.Add(MGRTYPESTR);
            gt.Columns.Add(NAME);//
            gt.Columns.Add(MOBILE);//
            gt.Columns.Add(QQ);//
            gt.Columns.Add(ACCNUMLIMIT);//
            gt.Columns.Add(MGRFK);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = secgrid;


            datasource.DataSource = gt;
            //datasource.Filter=""
            grid.DataSource = datasource;
            datasource.Sort = ID + " ASC";

            grid.Columns[MGRTYPE].Visible = false;
            grid.Columns[ID].Visible = false;
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        Dictionary<int, int> mgridmap = new Dictionary<int, int>();
        //Dictionary<int, Exchange> exchangemap = new Dictionary<int, Exchange>();

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
            //Globals.Debug("got manager %%%%%%%%%%%%%%%%%%%%%%5 name:"+manger.Name);
            //如果获得的ManagerID和登入回报的ID一致 则表明该Manger是自己 在列表中不显示
            if (manger.ID.Equals(Globals.LoginResponse.MGRID)) return;
            if (InvokeRequired)
            {
                Invoke(new ManagerDel(GotManager), new object[] { manger });
            }
            else
            {
                int r = MangerIdx(manger.ID);
                //添加
                if (r == -1)
                {
                    //Globals.Debug("add row");
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

                    //mgridmap.Add(manger.ID, ex);
                    mgridmap.Add(manger.ID, i);//记录全局ID和table序号的映射

                }
                else
                {
                    int i = r;
                    gt.Rows[i][LOGIN] = manger.Login;
                    gt.Rows[i][MGRTYPESTR] = Util.GetEnumDescription(manger.Type);
                    gt.Rows[i][MGRTYPE] = manger.Type;
                    gt.Rows[i][NAME] = manger.Name;
                    gt.Rows[i][MOBILE] = manger.Mobile;
                    gt.Rows[i][QQ] = manger.QQ;
                    gt.Rows[i][ACCNUMLIMIT] = manger.AccLimit;
                    gt.Rows[i][MGRFK] = manger.mgr_fk;

                }
            }
        }

        //private void secgrid_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        //{
        //    e.ContextMenu = menu.DropDown;
        //}

        //private void secgrid_DoubleClick(object sender, EventArgs e)
        //{
        //    EditManager_Click(null, null);
        //}
    }
}
