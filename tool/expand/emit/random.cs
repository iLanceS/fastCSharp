using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.code;
using System.Runtime.CompilerServices;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 随机对象生成
    /// </summary>
    public static class random
    {
        /// <summary>
        /// 最大随机数组尺寸
        /// </summary>
        private const uint maxSize = (1 << 4) - 1;
        /// <summary>
        /// 随机对象成员配置
        /// </summary>
        public sealed class member : ignoreMember
        {
        }
        /// <summary>
        /// 随机对象生成配置
        /// </summary>
        public sealed class config
        {
            /// <summary>
            /// 时间是否精确到秒
            /// </summary>
            public bool IsSecondDateTime;
            /// <summary>
            /// 浮点数是否转换成字符串
            /// </summary>
            public bool IsParseFloat;
            /// <summary>
            /// 是否生成字符0
            /// </summary>
            public bool IsNullChar = true;
            /// <summary>
            /// 历史对象集合
            /// </summary>
            public Dictionary<Type, list<object>> History;
            /// <summary>
            /// 获取历史对象
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            internal object TryGetValue(Type type)
            {
                if (History != null && fastCSharp.random.Default.NextBit() == 0)
                {
                    list<object> objects;
                    if (History.TryGetValue(type, out objects)) return objects.UnsafeArray[fastCSharp.random.Default.Next(objects.Count)];
                }
                return null;
            }
            /// <summary>
            /// 保存历史对象
            /// </summary>
            /// <typeparam name="valueType"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            internal valueType SaveHistory<valueType>(valueType value)
            {
                if (History != null && value != null)
                {
                    list<object> objects;
                    if (!History.TryGetValue(typeof(valueType), out objects)) History.Add(typeof(valueType), objects = new list<object>());
                    objects.Add(value);
                }
                return value;
            }
        }
        /// <summary>
        /// 默认随机对象生成配置
        /// </summary>
        internal static readonly config DefaultConfig = new config { History = dictionary.CreateAny<Type, list<object>>() };
#if NOJIT
#else
        /// <summary>
        /// 动态函数
        /// </summary>
        internal struct memberDynamicMethod
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
            /// 是否值类型
            /// </summary>
            private bool isValueType;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            public memberDynamicMethod(Type type)
            {
                dynamicMethod = new DynamicMethod("random", null, new Type[] { type.MakeByRefType(), typeof(config) }, this.type = type, true);
                generator = dynamicMethod.GetILGenerator();
                isValueType = type.IsValueType;
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(FieldInfo field)
            {
                generator.Emit(OpCodes.Ldarg_0);
                if (!isValueType) generator.Emit(OpCodes.Ldind_Ref);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, CreateMethod.MakeGenericMethod(field.FieldType));
                generator.Emit(OpCodes.Stfld, field);
            }
            /// <summary>
            /// 基类调用
            /// </summary>
            public void Base()
            {
                if (!isValueType && (type = type.BaseType) != typeof(object))
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, CreateMemberMethod.MakeGenericMethod(type));
                }
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
        /// 基本类型随机数创建函数
        /// </summary>
        private sealed class createMethod : Attribute { }
        /// <summary>
        /// 基本类型随机数创建函数
        /// </summary>
        private sealed class createConfigMethod : Attribute { }
        /// <summary>
        /// 基本类型随机数创建函数
        /// </summary>
        private sealed class createConfigNullMethod : Attribute { }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static bool createBool()
        {
            return fastCSharp.random.Default.NextBit() != 0;
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static byte CreateByte()
        {
            return fastCSharp.random.Default.NextByte();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static sbyte CreateSByte()
        {
            return (sbyte)fastCSharp.random.Default.NextByte();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static short CreateShort()
        {
            return (short)fastCSharp.random.Default.NextUShort();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static ushort CreateUShort()
        {
            return fastCSharp.random.Default.NextUShort();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static int CreateInt()
        {
            return fastCSharp.random.Default.Next();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static uint CreateUInt()
        {
            return (uint)fastCSharp.random.Default.Next();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static long CreateLong()
        {
            return (long)fastCSharp.random.Default.NextULong();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static ulong CreateULong()
        {
            return fastCSharp.random.Default.NextULong();
        }
        /// <summary>
        /// 随机数除数
        /// </summary>
        private static readonly decimal decimalDiv = 100;
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static decimal createDecimal()
        {
            return (decimal)(long)fastCSharp.random.Default.NextULong() / decimalDiv;
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        [createMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static Guid createGuid()
        {
            return Guid.NewGuid();
        }
        /// <summary>
        /// 创建随机字符
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [createConfigMethod]
        [createConfigNullMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static char createChar(config config)
        {
            if (config.IsNullChar) return (char)fastCSharp.random.Default.NextUShort();
            char value = (char)fastCSharp.random.Default.NextUShort();
            return value == 0 ? char.MaxValue : value;
        }
        /// <summary>
        /// 创建随机字符串
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [createConfigMethod]
        private static string createString(config config)
        {
            object historyValue = config.TryGetValue(typeof(string));
            if (historyValue != null) return (string)historyValue;
            return config.SaveHistory(new string(createArray<char>(config)));
        }
        /// <summary>
        /// 创建随机字符串
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [createConfigNullMethod]
        private static string createStringNull(config config)
        {
            object historyValue = config.TryGetValue(typeof(string));
            if (historyValue != null) return (string)historyValue;
            char[] value = createArrayNull<char>(config);
            return config.SaveHistory(value == null ? null : new string(value));
        }
        /// <summary>
        /// 创建随机字符串
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [createConfigMethod]
        [createConfigNullMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static subString createSubString(config config)
        {
            return new subString(createStringNull(config));
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [createConfigMethod]
        [createConfigNullMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static float createFloat(config config)
        {
            if (config.IsParseFloat)
            {
                return float.Parse(fastCSharp.random.Default.NextFloat().ToString());
            }
            return fastCSharp.random.Default.NextFloat();
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [createConfigMethod]
        [createConfigNullMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static double createDouble(config config)
        {
            if (config.IsParseFloat)
            {
                return double.Parse(fastCSharp.random.Default.NextDouble().ToString());
            }
            return fastCSharp.random.Default.NextDouble();
        }
        /// <summary>
        /// 创建随机时间
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [createConfigMethod]
        [createConfigNullMethod]
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static DateTime createDateTime(config config)
        {
            if (config.IsSecondDateTime)
            {
                return new DateTime((long)(fastCSharp.random.Default.NextULong() % (ulong)DateTime.MaxValue.Ticks) / date.SecondTicks * date.SecondTicks);
            }
            return new DateTime((long)(fastCSharp.random.Default.NextULong() % (ulong)DateTime.MaxValue.Ticks));
        }
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static valueType[] createArray<valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(valueType[]));
            if (historyValue != null) return (valueType[])historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            if (length > 0)
            {
                valueType[] value = config.SaveHistory(new valueType[--length]);
                while (length != 0) value[--length] = random<valueType>.CreateNull(config);
                return value;
            }
            return null;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateArrayMethod = typeof(random).GetMethod("createArray", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static valueType[] createArrayNull<valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(valueType[]));
            if (historyValue != null) return (valueType[])historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            valueType[] value = config.SaveHistory(new valueType[length]);
            while (length != 0) value[--length] = random<valueType>.CreateNull(config);
            return value;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateArrayNullMethod = typeof(random).GetMethod("createArrayNull", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建可空随机对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static Nullable<valueType> createNullable<valueType>(config config) where valueType : struct
        {
            if (createBool()) return random<valueType>.CreateNotNull(config);
            return new Nullable<valueType>();
        }
        /// <summary>
        /// 创建可空随机对象函数信息
        /// </summary>
        internal static readonly MethodInfo CreateNullableMethod = typeof(random).GetMethod("createNullable", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static subArray<valueType> createSubArray<valueType>(config config)
        {
            return new subArray<valueType>(createArray<valueType>(config));
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateSubArrayMethod = typeof(random).GetMethod("createSubArray", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static list<valueType> createList<valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(list<valueType>));
            if (historyValue != null) return (list<valueType>)historyValue;
            list<valueType> value = config.SaveHistory(new list<valueType>());
            value.Add(createArray<valueType>(config));
            return value;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateListMethod = typeof(random).GetMethod("createList", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static list<valueType> createListNull<valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(list<valueType>));
            if (historyValue != null) return (list<valueType>)historyValue;
            valueType[] array = createArrayNull<valueType>(config);
            return array == null ? null : config.SaveHistory(new list<valueType>(array, true));
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateListNullMethod = typeof(random).GetMethod("createListNull", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static List<valueType> createSystemList<valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(List<valueType>));
            if (historyValue != null) return (List<valueType>)historyValue;
            List<valueType> value = config.SaveHistory(new List<valueType>());
            value.AddRange(createArray<valueType>(config));
            return value;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateSystemListMethod = typeof(random).GetMethod("createSystemList", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="argumentType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static valueType createEnumerableConstructorNull<valueType, argumentType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(valueType));
            if (historyValue != null) return (valueType)historyValue;
            valueType[] array = createArrayNull<valueType>(config);
            return array == null ? default(valueType) : config.SaveHistory(pub.enumerableConstructor<valueType, argumentType>.Constructor(createArray<argumentType>(config)));
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateEnumerableConstructorNullMethod = typeof(random).GetMethod("createEnumerableConstructorNull", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="argumentType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static valueType createEnumerableConstructor<valueType, argumentType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(valueType));
            if (historyValue != null) return (valueType)historyValue;
            return config.SaveHistory(pub.enumerableConstructor<valueType, argumentType>.Constructor(createArray<argumentType>(config)));
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateEnumerableConstructorMethod = typeof(random).GetMethod("createEnumerableConstructor", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static List<valueType> createSystemListNull<valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(List<valueType>));
            if (historyValue != null) return (List<valueType>)historyValue;
            valueType[] array = createArrayNull<valueType>(config);
            return array == null ? null : config.SaveHistory(new List<valueType>(array));
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateSystemListNullMethod = typeof(random).GetMethod("createSystemListNull", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static Dictionary<keyType, valueType> createDictionary<keyType, valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(Dictionary<keyType, valueType>));
            if (historyValue != null) return (Dictionary<keyType, valueType>)historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            Dictionary<keyType, valueType> values = config.SaveHistory(dictionary.CreateAny<keyType, valueType>((int)length));
            while (length-- != 0)
            {
                keyType key = random<keyType>.CreateNotNull(config);
                valueType value;
                if (!values.TryGetValue(key, out value)) values.Add(key, random<valueType>.CreateNull(config));
            }
            return values;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateDictionaryMethod = typeof(random).GetMethod("createDictionary", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static Dictionary<keyType, valueType> createDictionaryNull<keyType, valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(Dictionary<keyType, valueType>));
            if (historyValue != null) return (Dictionary<keyType, valueType>)historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            if (length > 0)
            {
                Dictionary<keyType, valueType> values = config.SaveHistory(dictionary.CreateAny<keyType, valueType>((int)length));
                while (--length != 0)
                {
                    keyType key = random<keyType>.CreateNotNull(config);
                    valueType value;
                    if (!values.TryGetValue(key, out value)) values.Add(key, random<valueType>.CreateNull(config));
                }
                return values;
            }
            return null;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateDictionaryNullMethod = typeof(random).GetMethod("createDictionaryNull", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static SortedList<keyType, valueType> createSortedList<keyType, valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(Dictionary<keyType, valueType>));
            if (historyValue != null) return (SortedList<keyType, valueType>)historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            SortedList<keyType, valueType> values = config.SaveHistory(new SortedList<keyType, valueType>((int)length));
            while (length-- != 0)
            {
                keyType key = random<keyType>.CreateNotNull(config);
                valueType value;
                if (!values.TryGetValue(key, out value)) values.Add(key, random<valueType>.CreateNull(config));
            }
            return values;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateSortedListMethod = typeof(random).GetMethod("createSortedList", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static SortedList<keyType, valueType> createSortedListNull<keyType, valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(Dictionary<keyType, valueType>));
            if (historyValue != null) return (SortedList<keyType, valueType>)historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            if (length > 0)
            {
                SortedList<keyType, valueType> values = config.SaveHistory(new SortedList<keyType, valueType>((int)length));
                while (--length != 0)
                {
                    keyType key = random<keyType>.CreateNotNull(config);
                    valueType value;
                    if (!values.TryGetValue(key, out value)) values.Add(key, random<valueType>.CreateNull(config));
                }
                return values;
            }
            return null;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateSortedListNullMethod = typeof(random).GetMethod("createSortedListNull", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static SortedDictionary<keyType, valueType> createSortedDictionary<keyType, valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(Dictionary<keyType, valueType>));
            if (historyValue != null) return (SortedDictionary<keyType, valueType>)historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            SortedDictionary<keyType, valueType> values = config.SaveHistory(new SortedDictionary<keyType, valueType>());
            while (length-- != 0)
            {
                keyType key = random<keyType>.CreateNotNull(config);
                valueType value;
                if (!values.TryGetValue(key, out value)) values.Add(key, random<valueType>.CreateNull(config));
            }
            return values;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateSortedDictionaryMethod = typeof(random).GetMethod("createSortedDictionary", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机数组
        /// </summary>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        private static SortedDictionary<keyType, valueType> createSortedDictionaryNull<keyType, valueType>(config config)
        {
            object historyValue = config.TryGetValue(typeof(Dictionary<keyType, valueType>));
            if (historyValue != null) return (SortedDictionary<keyType, valueType>)historyValue;
            uint length = (uint)fastCSharp.random.Default.NextByte() & maxSize;
            if (length > 0)
            {
                SortedDictionary<keyType, valueType> values = config.SaveHistory(new SortedDictionary<keyType, valueType>());
                while (--length != 0)
                {
                    keyType key = random<keyType>.CreateNotNull(config);
                    valueType value;
                    if (!values.TryGetValue(key, out value)) values.Add(key, random<valueType>.CreateNull(config));
                }
                return values;
            }
            return null;
        }
        /// <summary>
        /// 创建随机数组函数信息
        /// </summary>
        internal static readonly MethodInfo CreateSortedDictionaryNullMethod = typeof(random).GetMethod("createSortedDictionaryNull", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static valueType create<valueType>(config config)
        {
            return random<valueType>.CreateNull(config);
        }
        /// <summary>
        /// 创建随机对象函数信息
        /// </summary>
        internal static readonly MethodInfo CreateMethod = typeof(random).GetMethod("create", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 创建随机成员对象
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="value"></param>
        /// <param name="config"></param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private static void createMember<valueType>(ref valueType value, config config)
        {
            random<valueType>.MemberCreator(ref value, config);
        }
        /// <summary>
        /// 创建随机对象函数信息
        /// </summary>
        internal static readonly MethodInfo CreateMemberMethod = typeof(random).GetMethod("createMember", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 基本类型随机数创建函数信息集合
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> createMethods;
        /// <summary>
        /// 获取基本类型随机数创建函数信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns>基本类型随机数创建函数信息</returns>
        internal static MethodInfo GetMethod(Type type)
        {
            MethodInfo method;
            if (createMethods.TryGetValue(type, out method))
            {
                createMethods.Remove(type);
                return method;
            }
            return null;
        }
        /// <summary>
        /// 基本类型随机数创建函数信息集合
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> createConfigMethods;
        /// <summary>
        /// 获取基本类型随机数创建函数信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns>基本类型随机数创建函数信息</returns>
        internal static MethodInfo GetConfigMethod(Type type)
        {
            MethodInfo method;
            if (createConfigMethods.TryGetValue(type, out method))
            {
                createConfigMethods.Remove(type);
                return method;
            }
            return null;
        }
        /// <summary>
        /// 基本类型随机数创建函数信息集合
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> createConfigNullMethods;
        /// <summary>
        /// 获取基本类型随机数创建函数信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns>基本类型随机数创建函数信息</returns>
        internal static MethodInfo GetConfigNullMethod(Type type)
        {
            MethodInfo method;
            if (createConfigNullMethods.TryGetValue(type, out method))
            {
                createConfigNullMethods.Remove(type);
                return method;
            }
            return null;
        }
        static random()
        {
            createMethods = dictionary.CreateOnly<Type, MethodInfo>();
            createConfigMethods = dictionary.CreateOnly<Type, MethodInfo>();
            createConfigNullMethods = dictionary.CreateOnly<Type, MethodInfo>();
            foreach (MethodInfo method in typeof(random).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (method.customAttribute<createMethod>() != null)
                {
                    createMethods.Add(method.ReturnType, method);
                }
                else
                {
                    if (method.customAttribute<createConfigMethod>() != null)
                    {
                        createConfigMethods.Add(method.ReturnType, method);
                    }
                    if (method.customAttribute<createConfigNullMethod>() != null)
                    {
                        createConfigNullMethods.Add(method.ReturnType, method);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 随机对象生成
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public static class random<valueType>
    {
        /// <summary>
        /// 创建随机对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="config"></param>
        internal delegate void creator(ref valueType value, random.config config);
        /// <summary>
        /// 基本类型随机数创建函数
        /// </summary>
        private static readonly Func<valueType> defaultCreator;
        /// <summary>
        /// 随机对象创建函数
        /// </summary>
        private static readonly Func<random.config, valueType> configNullCreator;
        /// <summary>
        /// 随机对象创建函数
        /// </summary>
        private static readonly Func<random.config, valueType> configCreator;
        /// <summary>
        /// 随机对象创建函数
        /// </summary>
        internal static readonly creator MemberCreator;
        /// <summary>
        /// 是否值类型
        /// </summary>
        private static readonly bool isValueType;
        /// <summary>
        /// 创建随机对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static valueType CreateNull(random.config config)
        {
            if (defaultCreator != null) return defaultCreator();
            if (configNullCreator != null) return configNullCreator(config);
            if (constructor<valueType>.New == null) return default(valueType);
            if (isValueType)
            {
                valueType value = constructor<valueType>.New();
                MemberCreator(ref value, config);
                return value;
            }
            else
            {
                object historyValue = config.TryGetValue(typeof(valueType));
                if (historyValue != null) return (valueType)historyValue;
                if (fastCSharp.random.Default.NextBit() == 0) return default(valueType);
                valueType value = config.SaveHistory(constructor<valueType>.New());
                MemberCreator(ref value, config);
                return value;
            }
        }
        /// <summary>
        /// 创建随机对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static valueType CreateNotNull(random.config config)
        {
            if (defaultCreator != null) return defaultCreator();
            if (configNullCreator != null) return configNullCreator(config);
            if (constructor<valueType>.New == null) return default(valueType);
            if (isValueType)
            {
                valueType value = constructor<valueType>.New();
                MemberCreator(ref value, config);
                return value;
            }
            else
            {
                object historyValue = config.TryGetValue(typeof(valueType));
                if (historyValue != null) return (valueType)historyValue;
                valueType value = config.SaveHistory(constructor<valueType>.New());
                MemberCreator(ref value, config);
                return value;
            }
        }
        /// <summary>
        /// 创建随机对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static valueType Create(random.config config = null)
        {
            if (defaultCreator != null) return defaultCreator();
            if (configCreator != null)
            {
                if (config == null) config = random.DefaultConfig;
                if (config.History == null) return configCreator(config);
                try
                {
                    return configCreator(config);
                }
                finally { config.History.Clear(); }
            }
            if (constructor<valueType>.New == null) return default(valueType);
            valueType value = constructor<valueType>.New();
            if (config == null) config = random.DefaultConfig;
            if (config.History == null) MemberCreator(ref value, config);
            else
            {
                try
                {
                    MemberCreator(ref value, config);
                }
                finally { config.History.Clear(); }
            }
            return value;
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumByte()
        {
            return pub.enumCast<valueType, byte>.FromInt(random.CreateByte());
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumSByte()
        {
            return pub.enumCast<valueType, sbyte>.FromInt(random.CreateSByte());
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumShort()
        {
            return pub.enumCast<valueType, short>.FromInt(random.CreateShort());
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumUShort()
        {
            return pub.enumCast<valueType, ushort>.FromInt(random.CreateUShort());
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumInt()
        {
            return pub.enumCast<valueType, int>.FromInt(random.CreateInt());
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumUInt()
        {
            return pub.enumCast<valueType, uint>.FromInt(random.CreateUInt());
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumLong()
        {
            return pub.enumCast<valueType, long>.FromInt(random.CreateLong());
        }
        /// <summary>
        /// 创建随机枚举值
        /// </summary>
        /// <returns></returns>
        private static valueType enumULong()
        {
            return pub.enumCast<valueType, ulong>.FromInt(random.CreateULong());
        }
        static random()
        {
            Type type = typeof(valueType);
            MethodInfo method = random.GetMethod(type);
            if (method != null)
            {
                defaultCreator = (Func<valueType>)Delegate.CreateDelegate(typeof(Func<valueType>), method);
                return;
            }
            if ((method = random.GetConfigMethod(type)) != null)
            {
                configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), method);
                configNullCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.GetConfigNullMethod(type));
                return;
            }
            if (type.IsArray)
            {
                if (type.GetArrayRank() == 1)
                {
                    configNullCreator = configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateArrayMethod.MakeGenericMethod(type.GetElementType()));
                }
                return;
            }
            if (type.IsEnum)
            {
                Type enumType = System.Enum.GetUnderlyingType(type);
                if (enumType == typeof(uint)) defaultCreator = enumUInt;
                else if (enumType == typeof(byte)) defaultCreator = enumByte;
                else if (enumType == typeof(ulong)) defaultCreator = enumULong;
                else if (enumType == typeof(ushort)) defaultCreator = enumUShort;
                else if (enumType == typeof(long)) defaultCreator = enumLong;
                else if (enumType == typeof(short)) defaultCreator = enumShort;
                else if (enumType == typeof(sbyte)) defaultCreator = enumSByte;
                else defaultCreator = enumInt;
                return;
            }
            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(Nullable<>))
                {
                    configNullCreator = configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateNullableMethod.MakeGenericMethod(type.GetGenericArguments()));
                    return;
                }
                if (genericType == typeof(subArray<>))
                {
                    configNullCreator = configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateSubArrayMethod.MakeGenericMethod(type.GetGenericArguments()));
                    return;
                }
                if (genericType == typeof(list<>))
                {
                    Type[] parameterTypes = type.GetGenericArguments();
                    configNullCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateListNullMethod.MakeGenericMethod(parameterTypes));
                    configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateListMethod.MakeGenericMethod(parameterTypes));
                    return;
                }
                if (genericType == typeof(List<>))
                {
                    Type[] parameterTypes = type.GetGenericArguments();
                    configNullCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateSystemListNullMethod.MakeGenericMethod(parameterTypes));
                    configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateSystemListMethod.MakeGenericMethod(parameterTypes));
                    return;
                }
                if (genericType == typeof(HashSet<>) || genericType == typeof(Queue<>) || genericType == typeof(Stack<>) || genericType == typeof(SortedSet<>) || genericType == typeof(LinkedList<>))
                {
                    Type[] parameterTypes = new Type[] { type, type.GetGenericArguments()[0] };
                    configNullCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateEnumerableConstructorNullMethod.MakeGenericMethod(parameterTypes));
                    configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateEnumerableConstructorMethod.MakeGenericMethod(parameterTypes));
                    return;
                }
                if (genericType == typeof(Dictionary<,>))
                {
                    Type[] parameterTypes = type.GetGenericArguments();
                    configNullCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateDictionaryNullMethod.MakeGenericMethod(parameterTypes));
                    configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateDictionaryMethod.MakeGenericMethod(parameterTypes));
                    return;
                }
                if (genericType == typeof(SortedDictionary<,>))
                {
                    Type[] parameterTypes = type.GetGenericArguments();
                    configNullCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateSortedDictionaryNullMethod.MakeGenericMethod(parameterTypes));
                    configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateSortedDictionaryMethod.MakeGenericMethod(parameterTypes));
                    return;
                }
                if (genericType == typeof(SortedList<,>))
                {
                    Type[] parameterTypes = type.GetGenericArguments();
                    configNullCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateSortedListNullMethod.MakeGenericMethod(parameterTypes));
                    configCreator = (Func<random.config, valueType>)Delegate.CreateDelegate(typeof(Func<random.config, valueType>), random.CreateSortedListMethod.MakeGenericMethod(parameterTypes));
                    return;
                }
            }
            if (type.IsPointer || type.IsInterface) return;
            isValueType = type.IsValueType;
#if NOJIT
            MemberCreator = new memberRandom().Random();
#else
            random.memberDynamicMethod dynamicMethod = new random.memberDynamicMethod(type);
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))// | BindingFlags.DeclaredOnly
            {
                dynamicMethod.Push(field);
            }
            dynamicMethod.Base();
            MemberCreator = (creator)dynamicMethod.Create<creator>();
#endif
        }
#if NOJIT
        /// <summary>
        /// 随机对象生成
        /// </summary>
        private sealed class memberRandom
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
                /// 创建数据函数信息
                /// </summary>
                public MethodInfo CreateMethod;
                /// <summary>
                /// 设置字段信息
                /// </summary>
                /// <param name="field"></param>
                public void Set(FieldInfo field)
                {
                    Field = field;
                    CreateMethod = fastCSharp.emit.random.CreateMethod.MakeGenericMethod(field.FieldType);
                }
            }
            /// <summary>
            /// 字段集合
            /// </summary>
            private field[] fields;
            /// <summary>
            /// 基类调用
            /// </summary>
            private MethodInfo baseMethod;
            /// <summary>
            /// 随机对象生成
            /// </summary>
            public memberRandom()
            {
                Type type = typeof(valueType);
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                this.fields = new field[fields.Length];
                int index = 0;
                foreach (FieldInfo field in fields) this.fields[index++].Set(field);
                if (!isValueType && (type = type.BaseType) != typeof(object)) baseMethod = fastCSharp.emit.random.CreateMemberMethod.MakeGenericMethod(type);
            }
            /// <summary>
            /// 随机对象生成
            /// </summary>
            /// <returns></returns>
            public creator Random()
            {
                return isValueType ? (creator)randomValue : random;
            }
            /// <summary>
            /// 随机对象生成
            /// </summary>
            /// <param name="value"></param>
            /// <param name="config"></param>
            private void random(ref valueType value, random.config config)
            {
                object[] parameters = new object[] { config };
                foreach (field field in fields) field.Field.SetValue(value, field.CreateMethod.Invoke(null, parameters));
                if (baseMethod != null) baseMethod.Invoke(null, new object[] { value, config });
            }
            /// <summary>
            /// 随机对象生成
            /// </summary>
            /// <param name="value"></param>
            /// <param name="config"></param>
            private void randomValue(ref valueType value, random.config config)
            {
                object[] parameters = new object[] { config };
                object objectValue = value;
                foreach (field field in fields) field.Field.SetValue(objectValue, field.CreateMethod.Invoke(null, parameters));
                value = (valueType)objectValue;
            }
        }
#endif
    }
}
