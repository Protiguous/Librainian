﻿// Copyright © Protiguous. All Rights Reserved.
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
// File "ErrorTrapper.cs" last formatted on 2022-02-13 at 11:20 AM by Protiguous.

namespace Librainian.Extensions;

using System;
using System.Diagnostics;
using Exceptions;
using Logging;

public static class ErrorTrapper {

	/// <summary>
	///     Wrap an action with a try/catch.
	///     <returns>Returns true if <paramref name="action" /> had no exception.</returns>
	/// </summary>
	/// <param name="action"></param>
	/// <param name="final"> </param>
	/// <returns>Returns true if successful.</returns>
	[DebuggerStepThrough]
	public static Boolean Trap( this Action action, Action? final = default ) {
		try {
			action();

			return true;
		}
		catch ( Exception exception ) {
			exception.Log();
		}
		finally {
			try {
				final?.Invoke();
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		}

		return false;
	}

	/// <summary>Wrap an each action with a try/catch.</summary>
	/// <param name="actions"></param>
	/// <returns>Returns true if successful.</returns>
	/// <exception cref="ArgumentEmptyException"></exception>
	[DebuggerStepThrough]
	public static Boolean Trap( params Action[]? actions ) {
		try {
			if ( actions is null ) {
				if ( Debugger.IsAttached ) {
					throw new ArgumentEmptyException( nameof( actions ), $"Null list of {nameof( actions )} given. Unable to execute {nameof( actions )}." );
				}

				return false;
			}

			foreach ( var action in actions ) {
				action.Trap();
			}

			return true;
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return false;
	}

	/// <summary>Wrap a function with a try/catch.</summary>
	/// <param name="func"> </param>
	/// <param name="final"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	[DebuggerStepThrough]
	public static T? Trap<T>( this Func<T>? func, Action? final = default ) {
		if ( func is null ) {
			if ( Debugger.IsAttached ) {
				throw new ArgumentEmptyException( nameof( func ) ).Log( BreakOrDontBreak.Break );
			}

			return default( T? );
		}

		try {
			return func();
		}
		catch ( Exception exception ) {
			exception.Log();
		}
		finally {
			try {
				final?.Invoke();
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		}

		return default( T );
	}

	/// <summary>Wrap a function with a try/catch.</summary>
	/// <param name="func">    </param>
	/// <param name="argument"></param>
	/// <param name="exception"></param>
	/// <param name="final">   </param>
	/// <param name="actions"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	[DebuggerStepThrough]
	public static TResult? Trap<T, TResult>( this Func<T?, TResult>? func, T? argument, out Exception? exception, Action? final = default, params Action[]? actions ) {
		if ( func is null ) {
			if ( Debugger.IsAttached ) {
				throw new ArgumentEmptyException( nameof( func ) );
			}

			exception = new ArgumentEmptyException( nameof( func ) );

			return default( TResult? );
		}

		try {
			exception = default( Exception? );

			return func( argument );
		}
		catch ( Exception e ) {
			exception = e.Log();
		}
		finally {
			try {
				if ( actions != null ) {
					Trap( actions );
				}
			}
			catch ( Exception e ) {
				exception = e.Log();
			}

			try {
				final?.Invoke();
			}
			catch ( Exception e ) {
				exception = e.Log();
			}
		}

		return default( TResult );
	}
}