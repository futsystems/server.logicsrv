using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public class WatchListFolder
    {
        // Fields
        public string BrokerService = "";
        public string RealtimeService = "";
        public List<WatchListItem> Contents = new List<WatchListItem>();
        public DateTime DownloadStartDate = DateTime.MinValue;
        public string FolderName = "";
        public BarFrequency Frequency =null;
        public string HistService = "";
        public bool InheritsBrokerService = true;
        public bool InheritsDownloadStartDate = true;
        public bool InheritsHistService = true;
        public bool InheritsRealtimeService = true;
        public bool InheritsSaveBars = true;
        public bool InheritsSaveTicks = true;
        
        public bool SaveBars = true;
        public bool SaveTicks;
        //[CompilerGenerated]
        //private static Func<WatchListItem, WatchListItem> x31af784cbc72c68d;

        // Methods
        /*
        public WatchListFolder Clone()
        {
            WatchListFolder folder = (WatchListFolder)base.MemberwiseClone();
            if (x31af784cbc72c68d == null)
            {
                x31af784cbc72c68d = new Func<WatchListItem, WatchListItem>(WatchListFolder.xb8f0a3429c7477b5);
            }
            folder.Contents = this.Contents.Select<WatchListItem, WatchListItem>(x31af784cbc72c68d).ToList<WatchListItem>();
            return folder;
        }

        [CompilerGenerated]
        private static WatchListItem xb8f0a3429c7477b5(WatchListItem xccb63ca5f63dc470)
        {
            WatchListItem item = xccb63ca5f63dc470.Clone();
            item.Parent = null;
            return item;
        }**/

        // Properties
        public bool InheritsFrequency
        {
            get
            {
                return (this.Frequency == null);
            }
            set
            {
                if (value)
                {
                    this.Frequency = null;
                }
                else if (this.Frequency == null)
                {
                    this.Frequency = BarFrequency.OneMin;
                }
            }
        }
    }


}
