using System;
using System.Drawing;

namespace fastCSharp.drawing
{
    /// <summary>
    /// 颜色
    /// </summary>
    public static class color
    {
        /// <summary>
        /// 颜色值转换成字符串 #RRGGBB
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static unsafe string toSharpRRGGBB(this Color color)
        {
            string value = String.FastAllocateString(7);
            fixed (char* valueFixed = value)
            {
                *valueFixed = '#';
                *(valueFixed + 1) = (char)numberExpand.ToHex((uint)color.R >> 4);
                *(valueFixed + 2) = (char)numberExpand.ToHex((uint)color.R & 0xf);
                *(valueFixed + 3) = (char)numberExpand.ToHex((uint)color.G >> 4);
                *(valueFixed + 4) = (char)numberExpand.ToHex((uint)color.G & 0xf);
                *(valueFixed + 5) = (char)numberExpand.ToHex((uint)color.B >> 4);
                *(valueFixed + 6) = (char)numberExpand.ToHex((uint)color.B & 0xf);
            }
            return value;
        }
    }
}
