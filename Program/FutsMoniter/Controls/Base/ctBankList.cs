﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class ctBankList : UserControl
    {
        public event VoidDelegate BankSelectedChangedEvent;
        bool _gotdata = false;
        public ctBankList()
        {
            InitializeComponent();
            //如果已经初始化完成 则直接读取数据填充 否则将资金放入事件回调中
            
            if (Globals.EnvReady)
            {
                if (!_gotdata)//请求银行列表
                {
                    Globals.TLClient.ReqQryBank();
                }
            }
            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryBank", this.OnQryBank);
            }
            this.Disposed += new EventHandler(ctBankList_Disposed);
            this.cbbank.SelectedIndexChanged +=new EventHandler(cbbank_SelectedIndexChanged);
        }

        void ctBankList_Disposed(object sender, EventArgs e)
        {
            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryBank", this.OnQryBank);
            }
        }

        void OnQryBank(string jsonstr)
        {

            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperBank[] obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperBank[]>(jd["Playload"].ToJson());
                GotBankList(obj);
                _gotdata = true;
            }
            else//如果没有配资服
            {

            }
        }
        delegate void del1(JsonWrapperBank[] banks);
        void GotBankList(JsonWrapperBank[] banks)
        {
            if (InvokeRequired)
            {
                Invoke(new del1(GotBankList), new object[] { banks });
            }
            else
            {
                ArrayList list = new ArrayList();
                foreach (JsonWrapperBank bank in banks)
                {
                    ValueObject<int> vo = new ValueObject<int>
                    {
                        Name = bank.Name,
                        Value = bank.ID,
                    };
                    list.Add(vo);
                }

                Factory.IDataSourceFactory(cbbank).BindDataSource(list);
                cbbank.SelectedValue = _bankselected;
                _gotdata = true;
            }
        }

        int _bankselected = 0;
        public int BankSelected 
        {
            get
            {
                try
                {
                    return int.Parse(cbbank.SelectedValue.ToString());
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
                    _bankselected = value;
                    cbbank.SelectedValue = value;
                }
                catch (Exception ex)
                { 
                    
                }
            }
        }

        private void cbbank_SelectedIndexChanged(object sender,EventArgs e)
        {

            if (BankSelectedChangedEvent!= null && _gotdata)
            {

                BankSelectedChangedEvent();
                
            }
        }


     
    }
}