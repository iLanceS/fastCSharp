using System;
using System.Reflection;
using fastCSharp.reflection;
using fastCSharp.emit;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 数据库表格模型配置
    /// </summary>
    public abstract class dataModel : memberFilter.publicInstanceField
    {
        /// <summary>
        /// 是否有序比较
        /// </summary>
        public bool IsComparable;
        ///// <summary>
        ///// 是否检查添加数据的自增值
        ///// </summary>
        //public bool IsCheckAppendIdentity = true;
        /// <summary>
        /// 获取数据库表格模型类型
        /// </summary>
        /// <param name="type">数据库表格绑定类型</param>
        /// <param name="modeType">数据库表格模型类型,失败返回null</param>
        /// <returns>数据库表格模型配置</returns>
        internal static modelType GetModelType<modelType>(Type type, out Type modeType) where modelType : dataModel
        {
            do
            {
                modelType sqlModel = fastCSharp.code.typeAttribute.GetAttribute<modelType>(type, false, true);
                if (sqlModel != null)
                {
                    modeType = type;
                    return sqlModel;
                }
                if ((type = type.BaseType) == null)
                {
                    modeType = null;
                    return null;
                }
            }
            while (true);
        }
        /// <summary>
        /// 获取字段成员集合
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <returns>字段成员集合</returns>
        public static subArray<memberInfo> GetPrimaryKeys<modeType>(Type type, modeType model) where modeType : dataModel
        {
            fieldIndex[] fields = (fieldIndex[])typeof(fastCSharp.code.memberIndexGroup<>).MakeGenericType(type).GetMethod("GetFields", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { model.MemberFilter });
            subArray<memberInfo> values = new subArray<memberInfo>();
            foreach (fieldIndex field in fields)
            {
                type = field.Member.FieldType;
                if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                {
                    dataMember attribute = field.GetSetupAttribute<dataMember>(true, true);
                    if (attribute != null && attribute.PrimaryKeyIndex != 0) values.Add(new memberInfo(field.Member, attribute.PrimaryKeyIndex));
                }
            }
            return values.Sort((left, right) =>
            {
                int value = left.MemberIndex - right.MemberIndex;
                return value == 0 ? left.MemberName.CompareTo(right.MemberName) : value;
            });
        }
        /// <summary>
        /// 获取自增字段成员
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <returns>自增字段成员</returns>
        public static memberInfo GetIdentity<modeType>(Type type, modeType model) where modeType : dataModel
        {
            fieldIndex[] fields = (fieldIndex[])typeof(fastCSharp.code.memberIndexGroup<>).MakeGenericType(type).GetMethod("GetFields", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { model.MemberFilter });
            memberInfo identity = null;
            int isCase = 0;
            foreach (fieldIndex field in fields)
            {
                type = field.Member.FieldType;
                if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                {
                    dataMember attribute = field.GetSetupAttribute<dataMember>(true, true);
                    if (attribute != null && attribute.IsIdentity) return new memberInfo(field.Member, 0);
                    if (isCase == 0 && field.Member.Name == fastCSharp.config.sql.Default.DefaultIdentityName)
                    {
                        identity = new memberInfo(field.Member, 0);
                        isCase = 1;
                    }
                    else if (identity == null && field.Member.Name.ToLower() == fastCSharp.config.sql.Default.DefaultIdentityName) identity = new memberInfo(field.Member, 0);
                }
            }
            return identity;
        }
    }
}
