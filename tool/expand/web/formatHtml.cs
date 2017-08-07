using System;
using System.Collections.Generic;

namespace fastCSharp.web
{
    /// <summary>
    /// HTML安全格式化
    /// </summary>
    public static class formatHtml
    {
        /// <summary>
        /// 允许tag名称集合
        /// </summary>
        public static readonly string[] TagNames = new string[] { "a", "b", "big", "blockquote", "br", "center", "code", "dd", "del", "div", "dl", "dt", "em", "font", "h1", "h2", "h3", "h4", "h5", "h6", "hr", "i", "img", "ins", "li", "ol", "p", "pre", "s", "small", "span", "strike", "strong", "sub", "sup", "table", "tbody", "td", "th", "thead", "title", "tr", "u", "ul" };
        /// <summary>
        /// 允许tag名称集合
        /// </summary>
        private static readonly fastCSharp.stateSearcher.ascii<string> tagNames = new fastCSharp.stateSearcher.ascii<string>(TagNames, TagNames, true);
        /// <summary>
        /// 安全格式化
        /// </summary>
        /// <param name="html">HTML</param>
        /// <returns>HTML</returns>
        public static unsafe string Format(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                htmlNode document = new htmlNode(html);
                document.Remove(node => node.TagName != null && !tagNames.ContainsKey(node.TagName));
                foreach (htmlNode node in document.Nodes)
                {
                    foreach (string name in node.AttributeNames)
                    {
                        if (!fastCSharp.web.html.SafeAttributes.Contains(name))
                        {
                            if (name == "style") node[name] = formatStyle(node[name]);
                            else if (fastCSharp.web.html.UriAttributes.Contains(name))
                            {
                                if (!IsHttpOrDefalut(node[name])) node[name] = null;
                            }
                            else node[name] = null;
                        }
                    }
                    if (node.TagName != null && node.TagName.Length == 1 && node.TagName[0] == 'a')
                    {
                        string href = node["href"];
                        if (href != null && href.Length != 0 && href[0] != '/') node["target"] = "_blank";
                    }
                }
                return document.Html(true);
            }
            return html;
        }
        /// <summary>
        /// 格式化样式表
        /// </summary>
        /// <param name="style">样式表</param>
        /// <returns>样式表</returns>
        private static unsafe string formatStyle(string style)
        {
            if (style != null)
            {
                Dictionary<subString, subString> values = dictionary.CreateSubString<subString>();
                foreach (subString value in style.split(';'))
                {
                    int index = value.IndexOf(':');
                    if (index != -1)
                    {
                        subString name = value.Substring(0, index).toLower();
                        //showjim 修改为数组索引模式
                        if (fastCSharp.web.html.SafeStyleAttributes.Contains(name.ToString())) values[name] = value.Substring(++index);
                    }
                }
                if (values.Count != 0)
                {
                    int length = (values.Count << 1) - 1;
                    foreach (KeyValuePair<subString, subString> value in values) length += value.Key.Length + value.Value.Length;
                    string newStyle = fastCSharp.String.FastAllocateString(length);
                    byte* bits = htmlNode.Bits.Byte;
                    fixed (char* newStyleFixed = newStyle, styleFixed = style)
                    {
                        char* write = newStyleFixed;
                        foreach (KeyValuePair<subString, subString> value in values)
                        {
                            if (write != newStyleFixed) *write++ = ';';
                            for (char* start = styleFixed + value.Key.StartIndex, end = start + value.Key.Length; start != end; *write++ = *start++) ;
                            *write++ = ':';
                            for (char* start = styleFixed + value.Value.StartIndex, end = start + value.Value.Length; start != end; ++start)
                            {
                                *write++ = ((bits[*(byte*)start] & htmlNode.CssFilterBit) | *(((byte*)start) + 1)) == 0 ? ' ' : *start;
                            }
                        }
                    }
                    return newStyle;
                }
            }
            return null;
        }
        /// <summary>
        /// 判断连接地址是否以 http:// 或者 https:// 或者 // 开头
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public unsafe static bool IsHttpOrDefalut(string url)
        {
            int length = url.length();
            if (length > 7 && url[6] == '/')
            {
                fixed (char* urlFixed = url)
                {
                    if ((*(int*)urlFixed | 0x200020) == 'h' + ('t' << 16) && (*(int*)(urlFixed + 2) | 0x200020) == 't' + ('p' << 16))
                    {
                        if (*(int*)(urlFixed + 4) == ':' + ('/' << 16)) return true;
                        else if ((*(int*)(urlFixed + 4) | 0x20) == 's' + (':' << 16) && urlFixed[7] == '/') return true;
                    }
                }
            }
            if (length > 2) return url[0] == '/' && url[1] == '/';
            return false;
        }

        static unsafe formatHtml()
        {
            byte* bits = htmlNode.Bits.Byte;
            bits['<'] &= htmlNode.CssFilterBit ^ 255;
            bits['>'] &= htmlNode.CssFilterBit ^ 255;
            bits['&'] &= htmlNode.CssFilterBit ^ 255;
            bits['"'] &= htmlNode.CssFilterBit ^ 255;
            bits['\''] &= htmlNode.CssFilterBit ^ 255;
        }
    }
}
