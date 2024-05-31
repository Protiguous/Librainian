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
// File "Status.cs" last formatted on 2022-02-03 at 9:38 AM by Protiguous.

namespace Librainian.Utilities;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Parsing;

[SuppressMessage( "Roslynator", "RCS1161:Enum should declare explicit values." )]
public enum Status : SByte {

	[Description( Symbols.SkullAndCrossbones )]
	Fatal = Exception - 1,

	[Description( Symbols.Exception )]
	Exception = Error - 1,

	[Description( Symbols.Error )]
	Error = Warning - 1,

	[Description( Symbols.Warning )]
	Warning = Skip - 1,

	[Description( Symbols.Fail )]
	Skip = Cancel - 1,

	[Description( Symbols.BlackStar )]
	Cancel = Timeout - 1,

	[Description( Symbols.Timeout )]
	Timeout = Stop - 1,

	[Description( Symbols.StopSign )]
	Stop = Halt - 1,

	[Description( Symbols.StopSign )]
	Halt = No - 1,

	[Description( Symbols.Fail )]
	No = Bad - 1,

	[Description( Symbols.Fail )]
	Bad = Negative - 1,

	[Description( Symbols.FailBig )]
	Negative = Failure - 1,

	[Description( Symbols.BlackStar )]
	Failure = Unknown - 1,

	[Description( Symbols.Unknown )]
	Unknown = 0,

	[Description( Symbols.Unknown )]
	None = Unknown,

	[Description( Symbols.WhiteStar )]
	Success = 1,

	[Description( Symbols.CheckMark )]
	Positive,

	[Description( Symbols.WhiteStar )]
	Okay,

	[Description( Symbols.CheckMark )]
	Done,

	[Description( Symbols.CheckMark )]
	Good,

	[Description( Symbols.CheckMark )]
	Yes,

	[Description( Symbols.CheckMark )]
	Continue,

	[Description( Symbols.CheckMark )]
	Go,

	[Description( Symbols.CheckMark )]
	Advance,

	[Description( Symbols.CheckMark )]
	Start,

	[Description( Symbols.CheckMark )]
	Proceed,

	[Description( Symbols.CheckMark )]
	Flawless
}