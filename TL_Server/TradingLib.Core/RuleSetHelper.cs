using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.Common;
using TradingLib.API;
using System.Reflection;
using System.IO;

namespace TradingLib.Core
{
    public class RuleSetHelper
    {

        //通过提供接口类型我们可以通过接口来查找我们实现的相应策略
        public static List<Type> GetRuleSetListViaType<T>()
        {
            List<Type> tmp = new List<Type>();
            foreach (Type t in GetRuleSetListFromPatha())
            {
                if (typeof(T).IsAssignableFrom(t))
                    tmp.Add(t);
            }

            return tmp;

        }
        //遍历文件夹下面所有的dll文件来加载所有的Response文件
        public static List<Type> GetRuleSetListFromPatha() { return GetRuleSetListFromPath(@"RuleSet"); }
        public static List<Type> GetRuleSetListFromPath(string filepath)
        {
            List<string> l = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(filepath);
            List<Type> tmp = new List<Type>();
            foreach(FileInfo dchild in dir.GetFiles())
            {
                l.Add(dchild.FullName);
                tmp.AddRange(GetRuleSetList(dchild.FullName));
            }
            return tmp;

        }

        public static List<Type> GetRuleSetList(string dllfilepath)
        {
            List<Type> reslist = new List<Type>();
            if (!File.Exists(dllfilepath)) return reslist;
            Assembly a;
            try
            {
                a = Assembly.LoadFile(dllfilepath);
            }
            catch (Exception ex) { return reslist; }
            return GetRuleSetList(a);
        }
        /// <summary>
        /// Gets all Response names found in a given assembly.  Names are FullNames, which means namespace.FullName.  eg 'BoxExamples.BigTradeUI'
        /// </summary>
        /// <param name="boxdll">The assembly.</param>
        /// <returns>list of response names</returns>
        public static List<Type> GetRuleSetList(Assembly responseassembly) { return GetRuleSetList(responseassembly, null); }
        public static List<Type> GetRuleSetList(Assembly responseassembly, DebugDelegate deb)
        {
            List<Type> reslist = new List<Type>();
            Type[] t;
            try
            {
                t = responseassembly.GetTypes();
                for (int i = 0; i < t.GetLength(0); i++)
                    reslist.Add(t[i]);
            }
            catch (Exception ex)
            {
                if (deb != null)
                {
                    deb(ex.Message + ex.StackTrace);
                }
            }

            return reslist;
        }
    }
}
