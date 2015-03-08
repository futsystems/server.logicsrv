using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class fmVendorManager
    {

        //选中某个路由组
        void ctRouterGroupList1_RouterGroupSelectedChangedEvent()
        {
            RefreshRouterItem();
        }

        void RefreshRouterItem()
        {
            itemgt.Clear();
            itemmap.Clear();
            itemrowid.Clear();

            RouterGroupSetting setting = ctRouterGroupList1.RouterGroup;
            if (setting != null)
            {
                rgid.Text = setting.ID.ToString();
                rgname.Text = setting.Name;
                rgdescrption.Text = setting.Description;
                cbrgstrategytype.SelectedValue = setting.Strategy;

                if (Globals.EnvReady)
                {
                    Globals.TLClient.ReqQryRouterItem(setting.ID);
                }
            }
        }

        //void OnNotifyRouterGroup(string jsonstr)
        //{
        //    ctRouterGroupList1.OnNotifyRouterGroup(jsonstr);
        //}


        void OnNotifyRouterItem(string jsonstr)
        {
            RouterItemSetting obj = MoniterUtils.ParseJsonResponse<RouterItemSetting>(jsonstr);
            if (obj!=null)
            {
                InvokeGotItem(obj);
            }
            else//如果没有配资服
            {

            }
        }

        bool _gotitem = false;
        void OnQryRouterItem(string jsonstr, bool islast)
        {
            RouterItemSetting[] objs = MoniterUtils.ParseJsonResponse<RouterItemSetting[]>(jsonstr);
            if (objs != null)
            {
                foreach (RouterItemSetting obj in objs)
                {
                    InvokeGotItem(obj);
                }
                _gotitem = true;
            }
            else//如果没有配资服
            {

            }
        }

        //得到当前选择的行号
        private RouterItemSetting CurrentRouterItem
        {
            get
            {
                int row = routeritemgrid.SelectedRows.Count > 0 ? routeritemgrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id = int.Parse(routeritemgrid[0, row].Value.ToString());

                    if (itemmap.Keys.Contains(id))
                        return itemmap[id];
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }
        ConcurrentDictionary<int, RouterItemSetting> itemmap = new ConcurrentDictionary<int, RouterItemSetting>();
        ConcurrentDictionary<int, int> itemrowid = new ConcurrentDictionary<int, int>();

        int ItemIdx(int id)
        {
            int rowid = -1;
            if (!itemrowid.TryGetValue(id, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }

        string GetVendorTitle(int id)
        {
            VendorSetting setting = ID2VendorSetting(id);
            string vtitle = string.Empty;
            if (setting == null)
            {
                vtitle = "异常";
            }
            else
            {
                vtitle = setting.Name;
            }
            return vtitle;
        }
        void InvokeGotItem(RouterItemSetting item)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<RouterItemSetting>(InvokeGotItem), new object[] { item });
            }
            else
            {
                int r = ItemIdx(item.ID);
                if (r == -1)
                {
                    itemgt.Rows.Add(item.ID);
                    int i = itemgt.Rows.Count - 1;

                    itemgt.Rows[i][GROUPID] = item.routegroup_id;

                    itemgt.Rows[i][ITEMVENDOR] = GetVendorTitle(item.vendor_id);
                    itemgt.Rows[i][RULE] = item.rule;
                    itemgt.Rows[i][PRIORITY] = item.priority;
                    itemgt.Rows[i][ACTIVE] = item.Active ? "允许" : "禁止";



                    itemrowid.TryAdd(item.ID, i);
                    itemmap.TryAdd(item.ID, item);
                }
                else
                {
                    //更新状态
                    itemgt.Rows[r][ITEMVENDOR] = GetVendorTitle(item.vendor_id);
                    itemgt.Rows[r][RULE] = item.rule;
                    itemgt.Rows[r][PRIORITY] = item.priority;
                    itemgt.Rows[r][ACTIVE] = item.Active ? "允许" : "禁止";
                    itemmap[item.ID] = item;
                }

            }
        }
        void btnUpdateRouterGroup_Click(object sender, EventArgs e)
        {
            RouterGroupSetting group = ctRouterGroupList1.RouterGroup;
            group.Strategy = (QSEnumRouterStrategy)cbrgstrategytype.SelectedValue;
            group.Description = rgdescrption.Text;
            if (fmConfirm.Show("确认更新路由组信息?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqUpdateRouterGroup(group);
            }
        }

        #region 表格
        #region 显示字段

        const string ROUTERITEMID = "路由ID";
        const string GROUPID = "GroupID";
        const string ITEMVENDOR = "帐户";
        const string RULE = "规则";
        const string PRIORITY = "优先级";
        const string ACTIVE = "开仓";


        #endregion

        DataTable itemgt = new DataTable();
        BindingSource itemdatasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences_RouterItem()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = routeritemgrid;

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
           
            
            grid.ContextMenuStrip.Items.Add("添加路由组", null, new EventHandler(AddRouterGroup_Click));
            grid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            grid.ContextMenuStrip.Items.Add("添加路由", null, new EventHandler(AddRouterItem_Click));
            grid.ContextMenuStrip.Items.Add("修改路由", null, new EventHandler(EditRouterItem_Click));
            grid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        }
        

        //初始化Account显示空格
        private void InitTable_RouterItem()
        {
            itemgt.Columns.Add(ROUTERITEMID);//0
            itemgt.Columns.Add(GROUPID);//1
            itemgt.Columns.Add(ITEMVENDOR);
            itemgt.Columns.Add(PRIORITY);
            itemgt.Columns.Add(RULE);//1
            itemgt.Columns.Add(ACTIVE);//1
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable_RouterItem()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = routeritemgrid;

            itemdatasource.DataSource = itemgt;
            grid.DataSource = itemdatasource;

            grid.Columns[GROUPID].Visible = false;

            grid.Columns[ROUTERITEMID].Width = 50;
            grid.Columns[ITEMVENDOR].Width = 120;
            grid.Columns[PRIORITY].Width = 50;
            grid.Columns[ACTIVE].Width = 50;
            
        }





        #endregion

        void AddRouterGroup_Click(object sender, EventArgs e)
        {
            fmRouterGroupEdit fm = new fmRouterGroupEdit();
            fm.Show();
        }
        void AddRouterItem_Click(object sender, EventArgs e)
        {
            ArrayList list = GetVendorIDCBList(false);
            if (list.Count == 0)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("没有可用实盘帐户");
                return;
            }
            RouterGroupSetting rg = ctRouterGroupList1.RouterGroup;
            if (rg == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请先添加路由组");
                return;
            }
            fmRouterItem fm = new fmRouterItem();
            fm.SetRouterGroup(rg);
            fm.SetVendorCBList(list);
            fm.Show();
        }
        void EditRouterItem_Click(object sender, EventArgs e)
        {
            RouterItemSetting item = CurrentRouterItem;
            if (item == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择路由项目");
                return;
            }
            fmRouterItem fm = new fmRouterItem();
            RouterGroupSetting rg = ctRouterGroupList1.RouterGroup;
            fm.SetRouterGroup(rg);
            fm.SetVendorCBList(GetVendorIDCBList(false));
            fm.SetRouterItemSetting(item);
            fm.Show();
        }


    }
}
