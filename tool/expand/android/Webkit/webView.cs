using System;
using Android.Webkit;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Webkit.WebView À©Õ¹
    /// </summary>
    public static class webView
    {
        /// <summary>
        /// VerticalScrollBarEnabled
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setVerticalScrollBarEnabled(this WebView webView, bool value)
        {
            webView.VerticalScrollBarEnabled = value;
        }
        /// <summary>
        /// HorizontalScrollBarEnabled
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setHorizontalScrollBarEnabled(this WebView webView, bool value)
        {
            webView.HorizontalScrollBarEnabled = value;
        }
        /// <summary>
        /// SetWebViewClient
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="client"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setWebViewClient(this WebView webView, WebViewClient client)
        {
            webView.SetWebViewClient(client);
        }
        /// <summary>
        /// SetWebChromeClient
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="client"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setWebChromeClient(this WebView webView, WebChromeClient client)
        {
            webView.SetWebChromeClient(client);
        }
        /// <summary>
        /// ClearFormData
        /// </summary>
        /// <param name="webView"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void clearFormData(this WebView webView)
        {
            webView.ClearFormData();
        }
        /// <summary>
        /// Settings
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static WebSettings getSettings(this WebView webView)
        {
            return webView.Settings;
        }
        /// <summary>
        /// LoadUrl
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void loadUrl(this WebView webView, string url)
        {
            webView.LoadUrl(url);
        }
        /// <summary>
        /// Visibility
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setVisibility(this WebView webView, Android.Views.ViewStates value)
        {
            webView.Visibility = value;
        }
        /// <summary>
        /// Visibility
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setVisibility(this WebView webView, int value)
        {
            webView.Visibility = (Android.Views.ViewStates)value;
        }
        /// <summary>
        /// Url
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getUrl(this WebView webView)
        {
            return webView.Url;
        }
    }
}