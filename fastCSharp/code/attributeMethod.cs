using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.threading;
using System.Reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 自定义属性函数信息
    /// </summary>
    public struct attributeMethod
    {
        /// <summary>
        /// 函数信息
        /// </summary>
        public MethodInfo Method;
        /// <summary>
        /// 自定义属性集合
        /// </summary>
        private object[] attributes;
        /// <summary>
        /// 获取自定义属性集合
        /// </summary>
        /// <typeparam name="attributeType"></typeparam>
        /// <returns></returns>
        internal IEnumerable<attributeType> Attributes<attributeType>() where attributeType : Attribute
        {
            foreach (object value in attributes)
            {
                if (typeof(attributeType).IsAssignableFrom(value.GetType())) yield return (attributeType)value;
            }
        }
        /// <summary>
        /// 根据成员属性获取自定义属性
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>自定义属性</returns>
        public attributeType GetAttribute<attributeType>(bool isInheritAttribute) where attributeType : Attribute
        {
            attributeType value = null;
            int minDepth = int.MaxValue;
            foreach (attributeType attribute in Attributes<attributeType>())
            {
                if (isInheritAttribute)
                {
                    int depth = 0;
                    for (Type type = attribute.GetType(); type != typeof(attributeType); type = type.BaseType) ++depth;
                    if (depth < minDepth)
                    {
                        if (depth == 0) return attribute;
                        minDepth = depth;
                        value = attribute;
                    }
                }
                else if (attribute.GetType() == typeof(attributeType)) return attribute;
            }
            return value;
        }
        ///// <summary>
        ///// 自定义属性函数信息集合
        ///// </summary>
        //private static interlocked.dictionary<Type, attributeMethod[]> methods = new interlocked.dictionary<Type,attributeMethod[]>(dictionary.CreateOnly<Type, attributeMethod[]>());
        ///// <summary>
        ///// 自定义属性函数信息集合访问锁
        ///// </summary>
        //private static readonly object createLock = new object();
        ///// <summary>
        ///// 根据类型获取自定义属性函数信息集合
        ///// </summary>
        ///// <param name="type">对象类型</param>
        ///// <returns>自定义属性函数信息集合</returns>
        //public static attributeMethod[] Get(Type type)
        //{
        //    attributeMethod[] values;
        //    if (methods.TryGetValue(type, out values)) return values;
        //    Monitor.Enter(createLock);
        //    try
        //    {
        //        if (methods.TryGetValue(type, out values)) return values;
        //        subArray<attributeMethod> array = default(subArray<attributeMethod>);
        //        foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        //        {
        //            object[] attributes = method.GetCustomAttributes(true);
        //            if (attributes.Length != 0) array.Add(new attributeMethod { Method = method, attributes = attributes });
        //        }
        //        methods.Set(type, values = array.ToArray());
        //    }
        //    finally { Monitor.Exit(createLock); }
        //    return values;
        //}
        /// <summary>
        /// 自定义属性函数信息集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, attributeMethod[]> staticMethods = new interlocked.dictionary<Type,attributeMethod[]>();
        /// <summary>
        /// 自定义属性函数信息集合访问锁
        /// </summary>
        private static readonly object createStaticLock = new object();
        /// <summary>
        /// 根据类型获取自定义属性函数信息集合
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>自定义属性函数信息集合</returns>
        public static attributeMethod[] GetStatic(Type type)
        {
            attributeMethod[] values;
            if (staticMethods.TryGetValue(type, out values)) return values;
            Monitor.Enter(createStaticLock);
            try
            {
                if (staticMethods.TryGetValue(type, out values)) return values;
                subArray<attributeMethod> array = default(subArray<attributeMethod>);
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    object[] attributes = method.GetCustomAttributes(true);
                    if (attributes.Length != 0) array.Add(new attributeMethod { Method = method, attributes = attributes });
                }
                staticMethods.Set(type, values = array.ToArray());
            }
            finally { Monitor.Exit(createStaticLock); }
            return values;
        }
    }
}
