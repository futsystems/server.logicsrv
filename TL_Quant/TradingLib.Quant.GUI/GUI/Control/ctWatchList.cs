using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
namespace TradingLib.Quant.GUI
{
    
    
    /// <summary>
    /// Symbol观察列表,用于添加,删除Symbol
    /// 并更新数据或者选择要操作的symbol
    /// x916e705eb4ff7a49
    /// </summary>
    public partial class ctWatchList : UserControl
    {
        SymbolItemManager symbolmanager;
        TreeNode tmpnode;
        WatchListItem rootwatchlistitem;

        TreeNode mouseclicknode;
        ImageList imglist = new ImageList();

        int mouseX;
        int mouseY;
        public ctWatchList()
        {
            try
            {
                InitializeComponent();
                symbolmanager = new SymbolItemManager();
                InitNodes();
                InitMenu();
                //watchItemTree.ContextMenuStrip = folderMenu;
                watchItemTree.AfterLabelEdit += new NodeLabelEditEventHandler(watchItemTree_AfterLabelEdit);
                watchItemTree.BeforeLabelEdit += new NodeLabelEditEventHandler(watchItemTree_BeforeLabelEdit);
                watchItemTree.MouseDown += new MouseEventHandler(watchItemTree_MouseDown);
                this.Load += new EventHandler(ctWatchList_Load);
            }
            catch (Exception ex)
            { 
            
            }
            
        }

        void watchItemTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.mouseX = e.X;
                this.mouseY = e.Y;
            }
        }

        void ctWatchList_Load(object sender, EventArgs e)
        {
            //imglist.Images.SetKeyName(0, "01.png");
            //imglist.Images.SetKeyName(1, "02.png");
            imglist.Images.Add("Root",(Image)Resource.editsymbol);
            imglist.Images.Add("Folder",(Image)Resource.folder1);
            imglist.Images.Add("Symbol", (Image)Resource.symbol);
            imglist.TransparentColor = Color.Transparent;

            //imglist.Images.Add("Symbol",)

            //imglist.Images.Add(
            imglist.Images.SetKeyName(0, "Root");
            imglist.Images.SetKeyName(1, "Folder");
            imglist.Images.SetKeyName(2, "Folder");

            watchItemTree.ImageList = imglist;
        }
        bool labeledit = false;

        void watchItemTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            QuantGlobals.GDebug("befre Edit: labeledit:" + this.labeledit.ToString());
            if (!this.labeledit)
            {
                WatchListItem tag;
                TreeNode selectedNode = this.watchItemTree.SelectedNode;
                QuantGlobals.GDebug(" selectednode :" + selectedNode.Text);
                if (selectedNode == null)
                {
                    return;
                }
                if (selectedNode.Tag == null)
                {
                    return;
                }

                tag = (WatchListItem)selectedNode.Tag;
                if (tag != null)
                {
                    if (tag.IsFolder)
                    {
                        QuantGlobals.GDebug(" it is folder return directly");
                        return;
                    }
                    QuantGlobals.GDebug(" set labeledit = false");
                    this.labeledit = false;
                    e.CancelEdit = true;
                }
            }
            else
            {

                this.labeledit = false;
                e.CancelEdit = true;
            }

        }

        void watchItemTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeNode node;
            QuantGlobals.GDebug("AfterLabelEdit");
            QuantGlobals.GDebug(e.Node.Text);
            bool isSelected = e.Node.IsSelected;
            QuantGlobals.GDebug("Selected: " + isSelected.ToString());
            bool isEditing = e.Node.IsEditing;
            QuantGlobals.GDebug("IsEditing: " + isEditing.ToString() + " e.cancelEdit:"+e.CancelEdit.ToString());
            if (e.CancelEdit)
            {
                return;
            }
            if (this.watchItemTree.SelectedNode == null)
            {
                return;
            }
            node = this.watchItemTree.SelectedNode;
            WatchListItem tag = (WatchListItem)node.Tag;
            QuantGlobals.GDebug("selected node:" + node.Text.ToString());

            if (e.Label != null)
            {
                tag.Folder.FolderName = e.Label;
                node.Text = e.Label;
            }
            if (e.Node.IsSelected)
            {
                this.symbolmanager.SaveSymbolItems();
                this.watchItemTree.Sort();
                //return;
            }
            this.watchItemTree.LabelEdit = false;
            //this.labeledit = true;
            e.CancelEdit = true;

        }

        private void watchItemTree_MouseUp(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("mouse event");
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //QuantGlobals.GDebug("mouse event");
                Point pt = new Point(e.X, e.Y);
                this.mouseclicknode = watchItemTree.GetNodeAt(pt);
                //QuantGlobals.GDebug("got node:" + tmpnode.ToString());
                if(this.mouseclicknode != null)
                {
                    WatchListItem tag = (WatchListItem) this.mouseclicknode.Tag;

                    if (!tag.IsFolder)
                    {
                        this.symbolMenu.Show(watchItemTree, pt);
                    }
                    else
                    {
                        if (tag.Parent == null)//根菜单 无法删除
                        {

                        }
                        else
                        {

                        }
                        this.folderMenu.Show(watchItemTree, pt);
                    }
                }
                     
            }
        }

        public WatchListItem GetRootItem()
        {
            return this.symbolmanager.RootItem;
        }
        private ContextMenuStrip folderMenu;
        private ContextMenuStrip symbolMenu;

        private void InitMenu()
        {
            folderMenu  = new ContextMenuStrip();
            folderMenu.Items.Add("添加合约组", null, new EventHandler(AddItemFolder));
            folderMenu.Items.Add("删除合约组", null, new EventHandler(DelItemFolder));
            folderMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            folderMenu.Items.Add("重命名", null, new EventHandler(RenameItemFolder));
            folderMenu.Items.Add("设置", null, new EventHandler(EditItemFolder));
            folderMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            folderMenu.Items.Add("添加合约", null, new EventHandler(AddSymbol));

            symbolMenu = new ContextMenuStrip();
            symbolMenu.Items.Add("信 息", null, new EventHandler(ShowSymbolInfo));
            symbolMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            symbolMenu.Items.Add("k线图", null, new EventHandler(ShowChart));
            symbolMenu.Items.Add("k线图(设置)", null, new EventHandler(ShowChart));
            symbolMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            symbolMenu.Items.Add("显示Bar数据", null, new EventHandler(ShowBarData));
            symbolMenu.Items.Add("导入Bar数据", null, new EventHandler(EditItemFolder));
            symbolMenu.Items.Add("导出Bar数据", null, new EventHandler(SaveBar2CSV));
            symbolMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            symbolMenu.Items.Add("显示Tick数据", null, new EventHandler(ShowTickData));
            symbolMenu.Items.Add("导入Tick数据", null, new EventHandler(ImportTick));
            symbolMenu.Items.Add("导出Tick数据", null, new EventHandler(SaveTick2CSV));

            symbolMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            symbolMenu.Items.Add("删除合约", null, new EventHandler(AddSymbol));
            symbolMenu.Items.Add("删除Bar数据", null, new EventHandler(DeleteBarData));
            symbolMenu.Items.Add("删除Tick数据", null, new EventHandler(DeleteTickData));
            symbolMenu.Items.Add("删除合约与数据", null, new EventHandler(AddSymbol));


        }
        #region symbol菜单
        private void DeleteBarData(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                s.Frequency = BarFrequency.OneMin;//功能完全后 这里由配置文件提供

                SecurityFreq sf = new SecurityFreq(s.Security, s.Frequency);

                PluginReference reference = QuantGlobals.PluginManager.CreatePlugin(QuantGlobals.DataStoreSetting, AppDomain.CurrentDomain);

                IDataStore bstore = (IDataStore)reference.Plugin;
                bstore.GetBarStorage(sf).Delete(DateTime.MinValue, DateTime.MaxValue);
                
            }
        }
        private void DeleteTickData(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                s.Frequency = BarFrequency.OneMin;//功能完全后 这里由配置文件提供

                SecurityFreq sf = new SecurityFreq(s.Security, s.Frequency);

                PluginReference reference = QuantGlobals.PluginManager.CreatePlugin(QuantGlobals.DataStoreSetting, AppDomain.CurrentDomain);

                IDataStore bstore = (IDataStore)reference.Plugin;
                
                bstore.GetTickStorage(sf.Security).Delete(DateTime.MinValue, DateTime.MaxValue);
            }
        }


        private void ImportTick(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                s.Frequency = BarFrequency.OneMin;//功能完全后 这里由配置文件提供

                SecurityFreq sf = new SecurityFreq(s.Security, s.Frequency);

                TickConvert fm = new TickConvert();
                fm.SecurityFreq = sf;
                fm.Text = "导入Tick数据(" + sf.Security.FullName + ")";
                fm.ShowDialog();
            }
        }
        private void SaveBar2CSV(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                s.Frequency = BarFrequency.OneMin;//功能完全后 这里由配置文件提供

                SecurityFreq sf = new SecurityFreq(s.Security, s.Frequency);

                DataOutPut fm = new DataOutPut();
                fm.Type = QSENumOutPutType.Bar;
                fm.SecurityFreq = sf;
                fm.ShowDialog();
            }


        }

        private void SaveTick2CSV(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                s.Frequency = BarFrequency.OneMin;//功能完全后 这里由配置文件提供

                SecurityFreq sf = new SecurityFreq(s.Security, s.Frequency);
                
                DataOutPut fm = new DataOutPut();
                fm.SecurityFreq = sf;
                fm.ShowDialog();
            }


        }
        private void ShowSymbolInfo(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            SecurityImpl sec;
            fmSymbolInfo fm;
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol!=null)
            {
                sec = (SecurityImpl)item.Symbol;
                fm = new fmSymbolInfo(sec);
                if (fm.ShowDialog() != DialogResult.OK) return;
                //保存修改
                this.mouseclicknode.Text = sec.ToString();
                this.symbolmanager.SaveSymbolItems();
                
            }

        }

        private void ShowChart(object sender, EventArgs e)
        { 
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            SecurityImpl sec;
            
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                s.Frequency = BarFrequency.OneMin;//功能完全后 这里由配置文件提供

                SecurityFreq sf = new SecurityFreq(s.Security, s.Frequency);

                QuantGlobals.Access.CreateChartInstance(sf);
            }
            
        }
        private void ShowBarData(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;
            SecurityImpl sec;
            
            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                s.Frequency = BarFrequency.OneMin;//功能完全后 这里由配置文件提供

                SecurityFreq sf = new SecurityFreq(s.Security, s.Frequency);
                QuantGlobals.GDebug("symbol:" + sf.ToUniqueId());
                QuantGlobals.Access.ShowBarData(sf);
            }
            
        }

        private void ShowTickData(object sender, EventArgs e)
        {
            if (this.mouseclicknode == null) return;
            WatchListItem item;

            item = (WatchListItem)this.mouseclicknode.Tag;
            if (!item.IsFolder && item.Symbol != null)
            {
                SymbolSetup s = item.GetSymbolSetup();
                
                QuantGlobals.Access.ShowTickData(s.Security);
            }

        }



        #endregion

        #region Folder菜单
        //设置合约目录组
        private void EditItemFolder(object sender, EventArgs e)
        {
            WatchListItem item;
            //xf4697accda07bb82 xfaccdabb;
            fmSymbolFolderConfig fm;
            int frequency;
            Dictionary<SecurityFreq, bool> dictionary;
            Dictionary<SecurityFreq, bool> dictionary2;
            if (this.mouseclicknode != null)
            {
                item = (WatchListItem)this.mouseclicknode.Tag;
                if (item != null && item.IsFolder)
                {
                     fm= new fmSymbolFolderConfig(item);
                    
                    if (fm.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    QuantGlobals.GDebug("folder name:" + fm.Item.Folder.FolderName);

                    this.mouseclicknode.Text = item.Folder.FolderName =fm.Folder.FolderName;
                    item.Folder.BrokerService = fm.Folder.BrokerService;
                    item.Folder.HistService = fm.Folder.HistService;
                    item.Folder.RealtimeService = fm.Folder.RealtimeService;

                    item.Folder.InheritsBrokerService = fm.Folder.InheritsBrokerService;
                    item.Folder.InheritsHistService = fm.Folder.InheritsHistService;
                    item.Folder.InheritsRealtimeService = fm.Folder.InheritsRealtimeService;

                    item.Folder.SaveBars = fm.Folder.SaveBars;
                    item.Folder.SaveTicks = fm.Folder.SaveTicks;
                    item.Folder.InheritsSaveBars = fm.Folder.InheritsSaveBars;
                    item.Folder.InheritsSaveTicks = fm.Folder.InheritsSaveTicks;

                    //item.Folder.Frequency = xfaccdabb.Folder.Frequency;
                    //item.Folder.InheritsSaveBars = xfaccdabb.Folder.InheritsSaveBars;
                    //item.Folder.SaveBars = xfaccdabb.Folder.SaveBars;
                    symbolmanager.SaveSymbolItems();
                }

            }

        }

        //重命名合约组
        private void RenameItemFolder(object sender, EventArgs e)
        {
            TreeNode nodeAt = this.watchItemTree.GetNodeAt(this.mouseX, this.mouseY);
            if (nodeAt != null)
            {
                WatchListItem tag = (WatchListItem)this.mouseclicknode.Tag;
                if (tag.Parent == null) return;
                watchItemTree.LabelEdit = true;
                nodeAt.BeginEdit();
            }
        }
        //增加合约文件夹
        void AddItemFolder(object sender, EventArgs e)
        {
            WatchListItem tag;
            WatchListItem item = new WatchListItem(true);
            TreeNode node = null;
            item.Folder.FolderName = "New Folder";
            node = new TreeNode(item.Folder.FolderName)
            {
                ImageIndex = 1,
                SelectedImageIndex = 1,
                Tag = item
            };
            node.Expand();
            if(this.mouseclicknode == null) return;
            tag = (WatchListItem)mouseclicknode.Tag;
            //MessageBox.Show(tag.IsFolder.ToString() + " " + tag.Folder.FolderName);

            item.Parent = tag;
            tag.Folder.Contents.Add(item);

            mouseclicknode.Nodes.Add(node);
            watchItemTree.SelectedNode = node;
            symbolmanager.SaveSymbolItems();
            watchItemTree.LabelEdit = true;
            node.BeginEdit();
        }
        //删除合约文件夹
        private void DelItemFolder(object sender, EventArgs e)
        {
            if (((this.mouseclicknode != null) && (0 == 0)) && (this.mouseclicknode.Tag != null))
            {
                WatchListItem tag = (WatchListItem)this.mouseclicknode.Tag;
                if (tag.IsFolder)//文件夹
                {
                    WatchListItem parent = null;
                    if ((tag.Parent != null))//非root文件夹
                    {
                        parent = tag.Parent;
                        fmConfirm fm = new fmConfirm();
                        fm.Message = "确认删除合约组与合约组内的所有合约？";

                        //if (/*this.x26773df9c7f0e618(tag) && **/(fmConfirm.Show("确认删除合约组与合约组内的所有合约？") == DialogResult.OK))
                        if(fm.ShowDialog()== DialogResult.OK)
                        {
                            parent.Folder.Contents.Remove(tag);
                            this.watchItemTree.Nodes.Remove(this.mouseclicknode);

                            this.mouseclicknode = null;
                            this.symbolmanager.SaveSymbolItems();
                        }
                    }
                }
            }
        }

        //添加合约
        private void AddSymbol(object sender, EventArgs e)
        {
            /*
            WatchListItem tag;
            if (this.mouseclicknode != null)
            {
                tag = (WatchListItem)this.mouseclicknode.Tag;
                if (tag == null) return;

                fmAddSymbol fm = new fmAddSymbol();
                QuantGlobals.GDebug("it is here a");
                if(fm.ShowDialog()!= DialogResult.OK) return;

                List<WatchListItem> collection = new List<WatchListItem>();
                foreach (SecurityImpl symbol in fm.SecurityList)
                {
                    QuantGlobals.GDebug("it is here b");
                    WatchListItem item2 = new WatchListItem(false)
                    {
                        Symbol = symbol,
                        //BarConstruction = dialog.BarConstruction
                    };
                    //x90d190a0a81758e8 xdaae = new x90d190a0a81758e8();
                    //this.xedc98063d4653351(tag, ref xdaae, false);

                    //if (!this.x8bffc0da13760e1e(xdaae.x8106778c6d829bd5(), item2, true))
                    {
                        item2.Parent = tag;
                        collection.Add(item2);
                    }

                }
                tag.Folder.Contents.AddRange(collection);
                this.AddItemListToNode(this.mouseclicknode, collection);

                //this.x3fc273297472ee43(this.xfdf4223cc2216c1a);
                this.symbolmanager.SaveSymbolItems();
            }**/
        
        }

        #endregion


        private bool x26773df9c7f0e618(WatchListItem item)
        {
            Dictionary<SecurityFreq, bool> dictionary = new Dictionary<SecurityFreq, bool>();
            this.xd63ff5fe6a910ed0(this.rootwatchlistitem, item, ref dictionary);//获得主

            List<SecurityFreq> list = new List<SecurityFreq>();
            if (!item.IsFolder)//如果不是文件夹
            {
                SecurityFreq key = new SecurityFreq(item.GetSymbolSetup());
                if (!dictionary.ContainsKey(key))//获得该item对应的security
                {
                    list.Add(key);
                }
            }
            else//如果是文件夹
            {
                Dictionary<SecurityFreq, bool> dictionary2 = new Dictionary<SecurityFreq, bool>();
                this.xd63ff5fe6a910ed0(item, null, ref dictionary2);//获得该文件夹下的所有数据

                foreach (SecurityFreq freq in dictionary2.Keys)
                {
                    if (!dictionary.ContainsKey(freq))//如果主dic中不包含folder下的securityfreq数据 则list加入该数据
                    {
                        list.Add(freq);
                    }
                }
            }
            //主要用于检查 该item是否包含了其他文件夹所不包含的数据
            return ((list.Count <= 0) || this.x3bf89bfae9804d68(list));
        }

        private bool x3bf89bfae9804d68(List<SecurityFreq> secfreqlist)
        {
            return true;
        }

        //获得folder中除watchitem之外的的所有item
        private void xd63ff5fe6a910ed0(WatchListItem folder, WatchListItem watchitem, ref Dictionary<SecurityFreq, bool> dict)
        {
            if (folder != watchitem)
            {
                foreach (WatchListItem item in folder.Folder.Contents)
                {
                    if(item != watchitem)
                    {
                        if (item.IsFolder)
                        {
                            this.xd63ff5fe6a910ed0(item, watchitem, ref dict);
                        }
                        else
                        {
                            SecurityFreq freq = new SecurityFreq(item.GetSymbolSetup());
                            dict[freq] = true;
                        }
                    }
                }
            }
        }


        private void InitNodes()
        {
            this.watchItemTree.Nodes.Clear();
            this.tmpnode = new TreeNode("合约列表");
            this.tmpnode.SelectedImageIndex = 0;
            this.tmpnode.ImageIndex = 0;

            this.watchItemTree.Nodes.Add(this.tmpnode);
            this.rootwatchlistitem = this.symbolmanager.LoadSymbolItems();
            this.tmpnode.Tag = this.rootwatchlistitem;

            this.AddItemListToNode(this.tmpnode, this.rootwatchlistitem.Folder.Contents);
            this.tmpnode.Expand();
            //this.x3fc273297472ee43(this.tmpnode, true);
        }
        private void AddItemListToNode(TreeNode treenode, List<WatchListItem> itemlist)
        {
            foreach (WatchListItem item in itemlist)
            {
                //WatchListItem item;
                string str;
                TreeNode node;
                DateTime time;

                if (!item.IsFolder)//不是文件夹
                {
                    str = item.Symbol.FullName;
                    //if(item.Symbol.Expriredate)

                    node = new TreeNode(str);
                    node.Tag = item;
                    node.SelectedImageIndex = 2;
                    node.ImageIndex = 2;
                    item.Parent = (WatchListItem)treenode.Tag;
                    treenode.Nodes.Add(node);
                }
                else
                {
                    node = new TreeNode(item.Folder.FolderName);
                    node.Tag = item;
                    item.Parent = (WatchListItem)treenode.Tag;
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    treenode.Nodes.Add(node);
                    this.AddItemListToNode(node, item.Folder.Contents);
                    
                }
            }
            
        }







       

    }
}
