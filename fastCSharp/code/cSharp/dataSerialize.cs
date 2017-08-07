using System;
using System.Reflection;
using System.Collections.Generic;
using fastCSharp.reflection;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// 序列化代码生成自定义属性
    /// </summary>
    public class dataSerialize : memberFilter.publicInstanceField
    {
        /// <summary>
        /// 是否序列化成员位图
        /// </summary>
        public bool IsMemberMap;
        /// <summary>
        /// 序列化接口
        /// </summary>
        public interface ISerialize
        {
            /// <summary>
            /// 对象序列化
            /// </summary>
            /// <param name="serializer">对象序列化器</param>
            void Serialize(fastCSharp.emit.dataSerializer serializer);
            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="deSerializer">对象反序列化器</param>
            void DeSerialize(fastCSharp.emit.dataDeSerializer deSerializer);
        }
        /// <summary>
        /// 获取字段成员集合
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        /// <param name="memberCountVerify"></param>
        /// <param name="fixedSize"></param>
        /// <param name="nullMapSize"></param>
        /// <returns>字段成员集合</returns>
        public static subArray<memberInfo> GetFields(Type type, dataSerialize attribute, out int memberCountVerify, out int fixedSize, out int nullMapSize)
        {
            fieldIndex[] fieldIndexs = (fieldIndex[])typeof(fastCSharp.code.memberIndexGroup<>).MakeGenericType(type).GetMethod("GetFields", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { attribute.MemberFilter });
            subArray<memberInfo> fields = new subArray<memberInfo>(fieldIndexs.Length);
            int nullMapIndex = 0;
            fixedSize = nullMapSize = 0;
            foreach (fieldIndex field in fieldIndexs)
            {
                type = field.Member.FieldType;
                if (!type.IsPointer && (!type.IsArray || type.GetArrayRank() == 1) && !field.IsIgnore)
                {
                    fastCSharp.emit.dataSerialize.member memberAttribute = field.GetAttribute<fastCSharp.emit.dataSerialize.member>(true, true);
                    if (memberAttribute == null || memberAttribute.IsSetup)
                    {
                        memberInfo value = memberInfo.GetSerialize(field);
                        if (type != typeof(bool)) fixedSize += value.SerializeFixedSize;
                        nullMapSize += value.NullMapSize;
                        if (value.NullMapSize == 2)
                        {
                            value.SerializeNullMapIndex = nullMapIndex;
                            nullMapIndex += 2;
                            --fixedSize;
                        }
                        fields.Add(value);
                    }
                }
            }
            memberCountVerify = fields.length + 0x40000000;
            fixedSize = (fixedSize + 3) & (int.MaxValue - 3);
            nullMapSize = ((nullMapSize + 31) >> 5) << 2;
            fields.Sort(memberInfo.SerializeFixedSizeSort);
            foreach (memberInfo value in fields)
            {
                if (value.NullMapSize == 1) value.SerializeNullMapIndex = nullMapIndex++;
            }
            return fields;
        }
        /// <summary>
        /// 固定类型字节数
        /// </summary>
        internal static readonly Dictionary<Type, byte> FixedSizes;
        static unsafe dataSerialize()
        {
            FixedSizes = dictionary.CreateOnly<Type, byte>();
            FixedSizes.Add(typeof(bool), sizeof(bool));
            FixedSizes.Add(typeof(byte), sizeof(byte));
            FixedSizes.Add(typeof(sbyte), sizeof(sbyte));
            FixedSizes.Add(typeof(short), sizeof(short));
            FixedSizes.Add(typeof(ushort), sizeof(ushort));
            FixedSizes.Add(typeof(int), sizeof(int));
            FixedSizes.Add(typeof(uint), sizeof(uint));
            FixedSizes.Add(typeof(long), sizeof(long));
            FixedSizes.Add(typeof(ulong), sizeof(ulong));
            FixedSizes.Add(typeof(char), sizeof(char));
            FixedSizes.Add(typeof(DateTime), sizeof(long));
            FixedSizes.Add(typeof(float), sizeof(float));
            FixedSizes.Add(typeof(double), sizeof(double));
            FixedSizes.Add(typeof(decimal), sizeof(decimal));
            FixedSizes.Add(typeof(Guid), (byte)sizeof(Guid));
        }
    }
}
namespace fastCSharp.code
{
    /// <summary>
    /// 成员信息
    /// </summary>
    public partial class memberInfo
    {
        /// <summary>
        /// 空值位图索引
        /// </summary>
        public int SerializeNullMapIndex;
        /// <summary>
        /// 基本序列化字节数
        /// </summary>
        public byte SerializeFixedSize;
        /// <summary>
        /// 空值位图位数
        /// </summary>
        internal byte NullMapSize;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static memberInfo GetSerialize(fieldIndex field)
        {
            memberInfo member = new memberInfo(field);
            Type type = field.Member.FieldType;
            if (type.IsEnum) cSharp.dataSerialize.FixedSizes.TryGetValue(type.GetEnumUnderlyingType(), out member.SerializeFixedSize);
            else if (type.IsValueType)
            {
                Type nullType = type.nullableType();
                if (nullType == null)
                {
                    cSharp.dataSerialize.FixedSizes.TryGetValue(type, out member.SerializeFixedSize);
                    if (type == typeof(bool)) member.NullMapSize = 1;
                }
                else
                {
                    member.NullMapSize = type == typeof(bool?) ? (byte)2 : (byte)1;
                    cSharp.dataSerialize.FixedSizes.TryGetValue(nullType, out member.SerializeFixedSize);
                }
            }
            else member.NullMapSize = 1;
            return member;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        internal static int SerializeFixedSizeSort(memberInfo left, memberInfo right)
        {
            return (int)((uint)right.SerializeFixedSize & (0U - (uint)right.SerializeFixedSize)) - (int)((uint)left.SerializeFixedSize & (0U - (uint)left.SerializeFixedSize));
        }
    }
}
