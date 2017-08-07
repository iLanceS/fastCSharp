using System;

namespace fastCSharp.openApi._51nod
{
    /// <summary>
    /// 外部提交结果
    /// </summary>
    public sealed class judgeResult
    {
        /// <summary>
        /// 提交测试ID
        /// </summary>
        public int Id;
        ///// <summary>
        ///// 最大用时毫秒数
        ///// </summary>
        //public int TimeUse;
        ///// <summary>
        ///// 最大内存使用字节数
        ///// </summary>
        //public long MemoryUse;
        /// <summary>
        /// 编译信息
        /// </summary>
        public string Message;
        /// <summary>
        /// 外部提交测试结果
        /// </summary>
        public struct item
        {
            /// <summary>
            /// 内存使用字节数
            /// </summary>
            public long MemoryUse;
            /// <summary>
            /// 用时毫秒数
            /// </summary>
            public int TimeUse;
            /// <summary>
            /// 测试数据标识
            /// </summary>
            public int TestId;
            /// <summary>
            /// 测试结果
            /// </summary>
            public enum result : byte
            {
                /// <summary>
                /// 运行中
                /// </summary>
                Running = 0,
                /// <summary>
                /// AC
                /// </summary>
                Accepted = 1,
                /// <summary>
                /// 输出错误
                /// </summary>
                WrongAnswer = 2,
                /// <summary>
                /// 超内存
                /// </summary>
                MemoryLimitExceed = 3,
                /// <summary>
                /// 超时
                /// </summary>
                TimeLimitExceed = 4,
                /// <summary>
                /// 运行时异常错误
                /// </summary>
                RunTimeError = 5,
                /// <summary>
                /// 判题服务没有找到测试数据
                /// </summary>
                DataFileLost = 6,
                /// <summary>
                /// 程序试图创建进程错误
                /// </summary>
                CreateProcess = 8,
                /// <summary>
                /// 程序阻塞超时
                /// </summary>
                Blocked = 9,

                /// <summary>
                /// 题目配置错误
                /// </summary>
                ProblemError = 21,
                /// <summary>
                /// 输出数据格式化验证错误
                /// </summary>
                FormatError = 22,
                /// <summary>
                /// 数据错误
                /// </summary>
                DataError = 23,
                /// <summary>
                /// 数据正常
                /// </summary>
                DataOk = 24,
                /// <summary>
                /// 挑战失败
                /// </summary>
                ChallangeFail = 25,
                /// <summary>
                /// 挑战成功
                /// </summary>
                Succeed = 26,

                /// <summary>
                /// 未知错误重试判题
                /// </summary>
                Retry = 100,
            }
            /// <summary>
            /// 测试结果
            /// </summary>
            public result Result;
        }
        /// <summary>
        /// 外部提交测试结果集合
        /// </summary>
        public item[] Items;
        /// <summary>
        /// 判题结果
        /// </summary>
        public enum judgeValue : byte
        {
            /// <summary>
            /// 默认未处理状态
            /// </summary>
            None = 0,
            /// <summary>
            /// 处理中
            /// </summary>
            Processing = 1,
            /// <summary>
            /// AC
            /// </summary>
            Accepted = 2,
            /// <summary>
            /// 输出错误
            /// </summary>
            WrongAnswer = 3,
            /// <summary>
            /// 超内存
            /// </summary>
            MemoryLimitExceed = 4,
            /// <summary>
            /// 超时
            /// </summary>
            TimeLimitExceed = 5,
            /// <summary>
            /// 运行时异常错误
            /// </summary>
            RunTimeError = 6,
            /// <summary>
            /// 判题服务错误
            /// </summary>
            JudgeError = 7,
            /// <summary>
            /// 编译错误
            /// </summary>
            CompileError = 8,

            /// <summary>
            /// 挑战成功
            /// </summary>
            ChallangeSucceed = 20,
            /// <summary>
            /// 挑战失败
            /// </summary>
            ChallangeFail = 21,
        }
        /// <summary>
        /// 判题结果
        /// </summary>
        public judgeValue JudgeValue;
    }
}
