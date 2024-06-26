﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "CountableTests.cs" last touched on 2021-11-30 at 7:23 PM by Protiguous.

namespace LibrainianUnitTests.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Librainian;
using Librainian.Collections;
using Librainian.Maths;
using NUnit.Framework;

[TestFixture]
public static class CountableTests {

	private static Int32 Threads { get; } = Environment.ProcessorCount;

	private static void Many( ref Countable<String> countable ) {
		foreach ( var _ in 1.To( Threads * Threads ) ) {
			var length = ( Int32 )Randem.NextByte( 2, 3 );
			var key = length.RandomPronounceableString();
			var number = Randem.NextBigInteger( 128 );

			if ( Randem.NextBoolean() ) {
				countable.Add( key, number );
			}
			else {
				countable.Subtract( key, number );
			}
		}
	}

	[Test]
	public static void AddNameThreadSafetyTest() {
		var countable = new Countable<String>();

		var threads = new List<Thread>( Threads );

		foreach ( var thread in 1.To( Threads ).Select( _ => new Thread( () => Many( ref countable ) ) ) ) {
			threads.Add( thread );
			thread.Start();
		}

		Parallel.ForEach( threads.AsParallel(), thread => thread.Join() );

		1.Nop();

		//Assert.AreEqual( expected: Threads * Threads, actual: list.Count );
	}
}