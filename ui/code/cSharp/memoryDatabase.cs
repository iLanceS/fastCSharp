using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Text;
using System.IO;
using fastCSharp.memoryDatabase;
using fastCSharp.threading;
using fastCSharp.reflection;
using fastCSharp.io;
using fastCSharp.memoryDatabase.cache;

namespace fastCSharp.code
{
    /// <summary>
    /// 内存数据库配置
    /// </summary>
    internal partial class memoryDatabase
    {
        /// <summary>
        /// 内存数据库代码生成
        /// </summary
        [auto(Name = "内存数据库", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : database.cSharp<fastCSharp.code.cSharp.memoryDatabase>, IAuto
        {
            /// <summary>
            /// 缓存成员位图名称替换
            /// </summary>
            private struct cacheMemberMapReplace { }
            /// <summary>
            /// 缓存成员关键字名称替换
            /// </summary>
            private struct cachePrimaryKeyReplace { }
            /// <summary>
            /// 删除关键字特列类型
            /// </summary>
            private static readonly HashSet<Type> tableDeletePrimaryKeyTypes;
            /// <summary>
            /// 默认空属性
            /// </summary>
            private static readonly fastCSharp.code.cSharp.serialize serializeAttribute = new fastCSharp.code.cSharp.serialize { IsReferenceMember = false };
            /// <summary>
            /// 序列化代码生成
            /// </summary>
            private serialize.cSharp serialize = new serialize.cSharp();
            /// <summary>
            /// 成员索引标识序列化代码生成
            /// </summary>
            private indexSerialize.cSharp indexSerialize = new indexSerialize.cSharp();
            /// <summary>
            /// 关键字
            /// </summary>
            public code.memberInfo PrimaryKey;
            /// <summary>
            /// 数据库文件名
            /// </summary>
            public string FileName
            {
                get { return Attribute.FileName ?? type.TypeOnlyName; }
            }
            /// <summary>
            /// 表格基本缓存类型
            /// </summary>
            public string TableCacheTypeName
            {
                get
                {
                    Type type = Attribute.CacheType;
                    if (type.IsGenericTypeDefinition)
                    {
                        string name = type.fullName();
                        switch (type.GetGenericArguments().Length)
                        {
                            case 2:
                                return name.Substring(0, name.LastIndexOf('<')) + "<" + this.type.FullName + ", " + this.type.FullName + ".memberMap>";
                            case 3:
                                return name.Substring(0, name.LastIndexOf('<')) + "<" + this.type.FullName + ", " + this.type.FullName + ".memberMap, " + PrimaryKeyTypeName + ">";
                        }
                    }
                    return type.fullName();
                }
            }
            /// <summary>
            /// 关键字类型名称
            /// </summary>
            public string PrimaryKeyTypeName
            {
                get
                {
                    if (Identity != null) return Identity.MemberType.FullName;
                    if (PrimaryKey != null) return PrimaryKey.MemberType.FullName;
                    return typeof(fastCSharp.data.primaryKey<,>).Namespace + "." + typeof(fastCSharp.data.primaryKey<,>).onlyName() + "<" + PrimaryKeys.joinString(',', value => value.Member.MemberType.FullName) + ">";
                }
            }
            /// <summary>
            /// 是否删除关键字特列类型
            /// </summary>
            public bool IsTablePrimaryKey
            {
                get
                {
                    return PrimaryKeys == null && tableDeletePrimaryKeyTypes.Contains((Identity ?? PrimaryKey).MemberType.Type);
                }
            }
            /// <summary>
            /// 客户端连接池获取接口类型名称
            /// </summary>
            public string MemoryDatabasePhysicalClientTypeName
            {
                get { return Attribute.ClientType.fullName(); }
            }
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                if (!Attribute.IsReflection)
                {
                    keyValue<code.memberInfo, fastCSharp.emit.dataMember>[] dataMembers = GetMembers(type, Attribute);
                    Identity = fastCSharp.code.cSharp.database.GetIdentity(dataMembers);
                    code.memberInfo[] primaryKeys = Identity == null ? fastCSharp.code.cSharp.database.GetPrimaryKey(dataMembers) : null;
                    if (Identity != null || primaryKeys != null)
                    {
                        int primaryKeyIndex = 0;
                        PrimaryKey = primaryKeys != null && primaryKeys.Length == 1 ? primaryKeys[0] : null;
                        PrimaryKeys = primaryKeys != null && primaryKeys.Length > 1 ? primaryKeys.getArray(member => new primaryKey { Member = member, PrimaryKeyName = "Key" + (++primaryKeyIndex).ToString(), IsLastPrimaryKey = primaryKeyIndex == primaryKeys.Length }) : null;
                        //    VerifyMembers = sqlMembers.getFindArray(value => IsMemberVerify(value));
                        if (Attribute.IsIndexSerialize) indexSerialize.Create(type, code.cSharp.indexSerialize.SerializeAttribute);
                        else serialize.Create(type, serializeAttribute);
                        memberMap.Create(type);
                        copy.Create(type);
                        create(true);
                    }
                    else error.Add("内存数据库 " + type.FullName + " 缺少关键字");
                }
            }
            static cSharp()
            {
                tableDeletePrimaryKeyTypes = new HashSet<Type>(typeof(fastCSharp.code.cSharp.memoryDatabase.table).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                    .getFind(method => method.Name == "deleteLog" && method.ReturnType == typeof(void))
                    .GetArray(method => method.GetParameters())
                    .getFind(parameters => parameters.Length == 2 && parameters[1].ParameterType == typeof(unmanagedStream))
                    .GetArray(parameters => parameters[0].ParameterType));
            }
        }
    }
}
