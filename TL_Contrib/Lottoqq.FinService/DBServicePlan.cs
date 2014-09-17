using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;

namespace TradingLib.Contrib.FinService
{

    public class ServicePlanTracker
    {
        Dictionary<int, DBServicePlan> spidxmap = new Dictionary<int, DBServicePlan>();
        Dictionary<string, DBServicePlan> spclassmap = new Dictionary<string, DBServicePlan>();

        //具体类名和类型的影射通过初始化时候动态加载
        Dictionary<string, Type> sptypemap = new Dictionary<string, Type>();
        public ServicePlanTracker()
        {
            //从数据库加载服务计划并放入内存Map
            foreach (DBServicePlan sp in ORM.MService.SelectServicePlan())
            {
                spidxmap.Add(sp.ID, sp);
                spclassmap.Add(sp.ClassName, sp);
            }
        }

        public void InitServicePlan(Type type)
        {
            string fullname = type.FullName;
            //如果数据库包含该计划 则将类型加入
            if (spclassmap.Keys.Contains(fullname))
            {
                sptypemap[fullname] = type;
            }
            //如果数据库中不存在 则需要同步数据库信息
            else
            { 
                DBServicePlan sp = new DBServicePlan();
                sp.ClassName = type.FullName;
                sp.Name = type.Name;
                sp.Title="测试";
                ORM.MService.InsertServicePlan(sp);
                spidxmap.Add(sp.ID, sp);
                spclassmap.Add(sp.ClassName, sp);
                sptypemap[fullname] = type;


                
            }

            //同步服务计划的基准参数 如果不存在则从程序集载入同步，如果存在则不作修改，如果修改基准参数 需要从其他入口进行修改
            int serviceplan_fk = spclassmap[fullname].ID;
            //查找用argumentattribute标注过的属性 这些属性是服务计划的参数
            List<PropertyInfo> propertyInfos = PluginHelper.FindProperty<ArgumentAttribute>(type);
            //遍历每个属性 获得对应的特性对象
            foreach (PropertyInfo pi in propertyInfos)
            {
                ArgumentAttribute attr = (ArgumentAttribute)Attribute.GetCustomAttribute(pi, typeof(ArgumentAttribute));
                FinTracker.ArgumentTracker.UpdateArgumentBase(serviceplan_fk, attr);
            }
        }

        /// <summary>
        /// 通过类名获得BDServicePlane数据
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public DBServicePlan this[string classname]
        {
            get
            {
                if (spclassmap.Keys.Contains(classname))
                {
                    return spclassmap[classname];
                }
                return null;
            }
        }
        /// <summary>
        /// 通过数据库serviceplan_fkid获得对应的DBServicePlane
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DBServicePlan this[int id]
        {
            get
            {
                if (!spidxmap.Keys.Contains(id))
                {
                    return spidxmap[id];
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
