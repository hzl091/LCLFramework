﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Data;
using System.Diagnostics;
using LCL.LData;

namespace LCL.Domain.Services
{
    /// <summary>
    /// 一个简单的日志记录类。
    /// 
    /// 目前只有处理异常的方法。
    /// </summary>
    public static class Logger
    {
        private static object _lock = new object();
        private static LoggerBase _impl = new FileLogger();

        /// <summary>
        /// 使用具体的日志记录器来接管本 API。
        /// </summary>
        /// <param name="loggerImpl"></param>
        public static void SetImplementation(LoggerBase loggerImpl)
        {
            _impl = loggerImpl;
        }
        public static void LogInfo(string message)
        {
            lock (_lock)
            {
                try
                {
                    Debug.WriteLine(message);
                    _impl.LogInfo(message);
                }
                catch { }
            }
        }
        public static void LogWarn(string message)
        {
            lock (_lock)
            {
                try
                {
                    Debug.WriteLine(message);
                    _impl.LogWarn(message);
                }
                catch { }
            }
        }
        public static void LogDebug(string message)
        {
            lock (_lock)
            {
                try
                {
                    Debug.WriteLine(message);
                    _impl.LogDebug(message);
                }
                catch { }
            }
        }
        /// <summary>
        /// 记录某个已经生成的异常到文件中。
        /// </summary>
        /// <param name="title"></param>
        /// <param name="e"></param>
        public static void LogError(string title, Exception e)
        {
            lock (_lock)
            {
                try
                {
                    Debug.WriteLine(e.Message);
                    _impl.LogError(title, e);
                }
                catch { }
            }
        }

        [ThreadStatic]
        private static long _threadDbAccessedCount = 0;
        private static long _dbAccessedCount = 0;

        /// <summary>
        /// 返回系统运行到现在，一共记录了多少次 Sql 语句。
        /// </summary>
        public static long DbAccessedCount
        {
            get { return _dbAccessedCount; }
        }

        /// <summary>
        /// 返回当前线程运行到现在，一共记录了多少次 Sql 语句。
        /// </summary>
        public static long ThreadDbAccessedCount
        {
            get { return _threadDbAccessedCount; }
        }

        /// <summary>
        /// 是否启用 Sql 查询监听。 默认为 false。
        /// 打开后，DbAccessed、ThreadDbAccessed 两个事件才会发生。
        /// </summary>
        public static bool EnableSqlObervation { get; set; }

        /// <summary>
        /// 发生了数据访问时的事件。
        /// </summary>
        public static event EventHandler<DbAccessedEventArgs> DbAccessed;

        [ThreadStatic]
        private static EventHandler<DbAccessedEventArgs> _threadDbAccessedHandler;
        /// <summary>
        /// 当前线程，发生了数据访问时的事件。
        /// </summary>
        public static event EventHandler<DbAccessedEventArgs> ThreadDbAccessed
        {
            add
            {
                _threadDbAccessedHandler = (EventHandler<DbAccessedEventArgs>)Delegate.Combine(_threadDbAccessedHandler, value);
            }
            remove
            {
                _threadDbAccessedHandler = (EventHandler<DbAccessedEventArgs>)Delegate.Remove(_threadDbAccessedHandler, value);
            }
        }

        /// <summary>
        /// 记录 Sql 执行过程。
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="connectionSchema">The connection schema.</param>
        public static void LogDbAccessed(string sql, IDbDataParameter[] parameters, DbConnectionSchema connectionSchema)
        {
            _dbAccessedCount++;
            _threadDbAccessedCount++;

            lock (_lock)
            {
                try
                {
                    _impl.LogDbAccessed(sql, parameters, connectionSchema);
                }
                catch { }
            }

            if (EnableSqlObervation)
            {
                var handler1 = _threadDbAccessedHandler;
                var handler2 = DbAccessed;
                if (handler1 != null || handler2 != null)
                {
                    var args = new DbAccessedEventArgs(sql, parameters, connectionSchema);

                    if (handler1 != null)
                    {
                        handler1(null, new DbAccessedEventArgs(sql, parameters, connectionSchema));
                    }
                    if (handler2 != null)
                    {
                        handler2(null, new DbAccessedEventArgs(sql, parameters, connectionSchema));
                    }
                }
            }
        }

        /// <summary>
        /// 数据访问事件参数。
        /// </summary>
        public class DbAccessedEventArgs : EventArgs
        {
            public DbAccessedEventArgs(string sql, IDbDataParameter[] parameters, DbConnectionSchema connectionSchema)
            {
                this.Sql = sql;
                this.Parameters = parameters;
                this.ConnectionSchema = connectionSchema;
            }

            /// <summary>
            /// 执行的 Sql
            /// </summary>
            public string Sql { get; private set; }

            /// <summary>
            /// 所有的参数值。
            /// </summary>
            public IDbDataParameter[] Parameters { get; private set; }

            /// <summary>
            /// 对应的数据库连接
            /// </summary>
            public DbConnectionSchema ConnectionSchema { get; private set; }
        }
    }
}