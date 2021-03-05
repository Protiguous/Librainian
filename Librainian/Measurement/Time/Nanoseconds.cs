// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Nanoseconds.cs" last formatted on 2020-08-14 at 8:38 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using Maths;
	using Newtonsoft.Json;
	using Parsing;
	using Rationals;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	[Immutable]
	public struct Nanoseconds : IComparable<Nanoseconds>, IQuantityOfTime {

		/// <summary>1000</summary>
		public const UInt16 InOneMicrosecond = 1000;

		/// <summary>Ten <see cref="Nanoseconds" /> s.</summary>
		public static Nanoseconds Fifteen = new( 15 );

		/// <summary>Five <see cref="Nanoseconds" /> s.</summary>
		public static Nanoseconds Five = new( 5 );

		/// <summary>Five Hundred <see cref="Nanoseconds" /> s.</summary>
		public static Nanoseconds FiveHundred = new( 500 );

		/// <summary>One <see cref="Nanoseconds" />.</summary>
		public static Nanoseconds One = new( 1 );

		/// <summary>One Thousand Nine <see cref="Nanoseconds" /> (Prime).</summary>
		public static Nanoseconds OneThousandNine = new( 1009 );

		/// <summary>Sixteen <see cref="Nanoseconds" />.</summary>
		public static Nanoseconds Sixteen = new( 16 );

		/// <summary>Ten <see cref="Nanoseconds" /> s.</summary>
		public static Nanoseconds Ten = new( 10 );

		/// <summary>Three <see cref="Nanoseconds" /> s.</summary>
		public static Nanoseconds Three = new( 3 );

		/// <summary>Three Three Three <see cref="Nanoseconds" />.</summary>
		public static Nanoseconds ThreeHundredThirtyThree = new( 333 );

		/// <summary>Two <see cref="Nanoseconds" /> s.</summary>
		public static Nanoseconds Two = new( 2 );

		/// <summary>Two Hundred <see cref="Nanoseconds" />.</summary>
		public static Nanoseconds TwoHundred = new( 200 );

		/// <summary>Two Hundred Eleven <see cref="Nanoseconds" /> (Prime).</summary>
		public static Nanoseconds TwoHundredEleven = new( 211 );

		/// <summary>Two Thousand Three <see cref="Nanoseconds" /> (Prime).</summary>
		public static Nanoseconds TwoThousandThree = new( 2003 );

		/// <summary>Zero <see cref="Nanoseconds" />.</summary>
		public static Nanoseconds Zero = new( 0 );

		[JsonProperty]
		public Rational Value { get; }

		public Nanoseconds( Decimal value ) => this.Value = ( Rational )value;

		public Nanoseconds( Rational value ) => this.Value = value;

		public Nanoseconds( Int64 value ) => this.Value = value;

		public Nanoseconds( BigInteger value ) => this.Value = value;

		public static Nanoseconds Combine( Nanoseconds left, Nanoseconds right ) => Combine( left, right.Value );

		public static Nanoseconds Combine( Nanoseconds left, Rational nanoseconds ) => new( left.Value + nanoseconds );

		public static Nanoseconds Combine( Nanoseconds left, BigInteger nanoseconds ) => new( left.Value + nanoseconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Nanoseconds left, Nanoseconds right ) => left.Value == right.Value;

		public static implicit operator Microseconds( Nanoseconds nanoseconds ) => nanoseconds.ToMicroseconds();

		public static implicit operator Picoseconds( Nanoseconds nanoseconds ) => nanoseconds.ToPicoseconds();

		public static implicit operator SpanOfTime( Nanoseconds nanoseconds ) => new( nanoseconds: nanoseconds );

		public static Nanoseconds operator -( Nanoseconds nanoseconds ) => new( nanoseconds.Value * -1 );

		public static Nanoseconds operator -( Nanoseconds left, Nanoseconds right ) => Combine( left, -right );

		public static Nanoseconds operator -( Nanoseconds left, Decimal nanoseconds ) => Combine( left, ( Rational )( -nanoseconds ) );

		public static Boolean operator !=( Nanoseconds left, Nanoseconds right ) => !Equals( left, right );

		public static Nanoseconds operator +( Nanoseconds left, Nanoseconds right ) => Combine( left, right );

		public static Nanoseconds operator +( Nanoseconds left, Decimal nanoseconds ) => Combine( left, ( Rational )nanoseconds );

		public static Nanoseconds operator +( Nanoseconds left, BigInteger nanoseconds ) => Combine( left, nanoseconds );

		public static Boolean operator <( Nanoseconds left, Nanoseconds right ) => left.Value < right.Value;

		public static Boolean operator <( Nanoseconds left, Microseconds right ) => ( Microseconds )left < right;

		public static Boolean operator ==( Nanoseconds left, Nanoseconds right ) => Equals( left, right );

		public static Boolean operator >( Nanoseconds left, Nanoseconds right ) => left.Value > right.Value;

		public static Boolean operator >( Nanoseconds left, Microseconds right ) => ( Microseconds )left > right;

		public Int32 CompareTo( Nanoseconds other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Nanoseconds other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) {
			if ( obj is null ) {
				return default;
			}

			return obj is Nanoseconds nanoseconds && this.Equals( nanoseconds );
		}

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		public Microseconds ToMicroseconds() => new( this.Value / InOneMicrosecond );

		public Picoseconds ToPicoseconds() => new( this.Value * Picoseconds.InOneNanosecond );

		public PlanckTimes ToPlanckTimes() => new( ( Rational )PlanckTimes.InOneNanosecond * this.Value );

		public Seconds ToSeconds() => throw new NotImplementedException();

		public override String ToString() {
			if ( this.Value > MathConstants.MaxiumDecimalValue ) {
				var whole = this.Value.WholePart;

				return $"{whole} {whole.PluralOf( "ns" )}";
			}

			var dec = ( Decimal )this.Value;

			return $"{dec} {dec.PluralOf( "ns" )}";
		}

		public TimeSpan? ToTimeSpan() => this.ToSeconds();

	}

}