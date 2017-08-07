using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using System.Reflection.Emit;
using fastCSharp.threading;
using System.Text;
using System.Runtime.CompilerServices;

namespace fastCSharp.emit
{
    /// <summary>
    /// URL查询字符串生成
    /// </summary>
    internal static class urlQueryGetter
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
                dynamicMethod = new DynamicMethod("urlQueryGetter", null, new Type[] { type, typeof(charStream), typeof(Encoding) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                isValueType = type.IsValueType;
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public unsafe void PushFrist(FieldInfo field)
            {
                generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_1, pub.GetNameAssignmentPool(field.Name), field.Name.Length + 1);
                pushValue(field);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field"></param>
            private void pushValue(FieldInfo field)
            {
                MethodInfo method = pubExtension.GetNumberToCharStreamMethod(field.FieldType);
                if (method == null)
                {
                    generator.Emit(OpCodes.Ldarg_1);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldflda, field);
                    if (field.FieldType.IsEnum)
                    {
                        generator.Emit(OpCodes.Box, field.FieldType);
                        generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(typeof(object)));
                    }
                    else generator.Emit(field.FieldType.IsValueType ? OpCodes.Call : OpCodes.Callvirt, pubExtension.GetToStringMethod(field.FieldType));
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Call, pubExtension.UrlEncodeMethod);
                    generator.charStreamWriteNotNull();
                }
                else
                {
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, method);
                }
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public unsafe void PushNext(FieldInfo field)
            {
                Type type = field.FieldType;
                if (type.IsValueType)
                {
                    Type nullType = type.nullableType();
                    if (nullType == null)
                    {
                        generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_1, GetNamePool(field.Name), field.Name.Length + 2);
                        pushValue(field);
                    }
                    else
                    {
                        Label end = generator.DefineLabel();
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldflda, field);
                        generator.Emit(OpCodes.Call, pubExtension.GetNullableHasValue(type));
                        generator.Emit(OpCodes.Brfalse_S, end);
                        generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_1, GetNamePool(field.Name), field.Name.Length + 2);
                        push(field, nullType);
                        generator.MarkLabel(end);
                    }
                }
                else
                {
                    Label end = generator.DefineLabel();
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field);
                    generator.Emit(OpCodes.Brfalse_S, end);
                    generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_1, GetNamePool(field.Name), field.Name.Length + 2);
                    push(field);
                    generator.MarkLabel(end);
                }
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="nullType">可空类型</param>
            private void push(FieldInfo field, Type nullType = null)
            {
                Type type = nullType ?? field.FieldType;
                if (type == typeof(string))
                {
                    generator.Emit(OpCodes.Ldarg_1);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Call, pubExtension.UrlEncodeMethod);
                    generator.charStreamWriteNotNull();
                }
                else
                {
                    MethodInfo method = pubExtension.GetNumberToCharStreamMethod(type);
                    if (method == null)
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldflda, field);
                        if (field.FieldType.IsEnum)
                        {
                            generator.Emit(OpCodes.Box, field.FieldType);
                            generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(typeof(object)));
                        }
                        else generator.Emit(field.FieldType.IsValueType ? OpCodes.Call : OpCodes.Callvirt, pubExtension.GetToStringMethod(type));
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.Emit(OpCodes.Call, pubExtension.UrlEncodeMethod);
                        generator.charStreamWriteNotNull();
                    }
                    else
                    {
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        if (nullType == null) generator.Emit(OpCodes.Ldfld, field);
                        else
                        {
                            generator.Emit(OpCodes.Ldflda, field);
                            generator.Emit(OpCodes.Call, pubExtension.GetNullableValue(field.FieldType));
                        }
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Call, method);
                    }
                }
            }
            /// <summary>
            /// 定义局部变量
            /// </summary>
            public void DeclareIsNext()
            {
                generator.DeclareLocal(typeof(bool));
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Stloc_0);
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
                    Label end = generator.DefineLabel();
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldflda, field);
                    generator.Emit(OpCodes.Call, pubExtension.GetNullableHasValue(type));
                    generator.Emit(OpCodes.Brfalse_S, end);
                    name(field);
                    push(field, type.nullableType());
                    generator.MarkLabel(end);
                }
                else
                {
                    Label end = generator.DefineLabel();
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field);
                    generator.Emit(OpCodes.Brfalse_S, end);
                    name(field);
                    push(field);
                    generator.MarkLabel(end);
                }
            }
            /// <summary>
            /// 添加名称
            /// </summary>
            /// <param name="field"></param>
            private unsafe void name(FieldInfo field)
            {
                Label notNext = generator.DefineLabel(), end = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Brfalse_S, notNext);
                generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_1, GetNamePool(field.Name), field.Name.Length + 2);
                generator.Emit(OpCodes.Br_S, end);
                generator.MarkLabel(notNext);
                generator.charStreamSimpleWriteNotNull(OpCodes.Ldarg_1, pub.GetNameAssignmentPool(field.Name), field.Name.Length + 1);
                generator.Emit(OpCodes.Ldc_I4_1);
                generator.Emit(OpCodes.Stloc_0);
                generator.MarkLabel(end);
            }
            /// <summary>
            /// 创建委托
            /// </summary>
            /// <returns>委托</returns>
            public Delegate Create<delegateType>()
            {
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
            /// <summary>
            /// 名称数据信息集合
            /// </summary>
            private static readonly interlocked.dictionary<string, pointer> namePools = new interlocked.dictionary<string, pointer>();
            /// <summary>
            /// 获取名称数据
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static unsafe char* GetNamePool(string name)
            {
                pointer pointer;
                if (namePools.TryGetValue(name, out pointer)) return pointer.Char;
                char* value = pub.GetNamePool(name, 1, 1);
                *value = '&';
                *(value + (1 + name.Length)) = '=';
                namePools.Set(name, new pointer(value));
                return value;
            }
        }
    }
    /// <summary>
    /// URL查询字符串生成
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public static class urlQueryGetter<valueType>
    {
        /// <summary>
        /// URL查询字符串生成委托
        /// </summary>
        private static readonly Action<valueType, charStream, Encoding> getter;

        /// <summary>
        /// 获取URL查询字符串
        /// </summary>
        /// <param name="value">查询对象</param>
        /// <param name="encoding">URL编码</param>
        public unsafe static string Get(valueType value, Encoding encoding)
        {
            if (getter != null)
            {
                pointer buffer = fastCSharp.unmanagedPool.TinyBuffers.Get();
                try
                {
                    using (charStream stream = new charStream(buffer.Char, fastCSharp.unmanagedPool.TinyBuffers.Size >> 1))
                    {
                        getter(value, stream, encoding);
                        return stream.ToString();
                    }
                }
                finally { fastCSharp.unmanagedPool.TinyBuffers.Push(ref buffer); }
            }
            return null;
        }
        /// <summary>
        /// 获取URL查询字符串
        /// </summary>
        /// <param name="value">查询对象</param>
        /// <param name="stream">URL查询字符串</param>
        /// <param name="encoding">URL编码</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void Get(valueType value, charStream stream, Encoding encoding)
        {
            if (getter != null)
            {
                if (stream == null) log.Default.Throw(log.exceptionType.Null);
                getter(value, stream, encoding);
            }
        }

        static urlQueryGetter()
        {
            Type type = typeof(valueType);
            if (type.IsArray || type.IsEnum || type.IsPointer || type.IsInterface) return;
            foreach (fastCSharp.code.attributeMethod methodInfo in fastCSharp.code.attributeMethod.GetStatic(type))
            {
                if (methodInfo.Method.ReturnType == typeof(void))
                {
                    ParameterInfo[] parameters = methodInfo.Method.GetParameters();
                    if (parameters.Length == 3 && parameters[0].ParameterType == type && parameters[1].ParameterType == typeof(charStream) && parameters[2].ParameterType == typeof(Encoding))
                    {
                        if (methodInfo.GetAttribute<urlQuery.custom>(true) != null)
                        {
                            getter = (Action<valueType, charStream, Encoding>)Delegate.CreateDelegate(typeof(Action<valueType, charStream, Encoding>), methodInfo.Method);
                            return;
                        }
                    }
                }
            }
            urlQuery attribute = fastCSharp.code.typeAttribute.GetAttribute<urlQuery>(type, true, true) ?? urlQuery.AllMember;
            subArray<FieldInfo> fields = pubExtension.GetFields<valueType, urlQuery.member>(attribute.MemberFilter, attribute.IsAllMember);
            if (fields.Count != 0)
            {
                FieldInfo valueField = null;
                urlQueryGetter.memberDynamicMethod dynamicMethod = new urlQueryGetter.memberDynamicMethod(type);
                foreach (FieldInfo member in fields)
                {
                    if (member.FieldType.IsValueType && member.FieldType.nullableType() == null)
                    {
                        dynamicMethod.PushFrist(valueField = member);
                        break;
                    }
                }
                if (valueField == null)
                {
                    dynamicMethod.DeclareIsNext();
                    foreach (FieldInfo member in fields) dynamicMethod.Push(member);
                }
                else
                {
                    foreach (FieldInfo member in fields)
                    {
                        if (valueField != member) dynamicMethod.PushNext(member);
                    }
                }
                getter = (Action<valueType, charStream, Encoding>)dynamicMethod.Create<Action<valueType, charStream, Encoding>>();
            }
        }
    }
}
