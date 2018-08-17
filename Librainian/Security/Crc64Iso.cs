// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Crc64Iso.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Crc64Iso.cs" was last formatted by Protiguous on 2018/07/13 at 1:37 AM.

namespace Librainian.Security {

	using System;
	using JetBrains.Annotations;

	/// <summary>
	///     <see cref="Crc64" />
	/// </summary>
	/// <copyright>Damien Guard. All rights reserved.</copyright>
	/// <see cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
	public class Crc64Iso : Crc64 {

		public const UInt64 Iso3309Polynomial = 0xD800000000000000;

		internal static UInt64[] Table;

		public Crc64Iso() : base( polynomial: Iso3309Polynomial ) { }

		public Crc64Iso( UInt64 seed ) : base( polynomial: Iso3309Polynomial, seed: seed ) { }

		public static UInt64 Compute( Byte[] buffer ) => Compute( seed: DefaultSeed, buffer: buffer );

		public static UInt64 Compute( UInt64 seed, [NotNull] Byte[] buffer ) {
			if ( Table is null ) { Table = CreateTable( polynomial: Iso3309Polynomial ); }

			return CalculateHash( seed: seed, table: Table, buffer: buffer, start: 0, size: buffer.Length );
		}
	}
}