using System;
using fastCSharp.code;
using System.Runtime.CompilerServices;
using System.Reflection;
using fastCSharp.reflection;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// WEB视图配置
    /// </summary>
    public static class webView
    {
        /// <summary>
        /// WEB视图成员清理配置
        /// </summary>
        public sealed class clearMember : ignoreMember
        {
        }
#if NOJIT
#else
        /// <summary>
        /// WEB视图成员清理动态函数
        /// </summary>
        private struct clearMemberDynamicMethod
        {
            /// <summary>
            /// 动态函数
            /// </summary>
            private DynamicMethod dynamicMethod;
            /// <summary>
            /// 
            /// </summary>
            private ILGenerator generator;
            /// <summary>
            /// 数据类型
            /// </summary>
            private Type type;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public clearMemberDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("webView.clearMember", null, new Type[] { type }, this.type = type, true);
                generator = dynamicMethod.GetILGenerator();
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(fieldIndex field)
            {
                generator.Emit(OpCodes.Ldarg_0);
                if (field.Member.FieldType.IsValueType)
                {
                    generator.Emit(OpCodes.Ldflda, field.Member);
                    generator.Emit(OpCodes.Initobj, field.Member.FieldType);
                }
                else
                {
                    generator.Emit(OpCodes.Ldnull);
                    generator.Emit(OpCodes.Stfld, field.Member);
                }
            }
            /// <summary>
            /// 基类调用
            /// </summary>
            /// <param name="type"></param>
            public void Base(Type type)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, clearMethod.MakeGenericMethod(type));
            }
            /// <summary>
            /// 创建成员转换委托
            /// </summary>
            /// <returns>成员转换委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
        }
        /// <summary>
        /// WEB视图成员清理函数信息
        /// </summary>
        private static readonly MethodInfo clearMethod = typeof(webView).GetMethod("clear", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// WEB视图成员清理
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void clear<valueType>(valueType value)
        {
            clearMember<valueType>.Cleaner(value);
        }
        /// <summary>
        /// WEB视图成员清理
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        internal static class clearMember<valueType>
        {
            /// <summary>
            /// 成员清理
            /// </summary>
            public static readonly Action<valueType> Cleaner;
            /// <summary>
            /// 忽略成员清理
            /// </summary>
            /// <param name="value"></param>
            private static void ignore(valueType value) { }
            
            static clearMember()
            {
                Type type = typeof(valueType);
                if (type.IsClass)
                {
                    subArray<fieldIndex> fields = memberIndexGroup<valueType>.GetFields(memberFilters.InstanceField)
                        .getFind(value => value.Member.DeclaringType == type && value.GetSetupAttribute<clearMember>(true, true) != null);
                    Type baseType = type.BaseType;
                    if (baseType == typeof(object)) baseType = null;
                    else
                    {
                        clearMember attribute = baseType.customAttribute<clearMember>();
                        if (attribute != null && !attribute.IsSetup) baseType = null;
                    }
                    if (fields.Count != 0 || baseType != null)
                    {
                        clearMemberDynamicMethod dynamicMethod = new clearMemberDynamicMethod(type);
                        foreach (fieldIndex field in fields) dynamicMethod.Push(field);
                        if (baseType != null) dynamicMethod.Base(baseType);
                        Cleaner = (Action<valueType>)dynamicMethod.Create<Action<valueType>>();
                        return;
                    }
                }
                Cleaner = ignore;
            }
        }
#endif
    }
}
