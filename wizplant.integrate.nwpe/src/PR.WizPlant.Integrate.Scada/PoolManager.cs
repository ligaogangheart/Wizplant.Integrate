using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    /// <summary>
    /// 池管理器
    /// </summary>
    public static class PoolManager
    {
        static ILog logger = LogManager.GetLogger("PoolManager");
        #region 锁对象
        static object counterPoolLock = new object();
        #endregion
        /// <summary>
        /// 对象池
        /// </summary>
        static Dictionary<string, object> objectPool = new Dictionary<string, object>();
        /// <summary>
        /// 计数器池
        /// </summary>
        static Dictionary<string, long> counterPool = new Dictionary<string, long>();

        #region Counter
        /// <summary>
        /// 创建计数器，计数器不需要先创建即可使用
        /// </summary>
        /// <param name="key">要创建的计数器名称</param>
        /// <remarks>如果有同名计数器存在，则计数器置0</remarks>
        public static void CreateCounter(string key)
        {
            lock (counterPoolLock)
            {
                counterPool[key] = 0;
            }
        }

        /// <summary>
        /// 计数器加1
        /// </summary>
        /// <param name="key">计数器名称</param>
        public static void CounterAddOne(string key)
        {
            lock (counterPoolLock)
            {
                if (counterPool.ContainsKey(key))
                {
                    counterPool[key] += 1;
                }
                else
                {
                    counterPool[key] = 1;
                }
            }
        }

        /// <summary>
        /// 计数器增加
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="count">要增加的数值</param>
        public static void CounterAdd(string key,long count)
        {
            lock (counterPoolLock)
            {
                if (counterPool.ContainsKey(key))
                {
                    counterPool[key] += count;
                }
                else
                {
                    counterPool[key] = count;
                }
            }
        }

        /// <summary>
        /// 设置计数器计数
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="count">要设置的计数值</param>
        public static void CounterSet(string key, int count)
        {
            lock (counterPoolLock)
            {
                counterPool[key] = count;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long GetCounterValue(string key)
        {
            lock (counterPoolLock)
            {
                if (counterPool.ContainsKey(key))
                {
                    return counterPool[key];
                }
                else
                {
                    return -1;
                }
            }
        }



        /// <summary>
        /// 计数器加1
        /// </summary>
        /// <param name="key">计数器名称</param>
        public static void UnlockCounterAddOne(string key)
        {
            lock (counterPoolLock)
            {
                if (counterPool.ContainsKey(key))
                {
                    counterPool[key] += 1;
                }
                else
                {
                    counterPool[key] = 1;
                }
            }
        }

        /// <summary>
        /// 计数器增加
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="count">要增加的数值</param>
        public static void UnlockCounterAdd(string key, long count)
        {
            lock (counterPoolLock)
            {
                if (counterPool.ContainsKey(key))
                {
                    counterPool[key] += count;
                }
                else
                {
                    counterPool[key] = count;
                }
            }
        }

        /// <summary>
        /// 设置计数器计数
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="count">要设置的计数值</param>
        public static void UnlockCounterSet(string key, int count)
        {
            lock (counterPoolLock)
            {
                counterPool[key] = count;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long UnlockGetCounterValue(string key)
        {
            lock (counterPoolLock)
            {
                if (counterPool.ContainsKey(key))
                {
                    return counterPool[key];
                }
                else
                {
                    return -1;
                }
            }
        }


        #endregion

        /// <summary>
        /// 创建对象池，对象池必须先创建才能使用
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public static void CreatePool<T>()
        {
            var key = typeof(T).FullName;
            CreatePool<T>(key);
        }

        /// <summary>
        /// 创建对象池，对象池必须先创建才能使用
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">池名称</param>
        public static void CreatePool<T>(string key)
        {
            if (objectPool.ContainsKey(key))
            {
                return;
            }
            lock (objectPool)
            {
                Queue<T> q = new Queue<T>();                
                objectPool[key] = q;
                lock (counterPoolLock)
                {
                    counterPool[Consts.CounterPoolMaxPrefix + key] = counterPool[Consts.CounterPoolCurPrefix + key] = 0;
                }
            }
            
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        /// <typeparam name="T">要获取的对象类型</typeparam>
        /// <returns>如果池中无对象，返回空</returns>
        public static T Get<T>() where T :class
        {
            var key = typeof(T).FullName;
            return Get<T>(key);
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        /// <typeparam name="T">要获取的对象类型</typeparam>
        /// <param name="key">池名称</param>
        /// <returns>如果池中无对象，返回空</returns>
        public static T Get<T>(string key) where T : class
        {
            T result = null;
            if (!objectPool.ContainsKey(key))
            {
                return result;
            }
            Queue<T> q = objectPool[key] as Queue<T>;
            if (q != null)
            {           
                lock (q)
                {
                    if (q.Count > 0)
                    {
                        result = q.Dequeue();
                        lock (counterPoolLock)
                        {
                            counterPool[Consts.CounterPoolCurPrefix + key] = q.Count;
                        }
                    }
                }                
            }
            return result;
        }
        /// <summary>
        /// 将对象加入池中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">对象</param>
        public static void EnPool<T>(T data)
        {
            var key = typeof(T).FullName;
            EnPool<T>(key, data);
        }

        /// <summary>
        /// 将对象加入池中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">池名称</param>
        /// <param name="data">对象</param>
        public static void EnPool<T>(string key ,T data)
        {
            if (objectPool.ContainsKey(key))
            {
                Queue<T> q = objectPool[key] as Queue<T>;
                if (q != null)
                {
                    int count;
                    lock (q)
                    {
                        q.Enqueue(data);
                        count = q.Count;
                        lock (counterPoolLock)
                        {
                            counterPool[Consts.CounterPoolCurPrefix + key] = count;
                            if (counterPool.ContainsKey(Consts.CounterPoolMaxPrefix + key))
                            {
                                if (count > counterPool[Consts.CounterPoolMaxPrefix + key])
                                {
                                    counterPool[Consts.CounterPoolMaxPrefix + key] = count;
                                }
                            }
                            else
                            {
                                counterPool[Consts.CounterPoolMaxPrefix + key] = count;
                            }
                        }
                    }                    
                }
            }
        }

        /// <summary>
        /// 获取队列
        /// </summary>
        /// <typeparam name="T">队列元素类型</typeparam>
        public static Queue<T> GetQueue<T>()
        {
            var key = typeof(T).FullName;
            return GetQueue<T>(key);
        }

        /// <summary>
        /// 获取队列
        /// </summary>
        /// <typeparam name="T">队列元素类型</typeparam>
        /// <param name="key">队列名称</param>
        /// <returns></returns>
        public static Queue<T> GetQueue<T>(string key)
        {
            if (objectPool.ContainsKey(key))
            {
                Queue<T> q = objectPool[key] as Queue<T>;
                return q;
            }
            return null;
        }

       /// <summary>
       /// 获取计数器
       /// </summary>
       /// <returns></returns>
        public static Dictionary<string, long> GetCounter()
        {
            var clone = new Dictionary<string, long>();
            lock (counterPoolLock)
            {
                foreach (var item in counterPool)
                {
                    clone.Add(item.Key, item.Value);
                }
            }
            return clone;
        }

     

        /// <summary>
        /// 释放所有池中对象
        /// </summary>
        public static void ReleasePool()
        {
            foreach (var key in objectPool.Keys)
            {
                dynamic q = objectPool[key];
                lock(q)
                {
                    q.Clear();
                    lock (counterPoolLock)
                    {
                        counterPool[Consts.CounterPoolMaxPrefix + key] = counterPool[Consts.CounterPoolCurPrefix + key] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 释放池中对象
        /// </summary>
        /// <typeparam name="T">要释放的池类型</typeparam>
        public static void ReleasePool<T>()
        {
            string key = typeof(T).FullName;
            ReleasePool<T>(key);
        }

        /// <summary>
        /// 释放池中对象
        /// </summary>
        /// <typeparam name="T">要释放的池类型</typeparam>
        /// <param name="key">要释放的池名称</param>
        public static void ReleasePool<T>(string key)
        {
            if (objectPool.ContainsKey(key))
            {
                var q = objectPool[key] as Queue<T>;
                if (q != null)
                {
                    lock (q)
                    {
                        q.Clear();
                        lock (counterPoolLock)
                        {
                            counterPool[Consts.CounterPoolMaxPrefix + key] = counterPool[Consts.CounterPoolCurPrefix + key] = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 释放池中对象
        /// </summary>
        /// <typeparam name="T">要释放的池类型</typeparam>
        /// <param name="size">要保留的数目</param>
        public static void ReleasePool<T>(int size)
        {
            string key = typeof(T).FullName;
            ReleasePool<T>(key,size);
        }

        /// <summary>
        /// 释放池中对象
        /// </summary>
        /// <typeparam name="T">要释放的池类型</typeparam>
        /// <param name="key">要释放的池名称</param>
        /// <param name="size">要保留的数目</param>
        public static void ReleasePool<T>(string key, int size)
        {
            if (objectPool.ContainsKey(key))
            {
                var q = objectPool[key] as Queue<T>;
                if (q != null)
                {
                    int remove = 0;
                    lock (q)
                    {
                        remove = q.Count - size;
                        if (remove > 0)
                        {
                            for (int i = 0; i < remove; i++)
                            {
                                q.Dequeue();
                            }
                            lock (counterPoolLock)
                            {
                               counterPool[Consts.CounterPoolMaxPrefix + key]= counterPool[Consts.CounterPoolCurPrefix + key] = 0;
                            }
                        }
                    }
                    if (remove > 0)
                    {
                        logger.DebugFormat("Release Pool[{0}] {1} objects, now size is {2}", Consts.CounterPoolMaxPrefix + key, remove, size);
                    }
                }
            }
        }

        /// <summary>
        /// 重置计数器
        /// </summary>
        public static void ResetCounter()
        {
            lock (counterPoolLock)
            {
                //int count = counterPool.Count;
                //string key = null;
                //for (int i = 0; i < count; i++)
                //{
                //    key = counterPool.ElementAt(i).Key;
                //    counterPool[key] = 0;
                //}
                counterPool.Clear();
            }
        }

        /// <summary>
        /// 获取并重置计数器
        /// </summary>
        public static Dictionary<string, long> GetAndResetCounter()
        {
            var clone = new Dictionary<string, long>();
            lock (counterPoolLock)
            {
                int count = counterPool.Count;
                string key = null;
                for (int i = 0; i < count;i++ )
                {
                    key = counterPool.ElementAt(i).Key;
                    clone.Add(key, counterPool[key]);
                    //counterPool[key] = 0;
                }
                counterPool.Clear();
            }
            return clone;           
        }

        /// <summary>
        /// 重置计数器
        /// </summary>
        public static void ResetCounter(string key)
        {
            
            if (counterPool.ContainsKey(key))
            {
                lock (counterPoolLock)
                {                   
                    counterPool[key] = 0;                   
                }
            }
        }
    }
}
