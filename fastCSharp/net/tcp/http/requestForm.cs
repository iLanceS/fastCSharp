using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using fastCSharp.code.cSharp;
using System.Runtime.CompilerServices;
#if XAMARIN
#else
using System.Drawing;
using System.Drawing.Imaging;
#endif

namespace fastCSharp.net.tcp.http
{
    /// <summary>
    /// HTTP请求表单
    /// </summary>
    public sealed partial class requestForm
    {
        /// <summary>
        /// HTTP请求表单加载接口
        /// </summary>
        public interface ILoadForm
        {
            /// <summary>
            /// 表单回调处理
            /// </summary>
            /// <param name="form">HTTP请求表单</param>
            void OnGetForm(requestForm form);
            /// <summary>
            /// 根据HTTP请求表单值获取内存流最大字节数
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>内存流最大字节数</returns>
            int MaxMemoryStreamSize(ref fastCSharp.net.tcp.http.requestForm.value value);
            /// <summary>
            /// 根据HTTP请求表单值获取保存文件全称
            /// </summary>
            /// <param name="value">HTTP请求表单值</param>
            /// <returns>文件全称</returns>
            string GetSaveFileName(ref fastCSharp.net.tcp.http.requestForm.value value);
        }
        /// <summary>
        /// HTTP请求表单值
        /// </summary>
        public struct value
        {
            /// <summary>
            /// 名称
            /// </summary>
            public subArray<byte> Name;
            /// <summary>
            /// 表单值
            /// </summary>
            public subArray<byte> Value;
            /// <summary>
            /// 客户端文件名称
            /// </summary>
            public subArray<byte> FileName;
            /// <summary>
            /// 文件扩展名
            /// </summary>
            public unsafe subArray<byte> ExtensionName
            {
                get
                {
                    if(FileName.length != 0)
                    {
                        fixed (byte* nameFixed = FileName.array)
                        {
                            byte* start = nameFixed + FileName.startIndex, end = fastCSharp.unsafer.memory.FindLast(start, start + FileName.length, (byte)'.');
                            if (end != null)
                            {
                                ++end;
                                return subArray<byte>.Unsafe(FileName.array, (int)(end - nameFixed), (int)(start + FileName.length - end));
                            }
                        }
                    }
                    return default(subArray<byte>);
                }
            }
            /// <summary>
            /// 服务器端文件名称
            /// </summary>
            public string SaveFileName;
            /// <summary>
            /// 设置文件表单数据
            /// </summary>
            internal void SetFileValue()
            {
                if (SaveFileName != null)
                {
                    try
                    {
                        if (File.Exists(SaveFileName))
                        {
                            byte[] data = File.ReadAllBytes(SaveFileName);
                            Value.UnsafeSet(data, 0, data.Length);
                            File.Delete(SaveFileName);
                        }
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                SaveFileName = null;
            }
            /// <summary>
            /// 保存到目标文件
            /// </summary>
            /// <param name="fileName">目标文件名称</param>
            /// <param name="isOver">是否覆盖原文件</param>
            public void SaveFile(string fileName, bool isOver = false)
            {
                if (SaveFileName == null)
                {
                    using (FileStream fileStream = new FileStream(fileName, isOver ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read, 1, FileOptions.WriteThrough))
                    {
                        fileStream.Write(Value.array, Value.startIndex, Value.length);
                    }
                }
                else
                {
                    fastCSharp.io.file.MoveBak(fileName);
                    File.Move(SaveFileName, fileName);
                    SaveFileName = null;
                }
            }
            /// <summary>
            /// 清表单数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Clear()
            {
                Name.Null();
                Value.Null();
                FileName.Null();
                if (SaveFileName != null)
                {
                    try
                    {
                        if (File.Exists(SaveFileName)) File.Delete(SaveFileName);
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    SaveFileName = null;
                }
            }
            /// <summary>
            /// 清除数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Null()
            {
                Name.Null();
                Value.Null();
                FileName.Null();
                SaveFileName = null;
            }

            /// <summary>
            /// 默认允许上传的图片扩展名集合
            /// </summary>
            private static readonly string[] defaultImageExtensions = new string[] { "jpeg", "gif", "bmp", "png" };
            /// <summary>
            /// 默认允许上传的图片扩展名集合
            /// </summary>
            public static readonly fastCSharp.stateSearcher.ascii<string> DefaultImageExtensions = new fastCSharp.stateSearcher.ascii<string>(defaultImageExtensions, defaultImageExtensions, true);
#if XAMARIN
#else
            /// <summary>
            /// 保存图片
            /// </summary>
            /// <param name="fileName">不包含扩展名的图片文件名称</param>
            /// <param name="imageTypes">默认允许上传的图片类型集合</param>
            /// <returns>包含扩展名的图片文件名称,失败返回null</returns>
            public string SaveImage(string fileName, Dictionary<ImageFormat, string> imageTypes = null)
            {
                try
                {
                    string type = null;
                    if (SaveFileName == null)
                    {
                        if (Value.length != 0)
                        {
                            using (MemoryStream stream = new MemoryStream(Value.array, Value.startIndex, Value.length))
                            using (Image image = Image.FromStream(stream))
                            {
                                (imageTypes ?? defaultImageTypes).TryGetValue(image.RawFormat, out type);
                            }
                        }
                    }
                    else
                    {
                        using (Image image = Image.FromFile(SaveFileName)) (imageTypes ?? defaultImageTypes).TryGetValue(image.RawFormat, out type);
                    }
                    if (type != null)
                    {
                        fileName += "." + type;
                        if (Value.length == 0)
                        {
                            File.Move(SaveFileName, fileName);
                            SaveFileName = null;
                        }
                        else
                        {
                            using (FileStream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                            {
                                fileStream.Write(Value.array, Value.startIndex, Value.length);
                            }
                        }
                        return fileName;
                    }
                }
                catch (Exception error)
                {
                    log.Default.Add(error, SaveFileName + "[" + fileName + "]", false);
                }
                return null;
            }
            /// <summary>
            /// 默认允许上传的图片类型集合
            /// </summary>
            private static readonly Dictionary<ImageFormat, string> defaultImageTypes;
            static value()
            {
                defaultImageTypes = dictionary.CreateOnly<ImageFormat, string>();
                defaultImageTypes.Add(ImageFormat.Jpeg, ImageFormat.Jpeg.ToString().toLower());
                defaultImageTypes.Add(ImageFormat.Gif, ImageFormat.Gif.ToString().toLower());
                defaultImageTypes.Add(ImageFormat.Bmp, ImageFormat.Bmp.ToString().toLower());
                defaultImageTypes.Add(ImageFormat.MemoryBmp, ImageFormat.Bmp.ToString().toLower());
                defaultImageTypes.Add(ImageFormat.Png, ImageFormat.Png.ToString().toLower());
            }
#endif
        }
        /// <summary>
        /// HTTP操作标识
        /// </summary>
        internal long Identity;
        /// <summary>
        /// 表单数据缓冲区
        /// </summary>
        internal byte[] Buffer;
        /// <summary>
        /// 表单数据缓冲区
        /// </summary>
        public byte[] UnsafeBuffer
        {
            get { return Buffer; }
        }
        /// <summary>
        /// 表单数据缓冲区数据长度
        /// </summary>
        public int BufferSize { get; internal set; }
        /// <summary>
        /// 表单数据集合
        /// </summary>
        internal list<value> FormValues = new list<value>(sizeof(int));
        /// <summary>
        /// 文件集合
        /// </summary>
        public readonly list<value> Files = new list<value>(sizeof(int));
        /// <summary>
        /// 字符串
        /// </summary>
        public string Text { get; internal set; }
        /// <summary>
        /// 查询字符
        /// </summary>
        internal char TextQueryChar;
        /// <summary>
        /// 清除表单数据
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal void Clear()
        {
            if (FormValues.length != 0) clear(FormValues);
            if (Files.length != 0) clear(Files);
            Text = null;
            TextQueryChar = (char)0;
        }
        /// <summary>
        /// 解析表单数据
        /// </summary>
        /// <param name="buffer">表单数据缓冲区</param>
        /// <param name="length">表单数据长度</param>
        /// <returns>是否成功</returns>
        internal unsafe bool Parse(byte[] buffer, int length)
        {
            fixed (byte* bufferFixed = Buffer = buffer)
            {
                byte* current = bufferFixed - 1, end = bufferFixed + (BufferSize = length);
                *end = (byte)'&';
                try
                {
                    do
                    {
                        int nameIndex = (int)(++current - bufferFixed);
                        while (*current != '&' && *current != '=') ++current;
                        int nameLength = (int)(current - bufferFixed) - nameIndex;
                        if (*current == '=')
                        {
                            int valueIndex = (int)(++current - bufferFixed);
                            while (*current != '&') ++current;
                            if (nameLength == 1)
                            {
                                switch (buffer[nameIndex])
                                {
                                    case (byte)fastCSharp.config.web.QueryJsonName:
                                    case (byte)fastCSharp.config.web.QueryXmlName:
                                        TextQueryChar = (char)buffer[nameIndex];
                                        if ((length = (int)(current - bufferFixed) - valueIndex) == 0) Text = string.Empty;
                                        else
                                        {
                                            subArray<byte> json = subArray<byte>.Unsafe(buffer, valueIndex, length);
                                            Text = fastCSharp.web.formQuery.JavascriptUnescapeUtf8(ref json);
                                        }
                                        break;
                                    default:
                                        FormValues.Add(new requestForm.value { Name = subArray<byte>.Unsafe(buffer, nameIndex, nameLength), Value = subArray<byte>.Unsafe(buffer, valueIndex, (int)(current - bufferFixed) - valueIndex) });
                                        break;
                                }
                            }
                            else FormValues.Add(new requestForm.value { Name = subArray<byte>.Unsafe(buffer, nameIndex, nameLength), Value = subArray<byte>.Unsafe(buffer, valueIndex, (int)(current - bufferFixed) - valueIndex) });
                        }
                        else if (nameLength != 0)
                        {
                            FormValues.Add(new requestForm.value { Name = subArray<byte>.Unsafe(buffer, nameIndex, nameLength), Value = subArray<byte>.Unsafe(buffer, 0, 0) });
                        }
                    }
                    while (current != end);
                    this.Buffer = buffer;
                    return true;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
            return false;
        }
        /// <summary>
        /// JSON数据转换成JSON字符串
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="length">JSON数据长度</param>
        /// <param name="encoding">编码</param>
        /// <param name="textQueryChar">查询字符</param>
        /// <returns>是否成功</returns>
        internal unsafe bool Parse(byte[] buffer, int length, Encoding encoding, char textQueryChar)
        {
            return Parse(Buffer = buffer, 0, BufferSize = length, encoding, textQueryChar);
        }
        /// <summary>
        /// JSON数据转换成JSON字符串
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="startIndex">数据其实位置</param>
        /// <param name="length">JSON数据长度</param>
        /// <param name="encoding">编码</param>
        /// <param name="textQueryChar">查询字符</param>
        /// <returns>是否成功</returns>
        internal unsafe bool Parse(byte[] buffer, int startIndex, int length, Encoding encoding, char textQueryChar)
        {
            this.TextQueryChar = textQueryChar;
            if (length == 0)
            {
                Text = string.Empty;
                return true;
            }
            try
            {
                if (encoding == Encoding.Unicode)
                {
                    Text = fastCSharp.String.FastAllocateString(length >> 1);
                    fixed (char* jsonFixed = Text)
                    fixed (byte* bufferFixed = buffer)
                    {
                        fastCSharp.unsafer.memory.Copy(bufferFixed + startIndex, jsonFixed, length);
                    }
                }
                else if (encoding == Encoding.ASCII)
                {
                    fixed (byte* bufferFixed = buffer) Text = fastCSharp.String.UnsafeDeSerialize(bufferFixed + startIndex, -length);
                }
                else Text = encoding.GetString(buffer, startIndex, length);
                this.Buffer = buffer;
                return true;
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            return false;
        }

        /// <summary>
        /// 设置文件表单数据
        /// </summary>
        internal unsafe void SetFileValue()
        {
            value[] formArray = FormValues.array;
            for (int index = 0, count = FormValues.length; index != count; ++index) formArray[index].SetFileValue();
        }
        /// <summary>
        /// 清除表单数据
        /// </summary>
        /// <param name="values">表单数据集合</param>
        private static void clear(list<value> values)
        {
            value[] formArray = values.array;
            for (int index = values.length; index != 0; formArray[--index].Clear()) ;
            values.Empty();
        }
    }
}
