using System;

namespace fastCSharp.test.uniqueHashCode
{
    class Program
    {
        /// <summary>
        /// 唯一HASH计算
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string[] keys = new string[] { "avi", "bmp", "css", "cur", "doc", "docx", "eot", "gif", "htm", "html", "ico", "jpg", "jpeg", "js", "mp3", "mp4", "mpg", "otf", "pdf", "png", "rar", "rm", "rmvb", "svg", "swf", "txt", "wav", "woff", "xml", "xls", "xlsx", "zip", "z7" };
            coder coder = new coder(keys, 8, 0, 8);
            string code = coder.Code();
            string value = coder.ToString();
            Console.WriteLine(value + @"

" + code);
            Console.ReadKey();
        }
    }
}
