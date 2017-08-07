using System;

namespace fastCSharp.code
{
    /// <summary>
    /// 数据序列化代码生成
    /// </summary>
    internal sealed partial class dataSerialize
    {
        /// <summary>
        /// 数据序列化代码生成
        /// </summary
        [auto(Name = "数据序列化", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : member<fastCSharp.code.cSharp.dataSerialize>
        {
            /// <summary>
            /// 固定类型字节数
            /// </summary>
            public int FixedSize;
            /// <summary>
            /// 空值位图字节数
            /// </summary>
            public int NullMapSize;
            /// <summary>
            /// 固定类型字节数+空值位图字节数
            /// </summary>
            public int NullMapFixedSize
            {
                get { return NullMapSize + FixedSize; }
            }
            /// <summary>
            /// 成员数量验证
            /// </summary>
            public int MemberCountVerify;
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                Members = fastCSharp.code.cSharp.dataSerialize.GetFields(type, Attribute, out MemberCountVerify, out FixedSize, out NullMapSize).ToArray();
                create(true);
            }
        }
    }
}
