using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
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
    public partial class fmVendorManager
    {
        
        void OnNotifyConnectorConfig(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                ConnectorConfig objs = TradingLib.Mixins.JsonReply.ParsePlayload<ConnectorConfig>(jd);
                InvokeGotConnector(objs);
            }
            else//如果没有配资服
            {

            }
        }

        bool _gotconnector = false;
        void OnQryConnectorConfig(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                ConnectorConfig[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<ConnectorConfig[]>(jd);
                foreach (ConnectorConfig op in objs)
                {
                    InvokeGotConnector(op);
                }
                _gotconnector = true;
            }
            else//如果没有配资服
            {

            }
        }

        //得到当前选择的行号
        private ConnectorConfig CurrentConnectorConfig
        {
            get
            {
                int row = configgrid.SelectedRows.Count > 0 ? configgrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id = int.Parse(configgrid[0, row].Value.ToString());

                    if (connectormap.Keys.Contains(id))
                        return connectormap[id];
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }

        void configgrid_DoubleClick(object sender, EventArgs e)
        {
            //ConnectorConfig cfg = CurrentConnectorConfig;
            //if (cfg != null)
            //{
            //    fmConnectorEdit fm = new fmConnectorEdit();
            //    fm.SetConnectorConfig(cfg);
            //    fm.Show();
            //}
        }


        ConcurrentDictionary<int, ConnectorConfig> connectormap = new ConcurrentDictionary<int, ConnectorConfig>();
        ConcurrentDictionary<int, int> connectorrowid = new ConcurrentDictionary<int, int>();

        int ConnectorIdx(int id)
        {
            int rowid = -1;
            if (!connectorrowid.TryGetValue(id, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }

        string GetInterfaceTite(int id)
        {
            string title = string.Empty;
            if (id == 0)
            {
                title = "未绑定";
            }
            else
            {
                ConnectorInterface setting = ID2Interface(id);

                if (setting == null)
                {
                    title = "绑定异常";
                }
                else
                {
                    title = string.Format("[{0}]{1}", setting.ID,setting.Name);
                }
            }
            return title;
        }

        string GetVendorTitle(ConnectorConfig c)
        {
            string vtitle = string.Empty;
            if (c.vendor_id == 0)
            {
                vtitle = "未绑定";
            }
            else
            {
                VendorSetting setting = ID2VendorSetting(c.vendor_id);
                if (setting == null)
                {
                    vtitle = "绑定异常";
                }
                else
                {
                    vtitle = setting.Name;
                }
            }
            return vtitle;
        }
        void InvokeGotConnector(ConnectorConfig c)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ConnectorConfig>(InvokeGotConnector), new object[] { c });
            }
            else
            {
                int r = ConnectorIdx(c.ID);
                if (r == -1)
                {
                    gt.Rows.Add(c.ID);
                    int i = gt.Rows.Count - 1;

                    gt.Rows[i][TOKEN] = c.Token;
                    gt.Rows[i][NAME] = c.Name;

                    gt.Rows[i][SRVADDRESS] = c.srvinfo_ipaddress;
                    gt.Rows[i][SRVPORT] = c.srvinfo_port;
                    gt.Rows[i][SRV1] = c.srvinfo_field1;
                    gt.Rows[i][SRV2] = c.srvinfo_field2;
                    gt.Rows[i][SRV3] = c.srvinfo_field3;
                    gt.Rows[i][USERID] = c.usrinfo_userid;
                    gt.Rows[i][PASSWORD] = c.usrinfo_password;
                    gt.Rows[i][USR1] = c.usrinfo_field1;
                    gt.Rows[i][USR2] = c.usrinfo_field2;
                    gt.Rows[i][INTERFACE] = GetInterfaceTite(c.interface_fk);

                    gt.Rows[i][VENDORACCOUNT] = GetVendorTitle(c);

                    connectorrowid.TryAdd(c.ID, i);
                    connectormap.TryAdd(c.ID, c);
                }
                else
                {
                    //更新状态
                    gt.Rows[r][SRVADDRESS] = c.srvinfo_ipaddress;
                    gt.Rows[r][SRVPORT] = c.srvinfo_port;
                    gt.Rows[r][SRV1] = c.srvinfo_field1;
                    gt.Rows[r][SRV2] = c.srvinfo_field2;
                    gt.Rows[r][SRV3] = c.srvinfo_field3;
                    gt.Rows[r][USERID] = c.usrinfo_userid;
                    gt.Rows[r][PASSWORD] = c.usrinfo_password;
                    gt.Rows[r][USR1] = c.usrinfo_field1;
                    gt.Rows[r][USR2] = c.usrinfo_field2;
                    gt.Rows[r][NAME] = c.Name;
                    gt.Rows[r][VENDORACCOUNT] = GetVendorTitle(c);
                    connectormap[c.ID] = c;
                }

            }
        }

        #region 表格
        #region 显示字段

        const string ID = "通道ID";
        const string SRVADDRESS = "服务器地址";
        const string SRVPORT = "端口";
        const string SRV1 = "参数1";
        const string SRV2 = "参数2";
        const string SRV3 = "参数3";
        const string USERID = "用户名";
        const string PASSWORD = "密码";
        const string USR1 = "参数1/U";
        const string USR2 = "参数2/U";
        const string INTERFACE = "接口";
        const string TOKEN = "标识";
        const string NAME = "名称";
        const string VENDORACCOUNT = "帐户";
        const string ISBINDED = "Binded";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = configgrid;

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
            grid.ContextMenuStrip.Items.Add("添加通道", null, new EventHandler(AddConnector_Click));
            grid.ContextMenuStrip.Items.Add("修改通道", null, new EventHandler(EditConnector_Click));
            grid.ContextMenuStrip.Items.Add("绑定通道", null, new EventHandler(BindConnector_Click));
            grid.ContextMenuStrip.Items.Add("解绑通道", null, new EventHandler(UnBindConnector_Click));
            

        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ID);//0
            gt.Columns.Add(TOKEN);//1
            gt.Columns.Add(NAME);//1

            gt.Columns.Add(SRVADDRESS);
            gt.Columns.Add(SRVPORT);//1
            gt.Columns.Add(SRV1);//1
            gt.Columns.Add(SRV2);//1
            gt.Columns.Add(SRV3);//1
            gt.Columns.Add(USERID);//1
            gt.Columns.Add(PASSWORD);//1
            gt.Columns.Add(USR1);//1
            gt.Columns.Add(USR2);//1

            gt.Columns.Add(INTERFACE);//1
            gt.Columns.Add(VENDORACCOUNT);
            gt.Columns.Add(ISBINDED);


        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = configgrid;

            datasource.DataSource = gt;
            grid.DataSource = datasource;
            grid.Columns[SRV1].Visible = false;
            grid.Columns[SRV2].Visible = false;
            grid.Columns[SRV3].Visible = false;
            grid.Columns[PASSWORD].Visible = false;
            grid.Columns[USR1].Visible = false;
            grid.Columns[USR2].Visible = false;
        }






        #endregion

        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddConnector_Click(object sender, EventArgs e)
        {
            fmConnectorEdit fm = new fmConnectorEdit();
            fm.SetInterfaceCBList(this.GetInterfaceCBList());
            fm.Show();
        }

        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditConnector_Click(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg != null)
            {
                fmConnectorEdit fm = new fmConnectorEdit();
                fm.SetInterfaceCBList(this.GetInterfaceCBList());
                fm.SetConnectorConfig(cfg);
                fm.Show();
            }
            else
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择需要编辑的通道");
            }
        }

        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BindConnector_Click(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
            }
            ArrayList list = GetVendorCBList();
            if (list.Count == 0)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("没有未绑定的帐户");
            }
            else
            {
                fmVendorSelect fm = new fmVendorSelect();
                fm.SetConnectorConfig(cfg);


                fm.SetVendorCBList(list);
                fm.Show();
            }
        }

        void UnBindConnector_Click(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg==null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
            }
            if (fmConfirm.Show(string.Format("确认解绑:{0}", cfg.Token)) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqUnBindVendor(cfg.ID);
            }
        }
    }
}
