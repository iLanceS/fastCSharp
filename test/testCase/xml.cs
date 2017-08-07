using System;

namespace fastCSharp.testCase
{
    /// <summary>
    /// XML序列化测试
    /// </summary>
    class xml
    {
        /// <summary>
        /// 随机对象生成参数
        /// </summary>
        private static readonly fastCSharp.emit.random.config randomConfig = new emit.random.config { IsSecondDateTime = true, IsParseFloat = true, IsNullChar = false };
        /// <summary>
        /// 带成员位图的XML序列化参数
        /// </summary>
        private static readonly fastCSharp.emit.xmlSerializer.config xmlSerializeConfig = new fastCSharp.emit.xmlSerializer.config();

        /// <summary>
        /// XML序列化测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            #region 引用类型字段成员XML序列化测试
            data.filedData filedData = fastCSharp.emit.random<data.filedData>.Create(randomConfig);
            string xml = fastCSharp.emit.xmlSerializer.ToXml(filedData);
            data.filedData newFieldData = fastCSharp.emit.xmlParser.Parse<data.filedData>(xml);
            if (!fastCSharp.emit.equals<data.filedData>.Equals(filedData, newFieldData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的引用类型字段成员XML序列化测试
            xmlSerializeConfig.MemberMap = fastCSharp.code.memberMap<data.filedData>.NewFull();
            xml = fastCSharp.emit.xmlSerializer.ToXml(filedData, xmlSerializeConfig);
            newFieldData = fastCSharp.emit.xmlParser.Parse<data.filedData>(xml);
            if (!fastCSharp.emit.equals<data.filedData>.MemberMapEquals(filedData, newFieldData, xmlSerializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

            #region 引用类型属性成员XML序列化测试
            data.propertyData propertyData = fastCSharp.emit.random<data.propertyData>.Create(randomConfig);
            xml = fastCSharp.emit.xmlSerializer.ToXml(propertyData);
            data.propertyData newPropertyData = fastCSharp.emit.xmlParser.Parse<data.propertyData>(xml);
            if (!fastCSharp.emit.equals<data.propertyData>.Equals(propertyData, newPropertyData))
            {
                return false;
            }
            #endregion

            #region 值类型字段成员XML序列化测试
            data.structFiledData structFiledData = fastCSharp.emit.random<data.structFiledData>.Create(randomConfig);
            xml = fastCSharp.emit.xmlSerializer.ToXml(structFiledData);
            data.structFiledData newStructFiledData = fastCSharp.emit.xmlParser.Parse<data.structFiledData>(xml);
            if (!fastCSharp.emit.equals<data.structFiledData>.Equals(structFiledData, newStructFiledData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的值类型字段成员XML序列化测试
            xmlSerializeConfig.MemberMap = fastCSharp.code.memberMap<data.structFiledData>.NewFull();
            xml = fastCSharp.emit.xmlSerializer.ToXml(structFiledData, xmlSerializeConfig);
            newStructFiledData = fastCSharp.emit.xmlParser.Parse<data.structFiledData>(xml);
            if (!fastCSharp.emit.equals<data.structFiledData>.MemberMapEquals(structFiledData, newStructFiledData, xmlSerializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

            #region 值类型属性成员XML序列化测试
            data.structPropertyData structPropertyData = fastCSharp.emit.random<data.structPropertyData>.Create(randomConfig);
            xml = fastCSharp.emit.xmlSerializer.ToXml(structPropertyData);
            data.structPropertyData newStructPropertyData = fastCSharp.emit.xmlParser.Parse<data.structPropertyData>(xml);
            if (!fastCSharp.emit.equals<data.structPropertyData>.Equals(structPropertyData, newStructPropertyData))
            {
                return false;
            }
            #endregion

            return true;
        }
    }
}
