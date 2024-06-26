﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "DatabaseServer.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.


namespace Librainian.Databases;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Configuration;
using Converters;
using Exceptions;
using Logging;
using Maths;
using Measurement.Time;
using Measurement.Time.Clocks;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Parsing;
using PooledAwait;
using Utilities;
using Utilities.Disposables;

public class DatabaseServer : ABetterClassDisposeAsync, IDatabaseServer {

	private const Int32 DefaultRetriesLeft = 10;

	/// <summary>The number of sql connections open across ALL threads.</summary>
	private static Int64 _connectionCounter;

	private Int32 _retriesLeft = DefaultRetriesLeft;

	private Int32 retriesLeft = DefaultRetriesLeft;

	~DatabaseServer() {
		new InvalidOperationException(
			$"Warning: We have an undisposed {nameof( DatabaseServer )} connection somewhere. This could cause a memory leak. Query={this.Query.DoubleQuote()}" ).Log(
			BreakOrDontBreak.Break );
		this.DisposeAsync().AsTask().Wait( this.DefaultMaximumTimeout() );
	}

	///// <summary>Allow this many (1024 by default) concurrent async database Commands.</summary>
	//private static SemaphoreSlim DatabaseCommandSemaphores { get; } = new(1024, 1024);
	/// <summary>Allow this many (1024 by default) concurrent async operations.</summary>
	private static SemaphoreSlim DatabaseConnectionSemaphores { get; } = new( 1024, 1024 );

	private static SqlRetryLogicBaseProvider RetryProvider { get; }

	private SqlConnection? Connection { get; set; }

	//private SqlCommand Command { get; }
	private String? ConnectionString { get; init; }

	private Boolean EnteredConnectionSemaphore { get; set; }

	//private Boolean EnteredCommandSemaphore { get; }
	private Stopwatch TimeSinceLastConnectAttempt { get; } = Stopwatch.StartNew();

	private static void ConnectionOnInfoMessage( Object sender, SqlInfoMessageEventArgs e ) =>
		$"{nameof( SqlInfoMessageEventArgs )} {nameof( e.Message )}={e.Message}".Verbose();

	[DebuggerStepThrough]
	private static String Rebuild( String query, IEnumerable<SqlParameter>? parameters = null ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new ArgumentEmptyException( nameof( query ) );
		}

		return parameters is null ? $"exec {query}" :
			$"exec {query} {parameters.Select( parameter => $"{parameter.ParameterName}={parameter.Value?.ToString().SingleQuote() ?? String.Empty}" ).ToStrings( ", " )}; ";
	}

	private void BreakOnSlowQueries() {
		var stopWatch = this.StopWatch;
		if ( stopWatch is null ) {
			return;
		}

		stopWatch.Stop();

		if ( Debugger.IsAttached && this.SlowQueriesTakeLongerThan is not null && ( stopWatch.Elapsed >= this.SlowQueriesTakeLongerThan ) ) {
			$"Slow query {this.Query.DoubleQuote()} took {stopWatch.Elapsed.Simpler()}.".DebugWriteLine();
		}
	}

	private async Task CloseConnection() {
		try {
			await using ( this.Connection ) {
				try {
					var task = this.Connection?.CloseAsync();
					if ( task is not null ) {
						await task.ConfigureAwait( false );
					}
				}
				catch ( SqlException exception ) {
					exception.Log();
				}
				catch ( DbException exception ) {
					exception.Log();
				}
			}

			if ( this.EnteredConnectionSemaphore ) {
				DatabaseConnectionSemaphores.Release();
			}

			//if ( this.EnteredCommandSemaphore ) {
			//	DatabaseCommandSemaphores.Release();
			//}
		}
		catch ( InvalidOperationException exception ) {
			exception.Log( BreakOrDontBreak.Break );
		}
		catch ( SqlException exception ) {
			exception.Log( BreakOrDontBreak.Break );
		}
		finally {
			Interlocked.Decrement( ref _connectionCounter );
			this.Connection = default( SqlConnection? );
		}
	}

	/// <summary>Return true if any retries are available.</summary>
	private Boolean DecrementRetries() => ( --this._retriesLeft ).Any();

	private void OnRetrying( Object? sender, SqlRetryingEventArgs e ) {
		$"Retrying SQL {this.Query.DoubleQuote()}. Retry count={e.RetryCount}.".Verbose();

		e.Cancel = this.IsDisposed;
	}

	/// <summary>Create a sql server database connection via async.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NullException"></exception>
	[NeedsTesting]
	private async PooledValueTask<Status> OpenConnectionAsync( CancellationToken cancellationToken ) {
		Debug.Assert( !String.IsNullOrWhiteSpace( this.ConnectionString ) );
		if ( String.IsNullOrWhiteSpace( this.ConnectionString ) ) {
			throw new NullException( nameof( this.ConnectionString ) );
		}

		try {
			if ( !this.retriesLeft.Any() ) {
				return Status.Stop;
			}

			$"{Interlocked.Read( ref _connectionCounter ):N0} active database connections.".Verbose();

			this.EnteredConnectionSemaphore = await DatabaseConnectionSemaphores.WaitAsync( this.DefaultConnectionTimeout, cancellationToken ).ConfigureAwait( false );
			if ( !this.EnteredConnectionSemaphore ) {
				return Status.Timeout;
			}

			if ( this.SlowQueriesTakeLongerThan is not null ) {
				this.StopWatch = Stopwatch.StartNew();
			}

			CreateConnection();

			AttachRetryLogic();

			await IntroduceArtificialDelay().ConfigureAwait( false );

			if ( cancellationToken.IsCancellationRequested ) {
				return Status.Cancel;
			}

			//FluentTimer? timer = null;

			//if ( progress is not null ) {
			//var stopwatch = Stopwatch.StartNew();
			//var sqlConnection = this.Connection ?? throw new NullException( nameof( this.Connection ) );
			//timer = FluentTimer.Create( Fps.Thirty, () => progress.Report( (stopwatch.Elapsed, sqlConnection.State) ) ).AutoReset( true );
			//}

			var connection = this.Connection;
			if ( connection == null ) {
				throw new NullException( nameof( this.Connection ) );
			}

			try {
				this.TimeSinceLastConnectAttempt.Restart();

				await connection.OpenAsync( cancellationToken )!.ConfigureAwait( false );

				Interlocked.Increment( ref _connectionCounter );

				return Status.Continue;
			}
			catch ( InvalidOperationException exception ) {
				return await InCaseOfException( exception ).ConfigureAwait( false );
			}
			catch ( SqlException exception ) {
				return await InCaseOfException( exception ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				return await InCaseOfException( exception ).ConfigureAwait( false );
			}
		}
		catch ( InvalidOperationException exception ) {
			return await InCaseOfException( exception ).ConfigureAwait( false );
		}
		catch ( SqlException exception ) {
			return await InCaseOfException( exception ).ConfigureAwait( false );
		}
		catch ( DbException exception ) {
			return await InCaseOfException( exception ).ConfigureAwait( false );
		}
		catch ( TaskCanceledException cancelled ) {
			$"Open database connection was {nameof( cancelled ).DoubleQuote()}.".Verbose();
		}

		return Status.Unknown;

		void CreateConnection() {
			if ( this.Connection is null ) {
				this.Connection = new SqlConnection( this.ConnectionString );
				this.Connection.InfoMessage += ConnectionOnInfoMessage;
			}
		}

		void AttachRetryLogic() {
			var sqlConnection = this.Connection;
			if ( sqlConnection is { RetryLogicProvider: null } ) {
				sqlConnection.RetryLogicProvider = RetryProvider;
				sqlConnection.RetryLogicProvider.Retrying = this.OnRetrying;
			}
		}

		async Task IntroduceArtificialDelay() {
			if ( ArtificialDatabaseDelay is not null ) {
				await Task.Delay( ArtificialDatabaseDelay.ToSeconds(), cancellationToken ).ConfigureAwait( false );
			}
		}

		async PooledValueTask<Status> InCaseOfException<T>( T exception ) where T : Exception {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );

				var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );

				if ( status.IsGood() ) {
					return status;
				}
			}

			throw exception.Log();
		}
	}

	internal static DataTable CreateDataTable<T>( String columnName, IEnumerable<T> rows ) {
		if ( String.IsNullOrWhiteSpace( columnName ) ) {
			throw new ArgumentEmptyException( nameof( columnName ) );
		}

		DataTable table = new();
		table.Columns.Add( columnName, typeof( T ) );
		foreach ( var row in rows ) {
			table.Rows.Add( row );
		}

		return table;
	}

	internal static IEnumerable<SqlDataRecord> CreateSqlDataRecords<T>( String columnName, SqlDbType sqlDbType, IEnumerable<T> rows ) {
		var metaData = new SqlMetaData[1];
		metaData[0] = new SqlMetaData( columnName, sqlDbType );
		SqlDataRecord record = new( metaData );
		foreach ( var row in rows ) {
			if ( row is not null ) {
				record.SetValue( 0, row );
			}

			yield return record;
		}
	}

	static DatabaseServer() {
		var sqlRetryLogicOption = new SqlRetryLogicOption {
			NumberOfTries = DefaultRetriesLeft,
			DeltaTime = Minutes.One,
			MaxTimeInterval = Minutes.One
		};

		RetryProvider = SqlConfigurableRetryFactory.CreateIncrementalRetryProvider( sqlRetryLogicOption ) ??
						throw new InvalidOperationException( $"{nameof( SqlConfigurableRetryFactory )} was null or invalid!" );
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="validatedConnectionString"></param>
	/// <param name="applicationSetting"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="NullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public DatabaseServer( IValidatedConnectionString validatedConnectionString, IApplicationSetting applicationSetting ) : base( applicationSetting.Info() ) {
		if ( String.IsNullOrWhiteSpace( validatedConnectionString.Value ) ) {
			throw new ArgumentNullException( nameof( validatedConnectionString ) );
		}

		this.DefaultConnectionTimeout = Minutes.One;
		this.DefaultExecuteTimeout = Minutes.One;

		if ( String.IsNullOrWhiteSpace( validatedConnectionString.Value ) ) {
			throw new NullException( nameof( this.ConnectionString ) );
		}

		this.ConnectionString = validatedConnectionString.Value;

		this.ApplicationSetting = applicationSetting ?? throw new ArgumentNullException( nameof( applicationSetting ) );

		//this.EnteredCommandSemaphore = DatabaseCommandSemaphores.Wait( this.DefaultMaximumTimeout() );
		//if ( this.EnteredCommandSemaphore ) {
		//	this.Command = new SqlCommand();
		//}
		//else {
		//	throw new InvalidOperationException( $"Timeout waiting to create {nameof(SqlCommand)} object." );
		//}

		//TODO Implement a linear fallback instead of a flat timespan for DefaultTimeBetweenRetries.
		AppContext.SetSwitch( "Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true );
	}

	/// <summary>A debugging aid. EACH database call will delay upon opening a connection.</summary>
	public static IQuantityOfTime? ArtificialDatabaseDelay { get; set; }

	public IApplicationSetting ApplicationSetting { get; }

	/// <summary>Set to 1 minute by default.</summary>
	public TimeSpan CommandTimeout { get; set; }

	/// <summary>Defaults to 1 minute.</summary>
	public TimeSpan DefaultConnectionTimeout { get; set; }

	/// <summary>Defaults to 1 minute.</summary>
	public TimeSpan DefaultExecuteTimeout { get; set; }

	public TimeSpan DefaultTimeBetweenRetries { get; set; } = Seconds.Three;

	public String? Query { get; set; }

	/// <summary>Defaults to 3 seconds.</summary>
	public TimeSpan? SlowQueriesTakeLongerThan { get; set; } = Seconds.Five;

	public Stopwatch? StopWatch { get; private set; }

	/// <summary>
	///     Try a best guess for the <see cref="SqlDbType" /> of <paramref name="type" />.
	///     <para>
	///         <remarks>Try <paramref name="type" /> ?? SqlDbType.Variant</remarks>
	///     </para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="type"></param>
	public static SqlDbType TranslateToSqlDbType<T>( T type ) =>
		type switch {
			String s => s.Length.Between( 1, 4000 ) ? SqlDbType.NVarChar : SqlDbType.NText,
			UInt64 => SqlDbType.BigInt,
			Int64 => SqlDbType.BigInt,
			Int32 => SqlDbType.Int,
			UInt32 => SqlDbType.Int,
			Byte[] bytes => bytes.Length.Between( 1, 8000 ) ? SqlDbType.VarBinary : SqlDbType.Image,
			Boolean => SqlDbType.Bit,
			Char => SqlDbType.NChar,
			DateTime => SqlDbType.DateTime2,
			Decimal => SqlDbType.Decimal,
			Single => SqlDbType.Float,
			Guid => SqlDbType.UniqueIdentifier,
			Byte => SqlDbType.TinyInt,
			Int16 => SqlDbType.SmallInt,
			XmlDocument => SqlDbType.Xml,
			Date => SqlDbType.Date,
			TimeSpan => SqlDbType.Time,
			TimeClock => SqlDbType.Time,
			DateTimeOffset => SqlDbType.DateTimeOffset,
			var _ => SqlDbType.Variant
		};

	/// <summary>
	///     Execute the stored procedure " <paramref name="query" />" with the optional <paramref name="parameters" />.
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public async FireAndForget BeginQuery( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
		await this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters );

	public async Task<Boolean> CreateDatabase( String databaseName, IValidatedConnectionString connectionString, CancellationToken cancellationToken ) {
		databaseName = databaseName.Trimmed() ?? throw new ArgumentEmptyException( nameof( databaseName ) );

		TryAgain:

		try {
			await using var database = new DatabaseServer( connectionString, this.ApplicationSetting );

			await using var adhoc = await database.QueryAdhocAsync( $"create database {databaseName.SmartBrackets()};", cancellationToken ).ConfigureAwait( false );

			return true;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log( BreakOrDontBreak.Break );
		}

		return false;
	}

	public TimeSpan DefaultMaximumTimeout() => this.DefaultConnectionTimeout + this.DefaultExecuteTimeout;

	public override async ValueTask DisposeManagedAsync() {
		this.BreakOnSlowQueries();
		await this.CloseConnection().ConfigureAwait( false );
	}

	/// <summary></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public async PooledValueTask<Int32?> ExecuteNonQueryAsync(
		String query,
		CommandType commandType,
		CancellationToken cancellationToken,
		params SqlParameter[]? parameters
	) {
		this.Query = query;
		$"ExecuteNonQueryAsync {this.Query.DoubleQuote()} starting..".Verbose();

		TryAgain:
		try {
			await using var command = await this.OpenConnectionAndReturnCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );
			return await command.ExecuteNonQueryAsync( cancellationToken )!.ConfigureAwait( false );
		}
		catch ( Exception exception ) {
			$"Query {this.Query.DoubleQuote()} timed out.".Verbose();
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}
		finally {
			$"ExecuteNonQueryAsync {this.Query.DoubleQuote()} done.".Verbose();
		}

		throw new InvalidOperationException( $"Database call {nameof( ExecuteNonQueryAsync )} failed." );
	}

	/// <summary>
	///     Execute the stored procedure " <paramref name="query" />" with the optional parameters
	///     <paramref name="parameters" />.
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public PooledValueTask<Int32?> ExecuteNonQueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[] parameters ) =>
		this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters );

	/// <summary></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	[NeedsTesting]
	public async Task<DataTableReader?> ExecuteReaderAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			await using var readerAsync = await command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			if ( readerAsync != null ) {
				using var table = readerAsync.ToDataTable();

				return table.CreateDataReader();
			}
		}
		catch ( SqlException exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( DataTableReader? );
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	public async Task<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:
		var table = new DataTable();

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			using var reader = command.ExecuteReaderAsync( cancellationToken );

			if ( reader != null ) {
				table.BeginLoadData();
				table.Load( await reader.ConfigureAwait( false ) );
				table.EndLoadData();
			}
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return table;
	}

	/// <summary>
	///     <para>Returns the first column of the first row.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	public async Task<T?> ExecuteScalarAsync<T>( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			var result = await command.ExecuteScalarAsync( cancellationToken )!.ConfigureAwait( false );

			return result is null ? default( T? ) : result.Cast<Object, T>();
		}
		catch ( InvalidCastException exception ) {
			//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
			exception.Log();

			throw;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( T? );
	}

	/// <summary>
	///     <para>Returns the first column of the first row.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	public async Task<T?> ExecuteScalarAsync<T>( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
		await this.ExecuteScalarAsync<T?>( query, CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

	/// <summary>
	///     Overwrites the <paramref name="table" /> contents with data from the <paramref name="query" />.
	///     <para>Note: Include the parameters after the query.</para>
	///     <para>Can throw exceptions on connecting or executing the query.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="table"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="progress"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	public async Task<Boolean> FillTableAsync(
		String query,
		CommandType commandType,
		DataTable table,
		CancellationToken cancellationToken,
		IProgress<(TimeSpan Elapsed, ConnectionState State)>? progress = null,
		params SqlParameter[]? parameters
	) {
		if ( table is null ) {
			throw new NullException( nameof( table ) );
		}

		this.Query = query;

		TryAgain:
		table.Clear();

		try {
			var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );
			if ( status.IsBad() ) {
				return false;
			}

			var connection = this.Connection;
			if ( connection is null ) {
				return false;
			}

			using var dataAdapter = new SqlDataAdapter( query, connection ) {
				AcceptChangesDuringFill = false,
				FillLoadOption = LoadOption.OverwriteChanges,
				MissingMappingAction = MissingMappingAction.Passthrough,
				MissingSchemaAction = MissingSchemaAction.Add,
				SelectCommand = {
					CommandTimeout = ( Int32 ) this.CommandTimeout.TotalSeconds,
					CommandType = commandType
				}
			};

			if ( parameters != null ) {
				dataAdapter.SelectCommand?.Parameters?.AddRange( parameters );
			}

			dataAdapter.Fill( table );
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return true;
	}

	public String? GetConnectionString() => this.ConnectionString;

	public async Task<IDictionary?> GetStats( CancellationToken cancellationToken ) {
		try {
			var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );
			if ( status.IsBad() ) {
				return default( IDictionary? );
			}

			return this.Connection?.RetrieveStatistics();
		}
		catch ( Exception exception ) {
			exception.Log();
		}
		finally {
			await this.DisposeManagedAsync().ConfigureAwait( false );
		}

		return default( IDictionary? );
	}

	[NeedsTesting]
	public async PooledValueTask<SqlCommand> OpenConnectionAndReturnCommand( CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( this.Query ) ) {
			throw new NullException( nameof( this.Query ) );
		}

		var status = await this.OpenConnectionAsync( cancellationToken ).ConfigureAwait( false );
		if ( status.IsBad() ) {
			throw new InvalidOperationException( "Error connecting to database server." );
		}

		if ( this.Connection is null ) {
			throw new NullException( nameof( this.Connection ) );
		}

		await using var command = this.Connection.CreateCommand();  //TODO Is this "using" correct?

		//$"Setting command value for query {this.Query.DoubleQuote()}..".Verbose();
		if ( command is null ) {
			throw new NullException( nameof( SqlCommand ) );
		}

		command.CommandType = commandType;
		command.CommandText = this.Query;
		command.CommandTimeout = ( Int32 )this.CommandTimeout.TotalSeconds;

		var commandParameters = command.Parameters;
		if ( ( commandParameters != null ) && ( parameters != null ) ) {
			foreach ( var parameter in parameters ) {
				commandParameters.Add( parameter );
			}

			Debug.Assert( parameters.Length == commandParameters.Count );
		}

		return command;
	}

	public async Task<DatabaseServer> QueryAdhocAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

			await command.ExecuteNonQueryAsync( cancellationToken )!.ConfigureAwait( false );
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return this;
	}

	public async Task<DataTableReader?> QueryAdhocReaderAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( CommandType.Text, cancellationToken, parameters ).ConfigureAwait( false );

			await using var reader = await command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			using var table = reader.ToDataTable();

			return table.CreateDataReader();
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( DataTableReader? );
	}

	/// <summary>
	///     Simplest possible database connection.
	///     <para>Connect and then run <paramref name="query" />.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="InvalidCastException"></exception>
	public async Task<SqlDataReader?> QueryAsync( String query, CommandType commandType, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			await using var result = await command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			return result;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( SqlDataReader? );
	}

	/// <summary>
	///     Simplest possible database connection.
	///     <para>Connect and then run <paramref name="query" />.</para>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<SqlDataReader?> QueryAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new NullException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

			return await command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );
		}
		catch ( InvalidCastException exception ) {
			//TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
			exception.Log();

			throw;
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( SqlDataReader? );
	}

	/// <summary>Returns a <see cref="DataTable" /></summary>
	/// <param name="query"></param>
	/// <param name="commandType"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public async Task<IEnumerable<TResult>?> QueryListAsync<TResult>(
		String query,
		CommandType commandType,
		CancellationToken cancellationToken,
		params SqlParameter[]? parameters
	) {
		if ( String.IsNullOrWhiteSpace( query ) ) {
			throw new ArgumentEmptyException( nameof( query ) );
		}

		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( commandType, cancellationToken, parameters ).ConfigureAwait( false );

			var reader = await command.ExecuteReaderAsync( cancellationToken )!.ConfigureAwait( false );

			if ( reader != null ) {
				return GenericPopulatorExtensions.CreateList<TResult>( reader );
			}
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( IEnumerable<TResult>? );
	}

	/// <summary>Execute the stored procedure <paramref name="query" /> with the optional <paramref name="parameters" />.</summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public PooledValueTask<Int32?> RunSprocAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) =>
		this.ExecuteNonQueryAsync( query, CommandType.StoredProcedure, cancellationToken, parameters );

	/// <summary>
	///     Execute the stored procedure " <paramref name="query" />" with the optional parameters
	///     <paramref name="parameters" />.
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="parameters"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="SqlException"></exception>
	/// <exception cref="DbException"></exception>
	public async Task<Int32?> RunStoredProcedureAsync( String query, CancellationToken cancellationToken, params SqlParameter[]? parameters ) {
		this.Query = query;

		TryAgain:

		try {
			await using var command = await this.OpenConnectionAndReturnCommand( CommandType.StoredProcedure, cancellationToken, parameters ).ConfigureAwait( false );

			return await command.ExecuteNonQueryAsync( cancellationToken )!.ConfigureAwait( false );
		}
		catch ( Exception exception ) {
			if ( exception.AttemptQueryAgain( ref this.retriesLeft ) ) {
				await this.CloseConnection().ConfigureAwait( false );
				await Task.Delay( this.DefaultTimeBetweenRetries, cancellationToken ).ConfigureAwait( false );
				goto TryAgain;
			}

			exception.Log();
		}

		return default( Int32? );
	}

	/// <summary>Defaults to 10 attempts.</summary>
	/// <param name="value"></param>
	public void SetRetriesLeft( Int32 value ) => this._retriesLeft = value;

	public async Task UseDatabaseAsync( String databaseName, CancellationToken cancellationToken ) {
		if ( String.IsNullOrWhiteSpace( databaseName ) ) {
			throw new NullException( nameof( databaseName ) );
		}

		await using var adhoc = await this.QueryAdhocAsync( $"USE {databaseName.SmartBrackets()};", cancellationToken ).ConfigureAwait( false );
	}
}