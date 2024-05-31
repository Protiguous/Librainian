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
// File "Logging.cs" last formatted on 2022-02-08 at 6:04 AM by Protiguous.


namespace Librainian.Logging;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using Parsing;

//using Microsoft.Extensions.Logging.Console;

public static class Logging {
	//See Also: Microsoft.Extensions.Logging.Console

	//public static void ConfigureServices( this IServiceProvider services ) {
	//	services.AddSimpleConsole();
	//}

	//public static void ConfigureServices( this IServiceProvider services ) {
	//	services.AddSimpleConsole();
	//}

	private static readonly ILogger<Type>? logger;	//TODO Figure out how to actually use depend injection

	private static readonly Boolean IsUsingNUnit = AppDomain.CurrentDomain.GetAssemblies()
													 .Any( static assembly => assembly.FullName?.StartsWith( "nunit.framework", true, CultureInfo.InvariantCulture ) ==
																			  true );

	public static Boolean IsRunningFromNUnit() => IsUsingNUnit;

	/// <summary>
	///     <para>Prints the <paramref name="breakReason" /></para>
	///     <para>Then calls <see cref="Debugger.Break" />.</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	/// <param name="breakReason"></param>
	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void Break<T>( this T self, String? breakReason = null ) {
		if ( !Debugger.IsAttached ) {
			return;
		}

		if ( breakReason is not null ) {
			$"Break: {breakReason}".DebugWriteLine();
		}

		$"{self}".DebugWriteLine();

		Debugger.Break();
	}

	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void BreakIf( this Boolean? condition, String message, String? breakReason = null ) {
		if ( condition == true ) {
			message.Break( breakReason );
		}
	}

	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void BreakIfFalse( this Boolean? condition, String message, String? breakReason = null ) => ( !condition ).BreakIf( message, breakReason );

	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void BreakIfTrue( this Boolean? condition, String message, String? breakReason = null ) => condition.BreakIf( message, breakReason );

	[DebuggerStepThrough]
	public static (Color fore, Color back) Colors( this LogLevel loggingLevel ) =>
		loggingLevel switch {
			LogLevel.Trace => (Color.Green, Color.White),
			LogLevel.Debug => (Color.DarkSeaGreen, Color.White),
			LogLevel.Information => (Color.Black, Color.White),
			LogLevel.Warning => (Color.Goldenrod, Color.White),
			LogLevel.Error => (Color.Red, Color.White),
			LogLevel.Critical => (Color.DarkRed, Color.Aqua),
			LogLevel.None => (Color.White, Color.DarkBlue),
			var _ => throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null )
		};

	/// <summary>Write to <see cref="Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void DebugWrite<T>( this T self ) => Debug.Write( self );

	/// <summary>Write line to <see cref="Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void DebugWriteLine<T>( this T self ) => Debug.WriteLine( self );

	/// <summary>Write to <see cref="Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	[Conditional( "DEBUG" )]
	[DebuggerStepThrough]
	public static void Error<T>( this T self ) => self.DebugWriteLine();

	/// <summary>Write to <see cref="Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	public static void Fatal<T>( this T self ) => self.DebugWriteLine();

	/// <summary>Write to <see cref="Debug" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	[Conditional( "DEBUG" )]
	[DebuggerStepThrough]
	public static void Info<T>( this T self ) => self.DebugWriteLine();

	[DebuggerStepThrough]
	public static String LevelName( this LogLevel loggingLevel ) =>
		loggingLevel switch {
			LogLevel.Trace => nameof( LogLevel.Trace ),
			LogLevel.Debug => nameof( LogLevel.Debug ),
			LogLevel.Information => nameof( LogLevel.Information ),
			LogLevel.Warning => nameof( LogLevel.Warning ),
			LogLevel.Error => nameof( LogLevel.Error ),
			LogLevel.Critical => nameof( LogLevel.Critical ),
			LogLevel.None => nameof( LogLevel.None ),
			var _ => throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null )
		};

	[DebuggerStepThrough]
	public static Exception Log<T>( this T exception, BreakOrDontBreak breakinto = BreakOrDontBreak.Break, String? breakReason = null ) where T : Exception {
		var message = exception.ToStringDemystified();
		message.LogTimeMessage();

		if ( breakinto == BreakOrDontBreak.Break  ) {
            message.Break( breakReason );
        }

		return exception;
	}

	
	/// <summary>Prefix <paramref name="message" /> with the datetime and write out to the attached debugger and/or trace.</summary>
	/// <param name="message"></param>
	[Conditional( "DEBUG" )]
	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void LogTimeMessage( this String? message ) => $"[{DateTime.Now:t}] {message ?? Symbols.Null}".DebugWriteLine();

	[Conditional( "DEBUG" )]
	[DebuggerStepThrough]
	public static void TimeDebug( this String message, Boolean newline = true, Boolean showThread = false ) {
		if ( newline ) {
			DebugWriteLine( showThread ? $"[{DateTime.UtcNow:s}].({Environment.CurrentManagedThreadId}) {message}" : $"[{DateTime.UtcNow:s}] {message}" );
		}
		else {
			DebugWrite( message );
		}
	}

	/// <summary>
	///     Write a message to System.Diagnostics.Trace.
	///     <para>See also <see cref="TraceWriteLine" />.</para>
	/// </summary>
	/// <param name="message"></param>
	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void Trace( this String message ) => System.Diagnostics.Trace.Write( message );

	/// <summary>
	///     Write a message to System.Diagnostics.TraceLine.
	///     <para>See also <see cref="Trace" />.</para>
	/// </summary>
	/// <param name="message"></param>
	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void TraceWriteLine( this String message ) => System.Diagnostics.Trace.WriteLine( message );

	[Conditional( "TRACE" )]
	[DebuggerStepThrough]
	public static void TraceWithTime( this String message, Boolean newline = true, Boolean showThread = false ) {
		if ( newline ) {
			( showThread ? $"[{DateTime.UtcNow:s}].({Environment.CurrentManagedThreadId}) {message}" : $"[{DateTime.UtcNow:s}] {message}" ).TraceWriteLine();
		}
		else {
			message.Trace();
		}
	}

	[Conditional( "VERBOSE" )]
	[DebuggerStepThrough]
	public static void Verbose( this String? message ) {
		if ( message is null ) {
			return;
		}

		if ( Debugger.IsAttached ) {
			DebugWriteLine( message );
		}
		else {
			TraceWriteLine( message );
		}
	}
}