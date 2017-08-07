using System;

namespace fastCSharp.code
{
    /// <summary>
    /// 禁止安装属性
    /// </summary>
    public abstract class ignoreMember : Attribute
    {
        /// <summary>
        /// 是否禁止当前安装
        /// </summary>
        public bool IsIgnoreCurrent;
        /// <summary>
        /// 是否安装[fastCSharp.code]
        /// </summary>
        public bool IsSetup
        {
            get
            {
                return !IsIgnoreCurrent;
            }
        }
    }
}