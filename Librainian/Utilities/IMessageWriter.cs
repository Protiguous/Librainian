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
// File "IMessageWriter.cs" last formatted on 2022-02-10 at 6:39 AM by Protiguous.

namespace Librainian.Utilities;

using System;
using Microsoft.Extensions.Logging;

public interface IMessageWriter {

	//void Write( String message );

	//void WriteLine( String message, params Object[] args );
}

public static class LoggerExtensions {

	static LoggerExtensions() => ControllerIndexExecuting = LoggerMessage.Define<DateTimeOffset>( LogLevel.Debug, 1, "Executing 'Index' action at '{StartTime}'" );

	private static Action<ILogger, DateTimeOffset, Exception?> ControllerIndexExecuting { get; }

	public static void HomeControllerIndexExecuting( this ILogger logger, DateTimeOffset executeTime ) => ControllerIndexExecuting( logger, executeTime, null );

	public static Int32 Test() {
		var a = 1;
		var b = 2;
		var c = a + b;
		return c;
	}
}