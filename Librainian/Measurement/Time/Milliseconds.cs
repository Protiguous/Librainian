// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Milliseconds.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Milliseconds.cs" was last formatted by Protiguous on 2018/07/13 at 1:29 AM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Numerics;
	using Parsing;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	[Immutable]
	public struct Milliseconds : IComparable<Milliseconds>, IQuantityOfTime {

		/// <summary>
		///     1000
		/// </summary>
		public const UInt16 InOneSecond = 1000;

		/// <summary>
		///     Ten <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds Fifteen = new Milliseconds( 15 );

		/// <summary>
		///     Five <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds Five = new Milliseconds( 5 );

		/// <summary>
		///     Five Hundred <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds FiveHundred = new Milliseconds( 500 );

		/// <summary>
		///     111. 1 Hertz (9 <see cref="Milliseconds" />).
		/// </summary>
		public static readonly Milliseconds Hertz111 = new Milliseconds( 9 );

		/// <summary>
		///     97 <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds NinetySeven = new Milliseconds( 97 );

		/// <summary>
		///     One <see cref="Milliseconds" />.
		/// </summary>
		public static readonly Milliseconds One = new Milliseconds( 1 );

		/// <summary>
		///     One <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds OneHundred = new Milliseconds( 100 );

		/// <summary>
		///     One Thousand Nine <see cref="Milliseconds" /> (Prime).
		/// </summary>
		public static readonly Milliseconds OneThousandNine = new Milliseconds( 1009 );

		/// <summary>
		///     Sixteen <see cref="Milliseconds" />.
		/// </summary>
		public static readonly Milliseconds Sixteen = new Milliseconds( 16 );

		/// <summary>
		///     Ten <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds Ten = new Milliseconds( 10 );

		/// <summary>
		///     Three <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds Three = new Milliseconds( 3 );

		/// <summary>
		///     Two <see cref="Milliseconds" /> s.
		/// </summary>
		public static readonly Milliseconds Two = new Milliseconds( 2 );

		/// <summary>
		///     Two Hundred <see cref="Milliseconds" />.
		/// </summary>
		public static readonly Milliseconds TwoHundred = new Milliseconds( 200 );

		/// <summary>
		///     Two Hundred Eleven <see cref="Milliseconds" /> (Prime).
		/// </summary>
		public static readonly Milliseconds TwoHundredEleven = new Milliseconds( 211 );

		/// <summary>
		///     Two Thousand Three <see cref="Milliseconds" /> (Prime).
		/// </summary>
		public static readonly Milliseconds TwoThousandThree = new Milliseconds( 2003 );

		//faster WPM than a female (~240wpm)
		/// <summary>
		///     Zero <see cref="Milliseconds" />.
		/// </summary>
		public static readonly Milliseconds Zero = new Milliseconds( 0 );

		/// <summary>
		///     Three Three Three <see cref="Milliseconds" />.
		/// </summary>
		public static Milliseconds ThreeHundredThirtyThree { get; } = new Milliseconds( 333 );

		[JsonProperty]
		public BigRational Value { get; }

		//faster WPM than a female (~240wpm)
		public Milliseconds( Decimal value ) => this.Value = value;

		public Milliseconds( BigRational value ) => this.Value = value;

		public Milliseconds( Int64 value ) => this.Value = value;

		public Milliseconds( BigInteger value ) => this.Value = value;

		public Milliseconds( Double value ) => this.Value = value;

		public static Milliseconds Combine( Milliseconds left, BigRational milliseconds ) => new Milliseconds( left.Value + milliseconds );

		public static Milliseconds Combine( Milliseconds left, BigInteger milliseconds ) => new Milliseconds( ( BigInteger ) left.Value + milliseconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Milliseconds left, Milliseconds right ) => left.Value == right.Value;

		public static explicit operator Double( Milliseconds milliseconds ) => ( Double ) milliseconds.Value;

		public static implicit operator BigRational( Milliseconds milliseconds ) => milliseconds.Value;

		/// <summary>
		///     Implicitly convert the number of <paramref name="milliseconds" /> to <see cref="Microseconds" />.
		/// </summary>
		/// <param name="milliseconds"></param>
		/// <returns></returns>
		public static implicit operator Microseconds( Milliseconds milliseconds ) => milliseconds.ToMicroseconds();

		public static implicit operator Seconds( Milliseconds milliseconds ) => milliseconds.ToSeconds();

		[NotNull]
		public static implicit operator SpanOfTime( Milliseconds milliseconds ) => new SpanOfTime( milliseconds: milliseconds );

		public static implicit operator TimeSpan( Milliseconds milliseconds ) => TimeSpan.FromMilliseconds( ( Double ) milliseconds.Value );

		public static Milliseconds operator -( Milliseconds milliseconds ) => new Milliseconds( milliseconds.Value * -1 );

		public static Milliseconds operator -( Milliseconds left, Milliseconds right ) => Combine( left, -right.Value );

		public static Milliseconds operator -( Milliseconds left, Decimal milliseconds ) => Combine( left, -milliseconds );

		public static Boolean operator !=( Milliseconds left, Milliseconds right ) => !Equals( left, right );

		public static Milliseconds operator +( Milliseconds left, Milliseconds right ) => Combine( left, right.Value );

		public static Milliseconds operator +( Milliseconds left, Decimal milliseconds ) => Combine( left, milliseconds );

		public static Milliseconds operator +( Milliseconds left, BigInteger milliseconds ) => Combine( left, milliseconds );

		public static Boolean operator <( Milliseconds left, Milliseconds right ) => left.Value < right.Value;

		public static Boolean operator <( Milliseconds left, Seconds right ) => ( Seconds ) left < right;

		public static Boolean operator ==( Milliseconds left, Milliseconds right ) => Equals( left, right );

		public static Boolean operator >( Milliseconds left, Milliseconds right ) => left.Value > right.Value;

		public static Boolean operator >( Milliseconds left, Seconds right ) => ( Seconds ) left > right;

		public Int32 CompareTo( Milliseconds other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Milliseconds other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) {
			if ( obj is null ) { return false; }

			return obj is Milliseconds milliseconds && this.Equals( milliseconds );
		}

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		public Microseconds ToMicroseconds() => new Microseconds( this.Value * Microseconds.InOneMillisecond );

		[System.Diagnostics.Contracts.Pure]
		public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneMillisecond * this.Value );

		[System.Diagnostics.Contracts.Pure]
		public Seconds ToSeconds() => new Seconds( this.Value / InOneSecond );

		[System.Diagnostics.Contracts.Pure]
		public override String ToString() {
			if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
				var whole = this.Value.GetWholePart();

				return $"{whole} {whole.PluralOf( "millisecond" )}";
			}

			var dec = ( Decimal ) this.Value;

			return $"{dec} {dec.PluralOf( "millisecond" )}";
		}
	}
}