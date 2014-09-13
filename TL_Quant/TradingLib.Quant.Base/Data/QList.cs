using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    public delegate void BarQListDelegate(QList<Bar> barlist);
    [Serializable]
    public class QList<T> : List<T>
    {
        public QList()
        {

        }
        public QList(IList<T> list)
        {
            foreach (T t in list)
            {
                this.Add(t);
            }
        }

        public void SetValue(int n,T value)
        {
            base[(base.Count - 1) - n] = value;
        }
        //往后回溯n值
        public T LookBack(int n)
        {
            if (n < 0)
                return Last;
            if (n > (base.Count - 1))
                return First;
            return base[(base.Count - 1) - n];
        }

        public T First
        {
            get
            {
                if (base.Count > 0)
                {
                    return base[0];
                }
                return default(T);
            }
        }

        public T[] Data
        {
            get
            {
                return base.ToArray();
            }
        }

        public T Last
        {
            get
            {
                if (base.Count > 0)
                {
                    return base[base.Count - 1];
                }
                return default(T);
                
            }
            set
            {
                base[base.Count - 1] = value;
            }

        }

        public int LastIndex
        {
            get
            {
                return (base.Count - 1);
            }
        }

       
    }
}
