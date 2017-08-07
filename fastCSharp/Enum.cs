using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 枚举相关操作
    /// </summary>
    public static class Enum
    {
        /// <summary>
        /// 默认枚举值
        /// </summary>
        public const int DefaultEnumValue = -1;
        /// <summary>
        /// 获取最大枚举值
        /// </summary>
        /// <typeparam name="enumType">枚举类型</typeparam>
        /// <param name="nullValue">默认空值</param>
        /// <returns>最大枚举值,失败返回默认空值</returns>
        public static int GetMaxValue<enumType>(int nullValue) where enumType : IConvertible
        {
            Type type = typeof(enumType);
            bool isEnum = type.IsEnum;
            if (isEnum)
            {
                enumType[] values = System.Enum.GetValues(type).toArray<enumType>();
                if (values.Length != 0)
                {
                    int maxValue = int.MinValue;
                    foreach (enumType value in System.Enum.GetValues(type).toArray<enumType>())
                    {
                        int intValue = value.ToInt32(null);
                        if (intValue > maxValue) maxValue = intValue;
                    }
                    return maxValue;
                }
            }
            return nullValue;
        }
        /// <summary>
        /// 获取枚举数组
        /// </summary>
        /// <typeparam name="enumType">枚举类型</typeparam>
        /// <returns>枚举数组</returns>
        public static enumType[] Array<enumType>()
        {
            Array array = System.Enum.GetValues(typeof(enumType));
            enumType[] values = new enumType[array.Length];
            int count = 0;
            foreach (enumType value in array) values[count++] = value;
            return values;
        }
        /// <summary>
        /// 获取枚举属性集合
        /// </summary>
        /// <typeparam name="enumType">枚举类型</typeparam>
        /// <typeparam name="attributeType">属性类型</typeparam>
        /// <returns>枚举属性集合</returns>
        public static attributeType[] GetAttributes<enumType, attributeType>()
            where enumType : IConvertible
            where attributeType : Attribute
        {
            int length = GetMaxValue<enumType>(-1) + 1;
            if (length != 0)
            {
                if (length >= config.pub.Default.MaxEnumArraySize) fastCSharp.log.Error.Add(typeof(enumType).ToString() + " 枚举数组过大 " + length.toString(), new System.Diagnostics.StackFrame(), false);
                int index;
                attributeType[] names = new attributeType[length];
                //Type enumAttributeType = typeof(attributeType);
                foreach (FieldInfo field in typeof(enumType).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    attributeType attribute = reflection.memberInfo.customAttribute<attributeType>(field);
                    if (attribute != null && (index = ((IConvertible)field.GetValue(null)).ToInt32(null)) < length) names[index] = attribute;
                }
                return names;
            }
            return null;
        }
    }
    /// <summary>
    /// 枚举属性获取器
    /// </summary>
    /// <typeparam name="enumType">枚举类型</typeparam>
    /// <typeparam name="attributeType">属性类型</typeparam>
    public static class Enum<enumType, attributeType>
        where enumType : IConvertible
        where attributeType : Attribute
    {
        /// <summary>
        /// 属性集合
        /// </summary>
        private static attributeType[] attributeArray;
        /// <summary>
        /// 属性集合
        /// </summary>
        internal static attributeType[] AttributeArray
        {
            get
            {
                if (attributeArray == null) attributeArray = fastCSharp.Enum.GetAttributes<enumType, attributeType>();
                return attributeArray;
            }
        }
        /// <summary>
        /// 根据索引获取属性
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>属性</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static attributeType Array(int index)
        {
            return AttributeArray.get(index, null);
        }
        /// <summary>
        /// 根据索引获取属性
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>属性</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static attributeType Array(enumType index)
        {
            return AttributeArray.get(index.ToInt32(null), null);
        }
        /// <summary>
        /// 获取属性集合
        /// </summary>
        /// <returns></returns>
        public static ICollection<attributeType> Array()
        {
            return AttributeArray;
        }
        /// <summary>
        /// 匹配自定义属性获取枚举集合
        /// </summary>
        /// <param name="isValue">自定义属性匹配器</param>
        /// <returns>枚举集合</returns>
        public static subArray<enumType> SubArray(Func<attributeType, bool> isValue)
        {
            Array enums = System.Enum.GetValues(typeof(enumType));
            enumType[] values = new enumType[enums.Length];
            int count = 0;
            foreach (enumType value in enums)
            {
                if (isValue(AttributeArray.get(value.ToInt32(null), null))) values[count++] = value;
            }
            return subArray<enumType>.Unsafe(values, 0, count);
        }
        /// <summary>
        /// 属性集合
        /// </summary>
        private static staticDictionary<int, attributeType> attributeDictionary;
        /// <summary>
        /// 属性集合
        /// </summary>
        internal static staticDictionary<int, attributeType> AttributeDictionary
        {
            get
            {
                if (attributeDictionary == null)
                {
                    subArray<keyValue<int, attributeType>> attributes = new subArray<keyValue<int, attributeType>>();
                    foreach (FieldInfo field in typeof(enumType).GetFields(BindingFlags.Public | BindingFlags.Static))
                    {
                        attributeType attribute = reflection.memberInfo.customAttribute<attributeType>(field);
                        if (attribute != null) attributes.Add(new keyValue<int, attributeType>(((enumType)field.GetValue(null)).ToInt32(null), attribute));
                    }
                    attributeDictionary = new staticDictionary<int, attributeType>(attributes.ToArray());
                }
                return attributeDictionary;
            }
        }
        /// <summary>
        /// 根据索引获取属性
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>属性</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static attributeType Dictionary(enumType index)
        {
            return AttributeDictionary.Get(index.ToInt32(null), null);
        }
        static Enum()
        {
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
