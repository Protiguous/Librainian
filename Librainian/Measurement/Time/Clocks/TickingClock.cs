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
// File "TickingClock.cs" last formatted on 2020-08-27 at 7:45 PM.

#nullable enable

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Timers;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Utilities;

	/// <summary>
	///     <para>Starts a forward-ticking clock at the given time with settable events.</para>
	///     <para>Should be threadsafe.</para>
	///     <para>
	///         Settable events are:
	///         <para>
	///             <see cref="OnHourTick" />
	///         </para>
	///         <para>
	///             <see cref="OnMinuteTick" />
	///         </para>
	///         <para>
	///             <see cref="OnSecondTick" />
	///         </para>
	///         <para>
	///             <see cref="OnMillisecondTick" />
	///         </para>
	///     </para>
	/// </summary>
	[JsonObject]
	public class TickingClock : ABetterClassDispose, IStandardClock {

		public enum Granularity {

			Microseconds,
			Milliseconds,
			Seconds,
			Minutes,
			Hours

		}

		/// <summary></summary>
		[CanBeNull]
		private Timer? _timer;

		public TickingClock( DateTime time, Granularity granularity = Granularity.Seconds ) {
			this.Hour = ( Hour )time.Hour;
			this.Minute = ( Minute )time.Minute;
			this.Second = ( Second )time.Second;
			this.Millisecond = ( Millisecond )time.Millisecond;
			this.Microsecond = 0; //TODO can we get using DateTime.Ticks vs StopWatch.TicksPer/Frequency stuff?
			this.ResetTimer( granularity );
		}

		public TickingClock( Time time, Granularity granularity = Granularity.Seconds ) {
			this.Hour = time.Hour;
			this.Minute = time.Minute;
			this.Second = time.Second;
			this.Millisecond = time.Millisecond;
			this.Microsecond = time.Microsecond;
			this.ResetTimer( granularity );
		}

		[JsonProperty]
		public Microsecond Microsecond { get; private set; }

		[CanBeNull]
		[JsonProperty]
		public Action<Hour>? OnHourTick { get; set; }

		[CanBeNull]
		[JsonProperty]
		public Action? OnMillisecondTick { get; set; }

		[CanBeNull]
		[JsonProperty]
		public Action? OnMinuteTick { get; set; }

		[CanBeNull]
		[JsonProperty]
		public Action? OnSecondTick { get; set; }

		/// <summary></summary>
		[JsonProperty]
		public Hour Hour { get; private set; }

		/// <summary></summary>
		[JsonProperty]
		public Millisecond Millisecond { get; private set; }

		/// <summary></summary>
		[JsonProperty]
		public Minute Minute { get; private set; }

		/// <summary></summary>
		[JsonProperty]
		public Second Second { get; private set; }

		public Boolean IsAm() => !this.IsPm();

		public Boolean IsPm() => this.Hour.Value >= 12;

		public Time Time() {
			try {
				this._timer?.Stop(); //stop the timer so the seconds don't tick while we get the values.

				return new Time( this.Hour.Value, this.Minute.Value, this.Second.Value );
			}
			finally {
				this._timer?.Start();
			}
		}

		private void OnHourElapsed( [CanBeNull]
		                            Object? sender, [CanBeNull]
		                            ElapsedEventArgs? e ) {
			this.Hour = this.Hour.Next( out var tocked );

			if ( tocked ) {
				this.OnHourTick?.Invoke( this.Hour );
			}

		}

		private void OnMillisecondElapsed( [CanBeNull]
		                                   Object? sender, [CanBeNull]
		                                   ElapsedEventArgs e ) {
			this.Millisecond = this.Millisecond.Next( out var tocked );

			if ( tocked ) {
				this.OnMillisecondTick?.Invoke();

				this.OnSecondElapsed( sender, e );
			}

		}

		private void OnMinuteElapsed( [CanBeNull]
		                              Object? sender, [CanBeNull]
		                              ElapsedEventArgs e ) {
			this.Minute = this.Minute.Next( out var tocked );

			if ( tocked ) {
				this.OnMinuteTick?.Invoke();

				this.OnHourElapsed( sender, e );
			}

		}

		private void OnSecondElapsed( [CanBeNull]
		                              Object? sender, [CanBeNull]
		                              ElapsedEventArgs e ) {
			this.Second = this.Second.Next( out var tocked );

			if ( tocked ) {
				this.OnSecondTick?.Invoke();

				this.OnMinuteElapsed( sender, e );
			}

		}

		/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
		public override void DisposeManaged() {
			using ( this._timer ) {
				this._timer?.Stop();
			}
		}

		public void ResetTimer( Granularity granularity ) {
			using ( this._timer ) {
				this._timer?.Stop();
			}

			switch ( granularity ) {
				case Granularity.Milliseconds:

					// ReSharper disable once UseObjectOrCollectionInitializer
					this._timer = new Timer( ( Double )Milliseconds.One.Value ) {
						AutoReset = true
					};

					this._timer.Elapsed += this.OnMillisecondElapsed;

					break;

				case Granularity.Seconds:

					// ReSharper disable once UseObjectOrCollectionInitializer
					this._timer = new Timer( ( Double )Seconds.One.Value ) {
						AutoReset = true
					};

					this._timer.Elapsed += this.OnSecondElapsed;

					break;

				case Granularity.Minutes:

					// ReSharper disable once UseObjectOrCollectionInitializer
					this._timer = new Timer( ( Double )Minutes.One.Value ) {
						AutoReset = true
					};

					this._timer.Elapsed += this.OnMinuteElapsed;

					break;

				case Granularity.Hours:

					// ReSharper disable once UseObjectOrCollectionInitializer
					this._timer = new Timer( ( Double )Hours.One.Value ) {
						AutoReset = true
					};

					this._timer.Elapsed += this.OnHourElapsed;

					break;

				default:
					throw new ArgumentOutOfRangeException( nameof( granularity ) );
			}

			this._timer.Start();
		}

	}

}