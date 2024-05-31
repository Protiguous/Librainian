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
// File "WalletStatistics.cs" last formatted on 2022-02-13 at 4:13 AM by Protiguous.

namespace Librainian.Financial.Containers.Wallets;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Maths;
using Newtonsoft.Json;
using PooledAwait;
using Utilities.Disposables;

[JsonObject]
public class WalletStatistics : ABetterClassDispose {

	[JsonProperty]
	private Decimal _allTimeDeposited;

	[JsonProperty]
	private Decimal _allTimeWithdrawn;

	public WalletStatistics( TimeSpan timeout, CancellationToken cancellationToken ) {
		this.Timeout = timeout;

		Debug.Assert( cancellationToken.CanBeCanceled );
		this.CancellationToken = cancellationToken;

		this.Reset().RunSynchronously();
	}

	private CancellationToken CancellationToken { get; set; }

	private SemaphoreSlim DepositResource { get; } = new( 1, 1 );

	private SemaphoreSlim WithdrawResource { get; } = new( 1, 1 );

	[JsonProperty]
	public DateTimeOffset InstanceCreationTime { get; private set; }

	[JsonIgnore]
	public TimeSpan Timeout { get; set; }

	public override void DisposeManaged() {
		using ( this.DepositResource ) { }

		using ( this.WithdrawResource ) { }
	}

	public async Task<Decimal> GetAllTimeDeposited() {
		if ( await this.DepositResource.WaitAsync( this.Timeout, this.CancellationToken ).ConfigureAwait( false ) ) {
			try {
				return this._allTimeDeposited;
			}
			finally {
				this.DepositResource.Release();
			}
		}

		return Decimal.Zero;
	}

	public async Task<Decimal> GetAllTimeWithdrawn() {
		if ( await this.WithdrawResource.WaitAsync( this.Timeout, this.CancellationToken ).ConfigureAwait( false ) ) {
			try {
				return this._allTimeWithdrawn;
			}
			finally {
				this.WithdrawResource.Release();
			}
		}

		return Decimal.Zero;
	}

	/// <summary>
	///     <para>
	///         Resets the semaphores, <see cref="_allTimeDeposited" />, <see cref="_allTimeWithdrawn" />, and
	///         <see cref="InstanceCreationTime" />.
	///     </para>
	///     <para>Does not and cannot reset the <see cref="CancellationToken" />.</para>
	/// </summary>
	/// <returns></returns>
	public async Task Reset() {
		while ( this.DepositResource.CurrentCount.NotAny() && this.CancellationToken is { IsCancellationRequested: false } ) {
			this.DepositResource.Release();
		}

		while ( this.WithdrawResource.CurrentCount.NotAny() && this.CancellationToken is { IsCancellationRequested: false } ) {
			this.WithdrawResource.Release();
		}

		await this.SetAllTimeDeposited( Decimal.Zero ).ConfigureAwait( false );
		await this.SetAllTimeWithdrawn( Decimal.Zero ).ConfigureAwait( false );

		this.InstanceCreationTime = DateTimeOffset.Now;
	}

	/// <summary>
	///     Sets the value of <see cref="_allTimeDeposited" /> to <paramref name="value" />.
	///     <para>Returns true if successful.</para>
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public async PooledValueTask<Boolean> SetAllTimeDeposited( Decimal value ) {
		if ( await this.DepositResource.WaitAsync( this.Timeout, this.CancellationToken ).ConfigureAwait( false ) ) {
			try {
				this._allTimeDeposited = value;
				return true;
			}
			finally {
				this.DepositResource.Release();
			}
		}

		return false;
	}

	/// <summary>
	///     Sets the value of <see cref="_allTimeDeposited" /> to <paramref name="value" />.
	///     <para>Returns true if successful.</para>
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public async Task<Boolean> SetAllTimeWithdrawn( Decimal value ) {
		if ( await this.WithdrawResource.WaitAsync( this.Timeout, this.CancellationToken ).ConfigureAwait( false ) ) {
			try {
				this._allTimeWithdrawn = value;
				return true;
			}
			finally {
				this.WithdrawResource.Release();
			}
		}

		return false;
	}
}