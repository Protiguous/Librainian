﻿// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "DatabaseServer.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "DatabaseServer.cs" was last formatted by Protiguous on 2020/03/16 at 3:04 PM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections.Extensions;
    using Collections.Sets;
    using Converters;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using OperatingSystem.FileSystem.Pri.LongPath;
    using Parsing;
    using Utilities;

    public class DatabaseServer : ABetterClassDispose, IDatabase {

        public TimeSpan CommandTimeout { get; set; } = Minutes.Ten;

        [CanBeNull]
        public String Sproc { get; set; }

        public Int32? ExecuteNonQuery( [NotNull] String query, Int32 retries, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            this.Sproc = query;

            TryAgain:
            --retries;

            try {
                using var connection = new SqlConnection( connectionString: this.ConnectionString );

                using var command = new SqlCommand {
                    Connection = connection,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                    CommandType = commandType,
                    CommandText = query
                };

                command.PopulateParameters( parameters: parameters );

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch ( InvalidOperationException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }

            return default;
        }

        /// <summary>Opens and then closes a <see cref="SqlConnection" />.</summary>
        /// <returns></returns>
        public Int32? ExecuteNonQuery( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            this.Sproc = query;

            try {
                using var connection = new SqlConnection( connectionString: this.ConnectionString );

                using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }

            return default;
        }

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            this.Sproc = query ?? throw new ArgumentNullException( paramName: nameof( query ) );

            try {
                await using var connection = new SqlConnection( connectionString: this.ConnectionString );

                await using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                using var open = connection.OpenAsync( cancellationToken: this.Token );

                await open.ConfigureAwait( continueOnCapturedContext: false );

                return await command.ExecuteNonQueryAsync( cancellationToken: this.Token ).ConfigureAwait( continueOnCapturedContext: false );
            }
            catch ( SqlException exception ) {
                if ( !exception.SQLTimeout() ) {
                    exception.Log( more: Rebuild( query: query, parameters: parameters ) );
                }
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="table">      </param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public Boolean ExecuteReader( String query, CommandType commandType, [NotNull] out DataTable table, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            this.Sproc = query;

            table = new DataTable();

            try {

                using var connection = new SqlConnection( connectionString: this.ConnectionString );

                using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                connection.Open();

                using var bob = command.ExecuteReader();

                if ( bob != null ) {
                    table.BeginLoadData();
                    table.Load( reader: bob );
                    table.EndLoadData();
                }

                return true;
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }

            return default;
        }

        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        [ItemCanBeNull]
        public async Task<DataTableReader> ExecuteReaderAsyncDataReader( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            this.Sproc = query;

            try {

                await using var connection = new SqlConnection( connectionString: this.ConnectionString );

                await using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                using var open = connection.OpenAsync( cancellationToken: this.Token );
                await open.ConfigureAwait( continueOnCapturedContext: false );
                using var reader = command.ExecuteReaderAsync( cancellationToken: this.Token );

                if ( reader != null ) {
                    await using var readerAsync = await reader.ConfigureAwait( continueOnCapturedContext: false );
                    using var table = readerAsync.ToDataTable();

                    return table.CreateDataReader();
                }
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemNotNull]
        public async Task<DataTable> ExecuteReaderDataTableAsync( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            this.Sproc = query;

            var table = new DataTable();

            try {
                await using var connection = new SqlConnection( connectionString: this.ConnectionString );

                await using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                using var open = connection.OpenAsync( cancellationToken: this.Token );
                await open.ConfigureAwait( continueOnCapturedContext: false );

                using var reader = command.ExecuteReaderAsync( cancellationToken: this.Token );

                if ( reader != null ) {
                    table.BeginLoadData();
                    table.Load( reader: await reader.ConfigureAwait( continueOnCapturedContext: false ) );
                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
                table.Clear();
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
                table.Clear();
            }

            return table;
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [CanBeNull]
        public T ExecuteScalar<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            this.Sproc = query;

            try {
                using var connection = new SqlConnection( connectionString: this.ConnectionString );

                using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                connection.Open();

                return command.ExecuteScalar().Cast<T>();
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                throw;
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                throw;
            }
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemCanBeNull]
        public async Task<T> ExecuteScalarAsync<T>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            this.Sproc = query;

            try {
                await using var connection = new SqlConnection( connectionString: this.ConnectionString );

                await using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                using var open = connection.OpenAsync( cancellationToken: this.Token );
                await open.ConfigureAwait( continueOnCapturedContext: false );

                using var run = command.ExecuteScalarAsync( cancellationToken: this.Token );
                var scalar = await run.ConfigureAwait( continueOnCapturedContext: false );

                return scalar.Cast<T>();
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                throw;
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                throw;
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );

                throw;
            }
        }

        /// <summary>
        ///     <para>Run a query, no rows expected to be read.</para>
        ///     <para>Does not catch any exceptions.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public async Task NonQueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            this.Sproc = $"Executing SQL command {sproc}.";

            await using var connection = new SqlConnection( connectionString: this.ConnectionString );

            await using var command = new SqlCommand {
                Connection = connection,
                CommandType = commandType,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandText = sproc
            };

            command.PopulateParameters( parameters: parameters );

            using var open = connection.OpenAsync( cancellationToken: this.Token );
            await open.ConfigureAwait( continueOnCapturedContext: false );
            await command.ExecuteNonQueryAsync( cancellationToken: this.Token ).ConfigureAwait( continueOnCapturedContext: false );
        }

        [NotNull]
        public DataTableReader QueryAdHoc( [NotNull] String sql, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sql ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sql ) );
            }

            this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

            using var connection = new SqlConnection( connectionString: this.ConnectionString );

            using var command = new SqlCommand {
                Connection = connection,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandType = CommandType.Text,
                CommandText = sql
            };

            command.PopulateParameters( parameters: parameters );

            connection.Open();

            using var reader = command.ExecuteReader();
            using var table = reader.ToDataTable();

            return table.CreateDataReader();
        }

        [NotNull]
        [ItemNotNull]
        public async Task<DataTableReader> QueryAdHocAsync( [NotNull] String sql, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sql ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sql ) );
            }

            this.Sproc = $"Executing AdHoc SQL: {sql.DoubleQuote()}.";

            await using var connection = new SqlConnection( connectionString: this.ConnectionString );

            await using var command = new SqlCommand {
                Connection = connection,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandType = CommandType.Text,
                CommandText = sql
            };

            command.PopulateParameters( parameters: parameters );

            using var open = connection.OpenAsync( cancellationToken: this.Token );
            await open.ConfigureAwait( continueOnCapturedContext: false );

            var execute = command.ExecuteReaderAsync( cancellationToken: this.Token );

            if ( execute != null ) {
                await using var reader = await execute.ConfigureAwait( continueOnCapturedContext: false );
                using var table = reader.ToDataTable();

                return table.CreateDataReader();
            }

            using var blank = new DataTable();
            await using var another = new DataTableReader( dataTable: blank );

            return another;
        }

        /// <summary>Simplest possible database connection.
        /// <para>Connect and then run <paramref name="sproc" />.</para>
        /// </summary>
        /// <param name="sproc"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        [ItemCanBeNull]
        public async Task<SqlDataReader> QueryAsync( [NotNull] String sproc, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: sproc ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( sproc ) );
            }

            this.Sproc = sproc;

            await using var connection = new SqlConnection( connectionString: this.ConnectionString );

            await using var command = new SqlCommand {
                Connection = connection,
                CommandType = commandType,
                CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds,
                CommandText = sproc
            };

            command.PopulateParameters( parameters: parameters );

            using var open = connection.OpenAsync( cancellationToken: this.Token );
            await open.ConfigureAwait( continueOnCapturedContext: false );

            using var reader = command.ExecuteReaderAsync( cancellationToken: this.Token );

            if ( reader != null ) {
                return await reader.ConfigureAwait( continueOnCapturedContext: false );
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">     </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        [ItemCanBeNull]
        public IEnumerable<TResult> QueryList<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            this.Sproc = query;

            try {
                using var connection = new SqlConnection( connectionString: this.ConnectionString );

                using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                connection.Open();
                var reader = command.ExecuteReader();

                if ( reader != null ) {
                    return GenericPopulatorExtensions.CreateList<TResult>( reader: reader );
                }
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }

            return default;
        }

        public void UseDatabase( [NotNull] String dbName ) {
            if ( String.IsNullOrWhiteSpace( value: dbName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( dbName ) );
            }

            using ( var _ = this.QueryAdHoc( sql: $"USE {dbName.Bracket()};" ) ) { }
        }

        public async Task UseDatabaseAsync( [NotNull] String dbName ) {
            if ( String.IsNullOrWhiteSpace( value: dbName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( dbName ) );
            }

            await using ( var _ = await this.QueryAdHocAsync( sql: $"USE {dbName.Bracket()};" ).ConfigureAwait( continueOnCapturedContext: false ) ) { }
        }

        [NotNull]
        private String ConnectionString { get; }

        private CancellationToken Token { get; }

        /// <summary>The parameter collection for this database connection.</summary>
        [NotNull]
        internal ConcurrentHashset<SqlParameter> ParameterSet { get; } = new ConcurrentHashset<SqlParameter>();

        /// <summary>
        ///     <para>Create a database object to the specified server.</para>
        /// </summary>
        public DatabaseServer( [CanBeNull] SqlConnectionStringBuilder builder, [CanBeNull] String useDatabase = null, CancellationToken? token = default ) : this(
            connectionString: builder?.ConnectionString, useDatabase: useDatabase, token: token ) { }

        /// <summary></summary>
        /// <param name="connectionString"></param>
        /// <param name="useDatabase"></param>
        /// <param name="token"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public DatabaseServer( String connectionString, [CanBeNull] String useDatabase = null, CancellationToken? token = default ) {
            this.Token = token ?? CancellationToken.None;
            this.ConnectionString = connectionString ?? throw new ArgumentNullException( paramName: nameof( connectionString ) );

            useDatabase = useDatabase.Trimmed();

            if ( !String.IsNullOrWhiteSpace( value: useDatabase ) ) {

                var builder = new SqlConnectionStringBuilder( connectionString: connectionString ) {
                    InitialCatalog = useDatabase.Bracket()
                };

                this.ConnectionString = builder.ConnectionString;
            }
        }

        [CanBeNull]
        public DataTableReader ExecuteDataReader( [NotNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            this.Sproc = query;

            try {

                using var connection = new SqlConnection( connectionString: this.ConnectionString );

                using var command = new SqlCommand( cmdText: query, connection: connection ) {
                    CommandType = commandType,
                    CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds
                };

                command.PopulateParameters( parameters: parameters );

                connection.Open();

                using var reader = command.ExecuteReader( behavior: CommandBehavior.CloseConnection );

                if ( reader != null ) {
                    using var table = reader.ToDataTable();

                    return table.CreateDataReader();
                }
            }
            catch ( SqlException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }
            catch ( DbException exception ) {
                exception.Log( more: Rebuild( query: query, parameters: parameters ) );
            }

            return default;
        }

#if VERBOSE
        ~DatabaseServer() {
            $"Warning: We have an undisposed Database() connection somewhere. This could cause a memory leak. Query={this.Sproc.DoubleQuote()}".Log();
        }

#endif

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() { }

        [DebuggerStepThrough]
        [NotNull]
        private static String Rebuild( [NotNull] String query, [CanBeNull] IEnumerable<SqlParameter> parameters = null ) {
            if ( String.IsNullOrWhiteSpace( value: query ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( query ) );
            }

            if ( parameters is null ) {
                return $"exec {query}";
            }

            return
                $"exec {query} {parameters.Where( predicate: parameter => !( parameter is null ) ).Select( selector: parameter => $"{parameter.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings( separator: "," )}; ";
        }

        public static async Task<Boolean> CreateDatabase( [NotNull] String databaseName, [NotNull] String connectionString ) {
            databaseName = databaseName.Trimmed();

            if ( String.IsNullOrWhiteSpace( value: databaseName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( databaseName ) );
            }

            if ( String.IsNullOrWhiteSpace( value: connectionString ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( connectionString ) );
            }

            try {
                using var db = new DatabaseServer( connectionString: connectionString, useDatabase: "master" );

                await db.QueryAdHocAsync( sql: $"create database {databaseName.Bracket()};" ).ConfigureAwait( continueOnCapturedContext: false );

                return true;
            }
            catch ( SqlException exception ) {
                if ( !exception.SQLTimeout() ) {
                    exception.Log();
                }
            }

            return default;
        }

        [NotNull]
        public static SqlConnectionStringBuilder PopulateConnectionStringBuilder( [NotNull] String serverName, [NotNull] String instanceName, TimeSpan connectTimeout,
            [CanBeNull] Credentials credentials = default ) {
            if ( String.IsNullOrWhiteSpace( value: serverName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( serverName ) );
            }

            if ( String.IsNullOrWhiteSpace( value: instanceName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( instanceName ) );
            }

            var builder = new SqlConnectionStringBuilder {
                DataSource = $@"{serverName}\{instanceName}",
                ApplicationIntent = ApplicationIntent.ReadWrite,
                ApplicationName = Assembly.GetEntryAssembly()?.Location.GetFileNameWithoutExtension(),
                ConnectRetryCount = 3,
                ConnectTimeout = ( Int32 )connectTimeout.TotalSeconds,
                ConnectRetryInterval = 1,
                PacketSize = 8060,
                Pooling = true
            };

            //security
            if ( String.IsNullOrWhiteSpace( value: credentials?.Username ) ) {
                builder.Remove( keyword: nameof( builder.UserID ) );
                builder.Remove( keyword: nameof( builder.Password ) );
                builder.IntegratedSecurity = true;
            }
            else {
                builder.Remove( keyword: "Integrated Security" );
                builder.Remove( keyword: "Authentication" );
                builder.IntegratedSecurity = false;
                builder.UserID = credentials.UserID;
                builder.Password = credentials.Password;
            }

            return builder;
        }

        /*
        [NotNull]
        [DebuggerStepThrough]
        public static async Task StartAnySQLBrowsers( TimeSpan timeout ) {
            await Task.Run( () => {

                "Searching for any database servers...".Log();

                var machines = new ConcurrentHashset<String> {
                    "Thor", Dns.GetHostName(), "127.0.0.1" //my development servers' names. Feel free to add your server.
                };

                var stopwatch = Stopwatch.StartNew();

                var services = machines.Where( s => !String.IsNullOrWhiteSpace( s ) ).Select( machineName => {
                    var service = new ServiceController {
                        ServiceName = "SQLBrowser", MachineName = machineName
                    };

                    if ( service.Status != ServiceControllerStatus.Running ) {
                        service.Start();
                    }

                    return service;
                } ).ToList();

                while ( stopwatch.Elapsed < timeout ) {
                    var notRunningYet = services.Where( controller =>
                        controller?.Status.In( ServiceControllerStatus.StartPending, ServiceControllerStatus.Running ) == false );

                    Parallel.ForEach( notRunningYet, controller => controller?.WaitForStatus( ServiceControllerStatus.Running, timeout ) );

                    if ( services.Any( controller => controller?.Status.In( ServiceControllerStatus.Running ) == true ) ) {
                        return;
                    }
                }
            } ).ConfigureAwait( false );
        }
        */
    }
}