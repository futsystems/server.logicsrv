using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.DataCore
{
    public class MDService
    {
        static MDService defaultinstance = null;

        static MDService()
        {
            defaultinstance = new MDService();
        }

        private MDService()
        { 
        
        }

        EventData _eventdata;

        /// <summary>
        /// 数据事件集合
        /// </summary>
        public static EventData EventData
        {
            get
            {
                if (defaultinstance._eventdata == null)
                    defaultinstance._eventdata = new EventData();
                return defaultinstance._eventdata;
            }
        }
    }
}
