using System;
using fastCSharp.reflection;
using fastCSharp.code;
using fastCSharp.code.cSharp;
using System.Reflection;

namespace fastCSharp.emit
{
    /// <summary>
    /// 内存数据库表格模型
    /// </summary>
    /// <typeparam name="valueType"></typeparam>
    internal abstract class memoryDatabaseModel<valueType> : databaseModel<valueType>
    {
        /// <summary>
        /// 内存数据库表格模型配置
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.memoryDatabaseModel Attribute;
        /// <summary>
        /// 内存数据库基本成员集合
        /// </summary>
        internal static readonly fastCSharp.code.memberMap MemberMap = fastCSharp.code.memberMap<valueType>.NewEmpty();
        /// <summary>
        /// 是否所有成员
        /// </summary>
        internal static readonly int IsAllMember;
        /// <summary>
        /// 设置自增标识
        /// </summary>
        internal static readonly Action<valueType, int> SetIdentity;
        /// <summary>
        /// 自增标识获取器
        /// </summary>
        internal static readonly Func<valueType, int> GetIdentity;
        /// <summary>
        /// 自增字段
        /// </summary>
        internal static readonly memoryDatabaseModel.fieldInfo Identity;
        /// <summary>
        /// 关键字集合
        /// </summary>
        internal static readonly memoryDatabaseModel.fieldInfo[] PrimaryKeys;
        /// <summary>
        /// 关键字集合
        /// </summary>
        internal static FieldInfo[] PrimaryKeyFields
        {
            get { return PrimaryKeys.getArray(value => value.Field); }
        }

        static memoryDatabaseModel()
        {
            Type type = typeof(valueType);
            Attribute = fastCSharp.code.typeAttribute.GetAttribute<memoryDatabaseModel>(type, true, true) ?? memoryDatabaseModel.Default;
            code.fieldIndex[] fieldArray = fastCSharp.code.memberIndexGroup<valueType>.GetFields(Attribute.MemberFilter);
            subArray<memoryDatabaseModel.fieldInfo> fields = new subArray<memoryDatabaseModel.fieldInfo>(), primaryKeys = new subArray<memoryDatabaseModel.fieldInfo>();
            memoryDatabaseModel.fieldInfo identity = default(memoryDatabaseModel.fieldInfo);
            int isCase = 0, isIdentity = 0;
            foreach (code.fieldIndex field in fieldArray)
            {
                Type memberType = field.Member.FieldType;
                if (!memberType.IsPointer && (!memberType.IsArray || memberType.GetArrayRank() == 1) && !field.IsIgnore)
                {
                    dataMember memberAttribute = field.GetAttribute<dataMember>(true, true) ?? dataMember.DefaultDataMember;
                    if (memberAttribute.IsSetup)
                    {
                        fields.Add(new memoryDatabaseModel.fieldInfo(field, memberAttribute));
                        MemberMap.SetMember(field.MemberIndex);
                        if (isIdentity == 0)
                        {
                            if (memberAttribute != null && memberAttribute.IsIdentity)
                            {
                                identity = new memoryDatabaseModel.fieldInfo(field, memberAttribute);
                                isIdentity = 1;
                            }
                            else if (isCase == 0 && field.Member.Name == fastCSharp.config.memoryDatabase.Default.DefaultIdentityName)
                            {
                                identity = new memoryDatabaseModel.fieldInfo(field, memberAttribute);
                                isCase = 1;
                            }
                            else if (identity.Field == null && field.Member.Name.ToLower() == fastCSharp.config.memoryDatabase.Default.DefaultIdentityName) identity = new memoryDatabaseModel.fieldInfo(field, memberAttribute);
                        }
                        if (memberAttribute.PrimaryKeyIndex != 0) primaryKeys.Add(new memoryDatabaseModel.fieldInfo(field, memberAttribute));
                    }
                }
            }
            IsAllMember = fields.length == fieldArray.Length ? 1 : 0;
            if ((Identity = identity).Field != null)
            {
#if NOJIT
                new sqlModel<valueType>.identity32(identity.Field).Get(out GetIdentity, out SetIdentity);
#else
                GetIdentity = getIdentityGetter32("GetMemoryDatabaseIdentity", identity.Field);
                SetIdentity = getIdentitySetter32("SetMemoryDatabaseIdentity", identity.Field);
#endif
            }
            PrimaryKeys = primaryKeys.ToArray();
        }
    }
}
