﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DurableDatabase.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "DurableDatabase.cs" was last formatted by Protiguous on 2019/09/12 at 10:39 AM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Maths;
    using Parsing;

    public class DurableDatabase : ABetterClassDispose, IDatabase {

        private String ConnectionString { get; }

        private UInt16 Retries { get; }

        private ThreadLocal<SqlConnection> SqlConnections { get; }

        public CancellationTokenSource CancelConnection { get; } = new CancellationTokenSource();

        /// <summary>
        ///     A database connection attempts to stay connected in the event of an unwanted disconnect.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="retries">         </param>
        /// <exception cref="InvalidOperationException"></exception>
        public DurableDatabase( String connectionString, UInt16 retries ) {
            this.Retries = retries;
            this.ConnectionString = connectionString;

            this.SqlConnections = new ThreadLocal<SqlConnection>( () => {

                var connection = new SqlConnection( this.ConnectionString );
                connection.StateChange += this.SqlConnection_StateChange;

                return connection;
            }, true );

            var test = this.OpenConnection(); //try/start the current thread's open;

            if ( null == test ) {
                var builder = new SqlConnectionStringBuilder( this.ConnectionString );

                throw new InvalidOperationException( $"Unable to connect to {builder.DataSource}" );
            }
        }

        [CanBeNull]
        private SqlConnection OpenConnection() {
            if ( this.SqlConnections.Value.State == ConnectionState.Open ) {
                return this.SqlConnections.Value;
            }

            try {
                this.SqlConnections.Value.Open();

                return this.SqlConnections.Value;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>
        ///     Return true if connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private Boolean ReOpenConnection( Object sender ) {
            if ( this.CancelConnection.IsCancellationRequested ) {
                return false;
            }

            if ( !( sender is SqlConnection connection ) ) {
                return false;
            }

            var retries = this.Retries;

            do {
                retries--;

                try {
                    if ( this.CancelConnection.IsCancellationRequested ) {
                        return false;
                    }

                    connection.Open();

                    if ( connection.State == ConnectionState.Open ) {
                        return true;
                    }
                }
                catch ( SqlException exception ) {
                    exception.Log();
                }
                catch ( DbException exception ) {
                    exception.Log();
                }
            } while ( retries > 0 );

            return false;
        }

        private void SqlConnection_StateChange( Object sender, [NotNull] StateChangeEventArgs e ) {
            switch ( e.CurrentState ) {
                case ConnectionState.Closed:
                    this.ReOpenConnection( sender );

                    break;

                case ConnectionState.Open: break; //do nothing

                case ConnectionState.Connecting:
                    Thread.SpinWait( 99 ); //TODO pooa.

                    break;

                case ConnectionState.Executing: break; //do nothing

                case ConnectionState.Fetching: break; //do nothing

                case ConnectionState.Broken:
                    this.ReOpenConnection( sender );

                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override void DisposeManaged() {
            if ( !this.CancelConnection.IsCancellationRequested ) {
                this.CancelConnection.Cancel();
            }

            foreach ( var connection in this.SqlConnections.Values ) {
                switch ( connection.State ) {
                    case ConnectionState.Open:
                        connection.Close();

                        break;

                    case ConnectionState.Closed: break;

                    case ConnectionState.Connecting:
                        connection.Close();

                        break;

                    case ConnectionState.Executing:
                        connection.Close();

                        break;

                    case ConnectionState.Fetching:
                        connection.Close();

                        break;

                    case ConnectionState.Broken:
                        connection.Close();

                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///     Opens and then closes a <see cref="SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        public Int32? ExecuteNonQuery( String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = CommandType.Text
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    return command.ExecuteNonQuery();
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "StoredProcedure" )]
        public Int32? ExecuteNonQuery( String query, Int32 retries, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            TryAgain:

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = CommandType.StoredProcedure
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    return command.ExecuteNonQuery();
                }
            }
            catch ( InvalidOperationException ) {

                //timeout probably
                retries--;

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Boolean ExecuteNonQuery( String query ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var sqlcommand = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = CommandType.Text
                } ) {
                    sqlcommand.ExecuteNonQuery();

                    return true;
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return false;
        }

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    return await command.ExecuteNonQueryAsync().ConfigureAwait( false );
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="table">      </param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public Boolean ExecuteReader( String query, CommandType commandType, [NotNull] out DataTable table, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            table = new DataTable();

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    table.BeginLoadData();

                    using ( var reader = command.ExecuteReader() ) {
                        table.Load( reader );
                    }

                    table.EndLoadData();

                    return true;
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return false;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [NotNull]
        public DataTable ExecuteReader( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    table.BeginLoadData();

                    using ( var reader = command.ExecuteReader() ) {
                        table.Load( reader );
                    }

                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return table;
        }

        /// <summary>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemCanBeNull]
        public async Task<DataTableReader> ExecuteReaderAsyncDataReader( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                DataTable table;

                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    using var reader = await command.ExecuteReaderAsync().ConfigureAwait( false );
                    table = reader.ToDataTable();
                }

                return table.CreateDataReader();
            }
            catch ( SqlException exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemNotNull]
        public async Task<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            var table = new DataTable();

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    table.BeginLoadData();

                    using ( var reader = await command.ExecuteReaderAsync( this.CancelConnection.Token ).ConfigureAwait( false ) ) {
                        table.Load( reader );
                    }

                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
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
        public (Status status, TResult result) ExecuteScalar<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    var scalar = command.ExecuteScalar();

                    if ( null == scalar || scalar == DBNull.Value || Convert.IsDBNull( scalar ) ) {
                        return (Status.Success, default);
                    }

                    if ( scalar is TResult result1 ) {
                        return (Status.Success, result1);
                    }

                    if ( scalar.TryCast<TResult>( out var result ) ) {
                        return (Status.Success, result);
                    }

                    return (Status.Success, ( TResult )Convert.ChangeType( scalar, typeof( TResult ) ));
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public async Task<(Status status, TResult result)> ExecuteScalarAsync<TResult>( String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = commandType,
                    CommandTimeout = 0
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    TryAgain:
                    Object scalar;

                    try {
                        scalar = await command.ExecuteScalarAsync().ConfigureAwait( false );
                    }
                    catch ( SqlException exception ) {
                        if ( exception.Number == DatabaseErrors.Deadlock ) {
                            goto TryAgain;
                        }

                        throw;
                    }

                    if ( null == scalar || scalar == DBNull.Value || Convert.IsDBNull( scalar ) ) {
                        return (Status.Success, default);
                    }

                    if ( scalar is TResult scalarAsync ) {
                        return (Status.Success, scalarAsync);
                    }

                    if ( scalar.TryCast<TResult>( out var result ) ) {
                        return (Status.Success, result);
                    }

                    return (Status.Success, ( TResult )Convert.ChangeType( scalar, typeof( TResult ) ));
                }
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.Log();
            }
            catch ( SqlException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     Returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="query">     </param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        [ItemCanBeNull]
        public IEnumerable<TResult> QueryList<TResult>( String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( nameof( query ) );
            }

            try {
                using ( var command = new SqlCommand( query, this.OpenConnection() ) {
                    CommandType = CommandType.StoredProcedure
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( parameters );
                    }

                    using ( var reader = command.ExecuteReader() ) {
                        var data = GenericPopulatorExtensions.CreateList<TResult>( reader );

                        return data;
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }
    }
}