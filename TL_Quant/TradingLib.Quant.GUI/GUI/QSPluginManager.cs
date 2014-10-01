using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

using System.Collections;
using System.IO;

namespace TradingLib.Quant.Chart
{
    public class QSPluginManager
    {


        // Fields
        private static Hashtable htAssembly = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
        private static Hashtable htFormulaSpace = new Hashtable();
        private static Hashtable htShadow = new Hashtable();
        private static FileSystemEventHandler OnPluginChanged;
        private static string PluginsDir;


        public static void RegExecutingAssembly()
        {
            RegAssembly(Assembly.GetCallingAssembly());
        }

 



        public static void LoadAssembly(string FileName)
        {
            if (FileName.StartsWith("http"))
            {
                Assembly a = Assembly.LoadFrom(FileName);
                FormulaBase.RegAssembly(a.GetHashCode().ToString(), a);
            }
            else
            {
                byte[] byteFromFile = GetByteFromFile(FileName);
                Assembly assembly2 = Assembly.Load(byteFromFile);
                htAssembly[FileName]=GetAssemblyHash(byteFromFile);
                FormulaBase.RegAssembly(htAssembly[FileName].ToString(), assembly2);
            }
        }


        private static string GetAssemblyHash(byte[] bs)
        {
            int num = 0;
            for (int i = 0; i < bs.Length; i++)
            {
                num += bs[i];
            }
            return num.ToString();
        }

        private static byte[] GetByteFromFile(string FileName)
        {
            if (File.Exists(FileName))
            {
                using (FileStream stream = File.OpenRead(FileName))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        return reader.ReadBytes((int)stream.Length);
                    }
                }
            }
            return null;
        }


        public static void RegAssembly(Assembly a)
        {
            FormulaBase.RegAssembly(a.GetHashCode().ToString(), a);

        }


        public static void RegAssemblyFromMemory()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                //if (assembly.FullName.IndexOf("_fml") >= 0)
                {
                    RegAssembly(assembly);
                }
            }
        }



    }

}
