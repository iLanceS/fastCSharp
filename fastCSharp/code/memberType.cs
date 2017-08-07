using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Threading;
using fastCSharp.reflection;
using fastCSharp.threading;
using System.IO;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员类型
    /// </summary>
    public sealed partial class memberType
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; private set; }
        ///// <summary>
        ///// SQL类型
        ///// </summary>
        //private Type sqlType;
        /// <summary>
        /// 自定义类型名称
        /// </summary>
        private string name;
        /// <summary>
        /// 类型名称
        /// </summary>
        private string typeName;
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName
        {
            get
            {
                if (typeName == null) typeName = name == null ? (Type != null ? Type.name() : null) : name;
                return typeName;
            }
        }
        /// <summary>
        /// 类型名称
        /// </summary>
        private string typeOnlyName;
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeOnlyName
        {
            get
            {
                if (typeOnlyName == null) typeOnlyName = name == null ? (Type != null ? Type.onlyName() : null) : name;
                return typeOnlyName;
            }
        }
        /// <summary>
        /// 类型全名
        /// </summary>
        private string fullName;
        /// <summary>
        /// 类型全名
        /// </summary>
        public string FullName
        {
            get
            {
                if (fullName == null) fullName = Type != null ? Type.fullName() : TypeName;
                return fullName;
            }
        }
        /// <summary>
        /// XML文档注释
        /// </summary>
        private string xmlDocument;
        /// <summary>
        /// XML文档注释
        /// </summary>
        public string XmlDocument
        {
            get
            {
                if (xmlDocument == null)
                {
                    xmlDocument = Type == null ? string.Empty : code.xmlDocument.Get(Type);
                }
                return xmlDocument.Length == 0 ? null : xmlDocument;
            }
        }
        /// <summary>
        /// 是否引用类型
        /// </summary>
        private bool? isNull;
        /// <summary>
        /// 是否引用类型
        /// </summary>
        public bool IsNull
        {
            get { return isNull == null ? Type == null || Type.isNull() : (bool)isNull; }
        }
        /// <summary>
        /// 是否object
        /// </summary>
        internal bool IsObject
        {
            get { return Type == typeof(object); }
        }
        /// <summary>
        /// 是否字符串
        /// </summary>
        public bool IsString
        {
            get { return Type == typeof(string); }
        }
        /// <summary>
        /// 是否字符串
        /// </summary>
        public bool IsSubString
        {
            get { return Type == typeof(subString); }
        }
        /// <summary>
        /// 是否字符类型(包括可空类型)
        /// </summary>
        public bool IsChar
        {
            get { return Type == typeof(char) || Type == typeof(char?); }
        }
        /// <summary>
        /// 是否逻辑类型(包括可空类型)
        /// </summary>
        public bool IsBool
        {
            get { return Type == typeof(bool) || Type == typeof(bool?); }
        }
        /// <summary>
        /// 是否时间类型(包括可空类型)
        /// </summary>
        public bool IsDateTime
        {
            get { return Type == typeof(DateTime) || Type == typeof(DateTime?); }
        }
        /// <summary>
        /// 数字类型集合
        /// </summary>
        private static readonly HashSet<Type> numberTypes = new HashSet<Type>(new Type[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(char) });
        /// <summary>
        /// 是否数字类型
        /// </summary>
        public bool IsNumber
        {
            get
            {
                return numberTypes.Contains(Type);
            }
        }
        /// <summary>
        /// 是否数字类型(包括可空类型)
        /// </summary>
        public bool IsDecimal
        {
            get { return Type == typeof(decimal) || Type == typeof(decimal?); }
        }
        /// <summary>
        /// 是否Guid类型(包括可空类型)
        /// </summary>
        public bool IsGuid
        {
            get { return Type == typeof(Guid) || Type == typeof(Guid?); }
        }
        /// <summary>
        /// 是否字节数组
        /// </summary>
        public bool IsByteArray
        {
            get { return Type == typeof(byte[]); }
        }
        /// <summary>
        /// 是否值类型(排除可空类型)
        /// </summary>
        public bool IsStruct
        {
            get { return Type.isStruct() && Type.nullableType() == null; }
        }
        /// <summary>
        /// 是否数组或者接口
        /// </summary>
        public bool IsArrayOrInterface
        {
            get
            {
                return Type.IsArray || Type.IsInterface;
            }
        }
        /// <summary>
        /// 是否字典
        /// </summary>
        public bool IsDictionary
        {
            get
            {
                return Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
            }
        }
        /// <summary>
        /// 是否流
        /// </summary>
        public bool IsStream
        {
            get { return Type == typeof(Stream); }
        }
        /// <summary>
        /// 是否泛型等于比较
        /// </summary>
        public bool IsIEquatable
        {
            get
            {
                foreach (Type type in Type.GetInterfaces())
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEquatable<>) && type.GetGenericArguments()[0] == Type) return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 成员类型
        /// </summary>
        /// <param name="name">类型名称</param>
        /// <param name="isNull">是否引用类型</param>
        public memberType(string name, bool isNull)
        {
            this.name = name;
            this.isNull = isNull;
        }
        /// <summary>
        /// 成员类型
        /// </summary>
        /// <param name="type">类型</param>
        private memberType(Type type)
        {
            this.Type = type;
        }
        ///// <summary>
        ///// 成员类型
        ///// </summary>
        ///// <param name="type">类型</param>
        ///// <param name="sqlType">SQL类型</param>
        //internal memberType(Type type, Type sqlType)
        //    : this(type)
        //{
        //    this.sqlType = sqlType;
        //}
        /// <summary>
        /// 空类型
        /// </summary>
        internal static readonly memberType Null = new memberType((Type)null);
        /// <summary>
        /// 成员类型隐式转换集合
        /// </summary>
        private static readonly Dictionary<Type, memberType> types = dictionary.CreateOnly<Type, memberType>();
        /// <summary>
        /// 隐式转换集合转换锁
        /// </summary>
        private static readonly object typeLock = new object();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value">成员类型</param>
        /// <returns>类型</returns>
        public static implicit operator Type(memberType value)
        {
            return value != null ? value.Type : null;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>成员类型</returns>
        public static implicit operator memberType(Type type)
        {
            if (type == null) return Null;
            memberType value;
            Monitor.Enter(typeLock);
            try
            {
                if (!types.TryGetValue(type, out value)) types.Add(type, value = new memberType(type));
            }
            finally { Monitor.Exit(typeLock); }
            return value;
        }
        /// <summary>
        /// 数组构造信息
        /// </summary>
        internal ConstructorInfo ArrayConstructor { get; private set; }
        /// <summary>
        /// 列表数组构造信息
        /// </summary>
        internal ConstructorInfo IListConstructor { get; private set; }
        /// <summary>
        /// 集合构造信息
        /// </summary>
        internal ConstructorInfo ICollectionConstructor { get; private set; }
        /// <summary>
        /// 可枚举泛型构造信息
        /// </summary>
        internal ConstructorInfo IEnumerableConstructor { get; private set; }
        /// <summary>
        /// 枚举基类类型
        /// </summary>
        public memberType EnumUnderlyingType
        {
            get { return Type.GetEnumUnderlyingType(); }
        }
        /// <summary>
        /// 可枚举泛型类型
        /// </summary>
        private memberType enumerableType;
        /// <summary>
        /// 可枚举泛型类型
        /// </summary>
        public memberType EnumerableType
        {
            get
            {
                if (enumerableType == null)
                {
                    if (!IsString)
                    {
                        Type value = Type.getGenericInterface(typeof(IEnumerable<>));
                        if (value != null)
                        {
                            if (Type.IsInterface)
                            {
                                Type interfaceType = Type.GetGenericTypeDefinition();
                                if (interfaceType == typeof(IEnumerable<>) || interfaceType == typeof(ICollection<>)
                                    || interfaceType == typeof(IList<>))
                                {
                                    enumerableArgumentType = value.GetGenericArguments()[0];
                                    enumerableType = value;
                                }
                            }
                            else if (Type.IsArray)
                            {
                                enumerableArgumentType = value.GetGenericArguments()[0];
                                enumerableType = value;
                            }
                            else
                            {
                                Type enumerableArgumentType = value.GetGenericArguments()[0];
                                Type[] parameters = new Type[1];
                                parameters[0] = enumerableArgumentType.MakeArrayType();
                                ArrayConstructor = Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                                if (ArrayConstructor != null)
                                {
                                    this.enumerableArgumentType = enumerableArgumentType;
                                    enumerableType = value;
                                }
                                else
                                {
                                    parameters[0] = typeof(IList<>).MakeGenericType(enumerableArgumentType);
                                    IListConstructor = Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                                    if (IListConstructor != null)
                                    {
                                        this.enumerableArgumentType = enumerableArgumentType;
                                        enumerableType = value;
                                    }
                                    else
                                    {
                                        parameters[0] = typeof(ICollection<>).MakeGenericType(enumerableArgumentType);
                                        ICollectionConstructor = Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                                        if (ICollectionConstructor != null)
                                        {
                                            this.enumerableArgumentType = enumerableArgumentType;
                                            enumerableType = value;
                                        }
                                        else
                                        {
                                            parameters[0] = typeof(IEnumerable<>).MakeGenericType(enumerableArgumentType);
                                            IEnumerableConstructor = Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                                            if (IEnumerableConstructor != null)
                                            {
                                                this.enumerableArgumentType = enumerableArgumentType;
                                                enumerableType = value;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (enumerableType == null) enumerableType = Null;
                }
                return enumerableType.Type != null ? enumerableType : null;
            }
        }
        /// <summary>
        /// 是否可枚举类型
        /// </summary>
        public bool IsEnumerable
        {
            get
            {
                return EnumerableType != null;
            }
        }
        /// <summary>
        /// 可枚举泛型参数类型
        /// </summary>
        private memberType enumerableArgumentType;
        /// <summary>
        /// 可枚举泛型参数类型
        /// </summary>
        public memberType EnumerableArgumentType
        {
            get
            {
                return EnumerableType != null ? enumerableArgumentType : null;
            }
        }
        /// <summary>
        /// 可控类型的值类型
        /// </summary>
        private memberType nullType;
        /// <summary>
        /// 可控类型的值类型
        /// </summary>
        public memberType NullType
        {
            get
            {
                if (nullType == null) nullType = (memberType)Type.nullableType();
                return nullType.Type != null ? nullType : null;
            }
        }
        /// <summary>
        /// 非可控类型为null
        /// </summary>
        public memberType NotNullType
        {
            get { return NullType != null ? nullType : this; }
        }
        /// <summary>
        /// 非可控类型为null
        /// </summary>
        public string StructNotNullType
        {
            get
            {
                if (NotNullType.Type.IsEnum) return NotNullType.Type.GetEnumUnderlyingType().fullName();
                return NotNullType.FullName;
            }
        }
        ///// <summary>
        ///// 结构体非可空类型
        ///// </summary>
        //private string structType;
        ///// <summary>
        ///// 结构体非可空类型
        ///// </summary>
        //public string StructType
        //{
        //    get
        //    {
        //        if (structType == null)
        //        {
        //            Type type = Type.nullableType();
        //            structType = type == null ? fullName : type.fullName();
        //        }
        //        return structType;
        //    }
        //}
        /// <summary>
        /// 是否拥有静态转换函数
        /// </summary>
        private bool? isTryParse;
        /// <summary>
        /// 是否拥有静态转换函数
        /// </summary>
        internal bool IsTryParse
        {
            get
            {
                if (isTryParse == null) isTryParse = (Type.nullableType() ?? Type).getTryParse() != null;
                return (bool)isTryParse;
            }
        }
        /// <summary>
        /// 键值对键类型
        /// </summary>
        internal memberType pairKeyType;
        /// <summary>
        /// 键值对键类型
        /// </summary>
        public memberType PairKeyType
        {
            get
            {
                if (pairKeyType == null)
                {
                    if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    {
                        pairKeyType = Type.GetGenericArguments()[0];
                    }
                    else pairKeyType = Null;
                }
                return pairKeyType.Type != null ? pairKeyType : null;
            }
        }
        /// <summary>
        /// 键值对值类型
        /// </summary>
        internal memberType pairValueType;
        /// <summary>
        /// 键值对值类型
        /// </summary>
        public memberType PairValueType
        {
            get
            {
                if (pairValueType == null)
                {
                    if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    {
                        pairValueType = Type.GetGenericArguments()[1];
                    }
                    else pairValueType = Null;
                }
                return pairValueType.Type != null ? pairValueType : null;
            }
        }
        /// <summary>
        /// 键值对键类型
        /// </summary>
        internal memberType keyValueKeyType;
        /// <summary>
        /// 键值对键类型
        /// </summary>
        public memberType KeyValueKeyType
        {
            get
            {
                if (keyValueKeyType == null)
                {
                    if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(keyValue<,>))
                    {
                        keyValueKeyType = Type.GetGenericArguments()[0];
                    }
                    else keyValueKeyType = Null;
                }
                return keyValueKeyType.Type != null ? keyValueKeyType : null;
            }
        }
        /// <summary>
        /// 键值对键类型
        /// </summary>
        internal memberType keyValueValueType;
        /// <summary>
        /// 键值对键类型
        /// </summary>
        public memberType KeyValueValueType
        {
            get
            {
                if (keyValueValueType == null)
                {
                    if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(keyValue<,>))
                    {
                        keyValueValueType = Type.GetGenericArguments()[1];
                    }
                    else keyValueValueType = Null;
                }
                return keyValueValueType.Type != null ? keyValueValueType : null;
            }
        }
        /// <summary>
        /// 泛型参数集合
        /// </summary>
        private memberType[] genericParameters;
        /// <summary>
        /// 泛型参数集合
        /// </summary>
        internal memberType[] GenericParameters
        {
            get
            {
                if (genericParameters == null)
                {
                    genericParameters = Type.IsGenericType ? Type.GetGenericArguments().getArray(value => (memberType)value) : nullValue<memberType>.Array;
                }
                return genericParameters;
            }
        }
        /// <summary>
        /// 泛型参数名称
        /// </summary>
        public string GenericParameterNames
        {
            get
            {
                return GenericParameters.joinString(',', value => value.FullName);
            }
        }
    }
}
