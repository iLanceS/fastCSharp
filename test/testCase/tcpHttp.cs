using System;
using System.Collections.Specialized;
using fastCSharp.reflection;
using fastCSharp.code.cSharp;
using System.IO;
using fastCSharp.io;

namespace fastCSharp.testCase
{
    /// <summary>
    /// TCP服务兼容HTTP服务测试，必须指定[IsHttpClient = true]
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(IsHttpClient = true, IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    internal partial class tcpHttp
    {
        /// <summary>
        /// 测试数据
        /// </summary>
        internal static int incValue;
        /// <summary>
        /// 无参数无返回值调用测试
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false)]
        private void Inc()
        {
            ++incValue;
        }
        /// <summary>
        /// 单参数无返回值调用测试+输入参数包装处理测试
        /// </summary>
        /// <param name="a"></param>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false, IsInputSerializeBox = true)]
        private void Set(int a)
        {
            incValue = a;
        }
        /// <summary>
        /// 多参数无返回值调用测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false)]
        private void Add(int a, int b)
        {
            incValue = a + b;
        }

        /// <summary>
        /// 无参数有返回值调用测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false)]
        private int inc()
        {
            return ++incValue;
        }
        /// <summary>
        /// 单参数有返回值调用测试+输入参数包装处理测试
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false, IsInputSerializeBox = true, HttpName = "inc1")]
        private int inc(int a)
        {
            return a + 1;
        }
        /// <summary>
        /// 多参数有返回值调用测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false)]
        private int add(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// 输出参数测试
        /// </summary>
        /// <param name="outValue"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false, HttpName = "inc2")]
        private int inc(out int outValue)
        {
            outValue = incValue;
            return outValue + 1;
        }
        /// <summary>
        /// 混合输出参数测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false, HttpName = "inc3")]
        private int inc(int a, out int outValue)
        {
            outValue = a;
            return a + 1;
        }
        /// <summary>
        /// 混合输出参数测试
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false, HttpName = "add1")]
        private int add(int a, int b, out int outValue)
        {
            outValue = b;
            return a + b;
        }

        /// <summary>
        /// 写HTTP客户端Cookie测试
        /// </summary>
        /// <param name="client"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false)]
        private void setCookie(fastCSharp.net.tcp.commandServer.socket client, string name, string value)
        {
            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = client.HttpPage;
            httpPage.SetCookie(new net.tcp.http.cookie(name, value));
        }
        /// <summary>
        /// 写HTTP客户端Session测试
        /// </summary>
        /// <param name="client"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false, IsInputSerializeBox = true)]
        private bool setSession(fastCSharp.net.tcp.commandServer.socket client, string value)
        {
            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = client.HttpPage;
            return httpPage.SetSession(value);
        }
        /// <summary>
        /// 读取HTTP客户端Session测试
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsHttpPostOnly = false)]
        private string getSession(fastCSharp.net.tcp.commandServer.socket client)
        {
            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = client.HttpPage;
            return httpPage.GetSession<string>();
        }

        /// <summary>
        /// HTTP客户端文件上传测试
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(InputParameterMaxLength = 4 << 20)]
        private long httpUpload(fastCSharp.net.tcp.commandServer.socket client)
        {
            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = client.HttpPage;
            foreach (fastCSharp.net.tcp.http.requestForm.value file in httpPage.Form.Files)
            {
                if (file.SaveFileName == null) return file.Value.Count;
                FileInfo fileInfo = new FileInfo(file.SaveFileName);
                long length = fileInfo.Length;
                fileInfo.Delete();
                File.Delete(file.SaveFileName);
                return length;
            }
            return long.MinValue;
        }
#if NotFastCSharpCode
#else
        /// <summary>
        /// 返回值与输出参数实体定义
        /// </summary>
        private struct outReturn
        {
            /// <summary>
            /// 输出参数
            /// </summary>
            public int outValue;
            /// <summary>
            /// 返回值
            /// </summary>
            public int Return;
        }
        /// <summary>
        /// TCP服务兼容HTTP服务测试
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.testCase]
        internal static bool TestCase()
        {
            using (tcpHttp.tcpServer server = new tcpHttp.tcpServer())
            {
                if (server.Start())
                {
                    fastCSharp.code.cSharp.tcpServer tcpServer = fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.code.cSharp.tcpServer>(typeof(tcpHttp), false, false);
                    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                    string url = "http://" + tcpServer.Host + ":" + tcpServer.Port.toString() + "/";
                    using (tcpHttp.tcpClient client = new tcpHttp.tcpClient())
                    using (fastCSharp.net.webClient webClient = new fastCSharp.net.webClient())
                    {
                        incValue = 0;
                        //TCP调用
                        client.Inc();
                        if (incValue != 1) return false;
                        //HTTP+JSON调用
                        incValue = 0;
                        webClient.KeepAlive = false;
                        if (webClient.UploadData(url + "Inc", new byte[0]).deSerialize() != web.ajax.Object || incValue != 1) return false;
                        //HTTP+POST调用
                        incValue = 0;
                        if (webClient.UploadValues(url + "Inc", new NameValueCollection()).deSerialize() != web.ajax.Object || incValue != 1) return false;
                        //HTTP+GET调用
                        incValue = 0;
                        if (webClient.DownloadData(url + "Inc").deSerialize() != web.ajax.Object || incValue != 1) return false;

                        client.Set(3);
                        if (incValue != 3) return false;
                        incValue = 0;
                        if (webClient.UploadData(url + "Set", encoding.GetBytes(3.ToJson())).deSerialize() != web.ajax.Object || incValue != 3) return false;
                        //incValue = 0;
                        //if (webClient.UploadValues(url + "Set", fastCSharp.emit.formGetter<.form.Get(new { a = 3 })).deSerialize() != web.ajax.Object || incValue != 3) return false;
                        //incValue = 0;
                        //if (webClient.DownloadData(url + "Set?" + urlQuery.query.Get(new { a = 3 }, encoding)).deSerialize() != web.ajax.Object || incValue != 3) return false;

                        client.Add(2, 3);
                        if (incValue != 5) return false;
                        incValue = 0;
                        if (webClient.UploadData(url + "Add", encoding.GetBytes(fastCSharp.emit.jsonSerializer.ObjectToJson(new { a = 2, b = 3 }))).deSerialize() != web.ajax.Object || incValue != 5) return false;
                        //incValue = 0;
                        //if (webClient.UploadValues(url + "Add", urlQuery.form.Get(new { a = 2, b = 3 })).deSerialize() != web.ajax.Object || incValue != 5) return false;
                        //incValue = 0;
                        //if (webClient.DownloadData(url + "Add?" + urlQuery.query.Get(new { a = 2, b = 3 }, encoding)).deSerialize() != web.ajax.Object || incValue != 5) return false;

                        if (client.inc().Value != 6 || incValue != 6) return false;
                        incValue = 5;
                        if (fastCSharp.emit.jsonParser.Parse<int>(webClient.UploadData(url + "inc", new byte[0]).deSerialize()) != 6 || incValue != 6) return false;
                        incValue = 5;
                        if (fastCSharp.emit.jsonParser.Parse<int>(webClient.UploadValues(url + "inc", new NameValueCollection()).deSerialize()) != 6 || incValue != 6) return false;
                        incValue = 5;
                        if (fastCSharp.emit.jsonParser.Parse<int>(webClient.DownloadData(url + "inc").deSerialize()) != 6 || incValue != 6) return false;

                        if (client.inc(8).Value != 9) return false;
                        if (fastCSharp.emit.jsonParser.Parse<int>(webClient.UploadData(url + "inc1", encoding.GetBytes(8.ToJson())).deSerialize()) != 9) return false;
                        //if (webClient.UploadValues(url + "inc1", urlQuery.form.Get(new { a = 8 })).deSerialize() != "9") return false;
                        //if (webClient.DownloadData(url + "inc1?" + urlQuery.query.Get(new { a = 8 }, encoding)).deSerialize() != "9") return false;

                        if (client.add(10, 13).Value != 23) return false;
                        if (fastCSharp.emit.jsonParser.Parse<int>(webClient.UploadData(url + "add", encoding.GetBytes(fastCSharp.emit.jsonSerializer.ObjectToJson(new { a = 10, b = 13 }))).deSerialize()) != 23) return false;
                        //if (webClient.UploadValues(url + "add", urlQuery.form.Get(new { a = 10, b = 13 })).deSerialize() != "23") return false;
                        //if (webClient.DownloadData(url + "add?" + urlQuery.query.Get(new { a = 10, b = 13 }, encoding)).deSerialize() != "23") return false;

                        int a;
                        incValue = 15;
                        if (client.inc(out a).Value != 16 || a != 15) return false;
                        incValue = 15;
                        outReturn value = fastCSharp.emit.jsonParser.Parse<outReturn>(webClient.UploadData(url + "inc2", new byte[0]).deSerialize());
                        if (value.Return != 16 || value.outValue != 15) return false;
                        incValue = 15;
                        value = fastCSharp.emit.jsonParser.Parse<outReturn>(webClient.UploadValues(url + "inc2", new NameValueCollection()).deSerialize());
                        if (value.Return != 16 || value.outValue != 15) return false;
                        incValue = 15;
                        value = fastCSharp.emit.jsonParser.Parse<outReturn>(webClient.DownloadData(url + "inc2").deSerialize());
                        if (value.Return != 16 || value.outValue != 15) return false;

                        if (client.inc(20, out a).Value != 21 || a != 20) return false;
                        value = fastCSharp.emit.jsonParser.Parse<outReturn>(webClient.UploadData(url + "inc3", encoding.GetBytes(fastCSharp.emit.jsonSerializer.ObjectToJson(new { a = 20 }))).deSerialize());
                        if (value.Return != 21 || value.outValue != 20) return false;
                        //if (webClient.UploadValues(url + "inc3", urlQuery.form.Get(new { a = 20 })).deSerialize() != @"{""b"":20,""_Return_"":21}") return false;
                        //if (webClient.DownloadData(url + "inc3?" + urlQuery.query.Get(new { a = 20 }, encoding)).deSerialize() != @"{""b"":20,""_Return_"":21}") return false;

                        if (client.add(30, 33, out a).Value != 63 || a != 33) return false;
                        value = fastCSharp.emit.jsonParser.Parse<outReturn>(webClient.UploadData(url + "add1", encoding.GetBytes(fastCSharp.emit.jsonSerializer.ObjectToJson(new { a = 30, b = 33 }))).deSerialize());
                        if (value.Return != 63 || value.outValue != 33) return false;
                        //if (webClient.UploadValues(url + "add1", urlQuery.form.Get(new { a = 30, b = 33 })).deSerialize() != @"{""c"":33,""_Return_"":63}") return false;
                        //if (webClient.DownloadData(url + "add1?" + urlQuery.query.Get(new { a = 30, b = 33 }, encoding)).deSerialize() != @"{""c"":33,""_Return_"":63}") return false;

                        if (webClient.UploadData(url + "setCookie", encoding.GetBytes(fastCSharp.emit.jsonSerializer.ObjectToJson(new { name = "a", value = "b" }))).deSerialize() != web.ajax.Object) return false;
                        if (webClient.ResponseHeaders["Set-Cookie"] != "a=b") return false;
                        //if (webClient.UploadValues(url + "setCookie", urlQuery.form.Get(new { name = "c", value = "d" })).deSerialize() != web.ajax.Object) return false;
                        //if (webClient.ResponseHeaders["Set-Cookie"] != "c=d") return false;
                        //if (webClient.DownloadData(url + "setCookie?" + urlQuery.query.Get(new { name = "e", value = "f" }, encoding)).deSerialize() != web.ajax.Object) return false;
                        //if (webClient.ResponseHeaders["Set-Cookie"] != "e=f") return false;

                        if (!fastCSharp.emit.jsonParser.Parse<bool>(webClient.UploadData(url + "setSession", encoding.GetBytes("b".ToJson())).deSerialize())) return false;
                        webClient.Cookies.SetCookies(new Uri(url), webClient.ResponseHeaders["Set-Cookie"].Split(';')[0]);
                        if (fastCSharp.emit.jsonParser.Parse<string>(webClient.UploadData(url + "getSession", new byte[0]).deSerialize()) != "b") return false;

                        //if (webClient.UploadValues(url + "setSession", urlQuery.form.Get(new { value = "d" })).deSerialize() != @"1") return false;
                        //if (webClient.UploadValues(url + "getSession", new NameValueCollection()).deSerialize() != @"""d""") return false;

                        //if (webClient.DownloadData(url + "setSession?" + urlQuery.query.Get(new { value = "f" }, encoding)).deSerialize() != @"1") return false;
                        //if (webClient.DownloadData(url + "getSession").deSerialize() != @"""f""") return false;
#if APP
#else
                        //HTTP文件上传
#if MONO
                        FileInfo fileInfo = new FileInfo((@"..\..\Program.cs").pathSeparator());
#else
                        FileInfo fileInfo = new FileInfo((@"..\..\tcpHttp.cs").pathSeparator());
#endif
                        if (fastCSharp.emit.jsonParser.Parse<long>(webClient.UploadFile(url + "httpUpload", fileInfo.FullName).deSerialize()) != fileInfo.Length) return false;
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
