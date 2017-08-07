using System;
using System.Collections.Specialized;
using System.Text;
using fastCSharp.net;
using System.Threading;

namespace fastCSharp.openApi
{
    /// <summary>
    /// web请求
    /// </summary>
    internal class request : IDisposable
    {
        /// <summary>
        /// web客户端
        /// </summary>
        private readonly webClient webClient = new webClient();
        /// <summary>
        /// web客户端 访问锁
        /// </summary>
        private readonly object webClientLock = new object();
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            webClient.Dispose();
            fastCSharp.domainUnload.Remove(Dispose, false);
        }
        /// <summary>
        /// 公用web请求
        /// </summary>
        public request()
        {
            webClient.KeepAlive = false;
            fastCSharp.domainUnload.Add(Dispose);
        }
        /// <summary>
        /// API请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="encoding">请求地址</param>
        /// <param name="form">POST表单内容</param>
        /// <param name="uploadData"></param>
        /// <returns>返回内容,失败为null</returns>
        public string Request(string url, Encoding encoding, NameValueCollection form = null, byte[] uploadData = null)
        {
            webClient.request request = new net.webClient.request
            {
                Uri = new Uri(url),
                Form = form,
                UploadData = uploadData,
                IsErrorOut = true,
                IsErrorOutUri = true
            };
            Monitor.Enter(webClientLock);
            try
            {
                return webClient.CrawlHtml(ref request, encoding);
            }
            finally { Monitor.Exit(webClientLock); }
        }
        /// <summary>
        /// 文件描述
        /// </summary>
        private static readonly byte[] contentDispositionData = @"
Content-Disposition: form-data; name=""".getBytes();
        /// <summary>
        /// 文件名称
        /// </summary>
        private static readonly byte[] filenameData = @"""; filename=""".getBytes();
        /// <summary>
        /// 文件类型
        /// </summary>
        private static readonly byte[] contentTypeData = @"""
Content-Type: ".getBytes();
        /// <summary>
        /// API请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="encoding">请求地址</param>
        /// <param name="data">文件数据</param>
        /// <param name="filename">文件名称</param>
        /// <param name="extensionName">扩展名称</param>
        /// <param name="form">表单数据</param>
        /// <returns>返回内容,失败为null</returns>
        public unsafe string Upload(string url, Encoding encoding, byte[] data, string filename, string extensionName, keyValue<byte[], byte[]>[] form)
        {
            string header = "multipart/form-data; boundary=----fastCSharpBoundary" + ((ulong)date.Now.Ticks).toHex16();
            byte[] contentType;
            int size = 40 + contentDispositionData.Length + filename.Length + filenameData.Length + filename.Length + data.Length + 46 + form.Length * (contentDispositionData.Length + 5 + 42);
            if (extensionName != null)
            {
                contentType = fastCSharp.web.contentTypeInfo.GetContentType(extensionName, null);
                size += extensionName.Length + 1;
            }
            else contentType = null;
            if (contentType == null) size += 5;
            else size += contentTypeData.Length + contentType.Length + 4;
            foreach (keyValue<byte[], byte[]> keyValue in form) size += keyValue.Key.Length + keyValue.Value.Length;
            byte[] body = new byte[size];
            fixed (byte* bodyFixed = body)
            {
                *(short*)(bodyFixed) = (short)('-' + ('-' << 8));
                byte* write = bodyFixed + sizeof(short);
                fixed (char* headerFixed = header)
                {
                    for (char* start = headerFixed + 30, end = start + 38; start != end; *write++ = (byte)*start++) ;
                }
                foreach (keyValue<byte[], byte[]> keyValue in form)
                {
                    fastCSharp.unsafer.memory.Copy(contentDispositionData, write, contentDispositionData.Length);
                    write += contentDispositionData.Length;
                    if (keyValue.Key.Length != 0)
                    {
                        fastCSharp.unsafer.memory.UnsafeSimpleCopy(keyValue.Key, write, keyValue.Key.Length);
                        write += keyValue.Key.Length;
                    }
                    *write++ = (byte)'"';
                    *(int*)write = 0x0a0d0a0d;
                    fastCSharp.unsafer.memory.Copy(keyValue.Value, write += sizeof(int), keyValue.Value.Length);
                    write += keyValue.Value.Length;
                    *(short*)write = 0x0a0d;
                    write += sizeof(short);
                    fastCSharp.unsafer.memory.UnsafeSimpleCopy(bodyFixed, write, 40);
                    write += 40;
                }
                fastCSharp.unsafer.memory.Copy(contentDispositionData, write, contentDispositionData.Length);
                write += contentDispositionData.Length;
                fixed (char* filenameFixed = filename)
                {
                    for (char* start = filenameFixed, end = filenameFixed + filename.Length; start != end; *write++ = (byte)*start++) ;
                    if (filenameData.Length != 0)
                    {
                        fastCSharp.unsafer.memory.UnsafeSimpleCopy(filenameData, write, filenameData.Length);
                        write += filenameData.Length;
                    }
                    for (char* start = filenameFixed, end = filenameFixed + filename.Length; start != end; *write++ = (byte)*start++) ;
                }
                if (extensionName != null)
                {
                    *write++ = (byte)'.';
                    fixed (char* extensionNameFixed = extensionName)
                    {
                        for (char* start = extensionNameFixed, end = extensionNameFixed + extensionName.Length; start != end; *write++ = (byte)*start++) ;
                    }
                }
                if (contentType == null) *write++ = (byte)'"';
                else
                {
                    if (contentTypeData.Length != 0)
                    {
                        fastCSharp.unsafer.memory.UnsafeSimpleCopy(contentTypeData, write, contentTypeData.Length);
                        write += contentTypeData.Length;
                    }
                    if (contentType.Length != 0)
                    {
                        fastCSharp.unsafer.memory.UnsafeSimpleCopy(contentType, write, contentType.Length);
                        write += contentType.Length;
                    }
                }
                *(int*)write = 0x0a0d0a0d;
                write += sizeof(int);
                fastCSharp.unsafer.memory.Copy(data, write, data.Length);
                write += data.Length;
                *(short*)write = 0x0a0d;
                write += sizeof(short);
                fastCSharp.unsafer.memory.UnsafeSimpleCopy(bodyFixed, write, 40);
                *(int*)(write + 40) = (int)('-' + ('-' << 8) + 0x0a0d0000);
            }
            webClient.request request = new net.webClient.request
            {
                Uri = new Uri(url),
                UploadData = body,
                IsErrorOut = true,
                IsErrorOutUri = true
            };
            Monitor.Enter(webClientLock);
            try
            {
                webClient.Headers.Add(fastCSharp.web.header.ContentType, header);
                return webClient.CrawlHtml(ref request, encoding);
            }
            finally { Monitor.Exit(webClientLock); }
        }
        /// <summary>
        /// API请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="uploadData"></param>
        /// <returns>返回内容,失败为null</returns>
        public byte[] Download(string url, byte[] uploadData)
        {
            webClient.request request = new webClient.request
            {
                Uri = new Uri(url),
                UploadData = uploadData,
                IsErrorOut = true,
                IsErrorOutUri = true
            };
            Monitor.Enter(webClientLock);
            try
            {
                return webClient.CrawlData(ref request);
            }
            finally { Monitor.Exit(webClientLock); }
        }
        /// <summary>
        /// 公用web请求
        /// </summary>
        public static readonly request Default = new request();
    }
}
