// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AsyncLock.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "AsyncLock.cs" was last formatted by Protiguous on 2018/07/13 at 1:39 AM.

namespace Librainian.Threading {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using Extensions;
	using Magic;
	using Measurement.Time;

	/// <summary>
	///     Usage: private readonly AsyncLock _lock = new AsyncLock(); using( var releaser = await _lock.LockAsync() ) {
	///     /*...*/ }
	/// </summary>
	/// <remarks>(I have no idea how to use this class.)</remarks>
	public sealed class AsyncLock : ABetterClassDispose {

		private readonly Task<IDisposable> _releaser;

		private SemaphoreSlim Semaphore { get; } = new SemaphoreSlim( initialCount: 1 );

		public AsyncLock() => this._releaser = Task.FromResult( new Releaser( this ) as IDisposable );

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() => this.Semaphore.Dispose();

		public Task<IDisposable> LockAsync() {
			var wait = this.Semaphore.WaitAsync( Minutes.Ten );

			return wait.IsCompleted ?
				this._releaser :
				wait.ContinueWith( ( _, state ) => ( IDisposable ) state, this._releaser.Result, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
		}

		private sealed class Releaser : ABetterClassDispose {

			private readonly AsyncLock _mToRelease;

			internal Releaser( AsyncLock toRelease ) => this._mToRelease = toRelease;

			public override void DisposeManaged() {
				if ( !this._mToRelease.Semaphore.CurrentCount.Any() ) { this._mToRelease.Semaphore.Release(); }
				else { Debugger.Break(); }

				base.DisposeManaged();
			}
		}
	}
}