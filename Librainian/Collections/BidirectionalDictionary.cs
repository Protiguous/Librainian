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
// File "BidirectionalDictionary.cs" last formatted on 2022-12-22 at 6:57 AM by Protiguous.


namespace Librainian.Collections;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

/// <summary>
///     This just simply keeps two dictionaries synced.
/// </summary>
public abstract record BidirectionalDictionary<TFirst, TSecond> where TFirst : notnull where TSecond : notnull {
	protected BidirectionalDictionary( IDictionary<TFirst, TSecond> firstToSecondDictionary ) =>
		this.AddRange( firstToSecondDictionary.Select( pair => (pair.Key, pair.Value) ).ToArray() );

	protected BidirectionalDictionary( params (TFirst one, TSecond two)[] values ) => this.AddRange( values );

	private ConcurrentDictionary<TFirst, TSecond> FirstToSecond { get; } = new();

	private ConcurrentDictionary<TSecond, TFirst> SecondToFirst { get; } = new();

	public void AddRange( params (TFirst one, TSecond two)[] values ) => Parallel.ForEach( values.AsParallel().AsUnordered(), this.Link );

	public void Link( TFirst first, TSecond second ) {
		Parallel.Invoke( () => this.FirstToSecond[ first ] = second, () => this.SecondToFirst[ second ] = first );

		Debug.Assert( this.AreLinked( first, second ) );
	}

	public void Link( (TFirst one, TSecond two) values ) => this.Link( values.one, values.two );

	[Pure]
	[NeedsTesting]
	public Boolean AreLinked( (TFirst first, TSecond second) values ) => this.AreLinked( values.first, values.second );

	[Pure]
	[NeedsTesting]
	public Boolean AreLinked( KeyValuePair<TFirst, TSecond> values ) => this.AreLinked( values.Key, values.Value );

	[Pure]
	[NeedsTesting]
	public Boolean AreLinked( TFirst first, TSecond second ) {
		var infirst = this.FirstToSecond.TryGetValue( first, out var inOne );
		var insecond = this.SecondToFirst.TryGetValue( second, out var inTwo );
		return infirst && insecond && inOne is { } && inTwo is { } && inOne.Equals( second ) && inTwo.Equals( first );
	}

	public Boolean RemoveLink( TFirst first, TSecond second ) =>
		this.AreLinked( first, second ) && this.FirstToSecond.TryRemove( first, out var inFirst ) && inFirst.Equals( second ) &&
		this.SecondToFirst.TryRemove( second, out var inSecond ) && inSecond.Equals( first );

	public void Link( KeyValuePair<TFirst, TSecond> pair ) => this.Link( pair.Key, pair.Value );

	[Pure]
	public Boolean ExistsInSecond( TSecond value ) => this.SecondToFirst.ContainsKey( value );

	[Pure]
	public Boolean ExistsInFirst( TFirst value ) => this.FirstToSecond.ContainsKey( value );

	[Pure]
	public TSecond? GetSecond( TFirst value ) => this.FirstToSecond.TryGetValue( value, out var second ) ? second : default;

	[Pure]
	public TFirst? GetFirst( TSecond value ) => this.SecondToFirst.TryGetValue( value, out var first ) ? first : default;
}