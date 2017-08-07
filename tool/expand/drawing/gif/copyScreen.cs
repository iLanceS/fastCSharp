using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Drawing.Imaging;
using fastCSharp.threading;

namespace fastCSharp.drawing.gif
{
    /// <summary>
    /// 截屏到GIF文件
    /// </summary>
    public sealed class copyScreen : IDisposable
    {
        /// <summary>
        /// GIF文件
        /// </summary>
        private file.writer gif;
        /// <summary>
        /// 截屏设备
        /// </summary>
        private Screen screen;
        /// <summary>
        /// X方向偏移量
        /// </summary>
        private int leftOffset;
        /// <summary>
        /// Y方向偏移量
        /// </summary>
        private int topOffset;
        /// <summary>
        /// 截屏定时器
        /// </summary>
        private System.Timers.Timer timer;
        /// <summary>
        /// 截屏定时毫秒数
        /// </summary>
        private double interval;
        /// <summary>
        /// 未处理的截屏图片集合
        /// </summary>
        private collection<Bitmap> bitmaps;
        /// <summary>
        /// 截屏处理访问锁
        /// </summary>
        private object bitmapLock = new object();
        /// <summary>
        /// 是否正在抓屏
        /// </summary>
        private int isScreen;
        /// <summary>
        /// 跳屏数量
        /// </summary>
        private int keepScreenCount;
        /// <summary>
        /// 最大色彩深度
        /// </summary>
        private byte maxPixel;
        /// <summary>
        /// 释放资源是否等待GIF文件处理结束
        /// </summary>
        private bool isWaitFinally;
        /// <summary>
        /// GIF文件处理是否结束
        /// </summary>
        private bool isFinally;
        /// <summary>
        /// 截屏到GIF文件
        /// </summary>
        /// <param name="filename">GIF文件名</param>
        /// <param name="interval">截屏定时毫秒数</param>
        /// <param name="leftOffset">X方向偏移量</param>
        /// <param name="topOffset">Y方向偏移量</param>
        /// <param name="width">截屏高度</param>
        /// <param name="height">截屏宽度</param>
        /// <param name="screen">截屏设备</param>
        /// <param name="maxPixel">最大色彩深度</param>
        /// <param name="isWaitFinally">释放资源是否等待GIF文件处理结束</param>
        public copyScreen(string filename, double interval, int leftOffset = 0, int topOffset = 0, int width = 0, int height = 0, Screen screen = null, byte maxPixel = 8, bool isWaitFinally = true)
        {
            this.screen = screen;
            if (screen == null) screen = Screen.PrimaryScreen;
            if (width == 0)
            {
                if (leftOffset < 0) leftOffset = 0;
                width = screen.Bounds.Width;
            }
            else
            {
                if (leftOffset < 0)
                {
                    width += leftOffset;
                    leftOffset = 0;
                }
            }
            if (height == 0)
            {
                if (topOffset < 0) leftOffset = 0;
                height = screen.Bounds.Height;
            }
            else
            {
                if (topOffset < 0)
                {
                    height += topOffset;
                    topOffset = 0;
                }
            }
            if (width <= 0 || height <= 0) fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.IndexOutOfRange);
            if (width > short.MaxValue) width = short.MaxValue;
            if (height > short.MaxValue) height = short.MaxValue;
            this.leftOffset = leftOffset;
            this.topOffset = topOffset;
            this.maxPixel = (byte)(maxPixel - 2) < 8 ? maxPixel : (byte)8;
            this.isWaitFinally = isWaitFinally;
            gif = new fastCSharp.drawing.gif.file.writer(filename, (short)width, (short)height);
            timer = new System.Timers.Timer(this.interval = interval < 40 ? 40 : interval);
            timer.Elapsed += nextScreen;
            bitmaps = new collection<Bitmap>();
            nextScreen(null, null);
            timer.Start();
            threadPool.TinyPool.Start(write);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Monitor.Enter(bitmapLock);
            try
            {
                dispose();
                Monitor.Pulse(bitmapLock);
            }
            finally { Monitor.Exit(bitmapLock); }
            if (isWaitFinally)
            {
                Monitor.Enter(bitmapLock);
                try
                {
                    while (!isFinally) Monitor.Wait(bitmapLock);
                }
                finally { Monitor.Exit(bitmapLock); }
            }
        }
        /// <summary>
        /// 停止截屏
        /// </summary>
        private void dispose()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            timer = null;
            screen = null;
        }
        /// <summary>
        /// 定时截屏处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextScreen(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref isScreen, 1) == 0)
            {
                Bitmap bitmap = null;
                try
                {
                    Screen screen = this.screen == null ? Screen.PrimaryScreen : this.screen;
                    int screenWidth = screen.Bounds.Width, screenHeight = screen.Bounds.Height;
                    int width = screenWidth - leftOffset, height = screenHeight - topOffset;
                    if (width > gif.Width) width = gif.Width;
                    if (height > gif.Height) height = gif.Height;
                    if (width > 0 && height > 0)
                    {
                        bitmap = new Bitmap(width, height);
                        try
                        {
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                graphics.CopyFromScreen(leftOffset, topOffset, 0, 0, new Size(width, height));
                            }
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            pub.Dispose(ref bitmap);
                        }
                    }
                }
                finally { isScreen = 0; }
                if (bitmap != null)
                {
                    bool isAdd = false;
                    Monitor.Enter(bitmapLock);
                    try
                    {
                        if (bitmaps != null)
                        {
                            bitmaps.Add(bitmap);
                            while (keepScreenCount != 0)
                            {
                                bitmaps.Add((Bitmap)null);
                                Interlocked.Decrement(ref keepScreenCount);
                            }
                            isAdd = true;
                        }
                        Monitor.Pulse(bitmapLock);
                    }
                    finally
                    {
                        Monitor.Exit(bitmapLock);
                        if (!isAdd) bitmap.Dispose();
                    }
                }
            }
            else Interlocked.Increment(ref keepScreenCount);
        }
        /// <summary>
        /// GIF文件处理线程
        /// </summary>
        private unsafe void write()
        {
            Bitmap lastBitmap = null, currentBitmap = null;
            BitmapData lastBitmapData = null, currentBitmapData = null;
            try
            {
                double currentInterval = -interval;
                while (true)
                {
                    Monitor.Enter(bitmapLock);
                    try
                    {
                        if (bitmaps.Count == 0)
                        {
                            if (timer == null) break;
                            Monitor.Wait(bitmapLock);
                        }
                        if (bitmaps.Count == 0) break;
                        currentBitmap = bitmaps.PopExpand();
                    }
                    finally { Monitor.Exit(bitmapLock); }
                    currentInterval += interval;
                    if (currentBitmap != null)
                    {
                        int left = 0, top = 0, right = currentBitmap.Width, bottom = currentBitmap.Height;
                        currentBitmapData = currentBitmap.LockBits(new Rectangle(0, 0, right, bottom), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        if (lastBitmapData != null)
                        {
                            byte* lastBitmapFixed = (byte*)lastBitmapData.Scan0, currentBitmapFixed = (byte*)currentBitmapData.Scan0;
                            int minHeight = currentBitmap.Height <= lastBitmap.Height ? currentBitmap.Height : lastBitmap.Height;
                            int minWidth = lastBitmap.Width, width3;
                            if (currentBitmap.Width <= lastBitmap.Width)
                            {
                                minWidth = currentBitmap.Width;
                                width3 = (minWidth << 1) + minWidth;
                                for (byte* lastRow = lastBitmapFixed, currentRow = currentBitmapFixed; top != minHeight; ++top)
                                {
                                    if (!fastCSharp.unsafer.memory.Equal(lastRow, currentRow, width3)) break;
                                    lastRow += lastBitmapData.Stride;
                                    currentRow += currentBitmapData.Stride;
                                }
                                if (currentBitmap.Height <= lastBitmap.Height && top != minHeight)
                                {
                                    ++top;
                                    for (byte* lastRow = lastBitmapFixed + lastBitmapData.Stride * minHeight, currentRow = currentBitmapFixed + currentBitmapData.Stride * minHeight; top != bottom; --bottom)
                                    {
                                        if (!fastCSharp.unsafer.memory.Equal(lastRow -= lastBitmapData.Stride, currentRow -= currentBitmapData.Stride, width3)) break;
                                    }
                                    --top;
                                }
                            }
                            if (currentBitmap.Height <= lastBitmap.Height && top != minHeight)
                            {
                                width3 = (minWidth << 1) + minWidth;
                                int endRowStride = lastBitmapData.Stride * (bottom - top);
                                byte* lastTopRow = lastBitmapFixed + lastBitmapData.Stride * top, currentTopRow = currentBitmapFixed + currentBitmapData.Stride * top;
                                if ((((int)lastBitmapFixed & (sizeof(ulong) - 1)) | (lastBitmapData.Stride & (sizeof(ulong) - 1)) | ((int)currentBitmapFixed & (sizeof(ulong) - 1)) | (currentBitmapData.Stride & (sizeof(ulong) - 1))) == 0)
                                {
                                    byte* lastTopCol = lastTopRow, topColEnd = lastTopRow + width3 - 1;
                                    ulong color = 0;
                                    for (byte* currentTopCol = currentTopRow; lastTopCol <= topColEnd; lastTopCol += sizeof(ulong), currentTopCol += sizeof(ulong))
                                    {
                                        color = 0;
                                        for (byte* lastRow = lastTopCol, currentRow = currentTopCol, endRow = lastRow + endRowStride; lastRow != endRow; lastRow += lastBitmapData.Stride, currentRow += currentBitmapData.Stride)
                                        {
                                            color |= *(ulong*)lastRow ^ *(ulong*)currentRow;
                                        }
                                        if (color != 0) break;
                                    }
                                    int length = (int)(lastTopCol - lastTopRow);
                                    if (lastTopCol <= topColEnd) length += color.endBits() >> 3;
                                    left += (length /= 3) < minWidth ? length : minWidth;
                                    if (currentBitmap.Width <= lastBitmap.Width && left != minWidth)
                                    {
                                        int offset = width3 & (sizeof(ulong) - 1);
                                        byte* currentTopCol = currentTopRow + width3;
                                        lastTopCol = lastTopRow + width3;
                                        length = 0;
                                        if (offset != 0)
                                        {
                                            currentTopCol -= offset;
                                            lastTopCol -= offset;
                                            color = 0;
                                            for (byte* lastRow = lastTopCol, currentRow = currentTopCol, endRow = lastRow + endRowStride; lastRow != endRow; lastRow += lastBitmapData.Stride, currentRow += currentBitmapData.Stride)
                                            {
                                                color |= *(ulong*)lastRow ^ *(ulong*)currentRow;
                                            }
                                            if ((color <<= ((sizeof(ulong) - offset) << 3)) == 0) length = offset;
                                            else length = ((sizeof(ulong) << 3) - color.bits()) >> 3;
                                        }
                                        if (length == offset)
                                        {
                                            topColEnd = lastTopCol;
                                            do
                                            {
                                                color = 0;
                                                for (byte* lastRow = (lastTopCol -= sizeof(ulong)), currentRow = (currentTopCol -= sizeof(ulong)), endRow = lastRow + endRowStride; lastRow != endRow; lastRow += lastBitmapData.Stride, currentRow += currentBitmapData.Stride)
                                                {
                                                    color |= *(ulong*)lastRow ^ *(ulong*)currentRow;
                                                }
                                            }
                                            while (color == 0);
                                            length += (int)(topColEnd - lastTopCol) - sizeof(ulong) + ((sizeof(ulong) << 3) - color.bits()) >> 3;
                                        }
                                        right -= length / 3;
                                    }
                                }
                                else
                                {
                                    for (byte* lastTopCol = lastTopRow, topColEnd = lastTopRow + width3, currentTopCol = currentTopRow; lastTopCol != topColEnd; lastTopCol += 3, currentTopCol += 3, ++left)
                                    {
                                        int color = 0;
                                        for (byte* lastRow = lastTopCol, currentRow = currentTopCol, endRow = lastRow + endRowStride; lastRow != endRow; lastRow += lastBitmapData.Stride, currentRow += currentBitmapData.Stride)
                                        {
                                            color |= *(int*)lastRow ^ *(int*)currentRow;
                                        }
                                        if ((color & 0xffffff) != 0) break;
                                    }
                                    if (currentBitmap.Width <= lastBitmap.Width && left != minWidth)
                                    {
                                        byte* lastTopCol = lastTopRow + width3, currentTopCol = currentTopRow + width3;
                                        do
                                        {
                                            int color = 0;
                                            for (byte* lastRow = (lastTopCol -= 3), currentRow = (currentTopCol -= 3), endRow = lastRow + endRowStride; lastRow != endRow; lastRow += lastBitmapData.Stride, currentRow += currentBitmapData.Stride)
                                            {
                                                color |= *(int*)lastRow ^ *(int*)currentRow;
                                            }
                                            if ((color & 0xffffff) != 0) break;
                                            --right;
                                        }
                                        while (true);
                                    }
                                }
                            }
                        }
                        if (top == bottom)
                        {
                            currentBitmap.UnlockBits(currentBitmapData);
                            currentBitmapData = null;
                            currentBitmap.Dispose();
                            currentBitmap = null;
                        }
                        else
                        {
                            int delayTime = (int)(currentInterval / 10);
                            for (currentInterval -= delayTime * 10; delayTime > short.MaxValue; delayTime -= short.MaxValue)
                            {
                                if (!gif.AddGraphicControl(short.MaxValue, fastCSharp.drawing.gif.file.graphicControl.methodType.None, true)) break;
                                if (!gif.addImage(lastBitmapData, 0, 0, 0, 0, 1, 1, false, maxPixel)) break;
                            }
                            if (lastBitmap != null)
                            {
                                lastBitmap.UnlockBits(lastBitmapData);
                                lastBitmapData = null;
                                lastBitmap.Dispose();
                                lastBitmap = null;
                            }
                            if (delayTime != 0 && !gif.AddGraphicControl((short)delayTime, fastCSharp.drawing.gif.file.graphicControl.methodType.None, true)) break;
                            if (!gif.addImage(currentBitmapData, left, top, left, top, right - left, bottom - top, false, maxPixel)) break;
                            lastBitmapData = currentBitmapData;
                            lastBitmap = currentBitmap;
                            currentBitmapData = null;
                            currentBitmap = null;
                        }
                    }
                }
            }
            finally
            {
                collection<Bitmap> bitmaps = this.bitmaps;
                Monitor.Enter(bitmapLock);
                try
                {
                    dispose();
                    gif.Dispose();
                    gif = null;
                    this.bitmaps = null;
                    isFinally = true;
                    Monitor.Pulse(bitmapLock);
                }
                finally
                {
                    Monitor.Exit(bitmapLock);
                    if (lastBitmap != null)
                    {
                        if (lastBitmapData != null) lastBitmap.UnlockBits(lastBitmapData);
                        lastBitmap.Dispose();
                    }
                    if (currentBitmap != null)
                    {
                        if (currentBitmapData != null) currentBitmap.UnlockBits(currentBitmapData);
                        currentBitmap.Dispose();
                    }
                    foreach (Bitmap bitmap in bitmaps) bitmap.Dispose();
                }
            }
        }
    }

}
