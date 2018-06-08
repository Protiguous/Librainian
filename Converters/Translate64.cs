// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Translate64.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Translate64.cs" was last formatted by Protiguous on 2018/06/04 at 3:49 PM.

namespace Librainian.Converters {

	using System;
	using System.Runtime.InteropServices;

#pragma warning disable IDE0015 // Use framework type
#pragma warning disable IDE0015 // Use framework type
#pragma warning disable IDE0015 // Use framework type
#pragma warning disable IDE0015 // Use framework type
	/// <summary>
	///     Struct for combining two <see cref="int" /> (or <see cref="uint" />) to and from a <see cref="ulong" /> (or
	///     <see cref="long" />) as easily as possible. Fast? No idea yet..
	/// </summary>
	[StructLayout( layoutKind: LayoutKind.Explicit )]
#pragma warning restore IDE0015 // Use framework type
#pragma warning restore IDE0015 // Use framework type
#pragma warning restore IDE0015 // Use framework type
#pragma warning restore IDE0015 // Use framework type
	public struct Translate64 {

		[FieldOffset( offset: 0 )]
		public UInt64 UnsignedValue;

		[FieldOffset( offset: 0 )]
		public Int64 SignedValue;

		[FieldOffset( offset: 0 )]
		public Int32 SignedLow;

		[FieldOffset( offset: 0 )]
		public readonly UInt32 UnsignedLow;

		[FieldOffset( offset: sizeof( UInt32 ) )]
		public readonly UInt32 UnsignedHigh;

		[FieldOffset( offset: sizeof( Int32 ) )]
		public Int32 SignedHigh;

		public Translate64( Int32 signedHigh, Int32 signedLow ) {
			this.UnsignedValue = default;
			this.SignedValue = default;
			this.UnsignedLow = default;
			this.UnsignedHigh = default;
			this.SignedLow = signedLow;
			this.SignedHigh = signedHigh;
		}

		public Translate64( UInt64 unsignedValue ) {
			this.SignedValue = default;
			this.SignedHigh = default;
			this.SignedLow = default;
			this.UnsignedLow = default;
			this.UnsignedHigh = default;
			this.UnsignedValue = unsignedValue;
		}

		public Translate64( Int64 signedValue ) {
			this.UnsignedValue = default;
			this.UnsignedLow = default;
			this.UnsignedHigh = default;
			this.SignedLow = default;
			this.SignedHigh = default;
			this.SignedValue = signedValue;
		}

		public Translate64( UInt32 unsignedLow, UInt32 unsignedHigh ) {
			this.UnsignedValue = default;
			this.SignedValue = default;
			this.SignedLow = default;
			this.SignedHigh = default;
			this.UnsignedLow = unsignedLow;
			this.UnsignedHigh = unsignedHigh;
		}

	}

}