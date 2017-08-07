using System;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Database.Sqlite.SQLiteDatabase À©Õ¹
    /// </summary>
    public static class sqliteDatabase
    {
        /// <summary>
        /// Query
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        /// <param name="groupBy"></param>
        /// <param name="having"></param>
        /// <param name="orderBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ICursor query(this SQLiteDatabase db, string table, string[] columns, string selection, string[] selectionArgs, string groupBy, string having, string orderBy, string limit)
        {
            return db.Query(table, columns, selection, selectionArgs, groupBy, having, orderBy, limit);
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="values"></param>
        /// <param name="whereClause"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int update(this SQLiteDatabase db, string table, ContentValues values, string whereClause, string[] whereArgs)
        {
            return db.Update(table, values, whereClause, whereArgs);
        }
        /// <summary>
        /// Replace
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="nullColumnHack"></param>
        /// <param name="initialValues"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long replace(this SQLiteDatabase db, string table, string nullColumnHack, ContentValues initialValues)
        {
            return db.Replace(table, nullColumnHack, initialValues);
        }
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="nullColumnHack"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long insert(this SQLiteDatabase db, string table, string nullColumnHack, ContentValues values)
        {
            return db.Insert(table, nullColumnHack, values);
        }
        /// <summary>
        /// BeginTransaction
        /// </summary>
        /// <param name="db"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void beginTransaction(this SQLiteDatabase db)
        {
            db.BeginTransaction();
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="whereClause"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int delete(this SQLiteDatabase db, string table, string whereClause, string[] whereArgs)
        {
            return db.Delete(table, whereClause, whereArgs);
        }
        /// <summary>
        /// SetTransactionSuccessful
        /// </summary>
        /// <param name="db"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setTransactionSuccessful(this SQLiteDatabase db)
        {
            db.SetTransactionSuccessful();
        }
        /// <summary>
        /// EndTransaction
        /// </summary>
        /// <param name="db"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void endTransaction(this SQLiteDatabase db)
        {
            db.EndTransaction();
        }
        /// <summary>
        /// Query
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        /// <param name="groupBy"></param>
        /// <param name="having"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ICursor query(this SQLiteDatabase db, string table, string[] columns, string selection, string[] selectionArgs, string groupBy, string having, string orderBy)
        {
            return db.Query(table, columns, selection, selectionArgs, groupBy, having, orderBy);
        }
        /// <summary>
        /// ExecSQL
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void execSQL(this SQLiteDatabase db, string sql)
        {
            db.ExecSQL(sql);
        }
        /// <summary>
        /// Close
        /// </summary>
        /// <param name="db"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void close(this SQLiteDatabase db)
        {
            db.Close();
        }
    }
}