using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace fastCSharp.xamarinForms.ui
{
    /// <summary>
    /// WebView
    /// </summary>
    public class webView : View
    {
        /// <summary>
        /// JavaScript 调用事件
        /// </summary>
        public event Action<string> OnJavascriptCall;
        /// <summary>
        /// URI 绑定属性
        /// </summary>
        private static readonly BindableProperty uriProperty = BindableProperty.Create(propertyName: "Uri", returnType: typeof(string), declaringType: typeof(webView), defaultValue: default(string));
        /// <summary>
        /// URI
        /// </summary>
        public string Uri
        {
            get { return (string)GetValue(uriProperty); }
            set { SetValue(uriProperty, value); }
        }
        /// <summary>
        /// 清除绑定数据
        /// </summary>
        public void Cleanup()
        {
            OnJavascriptCall = null;
        }
        /// <summary>
        /// JavaScript 调用
        /// </summary>
        /// <param name="data"></param>
        public void CallJavascript(string data)
        {
            OnJavascriptCall?.Invoke(data);
        }
    }
}
