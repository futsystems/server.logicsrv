using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace FutsMoniter
{
    public partial class ManagerForm : Telerik.WinControls.UI.RadForm
    {

        RadContextMenu menu = new RadContextMenu();


        public ManagerForm()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            Telerik.WinControls.UI.RadMenuItem MenuItem_add = new Telerik.WinControls.UI.RadMenuItem("添加管理员");
            MenuItem_add.Click += new EventHandler(AddManager_Click);
            menu.Items.Add(MenuItem_add);

            Telerik.WinControls.UI.RadMenuItem MenuItem_edit = new Telerik.WinControls.UI.RadMenuItem("编辑管理员");
            MenuItem_edit.Click += new EventHandler(EditManager_Click);
            menu.Items.Add(MenuItem_edit);
            
        }

        void AddManager_Click(object sender, EventArgs e)
        {
            Globals.Debug("manger type:" + Globals.Manager.Type.ToString());
            if (!Globals.RightAddManger)
            {
                fmConfirm.Show("没有添加柜员的权限");
                return;
            }
            ManagerEditForm fm = new ManagerEditForm();
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

                ManagerEditForm fm = new ManagerEditForm();
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
                if (secgrid.SelectedRows.Count > 0)
                {
                    return int.Parse(secgrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[ID].Value.ToString());
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
        
        const string MGRFK = "代理编号";
        

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = secgrid;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;

            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//不允许删除行
            grid.AllowEditRow = false;//不允许编辑行
            grid.AllowRowResize = false;
            //grid.EnableSorting = false;
            grid.TableElement.TableHeaderHeight = Globals.HeaderHeight;
            grid.TableElement.RowHeight = Globals.RowHeight;

            grid.EnableAlternatingRowColor = true;//隔行不同颜色
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 

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
            Telerik.WinControls.UI.RadGridView grid = secgrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;
            
            
            datasource.DataSource = gt;
            //datasource.Filter=""
            grid.DataSource = datasource;
            datasource.Sort = ID + " ASC";
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
            Globals.Debug("got manager %%%%%%%%%%%%%%%%%%%%%%5 name:"+manger.Name);
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
                    Globals.Debug("add row");
                    gt.Rows.Add(manger.ID);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][LOGIN] = manger.Login;
                    gt.Rows[i][MGRTYPESTR] = LibUtil.GetEnumDescription(manger.Type);
                    gt.Rows[i][MGRTYPE] = manger.Type;
                    gt.Rows[i][NAME] = manger.Name;
                    gt.Rows[i][MOBILE] = manger.Mobile;
                    gt.Rows[i][QQ] = manger.QQ;
                    gt.Rows[i][ACCNUMLIMIT] = manger.AccLimit;
                    gt.Rows[i][MGRFK] = manger.mgr_fk;

                    //mgridmap.Add(manger.ID, ex);
                    mgridmap.Add(manger.ID, i);//记录全局ID和table序号的映射

                }
                else
                {
                    int i = r;
                    gt.Rows[i][LOGIN] = manger.Login;
                    gt.Rows[i][MGRTYPESTR] = LibUtil.GetEnumDescription(manger.Type);
                    gt.Rows[i][MGRTYPE] = manger.Type;
                    gt.Rows[i][NAME] = manger.Name;
                    gt.Rows[i][MOBILE] = manger.Mobile;
                    gt.Rows[i][QQ] = manger.QQ;
                    gt.Rows[i][ACCNUMLIMIT] = manger.AccLimit;
                    gt.Rows[i][MGRFK] = manger.mgr_fk;

                }
            }
        }

        private void secgrid_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = menu.DropDown;
        }

        private void secgrid_DoubleClick(object sender, EventArgs e)
        {
            EditManager_Click(null, null);
        }

    }
}
