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
// File "UsableSemaphoreSlim.cs" last formatted on 2022-02-22 at 7:00 AM by Protiguous.

namespace Librainian.Threading;

using Exceptions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class UsableSemaphoreSlim : IUsableSemaphoreThrottle {

	private readonly SemaphoreSlim _semaphore;

	private class UsableSemaphoreWrapper : IUsableSemaphoreWrapper {

		private readonly SemaphoreSlim _semaphore;

		private readonly Stopwatch _stopwatch;

		private Boolean _isDisposed;

		/// <summary>
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private void CheckIfWait() {
			if ( this._stopwatch.IsRunning ) {
				throw new InvalidOperationException( "Already Initialized" );
			}

			this._stopwatch.Start();
		}

		public UsableSemaphoreWrapper( SemaphoreSlim semaphore ) {
			this._semaphore = semaphore ?? throw new ArgumentEmptyException( nameof( semaphore ) );
			this._stopwatch = new Stopwatch();
		}

		public TimeSpan Elapsed => this._stopwatch.Elapsed;

		public void Dispose() {
			if ( this._isDisposed ) {
				return;
			}

			if ( this._stopwatch.IsRunning ) {
				this._stopwatch.Stop();
				this._semaphore.Release();
			}

			this._isDisposed = true;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Task WaitAsync() {
			this.CheckIfWait();

			return this._semaphore.WaitAsync();
		}

		/// <summary>
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Task WaitAsync( TimeSpan timeout ) {
			this.CheckIfWait();

			return this._semaphore.WaitAsync( timeout );
		}

		/// <summary>
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Task WaitAsync( CancellationToken cancellationToken ) {
			this.CheckIfWait();

			return this._semaphore.WaitAsync( cancellationToken );
		}

		/// <summary>
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Task WaitAsync( TimeSpan timeout, CancellationToken cancellationToken ) {
			this.CheckIfWait();

			return this._semaphore.WaitAsync( timeout, cancellationToken );
		}
	}

	public UsableSemaphoreSlim( Int32 initialCount ) => this._semaphore = new SemaphoreSlim( initialCount );

	public UsableSemaphoreSlim( Int32 initialCount, Int32 maxCount ) => this._semaphore = new SemaphoreSlim( initialCount, maxCount );

	public void Dispose() {
		this._semaphore.Dispose();
		GC.SuppressFinalize( this );
	}

	public async Task<IUsableSemaphoreWrapper> WaitAsync() {
		using var wrapper = new UsableSemaphoreWrapper( this._semaphore );
		await wrapper.WaitAsync().ConfigureAwait( false );

		return wrapper;
	}

	public async Task<IUsableSemaphoreWrapper> WaitAsync( TimeSpan timeout ) {
		using var wrapper = new UsableSemaphoreWrapper( this._semaphore );
		await wrapper.WaitAsync( timeout ).ConfigureAwait( false );

		return wrapper;
	}

	public async Task<IUsableSemaphoreWrapper> WaitAsync( CancellationToken cancellationToken ) {
		using var wrapper = new UsableSemaphoreWrapper( this._semaphore );
		await wrapper.WaitAsync( cancellationToken ).ConfigureAwait( false );

		return wrapper;
	}

	public async Task<IUsableSemaphoreWrapper> WaitAsync( TimeSpan timeout, CancellationToken cancellationToken ) {
		using var wrapper = new UsableSemaphoreWrapper( this._semaphore );
		await wrapper.WaitAsync( timeout, cancellationToken ).ConfigureAwait( false );

		return wrapper;
	}
}