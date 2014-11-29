﻿using System;
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
    public partial class fmVendorManager : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmVendorManager()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            SetPreferences_Vendor();
            InitTable_Vendor();
            BindToTable_Vendor();

            SetPreferences_RouterItem();
            InitTable_RouterItem();
            BindToTable_RouterItem();

            Factory.IDataSourceFactory(cbrgstrategytype).BindDataSource(UIUtil.genEnumList<QSEnumRouterStrategy>());
            this.Load += new EventHandler(fmConnectorCfg_Load);
        }

        void fmConnectorCfg_Load(object sender, EventArgs e)
        {
            WireEvent();

            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryInterface();
            }
            
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            configgrid.DoubleClick +=new EventHandler(configgrid_DoubleClick);
            configgrid.RowPrePaint +=new DataGridViewRowPrePaintEventHandler(RowPrePaint);

            vendorgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            routeritemgrid.RowPrePaint +=new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            ctRouterGroupList1.RouterGroupSelectedChangedEvent += new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
        
            //ctRouterGroupList1.RouterGroupSelectedChangedEvent +=new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
        }




        public void OnInit()
        {
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "NotifyConnectorCfg", this.OnNotifyConnectorConfig);
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "NotifyVendor", this.OnNotifyVendorBind);

        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "NotifyConnectorCfg", this.OnNotifyConnectorConfig);
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "NotifyVendor", this.OnNotifyVendorBind);

        }

        ConnectorInterface ID2Interface(int id)
        {
            if (interfacemap.Keys.Contains(id))
                return interfacemap[id];
            return null;

        }
        public ArrayList GetInterfaceCBList()
        {
            ArrayList list = new ArrayList();
            foreach (ConnectorInterface item in interfacemap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = item.Name;
                vo.Value = item.ID;
                list.Add(vo);
            }
            return list;
        }

        ConcurrentDictionary<int, ConnectorInterface> interfacemap = new ConcurrentDictionary<int, ConnectorInterface>();
        bool _gotinterface = false;
        void OnQryInterface(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                ConnectorInterface[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<ConnectorInterface[]>(jd);
                foreach (ConnectorInterface op in objs)
                {
                    if (!interfacemap.Keys.Contains(op.ID))
                        interfacemap.TryAdd(op.ID, op);
                }
                _gotinterface = true;
                if (!_gotvendor)
                {
                    Globals.TLClient.ReqQryVendor();
                }
            }
            else//如果没有配资服
            {

            }
        }
        void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }




    

    }
}
