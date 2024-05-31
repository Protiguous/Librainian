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
// File "AutoNumber.cs" last formatted on 2022-03-04 at 7:11 AM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;

/// <summary>A threadsafe automatically incrementing Identity class. ( <see cref="Identity" /> is <see cref="Int64" /> )</summary>
[JsonObject( MemberSerialization.Fields )]
public sealed class AutoNumber {

	[JsonProperty( "id" )]
	private Int64 _identity;

	/// <summary>Initialize the Identity with the specified seed value.</summary>
	/// <param name="seed"></param>
	public AutoNumber( Int64 seed = Int64.MinValue ) => this.Reseed( seed );

	/// <summary>The current value of the AutoNumber</summary>
	public Int64 Identity => Interlocked.Read( ref this._identity );

	public void Ensure( Int64 atLeast ) {
		if ( this.Identity < atLeast ) {
			this.Reseed( atLeast );
		}
	}

	/// <summary>Returns the next Identity</summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public Int64 Next() => Interlocked.Increment( ref this._identity );

	/// <summary>Resets the Identity to the specified seed value</summary>
	/// <param name="newIdentity"></param>
	public void Reseed( Int64 newIdentity ) => Interlocked.Exchange( ref this._identity, newIdentity );

	public override String ToString() => $"{this.Identity}";
}