using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;

namespace TradingLib.Quant.GUI
{
    public partial class fmSymbolFolderConfig :KryptonForm
    {

        WatchListItem _item = null;
        public WatchListItem Item { get { return _item; } set { _item = value; } }

        public WatchListFolder newfoldersetting = new WatchListFolder();
        ServiceManager _servicemanger;
        public fmSymbolFolderConfig(WatchListItem item)
        {
            InitializeComponent();
            _servicemanger = QuantGlobals.Access.GetServiceManager();
            _item = item;
            folderName.Text = Item.Folder.FolderName;
            InitCombox();
        }


        public WatchListFolder Folder { get { return newfoldersetting; } }

        string getYesNo(bool b)
        {
            return b ? "Yes" : "No";
        }
        void InitCombox()
        {
            this.livdataCom.Items.Clear();
            this.hisdataCom.Items.Clear();
            this.brokerCom.Items.Clear();

            this.livdataCom.Items.Add(new ComboBoxItem("<无>", ""));
            this.hisdataCom.Items.Add(new ComboBoxItem("<无>", ""));
            this.brokerCom.Items.Add(new ComboBoxItem("<无>", ""));
            if (Item.Parent != null)
            {
                this.livdataCom.Items.Add(new ComboBoxItem("继承:(" + getServiceName(Item.Parent.RealtimeService)+")", ""));
                this.hisdataCom.Items.Add(new ComboBoxItem("继承:(" + getServiceName(Item.Parent.HistService) + ")", ""));
                this.brokerCom.Items.Add(new ComboBoxItem("继承:(" + getServiceName(Item.Parent.BrokerService) + ")", ""));

                this.saveBars.Items.Add(new ComboBoxItem("继承:(" + getYesNo(Item.Parent.SaveBars) + ")", ""));
                this.saveTicks.Items.Add(new ComboBoxItem("继承:(" + getYesNo(Item.Parent.SaveTicks) + ")", ""));
            }

            foreach (ServiceSetup service in _servicemanger.Services)
            {
                ServiceInfo serviceInfo;
                serviceInfo = this._servicemanger.GetServiceInfo(service.FriendlyName);//获得ServiceInfo
                if(serviceInfo.BarDataAvailable)
                    this.hisdataCom.Items.Add(new ComboBoxItem(service.FriendlyName,service.FriendlyName));
                if (serviceInfo.BrokerFunctionsAvailable)
                    this.brokerCom.Items.Add(new ComboBoxItem(service.FriendlyName, service.FriendlyName));
                if (serviceInfo.TickDataAvailable)
                    this.brokerCom.Items.Add(new ComboBoxItem(service.FriendlyName, service.FriendlyName));
            }

            this.saveBars.Items.Add(new ComboBoxItem("Yes", true));
            this.saveBars.Items.Add(new ComboBoxItem("No", false));
            this.saveTicks.Items.Add(new ComboBoxItem("Yes", true));
            this.saveTicks.Items.Add(new ComboBoxItem("No", false));



            this.SelectServiceCombox(livdataCom, Item.Folder.RealtimeService, Item.Folder.InheritsRealtimeService);
            this.SelectServiceCombox(hisdataCom, Item.Folder.HistService, Item.Folder.InheritsHistService);
            this.SelectServiceCombox(brokerCom, Item.Folder.BrokerService, Item.Folder.InheritsBrokerService);
            
            SelectSaveCombox(saveBars, Item.SaveBars, Item.Folder.InheritsSaveBars);
            SelectSaveCombox(saveTicks, Item.SaveTicks, Item.Folder.InheritsSaveTicks);

        }

        void SaveConfig()
        {
            newfoldersetting.FolderName= folderName.Text;

            this.GetComboxSelection(livdataCom, ref newfoldersetting.RealtimeService, ref newfoldersetting.InheritsRealtimeService);
            this.GetComboxSelection(hisdataCom, ref newfoldersetting.HistService, ref newfoldersetting.InheritsHistService);
            this.GetComboxSelection(brokerCom, ref newfoldersetting.BrokerService, ref newfoldersetting.InheritsBrokerService);

            if (saveBars.SelectedItem != null)
            { 
                int index = saveBars.SelectedIndex;
                if (index == 0)
                {
                    newfoldersetting.InheritsSaveBars = true;
                    newfoldersetting.SaveBars = Item.Parent.SaveBars;
                }
                else if (index == 1)
                {
                    newfoldersetting.InheritsSaveBars = false;
                    newfoldersetting.SaveBars = true;
                }
                else if (index == 2)
                {
                    newfoldersetting.InheritsSaveBars = false;
                    newfoldersetting.SaveBars = false;
                }
            }
            if (saveTicks.SelectedItem != null)
            {
                int index = saveTicks.SelectedIndex;
                if (index == 0)
                {
                    newfoldersetting.InheritsSaveTicks = true;
                    newfoldersetting.SaveTicks = Item.Parent.SaveBars;
                }
                else if (index == 1)
                {
                    newfoldersetting.InheritsSaveTicks = false;
                    newfoldersetting.SaveTicks = true;
                }
                else if (index == 2)
                {
                    newfoldersetting.InheritsSaveTicks = false;
                    newfoldersetting.SaveTicks = false;
                }
                
            }
        }


        private void GetComboxSelection(ComboBox combox, ref string service, ref bool inhert)
        {
            if (combox.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)combox.SelectedItem;
                if (!((string)selectedItem.Tag == ""))
                {
                    inhert = false;
                    service = (string)selectedItem.Tag;
                }
                else
                {
                    if (!(selectedItem.Name == "<无>"))
                    {
                        inhert = true;
                        service = "";
                    }
                    else
                    {
                        inhert = false;
                        service = "";
                    }
                }
            }
        }



        void SelectSaveCombox(ComboBox combox, bool save, bool inherit)
        {
            if (inherit)
                combox.SelectedIndex = 0;
            else
                combox.SelectedIndex = save ? 1 : 2;

        }

        bool SelectServiceCombox(ComboBox combox, string service, bool inherit)
        {

            if (inherit && Item.Parent != null)
            {
                foreach(ComboBoxItem item in combox.Items)
                {
                    if (item.Name.StartsWith("继承"))
                    {
                        combox.SelectedItem = item;
                        return true;
                    }
                }
                return false;
            }
            if (service != null)
            {
                return GUIUtils.SelectComboItem(combox, service);
            }
            foreach (ComboBoxItem item2 in combox.Items)
            {
                if(item2.Name == "<None>")
                {
                    combox.SelectedItem = item2;
                    return true;
                }
            }
            return false;

        }
        string getServiceName(string service)
        {
            if (!string.IsNullOrEmpty(service))
                return service;
            return "<无>";
        }
        private void ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            SaveConfig();
            this.Close();
        }
    }
}
