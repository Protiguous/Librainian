// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Extensions.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.


namespace Librainian.Financial.Currency.Bitcoin;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Exceptions;
using Librainian.Extensions;
using Librainian.Maths;
using PooledAwait;
using Threading;

public static class Extensions {

	/// <summary>All possible bitcoin denominations.</summary>
	public static Lazy<ImmutableHashSet<ICoin>> PossibleCoins { get; } = new( typeof( ICoin ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<ICoin>().ToImmutableHashSet() );

	/// <summary>Deposit <paramref name="coins" /> into this wallet.</summary>
	/// <param name="coinWallet"></param>
	/// <param name="coins"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static async Task Deposit( this CoinWallet coinWallet, IAsyncEnumerable<(ICoin coin, UInt64 quantity)> coins ) {
		if ( coinWallet is null ) {
			throw new ArgumentEmptyException( nameof( coinWallet ) );
		}

		await foreach ( (var coin, var quantity) in coins.ConfigureAwait( false ) ) {
			await coinWallet.Deposit( coin, quantity ).ConfigureAwait( false );
		}
	}

	public static async PooledValueTask Fund( CoinWallet coinWallet, params (ICoin coin, UInt64 quantity)[] sourceAmounts ) {
		if ( coinWallet is null ) {
			throw new ArgumentEmptyException( nameof( coinWallet ) );
		}

		await Fund( coinWallet, sourceAmounts.ToAsyncEnumerable() ).ConfigureAwait( false );
	}

	public static async PooledValueTask Fund( CoinWallet coinWallet, IAsyncEnumerable<(ICoin coin, UInt64 quantity)> sourceAmounts ) => await Deposit( coinWallet, sourceAmounts ).ConfigureAwait( false );

	/// <summary>
	///     Adds the optimal amount of <see cref="ICoin" />. Returns any unused portion of the money (fractions of the smallest
	///     <see cref="ICoin" />).
	/// </summary>
	/// <param name="coinWallet"></param>
	/// <param name="amount"></param>
	/// <param name="optimalAmountOfCoin"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static async PooledValueTask<Decimal> Fund( this CoinWallet coinWallet, Decimal amount, Boolean optimalAmountOfCoin = true ) {
		if ( coinWallet is null ) {
			throw new ArgumentEmptyException( nameof( coinWallet ) );
		}

		var leftOverFund = Decimal.Zero;

		IAsyncEnumerable<(ICoin coin, UInt64 quantity)> bob;

		if ( optimalAmountOfCoin ) {
			bob = amount.Optimal( ref leftOverFund );
		}
		else {
			bob = amount.UnOptimal( ref leftOverFund );
		}

		await coinWallet.Deposit( bob ).ConfigureAwait( false );

		return leftOverFund;
	}

	/// <summary>
	///     Given the <paramref name="amount" />, return the optimal amount of <see cref="ICoin" /> (
	///     <see cref="CoinWallet.Total" />) it would take to <see cref="CoinWallet.Total" /> the <paramref name="amount" />.
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="leftOverAmount">Fractions of Pennies not accounted for.</param>
	public static IAsyncEnumerable<(ICoin coin, UInt64 quantity)> Optimal( this Decimal amount, ref Decimal leftOverAmount ) {
		//TODO pre-Order this list?
		var left = new List<ICoin>( PossibleCoins.Value );

		var result = left.ToDictionary<ICoin, ICoin, UInt64>( denomination => denomination, _ => 0 );

		leftOverAmount += amount;

		while ( ( leftOverAmount > Decimal.Zero ) && left.Any() ) {
			var coin = left.OrderByDescending( denomination => denomination.FaceValue ).First();

			var chunks = ( UInt64 )( leftOverAmount / coin.FaceValue );

			if ( chunks > Decimal.Zero ) {
				result[coin] += chunks;
				leftOverAmount -= chunks * coin.FaceValue;
			}

			var removed = left.Remove( coin );
			Debug.Assert( removed );
		}

		return result.Select( pair => (pair.Key, pair.Value) ).ToAsyncEnumerable();
	}

	/// <summary>Truncate anything lesser than 1 <see cref="Satoshi" />.</summary>
	/// <param name="btc"></param>
	public static Decimal Sanitize( this Decimal btc ) {
		var sanitized = btc.ToSatoshi().ToBTC();

		//Assert.GreaterOrEqual( btc, sanitized );
		return sanitized;
	}

	public static String? SimplerBTC( this SimpleBitcoinWallet wallet ) {
		if ( wallet is null ) {
			throw new ArgumentEmptyException( nameof( wallet ) );
		}

		return wallet.Balance.GetValueOrDefault( Decimal.Zero ).SimplerBTC();
	}

	/// <summary>
	///     <para>0. 00000001 -&gt; 1 satoshi</para>
	///     <para>0. 00000011 -&gt; 11 satoshi</para>
	///     <para>0. 00000110 -&gt; 11 μBTC</para>
	/// </summary>
	/// <param name="btc"></param>
	/// <param name="coinSuffix">
	///     <para>BTC</para>
	///     <para>NMC</para>
	///     <para>etc...</para>
	/// </param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static String? SimplerBTC( this Decimal btc, String coinSuffix = "BTC" ) {
		if ( coinSuffix is null ) {
			throw new ArgumentEmptyException( nameof( coinSuffix ) );
		}

		btc.Sanitize();

		var set = new HashSet<String> {
			new SimpleBitcoinWallet( btc ).ToString().TrimEnd( '0' ).TrimEnd( '.' ),
			$"{$"{btc.TomBTC():N6}".TrimEnd( '0' ).TrimEnd( '.' )} m{coinSuffix}",
			$"{$"{btc.ToμBtc():N4}".TrimEnd( '0' ).TrimEnd( '.' )} μ{coinSuffix}",
			$"{btc.ToSatoshi():N0} satoshi"
		};

		//as btc

		//as mbtc

		//as μbtc

		//as satoshi
		var chosen = set.OrderBy( s => s.Length ).FirstOrDefault();

		return chosen;
	}

	/// <summary>Create a TPL dataflow task for depositing large volumes of money.</summary>
	/// <param name="coinWallet"></param>
	/// <param name="sourceAmounts"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static async PooledValueTask StartDeposit( this CoinWallet coinWallet, IAsyncEnumerable<(ICoin coin, UInt64 quantity)> sourceAmounts ) {
		if ( coinWallet is null ) {
			throw new ArgumentEmptyException( nameof( coinWallet ) );
		}

		await Parallel.ForEachAsync( sourceAmounts, async ( tuple, _ ) => {
			(var coin, var quantity) = tuple;

			await coinWallet.Deposit( coin, quantity ).ConfigureAwait( false );
		} ).ConfigureAwait( false );
	}

	public static Decimal ToBTC( this Int16 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

	public static Decimal ToBTC( this Int32 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

	public static Decimal ToBTC( this Int64 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

	public static Decimal TomBTC( this Decimal btc ) => btc * SimpleBitcoinWallet.mBTCInOneBTC;

	public static Int64 ToSatoshi( this Decimal btc ) => ( Int64 )( btc * SimpleBitcoinWallet.SatoshiInOneBtc );

	/// <summary>Return the <paramref name="wallet" /> in Satoshi.</summary>
	/// <param name="wallet"></param>
	public static Int64 ToSatoshi( this SimpleBitcoinWallet wallet ) => wallet.Balance.GetValueOrDefault( Decimal.Zero ).ToSatoshi();

	public static Decimal ToμBtc( this Decimal btc ) => btc * SimpleBitcoinWallet.ΜBtcInOneBtc;

	/*
	public static async IAsyncEnumerable<(ICoin coin, UInt64 quantity)> Transfer( this CoinWallet source, CoinWallet target, CancellationToken cancellationToken ) {
		if ( source is null ) {
			throw new ArgumentEmptyException( nameof( source ) );
		}

		if ( target is null ) {
			throw new ArgumentEmptyException( nameof( target ) );
		}

		await foreach ( var pair in source.AllCoins.ToAsyncEnumerable().WithCancellation( cancellationToken ).ConfigureAwait( false ) ) {
            if ( source.TryWithdrawSmallestCoin( out var coins ) ) {
            }

            if ( !source.Transfer( target, pair ) ) {
				continue;
			}

			var denomination = pair.coin;
			var count = pair.count;

			transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => {
				if ( denomination1 == denomination ) {
					return running + count;
				}

				return default( UInt64 );
			} );
		}

		return transferred;
	}
	*/

	public static async PooledValueTask<Boolean> Transfer( this CoinWallet source, CoinWallet target, (ICoin coin, UInt64 quantity) denominationAndAmount ) {
		if ( source is null ) {
			throw new ArgumentEmptyException( nameof( source ) );
		}

		if ( target is null ) {
			throw new ArgumentEmptyException( nameof( target ) );
		}

		return source.TryWithdraw( denominationAndAmount.coin, denominationAndAmount.quantity ) &&
			   ( await target.Deposit( denominationAndAmount.coin, denominationAndAmount.quantity ).ConfigureAwait( false ) > 0 );
	}

	/// <summary>Create a TPL dataflow task for depositing large volumes of money into this wallet.</summary>
	/// <param name="coinWallet"></param>
	/// <param name="sourceAmounts"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static async PooledValueTask Transfer( CoinWallet coinWallet, IEnumerable<(ICoin coin, UInt64 quantity)>? sourceAmounts ) {
		if ( coinWallet is null ) {
			throw new ArgumentEmptyException( nameof( coinWallet ) );
		}

		//TODO Incomplete. sourceAmounts
		var actionBlock = new ActionBlock<(ICoin coin, UInt64 quantity)>( async pair => await coinWallet.Deposit( pair.coin, pair.quantity ).ConfigureAwait( false ),
			Blocks.ManyProducers.ConsumeSensible( default( CancellationToken? ) ) );

		//BUG Incomplete. sourceAmounts

		actionBlock.Complete();

		await actionBlock.Completion.ConfigureAwait( false );
	}

	/// <summary>
	///     Given the <paramref name="amount" />, return the unoptimal amount of <see cref="ICoin" /> (
	///     <see
	///         cref="CoinWallet.Total" />
	///     ) it would take to <see cref="CoinWallet.Total" /> the <paramref name="amount" />.
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="leftOverAmount">Fractions of coin not accounted for.</param>
	public static IAsyncEnumerable<(ICoin coin, UInt64 quantity)> UnOptimal( this Decimal amount, ref Decimal leftOverAmount ) {
		var left = new List<ICoin>( PossibleCoins.Value );
		var result = left.ToDictionary<ICoin, ICoin, UInt64>( denomination => denomination, _ => 0 );

		leftOverAmount += amount;

		while ( leftOverAmount.Any() && left.Any() ) {
			var coin = left.OrderBy( denomination => denomination.FaceValue ).First();

			var chunks = ( UInt64 )( leftOverAmount / coin.FaceValue );

			if ( chunks > Decimal.Zero ) {
				result[coin] += chunks;
				leftOverAmount -= chunks * coin.FaceValue;
			}

			left.Remove( coin );
		}

		return result.Select( pair => (pair.Key, pair.Value) ).ToAsyncEnumerable();
	}
}