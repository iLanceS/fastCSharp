using System;
using System.Reflection;
using fastCSharp.emit;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 内存数据库表格模型配置
    /// </summary>
    public class memoryDatabaseModel : dataModel
    {
        /// <summary>
        /// 默认空属性
        /// </summary>
        internal static readonly memoryDatabaseModel Default = new memoryDatabaseModel();
        /// <summary>
        /// 字段信息
        /// </summary>
        internal struct fieldInfo
        {
            /// <summary>
            /// 字段信息
            /// </summary>
            public FieldInfo Field;
            /// <summary>
            /// 数据库成员信息
            /// </summary>
            public dataMember DataMember;
            /// <summary>
            /// 成员位图索引
            /// </summary>
            public int MemberMapIndex;
            /// <summary>
            /// 字段信息
            /// </summary>
            /// <param name="field"></param>
            /// <param name="attribute"></param>
            public fieldInfo(fieldIndex field, dataMember attribute)
            {
                Field = field.Member;
                DataMember = attribute;
                MemberMapIndex = field.MemberIndex;
            }
        }
    }
}
