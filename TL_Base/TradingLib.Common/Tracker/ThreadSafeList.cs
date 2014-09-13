using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    //通过加锁可以多线程进行添加数据,读取数据
    //单纯记录数据data safe,我们无法删除数据,删除数据的同时有线程在读取数据,任然会产生线程不安全
    [Serializable]
    public  class ThreadSafeList<T> : IEnumerable<T>
    {
        private List<T> m_list = new List<T>();
        private object m_lock = new object();

        public object SyncRoot
        {
            get
            {
                return m_lock;
            }
        }
        public void Clear()
        {
            lock (m_lock)
            {
                m_list.Clear();
            }
        }
        public T[] ToArray()
        {
            lock (m_lock)
            {
                return m_list.ToArray();
            }
        }
        public void Add(T value)
        {
            lock (m_lock)
            {
                m_list.Add(value);
            }
        }
        public void Remove(T value)
        {
            lock (m_lock)
            {
                m_list.Remove(value);
            }
        }

        public void RemoveAt(int i)
        {
            lock (m_lock)
            {
                m_list.RemoveAt(i);
            }
            
        }
        public bool Contains(T value)
        {
            lock (m_lock)
            {
                return m_list.Contains(value);
            }
        }
        public int Count { get { lock (m_lock) { return m_list.Count; } } }
        public T this[int index]
        {
            get { lock (m_lock) { return m_list[index]; } }
            set { lock (m_lock) { m_list[index] = value; } }
        }
        // IEnumerable<T> left outEnumerab IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator(); // this will return the one
            // available through the object reference
            // ie. "IEnumerator<int> GetEnumerator"
        }
        public IEnumerator<T> GetEnumerator()
        {
            lock (m_lock)
            {
                for (int i = 0; i < m_list.Count; i++)
                    yield return m_list[i];
            }
        }
    }
}
