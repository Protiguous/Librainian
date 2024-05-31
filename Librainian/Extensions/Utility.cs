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
// File "Utility.cs" last formatted on 2022-03-14 at 3:25 AM by Protiguous.


namespace Librainian.Extensions;

using System;
using System.Security;
using Logging;
using Maths;
using Utilities;

public static class Utility {
	//private static ReaderWriterLockSlim ConsoleOutputSynch { get; } = new( LockRecursionPolicy.SupportsRecursion );

	private static DummyXMLResolver DummyXMLResolver { get; } = new();

	/// <summary>Output the <paramref name="text" /> at the end of the current <see cref="Console" /> line.</summary>
	/// <param name="text">   </param>
	/// <param name="yOffset"></param>
	[NeedsTesting]
	public static void AtEndOfLine( this String? text, Int32 yOffset = 0 ) {
		if ( String.IsNullOrEmpty( text ) ) {
			return;
		}

		//ConsoleOutputSynch.EnterUpgradeableReadLock();
		(var oldTop, var oldLeft) = (Console.CursorTop, Console.CursorLeft);

		try {
			//ConsoleOutputSynch.EnterWriteLock();
			Console.CursorVisible = false;
			yOffset = oldTop + yOffset;

			if ( yOffset < 0 ) {
				yOffset = 0;
			}

			if ( yOffset >= Console.WindowHeight ) {
				yOffset = Console.WindowHeight;
			}

			Console.SetCursorPosition( Console.WindowWidth - ( text.Length + 2 ), yOffset );
			Console.Write( text );
			Console.SetCursorPosition( oldLeft, oldTop );
			Console.CursorVisible = true;
		}
		catch ( ArgumentOutOfRangeException exception ) {
			exception.Log();
		}
		catch ( IOException exception ) {
			exception.Log();
		}
		catch ( SecurityException exception ) {
			exception.Log();
		}
	}

	/// <summary>
	///     Copy from one stream to another.
	///     Example:
	///     using(var stream = response.GetResponseStream())
	///     using(var ms = new RecyclableMemoryStream())
	///     {
	///     stream.CopyTo(ms);
	///     // Do something with copied data
	///     }
	/// </summary>
	/// <param name="fromStream">From stream.</param>
	/// <param name="toStream">To stream.</param>
	/// <exception cref="ArgumentNullException"><paramref name="fromStream" /> is <c>null</c>.</exception>
	public static void CopyTo( this Stream fromStream!!, Stream toStream!! ) {
		var bytes = new Byte[UInt16.MaxValue];
		Int32 dataRead;
		do {
			dataRead = fromStream.Read( bytes, 0, bytes.Length );
			if ( dataRead.Any() ) {
				toStream.Write( bytes, 0, dataRead );
			}
		} while ( dataRead.Any() );
	}

	/// <summary>
	///     Copy from one stream to another.
	///     Example:
	///     using(var stream = response.GetResponseStream())
	///     using(var ms = new RecyclableMemoryStream())
	///     {
	///     stream.CopyToAsync(ms);
	///     // Do something with copied data
	///     }
	/// </summary>
	/// <param name="fromStream">From stream.</param>
	/// <param name="toStream">To stream.</param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="ArgumentNullException"><paramref name="fromStream" /> is <c>null</c>.</exception>
	[NeedsTesting]
	public static async Task CopyToAsync( this Stream fromStream!!, Stream toStream!!, CancellationToken cancellationToken ) {
		var memory = new Byte[UInt16.MaxValue].AsMemory();

		while ( ( await fromStream.ReadAsync( memory, cancellationToken ).ConfigureAwait( false ) ).Any() ) {
			await toStream.WriteAsync( memory, cancellationToken ).ConfigureAwait( false );
		}
	}

	public static void OnSet<T>( this EventHandler<T> @event, Object sender, T e ) where T : EventArgs => throw new NotImplementedException();

	public static void Spin( String? text ) {
		var oldTop = Console.CursorTop;
		var oldLeft = Console.CursorLeft;
		Console.Write( text );
		Console.SetCursorPosition( oldLeft, oldTop );
	}

	public static void TopRight( String? text ) {
		if ( String.IsNullOrEmpty( text ) ) {
			return;
		}

		//ConsoleOutputSynch.EnterUpgradeableReadLock();
		var oldTop = Console.CursorTop;
		var oldLeft = Console.CursorLeft;

		//ConsoleOutputSynch.EnterWriteLock();
		Console.CursorVisible = false;
		Console.SetCursorPosition( Console.WindowWidth - ( text.Length + 2 ), 0 );
		Console.Write( text );
		Console.SetCursorPosition( oldLeft, oldTop );
		Console.CursorVisible = true;
	}

	public static void WriteColor( this String? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params Object[]? parms ) {
		//ConsoleOutputSynch.EnterUpgradeableReadLock();

		(var fore, var back) = (Console.ForegroundColor, Console.BackgroundColor);
		(Console.ForegroundColor, Console.BackgroundColor) = (foreColor, backColor);
		Console.Write( text ?? String.Empty, parms );
		(Console.BackgroundColor, Console.ForegroundColor) = (back, fore);
	}

	public static void WriteLineColor( this String? text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params Object[]? parms ) {
		//ConsoleOutputSynch.EnterUpgradeableReadLock();

		(var fore, var back) = (Console.ForegroundColor, Console.BackgroundColor);
		(Console.ForegroundColor, Console.BackgroundColor) = (foreColor, backColor);
		Console.WriteLine( text ?? String.Empty, parms );
		(Console.ForegroundColor, Console.BackgroundColor) = (fore, back);
	}
}