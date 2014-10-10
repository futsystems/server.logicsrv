using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;

using System.Xml.Serialization;

namespace TradingLib.Quant.Base
{
    public class WatchListItem
    {
        // Fields
        private BarConstructionType x0d63afdb95496466;
        private WatchListFolder folder;//folder;
        private bool isfolder;//isfolder;
        private WatchListItem item;
        private SecurityImpl security;

        // Methods
        public WatchListItem()
        {
        }

        public WatchListItem(bool isFolder)
        {
            this.isfolder = isFolder;
        }

        public WatchListItem Clone()
        {
            WatchListItem item = (WatchListItem)base.MemberwiseClone();
            if (this.folder != null)
            {
                item.folder = this.folder;
                foreach (WatchListItem item2 in item.folder.Contents)
                {
                    item2.Parent = item;
                }
            }
            if (this.Symbol != null)
            {
                item.Symbol = this.Symbol;
            }
            return item;
        }

        public SymbolSetup GetSymbolSetup()
        {
            return new SymbolSetup(this.Symbol, this.Frequency) 
            { HistService = this.HistService, RealtimeService = this.RealtimeService, BrokerService = this.BrokerService, SaveLiveBars = this.SaveBars, SaveLiveTicks = this.SaveTicks, DownloadStartDate = this.DownloadStartDate,Frequency = this.Frequency };
        }

        public void SetFromSymbolSetup(SymbolSetup symbolSetup)
        {
            //this.BarConstruction = symbolSetup.BarConstruction;
            this.Symbol = symbolSetup.Security as SecurityImpl;
        }

        public override string ToString()
        {
            if (this.isfolder)
            {
                return this.folder.FolderName;
            }
            return this.security.ToString();
        }

        // Properties
        
        public BarConstructionType BarConstruction
        {
            get
            {
                return this.x0d63afdb95496466;
            }
            set
            {
                this.x0d63afdb95496466 = value;
            }
        }
        

        public string BrokerService
        {
            get
            {
                if (this.IsFolder && !this.Folder.InheritsBrokerService)
                {
                    return this.Folder.BrokerService;
                }
                if (this.item == null)
                {
                    return "";
                }
                return this.item.BrokerService;
            }
        }

        public DateTime DownloadStartDate
        {
            get
            {
                if (this.IsFolder && ((this.item == null) || !this.Folder.InheritsDownloadStartDate))
                {
                    return this.Folder.DownloadStartDate;
                }
                if (this.item == null)
                {
                    return DateTime.MinValue;
                }
                return this.item.DownloadStartDate;
            }
        }

        public WatchListFolder Folder
        {
            get
            {
                if (!this.isfolder)
                {
                    return null;
                }
                if(this.folder == null)
                {
                    this.folder = new WatchListFolder();
                }
                return this.folder;
            }
            set
            {
                this.folder = value;
            }
        }

        public BarFrequency Frequency
        {
            get
            {
                if (this.IsFolder)
                {
                    if(!this.Folder.InheritsFrequency)
                    {
                        return this.Folder.Frequency;
                    }
                }
                if(this.item == null)
                {
                    return BarFrequency.OneMin;
                }
                return this.item.Frequency;
            }
        }

        public string HistService
        {
            get
            {
                if (this.IsFolder && !this.Folder.InheritsHistService)
                {
                    return this.Folder.HistService;
                }
                if (this.item == null)
                {
                    return "";
                }
                return this.item.HistService;
            }
        }

        public bool IsFolder
        {
            get
            {
                return this.isfolder;
            }
            set
            {
                this.isfolder = value;
            }
        }

        [XmlIgnore]
        public WatchListItem Parent
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }

        public string RealtimeService
        {
            get
            {
                if (this.IsFolder)
                {
                    if (this.item!=null & this.Folder.InheritsRealtimeService)
                    {
                        return this.item.RealtimeService;
                    }
                    else
                    {
                        return this.Folder.RealtimeService;
                    }
                }
                
                if (this.item == null)
                {
                    return "";
                }
                return this.item.RealtimeService;
            }
        }

        public bool SaveBars
        {
            get
            {
                if (this.IsFolder)
                {
                    if ((this.item != null) && this.Folder.InheritsSaveBars)
                    {
                        return this.item.SaveBars;
                    }
                    else
                    {
                        return this.Folder.SaveBars;
                    }
                   
                }
                if (this.item == null)
                {
                    return false;
                }
                return this.item.SaveBars;
            }
        }

        public bool SaveTicks
        {
            get
            {
                if (this.IsFolder)
                {
                    if (this.item != null && this.Folder.InheritsSaveTicks)
                    {
                            return this.item.SaveTicks;
                    }
                    return this.Folder.SaveTicks;
                }
                if (this.item == null)
                {
                    return false;
                }
                return this.item.SaveTicks;
            }
        }

        public SecurityImpl Symbol
        {
            get
            {
                if (this.isfolder)
                {
                    return null;
                }
                if (this.security == null)
                {
                    this.security = new SecurityImpl();
                }
                return this.security;
            }
            set
            {
                this.security = value;
            }
        }
    }


}
