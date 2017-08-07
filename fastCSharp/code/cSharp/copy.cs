using System;
using System.Reflection;
using fastCSharp.reflection;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 成员复制接口
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    public interface ICopy<valueType>
    {
        /// <summary>
        /// 成员复制
        /// </summary>
        /// <param name="value">被复制对象</param>
        void CopyFrom(valueType value);
        /// <summary>
        /// 浅复制对象
        /// </summary>
        /// <returns>复制的对象</returns>
        valueType CopyMember();
    }
    /// <summary>
    /// 成员复制接口
    /// </summary>
    /// <typeparam name="valueType">数据类型</typeparam>
    /// <typeparam name="memberType">成员位图类型</typeparam>
    public interface ICopy<valueType, memberType> : ICopy<valueType> where memberType : IMemberMap<memberType>
    {
        /// <summary>
        /// 成员复制
        /// </summary>
        /// <param name="value">被复制对象</param>
        /// <param name="memberMap">复制成员位图</param>
        void CopyFrom(valueType value, memberType memberMap);
    }
    /// <summary>
    /// 成员复制(反射模式)
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public static class copy<valueType>
    {
        /// <summary>
        /// 对象成员复制
        /// </summary>
        /// <param name="value">目标对象</param>
        /// <param name="copyValue">被复制对象</param>
        /// <param name="filter">成员选择</param>
        /// <param name="memberMap">成员位图</param>
        public static valueType Copy(valueType value, valueType copyValue
            , code.memberFilters filter = code.memberFilters.InstanceField, memberMap<valueType> memberMap = default(memberMap<valueType>))
        {
            if (isStruct) return memberGroup.CopyValue(value, copyValue, filter, memberMap);
            else
            {
                if (value == null || copyValue == null) log.Error.Throw(log.exceptionType.Null);
                memberGroup.Copy(value, copyValue, filter, memberMap);
                return value;
            }
        }
        /// <summary>
        /// 是否值类型
        /// </summary>
        private static readonly bool isStruct;
        /// <summary>
        /// 动态成员分组
        /// </summary>
        private static readonly memberGroup<valueType> memberGroup;
        static copy()
        {
            isStruct = typeof(valueType).isStruct() || typeof(valueType).IsEnum;
            memberGroup = memberGroup<valueType>.Create(value => value.CanGet && value.CanSet);
        }
    }
}
