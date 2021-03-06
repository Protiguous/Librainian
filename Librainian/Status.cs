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
// File "Status.cs" last formatted on 2020-08-21 at 3:38 PM.

namespace Librainian {

	using System;
	using System.ComponentModel;
	using Parsing;

	public enum Status : Int16 {

		[Description( Symbols.SkullAndCrossbones )]
		Fatal = Exception - 1,

		[Description( Symbols.Exception )]
		Exception = Error - 1,

		[Description( Symbols.Error )]
		Error = Warning - 1,

		[Description( Symbols.Warning )]
		Warning = Skip - 1,

		[Description( Symbols.Fail )]
		Skip = Timeout - 1,

			[Description( Symbols.Timeout )]
			Timeout = Stop - 1,

				[Description( Symbols.StopSign )]
				Stop = Halt - 1,

					[Description( Symbols.StopSign )]
					Halt = Negative - 1,

						[Description( Symbols.FailBig )]
						Negative = No - 1,

							[Description( Symbols.Fail )]
							No = Bad - 1,

								[Description( Symbols.Fail )]
								Bad = Failure - 1,

									[Description( Symbols.BlackStar )]
									Failure = -1,

										[Description( Symbols.Unknown )]
										Unknown = 0,

									[Description( Symbols.WhiteStar )]
									Success = 1,

									[Description( Symbols.WhiteStar )]
									Okay = Success,

								[Description( Symbols.CheckMark )]
								Good = Success + 1,

							[Description( Symbols.CheckMark )]
							Yes = Good + 1,

						[Description( Symbols.CheckMark )]
						Positive = Yes + 1,

					[Description( Symbols.CheckMark )]
					Continue = Positive + 1,

				[Description( Symbols.CheckMark )]
				Go = Continue + 1,

			[Description( Symbols.CheckMark )]
			Proceed = Go + 1,

		[Description( Symbols.CheckMark )]
		Advance = Proceed + 1,

		[Description( Symbols.CheckMark )]
		Flawless = Advance + 1
	}

	
}