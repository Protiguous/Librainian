// Copyright © Protiguous. All Rights Reserved.
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
// File "Path.cs" last formatted on 2020-09-26 at 5:10 AM.

#nullable enable
namespace Librainian.FileSystem.Pri.LongPath {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using JetBrains.Annotations;

	public static class Path {
		[NotNull]
		public const String LongPathPrefix = @"\\?\";

		[NotNull]
		public const String UNCLongPathPrefix = @"\\?\UNC\";

		public const Char VolumeSeparatorChar = ':';

		public static readonly Char AltDirectorySeparatorChar = System.IO.Path.AltDirectorySeparatorChar;

		public static readonly Char DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar;

		[NotNull]
		public static readonly Char[] InvalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();

		[NotNull]
		public static readonly Char[] InvalidPathChars = System.IO.Path.GetInvalidPathChars();

		public static readonly Char PathSeparator = System.IO.Path.PathSeparator;

		private static Int32 GetUncRootLength( [NotNull] this String path ) {
			var components = path.ThrowIfBlank().Split( DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries );

			return components.Length >= 2 ? $@"\\{components[0]}\{components[1]}\".Length : throw new InvalidOperationException( "Invalid path components." );
		}

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String AddLongPathPrefix( [NotNull] this String path ) {
			path = path.ThrowIfBlank();

			if ( path.StartsWith( LongPathPrefix ) ) {
				return path;
			}

			// http://msdn.microsoft.com/en-us/library/aa365247.aspx
			return path.StartsWith( @"\\" ) ? $"{UNCLongPathPrefix}{path.Substring( 2 )}" : $"{LongPathPrefix}{path}";
		}

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "filename" )]
		[NotNull]
		public static String ChangeExtension( [NotNull] this String filename, [CanBeNull] String? extension ) =>
			System.IO.Path.ChangeExtension( filename.ThrowIfBlank(), extension );

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String CheckAddLongPathPrefix( [NotNull] this String path ) {
			path = path.ThrowIfBlank();

			if ( path.StartsWith( LongPathPrefix ) ) {
				return path;
			}

			var maxPathLimit = NativeMethods.MAX_PATH;

			if ( Uri.TryCreate( path.ThrowIfBlank(), UriKind.Absolute, out var uri ) && uri.IsUnc ) {
				// What's going on here?  Empirical evidence shows that Windows has trouble dealing with UNC paths
				// longer than MAX_PATH *minus* the length of the "\\hostname\" prefix.  See the following tests:
				//  - UncDirectoryTests.TestDirectoryCreateNearMaxPathLimit
				//  - UncDirectoryTests.TestDirectoryEnumerateDirectoriesNearMaxPathLimit
				var rootPathLength = 3 + uri.Host.Length;
				maxPathLimit -= rootPathLength;
			}

			return path.Length < maxPathLimit ? path : path.AddLongPathPrefix();
		}

		/// <summary></summary>
		/// <param name="path"></param>
		/// <exception cref="ArgumentNullException">Thrown if any invalid chars found in path.</exception>
		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		[Pure]
		public static String CheckInvalidPathChars( [NotNull] this String path ) {
			path = path.ThrowIfBlank();

			if ( path.HasIllegalCharacters() ) {
				throw new ArgumentNullException( nameof( path ), "Invalid characters in path" );
			}

			return path;
		}

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path1" )]
		[NotNull]
		public static String Combine( [NotNull] this String path1, [NotNull] String path2 ) {
			path1 = path1.CheckInvalidPathChars();

			path2 = path2.CheckInvalidPathChars();

			if ( path2.Length == 0 ) {
				return path1;
			}

			if ( path1.Length == 0 || path2.IsPathRooted() ) {
				return path2;
			}

			var ch = path1[^1];

			if ( ch.IsDirectorySeparator() || ch == VolumeSeparatorChar ) {
				return path1 + path2;
			}

			return $"{path1}{DirectorySeparatorChar}{path2}";
		}

		[NotNull]
		public static String Combine( [NotNull] this String path1, [NotNull] String path2, [NotNull] String path3 ) => Combine( path1, path2 ).Combine( path3 );

		[NotNull]
		public static String Combine( [NotNull] this String path1, [NotNull] String path2, [NotNull] String path3, [NotNull] String path4 ) =>
			Combine( path1.Combine( path2 ), path3 ).Combine( path4 );

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "paths" )]
		[NotNull]
		public static String Combine( [NotNull] [ItemNotNull] params String[] paths ) {
			if ( paths == null ) {
				throw new ArgumentNullException( nameof( paths ) );
			}

			switch ( paths.Length ) {
				case 0: return String.Empty;

				case 1: {
					var z = paths[0];

					//if ( z == null ) {
					//throw new ArgumentException( "Value cannot be null or whitespace." );
					//}

					return z.CheckInvalidPathChars().ThrowIfBlank();
				}

				default: {
					var z = paths[0];

					//if ( z == null ) {
					//throw new ArgumentException( "Value cannot be null or whitespace." );
					//}

					var path = z.CheckInvalidPathChars().ThrowIfBlank();

					for ( var i = 1; i < paths.Length; ++i ) {
						path = path.Combine( paths[i] );
					}

					return path;
				}
			}
		}

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String GetDirectoryName( [NotNull] this String path ) {
			path = path.CheckInvalidPathChars();

			String? basePath = null;

			if ( !path.IsPathRooted() ) {
				basePath = System.IO.Directory.GetCurrentDirectory();
			}

			path = path.NormalizeLongPath().RemoveLongPathPrefix();
			var rootLength = path.GetRootLength();

			if ( path.Length <= rootLength ) {
				return String.Empty;
			}

			var length = path.Length;

			do { } while ( length > rootLength && !path[--length].IsDirectorySeparator() );

			if ( basePath == null ) {
				return path.Substring( 0, length );
			}

			path = path.Substring( basePath.Length + 1 );
			length = length - basePath.Length - 1;

			if ( length < 0 ) {
				length = 0;
			}

			return path.Substring( 0, length );
		}

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String GetExtension( [NotNull] this String path ) => System.IO.Path.GetExtension( path.ThrowIfBlank() );

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String GetFileName( [NotNull] this String path ) => System.IO.Path.GetFileName( path.NormalizeLongPath() );

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[CanBeNull]
		public static String? GetFileNameWithoutExtension( [CanBeNull] this String? path ) => System.IO.Path.GetFileNameWithoutExtension( path );

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String GetFullPath( [NotNull] this String path ) => path.IsPathUnc() ? path : path.NormalizeLongPath().RemoveLongPathPrefix();

		[NotNull]
		public static IEnumerable<Char> GetInvalidFileNameChars() => InvalidFileNameChars;

		[NotNull]
		public static IEnumerable<Char> GetInvalidPathChars() => InvalidPathChars;

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String GetPathRoot( [NotNull] this String path ) {
			if ( !path.IsPathRooted() ) {
				return String.Empty;
			}

			if ( !path.IsPathUnc() ) {
				path = path.NormalizeLongPath().RemoveLongPathPrefix();
			}

			return path.Substring( 0, path.GetRootLength() );
		}

		/// <summary>
		///     Returns just the unique random file name (no path) with an optional <paramref name="extension" />.
		///     <remarks>Does not create a file.</remarks>
		/// </summary>
		/// <param name="extension"></param>
		[NotNull]
		public static String GetRandomFileName( [CanBeNull] String? extension = null ) {
			if ( String.IsNullOrEmpty( extension ) ) {
				extension = $"{Guid.NewGuid():D}";
			}

			return $"{Guid.NewGuid():D}.{extension}";
		}

		public static Int32 GetRootLength( [NotNull] this String path ) {
			if ( path.IsPathUnc() ) {
				return path.GetUncRootLength();
			}

			path = path.GetFullPath().CheckInvalidPathChars();

			var rootLength = 0;
			var length = path.Length;

			switch ( length ) {
				case >= 1 when path[0].IsDirectorySeparator(): {
					rootLength = 1;

					if ( length >= 2 && path[1].IsDirectorySeparator() ) {
						rootLength = 2;
						var num = 2;

						while ( rootLength >= length ||
						        ( path[rootLength] == System.IO.Path.DirectorySeparatorChar || path[rootLength] == System.IO.Path.AltDirectorySeparatorChar ) && --num <= 0 ) {
							++rootLength;
						}
					}

					break;
				}
				case >= 2 when path[1] == System.IO.Path.VolumeSeparatorChar: {
					rootLength = 2;

					if ( length >= 3 && path[2].IsDirectorySeparator() ) {
						++rootLength;
					}

					break;
				}
			}

			return rootLength;
		}

		[NotNull]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static String GetTempFileName( [CanBeNull] String? extension = null ) => GetRandomFileName( extension );

		[NotNull]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static String GetTempPath() => System.IO.Path.GetTempPath().ThrowIfBlank();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean HasExtension( [NotNull] this String path ) => System.IO.Path.HasExtension( path.ThrowIfBlank() );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean HasIllegalCharacters( [NotNull] this String path ) => path.ThrowIfBlank().Any( InvalidPathChars.Contains );

		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean IsDirectorySeparator( this Char c ) => c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;

		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean IsPathRooted( [NotNull] this String path ) => System.IO.Path.IsPathRooted( path.ThrowIfBlank() );

		/// <summary>Normalizes path (can be longer than MAX_PATH) and adds \\?\ long path prefix</summary>
		/// <param name="path"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String NormalizeLongPath( [NotNull] this String path, [NotNull] String parameterName = "path" ) {
			if ( path.IsPathUnc() ) {
				return path.CheckAddLongPathPrefix();
			}

			var buffer = new StringBuilder( NativeMethods.MAX_LONG_PATH + 1 ); // Add 1 for NULL
			var length = NativeMethods.GetFullPathNameW( path, ( UInt32 )buffer.Capacity, buffer, IntPtr.Zero );

			switch ( length ) {
				case 0: {
					throw Common.GetExceptionFromLastWin32Error( parameterName );
				}
				case > NativeMethods.MAX_LONG_PATH: {
					throw Common.GetExceptionFromWin32Error( NativeMethods.ERROR.ERROR_FILENAME_EXCED_RANGE, parameterName );
				}
				case > 1 when buffer[0].IsDirectorySeparator() && buffer[1].IsDirectorySeparator(): {
					if ( length < 2 ) {
						throw new ArgumentException( @"The UNC path should be of the form \\server\share." );
					}

					var parts = buffer.ToString().Split( DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries );

					if ( parts.Length < 2 ) {
						throw new ArgumentException( @"The UNC path should be of the form \\server\share." );
					}

					break;
				}
			}

			return buffer.ToString().AddLongPathPrefix();
		}

		[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull( "path" )]
		[NotNull]
		public static String RemoveLongPathPrefix( [NotNull] this String path ) {
			if ( String.IsNullOrWhiteSpace( path ) || !path.StartsWith( LongPathPrefix ) ) {
				return path;
			}

			return path.StartsWith( UNCLongPathPrefix, StringComparison.Ordinal ) ? $@"\\{path.Substring( UNCLongPathPrefix.Length )}" : path.Substring( LongPathPrefix.Length );
		}

		public static Boolean TryNormalizeLongPath( [NotNull] this String path, [CanBeNull] out String? result ) {
			if ( String.IsNullOrWhiteSpace( path ) ) {
				result = null;
				return false;
			}

			try {
				result = path.NormalizeLongPath();

				return true;
			}
			catch ( ArgumentException ) { }
			catch ( PathTooLongException ) { }

			result = null;

			return false;
		}
	}
}