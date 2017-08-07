using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace fastCSharp.testCase
{
    public class App : Application
    {
        private static string test(string name, Func<bool> test)
        {
            try
            {
                return name + " 测试 " + test().ToString();
            }
            catch (Exception error)
            {
                return name + @" 测试 ERROR
" + error.ToString();
            }
        }
        public App()
        {
            fastCSharp.list<string> tests = new list<string>();
            tests.Add("fastCSharp Android 支持测试");
            tests.Add(test("JSON 序列化", json.TestCase));
            tests.Add(test("二进制 Serialize", dataSerialize.TestCase));
            tests.Add(test("XML Serialize", xml.TestCase));
            tests.Add(test("Index Serialize", indexSerialize.TestCase));

            tests.Add(test("TCP跨类型静态调用", TcpCall.TestCase));
            tests.Add(test("TCP服务字段与属性支持", tcpMember.TestCase));
            tests.Add(test("TCP服务JSON序列化支持", tcpJson.TestCase));
            tests.Add(test("TCP服务客户端识别", tcpSession.TestCase));
            tests.Add(test("TCP服务Stream支持", tcpStream.TestCase));
            tests.Add(test("TCP服务泛型支持", tcpGeneric.TestCase));

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = tests.joinString(@"
")
                        }
                    }
                }
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
