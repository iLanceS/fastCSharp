using System;
using System.IO;
using fastCSharp.code.cSharp;
using fastCSharp.threading;

namespace fastCSharp.document.include
{
    /// <summary>
    /// 代码菜单
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsPool = true, IsReferer = false, IsPage = false, IsExportTypeScript = true, QueryName = null)]
    partial class codeMenu : webView.view<codeMenu>
    {
        /// <summary>
        /// 代码菜单
        /// </summary>
        public sealed class item
        {
            /// <summary>
            /// 代码菜单缓存
            /// </summary>
            private static readonly interlocked.dictionary<string, item> cache = new interlocked.dictionary<string, item>();
            /// <summary>
            /// 
            /// </summary>
            public FileInfo File;
            /// <summary>
            /// 代码菜单
            /// </summary>
            /// <param name="file"></param>
            private item(FileInfo file)
            {
                File = file;
            }
            /// <summary>
            /// 代码菜单
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static item Get(string file)
            {
                item item;
                if (!cache.TryGetValue(file, out item))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    cache.Set(file, item = fileInfo.Exists ? new item(fileInfo) : null);
                }
                return item;
            }
        }
        /// <summary>
        /// 代码菜单
        /// </summary>
        [fastCSharp.emit.webView.clearMember]
        public item Item;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool loadView(string file)
        {
            Item = item.Get(file);
            return true;
        }
    }
}
