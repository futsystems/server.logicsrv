using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.FinService
{

    public class ServicePlanTrcker
    {
        Dictionary<int, DBServicePlan> serviceplanemap = new Dictionary<int, DBServicePlan>();

        /// <summary>
        /// 通过id获得对应的DBServicePlane
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DBServicePlan this[int id]
        {
            get
            {
                if (!serviceplanemap.Keys.Contains(id))
                {
                    return serviceplanemap[id];
                }
                return null;
            }
        }

    }


    /// <summary>
    /// 服务计划
    /// 从数据库加载服务计划
    /// 并且找到对应的type
    /// 从而可以实现动态生成
    /// </summary>
    public class DBServicePlan
    {
        /// <summary>
        /// 服务外键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
    }


}
