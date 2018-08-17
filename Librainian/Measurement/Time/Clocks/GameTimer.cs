// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "GameTimer.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "GameTimer.cs" was last formatted by Protiguous on 2018/07/13 at 1:25 AM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Threading;
	using System.Timers;
	using Frequency;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Timer = System.Timers.Timer;

	/// <summary>
	///     A timer that fires at 60fps and Reports() back.
	/// </summary>
	[JsonObject]
	public class GameTimer {

		/// <summary>
		/// </summary>
		[JsonProperty]
		private UInt64 _counter;

		/// <summary>
		/// </summary>
		[JsonProperty]
		private volatile Boolean _isPaused;

		/// <summary>
		/// </summary>
		[JsonProperty]
		private TimeSpan _lastElapsed = TimeSpan.Zero;

		[JsonProperty]
		private DateTime _lastUpdate = DateTime.UtcNow;

		private DateTime LastProgressReport {
			get {
				try { return this._lastUpdate; }
				finally { Thread.MemoryBarrier(); }
			}

			set {
				try { Thread.MemoryBarrier(); }
				finally { this._lastUpdate = value; }
			}
		}

		[NotNull]
		private IProgress<ReportBack> Progress { get; }

		/// <summary>
		/// </summary>
		[NotNull]
		private Timer Timer { get; }

		private Double UpdateRate { get; } = ( Double )Fps.Sixty.Value;

		public UInt64 Counter {
			get => Thread.VolatileRead( ref this._counter );

			private set => Thread.VolatileWrite( ref this._counter, value );
		}

		/// <summary>
		///     Time since last tick().
		/// </summary>
		/// <value></value>
		public TimeSpan Elapsed {
			get {
				this.LastElapsed = DateTime.UtcNow - this.LastProgressReport;

				return this.LastElapsed;
			}
		}

		public Boolean IsPaused {
			get => this._isPaused;

			private set => this._isPaused = value;
		}

		/// <summary>
		///     A copy of the most recent <see cref="Elapsed" />.
		/// </summary>
		public TimeSpan LastElapsed {
			get {
				try { return this._lastElapsed; }
				finally { Thread.MemoryBarrier(); }
			}

			private set {
				try { Thread.MemoryBarrier(); }
				finally { this._lastElapsed = value; }
			}
		}

		public GameTimer( [NotNull] IProgress<ReportBack> progress ) {
			this.Progress = progress ?? throw new ArgumentNullException( nameof( progress ), "Progress must not be null." );

			// ReSharper disable once UseObjectOrCollectionInitializer
			this.Timer = new Timer( interval: this.UpdateRate ) {
				AutoReset = false
			};

			this.Timer.Elapsed += this.OnTimerElapsed;
			this.Resume();
		}

		private void OnTimerElapsed( Object sender, ElapsedEventArgs elapsedEventArgs ) {
			try {
				this.Pause();
				this.Counter++;

				this.Progress.Report( new ReportBack {
					Counter = this.Counter,
					Elapsed = this.Elapsed,
					RunningSlow = this.IsRunningSlow()
				} );
			}
			catch ( Exception exception ) { exception.Log(); }
			finally {
				this.LastProgressReport = DateTime.UtcNow;
				this.Resume();
			}
		}

		public Boolean IsRunningSlow() => this.LastElapsed.TotalMilliseconds > 70;

		public Boolean Pause() {
			this.Timer.Stop();
			this.IsPaused = true;

			return this.IsPaused;
		}

		public Boolean Resume() {
			this.IsPaused = false;
			this.Timer.Start();

			return !this.IsPaused;
		}

		/// <summary>
		///     Total time passed since timer was started.
		/// </summary>
		/// <returns></returns>
		public SpanOfTime TotalElapsed() => new SpanOfTime( milliseconds: this.Counter / this.UpdateRate );

		[JsonObject]
		public struct ReportBack {

			[JsonProperty]
			public UInt64 Counter { get; set; }

			[JsonProperty]
			public TimeSpan Elapsed { get; set; }

			[JsonProperty]
			public Boolean RunningSlow { get; set; }
		}
	}
}