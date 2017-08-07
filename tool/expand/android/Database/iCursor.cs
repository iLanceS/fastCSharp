using System;
using Android.Database;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Database.ICursor À©Õ¹
    /// </summary>
    public static class iCursor
    {
        /// <summary>
        /// MoveToNext
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool moveToNext(this ICursor cursor)
        {
            return cursor.MoveToNext();
        }
        /// <summary>
        /// GetString
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getString(this ICursor cursor, int columnIndex)
        {
            return cursor.GetString(columnIndex);
        }
        /// <summary>
        /// GetInt
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getInt(this ICursor cursor, int columnIndex)
        {
            return cursor.GetInt(columnIndex);
        }
        /// <summary>
        /// GetLong
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getLong(this ICursor cursor, int columnIndex)
        {
            return cursor.GetLong(columnIndex);
        }
        /// <summary>
        /// Close
        /// </summary>
        /// <param name="cursor"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void close(this ICursor cursor)
        {
            cursor.Close();
        }
        /// <summary>
        /// MoveToFirst
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool moveToFirst(this ICursor cursor)
        {
            return cursor.MoveToFirst();
        }
        /// <summary>
        /// IsAfterLast
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isAfterLast(this ICursor cursor)
        {
            return cursor.IsAfterLast;
        }
        /// <summary>
        /// GetColumnIndex
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getColumnIndex(this ICursor cursor, string columnName)
        {
            return cursor.GetColumnIndex(columnName);
        }
        /// <summary>
        /// Count
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getCount(this ICursor cursor)
        {
            return cursor.Count;
        }
        /// <summary>
        /// GetBlob
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte[] getBlob(this ICursor cursor, int columnIndex)
        {
            return cursor.GetBlob(columnIndex);
        }
    }
}