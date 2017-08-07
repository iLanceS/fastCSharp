using System;
using System.IO;
using fastCSharp.io;

namespace fastCSharp.testCase
{
    /// <summary>
    /// TCP服务Stream支持测试(因为安全问题仅支持客户端Stream)
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    internal partial class tcpStream
    {
        /// <summary>
        /// 数据缓存区
        /// </summary>
        private static byte[] buffer;
        /// <summary>
        /// 服务器端读取客户端Stream测试
        /// </summary>
        /// <param name="stream">客户端流</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private bool send(Stream stream)
        {
            byte[] data = new byte[buffer.Length];
            using (stream)
            {
                for (int length, index = 0; index != data.Length; index += length)
                {
                    if ((length = stream.Read(data, index, Math.Min(data.Length - index, data.Length >> 1))) <= 0) return false;
                }
            }
            return fastCSharp.unsafer.memory.Equal(data, buffer);
        }
        /// <summary>
        /// 服务器端读取客户端Stream测试(混合参数模式)
        /// </summary>
        /// <param name="stream">客户端流</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private bool send(Stream stream, int value)
        {
            return value == 1 && send(stream);
        }
        /// <summary>
        /// 服务器端读取客户端FileStream测试
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="stream">客户端文件流</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private unsafe bool readFile(string fileName, Stream stream)
        {
            byte[] data = File.ReadAllBytes(fileName), buffer = new byte[1 << 10];
            fixed (byte* dataFixed = data)
            {

                for (int index = 0, read; index != data.Length; index += read)
                {
                    read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0 || !fastCSharp.unsafer.memory.Equal(buffer, dataFixed + index, read)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 服务器端写客户端FileStream测试
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="stream">客户端文件流</param>
        [fastCSharp.code.cSharp.tcpMethod]
        private unsafe void writeFile(string fileName, Stream stream)
        {
            byte[] data = File.ReadAllBytes(fileName);
            for (int index = 0, count; index != data.Length; index += count)
            {
                stream.Write(data, index, count = Math.Min(data.Length - index, 1 << 10));
            }
        }
        /// <summary>
        /// 断点续传测试
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="stream">客户端文件流</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private unsafe bool partFile(string fileName, Stream stream)
        {
            try
            {
                using (FileStream writeStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    long length = stream.Length - writeStream.Length;
                    if (length <= 0) return true;
                    if (writeStream.Length != 0)
                    {
                        stream.Seek(writeStream.Length, SeekOrigin.Begin);
                        writeStream.Seek(writeStream.Length, SeekOrigin.Begin);
                    }
                    byte[] buffer = new byte[1 << 10];
                    bool isException = false;
                    do
                    {
                        int read = stream.Read(buffer, 0, buffer.Length);
                        if (read <= 0) return false;
                        writeStream.Write(buffer, 0, read);
                        length -= read;
                        if (isException) throw new Exception();//抛出异常，模拟客户端断开
                        isException = true;
                    }
                    while (length != 0);
                }
            }
            catch { }
            return false;
        }
#if NotFastCSharpCode
#else
        /// <summary>
        /// TCP服务Stream支持测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            using (tcpStream.tcpServer server = new tcpStream.tcpServer())
            {
                if (server.Start())
                {
                    using (tcpStream.tcpClient client = new tcpStream.tcpClient())
                    {
                        buffer = new byte[256];
                        for (int index = buffer.Length; index != 0; buffer[--index] = (byte)index) ;

                        using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length))
                        {
                            if (!client.send(memoryStream).Value) return false;
                        }

                        using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length))
                        {
                            if (!client.send(memoryStream, 1).Value) return false;
                        }
#if APP
#else
#if MONO
                        string fileName = (@"..\..\Program.cs").pathSeparator();
#else
                        string fileName = (@"..\..\tcpStream.cs").pathSeparator();
#endif
                        using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            if (!client.readFile(fileName, fileStream).Value) return false;
                        }

                        string bakFileName = fileName + ".bak";
                        if (File.Exists(bakFileName)) File.Delete(bakFileName);
                        try
                        {
                            using (FileStream fileStream = new FileStream(bakFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                            {
                                client.writeFile(fileName, fileStream);
                            }
                            if (!fastCSharp.unsafer.memory.Equal(File.ReadAllBytes(fileName), File.ReadAllBytes(bakFileName))) return false;
                        }
                        finally { File.Delete(bakFileName); }

                        try
                        {
                            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                while (!client.partFile(bakFileName, fileStream).Value) ;
                            }
                        }
                        finally
                        {
                            if (File.Exists(bakFileName)) File.Delete(bakFileName);
                        }
#endif
                        return true;
                    }
                }
            }
            return false;
        }
#endif
                    }
}
