using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.DataFarm.Common
{
    internal sealed class CloseWrapper<T> : IDisposable where T : class, IDisposable
    {
        private T _obj = default(T);

        private bool _closeBase = true;

        public T Obj
        {
            get
            {
                return this._obj;
            }
        }

        public bool CloseBase
        {
            get
            {
                return this._closeBase;
            }
        }

        public CloseWrapper(T writer, bool closeBase)
        {
            this._obj = writer;
            this._closeBase = closeBase;
        }

        private void Dispose(bool disposing)
        {
            if (disposing && this._closeBase)
            {
                this._obj.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        ~CloseWrapper()
        {
            this.Dispose(false);
        }
    }
}
