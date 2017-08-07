using System;
using Android.Database.Sqlite;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Database.Sqlite.SQLiteOpenHelper À©Õ¹
    /// </summary>
    public static class sqliteOpenHelper
    {
        /// <summary>
        /// WritableDatabase
        /// </summary>
        /// <param name="sqlHelper"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SQLiteDatabase getWritableDatabase(this SQLiteOpenHelper sqlHelper)
        {
            return sqlHelper.WritableDatabase;
        }
        /// <summary>
        /// ReadableDatabase
        /// </summary>
        /// <param name="sqlHelper"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SQLiteDatabase getReadableDatabase(this SQLiteOpenHelper sqlHelper)
        {
            return sqlHelper.ReadableDatabase;
        }
    }
}