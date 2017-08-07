using System;
using fastCSharp;
using System.Runtime.CompilerServices;

namespace fastCSharp.web
{
    /// <summary>
    /// HTML代码参数及其相关操作
    /// </summary>
    public static class html
    {
        /// <summary>
        /// 文档类型属性
        /// </summary>
        public sealed class docInfo : Attribute
        {
            /// <summary>
            /// 标准文档类型头部
            /// </summary>
            public string Html;
        }
        /// <summary>
        /// 标准引用类型
        /// </summary>
        public enum docType
        {
            /// <summary>
            /// 过渡(HTML4.01)
            /// </summary>
            [docInfo(Html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">")]
            Transitional = 0,
            /// <summary>
            /// 严格(不能使用任何表现层的标识和属性，例如[br])
            /// </summary>
            [docInfo(Html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">")]
            Strict,
            /// <summary>
            /// 框架(专门针对框架页面设计使用的DTD，如果你的页面中包含有框架，需要采用这种DTD)
            /// </summary>
            [docInfo(Html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">")]
            Frameset,
            /// <summary>
            /// 
            /// </summary>
            Xhtml11,
            /// <summary>
            /// HTML5
            /// </summary>
            [docInfo(Html = @"<!DOCTYPE html>")]
            Html5,
        }
        /// <summary>
        /// 标准文档集合
        /// </summary>
        private static readonly docInfo[] docTypes = fastCSharp.Enum.GetAttributes<docType, docInfo>();
        /// <summary>
        /// 获取标准引用代码
        /// </summary>
        /// <returns>文档类型</returns>
        public static string GetHtml(this docType type)
        {
            int typeIndex = (int)type;
            if (typeIndex < 0 || typeIndex >= docTypes.Length) typeIndex = 0;
            return docTypes[typeIndex].Html + @"
<html xmlns=""http://www.w3.org/1999/xhtml"">
";
        }
        /// <summary>
        /// 字符集类型属性
        /// </summary>
        public sealed class charsetInfo : Attribute
        {
            /// <summary>
            /// 字符串表示
            /// </summary>
            public string CharsetString;
        }

        /// <summary>
        /// 字符集类型
        /// </summary>
        public enum charsetType
        {
            /// <summary>
            /// UTF-8
            /// </summary>
            [charsetInfo(CharsetString = "UTF-8")]
            Utf8,
            /// <summary>
            /// GB2312
            /// </summary>
            [charsetInfo(CharsetString = "GB2312")]
            Gb2312,
        }
        /// <summary>
        /// 字符集类型名称集合
        /// </summary>
        private static readonly charsetInfo[] CharsetTypes = fastCSharp.Enum.GetAttributes<charsetType, charsetInfo>();
        /// <summary>
        /// 获取字符集代码
        /// </summary>
        /// <returns>字符集代码</returns>
        public static string GetHtml(this charsetType type)
        {
            int typeIndex = (int)type;
            if (typeIndex >= CharsetTypes.Length) typeIndex = -1;
            string html = string.Empty;
            if (typeIndex >= 0) html = @"<meta http-equiv=""content-type"" content=""text/html; charset=" + CharsetTypes[typeIndex].CharsetString + @""">
";
            return html;
        }
        /// <summary>
        /// 注释开始
        /// </summary>
        public const string NoteStart = @"
<![CDATA[
";
        /// <summary>
        /// 注释结束
        /// </summary>
        public const string NoteEnd = @"
]]>
";
        /// <summary>
        /// javscript开始
        /// </summary>
        public const string JsStart = @"
<script language=""javascript"" type=""text/javascript"">
//<![CDATA[
";
        /// <summary>
        /// javscript结束
        /// </summary>
        public const string JsEnd = @"
//]]>
</script>
";
        /// <summary>
        /// 加载js文件
        /// </summary>
        /// <param name="fileName">被加载的js文件地址</param>
        /// <returns>加载js文件的HTML代码</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string JsFile(string fileName)
        {
            return @"<script language=""javascript"" type=""text/javascript"" src=""" + fileName + @"""></script>";
        }
        /// <summary>
        /// style开始
        /// </summary>
        public const string StyleStart = @"
<style type=""text/css"">
<![CDATA[
";
        /// <summary>
        /// style结束
        /// </summary>
        public const string StyletEnd = @"
]]>
</style>
";
        /// <summary>
        /// 加载css文件
        /// </summary>
        /// <param name="fileName">被加载的css文件地址</param>
        /// <returns>加载css文件的HTML代码</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static string CssFile(string fileName)
        {
            return @"<style type=""text/css"" link=""" + fileName + @"""></style>";
        }
        /// <summary>
        /// 允许不回合的标签名称唯一哈希
        /// </summary>
        public struct canNonRoundTagName : IEquatable<canNonRoundTagName>
        {
            /// <summary>
            /// 允许不回合的标签名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">允许不回合的标签名称</param>
            /// <returns>允许不回合的标签名称唯一哈希</returns>
            public static implicit operator canNonRoundTagName(string name) { return new canNonRoundTagName { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                if (Name.Length == 0) return 1;
                int code = (Name[Name.Length - 1] << 7) + Name[0];
                return ((code >> 5) ^ code) & ((1 << 5) - 1);
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(canNonRoundTagName other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((canNonRoundTagName)obj);
            }
        }
        /// <summary>
        /// 允许不回合的标签名称集合
        /// </summary>
        public static readonly uniqueHashSet<canNonRoundTagName> CanNonRoundTagNames = new uniqueHashSet<canNonRoundTagName>(new canNonRoundTagName[] { "area", "areatext", "basefont", "br", "col", "colgroup", "hr", "img", "input", "li", "p", "spacer" }, 27);
        /// <summary>
        /// 必须回合的标签名称唯一哈希
        /// </summary>
        public struct mustRoundTagName : IEquatable<mustRoundTagName>
        {
            /// <summary>
            /// 必须回合的标签名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">必须回合的标签名称</param>
            /// <returns>必须回合的标签名称唯一哈希</returns>
            public static implicit operator mustRoundTagName(string name) { return new mustRoundTagName { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                if (Name.Length == 0) return 0;
                int code = (Name[Name.Length >> 1] << 14) + (Name[0] << 7) + Name[Name.Length - 1];
                return ((code >> 15) ^ (code >> 13) ^ (code >> 1)) & ((1 << 8) - 1);
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(mustRoundTagName other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((mustRoundTagName)obj);
            }
        }
        /// <summary>
        /// 必须回合的标签名称集合
        /// </summary>
        public static readonly uniqueHashSet<mustRoundTagName> MustRoundTagNames = new uniqueHashSet<mustRoundTagName>(new mustRoundTagName[] { "a", "b", "bgsound", "big", "body", "button", "caption", "center", "div", "em", "embed", "font", "form", "h1", "h2", "h3", "h4", "h5", "h6", "hn", "html", "i", "iframe", "map", "marquee", "multicol", "nobr", "ol", "option", "pre", "s", "select", "small", "span", "strike", "strong", "sub", "sup", "table", "tbody", "td", "textarea", "tfoot", "th", "thead", "tr", "u", "ul" }, 239);
        /// <summary>
        /// 脚本安全属性名称
        /// </summary>
        public struct safeAttribute : IEquatable<safeAttribute>
        {
            /// <summary>
            /// 脚本安全属性名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">脚本安全属性名称</param>
            /// <returns>脚本安全属性名称唯一哈希</returns>
            public static implicit operator safeAttribute(string name) { return new safeAttribute { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                if (Name.Length < 3) return 0;
                int code = (Name[Name.Length - 2] << 14) + (Name[Name.Length >> 1] << 7) + Name[Name.Length >> 3];
                return ((code >> 8) ^ (code >> 3) ^ (code >> 1)) & ((1 << 8) - 1);
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(safeAttribute other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((safeAttribute)obj);
            }
        }
        /// <summary>
        /// 脚本安全属性名称集合
        /// </summary>
        public static readonly uniqueHashSet<safeAttribute> SafeAttributes = new uniqueHashSet<safeAttribute>(new safeAttribute[] { "align", "allowtransparency", "alt", "behavior", "bgcolor", "border", "bordercolor", "bordercolordark", "bordercolorlight", "cellpadding", "cellspacing", "checked", "class", "clear", "color", "cols", "colspan", "controls", "coords", "direction", "face", "frame", "frameborder", "gutter", "height", "hspace", "loop", "loopdelay", "marginheight", "marginwidth", "maxlength", "method", "multiple", "rows", "rowspan", "rules", "scrollamount", "scrolldelay", "scrolling", "selected", "shape", "size", "span", "start", "target", "title", "type", "unselectable", "usemap", "valign", "value", "vspace", "width", "wrap" }, 253);
        /// <summary>
        /// URI属性名称唯一哈希
        /// </summary>
        public struct uriAttribute : IEquatable<uriAttribute>
        {
            /// <summary>
            /// URI属性名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">URI属性名称</param>
            /// <returns>URI属性名称唯一哈希</returns>
            public static implicit operator uriAttribute(string name) { return new uriAttribute { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                return Name[0] & 7;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(uriAttribute other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((uriAttribute)obj);
            }
        }
        /// <summary>
        /// URI属性名称集合
        /// </summary>
        public static readonly uniqueHashSet<uriAttribute> UriAttributes = new uniqueHashSet<uriAttribute>(new uriAttribute[] { "background", "dynsrc", "href", "src" }, 5);
        /// <summary>
        /// 安全样式名称唯一哈希
        /// </summary>
        public struct safeStyleAttribute : IEquatable<safeStyleAttribute>
        {
            /// <summary>
            /// 安全样式名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">安全样式名称</param>
            /// <returns>安全样式名称唯一哈希</returns>
            public static implicit operator safeStyleAttribute(string name) { return new safeStyleAttribute { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                return Name.Length < 4 ? 0 : Name[Name.Length - 4] & 7;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(safeStyleAttribute other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((safeStyleAttribute)obj);
            }
        }
        /// <summary>
        /// 安全样式名称集合
        /// </summary>
        public static readonly uniqueHashSet<safeStyleAttribute> SafeStyleAttributes = new uniqueHashSet<safeStyleAttribute>(new safeStyleAttribute[] { "font", "font-family", "font-size", "font-weight", "color", "text-decoration" }, 8);
        /// <summary>
        /// 非解析标签名称唯一哈希
        /// </summary>
        public struct nonanalyticTagName : IEquatable<nonanalyticTagName>
        {
            /// <summary>
            /// 非解析标签名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">非解析标签名称</param>
            /// <returns>非解析标签名称唯一哈希</returns>
            public static implicit operator nonanalyticTagName(string name) { return new nonanalyticTagName { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                if (Name.Length == 0) return 2;
                return (Name[Name.Length - 1] >> 2) & ((1 << 3) - 1);
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(nonanalyticTagName other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((nonanalyticTagName)obj);
            }
        }
        /// <summary>
        /// 非解析标签名称集合
        /// </summary>
        public static readonly uniqueHashSet<nonanalyticTagName> NonanalyticTagNames = new uniqueHashSet<nonanalyticTagName>(new nonanalyticTagName[] { "script", "style", "!", "/" }, 6);
        /// <summary>
        /// 非文本标签名称唯一哈希
        /// </summary>
        public struct noTextTagName : IEquatable<noTextTagName>
        {
            /// <summary>
            /// 非文本标签名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 隐式转换
            /// </summary>
            /// <param name="name">非文本标签名称</param>
            /// <returns>非文本标签名称唯一哈希</returns>
            public static implicit operator noTextTagName(string name) { return new noTextTagName { Name = name }; }
            /// <summary>
            /// 获取哈希值
            /// </summary>
            /// <returns>哈希值</returns>
            public override int GetHashCode()
            {
                if (Name.Length == 0) return 5;
                int code = (Name[0] << 7) + Name[Name.Length >> 2];
                return ((code >> 7) ^ (code >> 2)) & ((1 << 4) - 1);
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="other">待匹配数据</param>
            /// <returns>是否相等</returns>
            public bool Equals(noTextTagName other)
            {
                return Name == other.Name;
            }
            /// <summary>
            /// 判断是否相等
            /// </summary>
            /// <param name="obj">待匹配数据</param>
            /// <returns>是否相等</returns>
            public override bool Equals(object obj)
            {
                return Equals((noTextTagName)obj);
            }
        }
        /// <summary>
        /// 非文本标签名称集合
        /// </summary>
        public static readonly uniqueHashSet<noTextTagName> NoTextTagNames = new uniqueHashSet<noTextTagName>(new noTextTagName[] { "script", "style", "pre", "areatext", "!", "/", "input", "iframe", "img", "link", "head" }, 15);
        /// <summary>
        /// HTML编码器
        /// </summary>
        public interface IEncoder
        {
            /// <summary>
            /// 文本转HTML
            /// </summary>
            /// <param name="value">文本值</param>
            /// <returns>HTML编码</returns>
            string ToHtml(string value);
            /// <summary>
            /// 文本转HTML
            /// </summary>
            /// <param name="value">文本值</param>
            /// <returns>HTML编码</returns>
            void ToHtml(ref subString value);
            /// <summary>
            /// 文本转HTML
            /// </summary>
            /// <param name="value">文本值</param>
            /// <param name="stream">HTML编码流</param>
            void ToHtml(ref subString value, unmanagedStream stream);
        }
        /// <summary>
        /// HTML编码器
        /// </summary>
        internal unsafe sealed class encoder : IEncoder
        {
            /// <summary>
            /// HTML转义字符集合
            /// </summary>
            private readonly uint* htmls;
            /// <summary>
            /// 最大值
            /// </summary>
            private readonly int size;
            /// <summary>
            /// HTML编码器
            /// </summary>
            /// <param name="htmls">HTML转义字符集合</param>
            public encoder(string htmls)
            {
                size = 0;
                foreach (char htmlChar in htmls)
                {
                    if (htmlChar > size) size = htmlChar;
                }
                this.htmls = unmanaged.GetStatic(++size * sizeof(uint), true).UInt;
                foreach (char value in htmls)
                {
                    int div = (value * (int)number.Div10_16Mul) >> number.Div10_16Shift;
                    this.htmls[value] = (uint)(((value - div * 10) << 16) | div | 0x300030);
                }
            }
            /// <summary>
            /// HTML转义
            /// </summary>
            /// <param name="start">起始位置</param>
            /// <param name="end">结束位置</param>
            /// <param name="write">写入位置</param>
            private unsafe void toHtml(char* start, char* end, char* write)
            {
                while (start != end)
                {
                    char code = *start++;
                    if (code < size)
                    {
                        uint html = htmls[code];
                        if (html == 0) *write++ = code;
                        else
                        {
                            *(int*)write = '&' + ('#' << 16);
                            write += 2;
                            *(uint*)write = html;
                            write += 2;
                            *write++ = ';';
                        }
                    }
                    else *write++ = code;
                }
            }
            /// <summary>
            /// 文本转HTML
            /// </summary>
            /// <param name="value">文本值</param>
            /// <returns>HTML编码</returns>
            public unsafe string ToHtml(string value)
            {
                if (value != null)
                {
                    int length = value.Length;
                    fixed (char* valueFixed = value)
                    {
                        char* end = valueFixed + length;
                        int count = encodeCount(valueFixed, end);
                        if (count != 0)
                        {
                            value = fastCSharp.String.FastAllocateString(length += count << 2);
                            fixed (char* data = value) toHtml(valueFixed, end, data);
                        }
                    }
                }
                return value;
            }
            /// <summary>
            /// 文本转HTML
            /// </summary>
            /// <param name="value">文本值</param>
            /// <returns>HTML编码</returns>
            public unsafe void ToHtml(ref subString value)
            {
                if (value.Length != 0)
                {
                    int length = value.Length;
                    fixed (char* valueFixed = value.value)
                    {
                        char* start = valueFixed + value.StartIndex, end = start + length;
                        int count = encodeCount(start, end);
                        if (count != 0)
                        {
                            string newValue = fastCSharp.String.FastAllocateString(length += count << 2);
                            fixed (char* data = newValue) toHtml(start, end, data);
                            value.UnsafeSet(newValue, 0, newValue.Length);
                        }
                    }
                }
            }
            /// <summary>
            /// 文本转HTML
            /// </summary>
            /// <param name="value">文本值</param>
            /// <param name="stream">HTML编码流</param>
            public unsafe void ToHtml(ref subString value, unmanagedStream stream)
            {
                if (value.Length != 0)
                {
                    if (stream == null) log.Error.Throw(log.exceptionType.Null);
                    int length = value.Length;
                    fixed (char* valueFixed = value.value)
                    {
                        char* start = valueFixed + value.StartIndex, end = start + length;
                        int count = encodeCount(start, end);
                        if (count == 0)
                        {
                            stream.PrepLength(length <<= 1);
                            unsafer.memory.Copy(start, stream.CurrentData, length);
                        }
                        else
                        {
                            length += count << 2;
                            stream.PrepLength(length <<= 1);
                            toHtml(start, end, (char*)stream.CurrentData);
                        }
                        stream.UnsafeAddLength(length);
                    }
                }
            }
            /// <summary>
            /// 计算编码字符数量
            /// </summary>
            /// <param name="start">起始位置</param>
            /// <param name="end">结束位置</param>
            /// <returns>编码字符数量</returns>
            private unsafe int encodeCount(char* start, char* end)
            {
                int count = 0;
                while (start != end)
                {
                    if (*start < size && htmls[*start] != 0) ++count;
                    ++start;
                }
                return count;
            }
        }
        /// <summary>
        /// 默认HTML编码器
        /// </summary>
        internal readonly static encoder HtmlEncoder = new encoder(@"& <>""'");
        /// <summary>
        /// 默认HTML编码器
        /// </summary>
        public static IEncoder HtmlIEncoder { get { return HtmlEncoder; } }
        /// <summary>
        /// TextArea编码器
        /// </summary>
        internal readonly static encoder TextAreaEncoder = new encoder(@"&<>");
        /// <summary>
        /// TextArea编码器
        /// </summary>
        public static IEncoder TextAreaIEncoder { get { return TextAreaEncoder; } }
    }
}
