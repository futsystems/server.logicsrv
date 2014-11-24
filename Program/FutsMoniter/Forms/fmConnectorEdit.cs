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
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;


namespace FutsMoniter
{
    public partial class fmConnectorEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmConnectorEdit()
        {
            InitializeComponent();
        }

        public void SetConnectorConfig(ConnectorConfig cfg)
        {
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

        }
    }
}
