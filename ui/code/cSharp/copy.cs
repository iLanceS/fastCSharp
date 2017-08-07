using System;
using System.Reflection;
using fastCSharp.reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// 成员复制代码生成
    /// </summary
    [auto(Name = "成员复制", DependType = typeof(coder.cSharper))]
    internal sealed partial class copy : cSharper
    {
        /// <summary>
        /// 成员位图
        /// </summary>
        private fastCSharp.ui.code.memberMap.cSharp memberMap = new fastCSharp.ui.code.memberMap.cSharp();
        /// <summary>
        /// 成员集合
        /// </summary>
        public code.memberInfo[] Members;
        /// <summary>
        /// 生成成员位图
        /// </summary>
        /// <param name="type">类型</param>
        public void Create(Type type)
        {
            Members = code.memberInfo.GetMembers(this.type = type, code.memberFilters.Instance)
                .getFindArray(value => value.CanGet && value.CanSet && !value.IsIgnore);
            memberMap.Create(type);
            create(true);
        }
    }
}
