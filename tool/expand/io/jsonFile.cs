using System;
using fastCSharp.code.cSharp;
using System.IO;
using System.Text;
using fastCSharp.threading;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.io
{
    /// <summary>
    /// JSON对象缓存文件
    /// </summary>
    /// <typeparam name="valueType">对象类型</typeparam>
    public sealed class jsonFile<valueType> where valueType : class
    {
        /// <summary>
        /// 缓存文件名称
        /// </summary>
        private string fileName;
        /// <summary>
        /// 数据对象
        /// </summary>
        public valueType Value { get; private set; }
        /// <summary>
        /// 编码
        /// </summary>
        private Encoding encoding;
        /// <summary>
        /// 文件访问锁
        /// </summary>
        private readonly object fileLock;
        /// <summary>
        /// JSON对象缓存文件
        /// </summary>
        /// <param name="fileName">缓存文件名称</param>
        /// <param name="value">数据对象</param>
        /// <param name="encoding"></param>
        public jsonFile(string fileName, valueType value = null, Encoding encoding = null)
        {
            if (fileName == null) log.Error.Throw(log.exceptionType.Null);
            fileLock = new object();
            this.fileName = fileName;
            this.encoding = encoding ?? fastCSharp.config.appSetting.Encoding;
            bool isFile = false, isJson = false;
            try
            {
                if (File.Exists(fileName))
                {
                    isFile = true;
                    if (fastCSharp.emit.jsonParser.Parse(File.ReadAllText(fileName, this.encoding), ref value))
                    {
                        Value = value;
                        isJson = true;
                    }
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, fileName, false);
            }
            if (isFile && !isJson) file.MoveBak(fileName);
        }
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="value">新的对象值</param>
        /// <returns>是否修改成功</returns>
        public bool Rework(valueType value)
        {
            if (value == null) log.Error.Throw(log.exceptionType.Null);
            string json = fastCSharp.emit.jsonSerializer.ToJson(value);
            Monitor.Enter(fileLock);
            try
            {
                if (write(json))
                {
                    Value = value;
                    return true;
                }
            }
            finally { Monitor.Exit(fileLock); }
            return false;
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <returns>是否成功</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public bool Write()
        {
            return Rework(Value);
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>是否成功</returns>
        private bool write(string json)
        {
            try
            {
                if (File.Exists(fileName)) file.MoveBak(fileName);
                File.WriteAllText(fileName, json, this.encoding);
                return true;
            }
            catch (Exception error)
            {
                log.Error.Add(error, fileName, false);
            }
            return false;
        }
    }
}
