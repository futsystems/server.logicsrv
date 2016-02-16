using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Timers;
using TradingLib.Mixins;
using Common.Logging;


namespace TradingLib.Mixins.DataBase
{
    /// <summary>
    /// 对象池技术,用于为单一对象提供并发支持,比如建立高效数据库连接池,可以实现某个操作的并发操作
    /// </summary>
    public abstract class ObjectPool
    {
        protected ILog logger;
        //Last Checkout time of any object from the pool.
        private long lastCheckOut;

        //Hashtable of the check-out objects.被使用的对象(锁定)
        private static Hashtable locked;

        //Hashtable of available objects 未被使用的对象(自由)
        private static Hashtable unlocked;

        //Clean-Up interval
        internal static long GARBAGE_INTERVAL = 5 * 1000; //每5秒检查数据库连接有效性

        //
        internal static long OBJECT_EXPIRE = 30 * 1000;//30秒数据库连接未使用 则进行清除

        protected string _name = "ObjectPoll";
        public ObjectPool(string name)
        {
            _name = name;
            logger = LogManager.GetLogger(_name);
            //并发线程安全
            locked = Hashtable.Synchronized(new Hashtable());
            unlocked = Hashtable.Synchronized(new Hashtable());
            lastCheckOut = DateTime.Now.Ticks;

            //Create a Time to track the expired objects for cleanup.
            Timer aTimer = new Timer();
            aTimer.Enabled = true;
            aTimer.Interval = GARBAGE_INTERVAL;//定时器 用于定时检查数据库连接可用性
            aTimer.Elapsed += new ElapsedEventHandler(CollectGarbage);//定期清理垃圾
            aTimer.Start();
        }

        protected abstract object Create();//创建

        protected abstract bool Validate(object o);//检验是否可用

        protected abstract void Expire(object o);//过期

        public object GetObjectFromPool()//从对象池 取得一个对象 如果没有可用对象 则新建一个对象
        {
            long now = DateTime.Now.Ticks;
            lastCheckOut = now;
            object o = null;

            lock (this)
            {
                try
                {
                    foreach (DictionaryEntry myEntry in unlocked)
                    {
                        o = myEntry.Key;
                        unlocked.Remove(o);
                        if (Validate(o))
                        {
                            locked.Add(o, now);
                            return o;
                        }
                        else
                        {
                            Expire(o);
                            o = null;
                        }
                    }
                }
                catch (Exception ex) 
                {
                    logger.Error("Get object form poll error:" + ex.ToString());
                }
                o = Create();
                locked.Add(o, now);
            }
            return o;
        }

        public void ReturnObjectToPool(object o)//将对象归还到对象池
        {
            if (o != null)
            {
                lock (this)
                {
                    locked.Remove(o);
                    unlocked.Add(o, DateTime.Now.Ticks);
                }
            }
        }
        /*1.测试 短时间生成10个连接，超时后,每次释放1个,最终会全部释放
         *
         * 
         * 
         * */
        private void CollectGarbage(object sender, ElapsedEventArgs ea)//回收垃圾对象
        {
            lock (this)
            {
                //logger.Info("----------- collect garbage ------------------");
                object o;
                long now = DateTime.Now.Ticks;
                IDictionaryEnumerator e = unlocked.GetEnumerator();
                try
                {
                    //每次回收一个垃圾内存
                    while (e.MoveNext())
                    {
                        o = e.Key;
                        if ((now - (long)unlocked[o]) > OBJECT_EXPIRE)
                        {
                            unlocked.Remove(o);
                            Expire(o);
                            o = null;
                        }

                        //如果对象不可用则回收该对象
                        if (!Validate(o))
                        {
                            unlocked.Remove(o);
                            Expire(o);
                            o = null;
                        }
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
