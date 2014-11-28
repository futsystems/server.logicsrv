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

        bool _gotvendor = false;
        void OnQryVendor(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                VendorSetting[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<VendorSetting[]>(jd);
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
                    //gt.Rows[r][STATUS] = c.Status;
                    //connectormap[c.Token] = c;
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


    }
}
