using System;

namespace fastCSharp.testCase
{
    /// <summary>
    /// 索引识别的二进制序列化测试(用于内存数据库)
    /// </summary>
    class indexSerialize
    {
        /// <summary>
        /// 带成员位图的二进制序列化配置参数
        /// </summary>
        private static readonly fastCSharp.emit.indexSerializer.config serializeConfig = new fastCSharp.emit.indexSerializer.config();
        /// <summary>
        /// 索引识别的二进制序列化测试(用于内存数据库)
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            #region 引用类型二进制序列化测试
            data.filedData filedData = fastCSharp.emit.random<data.filedData>.Create();
            byte[] data = fastCSharp.emit.indexSerializer.Serialize(filedData);
            data.filedData newFieldData = fastCSharp.emit.indexDeSerializer.DeSerialize<data.filedData>(data);
            if (!fastCSharp.emit.equals<data.filedData>.Equals(filedData, newFieldData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的引用类型二进制序列化测试
            serializeConfig.MemberMap = fastCSharp.code.memberMap<data.filedData>.NewFull();
            data = fastCSharp.emit.indexSerializer.Serialize(filedData, serializeConfig);
            newFieldData = fastCSharp.emit.indexDeSerializer.DeSerialize<data.filedData>(data);
            if (!fastCSharp.emit.equals<data.filedData>.MemberMapEquals(filedData, newFieldData, serializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

            #region 值类型二进制序列化测试
            data.structFiledData structFiledData = fastCSharp.emit.random<data.structFiledData>.Create();
            data = fastCSharp.emit.indexSerializer.Serialize(structFiledData);
            data.structFiledData newStructFiledData = fastCSharp.emit.indexDeSerializer.DeSerialize<data.structFiledData>(data);
            if (!fastCSharp.emit.equals<data.structFiledData>.Equals(structFiledData, newStructFiledData))
            {
                return false;
            }
            #endregion

            #region 带成员位图的值类型二进制序列化测试
            serializeConfig.MemberMap = fastCSharp.code.memberMap<data.structFiledData>.NewFull();
            data = fastCSharp.emit.indexSerializer.Serialize(structFiledData, serializeConfig);
            newStructFiledData = fastCSharp.emit.indexDeSerializer.DeSerialize<data.structFiledData>(data);
            if (!fastCSharp.emit.equals<data.structFiledData>.MemberMapEquals(structFiledData, newStructFiledData, serializeConfig.MemberMap))
            {
                return false;
            }
            #endregion

            return true;
        }
    }
}
