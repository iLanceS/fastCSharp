using System;
using System.Collections.Specialized;
using System.Reflection;
using fastCSharp.reflection;
using System.Collections.Generic;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
#if NOJIT
#else
    /// <summary>
    /// web表单生成
    /// </summary>
    internal static class formGetter
    {
        /// <summary>
        /// 动态函数
        /// </summary>
        public struct memberDynamicMethod
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
            /// 是否值类型
            /// </summary>
            private bool isValueType;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public memberDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("formGetter", null, new Type[] { type, typeof(NameValueCollection) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                isValueType = type.IsValueType;
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(FieldInfo field)
            {
                Type type = field.FieldType;
                if (type.IsValueType)
                {
                    Type nullType = type.nullableType();
                    if (nullType == null) push(field);
                    else
                    {
                        Label end = generator.DefineLabel();
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldflda, field);
                        generator.Emit(OpCodes.Call, pubExtension.GetNullableHasValue(type));
                        generator.Emit(OpCodes.Brfalse_S, end);
                        push(field, nullType);
                        generator.MarkLabel(end);
                    }
                }
                else pushNull(field);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            private void pushNull(FieldInfo field)
            {
                Label end = generator.DefineLabel();
                if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                else generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Brfalse_S, end);
                push(field);
                generator.MarkLabel(end);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="nullType">可空类型</param>
            private void push(FieldInfo field, Type nullType = null)
            {
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldstr, field.Name);
                if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                else generator.Emit(OpCodes.Ldarg_0);
                Type type = nullType ?? field.FieldType;
                if (type == typeof(string)) generator.Emit(OpCodes.Ldfld, field);
                else
                {
                    MethodInfo method = pubExtension.GetNumberToStringMethod(type);
                    if (method == null)
                    {
                        generator.Emit(OpCodes.Ldflda, field);
                        if (type.IsEnum)
                        {
                            generator.Emit(OpCodes.Box, type);
                            generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(typeof(object)));
                        }
                        else generator.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, pubExtension.GetToStringMethod(type));
                    }
                    else
                    {
                        if (nullType == null) generator.Emit(OpCodes.Ldfld, field);
                        else
                        {
                            generator.Emit(OpCodes.Ldflda, field);
                            generator.Emit(OpCodes.Call, pubExtension.GetNullableValue(field.FieldType));
                        }
                        generator.Emit(OpCodes.Call, method);
                    }
                }
                generator.Emit(OpCodes.Callvirt, pubExtension.NameValueCollectionAddMethod);
            }
            
            /// <summary>
            /// 创建web表单委托
            /// </summary>
            /// <returns>web表单委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
        }
    }
#endif
    /// <summary>
    /// WEB表单生成
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public static class formGetter<valueType>
    {
        /// <summary>
        /// web表单生成委托
        /// </summary>
        private static readonly Action<valueType, NameValueCollection> getter;
        /// <summary>
        /// 成员数量
        /// </summary>
        private static readonly int memberCount;
        /// <summary>
        /// 获取POST表单
        /// </summary>
        /// <param name="value">查询对象</param>
        /// <returns>POST表单</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static NameValueCollection Get(valueType value)
        {
            if (getter != null)
            {
                NameValueCollection form = new NameValueCollection(memberCount);
                getter(value, form);
                return form;
            }
            return null;
        }
        /// <summary>
        /// 获取POST表单
        /// </summary>
        /// <param name="value">查询对象</param>
        /// <param name="form">POST表单</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void Get(valueType value, NameValueCollection form)
        {
            if (getter != null)
            {
                if (form == null) log.Default.Throw(log.exceptionType.Null);
                getter(value, form);
            }
        }

        static formGetter()
        {
            Type type = typeof(valueType);
            if (type.IsArray || type.IsEnum || type.IsPointer || type.IsInterface) return;
            foreach (fastCSharp.code.attributeMethod methodInfo in fastCSharp.code.attributeMethod.GetStatic(type))
            {
                if (methodInfo.Method.ReturnType == typeof(void))
                {
                    ParameterInfo[] parameters = methodInfo.Method.GetParameters();
                    if (parameters.Length == 2 && parameters[0].ParameterType == type && parameters[1].ParameterType == typeof(NameValueCollection))
                    {
                        if (methodInfo.GetAttribute<form.custom>(true) != null)
                        {
                            getter = (Action<valueType, NameValueCollection>)Delegate.CreateDelegate(typeof(Action<valueType, NameValueCollection>), methodInfo.Method);
                            return;
                        }
                    }
                }
            }
            form attribute = fastCSharp.code.typeAttribute.GetAttribute<form>(type, true, true) ?? form.AllMember;
            subArray<FieldInfo> fields = pubExtension.GetFields<valueType, form.member>(attribute.MemberFilter, attribute.IsAllMember);
            if ((memberCount = fields.Count) != 0)
            {
#if NOJIT
                getter = new memberGetter(ref fields).Get;
#else
                formGetter.memberDynamicMethod dynamicMethod = new formGetter.memberDynamicMethod(type);
                foreach (FieldInfo member in fields) dynamicMethod.Push(member);
                getter = (Action<valueType, NameValueCollection>)dynamicMethod.Create<Action<valueType, NameValueCollection>>();
#endif
            }
        }
#if NOJIT
        /// <summary>
        /// WEB表单生成
        /// </summary>
        private sealed class memberGetter
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            private struct field
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                public FieldInfo Field;
                /// <summary>
                /// 可空类型判断是否存在值
                /// </summary>
                public MethodInfo NullableHasValueMethod;
                /// <summary>
                /// 获取可空类型数据
                /// </summary>
                public MethodInfo NullableValueMethod;
                /// <summary>
                /// 是否值类型
                /// </summary>
                public bool IsValueType;
                /// <summary>
                /// 设置字段信息
                /// </summary>
                /// <param name="field"></param>
                public void Set(FieldInfo field)
                {
                    Field = field;
                    Type type = field.FieldType;
                    if (type.IsValueType)
                    {
                        IsValueType = true;
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            NullableHasValueMethod = pubExtension.GetNullableHasValue(type);
                            NullableValueMethod = pubExtension.GetNullableValue(type);
                        }
                    }
                }
            }
            /// <summary>
            /// 字段集合
            /// </summary>
            private field[] fields;
            /// <summary>
            /// WEB表单生成
            /// </summary>
            /// <param name="fields"></param>
            public memberGetter(ref subArray<FieldInfo> fields)
            {
                this.fields = new field[fields.Count];
                int index = 0;
                foreach (FieldInfo field in fields) this.fields[index++].Set(field);
            }
            /// <summary>
            /// WEB表单生成
            /// </summary>
            /// <param name="value"></param>
            /// <param name="form"></param>
            public void Get(valueType value, NameValueCollection form)
            {
                object objectValuee = value;
                foreach (field field in fields)
                {
                    object fieldValue = field.Field.GetValue(objectValuee);
                    if (field.IsValueType)
                    {
                        if (field.NullableValueMethod == null) form.Add(field.Field.Name, fieldValue.ToString());
                        else if ((bool)field.NullableHasValueMethod.Invoke(fieldValue, null)) form.Add(field.Field.Name, field.NullableValueMethod.Invoke(fieldValue, null).ToString());
                    }
                    else if (fieldValue != null) form.Add(field.Field.Name, fieldValue.ToString());
                }
            }
        }
#endif
    }
}
