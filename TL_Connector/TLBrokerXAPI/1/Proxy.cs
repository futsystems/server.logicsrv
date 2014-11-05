using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.BrokerXAPI
{
    public delegate int FuncDel();

    public class Proxy
    {
        private InvokeBase _Invoke;
        private PlatformID _PlatformID;

        public Proxy(string path)
        {
            _PlatformID = Environment.OSVersion.Platform;
            Console.WriteLine("Platform:" + _PlatformID);
            if (_PlatformID == PlatformID.Unix)
            {
                _Invoke = new SoInvoke(path);
            }
            else
            {
                _Invoke = new DllInvoke(path);
            }
        }

        ~Proxy()
        {
            Dispose(false);
        }

        private bool disposed;
        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            //base.Dispose();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                if (_Invoke != null)
                    _Invoke.Dispose();
                _Invoke = null;
                //_XRequest = null;
                disposed = true;
            }
            //base.Dispose(disposing);
        }

        public int func()
        {
            FuncDel democall = (FuncDel)_Invoke.Invoke("democall", typeof(FuncDel));

            return democall();
        }
    }
}
