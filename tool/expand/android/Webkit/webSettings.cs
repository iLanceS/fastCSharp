using System;
using Android.Webkit;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Webkit.WebSettings
    /// </summary>
    public static class webSettings
    {
        /// <summary>
        /// Android.Webkit.WebSettings.RenderPriority À©Õ¹
        /// </summary>
        public static class RenderPriority
        {
            /// <summary>
            /// Android.Webkit.WebSettings.RenderPriority.High
            /// </summary>
            public static Android.Webkit.WebSettings.RenderPriority HIGH
            {
                get { return Android.Webkit.WebSettings.RenderPriority.High; }
            }
        }
        /// <summary>
        /// SavePassword
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setSavePassword(this WebSettings settings, bool value)
        {
            settings.SavePassword = value;
        }
        /// <summary>
        /// SaveFormData
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setSaveFormData(this WebSettings settings, bool value)
        {
            settings.SaveFormData = value;
        }
        /// <summary>
        /// CacheMode
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setCacheMode(this WebSettings settings, CacheModes value)
        {
            settings.CacheMode = value;
        }
        /// <summary>
        /// CacheMode
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setCacheMode(this WebSettings settings, int value)
        {
            settings.CacheMode = (CacheModes)value;
        }
        /// <summary>
        /// SetNeedInitialFocus
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setNeedInitialFocus(this WebSettings settings, bool value)
        {
            settings.SetNeedInitialFocus(value);
        }
        /// <summary>
        /// BuiltInZoomControls
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setBuiltInZoomControls(this WebSettings settings, bool value)
        {
            settings.BuiltInZoomControls = value;
        }
        /// <summary>
        /// SetSupportZoom
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setSupportZoom(this WebSettings settings, bool value)
        {
            settings.SetSupportZoom(value);
        }
        /// <summary>
        /// SetRenderPriority
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setRenderPriority(this WebSettings settings, WebSettings.RenderPriority value)
        {
            settings.SetRenderPriority(value);
        }
        /// <summary>
        /// JavaScriptEnabled
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setJavaScriptEnabled(this WebSettings settings, bool value)
        {
            settings.JavaScriptEnabled = value;
        }
        /// <summary>
        /// DatabaseEnabled
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setDatabaseEnabled(this WebSettings settings, bool value)
        {
            settings.DatabaseEnabled = value;
        }
        /// <summary>
        /// DatabasePath
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="path"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setDatabasePath(this WebSettings settings, string path)
        {
            settings.DatabasePath = path;
        }
        /// <summary>
        /// DomStorageEnabled
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setDomStorageEnabled(this WebSettings settings, bool value)
        {
            settings.DomStorageEnabled = value;
        }
    }
}