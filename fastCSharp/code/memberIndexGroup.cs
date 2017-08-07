using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.reflection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员索引分组
    /// </summary>
    public sealed class memberIndexGroup
    {
        /// <summary>
        /// 成员索引分组集合
        /// </summary>
        private static readonly Dictionary<Type, memberIndexGroup> cache = dictionary.CreateOnly<Type, memberIndexGroup>();
        /// <summary>
        /// 成员索引分组集合访问锁
        /// </summary>
        private static readonly object cacheLock = new object();
        /// <summary>
        /// 公有字段
        /// </summary>
        internal readonly fieldIndex[] PublicFields;
        /// <summary>
        /// 非公有字段
        /// </summary>
        internal readonly fieldIndex[] NonPublicFields;
        /// <summary>
        /// 公有属性
        /// </summary>
        internal readonly propertyIndex[] PublicProperties;
        /// <summary>
        /// 非公有属性
        /// </summary>
        internal readonly propertyIndex[] NonPublicProperties;
        /// <summary>
        /// 所有成员数量
        /// </summary>
        public readonly int MemberCount;
        /// <summary>
        /// 成员索引分组
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="isStatic">是否静态成员</param>
        private memberIndexGroup(Type type, bool isStatic)
        {
            int index = 0;
            if (type.IsEnum)
            {
                PublicFields = type.GetFields(BindingFlags.Public | BindingFlags.Static).getArray(member => new fieldIndex(member, memberFilters.PublicStaticField, index++));
                NonPublicFields = nullValue<fieldIndex>.Array;
                PublicProperties = NonPublicProperties = nullValue<propertyIndex>.Array;
            }
            else
            {
                if (type.getTypeName() == null)
                {
                    if (isStatic)
                    {
                        memberIndex.group group = new memberIndex.group(type, true);
                        PublicFields = group.PublicFields.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new fieldIndex(value, memberFilters.PublicStaticField, index++));
                        NonPublicFields = group.NonPublicFields.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new fieldIndex(value, memberFilters.NonPublicStaticField, index++));
                        PublicProperties = group.PublicProperties.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new propertyIndex(value, memberFilters.PublicStaticProperty, index++));
                        NonPublicProperties = group.NonPublicProperties.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new propertyIndex(value, memberFilters.NonPublicStaticProperty, index++));
                    }
                    else
                    {
                        memberIndex.group group = new memberIndex.group(type, false);
                        PublicFields = group.PublicFields.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new fieldIndex(value, memberFilters.PublicInstanceField, index++));
                        NonPublicFields = group.NonPublicFields.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new fieldIndex(value, memberFilters.NonPublicInstanceField, index++));
                        PublicProperties = group.PublicProperties.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new propertyIndex(value, memberFilters.PublicInstanceProperty, index++));
                        NonPublicProperties = group.NonPublicProperties.sort((left, right) => left.Name.CompareTo(right.Name)).getArray(value => new propertyIndex(value, memberFilters.NonPublicInstanceProperty, index++));
                    }
                }
                else
                {
                    PublicFields = NonPublicFields = nullValue<fieldIndex>.Array;
                    PublicProperties = NonPublicProperties = nullValue<propertyIndex>.Array;
                }
            }
            MemberCount = index;
        }
        /// <summary>
        /// 获取成员索引集合
        /// </summary>
        /// <param name="isValue">成员匹配委托</param>
        /// <returns>成员索引集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private memberIndex[] get(Func<memberIndex, bool> isValue)
        {
            return array.concat(PublicFields.getFindArray(isValue), NonPublicFields.getFindArray(isValue), PublicProperties.getFindArray(isValue), NonPublicProperties.getFindArray(isValue));
        }
        /// <summary>
        /// 根据类型获取成员信息集合
        /// </summary>
        /// <param name="filter">选择类型</param>
        /// <param name="isFilter">是否完全匹配选择类型</param>
        /// <returns>成员信息集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal memberIndex[] Find(memberFilters filter, bool isFilter = false)
        {
            return get(value => isFilter ? (value.Filter & filter) == filter : ((value.Filter & filter) != 0));
        }
        ///// <summary>
        ///// 根据类型获取成员信息集合
        ///// </summary>
        ///// <typeparam name="attributeType">自定义属性类型</typeparam>
        ///// <param name="filter">成员选择</param>
        ///// <returns>成员信息集合</returns>
        //internal memberIndex[] Find<attributeType>(attributeType filter) where attributeType : fastCSharp.code.memberFilter
        //{
        //    return Find(filter.MemberFilter).getFindArray(value => filter.IsAttribute ? value.IsAttribute<attributeType>(filter.IsBaseTypeAttribute, filter.IsInheritAttribute) : !value.IsIgnoreAttribute<attributeType>(filter.IsBaseTypeAttribute, filter.IsInheritAttribute));
        //}
        /// <summary>
        /// 根据类型获取成员信息集合
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="filter">成员选择</param>
        /// <returns>成员信息集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal memberIndex[] Find<attributeType>(memberFilter filter) where attributeType : fastCSharp.code.ignoreMember
        {
            return Find(filter.MemberFilter).getFindArray(value => filter.IsAttribute ? value.IsAttribute<attributeType>(filter.IsBaseTypeAttribute, filter.IsInheritAttribute) : !value.IsIgnoreAttribute<attributeType>(filter.IsBaseTypeAttribute, filter.IsInheritAttribute));
        }
        /// <summary>
        /// 根据类型获取成员索引分组
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>成员索引分组</returns>
        public static memberIndexGroup Get(Type type)
        {
            memberIndexGroup value;
            Monitor.Enter(cacheLock);
            try
            {
                if (!cache.TryGetValue(type, out value)) cache.Add(type, value = new memberIndexGroup(type, false));
            }
            finally { Monitor.Exit(cacheLock); }
            return value;
        }
        /// <summary>
        /// 获取匹配成员集合
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="filter">选择类型</param>
        /// <param name="isFilter">是否完全匹配选择类型</param>
        /// <param name="isAttribute">是否匹配自定义属性类型</param>
        /// <param name="isBaseType">指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)。</param>
        /// <param name="isInheritAttribute">自定义属性类型是否可继承</param>
        /// <returns>匹配成员集合</returns>
        public static memberIndex[] Get<attributeType>(Type type, memberFilters filter, bool isFilter, bool isAttribute, bool isBaseType, bool isInheritAttribute)
             where attributeType : fastCSharp.code.ignoreMember
        {
            return Get(type).Find(filter, isFilter).getFindArray(value => isAttribute ? value.IsAttribute<attributeType>(isBaseType, isInheritAttribute) : !value.IsIgnoreAttribute<attributeType>(isBaseType, isInheritAttribute));
        }
        /// <summary>
        /// 成员索引分组集合
        /// </summary>
        private static readonly Dictionary<Type, memberIndexGroup> staticCache = dictionary.CreateOnly<Type, memberIndexGroup>();
        /// <summary>
        /// 成员索引分组集合访问锁
        /// </summary>
        private static readonly object staticCacheLock = new object();
        /// <summary>
        /// 根据类型获取成员索引分组
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>成员索引分组</returns>
        private static memberIndexGroup getStatic(Type type)
        {
            memberIndexGroup value;
            Monitor.Enter(staticCacheLock);
            try
            {
                if (!staticCache.TryGetValue(type, out value)) staticCache.Add(type, value = new memberIndexGroup(type, true));
            }
            finally { Monitor.Exit(staticCacheLock); }
            return value;
        }
        /// <summary>
        /// 获取匹配成员集合
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="filter">选择类型</param>
        /// <param name="isFilter">是否完全匹配选择类型</param>
        /// <param name="isAttribute">是否匹配自定义属性类型</param>
        /// <param name="isBaseType">指定是否搜索该成员的继承链以查找这些特性，参考System.Reflection.MemberInfo.GetCustomAttributes(bool inherit)。</param>
        /// <param name="isInheritAttribute">自定义属性类型是否可继承</param>
        /// <returns>匹配成员集合</returns>
        public static memberIndex[] GetStatic<attributeType>(Type type, memberFilters filter, bool isFilter, bool isAttribute, bool isBaseType, bool isInheritAttribute)
             where attributeType : fastCSharp.code.ignoreMember
        {
            return getStatic(type).Find(filter, isFilter).getFindArray(value => isAttribute ? value.IsAttribute<attributeType>(isBaseType, isInheritAttribute) : !value.IsIgnoreAttribute<attributeType>(isBaseType, isInheritAttribute));
        }
    }
    /// <summary>
    /// 成员索引分组
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    internal static class memberIndexGroup<valueType>
    {
        /// <summary>
        /// 成员索引分组
        /// </summary>
        public static readonly memberIndexGroup Group = memberIndexGroup.Get(typeof(valueType));
        /// <summary>
        /// 所有成员数量
        /// </summary>
        public static readonly int MemberCount = Group.MemberCount;
        /// <summary>
        /// 字段成员数量
        /// </summary>
        public static readonly int FieldCount = Group.PublicFields.Length + Group.NonPublicFields.Length;
        /// <summary>
        /// 成员集合
        /// </summary>
        public static memberIndex[] GetAllMembers()
        {
            subArray<memberIndex> members = new subArray<memberIndex>(MemberCount);
            members.Add(Group.PublicFields.toGeneric<memberIndex>());
            members.Add(Group.NonPublicFields.toGeneric<memberIndex>());
            members.Add(Group.PublicProperties.toGeneric<memberIndex>());
            members.Add(Group.NonPublicProperties.toGeneric<memberIndex>());
            return members.ToArray();
        }
        /// <summary>
        /// 获取字段集合
        /// </summary>
        /// <param name="memberFilter">成员选择类型</param>
        /// <returns></returns>
        public static fieldIndex[] GetFields(memberFilters memberFilter = memberFilters.InstanceField)
        {
            if ((memberFilter & memberFilters.PublicInstanceField) == 0)
            {
                if ((memberFilter & memberFilters.NonPublicInstanceField) == 0) return nullValue<fieldIndex>.Array;
                return Group.NonPublicFields;
            }
            else if ((memberFilter & memberFilters.NonPublicInstanceField) == 0) return Group.PublicFields;
            return Group.PublicFields.concat(Group.NonPublicFields);
        }
        /// <summary>
        /// 获取属性集合
        /// </summary>
        /// <param name="memberFilter">成员选择类型</param>
        /// <returns></returns>
        public static propertyIndex[] GetProperties(memberFilters memberFilter = memberFilters.InstanceField)
        {
            if ((memberFilter & memberFilters.PublicInstanceProperty) == 0)
            {
                if ((memberFilter & memberFilters.NonPublicInstanceProperty) == 0) return nullValue<propertyIndex>.Array;
                return Group.NonPublicProperties;
            }
            else if ((memberFilter & memberFilters.NonPublicInstanceProperty) == 0) return Group.PublicProperties;
            return Group.PublicProperties.concat(Group.NonPublicProperties);
        }
    }
}
