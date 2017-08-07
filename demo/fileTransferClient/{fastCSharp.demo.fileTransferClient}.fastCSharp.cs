//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.fileTransferClient
{
        internal partial class autoPath
        {
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.code.ignore]
            public struct primaryKey : IEquatable<primaryKey>
            {
                public int UserId;
                public string ServerPath;
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="other">关键字</param>
                /// <returns>是否相等</returns>
                public bool Equals(primaryKey other)
                {
                    return UserId/**/.Equals(other.UserId) && ServerPath/**/.Equals(other.ServerPath);
                }
                /// <summary>
                /// 哈希编码
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return UserId.GetHashCode() ^ ServerPath/**/.GetHashCode();
                }
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public override bool Equals(object obj)
                {
                    return Equals((primaryKey)obj);
                }
            }
        }
}
#endif