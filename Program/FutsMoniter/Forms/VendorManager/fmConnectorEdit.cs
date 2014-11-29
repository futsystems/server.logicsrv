using System;
using System.Collections.Generic;
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
    public partial class fmConnectorEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmConnectorEdit()
        {
            InitializeComponent();
            this.Text = "添加交易通道";
        }

        public void SetInterfaceCBList(ArrayList list)
        {
            Factory.IDataSourceFactory(cbinterfacelist).BindDataSource(list);
            
        }
        ConnectorConfig _cfg;
        public void SetConnectorConfig(ConnectorConfig cfg)
        {
            _cfg = cfg;
            id.Text = cfg.ID.ToString();
            token.Text = cfg.Token;
            address.Text = cfg.srvinfo_ipaddress;
            port.Text = cfg.srvinfo_port.ToString();
            srvf1.Text = cfg.srvinfo_field1;
            srvf2.Text = cfg.srvinfo_field2;
            srvf3.Text = cfg.srvinfo_field3;
            username.Text = cfg.usrinfo_userid;
            pass.Text = cfg.usrinfo_password;
            uf1.Text = cfg.usrinfo_field1;
            uf2.Text = cfg.usrinfo_field2;
            cbinterfacelist.SelectedValue = cfg.interface_fk;
            name.Text = cfg.Name;
            this.Text = string.Format("编辑交易通道[{0}]", cfg.Token);

            token.Enabled = false;
            cbinterfacelist.Enabled = false;

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //新增
            if (_cfg == null)
            {
                ConnectorConfig cfg = new ConnectorConfig();
                cfg.interface_fk = int.Parse(cbinterfacelist.SelectedValue.ToString());
                cfg.Name = name.Text;
                cfg.srvinfo_ipaddress = address.Text;
                cfg.srvinfo_port = int.Parse(port.Text);
                cfg.srvinfo_field1 = srvf1.Text;
                cfg.srvinfo_field2 = srvf2.Text;
                cfg.srvinfo_field3 = srvf3.Text;

                cfg.usrinfo_userid = username.Text;
                cfg.usrinfo_password = pass.Text;
                cfg.usrinfo_field1 = uf1.Text;
                cfg.usrinfo_field2 = uf2.Text;
                cfg.Token = token.Text;
                if (fmConfirm.Show("确认添加通道设置?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateConnectorConfig(TradingLib.Mixins.LitJson.JsonMapper.ToJson(cfg));
                }
            }
            else
            {
                _cfg.srvinfo_ipaddress = address.Text;
                _cfg.srvinfo_port = int.Parse(port.Text);
                _cfg.srvinfo_field1 = srvf1.Text;
                _cfg.srvinfo_field2 = srvf2.Text;
                _cfg.srvinfo_field3 = srvf3.Text;

                _cfg.usrinfo_userid = username.Text;
                _cfg.usrinfo_password = pass.Text;
                _cfg.usrinfo_field1 = uf1.Text;
                _cfg.usrinfo_field2 = uf2.Text;
                _cfg.Name = name.Text;
                if (fmConfirm.Show("确认修改通道设置?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateConnectorConfig(TradingLib.Mixins.LitJson.JsonMapper.ToJson(_cfg));
                }
                this.Close();
            }
        }
    }
}
