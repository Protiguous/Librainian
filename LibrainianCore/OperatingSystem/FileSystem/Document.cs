﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Document.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "Document.cs" was last formatted by Protiguous on 2020/03/16 at 3:09 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Internet;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using PooledAwait;
    using Security;
    using Utilities;

    // ReSharper disable RedundantUsingDirective
    using Path = Pri.LongPath.Path;
    using FileInfo = Pri.LongPath.FileInfo;
    using FileSystemInfo = Pri.LongPath.FileSystemInfo;
    using Directory = Pri.LongPath.Directory;
    using File = Pri.LongPath.File;

    // ReSharper restore RedundantUsingDirective

    public interface IDocument : IComparable<IDocument>, IEquatable<IDocument>, IEnumerable<Byte> {

        [NotNull]
        String FullPath { get; }

        /*

        /// <summary>
        ///     Gets or sets the <see cref="System.IO.FileAttributes" /> for <see cref="FullPath" />.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        FileAttributes? FileAttributes { get; set; }
        */

        /// <summary>Local file creation <see cref="DateTime" />.</summary>
        DateTime? CreationTime { get; set; }

        /// <summary>Gets or sets the file creation time, in coordinated universal time (UTC).</summary>
        DateTime? CreationTimeUtc { get; set; }

        //FileAttributeData FileAttributeData { get; }

        /// <summary>Gets or sets the time the current file was last accessed.</summary>
        DateTime? LastAccessTime { get; set; }

        /// <summary>Gets or sets the UTC time the file was last accessed.</summary>
        DateTime? LastAccessTimeUtc { get; set; }

        /// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
        DateTime? LastWriteTime { get; set; }

        /// <summary>Gets or sets the UTC datetime when the file was last written to.</summary>
        DateTime? LastWriteTimeUtc { get; set; }

        PathTypeAttributes PathTypeAttributes { get; }

        /// <summary>Returns the length of the file (if it exists).</summary>
        UInt64? Length { get; }

        /// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
        [CanBeNull]
        Object Tag { get; set; }

        Boolean DeleteAfterClose { get; set; }

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <example>
        ///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
        /// </example>
        /// <see cref="Path.GetFileName" />
        [NotNull]
        String FileName { get; }

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <see cref="Path.GetFileNameWithoutExtension" />
        [NotNull]
        String Name { get; }

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        IEnumerable<Byte> AsBytes( FileOptions options = FileOptions.SequentialScan );

        /// <summary>"poor mans hash" heh</summary>
        /// <returns></returns>
        Int32 CalculateHarkerHashInt32();

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        IEnumerable<Int32> AsInt32( FileOptions options = FileOptions.SequentialScan );

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        IEnumerable<Int64> AsInt64( FileOptions options = FileOptions.SequentialScan );

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Guid" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        [NotNull]
        IEnumerable<Guid> AsGuids( FileOptions options = FileOptions.SequentialScan );

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.</summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        IEnumerable<UInt64> AsUInt64( FileOptions options = FileOptions.SequentialScan );

        /// <summary>HarkerHash (hash-by-addition) ( )</summary>
        /// <remarks>
        ///     <para>The result is the same, No Matter What Order the bytes are read in. Right?</para>
        ///     <para>So it should be able to be read in parallel..</para>
        /// </remarks>
        /// <returns></returns>
        Int32 CalcHashInt32( Boolean inParallel = true );

        /// <summary>Deletes the file.</summary>
        void Delete();

        /// <summary>Returns whether the file exists.</summary>
        [Pure]
        Boolean Exists();

        Folder ContainingingFolder();

        /// <summary>
        ///     <para>Clone the entire document to the <paramref name="destination" /> as quickly as possible.</para>
        ///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="token"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        PooledValueTask<(Boolean success, TimeSpan timeElapsed)> Clone( [NotNull] IDocument destination, CancellationToken token,
            [CanBeNull] IProgress<Single> progress = null, [CanBeNull] IProgress<TimeSpan> eta = null );

        /// <summary>Returns the <see cref="WebClient" /> if a file copy was started.</summary>
        /// <param name="destination"></param>
        /// <param name="onProgress"> </param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        Task Copy( [NotNull] IDocument destination, [CanBeNull] Action<(UInt64 bytesReceived, UInt64 totalBytesToReceive)> onProgress, [CanBeNull] Action onCompleted );

        Int32? CRC32();

        Task<Int32?> CRC32Async( CancellationToken token );

        String CRC32Hex();

        /// <summary>Returns a lowercase hex-string of the hash.</summary>
        /// <returns></returns>
        Task<String> CRC32HexAsync( CancellationToken token );

        Int64? CRC64();

        Task<Int64?> CRC64Async( CancellationToken token );

        /// <summary>Returns a lowercase hex-string of the hash.</summary>
        /// <returns></returns>
        String CRC64Hex();

        /// <summary>Returns a lowercase hex-string of the hash.</summary>
        /// <returns></returns>
        Task<String> CRC64HexAsync( CancellationToken token );

        /// <summary>
        ///     <para>Downloads (replaces) the local document with the specified <paramref name="source" />.</para>
        ///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        PooledValueTask<(Exception exception, WebHeaderCollection responseHeaders)> DownloadFile( [NotNull] Uri source );

        /// <summary>
        ///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
        /// </summary>
        [NotNull]
        String Extension();

        /// <summary>Returns the size of the file, if it exists.</summary>
        /// <returns></returns>
        UInt64? Size();

        /// <summary>
        ///     <para>If the file does not exist, it is created.</para>
        ///     <para>Then the <paramref name="text" /> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        [NotNull]
        IDocument AppendText( String text );

        /// <summary>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        Boolean Equals( Object other );

        /// <summary>(file name, not contents)</summary>
        /// <returns></returns>
        Int32 GetHashCode();

        /// <summary>Returns the filename, without the extension.</summary>
        /// <returns></returns>
        String JustName();

        /// <summary>
        ///     <para>Could we allocate a full 2GB buffer if we wanted? that'd be really nice for the <see cref="IDocument" /> copy routines...</para>
        ///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
        /// </summary>
        Int32 GetBufferSize();

        /// <summary>Attempt to start the process.</summary>
        /// <param name="arguments"></param>
        /// <param name="verb">     "runas" is elevated</param>
        /// <param name="useShell"></param>
        /// <returns></returns>
        PooledValueTask<Process> Launch( [CanBeNull] String arguments = null, String verb = "runas", Boolean useShell = false );

        /// <summary>Attempt to return an object Deserialized from this JSON text file.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T LoadJSON<T>();

        Task<T> LoadJSONAsync<T>( CancellationToken token );

        Task<String> ReadStringAsync();

        /// <summary>
        ///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        PooledValueTask<Boolean> SameContent( [CanBeNull] Document right );

        /// <summary>Open the file for reading and return a <see cref="StreamReader" />.</summary>
        /// <returns></returns>
        StreamReader StreamReader();

        /// <summary>Open the file for writing and return a <see cref="StreamWriter" />.</summary>
        /// <returns></returns>
        StreamWriter StreamWriter( [CanBeNull] Encoding encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte );

        /// <summary>Return this <see cref="IDocument" /> as a JSON string.</summary>
        /// <returns></returns>
        PooledValueTask<String> ToJSON();

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        String ToString();

        /// <summary>
        ///     <para>Returns true if the <see cref="IDocument" /> no longer seems to exist.</para>
        /// </summary>
        /// <param name="delayBetweenRetries"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken token );

        /// <summary>Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.</summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        PooledValueTask<(Exception exception, WebHeaderCollection responseHeaders)> UploadFile( [NotNull] Uri destination );

        PooledValueTask<Boolean> IsAll( Byte number );

        /// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="Document.FullPath" />.</summary>
        /// <see cref="Document.op_Implicit" />
        /// <returns></returns>
        [NotNull]
        FileInfo GetFreshInfo();

    }

    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class Document : ABetterClassDispose, IDocument {

        /// <summary>Compares this. <see cref="FullPath" /> against other <see cref="FullPath" />.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [Pure]
        public Int32 CompareTo( [NotNull] IDocument other ) {
            if ( other == null ) {
                throw new ArgumentNullException( paramName: nameof( other ) );
            }

            return String.Compare( strA: this.FullPath, strB: other.FullPath, comparisonType: StringComparison.Ordinal );
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
        [Pure]
        public IEnumerator<Byte> GetEnumerator() => this.AsBytes().GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     <para>Compares the file names (case insensitive) and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use <see cref="IDocument.SameContent" />.</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [Pure]
        public Boolean Equals( IDocument other ) => Equals( left: this, right: other );

        /// <summary>Represents the fully qualified path of the file.
        /// <para>Fully qualified "Drive:\Path\Folder\Filename.Ext"</para>
        /// </summary>
        [JsonProperty]
        [NotNull]
        public String FullPath { get; }

        /*

        /// <summary>
        ///     Gets or sets the <see cref="System.IO.FileAttributes" /> for <see cref="FullPath" />.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        [JsonIgnore]
        [Pure] public FileAttributes? FileAttributes {
            get => this.FileAttributeData.FileAttributes;

            set {
                if ( value.HasValue ) {
                    if ( !NativeMethods.SetFileAttributes( fileName: this.FullPath, dwFileAttributes: ( UInt32 )value.Value ) ) {
                        NativeMethods.HandleLastError( fullPath: this.FullPath );
                    }
                }

                this._fileAttributeData.FileAttributes = value;
            }
        }
        */

        /// <summary>Local file creation <see cref="DateTime" />.</summary>
        [JsonIgnore]
        public DateTime? CreationTime {
            get => this.CreationTimeUtc?.ToLocalTime();
            set => this.CreationTimeUtc = value?.ToUniversalTime();
        }

        /// <summary>Gets or sets the file creation time, in coordinated universal time (UTC).</summary>
        [JsonIgnore]
        public DateTime? CreationTimeUtc {
            get => File.Exists( path: this.FullPath ) ? File.GetCreationTimeUtc( path: this.FullPath ) : default( DateTime? );

            set {
                if ( value.HasValue && File.Exists( path: this.FullPath ) ) {
                    File.SetCreationTimeUtc( path: this.FullPath, creationTimeUtc: value.Value );
                }
            }
        }

        /// <summary>Gets or sets the time the current file was last accessed.</summary>
        [JsonIgnore]
        public DateTime? LastAccessTime {
            get => this.LastAccessTimeUtc?.ToLocalTime();
            set => this.LastAccessTimeUtc = value?.ToUniversalTime();
        }

        /// <summary>Gets or sets the UTC time the file was last accessed.</summary>
        [JsonIgnore]
        public DateTime? LastAccessTimeUtc {
            get => File.Exists( path: this.FullPath ) ? File.GetLastAccessTimeUtc( path: this.FullPath ) : default( DateTime? );

            set {
                if ( value.HasValue && File.Exists( path: this.FullPath ) ) {
                    File.SetLastAccessTimeUtc( path: this.FullPath, lastAccessTimeUtc: value.Value );
                }
            }
        }

        /// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
        [JsonIgnore]
        public DateTime? LastWriteTime {
            get => this.LastWriteTimeUtc?.ToLocalTime();
            set => this.LastWriteTimeUtc = value?.ToUniversalTime();
        }

        /// <summary>Gets or sets the UTC datetime when the file was last written to.</summary>
        [JsonIgnore]
        public DateTime? LastWriteTimeUtc {
            get => File.Exists( path: this.FullPath ) ? File.GetLastWriteTimeUtc( path: this.FullPath ) : default( DateTime? );

            set {
                if ( value.HasValue && File.Exists( path: this.FullPath ) ) {
                    File.SetLastWriteTimeUtc( path: this.FullPath, lastWriteTimeUtc: value.Value );
                }
            }
        }

        [JsonIgnore]
        public PathTypeAttributes PathTypeAttributes { get; set; } = PathTypeAttributes.Unknown;

        /// <summary>Returns the length of the file (default if it doesn't exists).</summary>
        [JsonIgnore]
        public UInt64? Length {
            get {
                var info = this.GetFreshInfo();

                return info.Exists ? ( UInt64? ) info.Length : default;
            }
        }

        /// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
        [JsonIgnore]
        [CanBeNull]
        public Object Tag { get; set; }

        public Boolean DeleteAfterClose {
            get => this.PathTypeAttributes.HasFlag( flag: PathTypeAttributes.DeleteAfterClose );

            set {
                if ( value ) {
                    this.PathTypeAttributes |= PathTypeAttributes.DeleteAfterClose;
                    Debug.Assert( condition: this.PathTypeAttributes.HasFlag( flag: PathTypeAttributes.DeleteAfterClose ) );
                }
                else {
                    this.PathTypeAttributes &= ~PathTypeAttributes.DeleteAfterClose;
                    Debug.Assert( condition: this.PathTypeAttributes.HasFlag( flag: PathTypeAttributes.DeleteAfterClose ) );
                }
            }
        }

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        [NotNull]
        [Pure]
        public IEnumerable<Byte> AsBytes( FileOptions options = FileOptions.SequentialScan ) {
            if ( !this.Exists() ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneMegaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
                }

                var buffer = new Byte[ sizeof( Byte ) ];

                using ( var buffered = new BufferedStream( stream: stream ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: buffer.Length ).Any() ) {
                        yield return buffer[ 0 ];
                    }
                }
            }
        }

        /// <summary>"poor mans Int32 hash"</summary>
        /// <returns></returns>
        [Pure]
        public Int32 CalculateHarkerHashInt32() {
            this.ThrowIfNotExists();

            return this.AsInt32().AsParallel().WithMergeOptions( mergeOptions: ParallelMergeOptions.NotBuffered )
                       .Aggregate( seed: 0, func: ( current, i ) => unchecked( current + i ) );
        }

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public IEnumerable<Int32> AsInt32( FileOptions options = FileOptions.SequentialScan ) {
            if ( !this.Exists() ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}." );
                }

                var buffer = new Byte[ sizeof( Int32 ) ];

                using ( var buffered = new BufferedStream( stream: stream, bufferSize: sizeof( Int32 ) ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: buffer.Length ).Any() ) {
                        yield return BitConverter.ToInt32( value: buffer, startIndex: 0 );
                    }
                }
            }
        }

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public IEnumerable<Int64> AsInt64( FileOptions options = FileOptions.SequentialScan ) {
            if ( !this.Exists() ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}." );
                }

                var buffer = new Byte[ sizeof( Int64 ) ];
                var length = buffer.Length;

                using ( var buffered = new BufferedStream( stream: stream, bufferSize: MathConstants.Sizes.OneGigaByte ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: length ).Any() ) {
                        yield return BitConverter.ToInt64( value: buffer, startIndex: 0 );
                    }
                }
            }
        }

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public IEnumerable<Guid> AsGuids( FileOptions options = FileOptions.SequentialScan ) {
            if ( this.Exists() == false ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}." );
                }

                var buffer = new Byte[ sizeof( Decimal ) ]; //sizeof( Decimal ) == sizeof( Guid ) right?

                using ( var buffered = new BufferedStream( stream: stream, bufferSize: sizeof( Decimal ) ) ) {
                    while ( buffered.Read( array: buffer, offset: 0, count: buffer.Length ).Any() ) {
                        yield return new Guid( b: buffer );
                    }
                }
            }
        }

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.</summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        [Pure]
        public IEnumerable<UInt64> AsUInt64( FileOptions options = FileOptions.SequentialScan ) {
            if ( this.Exists() == false ) {
                yield break;
            }

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
                }

                var bytes = new Byte[ sizeof( UInt64 ) ];

                using ( var buffered = new BufferedStream( stream: stream, bufferSize: sizeof( UInt64 ) ) ) {
                    while ( buffered.Read( array: bytes, offset: 0, count: bytes.Length ).Any() ) {
                        yield return BitConverter.ToUInt64( value: bytes, startIndex: 0 );
                    }
                }
            }
        }

        /// <summary>HarkerHash (hash-by-addition) ( )</summary>
        /// <remarks>
        ///     <para>The result is the same, No Matter What Order the bytes are read in. Right?</para>
        ///     <para>So it should be able to be read in parallel..</para>
        /// </remarks>
        /// <returns></returns>
        [Pure]
        public Int32 CalcHashInt32( Boolean inParallel = true ) {

            var result = 0;

            if ( inParallel ) {
                result = this.CalculateHarkerHashInt32();
            }
            else {
                foreach ( var b in this.AsInt32() ) {
                    unchecked {
                        result += b == 0 ? 1 : b;
                    }
                }
            }

            return result;
        }

        /// <summary>Deletes the file.</summary>
        public void Delete() {
            var fileInfo = this.GetFreshInfo();

            if ( !fileInfo.Exists ) {
                return;
            }

            if ( fileInfo.IsReadOnly ) {
                fileInfo.IsReadOnly = false;
            }

            fileInfo.Delete();
        }

        /// <summary>Returns whether the file exists.</summary>
        [DebuggerStepThrough]
        [Pure]
        public Boolean Exists() => this.GetFreshInfo().Exists;

        [NotNull]
        [Pure]
        public Folder ContainingingFolder() {
            if ( this._containingFolder != default ) {
                return this._containingFolder;
            }

            var dir = Path.GetDirectoryName( path: this.FullPath );

            if ( String.IsNullOrWhiteSpace( value: dir ) ) {

                //empty means a root-level folder (C:\) was found. Right?
                dir = Path.GetPathRoot( path: this.FullPath );
            }

            return this._containingFolder = new Folder( fullPath: dir );
        }

        /// <summary>
        ///     <para>Clone the entire IDocument to the <paramref name="destination" /> as quickly as possible.</para>
        ///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="token"></param>
        /// <param name="progress">   </param>
        /// <param name="eta">        </param>
        /// <returns></returns>
        [Pure]
        public async PooledValueTask<(Boolean success, TimeSpan timeElapsed)> Clone( [NotNull] IDocument destination, CancellationToken token,
            [CanBeNull] IProgress<Single> progress = null, [CanBeNull] IProgress<TimeSpan> eta = null ) {
            if ( destination is null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            var stopwatch = Stopwatch.StartNew();

            try {
                if ( this.Length.Any() ) {
                    if ( Uri.TryCreate( uriString: this.FullPath, uriKind: UriKind.Absolute, result: out var sourceAddress ) ) {

                        if ( sourceAddress != null ) {
                            using var client = new WebClient().Add( token: token );

                            var task = client.DownloadFileTaskAsync( address: sourceAddress, fileName: destination.FullPath );

                            if ( task != null ) {
                                await task.ConfigureAwait( continueOnCapturedContext: false );
                            }
                        }

                        return ( true, stopwatch.Elapsed );
                    }
                }
            }
            catch ( WebException exception ) {
                exception.Log();
            }

            return ( false, stopwatch.Elapsed );
        }

        /// <summary>Start copying this document to the <paramref name="destination" />.</summary>
        /// <param name="destination"></param>
        /// <param name="onProgress"> </param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        [Pure]
        [CanBeNull]
        public Task Copy( [NotNull] IDocument destination, [CanBeNull] Action<(UInt64 bytesReceived, UInt64 totalBytesToReceive)> onProgress,
            [CanBeNull] Action onCompleted ) {
            if ( destination == null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            using var webClient = new WebClient();

            webClient.DownloadProgressChanged += ( sender, args ) => {
                if ( !( args is null ) ) {
                    onProgress?.Invoke( obj: ( ( UInt64 ) args.BytesReceived, ( UInt64 ) args.TotalBytesToReceive ) );
                }
            };

            webClient.DownloadFileCompleted += ( sender, args ) => onCompleted?.Invoke();

            return webClient.DownloadFileTaskAsync( address: new Uri( uriString: this.FullPath ), fileName: destination.FullPath );
        }

        [Pure]
        public Int32? CRC32() {
            try {

                var size = this.Size();

                if ( size?.Any() == true ) {
                    using ( var fileStream = File.OpenRead( path: this.FullPath ) ) {
                        using var crc32 = new CRC32( polynomial: ( UInt32 ) size, seed: ( UInt32 ) size );

                        var result = crc32.ComputeHash( inputStream: fileStream );

                        return BitConverter.ToInt32( value: result, startIndex: 0 );
                    }
                }

                return null;
            }
            catch ( FileNotFoundException exception ) {
                exception.Break();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Break();
            }
            catch ( PathTooLongException exception ) {
                exception.Break();
            }
            catch ( IOException exception ) {
                exception.Break();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Break();
            }

            return null;
        }

        [Pure]
        [NotNull]
        public Task<Int32?> CRC32Async( CancellationToken token ) => Task.Run( function: this.CRC32, cancellationToken: token );

        /// <summary></summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [CanBeNull]
        [Pure]
        public String CRC32Hex() {
            try {
                if ( this.Exists() == false ) {
                    return null;
                }

                var size = this.Size();

                if ( !size.HasValue ) {
                    return null;
                }

                if ( size > Int32.MaxValue / 2 ) {
                    throw new InvalidOperationException( message: "File too large to convert to hex." );
                }

                using ( var fileStream = File.OpenRead( path: this.FullPath ) ) {

                    using var crc32 = new CRC32( polynomial: ( UInt32 ) size.Value, seed: ( UInt32 ) size.Value );

                    return crc32.ComputeHash( inputStream: fileStream ).Aggregate( seed: String.Empty, func: ( current, b ) => current + $"{b:X}" );
                }
            }
            catch ( FileNotFoundException exception ) {
                exception.Break();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Break();
            }
            catch ( PathTooLongException exception ) {
                exception.Break();
            }
            catch ( IOException exception ) {
                exception.Break();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Break();
            }

            return null;
        }

        /// <summary>Returns a lowercase hex-string of the hash.</summary>
        /// <returns></returns>
        [Pure]
        [NotNull]
        public Task<String> CRC32HexAsync( CancellationToken token ) => Task.Run( function: this.CRC32Hex, cancellationToken: token );

        [Pure]
        public Int64? CRC64() {
            try {
                if ( this.Exists() == false ) {
                    return null;
                }

                using ( var fileStream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    var size = this.Size();

                    if ( !size.HasValue || !size.Any() ) {
                        return null;
                    }

                    using var crc64 = new CRC64( polynomial: size.Value, seed: size.Value );

                    return BitConverter.ToInt64( value: crc64.ComputeHash( inputStream: fileStream ), startIndex: 0 );
                }
            }
            catch ( FileNotFoundException exception ) {
                exception.Break();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Break();
            }
            catch ( PathTooLongException exception ) {
                exception.Break();
            }
            catch ( IOException exception ) {
                exception.Break();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Break();
            }

            return null;
        }

        [Pure]
        [NotNull]
        public Task<Int64?> CRC64Async( CancellationToken token ) => Task.Run( function: this.CRC64, cancellationToken: token );

        /// <summary>Returns a lowercase hex-string of the hash.</summary>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public String CRC64Hex() {
            try {
                var size = this.Size();

                if ( !size.HasValue || !size.Any() || size > Int32.MaxValue / 2 ) {
                    return null;
                }

                using var crc64 = new CRC64( polynomial: size.Value, seed: size.Value );

                using ( var fileStream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                    bufferSize: MathConstants.Sizes.OneMegaByte ) ) {
                    return crc64.ComputeHash( inputStream: fileStream ).Aggregate( seed: String.Empty, func: ( current, b ) => current + $"{b:X}" );
                }
            }
            catch ( FileNotFoundException ) { }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) { }
            catch ( UnauthorizedAccessException ) { }

            return null;
        }

        /// <summary>Returns a lowercase hex-string of the hash.</summary>
        /// <returns></returns>
        [Pure]
        [NotNull]
        public Task<String> CRC64HexAsync( CancellationToken token ) => Task.Run( function: this.CRC64Hex, cancellationToken: token );

        [Pure]
        public async PooledValueTask<Boolean> IsAll( Byte number ) {
            if ( !this.IsBufferLoaded ) {
                var result = await this.LoadDocumentIntoBuffer().ConfigureAwait( continueOnCapturedContext: false );

                if ( !result.IsGood() ) {
                    return default;
                }
            }

            if ( !this.IsBufferLoaded ) {
                return default;
            }

            var buffer = this.Buffer;

            if ( buffer is null ) {
                return default;
            }

            var max = buffer.Length;

            for ( var i = 0; i < max; i++ ) {
                if ( buffer[ i ] != number ) {
                    return default;
                }
            }

            return true;
        }

        /// <summary>
        ///     <para>Downloads (replaces) the local IDocument with the specified <paramref name="source" />.</para>
        ///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [Pure]
        public async PooledValueTask<(Exception exception, WebHeaderCollection responseHeaders)> DownloadFile( [NotNull] Uri source ) {
            if ( source is null ) {
                throw new ArgumentNullException( paramName: nameof( source ) );
            }

            try {
                if ( !source.IsWellFormedOriginalString() ) {
                    return ( new Exception( message: $"Could not use source Uri '{source}'." ), null );
                }

                using var webClient = new WebClient(); //from what I've read, Dispose should NOT be being called on a WebClient???
                var task = webClient.DownloadFileTaskAsync( address: source, fileName: this.FullPath );

                if ( task != null ) {
                    await task.ConfigureAwait( continueOnCapturedContext: false );
                }

                return ( null, webClient.ResponseHeaders );
            }
            catch ( Exception exception ) {
                return ( exception, null );
            }
        }

        /// <summary>
        ///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
        /// </summary>
        [NotNull]
        [Pure]
        public String Extension() => Path.GetExtension( path: this.FullPath ).Trim().NullIfEmptyOrWhiteSpace() ?? String.Empty;

        /// <summary>
        ///     <para>Just the file's name, including the extension (no path).</para>
        /// </summary>
        /// <example>
        ///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
        /// </example>
        /// <see cref="Path.GetFileName" />
        [NotNull]
        public String FileName => Path.GetFileName( path: this.FullPath );

        /// <summary>Returns the size of the file, if it exists.</summary>
        /// <returns></returns>
        [Pure]
        public UInt64? Size() => this.Length;

        /// <summary>
        ///     <para>If the file does not exist, it is created.</para>
        ///     <para>Then the <paramref name="text" /> is appended to the file.</para>
        /// </summary>
        /// <param name="text"></param>
        [NotNull]
        public IDocument AppendText( [CanBeNull] String text ) {
            var folder = this.ContainingingFolder();

            if ( !folder.Exists() ) {
                if ( !Directory.CreateDirectory( path: folder.FullPath ).Exists ) {
                    throw new DirectoryNotFoundException( message: $"Could not create folder \"{folder.FullPath}\"." );
                }
            }

            this.SetReadOnly( value: false );

            using var writer = File.AppendText( path: this.FullPath );

            writer.WriteLine( value: text );

            return this;
        }

        /// <summary>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [Pure]
        public override Boolean Equals( Object? other ) => other is IDocument document && Equals( left: this, right: document );

        /// <summary>(file name, not contents)</summary>
        /// <returns></returns>
        [Pure]
        public override Int32 GetHashCode() => this.FullPath.GetHashCode( comparisonType: StringComparison.Ordinal );

        /// <summary>Returns the filename, without the extension.</summary>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public String JustName() => Path.GetFileNameWithoutExtension( path: this.FileName );

        /// <summary>
        ///     <para>Could we allocate a full 2GB buffer if we wanted? that'd be really nice for the <see cref="IDocument" /> copy routines...</para>
        ///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
        /// </summary>
        [Pure]
        public Int32 GetBufferSize() {
            var oursize = this.Size();

            if ( !oursize.Any() ) {

                //empty document? no buffer!
                return default;
            }

            if ( oursize <= Int32.MaxValue ) {
                return ( Int32 ) oursize;
            }

            return Int32.MaxValue;
        }

        /// <summary>Attempt to start the process.</summary>
        /// <param name="arguments"></param>
        /// <param name="verb">     "runas" is elevated</param>
        /// <param name="useShell"></param>
        /// <returns></returns>
        [Pure]
        public PooledValueTask<Process> Launch( [CanBeNull] String arguments = null, [CanBeNull] String verb = "runas", Boolean useShell = false ) {
            try {
                var info = new ProcessStartInfo( fileName: this.FullPath ) {
                    Arguments = arguments ?? String.Empty, UseShellExecute = useShell, Verb = verb
                };

                return new PooledValueTask<Process>( result: Process.Start( startInfo: info ) );
            }
            catch ( Exception exception ) {
                exception.Log();

                return default;
            }
        }

        /// <summary>Attempt to return an object Deserialized from this JSON text file.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public T LoadJSON<T>() {
            if ( this.Exists() == false ) {
                return default;
            }

            try {
                using ( var textReader = File.OpenText( path: this.FullPath ) ) {
                    using ( var jsonReader = new JsonTextReader( reader: textReader ) ) {
                        return new JsonSerializer {
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.All
                        }.Deserialize<T>( reader: jsonReader );
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        [Pure]
        [NotNull]
        public Task<T> LoadJSONAsync<T>( CancellationToken token ) => Task.Run( function: this.LoadJSON<T>, cancellationToken: token );

        /// <summary>
        ///     <para>Just the file's name, including the extension.</para>
        /// </summary>
        /// <see cref="Path.GetFileNameWithoutExtension" />
        [NotNull]
        public String Name => this.FileName;

        [Pure]
        [NotNull]
        public Task<String> ReadStringAsync() {
            using var reader = new StreamReader( path: this.FullPath );

            return reader.ReadToEndAsync();
        }

        /// <summary>
        ///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [Pure]
        public async PooledValueTask<Boolean> SameContent( [CanBeNull] Document right ) {

            if ( right is null || !this.Exists() || !right.Exists() || this.Size() != right.Size() ) {
                return default;
            }

            if ( !this.IsBufferLoaded ) {
                await this.LoadDocumentIntoBuffer().ConfigureAwait( continueOnCapturedContext: false );
            }

            if ( !right.IsBufferLoaded ) {
                await right.LoadDocumentIntoBuffer().ConfigureAwait( continueOnCapturedContext: false );
            }

            if ( this.IsBufferLoaded && !( this.Buffer is null ) && right.IsBufferLoaded && !( right.Buffer is null ) ) {
                return this.Buffer.SequenceEqual( second: right.Buffer );
            }

            return this.Length == right.Length && this.AsGuids().SequenceEqual( second: right.AsGuids() );
        }

        /// <summary>Open the file for reading and return a <see cref="StreamReader" />.</summary>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public StreamReader StreamReader() => new StreamReader( stream: File.OpenRead( path: this.FullPath ) );

        /// <summary>Open the file for writing and return a <see cref="StreamWriter" />.
        /// <para>Optionally the <paramref name="encoding" /> can be given. Defaults to <see cref="Encoding.UTF8" />.</para>
        /// <para>Optionally the buffersze can be given. Defaults to 1 MB.</para>
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public StreamWriter StreamWriter( [CanBeNull] Encoding encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte ) {
            try {
                this.ReleaseWriterStream();

                this.OpenWriter();

                if ( this.Writer == default ) {
                    return default;
                }

                return this.WriterStream = new StreamWriter( stream: this.Writer, encoding: encoding ?? Encoding.UTF8, bufferSize: ( Int32 ) bufferSize, leaveOpen: false );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            this.ReleaseWriterStream();

            return default;
        }

        /// <summary>Return this <see cref="IDocument" /> as a JSON string.</summary>
        /// <returns></returns>
        [Pure]
        public async PooledValueTask<String> ToJSON() {
            using ( var reader = new StreamReader( path: this.FullPath ) ) {
                return await reader.ReadToEndAsync().ConfigureAwait( continueOnCapturedContext: false );
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        [Pure]
        [NotNull]
        public override String ToString() => this.FullPath;

        /// <summary>
        ///     <para>Returns true if this <see cref="Document" /> no longer seems to exist.</para>
        /// </summary>
        /// <param name="delayBetweenRetries"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken token ) {
            await Task.Run( function: async () => {
                while ( !token.IsCancellationRequested && this.Exists() ) {
                    try {
                        if ( this.Exists() ) {
                            this.Delete();
                        }
                    }
                    catch ( DirectoryNotFoundException ) { }
                    catch ( PathTooLongException ) { }
                    catch ( IOException ) {

                        // IOException is thrown when the file is in use by any process.
                        await Task.Delay( delay: delayBetweenRetries, cancellationToken: token ).ConfigureAwait( continueOnCapturedContext: false );
                    }
                    catch ( UnauthorizedAccessException ) { }
                    catch ( ArgumentNullException ) { }
                }
            }, cancellationToken: token ).ConfigureAwait( continueOnCapturedContext: false );

            return this.Exists();
        }

        /// <summary>Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.</summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        [Pure]
        public async PooledValueTask<(Exception exception, WebHeaderCollection responseHeaders)> UploadFile( [NotNull] Uri destination ) {
            if ( destination is null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
            }

            if ( !destination.IsWellFormedOriginalString() ) {
                return ( new ArgumentException( message: $"Destination address '{destination.OriginalString}' is not well formed.", paramName: nameof( destination ) ), null );
            }

            try {
                using var webClient = new WebClient();

                var task = webClient.UploadFileTaskAsync( address: destination, fileName: this.FullPath );

                if ( task != null ) {
                    await task.ConfigureAwait( continueOnCapturedContext: false );
                }

                return ( null, webClient.ResponseHeaders );
            }
            catch ( Exception exception ) {
                return ( exception, null );
            }
        }

        /*
        [DebuggerStepThrough]
        [Pure] public void Refresh( Boolean throwOnError = false ) {
            this._fileAttributeData.Reset();

            var handle = NativeMethods.FindFirstFile( lpFileName: this.FullPath, lpFindData: out var data );

            if ( handle.IsInvalid ) {
                if ( throwOnError ) {
                    NativeMethods.HandleLastError( fullPath: this.FullPath );

                    return;
                }

                return;
            }

            var fileAttributeData = new FileAttributeData( findData: data );
            this._fileAttributeData.FileAttributes = fileAttributeData.FileAttributes;
            this._fileAttributeData.CreationTime = fileAttributeData.CreationTime;
            this._fileAttributeData.Exists = fileAttributeData.Exists;
            this._fileAttributeData.FileSize = fileAttributeData.FileSize;
            this._fileAttributeData.LastAccessTime = fileAttributeData.LastAccessTime;
            this._fileAttributeData.LastWriteTime = fileAttributeData.LastWriteTime;
        }
        */

        /// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="FullPath" />.</summary>
        /// <see cref="op_Implicit" />
        /// <returns></returns>
        [NotNull]
        [Pure]
        public FileInfo GetFreshInfo() => this; //use the implicit operator

        private Folder? _containingFolder;

        [CanBeNull]
        public Byte[]? Buffer { get; set; }

        public Boolean IsBufferLoaded { get; [Pure] private set; }

        [CanBeNull]
        public FileStream? Writer { get; set; }

        [CanBeNull]
        public StreamWriter? WriterStream { get; set; }

        [CanBeNull]
        private Lazy<FileSystemWatcher>? Watcher { get; }

        [CanBeNull]
        private Lazy<FileWatchingEvents>? WatchEvents { get; }

        [NotNull]
        public static String InvalidFileNameCharacters { get; } = new String( value: Path.InvalidFileNameChars );

        [NotNull]
        public static Lazy<Regex> RegexForInvalidFileNameCharacters { get; } = new Lazy<Regex>( valueFactory: () =>
            new Regex( pattern: $"[{Regex.Escape( str: InvalidFileNameCharacters )}]", options: RegexOptions.Compiled | RegexOptions.Singleline ) );

        protected Document( [NotNull] SerializationInfo info ) {
            if ( info is null ) {
                throw new ArgumentNullException( paramName: nameof( info ) );
            }

            this.FullPath = ( info.GetString( name: nameof( this.FullPath ) ) ?? throw new InvalidOperationException() ).TrimAndThrowIfBlank();
        }

        /// <summary></summary>
        /// <param name="fullPath"></param>
        /// <param name="deleteAfterClose"></param>
        /// <param name="watchFile"></param>
        /// <exception cref="InvalidOperationException">Unable to parse the given path.</exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        public Document( [NotNull] String fullPath, Boolean deleteAfterClose = false, Boolean watchFile = false ) {
            if ( String.IsNullOrWhiteSpace( value: fullPath ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fullPath ) );
            }

            this.FullPath = Path.Combine( path1: Path.GetFullPath( path: fullPath ), path2: CleanFileName( filename: Path.GetFileName( path: fullPath ) ) )
                                .TrimAndThrowIfBlank();

            if ( Uri.TryCreate( uriString: fullPath, uriKind: UriKind.Absolute, result: out var uri ) ) {
                if ( uri.IsFile ) {

                    //this.FullPath = Path.GetFullPath( uri.AbsolutePath );
                    this.FullPath = Path.GetFullPath( path: fullPath );
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
#if DEBUG
                    throw new InvalidOperationException( message: $"Could not parse \"{fullPath}\"." );
#endif
                }
            }
            else {
                throw new InvalidOperationException( message: $"Could not parse \"{fullPath}\"." );
            }

            if ( deleteAfterClose ) {
                this.PathTypeAttributes &= PathTypeAttributes.DeleteAfterClose;
                Debug.Assert( condition: this.PathTypeAttributes.HasFlag( flag: PathTypeAttributes.DeleteAfterClose ) );
            }

            if ( watchFile ) {
                this.Watcher = new Lazy<FileSystemWatcher>( valueFactory: () => new FileSystemWatcher( path: this.ContainingingFolder().FullPath, filter: this.FileName ) {
                    IncludeSubdirectories = false, EnableRaisingEvents = true
                } );

                this.WatchEvents = new Lazy<FileWatchingEvents>( valueFactory: () => new FileWatchingEvents(), isThreadSafe: false );

                var watcher = this.Watcher.Value;

                if ( watcher != null ) {
                    watcher.Created += ( sender, e ) => this.WatchEvents.Value.OnCreated?.Invoke( obj: e );
                    watcher.Changed += ( sender, e ) => this.WatchEvents.Value.OnChanged?.Invoke( obj: e );
                    watcher.Deleted += ( sender, e ) => this.WatchEvents.Value.OnDeleted?.Invoke( obj: e );
                    watcher.Renamed += ( sender, e ) => this.WatchEvents.Value.OnRenamed?.Invoke( obj: e );
                    watcher.Error += ( sender, e ) => this.WatchEvents.Value.OnError?.Invoke( obj: e );
                }
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized

        private Document() => throw new NotImplementedException( message: "Private contructor is not allowed." );

        public Document( [NotNull] String justPath, [NotNull] String filename, Boolean deleteAfterClose = false ) : this(
            fullPath: Path.Combine( path1: justPath, path2: filename ), deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] FileSystemInfo info, Boolean deleteAfterClose = false ) : this( fullPath: info.FullPath, deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] IFolder folder, [NotNull] String filename, Boolean deleteAfterClose = false ) : this( justPath: folder.FullPath, filename: filename,
            deleteAfterClose: deleteAfterClose ) { }

        public Document( [NotNull] IFolder folder, [NotNull] IDocument document, Boolean deleteAfterClose = false ) : this(
            fullPath: Path.Combine( path1: folder.FullPath, path2: document.FileName ), deleteAfterClose: deleteAfterClose ) { }

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() {

            this.Buffer = default;

            this.ReleaseWriterStream();

            this.ReleaseWriter();

            if ( this.DeleteAfterClose ) {
                this.Delete();
            }
        }

        /// <summary>Attempt to load the entire file into memory. If it throws, it throws..</summary>
        /// <returns></returns>
        public async PooledValueTask<Status> LoadDocumentIntoBuffer() {

            var size = this.Size();

            if ( !size.HasValue ) {
                return Status.Exception;
            }

            if ( !size.Value.Any() ) {
                return Status.No;
            }

            if ( size.Value > Int32.MaxValue ) {
                return Status.Exception;
            }

            var filelength = ( Int32 ) size.Value; //will we EVER have an image (or whatever) larger than Int32? (yes, probably)
            var bytesLeft = filelength;
            this.Buffer = new Byte[ filelength ];

            var offset = 0;

            await using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: filelength,
                options: FileOptions.SequentialScan ) ) {

                if ( !stream.CanRead ) {

                    //throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}" );
                    return Status.Exception;
                }

                await using ( var buffered = new BufferedStream( stream: stream ) ) {
                    Int32 bytesRead;

                    do {
                        bytesRead = await buffered.ReadAsync( buffer: this.Buffer, offset: offset, count: filelength ).ConfigureAwait( continueOnCapturedContext: false );
                        bytesLeft -= bytesRead;

                        if ( !bytesRead.Any() || !bytesLeft.Any() ) {
                            this.IsBufferLoaded = true;

                            return Status.Success;
                        }

                        offset += bytesRead;
                    } while ( bytesRead.Any() && bytesLeft.Any() );
                }
            }

            return Status.Failure;
        }

        [Pure]
        [NotNull]
        public Task<Int32> CalculateHarkerHashInt32Async() {
            this.ThrowIfNotExists();

            return Task.Run( function: this.CalculateHarkerHashInt32 );
        }

        /// <summary>"poor mans Int64 hash"</summary>
        /// <returns></returns>
        [Pure]
        public Int64 CalculateHarkerHashInt64() {
            this.ThrowIfNotExists();

            return this.AsInt64().AsParallel().AsUnordered().WithMergeOptions( mergeOptions: ParallelMergeOptions.AutoBuffered )
                       .Aggregate( seed: 0L, func: ( current, i ) => unchecked( current + i ) );
        }

        /// <summary>"poor mans Int64 hash"</summary>
        /// <returns></returns>
        [Pure]
        public Decimal CalculateHarkerHashDecimal() {
            this.ThrowIfNotExists();

            return this.AsDecimal().AsParallel().AsUnordered().WithMergeOptions( mergeOptions: ParallelMergeOptions.AutoBuffered )
                       .Aggregate( seed: 0M, func: ( current, i ) => current + i );
        }

        private void ThrowIfNotExists() {
            if ( !this.Exists() ) {
                throw new FileNotFoundException( message: $"Could find document {this.FullPath.DoubleQuote()}." );
            }
        }

        [Pure]
        [NotNull]
        public Task<Int64> CalculateHarkerHashInt64Async() {
            this.ThrowIfNotExists();

            return Task.Run( function: this.CalculateHarkerHashInt64 );
        }

        [Pure]
        public PooledValueTask<Decimal> CalculateHarkerHashDecimalAsync() {
            this.ThrowIfNotExists();

            return new PooledValueTask<Decimal>( result: this.CalculateHarkerHashDecimal() );
        }

        /// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
        /// <param name="options">Defaults to <see cref="FileOptions.SequentialScan" />.</param>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public IEnumerable<Decimal> AsDecimal( FileOptions options = FileOptions.SequentialScan ) {
            var fileLength = this.Length;

            if ( !fileLength.HasValue ) {
                yield break;
            }

            //Span<Byte> inBuffer = stackalloc Byte[ MathConstants.Sizes.OneGigaByte / sizeof( Byte ) ];
            //var inLength = inBuffer.Length;

            //Span<Decimal> outBuffer = stackalloc Decimal[ MathConstants.Sizes.OneGigaByte / sizeof( Decimal ) ];
            //var outLength = outBuffer.Length;

            using ( var stream = new FileStream( path: this.FullPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read,
                bufferSize: MathConstants.Sizes.OneGigaByte, options: options ) ) {

                if ( !stream.CanRead ) {
                    throw new NotSupportedException( message: $"Cannot read from file stream on {this.FullPath}." );
                }

                using ( var buffered = new BufferedStream( stream: stream, bufferSize: MathConstants.Sizes.OneGigaByte ) ) {

                    using ( var br = new BinaryReader( input: buffered ) ) {

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
                }
            }
        }

        private void ReleaseWriterStream() {
            using ( this.WriterStream ) {
                this.WriterStream = default;
            }
        }

        /// <summary>Opens an existing file or creates a new file for writing.
        /// <para>Should be able to read and write from <see cref="FileStream" />.</para>
        /// <para>If there is any error opening or creating the file, <see cref="Writer" /> will be default.</para>
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public FileStream OpenWriter() {
            try {
                this.ReleaseWriter();

                if ( this.Exists() ) {
                    this.SetReadOnly( value: false );
                }

                return this.Writer = new FileStream( path: this.FullPath, mode: FileMode.OpenOrCreate, access: FileAccess.ReadWrite, share: FileShare.None );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            this.ReleaseWriter();

            return default;
        }

        private void ReleaseWriter() {
            using ( this.Writer ) {
                this.Writer = default;
            }
        }

        /// <summary>Returns true if this IDocument was copied to the <paramref name="destination" />.</summary>
        /// <param name="destination"></param>
        /// <param name="progress"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        [Pure]
        public async PooledValueTask<(WebClient downloader, Boolean exists)> Copy( [NotNull] IDocument destination,
            [CanBeNull] Action<DownloadProgressChangedEventArgs> progress = null,
            [CanBeNull] Action<AsyncCompletedEventArgs, (IDocument source, IDocument destination)> onComplete = null ) {
            if ( destination is null ) {
                throw new ArgumentNullException( paramName: nameof( destination ) );
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
                await using ( File.Create( path: destination.FullPath, bufferSize: 1, options: FileOptions.None ) ) {
                    return ( default, true ); //just create an empty file?
                }
            }

            var webClient = new WebClient();
            webClient.DownloadProgressChanged += ( sender, args ) => progress?.Invoke( obj: args );
            webClient.DownloadFileCompleted += ( sender, args ) => onComplete?.Invoke( arg1: args, arg2: ( this, destination ) );
            var task = webClient.DownloadFileTaskAsync( address: this.FullPath, fileName: destination.FullPath );

            if ( task != null ) {
                await task.ConfigureAwait( continueOnCapturedContext: false );
            }

            return ( webClient, destination.Exists() && destination.Size() == this.Size() );
        }

        /// <summary>I *really* dislike filenames starting with a period. Here's looking at you java..</summary>
        /// <returns></returns>
        [Pure]
        public Boolean BadlyNamedFile() => Path.GetFileName( path: this.FullPath ).Like( right: Path.GetExtension( path: this.FullPath ) );

        [NotNull]
        [Pure]
        public static implicit operator FileInfo( [NotNull] Document document ) {
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            var info = new FileInfo( fileName: document.FullPath );

            if ( info == null ) {
                throw new NotImplementedException();
            }

            return info;
        }

        [NotNull]
        [Pure]
        public FileInfo ToFileInfo() => this;

        /// <summary>
        ///     <para>If the file does not exist, return <see cref="Status.Error" />.</para>
        ///     <para>If an exception happens, return <see cref="Status.Exception" />.</para>
        ///     <para>Otherwise, return <see cref="Status.Success" />.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Status SetReadOnly( Boolean value ) {
            var info = this.GetFreshInfo();

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

        [Pure]
        public Status TurnOnReadonly() => this.SetReadOnly( value: true );

        [Pure]
        public Status TurnOffReadonly() => this.SetReadOnly( value: false );

        public virtual void GetObjectData( [NotNull] SerializationInfo info, StreamingContext context ) =>
            info.AddValue( name: nameof( this.FullPath ), value: this.FullPath, type: typeof( String ) );

        /*
        [StructLayout( layoutKind: LayoutKind.Sequential )]
        internal class SECURITY_ATTRIBUTES {

            internal Int32 bInheritHandle;

            internal Int32 nLength;

            internal unsafe Byte* pSecurityDescriptor = null;
        }

        /// <summary>
        ///     Maybe someday rewrite <see cref="IDocument" /> to use long path names, with faster creation, opening, and saving.
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="securityAttrs"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport( dllName: "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false )]
        [Pure] private static extern SafeFileHandle CreateFile( String lpFileName, Int32 dwDesiredAccess, FileShare dwShareMode, SECURITY_ATTRIBUTES securityAttrs,
            FileMode dwCreationDisposition, Int32 dwFlagsAndAttributes, IntPtr hTemplateFile );
        */

        /// <summary>this seems to work great!</summary>
        /// <param name="address"></param>
        /// <param name="fileName"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        [Pure]
        public static async PooledValueTask<WebClient> DownloadFileTaskAsync( [NotNull] Uri address, [NotNull] String fileName,
            [CanBeNull] IProgress<(Int64 BytesReceived, Int32 ProgressPercentage, Int64 TotalBytesToReceive)> progress ) {

            if ( address is null ) {
                throw new ArgumentNullException( paramName: nameof( address ) );
            }

            if ( String.IsNullOrWhiteSpace( value: fileName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fileName ) );
            }

            var tcs = new TaskCompletionSource<Object>( state: address, creationOptions: TaskCreationOptions.RunContinuationsAsynchronously );

            void CompletedHandler( Object cs, AsyncCompletedEventArgs ce ) {
                if ( ce.UserState != tcs ) {
                    return;
                }

                if ( ce.Error != null ) {
                    tcs.TrySetException( exception: ce.Error );
                }
                else if ( ce.Cancelled ) {
                    tcs.TrySetCanceled();
                }
                else {
                    tcs.TrySetResult( result: null );
                }
            }

            void ProgressChangedHandler( Object ps, DownloadProgressChangedEventArgs pe ) {
                if ( pe?.UserState == tcs ) {
                    progress?.Report( value: ( pe.BytesReceived, pe.ProgressPercentage, pe.TotalBytesToReceive ) );
                }
            }

            var webClient = new WebClient();
            Debug.Assert( condition: !webClient.IsBusy );

            try {
                webClient.DownloadFileCompleted += CompletedHandler;
                webClient.DownloadProgressChanged += ProgressChangedHandler;
                webClient.DownloadFileAsync( address: address, fileName: fileName, userToken: tcs );

                if ( tcs.Task != null ) {
                    await tcs.Task.ConfigureAwait( continueOnCapturedContext: false );
                }
            }
            finally {
                webClient.DownloadFileCompleted -= CompletedHandler;
                webClient.DownloadProgressChanged -= ProgressChangedHandler;
            }

            return webClient;
        }

        //[Pure] private static WebClient WebClientInstance() => WebClients.Value.Value;

        /// <summary>Returns a unique file in the user's temp folder.
        /// <para>If an extension is not provided, a random extension (a <see cref="Guid" />) will be used.</para>
        /// <para><b>Note</b>: Does not create a 0-byte file like <see cref="Path.GetTempFileName" />.</para>
        /// <para>If the temp folder is not found, one attempt will be made to create it.</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        [NotNull]
        [Pure]
        public static IDocument GetTempDocument( String extension = null ) {
            if ( String.IsNullOrEmpty( value: extension ) ) {
                extension = Guid.NewGuid().ToString();
            }

            extension = extension.Trim();

            while ( extension.StartsWith( value: ".", comparisonType: StringComparison.OrdinalIgnoreCase ) ) {
                extension = extension.Substring( startIndex: 1 ).Trim();
            }

            return new Document( folder: Folder.GetTempFolder(), filename: $"{Guid.NewGuid()}.{extension}" );
        }

        /// <summary>
        ///     <para>Compares the file names (case sensitive) and file sizes for inequality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean operator !=( [CanBeNull] Document left, [CanBeNull] IDocument right ) => !Equals( left: left, right: right );

        /// <summary>
        ///     <para>Compares the file names (case sensitive) and file sizes for equality.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean operator ==( [CanBeNull] Document left, [CanBeNull] IDocument right ) => Equals( left: left, right: right );

        /// <summary>
        ///     <para>Static case sensitive comparison of the file names and file sizes for equality.</para>
        ///     <para>To compare the contents of two <see cref="IDocument" /> use <see cref="IDocument.SameContent" />.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean Equals( [CanBeNull] IDocument left, [CanBeNull] IDocument right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.FullPath.Is( right: right.FullPath ); //&& left.Size() == right.Size();
        }

        [Pure]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        internal static Boolean IsExtended( [NotNull] String path ) =>
            path.Length >= 4 && path[ index: 0 ] == PathInternal.Constants.Backslash && ( path[ index: 1 ] == PathInternal.Constants.Backslash || path[ index: 1 ] == '?' ) &&
            path[ index: 2 ] == '?' && path[ index: 3 ] == PathInternal.Constants.Backslash;

        /// <summary>Returns the path with any invalid filename characters replaced with <paramref name="replacement" />. (Defaults to <see cref="String.Empty" />.)</summary>
        /// <param name="filename"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static String CleanFileName( [NotNull] String filename, [CanBeNull] String replacement = null ) {
            var file = RegexForInvalidFileNameCharacters.Value.Replace( input: Path.GetFileName( path: filename ), replacement: replacement ?? String.Empty ).Trimmed();

            return file ?? throw new InvalidOperationException( message: $"Invalid file name {filename.DoubleQuote()}." );
        }

    }

}