using System;
using System.Reflection;

namespace fastCSharp.reflection
{
    /// <summary>
    /// 成员属性相关操作
    /// </summary>
    public static class memberInfo
    {
        /// <summary>
        /// 根据成员属性获取自定义属性
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="member">成员属性</param>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <returns>自定义属性</returns>
        public static attributeType customAttribute<attributeType>(this MemberInfo member, bool isBaseType = false) where attributeType : Attribute
        {
            attributeType value = null;
            if (member != null)
            {
                foreach (object attribute in member.GetCustomAttributes(typeof(attributeType), isBaseType))
                {
                    if (attribute.GetType() == typeof(attributeType)) return attribute as attributeType;
                }
            }
            return value;
        }
    }
}
