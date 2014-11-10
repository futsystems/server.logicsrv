using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace TradingLib.BrokerXAPI.Interop
{
    internal static partial class Platform
    {
        [System.Flags]
        public enum LoadLibraryFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }


        public const string LibSuffix = ".dll";

        private const string KernelLib = "kernel32";

        /// <summary>
        /// 加载一个外部dll
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SafeLibraryHandle OpenHandle(string filename)
        {
            return LoadLibraryEx(filename, IntPtr.Zero, LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH);
            //return LoadLibrary(filename);
        }

        /// <summary>
        /// 加载一个函数
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public static IntPtr LoadProcedure(SafeLibraryHandle handle, string functionName)
        {
            return GetProcAddress(handle, functionName);
        }

        /// <summary>
        /// 释放到一个外部dll
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool ReleaseHandle(IntPtr handle)
        {
            return FreeLibrary(handle);
        }

        /// <summary>
        /// 获得上一次操作异常
        /// </summary>
        /// <returns></returns>
        public static Exception GetLastLibraryError()
        {
            return Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
        }


        [DllImport(KernelLib, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibrary(string fileName);

        [DllImport(KernelLib, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(KernelLib, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr moduleHandle);

        [DllImport(KernelLib)]
        private static extern IntPtr GetProcAddress(SafeLibraryHandle moduleHandle, string procname);


    }
}
