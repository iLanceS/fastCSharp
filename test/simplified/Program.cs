using System;
using System.Globalization;
using Microsoft.VisualBasic;
using System.IO;
using System.Text;

namespace fastCSharp.test.simplified
{
    class Program
    {
        /// <summary>
        /// 生成Unicode繁体转简体字符串代码
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            FileInfo file = new FileInfo(@"..\..\..\..\tool\expand\search\simplified.cs");
            if (file.Exists)
            {
                string code = File.ReadAllText(file.FullName, Encoding.UTF8), regionStart = "#region simplified", regionEnd = "#endregion simplified";
                int startIndex = code.IndexOf(regionStart), endIndex = code.IndexOf(regionEnd);
                if (startIndex == -1) Console.WriteLine("文件 " + file.FullName + " 没有找到 " + regionStart);
                else if (endIndex == -1) Console.WriteLine("文件 " + file.FullName + " 没有找到 " + regionEnd);
                else
                {
                    File.WriteAllText(file.FullName, code.Substring(0, startIndex + regionStart.Length) + @"
        private static readonly string simplifiedChars = @""" + get().Replace(@"""", @"""""") + @""";
        " + code.Substring(endIndex, code.Length - endIndex), Encoding.UTF8);
                    Console.WriteLine("文件 " + file.FullName + " 更新完毕");
                }
            }
            else Console.WriteLine("没有找到文件 " + file.FullName);
            Console.WriteLine("press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// 获取Unicode简体字符集合
        /// </summary>
        /// <returns></returns>
        unsafe static string get()
        {
            string simplified = new string((char)0, 65536);
            fixed (char* simplifiedFixed = simplified)
            {
                char* end = simplifiedFixed + 65536;
                for (char code = (char)65535; end != simplifiedFixed; *--end = code--) ;
                simplified = Strings.StrConv(simplified, VbStrConv.SimplifiedChinese, 0).ToLower();
            }
            fixed (char* simplifiedFixed = simplified)
            {
                char* end = simplifiedFixed + 65536;
                for (char code = (char)65535; end != simplifiedFixed; --code)
                {
                    --end;
                    UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(code);
                    if ((code >= 0x4E00 && code <= 0X9FA5)
                        || category == UnicodeCategory.LowercaseLetter || category == UnicodeCategory.UppercaseLetter
                        || category == UnicodeCategory.TitlecaseLetter || category == UnicodeCategory.ModifierLetter
                        || category == UnicodeCategory.OtherLetter || category == UnicodeCategory.DecimalDigitNumber
                        || category == UnicodeCategory.LetterNumber || category == UnicodeCategory.OtherNumber
                        || code == '&' || code == '.' || code == '+' || code == '#')
                    {
                        if (code > 65280 && code < 65375) *end = (char)(code - 65248);
                        else if (category == UnicodeCategory.DecimalDigitNumber) *end = (char)(48 + CharUnicodeInfo.GetDigitValue(code));
                        else if (*end == '?') *end = code;
                    }
                    else *end = ' ';
                }
                return simplified;
            }
        }
    }
}
