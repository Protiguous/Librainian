﻿// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "IOExtensions.cs" last formatted on 2020-10-20 at 7:51 AM.

#nullable enable

namespace Librainian.FileSystem {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Security;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using Collections.Extensions;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Measurement.Time;
	using OperatingSystem;
	using Parsing;
	using Directory = Pri.LongPath.Directory;
	using DirectoryInfo = Pri.LongPath.DirectoryInfo;
	using File = Pri.LongPath.File;
	using FileInfo = Pri.LongPath.FileInfo;
	using Path = Pri.LongPath.Path;

	public static class IOExtensions {
		public const Int32 FsctlSetCompression = 0x9C040;

		/// <summary>
		///     Example: WriteTextAsync( fullPath: fullPath, text: message ).Wait(); Example: await WriteTextAsync( fullPath:
		///     fullPath, text: message );
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="text">    </param>
		/// <param name="waitfor"></param>
		/// <returns></returns>
		public static async Task<(Status, Exception?)> AppendTextAsync( [NotNull] this FileInfo fileInfo, [NotNull] String text, TimeSpan? waitfor = null ) {
			try {
				var buffer = Common.DefaultEncoding.GetBytes( text ).AsMemory( 0 );
				var length = buffer.Length;

				await using var sourceStream = ReTry( () => new FileStream( fileInfo.FullPath, FileMode.Append, FileAccess.Write, FileShare.Write, length, true ), waitfor ?? Seconds.Seven, CancellationToken.None );

				if ( sourceStream is null ) {
					throw new InvalidOperationException( $"Could not open file {fileInfo.FullPath} for reading." );
				}

				await sourceStream.WriteAsync( buffer ).ConfigureAwait( false );

				await sourceStream.FlushAsync().ConfigureAwait( false );

				return (Status.Success, default( Exception? ));
			}
			catch ( UnauthorizedAccessException exception ) {
				exception.Log();
				return (Status.Exception, exception);
			}
			catch ( ArgumentNullException exception ) {
				exception.Log();
				return (Status.Exception, exception);
			}
			catch ( DirectoryNotFoundException exception ) {
				exception.Log();
				return (Status.Exception, exception);
			}
			catch ( PathTooLongException exception ) {
				exception.Log();
				return (Status.Exception, exception);
			}
			catch ( SecurityException exception ) {
				exception.Log();
				return (Status.Exception, exception);
			}
			catch ( IOException exception ) {
				exception.Log();
				return (Status.Exception, exception);
			}
		}

		/// <summary>Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.</summary>
		/// <param name="fileInfo"></param>
		/// <returns></returns>
		/// <remarks>Don't use on large files obviously..</remarks>
		[NotNull]
		public static IEnumerable<Byte> AsBytes( [NotNull] this FileInfo fileInfo ) {
			if ( fileInfo is null ) {
				throw new ArgumentNullException( nameof( fileInfo ) );
			}

			if ( !fileInfo.Exists ) {
				yield break;
			}

			using var stream = ReTry( () => new FileStream( fileInfo.FullPath, FileMode.Open, FileAccess.Read ), Seconds.Seven, CancellationToken.None );

			if ( stream is null ) {
				throw new InvalidOperationException( $"Could not open file {fileInfo.FullPath} for reading." );
			}

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file {fileInfo.FullPath}." );
			}

			using var buffered = new BufferedStream( stream );

			do {
				var b = buffered.ReadByte();

				if ( b == -1 ) {
					yield break;
				}

				yield return ( Byte )b;
			} while ( true );
		}

		/// <summary>Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.</summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static IEnumerable<Byte> AsBytes( [NotNull] this String filename ) {
			if ( String.IsNullOrWhiteSpace( filename ) ) {
				throw new ArgumentNullException( nameof( filename ) );
			}

			if ( !File.Exists( filename ) ) {
				yield break;
			}

			var stream = ReTry( () => new FileStream( filename, FileMode.Open, FileAccess.Read ), Seconds.Seven, CancellationToken.None );

			if ( stream is null ) {
				throw new InvalidOperationException( $"Could not open file {filename} for reading." );
			}

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file {filename}." );
			}

			using ( stream ) {
				using var buffered = new BufferedStream( stream );

				do {
					var b = buffered.ReadByte();

					if ( b == -1 ) {
						yield break;
					}

					yield return ( Byte )b;
				} while ( true );
			}
		}

		/// <summary>Enumerates a <see cref="FileInfo" /> as a sequence of <see cref="Byte" />.</summary>
		/// <param name="fileInfo"></param>
		/// <returns></returns>
		public static IEnumerable<UInt16> AsUInt16Array( [NotNull] this FileInfo fileInfo ) {
			// TODO this needs a unit test for endianness
			if ( fileInfo is null ) {
				throw new ArgumentNullException( nameof( fileInfo ) );
			}

			if ( !fileInfo.Exists ) {
				fileInfo.Refresh(); //check one more time

				if ( !fileInfo.Exists ) {
					yield break;
				}
			}

			using var stream = new FileStream( fileInfo.FullPath, FileMode.Open );

			if ( !stream.CanRead ) {
				throw new NotSupportedException( $"Cannot read from file {fileInfo.FullPath}" );
			}

			using var buffered = new BufferedStream( stream );

			var low = buffered.ReadByte();

			if ( low == -1 ) {
				yield break;
			}

			var high = buffered.ReadByte();

			if ( high == -1 ) {
				yield return ( ( Byte )low ).CombineBytes( 0 );

				yield break;
			}

			yield return ( ( Byte )low ).CombineBytes( ( Byte )high );
		}

		/// <summary>
		///     No guarantee of return order. Also, because of the way the operating system works (random-access), a directory
		///     may be created or deleted after a search.
		/// </summary>
		/// <param name="target">       </param>
		/// <param name="searchPattern"></param>
		/// <param name="searchOption"> Defaults to <see cref="SearchOption.AllDirectories" /></param>
		/// <returns></returns>
		[NotNull]
		[ItemNotNull]
		public static IEnumerable<DirectoryInfo> BetterEnumerateDirectories( [NotNull] this DirectoryInfo target, [CanBeNull] String? searchPattern = "*",
			SearchOption searchOption = SearchOption.AllDirectories ) {

			searchPattern ??= "*";

			var searchPath = Path.Combine( target.FullPath, searchPattern );

			using var hFindFile = NativeMethods.FindFirstFile( searchPath, out var findData );

			do {
				if ( hFindFile == null || hFindFile.IsInvalid ) {
					break;
				}

				if ( !findData.IsDirectory() ) {
					continue;
				}

				if ( findData.IsParentOrCurrent() ) {
					continue;
				}

				if ( findData.IsReparsePoint() ) {
					continue;
				}

				if ( findData.IsIgnoreFolder() ) {
					continue;
				}

				var subFolder = Path.Combine( target.FullPath, findData.cFileName );

				// Fix with @"\\?\" +System.IO.PathTooLongException?
				if ( subFolder.Length >= 260 ) {
					continue; //HACK
				}

				var subInfo = new DirectoryInfo( subFolder );

				yield return subInfo;

				switch ( searchOption ) {
					case SearchOption.AllDirectories: {

						foreach ( var info in subInfo.BetterEnumerateDirectories( searchPattern ) ) {
							yield return info;
						}

						break;
					}

					case SearchOption.TopDirectoryOnly: {
						break;
					}
					default: {
						break;
					}
				}
			} while ( NativeMethods.FindNextFile( hFindFile, out findData ) );
		}

		[NotNull]
		[ItemNotNull]
		public static IEnumerable<FileInfo> BetterEnumerateFiles( [NotNull] this DirectoryInfo target, [CanBeNull] String? searchPattern = "*.*" ) {

			searchPattern = searchPattern.NullIfEmptyOrWhiteSpace() ?? "*.*";

			var searchPath = Path.Combine( target.FullPath, searchPattern );

			using var hFindFile = NativeMethods.FindFirstFile( searchPath, out var findData );

			do {
				if ( hFindFile?.IsInvalid == true ) {
					continue;
				}

				if ( findData.IsParentOrCurrent() ) {
					continue;
				}

				if ( findData.IsReparsePoint() ) {
					continue;
				}

				if ( !findData.IsFile() ) {
					continue;
				}

				var newfName = Path.Combine( target.FullPath, findData.cFileName );

				yield return new FileInfo( newfName );
			} while ( hFindFile is not null && NativeMethods.FindNextFile( hFindFile, out findData ) );
		}

		/*
		public static List<FileInformation> FastFind( String path, String searchPattern, Boolean getFile, Boolean getDirectories, Boolean recurse, Int32? depth,
			Boolean parallel, Boolean suppressErrors, Boolean largeFetch, Boolean getHidden, Boolean getSystem, Boolean getReadOnly, Boolean getCompressed, Boolean getArchive,
			Boolean getReparsePoint, String filterMode ) {
			PriNativeMethods.FINDEX_ADDITIONAL_FLAGS additionalFlags = 0;
			if ( largeFetch ) {
				additionalFlags = PriNativeMethods.FINDEX_ADDITIONAL_FLAGS.FindFirstExLargeFetch;
			}

			// add prefix to allow for maximum path of up to 32,767 characters
			String prefixedPath;
			if ( path.StartsWith( @"\\" ) ) {
				prefixedPath = path.Replace( @"\\", Path.UNCLongPathPrefix );
			}
			else {
				prefixedPath = Path.LongPathPrefix + path;
			}

			var handle = PriNativeMethods.FindFirstFileExW( prefixedPath + @"\*", PriNativeMethods.FINDEX_INFO_LEVELS.FindExInfoBasic, out var lpFindFileData,
				PriNativeMethods.FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, additionalFlags );

			var resultList = new ConcurrentHashset<FileInformation>();
			var subDirectoryList = new ConcurrentHashset<PathInformation>();

			if ( !handle.IsInvalid ) {
				do {
					// skip "." and ".."
					if ( lpFindFileData.cFileName == "." || lpFindFileData.cFileName == ".." ) {
						continue;
					}

					// if directory...
					if ( getDirectories && recurse && ( lpFindFileData.dwFileAttributes & FileAttributes.Directory ) == FileAttributes.Directory ) {
						// ...and if we are performing a recursive search...
						// ... populate the subdirectory list
						var fullName = Path.Combine( path, lpFindFileData.cFileName );
						subDirectoryList.Add( new PathInformation( fullName ) );
					}

					// skip folders if only the getFile parameter is used
					if ( getFile && !getDirectories ) {
						if ( ( lpFindFileData.dwFileAttributes & FileAttributes.Directory ) == FileAttributes.Directory ) {
							continue;
						}
					}

					// if file matches search pattern and attribute filter, add it to the result list
					if ( MatchesFilter( lpFindFileData.dwFileAttributes, lpFindFileData.cFileName, searchPattern, getDirectories, getHidden, getSystem, getReadOnly,
						getCompressed, getArchive, getReparsePoint, filterMode ) ) {
						Int64? thisFileSize = null;
						if ( ( lpFindFileData.dwFileAttributes & FileAttributes.Directory ) != FileAttributes.Directory ) {
							thisFileSize = lpFindFileData.nFileSizeHigh * ( 2 ^ 32 ) + lpFindFileData.nFileSizeLow;
						}

						var item = new FileInformation( lpFindFileData.cFileName, new PathInformation( Path.Combine( path, lpFindFileData.cFileName ) ) ) {
							Parent = new PathInformation( path ), Attributes = lpFindFileData.dwFileAttributes, FileSize = thisFileSize,
							CreationTime = lpFindFileData.ftCreationTime.ToDateTime(), LastAccessTime = lpFindFileData.ftLastAccessTime.ToDateTime(),
							LastWriteTime = lpFindFileData.ftLastWriteTime.ToDateTime()
						};

						resultList.Add( item );
					}
				} while ( PriNativeMethods.FindNextFile( handle, out lpFindFileData ) );

				// close the file handle
				handle.Dispose();

				// handle recursive search
				if ( recurse ) {
					// handle depth of recursion
					if ( depth > 0 ) {
						if ( parallel ) {
							subDirectoryList.AsParallel().ForAll( x => {
								List<FileInformation> resultSubDirectory = FastFind( x.Path, searchPattern, getFile, getDirectories, recurse, depth - 1, false, suppressErrors,
									largeFetch, getHidden, getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode );
								resultList.AddRange( resultSubDirectory );
							} );
						}

						else {
							foreach ( var directory in subDirectoryList ) {
								foreach ( var result in FastFind( directory.Path, searchPattern, getFile, getDirectories, recurse, depth - 1, false, suppressErrors,
									largeFetch, getHidden, getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode ) ) {
									resultList.Add( result );
								}
							}
						}
					}

					// if no depth are specified
					else if ( depth == null ) {
						if ( parallel ) {
							subDirectoryList.AsParallel().ForAll( x => {
								var resultSubDirectory = new List<FileInformation>();
								resultSubDirectory = FastFind( x.Path, searchPattern, getFile, getDirectories, recurse, null, false, suppressErrors, largeFetch, getHidden,
									getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode );
								lock ( resultListLock ) {
									resultList.AddRange( resultSubDirectory );
								}
							} );
						}

						else {
							foreach ( var directory in subDirectoryList ) {
								foreach ( var result in FastFind( directory.Path, searchPattern, getFile, getDirectories, recurse, null, false, suppressErrors, largeFetch,
									getHidden, getSystem, getReadOnly, getCompressed, getArchive, getReparsePoint, filterMode ) ) {
									resultList.Add( result );
								}
							}
						}
					}
				}
			}

			// error handling
			else if ( handle.IsInvalid && !suppressErrors ) {
				Int32 hr = Marshal.GetLastWin32Error();
				if ( hr != 2 && hr != 0x12 ) {
					//throw new Win32Exception(hr);
					Console.WriteLine( "{0}:  {1}", path, new Win32Exception( hr ).Message );
				}
			}

			return resultList;
		}
		*/

		private static Boolean MatchesFilter( FileAttributes fileAttributes, String name, String searchPattern, Boolean aDirectory, Boolean aHidden, Boolean aSystem,
			Boolean aReadOnly, Boolean aCompressed, Boolean aArchive, Boolean aReparsePoint, String filterMode ) {
			// first make sure that the name matches the search pattern
			if ( NativeMethods.PathMatchSpec( name, searchPattern ) ) {
				// then we build our filter attributes enumeration
				var filterAttributes = new FileAttributes();

				if ( aDirectory ) {
					filterAttributes |= FileAttributes.Directory;
				}

				if ( aHidden ) {
					filterAttributes |= FileAttributes.Hidden;
				}

				if ( aSystem ) {
					filterAttributes |= FileAttributes.System;
				}

				if ( aReadOnly ) {
					filterAttributes |= FileAttributes.ReadOnly;
				}

				if ( aCompressed ) {
					filterAttributes |= FileAttributes.Compressed;
				}

				if ( aReparsePoint ) {
					filterAttributes |= FileAttributes.ReparsePoint;
				}

				if ( aArchive ) {
					filterAttributes |= FileAttributes.Archive;
				}

				// based on the filtermode, we match the file with our filter attributes a bit differently
				return filterMode switch {
					"Include" when ( fileAttributes & filterAttributes ) == filterAttributes => true,
					"Include" => false,
					"Exclude" when ( fileAttributes & filterAttributes ) != filterAttributes => true,
					"Exclude" => false,
					"Strict" when fileAttributes == filterAttributes => true,
					"Strict" => false,
					_ => false
				};
			}

			return false;
		}


		// --------------------------- CopyStream ---------------------------
		/// <summary>Copies data from a source stream to a target stream.</summary>
		/// <param name="source">The source stream to copy from.</param>
		/// <param name="target">The destination stream to copy to.</param>
		public static void CopyStream( [NotNull] this Stream source, [NotNull] Stream target ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			if ( !source.CanRead ) {
				throw new Exception( $"Cannot read from {nameof( source )}" );
			}

			if ( !target.CanWrite ) {
				throw new Exception( $"Cannot write to {nameof( target )}" );
			}

			const Int32 size = 0xffff;
			var buffer = new Byte[ size ];
			Int32 bytesRead;

			while ( ( bytesRead = source.Read( buffer, 0, size ) ) > 0 ) {
				target.Write( buffer, 0, bytesRead );
			}
		}

		/// <summary>Before: @"c:\hello\world". After: @"c:\hello\world\23468923475634836.extension"</summary>
		/// <param name="info">         </param>
		/// <param name="withExtension"></param>
		/// <param name="toBase">       </param>
		/// <returns></returns>
		[NotNull]
		public static FileInfo DateAndTimeAsFile( [NotNull] this DirectoryInfo info, [CanBeNull] String? withExtension, Int32 toBase = 16 ) {
			if ( info is null ) {
				throw new ArgumentNullException( nameof( info ) );
			}

			var now = Convert.ToString( DateTime.UtcNow.ToBinary(), toBase );
			var fileName = $"{now}{withExtension ?? info.Extension}";
			var path = Path.Combine( info.FullPath, fileName );

			return new FileInfo( path );
		}

		/// <summary>If the <paramref name="directoryInfo" /> does not exist, attempt to create it.</summary>
		/// <param name="directoryInfo">      </param>
		/// <param name="requestReadAccess">  </param>
		/// <param name="requestWriteAccess"> </param>
		/// <returns></returns>
		[CanBeNull]
		public static DirectoryInfo? Ensure( [NotNull] this DirectoryInfo directoryInfo, Boolean? requestReadAccess = null, Boolean? requestWriteAccess = null ) {
			if ( directoryInfo is null ) {
				throw new ArgumentNullException( nameof( directoryInfo ) );
			}

			try {
				directoryInfo.Refresh();

				if ( !directoryInfo.Exists ) {
					directoryInfo.Create();
					directoryInfo.Refresh();
				}

				if ( requestReadAccess.HasValue ) {
					directoryInfo.Refresh();
				}

				if ( requestWriteAccess.HasValue ) {
					var temp = Path.Combine( directoryInfo.FullPath, Path.GetRandomFileName() );
					File.WriteAllText( temp, "Delete Me!" );
					File.Delete( temp );
					directoryInfo.Refresh();
				}
			}
			catch ( Exception exception ) {
				exception.Log();

				return default( DirectoryInfo? );
			}

			return directoryInfo;
		}

		public static DateTime FileNameAsDateAndTime( [NotNull] this FileInfo info, DateTime? defaultValue = null ) {
			if ( info is null ) {
				throw new ArgumentNullException( nameof( info ) );
			}

			if ( null == defaultValue ) {
				defaultValue = DateTime.MinValue;
			}

			var now = defaultValue.Value;
			var fName = Path.GetFileNameWithoutExtension( info.Name );

			if ( String.IsNullOrWhiteSpace( fName ) ) {
				return now;
			}

			fName = fName.Trim();

			if ( String.IsNullOrWhiteSpace( fName ) ) {
				return now;
			}

			if ( Int64.TryParse( fName, NumberStyles.AllowHexSpecifier, null, out var data ) ) {
				return DateTime.FromBinary( data );
			}

			if ( Int64.TryParse( fName, NumberStyles.Any, null, out data ) ) {
				return DateTime.FromBinary( data );
			}

			return now;
		}

		//TODO This needs rewritten as a whole drive file searcher using tasks.

		/// <summary>
		///     Search the <paramref name="startingFolder" /> for any files matching the
		///     <paramref name="fileSearchPatterns" /> .
		/// </summary>
		/// <param name="startingFolder">    The folder to start the search.</param>
		/// <param name="fileSearchPatterns">List of patterns to search for.</param>
		/// <param name="token">      </param>
		/// <param name="onFindFile">        <see cref="Action" /> to perform when a file is found.</param>
		/// <param name="onEachDirectory">   <see cref="Action" /> to perform on each folder found.</param>
		/// <param name="searchStyle">       </param>
		public static void FindFiles( [NotNull] this DirectoryInfo startingFolder, [NotNull] IEnumerable<String> fileSearchPatterns, CancellationToken token,
			[CanBeNull] Action<FileInfo>? onFindFile = null, [CanBeNull] Action<DirectoryInfo>? onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
			if ( fileSearchPatterns is null ) {
				throw new ArgumentNullException( nameof( fileSearchPatterns ) );
			}

			if ( startingFolder is null ) {
				throw new ArgumentNullException( nameof( startingFolder ) );
			}

			try {
				var searchPatterns = fileSearchPatterns as IList<String> ?? fileSearchPatterns.ToList();

				searchPatterns.AsParallel().ForAll( searchPattern => {
					if ( token.IsCancellationRequested ) {
						return;
					}

					if ( String.IsNullOrWhiteSpace( searchPattern ) ) {
						return;
					}

					try {
						var folders = startingFolder.BetterEnumerateDirectories(  /*, SearchOption.TopDirectoryOnly*/ );

						folders.AsParallel().ForAll( folder => {
							if ( token.IsCancellationRequested ) {
								return;
							}

                            try {
								onEachDirectory?.Invoke( folder );
							}
							catch ( Exception exception ) {
								exception.Log();
							}

							if ( searchStyle == SearchStyle.FoldersFirst ) {
								folder.FindFiles( searchPatterns, token, onFindFile, onEachDirectory, SearchStyle.FoldersFirst ); //recurse
							}

							try {
								foreach ( var file in folder.BetterEnumerateFiles( searchPattern ) ) {
									try {
										if ( !token.IsCancellationRequested ) {
											onFindFile?.Invoke( file );
										}
									}
									catch ( Exception exception ) {
										exception.Log();
									}
								}
							}
							catch ( UnauthorizedAccessException ) { }
							catch ( DirectoryNotFoundException ) { }
							catch ( IOException ) { }
							catch ( SecurityException ) { }
							catch ( AggregateException aggregateException ) {
								aggregateException.Handle( exception => {
									switch ( exception ) {
										case UnauthorizedAccessException _:
										case DirectoryNotFoundException _:
										case IOException _:
										case SecurityException _:
											return true;
									}

									exception.Log();

									return false;
								} );
							}

							folder.FindFiles( searchPatterns, token, onFindFile, onEachDirectory, searchStyle ); //recurse
						} );
					}
					catch ( UnauthorizedAccessException ) { }
					catch ( DirectoryNotFoundException ) { }
					catch ( IOException ) { }
					catch ( SecurityException ) { }
					catch ( AggregateException aggregateException ) {
						aggregateException.Handle( exception => {
							switch ( exception ) {
								case UnauthorizedAccessException _:
								case DirectoryNotFoundException _:
								case IOException _:
								case SecurityException _:
									return true;
							}

							exception!.Log();

							return false;
						} );
					}
				} );
			}
			catch ( UnauthorizedAccessException ) { }
			catch ( DirectoryNotFoundException ) { }
			catch ( IOException ) { }
			catch ( SecurityException ) { }
			catch ( AggregateException aggregateException ) {
				aggregateException.Handle( exception => {
					switch ( exception ) {
						case UnauthorizedAccessException _:
						case DirectoryNotFoundException _:
						case IOException _:
						case SecurityException _:
							return true;
					}

					exception.Log();

					return false;
				} );
			}
		}

		/// <summary>
		///     <para>
		///         The code does not work properly on Windows Server 2008 or 2008 R2 or Windows 7 and Vista based systems as
		///         cluster size is always zero (GetDiskFreeSpaceW and
		///         GetDiskFreeSpace return -1 even with UAC disabled.)
		///     </para>
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		/// <see cref="http://stackoverflow.com/questions/3750590/get-size-of-file-on-disk" />
		public static UInt64? GetFileSizeOnDiskAlt( [NotNull] this FileInfo info ) {
			var result = NativeMethods.GetDiskFreeSpaceW( info.Directory.Root.FullPath, out var sectorsPerCluster, out var bytesPerSector, out _, out _ );

			if ( result == 0 ) {
				throw new Win32Exception();
			}

			var clusterSize = sectorsPerCluster * bytesPerSector;
			var losize = NativeMethods.GetCompressedFileSizeW( info.FullPath, out var sizeHigh );
			var size = ( ( Int64 )sizeHigh << 32 ) | losize;

			return ( UInt64 )( ( size + clusterSize - 1 ) / clusterSize * clusterSize );
		}

		[CanBeNull]
		public static DriveInfo? GetLargestEmptiestDrive() =>
			DriveInfo.GetDrives().AsParallel().Where( info => info.IsReady ).OrderByDescending( info => info.AvailableFreeSpace ).FirstOrDefault();

		/// <summary>
		///     Given the <paramref name="path" /> and <paramref name="searchPattern" /> pick any one file and return the
		///     <see cref="Librainian.FileSystem.Pri.LongPath.FileSystemInfo.FullPath" /> .
		/// </summary>
		/// <param name="path">         </param>
		/// <param name="searchPattern"></param>
		/// <param name="searchOption"> </param>
		/// <returns></returns>
		[NotNull]
		public static String GetRandomFile( [NotNull] String path, [NotNull] String searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
			if ( String.IsNullOrWhiteSpace( path ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
			}

			if ( String.IsNullOrWhiteSpace( searchPattern ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( searchPattern ) );
			}

			if ( !Directory.Exists( path ) ) {
				return String.Empty;
			}

			var dir = new DirectoryInfo( path );

			if ( !dir.Exists ) {
				return String.Empty;
			}

			var files = Directory.EnumerateFiles( dir.FullPath, searchPattern, searchOption );
			var pickedfile = files.OrderBy( r => Randem.Next() ).FirstOrDefault();

			if ( pickedfile != null && File.Exists( pickedfile ) ) {
				return new FileInfo( pickedfile ).FullPath;
			}

			return String.Empty;
		}

		/// <summary>Warning, this could OOM on a large folder structure.</summary>
		/// <param name="startingFolder"></param>
		/// <param name="foldersFound">  Warning, this could OOM on a *large* folder structure.</param>
		/// <param name="token">  </param>
		/// <returns></returns>
		public static Boolean GrabAllFolders( [NotNull] this Folder startingFolder, [NotNull] ConcurrentBag<String> foldersFound, CancellationToken token ) {
			if ( startingFolder is null ) {
				throw new ArgumentNullException( nameof( startingFolder ) );
			}

			if ( foldersFound is null ) {
				throw new ArgumentNullException( nameof( foldersFound ) );
			}

			try {
				if ( token.IsCancellationRequested ) {
					return false;
				}

				if ( !startingFolder.Exists() ) {
					return false;
				}

				//if ( startingFolder.Name.Like( "$OF" ) ) {return default;}

				/* quick little trick, but doesn't handle unicode properly
                var tempfile = Document.GetTempDocument();

                var bob = Windows.ExecuteCommandPrompt( $"DIR {startingFolder.FullPath} /B /S /AD > {tempfile.FullPathWithFileName}" )
                       .Result;
                bob.WaitForExit();

                var lines = File.ReadLines( tempfile.FullPathWithFileName );
                foreach ( var line in lines ) {
                    foldersFound.Add( line );
                }

                tempfile.Delete();
                */

				//foldersFound.Add( startingFolder.FullPath );

				//Parallel.ForEach( startingFolder.Info.BetterEnumerateDirectories().AsParallel(), ThreadingExtensions.CPUIntensive, info => GrabAllFolders( new Folder( info.FullPath ), foldersFound, cancellation ) );
				foreach ( var info in startingFolder.Info.EnumerateDirectories( "*.*", SearchOption.AllDirectories ) ) {
					//GrabAllFolders( new Folder( info.FullPath ), foldersFound, cancellation );
					foldersFound.Add( info.FullPath );
				}

				return true;
			}
			catch ( OutOfMemoryException ) {
				GC.Collect( 2, GCCollectionMode.Forced, true, true );
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return false;
		}

		/// <summary></summary>
		/// <param name="startingFolder">        </param>
		/// <param name="documentSearchPatterns"></param>
		/// <param name="onEachDocumentFound">   Warning, this could OOM on a large folder structure.</param>
		/// <param name="cancellation">          </param>
		/// <param name="progressFolders">       </param>
		/// <param name="progressDocuments">     </param>
		/// <returns></returns>
		public static Boolean GrabEntireTree( [NotNull] this IFolder startingFolder, [CanBeNull] IEnumerable<String>? documentSearchPatterns,
			[NotNull] Action<Document> onEachDocumentFound, [CanBeNull] IProgress<Int64>? progressFolders, [CanBeNull] IProgress<Int64>? progressDocuments,
			[NotNull] CancellationTokenSource cancellation ) {
			if ( startingFolder is null ) {
				throw new ArgumentNullException( nameof( startingFolder ) );
			}

			if ( onEachDocumentFound is null ) {
				throw new ArgumentNullException( nameof( onEachDocumentFound ) );
			}

			//if ( foldersFound is null ) {
			//    throw new ArgumentNullException( nameof( foldersFound ) );
			//}

			if ( cancellation.IsCancellationRequested ) {
				return false;
			}

			if ( !startingFolder.Exists() ) {
				return false;
			}

			//foldersFound.Add( startingFolder );
			var searchPatterns = documentSearchPatterns ?? new[] {
				"*.*"
			};

			Parallel.ForEach( startingFolder.GetFolders( "*" ).AsParallel(), folder => {
				progressFolders?.Report( 1 );
				GrabEntireTree( folder, searchPatterns, onEachDocumentFound, progressFolders, progressDocuments, cancellation );
				progressFolders?.Report( -1 );
			} );

			//var list = new List<FileInfo>();
			foreach ( var files in searchPatterns.Select( searchPattern => startingFolder.Info.EnumerateFiles( searchPattern ).OrderBy( info => Randem.Next() ) ) ) {
				foreach ( var info in files ) {
					progressDocuments?.Report( 1 );
					onEachDocumentFound( new Document( info ) );

					if ( cancellation.IsCancellationRequested ) {
						return false;
					}
				}
			}

			//if ( cancellation.HaveAnyCancellationsBeenRequested() ) {
			//    return documentsFound.Any();
			//}
			//foreach ( var folder in startingFolder.GetFolders() ) {
			//    GrabEntireTree( folder, searchPatterns, onEachDocumentFound, cancellation );
			//}

			return true;
		}

		[Pure]
		public static Boolean IsDirectory( this NativeMethods.Win32FindData data ) => data.dwFileAttributes.HasFlag( FileAttributes.Directory );

		[Pure]
		public static Boolean IsFile( this NativeMethods.Win32FindData data ) => !IsDirectory( data );

		/// <summary>Hard coded folders to skip.</summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[Pure]
		public static Boolean IsIgnoreFolder( this NativeMethods.Win32FindData data ) =>
			data.cFileName.EndsLike( "$RECYCLE.BIN" ) /*|| data.cFileName.Like( "TEMP" ) || data.cFileName.Like( "TMP" )*/ ||
			data.cFileName.Like( "System Volume Information" );

		[Pure]
		public static Boolean IsParentOrCurrent( this NativeMethods.Win32FindData data ) => data.cFileName is "." or "..";

		[Pure]
		public static Boolean IsReparsePoint( this NativeMethods.Win32FindData data ) => data.dwFileAttributes.HasFlag( FileAttributes.ReparsePoint );

		/// <summary>Open with Explorer.exe</summary>
		/// <param name="folder">todo: describe folder parameter on OpenDirectoryWithExplorer</param>
		public static Boolean OpenWithExplorer( [NotNull] this DirectoryInfo folder ) {
			if ( folder is null ) {
				throw new ArgumentNullException( nameof( folder ) );
			}

			var windows = Windows.WindowsSystem32Folder.Value;

			if ( windows is null ) {
				return false;
			}

			var proc = Process.Start( $@"{Path.Combine( windows.FullPath, "explorer.exe" )}", $" /separate /select,{folder.FullPath.DoubleQuote()} " );

			return proc switch {
				null => false,
				_ => proc.Responding
			};
		}

		/// <summary>Open with Explorer.exe</summary>
		/// <param name="folder">todo: describe folder parameter on OpenDirectoryWithExplorer</param>
		public static Boolean OpenWithExplorer( [NotNull] this Folder folder ) {
			if ( folder is null ) {
				throw new ArgumentNullException( nameof( folder ) );
			}

			var windows = Windows.WindowsSystem32Folder.Value;

			if ( windows is null ) {
				return false;
			}

			var proc = Process.Start( $@"{Path.Combine( windows.FullPath, "explorer.exe" )}", $" /separate /select,{folder.FullPath.DoubleQuote()} " );

			return proc switch {
				null => false,
				_ => proc.Responding
			};
		}

		/// <summary>Open with Explorer.exe</summary>
		public static Boolean OpenWithExplorer( [NotNull] this Document document ) {
			if ( document is null ) {
				throw new ArgumentNullException( nameof( document ) );
			}

			var windows = Windows.WindowsSystem32Folder.Value;

			if ( windows is null ) {
				return false;
			}

			var proc = Process.Start( $@"{Path.Combine( windows.FullPath, "explorer.exe" )}", $" /separate /select,{document.FullPath.DoubleQuote()} " );

			return proc switch {
				null => false,
				_ => proc.Responding
			};
		}

		/// <summary>Before: "hello.txt". After: "hello 345680969061906730476346.txt"</summary>
		/// <param name="info">        </param>
		/// <param name="newExtension"></param>
		/// <returns></returns>
		[NotNull]
		public static FileInfo PlusDateTime( [NotNull] this FileInfo info, [CanBeNull] String? newExtension = null ) {
			if ( info is null ) {
				throw new ArgumentNullException( nameof( info ) );
			}

			if ( info.Directory is null ) {
				throw new NullReferenceException( "info.directory" );
			}

			var now = Convert.ToString( DateTime.UtcNow.ToBinary(), 16 );
			var formatted = $"{Path.GetFileNameWithoutExtension( info.Name )} {now}{newExtension ?? info.Extension}";
			var path = Path.Combine( info.Directory.FullPath, formatted );

			return new FileInfo( path );
		}

		/// <summary>untested. is this written correctly? would it read from a *slow* media but not block the calling function?</summary>
		/// <param name="filePath">          </param>
		/// <param name="bufferSize">        </param>
		/// <param name="fileMissingRetries"></param>
		/// <param name="retryDelay">        </param>
		/// <returns></returns>
		[ItemNotNull]
		public static async Task<String> ReadTextAsync( [NotNull] String filePath, Int32? bufferSize = 65536, Int32? fileMissingRetries = 10, TimeSpan? retryDelay = null ) {
			if ( String.IsNullOrWhiteSpace( filePath ) ) {
				throw new ArgumentNullException( nameof( filePath ) );
			}

			bufferSize ??= 65536;

			while ( fileMissingRetries > 0 ) {
				if ( File.Exists( filePath ) ) {
					break;
				}

				await Task.Delay( retryDelay ?? Seconds.One ).ConfigureAwait( false );
				fileMissingRetries--;
			}

			if ( File.Exists( filePath ) ) {
				try {
					var sb = new StringBuilder( bufferSize.Value );
					var buffer = new Byte[ bufferSize.Value ];

					await using var sourceStream = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize.Value, true );

					Int32 numRead;

					while ( ( numRead = await sourceStream.ReadAsync( buffer, 0, buffer.Length ).ConfigureAwait( false ) ) != 0 ) {
						var text = Encoding.Unicode.GetString( buffer, 0, numRead );
						sb.Append( text );
					}

					return sb.ToString();
				}
				catch ( FileNotFoundException exception ) {
					exception.Log();
				}
			}

			return String.Empty;
		}

		/// <summary>Retry the <paramref name="ioFunction" /> if an <see cref="IOException" /> occurs.</summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="ioFunction"></param>
		/// <param name="tryFor">    </param>
		/// <param name="token">     </param>
		/// <returns></returns>
		/// <exception cref="IOException"></exception>
		[CanBeNull]
		public static TResult? ReTry<TResult>( [NotNull] this Func<TResult> ioFunction, TimeSpan tryFor, CancellationToken token ) where TResult : class {
			var stopwatch = Stopwatch.StartNew();
			TryAgain:

			if ( token.IsCancellationRequested ) {
				return default( TResult? );
			}

			try {
				return ioFunction();
			}
			catch ( IOException exception ) {
				exception.Message.Error();

				if ( stopwatch.Elapsed > tryFor ) {
					return default( TResult? );
				}

				goto TryAgain;
			}
		}

		/// <summary>Retry the <paramref name="ioFunction" /> if an <see cref="IOException" /> occurs.</summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="ioFunction"></param>
		/// <param name="tryFor">    </param>
		/// <param name="result"></param>
		/// <param name="token">     </param>
		/// <returns></returns>
		/// <exception cref="IOException"></exception>
		public static Boolean ReTry<TResult>( [NotNull] this Func<TResult> ioFunction, TimeSpan tryFor, out TResult? result, CancellationToken token ) where TResult : struct {
			var stopwatch = Stopwatch.StartNew();
			TryAgain:

			if ( token.IsCancellationRequested ) {
				result = null;
				return false;
			}

			try {
				result = ioFunction();
				return true;
			}
			catch ( IOException exception ) {
				exception.Message.Error();

				if ( stopwatch.Elapsed > tryFor ) {
					result = null;
					return false;
				}

				goto TryAgain;
			}
		}

		/// <summary>
		///     <para>performs a byte by byte file comparison</para>
		/// </summary>
		/// <param name="left"> </param>
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
		public static Boolean SameContent( [CanBeNull] this FileInfo? left, [CanBeNull] FileInfo? right ) {
			if ( left is null || right is null ) {
				return false;
			}

			if ( !left.Exists || !right.Exists ) {
				return false;
			}

			if ( left.Length != right.Length ) {
				return false;
			}

			var lba = left.AsBytes(); //.ToArray();
			var rba = right.AsBytes(); //.ToArray();

			return lba.SequenceEqual( rba );
		}

		/// <summary>
		///     <para>performs a byte by byte file comparison</para>
		/// </summary>
		/// <param name="leftFileName"> </param>
		/// <param name="rightFileName"></param>
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
		public static Boolean SameContent( [CanBeNull] this String? leftFileName, [CanBeNull] String? rightFileName ) {
			if ( leftFileName is null || rightFileName is null ) {
				return false;
			}

			if ( !File.Exists( leftFileName ) ) {
				return false;
			}

			if ( !File.Exists( rightFileName ) ) {
				return false;
			}

			if ( leftFileName.Length != rightFileName.Length ) {
				return false;
			}

			var lba = leftFileName.AsBytes().ToArray();
			var rba = rightFileName.AsBytes().ToArray();

			return lba.SequenceEqual( rba );
		}

		/// <summary>
		///     <para>performs a byte by byte file comparison</para>
		/// </summary>
		/// <param name="left"> </param>
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
		public static Boolean SameContent( [CanBeNull] this Document? left, [CanBeNull] FileInfo? right ) {
			if ( left == null || right == null ) {
				return false;
			}

			if ( left.Exists() == false ) {
				return false;
			}

			var leftLength = left.Length;

			if ( !leftLength.HasValue ) {
				return false;
			}

			if ( !right.Exists ) {
				right.Refresh();

				if ( !right.Exists ) {
					return false;
				}
			}

			var rightLength = ( UInt64 )right.Length;

			if ( !rightLength.Any() ) {
				return false;
			}

			return leftLength.Value == rightLength && left.AsBytes().SequenceEqual( right.AsBytes() );
		}

		/// <summary>
		///     <para>performs a byte by byte file comparison</para>
		/// </summary>
		/// <param name="right"> </param>
		/// <param name="left"></param>
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
		public static Boolean SameContent( [CanBeNull] this FileInfo? left, [CanBeNull] Document? right ) {
			if ( left == default( Object ) || right == default( IDocument? ) ) {
				return false;
			}

			if ( !left.Exists ) {
				left.Refresh();

				if ( !left.Exists ) {
					return false;
				}
			}

			var rightLength = ( UInt64 )left.Length;

			if ( !rightLength.Any() ) {
				return false;
			}

			if ( right.Exists() == false ) {
				return false;
			}

			var leftLength = right.Length;

			if ( !leftLength.HasValue ) {
				return false;
			}

			return leftLength.Value == rightLength && right.AsBytes().SequenceEqual( left.AsBytes() );
		}

		/// <summary>Search all possible drives for any files matching the <paramref name="fileSearchPatterns" /> .</summary>
		/// <param name="fileSearchPatterns">List of patterns to search for.</param>
		/// <param name="token">      </param>
		/// <param name="onFindFile">        <see cref="Action" /> to perform when a file is found.</param>
		/// <param name="onEachDirectory">   <see cref="Action" /> to perform on each folder found.</param>
		/// <param name="searchStyle">       </param>
		public static void SearchAllDrives( [NotNull] this IEnumerable<String> fileSearchPatterns, CancellationToken token, [CanBeNull] Action<FileInfo>? onFindFile = null,
			[CanBeNull] Action<DirectoryInfo>? onEachDirectory = null, SearchStyle searchStyle = SearchStyle.FilesFirst ) {
			if ( fileSearchPatterns is null ) {
				throw new ArgumentNullException( nameof( fileSearchPatterns ) );
			}

			try {
				DriveInfo.GetDrives().AsParallel().WithDegreeOfParallelism( 26 ).WithExecutionMode( ParallelExecutionMode.ForceParallelism ).ForAll( drive => {
					if ( !drive.IsReady || drive.DriveType == DriveType.NoRootDirectory || !drive.RootDirectory.Exists ) {
						return;
					}

					$"Scanning [{drive.VolumeLabel}]".Info();
					var root = new DirectoryInfo( drive.RootDirectory.FullName );
					root.FindFiles( fileSearchPatterns, token, onFindFile, onEachDirectory, searchStyle );
				} );
			}
			catch ( UnauthorizedAccessException ) { }
			catch ( DirectoryNotFoundException ) { }
			catch ( IOException ) { }
			catch ( SecurityException ) { }
			catch ( AggregateException exception ) {
				exception.Handle( ex => {
					switch ( ex ) {
						case UnauthorizedAccessException _:
						case DirectoryNotFoundException _:
						case IOException _:
						case SecurityException _: {
							return true;
						}
					}

					ex.Log();

					return false;
				} );
			}
		}

		[NotNull]
		public static String SimplifyFileName( [NotNull] this Document document ) {
			if ( document is null ) {
				throw new ArgumentNullException( nameof( document ) );
			}

			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension( document.FileName );

			TryAgain:

			//check for a double extension (image.jpg.tif),
			//remove the fake .tif extension?
			//OR remove the fake .jpg extension?
			if ( !Path.GetExtension( fileNameWithoutExtension ).IsNullOrEmpty() ) {
				// ReSharper disable once AssignNullToNotNullAttribute
				fileNameWithoutExtension = Path.GetFileNameWithoutExtension( fileNameWithoutExtension );

				goto TryAgain;
			}

			//TODO we have the document, see if we can just chop off down to a nonexistent filename.. just get rid of (3) or (2) or (1)

			var splitIntoWords = fileNameWithoutExtension.Split( new[] {
				' '
			}, StringSplitOptions.RemoveEmptyEntries ).ToList();

			if ( splitIntoWords.Count >= 2 ) {
				var list = splitIntoWords.ToList();
				var lastWord = list.TakeLast();

				//check for a copy indicator
				if ( lastWord.Like( "Copy" ) ) {
					fileNameWithoutExtension = list.ToStrings( " " );
					fileNameWithoutExtension = fileNameWithoutExtension.Trim();

					goto TryAgain;
				}

				//check for a trailing "-" or "_"
				if ( lastWord.Like( "-" ) || lastWord.Like( "_" ) ) {
					fileNameWithoutExtension = list.ToStrings( " " );
					fileNameWithoutExtension = fileNameWithoutExtension.Trim();

					goto TryAgain;
				}

				//check for duplicate "word word" at the string's ending.
				var nextlastWord = list.TakeLast();

				if ( lastWord.Like( nextlastWord ) ) {
					fileNameWithoutExtension = list.ToStrings( " " ) + " " + lastWord;
					fileNameWithoutExtension = fileNameWithoutExtension.Trim();

					goto TryAgain;
				}
			}

			return $"{fileNameWithoutExtension}{document.Extension()}";
		}

		[NotNull]
		public static IEnumerable<String> ToPaths( [NotNull] this DirectoryInfo directoryInfo ) {
			if ( directoryInfo is null ) {
				throw new ArgumentNullException( nameof( directoryInfo ) );
			}

			return directoryInfo.ToString().Split( Path.DirectorySeparatorChar );
		}

		[NotNull]
		public static MemoryStream TryCopyStream( [NotNull] String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open,
			FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {
			if ( String.IsNullOrWhiteSpace( filePath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( filePath ) );
			}

			//TODO
			TryAgain:
			var memoryStream = new MemoryStream();

			try {
				if ( File.Exists( filePath ) ) {
					using var fileStream = File.Open( filePath, fileMode, fileAccess, fileShare );

					var length = ( Int32 )fileStream.Length;

					if ( length > 0 ) {
						fileStream.CopyTo( memoryStream, length ); //BUG int-long possible issue.
						memoryStream.Seek( 0, SeekOrigin.Begin );
					}
				}
			}
			catch ( IOException ) {
				// IOExcception is thrown if the file is in use by another process.
				if ( bePatient ) {
					if ( !Thread.Yield() ) {
						Thread.Sleep( 0 );
					}

					goto TryAgain;
				}
			}

			return memoryStream;
		}

		[DebuggerStepThrough]
		public static Boolean TryGetFolderFromPath( this TrimmedString path, [CanBeNull] out DirectoryInfo? directoryInfo, [CanBeNull] out Uri? uri ) =>
			TryGetFolderFromPath( path.Value, out directoryInfo, out uri );

		[DebuggerStepThrough]
		public static Boolean TryGetFolderFromPath( [CanBeNull] this String? path, [CanBeNull] out DirectoryInfo? directoryInfo, [CanBeNull] out Uri? uri ) {
			directoryInfo = null;
			uri = null;

			try {
				if ( String.IsNullOrWhiteSpace( path ) ) {
					return false;
				}

				if ( Uri.TryCreate( path, UriKind.Absolute, out uri ) ) {
					directoryInfo = new DirectoryInfo( uri.LocalPath );

					return true;
				}

				directoryInfo = new DirectoryInfo( path ); //try it anyways

				return true;
			}
			catch ( ArgumentException ) { }
			catch ( UriFormatException ) { }
			catch ( SecurityException ) { }
			catch ( PathTooLongException ) { }
			catch ( InvalidOperationException ) { }

			return false;
		}

		/// <summary>Returns a temporary <see cref="Document" /> (but does not create the file in the file system).</summary>
		/// <param name="folder">   </param>
		/// <param name="extension">If no extension is given, a random <see cref="Guid" /> is used.</param>
		/// <param name="deleteAfterClose"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		[NotNull]
		public static Document TryGetTempDocument( [NotNull] this Folder folder, [CanBeNull] String? extension = null, Boolean deleteAfterClose = false ) {
			if ( folder is null ) {
				throw new ArgumentNullException( nameof( folder ) );
			}

			var randomFileName = Guid.NewGuid().ToString();
			extension = extension.Trimmed() ?? Guid.NewGuid().ToString();

			if ( !extension.StartsWith( ".", StringComparison.OrdinalIgnoreCase ) ) {
				extension = $".{extension}";
			}

			return new Document( folder.FullPath, $"{randomFileName}{extension}", deleteAfterClose );
		}

		/// <summary>Tries to open a file, with a user defined number of attempt and Sleep delay between attempts.</summary>
		/// <param name="filePath">  The full file path to be opened</param>
		/// <param name="fileMode">  Required file mode enum value(see MSDN documentation)</param>
		/// <param name="fileAccess">Required file access enum value(see MSDN documentation)</param>
		/// <param name="fileShare"> Required file share enum value(see MSDN documentation)</param>
		/// <returns>
		///     A valid FileStream object for the opened file, or null if the File could not be opened after the required
		///     attempts
		/// </returns>
		[CanBeNull]
		public static FileStream? TryOpen( [CanBeNull] String? filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare ) {
			//TODO
			try {
				return File.Open( filePath, fileMode, fileAccess, fileShare );
			}
			catch ( IOException ) {
				// IOExcception is thrown if the file is in use by another process.
			}

			return default( FileStream? );
		}

		[CanBeNull]
		public static FileStream? TryOpenForReading( [NotNull] String filePath, Boolean bePatient = true, FileMode fileMode = FileMode.Open,
			FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite ) {
			if ( String.IsNullOrWhiteSpace( filePath ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( filePath ) );
			}

			//TODO
			TryAgain:

			try {
				if ( File.Exists( filePath ) ) {
					return File.Open( filePath, fileMode, fileAccess, fileShare );
				}
			}
			catch ( IOException ) {
				// IOExcception is thrown if the file is in use by another process.
				if ( !bePatient ) {
					return default( FileStream? );
				}

				if ( !Thread.Yield() ) {
					Thread.Sleep( 0 );
				}

				goto TryAgain;
			}

			return default( FileStream? );
		}

		[CanBeNull]
		public static FileStream? TryOpenForWriting( [CanBeNull] String? filePath, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write,
			FileShare fileShare = FileShare.ReadWrite ) {
			//TODO
			try {
				return File.Open( filePath, fileMode, fileAccess, fileShare );
			}
			catch ( IOException ) {
				// IOExcception is thrown if the file is in use by another process.
			}

			return default( FileStream? );
		}

		public static Int32? TurnOnCompression( [NotNull] this FileInfo info ) {
			if ( info is null ) {
				throw new ArgumentNullException( nameof( info ) );
			}

			if ( !info.Exists ) {
				info.Refresh();

				if ( !info.Exists ) {
					return default( Int32? );
				}
			}

			var lpBytesReturned = 0;
			Int16 compressionFormatDefault = 1;

			using var fileStream = File.Open( info.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None );

			var success = false;

			try {
				fileStream.SafeFileHandle.DangerousAddRef( ref success );

				NativeMethods.DeviceIoControl( fileStream.SafeFileHandle.DangerousGetHandle(), FsctlSetCompression, ref compressionFormatDefault, sizeof( Int16 ),
					IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero );
			}
			finally {
				fileStream.SafeFileHandle.DangerousRelease();
			}

			return lpBytesReturned;
		}

		/// <summary>(does not create path)</summary>
		/// <param name="basePath"></param>
		/// <param name="d">       </param>
		/// <returns></returns>
		[NotNull]
		public static DirectoryInfo WithShortDatePath( [NotNull] this DirectoryInfo basePath, DateTime d ) {
			var path = Path.Combine( basePath.FullPath, d.Year.ToString(), d.DayOfYear.ToString(), d.Hour.ToString() );

			return new DirectoryInfo( path );
		}

		[Serializable]
		public class FileInformation {
			public FileAttributes Attributes;
			public DateTime? CreationTime;
			public Int64? FileSize;
			public DateTime? LastAccessTime;
			public DateTime? LastWriteTime;

			[CanBeNull]
			public PathInformation? Parent;

			[NotNull]
			public PathInformation Path;

			public FileInformation( [NotNull] String name, [NotNull] PathInformation path ) {
				this.Path = path;
				this.Name = name;
			}

			[NotNull]
			public String Name { get; }
		}

		[Serializable]
		public class PathInformation {
			public FileAttributes Attributes;
			public DateTime CreationTime;
			public DateTime LastAccessTime;
			public DateTime LastWriteTime;

			[CanBeNull]
			public PathInformation? Parent;

			public PathInformation( [NotNull] String path ) => this.Path = path;

			[NotNull]
			public String Path { get; }
		}

		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean IsExtended( [NotNull] this String path ) =>
			path.Length >= 4 && path[ 0 ] == PathInternal.Constants.Backslash && ( path[ 1 ] == PathInternal.Constants.Backslash || path[ 1 ] == '?' ) && path[ 2 ] == '?' &&
			path[ 3 ] == PathInternal.Constants.Backslash;

	}
}