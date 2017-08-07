using System;

namespace fastCSharp.config
{
    /// <summary>
    /// 数据库相关参数
    /// </summary>
    public abstract class database
    {
        /// <summary>
        /// 默认自增ID列名称
        /// </summary>
        private string defaultIdentityName = "id";
        /// <summary>
        /// 默认自增ID列名称
        /// </summary>
        public string DefaultIdentityName
        {
            get { return defaultIdentityName; }
        }
    }
}
