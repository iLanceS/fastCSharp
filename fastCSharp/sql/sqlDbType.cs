using System;
using System.Data;
using fastCSharp.reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql
{
    /// <summary>
    /// SQL数据类型相关操作
    /// </summary>
    internal unsafe static class sqlDbType
    {
        /// <summary>
        /// SQL数据类型转C#类型集合
        /// </summary>
        private static readonly Type[] toCSharpTypes;
        /// <summary>
        /// 根据SQL数据类型获取序列化类型
        /// </summary>
        /// <param name="type">SQL数据类型</param>
        /// <returns>序列化类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static Type toCSharpType(this SqlDbType type)
        {
            return toCSharpTypes.get((int)type, null);
        }
        /// <summary>
        /// 根据SQL数据类型获取序列化类型
        /// </summary>
        /// <param name="type">SQL数据类型</param>
        /// <param name="isNull">是否可空</param>
        /// <returns>序列化类型</returns>
        public static Type toCSharpType(this SqlDbType type, bool isNull)
        {
            Type value = toCSharpType(type);
            if (isNull)
            {
                Type nullType = value.toNullableType();
                if (nullType != null) value = nullType;
            }
            return value;
        }

        /// <summary>
        /// C#类型转SQL数据类型集合
        /// </summary>
        private static readonly Dictionary<Type, SqlDbType> formCSharpTypes;
        /// <summary>
        /// 根据C#类型获取SQL数据类型
        /// </summary>
        /// <param name="type">C#类型</param>
        /// <returns>SQL数据类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static SqlDbType formCSharpType(this Type type)
        {
            SqlDbType value;
            return formCSharpTypes.TryGetValue(fastCSharp.reflection.type.nullableType(type) ?? type, out value) ? value : SqlDbType.NVarChar;
        }
        /// <summary>
        /// 字符串类型占位集合
        /// </summary>
        private static readonly structReference<fixedMap.unsafer> stringTypeMap;
        /// <summary>
        /// 判断是否字符串类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>是否字符串类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool isStringType(this SqlDbType type)
        {
            return stringTypeMap.Value.Get((int)type);
        }
        /// <summary>
        /// 文本类型占位集合
        /// </summary>
        private static readonly structReference<fixedMap.unsafer> textImageTypeMap;
        /// <summary>
        /// 判断是否文本类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>是否文本类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool isTextImageType(this SqlDbType type)
        {
            return textImageTypeMap.Value.Get((int)type);
        }

        /// <summary>
        /// 未知数据长度
        /// </summary>
        public const int UnknownSize = -1;
        /// <summary>
        /// 类型默认长度
        /// </summary>
        private static readonly pointer.reference sizes;
        /// <summary>
        /// 获取数据长度
        /// </summary>
        /// <returns>数据长度</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int getSize(this SqlDbType type)
        {
            return sizes.Int[(int)type];
        }
        /// <summary>
        /// SQL数据类型最大枚举值
        /// </summary>
        private static readonly int MaxEnumValue;

        unsafe static sqlDbType()
        {
            #region SQL数据类型转C#类型集合
            toCSharpTypes = new Type[fastCSharp.Enum.GetMaxValue<SqlDbType>(-1) + 1];
            toCSharpTypes[(int)SqlDbType.BigInt] = typeof(long);
            toCSharpTypes[(int)SqlDbType.Binary] = typeof(byte[]);
            toCSharpTypes[(int)SqlDbType.Bit] = typeof(bool);
            toCSharpTypes[(int)SqlDbType.Char] = typeof(string);
            toCSharpTypes[(int)SqlDbType.DateTime] = typeof(DateTime);
            toCSharpTypes[(int)SqlDbType.Decimal] = typeof(decimal);
            toCSharpTypes[(int)SqlDbType.Float] = typeof(double);
            toCSharpTypes[(int)SqlDbType.Image] = typeof(byte[]);
            toCSharpTypes[(int)SqlDbType.Int] = typeof(int);
            toCSharpTypes[(int)SqlDbType.Money] = typeof(decimal);
            toCSharpTypes[(int)SqlDbType.NChar] = typeof(string);
            toCSharpTypes[(int)SqlDbType.NText] = typeof(string);
            toCSharpTypes[(int)SqlDbType.NVarChar] = typeof(string);
            toCSharpTypes[(int)SqlDbType.Real] = typeof(float);
            toCSharpTypes[(int)SqlDbType.UniqueIdentifier] = typeof(Guid);
            toCSharpTypes[(int)SqlDbType.SmallDateTime] = typeof(DateTime);
            toCSharpTypes[(int)SqlDbType.SmallInt] = typeof(short);
            toCSharpTypes[(int)SqlDbType.SmallMoney] = typeof(decimal);
            toCSharpTypes[(int)SqlDbType.Text] = typeof(string);
            toCSharpTypes[(int)SqlDbType.Timestamp] = typeof(byte[]);
            toCSharpTypes[(int)SqlDbType.TinyInt] = typeof(byte);
            toCSharpTypes[(int)SqlDbType.VarBinary] = typeof(byte[]);
            toCSharpTypes[(int)SqlDbType.VarChar] = typeof(string);
            toCSharpTypes[(int)SqlDbType.Variant] = typeof(object);
            //CSharpType[(int)SqlDbType.Xml] = typeof();
            //CSharpType[(int)SqlDbType.Udt] = typeof();
            //CSharpType[(int)SqlDbType.Structured] = typeof();
            //CSharpType[(int)SqlDbType.Date] = typeof();
            //CSharpType[(int)SqlDbType.Time] = typeof();
            //CSharpType[(int)SqlDbType.DateTime2] = typeof();
            //CSharpType[(int)SqlDbType.DateTimeOffset] = typeof();
            #endregion

            #region C#类型转SQL数据类型集合
            formCSharpTypes = dictionary.CreateOnly<Type, SqlDbType>();
            formCSharpTypes.Add(typeof(bool), SqlDbType.Bit);
            formCSharpTypes.Add(typeof(byte), SqlDbType.TinyInt);
            formCSharpTypes.Add(typeof(sbyte), SqlDbType.TinyInt);
            formCSharpTypes.Add(typeof(short), SqlDbType.SmallInt);
            formCSharpTypes.Add(typeof(ushort), SqlDbType.SmallInt);
            formCSharpTypes.Add(typeof(int), SqlDbType.Int);
            formCSharpTypes.Add(typeof(uint), SqlDbType.Int);
            formCSharpTypes.Add(typeof(long), SqlDbType.BigInt);
            formCSharpTypes.Add(typeof(ulong), SqlDbType.BigInt);
            formCSharpTypes.Add(typeof(decimal), SqlDbType.Decimal);
            formCSharpTypes.Add(typeof(float), SqlDbType.Real);
            formCSharpTypes.Add(typeof(double), SqlDbType.Float);
            formCSharpTypes.Add(typeof(string), SqlDbType.NVarChar);
            formCSharpTypes.Add(typeof(DateTime), SqlDbType.DateTime);
            formCSharpTypes.Add(typeof(Guid), SqlDbType.UniqueIdentifier);
            formCSharpTypes.Add(typeof(byte[]), SqlDbType.VarBinary);
            #endregion

            MaxEnumValue = fastCSharp.Enum.GetMaxValue<SqlDbType>(-1) + 1;
            int dataIndex = 0, mapSize = ((MaxEnumValue + 31) >> 5) << 2;
            pointer[] datas = unmanaged.GetStatic(true, mapSize, mapSize, MaxEnumValue * sizeof(int));
            stringTypeMap = new structReference<fixedMap.unsafer> { Value = new fixedMap(datas[dataIndex++]).Unsafer };
            textImageTypeMap = new structReference<fixedMap.unsafer> { Value = new fixedMap(datas[dataIndex++]).Unsafer };
            sizes = datas[dataIndex++].Reference;

            #region 字符串类型占位集合
            stringTypeMap.Value.Set((int)SqlDbType.Char);
            stringTypeMap.Value.Set((int)SqlDbType.NChar);
            stringTypeMap.Value.Set((int)SqlDbType.VarChar);
            stringTypeMap.Value.Set((int)SqlDbType.NVarChar);
            stringTypeMap.Value.Set((int)SqlDbType.Text);
            stringTypeMap.Value.Set((int)SqlDbType.NText);
            #endregion

            #region 文本类型占位集合
            textImageTypeMap.Value.Set((int)SqlDbType.Text);
            textImageTypeMap.Value.Set((int)SqlDbType.NText);
            textImageTypeMap.Value.Set((int)SqlDbType.Image);
            #endregion

            #region 类型默认长度
            int* sizeData = sizes.Int;
            for (int i = 0; i != MaxEnumValue; i++) sizeData[i] = UnknownSize;
            sizeData[(int)SqlDbType.BigInt] = sizeof(long);
            sizeData[(int)SqlDbType.Binary] = 8000;
            sizeData[(int)SqlDbType.Bit] = sizeof(bool);
            sizeData[(int)SqlDbType.Char] = 8000;
            //TypeSize[(int)SqlDbType.Date] = sizeof(long);
            sizeData[(int)SqlDbType.DateTime] = sizeof(long);
            //TypeSize[(int)SqlDbType.DateTime2] = sizeof(long);
            //TypeSize[(int)SqlDbType.DateTimeOffset] = sizeof(long);
            sizeData[(int)SqlDbType.Decimal] = sizeof(decimal);
            sizeData[(int)SqlDbType.Float] = sizeof(double);
            sizeData[(int)SqlDbType.Image] = int.MaxValue;
            sizeData[(int)SqlDbType.Int] = sizeof(int);
            sizeData[(int)SqlDbType.Money] = sizeof(decimal);
            //TypeSize[(int)SqlDbType.NChar] = MaxStringSize;
            sizeData[(int)SqlDbType.NText] = int.MaxValue;
            //TypeSize[(int)SqlDbType.NVarChar] = MaxStringSize;
            sizeData[(int)SqlDbType.Real] = sizeof(float);
            sizeData[(int)SqlDbType.UniqueIdentifier] = 8;
            sizeData[(int)SqlDbType.SmallDateTime] = sizeof(long);
            sizeData[(int)SqlDbType.SmallInt] = sizeof(short);
            sizeData[(int)SqlDbType.SmallMoney] = sizeof(decimal);
            sizeData[(int)SqlDbType.Text] = int.MaxValue;
            //TypeSize[(int)SqlDbType.Time] = 8;
            sizeData[(int)SqlDbType.Timestamp] = 8;
            sizeData[(int)SqlDbType.TinyInt] = sizeof(byte);
            sizeData[(int)SqlDbType.VarBinary] = 8000;
            sizeData[(int)SqlDbType.VarChar] = 8000;
            //TypeSize[(int)SqlDbType.Xml] = -1;
            #endregion
        }
    }
}
