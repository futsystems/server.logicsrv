using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Reflection;
using System.IO;


namespace TradingLib.Core
{
    public class ConnecterHelper
    {
        //通过提供接口类型我们可以通过接口来查找我们实现的相应策略

        public static List<Type> GetBroker()
        {
            return GetConnecterListViaType<IBroker>();
        }
        public static List<Type> GetDataFeed()
        {
            return GetConnecterListViaType<IDataFeed>();
        }
        public static List<Type> GetConnecterListViaType<T>()
        {
            List<Type> tmp = new List<Type>();
            foreach (Type t in GetConnecterListFromPath())
            {
                if (typeof(T).IsAssignableFrom(t))
                    tmp.Add(t);
            }

            return tmp;

        }

        //遍历文件夹下面所有的dll文件来加载所有的Response文件
        public static List<Type> GetConnecterListFromPath() { return GetConnecterListFromPath(@"Connecter"); }
        public static List<Type> GetConnecterListFromPath(string filepath)
        {
            List<string> l = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(filepath);
            List<Type> tmp = new List<Type>();
            foreach (FileInfo dchild in dir.GetFiles())
            {
                l.Add(dchild.FullName);
                tmp.AddRange(GetConnecterList(dchild.FullName));
            }
            return tmp;

        }

        public static List<Type> GetConnecterList(string dllfilepath)
        {
            if (!File.Exists(dllfilepath)) return null;
            Assembly a;
            try
            {
                a = Assembly.LoadFile(dllfilepath);
                
            }
            catch (Exception ex) { return null; }
            return GetConnecterList(a);
        }
        /// <summary>
        /// Gets all Response names found in a given assembly.  Names are FullNames, which means namespace.FullName.  eg 'BoxExamples.BigTradeUI'
        /// </summary>
        /// <param name="boxdll">The assembly.</param>
        /// <returns>list of response names</returns>
        public static List<Type> GetConnecterList(Assembly responseassembly) { return GetConnecterList(responseassembly, null); }
        public static List<Type> GetConnecterList(Assembly responseassembly, DebugDelegate deb)
        {
            List<Type> clist = new List<Type>();
            Type[] t;
            try
            {
                t = responseassembly.GetTypes();
                for (int i = 0; i < t.GetLength(0); i++)
                    clist.Add(t[i]);
                    //if (IsConnecter(t[i])) 
            }
            catch (Exception ex)
            {
                if (deb != null)
                {
                    deb(ex.Message + ex.StackTrace);
                }
            }

            return clist;
        }

        //检查是否是Response
        static bool IsConnecter(Type t)
        {
            return typeof(Response).IsAssignableFrom(t);
        }
    }
}
