using System;

namespace fastCSharp.config
{
    /// <summary>
    /// 中文分词+搜索 相关参数
    /// </summary>
    public sealed class search
    {
        /// <summary>
        /// 是否初始化搜索模块
        /// </summary>
        private bool isSearch = !config.pub.Default.IsDebug;
        /// <summary>
        /// 是否初始化搜索模块
        /// </summary>
        public bool IsSearch
        {
            get { return isSearch; }
        }
        /// <summary>
        /// 中文分词文本文件名称
        /// </summary>
        private string wordTxtFileName = fastCSharp.pub.ApplicationPath + fastCSharp.pub.fastCSharp + ".search.word.txt";
        /// <summary>
        /// 中文分词文本文件名称
        /// </summary>
        public string WordTxtFileName
        {
            get { return wordTxtFileName; }
        }
        /// <summary>
        /// 中文分词序列化文件名称
        /// </summary>
        private string wordSerializeFileName = fastCSharp.pub.ApplicationPath + fastCSharp.pub.fastCSharp + ".search.word.data";
        /// <summary>
        /// 中文分词序列化文件名称
        /// </summary>
        public string WordSerializeFileName
        {
            get { return wordSerializeFileName; }
        }
        /// <summary>
        /// 中文分词+搜索 相关参数
        /// </summary>
        private search()
        {
            pub.LoadConfig(this);
        }
        /// <summary>
        /// 默认中文分词+搜索 相关参数
        /// </summary>
        public static readonly search Default = new search();
    }
}
