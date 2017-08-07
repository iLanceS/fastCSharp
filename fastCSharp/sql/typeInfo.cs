using System;
using System.Collections.Generic;
using fastCSharp.threading;

namespace fastCSharp.sql
{
    /// <summary>
    /// SQL类型信息
    /// </summary>
    internal sealed class typeInfo : Attribute
    {
        /// <summary>
        /// SQL客户端处理类型
        /// </summary>
        public Type ClientType;
        /// <summary>
        /// SQL常量转换处理类型
        /// </summary>
        public Type ConverterType;
        /// <summary>
        /// 名称是否忽略大小写
        /// </summary>
        public bool IgnoreCase;
        /// <summary>
        /// SQL常量转换处理
        /// </summary>
        public expression.constantConverter Converter
        {
            get
            {
                if (ConverterType == null || ConverterType == typeof(expression.constantConverter)) return expression.constantConverter.Default;
                expression.constantConverter value;
                if (converters.TryGetValue(ConverterType, out value)) return value;
                converters.Set(ConverterType, value = (expression.constantConverter)ConverterType.GetConstructor(null).Invoke(null));
                return value;
            }
        }
        /// <summary>
        /// SQL常量转换处理类型集合
        /// </summary>
        private static readonly interlocked.dictionary<Type, expression.constantConverter> converters = new interlocked.dictionary<Type, expression.constantConverter>();
    }
    /// <summary>
    /// SQL类型
    /// </summary>
    public enum type : byte
    {
        /// <summary>
        /// SQL Server2000
        /// </summary>
        [typeInfo(ClientType = typeof(msSql.sql2000), ConverterType = typeof(expression.constantConverter), IgnoreCase = true)]
        Sql2000,
        /// <summary>
        /// SQL Server2005
        /// </summary>
        [typeInfo(ClientType = typeof(msSql.sql2005), IgnoreCase = true)]
        Sql2005,
        /// <summary>
        /// SQL Server2008
        /// </summary>
        [typeInfo(ClientType = typeof(msSql.sql2005), IgnoreCase = true)]
        Sql2008,
        /// <summary>
        /// SQL Server2012
        /// </summary>
        [typeInfo(ClientType = typeof(msSql.sql2005), IgnoreCase = true)]
        Sql2012,
        /// <summary>
        /// Excel
        /// </summary>
#if XAMARIN
        [typeInfo(IgnoreCase = true)]
#else
        [typeInfo(ClientType = typeof(excel.client))]
#endif
        Excel,
        /// <summary>
        /// MySql
        /// </summary>
#if NOMYSQL
        [typeInfo(IgnoreCase = true)]
#else
        [typeInfo(ClientType = typeof(mySql.client), IgnoreCase = true)]
#endif
        MySql,
    }
}
