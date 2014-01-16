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
            this.FormClosing += new FormClosingEventHandler(fmVendorManager_FormClosing);
        }

        void fmVendorManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (positionmetricmontier != null)
            //{
            //    positionmetricmontier.Close();
            //}
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
            configgrid.MouseClick += new MouseEventHandler(configgrid_MouseClick);

            vendorgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            routeritemgrid.RowPrePaint +=new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            ctRouterGroupList1.RouterGroupSelectedChangedEvent += new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);

            btnUpdateRouterGroup.Click += new EventHandler(btnUpdateRouterGroup_Click);
            tabholder.SelectedPageChanged += new EventHandler(tabholder_SelectedPageChanged);
            //ctRouterGroupList1.RouterGroupSelectedChangedEvent +=new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
        }



        void tabholder_SelectedPageChanged(object sender, EventArgs e)
        {
            if (tabholder.SelectedPage == this.pageroutergroup)
            {
                RefreshRouterItem();
            }
        }






        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyConnectorCfg", this.OnNotifyConnectorConfig);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyVendor", this.OnNotifyVendorBind);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyVendor", this.OnNotifyVendorBind);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyRouterItem", this.OnNotifyRouterItem);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyRouterGroup", this.OnNotifyRouterGroup);

            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryConnectorStatus", this.OnQryConnectorStatus);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyConnectorStatus", this.OnNotifyConnectorStatus);

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "NotifyConnectorCfg", this.OnNotifyConnectorConfig);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyVendor", this.OnNotifyVendorBind);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "NotifyRouterItem", this.OnNotifyRouterItem);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "NotifyVendor", this.OnNotifyVendorBind);

            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryConnectorStatus", this.OnQryConnectorStatus);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "NotifyConnectorStatus", this.OnNotifyConnectorStatus);

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