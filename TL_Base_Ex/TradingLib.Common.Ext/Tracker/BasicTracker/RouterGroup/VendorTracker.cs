//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using TradingLib.API;


//namespace TradingLib.Common
//{
//    public class VendorTracker
//    {
//        ConcurrentDictionary<int, VendorImpl> vendoermap = new ConcurrentDictionary<int, VendorImpl>();

//        public VendorTracker()
//        {
//            foreach (VendorImpl v in ORM.MRouterGroup.SelectVendor())
//            {
//                vendoermap.TryAdd(v.ID, v);
//                v.Domain = BasicTracker.DomainTracker[v.domain_id];
//            }
//        }

//        /// <summary>
//        /// 按全局ID获得Vendoer对象
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public VendorImpl this[int id]
//        {
//            get
//            {
//                if (vendoermap.Keys.Contains(id))
//                    return vendoermap[id];
//                return null;
//            }
//        }

//        /// <summary>
//        /// 获得所有实盘帐户对象
//        /// </summary>
//        public IEnumerable<VendorImpl> Vendors
//        {
//            get
//            {
//                return vendoermap.Values;
//            }
//        }

//        /// <summary>
//        /// 更新Vendor
//        /// </summary>
//        /// <param name="vendor"></param>
//        public void UpdateVendor(VendorSetting vendor)
//        {
//            VendorImpl target = null;
//            //更新
//            if (vendoermap.TryGetValue(vendor.ID, out target))
//            {
//                target.MarginLimit = vendor.MarginLimit;
//                target.LastEquity = vendor.LastEquity;
//                target.FutCompany = vendor.FutCompany;
//                target.Description = vendor.Description;

//                ORM.MRouterGroup.UpdateVendor(target);

//            }
//            else//添加
//            {
//                target = new VendorImpl();
//                target.MarginLimit = vendor.MarginLimit;
//                target.LastEquity = vendor.LastEquity;
//                target.FutCompany = vendor.FutCompany;
//                target.Description = vendor.Description;
//                target.Name = vendor.Name;
//                target.domain_id = vendor.domain_id;
//                target.Domain = BasicTracker.DomainTracker[target.domain_id];

//                ORM.MRouterGroup.InsertVendor(target);
//                vendor.ID = target.ID;//更新ID参数
//                vendoermap.TryAdd(target.ID, target);
            
//            }
//        }
//    }
//}
