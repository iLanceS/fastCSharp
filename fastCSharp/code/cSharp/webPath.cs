using System;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// WEB Path 配置
    /// </summary>
    public class webPath : memberFilter.publicInstanceProperty
    {
        /// <summary>
        /// 导出二进制位标识
        /// </summary>
        public int Flag = 1;
        /// <summary>
        /// 查询名称
        /// </summary>
        public string QueryName;
        /// <summary>
        /// 服务端生成属性名称
        /// </summary>
        public string MemberName = "Path";
        /// <summary>
        /// 默认绑定类型
        /// </summary>
        public Type Type;
    }
}
