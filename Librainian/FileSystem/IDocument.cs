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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "IDocument.cs" last touched on 2021-03-07 at 8:52 AM by Protiguous.

#nullable enable

namespace Librainian.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Security;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Maths;
	using Maths.Numbers;
	using PooledAwait;
	using FileInfo = Pri.LongPath.FileInfo;

	public interface IDocument : IEquatable<IDocument>, IAsyncEnumerable<Byte> {

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

		/// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
		[CanBeNull]
		Object? Tag { get; set; }

		Boolean DeleteAfterClose { get; set; }

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <example>
		///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
		/// </example>
		/// <see cref="Pri.LongPath.Path.GetFileName" />
		[NotNull]
		String FileName { get; }

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <see cref="Pri.LongPath.Path.GetFileNameWithoutExtension" />
		[NotNull]
		String Name { get; }

		/// <summary>Returns the length of the file (if it exists).</summary>
		PooledValueTask<UInt64?> Length();

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.</summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		IAsyncEnumerable<Byte> AsBytes( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		IAsyncEnumerable<Int32> AsInt32( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		IAsyncEnumerable<Int64> AsInt64( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Guid" />.</summary>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		[NotNull]
		IAsyncEnumerable<Guid> AsGuids( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.</summary>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		IAsyncEnumerable<UInt64> AsUInt64( CancellationToken cancellationToken );

		/// <summary>Deletes the file.</summary>
		PooledValueTask Delete();

		/// <summary>Returns whether the file exists.</summary>
		[Pure]
		PooledValueTask<Boolean> Exists();

		IFolder ContainingingFolder();

		/// <summary>
		///     <para>Clone the entire document to the <paramref name="destination" /> as quickly as possible.</para>
		///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="progress">   </param>
		/// <param name="eta">        </param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		PooledValueTask<(Status success, TimeSpan timeElapsed)> CloneDocument(
			[NotNull] IDocument destination,
			[NotNull] IProgress<Single> progress,
			[NotNull] IProgress<TimeSpan> eta,
			CancellationToken cancellationToken
		);

		/*
        /// <summary>Returns the <see cref="WebClient" /> if a file copy was started.</summary>
        /// <param name="destination"></param>
        /// <param name="onProgress"> </param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        (PooledValueTask? task, Exception? exception, Status Exception) Copy( [NotNull] IDocument destination,
            [NotNull] Action<(IDocument, UInt64 bytesReceived, UInt64 totalBytesToReceive)> onProgress, [NotNull] Action onCompleted );
        */

		PooledValueTask<Int32?> CRC32( CancellationToken cancellationToken );

		/// <summary>Returns a lowercase hex-string of the hash.</summary>
		/// <returns></returns>
		PooledValueTask<String?> CRC32Hex( CancellationToken cancellationToken );

		PooledValueTask<Int64?> CRC64( CancellationToken cancellationToken );

		/// <summary>Returns a lowercase hex-string of the hash.</summary>
		/// <returns></returns>
		PooledValueTask<String?> CRC64Hex( CancellationToken cancellationToken );

		/// <summary>
		///     <para>Downloads (replaces) the local document with the specified <paramref name="source" />.</para>
		///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> DownloadFile( [NotNull] Uri source );

		/// <summary>
		///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
		/// </summary>
		[NotNull]
		String Extension();

		/// <summary>Returns the size of the file, if it exists.</summary>
		/// <returns></returns>
		PooledValueTask<UInt64?> Size();

		/// <summary>
		///     <para>If the file does not exist, it is created.</para>
		///     <para>Then the <paramref name="text" /> is appended to the file.</para>
		/// </summary>
		/// <param name="text"></param>
		/// <param name="cancellationToken"></param>
		PooledValueTask<IDocument> AppendText( [NotNull] String text, CancellationToken cancellationToken );

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
		///     <para>
		///         Can we allocate a full 2GB buffer?
		///     </para>
		///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
		/// </summary>
		PooledValueTask<Int32?> GetOptimalBufferSize();

		/// <summary>Attempt to start the process.</summary>
		/// <param name="arguments"></param>
		/// <param name="verb">     "runas" is elevated</param>
		/// <param name="useShell"></param>
		/// <returns></returns>
		PooledValueTask<Process?> Launch( [CanBeNull] String? arguments = null, String verb = "runas", Boolean useShell = false );

		/// <summary>
		///     Attempt to return an object Deserialized from a JSON text file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="progress"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		PooledValueTask<(Status status, T? obj)> LoadJSON<T>( IProgress<ZeroToOne>? progress = null, CancellationToken? cancellationToken = null );

		PooledValueTask<String> ReadStringAsync();

		/// <summary>
		///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
		/// </summary>
		/// <param name="right"></param>
		/// <param name="cancellationToken"></param>
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
		PooledValueTask<Boolean> SameContent( [CanBeNull] Document? right, CancellationToken cancellationToken );

		/// <summary>Open the file for reading and return a <see cref="StreamReader" />.</summary>
		/// <returns></returns>
		StreamReader StreamReader();

		/// <summary>Open the file for writing and return a <see cref="StreamWriter" />.</summary>
		/// <returns></returns>
		Task<StreamWriter?> StreamWriter( [CanBeNull] Encoding? encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte );

		/// <summary>Return this <see cref="IDocument" /> as a JSON string.</summary>
		/// <returns></returns>
		PooledValueTask<String?> ToJSON();

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		String ToString();

		/// <summary>
		///     <para>Returns true if the <see cref="IDocument" /> no longer seems to exist.</para>
		/// </summary>
		/// <param name="delayBetweenRetries"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken cancellationToken );

		/// <summary>Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.</summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> UploadFile( [NotNull] Uri destination );

		PooledValueTask<Boolean> IsAll( Byte number, CancellationToken cancellationToken );

		/// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="Document.FullPath" />.</summary>
		/// <see cref="Document.op_Implicit" />
		/// <returns></returns>
		PooledValueTask<FileInfo> GetFreshInfo();

		PooledValueTask<FileCopyData> Copy( FileCopyData fileData, CancellationToken cancellationToken );

		//copy to dest under guid.guid, delete old dest, rename new dest, delete source

		//TODO PooledValueTask<FileCopyData> Move( FileCopyData fileData, CancellationToken cancellationToken );

		PooledValueTask<Int32> HarkerHash32( CancellationToken cancellationToken );
		PooledValueTask<Int64> HarkerHash64( CancellationToken cancellationToken );
		PooledValueTask<Decimal> HarkerHashDecimal( CancellationToken cancellationToken );

	}

}