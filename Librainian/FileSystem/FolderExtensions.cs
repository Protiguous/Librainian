// Copyright � Protiguous. All Rights Reserved.
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
// File "FolderExtensions.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.


namespace Librainian.FileSystem;

using ComputerSystem.Devices;
using Exceptions;
using Logging;
using Parsing;
using PooledAwait;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public static class FolderExtensions {
	/*
    public static Char[] InvalidPathChars {
        get;
    } = Path.GetInvalidPathChars();
    */

	/*
    [NeedsTesting]
    public static String CleanupForFolder([NeedsTesting] this String foldername) {
        if (String.IsNullOrWhiteSpace(foldername)) {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(foldername));
        }

        var sb = new StringBuilder(foldername.Length, UInt16.MaxValue / 2);

        foreach (var c in foldername) {
            if (!InvalidPathChars.Contains(c)) {
                sb.Append(c);
            }
        }

//         var idx = foldername.IndexOfAny( InvalidPathChars );

        //while ( idx.Any() ) {
//             if ( idx.Any() ) {
//                 foldername = foldername.Remove( idx, 1 );
//             }
        //	idx = foldername.IndexOfAny( InvalidPathChars );
        //}
//         return foldername.Trim();

        return sb.ToString().Trim();
    }
    */

	/// <summary>Returns a list of all files copied.</summary>
	/// <param name="sourceFolder">                 </param>
	/// <param name="destinationFolder">            </param>
	/// <param name="searchPatterns">               </param>
	/// <param name="overwriteDestinationDocuments"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static async Task<IEnumerable<DocumentCopyStatistics>> CopyFiles(
		this Folder sourceFolder,
		Folder destinationFolder,
		IEnumerable<String>? searchPatterns,
		Boolean overwriteDestinationDocuments,
		CancellationToken cancellationToken
	) {
		if ( sourceFolder is null ) {
			throw new ArgumentEmptyException( nameof( sourceFolder ) );
		}

		if ( destinationFolder is null ) {
			throw new ArgumentEmptyException( nameof( destinationFolder ) );
		}

		var documentCopyStatistics = new ConcurrentDictionary<IDocumentFile, DocumentCopyStatistics>();

		$"Searching for documents in {sourceFolder.FullPath.DoubleQuote()}.".Verbose();
		var sourceFiles = sourceFolder.EnumerateDocuments( searchPatterns ?? new[] {
			"*.*"
		}, cancellationToken );

		//TODO Create a better task manager instead of Parallel.ForEach.
		//TODO Limit # of active copies happening (disk saturation, fragmentation, thrashing).
		//TODO Check for stalled/failed copies, etc..
		var fileCopyManager = new FileCopyManager();

		await fileCopyManager.LoadFilesToBeCopied( sourceFiles, destinationFolder, overwriteDestinationDocuments, cancellationToken ).ConfigureAwait( false );

		await foreach ( var sourceFileTask in fileCopyManager.FilesToBeCopied().WithCancellation( cancellationToken ).ConfigureAwait( false ) ) {
			var fileCopyData = await sourceFileTask.ConfigureAwait( false );

			var dcs = new DocumentCopyStatistics {
				DestinationDocument = fileCopyData.Destination,
				DestinationDocumentCRC64 = default( String? ),
				SourceDocument = fileCopyData.Source,
				SourceDocumentCRC64 = default( String? )
			};

			if ( ( fileCopyData.WhenCompleted != null ) && ( fileCopyData.WhenStarted != null ) ) {
				dcs.TimeTaken = fileCopyData.WhenCompleted.Value - fileCopyData.WhenStarted.Value;
			}

			if ( fileCopyData.BytesCopied != null ) {
				dcs.BytesCopied = fileCopyData.BytesCopied.Value;
			}

			documentCopyStatistics[fileCopyData.Source] = dcs;
		}

		//        Parallel.ForEach( sourceFiles.AsParallel(), CPU.HalfOfCPU /*disk != cpu*/, async sourceDocument => {
		//if ( sourceDocument is null ) {
		//	return;
		//}
		//try {
		//	var beginTime = DateTime.UtcNow;

		//	var statistics = new DocumentCopyStatistics {
		//		TimeStarted = beginTime,
		//		SourceDocument = sourceDocument
		//	};

		//	if ( crc ) {
		//		statistics.SourceDocumentCRC64 = await sourceDocument.CRC64Hex( token );
		//	}

		//	var destinationDocument = new Document( destinationFolder, sourceDocument.FileName );

		//	if ( overwriteDestinationDocuments && destinationDocument.Exists() ) {
		//		destinationDocument.Delete();
		//	}

		//	//File.Copy( sourceDocument.FullPath, destinationDocument.FullPath );
		//	await sourceDocument.Copy( destinationDocument, token, progressChanged, onEachComplete ).ConfigureAwait( false );

		//	if ( crc ) {
		//		statistics.DestinationDocumentCRC64 = await destinationDocument.CRC64Hex( token ).ConfigureAwait( false );
		//	}

		//	var endTime = DateTime.UtcNow;

		//	if ( destinationDocument.Exists() == false ) {
		//		return;
		//	}

		//	statistics.BytesCopied = destinationDocument.Size().GetValueOrDefault( 0 );

		//	if ( crc ) {
		//		statistics.BytesCopied *= 2;
		//	}

		//	statistics.TimeTaken = endTime - beginTime;
		//	statistics.DestinationDocument = destinationDocument;
		//	documentCopyStatistics.TryAdd( statistics );
		//}
		//catch ( Exception ) {
		//	//swallow any errors
		//}
		//} );

		return documentCopyStatistics.Values;
	}

	public static async IAsyncEnumerable<IFolder> FindFolder( this String folderName, [EnumeratorCancellation] CancellationToken cancellationToken ) {
		if ( folderName is null ) {
			throw new ArgumentEmptyException( nameof( folderName ) );
		}

		//First check across all known drives.
		var found = false;

		await foreach ( var drive in Disk.GetDrives( cancellationToken ).ConfigureAwait( false ) ) {
			var path = Path.Combine( drive.RootDirectory, folderName );
			var asFolder = new Folder( path );

			if ( await asFolder.Exists( cancellationToken ).ConfigureAwait( false ) ) {
				found = true;

				yield return asFolder;
			}
		}

		if ( found ) {
			yield break;
		}

		//Next, check subfolders, beginning with the first drive.

		await foreach ( var drive in Disk.GetDrives( cancellationToken ).ConfigureAwait( false ) ) {
			await foreach ( var folder in drive.EnumerateFolders( cancellationToken ).ConfigureAwait( false ) ) {
				var parts = SplitPath( ( Folder )folder ); //TODO fix this

				if ( parts.Any( s => s.Like( folderName ) ) ) {
					found = true;

					yield return folder;
				}
			}
		}

		if ( !found ) { }
	}

	/// <summary><see cref="PathSplitter" />.</summary>
	/// <param name="path"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static IEnumerable<String> SplitPath( String path ) {
		if ( String.IsNullOrWhiteSpace( path ) ) {
			throw new ArgumentEmptyException( nameof( path ) );
		}

		return path.Split( Folder.FolderSeparatorChar, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
	}

	/// <summary><see cref="PathSplitter" />.</summary>
	/// <param name="info"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static IEnumerable<String> SplitPath( this DirectoryInfo info ) {
		if ( info is null ) {
			throw new ArgumentEmptyException( nameof( info ) );
		}

		return SplitPath( info.FullName );
	}

	/// <summary>
	///     <para>Returns true if the <see cref="DocumentFile" /> no longer seems to exist.</para>
	///     <para>Returns null if existence cannot be determined.</para>
	/// </summary>
	/// <param name="folder"></param>
	/// <param name="tryFor"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="ArgumentEmptyException"><paramref name="folder"/></exception>
	public static async PooledValueTask<Boolean?> TryDeleting( this Folder folder, TimeSpan tryFor, CancellationToken cancellationToken ) {
		if ( folder == null ) {
			throw new ArgumentEmptyException( nameof( folder ) );
		}

		var stopwatch = Stopwatch.StartNew();
		TryAgain:

		try {
			if ( !await folder.Exists( cancellationToken ).ConfigureAwait( false ) ) {
				return true;
			}

			Directory.Delete( folder.FullPath );

			return !Directory.Exists( folder.FullPath );
		}
		catch ( DirectoryNotFoundException ) { }
		catch ( PathTooLongException ) { }
		catch ( IOException ) {
			// IOExcception is thrown when the file is in use by any process.
			if ( stopwatch.Elapsed <= tryFor ) {
				Thread.Yield();

				goto TryAgain;
			}
		}
		catch ( UnauthorizedAccessException ) { }
		catch ( ArgumentEmptyException ) { }
		finally {
			stopwatch.Stop();
		}

		return default( Boolean? );
	}
}