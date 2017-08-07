using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.excel
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
        static sqlDbType()
        {
            #region 数据类型集合
            sqlTypeNames = new string[fastCSharp.Enum.GetMaxValue<SqlDbType>(-1) + 1];
            sqlTypeNames[(int)SqlDbType.BigInt] = "INTEGER";
            //SqlTypeNames[(int)SqlDbType.Binary] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.Bit] = "BOOLEAN";
            sqlTypeNames[(int)SqlDbType.Char] = "VARCHAR";
            sqlTypeNames[(int)SqlDbType.DateTime] = "DATETIME";
            sqlTypeNames[(int)SqlDbType.Decimal] = "DECIMAL";
            sqlTypeNames[(int)SqlDbType.Float] = "DECIMAL";
            //SqlTypeNames[(int)SqlDbType.Image] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.Int] = "INTEGER";
            sqlTypeNames[(int)SqlDbType.Money] = "DECIMAL";
            sqlTypeNames[(int)SqlDbType.NChar] = "VARCHAR";
            sqlTypeNames[(int)SqlDbType.NText] = "VARCHAR";
            sqlTypeNames[(int)SqlDbType.NVarChar] = "VARCHAR";
            sqlTypeNames[(int)SqlDbType.Real] = "DECIMAL";
            //SqlTypeNames[(int)SqlDbType.UniqueIdentifier] = typeof(Guid);
            sqlTypeNames[(int)SqlDbType.SmallDateTime] = "DATETIME";
            sqlTypeNames[(int)SqlDbType.SmallInt] = "INTEGER";
            sqlTypeNames[(int)SqlDbType.SmallMoney] = "DECIMAL";
            sqlTypeNames[(int)SqlDbType.Text] = "VARCHAR";
            //SqlTypeNames[(int)SqlDbType.Timestamp] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.TinyInt] = "INTEGER";
            //SqlTypeNames[(int)SqlDbType.VarBinary] = typeof(byte[]);
            sqlTypeNames[(int)SqlDbType.VarChar] = "VARCHAR";
            //SqlTypeNames[(int)SqlDbType.Variant] = typeof(object);
            #endregion
        }
    }
}
