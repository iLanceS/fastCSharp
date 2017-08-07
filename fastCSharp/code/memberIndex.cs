using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员索引
    /// </summary>
    public abstract class memberIndex
    {
        /// <summary>
        /// 动态成员分组
        /// </summary>
        internal struct group
        {
            /// <summary>
            /// 类型深度
            /// </summary>
            private struct typeDepth
            {
                /// <summary>
                /// 成员信息
                /// </summary>
                private MemberInfo member;
                /// <summary>
                /// 类型深度
                /// </summary>
                public int Depth;
                /// <summary>
                /// 是否字段
                /// </summary>
                private bool isField;
                /// <summary>
                /// 是否共有成员
                /// </summary>
                private bool isPublic;
                /// <summary>
                /// 共有字段成员
                /// </summary>
                public FieldInfo PublicField
                {
                    get
                    {
                        return isPublic && isField ? (FieldInfo)member : null;
                    }
                }
                /// <summary>
                /// 非共有字段成员
                /// </summary>
                public FieldInfo NonPublicField
                {
                    get
                    {
                        return !isPublic && isField ? (FieldInfo)member : null;
                    }
                }
                /// <summary>
                /// 共有属性成员
                /// </summary>
                public PropertyInfo PublicProperty
                {
                    get
                    {
                        return isPublic && !isField ? (PropertyInfo)member : null;
                    }
                }
                /// <summary>
                /// 非共有属性成员
                /// </summary>
                public PropertyInfo NonPublicProperty
                {
                    get
                    {
                        return !isPublic && !isField ? (PropertyInfo)member : null;
                    }
                }
                /// <summary>
                /// 类型深度
                /// </summary>
                /// <param name="type">类型</param>
                /// <param name="field">成员字段</param>
                /// <param name="isPublic">是否共有成员</param>
                public typeDepth(Type type, FieldInfo field, bool isPublic)
                {
                    Type memberType = field.DeclaringType;
                    member = field;
                    isField = true;
                    this.isPublic = isPublic;
                    for (Depth = 0; type != memberType; ++Depth) type = type.BaseType;
                }
                /// <summary>
                /// 类型深度
                /// </summary>
                /// <param name="type">类型</param>
                /// <param name="property">成员属性</param>
                /// <param name="isPublic">是否共有成员</param>
                public typeDepth(Type type, PropertyInfo property, bool isPublic)
                {
                    Type memberType = property.DeclaringType;
                    member = property;
                    isField = false;
                    this.isPublic = isPublic;
                    for (Depth = 0; type != memberType; ++Depth) type = type.BaseType;
                }
            }
            /// <summary>
            /// 公有动态字段
            /// </summary>
            public FieldInfo[] PublicFields;
            /// <summary>
            /// 非公有动态字段
            /// </summary>
            public FieldInfo[] NonPublicFields;
            /// <summary>
            /// 公有动态属性
            /// </summary>
            public PropertyInfo[] PublicProperties;
            /// <summary>
            /// 非公有动态属性
            /// </summary>
            public PropertyInfo[] NonPublicProperties;
            /// <summary>
            /// 动态成员分组
            /// </summary>
            /// <param name="type">目标类型</param>
            /// <param name="isStatic">是否静态成员</param>
            public group(Type type, bool isStatic)
            {
                Dictionary<hashString, typeDepth> members = dictionary.CreateHashString<typeDepth>();
                BindingFlags staticFlags = isStatic ? (BindingFlags.Static | BindingFlags.FlattenHierarchy) : BindingFlags.Instance;
                typeDepth oldMember;
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | staticFlags))
                {
                    typeDepth member = new typeDepth(type, field, true);
                    hashString nameKey = field.Name;
                    if (!members.TryGetValue(nameKey, out oldMember) || member.Depth < oldMember.Depth) members[nameKey] = member;
                }
                bool isAnonymous = !isStatic && type.Name[0] == '<';
                foreach (FieldInfo field in type.GetFields(BindingFlags.NonPublic | staticFlags))
                {
                    if (field.Name[0] == '<')
                    {
                        if (isAnonymous)
                        {
                            int index = field.Name.IndexOf('>');
                            if (index != -1)
                            {
                                typeDepth member = new typeDepth(type, field, false);
                                hashString nameKey = field.Name.Substring(1, index - 1);
                                if (!members.TryGetValue(nameKey, out oldMember)) members[nameKey] = member;
                            }
                        }
                    }
                    else
                    {
                        typeDepth member = new typeDepth(type, field, false);
                        hashString nameKey = field.Name;
                        if (!members.TryGetValue(nameKey, out oldMember) || member.Depth < oldMember.Depth) members[nameKey] = member;
                    }
                }
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | staticFlags))
                {
                    if (!members.ContainsKey(property.Name))
                    {
                        typeDepth member = new typeDepth(type, property, true);
                        string name = property.Name + ".";
                        ParameterInfo[] parameters = property.GetIndexParameters();
                        if (parameters.Length != 0) name += parameters.joinString(',', value => value.ParameterType.fullName());
                        hashString nameKey = name;
                        if (!members.TryGetValue(nameKey, out oldMember) || member.Depth < oldMember.Depth) members[nameKey] = member;
                    }
                }
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.NonPublic | staticFlags))
                {
                    if (!members.ContainsKey(property.Name))
                    {
                        typeDepth member = new typeDepth(type, property, false);
                        string name = property.Name + ".";
                        ParameterInfo[] parameters = property.GetIndexParameters();
                        if (parameters.Length != 0) name += parameters.joinString(',', value => value.ParameterType.fullName());
                        hashString nameKey = name;
                        if (!members.TryGetValue(nameKey, out oldMember) || member.Depth < oldMember.Depth) members[nameKey] = member;
                    }
                }
                PublicFields = members.Values.getArray(value => value.PublicField).getFindArray(value => value != null);
                NonPublicFields = members.Values.getArray(value => value.NonPublicField).getFindArray(value => value != null);
                PublicProperties = members.Values.getArray(value => value.PublicProperty).getFindArray(value => value != null);
                NonPublicProperties = members.Values.getArray(value => value.NonPublicProperty).getFindArray(value => value != null);
            }
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        public MemberInfo Member { get; protected set; }
        /// <summary>
        /// 自定义属性集合
        /// </summary>
        private object[] attributes;
        /// <summary>
        /// 自定义属性集合(包括基类成员属性)
        /// </summary>
        private object[] baseAttributes;
        /// <summary>
        /// 成员类型
        /// </summary>
        public Type Type { get; protected set; }
        /// <summary>
        /// 成员编号
        /// </summary>
        public int MemberIndex { get; protected set; }
        /// <summary>
        /// 选择类型
        /// </summary>
        internal memberFilters Filter;
        /// <summary>
        /// XML文档注释
        /// </summary>
        protected string xmlDocument;
        /// <summary>
        /// XML文档注释
        /// </summary>
        public virtual string XmlDocument
        {
            get
            {
                if (xmlDocument == null)
                {
                    xmlDocument = Member == null ? string.Empty : (IsField ? code.xmlDocument.Get((FieldInfo)Member) : code.xmlDocument.Get((PropertyInfo)Member));
                }
                return xmlDocument.Length == 0 ? null : xmlDocument;
            }
        }
        /// <summary>
        /// 是否字段
        /// </summary>
        public bool IsField { get; protected set; }
        /// <summary>
        /// 是否可赋值
        /// </summary>
        public bool CanSet { get; protected set; }
        /// <summary>
        /// 是否可读取
        /// </summary>
        public bool CanGet { get; protected set; }
        /// <summary>
        /// 是否忽略该成员
        /// </summary>
        private bool? isIgnore;
        /// <summary>
        /// 是否忽略该成员
        /// </summary>
        public bool IsIgnore
        {
            get
            {
                if (isIgnore == null) isIgnore = Member != null && GetAttribute<ignore>(true, false) != null;
                return (bool)isIgnore;
            }
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="member">成员信息</param>
        protected memberIndex(memberIndex member)
        {
            Member = member.Member;
            Type = member.Type;
            MemberIndex = member.MemberIndex;
            Filter = member.Filter;
            IsField = member.IsField;
            CanSet = member.CanSet;
            CanGet = member.CanGet;
            isIgnore = member.isIgnore;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="member">成员信息</param>
        /// <param name="filter">选择类型</param>
        /// <param name="index">成员编号</param>
        protected memberIndex(MemberInfo member, memberFilters filter, int index)
        {
            Member = member;
            MemberIndex = index;
            Filter = filter;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="index">成员编号</param>
        protected memberIndex(int index)
        {
            MemberIndex = index;
            IsField = CanSet = CanSet = true;
            Filter = memberFilters.PublicInstance;
        }
        /// <summary>
        /// 获取自定义属性集合
        /// </summary>
        /// <typeparam name="attributeType"></typeparam>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <returns></returns>
        internal IEnumerable<attributeType> Attributes<attributeType>(bool isBaseType) where attributeType : Attribute
        {
            if (Member != null)
            {
                object[] values;
                if (isBaseType)
                {
                    if (baseAttributes == null)
                    {
                        baseAttributes = Member.GetCustomAttributes(true);
                        if (baseAttributes.Length == 0) attributes = baseAttributes;
                    }
                    values = baseAttributes;
                }
                else
                {
                    if (attributes == null) attributes = Member.GetCustomAttributes(false);
                    values = attributes;
                }
                foreach (object value in values)
                {
                    if (typeof(attributeType).IsAssignableFrom(value.GetType())) yield return (attributeType)value;
                }
            }
        }
        /// <summary>
        /// 根据成员属性获取自定义属性
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>自定义属性</returns>
        public attributeType GetAttribute<attributeType>(bool isBaseType, bool isInheritAttribute) where attributeType : Attribute
        {
            attributeType value = null;
            int minDepth = int.MaxValue;
            foreach (attributeType attribute in Attributes<attributeType>(isBaseType))
            {
                if (isInheritAttribute)
                {
                    int depth = 0;
                    for (Type type = attribute.GetType(); type != typeof(attributeType); type = type.BaseType) ++depth;
                    if (depth < minDepth)
                    {
                        if (depth == 0) return attribute;
                        minDepth = depth;
                        value = attribute;
                    }
                }
                else if (attribute.GetType() == typeof(attributeType)) return attribute;
            }
            return value;
        }
        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>自定义属性,失败返回null</returns>
        public attributeType GetSetupAttribute<attributeType>(bool isBaseType, bool isInheritAttribute) where attributeType : fastCSharp.code.ignoreMember
        {
            if (!IsIgnore)
            {
                attributeType value = GetAttribute<attributeType>(isBaseType, isInheritAttribute);
                if (value != null && value.IsSetup) return value;
            }
            return null;
        }
        /// <summary>
        /// 判断是否存在自定义属性
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>是否存在自定义属性</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal bool IsAttribute<attributeType>(bool isBaseType, bool isInheritAttribute) where attributeType : fastCSharp.code.ignoreMember
        {
            return GetSetupAttribute<attributeType>(isBaseType, isInheritAttribute) != null;
        }
        /// <summary>
        /// 判断是否忽略自定义属性
        /// </summary>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>是否忽略自定义属性</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal bool IsIgnoreAttribute<attributeType>(bool isBaseType, bool isInheritAttribute) where attributeType : fastCSharp.code.ignoreMember
        {
            if (IsIgnore) return true;
            attributeType value = GetAttribute<attributeType>(isBaseType, isInheritAttribute);
            return value != null && !value.IsSetup;
        }
        /// <summary>
        /// 根据类型获取成员信息集合
        /// </summary>
        /// <typeparam name="memberType">成员索引类型</typeparam>
        /// <typeparam name="attributeType">自定义属性类型</typeparam>
        /// <param name="members">待匹配的成员信息集合</param>
        /// <param name="isAttribute">是否匹配自定义属性类型</param>
        /// <param name="isBaseType">是否搜索父类属性</param>
        /// <param name="isInheritAttribute">是否包含继承属性</param>
        /// <returns>成员信息集合</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        protected static memberType[] find<memberType, attributeType>(memberType[] members, bool isAttribute, bool isBaseType, bool isInheritAttribute)
            where memberType : memberIndex
            where attributeType : fastCSharp.code.ignoreMember
        {
            return members.getFindArray(value => isAttribute ? value.IsAttribute<attributeType>(isBaseType, isInheritAttribute) : !value.IsIgnoreAttribute<attributeType>(isBaseType, isInheritAttribute));
        }
    }
    /// <summary>
    /// 成员索引
    /// </summary>
    /// <typeparam name="memberType">成员类型</typeparam>
    internal abstract class memberIndex<memberType> : memberIndex where memberType : MemberInfo
    {
        /// <summary>
        /// 成员信息
        /// </summary>
        public new memberType Member;
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <param name="member">成员信息</param>
        /// <param name="filter">选择类型</param>
        /// <param name="index">成员编号</param>
        protected memberIndex(memberType member, memberFilters filter, int index)
            : base(member, filter, index)
        {
            Member = member;
        }
    }

}
