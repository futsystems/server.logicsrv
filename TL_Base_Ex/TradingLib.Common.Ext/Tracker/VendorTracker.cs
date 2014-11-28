using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class VendorTracker
    {
        ConcurrentDictionary<int, VendorImpl> vendoermap = new ConcurrentDictionary<int, VendorImpl>();

        public VendorTracker()
        {
            foreach (VendorImpl v in ORM.MRouterGroup.SelectVendor())
            {
                vendoermap.TryAdd(v.ID, v);
            }
        }

        /// <summary>
        /// 按全局ID获得Vendoer对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VendorImpl this[int id]
        {
            get
            {
                if (vendoermap.Keys.Contains(id))
                    return vendoermap[id];
                return null;
            }
        }

        /// <summary>
        /// 获得所有实盘帐户对象
        /// </summary>
        public IEnumerable<VendorImpl> Vendors
        {
            get
            {
                return vendoermap.Values;
            }
        }
    }
}
