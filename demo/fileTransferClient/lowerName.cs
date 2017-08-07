using System;

namespace fastCSharp.demo.fileTransferClient
{
    /// <summary>
    /// 小写名称
    /// </summary>
    internal struct lowerName
    {
        /// <summary>
        /// 名称
        /// </summary>
        private string name;
        /// <summary>
        /// 小写名称
        /// </summary>
        private string lower;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                lower = null;
            }
        }
        /// <summary>
        /// 小写名称
        /// </summary>
        public string LowerName
        {
            get
            {
                if (lower == null && name != null) lower = name.ToLower();
                return lower;
            }
        }
    }
}
