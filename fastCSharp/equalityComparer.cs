#if __IOS__
using System;
using System.Collections.Generic;
using fastCSharp.reflection;

namespace fastCSharp
{
    /// <summary>
    /// 默认等于比较
    /// </summary>
    public static class equalityComparer
    {
        /// <summary>
        /// 
        /// </summary>
        private sealed class byteComparer : IEqualityComparer<byte>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(byte x, byte y) { return x == y; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(byte obj) { return obj; }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<byte> Byte;
        /// <summary>
        /// 
        /// </summary>
        private sealed class shortComparer : IEqualityComparer<short>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(short x, short y) { return x == y; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(short obj) { return obj; }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<short> Short;
        /// <summary>
        /// 
        /// </summary>
        private sealed class intComparer : IEqualityComparer<int>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(int x, int y) { return x == y; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(int obj) { return obj; }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<int> Int;
        /// <summary>
        /// 
        /// </summary>
        private sealed class ulongComparer : IEqualityComparer<ulong>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(ulong x, ulong y) { return x == y; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(ulong obj) { return (int)((uint)obj ^ (uint)(obj >> 32)); }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<ulong> ULong;
        /// <summary>
        /// 
        /// </summary>
        private sealed class charComparer : IEqualityComparer<char>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(char x, char y) { return x == y; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(char obj) { return obj; }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<char> Char;
        /// <summary>
        /// 
        /// </summary>
        private unsafe sealed class pointerComparer : IEqualityComparer<pointer>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(pointer x, pointer y) { return x.Data == y.Data; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(pointer obj) { return obj.GetHashCode(); }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<pointer> Pointer;
        /// <summary>
        /// 
        /// </summary>
        private unsafe sealed class subStringComparer : IEqualityComparer<subString>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(subString x, subString y) { return x.Equals(y); }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(subString obj) { return obj.GetHashCode(); }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<subString> SubString;
        /// <summary>
        /// 
        /// </summary>
        private unsafe sealed class hashBytesComparer : IEqualityComparer<hashBytes>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(hashBytes x, hashBytes y) { return x.Equals(y); }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(hashBytes obj) { return obj.GetHashCode(); }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<hashBytes> HashBytes;
        /// <summary>
        /// 
        /// </summary>
        private unsafe sealed class hashStringComparer : IEqualityComparer<hashString>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(hashString x, hashString y) { return x.Equals(y); }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(hashString obj) { return obj.GetHashCode(); }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<hashString> HashString;
        /// <summary>
        /// 
        /// </summary>
        private unsafe sealed class sessionIdComparer : IEqualityComparer<sessionId>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(sessionId x, sessionId y) { return x.Equals(ref y) == 0; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(sessionId obj) { return obj.GetHashCode(); }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        public static readonly IEqualityComparer<sessionId> SessionId;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        private sealed class equatable<valueType> : IEqualityComparer<valueType> where valueType : IEquatable<valueType>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(valueType x, valueType y)
            {
                return x == null ? y == null : x.Equals(y);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(valueType obj) { return obj != null ? obj.GetHashCode() : 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        private sealed class comparable<valueType> : IEqualityComparer<valueType> where valueType : IComparable<valueType>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(valueType x, valueType y)
            {
                return x == null ? y == null : x.CompareTo(y) == 0;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(valueType obj) { return obj != null ? obj.GetHashCode() : 0; }
        }
        /// <summary>
        /// 默认等于比较
        /// </summary>
        /// <typeparam name="valueType">比较值类型</typeparam>
        public static class comparer<valueType>
        {
            /// <summary>
            /// 默认等于比较
            /// </summary>
            public static readonly IEqualityComparer<valueType> Default;
            /// <summary>
            /// 未知类型等于比较
            /// </summary>
            private sealed class unknownComparer : IEqualityComparer<valueType>
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public bool Equals(valueType x, valueType y)
                {
                    return x == null ? y == null : x.Equals(y);
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public int GetHashCode(valueType obj) { return obj != null ? obj.GetHashCode() : 0; }
            }
            /// <summary>
            /// 未知类型等于比较
            /// </summary>
            private sealed class unknownNotNullComparer : IEqualityComparer<valueType>
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public bool Equals(valueType x, valueType y) { return x.Equals(y); }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public int GetHashCode(valueType obj) { return obj.GetHashCode(); }
            }
            static comparer()
            {
                Type type = typeof(valueType);
                object comparer;
                if (!comparers.TryGetValue(type, out comparer))
                {
                    if (typeof(IEquatable<valueType>).IsAssignableFrom(type))
                    {
                        Default = (IEqualityComparer<valueType>)Activator.CreateInstance(typeof(equatable<>).MakeGenericType(type));
                    }
                    else if (typeof(IComparable<valueType>).IsAssignableFrom(type))
                    {
                        Default = (IEqualityComparer<valueType>)Activator.CreateInstance(typeof(comparable<>).MakeGenericType(type));
                    }
                    else Default = type.isStruct() ? (IEqualityComparer<valueType>)new unknownNotNullComparer() : new unknownComparer();
                }
                else Default = (IEqualityComparer<valueType>)comparer;
            }
        }
        /// <summary>
        /// 等于比较集合
        /// </summary>
        private static readonly Dictionary<Type, object> comparers;
        static equalityComparer()
        {
            comparers = dictionary.CreateOnly<Type, object>();
            comparers.Add(typeof(byte), Byte = new byteComparer());
            comparers.Add(typeof(short), Short = new shortComparer());
            comparers.Add(typeof(int), Int = new intComparer());
            comparers.Add(typeof(char), Char = new charComparer());
            comparers.Add(typeof(ulong), ULong = new ulongComparer());
            comparers.Add(typeof(pointer), Pointer = new pointerComparer());
            comparers.Add(typeof(subString), SubString = new subStringComparer());
            comparers.Add(typeof(hashBytes), HashBytes = new hashBytesComparer());
            comparers.Add(typeof(hashString), HashString = new hashStringComparer());
            comparers.Add(typeof(sessionId), SessionId = new sessionIdComparer());
        }
    }
}
#endif