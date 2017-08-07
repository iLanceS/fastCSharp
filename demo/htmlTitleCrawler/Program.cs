using System;
using System.Threading;

namespace htmlTitleCrawler
{
    class Program
    {
        /// <summary>
        /// URL抓取标题回调
        /// </summary>
        class urlTitle
        {
            /// <summary>
            /// 当前未完成的抓取数量
            /// </summary>
            private static int currentCount;
            /// <summary>
            /// URL
            /// </summary>
            private string url;
            /// <summary>
            /// 抓取标题回调
            /// </summary>
            /// <param name="title">标题</param>
            private void onTitle(string title)
            {
                Console.WriteLine("[" + Interlocked.Decrement(ref currentCount) + "] " + url + " => " + (title ?? "null"));
            }
            /// <summary>
            /// 根据URL抓取标题
            /// </summary>
            /// <param name="client">HTML标题获取客户端任务池</param>
            /// <param name="url">URL</param>
            internal static void Crawl(fastCSharp.net.htmlTitleClient.task client, string url)
            {
                urlTitle urlTitle = new urlTitle { url = url };
                Interlocked.Increment(ref currentCount);
                client.Get(url, urlTitle.onTitle);
            }
        }

        static void Main(string[] args)
        {
            using (fastCSharp.net.htmlTitleClient.task client = new fastCSharp.net.htmlTitleClient.task(100, 1 << 11, 128 << 10))
            {
                Console.WriteLine(@"Press quit to exit.");
                Console.WriteLine(@"Press url to crawl title.");
                urlTitle.Crawl(client, "http://www.baidu.com/");
                do
                {
                    string url = Console.ReadLine();
                    if (url == "quit") break;
                    urlTitle.Crawl(client, url);
                }
                while (true);
            }
        }
    }
}
