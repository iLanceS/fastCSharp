using System;
using System.Collections.Generic;
using Android.Hardware;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Hardware.SensorManager À©Õ¹
    /// </summary>
    public static class sensorManager
    {
        /// <summary>
        /// GetSensorList
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IList<Sensor> getSensorList(this SensorManager manager, SensorType type)
        {
            return manager.GetSensorList(type);
        }
        /// <summary>
        /// GetSensorList
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IList<Sensor> getSensorList(this SensorManager manager, int type)
        {
            return manager.GetSensorList((SensorType)type);
        }
    }
}