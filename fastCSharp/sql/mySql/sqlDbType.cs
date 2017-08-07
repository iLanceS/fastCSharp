using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.mySql
{
    /// <summary>
    /// SQL数据类型相关操作
    /// </summary>
    internal static class sqlDbType
    {
        /// <summary>
        /// 数据类型集合
        /// </summary>
        private static readonly string[] sqlTypeNames;
        /// <summary>
        /// 获取数据类型名称
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>数据类型名称</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string getSqlTypeName(this SqlDbType type)
        {
            return sqlTypeNames.get((int)type, null);
        }
        /// <summary>
        /// 默认值集合
        /// </summary>
        private static readonly string[] defaultValues;
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>默认值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string getDefaultValue(this SqlDbType type)
        {
            return defaultValues.get((int)type, null);
        }
        /// <summary>
        /// 数据类型名称唯一哈希
        /// </summary>
        private struct sqlTypeName : IEquatable<sqlTypeName>
        {
            /// <summary>
            /// 数据类型名称
            /// </summary>
            public string TypeName;
            /// <summary>
            /// 数据类型长度
            /// </summary>
            public int Length;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">数据类型名称</param>
            /// <returns>数据类型名称唯一哈希</returns>
            public static implicit operator sqlTypeName(string name) { return new sqlTypeName { TypeName = name, Length = name.Length }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public unsafe override int GetHashCode()
            {
                if (TypeName.Length <= 2) return 1;
                fixed (char* nameFixed = TypeName)
                {
                    return ((nameFixed[2] << 2) ^ ((nameFixed[Length >> 2] >> 2) | 0x20)) & (int)((1U << 4) - 1);
                }
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(sqlTypeName other)
            {
                return Length == other.Length && TypeName.equalCase(other.TypeName, Length);
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((sqlTypeName)obj);
            }
        }
        /// <summary>
        /// 数据类型集合唯一哈希
        /// </summary>
        private static readonly uniqueDictionary<sqlTypeName, keyValue<SqlDbType, int>> sqlTypes;
        /// <summary>
        /// 格式化数据类型
        /// </summary>
        /// <param name="typeString">数据类型字符串</param>
        /// <param name="size">列长</param>
        /// <returns>数据类型</returns>
        public unsafe static SqlDbType FormatDbType(string typeString, out int size)
        {
            fixed (char* typeFixed = typeString)
            {
                char* end = typeFixed + typeString.Length, typeEnd = *(end - 1) == ')' ? unsafer.String.Find(typeFixed, end, '(') : end;
                sqlTypeName typeName = new sqlTypeName { TypeName = typeString, Length = (int)(typeEnd - typeFixed) };
                keyValue<SqlDbType, int> value = sqlTypes.Get(typeName, new keyValue<SqlDbType, int>((SqlDbType)(-1), int.MinValue));
                if (value.Value == int.MinValue)
                {
                    size = 0;
                    if (typeEnd != end)
                    {
                        for (--end; ++typeEnd != end; size += *typeEnd - '0') size *= 10;
                    }
                }
                else size = value.Value;
                return value.Key;
            }
        }
        unsafe static sqlDbType()
        {
            #region 数据类型集合
            sqlTypeNames = new string[fastCSharp.Enum.GetMaxValue<SqlDbType>(-1) + 1];
            sqlTypeNames[(int)SqlDbType.BigInt] = "BIGINT";
            //SqlTypeNames[(int)SqlDbType.Binary] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.Bit] = "BIT";
            sqlTypeNames[(int)SqlDbType.Char] = "CHAR";
            sqlTypeNames[(int)SqlDbType.DateTime] = "DATETIME";
            sqlTypeNames[(int)SqlDbType.Decimal] = "DECIMAL";
            sqlTypeNames[(int)SqlDbType.Float] = "DOUBLE";
            //SqlTypeNames[(int)SqlDbType.Image] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.Int] = "INT";
            sqlTypeNames[(int)SqlDbType.Money] = "DECIMAL";
            sqlTypeNames[(int)SqlDbType.NChar] = "CHAR";
            sqlTypeNames[(int)SqlDbType.NText] = "TEXT";
            sqlTypeNames[(int)SqlDbType.NVarChar] = "VARCHAR";
            sqlTypeNames[(int)SqlDbType.Real] = "FLOAT";
            //SqlTypeNames[(int)SqlDbType.UniqueIdentifier] = typeof(Guid);
            sqlTypeNames[(int)SqlDbType.SmallDateTime] = "DATETIME";
            sqlTypeNames[(int)SqlDbType.SmallInt] = "SMALLINT";
            sqlTypeNames[(int)SqlDbType.SmallMoney] = "DECIMAL";
            sqlTypeNames[(int)SqlDbType.Text] = "TEXT";
            //SqlTypeNames[(int)SqlDbType.Timestamp] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.TinyInt] = "TINYINT UNSIGNED";
            //SqlTypeNames[(int)SqlDbType.VarBinary] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.VarChar] = "VARCHAR";
            //SqlTypeNames[(int)SqlDbType.Variant] = typeof(object);
            #endregion

            #region 默认值集合
            defaultValues = new string[fastCSharp.Enum.GetMaxValue<SqlDbType>(0) + 1];
            defaultValues[(int)SqlDbType.BigInt] = "0";
            defaultValues[(int)SqlDbType.Bit] = "0";
            defaultValues[(int)SqlDbType.Char] = "''";
            defaultValues[(int)SqlDbType.DateTime] = "now()";
            defaultValues[(int)SqlDbType.Decimal] = "0";
            defaultValues[(int)SqlDbType.Float] = "0";
            defaultValues[(int)SqlDbType.Int] = "0";
            defaultValues[(int)SqlDbType.Money] = "0";
            defaultValues[(int)SqlDbType.NChar] = "''";
            defaultValues[(int)SqlDbType.NText] = "''";
            defaultValues[(int)SqlDbType.NVarChar] = "''";
            defaultValues[(int)SqlDbType.Real] = "0";
            defaultValues[(int)SqlDbType.SmallDateTime] = "now()";
            defaultValues[(int)SqlDbType.SmallInt] = "0";
            defaultValues[(int)SqlDbType.SmallMoney] = "0";
            defaultValues[(int)SqlDbType.Text] = "''";
            defaultValues[(int)SqlDbType.TinyInt] = "0";
            defaultValues[(int)SqlDbType.VarChar] = "''";
            #endregion

            #region 数据类型集合唯一哈希
            subArray<keyValue<sqlTypeName, keyValue<SqlDbType, int>>> names = new subArray<keyValue<sqlTypeName, keyValue<SqlDbType, int>>>(12);
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"bigint", new keyValue<SqlDbType, int>(SqlDbType.BigInt, sizeof(long))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"bit", new keyValue<SqlDbType, int>(SqlDbType.Bit, sizeof(bool))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"char", new keyValue<SqlDbType, int>(SqlDbType.Char, sizeof(char))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"datetime", new keyValue<SqlDbType, int>(SqlDbType.DateTime, sizeof(DateTime))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"decimal", new keyValue<SqlDbType, int>(SqlDbType.Decimal, sizeof(decimal))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"double", new keyValue<SqlDbType, int>(SqlDbType.Float, sizeof(double))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"int", new keyValue<SqlDbType, int>(SqlDbType.Int, sizeof(int))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"text", new keyValue<SqlDbType, int>(SqlDbType.Text, int.MinValue)));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"varchar", new keyValue<SqlDbType, int>(SqlDbType.VarChar, int.MinValue)));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"float", new keyValue<SqlDbType, int>(SqlDbType.Real, sizeof(float))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"smallint", new keyValue<SqlDbType, int>(SqlDbType.SmallInt, sizeof(short))));
            names.UnsafeAdd(new keyValue<sqlTypeName, keyValue<SqlDbType, int>>((sqlTypeName)"tinyint", new keyValue<SqlDbType, int>(SqlDbType.TinyInt, sizeof(byte))));
            sqlTypes = new uniqueDictionary<sqlTypeName, keyValue<SqlDbType, int>>(names.array, 16);
            #endregion
        }
    }
}
