using System;
using fastCSharp.threading;
using System.Threading;

namespace fastCSharp.openApi._51nod
{
    /// <summary>
    /// API调用
    /// </summary>
    public sealed class api
    {
        /// <summary>
        /// 当前令牌超时
        /// </summary>
        private DateTime timeout;
        /// <summary>
        /// 当前令牌
        /// </summary>
        private string token;
        /// <summary>
        /// 应用配置
        /// </summary>
        private readonly config config;
        /// <summary>
        /// 令牌访问锁
        /// </summary>
        private readonly object tokenLock = new object();
        /// <summary>
        /// API调用
        /// </summary>
        /// <param name="config">应用配置</param>
        public api(config config = null)
        {
            this.config = config ?? config.Default;
        }
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <returns></returns>
        private string getToken()
        {
            DateTime now = date.UtcNowSecond;
            string value;
            Monitor.Enter(tokenLock);
            if (timeout > now)
            {
                value = token;
                Monitor.Exit(tokenLock);
                return value;
            }
            long timeoutTicks;
            try
            {
                if ((value = config.GetToken(out timeoutTicks)) != null)
                {
                    token = value;
                    timeout = new DateTime(timeoutTicks, DateTimeKind.Utc);
                }
            }
            finally { Monitor.Exit(tokenLock); }
            return value;
        }
        /// <summary>
        /// 题目参数
        /// </summary>
        private struct problemQuery
        {
            /// <summary>
            /// 令牌
            /// </summary>
            public string token;
            /// <summary>
            /// 题目
            /// </summary>
            public problem problem;
        }
        /// <summary>
        /// 添加题目
        /// </summary>
        /// <param name="problem">题目</param>
        /// <returns>题目ID</returns>
        public int AppendProblem(problem problem)
        {
            if (problem != null)
            {
                string token = getToken();
                if (token != null)
                {
                    returnValue<int> value = config.Request.RequestJson<returnValue<int>, problemQuery>(config.Domain + "ajax?n=api.problem.Append", new problemQuery { token = token, problem = problem });
                    if (value != null) return value.Value;
                }
            }
            return 0;
        }
        /// <summary>
        /// 修改题目
        /// </summary>
        /// <param name="problem">题目</param>
        /// <returns>是否成功</returns>
        public bool ReworkProblem(problem problem)
        {
            if (problem != null && problem.Id != 0)
            {
                string token = getToken();
                if (token != null)
                {
                    returnValue value = config.Request.RequestJson<returnValue, problemQuery>(config.Domain + "ajax?n=api.problem.Rework", new problemQuery { token = token, problem = problem });
                    return value != null && value.IsValue;
                }
            }
            return false;
        }
        /// <summary>
        /// Zip文件模式修改或者添加测试数据 参数名称
        /// </summary>
        private static readonly byte[] uploadTestDataParameterName = new byte[] { (byte)'j' };
        /// <summary>
        /// Zip文件模式修改或者添加测试数据
        /// </summary>
        private struct uploadTestDataQuery
        {
            /// <summary>
            /// 令牌
            /// </summary>
            public string token;
            /// <summary>
            /// 题目ID
            /// </summary>
            public int problemId;
        }
        /// <summary>
        /// Zip文件模式修改或者添加测试数据
        /// </summary>
        /// <param name="problemId">题目ID</param>
        /// <param name="zipFileData">zip文件内容</param>
        /// <returns>上传后的测试数据数量</returns>
        public int UploadTestData(int problemId, byte[] zipFileData)
        {
            if (problemId != 0 && zipFileData.length() != 0)
            {
                string token = getToken();
                if (token != null)
                {
                    returnValue<int> value = config.Request.RequestJson<returnValue<int>>(config.Domain + "upload/ReworkTestData", zipFileData, "file", "zip", new keyValue<byte[], byte[]>[] { new keyValue<byte[], byte[]>(uploadTestDataParameterName, new uploadTestDataQuery { token = token, problemId = problemId }.ToJson().getBytes()) });
                    if (value != null) return value.Value;
                }
            }
            return 0;
        }
        /// <summary>
        /// 删除测试数据
        /// </summary>
        private struct deleteTestDataQuery
        {
            /// <summary>
            /// 令牌
            /// </summary>
            public string token;
            /// <summary>
            /// 题目ID
            /// </summary>
            public int problemId;
            /// <summary>
            /// 测试数据ID
            /// </summary>
            public byte testId;
        }
        /// <summary>
        /// 删除测试数据
        /// </summary>
        /// <param name="problemId">题目ID</param>
        /// <param name="testId">测试数据ID</param>
        /// <returns></returns>
        public bool DeleteTestData(int problemId, byte testId)
        {
            if (problemId != 0 && testId != 0)
            {
                string token = getToken();
                if (token != null)
                {
                    returnValue value = config.Request.RequestJson<returnValue, deleteTestDataQuery>(config.Domain + "ajax?n=api.problem.DeleteTestData", new deleteTestDataQuery { token = token, problemId = problemId, testId = testId });
                    return value != null && value.IsValue;
                }
            }
            return false;
        }
        /// <summary>
        /// 提交测试
        /// </summary>
        private struct judgeQuery
        {
            /// <summary>
            /// 令牌
            /// </summary>
            public string token;
            /// <summary>
            /// 提交测试
            /// </summary>
            public judge judge;
        }
        /// <summary>
        /// 提交测试
        /// </summary>
        /// <param name="judge">提交测试</param>
        /// <returns></returns>
        public bool Judge(judge judge)
        {
            if (judge != null)
            {
                string token = getToken();
                if (token != null)
                {
                    returnValue value = config.Request.RequestJson<returnValue, judgeQuery>(config.Domain + "ajax?n=api.judge.Open", new judgeQuery { token = token, judge = judge });
                    return value != null && value.IsValue;
                }
            }
            return false;
        }
        /// <summary>
        /// 批量提交测试
        /// </summary>
        private struct batchJudgeQuery
        {
            /// <summary>
            /// 令牌
            /// </summary>
            public string token;
            /// <summary>
            /// 提交测试
            /// </summary>
            public judge[] judges;
        }
        /// <summary>
        /// 批量提交测试
        /// </summary>
        /// <param name="judges">提交测试</param>
        /// <returns>成功提交的测试ID</returns>
        public int[] BatchJudge(judge[] judges)
        {
            if (judges.length() != 0)
            {
                string token = getToken();
                if (token != null)
                {
                    returnValue<int[]> value = config.Request.RequestJson<returnValue<int[]>, batchJudgeQuery>(config.Domain + "/ajax?n=api.judge.Batch", new batchJudgeQuery { token = token, judges = judges });
                    if (value != null) return value.Value;
                }
            }
            return null;
        }
        /// <summary>
        /// Judge判题回调解析
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public judgeResult[] JudgeCallback(byte[] data)
        {
            return data.length() != 0 ? JudgeCallback(System.Text.Encoding.UTF8.GetString(data)) : null;
        }
        /// <summary>
        /// Judge判题回调解析
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public judgeResult[] JudgeCallback(string json)
        {
            callback<judgeResult[]> value = fastCSharp.emit.jsonParser.Parse<callback<judgeResult[]>>(json);
            return value.Type == callbackType.OpenJudge ? value.Value : null;
        }
    }
}
