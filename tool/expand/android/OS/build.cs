using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.Build À©Õ¹
    /// </summary>
    public sealed class build
    {
        /// <summary>
        /// Android.OS.Build.Model
        /// </summary>
        public static string MODEL
        {
            get { return Android.OS.Build.Model; }
        }
        /// <summary>
        /// Android.OS.Build.Device
        /// </summary>
        public static string DEVICE
        {
            get { return Android.OS.Build.Device; }
        }
        /// <summary>
        /// Android.OS.Build.CpuAbi
        /// </summary>
        public static string CPU_ABI
        {
            get { return Android.OS.Build.CpuAbi; }
        }
        /// <summary>
        /// Android.OS.Build.Manufacturer
        /// </summary>
        public static string MANUFACTURER
        {
            get { return Android.OS.Build.Manufacturer; }
        }
        /// <summary>
        /// Android.OS.Build.VERSION À©Õ¹
        /// </summary>
        public sealed class VERSION
        {
            /// <summary>
            /// Android.OS.Build.VERSION.Release
            /// </summary>
            public static string RELEASE
            {
                get { return Android.OS.Build.VERSION.Release; }
            }
            /// <summary>
            /// Android.OS.Build.VERSION.Sdk
            /// </summary>
            public static string SDK
            {
                get { return Android.OS.Build.VERSION.Sdk; }
            }
            /// <summary>
            /// Android.OS.Build.VERSION.SdkInt
            /// </summary>
            public static BuildVersionCodes SDK_INT
            {
                get { return Android.OS.Build.VERSION.SdkInt; }
            }
        }
    }
}