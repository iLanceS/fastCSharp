using System;
using System.Reflection;
using fastCSharp.reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public sealed class constructor : Attribute
    {
    }
    /// <summary>
    /// 默认构造函数
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public static class constructor<valueType>
    {
#if NOJIT
        /// <summary>
        /// 默认构造函数信息
        /// </summary>
        private static readonly ConstructorInfo constructorInfo;
        /// <summary>
        /// 构造函数调用
        /// </summary>
        /// <returns></returns>
        private static valueType invoke()
        {
            return (valueType)constructorInfo.Invoke(null);
        }
        /// <summary>
        /// 对象浅复制函数信息
        /// </summary>
        private static readonly MethodInfo cloneMethod;
        /// <summary>
        /// 对象浅复制
        /// </summary>
        /// <returns></returns>
        private static valueType clone()
        {
            return (valueType)cloneMethod.Invoke(uninitializedObject, null);
        }
#endif
        /// <summary>
        /// 未初始化对象，用于Clone
        /// </summary>
        private static readonly valueType uninitializedObject;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public static readonly Func<valueType> New;
        /// <summary>
        /// 默认空值
        /// </summary>
        /// <returns>默认空值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static valueType Default()
        {
            return default(valueType);
        }

        static constructor()
        {
            Type type = typeof(valueType);
            if (type.IsValueType || type.IsArray || type == typeof(string))
            {
                New = Default;
                return;
            }
            if (fastCSharp.code.typeAttribute.GetAttribute<constructor>(type, false, true) != null)
            {
                foreach (fastCSharp.code.attributeMethod methodInfo in fastCSharp.code.attributeMethod.GetStatic(type))
                {
                    if (methodInfo.Method.ReflectedType == type && methodInfo.Method.GetParameters().Length == 0 && methodInfo.GetAttribute<constructor>(true) != null)
                    {
                        New = (Func<valueType>)Delegate.CreateDelegate(typeof(Func<valueType>), methodInfo.Method);
                        return;
                    }
                }
            }
            if (!type.IsInterface && !type.IsAbstract)
            {
#if NOJIT
                constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, nullValue<Type>.Array, null);
#else
                ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, nullValue<Type>.Array, null);
#endif
                if (constructorInfo == null)
                {
                    try
                    {
                        uninitializedObject = (valueType)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
                        if (uninitializedObject != null)
                        {
#if NOJIT
                            cloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                            New = clone;
#else
                            DynamicMethod dynamicMethod = new DynamicMethod("uninitializedObjectClone", type, nullValue<Type>.Array, type, true);
                            ILGenerator generator = dynamicMethod.GetILGenerator();
                            generator.Emit(OpCodes.Ldsfld, typeof(constructor<valueType>).GetField("uninitializedObject", BindingFlags.Static | BindingFlags.NonPublic));
                            generator.Emit(OpCodes.Callvirt, typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic));
                            generator.Emit(OpCodes.Ret);
                            New = (Func<valueType>)dynamicMethod.CreateDelegate(typeof(Func<valueType>));
#endif
                        }
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, type.fullName() + " 实例创建失败", false);
                    }
                }
                else
                {
#if NOJIT
                    New = invoke;
#else
                    DynamicMethod dynamicMethod = new DynamicMethod("constructor", type, nullValue<Type>.Array, type, true);
                    dynamicMethod.InitLocals = true;
                    ILGenerator generator = dynamicMethod.GetILGenerator();
                    generator.Emit(OpCodes.Newobj, constructorInfo);
                    generator.Emit(OpCodes.Ret);
                    New = (Func<valueType>)dynamicMethod.CreateDelegate(typeof(Func<valueType>));
#endif
                }
            }
        }
    }
}
