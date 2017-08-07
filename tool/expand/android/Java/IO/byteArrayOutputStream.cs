using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.ByteArrayOutputStream 扩展
    /// </summary>
    public static class byteArrayOutputStream
    {
        private sealed class stream : System.IO.Stream
        {
            private Java.IO.ByteArrayOutputStream outputStream;
            public stream(Java.IO.ByteArrayOutputStream stream)
            {
                outputStream = stream;
            }
            public override bool CanRead
            {
                get
                {
                    return false;
                }
            }
            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }
            public override bool CanWrite
            {
                get
                {
                    return true;
                }
            }
            public override long Length
            {
                get
                {
                    return outputStream.Size();
                }
            }
            public override long Position
            {
                get
                {
                    return outputStream.Size();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public override void Flush()
            {
                outputStream.Flush();
            }
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
            public override long Seek(long offset, System.IO.SeekOrigin origin)
            {
                throw new NotImplementedException();
            }
            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public override void Write(byte[] buffer, int offset, int count)
            {
                outputStream.Write(buffer, offset, count);
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                fastCSharp.pub.Dispose(ref outputStream);
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public override void Close()
            {
                base.Close();
                fastCSharp.pub.Dispose(ref outputStream);
            }
        }
        /// <summary>
        /// System.IO.Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream toStream(this Java.IO.ByteArrayOutputStream stream)
        {
            return new stream(stream);
        }
        /// <summary>
        /// 获取字节数组 ToByteArray
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte[] toByteArray(this Java.IO.ByteArrayOutputStream stream)
        {
            return stream.ToByteArray();
        }
    }
}