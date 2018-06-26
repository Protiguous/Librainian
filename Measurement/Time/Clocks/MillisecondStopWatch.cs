﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "MillisecondStopWatch.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "MillisecondStopWatch.cs" was last formatted by Protiguous on 2018/06/04 at 4:12 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using JetBrains.Annotations;

	/// <summary>
	///     Use with WindowsCE and Silverlight which don't have Stopwatch
	/// </summary>
	/// <remarks>
	///     Based on <seealso cref="http://github.com/amibar/SmartThreadPool/blob/master/SmartThreadPool/Stopwatch.cs" />
	/// </remarks>
	internal class MillisecondStopWatch {

		/// <summary>
		/// </summary>
		[NotNull]
		public SpanOfTime Elapsed => new SpanOfTime( milliseconds: this.GetElapsedDateTimeTicks() / TicksPerMillisecond );

		/// <summary>
		/// </summary>
		public Boolean IsRunning { get; private set; }

		private UInt64 _elapsed;

		private UInt64 _startTimeStamp;

		private static UInt64 GetTimestamp() => ( UInt64 ) DateTime.UtcNow.Ticks;

		private UInt64 GetElapsedDateTimeTicks() {
			var elapsed = this._elapsed;

			if ( !this.IsRunning ) { return elapsed; }

			var ticks = GetTimestamp() - this._startTimeStamp;
			elapsed += ticks;

			return elapsed;
		}

		[NotNull]
		public static MillisecondStopWatch StartNew() {
			var stopwatch = new MillisecondStopWatch();
			stopwatch.Start();

			return stopwatch;
		}

		public void Reset() {
			this._elapsed = 0;
			this._startTimeStamp = 0;
			this.IsRunning = false;
		}

		public void Start() {
			if ( this.IsRunning ) { return; }

			this._startTimeStamp = GetTimestamp();
			this.IsRunning = true;
		}

		public void Stop() {
			if ( !this.IsRunning ) { return; }

			var ticks = GetTimestamp() - this._startTimeStamp;
			this._elapsed += ticks;
			this.IsRunning = false;
		}

		private const Decimal TicksPerMillisecond = 10000.0m;

		public MillisecondStopWatch() => this.Reset();

	}

}