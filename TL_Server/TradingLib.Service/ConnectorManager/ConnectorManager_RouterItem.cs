using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        /// <summary>
        /// 初始化Vendor
        /// 将实盘通道绑定到Vendor
        /// </summary>
        void InitVendor()
        {

            //foreach (VendorImpl v in BasicTracker.VendorTracker.Vendors)
            //{
            //    //将业务通道对象绑定到实盘对象
            //    v.BindBroker(ConnectorID2Broker(v.connector_id));
            //}
        }
    }
}
