// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries,
// repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper licenses and/or copyrights.
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "DocumentFile.cs" last formatted on 2022-03-14 at 3:55 AM by Protiguous.


namespace Librainian.FileSystem;

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text.RegularExpressions;
using Exceptions;
using Exceptions.Warnings;
using Maths;
using Maths.Numbers;
using Measurement.Time;
using Newtonsoft.Json;
using Parsing;
using PooledAwait;
using Security;
using Threading;
using Utilities;
using Utilities.Disposables;
using static Logging.Logging;

[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public class DocumentFile : ABetterClassDisposeAsync, IDocumentFile {

	private Folder? _containingFolder;

	private ThreadLocal<JsonSerializer> JsonSerializers { get; } = new( static () => new JsonSerializer {
		ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
		PreserveReferencesHandling = PreserveReferencesHandling.All
	} );

	private Lazy<FileSystemWatcher>? Watcher { get; }

	private Lazy<FileWatchingEvents>? WatchEvents { get; }

	private CancellationToken GetCancelToken() {
		this.CancellationTokenSource ??= new CancellationTokenSource();
		return this.CancellationTokenSource.Token;
	}

	protected DocumentFile( SerializationInfo info!! ) : base( info.FullTypeName ) =>
			this.FullPath = ( info.GetString( nameof( this.FullPath ) ) ?? throw new InvalidOperationException() ).TrimAndThrowIfBlank();

	/// <summary>
	///     Largest amount of memory that will be allocated for file reads.
	///     <para>1 gibibyte</para>
	/// </summary>
	public const Int32 MaximumBufferSize = MathConstants.Sizes.OneGigaByte;

	/// <summary></summary>
	/// <param name="fullPath"></param>
	/// <param name="deleteAfterClose"></param>
	/// <param name="watchFile"></param>
	/// <exception cref="InvalidOperationException">Unable to parse the given path.</exception>
	/// <exception cref="FileNotFoundException"></exception>
	/// <exception cref="DirectoryNotFoundException"></exception>
	/// <exception cref="IOException"></exception>
	public DocumentFile( String fullPath, Boolean deleteAfterClose = false, Boolean watchFile = false ) : base( fullPath ) {
		if ( String.IsNullOrWhiteSpace( fullPath ) ) {
			throw new NullException( nameof( fullPath ) );
		}

		this.FullPath = Path.Combine( Path.GetFullPath( fullPath ), Path.GetFileName( fullPath ) ).TrimAndThrowIfBlank();

		//TODO What about just keeping the full path along with the long path UNC prefix?

		if ( Uri.TryCreate( fullPath, UriKind.Absolute, out var uri ) ) {
			if ( uri.IsFile ) {
				//this.FullPath = Path.GetFullPath( uri.AbsolutePath );
				this.FullPath = Path.GetFullPath( fullPath );
				this.PathTypeAttributes = PathTypeAttributes.Document;
			}
			else if ( uri.IsAbsoluteUri ) {
				this.FullPath = uri.AbsolutePath; //how long can a URI be?
				this.PathTypeAttributes = PathTypeAttributes.Uri;
			}
			else if ( uri.IsUnc ) {
				this.FullPath = uri.AbsolutePath; //TODO verify this is the "long" path?
				this.PathTypeAttributes = PathTypeAttributes.UNC;
			}
			else {
				this.FullPath = fullPath;
				this.PathTypeAttributes = PathTypeAttributes.Unknown;

				throw new InvalidOperationException( $"Could not parse \"{fullPath}\"." );
			}
		}
		else {
			throw new InvalidOperationException( $"Could not parse \"{fullPath}\"." );
		}

		if ( deleteAfterClose ) {
			this.PathTypeAttributes &= PathTypeAttributes.DeleteAfterClose;
			Debug.Assert( this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose ) );
		}

		if ( watchFile ) {
			this.Watcher = new Lazy<FileSystemWatcher>( () => new FileSystemWatcher( this.ContainingingFolder().FullPath, this.FileName ) {
				IncludeSubdirectories = false,
				EnableRaisingEvents = true
			} );

			this.WatchEvents = new Lazy<FileWatchingEvents>( static () => new FileWatchingEvents(), false );

			var watcher = this.Watcher.Value;

			watcher.Created += ( _, e ) => this.WatchEvents.Value.OnCreated?.Invoke( e );
			watcher.Changed += ( _, e ) => this.WatchEvents.Value.OnChanged?.Invoke( e );
			watcher.Deleted += ( _, e ) => this.WatchEvents.Value.OnDeleted?.Invoke( e );
			watcher.Renamed += ( _, e ) => this.WatchEvents.Value.OnRenamed?.Invoke( e );
			watcher.Error += ( _, e ) => this.WatchEvents.Value.OnError?.Invoke( e );
		}
	}

	public DocumentFile( String justPath, String filename, Boolean deleteAfterClose = false ) : this( Path.Combine( justPath, filename ), deleteAfterClose ) {
	}

	public DocumentFile( FileSystemInfo info, Boolean deleteAfterClose = false ) : this( info.FullName, deleteAfterClose ) {
	}

	public DocumentFile( IFolder folder, String filename, Boolean deleteAfterClose = false ) : this( folder.FullPath, filename, deleteAfterClose ) {
	}

	public DocumentFile( IFolder folder, IDocumentFile documentFile, Boolean deleteAfterClose = false ) : this( Path.Combine( folder.FullPath, documentFile.FileName ),
		deleteAfterClose ) { }

	/// <summary>
	///     Get or sets the <see cref="TimeSpan" /> used when getting a fresh <see cref="CancellationToken" /> via
	///     <see cref="GetCancelToken" /> .
	/// </summary>
	public static TimeSpan DefaultDocumentTimeout { get; set; } = Seconds.Thirty;

	public static String InvalidFileNameCharacters { get; } = new( Path.GetInvalidFileNameChars() );

	public static Lazy<Regex> RegexForInvalidFileNameCharacters { get; } = new( static () =>
			   new Regex( $"[{Regex.Escape( InvalidFileNameCharacters )}]", RegexOptions.Compiled | RegexOptions.Singleline ) );

	public CancellationTokenSource? CancellationTokenSource { get; set; }

	/// <summary>Local file creation <see cref="DateTimeOffset" />.</summary>
	[JsonIgnore]
	public DateTimeOffset? CreationTime {
		get => this.CreationTimeUtc;

		set => this.CreationTimeUtc = value;
	}

	/// <summary>Gets or sets the file creation time, in coordinated universal time (UTC).</summary>
	[JsonIgnore]
	public DateTimeOffset? CreationTimeUtc {
		get => File.Exists( this.FullPath ) ? File.GetCreationTimeUtc( this.FullPath ) : default( DateTimeOffset? );

		set {
			if ( value is not null && File.Exists( this.FullPath ) ) {
				File.SetCreationTimeUtc( this.FullPath, value.Value.UtcDateTime );
			}
		}
	}

	public Boolean DeleteAfterClose {
		get => this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose );

		set {
			if ( value ) {
				this.PathTypeAttributes |= PathTypeAttributes.DeleteAfterClose;
				Debug.Assert( this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose ) );
			}
			else {
				this.PathTypeAttributes &= ~PathTypeAttributes.DeleteAfterClose;
				Debug.Assert( this.PathTypeAttributes.HasFlag( PathTypeAttributes.DeleteAfterClose ) );
			}
		}
	}

	/// <summary>
	///     <para>Just the file's name, including the extension (no path).</para>
	/// </summary>
	/// <example>
	///     <c>new Document("C:\Temp\Test.text").FileName() == "Test.text"</c>
	/// </example>
	/// <see cref="Path.GetFileName(ReadOnlySpan{Char})" />
	public String FileName => Path.GetFileName( this.FullPath );

	/// <summary>
	///     Represents the fully qualified path of the file.
	///     <para>Fully qualified "Drive:\Path\Folder\Filename.Ext"</para>
	/// </summary>
	[JsonProperty]
	public String FullPath { get; }

	/// <summary>Gets or sets the time the current file was last accessed.</summary>
	[JsonIgnore]
	public DateTimeOffset? LastAccessTime {
		get => this.LastAccessTimeUtc?.ToLocalTime();

		set => this.LastAccessTimeUtc = value;
	}

	/// <summary>Gets or sets the UTC time the file was last accessed.</summary>
	[JsonIgnore]
	public DateTimeOffset? LastAccessTimeUtc {
		get => File.Exists( this.FullPath ) ? File.GetLastAccessTimeUtc( this.FullPath ) : default( DateTimeOffset? );

		set {
			if ( value is not null && File.Exists( this.FullPath ) ) {
				File.SetLastAccessTimeUtc( this.FullPath, value.Value.UtcDateTime );
			}
		}
	}

	/// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
	[JsonIgnore]
	public DateTimeOffset? LastWriteTime {
		get => this.LastWriteTimeUtc?.ToLocalTime();

		set => this.LastWriteTimeUtc = value;
	}

	/// <summary>Gets or sets the UTC datetime when the file was last written to.</summary>
	[JsonIgnore]
	public DateTimeOffset? LastWriteTimeUtc {
		get => File.Exists( this.FullPath ) ? File.GetLastWriteTimeUtc( this.FullPath ) : default( DateTimeOffset? );

		set {
			if ( value is not null && File.Exists( this.FullPath ) ) {
				File.SetLastWriteTimeUtc( this.FullPath, value.Value.UtcDateTime );
			}
		}
	}

	/// <summary>
	///     <para>Just the file's name, including the extension.</para>
	/// </summary>
	/// <see cref="Path.GetFileNameWithoutExtension(ReadOnlySpan{Char})" />
	public String Name => this.FileName;

	[JsonIgnore]
	public PathTypeAttributes PathTypeAttributes { get; set; } = PathTypeAttributes.Unknown;

	/// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
	[JsonIgnore]
	public Object? Tag { get; set; }

	/// <summary>this seems to work great!</summary>
	/// <param name="address"></param>
	/// <param name="fileName"></param>
	/// <param name="progress"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	/// <exception cref="NullException"></exception>
	[NeedsTesting]
	public static async PooledValueTask<WebClient> DownloadFileTaskAsync(
		Uri address,
		String fileName,
		IProgress<(Int64 BytesReceived, Int32 ProgressPercentage, Int64 TotalBytesToReceive)>? progress
	) {
		Guards();

		var tcs = new TaskCompletionSource<Object?>( address, TaskCreationOptions.RunContinuationsAsynchronously );

		void CompletedHandler( Object? cs, AsyncCompletedEventArgs ce ) {
			if ( ce.UserState != tcs ) {
				return;
			}

			if ( ce.Error != null ) {
				_ = tcs.TrySetException( ce.Error );
			}
			else {
				_ = ce.Cancelled ? tcs.TrySetCanceled() : tcs.TrySetResult( null );
			}
		}

		void ProgressChangedHandler( Object? ps, DownloadProgressChangedEventArgs? pe ) {
			if ( pe?.UserState == tcs ) {
				progress?.Report( (pe.BytesReceived, pe.ProgressPercentage, pe.TotalBytesToReceive) );
			}
		}

		var webClient = new WebClient(); //TODO

		try {
			webClient.DownloadFileCompleted += CompletedHandler;
			webClient.DownloadProgressChanged += ProgressChangedHandler;
			webClient.DownloadFileAsync( address, fileName, tcs );

			await tcs.Task.ConfigureAwait( false );
		}
		finally {
			webClient.DownloadFileCompleted -= CompletedHandler;
			webClient.DownloadProgressChanged -= ProgressChangedHandler;
		}

		return webClient;

		void Guards() {
			if ( address is null ) {
				throw new ArgumentEmptyException( nameof( address ) );
			}

			if ( String.IsNullOrWhiteSpace( fileName ) ) {
				throw new NullException( nameof( fileName ) );
			}
		}
	}

	/// <summary>
	///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
	///     <para>To compare the contents of two <see cref="DocumentFile" /> use <see cref="IDocumentFile.SameContent" />.</para>
	///     <para>
	///         To quickly compare the contents of two <see cref="DocumentFile" /> use <see cref="CRC32" />,
	///         <see cref="HarkerHash32" />, <see cref="HarkerHash64" />, or
	///         <see cref="CRC64" /> .
	///     </para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	[NeedsTesting]
	public static Boolean Equals( IDocumentFile? x, IDocumentFile? y ) {
		if ( x is null || y is null ) {
			return false;
		}

		if ( ReferenceEquals( x, y ) ) {
			return true;
		}

		return x.FullPath.Is( y.FullPath ) && ( x.GetSize() == y.GetSize() );
	}

	[NeedsTesting]
	public static IDocumentFile GetTempDocument( String? extension = null ) {
		if ( String.IsNullOrEmpty( extension ) ) {
			extension = Guid.NewGuid().ToString();
		}

		extension = extension.TrimLeading( ".", StringComparison.OrdinalIgnoreCase );

		return new DocumentFile( Folder.GetTempFolder(), $"{Guid.NewGuid()}.{extension}" );
	}

	/// <summary>Pull a new <see cref="FileInfo" /> for the <paramref name="documentFile" />.</summary>
	/// <param name="documentFile"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	[NeedsTesting]
	public static implicit operator FileInfo( DocumentFile documentFile ) => ToFileInfo( documentFile ).AsValueTask().AsTask().Result;

	/// <summary>
	///     <para>Compares the file names (case sensitive) and file sizes for inequality.</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[NeedsTesting]
	public static Boolean operator !=( DocumentFile? left, IDocumentFile? right ) => !Equals( left, right );

	/// <summary>
	///     <para>Compares the file names (case sensitive) and file sizes for equality.</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[NeedsTesting]
	public static Boolean operator ==( DocumentFile? left, IDocumentFile? right ) => Equals( left, right );

	[NeedsTesting]
	public static async PooledValueTask<FileInfo> ToFileInfo( DocumentFile documentFile ) {
		if ( documentFile is null ) {
			throw new ArgumentEmptyException( nameof( documentFile ) );
		}

		return await Task.Run( () => {
			var info = new FileInfo( documentFile.FullPath );

			info.Refresh();

			return info;
		} )
						 .ConfigureAwait( false );
	}

	/// <summary>
	///     <para>If the file does not exist, it is created.</para>
	///     <para>Then the <paramref name="text" /> is appended to the file.</para>
	/// </summary>
	/// <param name="text"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="DirectoryNotFoundException"></exception>
	public async PooledValueTask<IDocumentFile> AppendText( String text, CancellationToken cancellationToken ) {
		var folder = this.ContainingingFolder();

		if ( !await folder.Exists( cancellationToken ).ConfigureAwait( false ) && !await folder.Create( cancellationToken ).ConfigureAwait( false ) ) {
			throw new DirectoryNotFoundException( $"Could not create folder {folder.FullPath.DoubleQuote()}." );
		}

		_ = await this.SetReadOnly( false, cancellationToken ).ConfigureAwait( false );

		await File.AppendAllTextAsync( this.FullPath, text, cancellationToken ).ConfigureAwait( false );

		return this;
	}

	/// <summary>Enumerates the <see cref="IDocumentFile" /> as a sequence of <see cref="Byte" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[NeedsTesting]
	public async IAsyncEnumerable<Byte> AsBytes( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[sizeof( Byte )];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return buffer[0];
		}
	}

	/// <summary>Enumerates the <see cref="IDocumentFile" /> as a sequence of <see cref="Int64" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[NeedsTesting]
	public async IAsyncEnumerable<Decimal> AsDecimal( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var fileLength = await this.Length( cancellationToken ).ConfigureAwait( false );

		if ( !fileLength.HasValue ) {
			yield break;
		}

		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffered = new BufferedStream( stream, optimal.Value ); //TODO Is this buffering twice??
		await using var __ = buffered.ConfigureAwait( false );

		using var br = new BinaryReader( buffered );

		while ( true ) {
			Decimal d;

			try {
				d = br.ReadDecimal();
			}
			catch ( EndOfStreamException ) {
				yield break;
			}
			catch ( IOException ) {
				yield break;
			}

			yield return d;
		}
	}

	/// <summary>Enumerates the <see cref="IDocumentFile" /> as a sequence of <see cref="Guid" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	public async IAsyncEnumerable<Guid> AsGuids( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[sizeof( Decimal )];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return new Guid( buffer );
		}
	}

	/// <summary>Enumerates the <see cref="IDocumentFile" /> as a sequence of <see cref="Int32" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[NeedsTesting]
	public async IAsyncEnumerable<Int32> AsInt32( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[sizeof( Int32 )];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return BitConverter.ToInt32( buffer, 0 );
		}
	}

	/// <summary>Enumerates the <see cref="IDocumentFile" /> as a sequence of <see cref="Int64" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[NeedsTesting]
	public async IAsyncEnumerable<Int64> AsInt64( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[sizeof( Int64 )];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, optimal.Value );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return BitConverter.ToInt64( buffer, 0 );
		}
	}

	/// <summary>Enumerates the <see cref="IDocumentFile" /> as a sequence of <see cref="UInt64" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
	[NeedsTesting]
	public async IAsyncEnumerable<UInt64> AsUInt64( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
		if ( optimal is null ) {
			yield break;
		}

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );

		if ( !stream.CanRead ) {
			throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
		}

		var buffer = new Byte[sizeof( UInt64 )];
		var length = buffer.Length;

		var buffered = new BufferedStream( stream, sizeof( UInt64 ) );
		await using var __ = buffered.ConfigureAwait( false );

		while ( ( await buffered.ReadAsync( buffer.AsMemory( 0, length ), cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			yield return BitConverter.ToUInt64( buffer, 0 );
		}
	}

	/// <summary>
	///     <para>Clone the entire IDocument to the <paramref name="destination" /> as quickly as possible.</para>
	///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
	/// </summary>
	/// <param name="destination"></param>
	/// <param name="progress"></param>
	/// <param name="eta"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	[NeedsTesting]
	public async PooledValueTask<(Status success, TimeSpan timeElapsed)> CloneDocument(
		IDocumentFile destination,
		IProgress<Single> progress,
		IProgress<TimeSpan> eta,
		CancellationToken cancellationToken
	) {
		if ( destination is null ) {
			throw new ArgumentEmptyException( nameof( destination ) );
		}

		if ( !Uri.TryCreate( this.FullPath, UriKind.Absolute, out var sourceAddress ) ) {
			return (Status.Failure, TimeSpan.Zero);
		}

		var stopwatch = Stopwatch.StartNew();

		try {
			var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
			if ( optimal is null or 0 ) {
				return (Status.Failure, stopwatch.Elapsed);
			}

			using var client = new HttpClient();

			await using var sourceStream = await client.GetStreamAsync( sourceAddress, cancellationToken ).ConfigureAwait( false );
			if ( !sourceStream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file stream on {this.FullPath.SmartQuote()}." );
			}

			await using var bufferedSource = new BufferedStream( sourceStream, optimal.Value );

			await using var destinationStream =
				new FileStream( destination.FullPath, FileMode.Create, FileAccess.Write, FileShare.None, optimal.Value, FileOptions.Asynchronous );

			await using var bufferedDestination = new BufferedStream( destinationStream, optimal.Value );

			await bufferedSource.CopyToAsync( bufferedDestination, optimal.Value, cancellationToken ).ConfigureAwait( false );

			//let go of the source asap. Not sure if this is needed.
			await using ( bufferedSource ) { }

			await using ( sourceStream ) { }

			return (Status.Success, stopwatch.Elapsed);
		}
		catch ( WebException exception ) {
			exception.Log();
			return (Status.Exception, stopwatch.Elapsed);
		}
	}

	[NeedsTesting]
	public IFolder ContainingingFolder() {
		if ( this._containingFolder is null ) {
			var directoryName = Path.GetDirectoryName( this.FullPath );

			if ( String.IsNullOrWhiteSpace( directoryName ) ) {
				//empty means a root-level folder (C:\) was found. Right?
				directoryName = Path.GetPathRoot( this.FullPath );
			}

			return this._containingFolder = new Folder( directoryName );
		}

		return this._containingFolder;
	}

	[NeedsTesting]
	public async Task<FileCopyData> Copy( FileCopyData fileCopyData, CancellationToken cancellationToken ) {
		if ( !Uri.TryCreate( fileCopyData.Source.FullPath, UriKind.RelativeOrAbsolute, out var uri ) ) {
			throw new UriFormatException( $"Unable to parse {this.FullPath.DoubleQuote()} into a Uri." );
		}

		try {
			if ( !await DoesSourceFileExist().ConfigureAwait( false ) ) {
				return fileCopyData with {
					Status = Status.Bad
				};
			}

			(var exists, var size) = await CheckSourceSize().ConfigureAwait( false );

			if ( !exists || size is null ) {
				return fileCopyData with {
					Status = Status.Failure
				};
			}

			fileCopyData.SourceSize = size.Value;

			var bytes = size.Value;
			Single bits = bytes * 8;
			const Int32 bitsPerSecond = 11 * MathConstants.Sizes.OneMegaByte;
			var guessSeconds = bits / bitsPerSecond;

			var estimatedTimeToCopy = TimeSpan.FromSeconds( guessSeconds * 2 );

			//TODO Add in capability to pause/resume?
			_ = await DownloadAndVerifySize().WithTimeout( estimatedTimeToCopy, fileCopyData.CancellationTokenSource.Token ).ConfigureAwait( false );
		}
		catch ( TaskCanceledException exception ) {
			//what is thrown when a Task<T> times out or is cancelled? TaskCanceledException or OperationCanceledException?
			RecordException( exception );
		}
		catch ( OperationCanceledException exception ) {
			RecordException( exception );
		}
		catch ( WebException exception ) {
			RecordException( exception );
		}
		catch ( Exception exception ) {
			RecordException( exception );
		}

		return fileCopyData;

		void RecordException( Exception? exception ) {
			if ( exception is null ) {
				return;
			}

			fileCopyData.Exceptions ??= new List<Exception>();
			fileCopyData.Exceptions.Add( exception );
		}

		async PooledValueTask<Boolean> DoesSourceFileExist() {
			$"Checking for existance of source file {fileCopyData.Source.FullPath.DoubleQuote()}".Verbose();
			var exists = await fileCopyData.Source.Exists( cancellationToken ).ConfigureAwait( false );
			if ( !exists ) {
				RecordException( new FileNotFoundException( "Missing file.", fileCopyData.Source.FullPath ) );

				return false;
			}

			return true;
		}

		async PooledValueTask<(Boolean exists, UInt64? size)> CheckSourceSize() {
			$"Checking for size of source file {fileCopyData.Source.FullPath.DoubleQuote()}".Verbose();
			var size = await fileCopyData.Source.Size( cancellationToken ).ConfigureAwait( false );

			if ( size is > 0 ) {
				return (true, size);
			}

			RecordException( new FileNotFoundException( "Empty file.", fileCopyData.Source.FullPath ) );

			return (false, default( UInt64? ));
		}

		async Task<Boolean> DownloadAndVerifySize() {
			using var webClient = new WebClient(); //TODO replace with HttpClient
			$"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} instantiated.".Verbose();

			webClient.Disposed += ( _, _ ) => $"{nameof( webClient )} {nameof( webClient.Disposed )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()}."
				.Verbose();

			webClient.DownloadFileCompleted += ( _, args ) => {
				fileCopyData.WhenCompleted = DateTimeOffset.UtcNow;

				RecordException( args.Error );

				if ( args.Error is null ) {
					fileCopyData.OnCompleted?.Invoke( fileCopyData );
				}
			};

			webClient.DownloadProgressChanged += ( _, args ) => fileCopyData.DataCopied?.Report( fileCopyData with {
				BytesCopied = ( UInt64 )args.BytesReceived
			} );

			$"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} started.".Verbose();
			await webClient.DownloadFileTaskAsync( uri, fileCopyData.Destination.FullPath ).ConfigureAwait( false );

			$"Checking existance of destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();
			if ( !await fileCopyData.Destination.Exists( cancellationToken ).ConfigureAwait( false ) ) {
				$"Could not find destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();

				return false;
			}

			$"Checking size of destination file {fileCopyData.Destination.FullPath.DoubleQuote()}.".Verbose();
			var destinationSize = await fileCopyData.Destination.Size( cancellationToken ).ConfigureAwait( false );

			if ( destinationSize is null ) {
				RecordException( new UnknownWarning(
					$"Unknown error occurred copying file {fileCopyData.Source.FullPath.DoubleQuote()} to {fileCopyData.Destination.ContainingingFolder().FullPath}." ) );

				return false;
			}

			if ( destinationSize != fileCopyData.SourceSize ) {
				RecordException( new UnknownWarning(
					$"Unknown error occurred copying file {fileCopyData.Source.FullPath.DoubleQuote()} to {fileCopyData.Destination.ContainingingFolder().FullPath}." ) );

				return false;
			}

			$"{nameof( webClient )} for file copy task {fileCopyData.Destination.FullPath.DoubleQuote()} done.".Verbose();

			return true;
		}
	}

	[NeedsTesting]
	public async PooledValueTask<Int32?> CRC32( CancellationToken cancellationToken ) {
		try {
			var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
			if ( optimal is null or 0 ) {
				return default( Int32? );
			}

			await using var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value,
				FileOptions.SequentialScan | FileOptions.Asynchronous );

			await using var buffered = new BufferedStream( fileStream, optimal.Value );

			using var crc32 = new CRC32( ( UInt32 )optimal, ( UInt32 )optimal );
			var hash = await crc32.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

			return BitConverter.ToInt32( hash, 0 );
		}
		catch ( FileNotFoundException exception ) {
			exception.Log();
		}
		catch ( DirectoryNotFoundException exception ) {
			exception.Log();
		}
		catch ( PathTooLongException exception ) {
			exception.Log();
		}
		catch ( IOException exception ) {
			exception.Log();
		}
		catch ( UnauthorizedAccessException exception ) {
			exception.Log();
		}

		return default( Int32? );
	}

	/// <summary></summary>
	/// <param name="cancellationToken"></param>
	/// <exception cref="InvalidOperationException"></exception>
	[NeedsTesting]
	public async PooledValueTask<String?> CRC32Hex( CancellationToken cancellationToken ) {
		try {
			var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );

			if ( optimal is null or 0 ) {
				return default( String? );
			}

			await using var sourceStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value,
				FileOptions.SequentialScan | FileOptions.Asynchronous );

			await using var buffered = new BufferedStream( sourceStream, optimal.Value );

			using var crc32 = new CRC32( ( UInt32 )optimal.Value, ( UInt32 )optimal.Value );
			var hash = await crc32.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

			return hash.Aggregate( String.Empty, static ( current, b ) => current + $"{b:X}" );
		}
		catch ( FileNotFoundException exception ) {
			exception.Log();
		}
		catch ( DirectoryNotFoundException exception ) {
			exception.Log();
		}
		catch ( PathTooLongException exception ) {
			exception.Log();
		}
		catch ( IOException exception ) {
			exception.Log();
		}
		catch ( UnauthorizedAccessException exception ) {
			exception.Break();
		}

		return null;
	}

	[NeedsTesting]
	public async PooledValueTask<Int64?> CRC64( CancellationToken cancellationToken ) {
		try {
			var size = await this.Size( cancellationToken ).ConfigureAwait( false );

			if ( size?.Any() is true ) {
				var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );

				using var crc64 = new CRC64( size.Value, size.Value );

				//TODO Would BufferedStream be any faster here?
				var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
				_ = fileStream.ConfigureAwait( false );
				var buffered = new BufferedStream( fileStream, optimal.Value );
				await using var __ = buffered.ConfigureAwait( false );

				var hash = await crc64.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

				return BitConverter.ToInt64( hash, 0 );
			}
		}
		catch ( FileNotFoundException exception ) {
			exception.Log();
		}
		catch ( DirectoryNotFoundException exception ) {
			exception.Log();
		}
		catch ( PathTooLongException exception ) {
			exception.Log();
		}
		catch ( IOException exception ) {
			exception.Log();
		}
		catch ( UnauthorizedAccessException exception ) {
			exception.Log();
		}

		return null;
	}

	/// <summary>Returns a lowercase hex-string of the hash.</summary>
	/// <param name="cancellationToken"></param>
	[NeedsTesting]
	public async PooledValueTask<String?> CRC64Hex( CancellationToken cancellationToken ) {
		try {
			var size = await this.Size( cancellationToken ).ConfigureAwait( false );

			if ( size?.Any() is true ) {
				var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false );
				using var crc64 = new CRC64( size.Value, size.Value );

				var fileStream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal.Value, FileOptions.SequentialScan );
				_ = fileStream.ConfigureAwait( false );
				var buffered = new BufferedStream( fileStream, optimal.Value );
				await using var __ = buffered.ConfigureAwait( false );

				var hash = await crc64.ComputeHashAsync( buffered, cancellationToken ).ConfigureAwait( false );

				return hash.Aggregate( String.Empty, ( current, b ) => current + $"{b:X}" );
			}
		}
		catch ( FileNotFoundException exception ) {
			exception.Log();
		}
		catch ( DirectoryNotFoundException exception ) {
			exception.Log();
		}
		catch ( PathTooLongException exception ) {
			exception.Log();
		}
		catch ( IOException exception ) {
			exception.Log();
		}
		catch ( UnauthorizedAccessException exception ) {
			exception.Log();
		}

		return null;
	}

	/// <summary>Deletes the file.</summary>
	/// <param name="cancellationToken"></param>
	public async PooledValueTask Delete( CancellationToken cancellationToken ) {
		var fileInfo = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );

		if ( fileInfo.Exists ) {
			if ( fileInfo.IsReadOnly ) {
				fileInfo.IsReadOnly = false;
			}

			fileInfo.Delete();
		}
	}

	public override async ValueTask DisposeManagedAsync() {
		if ( this.DeleteAfterClose ) {
			await this.Delete( this.GetCancelToken() ).ConfigureAwait( false );
		}

		await base.DisposeManagedAsync().ConfigureAwait( false );
	}

	/// <summary>
	///     <para>Downloads (replaces) the local IDocument with the specified <paramref name="source" />.</para>
	///     <para>Note: will replace the content of the this <see cref="IDocumentFile" />.</para>
	/// </summary>
	/// <param name="source"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	[NeedsTesting]
	public async PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> DownloadFile( Uri source ) {
		if ( source is null ) {
			throw new ArgumentEmptyException( nameof( source ) );
		}

		//TODO possibly download entire file, delete original version, then rename the newly downloaded file?

		try {
			if ( !source.IsWellFormedOriginalString() ) {
				return (new Exception( $"Could not use source Uri '{source}'." ), null);
			}

			using var webClient = new WebClient(); //from what I've read, Dispose should NOT be being called on a WebClient???

			await webClient.DownloadFileTaskAsync( source, this.FullPath ).ConfigureAwait( false );

			return (null, webClient.ResponseHeaders);
		}
		catch ( Exception exception ) {
			return (exception, null);
		}
	}

	/// <summary>
	///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
	///     <para>To compare the contents of two <see cref="IDocumentFile" /> use <see cref="IDocumentFile.SameContent" />.</para>
	/// </summary>
	/// <param name="other"></param>
	[NeedsTesting]
	public Boolean Equals( IDocumentFile? other ) => this.Equals( this, other );

	/// <summary>
	///     <para>To compare the contents of two <see cref="IDocumentFile" /> use SameContent( IDocument,IDocument).</para>
	/// </summary>
	/// <param name="obj"></param>
	[NeedsTesting]
	public override Boolean Equals( Object? obj ) => obj is IDocumentFile document && this.Equals( this, document );

	/// <summary>Returns whether the file exists.</summary>
	/// <param name="cancellationToken"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public async PooledValueTask<Boolean> Exists( CancellationToken cancellationToken ) {
		var info = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );
		return info.Exists;
	}

	/// <summary>
	///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
	/// </summary>
	[NeedsTesting]
	public String Extension() => Path.GetExtension( this.FullPath ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

	public IAsyncEnumerator<Byte> GetAsyncEnumerator( CancellationToken cancellationToken = default( CancellationToken ) ) => this.AsBytes( cancellationToken ).GetAsyncEnumerator( cancellationToken );

	/// <summary>Returns an enumerator that iterates through the collection.</summary>
	/// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
	[NeedsTesting]
	public IAsyncEnumerator<Byte> GetEnumerator() => this.AsBytes( CancellationToken.None ).GetAsyncEnumerator();

	/// <summary>Synchronous version.</summary>
	public Boolean GetExists() {
		var info = new FileInfo( this.FullPath );
		info.Refresh();
		return info.Exists;
	}

	/// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="FullPath" />.</summary>
	/// <param name="cancellationToken"></param>
	/// <see cref="op_Implicit" />
	/// <see cref="ToFileInfo" />
	[NeedsTesting]
	public PooledValueTask<FileInfo> GetFreshInfo( CancellationToken cancellationToken ) => ToFileInfo( this );

	/// <summary>(file name, not contents)</summary>
	[NeedsTesting]
	public override Int32 GetHashCode() => this.FullPath.GetHashCode();

	/// <summary>Synchronous version.</summary>
	public UInt64? GetLength() {
		var info = new FileInfo( this.FullPath );
		info.Refresh();
		return info.Exists ? ( UInt64 )info.Length : null;
	}

	public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) => info.AddValue( nameof( this.FullPath ), this.FullPath, typeof( String ) );

	/// <summary>
	///     <para>
	///         Could we allocate a full 2GB buffer if we wanted? that'd be really nice for the <see cref="Copy" />
	///         routines.
	///     </para>
	///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
	///     <para>
	///         <remarks>A null size means the file was not found.</remarks>
	///     </para>
	/// </summary>
	/// <param name="cancellationToken"></param>
	[NeedsTesting]
	public async PooledValueTask<Int32?> GetOptimalBufferSize( CancellationToken cancellationToken ) {
		var size = await this.Size( cancellationToken ).ConfigureAwait( false );

		return size switch {
			null => null,
			>= MaximumBufferSize => MaximumBufferSize,
			var _ => ( Int32 )size
		};
	}

	/// <summary>Synchronous version.</summary>
	public UInt64? GetSize() {
		var info = new FileInfo( this.FullPath );
		info.Refresh();
		return ( UInt64? )info.Length;
	}

	/// <summary>HarkerHash (hash-by-addition)</summary>
	/// <param name="cancellationToken"></param>
	[NeedsTesting]
	public async Task<Int32> HarkerHash32( CancellationToken cancellationToken ) =>
		await this.AsInt32( cancellationToken ).Select( static i => i == 0 ? 1 : i ).SumAsync( cancellationToken ).ConfigureAwait( false );

	[NeedsTesting]
	public async Task<Int64> HarkerHash64( CancellationToken cancellationToken ) =>
		await this.AsInt64( cancellationToken ).Select( static i => i == 0L ? 1L : i ).SumAsync( cancellationToken ).ConfigureAwait( false );

	/// <summary>"poor mans Decimal hash"</summary>
	/// <param name="cancellationToken"></param>
	[NeedsTesting]
	public async Task<Decimal> HarkerHashDecimal( CancellationToken cancellationToken ) =>
		await this.AsDecimal( cancellationToken ).Select( static i => i == 0M ? 1M : i ).SumAsync( cancellationToken ).ConfigureAwait( false );

	/// <summary>Returns the filename, without the extension.</summary>
	[NeedsTesting]
	public String JustName() => Path.GetFileNameWithoutExtension( this.FileName );

	/// <summary>Attempt to start the process.</summary>
	/// <param name="arguments"></param>
	/// <param name="verb">"runas" is elevated</param>
	/// <param name="useShell"></param>
	[NeedsTesting]
	public PooledValueTask<Process?> Launch( String? arguments = null, String? verb = "runas", Boolean useShell = false ) {
		try {
			var info = new ProcessStartInfo( this.FullPath ) {
				Arguments = arguments ?? String.Empty,
				UseShellExecute = useShell,
				Verb = verb ?? String.Empty
			};

			var process = Process.Start( info );
			if ( process != null ) {
				return new PooledValueTask<Process?>( process );
			}
		}
		catch ( Exception exception ) {
			exception.Log();
			throw;
		}

		return new PooledValueTask<Process?>( null );
	}

	/// <summary>Returns the length of the file (default if it doesn't exists).</summary>
	/// <param name="cancellationToken"></param>
	public async PooledValueTask<UInt64?> Length( CancellationToken cancellationToken ) {
		var info = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );

		return info.Exists ? ( UInt64? )info.Length : default( UInt64? );
	}

	/// <summary>Attempt to return an object Deserialized from this JSON text file.</summary>
	/// <param name="progress"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="T"></typeparam>
	public async PooledValueTask<(Status status, T? obj)> LoadJSON<T>( IProgress<ZeroToOne>? progress, CancellationToken cancellationToken ) {
		var i = 0.0;
		const Double maxsteps = 6.0;

		try {
			progress?.Report( ++i / maxsteps );

			if ( !await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
				progress?.Report( new ZeroToOne( ZeroToOne.MaximumValue ) );

				return (Status.Bad, default( T? ));
			}

			progress?.Report( ++i / maxsteps );

			using var textReader = File.OpenText( this.FullPath );
			progress?.Report( ++i / maxsteps );

			var jsonReader = new JsonTextReader( textReader );
			progress?.Report( ++i / maxsteps );

			try {
				var run = await Task.Run( () => this.JsonSerializers.Value!.Deserialize<T>( jsonReader ), cancellationToken ).ConfigureAwait( false );

				progress?.Report( ++i / maxsteps );

				return (Status.Success, run);
			}
			finally {
				progress?.Report( ++i / maxsteps );
			}
		}
		catch ( TaskCanceledException ) {
			progress?.Report( new ZeroToOne( ZeroToOne.MaximumValue ) );
		}
		catch ( Exception exception ) {
			progress?.Report( new ZeroToOne( ZeroToOne.MaximumValue ) );
			exception.Log();
		}

		return (Status.Exception, default( T? ));
	}

	public async IAsyncEnumerable<String> ReadLines( [EnumeratorCancellation] CancellationToken cancellationToken ) {
		var size = await this.Size( cancellationToken ).ConfigureAwait( false );

		if ( !size.Any() ) {
			yield break;
		}

		var optimal = await this.GetOptimalBufferSize( cancellationToken ).ConfigureAwait( false ) ?? 4096;

		var stream = new FileStream( this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, optimal, FileOptions.SequentialScan );
		await using var _ = stream.ConfigureAwait( false );
		var buffered = new BufferedStream( stream, optimal );
		await using var __ = buffered.ConfigureAwait( false );

		using var reader = new StreamReader( buffered );

		while ( !cancellationToken.IsCancellationRequested ) {
			var line = await reader.ReadLineAsync().ConfigureAwait( false );

			if ( line is null ) {
				break;
			}

			yield return line;
		}
	}

	[NeedsTesting]
	public async PooledValueTask<String> ReadStringAsync() {
		using var reader = new StreamReader( this.FullPath );

		return await reader.ReadToEndAsync().ConfigureAwait( false );
	}

	/// <summary>
	///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocumentFile" /> file names.</para>
	/// </summary>
	/// <param name="right"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	/// <exception cref="SecurityException"></exception>
	/// <exception cref="NullException"></exception>
	/// <exception cref="UnauthorizedAccessException"></exception>
	/// <exception cref="PathTooLongException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="IOException"></exception>
	/// <exception cref="DirectoryNotFoundException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	[NeedsTesting]
	public async PooledValueTask<Boolean> SameContent( DocumentFile? right, CancellationToken cancellationToken ) {
		if ( right is null ) {
			return false;
		}

		if ( !await this.Exists( cancellationToken ).ConfigureAwait( false ) || !await right.Exists( cancellationToken ).ConfigureAwait( false ) ) {
			return false;
		}

		if ( await this.Size( cancellationToken ).ConfigureAwait( false ) != await right.Size( cancellationToken ).ConfigureAwait( false ) ) {
			return false;
		}

		var lefts = this.AsDecimal( cancellationToken ); //Could use AsBytes.. any performance difference?
		var rights = right.AsDecimal( cancellationToken ); //Could use AsGuids also?

		return await lefts.SequenceEqualAsync( rights, cancellationToken ).ConfigureAwait( false );
	}

	/// <summary>
	///     <para>If the file does not exist, return <see cref="Status.Error" />.</para>
	///     <para>If an exception happens, return <see cref="Status.Exception" />.</para>
	///     <para>Otherwise, return <see cref="Status.Success" />.</para>
	/// </summary>
	/// <param name="value"></param>
	/// <param name="cancellationToken"></param>
	public async PooledValueTask<Status> SetReadOnly( Boolean value, CancellationToken cancellationToken ) {
		var info = await this.GetFreshInfo( cancellationToken ).ConfigureAwait( false );

		if ( !info.Exists ) {
			return Status.Error;
		}

		try {
			if ( info.IsReadOnly != value ) {
				info.IsReadOnly = value;
			}

			return Status.Success;
		}
		catch ( Exception ) {
			return Status.Exception;
		}
	}

	/// <summary>Returns the size of the file, if it exists.</summary>
	/// <param name="cancellationToken"></param>
	[NeedsTesting]
	public PooledValueTask<UInt64?> Size( CancellationToken cancellationToken ) => this.Length( cancellationToken );

	/// <summary>Open the file for reading and return a <see cref="StreamReader" />.</summary>
	[NeedsTesting]
	public StreamReader StreamReader() => new( File.OpenRead( this.FullPath ) );

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	[NeedsTesting]
	public override String ToString() => this.FullPath;

	/// <summary>
	///     <para>Returns true if this <see cref="DocumentFile" /> no longer seems to exist.</para>
	/// </summary>
	/// <param name="delayBetweenRetries"></param>
	/// <param name="cancellationToken"></param>
	public async PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken cancellationToken ) {
		while ( !cancellationToken.IsCancellationRequested && await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
			try {
				if ( await this.Exists( cancellationToken ).ConfigureAwait( false ) ) {
					await this.Delete( cancellationToken ).ConfigureAwait( false );
				}
			}
			catch ( DirectoryNotFoundException ) { }
			catch ( PathTooLongException ) { }
			catch ( IOException ) {
				// IOException is thrown when the file is in use by any process.
				await Task.Delay( delayBetweenRetries, cancellationToken ).ConfigureAwait( false );
			}
			catch ( UnauthorizedAccessException ) { }
			catch ( ArgumentEmptyException ) { }
		}

		return await this.Exists( cancellationToken ).ConfigureAwait( false );
	}

	[NeedsTesting]
	public PooledValueTask<Status> TurnOffReadonly( CancellationToken cancellationToken ) => this.SetReadOnly( false, cancellationToken );

	[NeedsTesting]
	public PooledValueTask<Status> TurnOnReadonly( CancellationToken cancellationToken ) => this.SetReadOnly( true, cancellationToken );

	/// <summary>Uploads this <see cref="IDocumentFile" /> to the given <paramref name="destination" />.</summary>
	/// <param name="destination"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	[NeedsTesting]
	public async PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> UploadFile( Uri destination ) {
		if ( destination is null ) {
			throw new ArgumentEmptyException( nameof( destination ) );
		}

		if ( !destination.IsWellFormedOriginalString() ) {
			return (new InvalidOperationException( $"Destination address '{destination.OriginalString}' is not well formed." ), null);
		}

		try {
			using var webClient = new WebClient();

			_ = await webClient.UploadFileTaskAsync( destination, this.FullPath ).ConfigureAwait( false );

			return (null, webClient.ResponseHeaders);
		}
		catch ( Exception exception ) {
			return (exception, null);
		}
	}

	/*
    private async PooledValueTask ThrowIfNotExists() {
        if ( !await this.Exists( this.GetCancelToken() ).ConfigureAwait( false ) ) {
            throw new FileNotFoundException( $"Could find document {this.FullPath.SmartQuote()}." );
        }
    }
    */
	/*

    /// <summary>Returns true if this IDocument was copied to the <paramref name="destination" />.</summary>
    /// <param name="destination"></param>
    /// <param name="onEachProgress"></param>
    /// <param name="progress"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    [NeedsTesting]
    public async PooledValueTask<(WebClient? downloader, Boolean? exists)> Copy(
        [NeedsTesting] IDocument destination,
        [NeedsTesting] Action<(IDocument, UInt64 bytesReceived, UInt64 totalBytesToReceive)> onEachProgress,
        [NeedsTesting] Action<DownloadProgressChangedEventArgs>? progress ,
        CancellationToken onComplete  ) {
        if ( destination is null ) {
            throw new ArgumentEmptyException( nameof( destination ) );
        }

        if ( !this.Exists() ) {
            return ( default, default );
        }

        if ( destination.Exists() ) {
            destination.Delete();

            if ( destination.Exists() ) {
                return ( default, default );
            }
        }

        if ( !this.Length.HasValue || !this.Length.Any() ) {
            await using var stream = File.Create( destination.FullPath, 1, FileOptions.None );

            return ( default, true ); //just create an empty file?
        }

        var webClient = new WebClient {
            DownloadProgressChanged += ( sender, args ) => progress?.Invoke( args ),
            DownloadFileCompleted += ( sender, args ) => onComplete?.Invoke( args, ( this, destination ) )
        };

        await webClient.DownloadFileTaskAsync( this.FullPath, destination.FullPath ).ConfigureAwait( false );

        return ( webClient, destination.Exists() && destination.Size() == this.Size() );
    }
    */
}