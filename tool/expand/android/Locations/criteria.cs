using System;
using Android.Locations;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Locations.Criteria À©Õ¹
    /// </summary>
    public static class criteria
    {
        /// <summary>
        /// CostAllowed
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setCostAllowed(this Criteria criteria, bool value)
        {
            criteria.CostAllowed = value;
        }
        /// <summary>
        /// Accuracy
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setAccuracy(this Criteria criteria, Accuracy value)
        {
            criteria.Accuracy = value;
        }
        /// <summary>
        /// Accuracy
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setAccuracy(this Criteria criteria, int value)
        {
            criteria.Accuracy = (Accuracy)value;
        }
    }
}