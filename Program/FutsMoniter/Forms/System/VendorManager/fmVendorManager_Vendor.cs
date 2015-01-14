using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
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

        void OnNotifyVendorBind(string jsonstr)
        {
            //JsonReply<VendorSetting> reply = JsonReply.ParseReply<VendorSetting>(jsonstr);
            VendorSetting setting = MoniterUtils.ParseJsonResponse<VendorSetting>(jsonstr);
            if (setting != null)
            {
                InvokeGotVendor(setting);
            }
            else//如果没有配资服
            {

            }
        }


        bool _gotvendor = false;
        void OnQryVendor(string jsonstr)
        {
            VendorSetting[] objs = MoniterUtils.ParseJsonResponse<VendorSetting[]>(jsonstr);
            if (objs != null)
            {
                foreach (VendorSetting obj in objs)
                {
                    InvokeGotVendor(obj);
                }
                _gotvendor = true;
                if (!_gotconnector)
                {
                    Globals.TLClient.ReqQryConnectorConfig();
                }
            }
            else//如果没有配资服
            {

            }
        }

        VendorSetting ID2VendorSetting(int id)
        {
            if (vendormap.Keys.Contains(id))
                return vendormap[id];
            return null;
        }

        public ArrayList GetVendorCBList(bool notbind = true)
        {
            ArrayList list = new ArrayList();
            foreach (VendorSetting item in vendormap.Values)
            {
                if (notbind && !string.IsNullOrEmpty(item.BrokerToken))
                    continue;
                ValueObject<VendorSetting> vo = new ValueObject<VendorSetting>();
                vo.Name = item.Name;
                vo.Value = item;
                list.Add(vo);
            }
            return list;
        }

        public ArrayList GetVendorIDCBList(bool notbind = true)
        {
            ArrayList list = new ArrayList();
            foreach (VendorSetting item in vendormap.Values)
            {
                if (notbind && !string.IsNullOrEmpty(item.BrokerToken))
                    continue;
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = item.Name;
                vo.Value = item.ID;
                list.Add(vo);
            }
            return list;
        }

        //得到当前选择的行号
        private VendorSetting CurrentVendorSetting
        {
            get
            {
                int row = vendorgrid.SelectedRows.Count > 0 ? vendorgrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id = int.Parse(vendorgrid[0, row].Value.ToString());

                    if (vendormap.Keys.Contains(id))
                        return vendormap[id];
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }


        ConcurrentDictionary<int, VendorSetting> vendormap = new ConcurrentDictionary<int, VendorSetting>();
        ConcurrentDictionary<int, int> vendorrowid = new ConcurrentDictionary<int, int>();

        int VendorIdx(int id)
        {
            int rowid = -1;
            if (!vendorrowid.TryGetValue(id, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }
        void InvokeGotVendor(VendorSetting vendor)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<VendorSetting>(InvokeGotVendor), new object[] { vendor });
            }
            else
            {
                int r = VendorIdx(vendor.ID);
                if (r == -1)
                {
                    vendorgt.Rows.Add(vendor.ID);
                    int i = vendorgt.Rows.Count - 1;

                    vendorgt.Rows[i][VENDORNAME] = vendor.Name;
                    vendorgt.Rows[i][FUTCOMPANY] = vendor.FutCompany;
                    vendorgt.Rows[i][LASTEQUITY] = vendor.LastEquity;
                    vendorgt.Rows[i][MARGINLIMIT] = vendor.MarginLimit;
                    vendorgt.Rows[i][BINDEDBROKER] = string.IsNullOrEmpty(vendor.BrokerToken)?"未绑定":vendor.BrokerToken;




                    vendorrowid.TryAdd(vendor.ID, i);
                    vendormap.TryAdd(vendor.ID, vendor);
                }
                else
                {
                    //更新状态

                    vendorgt.Rows[r][VENDORNAME] = vendor.Name;
                    vendorgt.Rows[r][FUTCOMPANY] = vendor.FutCompany;
                    vendorgt.Rows[r][LASTEQUITY] = vendor.LastEquity;
                    vendorgt.Rows[r][MARGINLIMIT] = vendor.MarginLimit;
                    vendorgt.Rows[r][BINDEDBROKER] = string.IsNullOrEmpty(vendor.BrokerToken) ? "未绑定" : vendor.BrokerToken;
                    vendormap[vendor.ID]= vendor;
                }

            }
        }

        #region 表格
        #region 显示字段

        const string VENDORID = "帐户ID";
        const string VENDORNAME = "帐户名称";
        const string FUTCOMPANY = "期货公司";
        const string LASTEQUITY = "昨日权益";
        const string MARGINLIMIT = "可用保证金";
        const string BINDEDBROKER = "通道标识";


        #endregion

        DataTable vendorgt = new DataTable();
        BindingSource vendordatasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences_Vendor()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = vendorgrid;

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
            grid.ContextMenuStrip.Items.Add("添加帐户", null, new EventHandler(AddVendor_Click));
            grid.ContextMenuStrip.Items.Add("修改帐户", null, new EventHandler(EditVendor_Click));
            grid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            grid.ContextMenuStrip.Items.Add("查看持仓矩阵", null, new EventHandler(ShowPositionMetric_Click));
        }

        //初始化Account显示空格
        private void InitTable_Vendor()
        {
            vendorgt.Columns.Add(VENDORID);//0
            vendorgt.Columns.Add(VENDORNAME);//1
            vendorgt.Columns.Add(FUTCOMPANY);//1

            vendorgt.Columns.Add(LASTEQUITY);
            vendorgt.Columns.Add(MARGINLIMIT);//1
            vendorgt.Columns.Add(BINDEDBROKER);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable_Vendor()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = vendorgrid;

            vendordatasource.DataSource = vendorgt;
            grid.DataSource = vendordatasource;

            grid.Columns[VENDORID].Width = 50;
            grid.Columns[VENDORNAME].Width = 120;
            grid.Columns[FUTCOMPANY].Width = 150;
            //grid.Columns[ID].Width = 50;
            //grid.Columns[NAME].Width = 120;
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


        void AddVendor_Click(object sender, EventArgs e)
        {
            fmVendorEdit fm = new fmVendorEdit();
            fm.Show();

        }
        void EditVendor_Click(object sender, EventArgs e)
        {
            VendorSetting setting = CurrentVendorSetting;
            if(setting == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择帐户设置");
                return;
            }
            fmVendorEdit fm = new fmVendorEdit();
            fm.SetVendorSetting(setting);
            fm.Show();
        }

        fmPositionMetricMoniter positionmetricmontier;
        void ShowPositionMetric_Click(object sender, EventArgs e)
        {
            VendorSetting setting = CurrentVendorSetting;
            if (setting == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择帐户设置");
                return;
            }
            if (positionmetricmontier == null)
            {
                positionmetricmontier = new fmPositionMetricMoniter();
            }

            positionmetricmontier.AppendVendor(setting);
            positionmetricmontier.Show();
        }
    }
}
