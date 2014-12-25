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
    public partial class fmSyncSymbol : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmSyncSymbol()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmSyncSymbol_Load);

        }

        void SetCutVendor()
        {
            if (Globals.Domain.CFG_SyncVendor_ID == 0)
            {
                this.cutVendor.Text = "未设置";
            }
            if(vendormap.Keys.Contains(Globals.Domain.CFG_SyncVendor_ID))
            {
                this.cutVendor.Text = vendormap[Globals.Domain.CFG_SyncVendor_ID].Name;
            }
            else
            {
                this.cutVendor.Text = "设置异常";
            }
        }


        void fmSyncSymbol_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            VendorSetting vendor = (VendorSetting)cbVendor.SelectedValue;
            if (MoniterUtils.WindowConfirm("确认默认从该实盘帐户:[" + vendor.Name + "] 同步合约数据?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqUpdateSyncVendor(vendor.ID);
            }
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.LogicEvent.GotDomainEvent += new Action<DomainImpl>(LogicEvent_GotDomainEvent);
            Globals.TLClient.ReqQryVendor();
        }

        void LogicEvent_GotDomainEvent(DomainImpl obj)
        {
            SetCutVendor();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.LogicEvent.GotDomainEvent -= new Action<DomainImpl>(LogicEvent_GotDomainEvent);
        }


        Dictionary<int, VendorSetting> vendormap = new Dictionary<int, VendorSetting>();

        bool _gotvendor = false;
        void OnQryVendor(string jsonstr)
        {
            VendorSetting[] objs = MoniterUtils.ParseJsonResponse<VendorSetting[]>(jsonstr);
            if (objs != null)
            {
                Factory.IDataSourceFactory(cbVendor).BindDataSource(GetVendorCBList(objs));
                foreach (VendorSetting vendor in objs)
                {
                    vendormap.Add(vendor.ID, vendor);
                }
                SetCutVendor();
                _gotvendor = true;
                
            }
        }

        public ArrayList GetVendorCBList(VendorSetting[] objs)
        {
            ArrayList list = new ArrayList();
            foreach (VendorSetting item in objs)
            {
                ValueObject<VendorSetting> vo = new ValueObject<VendorSetting>();
                vo.Name = item.Name;
                vo.Value = item;
                list.Add(vo);
            }
            return list;
        }



    }
}
