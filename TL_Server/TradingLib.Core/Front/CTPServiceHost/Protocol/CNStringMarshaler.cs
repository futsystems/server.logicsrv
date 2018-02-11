using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Runtime.InteropServices;
using Common.Logging;

namespace CTPService
{
    public class CNStringMarshaler:ICustomMarshaler
    {
        private static CNStringMarshaler Instance = new CNStringMarshaler();
        static ILog logger = LogManager.GetLogger("CNString");
        public static ICustomMarshaler GetInstance(string s)
        {
            return Instance;
        }

        public void CleanUpManagedData(object o)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            logger.Debug(string.Format("# FileNameMarshaler.CleanUpManagedData ({0:x})", pNativeData));
            //UnixMarshal.FreeHeap(pNativeData);
            Marshal.FreeHGlobal(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return 501;
        }

        public IntPtr MarshalManagedToNative(object obj)
        {
            string s = obj as string;
            if (s == null)
                return IntPtr.Zero;
            IntPtr p = Marshal.StringToHGlobalAnsi(s);//UnixMarshal.StringToHeap(s, UnixEncoding.Instance);
            logger.Debug(string.Format("# FileNameMarshaler.MarshalNativeToManaged for `{0}'={1:x}", s, p));
            return p;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            string s = Marshal.PtrToStringAnsi(pNativeData);//UnixMarshal.PtrToString(pNativeData, UnixEncoding.Instance);
            logger.Debug(string.Format("# FileNameMarshaler.MarshalNativeToManaged ({0:x})=`{1}'", pNativeData, s));
            return s;
        }

    }
}
