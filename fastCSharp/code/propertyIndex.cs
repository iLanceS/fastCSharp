using System;
using System.Reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 属性索引
    /// </summary>
    internal sealed class propertyIndex : memberIndex<PropertyInfo>
    {
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <param name="property">属性信息</param>
        /// <param name="filter">选择类型</param>
        /// <param name="index">成员编号</param>
        public propertyIndex(PropertyInfo property, memberFilters filter, int index)
            : base(property, filter, index)
        {
            CanSet = property.CanWrite;
            CanGet = property.CanRead;
            Type = property.PropertyType;
        }
    }
}
