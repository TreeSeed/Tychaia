// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tychaia.Globals
{
    public class DefaultArrayPool : IArrayPool
    {
        private Dictionary<Type, List<object>> m_Pools;
    
        public DefaultArrayPool()
        {
            this.m_Pools = new Dictionary<Type, List<object>>();
        }
        
        public T[] Get<T>(int size)
        {
            return this.Get(typeof(T[]), size);
        }
        
        public dynamic Get(Type type, int size)
        {
            var pool = this.m_Pools.Cast<KeyValuePair<Type, List<object>>?>().FirstOrDefault(x => x.Value.Key == type);
            if (pool == null)
            {
                this.m_Pools.Add(type, new List<object>());
                Console.WriteLine("Allocated " + type.FullName + " with size " + size);
                return Activator.CreateInstance(type, size);
            }
            
            var array = pool.Value.Value.Cast<dynamic>().FirstOrDefault(x => x.Length == size);
            if (array == null)
            {
                Console.WriteLine("Allocated " + type.FullName + " with size " + size);
                return Activator.CreateInstance(type, size);
            }
            
            Console.WriteLine("Reused " + type.FullName + " with size " + size);
            return array;
        }
        
        public void Release<T>(T[] array)
        {
            this.Release(typeof(T), array);
        }
        
        public void Release(dynamic array)
        {
            var type = array.GetType().GetElementType();
            this.Release(type, array);
        }
        
        private void Release(Type type, object array)
        {
            Console.WriteLine("Released " + type.FullName);
            var pool = this.m_Pools.Cast<KeyValuePair<Type, List<object>>?>().FirstOrDefault(x => x.Value.Key == type);
            if (pool == null)
                this.m_Pools.Add(type, new List<object>());
            this.m_Pools[type].Add(array);
        }
    }
}
