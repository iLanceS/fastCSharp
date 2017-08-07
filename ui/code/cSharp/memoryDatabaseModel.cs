using System;
using fastCSharp.emit;
using System.Collections.Generic;

namespace fastCSharp.code
{
    /// <summary>
    /// 内存数据库表格模型
    /// </summary>
    internal partial class memoryDatabaseModel
    {
        /// <summary>
        /// 内存数据库表格模型码生成
        /// </summary
        [auto(Name = "内存数据库表格模型", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : dataModel.cSharp<fastCSharp.code.cSharp.memoryDatabaseModel>
        {
            /// <summary>
            /// 内存数据库表格操作工具 字段名称
            /// </summary>
            public string MemoryDatabaseTableName
            {
                get { return fastCSharp.emit.memoryDatabaseTable.MemoryDatabaseTableName; }
            }
            /// <summary>
            /// 是否生成代码
            /// </summary>
            protected override bool IsCreate
            {
                get { return IsManyPrimaryKey; }
            }
        }
    }
}