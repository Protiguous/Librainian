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
// File "Benchmark.cs" last formatted on 2020-08-14 at 8:38 PM.

namespace Librainian.Measurement {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using JetBrains.Annotations;
	using Logging;
	using Seconds = Time.Seconds;

	/// <summary>
	///     Originally based upon the idea from
	///     <see cref="http://github.com/EBrown8534/Framework/blob/master/Evbpc.Framework/Utilities/BenchmarkResult.cs" />.
	/// </summary>
	/// <see cref="http://github.com/PerfDotNet/BenchmarkDotNet" />
	public static class Benchmark {

		public enum AorB {

			Unknown,
			MethodA,
			MethodB,
			Same

		}

		/// <summary>For benchmarking methods that are too fast for individual <see cref="Stopwatch" /> start and stops.</summary>
		/// <param name="method"></param>
		/// <param name="runFor"></param>
		/// <returns>Returns how many rounds are ran in the time given.</returns>
		public static UInt64 GetBenchmark( [NotNull] this Action method, TimeSpan? runFor ) {
			if ( method is null ) {
				throw new ArgumentNullException( nameof( method ) );
			}

			GC.Collect();

			var oldPriorityClass = Process.GetCurrentProcess().PriorityClass;
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

			var oldPriority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;

			if ( runFor is null ) {
				runFor = Seconds.One;
			}

			try {
				try {
					method.Invoke(); //jit per Eric Lippert (http://codereview.stackexchange.com/questions/125539/benchmarking-things-in-c)
				}
				catch ( Exception exception ) {
					exception.Log();
				}

				var rounds = 0UL;

				var stopwatch = Stopwatch.StartNew();

				while ( stopwatch.Elapsed < runFor ) {
					try {
						method.Invoke();
					}
					catch ( Exception exception ) {
						exception.Log();
					}
					finally {
						rounds++;
					}
				}

				return rounds;
			}
			finally {
				Process.GetCurrentProcess().PriorityClass = oldPriorityClass;
				Thread.CurrentThread.Priority = oldPriority;
			}
		}

		public static AorB WhichIsFaster( [NotNull] Action methodA, [NotNull] Action methodB, TimeSpan? runfor = null ) {
			if ( methodA is null ) {
				throw new ArgumentNullException( nameof( methodA ) );
			}

			if ( methodB is null ) {
				throw new ArgumentNullException( nameof( methodB ) );
			}

			if ( null == runfor ) {
				runfor = Seconds.One;
			}

			var a = methodA.GetBenchmark( runfor );

			var b = methodB.GetBenchmark( runfor );

			if ( a > b ) {
				return AorB.MethodA;
			}

			if ( b > a ) {
				return AorB.MethodB;
			}

			return AorB.Same;
		}

	}

}