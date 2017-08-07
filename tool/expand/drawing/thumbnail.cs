using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.CompilerServices;

namespace fastCSharp.drawing
{
    /// <summary>
    /// 缩略图
    /// </summary>
    public static class thumbnail
    {
        /// <summary>
        /// 高质量图像编码参数
        /// </summary>
        private static readonly EncoderParameters qualityEncoder;
        /// <summary>
        /// 图像编码解码器集合
        /// </summary>
        private static readonly fastCSharp.stateSearcher.byteArray<ImageCodecInfo> imageCodecs;
        /// <summary>
        /// JPEG图像编码解码器
        /// </summary>
        private static readonly ImageCodecInfo jpegImageCodecInfo;
        /// <summary>
        /// 获取图像编码解码器
        /// </summary>
        /// <param name="format">图像文件格式</param>
        /// <returns>图像编码解码器</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private unsafe static ImageCodecInfo getImageCodec(ImageFormat format)
        {
            if (format != null)
            {
                guid guid = new guid { Value = format.Guid };
                return imageCodecs.Get(&guid, 16);
            }
            return null;
        }
        /// <summary>
        /// 图像缩略切剪
        /// </summary>
        /// <param name="data">图像文件数据</param>
        /// <param name="width">缩略宽度,0表示与高度同比例</param>
        /// <param name="height">缩略高度,0表示与宽度同比例</param>
        /// <param name="type">目标图像文件格式</param>
        /// <param name="memoryPool">输出数据缓冲区内存池</param>
        /// <param name="seek">输出数据起始位置</param>
        /// <returns>图像缩略文件数据</returns>
        public static subArray<byte> Cut(byte[] data, int width, int height, ImageFormat type, memoryPool memoryPool = null, int seek = 0)
        {
            if (data == null) return default(subArray<byte>);
            subArray<byte> dataArray = subArray<byte>.Unsafe(data, 0, data.Length);
            Cut(ref dataArray, width, height, type, memoryPool, seek);
            return dataArray;
        }
        /// <summary>
        /// 图像缩略切剪
        /// </summary>
        /// <param name="data">图像文件数据</param>
        /// <param name="width">缩略宽度,0表示与高度同比例</param>
        /// <param name="height">缩略高度,0表示与宽度同比例</param>
        /// <param name="type">目标图像文件格式</param>
        /// <param name="memoryPool">输出数据缓冲区内存池</param>
        /// <param name="seek">输出数据起始位置</param>
        /// <returns>图像缩略文件数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<byte> Cut(subArray<byte> data, int width, int height, ImageFormat type, memoryPool memoryPool = null, int seek = 0)
        {
            Cut(ref data, width, height, type, memoryPool, seek);
            return data;
        }
        /// <summary>
        /// 图像缩略切剪
        /// </summary>
        /// <param name="data">图像文件数据</param>
        /// <param name="width">缩略宽度,0表示与高度同比例</param>
        /// <param name="height">缩略高度,0表示与宽度同比例</param>
        /// <param name="type">目标图像文件格式</param>
        /// <param name="memoryPool">输出数据缓冲区内存池</param>
        /// <param name="seek">输出数据起始位置</param>
        /// <returns>图像缩略文件数据</returns>
        public static void Cut(ref subArray<byte> data, int width, int height, ImageFormat type, memoryPool memoryPool = null, int seek = 0)
        {
            if (data.Count != 0 && width > 0 && height > 0 && (width | height) != 0 && seek >= 0)
            {
                try
                {
                    using (MemoryStream memory = new MemoryStream(data.UnsafeArray, data.StartIndex, data.Count))
                    {
                        builder builder = new builder();
                        using (Image image = builder.CreateImage(memory))
                        {
                            builder.Cut(ref data, ref width, ref height, type, memoryPool, seek);
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
            data.Null();
        }
        /// <summary>
        /// 图像缩略补白
        /// </summary>
        /// <param name="data">图像文件数据</param>
        /// <param name="width">缩略宽度,0表示与高度同比例</param>
        /// <param name="height">缩略高度,0表示与宽度同比例</param>
        /// <param name="type">目标图像文件格式</param>
        /// <param name="backColor">背景色</param>
        /// <param name="memoryPool">输出数据缓冲区内存池</param>
        /// <param name="seek">输出数据起始位置</param>
        /// <returns>图像缩略文件数据</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static subArray<byte> Pad(subArray<byte> data, int width, int height, ImageFormat type, Color backColor, memoryPool memoryPool = null, int seek = 0)
        {
            Pad(ref data, width, height, type, backColor, memoryPool, seek);
            return data;
        }
        /// <summary>
        /// 图像缩略补白
        /// </summary>
        /// <param name="data">图像文件数据</param>
        /// <param name="width">缩略宽度,0表示与高度同比例</param>
        /// <param name="height">缩略高度,0表示与宽度同比例</param>
        /// <param name="type">目标图像文件格式</param>
        /// <param name="backColor">背景色</param>
        /// <param name="memoryPool">输出数据缓冲区内存池</param>
        /// <param name="seek">输出数据起始位置</param>
        /// <returns>图像缩略文件数据</returns>
        public static void Pad(ref subArray<byte> data, int width, int height, ImageFormat type, Color backColor, memoryPool memoryPool = null, int seek = 0)
        {
            if (data.Count != 0 && width > 0 && height > 0 && (width | height) != 0 && seek >= 0)
            {
                try
                {
                    using (MemoryStream memory = new MemoryStream(data.UnsafeArray, data.StartIndex, data.Count))
                    {
                        builder builder = new builder();
                        using (Image image = builder.CreateImage(memory))
                        {
                            builder.Pad(ref data, ref width, ref height, type, backColor, memoryPool, seek);
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
            }
            data.Null();
        }
        /// <summary>
        /// 缩略图创建器
        /// </summary>
        private struct builder
        {
            /// <summary>
            /// 原始图片
            /// </summary>
            private Image image;
            /// <summary>
            /// 原始图片宽度
            /// </summary>
            private int width;
            /// <summary>
            /// 原始图片高度
            /// </summary>
            private int height;
            /// <summary>
            /// 原始图片裁剪横坐标起始位置
            /// </summary>
            private int left;
            /// <summary>
            /// 原始图片裁剪纵坐标起始位置
            /// </summary>
            private int top;
            /// <summary>
            /// 原始图片裁剪横坐标结束位置
            /// </summary>
            private int right;
            /// <summary>
            /// 原始图片裁剪纵坐标结束位置
            /// </summary>
            private int bottom;
            /// <summary>
            /// 根据数据流创建原始图片
            /// </summary>
            /// <param name="stream">数据流</param>
            /// <returns>原始图片</returns>
            public Image CreateImage(Stream stream)
            {
                image = Image.FromStream(stream);
                width = image.Width;
                height = image.Height;
                return image;
            }
            /// <summary>
            /// 计算缩略图尺寸位置
            /// </summary>
            /// <param name="width">缩略宽度,0表示与高度同比例</param>
            /// <param name="height">缩略高度,0表示与宽度同比例</param>
            /// <returns>是否需要生成缩略图</returns>
            private bool checkCut(ref int width, ref int height)
            {
                if (width > 0)
                {
                    if (height > 0)
                    {
                        if ((long)width * this.height >= (long)height * this.width)
                        {
                            int value = (int)((long)height * this.width / width);
                            if (width > this.width)
                            {
                                if (value == 0) value = 1;
                                width = this.width;
                            }
                            left = 0;
                            top = (this.height - value) >> 1;
                            right = this.width;
                            bottom = top + value;
                        }
                        else
                        {
                            int value = (int)((long)width * this.height / height);
                            if (height > this.height)
                            {
                                if (value == 0) value = 1;
                                height = this.height;
                            }
                            left = (this.width - value) >> 1;
                            top = 0;
                            right = left + value;
                            bottom = this.height;
                        }
                        return true;
                    }
                    if (width < this.width)
                    {
                        left = top = 0;
                        right = this.width;
                        bottom = this.height;
                        if ((height = (int)((long)this.height * width / this.width)) == 0) height = 1;
                        return true;
                    }
                }
                else if (height < this.height)
                {
                    left = top = 0;
                    right = this.width;
                    bottom = this.height;
                    if ((width = (int)((long)this.width * height / this.height)) == 0) width = 1;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 获取缩略图
            /// </summary>
            /// <param name="data">输出数据</param>
            /// <param name="width">缩略宽度</param>
            /// <param name="height">缩略高度</param>
            /// <param name="type">目标图像文件格式</param>
            /// <param name="memoryPool">输出数据缓冲区内存池</param>
            /// <param name="seek">输出数据起始位置</param>
            public void Cut(ref subArray<byte> data, ref int width, ref int height, ImageFormat type, memoryPool memoryPool, int seek)
            {
                if (checkCut(ref width, ref height))
                {
                    if (memoryPool == null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            if (seek != 0) stream.Seek(seek, SeekOrigin.Begin);
                            cut(stream, width, height, type);
                            data.UnsafeSet(stream.GetBuffer(), seek, (int)stream.Position - seek);
                            return;
                        }
                    }
                    byte[] buffer = memoryPool.Get();
                    try
                    {
                        using (MemoryStream stream = memoryStream.UnsafeNew(buffer))
                        {
                            if (seek != 0) stream.Seek(seek, SeekOrigin.Begin);
                            cut(stream, width, height, type);
                            byte[] bufferData = stream.GetBuffer();
                            if (buffer == bufferData)
                            {
                                buffer = null;
                                //showjim
                                if ((int)stream.Position > bufferData.Length)
                                {
                                    log.Error.Add("Position " + ((int)stream.Position).toString() + " > " + bufferData.Length.toString(), null, false);
                                }
                            }
                            data.UnsafeSet(bufferData, seek, (int)stream.Position - seek);
                            return;
                        }
                    }
                    finally { memoryPool.PushOnly(buffer); }
                }
                data.Null();
            }
            /// <summary>
            /// 获取缩略图
            /// </summary>
            /// <param name="stream">输出数据流</param>
            /// <param name="width">缩略宽度</param>
            /// <param name="height">缩略高度</param>
            /// <param name="type">目标图像文件格式</param>
            private void cut(Stream stream, int width, int height, ImageFormat type)
            {
                using (Bitmap bitmap = new Bitmap(width, height))
                using (Graphics graphic = Graphics.FromImage(bitmap))
                {
                    if (type != ImageFormat.Png) graphic.Clear(Color.White);
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;
                    graphic.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(left, top, right - left, bottom - top), GraphicsUnit.Pixel);
                    bitmap.Save(stream, getImageCodec(type ?? ImageFormat.Jpeg) ?? jpegImageCodecInfo, qualityEncoder);
                }
            }
            /// <summary>
            /// 计算缩略图尺寸位置
            /// </summary>
            /// <param name="width">缩略宽度,0表示与高度同比例</param>
            /// <param name="height">缩略高度,0表示与宽度同比例</param>
            /// <returns>是否需要生成缩略图</returns>
            private bool checkPad(ref int width, ref int height)
            {
                if (width > 0)
                {
                    if (height > 0)
                    {
                        if ((long)width * this.height >= (long)height * this.width)
                        {
                            int value = (int)((long)this.width * height / this.height);
                            if (this.height > height)
                            {
                                if (value == 0) value = 1;
                                //height = this.height;
                            }
                            left = (width - value) >> 1;
                            top = 0;
                            right = left + value;
                            bottom = height;
                        }
                        else
                        {
                            int value = (int)((long)this.height * width / this.width);
                            if (this.width > width)
                            {
                                if (value == 0) value = 1;
                                //width = this.width;
                            }
                            left = 0;
                            top = (height - value) >> 1;
                            right = width;
                            bottom = top + value;
                        }
                        return true;
                    }
                    if (width < this.width)
                    {
                        if ((height = (int)((long)this.height * width / this.width)) == 0) height = 1;
                        left = top = 0;
                        right = width;
                        bottom = height;
                        return true;
                    }
                }
                else if (height < this.height)
                {
                    if ((width = (int)((long)this.width * height / this.height)) == 0) width = 1;
                    left = top = 0;
                    right = width;
                    bottom = height;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 获取缩略图
            /// </summary>
            /// <param name="data">输出数据</param>
            /// <param name="width">缩略宽度</param>
            /// <param name="height">缩略高度</param>
            /// <param name="type">目标图像文件格式</param>
            /// <param name="backColor">背景色</param>
            /// <param name="memoryPool">输出数据缓冲区内存池</param>
            /// <param name="seek">输出数据起始位置</param>
            public void Pad(ref subArray<byte> data, ref int width, ref int height, ImageFormat type, Color backColor, memoryPool memoryPool, int seek)
            {
                if (checkPad(ref width, ref height))
                {
                    if (memoryPool == null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            if (seek != 0) stream.Seek(seek, SeekOrigin.Begin);
                            pad(stream, width, height, type, backColor);
                            data.UnsafeSet(stream.GetBuffer(), seek, (int)stream.Position - seek);
                            return;
                        }
                    }
                    byte[] buffer = memoryPool.Get();
                    try
                    {
                        using (MemoryStream stream = memoryStream.UnsafeNew(buffer))
                        {
                            if (seek != 0) stream.Seek(seek, SeekOrigin.Begin);
                            pad(stream, width, height, type, backColor);
                            byte[] bufferData = stream.GetBuffer();
                            if (buffer == bufferData)
                            {
                                buffer = null;
                                //showjim
                                if ((int)stream.Position > bufferData.Length)
                                {
                                    log.Error.Add("Position " + ((int)stream.Position).toString() + " > " + bufferData.Length.toString(), null, false);
                                }
                            }
                            data.UnsafeSet(bufferData, seek, (int)stream.Position - seek);
                            return;
                        }
                    }
                    finally { memoryPool.PushOnly(buffer); }
                }
                data.Null();
            }
            /// <summary>
            /// 获取缩略图
            /// </summary>
            /// <param name="stream">输出数据流</param>
            /// <param name="width">缩略宽度</param>
            /// <param name="height">缩略高度</param>
            /// <param name="type">目标图像文件格式</param>
            /// <param name="backColor">背景色</param>
            private void pad(Stream stream, int width, int height, ImageFormat type, Color backColor)
            {
                using (Bitmap bitmap = new Bitmap(width, height))
                using (Graphics graphic = Graphics.FromImage(bitmap))
                {
                    if (!backColor.IsEmpty) graphic.Clear(backColor);
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;
                    graphic.DrawImage(image, new Rectangle(left, top, right - left, bottom - top), new Rectangle(0, 0, this.width, this.height), GraphicsUnit.Pixel);
                    bitmap.Save(stream, getImageCodec(type ?? ImageFormat.Jpeg) ?? jpegImageCodecInfo, qualityEncoder);
                }
            }
        }
        unsafe static thumbnail()
        {
            (qualityEncoder = new EncoderParameters(1)).Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            ImageCodecInfo[] infos = ImageCodecInfo.GetImageDecoders();
            imageCodecs = new fastCSharp.stateSearcher.byteArray<ImageCodecInfo>(infos.getArray(value => fastCSharp.guid.ToByteArray(value.FormatID)), infos, true);
            guid guid = new guid { Value = ImageFormat.Jpeg.Guid };
            jpegImageCodecInfo = imageCodecs.Get(&guid, 16);
        }
    }
}
