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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous/".
//
// File "CoinWallet.cs" last formatted on 2022-06-20 at 5:44 PM by Protiguous.

namespace Librainian.Financial.Currency.Bitcoin;

using Containers.Wallets;
using Exceptions;
using Measurement.Time;
using Threading;
using Utilities.Disposables;

/// <summary>
///     My first go at a thread-safe CoinWallet class for bitcoin coins. It's more pseudocode for learning than for
///     production.. Use at your own risk. Any tips
///     or ideas? Any dos or dont's? Email me!
/// </summary>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
public class CoinWallet : ABetterClassDispose, IEnumerable<(ICoin coin, UInt64 quantity)>, ICoinWallet {

	private CoinWallet( Guid id ) {
		this.ID = id;
		this.CancellationTokenSource = new CancellationTokenSource();
		this.Actor = new ActionBlock<BitcoinTransactionMessage>( DoAction, Blocks.ManyProducers.ConsumeSerial( default( CancellationToken? ) ) );
		this.Statistics = new( Minutes.One, this.CancellationTokenSource.Token );

		void DoAction( BitcoinTransactionMessage message ) {
			if ( message is { TransactionType: TransactionType.Deposit } ) {
				this.Deposit( message.Coin, message.Quantity );

				return;
			}

			if ( message is { TransactionType: TransactionType.Withdraw } ) {
				this.TryWithdraw( message.Coin, message.Quantity );

				return;
			}

			throw new InvalidOperationException();
		}
	}

	private ActionBlock<BitcoinTransactionMessage>? Actor { get; set; }

	private CancellationTokenSource CancellationTokenSource { get; set; }

	/// <summary>Count of each <see cref="ICoin" />.</summary>
	private ConcurrentDictionary<ICoin, UInt64> Coins { get; } = new();

	private void NotifyOfDeposit( (ICoin coin, UInt64 quantity) coins ) => this.OnDeposit?.Report( coins );

	private void NotifyOfWithdrawal( (ICoin coin, UInt64 quantity) coins ) => this.OnWithdraw?.Report( coins );

	/// <summary>Return each <see cref="ICoin" /> in this <see cref="CoinWallet" />.</summary>
	public IReadOnlyCollection<ICoin> AllCoins =>
		( IReadOnlyCollection<ICoin> )this.Coins.SelectMany<KeyValuePair<ICoin, UInt64>, Int32, ICoin>( static pair => 1.To( pair.Value ), ( pair, _ ) => pair.Key );

	public IReadOnlyCollection<(ICoin coin, UInt64 quantity)> CoinsGrouped =>
		( IReadOnlyCollection<(ICoin coin, UInt64 quantity)> )this.Coins.Select( pair => (pair.Key, pair.Value) );

	public Guid ID { get; }

	public IProgress<(ICoin coin, UInt64 quantity)>? OnDeposit { get; set; }

	public IProgress<(ICoin coin, UInt64 quantity)>? OnWithdraw { get; set; }

	[JsonProperty]
	public WalletStatistics? Statistics { get; }

	/// <summary>Return the total amount of money contained in this <see cref="CoinWallet" />.</summary>
	public Decimal Total => this.Coins.Aggregate( Decimal.Zero, ( current, pair ) => current + ( pair.Key.FaceValue * pair.Value ) );

	/// <summary>
	///     Create an empty wallet with the given <paramref name="id" />. If the given <paramref name="id" /> is null or
	///     <see cref="Guid.Empty" />, a new random
	///     <paramref name="id" /> is generated.
	/// </summary>
	/// <param name="id"></param>
	public static CoinWallet Create( Guid? id = null ) {
		if ( !id.HasValue || ( id.Value == Guid.Empty ) ) {
			id = Guid.NewGuid();
		}

		return new CoinWallet( id.Value );
	}

	/// <summary>
	///     Mark as cancelled.
	/// </summary>
	public void Cancel() => this.CancellationTokenSource.Cancel();

	public Boolean Contains( ICoin coin ) {
		if ( coin is null ) {
			throw new ArgumentEmptyException( nameof( coin ) );
		}

		return this.Coins.ContainsKey( coin );
	}

	public UInt64 Count( ICoin coin ) {
		if ( coin is null ) {
			throw new ArgumentEmptyException( nameof( coin ) );
		}

		return this.Coins.TryGetValue( coin, out var result ) ? result : UInt64.MinValue;
	}

	public async PooledValueTask<UInt64> Deposit( (ICoin coin, UInt64 quantity) tuple, Boolean updateStatistics = true ) =>
		await this.Deposit( tuple.coin, tuple.quantity, updateStatistics ).ConfigureAwait( false );

	public async PooledValueTask<UInt64> Deposit( ICoin coin, UInt64 quantity, Boolean updateStatistics = true ) {
		try {
			lock ( this.Coins ) {
				UInt64 newQuantity = 0;

				if ( !this.Coins.ContainsKey( coin ) ) {
					if ( this.Coins.TryAdd( coin, quantity ) ) {
						newQuantity = quantity;
					}
				}
				else {
					newQuantity = this.Coins[coin] += quantity;
				}

				return newQuantity;
			}
		}
		finally {
			if ( updateStatistics ) {
				var statistics = this.Statistics;

				if ( statistics != null ) {
					var allTimeDeposited = await statistics.GetAllTimeDeposited().ConfigureAwait( false );
					await statistics.SetAllTimeDeposited( allTimeDeposited + ( coin.FaceValue * quantity ) ).ConfigureAwait( false );
				}
			}

			this.OnDeposit?.Report( (coin, quantity) );
		}
	}

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		using ( this.Statistics ) { }
	}

	public IEnumerator<(ICoin coin, UInt64 quantity)> GetEnumerator() => this.Coins.Select( pair => (pair.Key, pair.Value) ).GetEnumerator();

	/// <summary>
	///     Unset the cancelled status.
	/// </summary>
	/// <returns></returns>
	public Boolean ResetCancel() => this.CancellationTokenSource.TryReset();

	public override String ToString() {
		var coins = this.Coins.Aggregate( 0UL, static ( current, pair ) => current + pair.Value );

		return $"฿{this.Total:F8} (in {coins:N0} coins)";
	}

	/// <summary>
	///     Attempt to <see cref="TryWithdraw(ICoin,UInt64)" /> one or more <see cref="ICoin" /> from this
	///     <see cref="CoinWallet" /> .
	/// </summary>
	/// <param name="coin"></param>
	/// <param name="quantity"></param>
	/// <remarks>Locks the wallet.</remarks>
	/// <exception cref="ArgumentEmptyException"></exception>
	public Boolean TryWithdraw( ICoin coin, UInt64 quantity ) {
		if ( coin is null ) {
			throw new ArgumentEmptyException( nameof( coin ) );
		}

		if ( quantity.NotAny() ) {
			return false;
		}

		lock ( this.Coins ) {
			if ( !this.Coins.ContainsKey( coin ) || ( this.Coins[coin] < quantity ) ) {
				return false; //no coins to withdraw!
			}

			this.Coins[coin] -= quantity;
		}

		this.OnWithdraw?.Report( (coin, quantity) );

		return true;
	}

	public ICoin? TryWithdrawAnyCoin() {
		var possibleCoins = this.Coins.Where( static pair => pair.Value > 0 ).Select( static pair => pair.Key ).ToList();
		possibleCoins.TrimExcess();

		if ( !possibleCoins.Any() ) {
			return default( ICoin );
		}

		possibleCoins.Shuffle();
		var key = possibleCoins.First();

		return this.TryWithdraw( key, 1 ) ? key : default( ICoin );
	}

	public Boolean TryWithdrawLargestCoin( out (ICoin coin, UInt64 quantity)? coins ) {
		var largest = this.Coins.Where( static pair => pair.Value.Any() ).OrderBy( static pair => pair.Key.FaceValue ).FirstOrDefault();

		if ( this.TryWithdraw( largest.Key, largest.Value ) ) {
			coins = (largest.Key, largest.Value);

			return true;
		}

		coins = default( (ICoin coin, UInt64 quantity)? );

		return false;
	}

	public Boolean TryWithdrawSmallestCoin( out (ICoin coin, UInt64 quantity)? coins ) {
		var smallest = this.Coins.Where( static pair => pair.Value.Any() ).OrderBy( static pair => pair.Key.FaceValue ).FirstOrDefault();

		if ( this.TryWithdraw( smallest.Key, smallest.Value ) ) {
			coins = (smallest.Key, smallest.Value);

			return true;
		}

		coins = default( (ICoin coin, UInt64 quantity)? );

		return false;
	}

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}