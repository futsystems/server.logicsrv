using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter.Controls.Base
{
    public partial class ctAccountType : UserControl, IEventBinder
    {
        public event VoidDelegate AccountTypeSelectedChangedEvent;
        public ctAccountType()
        {
            InitializeComponent();
            this.Load += new EventHandler(ctAccountType_Load);
        }


        //属性获得和设置
        [DefaultValue(true)]
        bool _enableany = false;
        public bool EnableAny
        {
            get
            {
                return _enableany;
            }
            set
            {
                _enableany = value;
            }
        }

        public int SelectedIndex
        {
            get
            {

                return accountType.SelectedIndex;
            }
        }

        public QSEnumAccountCategory AccountType
        {
            get
            {
                try
                {
                    return (QSEnumAccountCategory)accountType.SelectedValue;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }

            set
            {
                try
                {
                    accountType.SelectedIndex = 1;
                }
                catch (Exception ex)
                {
                }
            }
        }

        void ctAccountType_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            accountType.SelectedIndexChanged += new EventHandler(accountType_SelectedIndexChanged);
        }

        void accountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AccountTypeSelectedChangedEvent != null)
                AccountTypeSelectedChangedEvent();
        }

        public void OnInit()
        {
            Factory.IDataSourceFactory(accountType).BindDataSource(MoniterUtils.GetAccountTypeCombList(this.EnableAny));
        }

        public void OnDisposed()
        {
            
        }
    }
}
