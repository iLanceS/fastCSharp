using System;
using System.IO;

namespace fastCSharp.document.ajax
{
    /// <summary>
    /// 公共Ajax调用
    /// </summary>
    [fastCSharp.code.cSharp.ajax(IsPool = true, IsExportTypeScript = true)]
    class pub : fastCSharp.code.cSharp.ajax.call<pub>
    {
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="file"></param>
        public void OpenFile(string file)
        {
            fastCSharp.diagnostics.process.StartDirectory(new FileInfo(file), null);
        }
    }
}
