using System;
using fastCSharp.reflection;
using fastCSharp.sql;
using fastCSharp.code;

namespace fastCSharp.emit
{
    /// <summary>
    /// 数据库成员信息
    /// </summary>
    public sealed class dataMember : ignoreMember
    {
        /// <summary>
        /// 删除列
        /// </summary>
        public sealed class delete : Attribute { }
        /// <summary>
        /// 数据库成员信息空值
        /// </summary>
        internal static readonly dataMember DefaultDataMember = new dataMember();
        /// <summary>
        /// 数据库类型
        /// </summary>
        public Type DataType;
        /// <summary>
        /// 数据库成员类型
        /// </summary>
        private memberType dataMemberType;
        /// <summary>
        /// 数据库成员类型
        /// </summary>
        public memberType DataMemberType
        {
            get
            {
                if (DataType == null) return null;
                if (dataMemberType == null) dataMemberType = DataType;
                return dataMemberType;
            }
        }
        /// <summary>
        /// 枚举真实类型
        /// </summary>
        public memberType EnumType { get; private set; }
        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity;
        /// <summary>
        /// 主键索引,0标识非主键
        /// </summary>
        public int PrimaryKeyIndex;
        /// <summary>
        /// 分组标识
        /// </summary>
        public int Group;
        /// <summary>
        /// 是否允许空值
        /// </summary>
        public bool IsNull;
        /// <summary>
        /// 字符串是否ASCII
        /// </summary>
        public bool IsAscii;
        /// <summary>
        /// 是否固定长度
        /// </summary>
        public bool IsFixedLength;
        /// <summary>
        /// 是否生成成员索引
        /// </summary>
        public bool IsMemberIndex;
        /// <summary>
        /// 是否生成当前时间
        /// </summary>
        public bool IsNowTime;
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue;
        /// <summary>
        /// 新增字段时的计算子查询
        /// </summary>
        public string UpdateValue;
        ///// <summary>
        ///// 正则验证,不可用
        ///// </summary>
        //public string RegularVerify;
        /// <summary>
        /// 字符串最大长度验证
        /// </summary>
        public int MaxStringLength;
        /// <summary>
        /// 是否忽略字符串最大长度提示
        /// </summary>
        public bool IsIgnoreMaxStringLength;
        /// <summary>
        /// 是否生成日志流代码
        /// </summary>
        public bool IsSqlStream;
        /// <summary>
        /// 是否生成日志流计数
        /// </summary>
        public bool IsSqlStreamCount = true;
        /// <summary>
        /// 日志流计数完成类型
        /// </summary>
        public Type SqlStreamCountType;
        /// <summary>
        /// 日志流计数完成类型表格编号
        /// </summary>
        public int SqlStreamCountTypeNumber;
        /// <summary>
        /// 计数超时秒数
        /// </summary>
        public int CounterTimeout;
        /// <summary>
        /// 计数读取服务名称
        /// </summary>
        public string CounterReadServiceName;
        /// <summary>
        /// 计数更新服务名称
        /// </summary>
        public string CounterWriteServiceName;
        /// <summary>
        /// 是否数据库成员信息空值
        /// </summary>
        internal bool IsDefaultMember
        {
            get
            {
                return this == DefaultDataMember;
            }
        }
        /// <summary>
        /// 格式化数据库成员信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="isSqlColumn"></param>
        /// <returns></returns>
        internal static dataMember FormatSql(dataMember value, Type type, ref bool isSqlColumn)
        {
            if (type.IsEnum)
            {
                Type enumType = System.Enum.GetUnderlyingType(type);
                if (enumType == typeof(sbyte)) enumType = typeof(byte);
                else if (enumType == typeof(ushort)) enumType = typeof(short);
                else if (enumType == typeof(ulong)) enumType = typeof(long);
                if (value == null) return new dataMember { DataType = enumType };
                if (value.DataType == null) value.DataType = enumType;
                else if (enumType != value.DataType) value.EnumType = enumType;
                return value;
            }
            Type nullableType = null;
            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(fastCSharp.sql.fileBlockMember<>))
                {
                    if (value == null) return new dataMember { DataType = typeof(fastCSharp.io.fileBlockStream.index) };
                    value.DataType = typeof(fastCSharp.io.fileBlockStream.index);
                    return value;
                }
                if (genericType == typeof(Nullable<>)) nullableType = type.GetGenericArguments()[0];
            }
            else if (fastCSharp.code.typeAttribute.GetAttribute<sqlColumn>(type, false, false) != null)
            {
                isSqlColumn = true;
                return DefaultDataMember;
            }
            if (value == null || value.DataType == null)
            {
                dataMember sqlMember = fastCSharp.code.typeAttribute.GetAttribute<dataMember>(type, false, false);
                if (sqlMember != null && sqlMember.DataType != null)
                {
                    if (value == null) value = new dataMember();
                    value.DataType = sqlMember.DataType;
                    if (sqlMember.DataType.IsValueType && sqlMember.DataType.IsGenericType && sqlMember.DataType.GetGenericTypeDefinition() == typeof(Nullable<>)) value.IsNull = true;
                }
            }
            if (value == null)
            {
                if (nullableType == null)
                {
                    Type dataType = type.formCSharpType().toCSharpType();
                    if (dataType != type) value = new dataMember { DataType = dataType };
                }
                else
                {
                    value = new dataMember { IsNull = true };
                    Type dataType = nullableType.formCSharpType().toCSharpType();
                    if (dataType != nullableType) value.DataType = dataType.toNullableType();
                }
            }
            return value ?? DefaultDataMember;
        }

        /// <summary>
        /// 获取数据库成员信息
        /// </summary>
        /// <param name="member">成员信息</param>
        /// <returns>数据库成员信息</returns>
        internal static dataMember Get(memberIndex member)
        {
            dataMember value = member.GetAttribute<dataMember>(true, false);
            if (value == null || value.DataType == null)
            {
                if (member.Type.IsEnum)
                {
                    if (value == null) value = new dataMember();
                    value.DataType = System.Enum.GetUnderlyingType(member.Type);
                }
                else
                {
                    dataMember sqlMember = fastCSharp.code.typeAttribute.GetAttribute<dataMember>(member.Type, false, false);
                    if (sqlMember != null && sqlMember.DataType != null)
                    {
                        if (value == null) value = new dataMember();
                        value.DataType = sqlMember.DataType;
                        if (sqlMember.DataType.nullableType() != null) value.IsNull = true;
                    }
                }
            }
            else if (member.Type.IsEnum)
            {
                Type enumType = System.Enum.GetUnderlyingType(member.Type);
                if (enumType != value.DataType) value.EnumType = enumType;
            }
            if (value == null)
            {
                Type nullableType = member.Type.nullableType();
                if (nullableType == null)
                {
                    if (fastCSharp.code.typeAttribute.GetAttribute<sqlColumn>(member.Type, false, false) == null)
                    {
                        Type dataType = member.Type.formCSharpType().toCSharpType();
                        if (dataType != member.Type)
                        {
                            value = new dataMember();
                            value.DataType = dataType;
                        }
                    }
                }
                else
                {
                    value = new dataMember();
                    value.IsNull = true;
                    Type dataType = nullableType.formCSharpType().toCSharpType();
                    if (dataType != nullableType) value.DataType = dataType.toNullableType();
                }
            }
            return value ?? DefaultDataMember;
        }
    }
}
