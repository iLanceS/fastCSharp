using System;
using fastCSharp.code.cSharp;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using fastCSharp.net;

namespace fastCSharp.demo.chatWeb.ajax
{
    /// <summary>
    /// 公共调用
    /// </summary>
    [fastCSharp.code.cSharp.ajax(IsPool = true)]
    internal sealed class pub : fastCSharp.code.cSharp.ajax.call<pub>
    {
        /// <summary>
        /// HTML标题获取客户端
        /// </summary>
        private static readonly fastCSharp.net.htmlTitleClient.task htmlTitleClient = new net.htmlTitleClient.task(128, 1 << 11, 128 << 10);
        /// <summary>
        /// 获取HTML标题
        /// </summary>
        /// <param name="link">目标URL</param>
        /// <param name="onReturn">回调委托</param>
        public void CrawlTitle(string link, Action<returnValue<string>> onReturn)
        {
            htmlTitleClient.Get(link, title => onReturn(title));
        }
        /// <summary>
        /// Chrome粘帖图片
        /// </summary>
        /// <param name="identity">最后一张图片标识</param>
        /// <returns>图片URL集合</returns>
        public string[] PasteImage(ref int identity)
        {
            string[] fileNames = new string[form.Files.Count];
            int index = 0;
            foreach (fastCSharp.net.tcp.http.requestForm.value file in form.Files)
            {
                string fileName = file.SaveImage(WorkPath + @"upload" + Path.DirectorySeparatorChar + ((ulong)date.NowSecond.Ticks).toHex16() + ((ulong)fastCSharp.pub.Identity).toHex16());
                if (fileName != null)
                {
                    fileName = fileName.Substring(WorkPath.Length - 1);
                    fileNames[index] = Path.DirectorySeparatorChar == '/' ? fileName : fileName.replace(Path.DirectorySeparatorChar, '/');
                }
                ++index;
            }
            return fileNames;
        }
    }
}
