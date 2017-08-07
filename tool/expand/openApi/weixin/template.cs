using System;
using System.Drawing;
#if __IOS__
#else
using fastCSharp.drawing;
#endif

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 模板消息
    /// </summary>
    /// <typeparam name="valueType">模板类型</typeparam>
    public sealed class template<valueType>
    {
        /// <summary>
        /// 公众号微信号
        /// </summary>
        public string touser;
        /// <summary>
        /// 模板ID
        /// </summary>
        public string template_id;
        /// <summary>
        /// 
        /// </summary>
        public string url;
        /// <summary>
        /// 
        /// </summary>
        public string topcolor;
#if __IOS__
#else
        /// <summary>
        /// 
        /// </summary>
        public Color TopColor
        {
            set
            {
                topcolor = value.toSharpRRGGBB();
            }
        }
#endif
        /// <summary>
        /// 模版数据
        /// </summary>
        public valueType data;
    }
}
