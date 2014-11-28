using System;
using System.Collections.Generic;
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
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;

namespace FutsMoniter
{
    public partial class fmVendorManager
    {

        //选中某个路由组
        void ctRouterGroupList1_RouterGroupSelectedChangedEvent()
        {
            itemgt.Clear();
            itemmap.Clear();
            itemrowid.Clear();

            RouterGroupSetting setting = ctRouterGroupList1.RouterGroudSelected;
            rgid.Text = setting.ID.ToString();
            rgname.Text = setting.Name;
            rgdescrption.Text = setting.Description;
            cbrgstrategytype.SelectedValue = setting.Strategy;

            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryRouterItem(setting.ID);
            }
        }


        bool _gotitem = false;
        void OnQryRouterItem(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                RouterItemSetting[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<RouterItemSetting[]>(jd);
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

        //VendorSetting ID2VendorSetting(int id)
        //{
        //    if (vendormap.Keys.Contains(id))
        //        return vendormap[id];
        //    return null;
        //}
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
                    VendorSetting setting = ID2VendorSetting(item.vendor_id);
                    string vtitle = string.Empty;
                    if (setting == null)
                    {
                        vtitle = "异常";
                    }
                    else
                    {
                        vtitle = setting.Name;
                    }
                    itemgt.Rows[i][ITEMVENDOR] = vtitle;
                    itemgt.Rows[i][RULE] = item.rule;
                    itemgt.Rows[i][PRIORITY] = item.priority;
                    itemgt.Rows[i][ACTIVE] = item.Active?"激活":"冻结";



                    itemrowid.TryAdd(item.ID, i);
                    itemmap.TryAdd(item.ID, item);
                }
                else
                {
                    //更新状态
                    //gt.Rows[r][STATUS] = c.Status;
                    //connectormap[c.Token] = c;
                }

            }
        }

        #region 表格
        #region 显示字段

        const string ROUTERITEMID = "路由ID";
        const string GROUPID = "GroupID";
        const string ITEMVENDOR = "帐户";
        const string RULE = "规则";
        const string PRIORITY = "优先级";
        const string ACTIVE = "激活";


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

        }





        #endregion

    }
}
