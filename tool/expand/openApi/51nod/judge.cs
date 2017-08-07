using System;

namespace fastCSharp.openApi._51nod
{
    /// <summary>
    /// 提交测试
    /// </summary>
    public sealed class judge
    {
        /// <summary>
        /// 程序语言
        /// </summary>
        public enum languageType : byte
        {
            /// <summary>
            /// c
            /// </summary>
            C = 1,
            /// <summary>
            /// c11
            /// </summary>
            C11 = 2,
            /// <summary>
            /// c++
            /// </summary>
            CPlus = 11,
            /// <summary>
            /// c++11
            /// </summary>
            CPlus11 = 12,
            /// <summary>
            /// c#
            /// </summary>
            CSharp = 21,
            /// <summary>
            /// java
            /// </summary>
            Java = 31,
            /// <summary>
            /// python2
            /// </summary>
            Python2 = 41,
            /// <summary>
            /// python3
            /// </summary>
            Python3 = 42,
            /// <summary>
            /// python pypy2
            /// </summary>
            PyPy2 = 45,
            /// <summary>
            /// python pypy3
            /// </summary>
            PyPy3 = 46,
            /// <summary>
            /// ruby
            /// </summary>
            Ruby = 51,
            /// <summary>
            /// php
            /// </summary>
            Php = 61,
            /// <summary>
            /// haskell
            /// </summary>
            Haskell = 71,
            /// <summary>
            /// scala
            /// </summary>
            Scala = 81,
            /// <summary>
            /// js
            /// </summary>
            Javascript = 91,
            /// <summary>
            /// go
            /// </summary>
            Go = 101,
            /// <summary>
            /// vc++
            /// </summary>
            VCPlus = 111,
            /// <summary>
            /// objective-c
            /// </summary>
            OC = 121
        }
        /// <summary>
        /// 提交测试ID
        /// </summary>
        public int Id;
        /// <summary>
        /// 题目ID
        /// </summary>
        public int ProblemId;
        /// <summary>
        /// 程序内容
        /// </summary>
        public string ProgramContent;
        /// <summary>
        /// 程序语言
        /// </summary>
        public languageType Language;
    }
}
