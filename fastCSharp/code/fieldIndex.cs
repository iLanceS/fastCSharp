using System;
using System.Reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 字段索引
    /// </summary>
    internal sealed class fieldIndex : memberIndex<FieldInfo>
    {
        /// <summary>
        /// 匿名字段名称
        /// </summary>
        public string AnonymousName
        {
            get
            {
                string name = Member.Name;
                return name[0] == '<' ? name.Substring(1, name.IndexOf('>') - 1) : name;
            }
        }
        /// <summary>
        /// 字段信息
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <param name="filter">选择类型</param>
        /// <param name="index">成员编号</param>
        public fieldIndex(FieldInfo field, memberFilters filter, int index)
            : base(field, filter, index)
        {
            IsField = CanGet = true;
            CanSet = !field.IsInitOnly;
            Type = field.FieldType;
        }
    }
}
