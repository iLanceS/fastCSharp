using System;
using fastCSharp.threading;
using System.Security.Cryptography;
using fastCSharp.code;
using fastCSharp.emit;
using System.Reflection;
using fastCSharp.reflection;
using System.Threading;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 签名计算
    /// </summary>
    sealed class sign : memberFilter.instanceField
    {
        /// <summary>
        /// 默认签名计算类型配置
        /// </summary>
        public static readonly sign AllMember = new sign { IsAllMember = true };
        /// <summary>
        /// 默认签名计算成员配置
        /// </summary>
        public static readonly member DefaultMember = new member();
        /// <summary>
        /// 是否序列化所有成员
        /// </summary>
        public bool IsAllMember;
        /// <summary>
        /// 签名计算成员配置
        /// </summary>
        public sealed class member : ignoreMember
        {
            /// <summary>
            /// 是否需要Utf-8编码
            /// </summary>
            public bool IsEncodeUtf8 = true;
        }
#if NOJIT
#else
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
                dynamicMethod = new DynamicMethod("signValueGetter", null, new Type[] { type, typeof(string[]) }, type, true);
                generator = dynamicMethod.GetILGenerator();
                isValueType = type.IsValueType;
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field"></param>
            /// <param name="index"></param>
            /// <returns>是否需要utf-8编码</returns>
            public bool Push(FieldInfo field, int index)
            {
                Type type = field.FieldType;
                if (type.IsValueType)
                {
                    MethodInfo numberToStringMethod;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        Label end = generator.DefineLabel();
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldflda, field);
                        generator.Emit(OpCodes.Call, pubExtension.GetNullableHasValue(type));
                        generator.Emit(OpCodes.Brfalse_S, end);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.int32(index);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldflda, field);
                        generator.Emit(OpCodes.Call, pubExtension.GetNullableValue(type));
                        Type nullableType = type.GetGenericArguments()[0];
                        if (nullableType.IsEnum)
                        {
                            numberToStringMethod = null;
                            generator.Emit(OpCodes.Box, nullableType);
                            generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(typeof(object)));
                        }
                        else generator.Emit(OpCodes.Call, (numberToStringMethod = pubExtension.GetNumberToStringMethod(nullableType)) ?? pubExtension.GetToStringMethod(nullableType));
                        generator.Emit(OpCodes.Stelem_Ref);
                        generator.MarkLabel(end);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.int32(index);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, field);
                        if (type.IsEnum)
                        {
                            numberToStringMethod = null;
                            generator.Emit(OpCodes.Box, type);
                            generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(typeof(object)));
                        }
                        else generator.Emit(OpCodes.Call, (numberToStringMethod = pubExtension.GetNumberToStringMethod(type)) ?? pubExtension.GetToStringMethod(type));
                        generator.Emit(OpCodes.Stelem_Ref);
                    }
                    if (numberToStringMethod != null) return false;
                }
                else
                {
                    Label end = default(Label);
                    if (type != typeof(string))
                    {
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, field);
                        generator.Emit(OpCodes.Brfalse_S, end = generator.DefineLabel());
                    }
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.int32(index);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field);
                    if (type != typeof(string)) generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(type));
                    generator.Emit(OpCodes.Stelem_Ref);
                    if (type != typeof(string)) generator.MarkLabel(end);
                }
                return (field.customAttribute<member>(false) ?? DefaultMember).IsEncodeUtf8;
            }
            /// <summary>
            /// 添加属性
            /// </summary>
            /// <param name="property"></param>
            /// <param name="index"></param>
            /// <returns>是否需要utf-8编码</returns>
            public bool Push(PropertyInfo property, int index)
            {
                Type type = property.PropertyType;
                MethodInfo method = property.GetGetMethod(true);
                if (type.IsValueType)
                {
                    MethodInfo numberToStringMethod;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        Label end = generator.DefineLabel();
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Call, method);
                        generator.Emit(OpCodes.Call, pubExtension.GetNullableHasValue(type));
                        generator.Emit(OpCodes.Brfalse_S, end);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.int32(index);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Call, method);
                        generator.Emit(OpCodes.Call, pubExtension.GetNullableValue(type));
                        Type nullableType = type.GetGenericArguments()[0];
                        if (nullableType.IsEnum)
                        {
                            numberToStringMethod = null;
                            generator.Emit(OpCodes.Box, nullableType);
                            generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(typeof(object)));
                        }
                        else generator.Emit(OpCodes.Call, (numberToStringMethod = pubExtension.GetNumberToStringMethod(nullableType)) ?? pubExtension.GetToStringMethod(nullableType));
                        generator.Emit(OpCodes.Stelem_Ref);
                        generator.MarkLabel(end);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.int32(index);
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Call, method);
                        if (type.IsEnum)
                        {
                            numberToStringMethod = null;
                            generator.Emit(OpCodes.Box, type);
                            generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(typeof(object)));
                        }
                        else generator.Emit(OpCodes.Call, (numberToStringMethod = pubExtension.GetNumberToStringMethod(type)) ?? pubExtension.GetToStringMethod(type));
                        generator.Emit(OpCodes.Stelem_Ref);
                    }
                    if (numberToStringMethod != null) return false;
                }
                else
                {
                    Label end = default(Label);
                    if (type != typeof(string))
                    {
                        if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                        else generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                        generator.Emit(OpCodes.Brfalse_S, end = generator.DefineLabel());
                    }
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.int32(index);
                    if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                    else generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(method.IsFinal || !method.IsVirtual ? OpCodes.Call : OpCodes.Callvirt, method);
                    if (type != typeof(string)) generator.Emit(OpCodes.Callvirt, pubExtension.GetToStringMethod(type));
                    generator.Emit(OpCodes.Stelem_Ref);
                    if (type != typeof(string)) generator.MarkLabel(end);
                }
                return (property.customAttribute<member>(false) ?? DefaultMember).IsEncodeUtf8;
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
        }
#endif
        /// <summary>
        /// 数据缓冲区池
        /// </summary>
        private static readonly arrayPool<string[]>[] pools = new arrayPool<string[]>[32];
        /// <summary>
        /// 数据缓冲区访问锁
        /// </summary>
        private static readonly object poolLock = new object();
        /// <summary>
        /// 获取数据缓冲区
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string[] GetValue(int index)
        {
            if (pools[index].UnsafeArray == null)
            {
                Monitor.Enter(poolLock);
                if (pools[index].UnsafeArray == null)
                {
                    try
                    {
                        pools[index].UnsafeCreate(sizeof(int));
                    }
                    finally { Monitor.Exit(poolLock); }
                }
                else Monitor.Exit(poolLock);
            }
            string[] value = null;
            return pools[index].TryGet(ref value) ? value : new string[1 << index];
        }
        /// <summary>
        /// 添加到数据缓冲区
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void PushValue(int index, string[] value)
        {
            pools[index].Push(value);
        }

        /// <summary>
        /// 字符串比较大小
        /// </summary>
        public static Func<keyValue<FieldInfo, PropertyInfo>, keyValue<FieldInfo, PropertyInfo>, int> NameCompare = compare;
        /// <summary>
        /// 字符串比较大小
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static int compare(keyValue<FieldInfo, PropertyInfo> left, keyValue<FieldInfo, PropertyInfo> right)
        {
            return string.CompareOrdinal(left.Key == null ? left.Value.Name : left.Key.Name, right.Key == null ? right.Value.Name : right.Key.Name);
        }
    }
    /// <summary>
    /// 签名计算 https://pay.weixin.qq.com/wiki/tools/signverify/
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    static class sign<valueType> where valueType : class
    {
        /// <summary>
        /// 获取数据到数据缓冲区
        /// </summary>
        private static Action<valueType, string[]> valueGetter;
        /// <summary>
        /// 设置签名
        /// </summary>
        private static Action<valueType, string> setSign;
        /// <summary>
        /// 签名名称集合
        /// </summary>
        private static readonly string[] names;
        /// <summary>
        /// 签名数据是否需要Utf8编码
        /// </summary>
        private static readonly pointer.reference isUtf8;
        /// <summary>
        /// 数据缓冲区池索引编号
        /// </summary>
        private static readonly int poolIndex;
        /// <summary>
        /// 签名计算
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key">签名密钥</param>
        public unsafe static void Set(valueType value, string key)
        {
            if (valueGetter == null) log.Default.Throw(log.exceptionType.ErrorOperation);
            if (value == null || key == null) log.Default.Throw(log.exceptionType.Null);
            string[] values = sign.GetValue(poolIndex);
            //fixedMap utf8Map = new fixedMap(isUtf8.Byte);
            try
            {
                int length = 4 + key.Length + getLength(value, values);
                memoryPool memoryPool = memoryPool.GetDefaultPool(length);
                byte[] buffer = memoryPool.Get(length);
                try
                {
                    concat(values, buffer, length, key);
                    using (MD5 md5 = new MD5CryptoServiceProvider()) setSign(value, md5.ComputeHash(buffer, 0, length).toUpperHex());
                }
                finally { memoryPool.PushNotNull(buffer); }
            }
            finally
            {
                Array.Clear(values, 0, names.Length);
                sign.PushValue(poolIndex, values);
            }
        }
        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public unsafe static bool Check(valueType value, string key, string sign)
        {
            if (valueGetter == null) log.Default.Throw(log.exceptionType.ErrorOperation);
            if (value == null || key == null) log.Default.Throw(log.exceptionType.Null);
            if (sign.length() == 32)
            {
                string[] values = weixin.sign.GetValue(poolIndex);
                try
                {
                    int length = 4 + key.Length + getLength(value, values);
                    memoryPool memoryPool = memoryPool.GetDefaultPool(length);
                    byte[] buffer = memoryPool.Get(length);
                    try
                    {
                        concat(values, buffer, length, key);
                        using (MD5 md5 = new MD5CryptoServiceProvider())
                        {
                            if (fastCSharp.unsafer.memory.CheckUpperHex(md5.ComputeHash(buffer, 0, length), sign)) return true;
                        }
                    }
                    finally { memoryPool.PushNotNull(buffer); }
                }
                finally
                {
                    Array.Clear(values, 0, names.Length);
                    weixin.sign.PushValue(poolIndex, values);
                }
            }
            return false;
        }
        /// <summary>
        /// 获取签名计算数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public unsafe static memoryPool.pushSubArray GetData(valueType value, string key)
        {
            if (valueGetter == null) log.Default.Throw(log.exceptionType.ErrorOperation);
            if (value == null || key == null) log.Default.Throw(log.exceptionType.Null);
            string[] values = sign.GetValue(poolIndex);
            //fixedMap utf8Map = new fixedMap(isUtf8.Byte);
            try
            {
                int length = 4 + key.Length + getLength(value, values);
                memoryPool memoryPool = memoryPool.GetDefaultPool(length);
                byte[] buffer = memoryPool.Get(length);
                concat(values, buffer, length, key);
                using (MD5 md5 = new MD5CryptoServiceProvider()) setSign(value, md5.ComputeHash(buffer, 0, length).toUpperHex());
                return new memoryPool.pushSubArray(subArray<byte>.Unsafe(buffer, 0, length - key.Length - 5), memoryPool);
            }
            finally
            {
                Array.Clear(values, 0, names.Length);
                sign.PushValue(poolIndex, values);
            }
        }
        /// <summary>
        /// 计算编码数据长度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private unsafe static int getLength(valueType value, string[] values)
        {
            valueGetter(value, values);
            int length = 0, index = 0;
            fixedMap utf8Map = new fixedMap(isUtf8.Byte);
            foreach (string name in names)
            {
                string valueString = values[index];
                if (!string.IsNullOrEmpty(valueString))
                {
                    length += 2 + name.Length + (utf8Map.Get(index) ? System.Text.Encoding.UTF8.GetByteCount(valueString) : valueString.Length);
                }
                ++index;
            }
            return length;
        }
        /// <summary>
        /// 字节拼接
        /// </summary>
        /// <param name="values"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="key"></param>
        private unsafe static void concat(string[] values, byte[] buffer, int length, string key)
        {
            fixed (byte* bufferFixed = buffer)
            {
                byte* write = bufferFixed, end = bufferFixed + length;
                int isValue = 0, index = 0;
                fixedMap utf8Map = new fixedMap(isUtf8.Byte);
                foreach (string name in names)
                {
                    string valueString = values[index];
                    if (!string.IsNullOrEmpty(valueString))
                    {
                        if (isValue == 0) isValue = 1;
                        else *write++ = (byte)'&';
                        fixed (char* nameFixed = name) unsafer.String.WriteBytes(nameFixed, name.Length, write);
                        write += name.Length;
                        *write++ = (byte)'=';
                        fixed (char* valueFixed = valueString)
                        {
                            if (utf8Map.Get(index)) write += System.Text.Encoding.UTF8.GetBytes(valueFixed, valueString.Length, write, (int)(end - write));
                            else
                            {
                                unsafer.String.WriteBytes(valueFixed, valueString.Length, write);
                                write += valueString.Length;
                            }
                        }
                    }
                    ++index;
                }
                if (isValue != 0) *write++ = (byte)'&';
                *(int*)write = 'k' + ('e' << 8) + ('y' << 16) + ('=' << 24);
                write += sizeof(int);
                fixed (char* keyFixed = key) unsafer.String.WriteBytes(keyFixed, key.Length, write);
            }
        }
        /// <summary>
        /// 没有成员
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        private static void empty(valueType value, string[] values)
        {
        }
        unsafe static sign()
        {
            Type type = typeof(valueType);
            if (type.IsArray || type.IsEnum || type.IsPointer || type.IsInterface) return;
            sign attribute = fastCSharp.code.typeAttribute.GetAttribute<sign>(type, true, true) ?? sign.AllMember;
            subArray<FieldInfo> fields = pubExtension.GetFields<valueType, sign.member>(attribute.MemberFilter, attribute.IsAllMember);
            subArray<PropertyInfo> properties = pubExtension.GetProperties<valueType, sign.member>(attribute.MemberFilter, attribute.IsAllMember, true, false);
            int count = fields.Count + properties.Count - 1;
            if (count < 0) return;
            subArray<keyValue<FieldInfo, PropertyInfo>> members = new subArray<keyValue<FieldInfo, PropertyInfo>>(count);
            FieldInfo signField = null;
            PropertyInfo signProperty = null;
            foreach (FieldInfo field in fields)
            {
                if (field.Name == "sign")
                {
                    if (field.FieldType != typeof(string)) return;
                    signField = field;
                }
                else members.UnsafeAdd(new keyValue<FieldInfo, PropertyInfo>(field, null));
            }
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "sign")
                {
                    if (property.PropertyType != typeof(string)) return;
                    signProperty = property;
                }
                else members.UnsafeAdd(new keyValue<FieldInfo, PropertyInfo>(null, property));
            }
            if ((signField == null) ^ (signProperty == null))
            {
                setSign = signField == null ? emit.pub.SetProperty<valueType, string>(signProperty) : emit.pub.UnsafeSetField<valueType, string>(signField);
                if (count == 0)
                {
                    names = nullValue<string>.Array;
                    valueGetter = empty;
                }
                else
                {
                    members.UnsafeArray.sort(sign.NameCompare);
                    names = new string[count];
                    isUtf8 = unmanaged.GetStatic(((count + 31) >> 5) << 2, true).Reference;
                    poolIndex = ((uint)count - 1).bits();
#if NOJIT
                    signer signer = new signer(members.UnsafeArray.Length);
#else
                    sign.memberDynamicMethod dynamicMethod = new sign.memberDynamicMethod(type);
#endif
                    fixedMap utf8Map = new fixedMap(isUtf8.Byte);
                    count = 0;
                    foreach (keyValue<FieldInfo, PropertyInfo> member in members.UnsafeArray)
                    {
#if NOJIT
                        if (member.Key == null ? signer.Push(member.Value, count) : signer.Push(member.Key, count)) utf8Map.Set(count);
#else
                        if (member.Key == null ? dynamicMethod.Push(member.Value, count) : dynamicMethod.Push(member.Key, count)) utf8Map.Set(count);
#endif
                        names[count++] = member.Key == null ? member.Value.Name : member.Key.Name;
                    }
#if NOJIT
                    valueGetter = signer.Sign;
#else
                    valueGetter = (Action<valueType, string[]>)dynamicMethod.Create<Action<valueType, string[]>>();
#endif
                }
            }
        }
#if NOJIT
        /// <summary>
        /// 签名计算
        /// </summary>
        private sealed class signer
        {
            /// <summary>
            /// 成员
            /// </summary>
            private struct member
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                public FieldInfo Field;
                /// <summary>
                /// 获取属性函数信息
                /// </summary>
                public MethodInfo GetProperty;
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
                /// 是否字符串
                /// </summary>
                public bool IsString;
                /// <summary>
                /// 设置字段信息
                /// </summary>
                /// <param name="field"></param>
                /// <returns></returns>
                public bool Set(FieldInfo field)
                {
                    Field = field;
                    bool? isUtf8 = set(field.FieldType);
                    if (isUtf8 == null) return (field.customAttribute<sign.member>(false) ?? sign.DefaultMember).IsEncodeUtf8;
                    return isUtf8.Value;
                }
                /// <summary>
                /// 设置属性信息
                /// </summary>
                /// <param name="property"></param>
                /// <returns></returns>
                public bool Set(PropertyInfo property)
                {
                    GetProperty = property.GetGetMethod(true);
                    bool? isUtf8 = set(property.PropertyType);
                    if(isUtf8 == null) return (property.customAttribute<sign.member>(false) ?? sign.DefaultMember).IsEncodeUtf8;
                    return isUtf8.Value;
                }
                /// <summary>
                /// 设置类型信息
                /// </summary>
                /// <param name="type"></param>
                /// <returns></returns>
                private bool? set(Type type)
                {
                    if (type.IsValueType)
                    {
                        IsValueType = true;
                        MethodInfo numberToStringMethod;
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            NullableHasValueMethod = pubExtension.GetNullableHasValue(type);
                            NullableValueMethod = pubExtension.GetNullableValue(type);
                            numberToStringMethod = pubExtension.GetNumberToStringMethod(type.GetGenericArguments()[0]);
                        }
                        else numberToStringMethod = pubExtension.GetNumberToStringMethod(type);
                        if (numberToStringMethod != null) return false;
                    }
                    else if (type == typeof(string)) IsString = true;
                    return null;
                }
            }
            /// <summary>
            /// 成员集合
            /// </summary>
            private member[] members;
            /// <summary>
            /// 签名计算
            /// </summary>
            /// <param name="count"></param>
            public signer(int count)
            {
                members = new member[count];
            }
            /// <summary>
            /// 添加字段信息
            /// </summary>
            /// <param name="field"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool Push(FieldInfo field, int index)
            {
                return members[index].Set(field);
            }
            /// <summary>
            /// 添加属性信息
            /// </summary>
            /// <param name="property"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool Push(PropertyInfo property, int index)
            {
                return members[index].Set(property);
            }
            /// <summary>
            /// 签名计算
            /// </summary>
            /// <param name="value"></param>
            /// <param name="values"></param>
            public void Sign(valueType value, string[] values)
            {
                int index = 0;
                foreach (member member in members)
                {
                    object memberValue = member.Field == null ? member.GetProperty.Invoke(value, null) : member.Field.GetValue(value);
                    if (member.IsValueType)
                    {
                        if (member.NullableHasValueMethod == null) values[index] = memberValue.ToString();
                        else if ((bool)member.NullableHasValueMethod.Invoke(value, null)) values[index] = member.NullableValueMethod.Invoke(memberValue, null).ToString();
                    }
                    else
                    {
                        if (member.IsString) values[index] = (string)memberValue;
                        else if (memberValue != null) values[index] = memberValue.ToString();
                    }
                    ++index;
                }
            }
        }
#endif
    }
}
