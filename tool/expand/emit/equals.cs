using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.threading;
using System.Collections;
using fastCSharp.code;
#if NOJIT
#else
using System.Reflection.Emit;
#endif

namespace fastCSharp.emit
{
    /// <summary>
    /// 对象对比
    /// </summary>
    internal static class equals
    {
#if NOJIT
        /// <summary>
        /// 类型比较字段与委托调用集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, Func<object, object, bool>> objectEquals = new interlocked.dictionary<Type, Func<object, object, bool>>();
        /// <summary>
        /// 获取类型比较字段与委托调用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<object, object, bool> GetObjectEquals(Type type)
        {
            Func<object, object, bool> method;
            if (objectEquals.TryGetValue(type, out method)) return method;
            objectEquals.Set(type, method = (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), typeof(equals<>).MakeGenericType(type).GetMethod("objectEquals", BindingFlags.Static | BindingFlags.NonPublic)));
            return method;
        }
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
            /// 
            /// </summary>
            private bool isMemberMap;
            /// <summary>
            /// 动态函数
            /// </summary>
            /// <param name="type"></param>
            /// <param name="isMemberMap"></param>
            public memberDynamicMethod(Type type, bool isMemberMap)
            {
                this.type = type;
                if (this.isMemberMap = isMemberMap) dynamicMethod = new DynamicMethod("memberMapEquals", typeof(bool), new Type[] { type, type, typeof(memberMap) }, type, true);
                else dynamicMethod = new DynamicMethod("equals", typeof(bool), new Type[] { type, type }, type, true);
                generator = dynamicMethod.GetILGenerator();
                if (!(isValueType = type.IsValueType))
                {
                    Label next = generator.DefineLabel();
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, pubExtension.ReferenceEqualsMethod);
                    generator.Emit(OpCodes.Brfalse_S, next);
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Ret);
                    generator.MarkLabel(next);
                }
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            public void Push(FieldInfo field)
            {
                keyValue<FieldInfo, MethodInfo> fieldInvoke = getEqualsFieldInvoke(field.FieldType);
                Label next = generator.DefineLabel();
                generator.Emit(OpCodes.Ldsfld, fieldInvoke.Key);
                if (isValueType) generator.Emit(OpCodes.Ldarga_S, 0);
                else generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                if (isValueType) generator.Emit(OpCodes.Ldarga_S, 1);
                else generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Callvirt, fieldInvoke.Value);
                generator.Emit(OpCodes.Brtrue_S, next);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ret);
                generator.MarkLabel(next);
            }
            /// <summary>
            /// 添加字段
            /// </summary>
            /// <param name="field">字段信息</param>
            /// <param name="memberIndex">字段信息</param>
            public void Push(FieldInfo field, int memberIndex)
            {
                Label next = generator.DefineLabel();
                generator.memberMapIsMember(OpCodes.Ldarg_2, memberIndex);
                generator.Emit(OpCodes.Brfalse_S, next);
                Push(field);
                generator.MarkLabel(next);
            }
            /// <summary>
            /// 基类调用
            /// </summary>
            public void Base()
            {
                if (!isValueType && (type = type.BaseType) != typeof(object))
                {
                    keyValue<FieldInfo, MethodInfo> fieldInvoke = getEqualsFieldInvoke(type);
                    generator.Emit(OpCodes.Ldsfld, fieldInvoke.Key);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Callvirt, fieldInvoke.Value);
                }
                else generator.Emit(OpCodes.Ldc_I4_1);
                generator.Emit(OpCodes.Ret);
            }
            /// <summary>
            /// 创建委托
            /// </summary>
            /// <returns>委托</returns>
            public Delegate Create<delegateType>()
            {
                if (isMemberMap)
                {
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Ret);
                }
                return dynamicMethod.CreateDelegate(typeof(delegateType));
            }
        }
        /// <summary>
        /// 类型比较字段与委托调用集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, keyValue<FieldInfo, MethodInfo>> equalsFieldInvokes = new interlocked.dictionary<Type, keyValue<FieldInfo, MethodInfo>>();
        /// <summary>
        /// 获取类型比较字段与委托调用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static keyValue<FieldInfo, MethodInfo> getEqualsFieldInvoke(Type type)
        {
            keyValue<FieldInfo, MethodInfo> fieldInvoke;
            if (equalsFieldInvokes.TryGetValue(type, out fieldInvoke)) return fieldInvoke;
            fieldInvoke.Key = typeof(equals<>).MakeGenericType(type).GetField("Equals", BindingFlags.Static | BindingFlags.Public);
            fieldInvoke.Value = typeof(Func<,,>).MakeGenericType(type, type, typeof(bool)).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { type, type }, null);
            equalsFieldInvokes.Set(type, fieldInvoke);
            return fieldInvoke;
        }
#endif
        /// <summary>
        /// 可空数据比较
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool nullable<valueType>(Nullable<valueType> left, Nullable<valueType> right) where valueType : struct
        {
            if (left.HasValue) return right.HasValue && equals<valueType>.Equals(left.Value, right.Value);
            return !right.HasValue;
        }
        /// <summary>
        /// 可空数据比较函数信息
        /// </summary>
        public static readonly MethodInfo NullableMethod = typeof(equals).GetMethod("nullable", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 结构体数据比较
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool structIEquatable<valueType>(valueType left, valueType right) where valueType : struct, IEquatable<valueType>
        {
            return left.Equals(right);
        }
        /// <summary>
        /// 结构体数据比较函数信息
        /// </summary>
        public static readonly MethodInfo StructIEquatableMethod = typeof(equals).GetMethod("structIEquatable", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 引用对象比较
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool classIEquatable<valueType>(valueType left, valueType right) where valueType : class, IEquatable<valueType>
        {
            if (Object.ReferenceEquals(left, right)) return true;
            return left != null && right != null && left.Equals(right);
        }
        /// <summary>
        /// 引用对象比较函数信息
        /// </summary>
        public static readonly MethodInfo ClassIEquatableMethod = typeof(equals).GetMethod("classIEquatable", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 数组比较
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        private static bool array<valueType>(valueType[] leftArray, valueType[] rightArray)
        {
            if (Object.ReferenceEquals(leftArray, rightArray)) return true;
            if (leftArray != null && rightArray != null && leftArray.Length == rightArray.Length)
            {
                int index = 0;
                foreach (valueType left in leftArray)
                {
                    if (!equals<valueType>.Equals(left, rightArray[index++])) return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 数组比较函数信息
        /// </summary>
        public static readonly MethodInfo ArrayMethod = typeof(equals).GetMethod("array", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 数组比较
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        private static bool subArray<valueType>(subArray<valueType> leftArray, subArray<valueType> rightArray)
        {
            if (leftArray.Count == rightArray.Count)
            {
                for (int index = leftArray.Count; index != 0; )
                {
                    --index;
                    if (!equals<valueType>.Equals(leftArray[index], rightArray[index])) return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 数组比较函数信息
        /// </summary>
        public static readonly MethodInfo SubArrayMethod = typeof(equals).GetMethod("subArray", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 数组比较
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        private static bool list<valueType>(list<valueType> leftArray, list<valueType> rightArray)
        {
            if (Object.ReferenceEquals(leftArray, rightArray)) return true;
            if (leftArray != null && rightArray != null && leftArray.Count == rightArray.Count)
            {
                for (int index = leftArray.Count; index != 0; )
                {
                    --index;
                    if (!equals<valueType>.Equals(leftArray[index], rightArray[index])) return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 数组比较函数信息
        /// </summary>
        public static readonly MethodInfo ListMethod = typeof(equals).GetMethod("list", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 数组比较
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <typeparam name="argumentType"></typeparam>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        private static bool collection<valueType, argumentType>(valueType leftArray, valueType rightArray) where valueType : IEnumerable<argumentType>, ICollection
        {
            if (Object.ReferenceEquals(leftArray, rightArray)) return true;
            if (leftArray != null && rightArray != null && leftArray.Count == rightArray.Count)
            {
                IEnumerator<argumentType> right = rightArray.GetEnumerator();
                foreach (argumentType left in leftArray)
                {
                    if (!right.MoveNext() || !equals<argumentType>.Equals(left, right.Current)) return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 数组比较函数信息
        /// </summary>
        public static readonly MethodInfo CollectionMethod = typeof(equals).GetMethod("collection", BindingFlags.Static | BindingFlags.NonPublic);
        /// <summary>
        /// 字典比较
        /// </summary>
        /// <typeparam name="dictionaryType"></typeparam>
        /// <typeparam name="keyType"></typeparam>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        private static bool dictionary<dictionaryType, keyType, valueType>(dictionaryType leftArray, dictionaryType rightArray) where dictionaryType : IDictionary<keyType, valueType>
        {
            if (Object.ReferenceEquals(leftArray, rightArray)) return true;
            if (leftArray != null && rightArray != null && leftArray.Count == rightArray.Count)
            {
                foreach (KeyValuePair<keyType, valueType> left in leftArray)
                {
                    valueType right;
                    if (!rightArray.TryGetValue(left.Key, out right) || !equals<valueType>.Equals(left.Value, right))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 字典比较函数信息
        /// </summary>
        public static readonly MethodInfo DictionaryMethod = typeof(equals).GetMethod("dictionary", BindingFlags.Static | BindingFlags.NonPublic);
    }
    /// <summary>
    /// 对象对比
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    public static class equals<valueType>
    {
        /// <summary>
        /// 对象对比委托
        /// </summary>
        public static new readonly Func<valueType, valueType, bool> Equals;
        /// <summary>
        /// 对象对比委托
        /// </summary>
        public static readonly Func<valueType, valueType, memberMap, bool> MemberMapEquals;
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumByte(valueType left, valueType right)
        {
            return pub.enumCast<valueType, byte>.ToInt(left) == pub.enumCast<valueType, byte>.ToInt(right);
        }
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumSByte(valueType left, valueType right)
        {
            return pub.enumCast<valueType, sbyte>.ToInt(left) == pub.enumCast<valueType, sbyte>.ToInt(right);
        }
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumUShort(valueType left, valueType right)
        {
            return pub.enumCast<valueType, ushort>.ToInt(left) == pub.enumCast<valueType, ushort>.ToInt(right);
        }
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumShort(valueType left, valueType right)
        {
            return pub.enumCast<valueType, short>.ToInt(left) == pub.enumCast<valueType, short>.ToInt(right);
        }
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumUInt(valueType left, valueType right)
        {
            return pub.enumCast<valueType, uint>.ToInt(left) == pub.enumCast<valueType, uint>.ToInt(right);
        }
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumInt(valueType left, valueType right)
        {
            return pub.enumCast<valueType, int>.ToInt(left) == pub.enumCast<valueType, int>.ToInt(right);
        }
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumULong(valueType left, valueType right)
        {
            return pub.enumCast<valueType, ulong>.ToInt(left) == pub.enumCast<valueType, ulong>.ToInt(right);
        }
        /// <summary>
        /// 枚举值比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool enumLong(valueType left, valueType right)
        {
            return pub.enumCast<valueType, long>.ToInt(left) == pub.enumCast<valueType, long>.ToInt(right);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool unknown(valueType left, valueType right)
        {
            return false;
        }
        static equals()
        {
            Type type = typeof(valueType);
            if (typeof(IEquatable<valueType>).IsAssignableFrom(type))
            {
                Equals = (Func<valueType, valueType, bool>)Delegate.CreateDelegate(typeof(Func<valueType, valueType, bool>), (type.IsValueType ? equals.StructIEquatableMethod : equals.ClassIEquatableMethod).MakeGenericMethod(type));
                return;
            }
            if (type.IsArray)
            {
                if (type.GetArrayRank() == 1)
                {
                    Equals = (Func<valueType, valueType, bool>)Delegate.CreateDelegate(typeof(Func<valueType, valueType, bool>), equals.ArrayMethod.MakeGenericMethod(type.GetElementType()));
                }
                else Equals = unknown;
                return;
            }
            if (type.IsEnum)
            {
                Type enumType = System.Enum.GetUnderlyingType(type);
                if (enumType == typeof(uint)) Equals = enumUInt;
                else if (enumType == typeof(byte)) Equals = enumByte;
                else if (enumType == typeof(ulong)) Equals = enumULong;
                else if (enumType == typeof(ushort)) Equals = enumUShort;
                else if (enumType == typeof(long)) Equals = enumLong;
                else if (enumType == typeof(short)) Equals = enumShort;
                else if (enumType == typeof(sbyte)) Equals = enumSByte;
                else Equals = enumInt;
                return;
            }
            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(Nullable<>))
                {
                    Equals = (Func<valueType, valueType, bool>)Delegate.CreateDelegate(typeof(Func<valueType, valueType, bool>), equals.NullableMethod.MakeGenericMethod(type.GetGenericArguments()));
                    return;
                }
                if (genericType == typeof(subArray<>))
                {
                    Equals = (Func<valueType, valueType, bool>)Delegate.CreateDelegate(typeof(Func<valueType, valueType, bool>), equals.SubArrayMethod.MakeGenericMethod(type.GetGenericArguments()));
                    return;
                }
                if (genericType == typeof(list<>))
                {
                    Equals = (Func<valueType, valueType, bool>)Delegate.CreateDelegate(typeof(Func<valueType, valueType, bool>), equals.ListMethod.MakeGenericMethod(type.GetGenericArguments()));
                    return;
                }
                if (genericType == typeof(List<>) || genericType == typeof(HashSet<>) || genericType == typeof(Queue<>) || genericType == typeof(Stack<>) || genericType == typeof(SortedSet<>) || genericType == typeof(LinkedList<>))
                {
                    Equals = (Func<valueType, valueType, bool>)Delegate.CreateDelegate(typeof(Func<valueType, valueType, bool>), equals.CollectionMethod.MakeGenericMethod(type, type.GetGenericArguments()[0]));
                    return;
                }
                if (genericType == typeof(Dictionary<,>) || genericType == typeof(SortedDictionary<,>) || genericType == typeof(SortedList<,>))
                {
                    Type[] parameterTypes = type.GetGenericArguments();
                    Equals = (Func<valueType, valueType, bool>)Delegate.CreateDelegate(typeof(Func<valueType, valueType, bool>), equals.DictionaryMethod.MakeGenericMethod(type, parameterTypes[0], parameterTypes[1]));
                    return;
                }
            }
            if (type.IsPointer || type.IsInterface)
            {
                Equals = unknown;
                return;
            }
#if NOJIT
            Equals = new memberEquals(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)).GetEquals();
            MemberMapEquals = new memberMapEquals().GetEquals();
#else
            equals.memberDynamicMethod dynamicMethod = new equals.memberDynamicMethod(type, false);
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                dynamicMethod.Push(field);
            }
            dynamicMethod.Base();
            Equals = (Func<valueType, valueType, bool>)dynamicMethod.Create<Func<valueType, valueType, bool>>();

            dynamicMethod = new equals.memberDynamicMethod(type, true);
            foreach (keyValue<FieldInfo, int> field in pubExtension.GetFieldIndexs<valueType>(memberFilters.InstanceField))
            {
                dynamicMethod.Push(field.Key, field.Value);
            }
            MemberMapEquals = (Func<valueType, valueType, memberMap, bool>)dynamicMethod.Create<Func<valueType, valueType, memberMap, bool>>();
#endif
        }
#if NOJIT
        /// <summary>
        /// 对象对比
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool objectEquals(object left, object right)
        {
            return Equals((valueType)left, (valueType)right);
        }
        /// <summary>
        /// 对象对比
        /// </summary>
        private class memberEquals
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            protected struct field
            {
                /// <summary>
                /// 字段信息
                /// </summary>
                public FieldInfo Field;
                /// <summary>
                /// 对比函数信息
                /// </summary>
                public new Func<object, object, bool> Equals;
                /// <summary>
                /// 成员编号
                /// </summary>
                public int MemberIndex;
                /// <summary>
                /// 设置字段信息
                /// </summary>
                /// <param name="field"></param>
                /// <param name="memberIndex"></param>
                public void Set(FieldInfo field, int memberIndex)
                {
                    Field = field;
                    Equals = fastCSharp.emit.equals.GetObjectEquals(field.FieldType);
                    MemberIndex = memberIndex;
                }
            }
            /// <summary>
            /// 字段集合
            /// </summary>
            protected field[] fields;
            /// <summary>
            /// 基类调用
            /// </summary>
            private Func<object, object, bool> baseEquals;
            /// <summary>
            /// 是否值类型
            /// </summary>
            protected bool isValueType;
            /// <summary>
            /// 对象对比
            /// </summary>
            protected memberEquals() { }
            /// <summary>
            /// 对象对比
            /// </summary>
            /// <param name="fields"></param>
            public memberEquals(FieldInfo[] fields)
            {
                Type type = typeof(valueType);
                this.fields = new field[fields.Length];
                int index = 0;
                foreach (FieldInfo field in fields) this.fields[index++].Set(field, 0);
                if (!(isValueType = type.IsValueType) && (type = type.BaseType) != typeof(object)) baseEquals = fastCSharp.emit.equals.GetObjectEquals(type);
            }
            /// <summary>
            /// 对象对比
            /// </summary>
            /// <returns></returns>
            public Func<valueType, valueType, bool> GetEquals()
            {
                return isValueType ? (Func<valueType, valueType, bool>)equalsValue : equals;
            }
            /// <summary>
            /// 对象对比
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            private bool equals(valueType left, valueType right)
            {
                if (object.ReferenceEquals(left, right)) return true;
                foreach (field field in fields)
                {
                    if (!field.Equals(field.Field.GetValue(left), field.Field.GetValue(right))) return false;
                }
                return baseEquals == null || baseEquals(left,right);
            }
            /// <summary>
            /// 对象对比
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            private bool equalsValue(valueType left, valueType right)
            {
                object leftObject = left, rightObject = right;
                foreach (field field in fields)
                {
                    if (!field.Equals(field.Field.GetValue(leftObject), field.Field.GetValue(rightObject))) return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 对象对比
        /// </summary>
        private class memberMapEquals : memberEquals
        {
            /// <summary>
            /// 对象对比
            /// </summary>
            public memberMapEquals()
            {
                keyValue<FieldInfo, int>[] fields = pubExtension.GetFieldIndexs<valueType>(memberFilters.InstanceField);
                this.fields = new field[fields.Length];
                int index = 0;
                foreach (keyValue<FieldInfo, int> field in fields) this.fields[index++].Set(field.Key, field.Value);
                isValueType = typeof(valueType).IsValueType;
            }
            /// <summary>
            /// 对象对比
            /// </summary>
            /// <returns></returns>
            public new Func<valueType, valueType, memberMap, bool> GetEquals()
            {
                return isValueType ? (Func<valueType, valueType, memberMap, bool>)equalsValue : equals;
            }
            /// <summary>
            /// 对象对比
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <param name="memberMap"></param>
            /// <returns></returns>
            private bool equals(valueType left, valueType right, memberMap memberMap)
            {
                if (object.ReferenceEquals(left, right)) return true;
                foreach (field field in fields)
                {
                    if (memberMap.UnsafeIsMember(field.MemberIndex) && !field.Equals(field.Field.GetValue(left), field.Field.GetValue(right))) return false;
                }
                return true;
            }
            /// <summary>
            /// 对象对比
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <param name="memberMap"></param>
            /// <returns></returns>
            private bool equalsValue(valueType left, valueType right, memberMap memberMap)
            {
                object leftObject = left, rightObject = right;
                foreach (field field in fields)
                {
                    if (memberMap.UnsafeIsMember(field.MemberIndex) && !field.Equals(field.Field.GetValue(leftObject), field.Field.GetValue(rightObject))) return false;
                }
                return true;
            }
        }
#endif
    }
}
