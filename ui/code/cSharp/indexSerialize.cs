using System;
using System.Collections.Generic;
using fastCSharp.reflection;
using System.Reflection;
using fastCSharp.threading;

namespace fastCSharp.code
{
    /// <summary>
    /// 序列化代码生成自定义属性
    /// </summary>
    internal sealed partial class indexSerialize
    {
        /// <summary>
        /// 序列化代码生成
        /// </summary
        [auto(Name = "成员索引标识序列化", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : serializeBase.cSharp<fastCSharp.code.cSharp.indexSerialize>
        {
            /// <summary>
            /// 成员数量
            /// </summary>
            public int MemberCount
            {
                get { return memberIndexGroup.Get(type).MemberCount; }
            }
        }
    }
}
