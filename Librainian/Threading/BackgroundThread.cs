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
// File "BackgroundThread.cs" last formatted on 2022-02-06 at 6:45 AM by Protiguous.

namespace Librainian.Threading;

using System;
using System.ComponentModel;
using System.Threading;
using Exceptions;
using Logging;
using Measurement.Time;
using Threadsafe;
using Utilities;

/// <summary>
///     Accepts an <see cref="Action" /> to perform, until the <see cref="CancellationToken" /> is set.
/// </summary>
[NeedsTesting]
public class BackgroundThread : BackgroundWorker {

	private readonly VolatileBoolean _runningAction = new( false );

	/// <summary>
	/// </summary>
	/// <param name="actionToPerform">Action to perform on each <see cref="Semaphore" />.</param>
	/// <param name="accessCount"></param>
	/// <param name="cancellationToken"></param>
	public BackgroundThread( Action actionToPerform, Int32 accessCount, CancellationToken cancellationToken ) {
		this.ActionToPerform = actionToPerform ?? throw new ArgumentEmptyException( nameof( actionToPerform ) );
		this.CancellationToken = cancellationToken;
		this.Semaphore = new( accessCount, accessCount );
	}

	private Action ActionToPerform { get; }

	private CancellationToken CancellationToken { get; }

	private SemaphoreSlim Semaphore { get; }

	/// <summary>
	///     <para>Every second wake up and see if we can get the semaphore.</para>
	///     <para>If we can, then run the ActionToPerform().</para>
	/// </summary>
	/// <param name="e"></param>
	protected override async void OnDoWork( DoWorkEventArgs e ) {
		while ( !this.CancellationToken.IsCancellationRequested ) {
			if ( await this.Semaphore.WaitAsync( Seconds.One, this.CancellationToken ).ConfigureAwait( false ) ) {
				try {
					this._runningAction.GetSetValue = true;

					await this.ActionToPerform;
				}
				catch ( Exception exception ) {
					exception.Log( BreakOrDontBreak.Break );
				}
				finally {
					this._runningAction.GetSetValue = false;
					this.Semaphore.Release();
				}
			}
		}
	}

	public Boolean IsRunningAction() => this._runningAction;
}